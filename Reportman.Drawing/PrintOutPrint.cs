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
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Reportman.Drawing
{
    /// <summary>
    /// Print driver implementing output to printer, it does not allow showing print dialog becuase it does not
    /// have dependencies on Windows.Forms
    /// </summary>
	public class PrintOutPrint : PrintOutNet
	{
        protected PrintOut overridedriver;
        private static SortedList<string, List<PaperSize>> cachedpagesizes;
        private static SortedList<string, PaperSize> cacheddefaultpagesizes;
        private MetaFile FMeta;
        private PrintDocument assigneddoc;
		private int FCurrentPage;
        private int FCurrentPrintPage;
		private PrintPageEventArgs Fev;		
		private bool doprint;
		private bool docancel;
        public bool UseStandardPrintController;
        public bool ShowPrintProgress;
        /// <summary>
        /// Internal property, when this option is set, the printes pages will remain in memory
        /// this is useful for printing multiple copies or process the generated MetaFile later
        /// </summary>
		protected bool DisableForwardOnly;
		int FPrintPage;
        static PrintOutPrint()
        {
            cachedpagesizes = new SortedList<string, List<PaperSize>>();
            cacheddefaultpagesizes = new SortedList<string, PaperSize>();
        }
        public string OutputDevice
        {
            get
            {
                if (doc == null)
                    return "";
                return doc.PrinterSettings.PrinterName;
            }
        }
        protected int FPagesPrinted;
        public int PagesPrinted
        {
            get
            {
                return FPagesPrinted;
            }
        }
        protected int FBlackLines;
        public int BlackLinesPrinted
        {
            get{
            return FBlackLines;}
        }
        protected int FWhiteLines;
        public int WhiteLinesPrinted
        {
            get{
            return FWhiteLines;}
        }
        /// <summary>
        /// Current PrintDocument
        /// </summary>
		    public PrintDocument doc;
        /// <summary>
        /// Constructor
        /// </summary>
        public PrintOutPrint()
            : base()
        {
            DrawFound = false;
        }
        /// <summary>
        /// When BrowsePaperSizes is true, the print driver will browse all paper sizes on the print
        /// server until it finds the correct page size. This procedure can be very slow, specially if
        /// the printer is a network printer, you can disable this browsing
        /// </summary>
		public bool BrowsePaperSizes=true;
        /// <summary>
        /// The procedure creates a new PrintDocument
        /// </summary>
		override public void NewDocument(MetaFile meta)
		{
            if (doc == null)
            {
                doc = new PrintDocument();
            }
            if (!ShowPrintProgress)
            {
                if (UseStandardPrintController)
                    if (!(doc.PrintController is StandardPrintController))
                        doc.PrintController = new StandardPrintController();
            }
            if (ForcePrinterName.Length > 0)
            {
                doc.PrinterSettings.PrinterName = ForcePrinterName;
            }
            else
            {
                if (DefaultPrinterName != "")
                    doc.PrinterSettings.PrinterName = DefaultPrinterName;
            }
        }
        /// <summary>
        /// Set paper orientation
        /// </summary>
        override public void SetOrientation(OrientationType PageOrientation)
		{
			base.SetOrientation(PageOrientation);
            OrientationType norientation = OrientationType.Portrait;
            if (PageOrientation != OrientationType.Default)
                norientation = PageOrientation;
    		if (doc == null)
				doc = new PrintDocument();
			doc.PrinterSettings.DefaultPageSettings.Landscape = (norientation == OrientationType.Landscape);				
		}
        /// <summary>
        /// Get current page size, depending on default printed
        /// </summary>
        /// <param name="indexqt">Output parameter, index on PageSizeArray</param>
        /// <returns></returns>
		override public Point GetPageSize(out int indexqt)
		{
			if (doc == null)
				doc = new PrintDocument();
			bool Landscape = false;
			PaperSize psize;
			if (PrinterSettings.InstalledPrinters.Count == 0)
			{
              psize=new PaperSize("A4",827, 1169);				
			}
			else
			{
              psize=doc.PrinterSettings.DefaultPageSettings.PaperSize;
              Landscape=doc.PrinterSettings.DefaultPageSettings.Landscape;
			}
			Point apoint = (new Point(psize.Width,
				psize.Height));
		//System.Console.WriteLine("Kind:"+psize.Kind.ToString()+
//			" Name: "+psize.PaperName+" Size:"+apoint.X.ToString()+"x"+apoint.Y.ToString());
            if ((apoint.X == 0) || (apoint.Y == 0))
            {
                indexqt = PaperSizeToQtIndex(psize);
                apoint = new Point(PageSizeArray[indexqt,0],PageSizeArray[indexqt,1]);
                apoint = new Point(apoint.X * 1440 / 1000, apoint.Y * 1440 / 1000);
            }
            else
            {
                indexqt = PaperSizeToQtIndex(psize);
                apoint = new Point(apoint.X * 1440 / 100, apoint.Y * 1440 / 100);
            }
			if (Landscape)
			{
				int y = apoint.Y;
                apoint.Y = apoint.X;
                apoint.X = y;
			}
			return apoint;
		}
		private void pd_BeginPrint(object sender, PrintEventArgs ev)
		{

		}
//		public PaperSize PageDetailToPaperSize(PageSizeDetail PageDetail)
//		{
//			PaperSize psize = new PaperSize("dfsdsd", 12, 12);
//
//			return (psize);
//		}
//		public PaperSource PageDetailToPaperSource(PageSizeDetail PageDetail)
//		{
//			return (new PrinterSettings().PaperSources[0]);
//		}
        /// <summary>
        /// Draw a page to the print surface
        /// </summary>
        /// <param name="meta">MetaFile containing the page</param>
        /// <param name="page">MetaPage to be drawn</param>
		override public void DrawPage(MetaFile meta, MetaPage page)
		{
            if (Fev == null)
            {
                base.DrawPage(meta, page);
                return;
            }
			Graphics gr = Fev.Graphics;
			for (int i = 0; i < page.Objects.Count; i++)
			{
				DrawObject(gr, page, page.Objects[i]);
			}
		}
		private void pd_QueryPageSettingsEvent(object sender, QueryPageSettingsEventArgs e)
		{
			doprint = true;
			if (FMeta.Finished)
			{
//				if (doc.PrinterSettings.ToPage > FMeta.Pages.CurrentCount)
			}
         //   throw new Exception("Imprimiendo por: " + e.PageSettings.PrinterSettings.PrinterName);
			while ((FCurrentPage + 1) < doc.PrinterSettings.FromPage)
				FCurrentPage++;
			UpdateHardMargins();
			if ((FCurrentPage + 1) > doc.PrinterSettings.ToPage)
				doprint = false;
			if (doprint)
			{
				FMeta.RequestPage(FCurrentPage+1);
				if (FMeta.Pages.CurrentCount > FCurrentPage)
				{
					FPrintPage = FCurrentPage;
					MetaPage apage = (MetaPage)FMeta.Pages[FCurrentPage];

					if (apage.UpdatedPageSize)
					{
						// Check for page size
                        int nwidth = apage.PhysicWidth;
                        int nheight = apage.PhysicHeight;
                        if (FMeta.Orientation == OrientationType.Landscape)
                        {
                            nwidth = apage.PhysicHeight;
                            nheight = apage.PhysicWidth;
                        } 
                        e.PageSettings.PaperSize = FindPaperSize(apage.PageDetail.Index, nwidth, nheight);
						if (apage.Orientation == OrientationType.Default)
							e.PageSettings.Landscape = doc.DefaultPageSettings.Landscape;
						else
							if (apage.Orientation == OrientationType.Landscape)
								e.PageSettings.Landscape = true;
							else
								e.PageSettings.Landscape = false;
					}
					else
						e.PageSettings = doc.DefaultPageSettings;
                    if (AutoScalePrint)
                    {
                        int physicwidth = apage.PhysicWidth * 100 / 1440;
                        int physicheight = apage.PhysicHeight * 100 / 1440;
                        int realpagewidth = e.PageSettings.PaperSize.Width;
                        int realpageheight = e.PageSettings.PaperSize.Height;
                        if (e.PageSettings.Landscape)
                        {
                            realpagewidth = e.PageSettings.PaperSize.Height;
                            realpageheight = e.PageSettings.PaperSize.Width;
                        }
                        if ((Math.Abs(physicheight - realpageheight) > 10) ||
                            (Math.Abs(physicwidth - realpagewidth) > 10))
                        {
                            // Scale and center
                            float propx = (float)realpagewidth/ physicwidth;
                            float propy = (float)realpageheight / physicheight;
                            float proportion = propx;
                            if (propy < propx)
                            {
                                proportion = propy;
                                Scale = proportion;

                                ScaleOffsetX = Convert.ToInt32((physicwidth*proportion - realpagewidth)/2);
                            }
                            else
                            {
                                Scale = proportion;
                                ScaleOffsetY = Convert.ToInt32((physicheight*proportion - realpageheight) / 2);
                            }
                            UpdateHardMargins();
                        }
                    }
                }
			}
			docancel = false;
			FCurrentPage++;
		}
        public bool AutoScalePrint = false;
		private void pd_PrintPage(object sender, PrintPageEventArgs ev)
		{
			Fev = ev;
            try
            {
                if (doprint)
                {
                    while ((FCurrentPrintPage + 1) < doc.PrinterSettings.FromPage)
                        FCurrentPrintPage++;
                    FMeta.RequestPage(FCurrentPrintPage + 1);
                    if (FMeta.Pages.CurrentCount > FCurrentPrintPage)
                    {
                        MetaPage apage = (MetaPage)FMeta.Pages[FCurrentPrintPage];
                        Fev.Graphics.PageUnit = GraphicsUnit.Pixel;
                        if (!AutoScalePrint)
                            Scale = 1.0f;
                        NewPage(FMeta, apage);
                        DrawPage(FMeta, apage);
                        EndPage(FMeta);
                        FPagesPrinted++;
                        if (doc.PrinterSettings.Copies != 1)
                            FPagesPrinted = FPagesPrinted+doc.PrinterSettings.Copies;
                        else
                            FPagesPrinted++;

                    }
                }
                docancel = false;
                bool hasmorepages = false;
                FCurrentPrintPage++;
                if ((FCurrentPrintPage < doc.PrinterSettings.ToPage) && (FCurrentPrintPage<FMeta.Pages.CurrentCount))
                {
                    hasmorepages = true;
                }
                else
                {
                    hasmorepages = false;
                    FCurrentPrintPage = 0;
                }
                ev.HasMorePages = hasmorepages;
                ev.Cancel = docancel;
            }
            finally
            {
                Fev = null;
            }
		}
#if REPMAN_MONO
#else
		private Duplex DuplexIntToDuplex(int dup)
		{
			Duplex a = new Duplex();
			switch (dup)
			{
				case 1:
					a = Duplex.Simplex;
					break;
				case 2:
					a = Duplex.Vertical;
					break;
				case 3:
					a = Duplex.Horizontal;
					break;
				default:
					a = Duplex.Default;
					break;

			}

			return a;
		}
#endif
#if REPMAN_DOTNET2
#else
#if REPMAN_MONO
#else
		[DllImport("gdi32")]
		static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
#endif
#endif
        /// <summary>
        /// Update HardMargin values from current printer driver
        /// </summary>
        /// 
		protected void UpdateHardMargins()
		{
			UseHardMargin = true;
#if REPMAN_DOTNET2
			HardMarginX = (int)Math.Round((doc.DefaultPageSettings.HardMarginX+ ScaleOffsetX) * 1440 / 100);
			HardMarginY = (int)Math.Round((doc.DefaultPageSettings.HardMarginY+ ScaleOffsetY) * 1440 / 100);
#else
#if REPMAN_MONO
			// Mono 1.X
			HardMarginX = 0;
			HardMarginY = 0;
#else
			// Hard Margins will be calculated after after opening device for Dot Net 1
			const int LOGPIXELSX = 88;
			const int LOGPIXELSY = 90;
//			const int PHYSICALWIDTH = 110;
//			const int PHYSICALHEIGHT=111;
			const int PHYSICALOFFSETX=112;
			const int PHYSICALOFFSETY = 113;
			Graphics gr = doc.PrinterSettings.CreateMeasurementGraphics();
			IntPtr hDC = gr.GetHdc();
			int phyOffsetX = GetDeviceCaps(hDC, PHYSICALOFFSETX);
			int phyOffsetY = GetDeviceCaps(hDC, PHYSICALOFFSETY);
//			int phyWidth = GetDeviceCaps(hDC, DEVICECAPS.PHYSICALWIDTH);
//			int phyHeight = GetDeviceCaps(hDC, DEVICECAPS.PHYSICALHEIGHT);
			int gdpix = GetDeviceCaps(hDC, LOGPIXELSX);
			int gdpiy = GetDeviceCaps(hDC, LOGPIXELSY);
			gr.ReleaseHdc(hDC);
			HardMarginX = (int)Math.Round((double)phyOffsetX / gdpix*1440);
			HardMarginY = (int)Math.Round((double)phyOffsetX / gdpiy*1440);
#endif
#endif
		}
        int ScaleOffsetX;
        int ScaleOffsetY;
        /// <summary>
        /// Assign events to the PrintDocument and assign page settings
        /// </summary>
		override public bool PreparePrint(MetaFile meta)
		{
            FBlackLines = 0;
            FWhiteLines = 0;
            FPagesPrinted = 0;
            
            if (overridedriver != null)
            {
                if (!overridedriver.PreparePrint(meta))
                    return false;
            }
            else
            {
                if (!base.PreparePrint(meta))
                    return false;
            }

            if (doc == null)
            {
                doc = new PrintDocument();
            }
            InitPrinter(meta);

			if (!DisableForwardOnly)
				meta.ForwardOnly = true;
			PRINTOUT_MAX_PAGES = 32000;
			if (ToPage > 32000)
				ToPage = 32000;
			FCurrentPage = 0;
            FCurrentPrintPage = 0;
			FMeta = meta;
			meta.RequestPage(0);
            if (assigneddoc!=doc)
            {
                doc.BeginPrint += new PrintEventHandler(pd_BeginPrint);
                doc.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                doc.QueryPageSettings += new QueryPageSettingsEventHandler(pd_QueryPageSettingsEvent);
                assigneddoc = doc;
            }
			if ((FMeta.Finished) && (FromPage > FMeta.Pages.CurrentCount))
				doc.PrinterSettings.FromPage = FMeta.Pages.CurrentCount;
			else
				doc.PrinterSettings.FromPage = FromPage;
			if ((FMeta.Finished) && (ToPage > FMeta.Pages.CurrentCount))
				doc.PrinterSettings.ToPage = FMeta.Pages.CurrentCount;
			else
				doc.PrinterSettings.ToPage = ToPage;
			doc.PrinterSettings.MinimumPage = 1;
			if (FMeta.Finished)
				doc.PrinterSettings.MaximumPage = FMeta.Pages.CurrentCount;
			else
				doc.PrinterSettings.MaximumPage = MetaFile.MAX_NUMBER_PAGES;
			doc.PrinterSettings.Collate = FMeta.CollateCopies;
            if (FMeta.Pages.Count > 0)
            {
                if (FMeta.Pages[0].PageDetail.PaperSource != 0)
                {
                    //PaperSource psource = new PaperSource();
                    //psource.RawKind = FMeta.Pages[0].PageDetail.PaperSource;
                    // Not working
                    //doc.PrinterSettings.DefaultPageSettings.PaperSource = psource;


                    foreach (PaperSource psource in doc.PrinterSettings.PaperSources)
                    {
                        if (psource.RawKind == FMeta.PaperSource)
                        {
                            doc.PrinterSettings.DefaultPageSettings.PaperSource = psource;
                            BrowsePaperSizes = true;
                            break;
                        }
                    }
                }
            }
            int nwidth = meta.CustomX;
            int nheight = meta.CustomY;
            if (meta.Orientation == OrientationType.Landscape)
            {
                nwidth = meta.CustomY;
                nheight = meta.CustomX;
            }
            if (!AutoScalePrint)
            {
                PaperSize npapersize = FindPaperSize(meta.PageSizeIndex, nwidth, nheight);
                doc.PrinterSettings.DefaultPageSettings.PaperSize = npapersize;
                doc.DefaultPageSettings.PaperSize = npapersize;
            }
           
#if REPMAN_MONO
#else
			doc.PrinterSettings.Duplex = DuplexIntToDuplex(FMeta.Duplex);
#endif
            if (FMeta.Orientation != OrientationType.Default)
            {
                doc.DefaultPageSettings.Landscape = (FMeta.Orientation == OrientationType.Landscape);
            }
            else
                doc.DefaultPageSettings.Landscape = false;
			if (Copies > 0)
				doc.PrinterSettings.Copies = (short)Copies;
			else
			{
				if (FMeta.Copies > 0)
					doc.PrinterSettings.Copies = (short)FMeta.Copies;
			}
            return true;
		}
        /// <summary>
        /// Find the PageSizeArray index given a PaperSize 
        /// </summary>
        /// <param name="psize">PaperSize structure</param>
        /// <returns>Integer index to PageSizeArray</returns>
        public static int PaperSizeToQtIndex(PaperSize psize)
        {
            int indexqt;
            switch (psize.Kind)
            {
                case PaperKind.A4:
                    indexqt = 0;       
                    break;
                case PaperKind.B5:
                    indexqt = 1;
                    break;
                case PaperKind.Letter:
                    indexqt = 2;
                    break;
                case PaperKind.Legal:
                    indexqt = 3;
                    break;
                case PaperKind.Executive:
                    indexqt = 4;
                    break;
                case PaperKind.A2:
                    indexqt = 7;
                    break;
                case PaperKind.A3:
                    indexqt = 8;
                    break;
                case PaperKind.A5:
                    indexqt = 9;
                    break;
                case PaperKind.A6:
                    indexqt = 10;
                    break;
                case PaperKind.B4:
                    indexqt = 19;
                    break;
                case PaperKind.C5Envelope:
                    indexqt = 24;
                    break;
                case PaperKind.DLEnvelope:
                    indexqt = 26;
                    break;
                case PaperKind.Folio:
                    indexqt = 27;
                    break;
                case PaperKind.Ledger:
                    indexqt = 28;
                    break;
                case PaperKind.Tabloid:
                    indexqt = 29;
                    break;
                default:
#if REPMAN_DOTNET2
#if REPMAN_MONO
			if (psize.Width==0 || psize.Height==0)
			{
				indexqt=0;
			}
			else
			{
				indexqt=30;
			}
			break;
#else
                    indexqt = 30+psize.RawKind;
                    break;
#endif
#else
                    indexqt = 30;
                    break;
#endif
            }
            return indexqt;
        }
        /// <summary>
        /// Converts a PageSizeArray index to a RawKind, the RawKind index is only available on .Net 2.x
        /// </summary>
        /// <param name="index">Index in the PageSizeArray</param>
        /// <param name="rwidth">It's filled with the width of the paper in inch thousands</param>
        /// <param name="rheight">It's filled with the height of the paper in inch thousands</param>
        /// <returns>RawKind value</returns>
        public static int PageIndexToPaperRawKind(int index,ref int rwidth,ref int rheight)
        {
            rwidth = 0;
            rheight = 0;
            int rawkind;
            switch (index)
            {
                case 0:
                    rawkind = 9;
                    break;
                case 1:
                    rawkind = 13;
                    break;
                case 2:
                    rawkind = 0;
                    break;
                case 3:
                    rawkind = 5;
                    break;
                case 4:
                    rawkind = 7;
                    break;
                // A0,A1
                case 7:
                    rawkind = 66;
                    break;
                case 8:
                    rawkind = 8;
                    break;
                case 9:
                    rawkind = 11;
                    break;
                case 10:
                    rawkind = 70;
                    break;
                // A7,A8,A9,B0,B1,B10,B2,B3,B4,B6,B7,B8,B9
                // C5E
                case 24:
                    rawkind = 28;
                    break;
                case 32:
                    rawkind = 2;
                    break;
                case 108:
                    rawkind = 61;
                    break;
                default:
                    if ((index >= 31) && (index <= 117))
                        rawkind = index;
                    else
                        rawkind = -1;
                    break;
            }
            if (rawkind >= 0)
            {
                rwidth = PageSizeArray[index,0];
                rheight = PageSizeArray[index,1];
            }
            return (rawkind);
        }
    private static List<PaperSize> GetPaperSizes(string printername)
    {
      List<PaperSize> nlist;
      Monitor.Enter(cachedpagesizes);
      try
      {
        if (cachedpagesizes == null)
          cachedpagesizes = new SortedList<string, List<PaperSize>>();
        int index = cachedpagesizes.IndexOfKey(printername);
        if (index < 0)
        {
          nlist = new List<PaperSize>();
          cachedpagesizes.Add(printername, nlist);
          foreach (string pname in PrinterSettings.InstalledPrinters)
          {
              if (pname == printername)
              {
                  using (PrintDocument ndoc = new PrintDocument())
                  {
                      ndoc.PrinterSettings.PrinterName = pname;
                      foreach (PaperSize psize in ndoc.PrinterSettings.PaperSizes)
                          nlist.Add(psize);
                  }
                  break;
              }
          }
        }
        else
          nlist = cachedpagesizes[printername];
        
      }
      finally
      {
        Monitor.Exit(cachedpagesizes);
      }
      return nlist;
    }
    PaperSize GetDefaultPrinterPageSize(string pname)
    {
        PaperSize defpapersize = null;
        Monitor.Enter(cacheddefaultpagesizes);
        try
        {
            int index = cacheddefaultpagesizes.IndexOfKey(pname);
            if (index < 0)
            {
                cacheddefaultpagesizes.Add(pname, doc.PrinterSettings.DefaultPageSettings.PaperSize);
            }
            defpapersize = cacheddefaultpagesizes[pname];
      }
      finally
      {
        Monitor.Exit(cacheddefaultpagesizes);
      }

        return defpapersize;

    }
		PaperSize FindPaperSize(int index,int width,int height)
		{
#if REPMAN_DOTNET2
            if (!BrowsePaperSizes)
            {
				if ((System.Environment.OSVersion.Platform == PlatformID.Unix) ||
				      (System.Environment.OSVersion.Platform == PlatformID.MacOSX))
				{
					PaperSize nsize = new PaperSize("A4", 827, 1169);
					return nsize;
				}
				else
				{
	                int rwidth=0, rheight=0;
	                int rawkind=PageIndexToPaperRawKind(index,ref rwidth,ref rheight);
	                if (rawkind >= 0)
	                {
	                    PaperSize psize = new PaperSize();
	                    psize.RawKind = rawkind;
	                    psize.Width = rwidth / 10;
	                    psize.Height = rheight / 10;
	                    return psize;
	                }
				}
            }
#endif

			const int DIFF_MARGIN = 10;

			int newsizex = (int)Math.Round(((double)width) / 1440 * 100);
			int newsizey = (int)Math.Round(((double)height) / 1440 * 100);
			// Check if the current papersize is correct
			PaperSize current;
            if (PrinterSettings.InstalledPrinters.Count == 0)
            {
                current = new PaperSize("A4", 827, 1169);

            }
            else
            {
                try
                {
                    //current = doc.PrinterSettings.DefaultPageSettings.PaperSize;
                    current = GetDefaultPrinterPageSize(doc.PrinterSettings.PrinterName);
                }
                catch
                {
                    current = new PaperSize("A4", 827, 1169);
                }
            }
			//System.Console.WriteLine(current.Width.ToString()+"x"+current.Height.ToString());
			if (Math.Abs(current.Width-newsizex)<=DIFF_MARGIN)
				 if (Math.Abs(current.Height - newsizey) <= DIFF_MARGIN)
				 {
					 return current;
				 }


			int i=0;
			bool found = false;
			string apapername = "User (" + width.ToString() + 'x' +
					height.ToString() + ')';
			PaperSize npapersize=new PaperSize(apapername,newsizex,newsizey);
			// This is really slow on some systems so there is an option to disable it
			if (BrowsePaperSizes)
			{
                List<PaperSize> nlist = GetPaperSizes(doc.PrinterSettings.PrinterName);

				for (i = 0; i < nlist.Count; i++)
				{
					current = nlist[i];
                    int paperkindx = 0;
                    int paperkindy = 0;
                    int rawkind = PageIndexToPaperRawKind(index, ref paperkindx, ref paperkindy);
                    if (rawkind == current.RawKind)
                    {
                        npapersize = current;
                        found = true;
                        break;
                    }
                    int paperWidth = current.Width;
                    int paperHeight = current.Height;
                    if (current.Kind.ToString().IndexOf("Transverse")>=0)
                    {
                        paperWidth = current.Height;
                        paperHeight = current.Width;
                    }
                    if (Math.Abs(paperWidth - newsizex) <= DIFF_MARGIN)
						if (Math.Abs(paperHeight - newsizey) <= DIFF_MARGIN)
						{
							npapersize = current;
							found = true;
							break;
						}
				}

/*				int pcount = doc.PrinterSettings.PaperSizes.Count;
				for (i = 0; i < pcount; i++)
				{
					current = doc.PrinterSettings.PaperSizes[i];
					if (Math.Abs(current.Width - newsizex) <= DIFF_MARGIN)
						if (Math.Abs(current.Height - newsizey) <= DIFF_MARGIN)
						{
							npapersize = current;
							found = true;
							break;
						}
				}*/
			}
			if (!found)
			{
				// We should create a form (AddForm Windows API)
				// with the new size, and ask por papersizes again, selecting the correct one.
				// 
			}
			return npapersize;
		}
        /// <summary>
        /// Prints a MetaFile
        /// </summary>
        /// <param name="meta">MetaFile to print</param>
		override public bool Print(MetaFile meta)
		{
            bool aresult=false;
			WMFOptimization oldoptimize = OptimizeWMF;
			try
			{
                bool olddrawfound = DrawFound;
                try
                {
                    OptimizeWMF = WMFOptimization.None;
                    if (PreparePrint(meta))
                    {
                        try
                        {
                            doc.Print();
                            aresult = true;
                        }
                        finally
                        {
                            doc = null;
                        }
                    }
                }
                finally
                {
                    DrawFound = olddrawfound;
                }
			}
			finally
			{
				OptimizeWMF = oldoptimize;
			}
            return aresult;
		}
        /// <summary>
        /// Select printer specified in the MetaFile
        /// </summary>
        /// <param name="meta">MetaFile, the PrinterSelect property is used to select the printer</param>
		public void InitPrinter(MetaFile meta)
		{
			SelectPrinter(meta.PrinterSelect);
            // Already done
			// doc.DefaultPageSettings.PaperSize = FindPaperSize(meta.CustomX, meta.CustomY);
		}
        public override void EndDocument(MetaFile meta)
        {
            doc.Dispose();
            doc = null;
            base.EndDocument(meta);
        }
        public static bool PrinterExists(string printername)
        {
            bool exists = false;
            foreach (string xname in PrinterSettings.InstalledPrinters)
            {
                if (xname == printername)
                {
                    exists = true;
                    break;
                }
            }
            return exists;
        }
        public override void SelectPrinter(PrinterSelectType PrinterSelect)
        {
            //if ((PrinterSelect != PrinterSelectType.DefaultPrinter) && (ForcePrinterName.Length==0))
            if ((PrinterSelect != PrinterSelectType.DefaultPrinter))
            {
                // Select the printer
                string printername = PrinterConfig.GetPrinterName(PrinterSelect);
                if (!PrinterExists(printername))
                    printername = "";
                if (printername.Length > 0)
                {
                    // Print document must be disposed
                    // Else the printer will not be selected by the printer
                    if (doc != null)
                    {
                        doc.Dispose();
                        doc = null;
                    }
                    if (doc == null)
                        doc = new PrintDocument();
                    doc.PrinterSettings.PrinterName = printername;
                }

            }
            base.SelectPrinter(PrinterSelect);
        }
        public static PaperSize PaperSizeFromPageIndex(int index)
        {
			if ((System.Environment.OSVersion.Platform == PlatformID.Unix) || 
			      (System.Environment.OSVersion.Platform == PlatformID.MacOSX))
			{
				PaperSize nsize = new PaperSize("A4", 827, 1169);
				return nsize;
			}
			
            int rwidth = 0, rheight = 0;
            int rawkind = PageIndexToPaperRawKind(index, ref rwidth, ref rheight);
            if (rawkind >= 0)
            {
                PaperSize psize = new PaperSize();
                psize.RawKind = rawkind;
                psize.Width = rwidth / 10;
                psize.Height = rheight / 10;
                return psize;
            }
            else
                throw new Exception("Page index not found:" + index.ToString());
        }

    }
}
