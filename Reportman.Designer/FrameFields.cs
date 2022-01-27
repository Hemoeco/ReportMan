using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Reportman.Reporting;
using Reportman.Drawing;
using Reportman.Drawing.Forms;

namespace Reportman.Designer
{
    public partial class FrameFields : UserControl
    {
        public event EventHandler OnReportChange;
        public FrameFields()
        {
            InitializeComponent();
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
            TreeNode anew;
            // Clear
            RView.BeginUpdate();
            try
            {
                RView.Nodes.Clear();
                if (FReport != null)
                {
                    // Show data sets
                    foreach (DataInfo dinfo in FReport.DataInfo)
                    {
                        anew = RView.Nodes.Add(dinfo.Alias);
                        anew.Tag = dinfo;
                        anew.Nodes.Add("");
                    }
                    // Show variables
                    anew = RView.Nodes.Add(Translator.TranslateStr(1147));
                    anew.Tag = FReport.Evaluator;
                    anew.Nodes.Add("");
                }
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
            using (FrameFields fm = new FrameFields())
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

        private void RView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // Change items inside the tree no tag assigned to the child
            if (e.Node.Tag is DataInfo)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    if (e.Node.Nodes[0].Tag == null)
                    {
                        RView.BeginUpdate();
                        try
                        {
                            e.Node.Nodes.Clear();
                            DataInfo ninfo = (DataInfo)e.Node.Tag;
                            ninfo.Connect();
                            foreach (DataColumn ncolumn in ninfo.Data.Columns)
                            {
                                FieldInfo newinfo = new FieldInfo(ninfo, ncolumn.ColumnName);
                                newinfo.DataType = ncolumn.DataType;
                                
                                if (ninfo.Data.ColumnSizes.IndexOfKey(ncolumn.ColumnName) >= 0)
                                {
                                    // Show field size only for strings
                                    if (newinfo.DataType.ToString()=="System.String")
                                      newinfo.fieldsize = ninfo.Data.ColumnSizes[ncolumn.ColumnName];
                                }
                                string colcaption = ncolumn.ColumnName + " - " + newinfo.DataType.ToString();
                                if (newinfo.fieldsize != 0)
                                    colcaption = colcaption + "(" + newinfo.fieldsize.ToString() + ")";                                
                                TreeNode newnode = e.Node.Nodes.Add(colcaption);
                                newnode.Tag = newinfo;
                            }

                        }
                        finally
                        {
                            RView.EndUpdate();
                        }                        
                    }
                }
            }
            if (e.Node.Tag is Evaluator)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    if (e.Node.Nodes[0].Tag == null)
                    {
                        RView.BeginUpdate();
                        try
                        {
                            e.Node.Nodes.Clear();
                            Evaluator eval = (Evaluator)e.Node.Tag;
                            Strings list=FReport.GetReportVariables();
                            foreach (string s in list)
                            {
                                TreeNode newnode = e.Node.Nodes.Add(s);
                                newnode.Tag = new FieldInfo(null, s);
                            }
                        }
                        finally
                        {
                            RView.EndUpdate();
                        }
                    }
                }
            }
        }

      private void RView_DragEnter(object sender, DragEventArgs e)
      {        
        e.Effect = DragDropEffects.None;
      }

      private void RView_ItemDrag(object sender, ItemDragEventArgs e)
      {
          //
          TreeNode aNode = (TreeNode)e.Item;
          if (aNode.Tag is FieldInfo)
          {
              FieldInfo finfo = (FieldInfo)aNode.Tag;
              string expression = finfo.fieldname;
              if (finfo.ninfo != null)
                  expression = finfo.ninfo.Alias + "." + expression;
              if (finfo.fieldsize > 0)
              {
                if (finfo.fieldsize<1000)
                  expression = expression + "_($" + finfo.fieldsize.ToString("000");
              }
              RView.DoDragDrop("__X__X__XX"+expression, DragDropEffects.All);
          }
      }

      private void RView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
      {
          switch (e.Effect)
          {
              case DragDropEffects.Move:
                  e.UseDefaultCursors = true;
                  break;
              case DragDropEffects.Copy:
                  e.UseDefaultCursors = true;
                  break;
              default:
                  e.UseDefaultCursors = false;
                  Cursor.Current = Cursors.No;
                  break;
          }          
      }

      private void RView_DragLeave(object sender, EventArgs e)
      {
      }
    }
      public class FieldInfo
      {
        public DataInfo ninfo;
        public string fieldname;
        public Type DataType;
        public int fieldsize;
        public FieldInfo(DataInfo dinfo, string fname)
        {
          ninfo = dinfo;
          fieldname = fname;
        }
      }
}
