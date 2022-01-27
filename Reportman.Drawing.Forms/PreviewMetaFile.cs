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
using System.Windows.Forms;
using Reportman.Drawing;
#if REPMAN_DESIGN
using System.ComponentModel;
using System.Windows.Forms.Design;
#endif

namespace Reportman.Drawing.Forms
{
    /// <summary>
    /// Event launched after a page is drawn
    /// </summary>
    /// <param name="prm">PreviewMetaFile containing the page</param>
	public delegate void PageDrawnEvent(PreviewMetaFile prm);
    /// <summary>
    /// Event launched when Page Setup action is activated by the user
    /// </summary>
    /// <returns>Returns true if the user accept page setup changes</returns>
	public delegate bool AlterReportEvent(IWin32Window parent);
    /// <summary>
    /// Event launched to alter mail options before send to mail in preview is done
    /// </summary>
    /// <returns>Returns true if the user accept page setup changes</returns>
    public delegate void SendMailEvent(object sender,SendMailEventArgs args);


    /// <summary>
    /// Windowed control to preview report MetaFiles, it's used by PreviewWinForms.
    /// </summary>
    /// <remarks>
    /// You can use this control to create customized preview windows
    /// </remarks>
#if REPMAN_DESIGN
	[DesignerAttribute(typeof(PreviewMetafileDesigner))]
#endif
#if REPMAN_COMPACT
#else
	[ToolboxBitmapAttribute(typeof(PreviewMetaFile), "previewmetafile.ico")]
#endif
    public class PreviewMetaFile : ScrollableControl
	{
#if REPMAN_COMPACT
		Point MyAutoScrollPosition;
		HScrollBar HBar;
		VScrollBar VBar;
#else
		LayoutEventHandler resizeevent;
#endif
        /// <summary>
        /// Event to launch when the user click page setup icon
        /// </summary>
		public AlterReportEvent OnPageSetup;
        /// <summary>
        /// Event to launch when the user click report parameters icon
        /// </summary>
        public AlterReportEvent OnReportParams;
        /// <summary>
        /// Event to modify mail parameters before sending
        /// </summary>
        public SendMailEvent OnMail;



		const int MAXENTIREPAGES = 128;
		int FBarWidth;
		int FBarHeight;
        bool forcerefresh;
		AutoScaleType FAutoScale;
		float FPreviewScale;
		private MetaFile FMetaFile;
		Bitmap FBitmap;
		Bitmap FIntBitmap;
		int FPage = -1;
        /// <summary>
        /// Internal print driver
        /// </summary>
		protected PrintOutNet prdriver;
		int FPageDrawn, FPagesDrawn;
		float FScaleDrawn;
		int dpix, dpiy;
		int FEntirePageCount;
		int FOldPage;
		private Object[] aparams;
		int pagecolsdrawn, pagerowsdrawn;
		int pagewidthdrawn, pageheightdrawn;
#if REPMAN_COMPACT
#else
		private IAsyncResult resultwork;
#endif
		private int fprogress_records;
		private int fprogress_pagecount;
#if REPMAN_COMPACT
#else
        /// <summary>
        /// Windows Metafile optimization
        /// </summary>
		public WMFOptimization OptimizeWMF;
#endif
        /// <summary>
        /// When the window is processing a report it stores the number of records processed
        /// </summary>
		public int ProgressRecords { get { return fprogress_records; } }
        /// <summary>
        /// When the window is processing a report it stores the number of pages processed
        /// </summary>
        public int ProgressPageCount { get { return fprogress_pagecount; } }
        /// <summary>
        /// When the window is processing a report it stores if the user canceled the report
        /// </summary>
        public bool ProgressCancel;
		private bool FEntireTopDown;
        /// <summary>
        /// Event launched each time a page is drawn
        /// </summary>
		public event PageDrawnEvent OnPageDrawn;
#if REPMAN_COMPACT
		public event EventHandler OnWorkProgress;
#else
        /// <summary>
        /// Event launched while processing the report, you can use it to provide information to the user
        /// </summary>
		public event MetaFileWorkProgress OnWorkProgress;
#endif

		System.Windows.Forms.PictureBox image;
        /// <summary>
        /// MetaFile to process
        /// </summary>
		public MetaFile MetaFile
		{
			get { return FMetaFile; }
			set { SetMetaFile(value); }
		}
        /// <summary>
        /// Force the event OnWorkProgress, used internally while processing a report
        /// </summary>
        /// <param name="records">Current number of records processed</param>
        /// <param name="pagecount">Current number of pages processed</param>
        /// <param name="docancel">Reference variable, then it is set to true the report will be cancelled</param>
		public void WorkProgress(int records, int pagecount, ref bool docancel)
		{
			fprogress_records = records;
			fprogress_pagecount = pagecount;
			ProgressCancel = docancel;
			if (OnWorkProgress != null)
			{
#if REPMAN_COMPACT
            this.Invoke(OnWorkProgress);
//						OnWorkProgress();
						docancel=progress_cancel;
#else
				//
				// TODO: Only call Invoke if the process identifier have been created, place condition
				//
				if (this.InvokeRequired)
				{
					bool dobegin = true;
					if (resultwork != null)
					{
						if (!resultwork.IsCompleted)
						{
							resultwork = null;
							docancel = (bool)aparams[2];
							if (docancel)
								dobegin = false;
						}
					}
					if (dobegin)
					{
						aparams[0] = records;
						aparams[1] = pagecount;
						aparams[2] = docancel;
						resultwork = this.BeginInvoke(OnWorkProgress, aparams);
					}

					//This method should be executed in main thread so
					// Sinchronous execution of WorkProgress is provided
					//                    this.Invoke(OnWorkProgress, aparams);
				}
				else
					if (IsHandleCreated)
						OnWorkProgress(records, pagecount, ref docancel);
#endif

			}
		}
        /// <summary>
        /// Override print driver
        /// </summary>
        /// <param name="newdriver"></param>
        public void SetDriver(PrintOutNet newdriver)
        {
            prdriver = newdriver;
        }
        /// <summary>
        /// Number of pages drawn, when the mode is full page and EntirePageCount is greater than one.
        /// </summary>
		public int PagesDrawn
		{
			get { return FPagesDrawn; }
		}
        /// <summary>
        /// When multiple pages are drawn (EntirePageCount), if you set this property the path to draw them will be
        /// from top to bottom
        /// </summary>
		public bool EntireToDown
		{
			get { return FEntireTopDown; }
			set
			{
				if (FEntireTopDown != value)
				{
					FEntireTopDown = value;
					if (AutoScale == AutoScaleType.EntirePage)
					{
						FPagesDrawn = 0;
						ReDrawPage();
					}
				}
			}
		}
        /// <summary>
        /// Draw multiple pages at a time, on modern high resolution monitors, two vertical standard pages can fit on screen 
        /// at a readable scale
        /// </summary>
		public int EntirePageCount
		{
			get { return FEntirePageCount; }
			set
			{
				if (FEntirePageCount != value)
				{
					FEntirePageCount = value;
					if (FEntirePageCount < 1)
						FEntirePageCount = 1;
					if (FEntirePageCount > MAXENTIREPAGES)
						FEntirePageCount = MAXENTIREPAGES;
					if (FEntirePageCount > 1)
					{
						FMetaFile.RequestPage(FPage + FEntirePageCount - 1);
						int pcount = FEntirePageCount;
						int i;
						for (i = FMetaFile.Pages.CurrentCount - 1; i > FPage; i--)
						{
							pcount--;
						}
						int newpage = FPage;
						for (i = 0; i < pcount - 1; i++)
							newpage--;
						if (newpage < 0)
							newpage = 0;
						FPage = newpage;
					}
					ReDrawPage();
				}
			}
		}
        /// <summary>
        /// Determines how the page is scaled to the window
        /// </summary>
		public AutoScaleType AutoScale
		{
			get { return FAutoScale; }
			set
			{
				if (FAutoScale != value)
				{
					FAutoScale = value;
#if REPMAN_COMPACT
					FreeBars();
#endif
					ReDrawPage();
				}
			}
		}
        /// <summary>
        /// Get or set current page, first page index is 0.
        /// </summary>
		public int Page
		{
			get { return FPage; }
			set
			{
				if (FMetaFile != null)
				{
					int request = value;
					if (AutoScale == AutoScaleType.EntirePage)
						request = value + FEntirePageCount - 1;
					FMetaFile.RequestPage(request);
				}
				FPage = value;
				ReDrawPage();
				FOldPage = -1;
			}
		}
        /// <summary>
        /// Get or set scaling index, setting this property will automatically set also the AutoScale to AutoScaleType.Custom
        /// </summary>
		public float PreviewScale
		{
			get { return FPreviewScale; }
			set
			{
				ChangePreviewScale(value);
				FAutoScale = AutoScaleType.Custom;
				FPageDrawn = -1;
				ReDrawPage();
			}
		}
		private void ChangePreviewScale(float newvalue)
		{
			const float MIN_SCALE = 0.01F;
			const float MAX_SCALE = 10.0F;
			if (newvalue < MIN_SCALE)
				newvalue = MIN_SCALE;
			if (newvalue > MAX_SCALE)
				newvalue = MAX_SCALE;
			FPreviewScale = newvalue;
			prdriver.Scale = FPreviewScale;
#if REPMAN_COMPACT
			FreeBars();
#endif
		}
        /// <summary>
        /// Draws the next page
        /// </summary>
		public void NextPage()
		{
			int increment = 1;
			int request = 1;
			FOldPage = -1;
			if (AutoScale == AutoScaleType.EntirePage)
			{
				increment = FPagesDrawn;
				request = FPage + increment + FEntirePageCount - 1;
			}
			else
				request = FPage + increment;
			if (FMetaFile != null)
			{
				FMetaFile.RequestPage(request);
				if ((FPage + increment) < FMetaFile.Pages.CurrentCount)
				{
					Page = FPage + increment;
				}
				else
				{
					if (FPage != (FMetaFile.Pages.CurrentCount - FPagesDrawn))
						Page = FMetaFile.Pages.CurrentCount - 1;
				}
			}
		}
        /// <summary>
        /// Draws the prior page
        /// </summary>
        public void PriorPage()
		{
			int decrement = 1;
			FOldPage = -1;
			if (AutoScale == AutoScaleType.EntirePage)
			{
				decrement = EntirePageCount;
			}
			if ((FPage - decrement) < 0)
				Page = 0;
			else
				Page = FPage - decrement;
		}
        /// <summary>
        /// Draws the last page
        /// </summary>
        public void LastPage()
		{
			FOldPage = -1;
			FMetaFile.RequestPage(int.MaxValue);
			if (FMetaFile != null)
			{
				int newpage = FMetaFile.Pages.Count;
				int decrement = 1;
				if (AutoScale == AutoScaleType.EntirePage)
				{
					decrement = EntirePageCount;
				}
				if ((newpage - decrement) < 0)
					Page = 0;
				else
					Page = newpage - decrement;
			}
		}
#if REPMAN_COMPACT
#else
		private void MyResizeHandler(object sender, LayoutEventArgs e)
		{
			ReDrawPage();
		}
#endif
		private void MyParentChanged(object sender, EventArgs e)
		{
			ReDrawPage();
#if REPMAN_COMPACT
			Width=Parent.Width;
			Height=Parent.Height;
#endif
		}
		private void MyMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ((pagecolsdrawn <= 0) || (pagerowsdrawn <= 0))
				return;
			if (FAutoScale != AutoScaleType.EntirePage)
			{
				if (FOldPage >= 0)
					FPage = FOldPage;
				AutoScale = AutoScaleType.EntirePage;
				return;
			}
			int offsetx, offsety;
			FOldPage = FPage;
			// Calculate the page number based on x/y position
			int newpagenum = FPage;
			int numrow = e.Y / pageheightdrawn;
			int numcol = e.X / pagewidthdrawn;
			offsetx = (int)((e.X - (numcol * pagewidthdrawn)) / FPreviewScale);
			offsety = (int)((e.Y - (numrow * pageheightdrawn)) / FPreviewScale);
			if (this.FEntireTopDown)
			{
				newpagenum = newpagenum + numcol * pagerowsdrawn + numrow;
			}
			else
			{
				newpagenum = newpagenum + numrow * pagecolsdrawn + numcol;
			}
			if (newpagenum >= FPage + this.PagesDrawn)
				return;
			FPage = newpagenum;
			AutoScale = AutoScaleType.Real;
#if REPMAN_COMPACT
			int posy=offsety-(Parent.Height-FBarHeight)/2;
			int posx=offsetx-(Parent.Width-FBarWidth)/2;
			if (HBar!=null)
			{
				HBar.Value=posx;
				VBar.Value=posy;
			}
#else
			int barheight = 0;
			int barwidth = 0;
			if (HScroll)
				barheight = FBarHeight;
			if (VScroll)
				barwidth = FBarWidth;
			int posx = 0;
			int posy = 0;
			if (VScroll)
			{
				posy = offsety - (Parent.Height - barheight) / 2;
			}
			if (HScroll)
			{
				posx = offsetx - (Parent.Width - barwidth) / 2;
			}
			AutoScrollPosition = new Point(posx, posy);
#endif
		}
		private void DoInit()
		{
#if REPMAN_COMPACT
#else
			OptimizeWMF = WMFOptimization.None;
#endif
			prdriver = new PrintOutNet();
			image = new PictureBox();
			image.MouseDown += new MouseEventHandler(MyMouseDown);
			image.Parent = this;
			FBitmap = new System.Drawing.Bitmap(10, 10);
			prdriver.Output = FBitmap;
			FEntirePageCount = 1;
			aparams = new Object[3];
			this.ParentChanged += new EventHandler(MyParentChanged);
#if REPMAN_COMPACT
			BackColor=System.Drawing.SystemColors.ControlDark;
			MyAutoScrollPosition=new Point(0,0);
			dpix=PrintOutNet.DEFAULT_RESOLUTION;
			dpiy=PrintOutNet.DEFAULT_RESOLUTION;
#else
			Graphics gr = this.CreateGraphics();
			try
			{
				dpix = (int)gr.DpiX;
				dpiy = (int)gr.DpiY;
			}
			finally
			{
				gr.Dispose();
			}
			resizeevent = new LayoutEventHandler(MyResizeHandler);
			Layout += resizeevent;
#endif
			image.Image = FBitmap;
			image.Top = 0;
			image.Left = 0;
			FPreviewScale = 1.0F;
#if REPMAN_COMPACT
			FBarWidth=0;
			FBarHeight=0;
#else
			FBarWidth = System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
			FBarHeight = System.Windows.Forms.SystemInformation.HorizontalScrollBarHeight;
#endif
		}
		#region Generated code by Desginer
		/// <summary>
		/// Needed by Designer
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
        /// <summary>
        /// Constructor required by Windows.Forms
        /// </summary>
		public PreviewMetaFile(System.ComponentModel.IContainer container)
		{
			//
			// Required for the designer in Windows.Forms
			//
#if REPMAN_DESIGN
			container.Add(this);
#endif
			InitializeComponent();

			//
			//
			DoInit();
		}
        /// <summary>
        /// Constructor, initialization of the control
        /// </summary>
		public PreviewMetaFile()
		{
			AutoScroll = true;
			//
			// Required for the designer in Windows.Forms
			//
			InitializeComponent();

			DoInit();
		}
#if REPMAN_COMPACT
		private void DoHBarChange(object sender,EventArgs e)
		{
			image.Left=-HBar.Value;
		}
		private void DoVBarChange(object sender,EventArgs e)
		{
			image.Top=-VBar.Value;
		}
		private void FreeBars()
		{
			if (HBar!=null)
			{
				HBar.Parent=null;
				HBar.Dispose();
				HBar=null;
				VBar.Parent=null;
				VBar.Dispose();
				VBar=null;
				image.Top=0;
				image.Left=0;
			}
		}
#endif
		private void DoResize()
		{
#if REPMAN_COMPACT
			if (Parent==null)
				return;
			if ((HBar==null) && (FAutoScale!=AutoScaleType.EntirePage))
			{
				HBar=new System.Windows.Forms.HScrollBar();
				VBar=new System.Windows.Forms.VScrollBar();
				HBar.Top=Height-HBar.Height;
				HBar.Left=0;
				HBar.Width=Width;
				VBar.Left=Width-VBar.Width;
				VBar.Top=0;
				VBar.Height=Height-HBar.Height;
				FBarWidth=VBar.Width;
				FBarHeight=HBar.Height;
				HBar.Parent=this;
				VBar.Parent=this;
				HBar.BringToFront();
				VBar.BringToFront();
				HBar.ValueChanged+=new EventHandler(DoHBarChange);
				VBar.ValueChanged+=new EventHandler(DoVBarChange);
				HBar.Maximum=image.Width-(Width-VBar.Width);
				VBar.Maximum=image.Height-(Height-HBar.Height);
				HBar.Minimum=0;
				VBar.Minimum=0;
			}
#else
			int hoffset = FBarWidth;
			int voffset = FBarHeight;
			if (!HScroll)
				voffset = 0;
			if (!VScroll)
				hoffset = 0;
			int imLeft, imTop;
			imLeft = 0;
			imTop = 0;
			if (Parent == null)
				return;
			if (image.Width < Parent.ClientSize.Width - hoffset)
			{
				imLeft = (Parent.ClientSize.Width - image.Width - hoffset) / 2;
			}
			if (image.Height < Parent.ClientSize.Height - voffset)
			{
				imTop = (Parent.ClientSize.Height - image.Height - voffset) / 2;
			}
			int newleft = imLeft - Math.Abs(AutoScrollPosition.X);
			int newtop = imTop - Math.Abs(AutoScrollPosition.Y);
			if ((image.Left != newleft) || (image.Top != newtop))
			{
				image.SetBounds(newleft, newtop, image.Width, image.Height);
			}
#endif
		}
        /// <summary>
        /// Internal main procedure performing the draw of the page at selected scale
        /// </summary>
		protected void ReDrawPage()
		{
			if (Parent == null)
				return;
			if (FMetaFile == null)
				return;
#if REPMAN_COMPACT
#else
			SuspendLayout();
            prdriver.Previewing = true;
            try
			{
#endif
				if (FPage >= FMetaFile.Pages.CurrentCount)
				{
					FMetaFile.RequestPage(FPage + FEntirePageCount - 1);
					if (FPage >= FMetaFile.Pages.CurrentCount)
						FPage = FMetaFile.Pages.CurrentCount - 1;
				}
                if (FPage < 0)
                    FPage = 0;
                MetaPage metapage = FMetaFile.Pages[FPage];
				int newwidth, newheight;
				int pagerows = 1, pagecols = 1;
				int intentirepcount = 1;
				SizeF clients = new SizeF((float)(Parent.ClientSize.Width),
					(float)(Parent.ClientSize.Height));
				SizeF asize = new SizeF((float)metapage.PhysicWidth * dpix / 1440,
					(float)metapage.PhysicHeight * dpiy / 1440);
				switch (AutoScale)
				{
					case AutoScaleType.Real:
						ChangePreviewScale(1.0F);
						break;
					case AutoScaleType.EntirePage:
						float scalev, scaleh;
						if (FEntirePageCount > 1)
						{
							FMetaFile.RequestPage(FPage + FEntirePageCount - 1);
							intentirepcount = FMetaFile.Pages.CurrentCount - FPage;
							if (intentirepcount > FEntirePageCount)
								intentirepcount = FEntirePageCount;
							if (intentirepcount < 1)
								intentirepcount = 1;
							// Look for diferent page sizes
							int pagesequal = 1;
							SizeF newsize;
							MetaPage newpage;
							while (pagesequal < intentirepcount)
							{
								newpage = FMetaFile.Pages[FPage + pagesequal];
								newsize = new SizeF((float)newpage.PhysicWidth * dpix / 1440,
									(float)newpage.PhysicHeight * dpiy / 1440);
								if (newsize != asize)
								{
									intentirepcount = pagesequal;
									break;
								}
								pagesequal++;
							}
						}
						if (intentirepcount == 1)
						{
							scalev = clients.Height / asize.Height;
							scaleh = clients.Width / asize.Width;
							if (scalev < scaleh)
								ChangePreviewScale(scalev);
							else
								ChangePreviewScale(scaleh);
						}
						else
						{
							// Determine how many pages will fit horizontal
							int intpagerowsv = 0;
							int intpagecolsv = intentirepcount;
							int pixelwidth;
							do
							{
								intpagerowsv++;
								scalev = (clients.Height / intpagerowsv) / asize.Height;
								pixelwidth = (int)(asize.Width * scalev);
								if (pixelwidth <= 0)
								{
									scalev = 0.0F;
									break;
								}
								intpagecolsv = (int)(clients.Width / pixelwidth);
							} while (((intpagecolsv * intpagerowsv) < intentirepcount) || (intpagecolsv < 0));
							while (((intpagecolsv - 1) * intpagerowsv) >= intentirepcount)
								intpagecolsv--;
							while (intpagerowsv > intentirepcount)
								intpagerowsv--;
							int intpagerowsh = intentirepcount;
							int intpagecolsh = 0;
							int pixelheight;
							do
							{
								intpagecolsh++;
								scaleh = (clients.Width / intpagecolsh) / asize.Width;
								pixelheight = (int)(asize.Height * scaleh);
								if (pixelheight <= 0)
								{
									scaleh = 0.0F;
									break;
								}
								intpagerowsh = (int)(clients.Height / pixelheight);
							} while (((intpagecolsh * intpagerowsh) < intentirepcount) || (intpagerowsh <= 0));
							while (((intpagecolsh) * (intpagerowsh - 1)) >= intentirepcount)
								intpagerowsh--;
							while (intpagecolsh > intentirepcount)
								intpagecolsh--;
							if (scaleh > scalev)
							{
								pagerows = intpagerowsh;
								pagecols = intpagecolsh;
								scalev = scaleh;
							}
							else
							{
								pagerows = intpagerowsv;
								pagecols = intpagecolsv;
							}
							if (pagecols < 0)
								pagecols = 1;
							if (pagerows < 0)
								pagerows = 1;
							ChangePreviewScale(scalev);
						}
						break;
					case AutoScaleType.Wide:
						float scalewide = (float)(clients.Width - FBarWidth) / asize.Width;
						ChangePreviewScale(scalewide);
						break;
					case AutoScaleType.Height:
						float scaleheight = (float)(clients.Height - FBarHeight) / asize.Height;
						ChangePreviewScale(scaleheight);
						break;
				}
				bool dorefresh = false;
				int pwidth = (int)((float)metapage.PhysicWidth * FPreviewScale * dpix / 1440);
				int pheight = (int)((float)metapage.PhysicHeight * FPreviewScale * dpiy / 1440);
				if (pwidth <= 0)
					pwidth = 2;
				if (pheight <= 0)
					pheight = 2;
				pagewidthdrawn = pwidth;
				pageheightdrawn = pheight;
				newwidth = pwidth * pagecols;
				newheight = pheight * pagerows;
				if (newwidth<=0)
					newwidth=1;
				if (newheight<=0)
					newheight=1;
				if ((newheight != FBitmap.Height) || (newwidth != FBitmap.Width))
				{
					dorefresh = true;
                    if (FBitmap != null)
                    {
                        FBitmap.Dispose();
                        FBitmap = null;
                    }
					FBitmap = new Bitmap(newwidth, newheight);
					image.Image = FBitmap;
					image.Width = newwidth;
					image.Height = newheight;
				}
				else
					if ((FPageDrawn != FPage) || (FScaleDrawn != FPreviewScale) || FPagesDrawn != intentirepcount)
						dorefresh = true;
				if ((dorefresh) || (forcerefresh))
				{
					FPagesDrawn = intentirepcount;
					Graphics gr = Graphics.FromImage(FBitmap);
					Pen apen = new Pen(BackColor);
					pagecolsdrawn = pagecols;
					pagerowsdrawn = pagerows;


					if (intentirepcount == 1)
					{
						prdriver.Output = FBitmap;
#if REPMAN_COMPACT
#else
						prdriver.OptimizeWMF = OptimizeWMF;
#endif
						prdriver.DrawPage(FMetaFile, FMetaFile.Pages[FPage]);
						gr.DrawLine(apen, FBitmap.Width - 1, 0, FBitmap.Width - 1, FBitmap.Height - 1);
						gr.DrawLine(apen, 0, FBitmap.Height - 1, FBitmap.Width - 1, FBitmap.Height - 1);
					}
					else
					{
						if (FIntBitmap == null)
						{
							FIntBitmap = new Bitmap(pwidth, pheight);
						}
						else
						{
                            if ((FIntBitmap.Width != pwidth) || (FIntBitmap.Height != pheight))
                            {
                                FIntBitmap.Dispose();
                                FIntBitmap = null;
                                FIntBitmap = new Bitmap(pwidth, pheight);
                            }
						}
						int pdrawn = 0;
						int x;
						int y;
						if (FEntireTopDown)
						{
							for (x = 0; x < pagecols; x++)
								for (y = 0; y < pagerows; y++)
								{
									if (pdrawn < intentirepcount)
									{
										prdriver.Output = FIntBitmap;
#if REPMAN_COMPACT
#else
										prdriver.OptimizeWMF = OptimizeWMF;
#endif
										prdriver.DrawPage(FMetaFile, FMetaFile.Pages[FPage + pdrawn]);
										gr.DrawImage(FIntBitmap, x * pwidth, y * pheight);
										gr.DrawLine(apen, (x + 1) * pwidth - 1, y * pheight - 1, (x + 1) * pwidth - 1, (y + 1) * pheight - 1);
										gr.DrawLine(apen, (x) * pwidth - 1, (y + 1) * pheight - 1, (x + 1) * pwidth - 1, (y + 1) * pheight - 1);
									}
									else
									{
										Brush abrush = new SolidBrush(BackColor);
										gr.FillRectangle(abrush, x * pwidth, y * pheight, (x + 1) * pwidth, (y + 1) * (pheight));
									}
									pdrawn++;
								}
						}
						else
						{
							for (y = 0; y < pagerows; y++)
								for (x = 0; x < pagecols; x++)
								{
									if (pdrawn < intentirepcount)
									{
										prdriver.Output = FIntBitmap;
#if REPMAN_COMPACT
#else
										prdriver.OptimizeWMF = OptimizeWMF;
#endif
										prdriver.DrawPage(FMetaFile, FMetaFile.Pages[FPage + pdrawn]);
										gr.DrawImage(FIntBitmap, x * pwidth, y * pheight);
										gr.DrawLine(apen, (x + 1) * pwidth - 1, y * pheight - 1, (x + 1) * pwidth - 1, (y + 1) * pheight - 1);
										gr.DrawLine(apen, (x) * pwidth - 1, (y + 1) * pheight - 1, (x + 1) * pwidth - 1, (y + 1) * pheight - 1);
									}
									else
									{
										Brush abrush = new SolidBrush(BackColor);
										gr.FillRectangle(abrush, x * pwidth, y * pheight, (x + 1) * pwidth, (y + 1) * (pheight));
									}
									pdrawn++;
								}
						}
					}
					FPageDrawn = FPage;
					FScaleDrawn = FPreviewScale;
					image.Refresh();
				}
				DoResize();
				if (dorefresh)
				{
					if (OnPageDrawn != null)
						OnPageDrawn(this);
				}
#if REPMAN_COMPACT
#else
			}
			finally
			{
                prdriver.Previewing = false;
                ResumeLayout();
			}
#endif
		}
        /// <summary>
        /// Redraws current page
        /// </summary>
        public void RefreshPage()
        {
            forcerefresh = true;
            try
            {
                ReDrawPage();
            }
            finally
            {
                forcerefresh = false;
            }
        }
		private void WorkAsyncError(string message)
		{
			MessageBox.Show("Error", message);
		}
        /// <summary>
        /// Internal procedure initializing data when setting metafile, it draws also the first page
        /// </summary>
		protected void SetMetaFile(MetaFile meta)
		{
			if (meta == null)
			{
				FMetaFile = null;
				return;
			}
            if (meta.Empty)
            {
                FMetaFile = meta;
                return;
            }
			prdriver.NewDocument(meta);
			FScaleDrawn = -1.0F;
			FPageDrawn = -1;
			FOldPage = -1;
			FMetaFile = meta;
			if (FPage < 0)
				FPage = 0;
            FAutoScale = FMetaFile.AutoScale;
			FMetaFile.OnWorkAsyncError += new MetaFileWorkAsyncError(WorkAsyncError);
			FMetaFile.OnWorkProgress += new MetaFileWorkProgress(WorkProgress);

			ReDrawPage();
		}
      protected override void Dispose(bool disposing)
      {
        if (FMetaFile != null)
        {
          FMetaFile.Release();
          FMetaFile = null;
        }
        if (FIntBitmap != null)
        {
            FIntBitmap.Dispose();
            FIntBitmap = null;
        }
        if (FBitmap != null)
        {
            FBitmap.Dispose();
            FBitmap = null;
        }
        base.Dispose(disposing);
      }
	}
    public class SendMailEventArgs
    {
        public string Name;
        public string Filename;
        public string Subject;
        public string From;
        public string To;
        public string Body;
        public byte[] Content;

        public SendMailEventArgs()
        {
            Name = "";
            Filename = "";
            Subject = "";
            From = "";
            To = "";
            Body = "";
        }
    }
#if REPMAN_DESIGN
	public class PreviewMetafileDesigner : ControlDesigner
	{

		protected override void PostFilterProperties(System.Collections.IDictionary
		properties)
		{
			foreach (string prop in unneededProperties)
				properties.Remove(prop);
			base.PostFilterProperties(properties);
		}
		private static readonly string[] unneededProperties = {
			"MetaFile","Finished","PagesDrawn","ProgressPageCount","ProgressRecords","Report"};

	}
#endif
}
