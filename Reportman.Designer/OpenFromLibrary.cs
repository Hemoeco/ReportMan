using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Designer
{
    public partial class OpenFromLibrary : UserControl
    {
        ReportLibraryConfig LibraryConfig;
        ISqlExecuter Executer;
        DataTable treports;
        DataTable tgroups;
        DataView vgroups;
        public class BeforeSelectEventArgs
        {
            public string ReportName = "";
            public int GroupCode = 0;
            public bool Cancel;
        }
        public class AfterSelectEventArgs
        {
            public string ReportName = "";
            public int GroupCode = 0;
        }
        public delegate void BeforeSelectEvent(object sender, BeforeSelectEventArgs args);
        public delegate void AfterSelectEvent(object sender, AfterSelectEventArgs args);
        public BeforeSelectEvent OnBeforeSelect;
        public AfterSelectEvent OnAfterSelect;
        public enum SelectionModeType { Selection,SelectionEdit,Edit}
        SelectionModeType FSelectionMode;
        string SelectedReport = "";
        bool FShowOkCancel = true;
        public bool ShowOkCancel
        {
            get
            {
                return FShowOkCancel;
            }
            set
            {
                FShowOkCancel = value;
                if (!value)
                    panelbottom.Visible = value;
            }

        }
        private SelectionModeType SelectionMode
        {
            get
            {
                return FSelectionMode;
            }
            set
            {
                FSelectionMode = value;
                switch (FSelectionMode)
                {
                    case SelectionModeType.Edit:
                        panelbottom.Visible = false;
                        bnuevo.Visible = true;
                        brename.Visible = true;
                        bdelete.Visible = true;
                        break;
                    case SelectionModeType.SelectionEdit:
                        panelbottom.Visible = FShowOkCancel;
                        bnuevo.Visible = true;
                        brename.Visible = true;
                        bdelete.Visible = true;
                        break;
                    case SelectionModeType.Selection:
                        panelbottom.Visible = FShowOkCancel;
                        bnuevo.Visible = false;
                        brename.Visible = false;
                        bdelete.Visible = false;
                        break;
                }
                switch (FSelectionMode)
                {
                    case SelectionModeType.Selection:
                        this.ReportTree.DragDrop -= arbde;
                        this.ReportTree.DragEnter -= arbden;
                        this.ReportTree.ItemDrag -= arbdrag;
                        this.ReportTree.DragOver -= arbdover;
                        break;
                    default:
                        this.ReportTree.DragDrop += arbde;
                        this.ReportTree.DragEnter += arbden;
                        this.ReportTree.ItemDrag += arbdrag;
                        this.ReportTree.DragOver += arbdover;
                        break;
                }
            }
        }
        DragEventHandler arbde;
        DragEventHandler arbden;
        ItemDragEventHandler arbdrag;
        DragEventHandler arbdover;

        public EventHandler OnAccept;
        public EventHandler OnCancel;
        public OpenFromLibrary()
        {
            InitializeComponent();
            int newwidth = Convert.ToInt32(GraphicUtils.DPIScale*19);
            imageList1.ImageSize = new Size(newwidth, newwidth);
            imageList1.Images.Add(Properties.Resources.document32);
            imageList1.Images.Add(Properties.Resources.closed_folder);
            imageList1.Images.Add(Properties.Resources.open_folder);
            ReportTree.ImageList = imageList1;

            baceptar.Text = Translator.TranslateStr(93);
            bcancel.Text = Translator.TranslateStr(271);
            mnewfolder.Text = Translator.TranslateStr(1141);
            mnewreport.Text = Translator.TranslateStr(1131);
            bdelete.Text = Translator.TranslateStr(1142);
            bsearch.Text = Translator.TranslateStr(1143);
            brename.Text = Translator.TranslateStr(1212);

            arbde = new System.Windows.Forms.DragEventHandler(this.Arbol_DragDrop);
            arbden = new System.Windows.Forms.DragEventHandler(this.Arbol_DragEnter);
            arbdrag = new System.Windows.Forms.ItemDragEventHandler(this.Arbol_ItemDrag);
            arbdover = new System.Windows.Forms.DragEventHandler(this.Arbol_DragOver);



            Dock = DockStyle.Fill;
        }
        public void Init(ISqlExecuter iexecute,ReportLibraryConfig libconfig,SelectionModeType newselectionmode)
        {
            SelectionMode = newselectionmode;
            LibraryConfig = libconfig;
            Executer = iexecute;
            if (!libconfig.AllowSorting)
            {
                bleft.Visible = false;
                bup.Visible = false;
                bdown.Visible = false;
            }
            ReportTree.Nodes.Clear();
            LoadReports();
        }

        private void LoadReports()
        {
            string sqltext = "SELECT " + LibraryConfig.ReportSearchField;
            //sqltext = sqltext + ',' + LibraryConfig.ReportField;
            if (LibraryConfig.ReportGroupsTable.Length > 0)
            {
                if (LibraryConfig.ReportGroupsTable == "GINFORME")
                    sqltext = sqltext + ",GRUPO AS REPORT_GROUP ";
                else
                    sqltext = sqltext + ",REPORT_GROUP";
                
            }
            if (LibraryConfig.AllowSorting)
            {
                sqltext = sqltext + ",SORT_ID ";
            }
            sqltext = sqltext + " FROM " + LibraryConfig.ReportTable;
            if (LibraryConfig.AllowSorting)
            {
                sqltext = sqltext + " ORDER BY SORT_ID";
            }
            treports = Executer.OpenInmediate(null, sqltext, "REPORTS");
            treports.CaseSensitive = true;

            treports.Constraints.Add("PRIMREPORTS", treports.Columns[0], true);
            if (LibraryConfig.ReportGroupsTable.Length>0)
            {
                if (LibraryConfig.ReportGroupsTable == "GINFORME")
                {
                    sqltext = "SELECT CODIGO AS GROUP_CODE,NOMBRE AS GROUP_NAME," +
                        " GRUPO AS PARENT_GROUP ";
                    if (LibraryConfig.AllowSorting)
                    {
                        sqltext = sqltext + ",SORT_ID ";
                    }
                    sqltext = sqltext + "FROM " + LibraryConfig.ReportGroupsTable;
                    if (LibraryConfig.AllowSorting)
                    {
                        sqltext = sqltext + " ORDER BY SORT_ID";
                    }
                }
                else
                {
                    sqltext = "SELECT GROUP_CODE,GROUP_NAME," +
                        " PARENT_GROUP ";
                    if (LibraryConfig.AllowSorting)
                    {
                        sqltext = sqltext + ",SORT_ID";
                    }
                    sqltext = sqltext + "FROM " + LibraryConfig.ReportGroupsTable;
                }
                tgroups = Executer.OpenInmediate(null, sqltext, "REPORT_GROUPS");
                tgroups.Constraints.Add("PRIMGROUS", tgroups.Columns["GROUP_CODE"], true);
                List<DataRow> lremove = new List<DataRow>();
                foreach (DataRow grow in tgroups.Rows)
                {
                    if (grow["PARENT_GROUP"] == DBNull.Value)
                    {
                        grow["PARENT_GROUP"] = 0;
                    }
                    else
                    {
                        int group = Convert.ToInt32(grow["PARENT_GROUP"]);
                        if (group == -1)
                        {
                            lremove.Add(grow);
                        }
                        else
                        if (tgroups.Rows.Find(grow["PARENT_GROUP"]) == null)
                        { 
                            
                            grow["PARENT_GROUP"] = 0;
                        }
                    }
                }
                foreach (DataRow xrow in lremove)
                    tgroups.Rows.Remove(xrow);
            }      
            else
            {
                tgroups = new DataTable("REPORT_GROUPS");
                tgroups.Columns.Add("GROUP_CODE", System.Type.GetType("System.Int32"));
                tgroups.Columns.Add("GROUP_NAME", System.Type.GetType("System.String"));
                tgroups.Columns.Add("PARENT_GROUP", System.Type.GetType("System.Int32"));
                if (LibraryConfig.AllowSorting)
                    tgroups.Columns.Add("SORT_ID", System.Type.GetType("System.Int32"));
                tgroups.Constraints.Add("PRIMGROUS", tgroups.Columns["GROUP_CODE"], true);
            }
            tgroups.Columns.Add("NODE",System.Type.GetType("System.Object"));
            tgroups.Columns.Add("EXPANDED", System.Type.GetType("System.Boolean"));
            treports.Columns.Add("NODE", System.Type.GetType("System.Object"));
            vgroups = new DataView(tgroups, "", "PARENT_GROUP", DataViewRowState.CurrentRows);


            AddDocuments();
        }
        private void ClearDocuments()
        {
            foreach (DataRow xrow in treports.Rows)
            {
                if (xrow["NODE"] != DBNull.Value)
                {
                    TreeNode newnode = (TreeNode)xrow["NODE"];
                    if (newnode.Parent != null)
                    {
                        newnode.Parent.Nodes.Remove(newnode);
                    }
                }
            }
        }
        private void ClearTree()
        {
            foreach (DataRow rgroup in tgroups.Rows)
            {
                if (rgroup["NODE"] != DBNull.Value)
                {
                    TreeNode newnode = (TreeNode)rgroup["NODE"];
                    rgroup["EXPANDED"] = newnode.IsExpanded;
                    if (newnode.Parent != null)
                        newnode.Parent.Nodes.Remove(newnode);
                }
            }
            foreach (DataRow rreport in treports.Rows)
            {
                if (rreport["NODE"] != DBNull.Value)
                {
                    TreeNode newnode = (TreeNode)rreport["NODE"];
                    if (newnode.Parent != null)
                        newnode.Parent.Nodes.Remove(newnode);
                }
            }

            ReportTree.Nodes.Clear();
        }
        private void AddDocuments()
        {
            ClearTree();
            // First groups 0
            FillFolders(null, 0);
            // Fill documents
            foreach (DataRow xrow in treports.Rows)
            {
                TreeNode newnode = null;
                if (xrow["NODE"] == DBNull.Value)
                {
                    newnode = new TreeNode();
                    xrow["NODE"] = newnode;
                    newnode.Text = xrow[LibraryConfig.ReportSearchField].ToString();
                    newnode.ImageIndex = 0;
                    newnode.SelectedImageIndex = 0;
                    newnode.Tag = xrow;
                }
                else
                    newnode = (TreeNode)xrow["NODE"];
                TreeNode parent_node = null;
                if (LibraryConfig.ReportGroupsTable.Length > 0)
                {
                    int group = 0;
                    if (xrow["REPORT_GROUP"] != DBNull.Value)
                    {
                        group = Convert.ToInt32(xrow["REPORT_GROUP"]);
                    }
                    DataRow group_row = tgroups.Rows.Find(group);
                    if (group_row != null)
                    {
                        if (group_row["NODE"] != DBNull.Value)
                            parent_node = (TreeNode)group_row["NODE"];
                    }

                }          
                if (InsideFilter(newnode.Text))
                {
                    if (newnode.Parent == null)
                    {
                        if (parent_node == null)
                        {
                            ReportTree.Nodes.Add(newnode);
                        }
                        else
                            parent_node.Nodes.Add(newnode);
                    }
                }
                else
                {
                    if (newnode.Parent != null)
                        newnode.Parent.Nodes.Remove(newnode);
                }
            }
            // Expand folders
            foreach (DataRow xrow in tgroups.Rows)
            {
                if (xrow["EXPANDED"] != DBNull.Value)
                {
                    bool expanded = Convert.ToBoolean(xrow["EXPANDED"]);
                    if (expanded)
                    {
                        if (xrow["NODE"] != DBNull.Value)
                        {
                            TreeNode nnode = (TreeNode)xrow["NODE"];
                           // if (nnode.Parent != null)
                                nnode.Expand();
                        }
                    }
                }
            }
            // Remove empty folders
            if (textfilter.Text.Trim().Length > 0)
            {
                EmptyFolders(null, 0);
                bleft.Enabled = false;
                bup.Enabled = false;
                bdown.Enabled = false;
            }
            else
            {
                bleft.Enabled = true;
                bup.Enabled = true;
                bdown.Enabled = true;
            }
        

        }
        private void FillFolders(TreeNode parent_node, int parent_group)
        {
            DataRowView[] vrows = vgroups.FindRows(parent_group);
            foreach (DataRowView vrow in vrows)
            {
                // Create node
                TreeNode newnode = null;
                if (vrow["NODE"] == DBNull.Value)
                {
                    newnode = new TreeNode();
                    newnode.Text = vrow["GROUP_NAME"].ToString();
                    newnode.ImageIndex = 1;
                    newnode.SelectedImageIndex = 1;
                    newnode.Tag = vrow.Row;
                }
                else
                {
                    newnode = (TreeNode)vrow["NODE"];
                }
                if (parent_node == null)
                {
                    ReportTree.Nodes.Add(newnode);
                }
                else
                    parent_node.Nodes.Add(newnode);
                vrow["NODE"] = newnode;
                FillFolders(newnode, Convert.ToInt32(vrow["GROUP_CODE"]));
            }
        }
        private void EmptyFolders(TreeNode parent_node, int parent_group)
        {
            DataRowView[] vrows = vgroups.FindRows(parent_group);
            foreach (DataRowView vrow in vrows)
            {
                // Create node
                TreeNode newnode = (TreeNode)vrow["NODE"];
                vrow["NODE"] = newnode;
                EmptyFolders(newnode, Convert.ToInt32(vrow["GROUP_CODE"]));
                if (newnode.Nodes.Count == 0)
                {
                    if (newnode.Parent != null)
                        newnode.Parent.Nodes.Remove(newnode);
                }
            }
        }

        private void ReportTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageIndex == 1)
                e.Node.ImageIndex = 2;
        }

        private void ReportTree_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageIndex == 2)
                e.Node.ImageIndex = 1;
        }

        private void Bnewfolder_Click(object sender, EventArgs e)
        {
            if (ReportTree.SelectedNode == null)
                return;
            int parent_group = 0;
            int groupcode = 1;
            TreeNode parent_treenode = null;
            if (ReportTree.SelectedNode.ImageIndex == 0)
            {
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
                parent_group = Convert.ToInt32(xrow["REPORT_GROUP"]);
                DataRow rowparent = tgroups.Rows.Find(parent_group);
                parent_treenode = (TreeNode)rowparent["NODE"];
            }
            else
            {
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
                parent_group = Convert.ToInt32(xrow["GROUP_CODE"]);
                parent_treenode = (TreeNode)xrow["NODE"];
            }
            // Ask for the name
            string group_name = Reportman.Drawing.Forms.InputBox.Execute(Translator.TranslateStr(276), Translator.TranslateStr(277), "");
            // New Code
            string sqltext = "";
            if (LibraryConfig.ReportGroupsTable == "GINFORME")
                sqltext = "SELECT MAX(CODIGO) GCODE" + " FROM " + LibraryConfig.ReportGroupsTable;
            else
                sqltext = "SELECT MAX(GROUP_CODE) GCODE" + " FROM " + LibraryConfig.ReportGroupsTable;
            Executer.StartTransaction(IsolationLevel.ReadCommitted);
            try
            {
                DataTable tgroupcode = Executer.OpenInmediate(null, sqltext, "DE");
                if (tgroupcode.Rows.Count > 0)
                {
                    if (tgroupcode.Rows[0][0] != DBNull.Value)
                        groupcode = Convert.ToInt32(tgroupcode.Rows[0][0])+1;
                }
                sqltext = "INSERT INTO " + LibraryConfig.ReportGroupsTable;
                if (LibraryConfig.ReportGroupsTable == "GINFORME")
                    sqltext = sqltext + " (CODIGO,NOMBRE,GRUPO)  ";
                else
                    sqltext = sqltext + " (GROUP_CODE,GROUP_NAME,PARENT_GROUP)  ";
                sqltext = sqltext + " VALUES (" +
                    groupcode.ToString() + ",@GROUPNAME," + parent_group.ToString() + ")";
                System.Data.Common.DbCommand ncommand = Executer.CreateCommand(sqltext);
                System.Data.Common.DbParameter param = ncommand.CreateParameter();
                param.ParameterName = "GROUPNAME";
                param.Value = group_name;
                ncommand.Parameters.Add(param);
                Executer.Execute(ncommand);


                Executer.Commit();
                Executer.Flush();
            }
            catch
            {
                Executer.RollbackInmediate();
                throw;
            }
            DataRow newrow = tgroups.NewRow();
            newrow["GROUP_CODE"] = groupcode;
            newrow["PARENT_GROUP"] = parent_group;
            newrow["GROUP_NAME"] = group_name;
            TreeNode newnode = new TreeNode();
            newnode.Text = group_name;
            newnode.ImageIndex = 1;
            newnode.SelectedImageIndex = 1;
            newnode.Tag = newrow;
            newrow["NODE"] = newnode;
            parent_treenode.Nodes.Add(newnode);
            tgroups.Rows.Add(newrow);
            ReportTree.SelectedNode = newnode;
            ReportTree.SelectedNode.EnsureVisible();
        }

        private void Bnewreport_Click(object sender, EventArgs e)
        {
            if (ReportTree.SelectedNode == null)
                return;
            int parent_group = 0;
            int groupcode = 1;
            TreeNode parent_treenode = null;
            if (ReportTree.SelectedNode.ImageIndex == 0)
            {
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
                parent_group = Convert.ToInt32(xrow["PARENT_GROUP"]);
                groupcode = parent_group;
                DataRow rowparent = tgroups.Rows.Find(parent_group);
                parent_treenode = (TreeNode)rowparent["NODE"];
            }
            else
            {
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
                parent_group = Convert.ToInt32(xrow["GROUP_CODE"]);
                groupcode = parent_group;
                parent_treenode = (TreeNode)xrow["NODE"];
            }
            // Ask for the name
            string report_name = Reportman.Drawing.Forms.InputBox.Execute(Translator.TranslateStr(1131), Translator.TranslateStr(1132), "");
            // New Code
            string sqltext = "";
            Executer.StartTransaction(IsolationLevel.ReadCommitted);
            try
            {
                sqltext = "INSERT INTO " + LibraryConfig.ReportTable + "("+
                     LibraryConfig.ReportSearchField + ',' + LibraryConfig.ReportField;

                if (LibraryConfig.ReportGroupsTable == "GINFORME")
                    sqltext = sqltext + ",GRUPO,DEF_USUARIO";
                else
                    sqltext = sqltext + ",REPORT_GROUP";
                sqltext = sqltext + ") VALUES (@REPNAME,@REPORT";
                if (LibraryConfig.ReportGroupsTable.Length > 0)
                {
                    sqltext = sqltext + ',' + groupcode.ToString() + ",0";
                }
                else
                {
                    sqltext = sqltext + ',' + groupcode.ToString();
                }
                sqltext = sqltext + ")";
                System.Data.Common.DbCommand ncommand = Executer.CreateCommand(sqltext);
                System.Data.Common.DbParameter param = ncommand.CreateParameter();
                param.ParameterName = "REPNAME";
                param.Value = report_name;
                ncommand.Parameters.Add(param);

                Reportman.Reporting.Report nreport = new Reporting.Report();
                nreport.CreateNew();
                nreport.ConvertToDotNet();

                System.IO.MemoryStream mstream = new System.IO.MemoryStream();
                nreport.SaveToStream(mstream);

                param = ncommand.CreateParameter();
                param.ParameterName = "REPORT";
                param.DbType = DbType.Binary;
                param.Value = mstream.ToArray();


                ncommand.Parameters.Add(param);
                Executer.Execute(ncommand);


                Executer.Commit();
                Executer.Flush();
            }
            catch
            {
                Executer.RollbackInmediate();
                throw;
            }
            DataRow newrow = treports.NewRow();
            TreeNode newnode = new TreeNode();
            newrow["NODE"] = newnode;
            newrow[LibraryConfig.ReportSearchField] = report_name;
            newrow["REPORT_GROUP"] = parent_group;
            newnode.Text = report_name;
            newnode.ImageIndex = 0;
            newnode.SelectedImageIndex = 0;
            newnode.Tag = newrow;
            parent_treenode.Nodes.Add(newnode);
            treports.Rows.Add(newrow);
            ReportTree.SelectedNode = newnode;
            ReportTree.SelectedNode.EnsureVisible();
        }

        private void Brename_Click(object sender, EventArgs e)
        {
            if (ReportTree.SelectedNode == null)
                return;
            if (ReportTree.SelectedNode.ImageIndex == 0)
            {
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
                string report_name = xrow[LibraryConfig.ReportSearchField].ToString();
                string newreport_name = Reportman.Drawing.Forms.InputBox.Execute(Translator.TranslateStr(1212), Translator.TranslateStr(1132), report_name);
                if (newreport_name == report_name)
                    return;
                string sqltext = "UPDATE " + LibraryConfig.ReportTable + " SET " +
                  LibraryConfig.ReportSearchField + "=@REPNAME" +
                  " WHERE " + LibraryConfig.ReportSearchField + "=@OLDREPNAME";
                System.Data.Common.DbCommand ncommand = Executer.CreateCommand(sqltext);
                System.Data.Common.DbParameter param = ncommand.CreateParameter();
                param.ParameterName = "REPNAME";
                param.Value = newreport_name;
                ncommand.Parameters.Add(param);

                param = ncommand.CreateParameter();
                param.ParameterName = "OLDREPNAME";
                param.Value = report_name;
                ncommand.Parameters.Add(param);

                Executer.StartTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    Executer.Execute(ncommand);
                    Executer.Commit();
                    Executer.Flush();
                }
                catch
                {
                    Executer.RollbackInmediate();
                    throw;
                }
                xrow[LibraryConfig.ReportSearchField] = newreport_name;
                ReportTree.SelectedNode.Text = newreport_name;
            }
            else
            {
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
                int group_code = Convert.ToInt32(xrow["GROUP_CODE"]);
                string group_name = xrow["GROUP_NAME"].ToString();
                string newgroup_name = Reportman.Drawing.Forms.InputBox.Execute(Translator.TranslateStr(1212), Translator.TranslateStr(277), group_name);
                if (newgroup_name == group_name)
                    return;
                string sqltext = "UPDATE " + LibraryConfig.ReportGroupsTable;
                if (LibraryConfig.ReportGroupsTable == "GINFORME")
                    sqltext = sqltext + " SET NOMBRE=@GROUPNAME WHERE CODIGO=@GROUPCODE  ";
                else
                    sqltext = sqltext + " SET GROUP_NAME=@GROUPNAME WHERE GROUP_CODE=@GROUPCODE";

                System.Data.Common.DbCommand ncommand = Executer.CreateCommand(sqltext);
                System.Data.Common.DbParameter param = ncommand.CreateParameter();
                param.ParameterName = "GROUPCODE";
                param.DbType = DbType.Int32;
                param.Value = group_code;
                ncommand.Parameters.Add(param);

                param = ncommand.CreateParameter();
                param.ParameterName = "GROUPNAME";
                param.Value = newgroup_name;
                ncommand.Parameters.Add(param);

                Executer.StartTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    Executer.Execute(ncommand);
                    Executer.Commit();
                    Executer.Flush();
                }
                catch
                {
                    Executer.RollbackInmediate();
                    throw;
                }
                xrow["GROUP_NAME"] = newgroup_name;
                ReportTree.SelectedNode.Text = newgroup_name;
            }

        }
        private void Bdelete_Click(object sender, EventArgs e)
        {
            if (ReportTree.SelectedNode == null)
                return;
            string description = Translator.TranslateStr(794);
            string title = Translator.TranslateStr(729);
            if (DialogResult.Cancel == MessageBox.Show(description, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                return;
            if (ReportTree.SelectedNode.ImageIndex == 0)
            {
                string sqltext = "DELETE FROM " + LibraryConfig.ReportTable + " WHERE " +
                    LibraryConfig.ReportSearchField + "=@REPNAME";
                System.Data.Common.DbCommand ncommand = Executer.CreateCommand(sqltext);
                System.Data.Common.DbParameter param = ncommand.CreateParameter();
                param.ParameterName = "REPNAME";
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
                param.Value = xrow[LibraryConfig.ReportSearchField];
                ncommand.Parameters.Add(param);
                Executer.StartTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    Executer.Execute(ncommand);
                    Executer.Commit();
                    Executer.Flush();
                }
                catch
                {
                    Executer.RollbackInmediate();
                    throw;
                }
                treports.Rows.Remove(xrow);
                ReportTree.SelectedNode.Remove();
            }
            else
            {
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
                int group_code = Convert.ToInt32(xrow["GROUP_CODE"]);
                string sqltext = "";
                // Check no reports
                if (LibraryConfig.ReportGroupsTable == "GINFORME")
                    sqltext = "SELECT COUNT(GRUPO) AS COUNTG FROM " + LibraryConfig.ReportTable +
                        " WHERE GRUPO=" + group_code.ToString();
                else
                    sqltext = "SELECT COUNT(REPORT_GROUP) AS COUNTG FROM " + LibraryConfig.ReportTable +
                        " WHERE REPORT_GROUP=" + group_code.ToString();

                Executer.StartTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    DataTable tcount = Executer.OpenInmediate(null, sqltext, "DD");
                    if (tcount.Rows.Count > 0)
                    {
                        if (tcount.Rows[0][0] != DBNull.Value)
                        {
                            if (Convert.ToInt32(tcount.Rows[0][0]) > 0)
                            {
                                throw new Exception(Translator.TranslateStr(1129));
                            }
                        }
                    }
                    // Check no groups inside
                    if (LibraryConfig.ReportGroupsTable == "GINFORME")
                        sqltext = "SELECT COUNT(CODIGO)AS COUNTG FROM " + LibraryConfig.ReportGroupsTable +
                          " WHERE GRUPO=" + group_code.ToString();
                    else
                        sqltext = "SELECT COUNT(GROUP_CODE) AS COUNTG FROM " + LibraryConfig.ReportGroupsTable +
                          " WHERE PARENT_GROUP=" + group_code.ToString();
                    tcount = Executer.OpenInmediate(null, sqltext, "DD");
                    if (tcount.Rows.Count > 0)
                    {
                        if (tcount.Rows[0][0] != DBNull.Value)
                        {
                            if (Convert.ToInt32(tcount.Rows[0][0]) > 0)
                            {
                                throw new Exception(Translator.TranslateStr(1130));
                            }
                        }
                    }
                    if (LibraryConfig.ReportGroupsTable == "GINFORME")
                        sqltext = "DELETE FROM " + LibraryConfig.ReportGroupsTable +
                            " WHERE CODIGO=" + group_code.ToString();
                    else
                        sqltext = "DELETE FROM " + LibraryConfig.ReportGroupsTable +
                            " WHERE GROUP_CODE=" + group_code.ToString();
                    Executer.Execute(sqltext);
                    Executer.Commit();
                    Executer.Flush();
                }
                catch
                {
                    Executer.RollbackInmediate();
                    throw;
                }
                tgroups.Rows.Remove(xrow);
                ReportTree.SelectedNode.Remove();

            }
        }
        private void GetAllReportNodes(TreeNode parent_node, List<TreeNode> list)
        {
            TreeNodeCollection collect = null;
            if (parent_node == null)
                collect = ReportTree.Nodes;
            else
                collect = parent_node.Nodes;
            foreach (TreeNode xnode in collect)
            {
                if (xnode.ImageIndex == 0)
                    list.Add(xnode);
                else
                    GetAllReportNodes(xnode, list);
            }
        }
        private void Bsearch_Click(object sender, EventArgs e)
        {
            List<TreeNode> report_list = new List<TreeNode>();
            GetAllReportNodes(null, report_list);
            int index = 0;
            // Localiza en el arbol de nodos
            if (ReportTree.SelectedNode != null)
            {
                while (index < report_list.Count)
                {
                    if (report_list[index] != ReportTree.SelectedNode)
                    {
                        index = index + 1;
                    }
                    else
                    {
                        index = index + 1;
                        break;
                    }
                }
            }
            string cadenabuscar = StringUtil.RemoveDiacritics(textboxsearch.Text.ToUpperInvariant());
            int initial_index = index;
            while (index < report_list.Count)
            {
                TreeNode nnode = report_list[index];
                string cadenanodo = StringUtil.RemoveDiacritics(nnode.Text.ToUpperInvariant());
                if (cadenanodo.IndexOf(cadenabuscar) >= 0)
                {
                    break;
                }
                index++;
            }
            // Se vuelve a buscar por el principio                
            if (index >= report_list.Count)
            {
                if (initial_index > 0)
                {
                    index = 0;
                    while (index < initial_index)
                    {
                        TreeNode nnode = report_list[index];
                        string cadenanodo = StringUtil.RemoveDiacritics(nnode.Text.ToUpperInvariant());
                        if (cadenanodo.IndexOf(cadenabuscar) >= 0)
                        {
                            break;
                        }
                        index++;
                    }
                    if (index >= initial_index)
                        index = report_list.Count;
                }
            }
            // Encontrado
            if (index < report_list.Count)
            {
                ReportTree.SelectedNode = report_list[index];
                ReportTree.SelectedNode.EnsureVisible();
            }


        }

        private void Baceptar_Click(object sender, EventArgs e)
        {
            OnAccept(this, new EventArgs());
        }
        private void Bcancel_Click(object sender, EventArgs e)
        {
            OnCancel(this, new EventArgs());
        }
        private void DefaultAccept(object sender,EventArgs args)
        {
            if (ReportTree.SelectedNode == null)
                return;
        }
        public string GetSelectedReport()
        {
            if (ReportTree.SelectedNode == null)
                return "";
            if (ReportTree.SelectedNode.ImageIndex == 0)
            {
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;

                return xrow[LibraryConfig.ReportSearchField].ToString();
            }
            else
                return "";
        }
        private void DefaultCancel(object sender, EventArgs args)
        {
            Form mainform = FindForm();
            if (mainform != null)
                mainform.Close();

        }
        public static bool SelectReport(ISqlExecuter iexecute, ReportLibraryConfig libconfig, SelectionModeType wselectionmode,
            ref string report_name, IWin32Window owner)
        {
            bool diagresult = false;
            Form fdia = new Form();
            try
            {
                report_name = "";
                OpenFromLibrary selectControl = new OpenFromLibrary();
                selectControl.Init(iexecute, libconfig, wselectionmode);
                fdia.Controls.Add(selectControl);
                selectControl.OnAccept += selectControl.DefaultAccept;
                selectControl.OnCancel += selectControl.DefaultCancel;
                if (owner == null)
                    fdia.ShowDialog();
                else
                    fdia.ShowDialog(owner);
                report_name = selectControl.SelectedReport;
                diagresult = report_name.Length > 0;
            }
            finally
            {
                fdia.Dispose();
            }
            return diagresult;
        }

        private void Textboxsearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Bsearch_Click(this, new EventArgs());
            }
        }
        private void UpdateFilter()
        {
            AddDocuments();

        }
        private bool InsideFilter(string newtext)
        {
            string filter = textfilter.Text.Trim().ToUpper();
            if (filter.Length == 0)
                return true;
            filter = StringUtil.RemoveDiacritics(filter.ToUpper());
            newtext = StringUtil.RemoveDiacritics(newtext.ToUpper());

            string[] words = filter.Split(' ');
            foreach (string word in words)
            {
                if (newtext.IndexOf(word.Trim()) < 0)
                    return false;
            }

            return true;
        }

        private void Textfilter_TextChanged(object sender, EventArgs e)
        {
            UpdateFilter();
        }

        private void ReportTree_DoubleClick(object sender, EventArgs e)
        {
            Baceptar_Click(this, new EventArgs());
        }

        private void Bexpand_Click(object sender, EventArgs e)
        {
            ReportTree.ExpandAll();
        }

        private void Bcontract_Click(object sender, EventArgs e)
        {
            ReportTree.CollapseAll();
        }
        private int NormalizeSortId(TreeNode childnode)
        {
            bool modified = false;
            int parent_group = 0;
            TreeNode parent_treenode = null;
            if (childnode.ImageIndex == 0)
            {
                DataRow xrow = (DataRow)childnode.Tag;
                parent_group = Convert.ToInt32(xrow["REPORT_GROUP"]);
                DataRow rowparent = tgroups.Rows.Find(parent_group);
                parent_treenode = (TreeNode)rowparent["NODE"];
            }
            else
            {
                DataRow xrow = (DataRow)childnode.Tag;
                parent_group = Convert.ToInt32(xrow["GROUP_CODE"]);
                parent_treenode = (TreeNode)xrow["NODE"];
            }
            TreeNodeCollection nodes = null;
            if (parent_treenode == null)
            {
                nodes = ReportTree.Nodes;
            }
            else
            {
                nodes = parent_treenode.Nodes;
            }
            int max_sort = 0;
            {
                List<TreeNode> nodes_null = new List<TreeNode>();
                 foreach (TreeNode nnode in nodes)
                {
                    DataRow xrow = (DataRow)nnode.Tag;
                    if (xrow["SORT_ID"] == DBNull.Value)
                    {
                        nodes_null.Add(nnode);
                    }
                    else
                    {
                        int sort_id = Convert.ToInt32(xrow["SORT_ID"]);
                        if (max_sort < sort_id)
                            max_sort = sort_id;
                    }
                }
                foreach (TreeNode nnode in nodes_null)
                {
                    max_sort = max_sort + 10;
                    UpdateSortId(nnode, max_sort);
                    modified = true;
                }
            }

            SortedList<int, List<TreeNode>> nodes_sorted = new SortedList<int, List<TreeNode>>();
            foreach (TreeNode nnode in nodes)
            {
                DataRow xrow = (DataRow)nnode.Tag;
                List<TreeNode> lnodes = null;
                int sort_id = Convert.ToInt32(xrow["SORT_ID"]);
                if (nodes_sorted.IndexOfKey(sort_id) >= 0)
                {
                    max_sort = max_sort + 10;
                    UpdateSortId(nnode, max_sort);
                    modified = true;
                    lnodes = new List<TreeNode>();
                    nodes_sorted.Add(max_sort, lnodes);
                }
                else
                {
                    lnodes = new List<TreeNode>();
                    nodes_sorted.Add(sort_id, lnodes);
                }
                lnodes.Add(nnode);
            }
            if(modified)
            {
                nodes.Clear();
                foreach (int key in nodes_sorted.Keys)
                {
                    nodes.Add(nodes_sorted[key][0]);
                }
            }
            return max_sort;
        }
        private void Bup_Click(object sender, EventArgs e)
        {
            if (ReportTree.SelectedNode == null)
                return;
            int parent_group = 0;
            TreeNode parent_treenode = null;
            TreeNode selnode = ReportTree.SelectedNode;
            if (selnode.ImageIndex == 0)
            {
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
                parent_group = Convert.ToInt32(xrow["REPORT_GROUP"]);
                DataRow rowparent = tgroups.Rows.Find(parent_group);
                parent_treenode = (TreeNode)rowparent["NODE"];
            }
            else
            {
                DataRow xrow = (DataRow)selnode.Tag;
                parent_group = Convert.ToInt32(xrow["GROUP_CODE"]);
                parent_treenode = (TreeNode)xrow["NODE"];
            }
            TreeNodeCollection nodes = null;
            if (parent_treenode == null)
            {
                nodes = ReportTree.Nodes;
            }
            else
            {
                nodes = parent_treenode.Nodes;
            }
            int nindex =  nodes.IndexOf(selnode);
            if (nindex <= 0)
                return;
            NormalizeSortId(selnode);
            nindex = nodes.IndexOf(selnode);
            if (nindex <= 0)
                return;
            TreeNode prevnode = selnode.PrevNode;
            if (prevnode != null)
            {
                int selnode_id = Convert.ToInt32(((DataRow)selnode.Tag)["SORT_ID"]);
                int prevnode_id = Convert.ToInt32(((DataRow)prevnode.Tag)["SORT_ID"]);
                UpdateSortId(prevnode, selnode_id);
                UpdateSortId(selnode, prevnode_id);
                int index = nodes.IndexOf(selnode);
                if (index > 0)
                {
                    nodes.RemoveAt(index);
                    nodes.Insert(index - 1, selnode);
                }
                ReportTree.SelectedNode = selnode;
            }
        }
        private void UpdateSortId(TreeNode nnode,int newid)
        {
            DataRow xrow = (DataRow)nnode.Tag;
            if (nnode.ImageIndex == 0)
            {
                string report_name = xrow[LibraryConfig.ReportSearchField].ToString();
                Executer.Execute("UPDATE " + LibraryConfig.ReportTable.ToString() + " SET SORT_ID=" + newid.ToString() +
                    " WHERE " + LibraryConfig.ReportSearchField + "=" + StringUtil.QuoteStr(xrow[LibraryConfig.ReportSearchField].ToString()));
                Executer.Commit();
            }
            else
            {
                string code_field = "GROUP_CODE";
                if (LibraryConfig.ReportGroupsTable == "GINFORME")
                    code_field = "CODIGO";

                    int group_code = Convert.ToInt32(xrow["GROUP_CODE"]);
                Executer.Execute("UPDATE " + LibraryConfig.ReportGroupsTable + " SET SORT_ID=" + newid.ToString() +
                    " WHERE "+code_field.ToString()+"=" + group_code.ToString());
                Executer.Commit();
            }
            xrow["SORT_ID"] = newid;
        }

        private void Bdown_Click(object sender, EventArgs e)
        {
            if (ReportTree.SelectedNode == null)
                return;
            int parent_group = 0;
            TreeNode parent_treenode = null;
            TreeNode selnode = ReportTree.SelectedNode;
            if (selnode.ImageIndex == 0)
            {
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
                parent_group = Convert.ToInt32(xrow["REPORT_GROUP"]);
                DataRow rowparent = tgroups.Rows.Find(parent_group);
                parent_treenode = (TreeNode)rowparent["NODE"];
            }
            else
            {
                DataRow xrow = (DataRow)selnode.Tag;
                parent_group = Convert.ToInt32(xrow["GROUP_CODE"]);
                parent_treenode = (TreeNode)xrow["NODE"];
            }
            TreeNodeCollection nodes = null;
            if (parent_treenode == null)
            {
                nodes = ReportTree.Nodes;
            }
            else
            {
                nodes = parent_treenode.Nodes;
            }
            int nindex = nodes.IndexOf(selnode);
            if (nindex <= 0)
                return;
            NormalizeSortId(selnode);
            nindex = nodes.IndexOf(selnode);
            if (nindex >= nodes.Count-1)
                return;
            TreeNode nextnode = selnode.NextNode;
            if (nextnode != null)
            {
                int selnode_id = Convert.ToInt32(((DataRow)selnode.Tag)["SORT_ID"]);
                int nextnode_id = Convert.ToInt32(((DataRow)nextnode.Tag)["SORT_ID"]);
                UpdateSortId(nextnode, selnode_id);
                UpdateSortId(selnode, nextnode_id);
                int index = nodes.IndexOf(selnode);
                if (index < nodes.Count - 1)
                {
                    nodes.RemoveAt(index);
                    nodes.Insert(index + 1, selnode);
                }
                ReportTree.SelectedNode = selnode;
            }
        }

        private void Bleft_Click(object sender, EventArgs e)
        {
            if (ReportTree.SelectedNode == null)
                return;
            int parent_group = 0;
            TreeNode parent_treenode = null;
            TreeNode selnode = ReportTree.SelectedNode;
            if (selnode.ImageIndex == 0)
            {
                DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
                 parent_group = Convert.ToInt32(xrow["REPORT_GROUP"]);
                DataRow rowparent = tgroups.Rows.Find(parent_group);
                parent_treenode = (TreeNode)rowparent["NODE"];
            }
            else
            {
                DataRow xrow = (DataRow)selnode.Tag;
                parent_group = Convert.ToInt32(xrow["GROUP_CODE"]);
                parent_treenode = (TreeNode)xrow["NODE"];
            }
            TreeNodeCollection nodes = null;
            if (parent_treenode == null)
            {
                return;
            }
            nodes = parent_treenode.Parent.Nodes;
            if (parent_treenode.Parent.Parent == null)
            {
                UpdateGroupId(selnode, null);
            }
            else
            {
                TreeNode parentnode = parent_treenode;
                UpdateGroupId(selnode, parent_treenode.Parent.Parent);
            }
        }
        private void UpdateGroupId(TreeNode nnode, TreeNode parent_node)
        {
            string group_code_field = "GROUP_CODE";
            if (LibraryConfig.ReportGroupsTable == "GINFORME")
                group_code_field = "CODIGO";
            int parent_group_code = 0;
            if (parent_node != null)
            {
                DataRow parentrow = (DataRow)parent_node.Tag;
                parent_group_code = Convert.ToInt32(parentrow[group_code_field]);

            }
            int max_id_1 = NormalizeSortId(nnode);
            int max_id_2 = max_id_1;
            if (parent_node == null)
                max_id_2 = NormalizeSortId(ReportTree.Nodes[0]);
            else
            {
                if (parent_node.Nodes.Count>0)
                    max_id_2 = NormalizeSortId(parent_node);
            }
            int max_id = Math.Max(max_id_1, max_id_2);
            max_id = max_id + 10;
            UpdateSortId(nnode, max_id);

            DataRow xrow = (DataRow)nnode.Tag;
            string group_parent_column = "GROUP_CODE";
            if (LibraryConfig.ReportGroupsTable == "GINFORME")
                group_parent_column = "GRUPO";

            if (nnode.ImageIndex == 0)
            {
                string report_name = xrow[LibraryConfig.ReportSearchField].ToString();
                Executer.Execute("UPDATE " + LibraryConfig.ReportTable.ToString() + " SET "+group_parent_column+"=" + parent_group_code.ToString() +
                    " WHERE " + LibraryConfig.ReportSearchField + "=" + StringUtil.QuoteStr(xrow[LibraryConfig.ReportSearchField].ToString()));
                Executer.Commit();
                if (parent_node == null)
                    xrow["REPORT_GROUP"] = 0;
                else
                    xrow["REPORT_GROUP"] = parent_group_code;
            }
            else
            {
                string code_field = "GROUP_CODE";
                if (LibraryConfig.ReportGroupsTable == "GINFORME")
                    code_field = "CODIGO";

                int group_code = Convert.ToInt32(xrow["GROUP_CODE"]);
                Executer.Execute("UPDATE " + LibraryConfig.ReportGroupsTable + " SET "+group_parent_column+"=" + parent_group_code.ToString() +
                    " WHERE " + code_field.ToString() + "=" + group_code.ToString());
                Executer.Commit();
                if (parent_node == null)
                    xrow["PARENT_GROUP"] = 0;
                else
                    xrow["PARENT_GROUP"] = parent_group_code;
            }
            if (nnode.Parent != null)
                nnode.Parent.Nodes.Remove(nnode);
            else
                ReportTree.Nodes.Remove(nnode);
            if (parent_node == null)
            {
                ReportTree.Nodes.Add(nnode);
            }
            else
                parent_node.Nodes.Add(nnode);
        }
        private void Arbol_DragOver(object sender, DragEventArgs e)
        {
            // Comprobación de validez de operación
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode DestinationNode = ((TreeView)sender).GetNodeAt(pt);
                if (DestinationNode == null)
                    return;

                // Carpeta con carpeta
                TreeNode original = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                if (DestinationNode.TreeView != original.TreeView)
                {
                    e.Effect = DragDropEffects.None;
                }
                if (original == DestinationNode)
                {
                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    if (DestinationNode.ImageIndex == 0)
                    {
                        e.Effect = DragDropEffects.None;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                        TreeNode destparent = DestinationNode.Parent;
                        while (destparent != null)
                        {
                            if (destparent == original)
                                return;
                            destparent = destparent.Parent;
                        }
                        e.Effect = DragDropEffects.Move;
                    }
                }
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void Arbol_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void Arbol_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void Arbol_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode DestinationNode = ((TreeView)sender).GetNodeAt(pt);

                // Carpeta con carpeta
                TreeNode original = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                if (original == DestinationNode)
                    return;
                if (DestinationNode.ImageIndex == 0)
                    return;
                // Check if destination is a child of original
                TreeNode destparent = DestinationNode.Parent;
                while (destparent != null)
                {
                    if (destparent == original)
                        return;
                    destparent = destparent.Parent;
                }
                UpdateGroupId(original, DestinationNode);
            }

        }

        private void Bexport_Click(object sender, EventArgs e)
        {
            if (ReportTree.SelectedNode == null)
                return;
            int parent_group = 0;
            TreeNode selnode = ReportTree.SelectedNode;
            if (selnode.ImageIndex != 0)
                return;
            DataRow xrow = (DataRow)ReportTree.SelectedNode.Tag;
            parent_group = Convert.ToInt32(xrow["REPORT_GROUP"]);
            string search_name = xrow[LibraryConfig.ReportSearchField].ToString();

            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Exportar informe";
            savedialog.Filter = "Archivos informe|*.rep";
            if (savedialog.ShowDialog(this.FindForm()) != DialogResult.OK)
                return;
            Executer.StartTransaction(IsolationLevel.ReadCommitted);
            try
            {
                string sql = "SELECT " + LibraryConfig.ReportField + " FROM " + LibraryConfig.ReportTable +
                    " WHERE " + LibraryConfig.ReportSearchField + "=" + StringUtil.QuoteStr(search_name);
                using (DataTable ntable = Executer.OpenInmediate(null, sql, "REPORT"))
                {
                    if (ntable.Rows.Count > 0)
                    {
                        DataRow resultrow = ntable.Rows[0];
                        if (resultrow[0] != DBNull.Value)
                        {
                            byte[] result = (byte[])resultrow[0];
                            Reportman.Drawing.StreamUtil.MemoryStreamToFile(new System.IO.MemoryStream(result), savedialog.FileName);
                        }

                    }

                }
            }
            finally
            {
                Executer.Commit();
            }

        }

        private void Bimport_Click(object sender, EventArgs e)
        {
            if (ReportTree.SelectedNode == null)
                return;
            int parent_group = 0;
            TreeNode selnode = ReportTree.SelectedNode;
            if (selnode.ImageIndex == 0)
            {
                UpdateReportFromFile();
                return;
            }
            DataRow xrow = (DataRow)selnode.Tag;
            parent_group = Convert.ToInt32(xrow["GROUP_CODE"]);
            TreeNode parent_treenode = (TreeNode)xrow["NODE"];
            OpenFileDialog opendialog = new OpenFileDialog();
            opendialog.Title = "Importar informe";
            opendialog.Filter = "Archivos informe|*.rep";
            if (opendialog.ShowDialog(this.FindForm()) != DialogResult.OK)
                return;
            // Validate report
            Reportman.Reporting.Report newreport = new Reporting.Report();
            newreport.LoadFromFile(opendialog.FileName);

            InsertNewreport(newreport, parent_group, parent_treenode);
        }
        private void InsertNewreport(Reportman.Reporting.Report newreport, int groupcode, TreeNode parent_treenode)
        {
            // Ask for the name
            string report_name = Reportman.Drawing.Forms.InputBox.Execute(Translator.TranslateStr(1131), Translator.TranslateStr(1132), "");
            // New Code
            string sqltext = "";
            Executer.StartTransaction(IsolationLevel.ReadCommitted);
            try
            {
                sqltext = "INSERT INTO " + LibraryConfig.ReportTable + "(" +
                     LibraryConfig.ReportSearchField + ',' + LibraryConfig.ReportField;

                if (LibraryConfig.ReportGroupsTable == "GINFORME")
                    sqltext = sqltext + ",GRUPO,DEF_USUARIO";
                else
                    sqltext = sqltext + ",REPORT_GROUP";
                sqltext = sqltext + ") VALUES (@REPNAME,@REPORT";
                if (LibraryConfig.ReportGroupsTable.Length > 0)
                {
                    sqltext = sqltext + ',' + groupcode.ToString() + ",0";
                }
                else
                {
                    sqltext = sqltext + ',' + groupcode.ToString();
                }
                sqltext = sqltext + ")";
                System.Data.Common.DbCommand ncommand = Executer.CreateCommand(sqltext);
                System.Data.Common.DbParameter param = ncommand.CreateParameter();
                param.ParameterName = "REPNAME";
                param.Value = report_name;
                ncommand.Parameters.Add(param);


                System.IO.MemoryStream mstream = new System.IO.MemoryStream();
                newreport.SaveToStream(mstream);

                param = ncommand.CreateParameter();
                param.ParameterName = "REPORT";
                param.DbType = DbType.Binary;
                param.Value = mstream.ToArray();


                ncommand.Parameters.Add(param);
                Executer.Execute(ncommand);


                Executer.Commit();
                Executer.Flush();
            }
            catch
            {
                Executer.RollbackInmediate();
                throw;
            }
            DataRow newrow = treports.NewRow();
            TreeNode newnode = new TreeNode();
            newrow["NODE"] = newnode;
            newrow[LibraryConfig.ReportSearchField] = report_name;
            newrow["REPORT_GROUP"] = groupcode;
            newnode.Text = report_name;
            newnode.ImageIndex = 0;
            newnode.SelectedImageIndex = 0;
            newnode.Tag = newrow;
            parent_treenode.Nodes.Add(newnode);
            treports.Rows.Add(newrow);
            ReportTree.SelectedNode = newnode;
            ReportTree.SelectedNode.EnsureVisible();

        }
        private void UpdateReportFromFile()
        {
            TreeNode selnode = ReportTree.SelectedNode;
            DataRow xrow = (DataRow)selnode.Tag;
            string report_name = xrow[LibraryConfig.ReportSearchField].ToString();
            OpenFileDialog opendialog = new OpenFileDialog();
            opendialog.Title = "Importar informe";
            opendialog.Filter = "Archivos informe|*.rep";
            if (opendialog.ShowDialog(this.FindForm()) != DialogResult.OK)
                return;
            Reportman.Reporting.Report newreport = new Reporting.Report();
            newreport.LoadFromFile(opendialog.FileName);
            if (MessageBox.Show("¿Actualizar informe " + report_name + " con el informe del archivo seleccionado?",
                "Confirmación", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;
            string sqltext = "";
            Executer.StartTransaction(IsolationLevel.ReadCommitted);
            try
            {
                sqltext = "UPDATE " + LibraryConfig.ReportTable + " SET " +
                     LibraryConfig.ReportField + "=@REPORT" +
                    " WHERE " + LibraryConfig.ReportSearchField + "=@REPNAME";
                System.Data.Common.DbCommand ncommand = Executer.CreateCommand(sqltext);
                System.Data.Common.DbParameter param = ncommand.CreateParameter();
                param.ParameterName = "REPNAME";
                param.Value = report_name;
                ncommand.Parameters.Add(param);


                System.IO.MemoryStream mstream = new System.IO.MemoryStream();
                newreport.SaveToStream(mstream);

                param = ncommand.CreateParameter();
                param.ParameterName = "REPORT";
                param.DbType = DbType.Binary;
                param.Value = mstream.ToArray();


                ncommand.Parameters.Add(param);
                Executer.Execute(ncommand);


                Executer.Commit();
                Executer.Flush();
            }
            catch
            {
                Executer.RollbackInmediate();
                throw;
            }
        }

        private void ReportTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (OnBeforeSelect != null)
            {
                BeforeSelectEventArgs args = new BeforeSelectEventArgs();
                if (e.Node != null)
                {
                    if (e.Node.ImageIndex == 0)
                    {
                        DataRow xrow = (DataRow)e.Node.Tag;
                        string report_name = xrow[LibraryConfig.ReportSearchField].ToString();
                        if (xrow["REPORT_GROUP"] != DBNull.Value)
                        {
                            args.GroupCode = Convert.ToInt32(xrow["REPORT_GROUP"]);
                        }
                        args.ReportName = report_name;
                    }
                    else
                    {
                        DataRow xrow = (DataRow)e.Node.Tag;
                        if (xrow["GROUP_CODE"] != DBNull.Value)
                        {
                            args.GroupCode = Convert.ToInt32(xrow["GROUP_CODE"]);
                        }
                    }
                }
                OnBeforeSelect(this, args);
                e.Cancel = args.Cancel;

            }

        }

        private void ReportTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (OnAfterSelect != null)
            {
                AfterSelectEventArgs args = new AfterSelectEventArgs();
                if (e.Node != null)
                {
                    if (e.Node.ImageIndex == 0)
                    {
                        DataRow xrow = (DataRow)e.Node.Tag;
                        string report_name = xrow[LibraryConfig.ReportSearchField].ToString();
                        if (xrow["REPORT_GROUP"] != DBNull.Value)
                        {
                            args.GroupCode = Convert.ToInt32(xrow["REPORT_GROUP"]);
                        }
                        args.ReportName = report_name;
                    }
                    else
                    {
                        DataRow xrow = (DataRow)e.Node.Tag;
                        if (xrow["GROUP_CODE"] != DBNull.Value)
                        {
                            args.GroupCode = Convert.ToInt32(xrow["GROUP_CODE"]);
                        }
                    }
                }
                OnAfterSelect(this, args);

            }

        }
    }
}


