using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reportman.Drawing
{
    public class PrintOutBitmap : PrintOutNet
    {
        private int FPageWidth;
        private int FPageHeight;
        private int PageQt;
        private Graphics gr;
        /// Draw a page to the print surface
        /// </summary>
        /// <param name="meta">MetaFile containing the page</param>
        /// <param name="page">MetaPage to be drawn</param>
        override public void DrawPage(MetaFile meta, MetaPage page)
        {
            for (int i = 0; i < page.Objects.Count; i++)
            {
                DrawObject(gr, page, page.Objects[i]);
            }
        }

        public override Point GetPageSize(out int indexqt)
        {
            indexqt = PageQt;
            return new Point(FPageWidth, FPageHeight);
        }
        class PageBitmapPosition
        {
            public int PageIndex;
            public int YPosition;
            public int XPosition;
            public int YSize;
            public int XSize;
        }
        public static Bitmap ExportToBitmap(MetaFile nmeta,
             bool allpages, int frompage, int topage, bool monoCrome, float horzRes,float vertRes)
        {
            if (allpages)
            {
                nmeta.RequestPage(MetaFile.MAX_NUMBER_PAGES);
                frompage = 0;
                topage = nmeta.Pages.CurrentCount - 1;
            }
            else
            {
                frompage = frompage - 1;
                topage = topage - 1;
                nmeta.RequestPage(topage);
                if (topage > nmeta.Pages.CurrentCount - 1)
                    topage = nmeta.Pages.Count - 1;
            }
            // First step create bitmap and draw background
            System.Collections.Generic.SortedList<int, PageBitmapPosition> Pages = new SortedList<int, PageBitmapPosition>();
            int pagePosition = 0;
            int maxWidth = 0;
            for (int i = frompage; i <= topage; i++)
            {
                var apage = nmeta.Pages[i];
                int twipsWidth = nmeta.CustomX;
                int twipsHeight = nmeta.CustomY;

                if (apage.UpdatedPageSize)
                {
                    twipsWidth = apage.PageDetail.CustomWidth;
                    twipsHeight = apage.PageDetail.CustomHeight;                    
                }
                int pageWidth = Convert.ToInt32(twipsWidth * horzRes / 1440);
                int pageHeight = Convert.ToInt32(twipsHeight * vertRes / 1440);
                PageBitmapPosition pagePos = new PageBitmapPosition();
                pagePos.PageIndex = i;
                pagePos.YPosition = pagePosition;
                pagePos.XSize = pageWidth;
                pagePos.XPosition = 0;
                pagePos.YSize = pageHeight;
                Pages.Add(i, pagePos);
                pagePosition = pagePosition + pagePos.YSize; ;
                if (pageWidth > maxWidth)
                {
                    maxWidth = pageWidth;
                }
            }
            if (Pages.Count == 0)
            {
                throw new Exception("No pages to export to bitmap");
            }
            Bitmap result = new Bitmap(maxWidth, pagePosition,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            result.SetResolution(horzRes, vertRes);
            using (var gr2 = Graphics.FromImage(result))
            {
                gr2.FillRectangle(new SolidBrush(System.Drawing.Color.DarkGray),new Rectangle(0,0,maxWidth,pagePosition));
                using (Brush backBrush = new SolidBrush(GraphicUtils.ColorFromInteger(nmeta.BackColor)))
                {
                    using (PrintOutNet pnet = new PrintOutNet())
                    {
                        foreach (var pagePosValue in Pages.Values)
                        {
                            var apage = nmeta.Pages[pagePosValue.PageIndex];
                            pagePosValue.XPosition = pagePosValue.XPosition + (maxWidth - pagePosValue.XSize) / 2;
                            using (Bitmap newBitmap = new Bitmap(pagePosValue.XSize,pagePosValue.YSize,System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                            {
                                newBitmap.SetResolution(horzRes, vertRes);
                                using (var grPage = Graphics.FromImage(newBitmap))
                                {
                                    grPage.FillRectangle(backBrush, new Rectangle(0, 0, pagePosValue.XSize, pagePosValue.YSize));
                                    for (int i = 0; i < apage.Objects.Count; i++)
                                    {
                                        pnet.DrawObject(grPage, apage, apage.Objects[i]);
                                    }
                                }
                                gr2.DrawImage(newBitmap, new Point(pagePosValue.XPosition, pagePosValue.YPosition));
                            }
                        }
                    }
                }
            }
            if (monoCrome)
            {
                Bitmap newBitmap = result.Clone(new Rectangle(0,0,result.Width,result.Height), System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                result.Dispose();
                result = newBitmap;
            }

            return result;
        }

        public override Point GraphicExtent(MemoryStream astream, Point extent, int dpi)
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

        public override Point SetPageSize(PageSizeDetail psize)
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

    }
}
