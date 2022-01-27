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

namespace Reportman.Designer
{
    public partial class FrameStructure : UserControl
    {
        public event EventHandler OnReportChange;
        public event EventHandler OnSelectionChange;
        private bool onlyselect;
        private FrameDataDef DataDef;


        public FrameStructure()
        {

            InitializeComponent();
            // Translations
            baddstruc.Text = Translator.TranslateStr(1158);
            bdelete.Text = Translator.TranslateStr(1159);
            mstrucadddetail.Text = Translator.TranslateStr(488);
            mstrucaddgroup.Text = Translator.TranslateStr(493);
            mstrucaddpheader.Text = Translator.TranslateStr(484);
            mstrucaddpfooter.Text = Translator.TranslateStr(491);
            bup.Text = Translator.TranslateStr(139);
            bdown.Text = Translator.TranslateStr(140);



            
            mstrucaddsubreport.Text = Translator.TranslateStr(353);

        }
        public void Init(FrameDataDef ndatader)
        {
            DataDef = ndatader;
        }
        private Report FReport;
        public Report Report
        {
            set
            {
                FReport = value;                
                RefreshInterface();
                if (RView.Nodes.Count>0)
                    RView.SelectedNode = RView.Nodes[0];
                if (RView.Nodes.Count > 1)
                    RView.CollapseAll();
            }
            get
            {
                return FReport;
            }
        }
        public void RefreshSubReport(SubReport nsubreport)
        {
            foreach (TreeNode nnode in RView.Nodes)
            {
                    if (nnode.Tag == nsubreport)
                    {
                        nnode.Text = nsubreport.GetDisplayName(true);

                    }
            }
        }
        public void RefreshInterface()
        {
            // Clear
            RView.BeginUpdate();
            try
            {
                RView.Nodes.Clear();
                if (FReport!=null)
                {
                    TreeNode anew;
                    TreeNode child;
                    foreach (SubReport subrep in FReport.SubReports)
                    {
                        anew=RView.Nodes.Add(subrep.GetDisplayName(true));
                        anew.Tag=subrep;
                        foreach (Section sec in subrep.Sections)
                        {
                            child = anew.Nodes.Add(sec.GetDisplayName(true));
                            child.Tag = sec;
                        }
                    }
                }
                RView.ExpandAll();
                if (RView.SelectedNode == null)
                    RView.SelectedNode = RView.TopNode;
            }
            finally
            {
                RView.EndUpdate();
            }
            if (OnReportChange != null)
                OnReportChange(FReport, new EventArgs());
        }

        private void mstrucaddpheader_Click(object sender, EventArgs e)
        {
            // Add page header
            Section sec=FindSelectedSubReport().AddPageHeader();
            RefreshInterface();
            SelectItem(sec,false);
        }
        private void FillNodes(TreeNodeCollection source,List<TreeNode> destination)
        {
            foreach (TreeNode node in source)
            {
                destination.Add(node);
                FillNodes(node.Nodes, destination);
            }
        }
        public List<TreeNode> GetAllNodes()
        {
            List<TreeNode> aresult = new List<TreeNode>();
            FillNodes(RView.Nodes,aresult);
            return aresult;
        }
        public void SelectItem(ReportItem sec,bool onlyfocus)
        {
            List<TreeNode> col = GetAllNodes();
            foreach (TreeNode node in col)
            {
                if (node.Tag == sec)
                {
                    onlyselect = onlyfocus;
                    try
                    {
                        RView.SelectedNode = node;
                    }
                    finally
                    {
                        onlyselect = false;
                    }
                    break;
                }
            }
        }
        public TreeNode FindSelectedNode()
        {
            if (RView.Nodes.Count == 0)
                throw new Exception("No nodes in the report tree");
            if (RView.SelectedNode == null)
                RView.SelectedNode = RView.Nodes[0];
            return RView.SelectedNode;

        }
        public SubReport FindSelectedSubReport()
        {
            TreeNode node = FindSelectedNode();
            if (node.Tag is SubReport)
            {
                return (SubReport)node.Tag;
            }
            if (node.Tag is Section)
                return ((Section)node.Tag).SubReport;
            throw new Exception("No selected subreport");
        }

        private void mstrucaddpfooter_Click(object sender, EventArgs e)
        {
            // Add page footer
            Section sec = FindSelectedSubReport().AddPageFooter();
            RefreshInterface();
            SelectItem(sec,false);
        }

        private void mstrucaddgroup_Click(object sender, EventArgs e)
        {
            // Ask the name
            string groupname = InputBox.Execute(Translator.TranslateStr(276),
                Translator.TranslateStr(277), "").Trim().ToUpper();
            if (groupname.Length > 0)
            {
                SubReport subrep = FindSelectedSubReport();
                if (subrep.IndexOfGroup(groupname) >= 0)
                {
                    MessageBox.Show(Translator.TranslateStr(278));
                    return;
                }
                Section sec = subrep.AddGroup(groupname);
                RefreshInterface();
                SelectItem(sec,false);
            }
        }

        private void mstrucadddetail_Click(object sender, EventArgs e)
        {
            // Add page footer
            Section sec = FindSelectedSubReport().AddDetail();
            RefreshInterface();
            SelectItem(sec,false);
        }

        private void mstrucaddsubreport_Click(object sender, EventArgs e)
        {
            // Add a new subreport
            SubReport sub = FReport.AddSubReport();
            RefreshInterface();
            SelectItem(sub,false);
        }

        private void bdelete_Click(object sender, EventArgs e)
        {
            // Ask confirmation
            if (MessageBox.Show(Translator.TranslateStr(312),
                Translator.TranslateStr(729), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;
            TreeNode nnode = FindSelectedNode();
            if (nnode.Tag is SubReport)
            {
                SubReport sub = (SubReport)nnode.Tag;
                if (MessageBox.Show("¿Eliminar parámetros y datos relacionados?",
                    Translator.TranslateStr(729), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    FReport.DeleteSubReport(sub);
                }
                else
                {
                    ExternalReport.DeleteSubReport(FReport, sub);
                }
                RefreshInterface();
                return;
            }
            if (nnode.Tag is Section)
            {
                SubReport subrep = FindSelectedSubReport();
                Section sec = (Section)nnode.Tag;
                sec.SubReport.DeleteSection(sec);
                RefreshInterface();
                SelectItem(subrep,false);
            }
        }

        private void bup_Click(object sender, EventArgs e)
        {
            int index;
            // Change the section or subreport up
            TreeNode nnode = FindSelectedNode();
            if (nnode.Tag is SubReport)
            {
                SubReport subrep = (SubReport)nnode.Tag;
                index = FReport.SubReports.IndexOf(subrep);
                if (index > 0)
                {
                    FReport.SubReports.Remove(subrep);
                    FReport.SubReports.Insert(index - 1, subrep);
                    RefreshInterface();
                    SelectItem(null,false);
                    SelectItem(subrep,false);
                }
                return;
            }
            if (nnode.Tag is Section)
            {
                int groupindex;
                Section sec = (Section)nnode.Tag;
                SubReport sub=sec.SubReport;
                switch (sec.SectionType)
                {
                    case SectionType.Detail:
                        index = sub.Sections.IndexOf(sec);
                        if (index > sub.FirstDetail)
                        {
                            sub.Sections.Remove(sec);
                            sub.Sections.Insert(index - 1,sec);
                        }
                        break;
                    case SectionType.PageHeader:
                        index = sub.Sections.IndexOf(sec);
                        if (index > 0)
                        {
                            sub.Sections.Remove(sec);
                            sub.Sections.Insert(index - 1,sec);
                        }
                        break;
                    case SectionType.PageFooter:
                        index = sub.Sections.IndexOf(sec);
                        if (index > sub.LastDetail+sub.GroupCount+1)
                        {
                            sub.Sections.Remove(sec);
                            sub.Sections.Insert(index - 1,sec);
                        }
                        break;
                    case SectionType.GroupHeader:
                    case SectionType.GroupFooter:
                        Section oldsec = sec;
                        index = sub.Sections.IndexOf(sec);
                        bool doexchange = false;
                        bool exchangeup = false;
                        if (index < sub.FirstDetail)
                        {
                            groupindex = sub.FirstDetail - index;
                            if (groupindex < sub.GroupCount)
                                doexchange = true;
                        }
                        else
                        {
                            groupindex = index - sub.LastDetail;
                            if ((groupindex >1) && (sub.GroupCount>1))
                                doexchange = true;
                            exchangeup = true;
                        }
                        sec = sub.Sections[sub.FirstDetail - groupindex];
                        index = sub.Sections.IndexOf(sec);
                        if (doexchange)
                        {
                            Section footer = sub.Sections[sub.LastDetail + groupindex];
                            sub.Sections.Remove(sec);
                            if (exchangeup)
                            {
                                sub.Sections.Insert(index + 1, sec);
                                index = sub.Sections.IndexOf(footer);
                                sub.Sections.Remove(footer);
                                sub.Sections.Insert(index - 1, footer);
                            }
                            else
                            {
                                sub.Sections.Insert(index - 1, sec);
                                index = sub.Sections.IndexOf(footer);
                                sub.Sections.Remove(footer);
                                sub.Sections.Insert(index+1,footer);
                            }
                        }
                        sec = oldsec;
                        break;
                }
                RefreshInterface();
                SelectItem(null,false);
                SelectItem(sec,false);
            }
        }
        public static void Test()
        {
            FrameStructure fm = new FrameStructure();
            Form nform = new Form();
            fm.Parent = nform;
            fm.Dock = DockStyle.Fill;
            Report rp = new Report();
            rp.CreateNew();
            fm.Report = rp;
            nform.ShowDialog();
        }

        private void RView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (onlyselect)
                return;
            bool enabledelete = false;
            bool enableup = false;
            bool enabledown = false;
            int index;
            if (RView.SelectedNode != null)
            {
                SubReport subrep = FindSelectedSubReport();
                if (RView.SelectedNode.Tag is SubReport)
                {
                    if (FReport.SubReports.Count > 1)
                        enabledelete = true;
                    index = FReport.SubReports.IndexOf(subrep);
                    if (index > 0)
                        enableup = true;
                    if (index < (FReport.SubReports.Count - 1))
                        enabledown = true;
                }
                else
                {
                    if (RView.SelectedNode.Tag is Section)
                    {
                        Section nsec = (Section)RView.SelectedNode.Tag;
                        index = subrep.Sections.IndexOf(nsec);
                        int groupindex;
                        switch (nsec.SectionType)
                        {
                            case SectionType.PageHeader:
                                enabledelete = true;
                                if (index > subrep.FirstPageHeader)
                                    enableup = true;
                                if (index < subrep.LastPageHeader)
                                    enabledown = true;
                                break;
                            case SectionType.PageFooter:
                                enabledelete = true;
                                if (index > subrep.FirstPageFooter)
                                    enableup = true;
                                if (index < subrep.LastPageFooter)
                                    enabledown = true;
                                break;
                            case SectionType.Detail:
                                enabledelete = subrep.DetailCount>1;
                                if (index > subrep.FirstDetail)
                                    enableup = true;
                                if (index < subrep.LastDetail)
                                    enabledown = true;
                                break;
                            case SectionType.GroupHeader:
                                enabledelete = true;
                                groupindex = subrep.FirstDetail - index;
                                if (groupindex < subrep.GroupCount)
                                    enableup = true;
                                if ((groupindex>1) && (subrep.GroupCount>1))
                                    enabledown = true;
                                break;
                            case SectionType.GroupFooter:
                                enabledelete = true;
                                groupindex = index - subrep.LastDetail;
                                if (groupindex < subrep.GroupCount)
                                    enabledown = true;
                                if ((groupindex > 1) && (subrep.GroupCount > 1))
                                    enableup = true;
                                break;
                        }
                    }
                }
            }
            bdelete.Enabled = enabledelete;
            bup.Enabled = enableup;
            bdown.Enabled = enabledown;
            if (OnSelectionChange != null)
                OnSelectionChange(RView,new EventArgs());
        }

        private void bdown_Click(object sender, EventArgs e)
        {
            int index;
            // Change the section or subreport up
            TreeNode nnode = FindSelectedNode();
            if (nnode.Tag is SubReport)
            {
                SubReport subrep = (SubReport)nnode.Tag;
                index = FReport.SubReports.IndexOf(subrep);
                if (index < (FReport.SubReports.Count-1))
                {
                    FReport.SubReports.Remove(subrep);
                    FReport.SubReports.Insert(index + 1, subrep);
                    RefreshInterface();
                    SelectItem(null,false);
                    SelectItem(subrep,false);
                }
                return;
            }
            if (nnode.Tag is Section)
            {
                Section sec = (Section)nnode.Tag;
                SubReport sub = sec.SubReport;
                switch (sec.SectionType)
                {
                    case SectionType.Detail:
                        index = sub.Sections.IndexOf(sec);
                        if (index < sub.LastDetail)
                        {
                            sub.Sections.Remove(sec);
                            sub.Sections.Insert(index + 1, sec);
                        }
                        break;
                    case SectionType.PageHeader:
                        index = sub.Sections.IndexOf(sec);
                        if (index < sub.LastPageHeader)
                        {
                            sub.Sections.Remove(sec);
                            sub.Sections.Insert(index + 1, sec);
                        }
                        break;
                    case SectionType.PageFooter:
                        index = sub.Sections.IndexOf(sec);
                        if (index < sub.LastPageFooter)
                        {
                            sub.Sections.Remove(sec);
                            sub.Sections.Insert(index + 1, sec);
                        }
                        break;
                    case SectionType.GroupHeader:
                    case SectionType.GroupFooter:
                        int groupindex;
                        Section oldsec = sec;
                        index = sub.Sections.IndexOf(sec);
                        bool doexchange = false;
                        bool exchangeup = false;
                        if (index < sub.FirstDetail)
                        {
                            groupindex = sub.FirstDetail - index;
                            if ((groupindex > 1) && (sub.GroupCount > 1))
                                doexchange = true;
                        }
                        else
                        {
                            groupindex = index - sub.LastDetail;
                            if (groupindex < sub.GroupCount)
                                doexchange = true;
                            exchangeup = true;
                        }
                        sec = sub.Sections[sub.FirstDetail - groupindex];
                        index = sub.Sections.IndexOf(sec);
                        if (doexchange)
                        {
                            Section footer = sub.Sections[sub.LastDetail + groupindex];
                            sub.Sections.Remove(sec);
                            if (exchangeup)
                            {
                                sub.Sections.Insert(index - 1, sec);
                                index = sub.Sections.IndexOf(footer);
                                sub.Sections.Remove(footer);
                                sub.Sections.Insert(index + 1, footer);
                            }
                            else
                            {
                                sub.Sections.Insert(index + 1, sec);
                                index = sub.Sections.IndexOf(footer);
                                sub.Sections.Remove(footer);
                                sub.Sections.Insert(index - 1, footer);
                            }
                        }
                        sec = oldsec;
                        break;
                }
                RefreshInterface();
                SelectItem(null,false);
                SelectItem(sec,false);
            }

        }

      private void RView_DragEnter(object sender, DragEventArgs e)
      {
      }

        private void bexpand_Click(object sender, EventArgs e)
        {
            RView.ExpandAll();
        }

        private void bcontract_Click(object sender, EventArgs e)
        {
            RView.CollapseAll();
        }

        private void bexport_Click(object sender, EventArgs e)
        {
            TreeNode nnode = FindSelectedNode();
            if (nnode.Tag is SubReport)
            {
                SubReport subrep = (SubReport)nnode.Tag;
                SaveFileDialog ndiag = new SaveFileDialog();
                ndiag.Filter = Translator.TranslateStr(704) + "|" + "*.rep";
                if (ndiag.ShowDialog() != DialogResult.OK)
                    return;
                using (System.IO.FileStream fstream = new System.IO.FileStream(ndiag.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
                {
                    ExternalReport.ExportSubReport(subrep, fstream);
                }
            }
        }

        private void bimport_Click(object sender, EventArgs e)
        {
            TreeNode nnode = FindSelectedNode();
            if (nnode.Tag is SubReport)
            {
                SubReport subrep = (SubReport)nnode.Tag;
                OpenFileDialog ndiag = new OpenFileDialog();
                ndiag.Filter = Translator.TranslateStr(704) + "|" + "*.rep";
                if (ndiag.ShowDialog() != DialogResult.OK)
                    return;
                using (System.IO.FileStream  fstream = new System.IO.FileStream(ndiag.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                {
                    Report newreport = new Report();
                    newreport.LoadFromStream(fstream);
                    ExternalReport.ImportReport(FReport,newreport);
                }
                DataDef.RefreshInterface();
                RefreshInterface();
                SelectItem(null, false);
            }
        }
    }
}
