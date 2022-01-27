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
using System.Collections;
using System.Collections.Generic;

namespace Reportman.Drawing
{
    public enum ExcelFileFormat { Auto = 0, Normal = -4143, Excel97 = 56, Csv = 6, Txt = -4158, Prn = 36, Excel2007 =51}
    /// <summary>
    /// Report preocessing driver, capable of generate excel files
    /// </summary>
	public class PrintOutExcel : PrintOut,IDisposable
	{
        DateTime mmfirst;
        int npage;
        int nrecord;
        MetaFile nmeta;
        public ExcelFileFormat ExcelFormat;
        int PageQt;
        int FPageWidth, FPageHeight;
        /// <summary>
        /// Excel filename
        /// </summary>
        public string FileName;
        /// <summary>
        /// Excel visibility
        /// </summary>
        public bool Visible;
        /// <summary>
        /// Set this property to force the excel to
        /// be conatained only in one sheet
        /// </summary>
        public bool OneSheet;
        public int Precision;
        /// <summary>
        /// Constructo and initialization
        /// </summary>
		public PrintOutExcel()
			: base()
		{
            const int XLS_PRECISION = 100;
            FileName = "";
			PageQt = 0;
			FPageWidth = 11904;
			FPageHeight = 16836;
            Precision = XLS_PRECISION;
            // Default autodetect from extension
            ExcelFormat = ExcelFileFormat.Auto;
        }
        public static ExcelFileFormat FileFormatFromFilename(string filename)
        {
            string extension = Path.GetExtension(filename).ToUpper();
            ExcelFileFormat aresult = ExcelFileFormat.Normal;

            switch (extension)
            {
                case ".XLS":
                    aresult = ExcelFileFormat.Excel97;
                    break;
                case ".XLSX":
                    aresult = ExcelFileFormat.Excel2007;
                    break;
                case ".TXT":
                    aresult = ExcelFileFormat.Txt;
                    break;
                case ".PRN":
                    aresult = ExcelFileFormat.Prn;
                    break;
                case ".CSV":
                    aresult = ExcelFileFormat.Csv;
                    break;
            }
            return aresult;
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
        public static void PrintObject(object sh, MetaPage page, MetaObject obj, int dpix,
             int dpiy, SortedList rows, SortedList columns,
             string FontName, int FontSize, int rowinit, double Precision)
        {
             const int xlHAlignCenter = -4108;
             const int xlHAlignLeft = -4131;
             const int xlHAlignRight = -4152;
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
            object cells = sh.GetType().InvokeMember("Cells",
                System.Reflection.BindingFlags.GetProperty, null, sh, null);
            object[] param2=new object[2];
            param2[0] = arow;
            param2[1] = acolumn;
            object[] param1=new object[1];
            object cell = cells.GetType().InvokeMember("Item",
                System.Reflection.BindingFlags.GetProperty, null, cells, param2);
            switch (obj.MetaType)
            {
                case MetaObjectType.Text:
                    MetaObjectText objt=(MetaObjectText)obj;
                    atext=page.GetText(objt);

                    // If it's a number
                    bool assigned = false;
                    isanumber=DoubleUtil.IsNumeric(atext,System.Globalization.NumberStyles.Any);
                    if (isanumber)
                    {
                        param1[0]=System.Convert.ToDouble(atext);
                        cell.GetType().InvokeMember("Value",
                            System.Reflection.BindingFlags.SetProperty, 
                            null, cell, param1);
                        assigned = true;
                    }
                    else
                    {
                        DateTime mydate;
                        isadate=DateUtil.IsDateTime(atext,out mydate);
                        if (isadate)
                        {
                            param1[0] = mydate.ToString("yyyyMMdd");
                            cell.GetType().InvokeMember("Value",
                                System.Reflection.BindingFlags.SetProperty,
                                null, cell, param1);
                            assigned = true;
                        }
                        else
                            if (atext.Length>0)
                            {
                                if (atext[0]=='=')
                                 atext="'"+atext;
                                param1[0]=atext;
                                cell.GetType().InvokeMember("Value",
                                    System.Reflection.BindingFlags.SetProperty, 
                                    null, cell, param1);
                                assigned = true;
                            }
                    }
                    if (assigned)
                    {
                        object shfont=cell.GetType().InvokeMember("Font",
                                System.Reflection.BindingFlags.GetProperty,
                                null, cell, null);
                        string nfontname=page.GetWFontNameText(objt);
                        if (FontName!=nfontname)
                        {
                            param1[0]=nfontname;
                            shfont.GetType().InvokeMember("Name",
                                System.Reflection.BindingFlags.SetProperty,
                                null, shfont, param1);                            
                        }
                        if (objt.FontSize!=FontSize)
                        {
                            param1[0]=objt.FontSize;
                            shfont.GetType().InvokeMember("Size",
                                System.Reflection.BindingFlags.SetProperty,
                                null, shfont, param1);
                        }
                        acolor=GraphicUtils.ColorFromInteger(objt.FontColor);
                        if (acolor.ToArgb()!=Color.Black.ToArgb())
                        {
                            param1[0]=objt.FontColor;
                            shfont.GetType().InvokeMember("Color",
                                System.Reflection.BindingFlags.SetProperty,
                                null, shfont, param1);                            
                        }
                        astyle=GraphicUtils.FontStyleFromInteger(objt.FontStyle);
                        if ((astyle & FontStyle.Italic)>0)
                        {
                            param1[0]=true;
                            shfont.GetType().InvokeMember("Italic",
                                System.Reflection.BindingFlags.SetProperty,
                                null, shfont, param1);    
                        }
                        if ((astyle & FontStyle.Bold)>0)
                        {
                            param1[0]=true;
                            shfont.GetType().InvokeMember("Bold",
                                System.Reflection.BindingFlags.SetProperty,
                                null, shfont, param1);    
                        }
                        if ((astyle & FontStyle.Underline)>0)
                        {
                            param1[0]=true;
                            shfont.GetType().InvokeMember("Underline",
                                System.Reflection.BindingFlags.SetProperty,
                                null, shfont, param1);    
                        }
                        if ((astyle & FontStyle.Strikeout)>0)
                        {
                            param1[0]=true;
                            shfont.GetType().InvokeMember("Strikethrough",
                                System.Reflection.BindingFlags.SetProperty,
                                null, shfont, param1);    
                        }
                        // Font rotation not possible
                        if ((objt.Alignment & MetaFile.AlignmentFlags_AlignHCenter)>0)
                        {
                            param1[0]=xlHAlignCenter;
                            cell.GetType().InvokeMember("HorizontalAlignment",
                                System.Reflection.BindingFlags.SetProperty,
                                null, cell, param1);    
                        }
                            // Multiline not supported
                         //    if (obj.AlignMent AND AlignmentFlags_SingleLine)=0 then
                        //     sh.Cells.item[arow,acolumn].Multiline:=true;
                        if ((objt.Alignment & MetaFile.AlignmentFlags_AlignLeft)>0)
                        {
                            if (isanumber)
                            {
                                param1[0]=xlHAlignLeft;
                                cell.GetType().InvokeMember("HorizontalAlignment",
                                    System.Reflection.BindingFlags.SetProperty,
                                    null, cell, param1);    
                            }
                        }
                        if ((objt.Alignment & MetaFile.AlignmentFlags_AlignRight)>0)
                        {
                            if (!isanumber)
                            {
                                param1[0]=xlHAlignRight;
                                cell.GetType().InvokeMember("HorizontalAlignment",
                                    System.Reflection.BindingFlags.SetProperty,
                                    null, cell, param1);    
                            }
                        }
                            // Word wrap not supported
                        //    if obj.WordWrap then
                        //     sh.Cells.item[arow,acolumn].WordWrap:=True;
                        //    if Not obj.CutText then
                        //     aalign:=aalign or DT_NOCLIP;
                        //    if obj.RightToLeft then
                        //     aalign:=aalign or DT_RTLREADING;
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
            const int MILIS_PROGRESS_DEFAULT=500;

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
                nmeta.WorkProgress(nrecord,npage,ref docancel);
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
            int PageLimit = ToPage-1;
            int FirstPage = FromPage-1;
            Type objClassType;
            objClassType = Type.GetTypeFromProgID("Excel.Application");
            object excel = Activator.CreateInstance(objClassType);
            object[] param1 = new object[1];
            object wbs = excel.GetType().InvokeMember("Workbooks",
                System.Reflection.BindingFlags.GetProperty, null, excel, null);
            object wb = wbs.GetType().InvokeMember("Add",
                        System.Reflection.BindingFlags.InvokeMethod, null, wbs, null);
            int shcount = 1;
            object shs = wb.GetType().InvokeMember("Sheets",
                System.Reflection.BindingFlags.GetProperty, null, wb, null);
            param1[0] = 1;
            object sh = shs.GetType().InvokeMember("Item",
                        System.Reflection.BindingFlags.GetProperty, null, shs, param1);
            object cells = sh.GetType().InvokeMember("Cells",
                System.Reflection.BindingFlags.GetProperty, null, sh, null);
            object shfont = cells.GetType().InvokeMember("Font",
                        System.Reflection.BindingFlags.GetProperty, null, cells, null);
            string FontName = System.Convert.ToString(shfont.GetType().InvokeMember("Name",
                        System.Reflection.BindingFlags.GetProperty, null, shfont, null));
            int FontSize = System.Convert.ToInt32(shfont.GetType().InvokeMember("Size",
                        System.Reflection.BindingFlags.GetProperty, null, shfont, null));

            
            int FCurrentPage = FirstPage;
            meta.RequestPage(int.MaxValue-1);
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
            int i,index;
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
                    rowinit=0;
                    int shcountactual=System.Convert.ToInt32(shs.GetType().InvokeMember("Count",
                        System.Reflection.BindingFlags.GetProperty, null, shs, null));                    
                    if (shcountactual<shcount)
                    {
                        param1[0] = shcountactual;
                        object lastsh= shs.GetType().InvokeMember("Item",
                            System.Reflection.BindingFlags.GetProperty, null, shs, param1);                    

                        object[] param4 = new object[4];
                        param4[0] = DBNull.Value;
                        param4[1] = lastsh;
                        param4[2] = 1;
                        param4[3] = DBNull.Value;
                        sh=shs.GetType().InvokeMember("Add",
                         System.Reflection.BindingFlags.InvokeMethod, null, shs, param4);
                    }
                    else
                    {
                        param1[0]=shcount;
                        sh=shs.GetType().InvokeMember("Item",
                            System.Reflection.BindingFlags.GetProperty, null, shs, param1);
                    }
                }
                else
                {
                    rowinit=rowinit+rows.Count;
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
                            rows.Add(topstring,null);
                    }
                }
                // Finally, draw objects
                foreach (MetaObject obj in apage.Objects)
                {
                    if ((obj.MetaType == MetaObjectType.Text) || (obj.MetaType == MetaObjectType.Image))
                    {
                        PrintObject(sh, apage, obj, dpix, dpiy,
                         rows, columns, FontName, FontSize, rowinit,Precision);
                    }
                    nrecord++;
                    CheckProgress(false);
                }
            }
            EndDocument(meta);
            CheckProgress(true);


            if (FileName.Length > 0)
            {
                object[] paramssav = null;
                if (!CheckVersion2010Up(excel))
                {
                    paramssav = new object[1];
                }
                else
                {
                    paramssav = new object[2];
                    // Excel 97 format
                    ExcelFileFormat nformat = ExcelFormat;
                    if (nformat == ExcelFileFormat.Auto)
                        nformat = FileFormatFromFilename(FileName);
                    paramssav[1] = (int)nformat;
                }
                paramssav[0] = FileName;
                // If xlsx extension, force WorkBookNormal
                wb.GetType().InvokeMember("SaveAs", System.Reflection.BindingFlags.InvokeMethod, null, wb, paramssav);
            }
            if (!Visible)
            {
                object[] paramclose = new object[1];
                paramclose[0] = false;
                wb.GetType().InvokeMember("Close", System.Reflection.BindingFlags.InvokeMethod, null, wb, paramclose);

                excel.GetType().InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, excel, null);
                excel = null;
                wb = null;
                shs = null;
                sh = null;
            }
            else
            {
                param1[0] = Visible;
                excel.GetType().InvokeMember("Visible", System.Reflection.BindingFlags.SetProperty,
                    null, excel, param1);

            }
            return aresult;
        }
    }
}
