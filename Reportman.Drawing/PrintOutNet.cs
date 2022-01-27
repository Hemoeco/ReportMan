using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
using System.IO;

namespace Reportman.Drawing
{
    /// <summary>
    /// This is the base class for all Report processing drivers working in Reportman.Drawing space,
    /// that is print to printer, preview on screen or exporting to bitmap, provides basic functionality
    /// to measure texts, bitmaps and shapes, but still not work with printers (System.Drawing.Printing)
    /// or printer dialogs (Windows Forms).
    /// So it's a step forward to the implementation of some Report processing drivers, useful for
    /// preview, becuase it can use bitmap as output
    /// <see cref="Variant">PrintOutPrint</see>
    /// </summary>
	public class PrintOutNet : PrintOut, IDisposable
    {
        private Graphics gbitmap;
        /// <summary>
        /// This property allows scaling, usefull for drawing a preview inside a sized bitmap
        /// </summary>
		public float Scale;
        private Font stock_font;
        private int stock_style;
        StringFormat fl;
        private bool stock_WordWrap;
        private bool stock_CutText;
        private int stock_Alignment;
        private bool stock_RightToLeft;
        /// <summary>
        /// Optimization selection.
        /// <see cref="Variant">WMFOptimization</see>
        /// </summary>
		public WMFOptimization OptimizeWMF;
        private PrintOutPDF npdfdriver;

        private Color stock_brushcolor;
        private Color stock_backbrushcolor;
        private float stock_fontsize;
        private SolidBrush stock_brush;
        private SolidBrush stock_backbrush;
        private string stock_fontname;
        /// <summary>
        /// This variables can be used as an offset, this is used by print preview to represent the non-printable
        /// area of the page
        /// </summary>
		protected int HardMarginX, HardMarginY;
        /// <summary>
        /// Because HardMargins are non-printable area, when printing to the device with this limitation, the offsets
        /// must be disabled
        /// </summary>
        protected bool UseHardMargin;
        private System.Drawing.Bitmap bitmap;
        /// <summary>
        /// The output in this driver is a bitmap, the output can be used to preview or convert it to serve in a
        /// web application
        /// </summary>
		public Bitmap Output;
        /// <summary>
        /// Free memory consumed by graphics resources
        /// </summary>
		public override void Dispose()
        {
            if (stock_brush != null)
                stock_brush.Dispose();
            if (stock_backbrush != null)
                stock_backbrush.Dispose();
            if (stock_font != null)
                stock_font.Dispose();
            if (fl != null)
                fl.Dispose();
            if (bitmap != null)
                bitmap.Dispose();
            if (npdfdriver != null)
                npdfdriver.Dispose();
            base.Dispose();
        }
        /// <summary>
        /// Constructor and initialization of graphics objects required for the driver
        /// </summary>
		public PrintOutNet() : base()
        {
            OptimizeWMF = WMFOptimization.None;
            bitmap = new System.Drawing.Bitmap(100, 100, PixelFormat.Format32bppArgb);
            bitmap.SetResolution(1440, 1440);
            gbitmap = Graphics.FromImage(bitmap);
            Scale = 1.0F;
        }
        /// <summary>
        /// Calculate graphic extent using System.Drawing, the stream must be a valid bitmap
        /// </summary>
        /// <param name="astream">Memory stream containing a valid bitmap</param>
        /// <param name="extent">Initial extent in twips of the drawing box)</param>
        /// <param name="dpi">Dots per inch, resolution of the bitmap</param>
        /// <returns>Final size of the bitmap in twips</returns>
		override public Point GraphicExtent(MemoryStream astream, Point extent,
            int dpi)
        {
            Point aresult = extent;
            astream.Seek(0, System.IO.SeekOrigin.Begin);
            System.Drawing.Bitmap abit = new Bitmap(astream);
            try
            {
                double bitwidth = abit.Width;
                double bitheight = abit.Height;
                aresult.X = (int)Math.Round(bitwidth * Twips.TWIPS_PER_INCH / dpi);
                aresult.Y = (int)Math.Round(bitheight * Twips.TWIPS_PER_INCH / dpi);
            }
            finally
            {
                abit.Dispose();
            }
            return aresult;

        }
        /// <summary>
        /// Calculate text size, font rotation is ignored
        /// </summary>
        /// <param name="aobj">Structure containing text properties like font, size and style</param>
        /// <param name="extent">Text box in twips where the text will be drawn</param>
        /// <returns>Returns the extent in twips of the text, it can be larger than the input extent</returns>
		override public Point TextExtent(TextObjectStruct aobj, Point extent)
        {
            // Text extent for justify is implemented separately
            if (((aobj.Alignment & MetaFile.AlignmentFlags_AlignHJustify) > 0) || (aobj.Type1Font == PDFFontType.Linked) || (aobj.Type1Font == PDFFontType.Embedded))
            {
                if (npdfdriver == null)
                    npdfdriver = new PrintOutPDF();
                aobj.Type1Font = PDFFontType.Linked;
                extent = npdfdriver.TextExtent(aobj, extent);

                return extent;
            }
            float dpix = gbitmap.DpiX;
            float dpiy = gbitmap.DpiY;
            Point maxextent = new Point(extent.X, extent.Y);
            // This procedure allways returns the extension without font rotation
            //			if (aobj.FontRotation != 0)
            //				return extent;
            Font font = FontFromStruct(aobj);
            string atext = aobj.Text;
            // Implement text rotation here
            // by using Transform matrix for Graphics
            SizeF asize = new SizeF(extent.X, extent.Y);
            SizeF layout = new SizeF(asize.Width * dpix / 1440, asize.Height * dpiy / 1440);
            int charsfit, linesfit;
            asize = MeasureString(gbitmap, atext, font, layout, MetStructToStringFormat(aobj), out charsfit, out linesfit);
            extent.X = (int)System.Math.Round(asize.Width * 1440 / dpix);
            extent.Y = (int)System.Math.Round(asize.Height * 1440 / dpiy);
            if (aobj.CutText)
            {
                if (extent.Y > maxextent.Y)
                    extent.Y = maxextent.Y;
            }
            return extent;
        }
        /// <summary>
        /// Set page size
        /// </summary>
        /// <param name="psize">Customized page size description</param>
        /// <returns>Returns the size in twips of the page</returns>
		override public Point SetPageSize(PageSizeDetail psize)
        {
            int nwidth, nheight;
            int index = psize.Index;
            if (psize.Custom)
            {
                nwidth = psize.CustomWidth;
                nheight = psize.CustomHeight;
            }
            else
            {
                nwidth = (int)Math.Round((double)PageSizeArray[index, 0] / 1000 * Twips.TWIPS_PER_INCH);
                nheight = (int)Math.Round((double)PageSizeArray[index, 1] / 1000 * Twips.TWIPS_PER_INCH);
            }
            if (FOrientation == OrientationType.Landscape)
            {
                int nwidth2 = nwidth;
                nwidth = nheight;
                nheight = nwidth2;
            }
            return (new Point(nwidth, nheight));
        }
        /// <summary>
        /// Obtain current page size and also an index to the PageSizeArray
        /// </summary>
        /// <param name="indexqt">Output parameter, it will be filled with the index inside the PageSizeArray</param>
        /// <returns>Returns the size of the page in twips</returns>
		override public Point GetPageSize(out int indexqt)
        {
            indexqt = 0;
            Point apoint = new Point(11904, 16836);
            return apoint;
        }
        /// <summary>
        /// Create a Font object from a MetaObjectText structure
        /// </summary>
        /// <param name="page">MetaFilePage</param>
        /// <param name="obj">MetaObjectText, containing information to create the font</param>
        /// <returns>A Font object, created with parameter information</returns>
		public Font FontFromObject(MetaPage page, MetaObjectText obj)
        {
            const float MIN_FONT_SIZE = 2.3F;
            int intfontstyle = obj.FontStyle;
            float fontsize = obj.FontSize;
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
                if ((intfontstyle & 1) > 0)
                    astyle = astyle | FontStyle.Bold;
                if ((intfontstyle & 2) > 0)
                    astyle = astyle | FontStyle.Italic;
                if ((intfontstyle & 4) > 0)
                    astyle = astyle | FontStyle.Underline;
                if ((intfontstyle & 8) > 0)
                    astyle = astyle | FontStyle.Strikeout;
                float nfontsize = fontsize;
                //                if (fontsize == 11)
                //                    nfontsize = 12f;
                stock_font = new Font(page.GetWFontNameText(obj), nfontsize, astyle);
                stock_fontname = fontname;
                stock_fontsize = fontsize;
                stock_style = intfontstyle;

                return stock_font;
            }
        }
        /// <summary>
        /// Create a Font object from a TextObjectStruct structure
        /// </summary>
        /// <param name="objt">TextObjectStruct structure with text parameters like font name, size and style</param>
        /// <returns>Returns a Font object created from parameter information</returns>
		public Font FontFromStruct(TextObjectStruct objt)
        {
            FontStyle astyle = new FontStyle();
            int intfontstyle = objt.FontStyle;
            if ((intfontstyle & 1) > 0)
                astyle = astyle | FontStyle.Bold;
            if ((intfontstyle & 2) > 0)
                astyle = astyle | FontStyle.Italic;
            if ((intfontstyle & 4) > 0)
                astyle = astyle | FontStyle.Underline;
            if ((intfontstyle & 8) > 0)
                astyle = astyle | FontStyle.Strikeout;
            Font newfont = new Font(objt.WFontName, objt.FontSize, astyle);
            return newfont;
        }
        /// <summary>
        /// Generate Stringformat from align properties
        /// </summary>
        /// <param name="align"></param>
        /// <returns></returns>
        public static StringFormat IntAlignToStringFormat(int Alignment, bool CutText, bool WordWrap, bool RightToLeft)
        {
            StringFormat fl = new StringFormat();
            fl.HotkeyPrefix = HotkeyPrefix.None;

            fl.FormatFlags = (StringFormatFlags)0;
            fl.Alignment = StringAlignment.Near;
            fl.LineAlignment = StringAlignment.Near;
            if (!CutText)
            {
                fl.FormatFlags = fl.FormatFlags | StringFormatFlags.NoClip;
                fl.Trimming = StringTrimming.None;
            }
            if (!WordWrap)
                fl.FormatFlags = fl.FormatFlags | StringFormatFlags.NoWrap;
            if (RightToLeft)
                fl.FormatFlags = fl.FormatFlags | StringFormatFlags.DirectionRightToLeft;
            if ((Alignment & MetaFile.AlignmentFlags_AlignRight) > 0)
                fl.Alignment = StringAlignment.Far;
            if ((Alignment & MetaFile.AlignmentFlags_AlignHCenter) > 0)
                fl.Alignment = StringAlignment.Center;
            if ((Alignment & MetaFile.AlignmentFlags_AlignBottom) > 0)
                fl.LineAlignment = StringAlignment.Far;
            if ((Alignment & MetaFile.AlignmentFlags_AlignVCenter) > 0)
                fl.LineAlignment = StringAlignment.Center;
            return fl;
        }
        /// <summary>
        /// Generates a StringFormat from formatting information at MetaObjectText structure
        /// </summary>
        /// <param name="obj">MetaObjectText structure containint CutText,WordWrap and Alignment</param>
        /// <returns>Returns a StringFormat usable in any System.Drawing function</returns>
        public StringFormat MetaObjectToStringFormat(MetaObjectText obj)
        {
            if (fl == null || stock_WordWrap != obj.WordWrap || stock_RightToLeft != obj.RightToLeft
                || stock_Alignment != obj.Alignment || obj.CutText != stock_CutText)
            {
                if (fl == null)
                    fl = new StringFormat();
                fl.HotkeyPrefix = HotkeyPrefix.None;

                fl.FormatFlags = (StringFormatFlags)0;
                fl.Alignment = StringAlignment.Near;
                fl.LineAlignment = StringAlignment.Near;


                if (!obj.CutText)
                {
                    fl.FormatFlags = fl.FormatFlags | StringFormatFlags.NoClip;
                    fl.Trimming = StringTrimming.None;
                }
                if (!obj.WordWrap)
                    fl.FormatFlags = fl.FormatFlags | StringFormatFlags.NoWrap;
                if (obj.RightToLeft)
                    fl.FormatFlags = fl.FormatFlags | StringFormatFlags.DirectionRightToLeft;
                if ((obj.Alignment & MetaFile.AlignmentFlags_AlignRight) > 0)
                    fl.Alignment = StringAlignment.Far;
                if ((obj.Alignment & MetaFile.AlignmentFlags_AlignHCenter) > 0)
                    fl.Alignment = StringAlignment.Center;
                if ((obj.Alignment & MetaFile.AlignmentFlags_AlignBottom) > 0)
                    fl.LineAlignment = StringAlignment.Far;
                if ((obj.Alignment & MetaFile.AlignmentFlags_AlignVCenter) > 0)
                    fl.LineAlignment = StringAlignment.Center;
                stock_CutText = obj.CutText;
                stock_Alignment = obj.Alignment;
                stock_RightToLeft = obj.RightToLeft;
                stock_WordWrap = obj.WordWrap;
            }
            return fl;
        }

        /// <summary>
        /// Generates a StringFormat from formatting information at TextObjectStruct structure
        /// </summary>
        /// <param name="obj">TextObjectStruct structure containint CutText,WordWrap and Alignment</param>
        /// <returns>Returns a StringFormat usable in any System.Drawing function</returns>
		public static StringFormat MetStructToStringFormat(TextObjectStruct obj)
        {
            StringFormat fl = new StringFormat();
            fl.HotkeyPrefix = HotkeyPrefix.None;
            fl.FormatFlags = (StringFormatFlags)0;

            if (!obj.CutText)
            {
                fl.FormatFlags = fl.FormatFlags | StringFormatFlags.NoClip;
                fl.Trimming = StringTrimming.None;
            }
            if (!obj.WordWrap)
                fl.FormatFlags = fl.FormatFlags | StringFormatFlags.NoWrap;
            if (obj.RightToLeft)
                fl.FormatFlags = fl.FormatFlags | StringFormatFlags.DirectionRightToLeft;
            if ((obj.Alignment & MetaFile.AlignmentFlags_AlignRight) > 0)
                fl.Alignment = StringAlignment.Far;
            if ((obj.Alignment & MetaFile.AlignmentFlags_AlignHCenter) > 0)
                fl.Alignment = StringAlignment.Center;
            if ((obj.Alignment & MetaFile.AlignmentFlags_AlignBottom) > 0)
                fl.LineAlignment = StringAlignment.Far;
            if ((obj.Alignment & MetaFile.AlignmentFlags_AlignVCenter) > 0)
                fl.LineAlignment = StringAlignment.Center;
            return fl;
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
        public virtual void DrawString(Graphics gr, string atext, Font font, Brush brush, Rectangle arec, StringFormat sformat)
        {
            //graph.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            //gr.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            gr.DrawString(atext, font, brush, arec, sformat);

        }
        public virtual void DrawString(Graphics gr, string atext, Font font, Brush brush, float posx, float posy, StringFormat sformat)
        {
            // gr.TextRenderingHint = TextRenderingHint.AntiAlias;
            //graph.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            gr.DrawString(atext, font, brush, posx, posy, sformat);
        }
        public virtual SizeF MeasureString(Graphics gr, string atext, Font font, SizeF layoutarea, StringFormat sformat, out int charsfit, out int linesfit)
        {
            return gr.MeasureString(atext, font, layoutarea, sformat, out charsfit, out linesfit);
        }

        /// <summary>
        /// Draws an object, into a Graphics surface
        /// </summary>
        /// <param name="graph">The grapchis surface</param>
        /// <param name="page">The MetaFilePage that contains the object</param>
        /// <param name="obj">The object to be drawn</param>
		public void DrawObject(Graphics graph, MetaPage page, MetaObject obj)
        {
            float dpix = graph.DpiX;
            float dpiy = graph.DpiY;
            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            int atop, aleft;
            if (UseHardMargin)
            {
                atop = obj.Top - HardMarginY;
                aleft = obj.Left - HardMarginX;
            }
            else
            {
                atop = obj.Top;
                aleft = obj.Left;
            }
            Rectangle arec = new Rectangle((int)Math.Round((float)aleft * dpix / 1440 * Scale),
                 (int)Math.Round((float)atop * dpiy / 1440 * Scale),
                 (int)Math.Round((float)obj.Width * dpix / 1440 * Scale),
                 (int)Math.Round((float)obj.Height * dpiy / 1440 * Scale));
            switch (obj.MetaType)
            {
                case MetaObjectType.Text:
                    MetaObjectText objt = (MetaObjectText)obj;
                    bool selected = false;
                    Font font = FontFromObject(page, objt);
                    Color BackColor = GraphicUtils.ColorFromInteger(objt.BackColor);
                    Color FontColor = GraphicUtils.ColorFromInteger(objt.FontColor);
                    // Change colors if drawselecte
                    if (DrawFound)
                    {
                        if (page.MetaFile.ObjectFound(obj))
                        {
                            BackColor = SystemColors.Highlight;
                            FontColor = SystemColors.HighlightText;
                            selected = true;
                        }
                    }
                    bool drawbackground = (!objt.Transparent) || selected;
                    if (stock_brush == null || stock_brushcolor != FontColor)
                    {
                        if (stock_brush != null)
                            stock_brush.Dispose();
                        stock_brush = new SolidBrush(FontColor);
                        stock_brushcolor = FontColor;
                    }
                    string atext = page.GetText(objt);
                    // Implement text rotation here
                    // by using Transform matrix for Graphics
                    //					graph.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
                    if (objt.FontRotation != 0)
                    {
                        graph.TranslateTransform(arec.Left, arec.Top);
                        graph.RotateTransform(-objt.FontRotation / 10);
                        arec = new Rectangle(0, 0, arec.Width, arec.Height);
                        aleft = 0;
                        atop = 0;
                    }
                    if (drawbackground)
                    {
                        if (stock_backbrush == null || stock_backbrushcolor != BackColor)
                        {
                            if (stock_backbrush != null)
                                stock_backbrush.Dispose();
                            stock_backbrush = new SolidBrush(BackColor);
                            stock_backbrushcolor = BackColor;
                        }
                        Point oldextent = new Point(objt.Width, objt.Height);
                        Point extent;
                        if (((objt.Alignment & MetaFile.AlignmentFlags_AlignHJustify) > 0) || (objt.Type1Font == PDFFontType.Linked) || (objt.Type1Font == PDFFontType.Embedded))
                        {
                            if (npdfdriver == null)
                                npdfdriver = new PrintOutPDF();
                            objt.Type1Font = PDFFontType.Linked;
                            extent = npdfdriver.TextExtent(TextObjectStruct.FromMetaObjectText(page, objt), oldextent);
                        }
                        else
                            extent = TextExtent(TextObjectStruct.FromMetaObjectText(page, objt), oldextent);
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

                        Rectangle nrec = new Rectangle((int)Math.Round((float)bleft * dpix / 1440 * Scale),
                            (int)Math.Round((float)btop * dpiy / 1440 * Scale),
                            (int)Math.Round((float)bwidth * dpix / 1440 * Scale),
                            (int)Math.Round((float)bheight * dpiy / 1440 * Scale));
                        graph.FillRectangle(stock_backbrush, nrec);
                    }
                    // Text justify is implemented separaterly
                    if (((objt.Alignment & MetaFile.AlignmentFlags_AlignHJustify) > 0) || (objt.Type1Font == PDFFontType.Embedded) || (objt.Type1Font == PDFFontType.Embedded))
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
                    }
                    break;
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
                            // Rounded rectangles implemented using a GraphicsPath instead of a rectangle (Alessio Pollero)
                            const int CornerRadius = 45;
                            int strokeOffset = Convert.ToInt32(Math.Ceiling(apen.Width));
                            arec = Rectangle.Inflate(arec, -strokeOffset, -strokeOffset);
                            apen.EndCap = apen.StartCap = LineCap.Round;
                            GraphicsPath gfxPath = new GraphicsPath();
                            gfxPath.AddArc(arec.X, arec.Y, CornerRadius, CornerRadius, 180, 90);
                            gfxPath.AddArc(arec.X + arec.Width - CornerRadius, arec.Y, CornerRadius, CornerRadius, 270, 90);
                            gfxPath.AddArc(arec.X + arec.Width - CornerRadius, arec.Y + arec.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
                            gfxPath.AddArc(arec.X, arec.Y + arec.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
                            gfxPath.CloseAllFigures();


                            if (drawinside)
                                graph.FillPath(abrush, gfxPath);
                            if (drawoutside)
                                graph.DrawPath(apen, gfxPath);
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
                    if ((obji.PreviewOnly) && (!Previewing))
                        return;
                    MemoryStream astream = page.GetStream(obji);
                    System.Drawing.Image abit = Image.FromStream(astream);
                    //Make trasparent the image background if necessary
                    // This crashes some bitmaps
                    //abit.MakeTransparent();

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

                            /*Rectangle olddest = destrec;
                            destrec = new Rectangle(arec.Left, arec.Top, (int)Math.Round((float)abit.Width / dpires * dpix * Scale), (int)Math.Round((float)abit.Height / dpires * dpiy * Scale));
                            if (srcrec.Width > 0)
                            {
                                float imaratio = (float)destrec.Width / (float)srcrec.Width;
                                // Center image
                                if (srcrec.Width * imaratio < olddest.Width)
                                    destrec = new Rectangle(System.Convert.ToInt32(destrec.Left + (olddest.Width - srcrec.Width * imaratio) / 2),
                                        destrec.Top, destrec.Width, destrec.Height);
                                else
                                {
                                    if (srcrec.Width * imaratio > olddest.Width)
                                    {
                                        srcrec = new Rectangle(0,0,System.Convert.ToInt32((srcrec.Width * imaratio-olddest.Width)/imaratio),
                                            srcrec.Height);
                                        imaratio = (float)destrec.Width / (float)srcrec.Width;
                                        srcrec = new Rectangle(0, 0, srcrec.Width,
                                            System.Convert.ToInt32(srcrec.Height / imaratio));
                                    }
                                }
                                if (srcrec.Height * imaratio < olddest.Height)
                                    destrec = new Rectangle(System.Convert.ToInt32(destrec.Left + (olddest.Height - srcrec.Height * imaratio) / 2),
                                        destrec.Top, destrec.Width, destrec.Height);
                                graph.DrawImage(abit, destrec, srcrec, GraphicsUnit.Pixel);
                            }*/
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
                    break;
            }

        }
        private void IntDrawPage(MetaFile meta, MetaPage page, Graphics gr)
        {
            for (int i = 0; i < page.Objects.Count; i++)
            {
                DrawObject(gr, page, page.Objects[i]);
            }
        }
        /// <summary>
        /// Draws all the objects inside a page to the Bitmap Output
        /// </summary>
        /// <param name="meta">MetaFile containing the page</param>
        /// <param name="page">MetaFilePage to be drawn into Output</param>
		override public void DrawPage(MetaFile meta, MetaPage page)
        {
            if (Output == null)
                throw new Exception("Ouptut not specified in printoutnet");
            Graphics gr = Graphics.FromImage(Output);
#if NETSTANDARD2_0
#else
            if (OptimizeWMF != WMFOptimization.None)
            {
                if (page.WindowsMetafile == null)
                {
                    EmfType wmftype = EmfType.EmfPlusOnly;
                    if (OptimizeWMF == WMFOptimization.Gdi)
                        wmftype = EmfType.EmfOnly;
                    else
                    if (OptimizeWMF == WMFOptimization.Gdiplus)
                        wmftype = EmfType.EmfPlusDual;

                    int awidth = (int)Math.Round(bitmap.HorizontalResolution * meta.CustomX / 1440);
                    int aheight = (int)Math.Round(bitmap.VerticalResolution * meta.CustomY / 1440);
                    float oldscale = Scale;
                    try
                    {
                        Scale = bitmap.HorizontalResolution / Output.HorizontalResolution;
                        Rectangle arec = new Rectangle(0, 0, awidth, aheight);
                        System.Drawing.Imaging.Metafile wmf;
                        IntPtr dc = gbitmap.GetHdc();
                        try
                        {
                            wmf = new System.Drawing.Imaging.Metafile(dc, arec, MetafileFrameUnit.Pixel, wmftype);
                        }
                        finally
                        {
                            gbitmap.ReleaseHdc(dc);
                        }
                        Graphics gr2 = Graphics.FromImage(wmf);
                        try
                        {
                            gr2.FillRectangle(new SolidBrush(GraphicUtils.ColorFromInteger(meta.BackColor)), 0, 0, awidth, aheight);
                            IntDrawPage(meta, page, gr2);
                        }
                        finally
                        {
                            gr2.Dispose();
                        }
                        page.WindowsMetafile = wmf;
                        page.WindowsMetafileScale = Scale;
                    }
                    finally
                    {
                        Scale = oldscale;
                    }
                }
                gr.DrawImage(page.WindowsMetafile, new Rectangle(0, 0, Output.Width, Output.Height));
            }
            else
#endif
            {
                page.WindowsMetafileScale = bitmap.HorizontalResolution / Output.HorizontalResolution;
                gr.FillRectangle(new SolidBrush(GraphicUtils.ColorFromInteger(meta.BackColor)), 0, 0, Output.Width, Output.Height);
                IntDrawPage(meta, page, gr);
            }
        }
        /// <summary>
        /// Draws the text full justified
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="arect"></param>
        /// <param name="atext"></param>
        /// <param name="nfont"></param>
        /// <param name="sbrush"></param>
        protected void TextRectJustify(Graphics gr, Rectangle arect, TextObjectStruct atext, Font nfont, SolidBrush sbrush)
        {
            int i, index;
            int posx, posy, currpos, alinedif;
            bool singleline;
            string astring;
            int alinesize;
            Strings lwords;
            Integers lwidths;
            StringBuilder aword;
            int nposx, nposy;
            string Text = atext.Text;
            int Alignment = atext.Alignment;
            bool wordbreak = atext.WordWrap;
            bool RightToLeft = atext.RightToLeft;
            StringFormat sformat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip);
            StringFormat rformat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip);
            rformat.Alignment = StringAlignment.Far;
            atext.Type1Font = PDFFontType.Linked;
            sformat.HotkeyPrefix = HotkeyPrefix.None;
            rformat.HotkeyPrefix = HotkeyPrefix.None;

            singleline = ((Alignment & MetaFile.AlignmentFlags_SingleLine) > 0);
            if (singleline)
                wordbreak = false;
            float intdpix = gr.DpiX;
            float intdpiy = gr.DpiY;
            // Calculates text extent and apply alignment
            if (npdfdriver == null)
                npdfdriver = new PrintOutPDF();
            Point full_extent = new Point(arect.Width, arect.Height);
            full_extent = npdfdriver.TextExtentLineInfo(atext, full_extent);
            Rectangle recsize = new Rectangle(0, 0, full_extent.X, full_extent.Y);
            // Align bottom or center
            posy = arect.Top;
            if ((Alignment & MetaFile.AlignmentFlags_AlignBottom) > 0)
                posy = arect.Bottom - recsize.Height;
            if ((Alignment & MetaFile.AlignmentFlags_AlignVCenter) > 0)
                posy = arect.Top + (((arect.Bottom - arect.Top) - recsize.Bottom) / 2);
            LineInfos linfos = npdfdriver.LineInfo;
            bool dojustify;
            for (i = 0; i < linfos.Count; i++)
            {
                LineInfo linfo = linfos[i];
                astring = Text.Substring(linfo.Position, linfo.Size);
                dojustify = (((Alignment & MetaFile.AlignmentFlags_AlignHJustify) > 0) &&
                     (!linfo.LastLine));
                lwords = new Strings();
                posx = arect.Left;
                if (dojustify)
                {
                    // Calculate the sizes of the words, then
                    // share space between words
                    aword = new StringBuilder();
                    index = 0;
                    while (index < astring.Length)
                    {
                        if (astring[index] != ' ')
                            aword.Append(astring[index]);
                        else
                        {
                            if (aword.Length > 0)
                                lwords.Add(aword.ToString());
                            aword = new StringBuilder();
                        }
                        index++;
                    }
                    if (aword.Length > 0)
                        lwords.Add(aword.ToString());
                    // Calculate all words size
                    alinesize = 0;
                    lwidths = new Integers();
                    foreach (string nword in lwords)
                    {
                        Point extent = new Point(arect.Width, arect.Height);
                        extent = npdfdriver.WordExtent(nword, extent);
                        if (RightToLeft)
                            lwidths.Add(-extent.X);
                        else
                            lwidths.Add(extent.X);
                        alinesize = alinesize + extent.X;
                    }
                    alinedif = arect.Width - alinesize;
                    if ((alinedif > 0) || ((alinedif == 0) && (lwords.Count == 1)))
                    {
                        if (lwords.Count > 1)
                            alinedif = alinedif / (lwords.Count - 1);
                        if (RightToLeft)
                        {
                            currpos = arect.Right;
                            alinedif = -alinedif;
                        }
                        else
                            currpos = posx;
                        index = 0;
                        for (int lindex = 0; lindex < lwords.Count; lindex++)
                        {
                            string nword = lwords[lindex];
                            nposy = posy + linfo.TopPos;
                            nposy = (int)Math.Round((double)(nposy * intdpiy) / 1440 * Scale);
                            // Last word aligned to the right
                            if ((lwords.Count > 1) && (lindex == lwords.Count - 1))
                            {
                                nposx = arect.Left;
                                nposx = (int)Math.Round(((double)(nposx) * intdpix) / 1440 * Scale);
                                DrawString(gr, nword, nfont, sbrush, new Rectangle(nposx, nposy,
                                    (int)Math.Round(((float)(arect.Width) * intdpix) / 1440 * Scale),
                                    (int)Math.Round(((float)linfo.Height * intdpix) / 1440 * Scale)), rformat);
                            }
                            else
                            {
                                nposx = currpos;
                                nposx = (int)Math.Round(((double)nposx * intdpix) / 1440 * Scale);
                                DrawString(gr, nword, nfont, sbrush, nposx, nposy, sformat);
                            }
                            currpos = currpos + lwidths[index] + alinedif;
                            index++;
                        }
                    }
                    else
                        dojustify = false;
                }
                // Not justified alignment
                if (!dojustify)
                {
                    sformat = IntAlignToStringFormat(atext.Alignment, atext.CutText, atext.WordWrap, atext.RightToLeft);
                    sformat.FormatFlags = sformat.FormatFlags | StringFormatFlags.NoWrap;
                    //StringFormat.GenericTypographic
                    posx = arect.Left;
                    nposx = posx;
                    nposy = posy + linfo.TopPos;

                    nposy = (int)Math.Round((double)(nposy * intdpiy) / 1440 * Scale);
                    //gr.DrawRectangle(new Pen(Brushes.Black, 0), new Rectangle((int)Math.Round((double)(nposx * intdpix) / 1440 * Scale),
                    //    nposy, (int)Math.Round(((float)arect.Width * intdpix) / 1440 * Scale),
                    //    (int)Math.Round(((float)linfo.Height * intdpix) / 1440 * Scale)));


                    int newWidth = arect.Width;
                    /*if ((Alignment & MetaFile.AlignmentFlags_AlignRight) > 0)
                    {
                        nposx = nposx - 60;
                    } else
                    if ((Alignment & MetaFile.AlignmentFlags_AlignHCenter) > 0)
                    {
                        nposx = nposx - 30;
                        newWidth = newWidth + 30;
                    } else
                    {
                        newWidth = newWidth + 60;
                    }^*/
                    nposx = (int)Math.Round((double)(nposx * intdpix) / 1440 * Scale);
                    newWidth = (int)Math.Round(((float)newWidth * intdpix) / 1440 * Scale);
                    int newHeight = (int)Math.Round(((float)linfo.Height * intdpix) / 1440 * Scale);
                    DrawString(gr, astring, nfont, sbrush, new Rectangle(nposx, nposy,
                        newWidth, newHeight) , sformat);
                }
            }
        }
    }
    /// <summary>
    /// This enumeration indicates diferent optimization implementations while drawing pages 
    /// (preview, web presentation or print). Microsoft .Net have an optimized graphic object, 
    /// called Windows Metafile, so when the same graphic operations must be done multiple times,
    /// they can be stored in it and "played" multiple times. This is the case for example, 
    /// when previewing a Report, you can go forward and backwards drawing again and again the
    /// same pages. If the engine have the capability of drawing multiple pages in the screen,
    /// like Report Manager Windows Forms preview does, the use of playing optimized windows
    /// metafiles can enhace performance, specially if the pages are dense (lot of text items).
    /// The drawbacks of playing metafiles is that they must be stored into memory, so when you
    /// optimize performance you hit memory consumption.
    /// <see cref="Variant">PrintOutNet</see>
    /// </summary>   
    public enum WMFOptimization : int
    {
        /// <summary>No Windows Metafile caching will be done</summary>
        None,
        /// <summary>Windows Metafile caching will be done, EmfType.EmfOnly Windows Metafile will be used</summary>
        Gdi,
        /// <summary>Windows Metafile caching will be done, EmfType.EmfPlusDual Windows Metafile will be used</summary>
        Gdiplus,
        /// <summary>Windows Metafile caching will be done, EmfType.EmfPlusOnly Windows Metafile will be used</summary>
        Gdiplusonly
    };
}
