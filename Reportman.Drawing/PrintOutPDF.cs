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

namespace Reportman.Drawing
{
    /// <summary>
    /// Report preocessing driver, capable of generate Adobe PDF files
    /// </summary>
	public class PrintOutPDF : PrintOut, IDisposable
    {

        const int AlignmentFlags_SingleLine = 64;
        const int Rectangle_Radius = 160;

        private PDFFile FPDFFile;
        private int FPageWidth;
        private int FPageHeight;
        private int PageQt;
        /// <summary>
        /// The pdf is not generated but all size calculations are done
        /// </summary>
        public bool CalculateOnly;
        /// <summary>
        /// The resulting PDF file can be compressed to reduce the file size
        /// </summary>
		public bool Compressed;
        /// <summary>
        /// Destination file where the Adobe PDF format file will be saved
        /// </summary>
		public string FileName;
        /// <summary>
        /// In memory stream containing the Adobe PDF format file
        /// </summary>
		public Stream PDFStream
        {
            get { return FPDFFile.MainPDF; }
        }
        /// <summary>
        /// Constructo and initialization
        /// </summary>
		public PrintOutPDF()
            : base()
        {
            FPDFFile = new PDFFile();
            FileName = "";
            PageQt = 0;
            FPageWidth = 11904;
            FPageHeight = 16836;
        }
        public LineInfos LineInfo
        {
            get
            {
                return FPDFFile.Canvas.LineInfo;
            }
        }
        /// <summary>
        /// Free memory consumed by graphics resources
        /// </summary>
        public override void Dispose()
        {
            if (FPDFFile != null)
            {
                FPDFFile.Dispose();
                FPDFFile = null;
            }
            base.Dispose();
        }
        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="meta">MetaFile to process</param>
		override public void NewDocument(MetaFile meta)
        {
            if (FPDFFile != null)
            {
                FPDFFile.Dispose();
                FPDFFile = null;
            }
            FPDFFile = new PDFFile();
            FPDFFile.FileName = FileName;
            FPDFFile.Compressed = Compressed;
            FPDFFile.PageWidth = meta.CustomX;
            FPDFFile.PageHeight = meta.CustomY;
            FPageWidth = meta.CustomX;
            FPageHeight = meta.CustomY;
            FPDFFile.CalculateOnly = CalculateOnly;
            FPDFFile.BeginDoc();
        }
        /// <summary>
        /// Finalization
        /// </summary>
        /// <param name="meta">MetaFile to process</param>
        override public void EndDocument(MetaFile meta)
        {
            FPDFFile.EndDoc();
            /*if (FileName.Length > 0)
			{
				FileStream fstream = new FileStream(FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
				try
				{
					FPDFFile.MainPDF.WriteTo(fstream);
				}
				finally
				{
					fstream.Close();
				}
			}*/
        }
        /// <summary>
        /// Creates a new page
        /// </summary>
		override public void NewPage(MetaFile meta, MetaPage page)
        {
            // Sets the page size for the pdf file, first if it's a qt page
            if (page.UpdatedPageSize)
            {
                FPageWidth = page.PageDetail.PhysicWidth;
                FPageHeight = page.PageDetail.PhysicHeight;
            }
            FPDFFile.NewPage(FPageWidth, FPageHeight);
        }
        /// <summary>
        /// Draw an object to the PDF file
        /// </summary>
        /// <param name="page">MetaPage conatining the object</param>
        /// <param name="obj">Object to be drawn</param>
		public void DrawObject(MetaPage page, MetaObject obj)
        {
            if (CalculateOnly)
                return;
            int X, Y, W, H, S;
            int Width, Height, posx, posy;
            Rectangle rec;
            int aalign;
            MemoryStream stream;
            string astring;

            posx = obj.Left;
            posy = obj.Top;
            switch (obj.MetaType)
            {
                case MetaObjectType.Text:
                    MetaObjectText objt = (MetaObjectText)obj;
                    FPDFFile.Canvas.Font.WFontName = page.GetWFontNameText(objt);
                    FPDFFile.Canvas.Font.FontName = FPDFFile.Canvas.Font.WFontName.Replace(" ", "");
                    FPDFFile.Canvas.Font.LFontName = page.GetLFontNameText(objt);
                    FPDFFile.Canvas.Font.Style = objt.FontStyle;
                    // Transparent ?
                    FPDFFile.Canvas.Font.Name = (PDFFontType)objt.Type1Font;
                    FPDFFile.Canvas.Font.Size = objt.FontSize;
                    FPDFFile.Canvas.Font.Color = objt.FontColor;
                    FPDFFile.Canvas.Font.Bold = (objt.FontStyle & 1) > 0;
                    FPDFFile.Canvas.Font.Italic = (objt.FontStyle & 2) > 0;
                    FPDFFile.Canvas.Font.Underline = (objt.FontStyle & 4) > 0;
                    FPDFFile.Canvas.Font.StrikeOut = (objt.FontStyle & 8) > 0;
                    FPDFFile.Canvas.Font.BackColor = objt.BackColor;
                    FPDFFile.Canvas.Font.Transparent = objt.Transparent;
                    FPDFFile.Canvas.UpdateFonts();
                    aalign = objt.Alignment;
                    rec = new Rectangle(posx, posy, objt.Width, objt.Height);
                    astring = page.GetText(objt);
                    FPDFFile.Canvas.TextRect(rec, astring, aalign, objt.CutText,
                        objt.WordWrap, objt.FontRotation, objt.RightToLeft);
                    break;
                case MetaObjectType.Draw:
                    MetaObjectDraw objd = (MetaObjectDraw)obj;
                    Width = objd.Width;
                    Height = objd.Height;
                    FPDFFile.Canvas.BrushStyle = objd.BrushStyle;
                    FPDFFile.Canvas.PenStyle = objd.PenStyle;
                    FPDFFile.Canvas.PenColor = objd.PenColor;
                    FPDFFile.Canvas.BrushColor = objd.BrushColor;
                    FPDFFile.Canvas.PenWidth = objd.PenWidth;
                    X = (int)FPDFFile.Canvas.PenWidth / 2;
                    Y = X;
                    W = Width - FPDFFile.Canvas.PenWidth + 1;
                    H = Height - FPDFFile.Canvas.PenWidth + 1;
                    if (FPDFFile.Canvas.PenWidth == 0)
                    {
                        W--;
                        H--;
                    }
                    if (W < H)
                        S = W;
                    else
                        S = H;
                    ShapeType shape = (ShapeType)objd.DrawStyle;
                    if ((shape == ShapeType.Square) || (shape == ShapeType.RoundSquare)
                        || (shape == ShapeType.Circle))
                    {
                        X = X + (int)((W - S) / 2);
                        Y = Y + (int)((H - S) / 2);
                        W = S;
                        H = S;
                    }
                    switch (shape)
                    {
                        case ShapeType.Rectangle:
                        case ShapeType.Square:
                            FPDFFile.Canvas.Rectangle(X + posx, Y + posy, X + posx + W, Y + posy + H);
                            break;
                        case ShapeType.RoundRect:
                        case ShapeType.RoundSquare:
                            FPDFFile.Canvas.RoundedRectangle(X + posx, Y + posy, X + posx + W, Y + posy + H, Rectangle_Radius);
                            break;
                        case ShapeType.Ellipse:
                        case ShapeType.Circle:
                            FPDFFile.Canvas.Ellipse(X + posx, Y + posy, X + posx + W, Y + posy + H);
                            break;
                        case ShapeType.HorzLine:
                            FPDFFile.Canvas.Line(X + posx, Y + posy, X + posx + W, Y + posy);
                            //							if (objd.PenStyle >= 3 && objd.PenStyle <= 4)
                            //							{
                            //								FPDFFile.Canvas.PenStyle = 6;
                            //								FPDFFile.Canvas.Line(X + posx, Y + posy, X + posx, Y + posx + H);
                            //							}
                            break;
                        case ShapeType.VertLine:
                            FPDFFile.Canvas.Line(X + posx, Y + posy, X + posx, Y + posy + H);
                            break;
                        case ShapeType.Oblique1:
                            FPDFFile.Canvas.Line(X + posx, Y + posy, X + posx + W, Y + posy + H);
                            break;
                        case ShapeType.Oblique2:
                            FPDFFile.Canvas.Line(X + posx, Y + posy + H, X + posx + W, Y + posy);
                            break;
                    }
                    break;
                case MetaObjectType.Image:
                    MetaObjectImage obji = (MetaObjectImage)obj;
                    // In pdf draw also preview only images
                    //if (!obji.PreviewOnly)
                    {
                        Width = obji.Width;
                        Height = obji.Height;
                        rec = new Rectangle(posx, posy, Width, Height);
                        stream = page.GetStream(obji);
                        ImageDrawStyleType dstyle = (ImageDrawStyleType)obji.DrawImageStyle;
                        long intimageindex = -1;
                        if (obji.SharedImage)
                            intimageindex = obji.StreamPos;
                        /*bool adaptsize = false;
                        if (dstyle == ImageDrawStyleType.Crop)
                        {
                            adaptsize = true;
                        }*/
                        switch (dstyle)
                        {
                            case ImageDrawStyleType.Full:
                                FPDFFile.Canvas.DrawImage(rec, stream, obji.DPIRes, false, false, intimageindex);
                                break;
                            case ImageDrawStyleType.Stretch:
                                FPDFFile.Canvas.DrawImage(rec, stream, 0, false, false, intimageindex);
                                break;
                            case ImageDrawStyleType.Crop:
                                int bitmapwidth = 0;
                                int bitmapheight = 0;
#if NETSTANDARD2_0
                                /*                                if (!BitmapUtil.GetJPegInfo(stream, out bitmapwidth, out bitmapheight))
                                                                {
                                                                    stream.Seek(0, SeekOrigin.Begin);
                                                                    int imagesize, bitsperpixel, numcolors;
                                                                    bool isgif, indexed;
                                                                    string palette;
                                                                    if (!BitmapUtil.GetBitmapInfo(stream, out bitmapwidth, out bitmapheight,
                                                                        out imagesize, null, out indexed, out bitsperpixel, out numcolors, out palette, out isgif))
                                                                    {
                                                                        throw new Exception("Image format not supported in .net standard");
                                                                    }
                                                                }*/
                                using (Image nimage = Image.FromStream(stream))
                                {
                                    bitmapwidth = nimage.Width;
                                    bitmapheight = nimage.Height;
                                }
#else
                                using (Image nimage = Image.FromStream(stream))
                                {
                                    bitmapwidth = nimage.Width;
                                    bitmapheight = nimage.Height;
                                }
#endif
                                /*if (adaptsize)
                                {
                                    double nwidth = bitmapwidth * Twips.TWIPS_PER_INCH / obji.DPIRes;
                                    double nheight = bitmapheight * Twips.TWIPS_PER_INCH / obji.DPIRes;
                                    if (nwidth > rec.Width)
                                        rec = new Rectangle(rec.Left, rec.Top, Convert.ToInt32(nwidth), Height);
                                    if (nheight > rec.Height)
                                        rec = new Rectangle(rec.Left, rec.Top, rec.Width, Convert.ToInt32(nheight));
                                }*/

                                double propx = (double)rec.Width / bitmapwidth;
                                double propy = (double)rec.Height / bitmapheight;
                                H = 0;
                                W = 0;
                                if (propy > propx)
                                {
                                    H = Convert.ToInt32(Math.Round(rec.Height * propx / propy));
                                    rec = new Rectangle(rec.Left, Convert.ToInt32(rec.Top + (rec.Height - H) / 2), rec.Width, H);
                                }
                                else
                                {
                                    W = Convert.ToInt32(rec.Width * propy / propx);
                                    rec = new Rectangle(rec.Left + (rec.Width - W) / 2, rec.Top, W, rec.Height);
                                }
                                FPDFFile.Canvas.DrawImage(rec, stream, 0, false, false, intimageindex);
                                break;
                            case ImageDrawStyleType.Tile:
                            case ImageDrawStyleType.Tiledpi:
                                FPDFFile.Canvas.DrawImage(rec, stream, PDFFile.CONS_PDFRES, true, true, intimageindex);
                                break;
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// Draw all objects of the page to current PDF file page
        /// </summary>
        /// <param name="meta">MetaFile containing the page</param>
        /// <param name="page">MetaPage to be drawn</param>
		override public void DrawPage(MetaFile meta, MetaPage page)
        {
            for (int i = 0; i < page.Objects.Count; i++)
            {
                DrawObject(page, page.Objects[i]);
            }
        }
        /// <summary>
        /// Generate the PDF file in a single pass
        /// </summary>
        /// <param name="meta"></param>
		override public bool Print(MetaFile meta)
        {


            bool aresult = base.Print(meta);
            int FCurrentPage = FromPage - 1;
            meta.RequestPage(FCurrentPage);
            if (meta.Pages.CurrentCount < FromPage)
                return false;
            SetPageSize(meta.Pages[0].PageDetail);
            SetOrientation(meta.Orientation);
            MetaPage apage;
            while (meta.Pages.CurrentCount > FCurrentPage)
            {
                apage = meta.Pages[FCurrentPage];
                if (apage.PageDetail.Custom)
                    SetPageSize(apage.PageDetail);
                if (FCurrentPage >= FromPage)
                {
                    NewPage(meta, apage);
                }
                DrawPage(meta, apage);
                FCurrentPage++;
                if (FCurrentPage > (ToPage - 1))
                    break;
                meta.RequestPage(FCurrentPage);
            }
            EndDocument(meta);
            return aresult;
        }
        /// <summary>
        /// Obtain word extent using current settings
        /// </summary>
        public Point WordExtent(string text, Point extent)
        {
            Rectangle rect = new Rectangle(0, 0, extent.X, extent.Y);
            FPDFFile.Canvas.TextExtent(text, ref rect, false, true, false);
            extent.X = rect.Width;
            extent.Y = rect.Height;
            return extent;
        }
        /// <summary>
        /// Obtain text extent
        /// </summary>
		override public Point TextExtent(TextObjectStruct aobj, Point extent)
        {
            bool singleline;
            Rectangle rect;
            Point maxextent;
            if (aobj.FontRotation != 0)
                return extent;
            maxextent = extent;
            // single line
            singleline = (aobj.Alignment & AlignmentFlags_SingleLine) > 0;
            FPDFFile.Canvas.Font.Name = aobj.Type1Font;
            FPDFFile.Canvas.Font.WFontName = aobj.WFontName;
            FPDFFile.Canvas.Font.LFontName = aobj.LFontName;

            FPDFFile.Canvas.Font.FontName = aobj.WFontName.Replace(" ", "");
            FPDFFile.Canvas.Font.Size = aobj.FontSize;
            FPDFFile.Canvas.Font.Bold = (aobj.FontStyle & 1) > 0;
            FPDFFile.Canvas.Font.Italic = (aobj.FontStyle & 2) > 0;
            rect = new Rectangle(0, 0, extent.X, 0);
            FPDFFile.Canvas.TextExtent(aobj.Text, ref rect, aobj.WordWrap, singleline, false);
            extent.X = rect.Width;
            extent.Y = rect.Height;
            if (aobj.CutText)
            {
                if (maxextent.Y < extent.Y)
                    extent.Y = maxextent.Y;
            }
            return extent;
        }
        /// <summary>
        /// Obtain text extent, fills LineInfo
        /// </summary>
        public Point TextExtentLineInfo(TextObjectStruct aobj, Point extent)
        {
            bool singleline;
            Rectangle rect;
            Point maxextent;
            if (aobj.FontRotation != 0)
                return extent;
            maxextent = extent;
            // single line
            singleline = (aobj.Alignment & AlignmentFlags_SingleLine) > 0;
            FPDFFile.Canvas.Font.Name = aobj.Type1Font;
            FPDFFile.Canvas.Font.WFontName = aobj.WFontName;
            FPDFFile.Canvas.Font.LFontName = aobj.LFontName;

            FPDFFile.Canvas.Font.FontName = aobj.WFontName.Replace(" ", "");
            FPDFFile.Canvas.Font.Size = aobj.FontSize;
            FPDFFile.Canvas.Font.Bold = (aobj.FontStyle & 1) > 0;
            FPDFFile.Canvas.Font.Italic = (aobj.FontStyle & 2) > 0;
            rect = new Rectangle(0, 0, extent.X, 0);
            FPDFFile.Canvas.TextExtent(aobj.Text, ref rect, aobj.WordWrap, singleline, true);
            extent.X = rect.Width;
            extent.Y = rect.Height;
            if (aobj.CutText)
            {
                if (maxextent.Y < extent.Y)
                    extent.Y = maxextent.Y;
            }
            return extent;
        }
        /// <summary>
        /// Obtain graphic extent
        /// </summary>
        /// <param name="astream">Stream containing a bitmap or a Jpeg image</param>
        /// <param name="extent">Initial bounding box</param>
        /// <param name="dpi">Resolution in Dots per inch of the image</param>
        /// <returns>Size in twips of the image</returns>
        override public Point GraphicExtent(MemoryStream astream, Point extent,
            int dpi)
        {
            int imagesize;
            int bitmapwidth, bitmapheight;
            bool indexed;
            int numcolors, bitsperpixel;
            string palette;
            bitmapwidth = 0;
            bitmapheight = 0;
            string mask;
            astream.Seek(0, System.IO.SeekOrigin.Begin);
            if (!BitmapUtil.GetJPegInfo(astream, out bitmapwidth, out bitmapheight))
            {
                bool isgif = false;
                astream.Seek(0, System.IO.SeekOrigin.Begin);
                BitmapUtil.GetBitmapInfo(astream, out bitmapwidth, out bitmapheight, out imagesize, null,
                    out indexed, out bitsperpixel, out numcolors, out palette, out isgif, out mask, null);
                astream.Seek(0, System.IO.SeekOrigin.Begin);
            }
            if (dpi <= 0)
                return (new Point(extent.X, extent.Y));
            extent.X = (int)Math.Round((double)bitmapwidth / dpi * Twips.TWIPS_PER_INCH);
            extent.Y = (int)Math.Round((double)bitmapheight / dpi * Twips.TWIPS_PER_INCH);
            return new Point(extent.X, extent.Y);
        }
        /// <summary>
        /// Sets page orientation
        /// </summary>
        /// <param name="PageOrientation">Input value</param>
		override public void SetOrientation(OrientationType PageOrientation)
        {
            if (PageOrientation == FOrientation)
                return;
            if (PageOrientation == OrientationType.Default)
                return;
            if (PageOrientation == OrientationType.Portrait)
                FOrientation = PageOrientation;
            else
            {
                int atemp = FPageWidth;
                FPageWidth = FPageHeight;
                FPageHeight = atemp;
                FOrientation = PageOrientation;
            }
        }
        /// <summary>
        /// Sets page size
        /// </summary>
        /// <param name="psize">Input value</param>
        /// <returns>Size in twips of the page</returns>
		override public Point SetPageSize(PageSizeDetail psize)
        {
            int newwidth, newheight;
            // Sets the page size for the pdf file, first if it's a qt page
            PageQt = psize.Index;
            if (psize.Custom)
            {
                PageQt = -1;
                newwidth = psize.CustomWidth;
                newheight = psize.CustomHeight;
            }
            else
            {
                newwidth = (int)Math.Round((double)MetaFile.PageSizeArray[psize.Index, 0] / 1000 * Twips.TWIPS_PER_INCH);
                newheight = (int)Math.Round((double)MetaFile.PageSizeArray[psize.Index, 1] / 1000 * Twips.TWIPS_PER_INCH);
            }
            if (FOrientation == OrientationType.Landscape)
            {
                FPageWidth = newheight;
                FPageHeight = newwidth;
            }
            else
            {
                FPageWidth = newwidth;
                FPageHeight = newheight;
            }
            return new Point(FPageWidth, FPageHeight);
        }
        /// <summary>
        /// Get page size
        /// </summary>
        /// <param name="indexqt">Output parameters, index for PageSizeArray</param>
        /// <returns>Page size in twips</returns>
		override public Point GetPageSize(out int indexqt)
        {
            indexqt = PageQt;
            return new Point(FPageWidth, FPageHeight);
        }
        public enum ImageDepth { Color, GrayScale, BW, Text, TextQuality, BWImage, Color8bit, Color4bit };

        public static System.IO.MemoryStream ImagesToPDF(System.Collections.Generic.IEnumerable<System.Drawing.Image> images, int dpi, System.Drawing.Imaging.ImageFormat nformat, int quality, ImageDepth ndepth)
        {
            System.IO.MemoryStream nresult = null;
            System.Drawing.Imaging.ImageCodecInfo icodec = null;
            bool getcodec = true;
            Reportman.Drawing.MetaFile nmetafile = new Reportman.Drawing.MetaFile();
            nmetafile.PageSizeIndex = 0;
            int index = 0;
            foreach (System.Drawing.Image nimage in images)
            {
                if (index == 0)
                {
                    nmetafile.CustomX = nimage.Width * 1440 / dpi;
                    nmetafile.CustomY = nimage.Height * 1440 / dpi;
                }
                Reportman.Drawing.MetaPage npage = new Reportman.Drawing.MetaPage(nmetafile);
                npage.PageDetail.Custom = true;
                npage.PageDetail.CustomWidth = nimage.Width * 1440 / dpi;
                npage.PageDetail.CustomHeight = nimage.Height * 1440 / dpi;
                nmetafile.Pages.Add(npage);

                int pwidth = nimage.Width * 1440 / dpi;
                int pheight = nimage.Height * 1440 / dpi;
                using (System.IO.MemoryStream mstream = new System.IO.MemoryStream())
                {
                    if ((nformat == System.Drawing.Imaging.ImageFormat.Gif) || (nformat == System.Drawing.Imaging.ImageFormat.Bmp))
                    {
                        if ((ndepth == ImageDepth.BW) || (ndepth == ImageDepth.Text) || (ndepth == ImageDepth.TextQuality)
                             || (ndepth == ImageDepth.BWImage))
                        {
                            System.Drawing.Bitmap nbitmaptosave = null;
                            System.Drawing.Bitmap nbitmaptoconvert = null;
                            if (nimage is System.Drawing.Bitmap)
                            {
                                if (nimage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
                                    nbitmaptosave = (System.Drawing.Bitmap)nimage;
                                else
                                    nbitmaptoconvert = (System.Drawing.Bitmap)nimage;
                            }
                            else
                            {
                                nbitmaptoconvert = new System.Drawing.Bitmap(nimage.Width, nimage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                using (System.Drawing.Graphics ngraph = System.Drawing.Graphics.FromImage(nbitmaptoconvert))
                                {
                                    ngraph.DrawImage(nimage, new System.Drawing.Point(0, 0));
                                }
                            }
                            if (nbitmaptosave == null)
                            {
                                nbitmaptosave = Reportman.Drawing.GraphicUtils.ConvertToBitonal(nbitmaptoconvert, 255 * 3 / 2);
                            }
                            if (nbitmaptoconvert != null)
                            {
                                if (nbitmaptoconvert != nimage)
                                {
                                    nbitmaptoconvert.Dispose();
                                }
                            }
                            nbitmaptosave.Save(mstream, System.Drawing.Imaging.ImageFormat.Bmp);
                        }
                        else
                        {
                            System.Drawing.Image nbitmap = null;
                            if ((nimage.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed) && (nimage.PixelFormat != System.Drawing.Imaging.PixelFormat.Format4bppIndexed))
                            {
                                throw new Exception("ImageDepth Indexed not supported in .Net Core");
                                // nbitmap = ImageOptimizer.ConvertToBitmapDepth(nimage, 256);
                            }
                            else
                            {
                                nbitmap = nimage;
                            }
                            mstream.SetLength(0);
                            nimage.Save(mstream, System.Drawing.Imaging.ImageFormat.Bmp);
                        }
                    }
                    else
                    {
                        if (getcodec)
                        {
                            icodec = Reportman.Drawing.GraphicUtils.GetImageCodec("image/jpeg");
                            getcodec = false;
                        }
                        if (icodec != null)
                        {
                            System.Drawing.Imaging.EncoderParameters eparams = new System.Drawing.Imaging.EncoderParameters(1);
                            System.Drawing.Imaging.EncoderParameter qParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality,
                                (long)quality);
                            eparams.Param[0] = qParam;
                            nimage.Save(mstream, icodec, eparams);
                        }
                        else
                            nimage.Save(mstream, System.Drawing.Imaging.ImageFormat.Bmp);

                    }
                    mstream.Seek(0, System.IO.SeekOrigin.Begin);
                    npage.DrawImage(0, 0, pwidth, pheight, Reportman.Drawing.ImageDrawStyleType.Stretch, dpi, mstream);

                }
                index++;
            }
            if (index == 0)
                throw new Exception("No images suplied to ImagesToPDF");

            nmetafile.Finish();
            Reportman.Drawing.PrintOutPDF npdf = new Reportman.Drawing.PrintOutPDF();
            if (nformat == System.Drawing.Imaging.ImageFormat.Bmp)
                npdf.Compressed = true;
            else
                npdf.Compressed = false;
            npdf.Print(nmetafile);
            nresult = Reportman.Drawing.StreamUtil.StreamToMemoryStream(npdf.PDFStream);
            return nresult;
        }

    }
}
