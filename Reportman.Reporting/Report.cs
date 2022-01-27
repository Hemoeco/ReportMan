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
 *  Copyright (c) 1994 - 2006 Toni Martir (toni@reportman.es)
 *  All Rights Reserved.
*/
#endregion

using System;
using System.IO;
using System.Drawing;
using Reportman.Drawing;
using System.Threading;

namespace Reportman.Reporting
{
	public class Report : BaseReport
	{
		public Report()
			: base()
		{
		}

		private SubReport CurrentSubReport => SubReports.Count > CurrentSubReportIndex ? SubReports[CurrentSubReportIndex] : null;

		override public void BeginPrint(PrintOut driver)
		{
			base.BeginPrint(driver);
			if (FExecuting)
			{
				FExecuting = false;
			}
			EndPrint();
			// Forward only apply to synchronous single pass report processing
			if (TwoPass || AsyncExecution)
				MetaFile.ForwardOnly = false;
			mmfirst = System.DateTime.Now;
			FDriver = driver;
            if (!FCompose)
			    MetaFile.Clear();

            MetaFile.Empty = false;
            try
            {
                MetaFile.PrinterFonts = PrinterFonts;
                UpdatePageSize = false;
                FillGlobalHeaders();
                PendingSections.Clear();
                ErrorProcessing = false;
                if (driver == null)
                    throw new UnNamedException("No Driver assigned");
                MetaFile.PrinterSelect = this.PrinterSelect;
                FDriver.SelectPrinter(PrinterSelect);
                if (FCompose)
                {
                    MetaFile.ForwardOnly = false;
                    InternalPageWidth = MetaFile.CustomX;
                    InternalPageHeight = MetaFile.CustomY;
                    PageNum = MetaFile.Pages.CurrentCount - 2;
                }
                else
                {
                    Point apagesize;
                    MetaFile.Empty = false;
                    MetaFile.PrinterFonts = PrinterFonts;
                    MetaFile.PreviewAbout = PreviewAbout;
                    MetaFile.PreviewMargins = PreviewMargins;
                    MetaFile.LinesPerInch = LinesPerInch;
                    MetaFile.PaperSource = PaperSource;
                    // Sets page orientation
                    CurrentOrientation = PageOrientation;
                    if (PageOrientation != OrientationType.Default)
                    {
                        FDriver.SetOrientation(PageOrientation);
                    }
                    PageDetail.PaperSource = PaperSource;
                    PageDetail.Duplex = Duplex;
                    PageDetail.ForcePaperName = ForcePaperName;
                    if (PageSize != PageSizeType.Default)
                    {
                        if (PageSize == PageSizeType.User)
                        {
                            MetaFile.PageSizeIndex = -1;
                            PageDetail.Index = PageSizeIndex;
                            PageDetail.Custom = true;
                            PageDetail.CustomHeight = CustomPageHeight;
                            PageDetail.CustomWidth = CustomPageWidth;
                        }
                        else
                        {
                            MetaFile.PageSizeIndex = PageSizeIndex;
                            PageDetail.Index = PageSizeIndex;
                            PageDetail.Custom = false;
                            PageDetail.CustomWidth = CustomPageWidth;
                            PageDetail.CustomHeight = CustomPageHeight;
                        }
                        apagesize = FDriver.SetPageSize(PageDetail);
                    }
                    else
                    {
                        int newpagesize = MetaFile.PageSizeIndex;
                        apagesize = FDriver.GetPageSize(out newpagesize);
                        MetaFile.PageSizeIndex = newpagesize;
                    }
                    InternalPageWidth = apagesize.X;
                    InternalPageHeight = apagesize.Y;
                    MetaFile.Orientation = PageOrientation;
                    MetaFile.BackColor = PageBackColor;
                    MetaFile.CustomX = InternalPageWidth;
                    MetaFile.CustomY = InternalPageHeight;
                    PageNum = -1;
                }
                MetaFile.PrinterSelect = PrinterSelect;
                MetaFile.AutoScale = AutoScale;
                MetaFile.PreviewWindow = PreviewWindow;
                MetaFile.OpenDrawerAfter = ActionAfter;
                MetaFile.OpenDrawerBefore = ActionBefore;
                int i;
                for (i = 0; i < SubReports.Count; i++)
                {
                    SubReports[i].LastRecord = false;
                }
                LastPage = false;
                InitEvaluator();
                PageNumGroup = -1;
                FRecordCount = 0;

                InitializeParams();

                FDataAlias.List.Clear();
                for (i = 0; i < DataInfo.Count; i++)
                {
                    AliasCollectionItem aitem = new AliasCollectionItem();
                    aitem.Alias = DataInfo[i].Alias;
                    aitem.Data = DataInfo[i].Data;
                    FDataAlias.List.Add(aitem);
                }
                try
                {
                    ActivateDatasets();
                }
                catch
                {
                    DeActivateDatasets();
                    throw;
                }
                CheckIfDataAvailable();
                Evaluator.AliasList = FDataAlias;

                PrepareParamsAfterOpen();
                for (i = 0; i < SubReports.Count; i++)
                {
                    SubReports[i].SubReportChanged(SubReportEvent.Start, "");
                }
                CurrentSubReportIndex = -1;
                SubReport subrep;
                bool dataavail = false;
                do
                {
                    CurrentSubReportIndex++;
                    if (CurrentSubReportIndex >= SubReports.Count)
                        break;
                    subrep = CurrentSubReport;
                    if (subrep.ParentSubReport == null)
                    {
                        if (subrep.Alias.Length == 0)
                            dataavail = true;
                        else
                        {
                            if (subrep.PrintOnlyIfDataAvailable)
                            {
                                int index = DataInfo.IndexOf(subrep.Alias);
                                if (!DataInfo[index].Data.Eof)
                                    dataavail = true;
                            }
                            else
                                dataavail = true;
                        }
                    }
                    if (dataavail)
                    {
                        subrep.SubReportChanged(SubReportEvent.SubReportStart, "");
                        subrep.SubReportChanged(SubReportEvent.DataChange, "");
                        CurrentSectionIndex = -1;
                        subrep.CurrentGroupIndex = -subrep.GroupCount;
                        if (subrep.CurrentGroupIndex < 0)
                        {
                            CurrentSectionIndex = subrep.FirstDetail + subrep.CurrentGroupIndex - 1;
                        }
                        section = null;
                        subreport = null;
                        if (!NextSection(true))
                            dataavail = false;
                    }
                } while (!dataavail);
                if (!dataavail)
                {
                    EndPrint();
                    AbortingThread = true;
                    MetaFile.Empty = true;
                    throw new NoDataToPrintException(Translator.TranslateStr(799));
                }
                printing = true;
            }
            catch
            {
                MetaFile.Empty=true;
                throw;
            }
		}
		bool NextSection(bool child)
		{
			SubReport subrep;
			Section sec;
			Section oldsection;
			int lastdetail;
			int firstdetail;
			bool dataavail;
			int index;
			bool SearchGroupHeader;

			SearchGroupHeader = false;
			oldsection = section;
			section = null;
			// If the old selected section has a child subreport then execute first
			if ((oldsection != null) && child)
			{
				if (oldsection.ChildSubReport != null)
				{
					dataavail = false;
					subrep = oldsection.ChildSubReport;
					if (subrep.Alias.Length == 0)
						dataavail = true;
					else
					{
						index = DataInfo.IndexOf(subrep.Alias);
						if (index < 0)
							throw new NamedException("Data alias not found:" + subrep.Alias,subrep.Alias);
						if (DataInfo[index].DataSource.Length == 0)
						{
							DataInfo[index].DisConnect();
							UpdateParamsBeforeOpen(index, true);
							DataInfo[index].Connect();
                            DataInfo[index].GoFirstMem();
                        }
						else
						{
							if (DataInfo[index].Data.Eof)
							{
								if (subrep.ReOpenOnPrint)
								{
									DataInfo[index].DisConnect();
									UpdateParamsBeforeOpen(index, true);
									DataInfo[index].Connect();
                                    DataInfo[index].GoFirstMem();
								}

							}
						}
                        if (!DataInfo[index].Data.Eof)
                            dataavail = true;
                        else
                            SearchGroupHeader = true;
					}
					subrep.LastRecord = !dataavail;
					if (dataavail || (!subrep.PrintOnlyIfDataAvailable))
					{
						subrep.SubReportChanged(SubReportEvent.SubReportStart, "");
						subreport = subrep;
						section = null;
						CurrentSectionIndex = -1;
						PendingSections.Add(oldsection);
						CurrentSubReportIndex = SubReports.IndexOf(subreport);
						subreport.SubReportChanged(SubReportEvent.DataChange, "");
						subreport.CurrentGroupIndex = -subreport.GroupCount;
						if (subreport.CurrentGroupIndex < 0)
						{
							CurrentSectionIndex = subreport.FirstDetail + subreport.CurrentGroupIndex - 1;
						}
					}
				}
				else
					SearchGroupHeader = true;
			}
			else
				SearchGroupHeader = true;
			if (FGroupHeaders.Count == 0)
				SearchGroupHeader = false;
			if (SearchGroupHeader)
			{
				index = FGroupHeaders.IndexOf(oldsection);
				if (index >= 0)
				{
					FGroupHeaders.RemoveAt(index);
					if (FGroupHeaders.Count > 0)
					{
						section = FGroupHeaders[0];
						Section asec = PendingSections[PendingSections.Count - 1];
						CurrentSubReportIndex = SubReports.IndexOf(asec.SubReport);
						if (CurrentSubReportIndex < 0)
							throw new UnNamedException("SubReport not found");
						subreport = section.SubReport;
					}
					else
					{
						section = PendingSections[PendingSections.Count - 1];
						CurrentSubReportIndex = SubReports.IndexOf(section.SubReport);
						if (CurrentSubReportIndex < 0)
							throw new UnNamedException("SubReport not found");
						PendingSections.RemoveAt(PendingSections.Count - 1);
						subreport = section.SubReport;
						CurrentSectionIndex = subreport.Sections.IndexOf(section);
						if (CurrentSectionIndex < 0)
							throw new UnNamedException("Section not found");
					}
				}
			}
			if (section != null)
				return true;
			// Check the condition
			while (CurrentSubReportIndex < SubReports.Count)
			{
				CheckProgress(false);
				subrep = CurrentSubReport;
				// The first section are the group footers until
				// CurrentGroup
				while (subrep.CurrentGroupIndex != 0)
				{
					CheckProgress(false);
					lastdetail = subrep.LastDetail;
					firstdetail = subrep.FirstDetail;
					CurrentSectionIndex++;
					if (subrep.CurrentGroupIndex > 0)
					{
						if (subrep.CurrentGroupIndex < (CurrentSectionIndex - lastdetail))
						{
							// Restore position
							// And the next will be group headers
							if (subrep.LastRecord)
							{
								CurrentSectionIndex = subrep.Sections.Count;
								subrep.CurrentGroupIndex = 0;
								break;
							}
							else
							{
								// Send Messages for each group
								subrep.InitGroups(subrep.CurrentGroupIndex);
								// Restores position
								NextRecord(true);
								CurrentSectionIndex = subrep.FirstDetail - subrep.CurrentGroupIndex;
								subrep.CurrentGroupIndex = -subrep.CurrentGroupIndex;
								sec = subrep.Sections[CurrentSectionIndex];
								if (sec.EvaluatePrintCondition())
								{
									section = sec;
									subreport = subrep;
									break;
								}
							}
						}
						else
						{
							sec = subrep.Sections[CurrentSectionIndex];
							if (sec.EvaluatePrintCondition())
							{
								section = sec;
								subreport = subrep;
								break;
							}
						}
					}
					else
					{
						// Group headers
						if (CurrentSectionIndex < firstdetail)
						{
							sec = subrep.Sections[CurrentSectionIndex];
							if (sec.EvaluatePrintCondition())
							{
								section = sec;
								subreport = subrep;
								break;
							}
						}
						else
						{
							subrep.CurrentGroupIndex = 0;
							CurrentSectionIndex = -1;
						}
					}
				}
				if (section != null)
					break;
				while (CurrentSectionIndex < subrep.Sections.Count)
				{
					CheckProgress(false);
					if (CurrentSectionIndex < 0)
						CurrentSectionIndex = subrep.FirstDetail;
					else
						CurrentSectionIndex++;
					if (!subrep.LastRecord)
					{
						if (CurrentSectionIndex > subrep.LastDetail)
						{
							if (NextRecord(false))
							{
								CurrentSectionIndex = subrep.LastDetail;
								break;
							}
							if (!subrep.LastRecord)
							{
								CurrentSectionIndex = subrep.FirstDetail;
								sec = subrep.Sections[CurrentSectionIndex];
								if (sec.EvaluatePrintCondition())
								{
									section = sec;
									subreport = subrep;
									break;
								}
							}
							else
							{
								CurrentSectionIndex = subrep.LastDetail;
								subrep.CurrentGroupIndex = subrep.GroupCount;
								break;
							}
						}
						else
						{
							if (CurrentSectionIndex <= subrep.LastDetail)
							{
								sec = subrep.Sections[CurrentSectionIndex];
								if (sec.EvaluatePrintCondition())
								{
									section = sec;
									subreport = subrep;
									break;
								}
							}
						}
					}
				}
				if ((section == null) && (subrep.CurrentGroupIndex == 0))
				{
					// If it's a child subreport
					// Returns null section so pending will print
					if (subrep.ParentSubReport != null)
						break;
					do
					{
						subrep.SubReportChanged(SubReportEvent.SubReportEnd, "");
						CurrentSubReportIndex++;
						if (CurrentSubReportIndex >= SubReports.Count)
							break;
						subrep = CurrentSubReport;
						if (subrep.ParentSubReport == null)
						{
							if (subrep.IsDataAvailable())
							{
								subrep.SubReportChanged(SubReportEvent.SubReportStart, "");
								subrep.SubReportChanged(SubReportEvent.DataChange, "");
								break;
							}
						}
					} while (true);
					if (CurrentSubReportIndex >= SubReports.Count)
						break;
					CurrentSectionIndex = subrep.FirstDetail - subrep.GroupCount - 1;
					subrep.CurrentGroupIndex = -subrep.GroupCount;
					subrep.LastRecord = false;
				}
				else
					if (subrep.CurrentGroupIndex == 0)
						break;
			}
			bool aresult = (section != null);
			// If there are still pending sections
			if (section == null)
			{
				if (PendingSections.Count > 0)
				{
					section = PendingSections[PendingSections.Count - 1];
					CurrentSubReportIndex = SubReports.IndexOf(section.SubReport);
					if (CurrentSubReportIndex < 0)
						throw new UnNamedException("Subreport not found");
					PendingSections.RemoveAt(PendingSections.Count - 1);
					subreport = section.SubReport;
					CurrentSectionIndex = subreport.Sections.IndexOf(section);
					if (CurrentSectionIndex < 0)
						throw new UnNamedException("Subreport not found");
					NextSection(false);
				}
			}
			return aresult;
		}
		void SkipToPageAndPosition()
		{
			int newpage, newposx, newposy;
			bool moveh, movev;
			newposx = 0;
			newposy = 0;
			try
			{
				// Go to page?
				if (asection.SkipToPageExpre.Length > 0)
				{
					if (!TwoPass)
						throw new UnNamedException("Two pass report required for skip to feature");
					newpage = Convert.ToInt32((Evaluator.EvaluateText(asection.SkipToPageExpre)).AsDouble);
					newpage = newpage - 1;
					if (newpage < 0)
						throw new UnNamedException("Can not combine skip to page");
					while (newpage > MetaFile.Pages.CurrentCount)
					{
						MetaFile.Pages.Add(new MetaPage(MetaFile));
						MetaFile.Pages[MetaFile.Pages.CurrentCount - 1].Orientation = CurrentOrientation;
						MetaFile.Pages[MetaFile.Pages.CurrentCount - 1].PageDetail = PageDetail;
						CheckProgress(false);
					}
					MetaFile.CurrentPage = newpage;
					PageNum = MetaFile.CurrentPage;
				}
			}
			catch (Exception E)
			{
				throw new ReportException(E.Message, asection, "SkipToPage");
			}
			moveh = false;
			try
			{
				// Go to page?
				if (asection.SkipExpreH.Length > 0)
				{
					newposx = System.Convert.ToInt32(Evaluator.EvaluateText(asection.SkipExpreH).AsDouble);
					moveh = true;
				}
			}
			catch (Exception E)
			{
				throw new ReportException(E.Message, asection, "SkipExpreH");
			}
			movev = false;
			try
			{
				// Go to page?
				if (asection.SkipExpreV.Length > 0)
				{
					newposy = System.Convert.ToInt32(Evaluator.EvaluateText(asection.SkipExpreV).AsDouble);
					movev = true;
				}
			}
			catch (Exception E)
			{
				throw new ReportException(E.Message, asection, "SkipExpreV");
			}
			if (moveh)
			{
				if (asection.SkipRelativeH)
					pageposx = pageposx + newposx;
				else
					pageposx = newposx;
			}
			if (movev)
			{
				if (asection.SkipRelativeV)
					pageposy = pageposy + newposy;
				else
					pageposy = newposy;
			}
			FreeSpace = pagefooterpos - pageposy;
			if (FreeSpace < 0)
				FreeSpace = 0;
		}
		bool CheckSpace()
		{
			Point MaxExtent = new Point();
			if (FreeSpace == 0)
				return false;
			MaxExtent.X = pagespacex;
			MaxExtent.Y = FreeSpace;
			if (!sectionextevaluated)
			{
				// Skip to page and position
				if (asection.SkipType == SkipType.Before)
					SkipToPageAndPosition();
				sectionext = asection.GetExtension(FDriver, MaxExtent,false);
			}
			bool aresult = true;
			if (sectionext.Y > FreeSpace)
			{
				if (PrintedSomething)
					aresult = false;
				else
				{
                    // Force partial print for expressions
                    sectionext = asection.GetExtension(FDriver, MaxExtent, true);
                    if (sectionext.Y > FreeSpace)
                      throw new UnNamedException("No Espace to print SubReport " +
						CurrentSubReportIndex.ToString() + " Section " +
						CurrentSectionIndex.ToString());
				}
			}
			sectionextevaluated = false;
			return aresult;
		}
		void PrintSection(bool datasection, ref bool PartialPrint)
		{
			Point MaxExtent = new Point();
			bool ispagerepeat;
			if (subreport.CurrentGroupIndex < 0)
			{
				if (section.GroupName == subreport.Sections[subreport.FirstDetail + subreport.CurrentGroupIndex].GroupName)
				{
					if (section.IniNumPage)
						PageNumGroup = 0;
					section.FirstPage = MetaFile.CurrentPage;
				}
			}
			pagefooterpos = pageposy + FreeSpace;
			PartialPrint = false;
			MaxExtent.X = pagespacex;
			MaxExtent.Y = FreeSpace;

			if (datasection)
			{
				ispagerepeat = false;
				if (asection.SectionType == SectionType.GroupHeader)
					ispagerepeat = asection.PageRepeat;
				if (!ispagerepeat)
				{
					oldprintedsection = section;
					oldprintedsectionext = sectionext;
					// If the section have been derived from a page repeat
					//...
					if (FGroupHeaders.Count < 1)
						PrintedSomething = true;
				}
			}
			// If the section is not aligned at bottom of the page then
			if (!asection.AlignBottom)
			{
				asection.Print(FDriver, pageposx, pageposy, -1, -1, MetaFile, MaxExtent, ref PartialPrint);
				FreeSpace = FreeSpace - sectionext.Y;
				pageposy = pageposy + sectionext.Y;
			}
			else
			// Align to bottom
			{
				pageposy = pageposy + FreeSpace - sectionext.Y;
				asection.Print(FDriver, pageposx, pageposy, -1, -1, MetaFile, MaxExtent, ref PartialPrint);
				FreeSpace = 0;
			}
			if (asection.SkipType == SkipType.After)
				SkipToPageAndPosition();
		}

		void PrintFixedSections(bool Headers)
		{
			int pheader, pfooter;
			int i, pheadercount, pfootercount;
			Section psection;
			int afirstdetail;
			bool printit;
			Point MaxExtent = new Point();
			bool PartialPrint;

			PartialPrint = false;
			MaxExtent.X = pagespacex;
			MaxExtent.Y = FreeSpace;
			if (Headers)
			{
				// First the global headers
				for (i = 0; i < GHeaders.Count; i++)
				{
					psection = GHeaders[i];
					if (psection.EvaluatePrintCondition())
					{
						asection = psection;
						CheckSpace();
						PrintSection(false, ref PartialPrint);
					}
				}
				// Print the header fixed sections
				pheader = subreport.FirstPageHeader;
				pheadercount = subreport.PageHeaderCount;
				for (i = 0; i < pheadercount; i++)
				{
					psection = subreport.Sections[i + pheader];
					if (!psection.Global)
					{
						if (psection.EvaluatePrintCondition())
						{
							asection = psection;
							CheckSpace();
							PrintSection(false, ref PartialPrint);
						}
					}
				}
				int index;
				SubReports psubreports = new SubReports();
				for (i = 0; i < PendingSections.Count; i++)
				{
					psection = PendingSections[i];
					index = psubreports.IndexOf(psection.SubReport);
					if (index < 0)
						psubreports.Add(psection.SubReport);
				}
				index = psubreports.IndexOf(subreport);
				if (index < 0)
					psubreports.Add(subreport);
				// Now prints repeated group headers
				for (int j = 0; j < psubreports.Count; j++)
				{
					SubReport subrep = psubreports[j];
					afirstdetail = subrep.FirstDetail;
					for (i = subrep.GroupCount; i > 0; i--)
					{
						psection = subrep.Sections[afirstdetail - i];
						if (psection.PageRepeat)
						{
//							if ((Math.Abs(subrep.CurrentGroupIndex) <= i) && (section!=psection))
//                            if ((Math.Abs(subreport.CurrentGroupIndex) < i))
                            // Group headers are printed when page repeat=true
                            bool dopagerepeat=false;
                            // Allways if there is another active subreport
                            if (subrep != subreport)
                                dopagerepeat = true;
                            else
                            {
                                // Never if the current section is just the group header
                                if (section != psection)
                                {
                                    // If the section is enclosed between the header
                                    // and the footer 
                                    if (CurrentSectionIndex > subrep.FirstDetail - i)
                                    {
                                        if (psection.ForcePrint)
                                        {
                                            //  (footer included)
                                            if (CurrentSectionIndex <= subrep.LastDetail + i)
                                                dopagerepeat = true;
                                        }
                                        else
                                        {
                                            //  (footer not included)
                                            if (CurrentSectionIndex < subrep.LastDetail + i)
                                                dopagerepeat = true;
                                        }
                                    }
                                }
                            }
                            if (dopagerepeat)
                            {
								if (psection.EvaluatePrintCondition())
								{
									// Add group headers to be printed at the
									// main print loop (child subreports and horz/vert.desp)
									FGroupHeaders.Add(psection);
								}
							}
						}
					}
				}
				pagefooterpos = pageposy + FreeSpace;
				// Reserve space for page footers
				// Print conditions for footers are evaluated at the begining of
				// the page

				// Global page footers
				for (i = 0; i < GFooters.Count; i++)
				{
					psection = GFooters[i];
					asection = psection;
					CheckSpace();
					pagefooterpos = pageposy + FreeSpace - sectionext.Y;
					FreeSpace = FreeSpace - sectionext.Y;
				}
				pfooter = subreport.FirstPageFooter;
				pfootercount = subreport.PageFooterCount;
				for (i = 0; i < pfootercount; i++)
				{
					psection = subreport.Sections[i + pfooter];
					if (!psection.Global)
					{
						asection = psection;
						havepagefooters = true;
						CheckSpace();
						pagefooters.Add(psection);
						pagefooterpos = pageposy + FreeSpace - sectionext.Y;
						FreeSpace = FreeSpace - sectionext.Y;
					}
				}
			}
			else
			{
				// Print page footers
				if ((GFooters.Count > 0) || (pagefooters.Count > 0))
				{
					pageposy = pagefooterpos;
					for (i = 0; i < pagefooters.Count; i++)
					{
						asection = pagefooters[i];
						if (!asection.Global)
						{
							printit = true;
							if (!asection.ForcePrint)
							{
								if (section == null)
									printit = false;
							}
							if (printit)
							{
								if (asection.EvaluatePrintCondition())
								{
									sectionext = asection.GetExtension(FDriver, MaxExtent,false);
									PrintSection(false, ref PartialPrint);
									FreeSpace = 0;
								}
							}
						}
					}
					// Global page footers
					for (i = 0; i < GFooters.Count; i++)
					{
						asection = GFooters[i];
						printit = true;
						if (!asection.ForcePrint)
						{
							if (section == null)
								printit = false;
						}
						if (printit)
						{
							if (asection.EvaluatePrintCondition())
							{
								sectionext = asection.GetExtension(FDriver, MaxExtent,false);
								PrintSection(false, ref PartialPrint);
								FreeSpace = 0;
							}
						}
					}
				}
			}
		}

		bool NextRecord(bool grouprestore)
		{
			SubReport subrep;
			bool aresult = false;
			subrep = CurrentSubReport;
			if (subrep.Alias.Length == 0) // Configure "Con.D.Principal" on subreport settings
				subrep.LastRecord = true;
			else
			{
				int index;
				index = DataInfo.IndexOf(subrep.Alias);
				if (index < 0)
					throw new NamedException("Alias not exists" + subrep.Alias,subrep.Alias);
				DataInfo[index].Data.Next();
				// If its the last record no group change
				if (!grouprestore)
				{
					subrep.LastRecord = DataInfo[index].Data.Eof;
				}
				if (!subrep.LastRecord)
				{
					if (!grouprestore)
					{
						subrep.GroupChanged();
						if (subrep.CurrentGroupIndex > 0)
						{
							aresult = true;
							DataInfo[index].Data.Prior();
						}
						else
							subrep.SubReportChanged(SubReportEvent.DataChange, "");
					}
					else
						subrep.SubReportChanged(SubReportEvent.DataChange, "");
				}
				FRecordCount++;
				CheckProgress(false);
			}
			return aresult;
		}
		public void DoUpdatePageSize(MetaPage MetaFilepage)
		{
			Point apagesize;
			// Sets page orientation and size
			PageDetail.PhysicWidth = MetaFile.CustomX;
			PageDetail.PhysicHeight = MetaFile.CustomY;
			MetaFilepage.Orientation = CurrentOrientation;
			MetaFilepage.PageDetail.Index = PageSizeIndex;
			if (!UpdatePageSize)
			{
				MetaFilepage.PageDetail = PageDetail;
				return;
			}
			MetaFilepage.UpdatedPageSize = true;
			// Sets and gets page size from the driver
			FDriver.SetOrientation(CurrentOrientation);
			if (PageSize != PageSizeType.Default)
			{
				apagesize = FDriver.SetPageSize(PageDetail);
			}
			else
			{
				apagesize = FDriver.GetPageSize(out PageSizeIndex);
			}
			InternalPageWidth = apagesize.X;
			InternalPageHeight = apagesize.Y;

			PageDetail.PhysicWidth = apagesize.X;
			PageDetail.PhysicHeight = apagesize.Y;
			MetaFilepage.PageDetail = PageDetail;
		}

		override public bool PrintNextPage()
		{
			int i;
			if (ErrorProcessing)
				throw new UnNamedException("Error processing\n" + LastErrorProcessing);
			PageNum++;
			PageNumGroup++;
			// Updates page size, and orientation
			if (MetaFile.Pages.CurrentCount <= PageNum)
			{
				if (TwoPass)
					MetaFile.ForwardOnly = false;
				MetaFile.Pages.Add(new MetaPage(MetaFile));
			}
			MetaFile.CurrentPage = PageNum;
			DoUpdatePageSize(MetaFile.Pages[MetaFile.Pages.CurrentCount - 1]);
			FGroupHeaders.Clear();
			PrintedSomething = false;
			if (section == null)
				throw new UnNamedException("Last section reached");
			if (subreport != null)
			{
				for (i = 0; i < SubReports.Count; i++)
				{
					SubReports[i].SubReportChanged(SubReportEvent.PageChange, "");
				}
			}
			havepagefooters = false;
			sectionextevaluated = false;
			oldprintedsection = null;
			if (!FCompose)
			{
				FreeSpace = InternalPageHeight;
				FreeSpace = FreeSpace - TopMargin - BottomMargin;
				pageposy = TopMargin;
				pageposx = LeftMargin;
			}
			else
			{
				if ((GHeaders.Count > 0) ||
					(subreport.PageHeaderCount > 0))
				{
					MetaFile.Pages.Add(new MetaPage(MetaFile));
					PageNum++;
					MetaFile.CurrentPage = MetaFile.Pages.CurrentCount - 1;
					FreeSpace = InternalPageHeight;
					FreeSpace = FreeSpace - TopMargin - BottomMargin;
					pageposy = TopMargin;
					pageposx = LeftMargin;
					DoUpdatePageSize(MetaFile.Pages[MetaFile.Pages.CurrentCount - 1]);
				}
				else
				{
					PrintedSomething = true;
				}
				//FCompose = false;
			}
			pagespacex = InternalPageWidth;
			int oldhorzdespposition = LeftMargin;

            System.Globalization.CultureInfo oldculture = System.Threading.Thread.CurrentThread.CurrentCulture;
            // Change culture
            switch (Language)
            {
                case 0:
                    Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                    break;
                case 1:
                    Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("es-ES");
                    break;
                case 2:
                    Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("ca");
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }

            pagefooters = new Sections();
			try
			{
				Point MaxExtent = new Point();
				bool PartialPrint;
				// Fills the page with fixed sections
				PrintFixedSections(true);
				// Group headers with child subreports
				if (FGroupHeaders.Count > 0)
				{
					PendingSections.Add(section);
					section = FGroupHeaders[0];
				}
				oldsubreport = subreport;
				Point lastvertpos = new Point();
				lastvertpos.X = pageposx;
				lastvertpos.Y = pageposy;
				while (section != null)
				{
					if (PrintedSomething)
					{
						if (section.EvaluateBeginPage())
							break;
					}
					asection = section;
					// Horz.Desp.
					if (oldprintedsection != null)
					{
						if (oldprintedsection.HorzDesp)
						{
							if (section.HorzDesp)
							{
								MaxExtent.X = pagespacex;
								MaxExtent.Y = FreeSpace;
								sectionext = section.GetExtension(FDriver, MaxExtent,false);
								sectionextevaluated = true;
								if ((pageposx + oldprintedsectionext.X + sectionext.X) <= pagespacex)
								{
									pageposx = pageposx + oldprintedsectionext.X;
									pageposy = pageposy - oldprintedsectionext.Y;
									FreeSpace = FreeSpace + oldprintedsectionext.Y;
								}
								else
									pageposx = LeftMargin;
							}
							else
							{
								pageposx = oldhorzdespposition;
							}
						}
						else
							oldhorzdespposition = pageposx;
					}
					if (!CheckSpace())
					{
						// If the current section have vert desp, do
						// a vertical and horz. desp if possible
						if (section.VertDesp)
						{
							// Go to old vertical position if fits horizontally
							MaxExtent.X = pagespacex;
							MaxExtent.Y = lastvertpos.Y - BottomMargin;
							sectionext = section.GetExtension(FDriver, MaxExtent,false);
							sectionextevaluated = true;
							if ((pageposx + sectionext.X * 2) <= pagespacex)
							{
								FreeSpace = FreeSpace + (pageposy - lastvertpos.Y);
								pageposx = pageposx + sectionext.X;
								pageposy = lastvertpos.Y;
							}
							else
							{
								pageposx = LeftMargin;
								break;
							}
							if (!CheckSpace())
								break;
						}
						else
							break;
					}
					PartialPrint = false;
					PrintSection(true, ref PartialPrint);
					if (!PartialPrint)
					{
						if (!section.VertDesp)
						{
							lastvertpos.X = pageposx;
							lastvertpos.Y = pageposy;
						}
						NextSection(true);
						if (section != null)
						{
							if (!section.VertDesp)
							{
								lastvertpos.X = pageposx;
								lastvertpos.Y = pageposy;
							}
						}
					}
					if (PrintedSomething)
					{
						if (asection.SkipPage)
							break;
					}
					// if Subreport changed and has have pagefooter
					if ((oldsubreport != subreport) && (havepagefooters))
						break;
					oldsubreport = subreport;
				}
				// Fills the page with fixed sections
				if (PrintedSomething)
				{
					if (oldprintedsection != null)
					{
						if (oldprintedsection.HorzDesp)
						{
							pageposx = oldhorzdespposition;
						}
					}
				}
				PrintFixedSections(false);
				pagefooters = null;
                System.Threading.Thread.CurrentThread.CurrentCulture = oldculture;
            }
            catch (Exception E)
			{
                System.Threading.Thread.CurrentThread.CurrentCulture = oldculture;

                pagefooters = null;
				ErrorProcessing = true;
				LastErrorProcessing = E.Message;
				throw;
			}
			bool aresult = (section == null);
			LastPage = aresult;
			if (LastPage)
			{
				MetaFile.Finish();
				CheckProgress(true);
			}
			if (aresult)
				EndPrint();
			return aresult;
		}
    }
}


