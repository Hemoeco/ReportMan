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
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using Reportman.Drawing;
using Reportman.Drawing.Forms;
using Reportman.Reporting;

namespace Reportman.Reporting.Forms
{
    /// <summary>
    /// Report processing driver allowing report properties alteration like parameters and page setup.
    /// This is the recommended driver for Windows.Forms applications
    /// </summary>
    /// <example>
    /// <code>
    /// Report rp=new Report();
    /// rp.LoadFromFile("test.rep");
    /// PrintOutReportWinForms prw=new PrintOutReportWinForms(rp);
    /// prw.Preview=true;
    /// prw.ShowPrintDialog=true;
    /// prw.Print(rp.MetaFile);
    /// </code>
    /// </example>
	public class PrintOutReportWinForms : PrintOutWinForms
	{
		private Report FReport;
		private AlterReportEvent AlterParamEvent;
#if REPMAN_MONO
#else
		private SendMailEvent MailEvent;
#endif
        private AlterReportEvent AlterPageEvent;
        /// <summary>
        /// Set this property to false to disable the option to show report parameters on preview window
        /// </summary>
		public bool PreviewParametersVisible;
        /// <summary>
        /// Set this property to false to disable the option to show page setup on preview window
        /// </summary>
        public bool PreviewPageSetupVisible;
        /// <summary>
        /// Get the report being processed
        /// </summary>
		public Report Report
		{
			get {return FReport;}
		}
    public override void Dispose()
    {
      base.Dispose();
      FReport = null;
    }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rp">Report to process</param>
		public PrintOutReportWinForms(Report rp):base()
		{
			FReport=rp;
			PreviewParametersVisible=true;
			PreviewPageSetupVisible=true;
			AlterParamEvent=new AlterReportEvent(AlterParams);
			AlterPageEvent=new AlterReportEvent(AlterPage);
#if REPMAN_MONO
#else
            MailEvent = new SendMailEvent(SendMailReport);
#endif
        }
		private void UpdateReport()
		{
			MetaFile meta = previewmetafile.MetaFile;
            previewmetafile.MetaFile = null;
            try
            {
                //			FReport.BeginPrint(this);
                FReport.EndPrint();
                FReport.MetaFile.Clear();
                PreparePrint(meta);
            }
            finally
            {
                previewmetafile.MetaFile = meta;
            }
		}
        private void SendMailReport(object sender, SendMailEventArgs args)
        {
            if (OnMail != null)
            {
                OnMail(sender, args);
                return;
            }
#if REPMAN_MONO
#else
			string mail_destination="";
            string mail_subject="";
            string mail_body="";
            string mail_filename = "";
            string filename = args.Filename;
            string short_file_name = System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(filename), ".PDF");
            if (FReport.Params.IndexOf("MAIL_DESTINATION") >= 0)
                mail_destination = FReport.Params["MAIL_DESTINATION"].Value.ToString();
            if (FReport.Params.IndexOf("MAIL_SUBJECT") >= 0)
                mail_subject = FReport.Params["MAIL_SUBJECT"].Value.ToString();
            if (FReport.Params.IndexOf("MAIL_BODY") >= 0)
                mail_body = FReport.Params["MAIL_BODY"].Value.ToString();
            if (FReport.Params.IndexOf("MAIL_FILE") >= 0)
                mail_filename = FReport.Params["MAIL_FILE"].Value.ToString();
            if (mail_filename.Length > 0)
            {
                short_file_name = mail_filename;
            }
            MAPI.SendMail(mail_subject, mail_body, null, null, mail_destination, "", short_file_name, filename, true);
#endif
        }
        private bool AlterPage(IWin32Window parent)
		{
			bool aresult=PageSetup.ShowPageSetup(FReport,false,parent);
			if (aresult)
				UpdateReport();
			return aresult;
		}
		private bool AlterParams(IWin32Window parent = null)
		{
            bool aresult=ParamsForm.ShowParams(FReport,parent);
			if (aresult)
				UpdateReport();
            return aresult;
            
		}
        /// <summary>
        /// Internally used to assign events for report alteration, report parameters and page setup
        /// </summary>
		override protected void SetReportEvents()
		{
			previewmetafile.OnReportParams+=AlterParamEvent;
			previewmetafile.OnPageSetup+=AlterPageEvent;
#if REPMAN_MONO
#else
			previewmetafile.OnMail += MailEvent;
#endif
		}
	}

    public class DataGridViewPrint
    {
        public static void SetAlignFromCellFormat(DataGridViewContentAlignment nalign,PrintItemText nitem)
        {
            switch (nalign)
            {
                case DataGridViewContentAlignment.BottomCenter:
                    nitem.Alignment = TextAlignType.Center;
                    nitem.VAlignment = TextAlignVerticalType.Bottom;
                    break;
                case DataGridViewContentAlignment.BottomLeft:
                    nitem.Alignment = TextAlignType.Left;
                    nitem.VAlignment = TextAlignVerticalType.Bottom;
                    break;
                case DataGridViewContentAlignment.BottomRight:
                    nitem.Alignment = TextAlignType.Right;
                    nitem.VAlignment = TextAlignVerticalType.Bottom;
                    break;
                case DataGridViewContentAlignment.MiddleCenter:
                    nitem.Alignment = TextAlignType.Center;
                    nitem.VAlignment = TextAlignVerticalType.Center;
                    break;
                case DataGridViewContentAlignment.MiddleLeft:
                    nitem.Alignment = TextAlignType.Left;
                    nitem.VAlignment = TextAlignVerticalType.Center;
                    break;
                case DataGridViewContentAlignment.MiddleRight:
                    nitem.Alignment = TextAlignType.Right;
                    nitem.VAlignment = TextAlignVerticalType.Center;
                    break;
                case DataGridViewContentAlignment.TopCenter:
                    nitem.Alignment = TextAlignType.Center;
                    nitem.VAlignment = TextAlignVerticalType.Top;
                    break;
                case DataGridViewContentAlignment.TopLeft:
                case DataGridViewContentAlignment.NotSet:
                    nitem.Alignment = TextAlignType.Left;
                    nitem.VAlignment = TextAlignVerticalType.Top;
                    break;
                case DataGridViewContentAlignment.TopRight:
                    nitem.Alignment = TextAlignType.Right;
                    nitem.VAlignment = TextAlignVerticalType.Top;
                    break;
            }
        }

        public static void PrepareReport(Report nreport,DataGridView ngrid, DataGridViewPrintOptions opts)
        {
            const int DefaultScreenDPI = 142;

            int BOOL_WIDTH = 100;
            int BOOL_SEP = 20;
            if (nreport.SubReports[0].Sections.Count == 1)
                nreport.SubReports[0].AddGroup("TOTAL");
            Section Detail = nreport.SubReports[0].Sections[nreport.SubReports[0].FirstDetail];
            Section GroupHeader = nreport.SubReports[0].Sections[nreport.SubReports[0].FirstDetail-1];
            Section GroupFooter = nreport.SubReports[0].Sections[nreport.SubReports[0].LastDetail + 1];
            string aliasname = nreport.DataInfo[0].Alias;
            int totalwidth = Detail.Width;

            int rowcount = ngrid.Rows.Count;
            if (opts.PrintSelected)
                rowcount = ngrid.SelectedRows.Count;
            SortedList<string,DataGridViewColumn> CampoColumna = new SortedList<string,DataGridViewColumn>();
            List<DataGridViewColumn> Columnas = new List<DataGridViewColumn>();
            SortedList<int,DataGridViewColumn> ColumnasString = new SortedList<int,DataGridViewColumn>();
            SortedList<int,DataGridViewColumn> ColumnasBool = new SortedList<int,DataGridViewColumn>();
            SortedList<int,DataGridViewColumn> ColumnasImage = new SortedList<int,DataGridViewColumn>();
            List<string> Campos = new List<string>();
            int idxcol = 0;
            if (opts.SelectedColumns.Count == 0)
            {
                foreach (DataGridViewColumn ncol in ngrid.Columns)
                {
                    if (ncol.Visible)
                    {
                        Columnas.Add(ncol);
                        idxcol++;
                        CampoColumna.Add("CAMPO"+idxcol.ToString(),ncol);
                        Campos.Add("CAMPO"+idxcol.ToString());
                    }
                }
            }
            else
            {
                foreach (DataGridViewColumn ncol in opts.SelectedColumns)
                {
                    Columnas.Add(ncol);
                    idxcol++;
                    CampoColumna.Add("CAMPO"+idxcol.ToString(),ncol);
                    Campos.Add("CAMPO"+idxcol.ToString());
                }
            }
            int separator = 40;
            int level_separator = 200;
            int level_separator_gap = 50;
            // Calculate total width for the selected
            int maxwidth = 0;
            float screendpi = GraphicUtils.ScreenDPI();
            float scalepixels = 1440f / screendpi;
            for (idxcol = 0; idxcol < Columnas.Count; idxcol++)
            {
                DataGridViewColumn ncol = Columnas[idxcol];
                int newwidth = System.Convert.ToInt32(Math.Round(ncol.Width * scalepixels));
                maxwidth = maxwidth +newwidth;

                if (idxcol < (Columnas.Count - 1))
                    maxwidth = maxwidth + separator;
            }

            List<ShapeItem> HorzShapes = new List<ShapeItem>();
            int pagewidth = opts.PaperWidth();
            pagewidth = pagewidth-nreport.LeftMargin-nreport.RightMargin;
            float fsize = ngrid.Font.Size;
            float fontscale = 1.0f;
            float scalesep = 1.0f;
            float newscale = 1.0f;
            if (opts.AdjustWidth)
            {
                newscale = (float)pagewidth / maxwidth;
                scalepixels = 1440f / screendpi*newscale;
                scalesep = newscale;
                if (maxwidth > pagewidth)
                {
                    fontscale = newscale;
                    fsize = fsize * fontscale;
                    if (fsize <= 0)
                        fsize = 1;
                }
            }
            // Calculate font size and width scale
            int rowheight = System.Convert.ToInt32(Math.Round(ngrid.RowTemplate.Height*scalepixels));
            BOOL_WIDTH = System.Convert.ToInt32(BOOL_WIDTH * fontscale);
            BOOL_SEP = System.Convert.ToInt32(BOOL_SEP * fontscale);
            separator = System.Convert.ToInt32(separator * fontscale);
            level_separator = System.Convert.ToInt32(level_separator * fontscale);
            level_separator_gap = System.Convert.ToInt32(level_separator_gap * fontscale);
            GroupHeader.Height = rowheight;
            Detail.Height = rowheight;
            // Prepare datatable
            DataTable ntable = new DataTable();
            idxcol = 0;
            int posx = 0;
            // Grey header
            ShapeItem sitem = new ShapeItem(nreport);
            sitem.Shape = ShapeType.Rectangle;
            sitem.BrushStyle = BrushType.Solid;
            sitem.PenStyle = Reportman.Drawing.PenType.Clear;
            sitem.BrushColor = GraphicUtils.IntegerFromColor(Color.FromArgb(200,200,200));
            sitem.Height = rowheight;
            sitem.Align = PrintItemAlign.TopBottom;
            HorzShapes.Add(sitem);
            nreport.GenerateNewName(sitem);
            GroupHeader.Components.Add(sitem);
            

            sitem = new ShapeItem(nreport);
            sitem.Shape = ShapeType.VertLine;
            sitem.Height = rowheight;
            sitem.Align = PrintItemAlign.TopBottom;
            nreport.GenerateNewName(sitem);
            GroupHeader.Components.Add(sitem);


            if (opts.DrawBackGroundColors)
            {
                // Background color
                sitem = new ShapeItem(nreport);
                sitem.PrintCondition = aliasname + ".BACKCOLOR<>NULL";
                sitem.BrushColorExpression = aliasname + ".BACKCOLOR";
                sitem.Shape = ShapeType.Rectangle;
                sitem.Width = maxwidth;
                sitem.PenStyle = Reportman.Drawing.PenType.Clear;
                sitem.PosX = posx;
                sitem.Align = PrintItemAlign.TopBottom;
                Detail.Components.Add(sitem);
                HorzShapes.Add(sitem);
            }

            sitem = new ShapeItem(nreport); ;
            sitem.Shape = ShapeType.HorzLine;
            sitem.Width = maxwidth;
            HorzShapes.Add(sitem);
            nreport.GenerateNewName(sitem);
            GroupHeader.Components.Add(sitem);


            sitem = new ShapeItem(nreport); ;
            sitem.Shape = ShapeType.HorzLine;
            sitem.Width = maxwidth;
            HorzShapes.Add(sitem);
            nreport.GenerateNewName(sitem);
            GroupFooter.Components.Add(sitem);

            sitem = new ShapeItem(nreport); ;
            sitem.Shape = ShapeType.HorzLine;
            sitem.Width = maxwidth;
            HorzShapes.Add(sitem);
            sitem.Align = PrintItemAlign.Bottom;
            nreport.GenerateNewName(sitem);
            GroupHeader.Components.Add(sitem);

            if (opts.VerticalLines)
            {
                sitem = new ShapeItem(nreport);
                sitem.Shape = ShapeType.VertLine;
                sitem.Height = rowheight;
                sitem.Align = PrintItemAlign.TopBottom;
                nreport.GenerateNewName(sitem);
                Detail.Components.Add(sitem);
            }

            string columnlevel = "";


            for (idxcol = 0; idxcol < Columnas.Count; idxcol++)
            {
                DataGridViewColumn ncol = Columnas[idxcol];
                DataGridViewContentAlignment cellalign = DataGridViewContentAlignment.NotSet;
                if (ngrid.DefaultCellStyle != null)
                    cellalign = ngrid.DefaultCellStyle.Alignment;
                if (ncol.DefaultCellStyle != null)
                {
                    cellalign = ncol.DefaultCellStyle.Alignment;
                }
                LabelItem litem = new LabelItem(nreport);
                SetAlignFromCellFormat(cellalign, litem);
                litem.VAlignment = TextAlignVerticalType.Top;
                if (litem.Alignment == TextAlignType.Left)
                    litem.Alignment = TextAlignType.Justify;
                litem.Height = rowheight;
                litem.FontSize = System.Convert.ToInt16(fsize);
                nreport.GenerateNewName(litem);
                litem.Text = ncol.HeaderText;
                GroupHeader.Components.Add(litem);
                litem.Height = rowheight;
                litem.PosX = posx;
                litem.WordWrap = opts.WordWrap;
                litem.CutText = !opts.WordWrap;
                litem.Width = System.Convert.ToInt32(Math.Round(ncol.Width * scalepixels));


                bool istextcolumn = false;
                istextcolumn = ncol is DataGridViewTextBoxColumn;
                if (!istextcolumn)
                {
                    if (ncol is DataGridViewColumnAdvanced)
                    {
                        istextcolumn = true;
                    }
                }
                TreeGridAdvanced ngridav;
                if (istextcolumn)
                {
                    ntable.Columns.Add(Campos[idxcol], System.Type.GetType("System.String"));
                    ColumnasString.Add(idxcol, ncol);
                    if ((ncol is TreeGridAdvancedColumn) && (ngrid is TreeGridAdvanced))
                    {
                        if (columnlevel == "")
                            columnlevel = "LEVEL";
                        ngridav = (TreeGridAdvanced)ngrid;
                        for (int idxlevel = 1;idxlevel<=ngridav.MaxLevel;idxlevel++)
                        {
                            // Add Image for level
                            ImageItem imaitem = new ImageItem(nreport);
                            imaitem.Expression = aliasname + ".IMAGE" ;
                            imaitem.PrintCondition = aliasname + ".LEVEL=" + idxlevel.ToString();
                            imaitem.Height = rowheight;
                            imaitem.Align = PrintItemAlign.TopBottom;
                            imaitem.Width = level_separator;
                            imaitem.PosX = level_separator_gap+posx + level_separator * (idxlevel) - level_separator+level_separator_gap;
                            imaitem.PosY = 0;
                            imaitem.DrawStyle = ImageDrawStyleType.Full;
                            imaitem.dpires = System.Convert.ToInt32(DefaultScreenDPI);
                            nreport.GenerateNewName(imaitem);


                            Detail.Components.Add(imaitem);
   
                            // Add expression foreach level
                            ExpressionItem exitem = new ExpressionItem(nreport);
                            exitem.Expression = aliasname + "." + Campos[idxcol];
                            exitem.PrintCondition = aliasname + ".LEVEL="+idxlevel.ToString();
                            exitem.WordWrap = opts.WordWrap;
                            exitem.CutText = !opts.WordWrap;
                            SetAlignFromCellFormat(cellalign, exitem);
                            if (exitem.Alignment == TextAlignType.Left)
                                exitem.Alignment = TextAlignType.Justify;
                            exitem.VAlignment = TextAlignVerticalType.Top;
                            exitem.FontSize = System.Convert.ToInt16(fsize);
                            exitem.Height = rowheight;
                            exitem.PosX = level_separator_gap*2+posx+level_separator*idxlevel;
                            exitem.Height = rowheight;
                            nreport.GenerateNewName(exitem);
                            exitem.Width = litem.Width - level_separator * (idxlevel + 1);
                            Detail.Components.Add(exitem);


                            if (opts.DrawTreeLines)
                            {
                                // Last child line
                                sitem = new ShapeItem(nreport);
                                sitem.PrintCondition = "((" + aliasname + ".LEVEL=" + idxlevel.ToString() + ") AND (" + aliasname + ".LASTCHILD))";
                                sitem.Shape = ShapeType.VertLine;
                                sitem.PenStyle = Reportman.Drawing.PenType.Dot;
                                sitem.PosX = level_separator_gap + posx + level_separator * idxlevel - level_separator;
                                sitem.Height = rowheight / 3;
                                Detail.Components.Add(sitem);

                                // Not last child horz line
                                sitem = new ShapeItem(nreport);
                                sitem.PrintCondition = "((" + aliasname + ".LEVEL=" + idxlevel.ToString() + ") AND (NOT (" + aliasname + ".LASTCHILD)))";
                                sitem.Shape = ShapeType.VertLine;
                                sitem.PenStyle = Reportman.Drawing.PenType.Dot;
                                sitem.PosX = level_separator_gap + posx + level_separator * idxlevel - level_separator;
                                sitem.Align = PrintItemAlign.TopBottom;
                                Detail.Components.Add(sitem);

                                // Other parent lines
                                sitem = new ShapeItem(nreport);
                                sitem.PrintCondition = aliasname + ".LEVEL>" + idxlevel.ToString();
                                sitem.Shape = ShapeType.VertLine;
                                sitem.Align = PrintItemAlign.TopBottom;
                                sitem.PenStyle = Reportman.Drawing.PenType.Dot;
                                sitem.PosX = level_separator_gap + posx + level_separator * idxlevel - level_separator;
                                sitem.Height = rowheight / 2;
                                Detail.Components.Add(sitem);

                                // Horz arrow
                                sitem = new ShapeItem(nreport);
                                sitem.PrintCondition = aliasname + ".LEVEL=" + idxlevel.ToString();
                                sitem.PenStyle = Reportman.Drawing.PenType.Dot;
                                sitem.Shape = ShapeType.HorzLine;
                                sitem.PosX = level_separator_gap + posx + level_separator * idxlevel - level_separator;
                                sitem.PosY = rowheight / 3;
                                sitem.Width = level_separator / 2;
                                Detail.Components.Add(sitem);
                            }

                        }
                    }
                    else
                    {
                        ExpressionItem exitem = new ExpressionItem(nreport);
                        exitem.Expression = aliasname + "." + Campos[idxcol];
                        exitem.WordWrap = opts.WordWrap;
                        exitem.CutText = !opts.WordWrap;
                        SetAlignFromCellFormat(cellalign, exitem);
                        if (exitem.Alignment == TextAlignType.Left)
                            exitem.Alignment = TextAlignType.Justify;
                        exitem.VAlignment = TextAlignVerticalType.Top;
                        exitem.FontSize = System.Convert.ToInt16(fsize);
                        exitem.Height = rowheight;
                        exitem.PosX = posx;
                        exitem.Height = rowheight;
                        nreport.GenerateNewName(exitem);
                        exitem.Width = litem.Width;


                        Detail.Components.Add(exitem);
                    }
                }
                else
                {
                    if (ncol is DataGridViewImageColumn)
                    {
                        ColumnasImage.Add(idxcol,ncol);
                        ntable.Columns.Add(Campos[idxcol], System.Type.GetType("System.Object"));

                        ImageItem imaitem = new ImageItem(nreport);
                        imaitem.Expression = aliasname + "." + Campos[idxcol];
                        imaitem.Height = rowheight;
                        imaitem.Align = PrintItemAlign.TopBottom;
                        imaitem.Width = litem.Width;
                        imaitem.PosX = posx+separator;
                        imaitem.PosY = 0;
                        imaitem.DrawStyle = ImageDrawStyleType.Full;
                        imaitem.dpires = System.Convert.ToInt32(DefaultScreenDPI);
                        nreport.GenerateNewName(imaitem);


                        Detail.Components.Add(imaitem);
                    }
                    else
                        if (ncol is DataGridViewCheckBoxColumn)
                        {
                            ColumnasBool.Add(idxcol,ncol);
                            ntable.Columns.Add(Campos[idxcol], System.Type.GetType("System.Boolean"));

                            ShapeItem cuaditem = new ShapeItem(nreport);
                            cuaditem.Shape = ShapeType.Rectangle;
                            cuaditem.PrintCondition = aliasname + "." + Campos[idxcol]+"<>NULL";
                            //cuaditem.Align = PrintItemAlign.TopBottom;
                            cuaditem.BrushStyle = BrushType.Clear;
                            cuaditem.Height = rowheight;
                            int shapepos = posx + (litem.Width / 2) - BOOL_WIDTH/2;
                            cuaditem.PosX = shapepos;
                            cuaditem.PosY = BOOL_SEP;
                            cuaditem.Width = BOOL_WIDTH;
                            cuaditem.Height = BOOL_WIDTH;
                            nreport.GenerateNewName(cuaditem);
                            Detail.Components.Add(cuaditem);

                            cuaditem = new ShapeItem(nreport);
                            cuaditem.Shape = ShapeType.Oblique1;
                            cuaditem.PrintCondition = aliasname + "." + Campos[idxcol];
                            //cuaditem.Align = PrintItemAlign.TopBottom;
                            cuaditem.BrushStyle = BrushType.Clear;
                            cuaditem.Height = rowheight;
                            cuaditem.PosX = shapepos;
                            cuaditem.PosY = BOOL_SEP;
                            cuaditem.Width = BOOL_WIDTH;
                            cuaditem.Height = BOOL_WIDTH;
                            nreport.GenerateNewName(cuaditem);
                            Detail.Components.Add(cuaditem);


                            cuaditem = new ShapeItem(nreport);
                            cuaditem.Shape = ShapeType.Oblique2;
                            cuaditem.PrintCondition = aliasname + "." + Campos[idxcol];
                            //cuaditem.Align = PrintItemAlign.TopBottom;
                            cuaditem.BrushStyle = BrushType.Clear;
                            cuaditem.Height = rowheight;
                            cuaditem.PosX = shapepos;
                            cuaditem.PosY = BOOL_SEP;
                            cuaditem.Width = BOOL_WIDTH;
                            cuaditem.Height = BOOL_WIDTH;
                            nreport.GenerateNewName(cuaditem);
                            Detail.Components.Add(cuaditem);

                        }
                            else
                                throw new Exception("Tipo de columna no soportada:" + ncol.GetType().ToString());
                }

 

                posx = posx + litem.Width;
                posx = posx + System.Convert.ToInt32(separator * scalesep);


                if (opts.VerticalLines)
                {
                    sitem = new ShapeItem(nreport);
                    sitem.Shape = ShapeType.VertLine;
                    sitem.PosX = posx;
                    sitem.Height = rowheight;
                    sitem.Align = PrintItemAlign.TopBottom;
                    Detail.Components.Add(sitem);
                }

                sitem = new ShapeItem(nreport);
                sitem.Shape = ShapeType.VertLine;
                sitem.PosX = posx;
                sitem.Height = rowheight;
                sitem.Align = PrintItemAlign.TopBottom;
                GroupHeader.Components.Add(sitem);

                if (opts.HorizontalLines)
                {
                    sitem = new ShapeItem(nreport);
                    sitem.Shape = ShapeType.HorzLine;
                    sitem.Width = maxwidth;
                    HorzShapes.Add(sitem);
                    //sitem.Align = PrintItemAlign.Top;
                    Detail.Components.Add(sitem);
                    
                    sitem = new ShapeItem(nreport);
                    sitem.PrintCondition = "CURRENTGROUP=0";
                    sitem.Shape = ShapeType.HorzLine;
                    sitem.Width = maxwidth;
                    HorzShapes.Add(sitem);
                    sitem.Align = PrintItemAlign.Bottom;
                    Detail.Components.Add(sitem);

                }
                                

            }
            foreach (ShapeItem nshape in HorzShapes)
            {
                nshape.Width = posx;
            }
            int colbackground = 0;
            if (opts.DrawBackGroundColors)
            {
                ntable.Columns.Add("BACKCOLOR", System.Type.GetType("System.Int32"));
                colbackground = ntable.Columns.Count - 1;
            }
            if (columnlevel != "")
            {
                ntable.Columns.Add("IMAGE", System.Type.GetType("System.Object"));
                ntable.Columns.Add("LEVEL", System.Type.GetType("System.Int32"));
                ntable.Columns.Add("LASTCHILD", System.Type.GetType("System.Boolean"));
            }
            object[] nvalues = new object[ntable.Columns.Count];
            int counter = 0;
            foreach (DataGridViewRow dgrow in ngrid.Rows)
            {
                if ((!opts.PrintSelected) || ((opts.PrintSelected) && (dgrow.Selected)))
                {
                    foreach (int index in ColumnasString.Keys)
                    {
                        nvalues[index] = dgrow.Cells[ColumnasString[index].Index].FormattedValue;
                    }
                    foreach (int index in ColumnasBool.Keys)
                    {
                        nvalues[index] = dgrow.Cells[ColumnasBool[index].Index].Value;
                    }
                    foreach (int index in ColumnasImage.Keys)
                    {
                        MemoryStream mstream = GetImageStreamFromObject(dgrow.Cells[ColumnasImage[index].Index].Value);
                        if (mstream != null)
                            nvalues[index] = mstream;
                        else
                            nvalues[index] = DBNull.Value;
                    }
                    if (opts.DrawBackGroundColors)
                    {
                        DataGridViewCellStyle nstyle = null;
                        if (!dgrow.DefaultCellStyle.BackColor.IsEmpty)
                         nstyle = dgrow.DefaultCellStyle;
                        if (nstyle==null)
                        {
                            if ((!ngrid.AlternatingRowsDefaultCellStyle.BackColor.IsEmpty))
                            {
                                if (counter % 2 == 1)
                                {
                                    nstyle = ngrid.AlternatingRowsDefaultCellStyle;
                                }
                                else
                                    if (!ngrid.DefaultCellStyle.BackColor.IsEmpty)
                                        nstyle = ngrid.DefaultCellStyle;
                            }
                            else
                            {
                                if (!ngrid.DefaultCellStyle.BackColor.IsEmpty)
                                    nstyle = ngrid.AlternatingRowsDefaultCellStyle;
                            }
                        }
                        nvalues[colbackground] = DBNull.Value;
                        if (nstyle != null)
                        {
                            if (nstyle.BackColor != Color.White)
                                if (nstyle.BackColor != Color.FromArgb(255, 255, 255))
                                {
                                    nvalues[colbackground] = GraphicUtils.IntegerFromColor(nstyle.BackColor);
                                }
                        }
                    }
                    if (columnlevel.Length > 0)
                    {
                        TreeGridRow trow = (TreeGridRow)dgrow;
                        MemoryStream mstream = GetImageStreamFromObject(trow.Image);
                        if (mstream != null)
                            nvalues[nvalues.Length - 3] = mstream;
                        nvalues[nvalues.Length - 2] = trow.Level;
                        nvalues[nvalues.Length - 1] = trow.IsLastSibling;
/*
                        if (trow.Parent != null)
                            nvalues[nvalues.Length - 1] = ((trow.Node.Childs.Count == 0) || (!trow.Node.Expanded)) && (((TreeGridRow)dgrow).ChildIndex + 1) == ((TreeGridRow)dgrow).Parent.Node.Childs.Count;
                        else
                        {
                            nvalues[nvalues.Length - 1] = ((trow.Node.Childs.Count == 0) || (!trow.Node.Expanded)) && (((TreeGridAdvanced)ngrid).MainNode.Childs.Count == trow.ChildIndex + 1);
                        }*/
                        
                    }
                    ntable.Rows.Add(nvalues);
                    counter++;
                }
            }
            nreport.DataInfo[aliasname].DataViewOverride = new DataView(ntable);
            //return meta;
        }
        public static MemoryStream GetImageStreamFromObject(object nobj)
        {
            MemoryStream mstream = null;
            if ((nobj != DBNull.Value) && (nobj != null))
            {

                Image img = null;
                if (nobj is Image)
                {
                    img = (Image)nobj;
                }
                else
                    if (nobj is Icon)
                    {
                        img = ((Icon)nobj).ToBitmap();
                    }
                if (img != null)
                {
                    using (Bitmap nbitmap = new Bitmap(img.Width, img.Height))
                    {
                        using (Graphics gr = Graphics.FromImage(nbitmap))
                        {
                            gr.FillRectangle(Brushes.White, new Rectangle(0, 0, img.Width, img.Height));
                            gr.DrawImage(img, 0, 0);
                        }
                        mstream = new MemoryStream();
                        nbitmap.Save(mstream, ImageFormat.Bmp);
                    }
                }
            }
            return mstream;

        }
    }
    public class DataGridViewPrintOptions
    {
        public bool AdjustWidth;
        public List<DataGridViewColumn> SelectedColumns;
        public PaperSize PaperSize;
        public bool LandsCape;
        public bool PrintSelected;
        public bool WordWrap;
        public Report Report;
        public bool Preview;
        public bool DrawTreeLines;
        public bool DrawBackGroundColors;
        public bool HorizontalLines;
        public bool VerticalLines;
        public DataGridViewPrintOptions()
        {
            SelectedColumns = new List<DataGridViewColumn>();
            PaperSize = PrintOutPrint.PaperSizeFromPageIndex(0);
            WordWrap = true;
            DrawTreeLines = true;
            DrawBackGroundColors = true;
            HorizontalLines = true;
            VerticalLines = true;
        }
        public int PaperWidth()
        {
            if (LandsCape)
                return PaperSize.Height * 1440 / 100;
            else
                return PaperSize.Width * 1440 / 100;
        }
    }

}
