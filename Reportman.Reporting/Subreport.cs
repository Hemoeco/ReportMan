#region Copyright
/*
 *  Report Manager:  Database Reporting tool for .Net and Mono
 *
 *     The contents of this file are subject to the MPL License
 *     with optional use of GPL or LGPL licenses.
 *     You may not use this file except in compliance with the
 *     Licenses. You may obtain copies of the Licenses at:
 *     http://reportman.sourceforge.net/license
 *
 *     Software is distributed on an "AS IS" basis,
 *     WITHOUT WARRANTY OF ANY KIND, either
 *     express or implied.  See the License for the specific
 *     language governing rights and limitations.
 *
 *  Copyright (c) 1994 - 2008 Toni Martir (toni@reportman.es)
 *  All Rights Reserved.
*/
#endregion

using System;
using System.IO;
using System.Collections;
using Reportman.Drawing;
#if REPMAN_DOTNET2
using System.Collections.Generic;
#endif

namespace Reportman.Reporting
{
	public enum SubReportEvent
	{
		Start, DataChange, GroupChange, PageChange,
		InvalidateValue, SubReportStart, SubReportEnd
	};
	public class SubReport : ReportItem
	{
		public bool LastRecord;
		public int CurrentGroupIndex;
		public Sections Sections;
		public string Alias;
		public SubReport ParentSubReport;
		public Section ParentSection;
		public bool PrintOnlyIfDataAvailable;
		public bool ReOpenOnPrint;
		public string ParentSub;
		public string ParentSec;
		public string CurrentGroupName;
		public SubReport(BaseReport rp)
			: base(rp)
		{
			PrintOnlyIfDataAvailable = true;
			ReOpenOnPrint = true;
			Sections = new Sections();
			Alias = "";
			ParentSub = "";
			ParentSec = "";
		}
		public void SubReportChanged(SubReportEvent aevent, string newgroup)
		{
			int i, j;
			Section sec;
			PrintItem compo;
			int index;
			int ffirstdetail, flastdetail;
			SubReportEvent newstate = aevent;

			// Updates group values
			if ((newstate == SubReportEvent.Start)
				|| (newstate == SubReportEvent.SubReportStart))
			{
				FillGroupValues();
				for (i = 0; i < Sections.Count; i++)
				{
					sec = Sections[i];
					sec.ClearPageCountList();
				}
			}
			else
			{
				index = -1;
				if (newstate == SubReportEvent.GroupChange)
				{
					index = GroupIndex(newgroup);
				}
				else
					if (newstate == SubReportEvent.SubReportEnd)
					{
						index = GroupCount;
					}
				if (index > 0)
				{
					ffirstdetail = FirstDetail;
					flastdetail = LastDetail;
					while (index > 0)
					{
						Sections[ffirstdetail - index].UpdatePageCounts();
						Sections[ffirstdetail - index].ClearPageCountList();
						Sections[flastdetail + index].UpdatePageCounts();
						Sections[ffirstdetail + index].ClearPageCountList();
						index--;
					}
				}
			}
			for (i = 0; i < Sections.Count; i++)
			{
				sec = Sections[i];
				sec.SubReportChanged(newstate, newgroup);
				for (j = 0; j < sec.Components.Count; j++)
				{
					compo = sec.Components[j];
					compo.SubReportChanged(newstate, newgroup);
				}

			}
		}
		public int FirstPageHeader
		{
			get
			{
				return FirstSectionThatIs(SectionType.PageHeader);
			}
		}
        public int LastPageHeader
        {
            get
            {
                return LastSectionThatIs(SectionType.PageHeader);
            }
        }
        public int NumberOfSectionsOfType(SectionType atype)
		{
			int index = FirstSectionThatIs(atype);
			int count = 0;
			if (index >= 0)
			{
				while (index < Sections.Count)
				{
					if (Sections[index].SectionType == atype)
						count++;
					index++;
				}
			}
			return count;
		}
		public int GroupIndex(string groupname)
		{
			int index;
			int aresult = -1;
			index = FirstSectionThatIs(SectionType.GroupHeader);
			if (index >= 0)
			{
				while (index < Sections.Count)
				{
					if (Sections[index].SectionType != SectionType.GroupHeader)
						break;
					if (Sections[index].GroupName == groupname)
					{
						aresult = FirstDetail - index;
						break;

					}
					index++;
				}
			}
			return aresult;
		}
		public int PageHeaderCount
		{
			get
			{
				return NumberOfSectionsOfType(SectionType.PageHeader);
			}
		}
		public int PageFooterCount
		{
			get
			{
				return NumberOfSectionsOfType(SectionType.PageFooter);
			}
		}
		public int FirstPageFooter
		{
			get
			{
				return FirstSectionThatIs(SectionType.PageFooter);
			}
		}
        public int LastPageFooter
        {
            get
            {
                return LastSectionThatIs(SectionType.PageFooter);
            }
        }
        public int FirstDetail
		{
			get
			{
				return FirstSectionThatIs(SectionType.Detail);
			}
		}
        public int DetailCount
        {
            get
            {
                if (Sections.Count == 0)
                    return 0;
                else
                    return (LastSectionThatIs(SectionType.Detail)-FirstSectionThatIs(SectionType.Detail))+1;
            }
        }
        public int LastDetail
		{
			get
			{
				return LastSectionThatIs(SectionType.Detail);
			}
		}
		public int GroupCount
		{
			get
			{
				return NumberOfSectionsOfType(SectionType.GroupHeader);
			}
		}
		public int FirstSectionThatIs(SectionType atype)
		{
			int i;
			int aresult = -1;
			i = 0;
			while (i < Sections.Count)
			{
				if (Sections[i].SectionType == atype)
				{
					aresult = i;
					break;
				}
				i++;
			}
			return aresult;
		}
		public int LastSectionThatIs(SectionType atype)
		{
			int i;
			int aresult = -1;
			i = 0;
			while (i < Sections.Count)
			{
				if (Sections[i].SectionType == atype)
				{
					aresult = i;
				}
				i++;
			}
			return aresult;
		}
		public void InitGroups(int groupindex)
		{
			int i, afirstdetail;
			afirstdetail = FirstDetail;
			i = afirstdetail - groupindex;
			while (i < afirstdetail)
			{
				SubReportChanged(SubReportEvent.GroupChange, Sections[i].GroupName);
				i++;
			}
		}
		public bool IsDataAvailable()
		{
			if (Alias.Length == 0)
				return true;
			if (!PrintOnlyIfDataAvailable)
				return true;

			bool aresult = false;
			int index = Report.DataInfo.IndexOf(Alias);
			if (index < 0)
				throw new UnNamedException("Data alias not found" + Alias);
			DataInfo dinfo = Report.DataInfo[index];
			if (dinfo.Data.Active)
			{
				if (!dinfo.Data.Eof)
					aresult = true;
			}
			return aresult;
		}
		public int GroupChanged()
		{
			int i, afirstdetail, agroupcount;
			Section sec;
			Evaluator eval;
			int acount;
			int aresult = 0;
			// Checks for group changes
			agroupcount = GroupCount;
			acount = agroupcount;
			afirstdetail = FirstDetail;
			i = afirstdetail - agroupcount;
			eval = Report.Evaluator;
			while (i < afirstdetail)
			{
				sec = Sections[i];
				eval.Expression = sec.ChangeExpression;

                eval.Evaluate();

				if (sec.ChangeBool)
				{
					if ((bool)eval.Result)
					{
						aresult = acount;
						FillGroupValues();
						break;
					}
				}
				else
				{
					if (eval.Result != sec.GroupValue)
					{
						aresult = acount;
						FillGroupValues();
						break;
					}
				}
				i++;
				acount--;
			}
			CurrentGroupIndex = aresult;
			return aresult;
		}
		void FillGroupValues()
		{
			int i, indexdetail;
			Section sec;
			Evaluator eval;
			CurrentGroupName = "";
			CurrentGroupIndex = 0;
			i = 0;
			indexdetail = FirstDetail;
			eval = Report.Evaluator;
			while (i < indexdetail)
			{
				sec = Sections[i];
				try
				{
					if (sec.SectionType == SectionType.GroupHeader)
					{
						eval.Expression = sec.ChangeExpression;
						eval.Evaluate();
						sec.GroupValue = eval.Result;
					}
				}
				catch (Exception E)
				{
					throw new ReportException(E.Message, this, "GroupChange");
				}
				i++;
			}
		}
        protected override string GetClassName()
        {
            return "TRPSUBREPORT";
        }
        /// <summary>
        /// Descriptive name to display to the user
        /// </summary>
        /// <param name="includedataset"></param>
        /// <returns></returns>
        public string GetDisplayName(bool includedataset)
        {
            string aresult=Name;

            int index = 0;
            while (index < Name.Length)
            {
                if ((Name[index] >= '0') && (Name[index] <= '9'))
                {
                    break;
                }
                index++;
            }
            if (index < Name.Length)
            {
                aresult = Translator.TranslateStr(353) + Name.Substring(index, Name.Length - index);
            }
            if (includedataset)
            {
                if (Alias.Length > 0)
                    aresult = aresult + '(' + Alias + ')';
                else
                    aresult = aresult + '(' + Translator.TranslateStr(1148) + ')';
            }
            return aresult;
        }
        private const int DEFAULT_SECTION_WITH=10770;
        private const int DEFAULT_SECTION_HEIGHT= 1113;
        /// <summary>
        /// Create a new section
        /// </summary>
        /// <returns></returns>
        private Section CreateSection()
        {
            Section sec = new Section(Report);
            Report.GenerateNewName(sec);
            sec.SubReport = this;
            sec.Width = DEFAULT_SECTION_WITH;
            sec.Height = DEFAULT_SECTION_HEIGHT;
            return sec;
        }

        /// <summary>
        /// Add a new detail to the subreport
        /// </summary>
        /// <returns></returns>
        public Section AddDetail()
        {
            Section sec = CreateSection();
            sec.SectionType = SectionType.Detail;
            if (DetailCount>0)
                Sections.Insert(FirstDetail, sec);
            else
                Sections.Add(sec);
            return sec;
        }
        /// <summary>
        /// Add a new page header to the subreport
        /// </summary>
        /// <returns></returns>
        public Section AddPageHeader()
        {
            Section sec = CreateSection();
            sec.SectionType = SectionType.PageHeader;
            Sections.Insert(0, sec);
            return sec;
        }
        /// <summary>
        /// Delete a section from the report
        /// </summary>
        /// <returns></returns>
        public void DeleteSection(Section sec)
        {
            Sections.Remove(sec);

            foreach (PrintItem pritem in  sec.Components)
            {
                pritem.Dispose();
            }
            sec.Components.Clear();
            Report.Components.Remove(sec.Name);
            sec.Dispose();
        }
        /// <summary>
        /// Add a new page footer to the subreport
        /// </summary>
        /// <returns></returns>
        public Section AddPageFooter()
        {
            Section sec = CreateSection();
            sec.SectionType = SectionType.PageFooter;
            // Page footer at the end allways
            Sections.Add(sec);
            return sec;
        }
        public int IndexOfGroup(string groupname)
        {
            int aresult=-1;
            for (int i = 0; i < Sections.Count; i++)
            {
                if (Sections[i].GroupName == groupname)
                {
                    aresult = FirstDetail - i;
                    break;
                }
            }
            return aresult;
        }
        /// <summary>
        /// Add a new group to the subreport
        /// </summary>
        /// <returns></returns>
        public Section AddGroup(string groupname)
        {
            Section sec = CreateSection();
            Section secf = sec;
            int index = FirstDetail;
            sec.SectionType = SectionType.GroupHeader;
            sec.GroupName = groupname;
            Sections.Insert(index,sec);
            sec = CreateSection();
            index = LastDetail;
            sec.SectionType = SectionType.GroupFooter;
            sec.SubReport = this;
            sec.GroupName = groupname;
            Sections.Insert(index+1, sec);
            return secf;
        }
    }

#if REPMAN_DOTNET1
	public class SubReports
	{
		SubReport[] FItems;
		const int FIRST_ALLOCATION_OBJECTS = 10;
		int FCount;
		public SubReports()
		{
			FCount = 0;
			FItems = new SubReport[FIRST_ALLOCATION_OBJECTS];
		}
		public void Clear()
		{
			for (int i = 0; i < FCount; i++)
				FItems[i] = null;
			FCount = 0;
		}
		public int IndexOf(SubReport avalue)
		{
			int aresult = -1;
			for (int i = 0; i < Count; i++)
			{
				if (FItems[i] == avalue)
				{
					aresult = i;
					break;
				}
			}
			return aresult;
		}
		public void RemoveAt(int index)
		{
			CheckRange(index);
			FItems[index] = null;
			while (index < FCount - 1)
			{
				FItems[index] = FItems[index + 1];
			}
			FCount--;
		}
		public void Remove(SubReport subrep)
		{
			RemoveAt(IndexOf(subrep));
		}
		private void CheckRange(int index)
		{
			if ((index < 0) || (index >= FCount))
				throw new UnNamedException("Index out of range on SubReport collection");
		}
		public SubReport this[int index]
		{
			get { CheckRange(index); return FItems[index]; }
			set { CheckRange(index); FItems[index] = value; }
		}
		public int Count { get { return FCount; } }
		public void Add(SubReport obj)
		{
			if (FCount > (FItems.Length - 2))
			{
				SubReport[] nobjects = new SubReport[FCount];
				System.Array.Copy(FItems, 0, nobjects, 0, FCount);
				FItems = new SubReport[FItems.Length * 2];
				System.Array.Copy(nobjects, 0, FItems, 0, FCount);
			}
			FItems[FCount] = obj;
			FCount++;
		}
		// IEnumerable Interface Implementation:
		//   Declaration of the GetEnumerator() method 
		//   required by IEnumerable
		public IEnumerator GetEnumerator()
		{
			return new SubReportsEnumerator(this);
		}
		// Inner class implements IEnumerator interface:
		public class SubReportsEnumerator : IEnumerator
		{
			private int position = -1;
			private SubReports t;

			public SubReportsEnumerator(SubReports t)
			{
				this.t = t;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < t.Count - 1)
				{
					position++;
					return true;
				}
				else
				{
					return false;
				}
			}

			// Declare the Reset method required by IEnumerator:
			public void Reset()
			{
				position = -1;
			}

			// Declare the Current property required by IEnumerator:
			public object Current
			{
				get
				{
					return t[position];
				}
			}
		}
	}
#else
	public class SubReports:System.Collections.Generic.List<SubReport>
	{

	}
#endif

}
