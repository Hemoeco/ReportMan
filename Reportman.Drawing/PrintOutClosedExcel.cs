using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Reportman.Drawing
{
    public class PrintOutClosedExcel: PrintOut, IDisposable
    {
        DateTime mmfirst;
        int npage;
        int nrecord;
        MetaFile nmeta;
        int PageQt;
        int FPageWidth, FPageHeight;
        /// <summary>
        /// Excel filename
        /// </summary>
        public string FileName;
        /// <summary>
        /// Set this property to force the excel to
        /// be conatained only in one sheet
        /// </summary>
        public bool OneSheet;
        public int Precision;
        /// <summary>
        /// Constructo and initialization
        /// </summary>
		public PrintOutClosedExcel()
            : base()
        {
            const int XLS_PRECISION = 100;
            FileName = "";
            PageQt = 0;
            FPageWidth = 11904;
            FPageHeight = 16836;
            Precision = XLS_PRECISION;
        }
        /// <summary>
        /// Draw all objects of the page to current PDF file page
        /// </summary>
        /// <param name="meta">MetaFile containing the page</param>
        /// <param name="page">MetaPage to be drawn</param>
        override public void DrawPage(MetaFile meta, MetaPage page)
        {
            //            for (int i = 0; i < page.Objects.Count; i++)
            //            {
            //                DrawObject(page, page.Objects[i]);
            //            }
        }

        /// <summary>
        /// Obtain text extent
        /// </summary>
        override public Point TextExtent(TextObjectStruct aobj, Point extent)
        {
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
            return new Point(0, 0);
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
        /// <summary>
        /// The driver should do initialization here, a print driver should start a print document,
        /// while a preview driver should initialize a bitmap
        /// </summary>
        public override void NewDocument(MetaFile meta)
        {
        }
        public static void PrintObject(ClosedXML.Excel.IXLWorksheet sh, MetaPage page, MetaObject obj, int dpix,
             int dpiy, SortedList rows, SortedList columns,
             string FontName, int FontSize, int rowinit, double Precision)
        {
            string atext;
            int arow;
            int acolumn;
            bool isanumber;
            FontStyle astyle;
            Color acolor;
            bool isadate;

            string topstring = ((double)obj.Top / Precision).ToString("0000000000");
            string leftstring = ((double)obj.Left / Precision).ToString("0000000000");
            arow = rows.IndexOfKey(topstring) + 1 + rowinit;
            acolumn = columns.IndexOfKey(leftstring) + 1;
            if (acolumn < 1)
                acolumn = 1;
            if (arow < 1)
                arow = 1;

            var cell = sh.Cell(arow,acolumn);
            /*var cell = cells.
            object[] param2 = new object[2];
            param2[0] = arow;
            param2[1] = acolumn;
            object[] param1 = new object[1];
            object cell = cells.GetType().InvokeMember("Item",
                System.Reflection.BindingFlags.GetProperty, null, cells, param2);*/

            switch (obj.MetaType)
            {
                case MetaObjectType.Image:
                    MetaObjectImage obji = (MetaObjectImage)obj;
                    using (MemoryStream mstream = page.GetStream(obji))
                    {
                        //System.Drawing.Image img = System.Drawing.Image.FromStream(mstream));
                        var image = sh.AddPicture(mstream);
                        image.MoveTo(cell);
                    }
                    break;
                case MetaObjectType.Text:
                    MetaObjectText objt = (MetaObjectText)obj;
                    atext = page.GetText(objt);

                    // If it's a number
                    bool assigned = false;
                    isanumber = DoubleUtil.IsNumeric(atext, System.Globalization.NumberStyles.Any);
                    if (isanumber)
                    {
                        Double result;
                        bool boolresult = (Double.TryParse(atext, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.CurrentCulture, out result));
                        cell.Value = System.Convert.ToDouble(result, System.Globalization.CultureInfo.CurrentCulture);
                        assigned = true;
                    }
                    else
                    {
                        DateTime mydate;
                        isadate = DateUtil.IsDateTime(atext, out mydate);
                        if (isadate)
                        {
                            cell.Value = mydate;
                            assigned = true;
                        }
                        else
                            if (atext.Length > 0)
                        {
                            if (atext[0] == '=')
                                atext = "'" + atext;
                            /*param1[0] = atext;
                            cell.GetType().InvokeMember("Value",
                                System.Reflection.BindingFlags.SetProperty,
                                null, cell, param1);*/
                            cell.Value = atext;
                            assigned = true;
                        }
                    }
                    if (assigned)
                    {
                        var shfont = cell.Style.Font;

                        string nfontname = page.GetWFontNameText(objt);
                        if (FontName != nfontname)
                        {
                            cell.Style.Font.FontName = nfontname;
                        }
                        if (objt.FontSize != FontSize)
                        {
                            cell.Style.Font.FontSize = objt.FontSize;
                        }
                        acolor = GraphicUtils.ColorFromInteger(objt.FontColor);
                        if (acolor.ToArgb() != Color.Black.ToArgb())
                        {
                            ClosedXML.Excel.XLColor xlColor = ClosedXML.Excel.XLColor.FromArgb(acolor.R, acolor.G, acolor.B);
                            cell.Style.Font.FontColor = xlColor;
                        }
                        astyle = GraphicUtils.FontStyleFromInteger(objt.FontStyle);
                        if ((astyle & FontStyle.Italic) > 0)
                        {
                            cell.Style.Font.Italic = true;
                        }
                        if ((astyle & FontStyle.Bold) > 0)
                        {
                            cell.Style.Font.Bold = true;
                        }
                        if ((astyle & FontStyle.Underline) > 0)
                        {
                            cell.Style.Font.SetUnderline(ClosedXML.Excel.XLFontUnderlineValues.Single);
                        }
                        if ((astyle & FontStyle.Strikeout) > 0)
                        {
                            cell.Style.Font.SetStrikethrough(true);
                        }
                        // Font rotation not possible
                        if ((objt.Alignment & MetaFile.AlignmentFlags_AlignHCenter) > 0)
                        {
                            cell.Style.Alignment.SetHorizontal(ClosedXML.Excel.XLAlignmentHorizontalValues.Center);
                        }
                        // Multiline not supported
                        //    if (obj.AlignMent AND AlignmentFlags_SingleLine)=0 then
                        //     sh.Cells.item[arow,acolumn].Multiline:=true;
                        if ((objt.Alignment & MetaFile.AlignmentFlags_AlignLeft) > 0)
                        {
                            if (isanumber)
                            {
                                cell.Style.Alignment.SetHorizontal(ClosedXML.Excel.XLAlignmentHorizontalValues.Left);
                            }
                        }
                        if ((objt.Alignment & MetaFile.AlignmentFlags_AlignRight) > 0)
                        {
                            if (!isanumber)
                            {
                                cell.Style.Alignment.SetHorizontal(ClosedXML.Excel.XLAlignmentHorizontalValues.Right);
                            }
                        }
                        if (objt.WordWrap)
                            cell.Style.Alignment.SetWrapText(true);
                        //    if Not obj.CutText then
                        //     aalign:=aalign or DT_NOCLIP;
                        //    if obj.RightToLeft then
                        //     aalign:=aalign or DT_RTLREADING;
                        if (!objt.Transparent)
                        {
                            acolor = GraphicUtils.ColorFromInteger(objt.FontColor);
                            ClosedXML.Excel.XLColor xlColor = ClosedXML.Excel.XLColor.FromArgb(acolor.R, acolor.G, acolor.B);
                        }
                        // In office 97, not supported
                        //    if Not obj.Transparent then
                        //     sh.Cells.Item[arow,acolumn].Color:=CLXColorToVCLColor(obj.BackColor);

                    }
                    break;
            }
        }

        /// <summary>
        /// The driver should do cleanup here, a print driver should finish print document.
        /// </summary>
        public override void EndDocument(MetaFile meta)
        {

        }
        /// <summary>
        /// The driver should start a new page
        /// </summary>
        public override void NewPage(MetaFile meta, MetaPage page)
        {
        }
        private bool CheckVersion2010Up(object excel)
        {
            bool nresult = true;
            string nversion = excel.GetType().InvokeMember("Version", System.Reflection.BindingFlags.GetProperty, null, excel, new object[] { }).ToString();
            switch (nversion)
            {
                case "12.0":
                case "11.0":
                case "10.0":
                case "9.0":
                case "8.0":
                case "7.0":
                    nresult = false;
                    break;
            }

            return nresult;
        }

        // Check for the progress
        protected void CheckProgress(bool finished)
        {
            const int MILIS_PROGRESS_DEFAULT = 500;

            DateTime mmlast = System.DateTime.Now;
            TimeSpan difmilis = mmlast - mmfirst;
#if REPMAN_COMPACT
			if ((difmilis.Seconds>=(double)MILIS_PROGRESS_DEFAULT/1000) || finished)
#else
            if ((difmilis.Milliseconds >= MILIS_PROGRESS_DEFAULT) || finished)
#endif
            {
                mmfirst = System.DateTime.Now;
                bool docancel = false;
                nmeta.WorkProgress(nrecord, npage, ref docancel);
                if (docancel)
                    throw new UnNamedException(Translator.TranslateStr(503));
            }
        }
        /// <summary>
        /// Generate the excel file
        /// </summary>
        /// <param name="meta"></param>
        override public bool Print(MetaFile meta)
        {
            npage = 0;
            nrecord = 0;
            nmeta = meta;
            mmfirst = System.DateTime.Now;
            bool aresult = base.Print(meta);
            int PageLimit = ToPage - 1;
            int FirstPage = FromPage - 1;
            ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook();
            int shcount = 1;
            ClosedXML.Excel.IXLWorksheet sh;
            if (wb.Worksheets.Count == 0)
                sh = wb.AddWorksheet("Page " + shcount.ToString());
            else
            {
                sh = wb.Worksheets.Worksheet(1);
                sh.Name = "Page" + shcount.ToString();
            }
            var cells = sh.Cells();
            var font = cells.Style.Font;
            string FontName = font.FontName;
            double FontSize = font.FontSize;

            int FCurrentPage = FirstPage;
            meta.RequestPage(int.MaxValue - 1);
            if (meta.Pages.CurrentCount <= FirstPage)
                return false;
            if (ToPage > (meta.Pages.CurrentCount - 1))
                PageLimit = meta.Pages.CurrentCount - 1;
            SetPageSize(meta.Pages[0].PageDetail);
            SetOrientation(meta.Orientation);
            int dpix = GraphicUtils.DefaultDPI;
            int dpiy = dpix;

            SortedList columns = new SortedList();
            SortedList rows = new SortedList();
            MetaPage apage;
            int i, index;
            // First pass to determine columns
            for (i = FirstPage; i <= PageLimit; i++)
            {
                apage = meta.Pages[i];

                foreach (MetaObject obj1 in apage.Objects)
                {
                    if ((obj1.MetaType == MetaObjectType.Text) || (obj1.MetaType == MetaObjectType.Image))
                    {
                        string leftstring = ((double)obj1.Left / Precision).ToString("0000000000");
                        index = columns.IndexOfKey(leftstring);
                        if (index < 0)
                            columns.Add(leftstring, null);
                    }
                }
            }
            int rowinit = 0;
            // Second pass determine rows
            for (i = FirstPage; i <= PageLimit; i++)
            {
                npage = i;
                if (!OneSheet)
                {
                    rowinit = 0;
                    int shcountactual = wb.Worksheets.Count;
                    if (shcountactual < shcount)
                    {
                        sh = wb.Worksheets.Add("Page " + shcount);
                    }
                    else
                    {
                        sh = wb.Worksheets.Worksheet(shcount);
                    }
                }
                else
                {
                    rowinit = rowinit + rows.Count;
                }

                shcount++;
                rows.Clear();

                apage = meta.Pages[i];
                // Calculate rows
                nrecord = 0;
                foreach (MetaObject obj2 in apage.Objects)
                {
                    if ((obj2.MetaType == MetaObjectType.Text) || (obj2.MetaType == MetaObjectType.Image))
                    {
                        string topstring = ((double)obj2.Top / Precision).ToString("0000000000");
                        index = rows.IndexOfKey(topstring);
                        if (index < 0)
                            rows.Add(topstring, null);
                    }
                }
                // Finally, draw objects
                foreach (MetaObject obj in apage.Objects)
                {
                    if ((obj.MetaType == MetaObjectType.Text) || (obj.MetaType == MetaObjectType.Image))
                    {
                        PrintObject(sh, apage, obj, dpix, dpiy,
                         rows, columns, FontName, Convert.ToInt32(FontSize), rowinit, Precision);
                    }
                    nrecord++;
                    CheckProgress(false);
                }
            }
            EndDocument(meta);
            CheckProgress(true);


            if (FileName.Length > 0)
            {
                wb.SaveAs(FileName);
            }
            wb.Dispose();
            return aresult;
        }

    }
}
