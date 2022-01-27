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
    public partial class FrameDataDef : UserControl
    {
        public event EventHandler OnReportChange;
        public event EventHandler OnSelectionChange;
        public FrameDataDef()
        {
            InitializeComponent();
            // Translations
            badd.Text = Translator.TranslateStr(1158);
            bdelete.Text = Translator.TranslateStr(1159);
            bconnect.Text = Translator.TranslateStr(156);
            bup.Text = Translator.TranslateStr(139);
            bdown.Text = Translator.TranslateStr(140);
            
        }
      public TreeNode FindSelectedNode()
      {
        if (RView.SelectedNode != null)
          if (RView.SelectedNode.Tag is ReportItem)
            return RView.SelectedNode;
        return null;
      }
        private Report FReport;
        public Report Report
        {
            set
            {
                FReport = value;
                RefreshInterface();
            }
            get
            {
                return FReport;
            }
        }
        public void RefreshInterface()
        {
            // Clear
            RView.BeginUpdate();
            try
            {
                RView.Nodes.Clear();
                if (FReport != null)
                {
                    TreeNode aparent = RView.Nodes.Add(Translator.TranslateStr(142));
                    TreeNode anew;
                    aparent.Tag = FReport.DatabaseInfo;
                    foreach (DatabaseInfo dbinfo in FReport.DatabaseInfo)
                    {
                        anew = aparent.Nodes.Add(dbinfo.Alias);
                        anew.Tag = dbinfo;
                    }
                    aparent = RView.Nodes.Add(Translator.TranslateStr(148));
                    aparent.Tag = FReport.DataInfo;
                    foreach (DataInfo dinfo in FReport.DataInfo)
                    {
                        anew = aparent.Nodes.Add(dinfo.Alias);
                        anew.Tag = dinfo;
                    }
                    aparent = RView.Nodes.Add(Translator.TranslateStr(152));
                    aparent.Tag = FReport.Params;
                    foreach (Param nparam in FReport.Params)
                    {
                        anew = aparent.Nodes.Add(nparam.Alias);
                        anew.Tag = nparam;
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
        public static void Test(string filename)
        {
            using (FrameDataDef fm = new FrameDataDef())
            {
                using (Form nform = new Form())
                {
                    fm.Parent = nform;
                    fm.Dock = DockStyle.Fill;
                    Report rp = new Report();
                    if (filename.Length > 0)
                        rp.LoadFromFile(filename);
                    else
                        rp.CreateNew();
                    fm.Report = rp;
                    nform.ShowDialog();
                }
            }
        }
        private void mdataaddconnection_Click(object sender, EventArgs e)
        {
            // Adding a new connection
            string conname = InputBox.Execute(Translator.TranslateStr(399),
                Translator.TranslateStr(400), "").Trim().ToUpper();
            if (conname.Length == 0)
                return;
            if (FReport.DatabaseInfo.IndexOf(conname) >= 0)
            {
                MessageBox.Show(Translator.TranslateStr(505));
                return;
            }
            DatabaseInfo dbinfo = new DatabaseInfo(FReport);
            dbinfo.Alias = conname;
            FReport.DatabaseInfo.Add(dbinfo);
            RefreshInterface();
            SelectItem(dbinfo);
        }
        private void FillNodes(TreeNodeCollection source, List<TreeNode> destination)
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
            FillNodes(RView.Nodes, aresult);
            return aresult;
        }
        public void SelectItem(ReportItem sec)
        {
            List<TreeNode> col = GetAllNodes();
            foreach (TreeNode node in col)
            {
                if (node.Tag == sec)
                {
                    RView.SelectedNode = node;
                    break;
                }
            }
        }

        private void RView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (OnSelectionChange != null)
                OnSelectionChange(RView,new EventArgs());
        }
        private void madddataset_Click(object sender, EventArgs e)
        {
            // Adding a new dataset
            string conname = InputBox.Execute(Translator.TranslateStr(539),
                Translator.TranslateStr(540), "").Trim().ToUpper();
            if (conname.Length == 0)
                return;
            if (FReport.DataInfo.IndexOf(conname) >= 0)
            {
                MessageBox.Show(Translator.TranslateStr(519));
                return;
            }
            DataInfo dinfo = new DataInfo(FReport);
            dinfo.Alias = conname;
            if (FReport.DatabaseInfo.Count > 0)
                dinfo.DatabaseAlias = FReport.DatabaseInfo[0].Alias;
            FReport.DataInfo.Add(dinfo);
            RefreshInterface();
            SelectItem(dinfo);
        }
        private void maddparam_Click(object sender, EventArgs e)
        {
            // Adding a new dataset
            string paramname = InputBox.Execute(Translator.TranslateStr(543),
                Translator.TranslateStr(544), "").Trim().ToUpper();
            if (paramname.Length == 0)
                return;
            if (FReport.Params.IndexOf(paramname) >= 0)
            {
                MessageBox.Show(Translator.TranslateStr(545));
                return;
            }
            Param nparam = new Param(FReport);
            nparam.Alias = paramname;
            FReport.Params.Add(nparam);
            RefreshInterface();
            SelectItem(nparam);
        }

        private void bdelete_Click(object sender, EventArgs e)
        {
            if (RView.SelectedNode == null)
                return;
            if (!(RView.SelectedNode.Tag is ReportItem))
                return;
            ReportItem pitem = (ReportItem)RView.SelectedNode.Tag;
            if (pitem is DatabaseInfo)
            {
                DatabaseInfo dbinfo = (DatabaseInfo)pitem;
                FReport.DatabaseInfo.Remove(dbinfo);
                int index = FReport.Components.IndexOfValue(dbinfo);
                if (index >= 0)
                    FReport.Components.RemoveAt(index);
            }
            else
                if (pitem is DataInfo)
                {
                    DataInfo dinfo = (DataInfo)pitem;
                    FReport.DataInfo.Remove(dinfo);
                    int index = FReport.Components.IndexOfValue(dinfo);
                    if (index >= 0)
                        FReport.Components.RemoveAt(index);
                }
                else
                if (pitem is Param)
                {
                    Param nparam = (Param)pitem;
                    FReport.Params.Remove(nparam);
                    int index = FReport.Components.IndexOfValue(nparam);
                    if (index >= 0)
                        FReport.Components.RemoveAt(index);
                }
            RefreshInterface();
        }

        private void bup_Click(object sender, EventArgs e)
        {
            if (RView.SelectedNode == null)
                return;
            TreeNode nnode = RView.SelectedNode;
            if (nnode.Index == 0)
                return;
            if (nnode.Parent == null)
                return;
            int index;
            index = nnode.Parent.Nodes.IndexOf(nnode);
            if (index == 0)
                return;
            TreeNode segnode = nnode.Parent.Nodes[index-1];
            ReportItem pitem = (ReportItem)RView.SelectedNode.Tag;
            if (pitem is DatabaseInfo)
            {
                DatabaseInfo dbinfo = (DatabaseInfo)pitem;
                index = FReport.DatabaseInfo.IndexOf(dbinfo);
                if (index > 0)
                {
                    DatabaseInfo buf = FReport.DatabaseInfo[index - 1];
                    FReport.DatabaseInfo[index - 1] = dbinfo;
                    FReport.DatabaseInfo[index] = buf;
                }
            }
            else
                if (pitem is DataInfo)
                {
                    DataInfo dinfo = (DataInfo)pitem;
                    index = FReport.DataInfo.IndexOf(dinfo);
                    if (index > 0)
                    {
                        DataInfo buf2 = FReport.DataInfo[index - 1];
                        FReport.DataInfo[index - 1] = dinfo;
                        FReport.DataInfo[index] = buf2;
                    }
                    
                }
                else
                    if (pitem is Param)
                    {
                        Param nparam = (Param)pitem;
                        index = FReport.Params.IndexOf(nparam);
                        if (index > 0)
                        {
                            FReport.Params.Switch(index, index - 1);
                        }
                    }
                    else
                        pitem = null;
            if (pitem != null)
            {
                index = nnode.Parent.Nodes.IndexOf(nnode);
                nnode.Parent.Nodes.Remove(segnode);
                nnode.Parent.Nodes.Insert(index, segnode);
            }
        }

        private void bdown_Click(object sender, EventArgs e)
        {
            if (RView.SelectedNode == null)
                return;
            TreeNode nnode = RView.SelectedNode;
            if (nnode.Parent == null)
                return;
            if (nnode.Index >= (nnode.Parent.Nodes.Count-1))
                return;
            if (nnode.Parent.Nodes.Count<1)
                return;
            int index = nnode.Parent.Nodes.IndexOf(nnode); ;
            TreeNode segnode = nnode.Parent.Nodes[index + 1];
            ReportItem pitem = (ReportItem)RView.SelectedNode.Tag;
            if (pitem is DatabaseInfo)
            {
                DatabaseInfo dbinfo = (DatabaseInfo)pitem;
                index = FReport.DatabaseInfo.IndexOf(dbinfo);
                if (index > 0)
                {
                    DatabaseInfo buf = FReport.DatabaseInfo[index + 1];
                    FReport.DatabaseInfo[index + 1] = dbinfo;
                    FReport.DatabaseInfo[index] = buf;
                }
            }
            else
                if (pitem is DataInfo)
                {
                    DataInfo dinfo = (DataInfo)pitem;
                    index = FReport.DataInfo.IndexOf(dinfo);
                    if (index > 0)
                    {
                        DataInfo buf2 = FReport.DataInfo[index + 1];
                        FReport.DataInfo[index + 1] = dinfo;
                        FReport.DataInfo[index] = buf2;
                    }

                }
                else
                    if (pitem is Param)
                    {
                        Param nparam = (Param)pitem;
                        index = FReport.Params.IndexOf(nparam);
                        if (index > 0)
                        {
                            FReport.Params.Switch(index, index + 1);
                        }
                    }
                    else
                        pitem = null;
            if (pitem != null)
            {
                index = nnode.Parent.Nodes.IndexOf(nnode);
                nnode.Parent.Nodes.Remove(segnode);
                nnode.Parent.Nodes.Insert(index, segnode);
            }
        }

        private void bconnect_Click(object sender, EventArgs e)
        {
            TreeNode nnode = FindSelectedNode();
            if (nnode == null)
                return;
            if (nnode.Tag is DatabaseInfo)
            {
                DatabaseInfo dbinfo = (DatabaseInfo)nnode.Tag;
                try
                {
                    dbinfo.Connect();
                    dbinfo.DisConnect();
                }
                catch(Exception E)
                {
                    MessageBox.Show(E.Message);
                }
            }
        }
    }
}
