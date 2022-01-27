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
using System.Data;
using Reportman.Drawing;
using System.Collections;

namespace Reportman.Reporting
{
	public class Param : ReportItem,ICloneable
	{
		private Variant FValue;
		public Variant Value
		{
			get
			{
                if (ParamType == ParamType.Multiple)
                    return GetMultiValue();
                else

                    return FValue;
			}
			set
			{
				FValue=value;

			}
		}
		public ParamType ParamType;
		public string Alias;
		public string Descriptions;
		public string Description
		{
				get
				{
					return Strings.GetStringByIndex(Descriptions,Report.Language);	
				}
				set
				{
					Descriptions=Strings.SetStringByIndex(Descriptions,value, Report.Language);
				}
		}
		public string Hints;
        protected override string GetClassName()
        {
            return "TRPPARAM";
        }
        public bool UserVisible
        {
            get { return (Visible && (!NeverVisible)); }
        }
		public string Hint
		{
			get
			{
				return Strings.GetStringByIndex(Hints, Report.Language);
			}
			set
			{
				Hints = Strings.SetStringByIndex(Hints,value, Report.Language);
			}
		}
		public string ErrorMessages;
		public string ErrorMessage
		{
			get
			{
                if (ErrorMessages.Length > Report.Language)
                    return Strings.GetStringByIndex(ErrorMessages, Report.Language);
                else
                {
                    if (ErrorMessages.Length > 0)
                        return Strings.GetStringByIndex(ErrorMessages, 0);
                    else
                        return "";
                }
			}
			set
			{
				ErrorMessages = Strings.SetStringByIndex(ErrorMessages,value, Report.Language);
			}
		}
		public string Validation;
		public string LookupDataset, SearchDataset, Search, SearchParam;
		public Strings Items;
		public Strings Values;
		public Strings Selected;
		public Strings Datasets;
        private Variant FLastValue;
		public Variant LastValue
        {
            get
            {
                return FLastValue;
            }
            set
            {

                FLastValue = value;
            }

        }
		public bool Visible, IsReadOnly, NeverVisible, AllowNulls;
		public Param(BaseReport rp)
			: base(rp)
		{
			Items = new Strings();
			Values = new Strings();
			Selected = new Strings();
			Datasets = new Strings();
			Descriptions = "";
			Hints = "";
			ErrorMessages = "";
            Validation = "";
			Alias = "";
			FValue = new Variant();
			LookupDataset = ""; SearchDataset = ""; Search = ""; SearchParam = "";
		}
		public DbType GetDbType()
		{
			DbType aresult = DbType.Object;
			switch (ParamType)
			{
				case ParamType.Bool:
					aresult = DbType.Boolean;
					break;
				case ParamType.Currency:
					aresult = DbType.Currency;
					break;
				case ParamType.Date:
					aresult = DbType.Date;
					break;
				case ParamType.Time:
					aresult = DbType.Time;
					break;
				case ParamType.DateTime:
					aresult = DbType.DateTime;
					break;
				case ParamType.Double:
					aresult = DbType.Double;
					break;
				case ParamType.String:
					aresult = DbType.String;
					break;
				case ParamType.ExpreA:
				case ParamType.ExpreB:
				case ParamType.List:
                case ParamType.SubsExpreList:
                case ParamType.Multiple:
					aresult = LastValue.GetDbType();
					break;
			}
			return aresult;
		}
        public string GetSubExpreValue()
        {
            if (FValue.IsInteger()) 
            {
                return Values[FValue];
            }
            else
                return FValue;
        }
		public string GetMultiValue()
		{
			int i;

			string aresult = "";
            if (ParamType != ParamType.Multiple)
                return aresult;
			for (i = 0; i < Selected.Count; i++)
			{
/*				astring = Selected[i];
				aindex = System.Convert.ToInt32(astring);
				if (Values.Count > aindex)
				{
					if (aresult.Length > 0)
						aresult = aresult + "," + Values[aindex];
					else
						aresult = Values[aindex];
				}
*/
                if (aresult.Length > 0)
                 aresult = aresult + "," + Selected[i];
                else
                 aresult = Selected[i];
			}
			return aresult;
		}

		public Variant ListValue
		{
			get
			{
				Variant aresult = new Variant();
				string aexpression;
				int aoption;
				if (!((ParamType == ParamType.List) ||
                    (ParamType == ParamType.Multiple) || (ParamType == ParamType.SubsExpreList)))
					aresult = Value;
				else
				{
					if (ParamType == ParamType.Multiple)
						aresult = GetMultiValue();
					else
					{
						aoption = 0;
						if (Value.IsInteger())
						{
							aoption = Value;
							if (aoption < 0)
								aoption = 0;
						}
						else
						{
							if (Value.IsString())
							{
								aoption = Values.IndexOf(Value);
								if (aoption < 0)
									aoption = 0;
							}
						}
						if (aoption >= Values.Count)
						{
							aresult = Value;
						}
						else
						{
							aexpression = Values[aoption];
							aresult = Report.Evaluator.EvaluateText(aexpression);
						}
					}
				}
				return aresult;
			}
		}
		public object Clone()
		{
			Param p=new Param(Report);
			p.AllowNulls=AllowNulls;
			p.Alias=Alias;
			p.Datasets=(Strings)Datasets.Clone();
			p.Descriptions=Descriptions;
			p.ErrorMessage=ErrorMessage;
			p.FValue=FValue;
			p.Hint=Hint;
			p.Hints=Hints;
			p.IsReadOnly=IsReadOnly;
			p.Items=(Strings)Items.Clone();
			p.LastValue=LastValue;
			p.LookupDataset=LookupDataset;
			p.Name=Name;
			p.NeverVisible=NeverVisible;
			p.ParamType=ParamType;
			p.Search=Search;
			p.SearchDataset=SearchDataset;
			p.SearchParam=SearchParam;
			p.Selected=(Strings)Selected.Clone();
			p.Validation=Validation;
			p.Values=(Strings)Values.Clone();
			p.Visible=Visible;
			return p;
		}
        public void SelectAllValues()
        {
            Selected.Clear();
            foreach (string s in Values)
                Selected.Add(s);
        }
        public void UpdateLookupValues()
        {
            if (LookupDataset.Length > 0)
            {
                Values.Clear();
                Items.Clear();
                DataInfo dinfo = Report.DataInfo[LookupDataset];
                dinfo.DisConnect();
                dinfo.Connect();
                try
                {
                    int indexvalue = 0;
                    if (dinfo.Data.Columns.Count > 1)
                        indexvalue = 1;
                    while (!dinfo.Data.Eof)
                    {
                        Items.Add(dinfo.Data.CurrentRow[0].ToString());
                        Values.Add(dinfo.Data.CurrentRow[indexvalue].ToString());
                        dinfo.Data.Next();
                    }
                }
                finally
                {
                    dinfo.DisConnect();
                }
            }
        }
	}
}
