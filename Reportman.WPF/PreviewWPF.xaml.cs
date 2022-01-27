using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Reportman.Drawing;

namespace Reportman.WPF
{
    /// <summary>
    /// Lógica de interacción para PreviewWPF.xaml
    /// </summary>
    public partial class PreviewWPF : UserControl
    {
        Typeface stock_font = null;
        string stock_fontname = "";
        double stock_fontsize = 0;
        double Scale = 1.0;
        int stock_style = 0;
        Pen blackpen = null;
        Brush transbrush = null;

        Brush stock_brush = null;
        Color stock_brushcolor;
        Brush stock_backbrush = null;
        Pen stock_backpen = null;
        Color stock_backbrushcolor;
        PrintOutPDF npdfdriver;
        public PreviewWPF()
        {
            InitializeComponent();
        }
        MetaFile FMetaFile;
        /// <summary>
        /// MetaFile to process
        /// </summary>
        public MetaFile MetaFile
        {
            get { return FMetaFile; }
            set { SetMetaFile(value); }
        }
        int FPage = 0;
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
            FMetaFile = meta;
            InvalidateVisual();
/*            prdriver.NewDocument(meta);
            FScaleDrawn = -1.0F;
            FPageDrawn = -1;
            FOldPage = -1;
            FMetaFile = meta;
            if (FPage < 0)
                FPage = 0;
            FAutoScale = FMetaFile.AutoScale;
            FMetaFile.OnWorkAsyncError += new MetaFileWorkAsyncError(WorkAsyncError);
            FMetaFile.OnWorkProgress += new MetaFileWorkProgress(WorkProgress);

            ReDrawPage();*/
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
                    FMetaFile.RequestPage(request);
                }
                FPage = value;
                InvalidateVisual();
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.ActualWidth <= 0)
                return;
            if (this.ActualHeight <= 0)
                return;

            if (blackpen == null)
            {
                transbrush = new SolidColorBrush(Colors.Transparent);
                blackpen = new Pen(Brushes.Black,1);
            }
            drawingContext.DrawRectangle(transbrush,blackpen,new Rect(0,0,ActualWidth,ActualHeight));
            if (FMetaFile != null)
            {
                if (FMetaFile.Pages.Count > FPage)
                {
                    Reportman.Drawing.MetaPage page = FMetaFile.Pages[FPage];
                    for (int i = 0; i < page.Objects.Count; i++)
                    {
                        DrawObject(drawingContext, page, page.Objects[i]);
                    }
                }
            }

            base.OnRender(drawingContext);
        }

        /// <summary>
        /// Create a Font object from a MetaObjectText structure
        /// </summary>
        /// <param name="page">MetaFilePage</param>
        /// <param name="obj">MetaObjectText, containing information to create the font</param>
        /// <returns>A Font object, created with parameter information</returns>
        public Typeface FontFromObject(MetaPage page, MetaObjectText obj)
        {
            const float MIN_FONT_SIZE = 2.3F;
            int intfontstyle = obj.FontStyle;
            double fontsize = obj.FontSize;
            fontsize = fontsize * Scale;
            if (fontsize < MIN_FONT_SIZE)
                fontsize = MIN_FONT_SIZE;
            string fontname = page.GetWFontNameText(obj);
            if (stock_font != null && fontname == stock_fontname && fontsize == stock_fontsize && intfontstyle == stock_style)
            {
                return stock_font;
            }
            else
            {
                FontStyle astyle = new FontStyle();
                FontWeight aweigh = new FontWeight();
                FontFamily nfam = new FontFamily(page.GetWFontNameText(obj));
                if ((intfontstyle & 1) > 0)
                    aweigh = FontWeights.Bold;
                if ((intfontstyle & 2) > 0)
                    astyle = FontStyles.Italic;
                //if ((intfontstyle & 4) > 0)
                //    astyle = astyle | FontStyle.Underline;
                //if ((intfontstyle & 8) > 0)
                //    astyle = astyle | FontStyle.Strikeout;
                double nfontsize = fontsize;
                //                if (fontsize == 11)
                //                    nfontsize = 12f;
                stock_font = new Typeface(nfam,astyle,aweigh,FontStretches.Normal);
                stock_fontname = fontname;
                stock_fontsize = fontsize;
                stock_style = intfontstyle;

                return stock_font;
            }
        }
        public static Color ColorFromInteger(int aint)
        {
            byte r = (byte)(aint);
            byte g = (byte)(aint >> 8);
            byte b = (byte)(aint >> 16);
            Color ncolor = Color.FromArgb(255,r, g, b);
            return ncolor;
        }

        /// <summary>
        /// Draws an object, into a Graphics surface
        /// </summary>
        /// <param name="graph">The grapchis surface</param>
        /// <param name="page">The MetaFilePage that contains the object</param>
        /// <param name="obj">The object to be drawn</param>
        public void DrawObject(DrawingContext graph, MetaPage page, MetaObject obj)
        {
            float dpix = 96;
            float dpiy = 96;

            int atop, aleft;
            /*if (UseHardMargin)
            {
                atop = obj.Top - HardMarginY;
                aleft = obj.Left - HardMarginX;
            }
            else*/
            {
                atop = obj.Top;
                aleft = obj.Left;
            }
            Rect arec = new Rect((double)aleft * dpix / 1440 * Scale,
                  (double)atop * dpiy / 1440 * Scale,
                 (double)obj.Width * dpix / 1440 * Scale,
                 (double)obj.Height * dpiy / 1440 * Scale);
            switch (obj.MetaType)
            {
                case MetaObjectType.Text:
                    MetaObjectText objt = (MetaObjectText)obj;
                    bool selected = false;
                    Typeface font = FontFromObject(page, objt);
                    Color BackColor = ColorFromInteger(objt.BackColor);
                    Color FontColor = ColorFromInteger(objt.FontColor);
                    // Change colors if drawselecte
                    bool DrawFound = false;
                    if (DrawFound)
                    {
                        if (page.MetaFile.ObjectFound(obj))
                        {
                            BackColor = SystemColors.HighlightColor;
                            FontColor = SystemColors.HighlightTextColor;
                            selected = true;
                        }
                    }
                    bool drawbackground = (!objt.Transparent) || selected;
                    if (stock_brush == null || stock_brushcolor != FontColor)
                    {
                        stock_brush = new SolidColorBrush(FontColor);
                        stock_brushcolor = FontColor;
                    }
                    string atext = page.GetText(objt);
/*                    if (objt.FontRotation != 0)
                    {
                        graph.TranslateTransform(arec.Left, arec.Top);
                        graph.RotateTransform(-objt.FontRotation / 10);
                        arec = new Rectangle(0, 0, arec.Width, arec.Height);
                        aleft = 0;
                        atop = 0;
                    }*/
                    if (drawbackground)
                    {
                        if (stock_backbrush == null || stock_backbrushcolor != BackColor)
                        {
                            stock_backbrush = new SolidColorBrush(BackColor);
                            stock_backbrushcolor = BackColor;
                            stock_backpen = new Pen(stock_backbrush,0);
                        }
                        System.Drawing.Point oldextent = new System.Drawing.Point(objt.Width, objt.Height);
                        System.Drawing.Point extent;
                        if (npdfdriver == null)
                            npdfdriver = new PrintOutPDF();
                        objt.Type1Font = PDFFontType.Linked;
                        extent = npdfdriver.TextExtent(TextObjectStruct.FromMetaObjectText(page, objt), oldextent);

                        int bleft, btop, bwidth, bheight;
                        if ((objt.Alignment & MetaFile.AlignmentFlags_AlignHCenter) > 0)
                            bleft = aleft + obj.Width / 2 - extent.X / 2;
                        else
                            if ((objt.Alignment & MetaFile.AlignmentFlags_AlignRight) > 0)
                                bleft = aleft + obj.Width - extent.X;
                            else
                                bleft = aleft;
                        if ((objt.Alignment & MetaFile.AlignmentFlags_AlignVCenter) > 0)
                            btop = atop + obj.Height / 2 - extent.Y / 2;
                        else
                            if ((objt.Alignment & MetaFile.AlignmentFlags_AlignBottom) > 0)
                                btop = atop + obj.Height - extent.Y;
                            else
                                btop = atop;

                        bwidth = extent.X;
                        bheight = extent.Y;
                        if ((objt.Alignment & MetaFile.AlignmentFlags_AlignHJustify) > 0)
                        {
                            bwidth = obj.Width;
                        }

                        Rect nrec = new Rect((double)bleft * dpix / 1440 * Scale,
                            (double)btop * dpiy / 1440 * Scale,
                            (double)bwidth * dpix / 1440 *Scale,
                            (double)bheight * dpiy / 1440 * Scale);
                        graph.DrawRectangle(stock_backbrush,stock_backpen,nrec);
                    }
                    // Text justify is implemented separaterly
                    TextObjectStruct nstruc = TextObjectStruct.FromMetaObjectText(page, objt);
                    FormattedText nformat = new FormattedText(atext,System.Globalization.CultureInfo.CurrentCulture,FlowDirection.LeftToRight,
                        stock_font,stock_fontsize,stock_brush);
                    TextDecorationCollection ncollection = null;
                    if ((objt.FontStyle & 4) > 0)
                        ncollection = TextDecorations.Underline;
                    else
                      if ((objt.FontStyle & 8) > 0)
                          ncollection = TextDecorations.Strikethrough;
                    if (ncollection!=null)
                        nformat.SetTextDecorations(ncollection);
                    nformat.MaxTextHeight = obj.Height;
                    nformat.MaxTextWidth = obj.Width;
                    if (!nstruc.WordWrap)
                        nformat.MaxLineCount = 1;
                    nformat.TextAlignment = TextAlignment.Left;
                    double topoffset = 0;
                    if ((nstruc.Alignment & MetaFile.AlignmentFlags_AlignRight) > 0)
                        nformat.TextAlignment = TextAlignment.Right;
				    if ((nstruc.Alignment & MetaFile.AlignmentFlags_AlignHCenter) > 0)
					    nformat.TextAlignment = TextAlignment.Center;
                    if ((nstruc.Alignment & MetaFile.AlignmentFlags_AlignHJustify) > 0)
                        nformat.TextAlignment = TextAlignment.Justify;
    				if ((nstruc.Alignment & MetaFile.AlignmentFlags_AlignBottom) > 0)
                    {
                        double nheight = nformat.Height;
                        topoffset = (double)objt.Height/1440*96-nheight;
                    }
                    else
				    if ((nstruc.Alignment & MetaFile.AlignmentFlags_AlignVCenter) > 0)
                    {
                        double nheight = nformat.Height;
                        topoffset = ((double)objt.Height/1440*96-nheight)/2;
                    }
                    
                    graph.DrawText(nformat,new Point(arec.Left,arec.Top+topoffset));
/*                    if ((objt.Alignment & MetaFile.AlignmentFlags_AlignHJustify) > 0)
                    {
                        TextRectJustify(graph, new Rectangle(aleft, atop, obj.Width, obj.Height), TextObjectStruct.FromMetaObjectText(page, objt), font, stock_brush);
                    }
                    else
                    {
                        DrawString(graph, atext, font, stock_brush, arec, MetaObjectToStringFormat(objt));
                    }
                    if (objt.FontRotation != 0)
                    {
                        graph.ResetTransform();
                    }*/
                    break;
                    /*
                case MetaObjectType.Draw:
                    MetaObjectDraw objd = (MetaObjectDraw)obj;
                    bool drawoutside = true;
                    bool drawinside = true;
                    Pen apen = new Pen(GraphicUtils.ColorFromInteger(objd.PenColor), (float)objd.PenWidth / 1440 * dpix * Scale);
                    switch (objd.PenStyle)
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
                    switch ((BrushType)objd.BrushStyle)
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
                        abrush = new SolidBrush(GraphicUtils.ColorFromInteger(objd.BrushColor));
                    else
                        abrush = new HatchBrush(hstyle, GraphicUtils.ColorFromInteger(objd.BrushColor), Color.Empty);

                    ShapeType shape = (ShapeType)objd.DrawStyle;
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
                    break;
                    
                case MetaObjectType.Image:
                    MetaObjectImage obji = (MetaObjectImage)obj;
                    MemoryStream astream = page.GetStream(obji);
                    System.Drawing.Bitmap abit = new Bitmap(astream);
                    ImageDrawStyleType dstyle = (ImageDrawStyleType)obji.DrawImageStyle;
                    float dpires = obji.DPIRes;
                    float bitwidth = abit.Width;
                    float bitheight = abit.Height;
                    //RectangleF srcrec=new RectangleF(0,0,bitwidth-1,bitheight-1);
                    Rectangle srcrec = new Rectangle(0, 0, (int)Math.Round(bitwidth), (int)Math.Round(bitheight));
                    Rectangle destrec = arec;
                    switch (dstyle)
                    {
                        case ImageDrawStyleType.Crop:
                            destrec = new Rectangle(arec.Left, arec.Top, (int)Math.Round((float)abit.Width / dpires * dpix * Scale), (int)Math.Round((float)abit.Height / dpires * dpiy * Scale));
                            if (destrec.Width < arec.Width)
                                destrec = new Rectangle(destrec.Left + (arec.Width - destrec.Width) / 2, destrec.Top,
                                    destrec.Width, destrec.Height);
                            if (destrec.Height < arec.Height)
                                destrec = new Rectangle(destrec.Left, destrec.Top + (arec.Height - destrec.Height) / 2,
                                    destrec.Width, destrec.Height);
                            Rectangle newdestrect = destrec;
                            float imascale = 1.0f;
                            if (destrec.Width > arec.Width)
                            {
                                imascale = ((float)arec.Width / destrec.Width);
                                srcrec.Width = System.Convert.ToInt32(srcrec.Width * imascale);
                                newdestrect.Width = arec.Width;
                            }
                            if (destrec.Height > arec.Height)
                            {
                                imascale = ((float)arec.Height / destrec.Height);
                                srcrec.Height = System.Convert.ToInt32(srcrec.Height * imascale);
                                newdestrect.Height = arec.Height;
                            }
                            graph.DrawImage(abit, newdestrect, srcrec, GraphicsUnit.Pixel);

                            break;
                        case ImageDrawStyleType.Full:
                            destrec = new Rectangle(arec.Left, arec.Top, (int)Math.Round((float)abit.Width / dpires * dpix * Scale), (int)Math.Round((float)abit.Height / dpires * dpiy * Scale));
                            if (destrec.Width < arec.Width)
                                destrec = new Rectangle(destrec.Left + (arec.Width - destrec.Width) / 2, destrec.Top,
                                    destrec.Width, destrec.Height);
                            if (destrec.Height < arec.Height)
                                destrec = new Rectangle(destrec.Left, destrec.Top + (arec.Height - destrec.Height) / 2,
                                    destrec.Width, destrec.Height);
                            graph.DrawImage(abit, destrec, srcrec, GraphicsUnit.Pixel);
                            break;
                        case ImageDrawStyleType.Stretch:
                            graph.DrawImage(abit, destrec, srcrec, GraphicsUnit.Pixel);
                            break;
                        case ImageDrawStyleType.Tile:
                            TextureBrush br2 = new TextureBrush(abit);
                            graph.FillRectangle(br2, destrec);
                            break;
                        case ImageDrawStyleType.Tiledpi:
                            // Pending, scale image to adjust dpi brush
                            TextureBrush br = new TextureBrush(abit, srcrec);
                            graph.FillRectangle(br, destrec);
                            break;
                    }
                    break;*/
            }

        }

    }
}
