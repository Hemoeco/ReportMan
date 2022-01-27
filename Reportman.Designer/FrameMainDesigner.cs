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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Reportman.Drawing;
using Reportman.Reporting;
using Reportman.Drawing.Forms;
using Reportman.Reporting.Forms;


namespace Reportman.Designer
{
    public partial class FrameMainDesigner : UserControl
    {
        public class SaveReportArgs
        {
            public bool Saved;
        }
        public bool ConvertToDotNetOnExecute;
        public class ExitReportArgs
        {
            public bool RemoveControl = true;
            public bool Cancelled = false;
        }
        public class PreviewReportArgs
        {
            public PreviewReportArgs(MetaFile meta)
            {
                MetaFile = meta;
            }
            public MetaFile MetaFile;
        }
        public delegate void SaveReportEvent(object sender, SaveReportArgs args);
        public delegate void ExitReportEventHandler(object sender, ExitReportArgs args);
        public delegate void PreviewReportEvent(object sender, PreviewReportArgs args);
        private string CurrentFilename = "";
        ReportLibrarySelection CurrentReportSelection = null;

        private Report FReport;
        private SubReport CurrentSubReport;
        DesignerInterface CurrentInterface;
        PrintOutReportWinForms nprintdriver;
        private SortedList<SelectedItemPalette, ToolStripButton> PaletteButtons;
        private List<ToolStripItem> EditButtons;
        private List<ToolStripItem> SelectedButtons;
        private List<ToolStripItem> TwoSelectedButtons;
        private List<ToolStripItem> ThreeSelectedButtons;
        public enum EditModeType { OpenSave, Save, SelfSave }
        EditModeType FEditMode;
        public EditModeType EditMode
        {
            get
            {
                return FEditMode;
            }
            set
            {
                FEditMode = value;
                switch (EditMode)
                {
                    case EditModeType.OpenSave:
                        bopen.Visible = true;
                        bnew.Visible = true;
                        break;
                    case EditModeType.Save:
                        bopen.Visible = false;
                        bnew.Visible = false;
                        break;
                    case EditModeType.SelfSave:
                        bopen.Visible = false;
                        bnew.Visible = false;
                        bsave.DropDownItems.Clear();
                        break;
                }
            }
        }
        public bool ShowPrintOption
        {
            get
            {
                return bprint.Visible;
            }
            set
            {
                bprint.Visible = value;
                bsep1.Visible = value;
                bsep2.Visible = value;
            }
        }
        public bool ShowExitOption
        {
            get
            {
                return bexit.Visible;
            }
            set
            {
                bexit.Visible = value;
            }
        }
        public bool ShowPreviewOption
        {
            get
            {
                return bpreview.Visible;
            }
            set
            {
                bpreview.Visible = value;
            }
        }
        public bool ShowExportOption
        {
            get
            {
                return bexport.Visible;
            }
            set
            {
                bexport.Visible = value;
            }
        }
        public bool ShowSaveOption
        {
            get
            {
                return bsave.Visible;
            }
            set
            {
                bsave.Visible = value;
            }
        }
        public ExitReportEventHandler OnExitClick;
        public SaveReportEvent OnSaveClick;
        public PreviewReportEvent OnPreviewClick;
        ReportLibraryConfigCollection libs = new ReportLibraryConfigCollection();
        string configFilenameLibs = ReportLibraryConfig.GetConfigFilename();
        public FrameMainDesigner()
        {
            InitializeComponent();

            libs.LoadFromFile(configFilenameLibs);


            frameproperties.Initialize(this);
            frameproperties.inspector.OnPropertyChange += new PropertyChanged(PropChange);

            frameproperties.inspector.Structure = fstructure;
            frameproperties.inspector.SubReportEdit = subreportedit;

            fstructure.Init(fdatadef);
            fstructure.OnReportChange += new EventHandler(StructureChange);
            fstructure.OnSelectionChange += new EventHandler(StructureSelectionChange);

            fdatadef.OnSelectionChange += new EventHandler(DataSelectionChange);

            subreportedit.AfterInsert += new EventHandler(AfterInsertDesign);
            subreportedit.AfterSelect += new EventHandler(AfterSelectDesign);
            EditButtons = new List<ToolStripItem>
            {
                bsave,
                bprint,
                bexecute,
                bexpression,
                dropdownzoom,
                bgrid,
                barrow,
                blabel,
                bexpression,
                bimage,
                bshape,
                bchart,
                bbarcode,
                bpaste,
                bpagesetup,
                bpreview
            };

            SelectedButtons = new List<ToolStripItem>
            {
                bdelete,
                bcut,
                bcopy
            };

            TwoSelectedButtons = new List<ToolStripItem>
            {
                balignbottom,
                baligntop,
                balignright,
                balignleft
            };

            ThreeSelectedButtons = new List<ToolStripItem>
            {
                bverticalgap,
                bhorizontalgap
            };

            PaletteButtons = new SortedList<SelectedItemPalette, ToolStripButton>
            {
                { SelectedItemPalette.Arrow, barrow },
                { SelectedItemPalette.Label, blabel },
                { SelectedItemPalette.Expression, bexpression },
                { SelectedItemPalette.Shape, bshape },
                { SelectedItemPalette.Image, bimage },
                { SelectedItemPalette.Chart, bchart },
                { SelectedItemPalette.Barcode, bbarcode }
            };
            FReport = null;
            bexport.Text = Translator.TranslateStr(704);
            bnew.Text = Translator.TranslateStr(40);
            bopen.Text = Translator.TranslateStr(42);
            bsave.Text = Translator.TranslateStr(46);
            bpreview.Text = Translator.TranslateStr(54);
            bexecute.Text = Translator.TranslateStr(779);
            madobepdf.Text = Translator.TranslateStr(701);
            mpdffile2.Text = Translator.TranslateStr(702);
            mmetafile.Text = Translator.TranslateStr(703);
            mmetafile2.Text = Translator.TranslateStr(1397);
            mtextfile.Text = Translator.TranslateStr(1049);
            mhtmlfile.Text = Translator.TranslateStr(1221);
            mhtml2.Text = Translator.TranslateStr(1438);
            mtextfile2.Text = Translator.TranslateStr(1260);
            msvgfile.Text = Translator.TranslateStr(1257);
            mimagefile.Text = Translator.TranslateStr(1110);
            openreportdialog.Title = Translator.TranslateStr(43);
            openreportdialog.Filter = Translator.TranslateStr(704) + '|' + "*.rep";
            tabstruc.Text = Translator.TranslateStr(1151);
            tabdata.Text = Translator.TranslateStr(131);
            tabfields.Text = Translator.TranslateStr(1150);
            msave.Text = Translator.TranslateStr(46);
            msave.ToolTipText = Translator.TranslateStr(47);
            msaveas.Text = Translator.TranslateStr(48);
            msaveas.ToolTipText = Translator.TranslateStr(49);
            msavetolibrary.Text = Translator.TranslateStr(1136);
            msavetolibrary.ToolTipText = Translator.TranslateStr(1139);
            msetuplib.Text = Translator.TranslateStr(1134);
            msetuplib.ToolTipText = Translator.TranslateStr(1137);
            msetuplib2.Text = msetuplib.Text;
            msetuplib2.ToolTipText = msetuplib2.ToolTipText;
            mopenfromlib.Text = Translator.TranslateStr(1135);
            mopenfromlib.ToolTipText = Translator.TranslateStr(1138);
            mopen.Text = Translator.TranslateStr(42);
            mopen.ToolTipText = Translator.TranslateStr(43);
            bpagesetup.Text = Translator.TranslateStr(50);
            bpagesetup.ToolTipText = Translator.TranslateStr(51);
            bprint.Text = Translator.TranslateStr(52);
            bprint.ToolTipText = Translator.TranslateStr(53);
            bgrid.Text = Translator.TranslateStr(7);
            bgrid.ToolTipText = Translator.TranslateStr(8);
            bcut.Text = Translator.TranslateStr(9);
            bcopy.Text = Translator.TranslateStr(10);
            bpaste.Text = Translator.TranslateStr(11);

            barrow.Text = Translator.TranslateStr(81);

            blabel.Text = Translator.TranslateStr(1203);
            bexpression.Text = Translator.TranslateStr(1204);
            bshape.Text = Translator.TranslateStr(1206);
            bimage.Text = Translator.TranslateStr(1205);
            bbarcode.Text = Translator.TranslateStr(1209);
            bchart.Text = "Chart";


            blabel.ToolTipText = Translator.TranslateStr(82);
            bexpression.ToolTipText = Translator.TranslateStr(83);
            bshape.ToolTipText = Translator.TranslateStr(84);
            bimage.ToolTipText = Translator.TranslateStr(85);
            bbarcode.ToolTipText = Translator.TranslateStr(86);
            bchart.ToolTipText = Translator.TranslateStr(87);

            balignleft.Text = Translator.TranslateStr(23);
            balignbottom.Text = Translator.TranslateStr(29);
            balignright.Text = Translator.TranslateStr(25);
            baligntop.Text = Translator.TranslateStr(27);
            bhorizontalgap.Text = Translator.TranslateStr(36);
            bverticalgap.Text = Translator.TranslateStr(38);



            bdelete.Text = Translator.TranslateStr(150);
            bdelete.ToolTipText = Translator.TranslateStr(1106);

            DisableMenus();

        }
        System.IO.MemoryStream OriginalStream;
        public Report Report
        {
            get
            {
                return FReport;
            }
            set
            {
                DisableMenus();
                if (FReport != null)
                {
                    OriginalStream = new System.IO.MemoryStream();
                    FReport.SaveToStream(OriginalStream);
                }
                else
                    OriginalStream = null;
                FReport = value;
                FixReport(FReport);
                fstructure.Report = FReport;
                fdatadef.Report = FReport;
                ffields.Report = FReport;
                CurrentSubReport = FReport.SubReports[0];
                subreportedit.SetSubReport(FReport, CurrentSubReport);
                if (FReport == null)
                    return;
                EnableMenus();
            }
        }
        private static void FixReport(Report xreport)
        {
            foreach (ReportItem nitem in xreport.Components)
            {
                foreach (ReportItem xitem in xreport.Components)
                {
                    if (xitem != nitem)
                    {
                        if (xitem.Name == nitem.Name)
                            xreport.GenerateNewName(xitem);
                    }
                }
            }
        }
        public void SetSaved(System.IO.MemoryStream neworiginalstream)
        {
            OriginalStream = neworiginalstream;
        }
        public void EnableMenus()
        {
            foreach (ToolStripItem aitem in EditButtons)
            {
                aitem.Enabled = true;
            }

            panelcontent.Visible = true;
        }
        public void DisableMenus()
        {
            foreach (ToolStripItem aitem in EditButtons)
            {
                aitem.Enabled = false;
            }
            foreach (ToolStripItem aitem in SelectedButtons)
            {
                aitem.Enabled = false;
            }
            foreach (ToolStripItem aitem in TwoSelectedButtons)
            {
                aitem.Enabled = false;
            }
            foreach (ToolStripItem aitem in ThreeSelectedButtons)
            {
                aitem.Enabled = false;
            }



            panelcontent.Visible = false;
        }
        private void ButtonNewClick(object sender, EventArgs e)
        {
            if (!CheckSave())
                return;
            Report nrep = new Report();
            nrep.CreateNew();
            Report = nrep;
        }
        public bool ReportChanged()
        {
            if (FReport == null)
                return false;
            if (OriginalStream == null)
                return true;
            using (System.IO.MemoryStream newstream = new System.IO.MemoryStream())
            {
                FReport.SaveToStream(newstream);
                newstream.Seek(0, System.IO.SeekOrigin.Begin);
                OriginalStream.Seek(0, System.IO.SeekOrigin.Begin);
                if (StreamUtil.CompareArrayContent(newstream.ToArray(), OriginalStream.ToArray()))
                    return false;
                else return true;
            }
        }
        public bool CheckSave()
        {
            if (!ReportChanged())
                return true;
            else
            {
                DialogResult nresult = MessageBox.Show(this.FindForm(), Translator.TranslateStr(498), Translator.TranslateStr(729), MessageBoxButtons.YesNoCancel);
                switch (nresult)
                {
                    case DialogResult.Yes:
                        return SaveChanges();
                    case DialogResult.No:
                        return true;
                    case DialogResult.Cancel:
                        return false;
                }
                return SaveChanges();
            }
        }
        public void Open(string filename, bool check)
        {
            if (check)
            {
                if (!CheckSave())
                    return;
            }
            Report nrep = new Report();
            System.IO.MemoryStream memstream = StreamUtil.FileToMemoryStream(filename);
            memstream.Seek(0, System.IO.SeekOrigin.Begin);
            nrep.LoadFromStream(memstream);
            nrep.ConvertToDotNet();
            FixReport(nrep);
            Report = nrep;
            CurrentFilename = filename;
            CurrentReportSelection = null;
            SetSaved(memstream);

        }
        private void ButtonOpenClick(object sender, EventArgs e)
        {
            if (!CheckSave())
                return;
            if (openreportdialog.ShowDialog() != DialogResult.OK)
                return;
            Open(openreportdialog.FileName, false);
        }

        public static void Test(string filename)
        {
            FrameMainDesigner fm = new FrameMainDesigner();
            Form nform = new Form();
            fm.Parent = nform;
            fm.Dock = DockStyle.Fill;
            Report rp = new Report();
            if (filename.Length > 0)
                rp.LoadFromFile(filename);
            rp.CreateNew();
            FixReport(rp);
            fm.Report = rp;
            nform.ShowDialog();
        }
        public void OpenFile(string nfilename)
        {
            if (!CheckSave())
                return;
            Report nrep = new Report();
            nrep.LoadFromFile(nfilename);
            FixReport(nrep);
            Report = nrep;
            CurrentFilename = nfilename;
            CurrentReportSelection = null;

        }
        public bool PrintReport(bool preview)
        {
            Reportman.Reporting.Report ReportToPrint = Report;
            if (ConvertToDotNetOnExecute)
            {
                ReportToPrint = new Report();
                using (System.IO.MemoryStream mstream = new System.IO.MemoryStream())
                {
                    Report.SaveToStream(mstream);
                    mstream.Seek(0, System.IO.SeekOrigin.Begin);
                    ReportToPrint.LoadFromStream(mstream);
                    ReportToPrint.ConvertToDotNet();
                    foreach (DatabaseInfo bdInfo in Report.DatabaseInfo)
                    {
                        if (bdInfo.Connection != null)
                            ReportToPrint.DatabaseInfo[bdInfo.Alias].Connection = bdInfo.Connection;
                    }
                }
            }
            ReportToPrint.MetaFile.Clear();
            if (nprintdriver == null)
            {
                nprintdriver = new PrintOutReportWinForms(ReportToPrint);
            }
            if (Report != nprintdriver.Report)
            {
                nprintdriver.Dispose();
                nprintdriver = new PrintOutReportWinForms(ReportToPrint);
            }
            //PrintOutReportWinForms nprintdriver = new PrintOutReportWinForms(Report);
            nprintdriver.Preview = preview;
            nprintdriver.WindowMode = PreviewWindowMode.Window;
            nprintdriver.PreviewWindow.Parent = Parent;
            nprintdriver.PreviewWindow.Dock = DockStyle.Fill;
            nprintdriver.ShowEmptyReportMessage = true;
            try
            {
                nprintdriver.Print(ReportToPrint.MetaFile);
                nprintdriver.PreviewWindow.BringToFront();
            }
            catch
            {
                nprintdriver.PreviewWindow.SendToBack();
                throw;
            }
            return true;
        }
        private void ButtonPreviewClick(object sender, EventArgs e)
        {
            Report.MetaFile.Clear();
            if (OnPreviewClick != null)
            {
                OnPreviewClick(this, new PreviewReportArgs(Report.MetaFile));
                return;
            }
            PrintReport(true);
            /*        previewmetafile = new PreviewMetaFile();
                    SetReportEvents();
                    previewmetafile.OptimizeWMF = OptimizeWMF;
                    previewmetafile.MetaFile = meta;
                    previewmetafile.SetDriver(this);
                    previewwindow.ShowInTaskbar = ShowInTaskbar;
                    previewwindow.PreviewReport(previewmetafile);

                    PreviewWindow.BringToFront();
                    try
                    {
                      PreviewWindow.PreviewReport(Report.MetaFile);
                    }
                    catch
                    {
                      PreviewWindow.SendToBack();
                      throw;
                    }*/
        }


        private void MenuScale100Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mitem = (ToolStripMenuItem)sender;
            double DrawScale = 1.0;
            string ntext = mitem.Text.Substring(0, mitem.Text.Length - 1);
            DrawScale = System.Convert.ToDouble(ntext);
            DrawScale = DrawScale / 100.0;
            subreportedit.DrawScale = DrawScale;
            dropdownzoom.Text = mitem.Text;
        }

        private void ButtonGridClick(object sender, EventArgs e)
        {
            if (GridOptions.AlterGridOptions(Report))
            {
                subreportedit.Redraw();
            }
        }
        private void ButtonArrowClick(object sender, EventArgs e)
        {
            SelectedItemPalette nselitem = (SelectedItemPalette)System.Convert.ToInt32(((ToolStripButton)sender).Tag.ToString());
            foreach (SelectedItemPalette nkey in PaletteButtons.Keys)
            {
                PaletteButtons[nkey].Checked = (nkey == nselitem);
            }
            subreportedit.SelectedPalette = nselitem;
        }
        private void AfterInsertDesign(object sender, EventArgs e)
        {
            ButtonArrowClick(barrow, null);
        }
        private void AfterSelectDesign(object sender, EventArgs e)
        {
            // for band selection clear all items
            int selectedcount = 0;
            if (subreportedit.SelectedItems.Count == 1)
            {
                if (subreportedit.SelectedItems.Values[0] is PrintPosItem)
                    selectedcount = 1;
            }
            else
                selectedcount = subreportedit.SelectedItems.Count;
            switch (selectedcount)
            {
                case 0:
                    foreach (ToolStripItem nitem in SelectedButtons)
                    {
                        nitem.Enabled = false;
                    }
                    foreach (ToolStripItem nitem in TwoSelectedButtons)
                    {
                        nitem.Enabled = false;
                    }
                    foreach (ToolStripItem nitem in ThreeSelectedButtons)
                    {
                        nitem.Enabled = false;
                    }
                    break;
                case 1:
                    foreach (ToolStripItem nitem in SelectedButtons)
                    {
                        nitem.Enabled = true;
                    }
                    foreach (ToolStripItem nitem in TwoSelectedButtons)
                    {
                        nitem.Enabled = false;
                    }
                    foreach (ToolStripItem nitem in ThreeSelectedButtons)
                    {
                        nitem.Enabled = false;
                    }
                    // Set Object inspector object                    
                    break;
                case 2:
                    foreach (ToolStripItem nitem in SelectedButtons)
                    {
                        nitem.Enabled = true;
                    }
                    foreach (ToolStripItem nitem in TwoSelectedButtons)
                    {
                        nitem.Enabled = true;
                    }
                    foreach (ToolStripItem nitem in ThreeSelectedButtons)
                    {
                        nitem.Enabled = false;
                    }
                    break;
                default:
                    foreach (ToolStripItem nitem in SelectedButtons)
                    {
                        nitem.Enabled = true;
                    }
                    foreach (ToolStripItem nitem in TwoSelectedButtons)
                    {
                        nitem.Enabled = true;
                    }
                    foreach (ToolStripItem nitem in ThreeSelectedButtons)
                    {
                        nitem.Enabled = true;
                    }
                    break;
            }
            SortedList<int, ReportItem> lselec = new SortedList<int, ReportItem>();
            if (PControl.SelectedIndex == 0)
            {
                foreach (int key in subreportedit.SelectedItems.Keys)
                    lselec.Add(key, subreportedit.SelectedItems[key]);
                if (lselec.Count == 0)
                {
                    if (fstructure.FindSelectedNode().Tag is ReportItem)
                        lselec.Add(1, (ReportItem)fstructure.FindSelectedNode().Tag);
                }
            }
            else
            if (PControl.SelectedIndex == 1)
            {
                TreeNode nnode = fdatadef.FindSelectedNode();
                if (nnode != null)
                    if (fdatadef.FindSelectedNode().Tag is ReportItem)
                    {
                        lselec.Add(1, (ReportItem)fdatadef.FindSelectedNode().Tag);
                    }
            }
            if (lselec.Count > 0)
            {
                CurrentInterface = DesignerInterface.GetFromOject(lselec, frameproperties.inspector);
                frameproperties.SetObject(CurrentInterface);
            }
        }
        private void StructureChange(object sender, EventArgs args)
        {
            if (FReport.SubReports.IndexOf(CurrentSubReport) < 0)
            {
                CurrentSubReport = FReport.SubReports[0];
            }
            subreportedit.SetSubReport(FReport, CurrentSubReport);
        }
        private void StructureSelectionChange(object sender, EventArgs args)
        {
            SubReport nsubreport = fstructure.FindSelectedSubReport();
            if (subreportedit.SubReport != nsubreport)
                subreportedit.SetSubReport(FReport, nsubreport);
            if ((fstructure.FindSelectedNode().Tag is Section))
            {
                Section sec = (Section)fstructure.FindSelectedNode().Tag;
                subreportedit.ClearSelection();
                subreportedit.SelectedItems.Add(sec.SelectionIndex, sec);
                subreportedit.SelectedSection = sec;
                AfterSelectDesign(this, null);
                subreportedit.parentcontrol.Invalidate();
            }
            else
              if ((fstructure.FindSelectedNode().Tag is ReportItem))
            {
                ReportItem sub = (ReportItem)fstructure.FindSelectedNode().Tag;
                subreportedit.ClearSelection();
                AfterSelectDesign(this, null);
            }
        }
        private void DataSelectionChange(object sender, EventArgs args)
        {
            TreeNode nnode = fdatadef.FindSelectedNode();
            if (nnode == null)
                return;
            if ((nnode.Tag is ReportItem))
            {
                ReportItem sub = (ReportItem)nnode.Tag;
                subreportedit.ClearSelection();
                AfterSelectDesign(this, null);
            }
        }

        private void ButtonCopyClick(object sender, EventArgs e)
        {
            // Copy
            if (subreportedit.SelectedItems.Count == 0)
            {
                MessageBox.Show("No items selected");
                return;
            }
            if (!(subreportedit.SelectedItems.Values[0] is PrintPosItem))
            {
                MessageBox.Show("No items selected");
                return;
            }
            List<PrintPosItem> nlist = new List<PrintPosItem>();
            foreach (BandInfo nband in subreportedit.SelectedItemsBands.Values)
            {
                Section nsec = nband.Section;
                if (nsec != null)
                {
                    foreach (PrintPosItem nitem in nsec.Components)
                    {
                        int index = subreportedit.SelectedItems.IndexOfKey(nitem.SelectionIndex);
                        if (index >= 0)
                        {
                            nlist.Add(nitem);
                        }
                    }
                }
            }
            string nresult = ReportWriter.WriteComponents(nlist);
            Clipboard.SetText(nresult);
        }

        private void ButtonPasteClick(object sender, EventArgs e)
        {
            // Paste
            if (subreportedit.SelectedSection == null)
            {
                MessageBox.Show("Select a destination section first");
                return;
            }
            if (!Clipboard.ContainsText())
            {
                MessageBox.Show("Clipboard data not valid");
                return;
            }
            string ntext = Clipboard.GetText().Trim();
            if (ntext.Length < 10)
            {
                MessageBox.Show("Clipboard content not valid");
                return;
            }
            if (ntext.Substring(0, 8) != "<SECTION")
            {
                MessageBox.Show("Clipboard content not valid");
                return;
            }
            Section sec = subreportedit.SelectedSection;
            Report nreport = new Report();
            {
                ReportReader rreader = new ReportReader(nreport);
                {
                    List<PrintPosItem> nlist = rreader.ReadFromString(ntext);
                    foreach (PrintPosItem xitem in nlist)
                    {
                        // Validate name
                        if (FReport.Components.IndexOfKey(xitem.Name) >= 0)
                        {
                            FReport.GenerateNewName(xitem);
                        }
                        FReport.Components.Add(xitem.Name, xitem);
                        sec.Components.Add(xitem);
                        xitem.Section = sec;
                    }
                    subreportedit.Redraw();
                    // Select recently added items
                    subreportedit.ClearSelection();
                    foreach (PrintPosItem xitem in nlist)
                    {
                        subreportedit.SelectedItems.Add(xitem.SelectionIndex, xitem);
                    }
                }
            }
            BandInfo nband = subreportedit.BandsList[sec.SelectionIndex];
            subreportedit.SelectedItemsBands.Add(sec.SelectionIndex, nband);
            subreportedit.SelectPosItem();
            AfterSelectDesign(this, null);
            subreportedit.parentcontrol.Invalidate();
        }

        private void ButtonDeleteClick(object sender, EventArgs e)
        {
            foreach (PrintItem nitem in subreportedit.SelectedItems.Values)
            {
                if (nitem is PrintPosItem)
                {
                    PrintPosItem positem = (PrintPosItem)nitem;
                    positem.Section.Components.Remove(positem);
                    FReport.RemoveComponent(positem);
                }
            }
            subreportedit.ClearSelection();
            foreach (BandInfo ninfo in subreportedit.SelectedItemsBands.Values)
                subreportedit.ReDrawBand(ninfo);
            AfterSelectDesign(this, null);
            subreportedit.parentcontrol.Invalidate();
        }

        private void ButtonCutClick(object sender, EventArgs e)
        {
            ButtonCopyClick(this, null);
            ButtonDeleteClick(this, null);
        }

        private void ButtonAlignLeftClick(object sender, EventArgs e)
        {
            //
            int minx = int.MaxValue;
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                if (nitem.PosX < minx)
                    minx = nitem.PosX;
            }
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                nitem.PosX = minx;
            }
            foreach (BandInfo binfo in subreportedit.SelectedItemsBands.Values)
            {
                subreportedit.ReDrawBand(binfo);
            }
            subreportedit.parentcontrol.Invalidate();
        }

        private void ButtonAlignRightClick(object sender, EventArgs e)
        {
            //
            int maxx = int.MinValue;
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                if (nitem.PosX + nitem.Width > maxx)
                    maxx = nitem.PosX + nitem.Width;
            }
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                nitem.PosX = maxx - nitem.Width;
            }
            foreach (BandInfo binfo in subreportedit.SelectedItemsBands.Values)
            {
                subreportedit.ReDrawBand(binfo);
            }
            subreportedit.parentcontrol.Invalidate();
        }

        private void ButtonAlignTopClick(object sender, EventArgs e)
        {
            //
            int minx = int.MaxValue;
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                if (nitem.PosY < minx)
                    minx = nitem.PosY;
            }
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                nitem.PosY = minx;
            }
            foreach (BandInfo binfo in subreportedit.SelectedItemsBands.Values)
            {
                subreportedit.ReDrawBand(binfo);
            }
            subreportedit.parentcontrol.Invalidate();
        }

        private void ButtonAlignBottomClick(object sender, EventArgs e)
        {
            //
            int maxx = int.MinValue;
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                if (nitem.PosY + nitem.Height > maxx)
                    maxx = nitem.PosY + nitem.Height;
            }
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                nitem.PosY = maxx - nitem.Height;
            }
            foreach (BandInfo binfo in subreportedit.SelectedItemsBands.Values)
            {
                subreportedit.ReDrawBand(binfo);
            }
            subreportedit.parentcontrol.Invalidate();
        }

        private void ButtonHorizontalGapClick(object sender, EventArgs e)
        {
            //
            int maxx = int.MinValue;
            int minx = int.MaxValue;
            PrintPosItem firstitem = null;
            PrintPosItem lastitem = null;
            int totalwidth = 0;
            int itemcount = 0;
            SortedList<int, PrintPosItem> sorteditems = new SortedList<int, PrintPosItem>();
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                if (nitem.PosX + nitem.Width > maxx)
                {
                    maxx = nitem.PosX + nitem.Width;
                    lastitem = nitem;
                }
                if (nitem.PosX < minx)
                {
                    minx = nitem.PosX;
                    firstitem = nitem;
                }
                totalwidth = totalwidth + nitem.Width;
                itemcount++;
            }
            if (firstitem == null)
                return;
            if (lastitem == null)
                return;
            if (firstitem == lastitem)
                return;
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                if ((nitem != firstitem) && (nitem != lastitem))
                {
                    sorteditems.Add(nitem.PosX, nitem);
                }
            }
            // Calculate free distance
            int firstpos = firstitem.PosX + firstitem.Width;
            int lastpos = lastitem.PosX;
            int freespace = lastpos - firstpos;
            foreach (PrintPosItem nitem in sorteditems.Values)
            {
                freespace = freespace - nitem.Width;
            }
            int dif = freespace / (itemcount - 1);
            foreach (PrintPosItem nitem in sorteditems.Values)
            {
                nitem.PosX = firstpos + dif;
                firstpos = nitem.PosX + nitem.Width;
            }
            foreach (BandInfo binfo in subreportedit.SelectedItemsBands.Values)
            {
                subreportedit.ReDrawBand(binfo);
            }
            subreportedit.parentcontrol.Invalidate();
        }

        private void ButgtonVerticalGapClick(object sender, EventArgs e)
        {
            //
            int maxx = int.MinValue;
            int minx = int.MaxValue;
            PrintPosItem firstitem = null;
            PrintPosItem lastitem = null;
            int totalwidth = 0;
            int itemcount = 0;
            SortedList<int, PrintPosItem> sorteditems = new SortedList<int, PrintPosItem>();
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                if (nitem.PosY + nitem.Height > maxx)
                {
                    maxx = nitem.PosY + nitem.Height;
                    lastitem = nitem;
                }
                if (nitem.PosY < minx)
                {
                    minx = nitem.PosY;
                    firstitem = nitem;
                }
                totalwidth = totalwidth + nitem.Height;
                itemcount++;
            }
            if (firstitem == null)
                return;
            if (lastitem == null)
                return;
            if (firstitem == lastitem)
                return;
            foreach (PrintPosItem nitem in subreportedit.SelectedItems.Values)
            {
                if ((nitem != firstitem) && (nitem != lastitem))
                {
                    sorteditems.Add(nitem.PosY, nitem);
                }
            }
            // Calculate free distance
            int firstpos = firstitem.PosY + firstitem.Height;
            int lastpos = lastitem.PosY;
            int freespace = lastpos - firstpos;
            foreach (PrintPosItem nitem in sorteditems.Values)
            {
                freespace = freespace - nitem.Height;
            }
            int dif = freespace / (itemcount - 1);
            foreach (PrintPosItem nitem in sorteditems.Values)
            {
                nitem.PosY = firstpos + dif;
                firstpos = nitem.PosY + nitem.Height;
            }
            foreach (BandInfo binfo in subreportedit.SelectedItemsBands.Values)
            {
                subreportedit.ReDrawBand(binfo);
            }
            subreportedit.parentcontrol.Invalidate();
        }

        private void ButtonPageSetupClick(object sender, EventArgs e)
        {
            PageSetup.ShowPageSetup(FReport, true);
        }
        public bool HasChanges()
        {
            return false;
        }
        private void ButtonExitClick(object sender, EventArgs e)
        {
            // Ask for saving changes
            if (OnExitClick != null)
            {
                ExitReportArgs args = new ExitReportArgs();
                OnExitClick(this, args);
                if (!args.Cancelled)
                {
                    if (args.RemoveControl)
                        Parent.Controls.Remove(this);
                }
            }                
            else
            {
                Parent.Controls.Remove(this);
            }
        }
        private void PropChange(string propertyName, object propertyValue)
        {
            if (CurrentInterface == null)
                return;

            if (CurrentInterface.SelectionList.Count == 0)
                return;
            if (CurrentInterface.SelectionList.Values[0] is PrintItem)
            {
                foreach (ReportItem aitem in CurrentInterface.SelectionList.Values)
                {
                    if (aitem is Section)
                    {
                        subreportedit.Redraw();
                    }
                    else
                    {
                        subreportedit.ReDrawBand(subreportedit.BandsList[((PrintPosItem)aitem).Section.SelectionIndex]);
                    }
                }
                subreportedit.parentcontrol.Invalidate();
                subreportedit.SelectPosItem();
            }
        }

        private void FinishEdit()
        {
            frameproperties.inspector.FinishEdit();
        }
        private bool SaveChanges()
        {
            if (FReport == null)
                throw new Exception("Report Not readed");
            if (OnSaveClick != null)
            {
                SaveReportArgs args = new SaveReportArgs();
                OnSaveClick(this, args);
                OriginalStream = new System.IO.MemoryStream();
                FReport.SaveToStream(OriginalStream);
                return args.Saved;
            }
            if ((CurrentFilename.Length == 0) && (CurrentReportSelection == null))
            {
                SaveFileDialog ndiag = new SaveFileDialog();
                ndiag.Filter = Translator.TranslateStr(704) + "|" + "*.rep";
                if (ndiag.ShowDialog() != DialogResult.OK)
                    return false;
                string nfilename = ndiag.FileName;
                FReport.SaveToFile(nfilename);
                CurrentFilename = nfilename;
                OriginalStream = StreamUtil.FileToMemoryStream(CurrentFilename);
            }
            else
            {
                if (CurrentFilename.Length > 0)
                {
                    FReport.SaveToFile(CurrentFilename);
                    OriginalStream = StreamUtil.FileToMemoryStream(CurrentFilename);
                }
                else
                {
                    CurrentReportSelection.Save(FReport);
                    OriginalStream = CurrentReportSelection.Stream;
                }
            }
            return true;
        }


        private void ButtonExportClick(object sender, EventArgs e)
        {
            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Filter = Translator.TranslateStr(704) + "|*.rep";
            savedialog.Title = "";
            if (sender == msaveas)
            {
                savedialog.FileName = CurrentFilename;
            }
            if (savedialog.ShowDialog(this.FindForm()) == DialogResult.OK)
            {
                Report.SaveToFile(savedialog.FileName);
                if (sender == msaveas)
                {
                    CurrentFilename = savedialog.FileName;
                }
            }
        }

        private void ButtonSaveAsClick(object sender, EventArgs e)
        {
            ButtonExportClick(this, new EventArgs());
        }

        private void ButtonSaveClick(object sender, EventArgs e)
        {
            FinishEdit();
            SaveChanges();
        }

        private void ButtonPrintClick(object sender, EventArgs e)
        {
            PrintReport(false);
        }

        private int GetCurrentZoomIndex()
        {
            for (int i = 0; i < dropdownzoom.DropDownItems.Count; i++)
            {
                if (dropdownzoom.Text == dropdownzoom.DropDownItems[i].Text)
                    return i;

            }
            return -1;
        }
        private void ButtonZoomPlusClick(object sender, EventArgs e)
        {
            int index = GetCurrentZoomIndex();
            if (index < 0)
                return;
            index++;
            if (dropdownzoom.DropDownItems.Count > index)
                dropdownzoom.DropDownItems[index].PerformClick();
        }

        private void ButtonZoomMinusClick(object sender, EventArgs e)
        {
            int index = GetCurrentZoomIndex();

            index--;
            if (index >= 0)
                dropdownzoom.DropDownItems[index].PerformClick();
        }

        private void ButtonHideRightClick(object sender, EventArgs e)
        {
            bhideRight.Checked = !bhideRight.Checked;
            splitter1.Visible = !bhideRight.Checked;
            panelprops.Visible = !bhideRight.Checked;
        }

        private void ButtonSetupLibClick(object sender, EventArgs e)
        {
            LibraryConfigForm.ShowConfig(this.FindForm());
        }

        private void ButtonOpenFromLibClick(object sender, EventArgs e)
        {
            libs.LoadFromFile(configFilenameLibs);
            ReportLibrarySelection selection = OpenFromLibraryForm.SelectReportFromLibraries(libs, OpenFromLibrary.SelectionModeType.SelectionEdit, this.FindForm());
            Report nrep = new Report();
            System.IO.MemoryStream memstream = selection.Stream;
            memstream.Seek(0, System.IO.SeekOrigin.Begin);
            nrep.LoadFromStream(memstream);
            nrep.ConvertToDotNet();
            FixReport(nrep);
            Report = nrep;
            CurrentFilename = "";
            SetSaved(memstream);
            CurrentReportSelection = selection;
        }

        private void majustar1_5_Click(object sender, EventArgs e)
        {
            Report.AlignSectionsTo(6);
            subreportedit.Redraw();
        }
    }
}
