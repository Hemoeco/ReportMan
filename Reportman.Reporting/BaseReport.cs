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
using System.Collections;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Collections.Specialized;
using Reportman.Drawing;
using System.Collections.Generic;

namespace Reportman.Reporting
{
    /// <summary>
    /// Delegate definition for report processing 
    /// </summary>
    /// <param name="Report">Report being processed</param>
    /// <param name="docancel">Setting this variable to true, the report processing will be cancelled</param>
	public delegate void ReportProgress(BaseReport Report, ref bool docancel);
    /// <summary>
    /// Report stream format
    /// </summary>
	public enum StreamFormatType { 
        /// <summary>
        /// Zlib, binary and compressed, only usable in Win32 native reporting engine
        /// </summary>
        Zlib,
        /// <summary>
        /// Text, objext oriented, only usable in Win32 native reporting engine
        /// </summary>
        Text,
        /// <summary>
        /// Binary, only usable in Win32 native reporting engine
        /// </summary>
        Binary,
        /// <summary>
        /// XML Text, usable in Win32 native reporting engine and .Net engine
        /// </summary>
        XML,
        /// <summary>
        /// XML Text, zlib compressed, usable in Win32 native reporting engine and .Net engine
        /// </summary>
        XMLZlib
    };
    /// <summary>
    /// Page size configuration for report scheme
    /// </summary>
	public enum PageSizeType { 
        /// <summary>
        /// The printer configuration will be used to obtain the page size
        /// </summary>
        Default, 
        /// <summary>
        /// A page size will be selected from a list of available page sizes
        /// </summary>
        Custom, 
        /// <summary>
        /// The page size will be specified with fixed height and width. The user executing the
        /// report must have administrator privileges over destination printer to use this setting,
        /// at least the first time the report is executed, because a new form will be created in
        /// the printer configuration
        /// </summary>
        User };
    /// <summary>
    /// Exception class, used by reporting engine when an error in report processing occur
    /// </summary>
	public class ReportException : System.Exception
	{
        /// <summary>
        /// Property name when applicable
        /// </summary>
		public string PropertyName;
        /// <summary>
        /// Report item causing the problem, when applicable
        /// </summary>
		public ReportItem Item;
        /// <summary>
        /// Constructor to launch a report exception
        /// </summary>
        /// <param name="amessage"></param>
        /// <param name="element"></param>
        /// <param name="propname"></param>
		public ReportException(string amessage, ReportItem element, string propname)
			: base(amessage)
		{
			Item = element;
			PropertyName = propname;
		}
	}
    /// <summary>
    /// Base class for Report, defines basic functionallity
    /// </summary>
	abstract public class BaseReport:IDisposable
	{
        /// <summary>
        /// Maximum number of pages a report can handle
        /// </summary>
		public const int MAX_PAGENUM = 100000;
        /// <summary>
        /// A Dataset set can be passed to the engine to replace the datasets 
        /// definitions in the report scheme
        /// </summary>
		protected DatasetAlias FDataAlias;
		private const int MILIS_PROGRESS_DEFAULT = 500;
		// 1 cms=574
		// 0.5 cms=287
		private const int CONS_DEFAULT_GRIDWIDTH = 115;
		private const int CONS_DEFAULT_GRIDCOLOR = 0xFF0000;
		private const int CONS_MIN_GRID_WIDTH = 50;
		// 29,7/2.54*1440
		private const int DEFAULT_PAGEHEIGHT = 16837;
		private const int DEFAULT_PAGEWIDTH = 11906;
		// default Margins
		// Left 1 cm, Right 1 cm, Top 1 cm Bottom 1.5 cm
		private const int DEFAULT_LEFTMARGIN = 574;
		private const int DEFAULT_RIGHTMARGIN = 574;
		private const int DEFAULT_BOTTOMMARGIN = 861;
		private const int DEFAULT_TOPMARGIN = 574;
		private int fintpageindex;
        /// <summary>
        /// Tells the engine if the background thread must be abort
        /// </summary>
		protected bool AbortingThread;
        /// <summary>
        /// Current print driver
        /// </summary>
		protected PrintOut FDriver;
        /// <summary>
        /// Driver por charting
        /// </summary>
        public PrintOut ChartingDriver;
        /// <summary>
        /// Used to check a runtime error in report processing
        /// </summary>
		protected bool ErrorProcessing;
        /// <summary>
        /// Error message when a error in report processing occur
        /// </summary>
		protected string LastErrorProcessing;
		private MetaFile FMetaFile;
        /// <summary>
        /// Report Metafile storing drawing information for the report, 
        /// that is pages and objects inside pages
        /// </summary>
		public MetaFile MetaFile
		{
			get { return FMetaFile; }
			set { FMetaFile = value; }
		}
        /// <summary>
        /// Sections still not drawn, but mandatory to draw before continuing report
        /// processing
        /// </summary>
		protected Sections PendingSections;
        /// <summary>
        /// A list of expression evaluator identifiers, added to handle special report
        /// items like  current page, current language...
        /// </summary>
		protected EvalIdentifiers EvalIdens;
        /// <summary>
        /// List of group headers
        /// </summary>
		protected Sections FGroupHeaders;
        /// <summary>
        /// List of page headers, to be repeated on each page (groups and page headers)
        /// </summary>
		protected Sections GHeaders;
        /// <summary>
        /// List of page footers to be printed in the bottom of the page
        /// </summary>
		protected Sections GFooters;
		private TotalPages FTotalPages;
        /// <summary>
        /// Variable storing the last time the report progress was updated
        /// </summary>
		protected DateTime mmlast;
        /// <summary>
        /// Variable storing the first time the report progress was updated
        /// </summary>
		protected DateTime mmfirst;
        /// <summary>
        /// Determine if the report is composed of multiple reports
        /// </summary>
		protected bool FCompose;

        public bool Compose
        {
            set { FCompose = value; }
            get { return FCompose; }
        }
        /// <summary>
        /// This variable is true if the report have ended and the current page is the last
        /// page
        /// </summary>
		protected bool LastPage;
        /// <summary>
        /// Current section being processed
        /// </summary>
		protected Section section;
        /// <summary>
        /// Current subreport being processed
        /// </summary>
		protected SubReport subreport;
        /// <summary>
        /// True if the report is printing
        /// </summary>
		protected bool printing;
        /// <summary>
        /// Number of records processed by the main dataset in the current subreport
        /// </summary>
		protected int FRecordCount;
        /// <summary>
        /// Number of records processed by the main dataset in the current subreport
        /// </summary>
		public int RecordCount
		{
			get { return FRecordCount; }
		}
        /// <summary>
        /// Current subreport index being processed
        /// </summary>
		public int CurrentSubReportIndex;
        /// <summary>
        /// Current section index being processed
        /// </summary>
        protected int CurrentSectionIndex;
        /// <summary>
        /// Latest section index already measured
        /// </summary>
        protected Point oldprintedsectionext;
        /// <summary>
        /// Latest subreport
        /// </summary>
        protected SubReport oldsubreport;
        /// <summary>
        /// True if there are page footers to print
        /// </summary>
		protected bool havepagefooters;
        /// <summary>
        /// True if the section extension have already evaluated
        /// </summary>
		protected bool sectionextevaluated;
        /// <summary>
        /// Latest printed section
        /// </summary>
		protected Section oldprintedsection;
        /// <summary>
        /// Current horizontal position in the current page
        /// </summary>
        protected int pageposx;
        /// <summary>
        /// Current vertical position in the current page
        /// </summary>
        protected int pageposy;
        /// <summary>
        /// Maximum horizontal space in the current page
        /// </summary>
        protected int pagespacex;
        /// <summary>
        /// Page footers to print at the end of the page
        /// </summary>
		protected Sections pagefooters;
        /// <summary>
        /// Temporary section
        /// </summary>
		protected Section asection;
        /// <summary>
        /// Position to print page footers
        /// </summary>
		protected int pagefooterpos;
        /// <summary>
        /// Currrent section size
        /// </summary>
		protected Point sectionext;
        /// <summary>
        /// Secondary execution thread (while asynchronous execution is enabled)
        /// </summary>
		private Thread FThreadExec;
        /// <summary>
        /// True if the report is still executing in the bacground thread
        /// </summary>
		protected bool FExecuting;
        /// <summary>
        /// Set this property to true if you want to enable asynchronous execution,
        /// that is the first page will be calculated and the other pages will be
        /// calculated in a background thread
        /// </summary>
		public bool AsyncExecution;
        /// <summary>
        /// Priority for the background execution thread, usually lower than the main thread
        /// </summary>
		public ThreadPriority AsyncPriority;
        /// <summary>
        /// Event to track the report progress and cancel the progress if needed
        /// </summary>
		public event ReportProgress OnProgress;
        /// <summary>
        /// Event to track report progress
        /// </summary>
		public event MetaFileWorkProgress OnWorkProgress;
        /// <summary>
        /// Event to track report errors
        /// </summary>
        public event MetaFileWorkAsyncError OnWorkAsyncError;
        /// <summary>
        /// True if the engine have printed some item
        /// </summary>
		public bool PrintedSomething;
        /// <summary>
        /// The print driver used by the engine to calculate or print the report
        /// </summary>
		public PrintOut Driver { get { return FDriver; } }
        /// <summary>
        /// Current page number (zero based index)
        /// </summary>
        public int PageNum;
        /// <summary>
        /// Current page number for current group (zero based index)
        /// </summary>
        public int PageNumGroup;
        /// <summary>
        /// Effective page width for current page
        /// </summary>
		public int InternalPageWidth;
        /// <summary>
        /// Effective page height for current page
        /// </summary>
        public int InternalPageHeight;
        /// <summary>
        /// Current free space in the current page (at the bottom)
        /// </summary>
		public int FreeSpace;
        /// <summary>
        /// List of all components related to the report
        /// </summary>
		public ReportItems Components;
		private int FLanguage;
        /// <summary>
        /// Current language index
        /// </summary>
		public int Language
		{
			set
			{
				FLanguage=value;
				if (Evaluator!=null)
					Evaluator.Language=FLanguage;
			}
			get
			{
				return FLanguage;
			}
		}
        /// <summary>
        /// Windows font name, that is the default font name while designing the report scheme
        /// </summary>		
		public string WFontName;
        /// <summary>
        /// Linux font name, that is the default font name while designing the report scheme
        /// </summary>		
        public string LFontName;
        /// <summary>
        /// Define if the grid should be visible while designing the report
        /// </summary>
		public bool GridVisible;
        /// <summary>
        /// Define if the grid should be painted as horizontal and vertical lines instead of
        /// points
        /// </summary>
        public bool GridLines;
        /// <summary>
        /// Define if the grid is enabled while designing the report, that is, by default
        /// report components will be placed at the nearest grid position
        /// </summary>
        public bool GridEnabled;
        /// <summary>
        /// Define the grid color while designing the report
        /// </summary>
		public int GridColor;
        /// <summary>
        /// Grid width interval in twips
        /// </summary>
		public int GridWidth;
        /// <summary>
        /// Grid height interval in twips
        /// </summary>
        public int GridHeight;
        /// <summary>
        /// Page orientation for the report
        /// </summary>
		public OrientationType PageOrientation;
        /// <summary>
        /// Page size for the report
        /// </summary>
		public PageSizeType PageSize;
        /// <summary>
        /// Detailed page size for the report
        /// </summary>
		public PageSizeDetail PageDetail;
        /// <summary>
        /// Page size index if applicable, when it matchs the Windows GDI page size indexes
        /// </summary>
		public int PageSizeIndex;
        /// <summary>
        /// Page height in twips
        /// </summary>
        public int PageHeight;
        /// <summary>
        /// Page width in twips
        /// </summary>
        public int PageWidth;
        /// <summary>
        /// Page height in twips, if the page size is custom defined
        /// </summary>
        public int CustomPageHeight;
        /// <summary>
        /// Page width in twips, if the page size is custom defined
        /// </summary>
        public int CustomPageWidth;
        /// <summary>
        /// Background color for the pages in preview
        /// </summary>
		public int PageBackColor;
        /// <summary>
        /// Default scale when the preview is executed
        /// </summary>
        public AutoScaleType AutoScale;
        /// <summary>
        /// Default window size when the preview is executed
        /// </summary>
        public PreviewWindowStyleType PreviewWindow;
        /// <summary>
        /// Left margin in twips
        /// </summary>
		public int LeftMargin;
        /// <summary>
        /// Top margin in twips
        /// </summary>
        public int TopMargin;
        /// <summary>
        /// Right margin in twips
        /// </summary>
        public int RightMargin;
        /// <summary>
        /// Bottom margin in twips
        /// </summary>
        public int BottomMargin;
        private PrinterSelectType FPrinterSelect;
        /// <summary>
        /// Printer selection option
        /// </summary>
        public PrinterSelectType PrinterSelect
        {
            get { return FPrinterSelect; }
            set
            {
                FPrinterSelect = value;
                FMetaFile.PrinterSelect = FPrinterSelect;
            }
        }
        /// <summary>
        /// Collection of subreports
        /// </summary>
		public SubReports SubReports;
        /// <summary>
        /// Report parameters collection
        /// </summary>
		public Params Params;
        /// <summary>
        /// Report datasets collection
        /// </summary>
		public DataInfos DataInfo;
        /// <summary>
        /// Report databases collections (available connections)
        /// </summary>
		public DatabaseInfos DatabaseInfo;
        /// <summary>
        /// Number of copies to print by default
        /// </summary>
		public int Copies;
        /// <summary>
        /// Collate copies by default
        /// </summary>
		public bool CollateCopies;
        /// <summary>
        /// Two pass report option, when two pass is enabled the first page of the report 
        /// is not shown (or print) until the full report is calculated, this option allow
        /// the use of PAGECOUNT special expression and other advanced features like skip to
        /// page and position properties in sections.
        /// </summary>
        public bool TwoPass;
		public PrinterFontsType PrinterFonts;
		public bool PrintOnlyIfDataAvailable;
		public StreamFormatType StreamFormat;
		public bool ActionBefore;
		public bool ActionAfter;
		public bool PreviewAbout;
		public PDFFontType Type1Font;
		public short FontSize, FontRotation;
		public int FontStyle;
        public int FontColor;
        public int BackColor;
		public bool Transparent;
        public bool CutText;
		public TextAlignType Alignment;
		public TextAlignVerticalType VAlignment;
		public bool WordWrap;
        public bool SingleLine;
		public bool MultiPage;
		public PrintStepType PrintStep;
		public int PaperSource;
		public int Duplex;
		public string ForcePaperName;
        private short FLinesPerInch;
        public short LinesPerInch
        {
            get
            {
                return FLinesPerInch;
            }
            set
            {
                FLinesPerInch = value;
                FMetaFile.LinesPerInch = FLinesPerInch;
            }
        }
		public bool PreviewMargins;
		public Evaluator Evaluator;
		public OrientationType CurrentOrientation;
		public bool UpdatePageSize;
        /// <summary>
        /// Internal progress procedure
        /// </summary>
        /// <param name="finished">When finished calls an event if assigned</param>
		protected void CheckProgress(bool finished)
		{
			mmlast = System.DateTime.Now;
			TimeSpan difmilis = mmlast - mmfirst;
#if REPMAN_COMPACT
			if ((difmilis.Seconds>=(double)MILIS_PROGRESS_DEFAULT/1000) || finished)
#else
			if ((difmilis.Milliseconds >= MILIS_PROGRESS_DEFAULT) || finished)
#endif
			{
				mmfirst = System.DateTime.Now;
				bool docancel = false;
				if (OnProgress != null)
					OnProgress(this, ref docancel);
				if (!docancel)
				{
					if (OnWorkProgress != null)
						OnWorkProgress(FRecordCount, FMetaFile.Pages.CurrentCount, ref docancel);
				}
				if (docancel)
					throw new UnNamedException(Translator.TranslateStr(503));
			}
		}
		public void AddComponent(ReportItem it)
		{
            if (it.Name.Length > 0)
            {
                if (Components.IndexOfKey(it.Name.ToUpper())>=0)
                {
                    
                }
                else
                    Components.Add(it.Name.ToUpper(), it);
            }
		}
        public void AlignSectionsTo(int linesPerInch)
        {
            double lineHeightDouble = Math.Round((double)Twips.TWIPS_PER_INCH / ((double)linesPerInch));
            double lineasTopMarginDouble = (double)TopMargin / lineHeightDouble;
            int lineasTopMargin = Convert.ToInt32(Math.Round(lineasTopMarginDouble));
            TopMargin = Convert.ToInt32(lineasTopMargin * lineHeightDouble);
            foreach (var subrep in SubReports)
            {
                foreach (var section in subrep.Sections)
                {
                    double lineasSeccionDouble = (double)section.Height / lineHeightDouble;
                    int lineas = Convert.ToInt32(Math.Round(lineasSeccionDouble));
                    int newHeight = Convert.ToInt32(lineas * lineHeightDouble);
                    section.Height = newHeight;
                }
            }
        }
        public void RemoveComponent(ReportItem it)
		{
			int index = Components.IndexOfKey(it.Name);
			if (index >= 0)
				Components.Remove(it.Name);
		}
		private void StopWork()
		{
			if (FExecuting)
				if (FThreadExec != null)
				{
					AbortingThread = true;
#if REPMAN_COMPACT
#else
					FThreadExec.Abort();
#endif
					FExecuting = false;
					FThreadExec = null;
				}
		}
    private void ReleaseMetafile()
    {
      EndPrint();
    }

		protected BaseReport()
		{
            FMetaFile = new MetaFile();
            AsyncPriority = ThreadPriority.Normal;
			FGroupHeaders = new Sections();
			GFooters = new Sections();
			GHeaders = new Sections();
			Components = new ReportItems();
			FTotalPages = new TotalPages();
			LinesPerInch = 600;
			PaperSource = 0;
			Duplex = 0;
			PreviewMargins = false;
			PreviewAbout = true;
			StreamFormat = StreamFormatType.Text;
			Language = -1;
			Copies = 1;
			PageOrientation = OrientationType.Default;
			// Means default pagesize
			PageSize = PageSizeType.Default;
			LeftMargin = DEFAULT_LEFTMARGIN;
			RightMargin = DEFAULT_RIGHTMARGIN;
			BottomMargin = DEFAULT_BOTTOMMARGIN;
			TopMargin = DEFAULT_TOPMARGIN;
			// Means white
			PageBackColor = 0xFFFFFF;
			PageWidth = DEFAULT_PAGEWIDTH;
			PageHeight = DEFAULT_PAGEHEIGHT;
			CustomPageWidth = DEFAULT_PAGEWIDTH;
			CustomPageHeight = DEFAULT_PAGEHEIGHT;
            AutoScale = AutoScaleType.Wide;
			// Def values of grid
			GridVisible = true;
			GridEnabled = true;
			GridColor = CONS_DEFAULT_GRIDCOLOR;
			GridWidth = CONS_DEFAULT_GRIDWIDTH;
			GridHeight = CONS_DEFAULT_GRIDWIDTH;
			// Subreports
			SubReports = new SubReports();
			// Data Info
			DataInfo = new DataInfos();
			DatabaseInfo = new DatabaseInfos();
			Params = new Params();
			// Metafile
			FMetaFile.OnRequestPage += new RequestPageEvent(RequestPage);
      FMetaFile.OnRelease += new MetaStopWork(ReleaseMetafile);
			FMetaFile.OnBeginPrint += new BeginPrintOut(BeginPrint);
			this.OnWorkProgress += new MetaFileWorkProgress(FMetaFile.WorkProgress);
			this.OnWorkAsyncError += new MetaFileWorkAsyncError(FMetaFile.WorkAsyncError);
			FMetaFile.OnStopWork += new MetaStopWork(StopWork);
			FDataAlias = new DatasetAlias();
			// Other
			// Default font
			LFontName = "Helvetica";
			WFontName = "Arial";
			FontSize = 10;
			FontRotation = 0;
			FontStyle = 0;
			FontColor = 0;
			BackColor = 0xFFFFFF;
			Transparent = true;
			CutText = false;
			PendingSections = new Sections();
            EvalIdens = new EvalIdentifiers
            {
                { "PAGE", new IdenReportVar(Evaluator, this, "PAGE") },
                { "PAGINA", new IdenReportVar(Evaluator, this, "PAGE") },
                { "LANGUAGE", new IdenReportVar(Evaluator, this, "LANGUAGE") },
                { "PAGENUM", new IdenReportVar(Evaluator, this, "PAGENUM") },
                { "PAGEWIDTH", new IdenReportVar(Evaluator, this, "PAGEWIDTH") },
                { "PAGEHEIGHT", new IdenReportVar(Evaluator, this, "PAGEHEIGHT") },
                { "FREE_SPACE_CMS", new IdenReportVar(Evaluator, this, "FREE_SPACE_CMS") },
                { "FREE_SPACE_INCH", new IdenReportVar(Evaluator, this, "FREE_SPACE_INCH") },
                { "FREE_SPACE_TWIPS", new IdenReportVar(Evaluator, this, "FREE_SPACE_TWIPS") },
                { "CURRENTGROUP", new IdenReportVar(Evaluator, this, "CURRENTGROUP") },
                { "CURRENTPOSX", new IdenReportVar(Evaluator, this, "CURRENTPOSX") },
                { "CURRENTPOSY", new IdenReportVar(Evaluator, this, "CURRENTPOSY") },
                { "FIRSTSECTION", new IdenReportVar(Evaluator, this, "FIRSTSECTION") },
                { "EOF", new IdenEof(Evaluator, this) },
                { "SETLANGUAGE", new IdenSetLanguage(Evaluator, this) },
                { "REOPEN", new IdenReOpen(Evaluator, this) },
                { "GRAPHICOP", new IdenGraphicOp(Evaluator, this) },
                { "IMAGEOP", new IdenImageOp(Evaluator, this) },
                { "BARCODEOP", new IdenBarcodeOp(Evaluator, this) },
                { "TEXTOP", new IdenTextOp(Evaluator, this) },
                { "TEXTHEIGHT", new IdenTextHeight(Evaluator, this) },
                { "PARAMINFO", new IdenParamInfo(Evaluator, this) },
                { "SETPAGESOURCE", new IdenPageOp(Evaluator, this) },
                { "SETPAGEORIENTATION", new IdenOrientationOp(Evaluator, this) },
                { "GETVALUEFROMSQL", new IdenGetValueFromSQL(Evaluator, this) },
                { "GRAPHICNEW", new IdenGraphicNew(Evaluator, this) },
                { "GRAPHICNEWXY", new IdenGraphicNewXY(Evaluator, this) },
				{ "GRAPHICNEWFUNCTION", new IdenGraphicNewFunction(Evaluator, this) },
				{ "GRAPHICCLEAR", new IdenGraphicClear(Evaluator, this) },
                { "GRAPHICCOLOR", new IdenGraphicColor(Evaluator, this) },
                { "GRAPHICSERIECOLOR", new IdenGraphicSerieColor(Evaluator, this) },
                { "GRAPHICBOUNDS", new IdenGraphicBounds(Evaluator, this) }
            };

            InitEvaluator();
		}
        public void LoadFromCommand(IDbCommand selecommand)
        {
            using (IDataReader reader = selecommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    LoadFromDataReader(reader);
                }
                else
                    throw new Exception("No rows in command");
            }
        }
        public void LoadFromDataReader(IDataReader areader)
        {
            int blength = System.Convert.ToInt32(areader.GetBytes(0, 0, null, 0, 0));
            byte[] bytes = new byte[blength];
            areader.GetBytes(0, 0, bytes, 0, blength);
            MemoryStream mems = new MemoryStream(bytes);
            try
            {
                LoadFromStream(mems);
            }
            finally
            {
#if REPMAN_DOTNET1

#else
                mems.Dispose();               
#endif
            }
        }
		public void LoadFromStream(Stream astream)
		{
			ReportReader areader=new ReportReader(this);
			areader.LoadFromStream(astream);
      ConvertToDotNet();
		}
        /// <summary>
        /// Converts a report to dot net, changing data driver type and parameters in sql sentences
        /// </summary>
        /// <param name="nreport"></param>
        public void ConvertToDotNet()
        {
            int index;
            bool doconvert = false;
            for (index = 0; index < DatabaseInfo.Count; index++)
            {
                if ((DatabaseInfo[index].Driver != (DriverType.DotNet2)) && (DriverType.Mybase!=DatabaseInfo[index].Driver))
                {
                    doconvert = true;
                    break;
                }
            }
            if (!doconvert)
                return;
            ReportReader.ConvertToDotNet((Report)this, DriverType.DotNet2, "");
        }
        public void LoadFromStream(Stream astream, int bufsize)
		{
			ReportReader areader = new ReportReader(this);
			areader.LoadFromStream(astream,bufsize);
		}
		public void LoadFromFile(string filename)
		{
			ReportReader areader=new ReportReader(this);
			areader.LoadFromFile(filename);

		}
        public void SaveToStream(Stream astream)
        {
            ReportWriter areader = new ReportWriter(this);
            areader.SaveToStream(astream);
        }
        public void SaveToFile(string filename)
        {
            ReportWriter areader = new ReportWriter(this);
            areader.SaveToFile(filename);

        }
        public void AddTotalPagesItem(int apageindex, int aobjectindex,
						string adisplayformat)
		{
			TotalPage aobject = new TotalPage();
			aobject.PageIndex = apageindex;
			aobject.ObjectIndex = aobjectindex;
			aobject.DisplayFormat = adisplayformat;
			FTotalPages.Add(aobject);
		}
		public bool RequestPage(int pageindex)
		{
			if (pageindex == int.MaxValue)
				pageindex--;
			if (this.LastPage)
				return true;
			if (this.TwoPass)
			{
				AsyncExecution = false;
				pageindex = int.MaxValue - 1;
			}
			if (pageindex < FMetaFile.Pages.CurrentCount)
				return LastPage;
			if (FExecuting)
			{
				while ((FExecuting) && (FMetaFile.Pages.CurrentCount <= (pageindex)))
#if REPMAN_COMPACT
					Thread.Sleep(100);
#else
					Thread.Sleep(System.TimeSpan.FromMilliseconds(100));
#endif
				return LastPage;
			}
			else
			{
				if (AsyncExecution)
				{
					PrintNextPage();
					fintpageindex = int.MaxValue;
					FExecuting = true;
					try
					{
						FThreadExec = new System.Threading.Thread(new ThreadStart(DoPrintInternal));
						FThreadExec.Priority = AsyncPriority;
						AbortingThread = false;
						FThreadExec.Start();
					}
					catch (Exception)
					{
						FExecuting = false;
						FThreadExec = null;
						throw;
					}
					while ((FExecuting) && (FMetaFile.Pages.CurrentCount <= (pageindex)))
#if REPMAN_COMPACT
						Thread.Sleep(100);
#else
					Thread.Sleep(System.TimeSpan.FromMilliseconds(100));
#endif
				}
				else
				{
					fintpageindex = pageindex;
					DoPrintInternal();
				}
			}
			return LastPage;
		}
		private void DoPrintInternal()
		{
			try
			{
				if (!this.LastPage)
				{
					CheckProgress(false);
					while (!PrintNextPage())
					{
						CheckProgress(false);
						if (PageNum > MAX_PAGENUM)
							throw new UnNamedException("Maximum number of pages reached" +
								MAX_PAGENUM.ToString());
						if (PageNum >= fintpageindex)
							break;
						if (LastPage)
							break;
					}
				}
				FExecuting = false;
				CheckProgress(false);
			}
			catch (Exception E)
			{
				FExecuting = false;
				if ((FThreadExec == null && (!AbortingThread)))
					throw;
				if (!(AbortingThread))
				{
					if (OnWorkAsyncError != null)
						OnWorkAsyncError(E.Message);
					else
						throw;
				}
			}
		}

		protected void FillGlobalHeaders()
		{
			GHeaders.Clear();
			GFooters.Clear();
			for (int i = 0; i < SubReports.Count; i++)
			{
				SubReport subrep = SubReports[i];
				int j = subrep.FirstPageHeader;
				for (int k = 0; k < subrep.PageHeaderCount; k++)
				{
					if (subrep.Sections[j+k].Global)
						GHeaders.Add(subrep.Sections[j+k]);

				}
				j = subrep.FirstPageFooter;
				for (int k = 0; k < subrep.PageFooterCount; k++)
				{
					if (subrep.Sections[j+k].Global)
						GFooters.Add(subrep.Sections[j+k]);

				}
			}
		}
		public void EndPrint()
		{
            DeActivateDatasets();
			section = null;
			subreport = null;
			printing = false;
			FDriver = null;
      if (FMetaFile != null)
      {
        FMetaFile.UpdateTotalPages(FTotalPages);
        FMetaFile.Finish();
      }
      LastPage = true;
		}
		public void InitEvaluator()
		{
			Evaluator = new Evaluator();
			Evaluator.Language = Language;
		}
        public void InitializeParams()
        {
            AddReportItemsToEvaluator(Evaluator);

            for (int i = 0; i < Params.Count; i++)
            {
                if (Params[i].ParamType == ParamType.ExpreB) 
                {
                    string paramname = Params[i].Alias;
                    try
                    {
                        if (Params[i].Value.VarType != VariantType.Null)
                        {
                            string text = "M." + paramname + ":=(" + Params[i].Value.ToString() + ")";
                            Evaluator.EvaluateText(text);
                            Params[i].LastValue = Evaluator.EvaluateText(paramname);
                        }
                    }
                    catch (Exception E)
                    {
                        throw new ReportException(E.Message + "Parameter error:" +
                            Params[i].Alias, Params[i], "Value");
                    }
                }
                else
                    Params[i].LastValue = Params[i].ListValue;
            }
            Evaluator.Language = Language;

        }
        private int NewLanguage(int newlanguage)
		{
			// Setting this property, sets the language for evaluator and parameters
			Language = newlanguage;
			return newlanguage;
		}
		private Variant GetSQLValueOp(string connectionname, string SQL)
		{
			int index = DatabaseInfo.IndexOf(connectionname.ToUpper());
			if (index < 0)
				throw new NamedException("Connection name not found:" + connectionname,connectionname);
			IDataReader adatareader =
				DatabaseInfo[index].GetDataReaderFromSQL(SQL, "",Params, false);
			Variant aresult = new Variant();
			if (adatareader != null)
			{
				if (adatareader.Read())
				{
					aresult.AssignFromObject(adatareader[0]);
				}
			}
			return aresult;
		}
        private bool GraphicClear(string graphicident)
        {
            EvalIdentifier niden = Evaluator.SearchIdentifier(graphicident);
            if (!(niden is VariableGraph))
                throw new Exception("Identifier: "+graphicident +" is not a grafic, calling GraphicClear");
            VariableGraph vgraph = (VariableGraph)niden;
            vgraph.Clear();
            return true;
        }
        private bool GraphicNew(string graphicident, double newvalue, bool change, string valuecaption, string seriecaption)
        {
            EvalIdentifier niden = Evaluator.SearchIdentifier(graphicident);
            if (!(niden is VariableGraph))
                throw new Exception("Identifier: " + graphicident + " is not a grafic, calling GraphicNew");
            VariableGraph vgraph = (VariableGraph)niden;
            vgraph.NewValue(newvalue,change,valuecaption,seriecaption);
            return true;
        }
        private bool GraphicNewXY(string graphicident, double xvalue, double newvalue, bool change, string valuecaption, string seriecaption)
        {
            EvalIdentifier niden = Evaluator.SearchIdentifier(graphicident);
            if (!(niden is VariableGraph))
                throw new Exception("Identifier: " + graphicident + " is not a grafic, calling GraphicNew");
            VariableGraph vgraph = (VariableGraph)niden;
            vgraph.NewValueXY(xvalue,newvalue, change, valuecaption, seriecaption);
            return true;
        }
		private bool GraphicNewFunction(string graphicident, string functionName, string functionParams, string seriecaption)
		{
			EvalIdentifier niden = Evaluator.SearchIdentifier(graphicident);
			if (!(niden is VariableGraph))
				throw new Exception("Identifier: " + graphicident + " is not a grafic, calling GraphicNew");
			VariableGraph vgraph = (VariableGraph)niden;
			vgraph.NewFunction(functionName, functionParams, seriecaption);
			return true;
		}
		private bool GraphicBounds(string graphicident, bool autol, bool autoh, double lvalue, double hvalue, bool logaritmic,
            double logBase,bool inverted)
        {
            EvalIdentifier niden = Evaluator.SearchIdentifier(graphicident);
            if (!(niden is VariableGraph))
                throw new Exception("Identifier: " + graphicident + " is not a grafic, calling GraphicNew");
            VariableGraph vgraph = (VariableGraph)niden;
            vgraph.GraphicBounds(autol,autoh,lvalue,hvalue,logaritmic,logBase,inverted);
            return true;
        }
        private bool GraphicColor(string graphicident,int newcolor)
        {
            EvalIdentifier niden = Evaluator.SearchIdentifier(graphicident);
            if (!(niden is VariableGraph))
                throw new Exception("Identifier: " + graphicident + " is not a grafic, calling GraphicColor");
            VariableGraph vgraph = (VariableGraph)niden;
            vgraph.GraphicColor(newcolor);
            return true;
        }
        private bool GraphicSerieColor(string graphicident, int newcolor)
        {
            EvalIdentifier niden = Evaluator.SearchIdentifier(graphicident);
            if (!(niden is VariableGraph))
                throw new Exception("Identifier: " + graphicident + " is not a grafic, calling GraphicSerieColor");
            VariableGraph vgraph = (VariableGraph)niden;
            vgraph.GraphicSerieColor(newcolor);
            return true;
        }
        private bool GraphicOp(int Left, int Top, int Width,
			int Height, int DrawStyle, int BrushStyle, int BrushColor,
			int PenStyle, int PenWidth, int PenColor)
		{
			MetaObjectDraw obj = new MetaObjectDraw();
			obj.MetaType = MetaObjectType.Draw;
			obj.Top = Top; obj.Left = Left; obj.Height = Height; obj.Width = Width;
			obj.DrawStyle = (ShapeType)DrawStyle; obj.BrushStyle = BrushStyle;
			obj.BrushColor = BrushColor; obj.PenStyle = PenStyle;
			obj.PenWidth = PenWidth; obj.PenColor = PenColor;
			FMetaFile.Pages[FMetaFile.CurrentPage].Objects.Add(obj);
			return true;
		}
		private bool OrientationOp(int orientation)
		{
			if ((orientation < 0) || (orientation > 2))
			{
				throw new UnNamedException("Invalid page orientation");
			}
			CurrentOrientation = (OrientationType)orientation;
			UpdatePageSize = true;
			return true;
		}
		private bool PageOp(int pageIndex, bool custom,
			int customwidth, int customheight, int papersource,
			string forcepapername, int duplex)
		{
			PageDetail.Index = pageIndex;
			PageDetail.Custom = custom;
			PageDetail.CustomHeight = customheight;
			PageDetail.CustomWidth = customwidth;
			PageDetail.PaperSource = papersource;
			PageDetail.ForcePaperName = forcepapername;
			UpdatePageSize = true;
			return true;

		}
		private bool ImageOp(int Left, int Top, int Width,
			int Height, int DrawStyle, int dpires, bool PreviewOnly,
			string Image)
		{
			MemoryStream astream;
			// Search for the image
			bool aresult = false;
			astream = Evaluator.GetStreamFromExpression(Image);
			if (astream != null)
			{
				MetaObjectImage obj = new MetaObjectImage();
				obj.MetaType = MetaObjectType.Image;
				obj.Top = Top; obj.Left = Left; obj.Height = Height; obj.Width = Width;
				obj.DrawImageStyle = (ImageDrawStyleType)DrawStyle; obj.DPIRes = dpires;
				obj.PreviewOnly = PreviewOnly;
				obj.DPIRes = dpires;
				obj.StreamPos = FMetaFile.Pages[FMetaFile.CurrentPage].AddStream(astream, false);
				obj.StreamSize = astream.Length;
				FMetaFile.Pages[FMetaFile.CurrentPage].Objects.Add(obj);
			}
			return aresult;
		}
		private bool BarcodeOp(int Left, int Top, int Width,
			int Height, string Expression, string DisplayFormat,
			int BarType, int Modul, double Ratio, double Rotation,
			bool CalcChecksum, int BrushColor)
		{
			Variant FValue;
			string data;
			BarcodeItem barcode = new BarcodeItem(null);
			barcode.Width = Width;
			barcode.Height = Height;
			barcode.BarType = (BarcodeType)BarType;
			barcode.Modul = Modul;
			barcode.Ratio = Ratio;
			barcode.Rotation = System.Convert.ToInt16(Rotation * 10);
			barcode.Checksum = CalcChecksum;
			FValue = Evaluator.EvaluateText(Expression);
			barcode.CurrentText = FValue.ToString(DisplayFormat, ParamType.Unknown, true);
			data = barcode.CalculateBarcode();
			// Draws Barcode
			barcode.PrintHeight = Height;
			barcode.BColor = BrushColor;
			barcode.DoLines(data, Left, Top, FMetaFile);    // draw the barcode
			return true;
		}
		private bool TextOp(int Left, int Top, int Width,
			int Height, string Text, string LFontName, string WFontName,
			int FontSize, int FontRotation, int FontStyle, int FontColor,
			PDFFontType Type1Font, bool CutText, int Alignment, bool WordWrap,
            bool RightToLeft, PrintStepType PrintStep, int BackColor,
			bool Transparent)
		{
			MetaObjectText obj = new MetaObjectText();
			obj.Left = Left; obj.Top = Top; obj.Height = Height; obj.Width = Width;
			obj.FontSize = (short)FontSize;
			obj.FontRotation = (short)FontRotation;
			obj.FontStyle = (short)FontStyle;
			obj.FontColor = FontColor; obj.RightToLeft = RightToLeft;
			obj.CutText = CutText; obj.Alignment = Alignment;
			obj.WordWrap = WordWrap; obj.PrintStep = PrintStep;
			obj.BackColor = BackColor; obj.Transparent = Transparent;
			obj.Type1Font = Type1Font;
			MetaPage page = FMetaFile.Pages[FMetaFile.CurrentPage];
			obj.TextP = page.AddString(Text);
			obj.TextS = Text.Length;
			obj.LFontNameP = page.AddString(LFontName);
			obj.LFontNameS = LFontName.Length;
			obj.WFontNameP = page.AddString(WFontName);
			obj.WFontNameS = WFontName.Length;
			page.Objects.Add(obj);
			return true;
		}
		private int TextHeightOp(string Text, string LFontName,
			string WFontNamem, int RecWidth, int FontSize, int FontStyle,
			int Type1Font, int PrintStep)
		{
			TextObjectStruct textr;
			System.Drawing.Point extent = new System.Drawing.Point();
			textr.Text = Text;
			textr.LFontName = LFontName;
			textr.WFontName = WFontName;
			textr.FontSize = (short)FontSize;
			textr.FontRotation = 0;
			textr.FontStyle = (short)FontStyle;
			textr.FontColor = 0;
			textr.Type1Font = (PDFFontType)Type1Font;
			textr.CutText = false;
			textr.Alignment = 0;
			textr.WordWrap = true;
			textr.RightToLeft = false;
			textr.PrintStep = (PrintStepType)PrintStep;
			extent.Y = 0;
			extent.X = RecWidth;
			extent = FDriver.TextExtent(textr, extent);
			return extent.Y;
		}
		private Variant ParamInfoOp(string paramName, int index)
		{
			Param param;
			int i, ind;
			Variant aresult = "";
			ind = Params.IndexOf(paramName.ToUpper());
			if (ind < 0)
				throw new NamedException("Parameter not found:" + paramName,paramName);
			param = Params[ind];
			if ((param.ParamType == ParamType.Multiple) ||
             (param.ParamType == ParamType.List) || (param.ParamType == ParamType.SubsExpreList))
			{
				switch (index)
				{
					case 0:
						aresult = param.LastValue;
						break;
					case 1:
						// Parameter value as multiple line string, real values
						if (param.ParamType == ParamType.Multiple)
						{
							for (i = 0; i < param.Selected.Count; i++)
							{
								ind = System.Convert.ToInt32(param.Selected[i]);
								if (param.Values.Count > ind)
								{
									if (aresult.ToString().Length > 0)
										aresult = aresult + "\n" + param.Values[ind];
									else
										aresult = param.Values[ind];
								}
							}
						}
						else
						{
							aresult = param.ListValue;
						}
						break;
					case 2:
						// Parameter value as multiple line string, indexes selected
						if (param.ParamType==ParamType.Multiple)
						{
							for (i=0;i<param.Selected.Count;i++)
							{
						    ind = System.Convert.ToInt32(param.Selected[i]);
								if (aresult.ToString().Length>0)
									aresult=aresult+(char)10+ind.ToString();
								else
									aresult=ind.ToString();
							}
						}
						else
						{
							aresult=param.ListValue.ToString();
						}
						break;
					case 3:
						// Parameter value as multiple line string, user selection
						if (param.ParamType==ParamType.Multiple)
						{
							for (i=0;i<param.Selected.Count;i++)
							{
								ind=System.Convert.ToInt32(param.Selected[i]);
								if (param.Items.Count>ind)
								{
									if (aresult.ToString().Length>0)
										aresult=aresult+(char)10+param.Items[ind];
									else
										aresult=param.Items[ind];
								}
							}
						}
						else
//                            if (param.ParamType==ParamType.List)
//							    aresult=param.Items[param.IndexSelected];
//                            else
                            {
                                aresult = param.LastValue.ToString();
                            }
                        break;
					case 4:
						// Parameter value as multiple line string, possible values as user see
						for (i=0;i<param.Items.Count;i++)
						{
							if (aresult.ToString().Length>0)
								aresult=aresult+(char)10+param.Items[i];
							else
								aresult=param.Items[i];
						}
						break;
					case 5:
						// Parameter value as multiple line string, possible values as real values
						for (i=0;i<param.Values.Count;i++)
						{
							if (aresult.ToString().Length>0)
								aresult=aresult+(char)10+param.Values[i];
							else
								aresult=param.Values[i];
						}
						break;
					case 6:
						// Datasets related to the param
						for (i=0;i<param.Datasets.Count;i++)
						{
							if (aresult.ToString().Length>0)
								aresult=aresult+(char)10+param.Datasets[i];
							else
								aresult=param.Datasets[i];
						}
						break;
					case 7:
						// Description
						aresult=param.Description;
						break;
					case 8:
						// Hint
						aresult=param.Hint;
						break;
					case 9:
						// ErrorMessage
						aresult=param.ErrorMessage;
						break;
					case 10:
						// Validation
						aresult=param.Validation;
						break;
				}
			}
			else
			{
				switch (index)
				{
					case 6:
						// Datasets related to the param
						for (i = 0; i < param.Datasets.Count; i++)
						{
							if (aresult.ToString().Length > 0)
								aresult = aresult + (char)10 + param.Datasets[i];
							else
								aresult = param.Datasets[i];
						}
						break;
					case 7:
						// Description
						aresult = param.Description;
						break;
					case 8:
						// Hint
						aresult = param.Hint;
						break;
					case 9:
						// ErrorMessage
						aresult = param.ErrorMessage;
						break;
					case 10:
						// Validation
						aresult = param.Validation;
						break;
					default:
						aresult = param.LastValue.AsString;
						break;
				}
			}
			return aresult;
		}
		private bool ReOpenOp(string datasetName, string sql)
		{
			DataInfo adata;
			int index, i;
			index = DataInfo.IndexOf(datasetName);
			if (index < 0)
				throw new NamedException("No data info found:" + datasetName,datasetName);
			adata = DataInfo[index];
			adata.DisConnect();
			// Evaluates from evaluator all parameters
			for (i = 0; i < Params.Count; i++)
			{
				Params[i].LastValue = Evaluator.EvaluateText("M." + Params[i].Alias);
			}
			adata.SQLOverride = sql;
			adata.Connect();
			return true;
		}

		public void PrintAll(PrintOut Driver)
		{
			BeginPrint(Driver);
			try
			{
				Driver.NewDocument(FMetaFile);
				try
				{
					RequestPage(int.MaxValue);
				}
				finally
				{
					Driver.EndDocument(FMetaFile);
				}
			}
			finally
			{
				EndPrint();
			}
		}
		public void UpdateParamsBeforeOpen(int index, bool doeval)
		{
			int i, aindex;
			Param aparam;
			string paramname;
			for (i = 0; i < Params.Count; i++)
			{
				aparam = Params[i];
				aindex = aparam.Datasets.IndexOf(DataInfo[index].Alias);
				if (aindex >= 0)
					if (aparam.ParamType == ParamType.ExpreB)
					{
						paramname = aparam.Alias;
						if (aparam.Value.VarType != VariantType.Null)
						{
							if (doeval)
							{
								Evaluator.EvaluateText("M." + paramname + ":=(" + aparam.Value.AsString + ")");
								aparam.LastValue = Evaluator.EvaluateText("M." + paramname);
							}
							else
							{
								aparam.LastValue = Evaluator.EvaluateText(aparam.Value.AsString);
							}
						}
					}
			}
			for (i = 0; i < Params.Count; i++)
			{
				aparam = Params[i];
				aindex = aparam.Datasets.IndexOf(DataInfo[index].Alias);
				if (aindex >= 0)
                    if ((aparam.ParamType == ParamType.SubstExpre) || (aparam.ParamType == ParamType.SubsExpreList))
					{
						paramname = aparam.Alias;
						if (aparam.Value.VarType != VariantType.Null)
						{
							if (doeval)
							{
                                Variant oldvalue = aparam.LastValue;
								Evaluator.EvaluateText("M." + paramname + ":=(" + aparam.Value.AsString + ")");
								aparam.LastValue = oldvalue;
							}
							else
							{
								aparam.LastValue = Evaluator.EvaluateText(aparam.Value.AsString);
							}
						}
					}
			}
		}
		protected void UpdateParamsAfterOpen(int index, bool doeval)
		{
			int i, aindex;
			Param aparam;
			string paramname;
			for (i = 0; i < Params.Count; i++)
			{
				aparam = Params[i];
				aindex = aparam.Datasets.IndexOf(DataInfo[index].Alias);
				if (aindex >= 0)
					if (aparam.ParamType == ParamType.ExpreA)
					{
						paramname = aparam.Alias;
						if (aparam.Value.VarType != VariantType.Null)
						{
							if (doeval)
							{
								Evaluator.EvaluateText("M." + paramname + ":=(" + aparam.Value.AsString + ")");
								aparam.LastValue = Evaluator.EvaluateText("M." + paramname);
							}
							else
							{
								aparam.LastValue = Evaluator.EvaluateText(aparam.Value.AsString);
							}
						}
					}
			}
		}
		protected void PrepareParamsBeforeOpen()
		{
			for (int i = 0; i < DataInfo.Count; i++)
				UpdateParamsBeforeOpen(i, false);
		}
		protected void PrepareParamsAfterOpen()
		{
			for (int i = 0; i < DataInfo.Count; i++)
				UpdateParamsAfterOpen(i, false);
            foreach (Param aparam in this.Params)
            {
                if (aparam.ParamType == ParamType.ExpreA)
                {
                    if (aparam.Datasets.Count == 0)
                    {
                        string paramname = aparam.Alias;
                        if (aparam.Value.VarType != VariantType.Null)
                        {
                            //if (doeval)
                            {
                                Evaluator.EvaluateText("M." + paramname + ":=(" + aparam.Value.AsString + ")");
                                aparam.LastValue = Evaluator.EvaluateText("M." + paramname);
                            }
                            //else
                            //{
                            //    aparam.LastValue = Evaluator.EvaluateText(aparam.Value.AsString);
                            //}
                        }
                    }
                }
            }
        }
		protected void CheckIfDataAvailable()
		{
			if (!PrintOnlyIfDataAvailable)
				return;
			bool dataavail = false;
			for (int i = 0; i < SubReports.Count; i++)
			{
				if (SubReports[i].Alias.Length > 0)
				{
					int index = DataInfo.IndexOf(SubReports[i].Alias);
					if (index >= 0)
					{
						DataInfo dinfo = DataInfo[index];
						if (dinfo.Data.Active)
						{
							if (!dinfo.Data.Eof)
							{
								dataavail = true;
								break;
							}
						}
					}
				}
			}
            if (!dataavail)
            {
                AbortingThread = true;
                MetaFile.Empty = true;
                throw new NoDataToPrintException(Translator.TranslateStr(799));
            }
		
        }
		protected void DeActivateDatasets()
		{
			int i;
			for (i = 0; i < DataInfo.Count; i++)
			{
				DataInfo[i].DisConnect();
                DataInfo[i].DisConnectMem();
			}
			for (i = 0; i < DatabaseInfo.Count; i++)
			{
				DatabaseInfo[i].DisConnect();
			}
		}
        public SortedList<string, string> UsedDataReaders = new SortedList<string, string>();
		protected void ActivateDatasets()
		{
            UsedDataReaders.Clear();

            try
			{
				int i;
				string alias;
				for (i = 0; i < DataInfo.Count; i++)
				{
					DataInfo[i].SQLOverride = "";
				}
				for (i = 0; i < DataInfo.Count; i++)
				{
					DataInfo dinfo = DataInfo[i];
					dinfo.SQLOverride = "";
					if (dinfo.OpenOnStart)
					{
                        if (dinfo.DataReader == null)
                        {
                            UpdateParamsBeforeOpen(i, true);
                            dinfo.Connect();
                        }
					}
				}
				for (i = 0; i < SubReports.Count; i++)
				{
					alias = SubReports[i].Alias;
					if (alias.Length > 0)
					{
						int index = DataInfo.IndexOf(alias);
						if (index < 0)
							throw new NamedException("Data alias not found:" + alias,alias);
					}
				}
			}
			catch
			{
				DeActivateDatasets();
				throw;
			}
		}
		public void CreateNew()
		{
			SubReports.Clear();
            AddSubReport();
		}
        public SubReport AddSubReport()
        {
            SubReport subrep = new SubReport(this);
            GenerateNewName(subrep);
            subrep.AddDetail();
            SubReports.Add(subrep);
            return subrep;
        }
        public void DeleteSubReport(SubReport sub)
        {
            SubReports.Remove(sub);

            for (int i=0;i<sub.Sections.Count;i++)
            {
                Section sec = sub.Sections[i];
                sub.DeleteSection(sec);
            }
            Components.Remove(sub.Name);
            sub.Dispose();
        }
        abstract public bool PrintNextPage();


		virtual public void BeginPrint(PrintOut driver)
		{
            if (!FCompose)
            {
                FTotalPages.Clear();
            }
		}
		class IdenSetLanguage : IdenFunctionReport
		{
			public IdenSetLanguage(Evaluator eval,  BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(1);
				Name = "SETLANGUAGE";
				FModel = "function SetLanguage(newlang:integer):integer";
			}
			protected override Variant GetValue()
			{
				Variant aresult = new Variant();
				if (!Params[0].IsInteger())
				{
					throw new NamedException(Translator.TranslateStr(438), "SETLANGUAGE");
				}
				int avalue = Params[0];
				Report.NewLanguage(avalue);
                
				return aresult;
			}
		}
		abstract class IdenFunctionReport : IdenFunction
		{
			public BaseReport Report;
			public IdenFunctionReport(Evaluator eval, BaseReport rp)
				: base(eval)
			{
				Report = rp;
			}
		}
		class IdenEof : IdenFunctionReport
		{
			public IdenEof(Evaluator eval, BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(1);
				Name = "EOF";
				FModel = "function Eof(alias:string):Boolean";
			}
			override protected Variant GetValue()
			{
				if (Params[0].VarType != VariantType.String)
				{
                    throw new NamedException(Translator.TranslateStr(438), Name);
				}
				bool aresult = true;
				string aliasname = Params[0];
				aliasname = aliasname.ToUpper();
				int index = Report.DataInfo.IndexOf(aliasname);
				if (index < 0)
				{
                    throw new NamedException(Translator.TranslateStr(283) + ":" + aliasname,
						Name);
				}
				if (Report.DataInfo[index].Data.Active)
					aresult = Report.DataInfo[index].Data.Eof;
				return aresult;
			}
		}
		class IdenReportVar : IdenFunctionReport
		{
			public string varname;
			public IdenReportVar(Evaluator eval, BaseReport rp, string aname)
				: base(eval,rp)
			{
				varname = aname;
			}
			override protected Variant GetValue()
			{
				Variant aresult = new Variant();
				switch (varname)
				{
					case "PAGE":
					case "PAGINA":
						aresult = Report.PageNum + 1;
						break;
					case "PAGENUM":
					case "NUMPAGINA":
						aresult = Report.PageNumGroup + 1;
						break;
					case "FREE_SPACE_TWIPS":
						aresult = Report.FreeSpace;
						break;
                    case "CURRENTPOSX":
                        aresult = Report.pageposx;
                        break;
                    case "CURRENTPOSY":
                        aresult = Report.pageposy;
                        break;
                    case "FREE_SPACE_CMS":
						aresult = Twips.TwipsToCms(Report.FreeSpace);
						break;
					case "FREE_SPACE_INCH":
						aresult = Twips.TwipsToInch(Report.FreeSpace);
						break;
					case "CURRENTGROUP":
						SubReport subrep;
						if (Report.CurrentSubReportIndex >= Report.SubReports.Count)
							subrep = Report.SubReports[Report.CurrentSubReportIndex - 1];
						else
							subrep = Report.SubReports[Report.CurrentSubReportIndex];
						aresult = Twips.TwipsToInch(Report.FreeSpace);
						if (subrep.LastRecord)
							aresult = subrep.GroupCount;
						else
							aresult = subrep.CurrentGroupIndex;
						break;
					case "FIRSTSECTION":
						aresult = !Report.PrintedSomething;
						break;
					case "PAGEWIDTH":
						aresult = Report.InternalPageWidth;
						break;
					case "PAGEHEIGHT":
						aresult = Report.InternalPageHeight;
						break;
					case "LANGUAGE":
						aresult = Report.Language;
						break;
				}
				return aresult;
			}
		}
		class IdenOrientationOp : IdenFunctionReport
		{
			public IdenOrientationOp(Evaluator eval,  BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(1);
				Name = "SETPAGEORIENTATION";
				FModel = "function SetPageOrientation(orientation:integer):integer";
			}
			protected override Variant GetValue()
			{
				Variant aresult = new Variant();
				if (!Params[0].IsInteger())
				{
					throw new NamedException(Translator.TranslateStr(438), "SETPAGEORIENTATION");
				}
				int avalue = Params[0];
				Report.OrientationOp(avalue);
				return aresult;
			}
		}
		class IdenGraphicOp : IdenFunctionReport
		{
			public IdenGraphicOp(Evaluator eval,  BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(10);
				Name = "GRAPHICOP";
				FModel = "function GraphicOp(Top,Left,Width,Height:integer;\n" +
					"DrawStyle:integer;BrushStyle:integer;BrushColor:integer;\n" +
					"PenStyle:integer;PenWidth:integer; PenColor:integer):Boolean'";
			}
			protected override Variant GetValue()
			{
				for (int i = 0; i < 4; i++)
				{
					if (!Params[i].IsNumber())
						throw new NamedException(Translator.TranslateStr(438), Name);
				}
				for (int i = 4; i < ParamCount; i++)
				{
					if (!Params[i].IsInteger())
						throw new NamedException(Translator.TranslateStr(438), Name);
				}
				return Report.GraphicOp(System.Convert.ToInt32((double)Params[0]), System.Convert.ToInt32((double)Params[1]),
					System.Convert.ToInt32((double)Params[2]), System.Convert.ToInt32((double)Params[3]), (int)Params[4], (int)Params[5],
					(int)Params[6], (int)Params[7], (int)Params[8], (int)Params[9]);
			}
		}
        class IdenGraphicClear : IdenFunctionReport
        {
            public IdenGraphicClear(Evaluator eval, BaseReport rp)
                : base(eval, rp)
            {
                SetParamCount(1);
                Name = "GRAPHICCLEAR";
                FModel = "function GraphicClear(gr:string):Boolean";
            }
            protected override Variant GetValue()
            {
                if (!Params[0].IsString())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                return Report.GraphicClear(Params[0].ToString());
            }
        }
        class IdenGraphicNew : IdenFunctionReport
        {
            public IdenGraphicNew(Evaluator eval, BaseReport rp)
                : base(eval, rp)
            {
                SetParamCount(5);
                Name = "GRAPHICNEW";
                FModel = "function GraphicClear(gr:string;newvalue:double;changeserie:boolean;valuecaption:string;seriecaption:string):Boolean";
            }
            protected override Variant GetValue()
            {
                if (!Params[0].IsString())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[1].IsNumber())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[2].IsBoolean())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[3].IsString())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[4].IsString())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                return Report.GraphicNew(Params[0].ToString(),Params[1].AsDouble,
                    (bool)Params[2],Params[3].ToString(),Params[4].ToString());
            }
        }

        /*
         * 
         * constructor TIdenGraphicBounds.Create(AOwner:TComponent);
begin
 inherited Create(AOwner);
 FParamcount:=8;
 IdenName:='GraphicBounds';
 Help:=SRpGraphicBounds;
 model:='function '+'GraphicBounds'+'(Gr:string; autol,autoh:boolean; low,high:double;log:boolean; logbase:double; inverted:boolean):Boolean';
 aParams:=SRPPgraphicBounds;
end;


function TIdenGraphicBounds.GeTRpValue:TRpValue;
var
 iden:TRpIdentifier;
begin
 if Not VarIsString(Params[0]) then
   Raise TRpNamedException.Create(SRpEvalType,
         IdenName);
 if Vartype(Params[1])<>varBoolean then
   Raise TRpNamedException.Create(SRpEvalType,
         IdenName);
 if Vartype(Params[2])<>varBoolean then
   Raise TRpNamedException.Create(SRpEvalType,
         IdenName);
 if Not VarIsNumber(Params[3]) then
   Raise TRpNamedException.Create(SRpEvalType,
         IdenName);
 if Not VarIsNumber(Params[4]) then
   Raise TRpNamedException.Create(SRpEvalType,
         IdenName);
 if Vartype(Params[5])<>varBoolean then
   Raise TRpNamedException.Create(SRpEvalType,
         IdenName);
 if Not (VarIsNumber(Params[6])) then
   Raise TRpNamedException.Create(SRpEvalType,
         IdenName);
 if Vartype(Params[7])<>varBoolean then
   Raise TRpNamedException.Create(SRpEvalType,
         IdenName);
 // Buscamos el identificador
 iden:=(evaluator As TRpEvaluator).SearchIdentifier(String(Params[0]));
 if iden=nil then
 begin
   Raise TRpNamedException.Create(SRpIdentifierexpected,
         IdenName+'-'+Params[0]);
 end;
 if Not (iden is TVariableGrap) then
   Raise TRpNamedException.Create(SRpEvalType,
         IdenName+'-'+Params[0]);

 Result:=True;
 (iden As TVariableGrap).OnBounds(Boolean(Params[1]),Boolean(Params[2]),
  double(Params[3]),double(Params[4]),Boolean(Params[5]),double(Params[6]),  Boolean(Params[7]));
end;

         * 
         */

        class IdenGraphicBounds : IdenFunctionReport
        {
            public IdenGraphicBounds(Evaluator eval, BaseReport rp)
                : base(eval, rp)
            {
                SetParamCount(8);
                Name = "GRAPHICBOUNDS";
                FModel = "function (Gr:string; autol,autoh:boolean; low,high:double;log:boolean; logbase:double; inverted:boolean):Boolean";
            }
            protected override Variant GetValue()
            {
                if (!Params[0].IsString())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[1].IsBoolean())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[2].IsBoolean())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[3].IsNumber())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[4].IsNumber())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[5].IsBoolean())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[6].IsNumber())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[7].IsBoolean())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                return Report.GraphicBounds(Params[0].ToString(), (bool)Params[1],
                    (bool)Params[2],Params[3].AsDouble, Params[4].AsDouble, (bool)Params[5],
                    Params[6].AsDouble, (bool)Params[7]);
            }
        }
        class IdenGraphicNewXY : IdenFunctionReport
        {
            public IdenGraphicNewXY(Evaluator eval, BaseReport rp)
                : base(eval, rp)
            {
                SetParamCount(6);
                Name = "GRAPHICNEWXY";
                FModel = "function GraphicClear(gr:string;xvalue,yvalue:double;changeserie:boolean;valuecaption:string;seriecaption:string):Boolean";
            }
            protected override Variant GetValue()
            {
                if (!Params[0].IsString())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[1].IsNumber())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[2].IsNumber())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[3].IsBoolean())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[4].IsString())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[5].IsString())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                return Report.GraphicNewXY(Params[0].ToString(), Params[1].AsDouble,
                    Params[2].AsDouble, (bool)Params[3], Params[4].ToString(), Params[5].ToString());
            }
        }
		class IdenGraphicNewFunction : IdenFunctionReport
		{
			public IdenGraphicNewFunction(Evaluator eval, BaseReport rp)
				: base(eval, rp)
			{
				SetParamCount(4);
				Name = "GRAPHICNEWFUNCTION";
				FModel = "function GraphicnewFunction(gr:string;functionName.string;functionParams:string;seriecaption:string):Boolean";
			}
			protected override Variant GetValue()
			{
				if (!Params[0].IsString())
					throw new NamedException(Translator.TranslateStr(438), Name);
				if (!Params[1].IsString())
					throw new NamedException(Translator.TranslateStr(438), Name);
				if (!Params[2].IsString())
					throw new NamedException(Translator.TranslateStr(438), Name);
				if (!Params[3].IsString())
					throw new NamedException(Translator.TranslateStr(438), Name);
				return Report.GraphicNewFunction(Params[0].ToString(), Params[1].ToString(),
					Params[2].ToString(), Params[3].ToString());
			}
		}
		class IdenGraphicColor : IdenFunctionReport
        {
            public IdenGraphicColor(Evaluator eval, BaseReport rp)
                : base(eval, rp)
            {
                SetParamCount(2);
                Name = "GRAPHICCOLOR";
                FModel = "function GraphicColor(gr:string;Color:integer):Boolean";
            }
            protected override Variant GetValue()
            {
                if (!Params[0].IsString())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[1].IsInteger())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                return Report.GraphicColor(Params[0].ToString(),Params[1].AsInteger);
            }
        }
        class IdenGraphicSerieColor : IdenFunctionReport
        {
            public IdenGraphicSerieColor(Evaluator eval, BaseReport rp)
                : base(eval, rp)
            {
                SetParamCount(2);
                Name = "GRAPHICSERIECOLOR";
                FModel = "function GraphicSerieColor(gr:string;Color:integer):Boolean";
            }
            protected override Variant GetValue()
            {
                if (!Params[0].IsString())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                if (!Params[1].IsInteger())
                    throw new NamedException(Translator.TranslateStr(438), Name);
                return Report.GraphicSerieColor(Params[0].ToString(), Params[1].AsInteger);
            }
        }
        class IdenImageOp : IdenFunctionReport
		{
			public IdenImageOp(Evaluator eval,  BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(8);
				Name = "IMAGEOP";
				FModel = "function ImageOp(Top,Left,Width,Height:integer;\n" +
					"DrawStyle,PixelsPerinch:integer;PreviewOnly:Boolean;\n" +
					"Image:String):Boolean";
			}
			protected override Variant GetValue()
			{
				for (int i = 0; i < 4; i++)
				{
					if (!Params[i].IsNumber())
						throw new NamedException(Translator.TranslateStr(438), "IMAGEOP");
				}
				for (int i = 4; i < 6; i++)
				{
					if (!Params[i].IsInteger())
						throw new NamedException(Translator.TranslateStr(438), "IMAGEOP");
				}
				if (!Params[6].IsBoolean())
					throw new NamedException(Translator.TranslateStr(438), "IMAGEOP");
				if (!Params[7].IsString())
					throw new NamedException(Translator.TranslateStr(438), "IMAGEOP");
				return Report.ImageOp((int)Params[0], (int)Params[1],
					(int)Params[2], (int)Params[3], (int)Params[4], (int)Params[5],
					(bool)Params[6], Params[7].AsString);
			}
		}
		class IdenTextOp : IdenFunctionReport
		{
			public IdenTextOp(Evaluator eval,  BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(19);
				Name = "TEXTOP";
				FModel = "function TextOp(Top,Left,Width,Height:integer;\n" +
					"Text,LFontName,WFontName:WideString;\n" +
					"FontSize,FontRotation,FontStyle,FontColor,Type1Font:integer;\n" +
					"CutText:boolean;Alignment:integer;WordWrap,RightToLeft:Boolean;\n" +
					"PrintStep,BackColor:integer;transparent:boolean)";
			}
			protected override Variant GetValue()
			{
				for (int i = 0; i < ParamCount; i++)
				{
					switch (i)
					{
						case 0:
						case 1:
						case 2:
						case 3:
							if (!Params[i].IsNumber())
								throw new NamedException(Translator.TranslateStr(438), "TEXTOP");
							break;
						case 7:
						case 8:
						case 9:
						case 10:
						case 11:
						case 13:
						case 16:
						case 17:
							if (!Params[i].IsInteger())
								throw new NamedException(Translator.TranslateStr(438), "TEXTOP");
							break;
						case 12:
						case 14:
						case 15:
						case 18:
							if (!Params[i].IsBoolean())
								throw new NamedException(Translator.TranslateStr(438), "TEXTOP");
							break;
						case 4:
						case 5:
						case 6:
							if (!Params[i].IsString())
								throw new NamedException(Translator.TranslateStr(438), "TEXTOP");
							break;
					}
				}
				return Report.TextOp((int)Params[0], (int)Params[1],
					(int)Params[2], (int)Params[3], Params[4].AsString, Params[5].AsString,
					Params[6].AsString, (int)Params[7], (int)Params[8], (int)Params[9],
					(int)Params[10], (PDFFontType)((int)Params[11]), (bool)Params[12],
					(int)Params[13], (bool)Params[14], (bool)Params[15],
					(PrintStepType)((int)Params[16]), (int)Params[17], (bool)Params[18]);
			}
		}
		class IdenBarcodeOp : IdenFunctionReport
		{
			public IdenBarcodeOp(Evaluator eval,  BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(12);
				Name = "BARCODEOP";
				FModel = "function BarcodeOp(Top,Left,Width,Height:integer;\n" +
					"Expression,DisplayFormat:WideString;\n" +
					"BarType,Modul:Integer; Ratio,Rotation:Currency;\n" +
					"CalcChecksum:Boolean; BColor:Integer):Boolean";
			}
			protected override Variant GetValue()
			{
				for (int i = 0; i < ParamCount; i++)
				{
					switch (i)
					{
						case 0:
						case 1:
						case 2:
						case 3:
						case 8:
						case 9:
							if (!Params[i].IsNumber())
								throw new NamedException(Translator.TranslateStr(438), "BARCODEOP");
							break;
						case 6:
						case 7:
						case 11:
							if (!Params[i].IsInteger())
								throw new NamedException(Translator.TranslateStr(438), "BARCODEOP");
							break;
						case 5:
						case 4:
							if (!Params[i].IsString())
								throw new NamedException(Translator.TranslateStr(438), "BARCODEOP");
							break;
						case 10:
							if (!Params[i].IsBoolean())
								throw new NamedException(Translator.TranslateStr(438), "BARCODEOP");
							break;
					}
				}
				return Report.BarcodeOp((int)Params[0], (int)Params[1],
					(int)Params[2], (int)Params[3], Params[4].AsString,
					Params[5].AsString, (int)Params[6], (int)Params[7],
					(double)Params[8], (double)Params[9], (bool)Params[10],
					(int)Params[11]);
			}
		}
		class IdenPageOp : IdenFunctionReport
		{
			public IdenPageOp(Evaluator eval,  BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(7);
				Name = "SETPAGESOURCE";
				FModel = "function SetPageSource(QtIndex:integer;\n" +
					"Custom:Boolean;CustomWidth,CustomHeight,PaperSource:integer;\n" +
					"ForcePaperName:String;Duplex:integer):Boolean";
			}
			protected override Variant GetValue()
			{
				for (int i = 0; i < ParamCount; i++)
				{
					switch (i)
					{
						case 0:
							if (!Params[i].IsNumber())
								throw new NamedException(Translator.TranslateStr(438), "SETPAGESOURCE");
							break;
						case 2:
						case 3:
						case 4:
						case 6:
							if (!Params[i].IsInteger())
								throw new NamedException(Translator.TranslateStr(438), "SETPAGESOURCE");
							break;
						case 5:
							if (!Params[i].IsString())
								throw new NamedException(Translator.TranslateStr(438), "SETPAGESOURCE");
							break;
						case 1:
							if (!Params[i].IsBoolean())
								throw new NamedException(Translator.TranslateStr(438), "SETPAGESOURCE");
							break;
					}
				}
				return Report.PageOp((int)Params[0], (bool)Params[1],
					(int)Params[2], (int)Params[3], (int)Params[4],
					Params[5].AsString, (int)Params[6]);
			}
		}
		class IdenParamInfo : IdenFunctionReport
		{
			public IdenParamInfo(Evaluator eval,  BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(2);
				Name = "PARAMLIST";
				FModel = "function ParamInfo(paramname:String;index:integer)";
			}
			protected override Variant GetValue()
			{
				Variant aresult = new Variant();
				if (!Params[1].IsInteger())
				{
					throw new NamedException(Translator.TranslateStr(438), "PARAMLIST");
				}
				if (!Params[0].IsString())
				{
					throw new NamedException(Translator.TranslateStr(438), "PARAMLIST");
				}
				int avalue = Params[1];
				aresult = Report.ParamInfoOp(Params[0].AsString, avalue);
				return aresult;
			}
		}
		class IdenTextHeight : IdenFunctionReport
		{
			public IdenTextHeight(Evaluator eval,  BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(8);
				Name = "TEXTHEIGHT";
				FModel = "function OnTextheight(Text,LFontName,WFontName:WideString;\n" +
					"RectWidth,FontSize,FontStyle,Type1Font:integer;\n" +
					"PrintStep:integer):integer";
			}
			protected override Variant GetValue()
			{
				for (int i = 0; i < ParamCount; i++)
				{
					switch (i)
					{
						case 3:
						case 4:
						case 5:
						case 6:
						case 7:
							if (!Params[i].IsInteger())
								throw new NamedException(Translator.TranslateStr(438), "TEXTHEIGHT");
							break;
						case 0:
						case 1:
						case 2:
							if (!Params[i].IsString())
								throw new NamedException(Translator.TranslateStr(438), "TEXTHEIGHT");
							break;
					}
				}
				return Report.TextHeightOp(Params[0].AsString, Params[1].AsString,
					Params[2].AsString, (int)Params[3], (int)Params[4],
					(int)Params[5], (int)Params[6], (int)Params[7]);
			}
		}
		class IdenReOpen : IdenFunctionReport
		{
			public IdenReOpen(Evaluator eval,  BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(2);
				Name = "REOPEN";
				FModel = "function ReOpen(dataset, sql:string):Boolean";
			}
			protected override Variant GetValue()
			{
				for (int i = 0; i < ParamCount; i++)
				{
					if (!Params[i].IsString())
						throw new NamedException(Translator.TranslateStr(438), "TEXTHEIGHT");
				}
				return Report.ReOpenOp(Params[0].AsString, Params[1].AsString);
			}
		}
		class IdenGetValueFromSQL : IdenFunctionReport
		{
			public IdenGetValueFromSQL(Evaluator eval,  BaseReport rp)
				: base(eval,rp)
			{
				SetParamCount(2);
				Name = "GETVALUEFROMSQL";
				FModel = "function GetValueFromSQL(connectionname, sql:string):Variant";
			}
			protected override Variant GetValue()
			{
				for (int i = 0; i < ParamCount; i++)
				{
					if (!Params[i].IsString())
						throw new NamedException(Translator.TranslateStr(438), "GETVALUEFROMSQL");
				}
				return Report.GetSQLValueOp(Params[0].AsString, Params[1].AsString);
			}
		}
		public void AddReportItemsToEvaluator(Evaluator eval)
		{
			int i;
			ReportItem acompo;
			for (i = 0; i < Params.Count; i++)
			{
                IdenVariableParam idenp = new IdenVariableParam(eval, Params[i]);
                eval.AddIden("M." + Params[i].Alias, idenp);
			}
			for (i = 0; i < Components.Count; i++)
			{
				acompo = Components[i];
				if (acompo is ExpressionItem)
				{
					ExpressionItem eitem = (ExpressionItem)acompo;
					if (eitem.Identifier.Length > 0)
						eval.AddVariable(eitem.Identifier, eitem.IdenExpression);
				}
				else
				{
					if (acompo is ChartItem)
					{
						ChartItem citem = (ChartItem)acompo;
						if (citem.Identifier.Length > 0)
							eval.AddVariable(citem.Identifier,citem.IdenChart);
					}
				}
			}
			for (i = 0; i < EvalIdens.Count; i++)
			{
				string aname = EvalIdens.Keys[i];
				if (aname != "EOF")
					eval.AddVariable(aname, EvalIdens[i]);
				else
					eval.AddIden(aname, EvalIdens[i]);
			}

		}
		public string CheckParameters()
		{
			string aresult="";
			InitEvaluator();
			AddReportItemsToEvaluator(Evaluator);
			foreach (Param p in Params)
			{
				string validation=p.Validation.Trim();
				string errormessage=p.ErrorMessage;
				if (validation.Length>0)
				{
					Variant evalresult=Evaluator.EvaluateText(validation);
					if (!evalresult.IsNull)
					{
						if (evalresult.IsBoolean())
						{
							if (!evalresult)
							{
								aresult=p.Alias;
								break;
							}
						}
					}
				}
			}
			return aresult;
		}
        /// <summary>
        /// Create a new name for a report item
        /// </summary>
        /// <param name="ritem"></param>
        public void GenerateNewName(ReportItem ritem)
        {
            ritem.Name = FindNewName(ritem);
        }
        /// <summary>
        /// Find  new name for a report item
        /// </summary>
        /// <param name="ritem"></param>
        public string FindNewName(ReportItem ritem)
        {
            string clname = ritem.ClassName;
            int i = 1;
            string nname = clname + i.ToString();
            while (Components.IndexOfKey(nname) >= 0)
            {
                i++;
                nname = clname + i.ToString();
            }
            return nname;
         }
        /// <summary>
        /// Obtain a list of available identifiers that can be used in a expression
        /// inside Evaluator class
        /// </summary>
        /// <returns></returns>
        public Strings GetReportVariables()
        {
            Strings aresult = new Strings();

            int i;
            ReportItem acompo;
            for (i = 0; i < Params.Count; i++)
            {
                aresult.Add("M." + Params[i].Alias);
            }
            for (i = 0; i < Components.Count; i++)
            {
                acompo = Components[i];
                if (acompo is ExpressionItem)
                {
                    ExpressionItem eitem = (ExpressionItem)acompo;
                    if (eitem.Identifier.Length > 0)
                        aresult.Add("M." + eitem.Identifier.ToUpper());
                }
                else
                {
                    if (acompo is ChartItem)
                    {
                        ChartItem citem = (ChartItem)acompo;
                        if (citem.Identifier.Length > 0)
                            aresult.Add("M." + citem.Identifier.ToUpper());
                    }
                }
            }
            for (i = 0; i < EvalIdens.Count; i++)
            {
                string aname = EvalIdens.Keys[i];
                if (aname != "EOF")
                    aresult.Add(aname);
                else
                    aresult.Add(aname);
            }
            return aresult;
        }
        public void Dispose()
        {
          if (MetaFile != null)
          {
            MetaFile.Dispose();
            MetaFile = null;
          }
        }
      }
}