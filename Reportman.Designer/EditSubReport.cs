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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Threading;
using System.IO;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Reportman.Drawing;
using Reportman.Drawing.Forms;
using Reportman.Reporting;

namespace Reportman.Designer
{
  public partial class EditSubReport : UserControl
  {
    List<BandInfo> Bands;
    public SortedList<int, BandInfo> BandsList;
    SolidBrush backbrush;
    int countselection;
//    private static object flag = 2;
    SolidBrush appworkbrush;
    SolidBrush gridbrush;
    SolidBrush selecbrush;
    SolidBrush penbrush;
    SolidBrush titlebrush;
    SolidBrush selectedtitlebrush;
    SubReport FSubReport;
    SortedList<int, BandInfo> BandsToRedraw;
    Report FReport;
    Pen gridpen;
    Pen drawpen;
    Pen drawlabelpen;
    PictureBox ncuadre;
    Bitmap ncuadreimage;
    Point mouseorigin;
    PictureBox qtop;
    PictureBox qbottom;
    PictureBox qleft;
    PictureBox qright;
    RuntimeResize ResizeControl;
    PrintPosItem selectedpositem;
    BandInfo selectedposband;
    public Section SelectedSection;    
    private Bitmap bitselec;
    Graphics grcontrol;
    private bool CapturedSelection;
    private bool CapturedMove;
    private double FDrawScale;
    ImageAttributes natributes80;
    ImageAttributes natributesXOR;
    int MarginResize;
    int BandHeight;
    int BandHeightLast;
    int RulerWidth;
    int TopBandMargin;
    int ResizeHeight;
    int ResizeWidth;
    public CustomPaintControl parentcontrol;
    BandInfo CapturedBand;
    bool RightBandCapture;
    BandInfo PreviousBand;
    public System.Windows.Forms.Timer timerredraw;
    public SortedList<int,PrintItem> SelectedItems;
    public SortedList<int, BandInfo> SelectedItemsBands;
    public SortedList<int, BandInfo> SelectedBands;
    public SelectedItemPalette SelectedPalette;
    public EventHandler AfterInsert;
    public EventHandler AfterSelect;
      public bool UseWindowsMetafiles;
    public double DrawScale
    {
      get
      {
        return FDrawScale;
      }
      set
      {
        SetDrawScale(value);
      }
    }
    const int MAX_SELEC = 1000;
    const int MAX_SELEC_WIDTH = 1;
    const int SQUARE_SELEC_WIDTH = 5;
    const int EmptyBarWidth = 10;
    public SubReport SubReport
    {
        get { return FSubReport; }
    }
    public EditSubReport()
    {
      InitializeComponent();

      UseWindowsMetafiles = true;

      BandsToRedraw = new SortedList<int, BandInfo>();

      timerredraw = new System.Windows.Forms.Timer();
      timerredraw.Interval = 50;
      timerredraw.Tick += new EventHandler(DoRedraw);

      SelectedPalette = SelectedItemPalette.Arrow;
      parentcontrol = new CustomPaintControl();
      parentcontrol.AllowDrop = true;
      parentcontrol.ContextMenuStrip = parentcontextmenu;
      parentcontrol.Parent = panelclient;
      parentcontrol.Paint += new PaintEventHandler(DoPaint);
      parentcontrol.Move += new EventHandler(npicture_Move);
      parentcontrol.DragOver += new DragEventHandler(npicture_DragOver);
      parentcontrol.DragDrop += new DragEventHandler(npicture_DragDrop);
      parentcontrol.MouseDown += new MouseEventHandler(npicture_MouseDown);
      parentcontrol.MouseMove += new MouseEventHandler(npicture_MouseMove);
      parentcontrol.MouseUp += new MouseEventHandler(npicture_MouseUp);

      grcontrol = this.CreateGraphics();

      
      ResizeControl = new RuntimeResize();
      ResizeControl.OnGetBounds += new ResizeEventGetBounds(SelGetBounds);
      ResizeControl.OnNewBounds += new ResizeEventSetBounds(SelSetBounds);

      SelectedItems = new SortedList<int,PrintItem>();
      SelectedBands = new SortedList<int,BandInfo>();
      SelectedItemsBands = new SortedList<int, BandInfo>();
      BandsList = new SortedList<int, BandInfo>();



      penbrush = new SolidBrush(SystemColors.ControlText);

      qright = new PictureBox();
      qleft = new PictureBox();
      qtop = new PictureBox();
      qtop.BorderStyle = BorderStyle.FixedSingle;
      qbottom = new PictureBox();
      qright.Image = new Bitmap(MAX_SELEC_WIDTH, MAX_SELEC);
      qleft.Image = new Bitmap(MAX_SELEC_WIDTH, MAX_SELEC);
      qtop.Image = new Bitmap(MAX_SELEC, MAX_SELEC_WIDTH);
      qbottom.Image = new Bitmap(MAX_SELEC, MAX_SELEC_WIDTH);
      using (Graphics ngr = Graphics.FromImage(qtop.Image))
      {
        ngr.FillRectangle(penbrush,new Rectangle(0,0,MAX_SELEC,MAX_SELEC_WIDTH));  
      }
      using (Graphics ngr = Graphics.FromImage(qbottom.Image))
      {
        ngr.FillRectangle(penbrush,new Rectangle(0,0,MAX_SELEC,MAX_SELEC_WIDTH));  
      }
      using (Graphics ngr = Graphics.FromImage(qleft.Image))
      {
        ngr.FillRectangle(penbrush,new Rectangle(0,0,MAX_SELEC_WIDTH,MAX_SELEC));  
      }
      using (Graphics ngr = Graphics.FromImage(qright.Image))
      {
        ngr.FillRectangle(penbrush, new Rectangle(0, 0, MAX_SELEC_WIDTH, MAX_SELEC));
      }      /*      // Adjust brightness
      ColorMatrix cMatrix = new ColorMatrix(new float[][] {
          new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
          new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
          new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
          new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
          new float[] { adjValueR, adjValueG, adjValueB, 0.0f, 1.0f }
              } );Brightness matrix simply translates colors in each channel by specified values. -1.0f will result in complete darkness (black), 1.0f will result in pure white colors.

      // Adjust saturation
      ColorMatrix cMatrix = new ColorMatrix(new float[][] {
          new float[] { (1.0f-sat)*rweight+sat, 
              (1.0f-sat)*rweight, (1.0f-sat)*rweight, 0.0f, 0.0f },
          new float[] { (1.0f-sat)*gweight, 
              (1.0f-sat)*gweight+sat, (1.0f-sat)*gweight, 0.0f, 0.0f },
          new float[] { (1.0f-sat)*bweight, 
              (1.0f-sat)*bweight, (1.0f-sat)*bweight+sat, 0.0f, 0.0f },
          new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
          new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f }
              } );*/
      float[][] matrixItems ={ 
            new float[] {1, 0, 0, 0, 0},
            new float[] {0, 1, 0, 0, 0},
            new float[] {0, 0, 1, 0, 0},
            new float[] {0, 0, 0, 0.8f, 0}, 
            new float[] {0, 0, 0, 0, 1}};
      ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
      natributes80 = new ImageAttributes();
      natributes80.SetColorMatrix(colorMatrix);

      // Invert
      ColorMatrix cMatrix = new ColorMatrix(new float[][] {
        new float[] {-1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
        new float[] { 0.0f,-1.0f, 0.0f, 0.0f, 0.0f },
        new float[] { 0.0f, 0.0f,-1.0f, 0.0f, 0.0f },
        new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
        new float[] { 1.0f, 1.0f, 1.0f, 0.0f, 1.0f }
        });
      natributesXOR = new ImageAttributes();
      natributesXOR.SetColorMatrix(cMatrix);

      Bands = new List<BandInfo>();
      FDrawScale = 1.0;
      ncuadre = new PictureBox();
      ncuadre.BorderStyle = BorderStyle.FixedSingle;
      backbrush = new SolidBrush(Color.White);
      gridbrush = new SolidBrush(Color.Blue);
      //selecbrush = new SolidBrush(SystemColors.Highlight);
      selecbrush = new SolidBrush(Color.Black);
      appworkbrush = new SolidBrush(SystemColors.AppWorkspace);
      titlebrush = new SolidBrush(SystemColors.ButtonFace);
      selectedtitlebrush = new SolidBrush(SystemColors.AppWorkspace);
      drawpen = new Pen(penbrush);
      drawlabelpen = new Pen(penbrush);
      drawlabelpen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

      bitselec = new Bitmap(SQUARE_SELEC_WIDTH, SQUARE_SELEC_WIDTH);
      using (Graphics grselec = Graphics.FromImage(bitselec))
      {
        grselec.FillRectangle(selecbrush,new Rectangle(0,0,SQUARE_SELEC_WIDTH,SQUARE_SELEC_WIDTH));
      }


      gridpen = new Pen(gridbrush);
      DoubleBuffered = true;

      TopBandMargin = 5;
      ResizeHeight = 5;
      ResizeWidth = 5;
      RulerWidth = 25;
      panellefttop.Width = RulerWidth;
    }
    private void InterrnalDispose()
    {
        qleft.Image.Dispose();
        qleft.Dispose();
        qright.Image.Dispose();
        qright.Dispose();
            qtop.Image.Dispose();
            qtop.Dispose();
            qbottom.Image.Dispose();
            qbottom.Dispose();
            SetSubReport(null,null);
     }
        public void SetSubReport(Report nreport,SubReport nsubreport)
    {
      ClearSelection();

            FSubReport = nsubreport;
      FReport = nreport;
      countselection = 0;
            if (nreport == null)
            {
                if (rulertop != null)
                {
                    rulertop.Dispose();
                    rulertop = null;
                }
                foreach (BandInfo band in Bands)
                {
                    band.Dispose();
                }
                Bands.Clear();
            }
            else
            {
                foreach (Section nsec in nsubreport.Sections)
                {
                    countselection++;
                    nsec.SelectionIndex = countselection;
                    foreach (PrintItem nprin in nsec.Components)
                    {
                        countselection++;
                        nprin.SelectionIndex = countselection;
                    }
                }

                SelectedSection = nsubreport.Sections[0];
                  Redraw();
                  SelectPosItem();
            }
        }
        private void SetDrawScale(double newscale)
    {
      if (newscale == FDrawScale)
        return;
      if (newscale < 0)
        newscale = 0;
      if (newscale > 400)
        newscale = 400;
      FDrawScale = newscale;
      rulertop.RulerScale = newscale;
      ClearSelection();
      Redraw();
      parentcontrol.Invalidate();
    }
    public void ClearSelection()
    {
        if (SelectedItemsBands.Count > 0)
        {
            foreach (BandInfo binfo in SelectedItemsBands.Values)
                ReDrawBand(binfo);
        }
      SelectedItems.Clear();
      SelectedBands.Clear();
      SelectedItemsBands.Clear();
      SelectPosItem();
    }
    public void Redraw()
    {
//      Point oldpos = new Point(npicture.Left, npicture.Top);

//      npicture.SetBounds(0, 0, npicture.Width, npicture.Height);
        BandsToRedraw.Clear();
      int FontHeight = System.Convert.ToInt32(Math.Round(grcontrol.MeasureString("Mg", Font).Height));
      BandHeight = FontHeight + TopBandMargin;
      BandHeightLast = EmptyBarWidth;
      MarginResize = TwipsGraphics.TwipsToPixels(1440 * 7, FDrawScale);

      int MaxHeight = 0;
      int MaxWidth = 0;

      for (int i = 0; i < Bands.Count; i++)
        Bands[i].Dispose();
      Bands.Clear();
      BandsList.Clear();

      BandInfo ninfo;
      Section LastSection=null;
      foreach (Section xsection in FSubReport.Sections)
      {
        if (xsection.SelectionIndex==0)
        {
          countselection++;
          xsection.SelectionIndex = countselection;
        }

        foreach (PrintPosItem positem in xsection.Components)
        {
          if (positem.SelectionIndex == 0)
          {
            countselection++;
            positem.SelectionIndex = countselection;
          }
        }

        ninfo = new BandInfo();
        ninfo.BandPosY = MaxHeight;
        ninfo.BandPosX = RulerWidth;
        ninfo.SubReport = xsection.SubReport;
        ninfo.Section = xsection;
        ninfo.TitleHeight = BandHeight;
        ninfo.Height = TwipsGraphics.TwipsToPixels(xsection.Height,FDrawScale);
        ninfo.Width = TwipsGraphics.TwipsToPixels(xsection.Width, FDrawScale);
        if (ninfo.Width > MaxWidth)
          MaxWidth = ninfo.Width;
        MaxHeight = MaxHeight + BandHeight;
        ninfo.PosY = MaxHeight;
        ninfo.PosX = 0;

        MaxHeight = MaxHeight + ninfo.Height;
        Bands.Add(ninfo);
        BandsList.Add(xsection.SelectionIndex, ninfo);
        LastSection = xsection;
      }
      // Add last band
      ninfo = new BandInfo();
      ninfo.BandPosY = MaxHeight;
      ninfo.BandPosX = RulerWidth;
      ninfo.SubReport = FSubReport;
      ninfo.Section = null;
      ninfo.TitleHeight = BandHeightLast;
      ninfo.Height = 0;
      ninfo.Width = TwipsGraphics.TwipsToPixels(LastSection.Width, FDrawScale);
      if (ninfo.Width > MaxWidth)
        MaxWidth = ninfo.Width;
      MaxHeight = MaxHeight + BandHeightLast;
      ninfo.PosY = MaxHeight;
      ninfo.PosX = 0;

      MaxHeight = MaxHeight + ninfo.Height;
      Bands.Add(ninfo);

      MaxHeight = MaxHeight + ResizeHeight;
      MaxWidth = MaxWidth + ResizeWidth;

      int TotalWidth = MaxWidth + MarginResize;
      int TotalHeight = MaxHeight+MarginResize;

      if (TotalWidth <= 0)
        TotalWidth = 1;
      if (TotalHeight <= 0)
        TotalHeight = 1;

     parentcontrol.SetBounds(parentcontrol.Left, parentcontrol.Top,
           TotalWidth, TotalHeight);

      rulertop.Width = TotalWidth + 200;

      // Draw all titles and sections
      for (int i=0;i<Bands.Count;i++)
      {
        BandInfo nband = Bands[i];
        nband.TotalWidth = TotalWidth;

        ReDrawBand(nband);

      }
//      npicture.SetBounds(oldpos.X, oldpos.Y, TotalWidth, TotalHeight);

      parentcontrol.Invalidate();
    }
    public void ReDrawBand(BandInfo nband)
    {
        if (nband.Section != null)
        {
//            Monitor.Enter(flag);
//            try
            //{
                if (BandsToRedraw.IndexOfKey(nband.Section.SelectionIndex) < 0)
                    BandsToRedraw.Add(nband.Section.SelectionIndex, nband);
            //}
            //finally
            //{
            //    Monitor.Exit(flag);
            //}
        }
    }
    private void ReDrawBandInt(BandInfo nband)
    {
          bool multiselect = false;
          if (SelectedItems.Count > 1)
              multiselect = true;
          bool leftruler = true;
          bool mainbitmap = true;
          if ((nband.Width == 0) && (nband.Height == 0))
          {
              mainbitmap = false;
              leftruler = false;
          }
          else
              if (nband.Height == 0)
              {
                  leftruler = false;
                  mainbitmap = false;
              }
              else
                  if (nband.Width == 0)
                      mainbitmap = false;
          if (nband.Section == null)
              mainbitmap = false;
          if (!mainbitmap)
          {
              if (nband.SectionBitmap != null)
              {
                  nband.SectionBitmap.Dispose();
                  nband.SectionBitmap = null;
              }
          }
          else
          {
              if (nband.SectionBitmap != null)
              {

                  if ((nband.BitmapWidth != nband.Width) || (nband.BitmapHeight != nband.Height))
                  {
                      nband.SectionBitmap.Dispose();
                      nband.SectionBitmap = null;
                  }
              }
              if (nband.SectionBitmap == null)
              {
                  if (UseWindowsMetafiles)
                  {
                      nband.SectionBitmap = GraphicUtils.CreateWindowsMetafile(nband.Width, nband.Height);
                  }
                  else
                  {
                      nband.SectionBitmap = new Bitmap(nband.Width, nband.Height);
                  }
                  nband.BitmapWidth = nband.Width;
                  nband.BitmapHeight = nband.Height;
              }
          }

          if (leftruler)
          {
              if (nband.RulerBitmap != null)
              {
                  if ((nband.RulerBitmapWidth != RulerWidth) || (nband.RulerBitmapHeight != nband.Height))
                  {
                      nband.RulerBitmap.Dispose();
                      nband.RulerBitmap = null;
                  }
              }
              if (nband.RulerBitmap == null)
              {
                  nband.RulerBitmap = new Bitmap(RulerWidth, nband.Height);
                  nband.RulerBitmapWidth = RulerWidth;
                  nband.RulerBitmapHeight = nband.Height;
              }
          }
          else
          {
              if (nband.RulerBitmap != null)
              {
                  nband.RulerBitmap.Dispose();
                  nband.RulerBitmap = null;
              }
          }
          // Draw the section
          if (nband.SectionBitmap != null)
          {              
              Image gbitmap = GraphicUtils.GetImageGrid(nband.Width, nband.Height,
                 FReport.GridWidth, FReport.GridHeight,
                 GraphicUtils.ColorFromInteger(FReport.GridColor), GraphicUtils.ColorFromInteger(FReport.BackColor),
                 FReport.GridLines, FDrawScale);;
              if (nband.SectionBitmap is System.Drawing.Imaging.Metafile)
              {
                  nband.SectionBitmap.Dispose();
                  nband.SectionBitmap = GraphicUtils.CreateWindowsMetafile(nband.Width, nband.Height);
              }
              using (Graphics grsec = Graphics.FromImage(nband.SectionBitmap))
              {
                  if (FReport.GridVisible)
                      grsec.DrawImage(gbitmap, 0, 0, new Rectangle(0, 0, nband.Width, nband.Height), GraphicsUnit.Pixel);
                  else
                      grsec.FillRectangle(new SolidBrush(GraphicUtils.ColorFromInteger(FReport.BackColor)), 
                          new Rectangle(0, 0, nband.Width, nband.Height));
                  // Draw components
                  foreach (PrintPosItem positem in nband.Section.Components)
                  {
                    if (!positem.Hidden)
                    {

                      Rectangle nrec = new Rectangle(TwipsGraphics.TwipsToPixels(positem.PosX, FDrawScale),
                         TwipsGraphics.TwipsToPixels(positem.PosY, FDrawScale), TwipsGraphics.TwipsToPixels(positem.Width, FDrawScale),
                         TwipsGraphics.TwipsToPixels(positem.Height, FDrawScale));
                      positem.SelectionRectangle = nrec;

                      string cname = positem.ClassName;
                      switch (cname)
                      {
                        case "TRPEXPRESSION":
                          DrawExpression(grsec, nrec, (ExpressionItem)positem);
                          grsec.DrawRectangle(drawlabelpen, nrec);
                          break;
                        case "TRPIMAGE":
                          DrawImageObject(grsec, nrec, (ImageItem)positem);
                          grsec.DrawRectangle(drawpen, nrec);
                          break;
                        case "TRPSHAPE":
                          DrawShapeObject(grsec, nrec, (ShapeItem)positem);
                          grsec.DrawRectangle(drawpen, nrec);
                          break;
                        case "TRPLABEL":
                          DrawLabel(grsec, nrec, (LabelItem)positem);
                          grsec.DrawRectangle(drawpen, nrec);
                          break;
                        case "TRPBARCODE":
                          DrawBarCode(grsec, nrec, (BarcodeItem)positem);
                          grsec.DrawRectangle(drawpen, nrec);
                          break;
                        case "TRPCHART":
                                    ChartItem nchartitem = (ChartItem)positem;
                                    ChartDrawHelper helper = (ChartDrawHelper)nchartitem.DrawHelper;
                                    if (helper == null)
                                    {
                                        helper = new ChartDrawHelper(nchartitem);
                                        nchartitem.DrawHelper = helper;
                                    }
                                    
                          DrawChart(grsec, nrec, (ChartItem)positem,helper);
                          grsec.DrawRectangle(drawpen, nrec);
                          break;
                      }
                      int index = SelectedItems.IndexOfKey(positem.SelectionIndex);
                      if (index >= 0)
                      {
                        DrawSelection(grsec, nrec, multiselect);
                      }
                    }
                  }
              }
          }
          if (nband.RulerBitmap != null)
          {
              using (Graphics grrul = Graphics.FromImage(nband.RulerBitmap))
              {
                  // Draw Left ruler
                  grrul.FillRectangle(backbrush, new Rectangle(0, 0, RulerWidth, nband.Height));
                  Ruler.PaintRuler(grrul, Units.Cms, FDrawScale, RulerStyle.Vertical, penbrush, drawpen, Font,
                    new Rectangle(0, 0, RulerWidth, nband.Height), System.Convert.ToInt32(grcontrol.DpiX), System.Convert.ToInt32(grcontrol.DpiY));
                  grrul.DrawRectangle(drawpen, new Rectangle(0, 0, RulerWidth - 1, nband.Height - 1));
              }

          }
    }
    private void DrawSelection(Graphics grsection,Rectangle nrec, bool multiselec)
    {
      if (multiselec)
      {
        // Selection color brush        
        //grsection.DrawImage(bitselec, new Rectangle(nrec.Left, nrec.Top, SQUARE_SELEC_WIDTH, SQUARE_SELEC_WIDTH),
        //  (float)0, (float)0, (float)SQUARE_SELEC_WIDTH, (float)SQUARE_SELEC_WIDTH, GraphicsUnit.Pixel, natributesXOR);

        grsection.DrawImage(bitselec,new Point(nrec.Left,nrec.Top));
//      
//        grsection.DrawImage(bitselec,
        //grsection.FillRectangle(selecbrush, new Rectangle(nrec.Left,nrec.Top,SQUARE_SELEC_WIDTH,SQUARE_SELEC_WIDTH));
      }
    }
    private void DrawBandTitle(BandInfo nband,Graphics egr)
    {
        int position;
        int positiony;
        position = -parentcontrol.Left;
       positiony = -parentcontrol.Top;

      if ((nband.oldposition == -position) && (nband.oldpositiony == -positiony))
        return;
      rulertop.Left = -position;
      //Rectangle rec = new Rectangle(0, nband.BandPosY, nband.TotalWidth-1, BandHeight-1);
      int nbandheight = nband.TitleHeight;
      //Rectangle rec = new Rectangle(0, 0, nband.TotalWidth - 1, nbandheight - 1);
      Rectangle rec = new Rectangle(0, 0, nband.Width+RulerWidth - 1, nbandheight - 1);
            if (nband.TotalWidth == 0)
        nband.TotalWidth = 1;
      if (nband.BandBitmap != null)
      {
        if ((nband.BandBitmap.Width != nband.TotalWidth) || (nband.BandBitmap.Height != nbandheight))
        {
          nband.BandBitmap.Dispose();
          nband.BandBitmap = null;
        }
      }
      if (nband.BandBitmap == null)
        nband.BandBitmap = new Bitmap(nband.TotalWidth, nbandheight);
      bool selected = false;
      if (nband.Section != null)
        if (nband.Section == SelectedSection)
          selected = true;
      using (Graphics ngr = Graphics.FromImage(nband.BandBitmap))
      {
        if (selected)
          ngr.FillRectangle(selectedtitlebrush, rec);
        else
          ngr.FillRectangle(titlebrush, rec);
        ngr.DrawRectangle(drawpen, rec);
        if (nband.Section!=null)
          ngr.DrawString(nband.Section.GetDisplayName(true), Font, penbrush, new PointF(position + RulerWidth, TopBandMargin / 2));
      }

      nband.oldposition = position;
      nband.oldpositiony = positiony;

      if (nband.RightBitmap != null)
      {
        nband.RightBitmap.Dispose();
      }
      int sizeheight = nband.Height;
      
      if (sizeheight <= 0)
        sizeheight = 1;
      nband.RightBitmap = new Bitmap(BandHeightLast, sizeheight);
      rec = new Rectangle(0, 0, BandHeightLast - 1, sizeheight - 1);
      using (Graphics ngr = Graphics.FromImage(nband.RightBitmap))
      {
        ngr.FillRectangle(titlebrush, rec);
        ngr.DrawRectangle(drawpen, rec);
      }



      // Draw the band, the section and ruler

      DrawBandInt(nband,egr);

    }
    private void DrawBandInt(BandInfo nband,Graphics egr)
    {
            egr.DrawImage(nband.BandBitmap, new Point(0, nband.BandPosY));
            egr.DrawImage(nband.RightBitmap, new Point(RulerWidth + nband.Width, nband.PosY));
            if (nband.SectionBitmap != null)
                egr.DrawImage(nband.SectionBitmap, new Point(RulerWidth, nband.PosY));
            if (nband.RulerBitmap != null)
                egr.DrawImage(nband.RulerBitmap, new Point(nband.oldposition, nband.PosY));
    }
    private Rectangle SquareRect(Rectangle arec)
    {
      int Left = arec.Left;
      int Top = arec.Top;
      int Width = arec.Width;
      int Height = arec.Height;
      if (Width > Height)
      {
        Left = Left + (Width - Height) / 2;
        Width = Height;
      }
      else
      {
        Top = Top + (Height - Width) / 2;
        Height = Width;
      }

      return new Rectangle(Left, Top, Width, Height);
    }

    private void RePositionBands(Graphics egr,bool forcedraw)
    {
        int TotalWidth;
        int TotalHeight;
        TotalWidth = parentcontrol.Width;
        TotalHeight = parentcontrol.Height;

        egr.FillRectangle(appworkbrush, new Rectangle(0, 0, TotalWidth, TotalHeight));

      foreach (BandInfo nband in Bands)
      {
          if (forcedraw)
          {
              nband.oldposition = int.MinValue;
              nband.oldpositiony = int.MinValue;
          }
        DrawBandTitle(nband,egr);
      }
        

      //parentcontrol.Invalidate();
    }
    private void npicture_Move(object sender, EventArgs e)
    {
        SelectPosItem();
        parentcontrol.Invalidate();
    }
    private void DrawExpression(Graphics gr, Rectangle nrec, ExpressionItem textitem)
    {
      DrawTextObject(gr, nrec, textitem, textitem.Expression);
    }
    private void DrawBarCode(Graphics gr, Rectangle nrec, BarcodeItem textitem)
    {
        DrawBarCodeObject(gr, nrec, textitem, "BARCODE - " + textitem.Expression);
    }
    private void DrawChart(Graphics gr, Rectangle nrec, ChartItem textitem,ChartDrawHelper dchart)
    {
        DrawChartObject(gr, nrec, textitem, "CHART - " + textitem.ValueExpression,dchart);
    }
    private void DrawLabel(Graphics gr, Rectangle nrec, LabelItem textitem)
    {
      DrawTextObject(gr, nrec, textitem, textitem.Text);
    }
    private void DrawShapeObject(Graphics graph, Rectangle arec, ShapeItem sitem)
    {
        GraphicsState gstate = graph.Save();
        graph.IntersectClip(arec);
      int dpix = System.Convert.ToInt32(graph.DpiX);
      bool drawoutside = true;
      bool drawinside = true;
#if REPMAN_COMPACT
					Pen apen=new Pen(GraphicUtils.ColorFromInteger(objd.PenColor));
                    Brush abrush = new SolidBrush(GraphicUtils.ColorFromInteger(objd.BrushColor));
#else
      Pen apen = new Pen(GraphicUtils.ColorFromInteger(sitem.PenColor), (float)sitem.PenWidth / 1440 * dpix * (float)FDrawScale);
      switch ((int)sitem.PenStyle)
      {
        case 1:
          apen.DashStyle = DashStyle.Dash;
          break;
        case 2:
          apen.DashStyle = DashStyle.Dot;
          break;
        case 3:
          apen.DashStyle = DashStyle.DashDot;
          break;
        case 4:
          apen.DashStyle = DashStyle.DashDotDot;
          break;
        case 5:
          drawoutside = false;
          break;
      }
      HatchStyle hstyle = HatchStyle.SolidDiamond;
      switch (sitem.BrushStyle)
      {
        case BrushType.Clear:
          drawinside = false;
          break;
        case BrushType.Horizontal:
          hstyle = HatchStyle.Horizontal;
          break;
        case BrushType.Vertical:
          hstyle = HatchStyle.Vertical;
          break;
        case BrushType.ADiagonal:
          hstyle = HatchStyle.LightUpwardDiagonal;
          break;
        case BrushType.BDiagonal:
          hstyle = HatchStyle.LightDownwardDiagonal;
          break;
        case BrushType.ACross:
          hstyle = HatchStyle.Cross;
          break;
        case BrushType.BCross:
          hstyle = HatchStyle.DiagonalCross;
          break;
        case BrushType.Dense1:
          hstyle = HatchStyle.Percent10;
          break;
        case BrushType.Dense2:
          hstyle = HatchStyle.Percent20;
          break;
        case BrushType.Dense3:
          hstyle = HatchStyle.Percent20;
          break;
        case BrushType.Dense4:
          hstyle = HatchStyle.Percent40;
          break;
        case BrushType.Dense5:
          hstyle = HatchStyle.Percent50;
          break;
        case BrushType.Dense6:
          hstyle = HatchStyle.Percent60;
          break;
        case BrushType.Dense7:
          hstyle = HatchStyle.Percent70;
          break;
      }
      Brush abrush;
      if (hstyle == HatchStyle.SolidDiamond)
        abrush = new SolidBrush(GraphicUtils.ColorFromInteger(sitem.BrushColor));
      else
        abrush = new HatchBrush(hstyle, GraphicUtils.ColorFromInteger(sitem.BrushColor), Color.Empty);
#endif

      ShapeType shape = sitem.Shape;
      if ((shape == ShapeType.Square) || (shape == ShapeType.RoundSquare)
        || (shape == ShapeType.Circle))
        arec = SquareRect(arec);
      switch (shape)
      {
        case ShapeType.Rectangle:
        case ShapeType.Square:
          if (drawinside)
            graph.FillRectangle(abrush, arec);
          if (drawoutside)
            graph.DrawRectangle(apen, arec.Left, arec.Top, arec.Width, arec.Height);
          break;
        case ShapeType.RoundRect:
        case ShapeType.RoundSquare:
          // Rounded rectangles not implemeted for now
          if (drawinside)
            graph.FillRectangle(abrush, arec);
          if (drawoutside)
            graph.DrawRectangle(apen, arec.Left, arec.Top, arec.Width, arec.Height);
          break;
        case ShapeType.Ellipse:
        case ShapeType.Circle:
          if (drawinside)
            graph.FillEllipse(abrush, arec);
          if (drawoutside)
            graph.DrawEllipse(apen, arec.Left, arec.Top, arec.Width, arec.Height);
          break;
        case ShapeType.HorzLine:
          if (drawoutside)
            graph.DrawLine(apen, arec.Left, arec.Top, arec.Left + arec.Width, arec.Top);
          break;
        case ShapeType.VertLine:
          if (drawoutside)
            graph.DrawLine(apen, arec.Left, arec.Top, arec.Left, arec.Top + arec.Height);
          break;
        case ShapeType.Oblique1:
          if (drawoutside)
            graph.DrawLine(apen, arec.Left, arec.Top, arec.Left + arec.Width, arec.Top + arec.Height);
          break;
        case ShapeType.Oblique2:
          if (drawoutside)
            graph.DrawLine(apen, arec.Left, arec.Top + arec.Height, arec.Left + arec.Width, arec.Top);
          break;

      }
        graph.Restore(gstate);
    }
    private void DrawImageObject(Graphics gr, Rectangle arec, ImageItem imaitem)
    {
      MemoryStream nstream = imaitem.GetMemoryStream();
      Graphics graph = gr;
      string amessage = "";
      if (nstream!=null)
      {
        nstream.Seek(0, SeekOrigin.Begin);
        try
        {
          System.Drawing.Image abit = Image.FromStream(imaitem.Stream);
          ImageDrawStyleType dstyle = imaitem.DrawStyle;
          int dpix = System.Convert.ToInt32(gr.DpiX);
          int dpiy = System.Convert.ToInt32(gr.DpiY);
          float dpires = imaitem.dpires;
          float bitwidth = abit.Width;
          float bitheight = abit.Height;
          Rectangle srcrec = new Rectangle(0, 0, (int)Math.Round(bitwidth), (int)Math.Round(bitheight));
          Rectangle destrec = arec;
          switch (dstyle)
          {
            case ImageDrawStyleType.Crop:
                double propx = (double)destrec.Width / bitwidth;
                double propy = (double)destrec.Height / bitheight;
                int H = 0;
                int W = 0;
                if (propy > propx)
                {
                    H = Convert.ToInt32(Math.Round(destrec.Height * propx / propy));
                    destrec = new Rectangle(destrec.Left, Convert.ToInt32(destrec.Top + (destrec.Height - H) / 2), destrec.Width, H);
                }
                else
                {
                    W = Convert.ToInt32(destrec.Width * propy / propx);
                    destrec = new Rectangle(destrec.Left + (destrec.Width - W) / 2, destrec.Top, W, destrec.Height);
                }
                graph.DrawImage(abit, destrec, srcrec, GraphicsUnit.Pixel);
                break;
            case ImageDrawStyleType.Full:
              destrec = new Rectangle(arec.Left, arec.Top, (int)Math.Round((float)abit.Width / dpires * dpix * FDrawScale), (int)Math.Round((float)abit.Height / dpires * dpiy * FDrawScale));
              graph.DrawImage(abit, destrec, srcrec, GraphicsUnit.Pixel);
              break;
            case ImageDrawStyleType.Stretch:
              graph.DrawImage(abit, destrec, srcrec, GraphicsUnit.Pixel);
              break;
  #if REPMAN_COMPACT
						    case ImageDrawStyleType.Tile:
							    break;
						    case ImageDrawStyleType.Tiledpi:
							    // Pending, scale image to adjust dpi brush
							    break;
  #else
            case ImageDrawStyleType.Tile:
              TextureBrush br2 = new TextureBrush(abit);
              graph.FillRectangle(br2, destrec);
              break;
            case ImageDrawStyleType.Tiledpi:
              // Pending, scale image to adjust dpi brush
              TextureBrush br = new TextureBrush(abit, srcrec);
              graph.FillRectangle(br, destrec);
              break;
  #endif
          }
        }
        catch(Exception E)
        {
          amessage = E.Message;
        }
      }
      if (amessage.Length == 0)
        if (imaitem.Expression.Length > 0)
          amessage = imaitem.Expression;
      if (amessage.Length>0)
      {
        using (Font font = new Font(this.Font.FontFamily, 10.0F * (float)FDrawScale, FontStyle.Regular))
        {
          using (SolidBrush stock_brush = new SolidBrush(ForeColor))
          {
            gr.DrawString(amessage, Font, stock_brush, new PointF(arec.Left, arec.Top));
          }
        }
      }

    }
    private void DrawTextObject(Graphics gr, Rectangle nrec, PrintItemText textitem, string ntext)
    {
      using (Font font = new Font(textitem.WFontName, (float)textitem.FontSize * (float)FDrawScale, GraphicUtils.FontStyleFromInteger(textitem.FontStyle)))
      {
        Color BackColor = GraphicUtils.ColorFromInteger(textitem.BackColor);
        Color FontColor = GraphicUtils.ColorFromInteger(textitem.FontColor);
        // Change colors if drawselecte
        bool drawbackground = (!textitem.Transparent);
        using (SolidBrush stock_brush = new SolidBrush(FontColor))
        {
          // Implement text rotation here
          // by using Transform matrix for Graphics
    #if REPMAN_COMPACT
					    graph.DrawString(atext,font,stock_brush,arec);
    #else
          //					graph.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
          if (drawbackground)
          {
            using (SolidBrush stock_backbrush= new SolidBrush(BackColor))
            {
              gr.FillRectangle(stock_backbrush, nrec);
            }
          }
          int aalign = textitem.PrintAlignment | textitem.VPrintAlignment;
          if (textitem.SingleLine)
            aalign = aalign | MetaFile.AlignmentFlags_SingleLine;

          // Text justify is implemented separaterly
          StringFormat nformat = PrintOutNet.IntAlignToStringFormat(aalign,textitem.CutText,textitem.WordWrap,textitem.RightToLeft);
          nformat.FormatFlags = nformat.FormatFlags & (~StringFormatFlags.NoClip);
          gr.DrawString(ntext, font, stock_brush, new RectangleF(nrec.Left, nrec.Top, nrec.Width, nrec.Height), nformat);
    #endif
        }
      }
    }
    private void DrawBarCodeObject(Graphics gr, Rectangle nrec, BarcodeItem textitem, string ntext)
    {
        using (Font font = new Font("Arial", (float)10 * (float)FDrawScale,FontStyle.Regular ))
        {
            Color BackColor = GraphicUtils.ColorFromInteger(FReport.BackColor);
            Color FontColor = GraphicUtils.ColorFromInteger(textitem.BColor);
            // Change colors if drawselecte
            bool drawbackground = false;
            using (SolidBrush stock_brush = new SolidBrush(FontColor))
            {
                // Implement text rotation here
                // by using Transform matrix for Graphics
#if REPMAN_COMPACT
					    graph.DrawString(atext,font,stock_brush,arec);
#else
                //					graph.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
                if (drawbackground)
                {
                    using (SolidBrush stock_backbrush = new SolidBrush(BackColor))
                    {
                        gr.FillRectangle(stock_backbrush, nrec);
                    }
                }
                // Draw barcode

                int aalign = 0;

                // Text justify is implemented separaterly
                StringFormat nformat = PrintOutNet.IntAlignToStringFormat(aalign, true, true,false);
                nformat.FormatFlags = nformat.FormatFlags & (~StringFormatFlags.NoClip);
                gr.DrawString(ntext, font, stock_brush, new RectangleF(nrec.Left, nrec.Top, nrec.Width, nrec.Height), nformat);
#endif
            }
        }
    }
        static PrintOutWinForms printdriver = new PrintOutWinForms();
    private void DrawChartObject(Graphics gr, Rectangle nrec, ChartItem textitem, string ntext, ChartDrawHelper designchart)
    {
        using (Font font = new Font(textitem.WFontName, (float)textitem.FontSize * (float)FDrawScale,
            GraphicUtils.FontStyleFromInteger(textitem.FontStyle)))
        {
            Color BackColor = GraphicUtils.ColorFromInteger(textitem.BackColor);
            Color FontColor = GraphicUtils.ColorFromInteger(textitem.FontColor);
            // Change colors if drawselecte
            bool drawbackground = false;
            using (SolidBrush stock_brush = new SolidBrush(FontColor))
            {
                    // Implement text rotation here
                    // by using Transform matrix for Graphics
#if REPMAN_COMPACT
					    graph.DrawString(atext,font,stock_brush,arec);
#else
                    //					graph.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
                    if (drawbackground)
                    {
                        
                    }
                    using (SolidBrush stock_backbrush = new SolidBrush(BackColor))
                    {
                        gr.FillRectangle(stock_backbrush, nrec);
                    }
                    // Draw the chart
                    if (designchart.Bitmap == null)
                    {
                        designchart.UpdateSeries();
                        designchart.Bitmap = printdriver.DrawChartBitmap(designchart.ChartSeries);
                    }
                    gr.DrawImage(designchart.Bitmap, new RectangleF(nrec.Left, nrec.Top, nrec.Width, nrec.Height));
                    int aalign = 0;

                // Text justify is implemented separaterly
                StringFormat nformat = PrintOutNet.IntAlignToStringFormat(aalign, true, true, false);
                nformat.FormatFlags = nformat.FormatFlags & (~StringFormatFlags.NoClip);
                gr.DrawString(ntext, font, stock_brush, new RectangleF(nrec.Left, nrec.Top, nrec.Width, nrec.Height), nformat);
#endif
            }
        }
    }
    private Point AlignToGridPixelds(Point source, BandInfo binfo)
    {
        if (binfo == null)
            return source;
        Point npoint = new Point(source.X - binfo.BandPosX, source.Y - binfo.BandPosY - binfo.TitleHeight);
        npoint = TwipsGraphics.AlignToGridPixels(npoint,FReport.GridWidth,FReport.GridHeight,
            DrawScale);
        npoint = new Point(npoint.X + binfo.BandPosX, npoint.Y + binfo.BandPosY + binfo.TitleHeight);
        return npoint;
    }

    private void npicture_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      selectedpositem = null;
      selectedposband = null;
      Control ncontrol;
      ncontrol = parentcontrol;

      //gr.FillRectangle(gridbrush, new Rectangle(e.X, e.Y, 2, 2));
      // Check if position is inside a band
      CapturedBand = null;
      CapturedSelection = false;
      CapturedMove = false;
      int indexband = 0;
      foreach (BandInfo nband in Bands)
      {
        if ((nband.BandPosY < e.Y) && ((nband.BandPosY + nband.TitleHeight) > e.Y) && (indexband>0))
        {
          if (indexband == 0)
          {
            SelectedItems.Clear();
            SelectedItems.Add(nband.Section.SelectionIndex,nband.Section);
            //Redraw();
            return;
          }
          CapturedBand = nband;
          RightBandCapture = false;
          break;
        }
        if (((nband.BandPosX + nband.Width) < e.X) && ((nband.BandPosX + nband.Width + nband.RightBitmap.Width) > e.X)
          && ((nband.BandPosY + nband.TitleHeight) < e.Y) && ((nband.BandPosY + nband.TitleHeight + nband.Height) > e.Y))
        {
          CapturedBand = nband;
          RightBandCapture = true;
          break;
        }
        PreviousBand = nband;
        indexband++;
      }
      if (CapturedBand != null)
      {
        if (ncuadre.Image != null)
        {
          Image xbitmap = ncuadre.Image;
          ncuadre.Image = null;
          xbitmap.Dispose();
        }
        if (ncuadreimage != null)
        {
          ncuadreimage.Dispose();
          ncuadreimage = null;
        }
        if (RightBandCapture)
          ncuadreimage = new Bitmap(CapturedBand.RightBitmap);
        else
          ncuadreimage = new Bitmap(CapturedBand.BandBitmap);

        using (Graphics ngr = Graphics.FromImage(ncuadreimage))
        {
          ngr.DrawRectangle(drawpen, new Rectangle(0, 0, ncuadreimage.Width, ncuadreimage.Height));
        }
        ncuadre.Image = new Bitmap(ncuadreimage);

        if (RightBandCapture)
          ncuadre.SetBounds(CapturedBand.BandPosX+CapturedBand.Width, CapturedBand.BandPosY+CapturedBand.TitleHeight, ncuadreimage.Width, ncuadreimage.Height);
        else
          ncuadre.SetBounds(0, CapturedBand.BandPosY+ncontrol.Top, ncuadreimage.Width, ncuadreimage.Height);
        if (ncuadre.Parent == null)
        {
          ncontrol.Parent.Controls.Add(ncuadre);
        }
        ncuadre.BringToFront();
        ncontrol.Capture = true;
        mouseorigin = new Point(e.X, e.Y);
        if (RightBandCapture)
          ncontrol.Cursor = Cursors.SizeWE;
        else
          ncontrol.Cursor = Cursors.SizeNS;
      }
      else
      {
        // If in top of a control then captured move
        if (SelectedPalette == SelectedItemPalette.Arrow)
        {
          indexband = 0;
          foreach (BandInfo nband in Bands)
          {
            // Inside band
            // And Selection mode
            if (((nband.BandPosX + nband.Width) >= e.X) && ((nband.BandPosX) <= e.X)
              && ((nband.BandPosY + nband.TitleHeight) < e.Y) && ((nband.BandPosY + nband.TitleHeight + nband.Height) > e.Y)
              )
            {
              // Look for if it's inside  acontrol
              Section nsec = nband.Section;
              Rectangle rselec = new Rectangle(e.X - nband.BandPosX, e.Y - nband.BandPosY - nband.TitleHeight, 1, 1);
              Rectangle intersec;
              foreach (PrintPosItem npositem in nsec.Components)
              {
                if (!npositem.Hidden)
                {
                  Rectangle recelement = npositem.SelectionRectangle;
                  intersec = Rectangle.Intersect(recelement, rselec);
                  if ((intersec.Width > 0) || (intersec.Height > 0))
                  {
                    selectedpositem = npositem;
                    selectedposband = nband;
                    //break;
                  }
                }
              }
              //if (selectedpositem!=null)
              //  break;
            }
            indexband++;

          }
        }
        if (selectedpositem == null)
        {
          CapturedSelection = true;
          qtop.SetBounds(e.X, e.Y, 1, MAX_SELEC_WIDTH);
          qbottom.SetBounds(e.X, e.Y, 1, MAX_SELEC_WIDTH);
          qleft.SetBounds(e.X, e.Y, MAX_SELEC_WIDTH, 1);
          qright.SetBounds(e.X, e.Y, MAX_SELEC_WIDTH, 1);
          ncontrol.Parent.Controls.Add(qtop);
          qtop.BringToFront();
          ncontrol.Parent.Controls.Add(qbottom);
          qbottom.BringToFront();
          ncontrol.Parent.Controls.Add(qleft);
          qleft.BringToFront();
          ncontrol.Parent.Controls.Add(qright);
          qright.BringToFront();
          ncontrol.Capture = true;
          ncontrol.Cursor = Cursors.Default;
        }
        else
        {
          CapturedMove = true;
          int dify = selectedposband.BandPosY + selectedposband.TitleHeight + ncontrol.Top;
          int difx = selectedposband.BandPosX + ncontrol.Left;
          qtop.SetBounds(selectedpositem.SelectionRectangle.Left+difx, selectedpositem.SelectionRectangle.Top+dify, selectedpositem.SelectionRectangle.Width, MAX_SELEC_WIDTH);
          qbottom.SetBounds(selectedpositem.SelectionRectangle.Left +difx, selectedpositem.SelectionRectangle.Bottom + dify, selectedpositem.SelectionRectangle.Width, MAX_SELEC_WIDTH);
          qleft.SetBounds(selectedpositem.SelectionRectangle.Left +difx, selectedpositem.SelectionRectangle.Top + dify, MAX_SELEC_WIDTH, selectedpositem.SelectionRectangle.Height);
          qright.SetBounds(selectedpositem.SelectionRectangle.Right +difx, selectedpositem.SelectionRectangle.Top + dify, MAX_SELEC_WIDTH, selectedpositem.SelectionRectangle.Height);
          ncontrol.Parent.Controls.Add(qtop);
          qtop.BringToFront();
          ncontrol.Parent.Controls.Add(qbottom);
          qbottom.BringToFront();
          ncontrol.Parent.Controls.Add(qleft);
          qleft.BringToFront();
          ncontrol.Parent.Controls.Add(qright);
          qright.BringToFront();
          ncontrol.Capture = true;
          ncontrol.Cursor = Cursors.SizeAll;
          Keys modkeys = Control.ModifierKeys;


          bool addselection = ((modkeys & Keys.Shift) > 0);
          bool switchselection = ((modkeys & Keys.Control) > 0);

          Rectangle rselec = new Rectangle(e.X, e.Y, 1, 1);
          SelectItems(rselec, addselection, switchselection);
          if (SelectedItems.IndexOfKey(selectedpositem.SelectionIndex) < 0)
            CapturedMove = false;
        }
        mouseorigin = new Point(e.X, e.Y);
      }
    }
    private void npicture_MouseMove(object sender, MouseEventArgs e)
    {
        Control ncontrol;
        ncontrol = parentcontrol;
      if (!ncontrol.Capture)
      {
        // Diferent cursor for bands and items
        int indexband = 0;
        ncontrol.Cursor = Cursors.Default;
        foreach (BandInfo nband in Bands)
        {
          // Band titles
          if ((nband.BandPosY < e.Y) && ((nband.BandPosY + nband.TitleHeight) > e.Y) && (indexband > 0))
          {
            ncontrol.Cursor = Cursors.SizeNS;
            break;
          }
          if (SelectedPalette == SelectedItemPalette.Arrow)
          {

            // Inside band
            if (((nband.BandPosX + nband.Width) >= e.X) && ((nband.BandPosX) <= e.X)
              && ((nband.BandPosY + nband.TitleHeight) < e.Y) && ((nband.BandPosY + nband.TitleHeight + nband.Height) > e.Y))
            {
              // Look for if it's inside  acontrol
              Section nsec = nband.Section;
              Rectangle rselec = new Rectangle(e.X - nband.BandPosX, e.Y - nband.BandPosY - nband.TitleHeight, 1, 1);
              Rectangle intersec;
              foreach (PrintPosItem npositem in nsec.Components)
              {
                if (!npositem.Hidden)
                {
                  Rectangle recelement = npositem.SelectionRectangle;
                  intersec = Rectangle.Intersect(recelement, rselec);
                  if ((intersec.Width > 0) || (intersec.Height > 0))
                  {
                    ncontrol.Cursor = Cursors.SizeAll;
                    break;
                  }
                }

              }
              if (ncontrol.Cursor == Cursors.SizeAll)
                break;
            }
          }
          // Band right side
          if (nband.RightBitmap != null)
          {
              if (((nband.BandPosX + nband.Width) < e.X) && ((nband.BandPosX + nband.Width + nband.RightBitmap.Width) > e.X)
                && ((nband.BandPosY + nband.TitleHeight) < e.Y) && ((nband.BandPosY + nband.TitleHeight + nband.Height) > e.Y))
              {
                  ncontrol.Cursor = Cursors.SizeWE;
                  break;
              }
          }
          indexband++;
        }
        if (ncontrol.Cursor == Cursors.Default)
        {
          // Right bands
        }
        return;
      }
      if (CapturedBand != null)
      {
        // Alpha blend with background image
        using (Graphics ngr = Graphics.FromImage(ncuadre.Image))
        {
          int newposy = 0;
          int newposx = 0;
          if (RightBandCapture)
          {
            newposy = CapturedBand.BandPosY+CapturedBand.TitleHeight+ncontrol.Top;
            newposx = CapturedBand.BandPosX + CapturedBand.Width + e.X - mouseorigin.X + ncontrol.Left;
            if (newposx < (CapturedBand.BandPosX + ncontrol.Left))
              newposx = CapturedBand.BandPosX + ncontrol.Left;
            Rectangle nrec = new Rectangle(newposx, newposy, ncuadre.Width, ncuadre.Height);
            /*if (usebitmap)
            {
                ngr.DrawImage(npicture.Image, new Rectangle(0, 0, ncuadre.Width, ncuadre.Height),
                  new Rectangle(newposx - npicture.Left, newposy - npicture.Top + 1, ncuadre.Width, ncuadre.Height), GraphicsUnit.Pixel);
                ngr.DrawImage(ncuadreimage, new Rectangle(0, 0, ncuadre.Width, ncuadre.Height),
                  0f, 0f, (float)ncuadre.Width, (float)ncuadre.Height, GraphicsUnit.Pixel, natributes80);
            }
            else*/
            {
                ngr.DrawImage(ncuadreimage, new Rectangle(0, 0, ncuadre.Width, ncuadre.Height),
                  0f, 0f, (float)ncuadre.Width, (float)ncuadre.Height, GraphicsUnit.Pixel);
            }
          }
          else
          {
            newposy = CapturedBand.BandPosY + e.Y - mouseorigin.Y + ncontrol.Top;
            if (newposy < (PreviousBand.BandPosY + PreviousBand.TitleHeight + ncontrol.Top))
              newposy = PreviousBand.BandPosY + PreviousBand.TitleHeight + ncontrol.Top;
            Rectangle nrec = new Rectangle(0, newposy, ncuadre.Width, ncuadre.Height);
            /*if (usebitmap)
            {
                ngr.DrawImage(npicture.Image, new Rectangle(0, 0, ncuadre.Width, ncuadre.Height),
                  new Rectangle(0 - npicture.Left + 1, newposy - npicture.Top, ncuadre.Width, ncuadre.Height), GraphicsUnit.Pixel);
                ngr.DrawImage(ncuadreimage, new Rectangle(0, 0, ncuadre.Width, ncuadre.Height),
                  0f, 0f, (float)ncuadre.Width, (float)ncuadre.Height, GraphicsUnit.Pixel, natributes80);
            }
            else*/
            {
                ngr.DrawImage(ncuadreimage, new Rectangle(ncontrol.Left, 0, ncuadre.Width, ncuadre.Height),
                  0f, 0f, (float)ncuadre.Width, (float)ncuadre.Height, GraphicsUnit.Pixel);
            }
          }
          ncuadre.SetBounds(newposx, newposy, ncuadre.Width, ncuadre.Height);
        }
        ncuadre.Invalidate();
      }
      else
        if (CapturedSelection)
        {
          int x1, x2, y1, y2;
          if (e.X > mouseorigin.X)
          {
            x1 = mouseorigin.X;
            x2 = e.X;
          }
          else
          {
            x1 = e.X;
            x2 = mouseorigin.X;
          }
          if (e.Y > mouseorigin.Y)
          {
            y1 = mouseorigin.Y;
            y2 = e.Y;
          }
          else
          {
            y1 = e.Y;
            y2 = mouseorigin.Y;
          }
          bool aligntogrid = FReport.GridEnabled;
          if (SelectedPalette == SelectedItemPalette.Arrow)
            aligntogrid = false;
          if (aligntogrid)
          {
            BandInfo firstband = GetDestinationBand(mouseorigin.X, mouseorigin.Y);
            Point pospoint = AlignToGridPixelds(new Point(x1, y1), firstband);
            Point widthpoint = AlignToGridPixelds(new Point(x2, y2), firstband);
            x1 = pospoint.X;
            x2 = widthpoint.X;
            y1 = pospoint.Y;
            y2 = widthpoint.Y;
          }
          qtop.SetBounds(x1 + ncontrol.Left, y1 + ncontrol.Top, x2 - x1, MAX_SELEC_WIDTH);
          qbottom.SetBounds(x1 + ncontrol.Left, y2 + ncontrol.Top, x2 - x1, MAX_SELEC_WIDTH);
          qleft.SetBounds(x1 + ncontrol.Left, y1 + ncontrol.Top, MAX_SELEC_WIDTH, y2 - y1);
          qright.SetBounds(x2 + ncontrol.Left, y1 + ncontrol.Top, MAX_SELEC_WIDTH, y2 - y1);
          // Select items inside the rectangle
        }
        else
          if (CapturedMove)
          {

            // Change position and possible the parent section
            // Check the parent destination band
            BandInfo destband = GetDestinationBandMove(e.X, e.Y);
            if (destband != null)
              ncontrol.Cursor = Cursors.SizeAll;
            else
              ncontrol.Cursor = Cursors.No;
            if (destband != null)
            {
              // Get diferences and apply to all components
              int offsetx = e.X - mouseorigin.X;
              int offsety = e.Y - mouseorigin.Y;
              if (destband != selectedposband)
              {
                offsety = mouseorigin.Y - (selectedposband.BandPosY + selectedposband.TitleHeight);
                offsety = -offsety + (e.Y - destband.BandPosY - destband.TitleHeight);
              }
              PrintItem nitem = selectedpositem;
              PrintPosItem npositem = (PrintPosItem)nitem;
              int nposx = npositem.PosX + TwipsGraphics.PixelsToTwips(offsetx, DrawScale);
              int nposy;
              if (destband != selectedposband)
              {
                nposy = offsety + nitem.SelectionRectangle.Top;
                nposy = TwipsGraphics.PixelsToTwips(nposy, DrawScale);
              }
              else
              {
                nposy = npositem.PosY + TwipsGraphics.PixelsToTwips(offsety, DrawScale);
              }
              if (FReport.GridEnabled)
              {
                Point npoint = TwipsGraphics.AlignToGridTwips(new Point(nposx, nposy),
                    FReport.GridWidth, FReport.GridHeight);
                nposx = npoint.X;
                nposy = npoint.Y;
                offsetx = TwipsGraphics.TwipsToPixels(nposx - npositem.PosX, FDrawScale);
                offsety = TwipsGraphics.TwipsToPixels(nposy - npositem.PosY, FDrawScale);
              }
              nposx = TwipsGraphics.TwipsToPixels(nposx,FDrawScale)+destband.BandPosX+parentcontrol.Left;
              nposy = TwipsGraphics.TwipsToPixels(nposy, FDrawScale) + destband.BandPosY+destband.TitleHeight
                  +parentcontrol.Top;

              qtop.SetBounds(nposx, nposy, selectedpositem.SelectionRectangle.Width, MAX_SELEC_WIDTH);
              qbottom.SetBounds(nposx, nposy + selectedpositem.SelectionRectangle.Height, selectedpositem.SelectionRectangle.Width, MAX_SELEC_WIDTH);
              qleft.SetBounds(nposx, nposy, MAX_SELEC_WIDTH, selectedpositem.SelectionRectangle.Height);
              qright.SetBounds(nposx + selectedpositem.SelectionRectangle.Width, nposy, MAX_SELEC_WIDTH, selectedpositem.SelectionRectangle.Height);
              qtop.Visible = true;
              qbottom.Visible = true;
              qleft.Visible = true;
              qright.Visible = true;
            }
            else
            {
              qtop.Visible = false;
              qbottom.Visible = false;
              qleft.Visible = false;
              qright.Visible = false;
            }
          }
    }
    private BandInfo GetDestinationBand(int X, int Y)
    {
        BandInfo destband = null;
        foreach (BandInfo nband in Bands)
        {
            if (((nband.BandPosX + nband.Width) >= X) && ((nband.BandPosX) <= X)
              && ((nband.BandPosY + nband.TitleHeight) <= Y) && ((nband.BandPosY + nband.TitleHeight + nband.Height) >= Y))
            {
                // Validating move
                    destband = nband;
                    break;
            }
        }
        return destband;
    }
    private BandInfo GetDestinationBandMove(int X, int Y)
    {
      BandInfo destband = null;
      foreach (BandInfo nband in Bands)
      {
        if (((nband.BandPosX + nband.Width) >= X) && ((nband.BandPosX) <= X)
          && ((nband.BandPosY + nband.TitleHeight) <= Y) && ((nband.BandPosY + nband.TitleHeight + nband.Height) >= Y))
        {
          // Validating move
          // If selecteditemsBands>0, the origin band must be the same as the destination band
          if (SelectedItemsBands.Count > 1)
          {
            if (nband.Section == selectedpositem.Section)
            {
              destband = nband;
              break;
            }
          }
          else
          {
            destband = nband;
            break;
          }
        }
      }
      return destband;
    }

    private void npicture_MouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      Keys modkeys = Control.ModifierKeys;
      bool addselection = ((modkeys & Keys.Shift) > 0);
      bool switchselection = ((modkeys & Keys.Control) > 0);

      if (switchselection)
        addselection = true;
      Control ncontrol;
      ncontrol = parentcontrol;

      ncontrol.Capture = false;
      ncontrol.Cursor = Cursors.Default;
      ncuadre.Parent = null;
      qtop.Parent = null;
      qbottom.Parent = null;
      qleft.Parent = null;
      qright.Parent = null;



      if (CapturedBand != null)
      {
        CapturedSelection = false;
        int newposy;
        int newposx;
        if (RightBandCapture)
        {
          newposy = CapturedBand.BandPosY + CapturedBand.TitleHeight;
          newposx = CapturedBand.BandPosX + CapturedBand.Width + e.X - mouseorigin.X + ncontrol.Left;
          if (newposx < (CapturedBand.BandPosX + ncontrol.Left))
            newposx = CapturedBand.BandPosX + ncontrol.Left;
          CapturedBand.Section.Width = TwipsGraphics.PixelsToTwips(
           ((newposx - ncontrol.Left) - (CapturedBand.BandPosX)), FDrawScale);
        }
        else
        {
          newposy = CapturedBand.BandPosY + e.Y - mouseorigin.Y + ncontrol.Top;
          if (newposy < (PreviousBand.BandPosY + PreviousBand.TitleHeight + ncontrol.Top))
            newposy = PreviousBand.BandPosY + PreviousBand.TitleHeight + ncontrol.Top;
          //PreviousBand.Section.Height = ((newposy-ncontrol.Top) - (PreviousBand.PosY))*1440/96;
          PreviousBand.Section.Height = TwipsGraphics.PixelsToTwips(
           ((newposy - ncontrol.Top) - (PreviousBand.PosY)), FDrawScale);
        }
        ClearSelection();
        SelectPosItem();
        Redraw();
        if (CapturedBand != null)
        {
          if (CapturedBand.Section == null)
          {
            SelectedItems.Add(PreviousBand.Section.SelectionIndex, PreviousBand.Section);
            SelectedSection = PreviousBand.Section;
          }
          else
          {
            SelectedItems.Add(CapturedBand.Section.SelectionIndex, CapturedBand.Section);
            SelectedSection =CapturedBand.Section;
          }
        }
        parentcontrol.Invalidate();
        CapturedBand = null;
        if (AfterSelect != null)
          AfterSelect(this, null);
        //timerredraw.Enabled = true;
      }
      else
        if (CapturedSelection)
        {
          CapturedSelection = false;
          int x1, x2, y1, y2;
          if (e.X > mouseorigin.X)
          {
            x1 = mouseorigin.X;
            x2 = e.X;
          }
          else
          {
            x1 = e.X;
            x2 = mouseorigin.X;
          }
          if (e.Y > mouseorigin.Y)
          {
            y1 = mouseorigin.Y;
            y2 = e.Y;
          }
          else
          {
            y1 = e.Y;
            y2 = mouseorigin.Y;
          }
          bool aligntogrid = FReport.GridEnabled;
          if (SelectedPalette == SelectedItemPalette.Arrow)
            aligntogrid = false;
          if (aligntogrid)
          {
            BandInfo firstband = GetDestinationBand(mouseorigin.X, mouseorigin.Y);
            Point pospoint = AlignToGridPixelds(new Point(x1,y1),firstband);
            Point widthpoint = AlignToGridPixelds(new Point(x2, y2),firstband);
            x1 = pospoint.X;
            x2 = widthpoint.X;
            y1 = pospoint.Y;
            y2 = widthpoint.Y;
          }

          int leftpos = x1 + ncontrol.Left;
          int toppos = y1 + ncontrol.Top;
          int widthpos = x2 - x1;
          int heightpos = y2 - y1;
          int rightpos = x2 + ncontrol.Left;
          int bottompos = y2 + ncontrol.Top;
          qtop.SetBounds(leftpos, toppos, widthpos, MAX_SELEC_WIDTH);
          qbottom.SetBounds(leftpos, bottompos, widthpos, MAX_SELEC_WIDTH);
          qleft.SetBounds(leftpos, toppos, MAX_SELEC_WIDTH, heightpos);
          qright.SetBounds(rightpos, toppos, MAX_SELEC_WIDTH, heightpos);

          // Check Shift or CTRL state
          PrintPosItem newitem = CreateFromSelectedPalette(SelectedPalette);
          if (newitem == null)
          {
            SelectItems(new Rectangle(qtop.Left - ncontrol.Left, qtop.Top - ncontrol.Top,
               qright.Left - qleft.Left, qbottom.Top - qtop.Top), addselection, switchselection);
          }
          else
          {
            BandInfo createband = GetDestinationBand(mouseorigin.X, mouseorigin.Y);
            if (createband==null)
              throw new Exception("No band in position "+mouseorigin.X.ToString()+" - "+mouseorigin.Y.ToString());
            newitem.PosX = Twips.AlignToGridTwips(TwipsGraphics.PixelsToTwips(leftpos-createband.BandPosX-ncontrol.Left, FDrawScale), FReport.GridWidth, FReport.GridHeight);
            newitem.PosY = Twips.AlignToGridTwips(TwipsGraphics.PixelsToTwips(toppos-createband.BandPosY-createband.TitleHeight-ncontrol.Top,FDrawScale),FReport.GridWidth,FReport.GridHeight);
            newitem.Width = Twips.AlignToGridTwips(TwipsGraphics.PixelsToTwips(widthpos,FDrawScale),FReport.GridWidth,FReport.GridHeight);
            newitem.Height = Twips.AlignToGridTwips(TwipsGraphics.PixelsToTwips(heightpos,FDrawScale), FReport.GridWidth, FReport.GridHeight);
            InitializeNewItem(createband, newitem);
          }
          parentcontrol.Invalidate();
        }
        else
          if (CapturedMove)
          {
            // Change position and possible the parent section
            // Check the parent destination band
            CapturedMove = false;
            BandInfo destband = GetDestinationBand(e.X, e.Y);
            if (destband != null)
              ncontrol.Cursor = Cursors.SizeAll;
            else
              ncontrol.Cursor = Cursors.No;
            if (destband != null)
            {              
              // Get diferences and apply to all components
              int offsetx = e.X - mouseorigin.X;
              int offsety = e.Y - mouseorigin.Y;
              if (destband != selectedposband)
              {
                offsety = mouseorigin.Y - (selectedposband.BandPosY + selectedposband.TitleHeight);
                offsety = -offsety + (e.Y - destband.BandPosY - destband.TitleHeight);
                // Mantain Z order
                List<PrintPosItem> ListRemove = new List<PrintPosItem>();
                foreach (PrintPosItem posi in selectedposband.Section.Components)
                {
                  if (SelectedItems.IndexOfKey(posi.SelectionIndex) >= 0)
                  {
                    destband.Section.Components.Add(posi);
                    posi.Section = destband.Section;
                    ListRemove.Add(posi);
                  }
                }
                foreach (PrintPosItem ritem in ListRemove)
                {
                  selectedposband.Section.Components.Remove(ritem);
                }
              }
              int i = 0;
              foreach (PrintItem nitem in SelectedItems.Values)
              {
                PrintPosItem npositem = (PrintPosItem)nitem;
                int nposx = npositem.PosX + TwipsGraphics.PixelsToTwips(offsetx,DrawScale);
                int nposy;
                if (destband != selectedposband)
                {
                  nposy = offsety + nitem.SelectionRectangle.Top;
                  nposy = TwipsGraphics.PixelsToTwips(nposy, DrawScale);
                }
                else
                {
                  nposy = npositem.PosY + TwipsGraphics.PixelsToTwips(offsety, DrawScale);
                }
                if (i == 0)
                {
                    if (FReport.GridEnabled)
                    {
                        Point npoint = TwipsGraphics.AlignToGridTwips(new Point(nposx, nposy),
                            FReport.GridWidth, FReport.GridHeight);
                        nposx = npoint.X;
                        nposy = npoint.Y;
                        offsetx = TwipsGraphics.TwipsToPixels(nposx - npositem.PosX, FDrawScale);
                        offsety = TwipsGraphics.TwipsToPixels(nposy - npositem.PosY, FDrawScale);
                    }
                }
                npositem.PosX = nposx;
                npositem.PosY = nposy;
                i++;
              }
              foreach (BandInfo binfo in SelectedItemsBands.Values)
              {
                ReDrawBand(binfo);
                binfo.oldposition = Int32.MinValue;
                binfo.oldpositiony = Int32.MinValue;
              }
              if (destband != selectedposband)
              {
                ReDrawBand(destband);
                destband.oldposition = Int32.MinValue;
                destband.oldpositiony = Int32.MinValue;

                SelectedItemsBands.Clear();
                SelectedItemsBands.Add(destband.Section.SelectionIndex,destband);
              }
              SelectPosItem();
              if (destband != null)
                SelectedSection = destband.Section;
              parentcontrol.Invalidate();
            }
          }
    }
      public void SelectPrintItem(PrintItem nitem)
      {

          SelectedItems.Clear();
          SelectedItemsBands.Clear();
          SelectedBands.Clear();

          if (nitem is Section)
          {
              SelectedItemsBands.Add(nitem.SelectionIndex, BandsList[nitem.SelectionIndex]);
              SelectedBands.Add(nitem.SelectionIndex, BandsList[nitem.SelectionIndex]);
          }
          else
          {
              SelectedItems.Add(nitem.SelectionIndex, nitem);
              SelectedItemsBands.Add(((PrintPosItem)nitem).Section.SelectionIndex, BandsList[((PrintPosItem)nitem).Section.SelectionIndex]);
          }
          SelectPosItem();
          //SetObject(CurrentList[ComboSelection.SelectedIndex]);
          if (AfterSelect != null)
              AfterSelect(this, null);
      }

    private void boptions_Click(object sender, EventArgs e)
    {
      // Show configuration options
      // Scale and grid properties
    }
    private void SelectItems(Rectangle rselec,bool addselection,bool switchselection)
    {
      bool onlyone = false;
      ResizeControl.Parent = null;
      if (rselec.Width == 0)
        rselec = new Rectangle(rselec.Left, rselec.Top, 1, rselec.Height);
      if (rselec.Height == 0)
        rselec = new Rectangle(rselec.Left, rselec.Top, rselec.Width, 1);
      if ((rselec.Width == 1) && (rselec.Height == 1))
        onlyone = true;
      Rectangle intersec;
      Section SectionToSelec = null;
      SortedList<int,BandInfo> OldSelectedBands = new SortedList<int,BandInfo>();
      foreach (int ix in SelectedItemsBands.Keys)
      {
        OldSelectedBands.Add(ix, SelectedItemsBands[ix]);
      }
      if ((!addselection) && (!switchselection))
      {
        SelectedItems.Clear();
      }
      else
      {
        if (SelectedItems.Count == 1)
          if (SelectedItems.Values[0] is Section)
            SelectedItems.Clear();
      }
      SelectedItemsBands.Clear();
      SelectedBands.Clear();
      foreach (BandInfo xband in Bands)
      {
        Rectangle bandrec = new Rectangle(xband.BandPosX,xband.BandPosY,xband.TotalWidth,xband.TitleHeight+xband.Height);
        intersec = Rectangle.Intersect(bandrec,rselec);
        if ((intersec.Width > 0) || (intersec.Height > 0))
        {
            if (xband.Section != null)
            {
                SelectedBands.Add(xband.Section.SelectionIndex, xband);
                List<PrintPosItem> itemstoselec = new List<PrintPosItem>();
                // Calculate rectangle relative to the current section objects
                foreach (PrintPosItem npositem in xband.Section.Components)
                {
                  if (!npositem.Hidden)
                  {
                    Rectangle recelement = new Rectangle(npositem.SelectionRectangle.Left + bandrec.Left,
                      npositem.SelectionRectangle.Top + xband.PosY, npositem.SelectionRectangle.Width,
                      npositem.SelectionRectangle.Height);
                    intersec = Rectangle.Intersect(recelement, rselec);
                    if ((intersec.Width > 0) || (intersec.Height > 0))
                    {
                      itemstoselec.Insert(0, npositem);
                    }
                  }
                }
                foreach (PrintPosItem npositem in itemstoselec)
                {
                    int ipos = SelectedItems.IndexOfKey(npositem.SelectionIndex);
                    if (ipos < 0)
                    {
                        SelectedItems.Add(npositem.SelectionIndex, npositem);
                    }
                    else
                    {
                        if (switchselection)
                        {
                            SelectedItems.Remove(npositem.SelectionIndex);
                        }
                    }
                    if (onlyone)
                        break;
                }
            }
        }
      }
      SelectedItemsBands.Clear();
      if ((SelectedBands.Count == 1) && (SelectedItems.Count == 0))
      {
        SelectedItems.Clear();
        SelectedItems.Add(SelectedBands.Values[0].Section.SelectionIndex, SelectedBands.Values[0].Section);
        SelectedSection = SelectedBands.Values[0].Section;
      }
      else
      {
        SelectedBands.Clear();
        if (SelectedItems.Count > 0)
        {
          foreach (PrintPosItem nitem in SelectedItems.Values)
          {
            int nindex = nitem.Section.SelectionIndex;
            if (SelectedItemsBands.IndexOfKey(nindex) < 0)
            {
              if (SectionToSelec==null)
                SectionToSelec = BandsList[nindex].Section;
              SelectedItemsBands.Add(nindex, BandsList[nindex]);
            }
          }
        }
        if (SectionToSelec != null)
          SelectedSection = SectionToSelec;
      }

      SortedList<int, BandInfo> RedrawBands = new SortedList<int, BandInfo>();
      foreach (int ix in OldSelectedBands.Keys)
      {
        RedrawBands.Add(ix, OldSelectedBands[ix]);
      }
      foreach (int ix in SelectedItemsBands.Keys)
      {
        if (RedrawBands.IndexOfKey(ix)<0)
          RedrawBands.Add(ix, SelectedItemsBands[ix]);
      }

      foreach (BandInfo selband in RedrawBands.Values)
      {
        ReDrawBand(selband);
        selband.oldposition = Int32.MinValue;
        selband.oldpositiony = Int32.MinValue;        
      }
      if (AfterSelect != null)
          AfterSelect(this, null);
      SelectPosItem();
      parentcontrol.Invalidate();
    }
    public void SelectPosItem()
    {
      if (SelectedItems.Count != 1)
      {
        if (ResizeControl.Parent!=null)
          ResizeControl.Parent = null;
        return;
      }
      if (!(SelectedItems.Values[0] is PrintPosItem))
      {
        if (ResizeControl.Parent != null)
          ResizeControl.Parent = null;
        return;
      }

      PrintPosItem positem = (PrintPosItem)SelectedItems.Values[0];
      ResizeControl.GridEnabled = FReport.GridEnabled;
      ResizeControl.GridWidth = FReport.GridWidth;
      ResizeControl.GridHeight = FReport.GridHeight;
      BandInfo selband = SelectedItemsBands.Values[0];
      ResizeControl.GridOffsetX = selband.BandPosX;
      ResizeControl.GridOffsetY = selband.BandPosY + selband.TitleHeight;
      ResizeControl.GridScale = FDrawScale;

      //ResizeControl.Parent = null;
      if (ResizeControl.Parent != parentcontrol.Parent)
      {
        ResizeControl.Parent = parentcontrol.Parent;
        ResizeControl.BringToFront();
      }
      
      Rectangle nrec = new Rectangle(TwipsGraphics.TwipsToPixels(positem.PosX, FDrawScale),
         TwipsGraphics.TwipsToPixels(positem.PosY, FDrawScale), TwipsGraphics.TwipsToPixels(positem.Width, FDrawScale),
         TwipsGraphics.TwipsToPixels(positem.Height, FDrawScale));
      BandInfo ninfo = null;
      foreach (BandInfo xinfo in BandsList.Values)
      {
        if (xinfo.Section == positem.Section)
        {
          ninfo = xinfo;
          break;
        }
      }
      if (ninfo == null)
        return;
      nrec = new Rectangle(nrec.Left + ninfo.BandPosX+parentcontrol.Left, nrec.Top + ninfo.PosY+parentcontrol.Top, nrec.Width, nrec.Height);
      ResizeControl.SetBounds(nrec.Left, nrec.Top, nrec.Width, nrec.Height, true);
    }
    private Rectangle SelSetBounds(Rectangle newvalue)
    {
        if ((SelectedItems.Count == 1) && (SelectedItemsBands.Count == 1))
        {
            BandInfo binfo = SelectedItemsBands.Values[0];

            int newleft = newvalue.Left-binfo.BandPosX-parentcontrol.Left;
            int newtop = newvalue.Top-binfo.BandPosY-binfo.TitleHeight-parentcontrol.Top;
            int newwidth = newvalue.Width;
            int newheight = newvalue.Height;

            PrintPosItem positem = (PrintPosItem)SelectedItems.Values[0];
            positem.PosX = TwipsGraphics.PixelsToTwips(newleft, FDrawScale);
            positem.PosY = TwipsGraphics.PixelsToTwips(newtop, FDrawScale);
            positem.Width = TwipsGraphics.PixelsToTwips(newwidth, FDrawScale);
            positem.Height = TwipsGraphics.PixelsToTwips(newheight, FDrawScale);

            ReDrawBand(binfo);
            parentcontrol.Invalidate();
        }
        
      return newvalue;
    }
    private Rectangle SelGetBounds()
    {
      Rectangle nrec = new Rectangle(0,0,0,0);
      if (SelectedItems.Count == 1)
      {
        PrintItem sitem = SelectedItems.Values[0];
        if (sitem is PrintPosItem)
        {
          PrintPosItem positem = (PrintPosItem)sitem;
          nrec = new Rectangle(TwipsGraphics.TwipsToPixels(positem.PosX, FDrawScale),
             TwipsGraphics.TwipsToPixels(positem.PosY, FDrawScale), TwipsGraphics.TwipsToPixels(positem.Width, FDrawScale),
             TwipsGraphics.TwipsToPixels(positem.Height, FDrawScale));
        }
      }
      return nrec;
    }
    public void DoPaint(object sender, PaintEventArgs e)
    {
//        Monitor.Enter(flag);
//        try
//        {
        try
        {
            foreach (BandInfo binfo in BandsToRedraw.Values)
            {
                ReDrawBandInt(binfo);
            }
            BandsToRedraw.Clear();
            RePositionBands(e.Graphics, true);
        }
        catch (Exception ex)
        {
            using (SolidBrush sb = new SolidBrush(Color.White))
            {
                e.Graphics.FillRectangle(sb,parentcontrol.ClientRectangle);
                using (SolidBrush penbrush = new SolidBrush(Color.Black))
                    e.Graphics.DrawString(ex.Message, this.Font, penbrush, new PointF(0f,0f));
            }
        }
//        }
//        finally
//        {
            //Monitor.Exit(flag);
//        }
    }
    public PrintPosItem CreateFromSelectedPalette(SelectedItemPalette nselection)
    {
      PrintPosItem nresult = null;
      switch (nselection)
      {
        case SelectedItemPalette.Arrow:
          break;
        case SelectedItemPalette.Label:
          nresult = new LabelItem(FReport);
          ((LabelItem)nresult).Text = "Text";
          break;
        case SelectedItemPalette.Expression:
          nresult = new ExpressionItem(FReport);
          break;
        case SelectedItemPalette.Shape:
          nresult = new ShapeItem(FReport);
          break;
        case SelectedItemPalette.Image:
          nresult = new ImageItem(FReport);
          break;
        case SelectedItemPalette.Chart:
          nresult = new ChartItem(FReport);
          break;
        case SelectedItemPalette.Barcode:
          nresult = new BarcodeItem(FReport);
          break;
      }
      return nresult;
    }
    private void DoRedraw(object sender, EventArgs args)
    {
        timerredraw.Enabled = false;
        ClearSelection();
        Redraw();
        parentcontrol.Invalidate();
    }

    private void parentcontextmenu_Opening(object sender, CancelEventArgs e)
    {
      msendtoback.Enabled = false;
      mbringtofront.Enabled = false;
      mhide.Enabled = false;
      if (SelectedItems.Count>0)
        if (SelectedItems.Values[0] is PrintPosItem)
        {
          msendtoback.Enabled = true;
          mbringtofront.Enabled = true;
          mhide.Enabled = true;
        }
      string fontname = "";
      if (PlatformID.Unix == System.Environment.OSVersion.Platform)
          fontname = FReport.LFontName;
      else
          fontname = FReport.WFontName;
      mcurrentfont.Text = "Default font: "+fontname+" "+FReport.FontSize.ToString();
      mcurrentfont.Font = new Font(fontname, mcurrentfont.Font.Size, GraphicUtils.FontStyleFromInteger(FReport.FontStyle));
      mcurrentfont.ForeColor = GraphicUtils.ColorFromInteger(FReport.FontColor);
      mcurrentfont.Enabled = false;
    }

    private void msendtoback_Click(object sender, EventArgs e)
    {
      //
      foreach (BandInfo binfo in SelectedItemsBands.Values)
      {
        List<PrintPosItem> toremove = new List<PrintPosItem>();
        foreach (PrintPosItem nitem in binfo.Section.Components)
        {
          int index = SelectedItems.IndexOfKey(nitem.SelectionIndex);
          if (index < 0)
          {
            if (!nitem.Hidden)
              toremove.Add(nitem);
          }
        }
        foreach (PrintPosItem nitem in toremove)
        {
          binfo.Section.Components.Remove(nitem);
          binfo.Section.Components.Add(nitem);
        }
        ReDrawBand(binfo);
      }
      parentcontrol.Invalidate();
    }

    private void mbringtofront_Click(object sender, EventArgs e)
    {
      //
      foreach (BandInfo binfo in SelectedItemsBands.Values)
      {
        List<PrintPosItem> toremove = new List<PrintPosItem>();
        foreach (PrintPosItem nitem in binfo.Section.Components)
        {
          int index = SelectedItems.IndexOfKey(nitem.SelectionIndex);
          if (index >= 0)
            toremove.Add(nitem);
        }
        foreach (PrintPosItem nitem in toremove)
        {
          binfo.Section.Components.Remove(nitem);
          binfo.Section.Components.Add(nitem);
        }
        ReDrawBand(binfo);
      }
      parentcontrol.Invalidate();
    }

    private void mhide_Click(object sender, EventArgs e)
    {
      foreach (PrintItem nitem in SelectedItems.Values)
      {
        if (nitem is PrintPosItem)
          ((PrintPosItem)nitem).Hidden = true;
      }
      foreach (BandInfo binfo in SelectedItemsBands.Values)
      {
        ReDrawBand(binfo);
      }
      ClearSelection();
      parentcontrol.Invalidate();      
    }

    private void mshowall_Click(object sender, EventArgs e)
    {
      bool doinvalidate = false;
      foreach (BandInfo binfo in Bands)
      {
        Section sec = binfo.Section;
        if (sec != null)
        {
          foreach (PrintPosItem pitem in sec.Components)
          {
            if (pitem.Hidden)
            {
              pitem.Hidden = false;
              ReDrawBand(binfo);
              doinvalidate = true;
            }
          }
        }

      }
      if (doinvalidate)
        parentcontrol.Invalidate();
    }
    private static string GetDragValue(IDataObject data,ref int fieldsize)
    {
        string[] formats = data.GetFormats();
        string expression = "";
        fieldsize = 0;
        foreach (string s in formats)
        {
            if (s == "System.String")
            {
                string exs = data.GetData(s).ToString();
                int indexsize = exs.IndexOf("_($");
                if (indexsize >= 0)
                {
                  fieldsize = System.Convert.ToInt32(exs.Substring(indexsize + 3, 3));
                  exs = exs.Substring(0, indexsize);
                }
                if (exs.Length > 10)
                {
                    if (exs.Substring(0, 10) == "__X__X__XX")
                    {
                        expression = exs.Substring(10, exs.Length - 10);
                    }
                }
                break;
            }
        }
        return expression;
    }
    private void npicture_DragOver(object sender, DragEventArgs args)
    {
        Point clientpoint = parentcontrol.PointToClient(new Point(args.X, args.Y));
      BandInfo binfo = GetDestinationBand(clientpoint.X, clientpoint.Y);
      if (binfo == null)
      {
          args.Effect = DragDropEffects.None;
      }
      else
      {
        int fieldsize = 0;
          string expression = GetDragValue(args.Data,ref fieldsize);
          if (expression.Length>0)
              args.Effect = DragDropEffects.Move;
          else
              args.Effect = DragDropEffects.None;
      }
    }
    private void InitializeNewItem(BandInfo binfo, PrintPosItem newitem)
    {
        binfo.Section.Components.Add(newitem);
        newitem.Section = binfo.Section;
        if (newitem is PrintItemText)
        {
            PrintItemText textitem = (PrintItemText)newitem;
            textitem.WFontName = FReport.WFontName;
            textitem.LFontName = FReport.LFontName;
            textitem.FontSize = FReport.FontSize;
            textitem.FontColor = FReport.FontColor;
            textitem.FontStyle = FReport.FontStyle;
        }
        countselection++;
        newitem.SelectionIndex = countselection;
        countselection++;
        FReport.GenerateNewName(newitem);
        ClearSelection();
        SelectedItems.Add(newitem.SelectionIndex, newitem);
        SelectedItemsBands.Add(binfo.Section.SelectionIndex, binfo);
        if (AfterSelect != null)
            AfterSelect(this, null);
        if (AfterInsert != null)
            AfterInsert(newitem, null);
        SelectedSection = binfo.Section;
        ReDrawBand(binfo);
        SelectPosItem();
        parentcontrol.Invalidate();

    }
    private void npicture_DragDrop(object sender, DragEventArgs args)
    {       
      Point clientpoint = parentcontrol.PointToClient(new Point(args.X, args.Y));
      BandInfo binfo = GetDestinationBand(clientpoint.X, clientpoint.Y);
      if (binfo == null)
      {
          args.Effect = DragDropEffects.None;
      }
      else
      {
          int fieldsize = 0;
          string expression = GetDragValue(args.Data,ref fieldsize);
          if (expression.Length > 0)
          {
              args.Effect = DragDropEffects.Move;
              int leftpos = clientpoint.X - binfo.BandPosX;
              int toppos = clientpoint.Y - binfo.TitleHeight - binfo.BandPosY;
              // Default WIDTH
              int widthpos = 1440;
                string fontfamily = "";
                if (System.Environment.OSVersion.Platform == PlatformID.Unix)
                  fontfamily = FReport.LFontName;
                else
                  fontfamily = FReport.WFontName;
              Size nsize = GraphicUtils.GetAvgFontSizeTwips(fontfamily, FReport.FontSize, GraphicUtils.FontStyleFromInteger(FReport.FontStyle));
              if (fieldsize > 0)
              {
                widthpos = fieldsize * nsize.Width;
              }

              // Default Height based on default font size
              int expheight = nsize.Height;

              PrintPosItem newitem = CreateFromSelectedPalette(SelectedItemPalette.Expression);
              ExpressionItem exitem = (ExpressionItem)newitem;
              exitem.Expression = expression;
              newitem.PosX = Twips.AlignToGridTwips(TwipsGraphics.PixelsToTwips(leftpos, FDrawScale), FReport.GridWidth, FReport.GridHeight);
              newitem.PosY = Twips.AlignToGridTwips(TwipsGraphics.PixelsToTwips(toppos, FDrawScale), FReport.GridWidth, FReport.GridHeight);
              newitem.Width = Twips.AlignToGridTwips(widthpos, FReport.GridWidth, FReport.GridHeight);
              newitem.Height = Twips.AlignToGridTwips(expheight, FReport.GridWidth, FReport.GridHeight);

              InitializeNewItem(binfo,newitem);
          }
          else
              args.Effect = DragDropEffects.None;
      }
    }

    private void mchangedefaultfontToolStripMenuItem_Click(object sender, EventArgs e)
    {
        string fontname = "";
        if (System.Environment.OSVersion.Platform == PlatformID.Unix)
            fontname = FReport.LFontName;
        else
            fontname = FReport.WFontName;
        Font nfont = new Font(fontname,(float)FReport.FontSize, GraphicUtils.FontStyleFromInteger(FReport.FontStyle));
        nfontdialog.Font = nfont;
        nfontdialog.Color = GraphicUtils.ColorFromInteger(FReport.FontColor);
        nfontdialog.ShowColor = true;
        if (nfontdialog.ShowDialog() == DialogResult.OK)
        {
            if (System.Environment.OSVersion.Platform == PlatformID.Unix)
                FReport.LFontName = nfontdialog.Font.FontFamily.Name;
            else
                FReport.WFontName = nfontdialog.Font.FontFamily.Name;
            FReport.FontSize = System.Convert.ToInt16(Math.Round(nfontdialog.Font.Size));
            FReport.FontColor = GraphicUtils.IntegerFromColor(nfontdialog.Color);
            FReport.FontStyle = GraphicUtils.IntegerFromFontStyle(nfontdialog.Font.Style);
        }
    }
  }
  public enum SelectedItemPalette { Arrow, Label, Expression, Shape, Image, Chart,Barcode };

  public class BandInfo:IDisposable
  {
    public BandInfo()
    {
      TitleCaption = "";
      oldposition = int.MaxValue;
      oldpositiony = int.MaxValue;
    }
    public SubReport SubReport;
    public Section Section;
    public Image SectionBitmap;
    public Bitmap RulerBitmap;
    public Bitmap BandBitmap;
    public Bitmap RightBitmap;
    public int BitmapWidth;
    public int BitmapHeight;
    public int RulerBitmapWidth;
    public int RulerBitmapHeight;
    public int BandPosY;
    public int BandPosX;
    public int PosY;
    public int PosX;
    public int Height;
    public int Width;
    public int TitleHeight;
    public int TotalWidth;
    public int oldposition;
    public int oldpositiony;
    public string TitleCaption;
    public void Dispose()
    {
            if (SectionBitmap != null)
            {
                SectionBitmap.Dispose();
                SectionBitmap = null;
            }
            if (RulerBitmap != null)
            {
                RulerBitmap.Dispose();
                RulerBitmap = null;
            }
            if (RightBitmap != null)
            {
                RightBitmap.Dispose();
                RightBitmap = null;
            }
    }
  }
}
