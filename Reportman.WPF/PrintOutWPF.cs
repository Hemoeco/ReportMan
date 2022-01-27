using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Reportman.Drawing;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;

namespace Reportman.WPF
{
    public class PrintOutWPF:PrintOut
    {
        FixedDocument FDoc;
        public FixedDocument Document
        {
            get
            {
                return FDoc;
            }
        }
        double FPageWidth = 0;
        double FPageHeight = 0;
        List<PreviewWPF> PagePreviews = new List<PreviewWPF>();
        MetaFile mainmeta;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="meta">MetaFile to process</param>
        override public void NewDocument(MetaFile meta)
        {
            FDoc = new FixedDocument();
            FPageWidth = meta.CustomX;
            FPageHeight = meta.CustomY;
            mainmeta = meta;

            PagePreviews.Clear();
            
        }
        /// <summary>
        /// Finalization
        /// </summary>
        /// <param name="meta">MetaFile to process</param>
        override public void EndDocument(MetaFile meta)
        {

        }
        PageContent FCurrentPage;
        /// <summary>
        /// Creates a new page
        /// </summary>
        override public void NewPage(MetaFile meta, MetaPage page)
        {
            if (page.UpdatedPageSize)
            {
                FPageWidth = page.PageDetail.PhysicWidth;
                FPageHeight = page.PageDetail.PhysicHeight;
            }
        }
        int InternalPage;
        /// <summary>
        /// Generate the Document a single pass
        /// </summary>
        /// <param name="meta"></param>
        override public bool Print(MetaFile meta)
        {
            PrintOutText overridedriver = null;

            overridedriver = null;
            string drivername = PrinterConfig.GetDriverName(meta.PrinterSelect);
            if (drivername.Length > 0)
            {
                PrintOutText ntextdriver = new PrintOutText();
                ntextdriver.ForceDriverName = drivername;
                overridedriver = ntextdriver;
                if (overridedriver.PreparePrint(meta))
                {
                    bool doprint = true;
                    if (doprint)
                    {
                        ntextdriver.Print(meta);

                        string atext = ntextdriver.PrintResult.ToString();
                        if (atext.Length > 0)
                        {
                            //RawPrinterHelper.SendStringToPrinter(doc.PrinterSettings.PrinterName, atext);
                            return true;
                        }
                        else return false;
                    }
                    else
                        return false;
                }
            }

			bool aresult=base.Print(meta);
			int FCurrentPage = FromPage-1;
			meta.RequestPage(FCurrentPage);
			if (meta.Pages.CurrentCount < FromPage)
				return false;
			SetPageSize(meta.Pages[0].PageDetail);
			SetOrientation(meta.Orientation);
			MetaPage apage;
			while (meta.Pages.CurrentCount > FCurrentPage)
			{
				apage = meta.Pages[FCurrentPage];
				if (FCurrentPage >= FromPage)
				{
					NewPage(meta, apage);
				}
                InternalPage = FCurrentPage;
				DrawPage(meta, apage);
				FCurrentPage++;
                if (FCurrentPage > (ToPage-1))
                    break;
                meta.RequestPage(FCurrentPage);
			}
			EndDocument(meta);



            return aresult;
		}
        /// <summary>
        /// Get page size
        /// </summary>
        /// <param name="indexqt">Output parameters, index for PageSizeArray</param>
        /// <returns>Page size in twips</returns>
        override public System.Drawing.Point GetPageSize(out int indexqt)
        {
            indexqt = PageQt;
            if (FPageHeight == 0)
            {
                FPageWidth = 11904;
                FPageHeight = 16836;
            }
            return new System.Drawing.Point(System.Convert.ToInt32(FPageWidth), System.Convert.ToInt32(FPageHeight));
        }

        /// <summary>
        /// Sets page size
        /// </summary>
        /// <param name="psize">Input value</param>
        /// <returns>Size in twips of the page</returns>
        private int PageQt;
        override public System.Drawing.Point SetPageSize(PageSizeDetail psize)
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
            return new System.Drawing.Point(System.Convert.ToInt32(FPageWidth), System.Convert.ToInt32(FPageHeight));
        }
        /// <summary>
        /// Obtain graphic extent
        /// </summary>
        /// <param name="astream">Stream containing a bitmap or a Jpeg image</param>
        /// <param name="extent">Initial bounding box</param>
        /// <param name="dpi">Resolution in Dots per inch of the image</param>
        /// <returns>Size in twips of the image</returns>
        override public System.Drawing.Point GraphicExtent(MemoryStream astream, System.Drawing.Point extent,
            int dpi)
        {
            int imagesize;
            int bitmapwidth, bitmapheight;
            bool indexed;
            int numcolors, bitsperpixel;
            string palette;
            bitmapwidth = 0;
            bitmapheight = 0;
            astream.Seek(0, System.IO.SeekOrigin.Begin);
            if (!BitmapUtil.GetJPegInfo(astream, out bitmapwidth, out bitmapheight))
            {
                bool isgif = false;
                astream.Seek(0, System.IO.SeekOrigin.Begin);
                MemoryStream streamMask = new MemoryStream();
                string smask = "";
                BitmapUtil.GetBitmapInfo(astream, out bitmapwidth, out bitmapheight, out imagesize, null,
                    out indexed, out bitsperpixel, out numcolors, out palette, out isgif, out smask, streamMask);
                astream.Seek(0, System.IO.SeekOrigin.Begin);
            }
            if (dpi <= 0)
                return (new System.Drawing.Point(extent.X, extent.Y));
            extent.X = (int)Math.Round((double)bitmapwidth / dpi * Twips.TWIPS_PER_INCH);
            extent.Y = (int)Math.Round((double)bitmapheight / dpi * Twips.TWIPS_PER_INCH);
            return new System.Drawing.Point(extent.X, extent.Y);
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
                int atemp = System.Convert.ToInt32(FPageWidth);
                FPageWidth = FPageHeight;
                FPageHeight = atemp;
                FOrientation = PageOrientation;
            }
        }
        /// <summary>
        /// Draw all objects of the page to current PDF file page
        /// </summary>
        /// <param name="meta">MetaFile containing the page</param>
        /// <param name="page">MetaPage to be drawn</param>
        override public void DrawPage(MetaFile meta, MetaPage page)
        {
            FCurrentPage = new PageContent();
            FDoc.DocumentPaginator.PageSize = new System.Windows.Size(FPageWidth*96/1000, FPageHeight*96/1000);
            PreviewWPF npreview = new PreviewWPF();
            PagePreviews.Add(npreview);

            npreview.MetaFile = meta;
            npreview.Page = InternalPage;
            npreview.Width = FDoc.DocumentPaginator.PageSize.Width;
            npreview.Height = FDoc.DocumentPaginator.PageSize.Height;
            FixedPage page1 = new FixedPage();
            page1.Width = FDoc.DocumentPaginator.PageSize.Width;
            page1.Height = FDoc.DocumentPaginator.PageSize.Height;



            page1.Children.Add(npreview);

            //npreview.Measure(nsize);
            //npreview.Arrange(new System.Windows.Rect(0, 0, nsize.Width, nsize.Height));

            ((IAddChild)FCurrentPage).AddChild(page1);

            FDoc.Pages.Add(FCurrentPage);

        }
        PrintOutPDF npdfdriver;
        /// <summary>
        /// Obtain text extent
        /// </summary>
        override public System.Drawing.Point TextExtent(TextObjectStruct aobj, System.Drawing.Point extent)
        {
            // Text extent for justify is implemented separately
            if (npdfdriver == null)
                npdfdriver = new PrintOutPDF();
            aobj.Type1Font = PDFFontType.Linked;
            extent = npdfdriver.TextExtent(aobj, extent);

            return extent;
        }



    }
}
