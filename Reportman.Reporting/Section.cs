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
using System.Drawing;
using System.Collections;
using Reportman.Drawing;
using System.ComponentModel;
#if REPMAN_DOTNET2
using System.Collections.Generic;
#endif

namespace Reportman.Reporting
{
    /// <summary>
    /// The background of a section can be visible only on design time, while preview only (will not print) or
    /// also print the background
    /// </summary>
	public enum BackStyleType { 
        /// <summary>
        /// The background will be only visible at design time, useful to fill preprinted forms, 
        /// you can place the background in the section, with exact dots per inch define,
        /// so you place each expression in each predefined location
        /// </summary>
        Design, 
        /// <summary>
        /// The background will be also visible at preview, but will not be printed
        /// </summary>
        Preview, 
        /// <summary>
        /// The background will be visible at design, preview and will be printed
        /// </summary>
        Print };
    /// <summary>
    /// Section type, each section of a subreport have different function
    /// </summary>
	public enum SectionType
	{
        /// <summary>
        /// Printed at the page begin
        /// </summary>
		PageHeader,
        /// <summary>
        /// Printed when a group starts
        /// </summary>
        GroupHeader,
        /// <summary>
        /// Printed for each record in the main dataset of the subreport containg the section
        /// </summary>
        Detail,
        /// <summary>
        /// Printed when a group ends
        /// </summary>
        GroupFooter,
        /// <summary>
        /// Printed at the page begin
        /// </summary>
        PageFooter
	};
    /// <summary>
    /// A section can skip to a page or position
    /// </summary>
	public enum SkipType { 
        /// <summary>
        /// The section will be printed in the natural place, just bellow the previous section, or
        /// right to it if Horzontal displacement property is true and there is enought space
        /// </summary>
        Default, 
        /// <summary>
        /// The section will skip to a diferent position before print, the other properties
        /// related (Skip to page,Horizontal Skip...) will be used to determine the position
        /// </summary>
        Before, 
        /// <summary>
        /// The section will skip to a diferent position before print, the other properties
        /// related (Skip to page,Horizontal Skip...) will be used to determine the position
        /// </summary>
        After };
    /// <summary>
    /// The sections are contained inside subreports, depending on the section type it 
    /// will print after the detail (group header), before the detail (group header), 
    /// or in special places (page headers and footers)
    /// </summary>
	public class Section : PrintItem,IDisposable
	{
		private const int DEF_DRAWWIDTH = 10700;
		private const int DEF_DRAWHEIGHT = 1500;
		private const int DEFAULT_DPI_BACK = 96;
        /// <summary>
        /// This expression is evaluated for each record of the Main dataset for the subreport, 
        /// it will determine a group break with the Bool Expression property
        /// </summary>
		public string ChangeExpression;
        /// <summary>
        /// If true, the engine breaks the group when the Group Expression is true, 
        /// else will break the group when the group expression result changes
        /// </summary>
        public bool ChangeBool;
        /// <summary>
        /// If true, the group header will print in each page where the group is still active.
        /// </summary>
        public bool PageRepeat;
        /// <summary>
        /// If this property is true, after printing the section a new page 
        /// will begin (unless there is nothing more to print)
        /// </summary>
        public bool SkipPage;
        /// <summary>
        /// This expression is evaluated before printing, if result is true the group will begin a page
        /// </summary>
        public string BeginPageExpression;
        /// <summary>
        /// The skip is a repositioning of the print pointer, you can place print pointer to another 
        /// page and/or page position with properties bellow
        /// </summary>
        public SkipType SkipType;
        /// <summary>
        /// If a skip type is assigned then this expression is used to determine the page to 
        /// relocate the print pointer
        /// </summary>
        public string SkipToPageExpre;
        /// <summary>
        /// If true the skip is relative to the print pointer else is relative to the current page
        /// </summary>
        public bool SkipRelativeH;
        /// <summary>
        /// If true the skip is relative to the print pointer else is relative to the current page
        /// </summary>
        public bool SkipRelativeV;
        /// <summary>
        /// Determines the horizontal position, the result of the expression must be an integer, 
        /// the unit is in twips (1440twips=1 inch), the skip may be relative or absolute 
        /// depending on property bellow
        /// </summary>
		public string SkipExpreH;
        /// <summary>
        /// Determines the verticalposition, the result of the expression must be an integer, 
        /// the unit is in twips (1440twips=1 inch), the skip may be relative or absolute 
        /// depending on property bellow
        /// </summary>
		public string SkipExpreV;
        /// <summary>
        /// If this property is true, the section will be aligned to the bottom of the page, 
        /// and of course next section will be printed in the next page, the alignment is done 
        /// after resizing (so it's compatible with Auto expand and Auto contract properties)
        /// </summary>
        public bool AlignBottom;
        /// <summary>
        /// The section will expand vertically to keep space for any component inside it. 
        /// This is useful when you want to print some variable text and you don't know how 
        /// much page space you need
        /// </summary>
        public bool AutoExpand;
        /// <summary>
        /// The section will contract vertically to avoid unused space. This is useful 
        /// when you want to print some variable text and you don't know how much page 
        /// space you need, of if some of the components are printed under certain conditions 
        /// and you want to profit space
        /// </summary>
        public bool AutoContract;
        /// <summary>
        /// When horizzontal desplacement is set to true, after printing the section 
        /// the print position will move to the left, in an amount equivalent to the section 
        /// width. But this will happen only when the next section to print has also the 
        /// horizontal desplacement set to true, else the horizontal print position will be 
        /// restored. Set this property to true in the detail section for example to print 
        /// labels left to right, adjust the section with so many details fit in the 
        /// total page width.
        /// </summary>
        public bool HorzDesp;
        /// <summary>
        /// When vertical desplacement is set to true, the print is performed as usually 
        /// (up to down), after reaching the bottom of the page, the print position will 
        /// place at the top of the page (or group header) , and the left print position 
        /// will move an amount equivalent to the section with (if fits in the page), 
        /// then printing continue. But this will happen only when the next section to print 
        /// has also the vertical desplacement set to true, else the engine will skip to 
        /// next page. Set this property to true in the detail section for example to 
        /// print labels up to down, adjust the section with so many details fit in the 
        /// total page width. 
        /// </summary>
        public bool VertDesp;
        /// <summary>
        /// Will load the section from a file so the report file can share sections. 
        /// The external path canIf a child subreport is assigned it will be processed 
        /// and printed after printing this section be an expression by preceding the 
        /// string by the @ symbol.
        /// </summary>
        public string ExternalFilename;
        /// <summary>
        /// Will load the section from a database so the reports can share sections.
        /// </summary>
        public string ExternalConnection;
        /// <summary>
        /// Will load the section from a database so the reports can share sections.
        /// </summary>
        public string ExternalTable;
        /// <summary>
        /// Will load the section from a database so the reports can share sections.
        /// </summary>
        public string ExternalField;
        /// <summary>
        /// Will load the section from a database so the reports can share sections.
        /// </summary>
        public string ExternalSearchField;
        /// <summary>
        /// Will load the section from a database so the reports can share sections.
        /// </summary>
        public string ExternalSearchValue;
        /// <summary>
        /// Stream format while saving the image, by default, it matches the parent
        /// subreport stream format
        /// </summary>
        public StreamFormatType StreamFormat;
        /// <summary>
        /// If a child subreport is assigned it will be processed and printed after 
        /// printing this section
        /// </summary>
        public SubReport ChildSubReport;
        /// <summary>
        /// This expression is evaluated before printing, if result is true the group 
        /// will begin a page
        /// </summary>
        public bool BeginPage;
        /// <summary>
        /// For page footers, means the page footer must print at the end of the 
        /// subreport if no data is still available. For group headers means the header
        /// will print also when footer is pending if page repeat is true
        /// </summary>
        public bool ForcePrint;
        /// <summary>
        /// Available only for groups, initialize the variable PageNum to 1 when the group 
        /// changes
        /// </summary>
        public bool IniNumPage;
        /// <summary>
        /// For page header and footers only, if this property is true, the header/footer will 
        /// be processed globally just like if it was outside the subreport
        /// </summary>
        public bool Global;
        /// <summary>
        /// Resolution of the image in pixels per inch unit, the size of the bitmap and the 
        /// resolution will determine the final size
        /// </summary>
        public int dpires;
        /// <summary>
        /// The background of the section can be used for multiple purposes, one is help 
        /// in design time to fill a form for example, but no preview or print (preprinted forms). 
        /// You can also select to draw the background image in preview or print the image on 
        /// the paper also.
        /// </summary>
        public BackStyleType BackStyle;
        /// <summary>
        /// Image draw style, for background image
        /// </summary>
        public ImageDrawStyleType DrawStyle;
        /// <summary>
        /// Type of section inside the subreport (page header,group header, detail)
        /// </summary>
        public SectionType SectionType;
        /// <summary>
        /// For sections with type Group, is the group name 
        /// </summary>
        public string GroupName;
        /// <summary>
        /// Determines how the image is shared in the report to enhace final metafile 
        /// or pdf size
        /// </summary>
        public SharedImageType SharedImage;

        /// <summary>
        /// List of child components, this components are labels, expressions, images...
        /// </summary>
        public PrintPosItems Components;
        /// <summary>
        /// The backgorund image can be embedded (Stream propery) or obtained throught 
        /// this expression property, the expression is evaluated, if the expression result 
        /// is a binary database field will try to read the image from there, see below the 
        /// image formats supported. If the field is a string field or the result of evaluation 
        /// is a string the engine will try to load the image as a file reference.
        /// </summary>
        public string BackExpression;
        /// <summary>
        /// Latest group value while report processing is in progress
        /// </summary>
		public Variant GroupValue;
		private TotalPages FPageGroupCountList;
		private MemoryStream FStream;
		private MemoryStream FOldStream;
		private long OldStreamPos;
#if REPMAN_ZLIB
		private MemoryStream FDecompStream;
#endif
        /// <summary>
        /// While processing the report, it contains the first page where the group began
        /// </summary>
		public int FirstPage;
        /// <summary>
        /// Returns true if the section is an external section
        /// </summary>
        [Browsable(false)]
		public bool IsExternal
		{
			get { return false; }
		}
        /// <summary>
        /// Embedded image, stored with the report structure containing a image stream, 
        /// will be drawn bellow the grid, using the resolution bellow. When embedding 
        /// large images you should set prefered save format in page setup to xml/gzip 
        /// to enhace performance.
        /// </summary>
        [Browsable(false)]
        public MemoryStream Stream
		{
			get { return FStream; }
		}
        /// <summary>
        /// Free resources used by the section
        /// </summary>
		override public void Dispose()
		{
            base.Dispose();
#if REPMAN_DOTNET1
#else
			if (FStream!=null)
			{
				FStream.Dispose();
				FStream=null;
			}
	#if	REPMAN_ZLIB
			if (FDecompStream!=null)
			{
				FDecompStream.Dispose();
				FDecompStream=null;
			}
	#endif
#endif
		}
    public string GetExternalDataDescription()
    {
      if (ExternalConnection.Length == 0)
        return "";
      else
      {
        return ExternalConnection + "-" + ExternalTable + "." + ExternalSearchValue;
      }
    }
        /// <summary>
        /// While processing the report, clears the page count items cache
        /// </summary>
		public void ClearPageCountList()
		{
			FPageGroupCountList.Clear();
		}
        /// <summary>
        /// While processing the report, update all page count references with the
        /// updated page count value, used by PAGEGROUPCOUNT special expression feature.
        /// </summary>
        public void UpdatePageCounts()
		{
			MetaFile meta = Report.MetaFile;
			meta.UpdateTotalPagesPCount(FPageGroupCountList, meta.Pages.CurrentCount - FirstPage);
		}
        /// <summary>
        /// The report notifies the section when a report starts or moves, so the section can
        /// update internal values
        /// </summary>
        /// <param name="newstate"></param>
        /// <param name="newgroup"></param>
		public override void SubReportChanged(SubReportEvent newstate, string newgroup)
		{
			base.SubReportChanged(newstate, newgroup);
			if (newstate == SubReportEvent.Start)
			{
                FirstPage = 0;
				OldStreamPos = -1;
				FOldStream = null;
			}
		}
        /// <summary>
        /// Parent subreport of the section
        /// </summary>
		public SubReport SubReport;
        /// <summary>
        /// Child subreport name, it stores the reference to the other subreport
        /// </summary>
		public string ChildSubReportName;
        /// <summary>
        /// Parent subreport name, it stores the reference to the parent subreport
        /// </summary>
		public string SubReportName;
        /// <summary>
        /// Initialization of the section
        /// </summary>
        /// <param name="rp"></param>
		public Section(BaseReport rp)
			: base(rp)
		{
			FPageGroupCountList = new TotalPages();
			FStream = new MemoryStream();
#if REPMAN_ZLIB
			FDecompStream = new MemoryStream();
#endif
			Height = DEF_DRAWWIDTH;
			Width = Height;
			Components = new PrintPosItems();
			ExternalTable = "REPMAN_REPORTS";
			ExternalField = "REPORT";
			ExternalSearchField = "REPORT_NAME";
			ForcePrint = false;
			dpires = DEFAULT_DPI_BACK;
			DrawStyle = ImageDrawStyleType.Full;
			StreamFormat = StreamFormatType.Text;
			Width = DEF_DRAWWIDTH;
			Height = DEF_DRAWHEIGHT;
			ChildSubReportName = "";
			SubReportName = "";
			GroupName = "";
			ChangeExpression = "";
			BeginPageExpression = "";
			SkipToPageExpre = "";
			SkipExpreH = "";
			SkipExpreV = "";
			BackExpression = "";
			ExternalFilename = ""; ExternalConnection = ""; ExternalTable = "";
			ExternalField = ""; ExternalSearchField = ""; ExternalSearchValue = "";
		}
        /// <summary>
        /// Evaluates the begin page expression, if true, the section must be the first
        /// page in the page, excluding page headers,group repeats
        /// </summary>
        /// <returns></returns>
		public bool EvaluateBeginPage()
		{
			Evaluator eval;
			bool aresult = false;
			if (BeginPageExpression.Length == 0)
				return false;
			eval = Report.Evaluator;
			eval.Expression = BeginPageExpression;
			try
			{
				eval.Evaluate();
				aresult = eval.Result;
			}
			catch (Exception E)
			{
				throw new ReportException(E.Message + ":BeginPageExpression",
					this, "BeginPageExpression");
			}
			return aresult;
		}
        /// <summary>
        /// Used by report processing to determine the size of the section, depending on
        /// the output driver the size may vary
        /// </summary>
        /// <param name="adriver"></param>
        /// <param name="MaxExtent"></param>
        /// <param name="ForcePartial"></param>
        /// <returns></returns>
        public override Point GetExtension(PrintOut adriver, Point MaxExtent, bool ForcePartial)
		{
			int minsize, maxsize, currentsize;
			Point compsize;
			int newsize, i;
			Point newextent = new Point(Width, Height);
			PrintPosItem acompo;
			bool DoPartialPrint;
			ExpressionItem eitem;
			Point aresult = base.GetExtension(adriver, MaxExtent,ForcePartial);
			DoPartialPrint = false;
			// Look for a partial print
			for (i = 0; i < Components.Count; i++)
			{
				acompo = (PrintPosItem)Components[i];
				if (acompo is ExpressionItem)
				{
					eitem = (ExpressionItem)acompo;
					if (eitem.IsPartial)
					{
						DoPartialPrint = true;
						break;
					}
				}
			}
			if (AutoContract)
			{
				minsize = 0;
				currentsize = 0;
			}
			else
			{
				minsize = aresult.Y;
				currentsize = aresult.Y;
			}
			if (AutoExpand)
				maxsize = int.MaxValue;
			else
				maxsize = aresult.X;
			if ((!AutoExpand) && (!AutoContract))
			{
				aresult.Y = currentsize;
				LastExtent = aresult;
				for (i = 0; i < Components.Count; i++)
				{
					acompo = (PrintPosItem)Components[i];
					if (acompo.Align != PrintItemAlign.None)
						acompo.GetExtension(adriver, newextent,ForcePartial);
				}
				return aresult;
			}
			for (i = 0; i < Components.Count; i++)
			{
				acompo = (PrintPosItem)Components[i];
				if (acompo.EvaluatePrintCondition())
					if (DoPartialPrint)
					{
						if (acompo is ExpressionItem)
						{
							eitem = (ExpressionItem)acompo;
							if (eitem.IsPartial)
							{
								newextent = MaxExtent;
								newextent.Y = newextent.Y - acompo.PosY;
								compsize = acompo.GetExtension(adriver, newextent,ForcePartial);
								if (compsize.Y > 0)
								{
									if ((acompo.Align == PrintItemAlign.Bottom) || (acompo.Align == PrintItemAlign.Right))
										newsize = compsize.Y;
									else
										newsize = acompo.PosY + compsize.Y;
									if (newsize < maxsize)
									{
										if (newsize > currentsize)
											currentsize = newsize;
									}
								}
							}
						}
					}
					else
					{
						newextent = MaxExtent;
						newextent.Y = newextent.Y - acompo.PosY;
						compsize = acompo.GetExtension(adriver, newextent,ForcePartial);
						if (compsize.Y > 0)
						{
							if ((acompo.Align == PrintItemAlign.Bottom) || (acompo.Align == PrintItemAlign.Right))
								newsize = compsize.Y;
							else
								newsize = acompo.PosY + compsize.Y;
							if (newsize < maxsize)
							{
								if (newsize > currentsize)
									currentsize = newsize;
							}
						}
					}
			}
			if (currentsize < minsize)
				currentsize = minsize;
			aresult.Y = currentsize;
			LastExtent = aresult;
			return aresult;
		}
        public string GetDisplayName(bool addchildsubreport)
        {
            string aresult="";
            switch (SectionType)
            {
                case SectionType.Detail:
                    if (SubReport.DetailCount > 1)
                    {
                        int acount = 1;
                        int index = SubReport.FirstDetail;
                        while (this != SubReport.Sections[index])
                        {
                            acount++;
                            index++;
                            if (index > SubReport.Sections.Count)
                                break;
                        }
                        aresult = Translator.TranslateStr(488) + "_" + acount.ToString();
                    }
                    else
                        aresult = Translator.TranslateStr(488);
                    break;
                case SectionType.PageHeader:
                    aresult = Translator.TranslateStr(484);
                    break;
                case SectionType.PageFooter:
                    aresult = Translator.TranslateStr(491);
                    break;
                case SectionType.GroupHeader:
                    aresult = Translator.TranslateStr(489)+" - "+GroupName;
                    break;
                case SectionType.GroupFooter:
                    aresult = Translator.TranslateStr(490) + " - " + GroupName;
                    break;
            }
            if (addchildsubreport)
            {
                if (ChildSubReport != null)
                    aresult = aresult +"("+ ChildSubReport.GetDisplayName(false)+")";
            }
            return aresult;
        }
        /// <summary>
        /// While processing the report, this is a helper to get the background image stream. It
        /// processes the background expression if necessary
        /// </summary>
        /// <returns></returns>
		public MemoryStream GetStream()
		{
			MemoryStream aresult = null;
			if (BackExpression.Length > 0)
			{
				aresult = Report.Evaluator.GetStreamFromExpression(BackExpression);
				if (aresult != null)
				{
					if (FOldStream != null)
					{
						if (FOldStream.Length == aresult.Length)
						{
							if (SharedImage == SharedImageType.Variable)
							{
								byte[] sx = FOldStream.ToArray();
								byte[] sy = aresult.ToArray();
								for (int i = 0; i < FOldStream.Length; i++)
								{
									if (sx[i] != sy[i])
									{
										OldStreamPos = -1;
										break;
									}
								}
							}
							else
								OldStreamPos = -1;
						}
						else
							OldStreamPos = -1;
					}
				}
			}
			else
			{
				if (FStream.Length > 0)
				{
					if (FOldStream != null)
						aresult = FOldStream;
					else
					{
						FStream.Seek(0, SeekOrigin.Begin);
						if (StreamUtil.IsCompressed(FStream))
						{
#if	REPMAN_ZLIB
							FStream.Seek(0, SeekOrigin.Begin);
							StreamUtil.DeCompressStream(FStream, FDecompStream);
							aresult = FDecompStream;
#else
	    					throw new UnNamedException("REPMAN_ZLIB not defined compressed streams not supported");
#endif
						}
						else
							aresult = FStream;
					}
				}
			}
			FOldStream = aresult;
			return aresult;
		}
        /// <summary>
        /// Prints the full section in a specific position of the page, the section will iterate the
        /// child components, printing each one
        /// </summary>
        /// <param name="adriver"></param>
        /// <param name="aposx"></param>
        /// <param name="aposy"></param>
        /// <param name="newwidth"></param>
        /// <param name="newheight"></param>
        /// <param name="metafile"></param>
        /// <param name="MaxExtent"></param>
        /// <param name="PartialPrint"></param>
		override protected void DoPrint(PrintOut adriver, int aposx, int aposy,
			int newwidth, int newheight, MetaFile metafile, Point MaxExtent,
			ref bool PartialPrint)
		{
			int i;
			PrintPosItem compo;
			int newposx, newposy;
			bool intPartialPrint;
			bool DoPartialPrint;
			MemoryStream astream;
			ExpressionItem compe=null;

			base.DoPrint(adriver, aposx, aposy, newwidth, newheight, metafile, MaxExtent, ref PartialPrint);
			// Draw the background if needed
			if (BackStyle != BackStyleType.Design)
			{
				astream = GetStream();
				if (astream == null)
					return;
				if (astream.Length > 0)
				{
					MetaObjectImage obj = new MetaObjectImage();
					obj.MetaType = MetaObjectType.Image;
					obj.Top = aposy; obj.Left = aposx;
					obj.Height = PrintHeight; obj.Width = PrintWidth;
					obj.DrawImageStyle = DrawStyle;
					obj.DPIRes = dpires;
					obj.PreviewOnly = BackStyle == BackStyleType.Preview;
					obj.DPIRes = dpires;
					if (OldStreamPos >= 0)
					{
						obj.StreamPos = OldStreamPos;
						obj.SharedImage = true;
					}
					else
					{
						obj.StreamPos = metafile.Pages[metafile.CurrentPage].AddStream(astream, SharedImage != SharedImageType.None);
						if (SharedImage != SharedImageType.None)
							OldStreamPos = obj.StreamPos;
						obj.SharedImage = SharedImage != SharedImageType.None;
					}
					obj.StreamSize = astream.Length;
					metafile.Pages[metafile.CurrentPage].Objects.Add(obj);
				}
			}
			DoPartialPrint = false;
			// Look for a partial print
			for (i = 0; i < Components.Count; i++)
			{
				compo = Components[i];
				if (compo is ExpressionItem)
				{
					compe = (ExpressionItem)compo;
					if (compe.IsPartial)
					{
						DoPartialPrint = true;
						break;
					}
				}
			}
			PartialPrint = false;
			for (i = 0; i < Components.Count; i++)
			{
				compo = Components[i];
				newwidth = -1;
				newheight = -1;
				// Component alignment
				switch (compo.Align)
				{
					case PrintItemAlign.None:
						newposx = aposx + compo.PosX;
						newposy = aposy + compo.PosY;
						break;
					case PrintItemAlign.Bottom:
						newposx = aposx + compo.PosX;
						newposy = aposy + LastExtent.Y - compo.LastExtent.Y;
						break;
					case PrintItemAlign.Right:
						newposx = aposx + LastExtent.X - compo.LastExtent.X;
						newposy = aposy + compo.PosY;
						break;
					case PrintItemAlign.BottomRight:
						newposx = aposx + compo.PosX;
						newposy = aposy + LastExtent.Y - compo.LastExtent.Y;
						break;
					case PrintItemAlign.LeftRight:
						newposx = aposx;
						newposy = aposy + compo.PosY;
						newwidth = LastExtent.X;
						break;
					case PrintItemAlign.TopBottom:
						newposx = aposx + compo.PosX;
						newposy = aposy + compo.PosY;
						newheight = LastExtent.Y;
						break;
					case PrintItemAlign.AllClient:
						newposx = aposx;
						newposy = aposy;
						newwidth = LastExtent.X;
						newheight = LastExtent.Y;
						break;
					default:
						newposx = aposx + compo.PosX;
						newposy = aposy + compo.PosY;
						break;
				}

				if (DoPartialPrint)
				{
                    bool compoprinted = false;
                    if (compo is ExpressionItem)
					{
						compe = (ExpressionItem)compo;
						if (compe.IsPartial)
						{
							intPartialPrint = false;
							compo.Print(adriver, newposx, newposy,
								newwidth, newheight, metafile, MaxExtent, ref intPartialPrint);
							if (intPartialPrint)
								PartialPrint = true;
                            compoprinted = true;
						}
					}
                    if ((!compoprinted) && compo.PartialFlag && (!PartialPrint))
                    {
                        intPartialPrint = false; 
                        compo.PartialFlag = false;
                        compo.Print(adriver, newposx, newposy,
                            newwidth, newheight, metafile, MaxExtent, ref intPartialPrint);
                    }
                    if ((compe==null) &&  ((compo.Align == PrintItemAlign.TopBottom) ||
                             (compo.Align == PrintItemAlign.AllClient)))
                    {
                        bool dummypartial = false;
                        compo.Print(adriver, newposx, newposy,
                            newwidth, newheight, metafile, MaxExtent, ref dummypartial);
                    }
				}
				else
				{
                    compo.PartialFlag = false;
					// Evaluates print condition of each comonent
					if (compo.EvaluatePrintCondition())
					{
						intPartialPrint = false;
                        // For aligned to bottom elements, the partial print will
                        // force to align bottom at the last section of the partial
                        // print
                        if (PartialPrint && ((compo.Align==PrintItemAlign.Bottom) || 
                            (compo.Align==PrintItemAlign.BottomRight)) )
                            compo.PartialFlag=true;
                        else
						    compo.Print(adriver, newposx, newposy,
							    newwidth, newheight, metafile,
							    MaxExtent, ref intPartialPrint);
						if (intPartialPrint)
							PartialPrint = true;
						if (compo is ExpressionItem)
						{
							compe = (ExpressionItem)compo;
							if (compe.IsGroupPageCount)
								if (compe.LastMetaIndex > 0)
									AddPageGroupCountItem(metafile.CurrentPage, compe.LastMetaIndex, compe.DisplayFormat);
						}
					}
				}
			}
		}
        protected override string GetClassName()
        {
            return "TRPSECTION";
        }
        void AddPageGroupCountItem(int apageindex, int aobjectindex,
			string adisplayformat)
		{
			TotalPage aobject;
			SubReport subrep;
			int index;
			subrep = SubReport;
			index = subrep.GroupIndex(GroupName);
			if (index > 0)
			{
				aobject = new TotalPage();
				aobject.PageIndex = apageindex;
				aobject.ObjectIndex = aobjectindex;
				aobject.DisplayFormat = adisplayformat;
				subrep.Sections[subrep.FirstDetail - index].FPageGroupCountList.Add(aobject);
			}
		}

	}
    /// <summary>
    /// Collection of sections
    /// </summary>
#if REPMAN_DOTNET1
	public class Sections
	{
		Section[] FItems;
		const int FIRST_ALLOCATION_OBJECTS = 10;
		int FCount;
		public Sections()
		{
			FCount = 0;
			FItems = new Section[FIRST_ALLOCATION_OBJECTS];
		}
		public void Clear()
		{
			for (int i = 0; i < FCount; i++)
				FItems[i] = null;
			FCount = 0;
		}
		public int IndexOf(Section avalue)
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
		public void Remove(Section sec)
		{
			RemoveAt(IndexOf(sec));
		}
		public void Insert(int index,Section sec)
		{
			Add(sec);
			for (int i=index;i<FCount-1;i++)
			{
				FItems[i+1]=FItems[i];
			}
			FItems[index]=sec;
		}
		private void CheckRange(int index)
		{
			if ((index < 0) || (index >= FCount))
				throw new UnNamedException("Index out of range on Sections collection");
		}
		public Section this[int index]
		{
			get { CheckRange(index); return FItems[index]; }
			set { CheckRange(index); FItems[index] = value; }
		}
		public int Count { get { return FCount; } }
		public void Add(Section obj)
		{
			if (FCount > (FItems.Length - 2))
			{
				PrintItem[] nobjects = new Section[FCount];
				System.Array.Copy(FItems, 0, nobjects, 0, FCount);
				FItems = new Section[FItems.Length * 2];
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
			return new SectionsEnumerator(this);
		}
		// Inner class implements IEnumerator interface:
		public class SectionsEnumerator : IEnumerator
		{
			private int position = -1;
			private Sections t;

			public SectionsEnumerator(Sections t)
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
	public class Sections:System.Collections.Generic.List<Section>
	{

	}
#endif
}
