using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reportman.Drawing.Forms
{
    public enum PreviewWindowMode { Form, ModalForm, Window };
	/// <summary>
	/// Preview Window implementation for Windows.Forms
	/// </summary>
    public partial class PreviewWinFormsControl : UserControl
    {
        private bool searchchanged;
        private MetaFileWorkProgress eventprogress;
        private PageDrawnEvent eventdrawn;
        private PreviewMetaFile fmetapr;
        private Form WindowForm;

        public bool ShowInTaskbar;
        public PreviewWindowMode WindowMode;

        public EventHandler OnClose;

        /// <summary>
        /// Constructor for PreviewInForms
        /// </summary>
        public PreviewWinFormsControl()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            Dock = DockStyle.Fill;
            BDivPageSetup.Visible = false;
            BPageSetup.Visible = false;
            BParameters.Visible = false;
            BMail.Visible = false;
            //ActiveControl = EPage;
            //            this.BFirst.Text = Translator.TranslateStr(220);
            this.bsfirst.ToolTipText = Translator.TranslateStr(221);
            this.bsprior.ToolTipText = Translator.TranslateStr(223);
            this.bsnext.ToolTipText = Translator.TranslateStr(225);
            this.bslast.ToolTipText = Translator.TranslateStr(227);
            this.bssearch.ToolTipText = Translator.TranslateStr(1435);
            this.BScaleEntire.ToolTipText = Translator.TranslateStr(233);
            this.BScaleWide.ToolTipText = Translator.TranslateStr(231);
            this.BScaleFull.ToolTipText = Translator.TranslateStr(229);
            this.BPrint.ToolTipText = Translator.TranslateStr(53);
            this.BSave.ToolTipText = Translator.TranslateStr(216);
            this.BMail.ToolTipText = Translator.TranslateStr(1231);
            this.BZoomMinus.ToolTipText = Translator.TranslateStr(235);
            this.BZoomPlus.ToolTipText = Translator.TranslateStr(237);
            this.BExit.ToolTipText = Translator.TranslateStr(212);
            this.BParameters.ToolTipText = Translator.TranslateStr(136);
            this.BPageSetup.ToolTipText = Translator.TranslateStr(51);
            this.saveFileDialog1.Filter = Translator.TranslateStr(703) + "|*.rpmf|" +
                Translator.TranslateStr(701) + "|*.pdf|" +
                Translator.TranslateStr(702) + "|*.pdf|" +
              // Excel file
                Translator.TranslateStr(1031) + "|*.xlsx|" +
              // Excel file one sheet
                Translator.TranslateStr(1342) + "|*.xlsx|"+
              // CSV File
               Translator.TranslateStr(1259) + "|*.csv|"+
               // CSV Win1252 charset
               Translator.TranslateStr(1259) + "(WIN1252)|*.csv" +
               // Bitmap
               "|" + Translator.TranslateStr(1110) + "| *.bmp" +
               // Monochrome bitmap
               "|" + Translator.TranslateStr(1111) + "| *.bmp";
            DefaultButtonSize = bssearch.Width;
        }
        int DefaultButtonSize;

        bool FLargeButtons = false;
        public bool LargeButtons
        {
            get
            {
                return FLargeButtons;
            }
            set
            {
                if (value)
                {
                    foreach (ToolStripItem nitem in maintoolstrip.Items)
                    {
                        if (nitem is ToolStripButton)
                        {
                            ToolStripButton nbutton = ((ToolStripButton)nitem);
                            nbutton.AutoSize = false;
                            nbutton.Width = 40;
                            nbutton.Height = 40;
                        }
                    }
                }
                else
                {
                    foreach (ToolStripItem nitem in maintoolstrip.Items)
                    {
                        if (nitem is ToolStripButton)
                        {
                            ToolStripButton nbutton = ((ToolStripButton)nitem);
                            nbutton.AutoSize = true;
                        }
                    }
                }
                FLargeButtons = value;
            }
        }
        /// <summary>
        /// By default, the window will be modal (dialog)
        /// </summary>
		public PreviewMetaFile MetaFileControl
		{
			get
			{
				return fmetapr;
			}
			set
			{
				fmetapr = value;
				if (fmetapr == null)
					return;
        if (fmetapr.Parent != PParent)
          fmetapr.Parent = PParent;
        fmetapr.BringToFront();
				MTopDown.Checked = !fmetapr.EntireToDown;
        fmetapr.Dock = DockStyle.Fill;
        fmetapr.BackColor = System.Drawing.SystemColors.AppWorkspace;

/*				this.BScaleEntire.Pressed = false;
				this.BScaleFull.Checked = false;
				this.BScaleWide.Checked = false;
				switch (fmetapr.AutoScale)
				{
					case AutoScaleType.EntirePage:
						BScaleEntire.Checked = true;
						break;
					case AutoScaleType.Real:
						BScaleFull.Checked= true;
						break;
					case AutoScaleType.Wide:
						BScaleWide.Checked = true;
						break;
				}
*/				switch (fmetapr.MetaFile.PreviewWindow)
				{
					case PreviewWindowStyleType.Maximized:
            if (WindowForm != null)
              WindowForm.WindowState = FormWindowState.Maximized;
						break;
				}
				if (MetaFileControl.OnPageSetup!=null)
				{
					BDivPageSetup.Visible=true;
					BPageSetup.Visible=true;
				}
				if (MetaFileControl.OnReportParams!=null)
				{
					BDivPageSetup.Visible=true;
					BParameters.Visible=true;
				}
                if (MetaFileControl.OnMail != null)
                {
                    BMail.Visible = true;
                }
                eventprogress = new MetaFileWorkProgress(WorkProgress);
				MetaFileControl.OnWorkProgress += eventprogress;
				eventdrawn = new PageDrawnEvent(PageDrawn);
				MetaFileControl.OnPageDrawn += eventdrawn;
				bool docancel = false;
				WorkProgress(-1, MetaFileControl.MetaFile.Pages.CurrentCount, ref docancel);
                EPage.Text = (MetaFileControl.Page + 1).ToString();
			}
		}
        /// <summary>
        /// Preview a MetaFile, it will show the form on screen
        /// </summary>
		public void PreviewReport(PreviewMetaFile metapreview)
		{      
            try
            {
                Form nform = null;
                switch (WindowMode)
                {
                    case PreviewWindowMode.ModalForm:
                        using (nform = new Form())
                        {
                            this.WindowForm = nform;
                            MetaFileControl = metapreview;
                            MetaFileControl.Parent = PParent;
                            MetaFileControl.BringToFront();
                            MetaFileControl.BackColor = System.Drawing.SystemColors.AppWorkspace;
                            MetaFileControl.Dock = System.Windows.Forms.DockStyle.Fill;
                            nform.Width = 800;
                            nform.Height = 600;
                            nform.ShowIcon = false;
                            nform.ShowInTaskbar = false;
                            nform.Controls.Add(this);
                            try
                            {
                                nform.KeyDown += new KeyEventHandler(ExecuteKeyDown);
                                nform.KeyPreview = true;
                                nform.ShowDialog();
                            }
                            finally
                            {
                                this.WindowForm = null;
                                nform.Controls.Remove(this);
                                nform.Dispose();
                            }
                        }
                        break;
                    case PreviewWindowMode.Form:
                        nform = new Form();
                        MetaFileControl = metapreview;
                        MetaFileControl.BringToFront();
                        MetaFileControl.BackColor = System.Drawing.SystemColors.AppWorkspace;
                        MetaFileControl.Dock = System.Windows.Forms.DockStyle.Fill;
                        MetaFileControl.Parent = PParent;
                        this.WindowForm = nform;
                        nform.Width = 800;
                        nform.Height = 600;
                        nform.ShowIcon = false;
                        nform.ShowInTaskbar = false;
                        nform.Controls.Add(this);
                        nform.Show();
                        break;
                    case PreviewWindowMode.Window:
                        MetaFileControl = metapreview;
                        break;
                }
            }
            finally
            {
                if (WindowMode == PreviewWindowMode.ModalForm)
                  PParent.Controls.Remove(MetaFileControl);
            }
		}
      public void PreviewReport(MetaFile nmetafile)
      {
        if (MetaFileControl == null)
        {
          fmetapr = new PreviewMetaFile();
          MetaFileControl.Parent = PParent;
          MetaFileControl.BackColor = System.Drawing.SystemColors.AppWorkspace;
          MetaFileControl.Dock = System.Windows.Forms.DockStyle.Fill;
        }
        MetaFileControl.MetaFile = nmetafile;
//            nform.KeyDown += new KeyEventHandler(ExecuteKeyDown);
      }

        private void MScale1_Click(object sender, EventArgs e)
        {
            MScale1.Checked = false;
            MScale2.Checked = false;
            MScale3.Checked = false;
            MScale4.Checked = false;
            MScale5.Checked = false;
            MScale6.Checked = false;
            MScale8.Checked = false;
            MScale9.Checked = false;
            MScale12.Checked = false;
            MScale14.Checked = false;
            MScale15.Checked = false;
            MScale16.Checked = false;
            MScale18.Checked = false;
            MScale32.Checked = false;
            MScale64.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            fmetapr.EntirePageCount = System.Convert.ToInt32(((ToolStripMenuItem)sender).Text);
            fmetapr.AutoScale = AutoScaleType.EntirePage;
            BScaleFull.Checked = false;
//            BScaleEntire.Checked = true;
            BScaleWide.Checked = false;
        }

        private void RefreshStatus()
        {
            BarStatusEdit.Text = Translator.TranslateStr(1416) + ": " +
                (fmetapr.Page + 1).ToString("#,##0");
            StatusPage.Text = Translator.TranslateStr(1414) + ": "
            + (fmetapr.MetaFile.Pages.CurrentCount).ToString("#,##0");
            if (fmetapr.PagesDrawn > 1)
                BarStatusEdit.Text = BarStatusEdit.Text +
                    " " + Translator.TranslateStr(1415) + ": " +
                    fmetapr.PagesDrawn.ToString(",##0");
/*            this.BScaleEntire.Checked = false;
            this.BScaleFull.Checked = false;
            this.BScaleWide.Checked = false;
            switch (fmetapr.AutoScale)
            {
                case AutoScaleType.EntirePage:
                    //BScaleEntire.Pushed = true;
                    break;
                case AutoScaleType.Real:
                    BScaleFull.Checked = true;
                    break;
                case AutoScaleType.Wide:
                    BScaleWide.Checked = true;
                    break;
            }
*/
        }
        private void RefreshPage()
        {
            if (EPage.Text != (fmetapr.Page + 1).ToString())
                EPage.Text = (fmetapr.Page + 1).ToString();
            //			if (fmetapr.MetaFile.Finished)
            //				EPage.Maximum = fmetapr.MetaFile.Pages.CurrentCount;
            if (fmetapr.CanFocus)
                if (!fmetapr.Focused)
                    fmetapr.Focus();
            RefreshStatus();
        }
        
        private void MTopDown_Click(object sender, System.EventArgs e)
        {
            MTopDown.Checked = !MTopDown.Checked;
            fmetapr.EntireToDown = !MTopDown.Checked;
        }
        private void PageDrawn(PreviewMetaFile prm)
        {
            RefreshPage();
        }
        private void WorkProgress(int records, int pagecount, ref bool docancel)
        {
            //			if (fmetapr.MetaFile.Finished)
            //				EPage.Maximum = fmetapr.MetaFile.Pages.CurrentCount;
            string atext = "";
            if (records > 0)
                atext = atext + Translator.TranslateStr(684) + ": " +
                records.ToString("##,##0");
            atext = atext + " " + Translator.TranslateStr(1414) + ": " +
                (pagecount).ToString("#,##0");
            StatusPage.Text = atext;
            if (mainstatus.Visible)
                mainstatus.Refresh();
        }
        private void DisableButtons()
        {
            bsfirst.Enabled = false;
            bslast.Enabled = false;
            bsnext.Enabled = false;
            bsprior.Enabled = false;
            BPrint.Enabled = false;
            BMail.Enabled = false;
            BSave.Enabled = false;
            bssearch.Enabled = false;
            EPage.Enabled = false;
            textsearch.Enabled = false;
            BScaleFull.Enabled = false;
            BScaleWide.Enabled = false;
            BScaleEntire.Enabled = false;
            BZoomMinus.Enabled = false;
            BZoomPlus.Enabled = false;
            MetaFileControl.Visible = false;
        }
        private void EnableButtons()
        {
            bsfirst.Enabled = true;
            bslast.Enabled = true;
            bsnext.Enabled = true;
            bsprior.Enabled = true;
            BPrint.Enabled = true;
            BMail.Enabled = true;
            BSave.Enabled = true;
            bssearch.Enabled = true;
            EPage.Enabled = true;
            textsearch.Enabled = true;
            BScaleWide.Enabled = true;
            BScaleEntire.Enabled = true;
            BZoomMinus.Enabled = true;
            BZoomPlus.Enabled = true;

            MetaFileControl.Visible = true;
        }
        private void bsfirst_Click(object sender, EventArgs e)
        {
            if (sender == BExit)
            {
              if (WindowForm != null)
                WindowForm.Close();
              else
                SendToBack();
              if (OnClose != null)
              {
                OnClose(this,new EventArgs());
              }
              return;
            }
            if (sender == bsfirst)
            {
                fmetapr.Page = 0;
                return;
            }
            if (sender == bsprior)
            {
                fmetapr.PriorPage();
                return;
            }
            if (sender == bsnext)
            {
                fmetapr.NextPage();
                return;
            }
            if (sender == bslast)
            {
                fmetapr.LastPage();
                return;
            }
            if (sender == bssearch)
            {
                FindNext();
                return;
            }
            if (sender == BScaleFull)
            {
                fmetapr.AutoScale = AutoScaleType.Real;
//                BScaleFull.Checked = true;
//                BScaleEntire.Pushed = false;
//                BScaleWide.Checked = false;
                return;
            }
            if (sender == this.BPageSetup)
            {
                try
                {
                    if (fmetapr.OnPageSetup(this.FindForm()))
                    {
                        if (fmetapr.MetaFile.Empty)
                            DisableButtons();
                        else
                            EnableButtons();
                    }
                }
                catch
                {
                    DisableButtons();
                    throw;
                }
            return;
            }
            if (sender == this.BParameters)
            {
                try
                {
                    if (fmetapr.OnReportParams(this.FindForm()))
                    {
                        if (fmetapr.MetaFile.Empty)
                            DisableButtons();
                        else
                            EnableButtons();
                    }
                }
                catch
                {
                    DisableButtons();
                    throw;
                }
                return;
            }
            if (sender == BScaleWide)
            {
                fmetapr.AutoScale = AutoScaleType.Wide;
//                BScaleFull.Checked = false;
//                BScaleEntire.Pushed = false;
//                BScaleWide.Checked = true;
                return;
            }
            if (sender == BScaleEntire)
            {
                fmetapr.AutoScale = AutoScaleType.EntirePage;
//                BScaleFull.Checked = false;
//                BScaleEntire.Pushed = true;
//                BScaleWide.Checked = false;
                return;
            }
            if (sender == BZoomMinus)
            {
                fmetapr.PreviewScale = fmetapr.PreviewScale - 0.1F;
//                BScaleFull.Checked = false;
//                BScaleEntire.Pushed = false;
//                BScaleWide.Checked = false;
                return;
            }
            if (sender == BZoomPlus)
            {
                fmetapr.PreviewScale = fmetapr.PreviewScale + 0.1F;
//                BScaleFull.Checked = false;
//                BScaleEntire.Checked = false;
//                BScaleWide.Checked = false;
                return;
            }
            if (sender == BPrint)
            {
                PrintOutWinForms prw = new PrintOutWinForms();
                //				prw.Preview=true;
                if ((fmetapr.MetaFile.PrinterFonts == PrinterFontsType.Recalculate) ||
                     (fmetapr.MetaFile.PrinterFonts == PrinterFontsType.Always))
                {
                    fmetapr.MetaFile.Clear();
                    fmetapr.MetaFile.BeginPrint(prw);
                }
                prw.ShowPrintDialog = true;
                prw.Print(fmetapr.MetaFile);
                return;
            }
            if (sender == BSave)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                        case 2:
                        case 3:
                            PrintOutPDF prpdf = new PrintOutPDF();
                            if ((fmetapr.MetaFile.PrinterFonts == PrinterFontsType.Recalculate) ||
                                 (fmetapr.MetaFile.PrinterFonts == PrinterFontsType.Always))
                            {
                                fmetapr.MetaFile.Clear();
                                fmetapr.MetaFile.BeginPrint(prpdf);
                            }
                            prpdf.Compressed = (saveFileDialog1.FilterIndex == 2);
                            if (saveFileDialog1.FilterIndex != 1)
                            {
                                string nfilename = saveFileDialog1.FileName;
                                string nextension = System.IO.Path.GetExtension(nfilename).ToUpper();
                                if (nextension != ".PDF")
                                    nfilename = nfilename + ".pdf";
                                prpdf.FileName = nfilename;
                                prpdf.Print(fmetapr.MetaFile);
                            }
                            else
                            {
                                fmetapr.MetaFile.SaveToFile(saveFileDialog1.FileName,true);
                            }
                            break;
                        case 4:
                        case 5:
                            /*PrintOutExcel prex = new PrintOutExcel();*/
                            PrintOutClosedExcel prex = new PrintOutClosedExcel();
                            prex.OneSheet = (saveFileDialog1.FilterIndex==5);
                            prex.FileName = saveFileDialog1.FileName;

                            if ((fmetapr.MetaFile.PrinterFonts == PrinterFontsType.Recalculate) ||
                                 (fmetapr.MetaFile.PrinterFonts == PrinterFontsType.Always))
                            {
                                fmetapr.MetaFile.Clear();
                                fmetapr.MetaFile.BeginPrint(prex);
                            }
                            prex.Print(fmetapr.MetaFile);
                            break;
                        case 6:
                        case 7:
                            string nresult = PrintOutCSV.ExportToCSV(fmetapr.MetaFile, true, 0, 0, System.Globalization.CultureInfo.CurrentUICulture.TextInfo.ListSeparator, '\"', 5);
                            using (System.IO.FileStream nfstream = new System.IO.FileStream(saveFileDialog1.FileName, System.IO.FileMode.Create,System.IO.FileAccess.Write))
                            {
                               if (saveFileDialog1.FilterIndex == 6)
                                  StreamUtil.WriteStringToUTF8Stream(nresult,nfstream);
                               else
                                  StreamUtil.WriteStringToStream(nresult,nfstream,Encoding.GetEncoding(1252));
                            }
                            break;
                        case 8:
                        case 9:
                            bool mono = saveFileDialog1.FilterIndex == 9;
                            string bitmapExtension = System.IO.Path.GetExtension(saveFileDialog1.FileName).ToUpper();
                            System.Drawing.Bitmap outputImage = PrintOutBitmap.ExportToBitmap(fmetapr.MetaFile, true, 0, 0, mono, 96, 96);
                            System.Drawing.Imaging.ImageFormat imageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                            switch (bitmapExtension)
                            {
                                case ".PNG":
                                    imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                                    break;
                                case ".JPEG":
                                case ".JPG":
                                    imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                                    break;
                                case ".TIF":
                                case ".TIFF":
                                    imageFormat = System.Drawing.Imaging.ImageFormat.Tiff;
                                    break;
                                case ".GIF":
                                    imageFormat = System.Drawing.Imaging.ImageFormat.Gif;
                                    break;
                            }
                            outputImage.Save(saveFileDialog1.FileName, imageFormat);
                            break;

                    }
                }
                return;
            }
            if (sender == BMail)
            {
                // Update mail params
                if (fmetapr.OnMail != null)
                {
                    SendMailEventArgs margs = new SendMailEventArgs();
                    //                    string file_name;
                    string filename;
                    PrintOutPDF prpdf = new PrintOutPDF();
                    if ((fmetapr.MetaFile.PrinterFonts == PrinterFontsType.Recalculate) ||
                         (fmetapr.MetaFile.PrinterFonts == PrinterFontsType.Always))
                    {
                        fmetapr.MetaFile.Clear();
                        fmetapr.MetaFile.BeginPrint(prpdf);
                    }
                    filename = System.IO.Path.GetTempFileName();
                    try
                    {
                        filename = System.IO.Path.ChangeExtension(filename, ".pdf");
                        prpdf.Compressed = true;
                        //prpdf.FileName = filename;
                        prpdf.FileName = "";
                        margs.Filename = filename;
                        prpdf.Print(fmetapr.MetaFile);
                        margs.Filename = filename;
                        margs.Content = ((System.IO.MemoryStream)prpdf.PDFStream).ToArray();
                        StreamUtil.MemoryStreamToFile((System.IO.MemoryStream)prpdf.PDFStream, filename);
                        fmetapr.OnMail(this, margs);
                    }
                    finally
                    {
                        if (System.IO.File.Exists(filename))
                            System.IO.File.Delete(filename);
                    }
                }
                return;
            }

        }

        private void textsearch_TextChanged(object sender, EventArgs e)
        {
            searchchanged = true;
        }
        private void FindNext()
        {
            int pageindex;
            if (searchchanged)
            {
                fmetapr.MetaFile.DoSearch(textsearch.Text);
                pageindex = fmetapr.MetaFile.NextPageFound(-1);
                searchchanged = false;
            }
            else
            {
                pageindex = fmetapr.MetaFile.NextPageFound(fmetapr.Page + fmetapr.PagesDrawn - 1);
            }
            if (pageindex == fmetapr.Page)
                fmetapr.RefreshPage();
            else
                fmetapr.Page = pageindex;
        }

        private void EPage_Validating(object sender, CancelEventArgs e)
        {
            if (fmetapr != null)
            {
                string pages = EPage.Text.Trim();
                if (pages.Length > 0)
                {
                    bool valid=true;
                    for (int i = 0; i < pages.Length; i++)
                    {
                        if (!Char.IsDigit(pages[i]))
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        int newpage=System.Convert.ToInt32(EPage.Text) - 1;
                        if (newpage <= 0)
                            newpage = 1;
                        if (fmetapr.Page != newpage)
                        {
                            fmetapr.Page = newpage;
                        }
                    }
                }
            }
        }

        private void EPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                EPage_Validating(EPage, null);
        }
        public void ExecuteKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PageDown:
                    if (e.Control)
                        bsfirst_Click(bslast, new EventArgs());
                    else
                        bsfirst_Click(bsnext, new EventArgs());

                    e.Handled = true;
                    break;
                case Keys.PageUp:
                    if (e.Control)
                        bsfirst_Click(bsfirst, new EventArgs());
                    else
                        bsfirst_Click(bsprior, new EventArgs());
                    e.Handled = true;
                    break;
            }
        }

        private void PreviewWinForms2_Load(object sender, EventArgs e)
        {

        }

        private void textsearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                bsfirst_Click(bssearch, new EventArgs());
        }

        private void EPage_Leave(object sender, EventArgs e)
        {
            EPage_Validating(EPage,new CancelEventArgs());
        }
      public void DoDispose()
      {
        if (fmetapr != null)
        {
          try
          {
            fmetapr.Dispose();
            fmetapr = null;
          }
          finally
          {
            fmetapr = null;
          }
        }
      }
        public new void Dispose()
        {
          DoDispose();
          base.Dispose();
        }
    

    }
}