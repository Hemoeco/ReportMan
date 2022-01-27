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
using System.Windows.Forms;
using Reportman.Drawing;

namespace Reportman.Drawing.Forms
{
    /// <summary>
    /// Report processing driver using Windows.Forms, it can preview, print, show a print dialog.
    /// This driver does not support interaction with the Report, like altering Report parameters in
    /// preview window. But is useful when working the already generated MetaFiles.
    /// The PrintOutReportWinForms at Reportman.Reporting.Forms provides the full interactive functionality
    /// like page setup or altering parameters at preview window.
    /// </summary>
    /// <example>
    /// <code>
    /// Report rp=new Report();
    /// rp.LoadFromFile("test.rep");
    /// PrintOutWinForms prw=new PrintOutWinForms;
    /// prw.Print(rp.MetaFile);
    /// </code>
    /// </example>
    public class PrintOutWinForms : PrintOutPrint
    {
#if REPMAN_MONO
#else
        private SendMailEvent MailEvent;
#endif
        public SendMailEvent OnMail;
        /// <summary>
        /// By default GDI+ is ude, if this is set to false the Internal GDI functions will be used
        /// </summary>
        public bool UseGDIPlus = true;
        public PrintOutWinForms()
          : base()
        {
            ShowEmptyReportMessage = true;
            MailEvent = new SendMailEvent(SendMailPdf);
            WindowMode = PreviewWindowMode.ModalForm;
        }
        public bool LargeButtons;
        private bool FPreview;
        private bool FSystemPreview;
        /// <summary>
        /// Set it to true to show an incon in task bar for the preview window
        /// </summary>
        public bool ShowInTaskbar;
        /// <summary>
        /// Set preview if you want the output on the screen
        /// </summary>
        public bool Preview
        {
            get
            {
                return FPreview;
            }
            set
            {
                FPreview = value;
                DrawFound = ((FPreview) && (FSystemPreview));
            }
        }
        /// <summary>
        /// Show a message when the report is empty (no data available to print)
        /// </summary>
        public bool ShowEmptyReportMessage;
        /// <summary>
        /// When the output is the printer, before processing, a standard print dialog will be shown to the user
        /// </summary>
        public bool ShowPrintDialog;
        /// <summary>
        /// Set this property to true, combined with Preview, to show the standard system preview dialog,
        /// instead of the Report Manager preview dialog.
        /// </summary>
        public bool SystemPreview
        {
            get
            {
                return FSystemPreview;
            }
            set
            {
                FSystemPreview = value;
                DrawFound = ((FPreview) && (FSystemPreview));
            }
        }
        public PreviewWindowMode WindowMode;
        public PreviewWinFormsControl PreviewWindow
        {
            get
            {
                if (FPreviewWindow == null)
                    FPreviewWindow = new PreviewWinFormsControl();
                return FPreviewWindow;
            }
        }
        /// <summary>
        /// Internal PreviewWinForms
        /// </summary>
        protected PreviewWinFormsControl FPreviewWindow;
        /// <summary>
        /// Internal PreviewMetaFile
        /// </summary>
        protected PreviewMetaFile previewmetafile;
        /// <summary>
        /// Used in derived versions to manager events for report alteration (page setup and parameters)
        /// </summary>
        virtual protected void SetReportEvents()
        {
#if REPMAN_MONO
#else
            previewmetafile.OnMail += MailEvent;
#endif
        }

        /// <summary>
        /// When the user does not handle the no data to print event. A message is shown
        /// </summary>
        override protected void ProcessNoDataToPrint()
        {
            base.ProcessNoDataToPrint();
            if (ShowEmptyReportMessage)
                MessageBox.Show(Translator.TranslateStr(799));
        }
        public bool ExecutePrintDialog()
        {
            using (PrintDialog pdia = new PrintDialog())
            {
                pdia.ShowHelp = true;
                pdia.ShowNetwork = true;
                pdia.AllowSomePages = true;
                // bug from Microsoft not showing dialog
                //pdia.UseEXDialog = true;
                pdia.Document = doc;
                DialogResult aresult = pdia.ShowDialog();
                return aresult == DialogResult.OK;
            }
        }
        public static bool SelectDefaultPrinter()
        {
            using (PrintDialog pdia = new PrintDialog())
            {
                pdia.ShowHelp = true;
                pdia.ShowNetwork = true;
                pdia.AllowSomePages = false;
                pdia.AllowSelection = false;
                pdia.AllowCurrentPage = false;
                // bug from Microsoft not showing dialog
                //pdia.UseEXDialog = true;
                pdia.Document = new PrintDocument();
                if (DefaultPrinterName.Length >= 0)
                {
                    pdia.Document.PrinterSettings.PrinterName = DefaultPrinterName;
                }
                DialogResult aresult = pdia.ShowDialog();
                if (aresult == DialogResult.OK)
                {
                    DefaultPrinterName = pdia.Document.PrinterSettings.PrinterName;
                }
                return (aresult == DialogResult.OK);
            }
        }
        /// <summary>
        /// Process a MetaFile, printing or previewing
        /// </summary>
        /// <param name="meta">MetaFile to print</param>
        override public bool Print(MetaFile meta)
        {
            overridedriver = null;
            DrawFound = (FPreview && (!SystemPreview));

            if (!FPreview)
            {
                // Get printer data so it can be a dot matrix print option
                string drivername = PrinterConfig.GetDriverName(meta.PrinterSelect);
                if (drivername.Length > 0)
                {
                    using (PrintOutText ntextdriver = new PrintOutText())
                    {
                        ntextdriver.ForceDriverName = drivername;
                        overridedriver = ntextdriver;
                        if (PreparePrint(meta))
                        {
                            bool doprint = true;
                            if (ShowPrintDialog)
                            {
                                using (PrintDialog pdia = new PrintDialog())
                                {
                                    pdia.ShowHelp = true;
                                    pdia.ShowNetwork = true;
                                    pdia.AllowSomePages = true;
                                    // bug from Microsoft not showing dialog
                                    //pdia.UseEXDialog = true;
                                    pdia.Document = doc;
                                    DialogResult aresult = pdia.ShowDialog();
                                    doprint = aresult == DialogResult.OK;
                                }
                            }
                            if (doprint)
                            {
                                ntextdriver.Print(meta);
                                NewDocument(meta);

                                string atext = ntextdriver.PrintResult.ToString();
                                //MessageBox.Show("Blank lines:"+ntextdriver.blanklines.ToString());
                                if (atext.Length > 0)
                                {
                                    FBlackLines = FBlackLines + ntextdriver.blacklines;
                                    FWhiteLines = FWhiteLines + ntextdriver.whitelines;

                                    RawPrinterHelper.SendStringToPrinter(doc.PrinterSettings.PrinterName, atext);
                                    return true;
                                }
                                EndDocument(meta);
                            }
                            else
                                return false;
                        }
                    }
                }
            }



            bool nresult = false;
            WMFOptimization oldoptimization = OptimizeWMF;
            try
            {
                DisableForwardOnly = false;
                if (!Preview)
                    OptimizeWMF = WMFOptimization.None;
                else
                {
                    // For preview, the metafile must save all pages
                    if (!SystemPreview)
                        DisableForwardOnly = true;
                }
                if (PreparePrint(meta))
                {
                    bool doprint = true;
                    if (ShowPrintDialog)
                    {
                        using (PrintDialog pdia = new PrintDialog())
                        {
                            // Bug from microsft not showing dialog in 64bit machines
                            //pdia.UseEXDialog = true;
                            pdia.ShowHelp = true;
                            pdia.ShowNetwork = true;
                            pdia.AllowSomePages = true;
                            pdia.Document = doc;
                            DialogResult aresult = pdia.ShowDialog();
                            doprint = aresult == DialogResult.OK;
                        }
                    }
                    if (doprint)
                    {
                        if (!Preview)
                        {
                            meta.ForwardOnly = true;
                            try
                            {
                                doc.Print();
                            }
                            catch (Exception E)
                            {
                                string printername = doc.PrinterSettings.PrinterName;
                                throw new Exception("Printer error: " + printername + " " + E.Message);
                            }
                        }
                        else
                        {
#if REPMAN_NOPREVIEW
						throw new Exception("Feature PrintPreviewDialog not supported");
#else
                            if (SystemPreview)
                            {
                                using (PrintPreviewDialog previewdia = new PrintPreviewDialog())
                                {
                                    previewdia.Document = doc;
                                    previewdia.ShowDialog();
                                }
                            }
                            else
                            {
                                if (previewmetafile != null)
                                {
                                    previewmetafile.Dispose();
                                    previewmetafile = null;
                                }
                                previewmetafile = new PreviewMetaFile();
                                SetReportEvents();
                                previewmetafile.OptimizeWMF = OptimizeWMF;
                                previewmetafile.MetaFile = meta;
                                previewmetafile.SetDriver(this);
                                PreviewWindow.WindowMode = WindowMode;
                                PreviewWindow.ShowInTaskbar = ShowInTaskbar;
                                PreviewWindow.LargeButtons = LargeButtons;
                                PreviewWindow.PreviewReport(previewmetafile);
                            }
#endif
                        }
                        nresult = true;
                    }
                }
            }
            finally
            {
                OptimizeWMF = oldoptimization;
            }
            return nresult;
        }
        public static TextFormatFlags FormatFlatsToTextFormatFlags(StringFormat oldflags)
        {
            TextFormatFlags newflags = TextFormatFlags.Default;
            if (oldflags.LineAlignment == StringAlignment.Center)
                newflags = TextFormatFlags.HorizontalCenter;
            else
              if (oldflags.LineAlignment == StringAlignment.Far)
                newflags = TextFormatFlags.Right;
            if (oldflags.Alignment == StringAlignment.Center)
                newflags = newflags | TextFormatFlags.VerticalCenter;
            else
              if (oldflags.Alignment == StringAlignment.Far)
                newflags = newflags | TextFormatFlags.Bottom;
            else
                newflags = newflags | TextFormatFlags.Top;
            if (oldflags.Trimming != StringTrimming.None)
                newflags = newflags | TextFormatFlags.NoClipping;
            if ((oldflags.FormatFlags | StringFormatFlags.NoWrap) == 0)
                newflags = newflags | TextFormatFlags.WordBreak;

            return newflags;
        }
        public override void DrawString(Graphics gr, string atext, Font font, Brush brush, Rectangle arec, StringFormat sformat)
        {
            //gr.PageUnit = GraphicsUnit.Point;
            //gr.PageUnit = GraphicsUnit.Point;
            if (UseGDIPlus)
                base.DrawString(gr, atext, font, brush, arec, sformat);
            else
                TextRenderer.DrawText(gr, atext, font, arec, Color.Black, Color.Transparent, FormatFlatsToTextFormatFlags(sformat));
        }
        public override void DrawString(Graphics gr, string atext, Font font, Brush brush, float posx, float posy, StringFormat sformat)
        {
            if (UseGDIPlus)
                base.DrawString(gr, atext, font, brush, posx, posy, sformat);
            else
                TextRenderer.DrawText(gr, atext, font, new Point((int)Math.Round(posx), (int)Math.Round(posy)), Color.Black, Color.Transparent, FormatFlatsToTextFormatFlags(sformat));
        }
        public override SizeF MeasureString(Graphics gr, string atext, Font font, SizeF layoutarea, StringFormat sformat, out int charsfit, out int linesfit)
        {
            charsfit = atext.Length;
            linesfit = atext.Length;
            if (UseGDIPlus)
                return base.MeasureString(gr, atext, font, layoutarea, sformat, out charsfit, out linesfit);
            else
            {
                Size nsize = TextRenderer.MeasureText(gr, atext, font, new Size((int)Math.Round(layoutarea.Width), (int)Math.Round(layoutarea.Height)), FormatFlatsToTextFormatFlags(sformat));
                return new SizeF(nsize.Width, nsize.Height);
            }
        }

        private void SendMailPdf(object sender, SendMailEventArgs args)
        {
            if (OnMail != null)
            {
                OnMail(sender, args);
                return;
            }
#if REPMAN_MONO
#else
            string mail_destination = args.To;
            string mail_subject = args.Subject;
            string mail_body = args.Body;
            string mail_filename = "";
            string filename = args.Filename;
            string short_file_name = System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(filename), ".PDF");
            if (mail_filename.Length > 0)
            {
                short_file_name = mail_filename;
            }
            MAPI.SendMail(mail_subject, mail_body, null, null, mail_destination, "", short_file_name, filename, true);
#endif
        }
        public override void Dispose()
        {
            if (FPreviewWindow != null)
            {
                FPreviewWindow.Dispose();
                FPreviewWindow = null;
            }
            if (previewmetafile != null)
            {
                previewmetafile.Dispose();
                previewmetafile = null;
            }
            base.Dispose();
        }
        public const int MAX_SERIECOLORS = 21;
        static int[] SeriesColors =
            {0xFF0000,0xFF22FF,0x00FF00,0x0000FF,0xFFFF00,0xFF033F,0x00FFFF,
            0xAAAAAA,0xBB0000,0x00BB00,0x0000BB,0xBBBB00,0xBB00BB,0x00BBBB,
            0x777777,0x773333,0x337733,0x333377,0x777700,0x770077,0x007777};

        public static void AddFunctionToChart(
            System.Windows.Forms.DataVisualization.Charting.Chart nchart ,
            System.Windows.Forms.DataVisualization.Charting.Series serie, SeriesItem sitem)
        {
            int serieIndex = nchart.Series.Count - 1;
            while (serieIndex < nchart.Series.Count)
            {
                if (nchart.Series[serieIndex].Tag != null)
                    serieIndex--;
                else
                    break;
            }
            var sourceSerie = nchart.Series[serieIndex];
            if (serieIndex < 0)
                return;
            switch (sitem.FunctionName)
            {
                case "MEDIAN":
                    double valorMedio = nchart.DataManipulator.Statistics.Median(sourceSerie.Name);
                    sitem.ChartStyle = ChartType.Line;
                    serie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    //serie.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                    foreach (var sourcePoint in sourceSerie.Points)
                    {
                        var point = serie.Points.Add(valorMedio);
                    }
                    break;
                case "MIN":
                    double valorMin = double.MaxValue;
                    foreach (var sourcePoint in sourceSerie.Points)
                    {
                        if (sourcePoint.YValues[0] < valorMin)
                            valorMin = sourcePoint.YValues[0];
                    }
                    foreach (var sourcePoint in sourceSerie.Points)
                    {
                        var point = serie.Points.Add(valorMin);
                    }
                    break;
                case "MAX":
                    double valorMax = double.MinValue;
                    foreach (var sourcePoint in sourceSerie.Points)
                    {
                        if (sourcePoint.YValues[0] > valorMax)
                            valorMin = sourcePoint.YValues[0];
                    }
                    foreach (var sourcePoint in sourceSerie.Points)
                    {
                        var point = serie.Points.Add(valorMax);
                    }
                    break;
                case "AVG":
                    double valorAvg = nchart.DataManipulator.Statistics.Mean(sourceSerie.Name);
                    sitem.ChartStyle = ChartType.Line;
                    serie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    //serie.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                    foreach (var sourcePoint in sourceSerie.Points)
                    {
                        var point = serie.Points.Add(valorAvg);
                    }
                    break;
                case "MAVG":
                    nchart.DataManipulator.FinancialFormula(
                        System.Windows.Forms.DataVisualization.Charting.FinancialFormula.MovingAverage, 
                        sourceSerie.Name,serie.Name);
                    sitem.ChartStyle = ChartType.Line;
                    serie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                    //serie.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                    break;
                case "EMAVG":
                    nchart.DataManipulator.FinancialFormula(
                        System.Windows.Forms.DataVisualization.Charting.FinancialFormula.ExponentialMovingAverage,
                        sourceSerie.Name, serie.Name);
                    sitem.ChartStyle = ChartType.Line;
                    serie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                    //serie.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                    break;
                case "MACD":
                    nchart.DataManipulator.FinancialFormula(
                        System.Windows.Forms.DataVisualization.Charting.FinancialFormula.MovingAverageConvergenceDivergence,"2,4",
                        sourceSerie.Name, serie.Name);
                    sitem.ChartStyle = ChartType.Line;
                    serie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                    //serie.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                    break;
            }
            foreach (var point in serie.Points)
            {
                point.Label = "";
            }

        }
        public Bitmap DrawChartBitmap(Series nseries)
        {
            System.Windows.Forms.DataVisualization.Charting.Chart nchart = new System.Windows.Forms.DataVisualization.Charting.Chart();

            /*float afontsize = Convert.ToSingle(Math.Round( * nseries.Resolution / 100));
            float afontsizeLabel = Convert.ToSingle(Math.Round(nseries.FontSize * nseries.Resolution / 100));
            float afontsizeX = Convert.ToSingle(Math.Round(nseries.VertFontSize * nseries.Resolution / 100));
            float afontsizeY = Convert.ToSingle(Math.Round(nseries.HorzFontSize * nseries.Resolution / 100));*/

            float afontsize = nseries.FontSize;
            if (afontsize == 0)
                afontsize = 10;
                
            float afontsizeX = nseries.HorzFontSize;
            float afontsizeY = nseries.VertFontSize;
            Font FontPunt = null;
            if (nseries.Resolution != 100)
            {
                float escala = nseries.Resolution / 100;
                nchart.Scale(new SizeF(escala, escala));
                //nchart.Font = new Font(nchart.Font.FontFamily, afontsize);
            }

            //achart.BackColor:=clTeeColor;
            nchart.BackColor = Color.White;
            /*achart.LeftAxis.LabelsFont.Name:=nchart.WFontName;
            achart.BottomAxis.LabelsFont.Name:=nchart.WFontName;
            achart.Legend.Font.Name:=nchart.WFontName;
            achart.LeftAxis.LabelsFont.Style:=CLXIntegerToFontStyle(nchart.FontStyle);
            achart.BottomAxis.LabelsFont.Style:=CLXIntegerToFontStyle(nchart.FontStyle);
            achart.Legend.Font.Size:=aFontSize;
            achart.Legend.Font.Style:=CLXIntegerToFontStyle(nchart.FontStyle);*/
            /*achart.Legend.Visible:=nchart.ShowLegend;*/
            // autorange and other ranges
            /*achart.LeftAxis.Maximum:=Series.HighValue;
            achart.LeftAxis.Minimum:=Series.LowValue;
            achart.LeftAxis.Automatic:=false;
            achart.LeftAxis.AutomaticMaximum:=Series.AutoRangeH;
            achart.LeftAxis.AutomaticMinimum:=Series.AutoRangeL;
            achart.LeftAxis.LabelsAngle:=nchart.VertFontRotation mod 360;
            achart.LeftAxis.LabelsFont.Size:=Round(nchart.VertFontSize*nchart.Resolution/100);
            achart.BottomAxis.LabelsAngle:=nchart.HorzFontRotation mod 360;
            achart.BottomAxis.LabelsFont.Size:=Round(nchart.HorzFontSize*nchart.Resolution/100);*/
            nchart.ChartAreas.Add("");
            if (nseries.Effect3D)
            {
                nchart.ChartAreas[0].Area3DStyle.Enable3D = nseries.Effect3D; ;
                nchart.ChartAreas[0].Area3DStyle.IsClustered = true;
            }
            //nchart.ChartAreas[0].AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            //nchart.ChartAreas[0].AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            //nchart.ChartAreas[0].AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            //nchart.ChartAreas[0].AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            nchart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            nchart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            if (nseries.Logaritmic)
            {
                nchart.ChartAreas[0].AxisY.IsLogarithmic = true;
                nchart.ChartAreas[0].AxisY.LogarithmBase = nseries.LogBase;
            }
            nchart.ChartAreas[0].AxisY.IsReversed = nseries.Inverted;
            nchart.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0.###########";
            //nchart.ChartAreas[0].AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;

            if (nseries.VertFontRotation != 0)
            {
                nchart.ChartAreas[0].AxisY.LabelStyle.Angle = -nseries.VertFontRotation;
            }
            if (nseries.HorzFontRotation != 0)
            {
                nchart.ChartAreas[0].AxisX.LabelStyle.Angle = -nseries.HorzFontRotation;
            }


            int numdecimals_default = 2;
            double MaxValue = Double.MinValue;
            double MinValue = Double.MaxValue;
            int contadorSeries = 0;
            //nchart.Legends.Add("xx");
            foreach (SeriesItem sitem in nseries.SeriesItems)
            {

                foreach (var item in sitem.Values)
                {
                    if (item > MaxValue)
                        MaxValue = item;
                    if (item < MinValue)
                        MinValue = item;
                }
                contadorSeries++;
            }
            if ((MaxValue > 1000) || (MinValue < 1000))
                numdecimals_default = 0;
            if (nseries.AutoRange != Series.AutoRangeAxis.Default)
            {
                switch (nseries.AutoRange)
                {
                    case Series.AutoRangeAxis.AutoBoth:
                        nchart.ChartAreas[0].AxisY.IsStartedFromZero = false;
                        break;
                    case Series.AutoRangeAxis.AutoUpper:
                        nchart.ChartAreas[0].AxisY.Minimum = nseries.LowValue;
                        break;
                    case Series.AutoRangeAxis.AutoLower:
                        nchart.ChartAreas[0].AxisY.Maximum = nseries.HighValue;
                        break;
                    case Series.AutoRangeAxis.None:
                        nchart.ChartAreas[0].AxisY.Maximum = nseries.HighValue;
                        nchart.ChartAreas[0].AxisY.Minimum = nseries.LowValue;
                        break;
                }
            }

            /*if ((nseries.AutoRange == Series.AutoRangeAxis.AutoBoth || nseries.AutoRange == Series.AutoRangeAxis.AutoLower)
                && (MinValue != MaxValue))
            {
                //nchart.ChartAreas[0].AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.  
                nchart.ChartAreas[0].AxisY.Minimum = MinValue;
            }
            if ((nseries.AutoRange == Series.AutoRangeAxis.AutoBoth || nseries.AutoRange == Series.AutoRangeAxis.AutoUpper)
                && (MinValue != MaxValue))
            {
                //nchart.ChartAreas[0].AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.  
                nchart.ChartAreas[0].AxisY.Maximum = MaxValue;
            }*/
            // Create series
            int acolor = 0;
            int idxserie = 0;
            foreach (SeriesItem sitem in nseries.SeriesItems)
            {
                while (nchart.Series.IndexOf(sitem.Caption) >= 0)
                {
                    sitem.Caption = sitem.Caption + "_";
                }
                System.Windows.Forms.DataVisualization.Charting.Series chartserie = nchart.Series.Add(sitem.Caption);
                if (sitem.FunctionName != null)
                {
                    chartserie.Tag = 2;
                    AddFunctionToChart(nchart ,chartserie, sitem);
                    if (chartserie == null)
                        continue;
                }
                else
                {
                    switch (sitem.ChartStyle)
                    {
                        case ChartType.Area:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
                            break;
                        case ChartType.Gantt:
                        case ChartType.Arrow:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                            break;
                        case ChartType.Bar:
                            switch (nseries.MultiBar)
                            {
                                case BarType.Stacked:
                                    chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
                                    break;
                                case BarType.Stacked100:
                                    chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn100;
                                    break;
                                default:
                                    chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                                    break;
                            }
                            break;
                        case ChartType.Bubble:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bubble;
                            break;
                        case ChartType.Horzbar:
                            switch (nseries.MultiBar)
                            {
                                case BarType.Stacked:
                                    chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedBar;
                                    break;
                                case BarType.Stacked100:
                                    chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedBar100;
                                    break;
                                default:
                                    chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
                                    break;
                            }
                            break;
                        case ChartType.Line:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                            break;
                        case ChartType.Splines:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                            break;
                        case ChartType.Pie:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
                            break;
                        case ChartType.Point:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                            break;
                        case ChartType.CandleStick:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
                            break;
                        case ChartType.Pyramid:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pyramid;
                            break;
                        case ChartType.Polar:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Polar;
                            break;
                        case ChartType.PointFigure:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.PointAndFigure;
                            break;
                        case ChartType.Funnel:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Funnel;
                            break;
                        case ChartType.Kagi:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Kagi;
                            break;
                        case ChartType.Doughnut:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
                            break;
                        case ChartType.Radar:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Radar;
                            break;
                        case ChartType.Renko:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Renko;
                            break;
                        case ChartType.ErrorBar:
                            chartserie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.ErrorBar;
                            break;
                    }
                }
                chartserie.Label = sitem.Caption;
                if ((nseries.ShowHint) && (sitem.FunctionName == null))
                {
                    chartserie.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Square;
                }
                else
                {
                    chartserie.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                }
                if ((sitem.ChartStyle == ChartType.Pie) || ((sitem.ChartStyle == ChartType.Doughnut)))
                {
                    // Set these other two properties so that you can see the connecting lines
                    nchart.Series[0].BorderWidth = 1;
                    nchart.Series[0].BorderColor = System.Drawing.Color.FromArgb(26, 59, 105);
                    // Set the pie label as well as legend text to be displayed as percentage
                    // The P2 indicates a precision of 2 decimals
                    //nchart.Series[0].Label = "#VALX #PERCENT{P2} #VAL";

                    if (nseries.ShowHint)
                    {
                        nchart.Series[0]["PieLabelStyle"] = "Outside";
                        //nchart.Legends[0].CustomItems.Add(npoint.Color, sitem.ValueCaptions[j]);
                        switch (nseries.MarkStyle)
                        {
                            case 3:
                                nchart.Series[0].Label = "#VALX #PERCENT{P2}";
                                break;
                            case 7:
                                nchart.Series[0].Label = "#VALX #PERCENT{P2} (#VAL)";
                                break;
                            default:
                                nchart.Series[0].Label = "#PERCENT{P2}";
                                break;
                        }
                    }
                    nchart.Series[0].LegendText = "#VALX #VAL";
                    //this.Chart2.Series[0].LegendText = "#VALX (#PERCENT)"
                    //nchart.Legends[0].= sitem.ValueCaptions[j];

                    //n.Legends.Add("Legend1");
                    if (nseries.ShowLegend)
                    {
                        if (nchart.Legends.Count == 0)
                        {
                            nchart.Legends.Add("");
                            nchart.Legends[0].Font = new Font(nchart.Legends[0].Font.FontFamily, afontsize);
                        }
                        nchart.Legends[0].Enabled = true;
                        nchart.Legends[0].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
                        nchart.Legends[0].Alignment = System.Drawing.StringAlignment.Center;

                    }

                }
                if (sitem.Color > 0)
                {
                    chartserie.Color = GraphicUtils.ColorFromInteger(sitem.Color);
                }
                else
                {
                    chartserie.Color = GraphicUtils.ColorFromInteger(SeriesColors[acolor]);
                }
                double total = 0;
                if (nseries.MarkStyle == 1)
                {
                    for (int j = 0; j < sitem.Values.Count; j++)
                    {
                        object nobj = sitem.Values[j];
                        if (DoubleUtil.IsNumericType(nobj))
                            total = total + Convert.ToDouble(nobj);
                    }
                }
                if (sitem.ValuesX.Count == 0)
                {
                    nchart.ChartAreas[0].AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.FixedCount;
                    nchart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                }
                for (int j = 0; j < sitem.Values.Count; j++)
                {
                    string ncaption = sitem.ValueCaptions[j];
                    if (ncaption == null)
                        ncaption = "";
                    System.Windows.Forms.DataVisualization.Charting.DataPoint npoint = null;
                    if (sitem.ValuesX.Count > j)
                    {
                        object valuex = sitem.ValuesX[j];
                        if (j == 0)
                        {
                            if (valuex is DateTime)
                            {
                                chartserie.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                                nchart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                                nchart.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
                                nchart.ChartAreas[0].AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                                nchart.ChartAreas[0].AxisX2.MinorGrid.Enabled = true;
                                nchart.ChartAreas[0].AxisX2.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                                if (ncaption.Length == 0)
                                {
                                    nchart.ChartAreas[0].AxisX.LabelStyle.Format = "dd/MM/yyyy HH:mm";
                                }

                            }
                            else
                            {
                                nchart.ChartAreas[0].AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.FixedCount;
                                nchart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                            }
                        }
                        int idx;
                        if (valuex is DateTime)
                        {
                            double valorDouble;
                            valorDouble = Convert.ToDateTime(valuex).ToOADate();
                            idx = chartserie.Points.AddXY(valorDouble, sitem.Values[j]);
                        }
                        else
                            idx = chartserie.Points.AddXY(valuex, sitem.Values[j]);
                        npoint = chartserie.Points[idx];
                        /*if (valuex is DateTime)
                        {
                            npoint.AxisLabel = Convert.ToDateTime(valuex).ToString("dd/MM/yyyy HH:mm");
                        }*/
                    }
                    else
                    {
                        int idx = chartserie.Points.AddXY(ncaption, sitem.Values[j]);
                        npoint = chartserie.Points[idx];
                    }
                    if (afontsizeY != 0)
                    {
                        if (FontPunt == null)
                            FontPunt = new Font(npoint.Font.FontFamily, afontsizeY);
                        npoint.Font = FontPunt;
                    }
                    if (sitem.ChartStyle != ChartType.Pie)
                    {
                        if (nseries.ShowHint)
                        {
                            switch (nseries.MarkStyle)
                            {
                                case 1:
                                    npoint.Label = "";
                                    if ((DoubleUtil.IsNumericType(sitem.Values[j])) && (total != 0))
                                        npoint.Label = (Convert.ToDouble(sitem.Values[j]) / total * 100).ToString("N2") + "%";
                                    break;
                                case 6:
                                    npoint.Label = "";
                                    if (DoubleUtil.IsNumericType(sitem.Values[j]))
                                    {
                                        double nuevo_valor = Convert.ToDouble(sitem.Values[j]);
                                        npoint.Label = nuevo_valor.ToString("N1") + "%";
                                    }
                                    break;
                                // Valor + 2 decimales
                                case 4:
                                    npoint.Label = sitem.Values[j].ToString("N2");
                                    break;
                                // Valor + 3 decimales
                                case 5:
                                    npoint.Label = sitem.Values[j].ToString("N2");
                                    npoint.Label = sitem.Values[j].ToString("N2");
                                    break;
                                default:
                                    npoint.Label = sitem.Values[j].ToString("N" + numdecimals_default.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            npoint.Label = "";
                        }
                    }
                    if (nseries.SeriesItems.Count < 2)
                    {
                        if (sitem.Colors[j] >= 0)
                        {
                            npoint.Color = GraphicUtils.ColorFromInteger(sitem.Colors[j]);
                        }
                        else
                        {
                            npoint.Color = GraphicUtils.ColorFromInteger(SeriesColors[acolor]);
                        }
                        if ((sitem.ChartStyle == ChartType.Pie) || (sitem.ChartStyle == ChartType.Doughnut) || nseries.ShowLegend)
                        {
                            if ((sitem.ChartStyle == ChartType.Pie) || (sitem.ChartStyle == ChartType.Doughnut))
                            {
                            }
                            else
                            {
                                if (j != 0)
                                {
                                    if (nchart.Legends.Count == 0)
                                    {
                                        nchart.Legends.Add("");
                                        nchart.Legends[0].Font = new Font(nchart.Legends[0].Font.FontFamily, afontsize);
                                    }
                                    nchart.Legends[0].CustomItems.Add(npoint.Color, sitem.ValueCaptions[j]);
                                    nchart.Legends[0].Font = new Font(nchart.Legends[0].Font.FontFamily, afontsize);
                                }
                                else
                                {
                                    nchart.Legends.Add(sitem.ValueCaptions[j]);
                                    if (nchart.Series.Count != 0)
                                        nchart.Series[0].Name = sitem.ValueCaptions[j];

                                }
                            }

                            acolor = ((acolor + 1) % (MAX_SERIECOLORS));
                        }
                    }
                    else
                    {
                        if (sitem.Colors[j] >= 0)
                        {
                            npoint.Color = GraphicUtils.ColorFromInteger(sitem.Colors[j]);
                        }
                        else
                        {
                            npoint.Color = GraphicUtils.ColorFromInteger(SeriesColors[acolor]);
                        }
                    }
                }
                acolor = ((acolor + 1) % (MAX_SERIECOLORS));
                idxserie++;
            }
            if ((nseries.SeriesItems.Count>=2) && nseries.ShowLegend)
            {
                if (nchart.Legends.Count == 0)
                {
                    nchart.Legends.Add("");
                    nchart.Legends[0].Font = new Font(nchart.Legends[0].Font.FontFamily, afontsize);
                }
            }
            if (afontsizeX > 0)
            { 
                nchart.ChartAreas[0].AxisX.TitleFont = new Font(nchart.ChartAreas[0].AxisX.TitleFont.FontFamily, afontsizeX);
                nchart.ChartAreas[0].AxisX.LabelStyle.Font = new Font(nchart.ChartAreas[0].AxisX.LabelStyle.Font.FontFamily, afontsizeX);
                nchart.ChartAreas[0].AxisX2.TitleFont = new Font(nchart.ChartAreas[0].AxisX2.TitleFont.FontFamily, afontsizeX);
                nchart.ChartAreas[0].AxisX2.LabelStyle.Font = new Font(nchart.ChartAreas[0].AxisX2.LabelStyle.Font.FontFamily, afontsizeX);
            }
            if (afontsizeY > 0)
            {
                nchart.ChartAreas[0].AxisY.TitleFont = new Font(nchart.ChartAreas[0].AxisY.TitleFont.FontFamily, afontsizeY);
                nchart.ChartAreas[0].AxisY.LabelStyle.Font = new Font(nchart.ChartAreas[0].AxisY.LabelStyle.Font.FontFamily, afontsizeY);
                nchart.ChartAreas[0].AxisY2.TitleFont = new Font(nchart.ChartAreas[0].AxisY2.TitleFont.FontFamily, afontsizeY);
                nchart.ChartAreas[0].AxisY2.LabelStyle.Font = new Font(nchart.ChartAreas[0].AxisY2.LabelStyle.Font.FontFamily, afontsizeY);
            }
            if (afontsize != 0)
            {
                if (nchart.Legends.Count > 0)
                    nchart.Legends[0].Font = new Font(nchart.Legends[0].Font.FontFamily, afontsize);
                for (int i = 0; i < nchart.Series.Count; i++)
                {
                    nchart.Series[i].Font = new Font(nchart.Series[i].Font.FontFamily, afontsize);
                }
            }



            int bitmapwidth = Convert.ToInt32(Convert.ToSingle(Twips.TwipsToInch(nseries.PrintWidth)) * nseries.Resolution);
            int bitmapheight = Convert.ToInt32(Convert.ToSingle(Twips.TwipsToInch(nseries.PrintHeight)) * nseries.Resolution);
            if (bitmapwidth <= 0)
                bitmapwidth = 1;
            if (bitmapheight <= 0)
                bitmapheight = 1;

            Bitmap nbitmap = new Bitmap(bitmapwidth,
                bitmapheight, PixelFormat.Format24bppRgb);
			Rectangle rec = new Rectangle(0, 0, nbitmap.Width, nbitmap.Height);
            nchart.SetBounds(0, 0, nbitmap.Width, nbitmap.Height);
            nchart.Update();
            nchart.DrawToBitmap(nbitmap, rec);
            return nbitmap;
        }
        public override void DrawChart(Series nseries, MetaFile ametafile, int posx, int posy, object achart)
        {
            System.IO.MemoryStream mstream = new MemoryStream();

            using (Bitmap nbitmap = DrawChartBitmap(nseries))
            {
                nbitmap.Save(mstream, ImageFormat.Bmp);
            }
            MetaObjectImage metaobj = new MetaObjectImage();
            metaobj.MetaType = MetaObjectType.Image;
            metaobj.Top = posy; metaobj.Left = posx;
            metaobj.Width = nseries.PrintWidth;
            metaobj.Height = nseries.PrintHeight;
            metaobj.CopyMode = 20;
            metaobj.DrawImageStyle = ImageDrawStyleType.Stretch;
            metaobj.DPIRes = Convert.ToInt32(nseries.Resolution);
            metaobj.PreviewOnly = false;
            metaobj.StreamPos = ametafile.Pages[ametafile.CurrentPage].AddStream(mstream, false);
            metaobj.SharedImage = false;
            metaobj.StreamSize = mstream.Length;
            ametafile.Pages[ametafile.CurrentPage].Objects.Add(metaobj);

            base.DrawChart(nseries, ametafile, posx, posy, achart);
        }

    }
}
