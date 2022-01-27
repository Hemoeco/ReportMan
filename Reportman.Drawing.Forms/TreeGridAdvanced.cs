using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing.Printing;
using System.IO;

namespace Reportman.Drawing.Forms
{
    public delegate void ProgresEvent(int current, int count, ref bool cancelar);
    public class TreeGridAdvanced : DataGridView
    {
        const int INDENT_FIRST_UNSCALED = 8;
        public const int INDENT_MARGIN = 5;
        static int findent_first = 0;
        public static int INDENT_FIRST
        {
            get
            {
                if (findent_first == 0)
                {
                    findent_first = Convert.ToInt32(INDENT_FIRST_UNSCALED * GraphicUtils.DPIScale);
                }
                return findent_first;
            }

        }
        private static int findent_width;
        public static int INDENT_WIDTH
        {
            get
            {
                if (findent_width == 0)
                    findent_width = Convert.ToInt32(20 * GraphicUtils.DPIScale);
                return findent_width;
            }
        }
        public static bool DoubleBufferedPerformance = true;
        internal bool themesenabled;
        public long rowid_generator;
        internal VisualStyleRenderer rOpen = null;
        internal ImageList _imageList;
        internal VisualStyleRenderer rClosed = null;
        int FMaxLevel;
        public int MaxLevel
        {
            get
            {
                return FMaxLevel;
            }
        }
        public ImageList ImageList
        {
            get { return this._imageList; }
            set
            {
                this._imageList = value;
                Invalidate();
            }
        }
        protected override void OnSortCompare(DataGridViewSortCompareEventArgs e)
        {
            if (e.CellValue1 == DBNull.Value)
            {
                e.Handled = true;
                if (e.CellValue2 == DBNull.Value)
                    e.SortResult = 0;
                else
                    e.SortResult = -1;
                return;
            }
            else
            {
                if (e.CellValue2 == DBNull.Value)
                {
                    e.Handled = true;
                    e.SortResult = 1;
                    return;
                }
            }
            base.OnSortCompare(e);
        }
        public SortedList<int, DataGridViewCellStyle> paddings_list = new SortedList<int, DataGridViewCellStyle>();
        internal int treeboxwidth;
        public SortedList<long, TreeGridAdvancedNode> AllRows = new SortedList<long, TreeGridAdvancedNode>();
        public TreeGridAdvancedNode MainNode = new TreeGridAdvancedNode();
        private Graphics grint;
        public bool ShowLines = true;
        public TreeGridAdvanced()
            : base()
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                DoubleBuffered = false;
            else
                DoubleBuffered = DoubleBufferedPerformance;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.RowTemplate = new TreeGridRow();

            this.themesenabled = true;
        }
        public void ScaleColumns()
        {
            foreach (DataGridViewColumn ncol in Columns)
            {
                ncol.Width = Convert.ToInt32(ncol.Width * Reportman.Drawing.GraphicUtils.DPIScale);
            }
        }
        public void ClearNodes()
        {
            AllRows.Clear();
            MainNode.Clear();
            Rows.Clear();
        }
        protected override void Dispose(bool disposing)
        {
            MainNode.Clear();
            this.RowTemplate.Dispose();
            // no need to dispose datagridviewrows

            //foreach (TreeGridAdvancedNode nnode in AllRows.Values)
            //{
            //  nnode.Row.Dispose();
            //  nnode.Row = null;
            //}
            AllRows.Clear();
            paddings_list = new SortedList<int, DataGridViewCellStyle>();
            base.Dispose(disposing);
        }
        public Graphics GetGraphics()
        {
            if (grint == null)
                grint = CreateGraphics();
            return grint;
        }
        private static string ExcelColumnToName(int idx)
        {
            string nresult = ((char)(idx + (int)'A')).ToString();
            return nresult + ":" + nresult;
        }
        private static ExcelFileFormat FileFormatFromFilename(string filename)
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
        public void SaveToExcel(string filename)
        {
            //if (DataSource == null)
            //{
            //                throw new Exception("DataSource required to save to excel");
            //}
            List<int> ColumnIndexes = new List<int>();
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].Visible)
                    ColumnIndexes.Add(i);
            }
            Type objClassType;
            objClassType = Type.GetTypeFromProgID("Excel.Application");
            object excel = Activator.CreateInstance(objClassType);
            object[] param1 = new object[1];
            object wbs = excel.GetType().InvokeMember("Workbooks",
                System.Reflection.BindingFlags.GetProperty, null, excel, null);
            object wb = wbs.GetType().InvokeMember("Add",
                        System.Reflection.BindingFlags.InvokeMethod, null, wbs, null);
            param1[0] = 1;
            object shs = wb.GetType().InvokeMember("Sheets",
                System.Reflection.BindingFlags.GetProperty, null, wb, null);
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
            object cell = null;
            int nline = 1;
            object[] param2 = new object[2];
            param2[0] = nline;
            // Column titles
            for (int i = 0; i < ColumnIndexes.Count; i++)
            {
                param2[1] = i + 1;
                cell = cells.GetType().InvokeMember("Item",
                    System.Reflection.BindingFlags.GetProperty, null, cells, param2);
                param1[0] = Columns[ColumnIndexes[i]].HeaderText;
                cell.GetType().InvokeMember("Value",
                    System.Reflection.BindingFlags.SetProperty,
                    null, cell, param1);
            }
            // Grid values
            foreach (DataGridViewRow grow in Rows)
            {
                nline++;
                param2[0] = nline;
                // Row values
                for (int i = 0; i < ColumnIndexes.Count; i++)
                {
                    object nvalue = grow.Cells[ColumnIndexes[i]].Value;
                    if (nvalue != DBNull.Value)
                    {
                        param1[0] = nvalue;
                        param2[1] = i + 1;
                        cell = cells.GetType().InvokeMember("Item",
                            System.Reflection.BindingFlags.GetProperty, null, cells, param2);
                        cell.GetType().InvokeMember("Value",
                            System.Reflection.BindingFlags.SetProperty,
                            null, cell, param1);
                    }
                }
            }
            // Column numeric formats
            for (int i = 0; i < ColumnIndexes.Count; i++)
            {
                DataGridViewColumn ncol = Columns[ColumnIndexes[i]];
                if (ncol.DefaultCellStyle != null)
                {
                    if (ncol.DefaultCellStyle.Format.Length > 0)
                    {
                        object range = sh.GetType().InvokeMember("Range", System.Reflection.BindingFlags.GetProperty, null, sh, new object[] { ExcelColumnToName(i) });
                        string numberformat = "";
                        switch (ncol.DefaultCellStyle.Format)
                        {
                            case "N0":
                                numberformat = "##,##0";
                                break;
                            case "N1":
                                numberformat = "##,##0.0";
                                break;
                            case "N2":
                                numberformat = "##,##0.00";
                                break;
                            case "N3":
                                numberformat = "##,##0.000";
                                break;
                            case "N4":
                                numberformat = "##,##0.0000";
                                break;
                        }
                        if (numberformat.Length > 0)
                        {
                            range.GetType().InvokeMember("NumberFormat",
                                System.Reflection.BindingFlags.SetProperty,
                                null, range, new object[] { numberformat });
                        }
                    }
                }
            }
            object[] paramssav = null;
            paramssav = new object[2];
            // Excel 97 format
            ExcelFileFormat nformat = ExcelFileFormat.Auto;
            if (nformat == ExcelFileFormat.Auto)
                nformat = FileFormatFromFilename(filename);
            paramssav[1] = (int)nformat;
            paramssav[0] = filename;
            // If xlsx extension, force WorkBookNormal
            wb.GetType().InvokeMember("SaveAs", System.Reflection.BindingFlags.InvokeMethod, null, wb, paramssav);
            wb.GetType().InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, excel, null);

        }
        public void DrawNodes()
        {
            SuspendLayout();
            try
            {
                TreeGridAdvancedNode selnode = null;
                if (SelectedRows.Count == 1)
                    if (SelectedRows[0] is TreeGridRow)
                        selnode = ((TreeGridRow)SelectedRows[0]).Node;
                Rows.Clear();
                // Disable user sorting if mor than ona level
                bool disablesort = false;
                foreach (DataGridViewColumn ncol in Columns)
                {
                    if (ncol is TreeGridAdvancedColumn)
                    {
                        disablesort = true;
                    }
                }
                if (disablesort)
                {
                    foreach (DataGridViewColumn ncol in Columns)
                    {
                        ncol.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }

                MainNode.Expanded = true;
                List<TreeGridRow> AddList = new List<TreeGridRow>();
                foreach (TreeGridAdvancedNode nnode in MainNode.Childs)
                {
                    TreeGridAdvancedNode.FillExpanded(nnode, AddList);
                }
                Rows.AddRange(AddList.ToArray());
                if (selnode != null)
                {
                    foreach (DataGridViewRow xrow in Rows)
                    {
                        if (((TreeGridRow)xrow).Node == selnode)
                        {
                            CurrentCell = xrow.Cells[FirstDisplayedScrollingColumnIndex];
                        }
                    }
                }
            }
            finally
            {
                ResumeLayout();
            }
        }

        public TreeGridAdvancedNode FindNode(TreeGridRow nrow)
        {
            long aindex = (long)nrow.NodeId;
            return AllRows[aindex];
        }
        public void DrawLevel(int level)
        {
            TreeGridAdvancedNode.ExpandToLevel(MainNode, level);
            DrawNodes();
        }
        public TreeGridAdvancedNode AddNode(TreeGridAdvancedNode parent, object[] nvalues, bool addexpanded)
        {
            TreeGridRow nrow = new TreeGridRow();
            nrow.CreateCells(this, nvalues);

            return AddNode(parent, nrow, addexpanded);
        }
        public TreeGridAdvancedNode AddNode(TreeGridAdvancedNode parent, TreeGridRow xrow, bool addexpanded)
        {
            if (parent == null)
            {
                parent = MainNode;
                //throw new Exception("Node must have parent, use grid.MainNode instead");
            }
            rowid_generator++;
            TreeGridAdvancedNode newnode = new TreeGridAdvancedNode();
            newnode.RowId = rowid_generator;
            newnode.Parent = parent;
            newnode.Level = parent.Level + 1;
            xrow.ChildIndex = parent.Childs.Count;
            xrow.NodeId = rowid_generator;
            xrow.Level = newnode.Level;
            xrow.IsRoot = newnode.Level == 0;
            xrow.Parent = parent.Row;
            xrow.Node = newnode;
            newnode.Expanded = addexpanded;
            newnode.Row = xrow;
            newnode.Grid = this;

            if (FMaxLevel < newnode.Level)
                FMaxLevel = newnode.Level;
            AllRows.Add(rowid_generator, newnode);
            parent.Childs.Add(newnode);
            return newnode;
        }
        public void ExpandAll(bool redraw)
        {
            foreach (TreeGridAdvancedNode nnode in AllRows.Values)
            {
                nnode.Expanded = true;
            }
            if (redraw)
                DrawNodes();
        }
        public void RemoveBlankGroups(int colindex)
        {
            List<TreeGridAdvancedNode> toremove = new List<TreeGridAdvancedNode>();
            foreach (TreeGridAdvancedNode nnode in AllRows.Values)
            {
                string nvalue = nnode.Row.Cells[colindex].Value.ToString().Trim();
                if (nvalue.Length == 0)
                    toremove.Add(nnode);
            }
            foreach (TreeGridAdvancedNode nnode in toremove)
            {
                if (nnode.Childs.Count > 0)
                {
                    if (nnode.Parent != null)
                    {
                        int index = nnode.Parent.Childs.IndexOf(nnode);
                        foreach (TreeGridAdvancedNode childnode in nnode.Childs)
                        {
                            childnode.Parent = nnode.Parent;

                            nnode.Parent.Childs.Insert(index, childnode);
                        }
                        nnode.Childs.Clear();
                        nnode.Parent.Childs.Remove(nnode);
                    }
                    else
                    {
                    }
                }
                else
                {

                }
                nnode.RemoveFromGridForEver();
                AllRows.Remove(nnode.RowId);
            }
        }
        public void ExpandToLevel(int level, bool redraw)
        {
            foreach (TreeGridAdvancedNode nnode in AllRows.Values)
            {
                nnode.Expanded = nnode.Level <= level;
            }
            if (redraw)
                DrawNodes();
        }
        public bool FindTextNode(TreeGridRow nnode, string busca, int firstcolumn, int lastcolumn, bool searchchild)
        {
            bool found = false;

            for (int i = firstcolumn; i <= lastcolumn; i++)
            {
                if (Columns[i].Visible)
                {
                    string nvalue = "";
                    DataGridViewCell nextcell = nnode.Cells[i];
                    if (nextcell.Value != null)
                        nvalue = nextcell.Value.ToString().ToUpper().Replace(',', '.');
                    if (nvalue.IndexOf(busca) >= 0)
                    {
                        found = true;
                        // Expand all parents
                        if (nnode.Node.Parent != null)
                            nnode.Node.Parent.Expand();
                        //ExpandParent(nnode, null);
                        CurrentCell = nnode.Cells[i];
                        FirstDisplayedScrollingColumnIndex = CurrentCell.ColumnIndex;
                        break;
                    }
                }
            }
            if ((!found) && (searchchild))
            {
                foreach (TreeGridAdvancedNode childnoder in nnode.Node.Childs)
                {
                    found = FindTextNode(childnoder.Row, busca, 0, Columns.Count - 1, true);
                    if (found)
                        break;
                }
            }

            return found;
        }
        public void FindText(string ntext)
        {
            if (CurrentCell == null)
                return;
            // Busca desde la celda seleccionada hacia la derecha y abajo
            string busca = ntext.ToUpper().Replace(',', '.');

            if (busca.Length == 0)
                return;
            TreeGridRow currnode = (TreeGridRow)CurrentRow;
            int ncolumn = CurrentCell.ColumnIndex + 1;
            bool found = false;
            for (int i = currnode.Index; i < Rows.Count; i++)
            {
                found = FindTextNode((TreeGridRow)Rows[i], busca, ncolumn, Columns.Count - 1, true);
                if (found)
                    break;
                ncolumn = 0;
            }
            if (!found)
            {
                for (int i = 0; i < currnode.Index; i++)
                {
                    found = FindTextNode((TreeGridRow)Rows[i], busca, 0, Columns.Count - 1, true);
                    if (found)
                        break;
                }
                if (!found)
                {
                    FindTextNode(currnode, busca, 0, CurrentCell.ColumnIndex - 1, false);
                }
            }
        }
        public void CopySelectionToClipBoard()
        {
            DataGridViewSelectedCellCollection collect = SelectedCells;
            SortedList<int, DataGridViewColumn> acolumns = new SortedList<int, DataGridViewColumn>();
            SortedList<int, DataGridViewRow> arows = new SortedList<int, DataGridViewRow>();
            int index;
            foreach (DataGridViewCell ncell in collect)
            {
                if (Columns[ncell.ColumnIndex].Visible)
                {
                    index = acolumns.IndexOfKey(ncell.ColumnIndex);
                    if (index < 0)
                        acolumns.Add(ncell.ColumnIndex, Columns[ncell.ColumnIndex]);
                }
                index = arows.IndexOfKey(ncell.RowIndex);
                if (index < 0)
                    arows.Add(ncell.RowIndex, Rows[ncell.RowIndex]);
            }
            int i = 0;
            StringBuilder nresult = new StringBuilder();
            foreach (KeyValuePair<int, DataGridViewRow> npair in arows)
            {
                if (i > 0)
                    nresult.Append("\n");
                int j = 0;
                foreach (KeyValuePair<int, DataGridViewColumn> colpari in acolumns)
                {
                    if (j > 0)
                        nresult.Append("\t");
                    object nvalor = this[colpari.Key, npair.Key].Value;
                    if (nvalor == null)
                        nresult.Append("");
                    else
                    {
                        string valorstring = nvalor.ToString();
                        nresult.Append(valorstring);
                    }
                    j++;
                }
                i++;
            }
            Clipboard.SetDataObject(nresult.ToString(), false);
        }

    }

    public class TreeGridAdvancedNode
    {
        public TreeGridAdvanced Grid;
        public TreeGridRow Row;
        public long RowId;
        public int Level;
        public bool UpdatedCheck;
        public TreeGridAdvancedNode Parent;
        public List<TreeGridAdvancedNode> Childs = new List<TreeGridAdvancedNode>();
        public List<TreeGridRow> Rows = new List<TreeGridRow>();
        public bool Expanded;
        public static void ClearUpdatedCheck(TreeGridAdvancedNode node)
        {
            node.UpdatedCheck = false;
            if (node.Parent != null)
                ClearUpdatedCheck(node.Parent);
        }
        private static void ClearNode(TreeGridAdvancedNode nnode)
        {
            foreach (TreeGridAdvancedNode childnode in nnode.Childs)
            {
                ClearNode(childnode);
            }
            nnode.Childs.Clear();
            nnode.Rows.Clear();
        }
        public void Clear()
        {
            ClearNode(this);
        }
        public static void FillExpanded(TreeGridAdvancedNode nnode, List<TreeGridRow> AddList)
        {
            AddList.Add(nnode.Row);
            if (nnode.Expanded)
            {
                foreach (TreeGridAdvancedNode childnode in nnode.Childs)
                {
                    //if (childnode.Expanded)
                    FillExpanded(childnode, AddList);
                }
            }
        }
        public static void ExpandToLevel(TreeGridAdvancedNode nnode, int newlevel)
        {
            nnode.Expanded = nnode.Level <= newlevel;
            foreach (TreeGridAdvancedNode childnode in nnode.Childs)
            {
                ExpandToLevel(childnode, newlevel);
            }
        }
        public void RemoveFromGrid()
        {
            for (int i = Childs.Count - 1; i >= 0; i--)
            {
                TreeGridAdvancedNode nchild = Childs[i];
                nchild.RemoveFromGrid();
            }
            int index = Grid.Rows.IndexOf(Row);
            if (index >= 0)
                Grid.Rows.RemoveAt(index);
            Expanded = false;
        }
        public void RemoveFromGridForEver()
        {
            for (int i = Childs.Count - 1; i >= 0; i--)
            {
                TreeGridAdvancedNode nchild = Childs[i];
                nchild.RemoveFromGrid();
            }
            int index = Grid.Rows.IndexOf(Row);
            if (index >= 0)
                Grid.Rows.RemoveAt(index);
            if (Parent != null)
            {
                Parent.Childs.Remove(this);
            }
            Expanded = false;
        }

        public void Contract()
        {
            if (Expanded)
            {
                int index = Grid.Rows.IndexOf(Row);
                Grid.SuspendLayout();
                try
                {
                    if (index >= 0)
                    {
                        if ((Childs.Count > 100) || (Grid.Rows.Count > 1000))
                        {
                            Expanded = false;
                            Grid.DrawNodes();
                        }
                        else
                        {
                            for (int i = Childs.Count - 1; i >= 0; i--)
                            {
                                TreeGridAdvancedNode nchild = Childs[i];
                                nchild.RemoveFromGrid();
                                Expanded = false;
                            }
                        }
                    }
                }
                finally
                {
                    Grid.ResumeLayout();
                }
            }
        }
        public void Expand()
        {
            if (Expanded)
                return;
            int index = Grid.Rows.IndexOf(Row);
            if (index >= 0)
            {
                TreeGridRow[] xrows = new TreeGridRow[Childs.Count];
                for (int i = 0; i < Childs.Count; i++)
                {
                    xrows[i] = Childs[i].Row;
                }
                Grid.Rows.InsertRange(index + 1, xrows);
            }
            Expanded = true;
        }

    }
    public class TreeGridAdvancedColumn : DataGridViewTextBoxColumn
    {
        public void ClickNode(TreeGridRow nrow)
        {
            TreeGridAdvancedNode nnode = ((TreeGridAdvanced)(nrow.DataGridView)).FindNode(nrow);

            if (nnode.Expanded)
                nnode.Contract();
            else
                nnode.Expand();
        }
        internal bool _ismousecap;
        public TreeGridAdvancedColumn()
        {
            this.CellTemplate = new TreeGridAdvancedCell();
            this.ReadOnly = true;
        }
    }

    /// <summary>
    /// TreeGridAdvancedCell
    /// </summary>
    public class TreeGridAdvancedCell : DataGridViewTextBoxCell
    {
        public const int INDENT_WIDTH_UNSCALED = 20;
        public const int INDENT_MARGIN = 5;
        public const int INDENT_FIRST = 8;
        public const int DEF_GLYPHWIDTH = 15;
        private bool paddingok;
        private int glyphWidth;
        internal bool IsSited;

        int findent_width = 0;
        public int INDENT_WIDTH
        {
            get
            {
                if (findent_width == 0)
                    findent_width = Convert.ToInt32(INDENT_WIDTH_UNSCALED * GraphicUtils.DPIScale);
                return findent_width;
            }
        }
        private int _imageWidth = 0, _imageHeight = 0, _imageHeightOffset = 0;

        public TreeGridAdvancedCell()
        {
            glyphWidth = DEF_GLYPHWIDTH;
            this.IsSited = false;

        }

        public override object Clone()
        {
            TreeGridAdvancedCell c = (TreeGridAdvancedCell)base.Clone();

            c.glyphWidth = this.glyphWidth;

            return c;
        }
        public void PrepareDraw()
        {
            UpdateStyle();
        }
        // Performance bottleneck
        internal protected virtual void UpdateStyle()
        {
            if (this.RowIndex < 0)
                return;
            if (paddingok)
                return;
            int level = this.Level;

            Size preferredSize;

            // This line consumes lot of memory
            Graphics g = ((TreeGridAdvanced)DataGridView).GetGraphics();
            // Graphics g = ((TreeGridView)this.DataGridView).GetGraphics();

            preferredSize = this.GetPreferredSize(g, this.InheritedStyle, this.RowIndex, new Size(0, 0));
            //preferredSize = new Size(20, 21);

            Image image = ((TreeGridRow)DataGridView.Rows[RowIndex]).Image;

            if (image != null)
            {
                // calculate image size
                _imageWidth = image.Width + 2;
                _imageHeight = image.Height + 2;

            }
            else
            {
                _imageWidth = glyphWidth;
                _imageHeight = 0;
            }
            if (GraphicUtils.DPIScale != 1)
            {
                _imageWidth = Convert.ToInt32(_imageWidth * Reportman.Drawing.GraphicUtils.DPIScale);
                _imageHeight = Convert.ToInt32(_imageHeight * Reportman.Drawing.GraphicUtils.DPIScale);
            }
            Padding p = new Padding();
            // TODO: Make this cleaner
            if (preferredSize.Height < _imageHeight)
            {
                int leftpad = p.Left + (level * INDENT_WIDTH) + _imageWidth + INDENT_MARGIN;
                // Performance bottleneck changing padding takes lot of time
                TreeGridAdvanced _grid = (TreeGridAdvanced)this.DataGridView;
                if (_grid.paddings_list.IndexOfKey(leftpad) >= 0)
                {
                    this.Style = _grid.paddings_list[leftpad];
                }
                else
                {
                    Padding npad = new Padding(leftpad,
                                                   p.Top /*+ (_imageHeight / 2)*/, p.Right, p.Bottom /*+ (_imageHeight / 2)*/);
                    this.Style.Padding = npad;
                    _grid.paddings_list.Add(leftpad, this.Style);
                }
                _imageHeightOffset = 2;// (_imageHeight - preferredSize.Height) / 2;
            }
            else
            {
                int leftpad = p.Left + (level * INDENT_WIDTH) + _imageWidth + INDENT_MARGIN;

                Padding oldpad = this.Style.Padding;

                //                if ((oldpad.Left != npad.Left) || (oldpad.Top != npad.Top) || (oldpad.Right != npad.Right) || (oldpad.Bottom != npad.Bottom))
                if (oldpad.Left != leftpad)
                {
                    // Performance bottleneck changing padding takes lot of time
                    TreeGridAdvanced _grid = (TreeGridAdvanced)this.DataGridView;

                    if (_grid.paddings_list.IndexOfKey(leftpad) >= 0)
                    {
                        this.Style = _grid.paddings_list[leftpad];
                    }
                    else
                    {
                        Padding npad = new Padding(leftpad,
                                                       p.Top, p.Right, p.Bottom);
                        this.Style.Padding = npad;
                        _grid.paddings_list.Add(leftpad, this.Style);
                    }
                }
            }

            paddingok = true;
        }

        public int Level
        {
            get
            {
                if (RowIndex < 0)
                    return 0;
                TreeGridRow row = (TreeGridRow)DataGridView.Rows[RowIndex];
                if (row != null)
                {
                    return row.Level;
                }
                else
                    return -1;
            }
        }

        protected virtual int GlyphMargin
        {
            get
            {
                return ((this.Level - 1) * INDENT_WIDTH) + INDENT_MARGIN;
            }
        }

        protected virtual int GlyphOffset
        {
            get
            {
                return ((this.Level - 1) * INDENT_WIDTH);
            }
        }


        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (!paddingok)
            {
                PrepareDraw();
                cellStyle.Padding = Style.Padding;
            }

            DataGridViewRow xrow = this.DataGridView.Rows[rowIndex];
            if (!(xrow is TreeGridRow))
                return;
            TreeGridRow node = (TreeGridRow)xrow;
            if (node == null)
                return;

            Image image = node.Image;

            if (this._imageHeight == 0 && image != null)
                this.UpdateStyle();

            // paint the cell normally
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            // TODO: Indent width needs to take image size into account
            Rectangle glyphRect = new Rectangle(cellBounds.X + this.GlyphMargin, cellBounds.Y, INDENT_WIDTH, cellBounds.Height - 1);
            int glyphHalf = glyphRect.Width / 2;

            //TODO: This painting code needs to be rehashed to be cleaner
            int level = this.Level;

            //TODO: Rehash this to take different Imagelayouts into account. This will speed up drawing
            //		for images of the same size (ImageLayout.None)
            if (image != null)
            {
                Point pp;
                //int newimageHeight = Convert.ToInt32(_imageHeight * GraphicUtils.DPIScale);
                if (_imageHeight > cellBounds.Height)
                    pp = new Point(glyphRect.X + this.glyphWidth, cellBounds.Y + _imageHeightOffset);
                else
                    pp = new Point(glyphRect.X + this.glyphWidth, (cellBounds.Height / 2 - _imageHeight / 2) + cellBounds.Y);

                // Graphics container to push/pop changes. This enables us to set clipping when painting
                // the cell's image -- keeps it from bleeding outsize of cells.
                System.Drawing.Drawing2D.GraphicsContainer gc = graphics.BeginContainer();
                {
                    graphics.SetClip(cellBounds);
                    if (GraphicUtils.DPIScale == 1)
                        graphics.DrawImageUnscaled(image, pp);
                    else
                    {
                        int offset = Convert.ToInt32(2 * GraphicUtils.DPIScale);
                        int offsetx = Convert.ToInt32(glyphWidth * GraphicUtils.DPIScale) + INDENT_MARGIN;
                        int newimageHeight = _imageHeight - offset;
                        graphics.DrawImage(image, new Rectangle(glyphRect.X + offsetx, cellBounds.Y + offset / 2, newimageHeight, newimageHeight), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                    }
                }
                graphics.EndContainer(gc);
            }

            // Paint tree lines			
            if (((TreeGridAdvanced)(DataGridView)).ShowLines)
            {
                int drawoffset = Convert.ToInt32(4 * GraphicUtils.DPIScale);
                using (Pen linePen = new Pen(SystemBrushes.ControlDark, 1.0f))
                {
                    linePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    bool isLastSibling = node.IsLastSibling;
                    bool isFirstSibling = node.IsFirstSibling;
                    if (node.Level == 1)
                    {
                        // the Root nodes display their lines differently
                        if (isFirstSibling && isLastSibling)
                        {
                            // only node, both first and last. Just draw horizontal line
                            graphics.DrawLine(linePen, glyphRect.X + drawoffset, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                        }
                        else if (isLastSibling)
                        {
                            // last sibling doesn't draw the line extended below. Paint horizontal then vertical
                            graphics.DrawLine(linePen, glyphRect.X + drawoffset, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                            graphics.DrawLine(linePen, glyphRect.X + drawoffset, cellBounds.Top, glyphRect.X + drawoffset, cellBounds.Top + cellBounds.Height / 2);
                        }
                        else if (isFirstSibling)
                        {
                            // first sibling doesn't draw the line extended above. Paint horizontal then vertical
                            graphics.DrawLine(linePen, glyphRect.X + drawoffset, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                            graphics.DrawLine(linePen, glyphRect.X + drawoffset, cellBounds.Top + cellBounds.Height / 2, glyphRect.X + drawoffset, cellBounds.Bottom);
                        }
                        else
                        {
                            // normal drawing draws extended from top to bottom. Paint horizontal then vertical
                            graphics.DrawLine(linePen, glyphRect.X + drawoffset, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                            graphics.DrawLine(linePen, glyphRect.X + drawoffset, cellBounds.Top, glyphRect.X + drawoffset, cellBounds.Bottom);
                        }
                    }
                    else
                    {
                        if (isLastSibling)
                        {
                            // last sibling doesn't draw the line extended below. Paint horizontal then vertical
                            graphics.DrawLine(linePen, glyphRect.X + drawoffset, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                            graphics.DrawLine(linePen, glyphRect.X + drawoffset, cellBounds.Top, glyphRect.X + drawoffset, cellBounds.Top + cellBounds.Height / 2);
                        }
                        else
                        {
                            // normal drawing draws extended from top to bottom. Paint horizontal then vertical
                            graphics.DrawLine(linePen, glyphRect.X + drawoffset, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                            graphics.DrawLine(linePen, glyphRect.X + drawoffset, cellBounds.Top, glyphRect.X + drawoffset, cellBounds.Bottom);
                        }

                        // paint lines of previous levels to the root
                        TreeGridRow previousNode = node.Parent;
                        int horizontalStop = (glyphRect.X + drawoffset) - INDENT_WIDTH;

                        if (previousNode != null)
                        {
                            while (!previousNode.IsRoot)
                            {
                                if (previousNode.HasChildren && !previousNode.IsLastSibling)
                                {
                                    // paint vertical line
                                    graphics.DrawLine(linePen, horizontalStop, cellBounds.Top, horizontalStop, cellBounds.Bottom);
                                }
                                previousNode = previousNode.Parent;
                                horizontalStop = horizontalStop - INDENT_WIDTH;
                                if (previousNode == null)
                                    break;
                            }
                        }
                    }

                }
            }

            if (node.HasChildren)
            {
                TreeGridAdvanced _grid = (TreeGridAdvanced)DataGridView;
                // Paint node glyphs				
                if (node.Node.Expanded)
                {
                    if (_grid.themesenabled)
                    {
                        try
                        {
                            if (_grid.rOpen == null)
                            {
                                _grid.rOpen = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
                                _grid.treeboxwidth = System.Convert.ToInt32(WinFormsGraphics.DPIScale * 10);
                            }
                            _grid.rOpen.DrawBackground(graphics, new Rectangle(glyphRect.X, glyphRect.Y + (glyphRect.Height / 2) - Convert.ToInt32(4 * GraphicUtils.DPIScaleY),
                                _grid.treeboxwidth, _grid.treeboxwidth));
                            /*
                            Pen npen = Pens.Black;
                            int recwidth = 8;
                            int margin = 2;
                            int leftmargin = -recwidth / 2 - 2;
                            int topmargin = 0;
                            Rectangle nrect = new Rectangle(leftmargin + glyphRect.X + glyphRect.Width / 2 - recwidth / 2,
                                                            topmargin + glyphRect.Y + glyphRect.Height / 2 - recwidth / 2,
                                                            recwidth,
                                                            recwidth);
                            graphics.DrawLine(npen, new Point(nrect.X + margin, nrect.Top + recwidth / 2),
                                                    new Point(nrect.X + recwidth - margin, nrect.Top + recwidth / 2));*/
                        }
                        catch
                        {
                            _grid.themesenabled = false;
                        }

                    }
                    if (!_grid.themesenabled)
                    {
                        Pen npen = Pens.Black;
                        int recwidth = 8;
                        int margin = 2;
                        int leftmargin = -recwidth / 2 - 2;
                        int topmargin = 0;
                        Rectangle nrect = new Rectangle(leftmargin + glyphRect.X + glyphRect.Width / 2 - recwidth / 2,
                                                        topmargin + glyphRect.Y + glyphRect.Height / 2 - recwidth / 2,
                                                        recwidth,
                                                        recwidth);
                        using (Brush nbrush = new SolidBrush(cellStyle.BackColor))
                        {
                            graphics.FillRectangle(nbrush, nrect);
                            graphics.DrawRectangle(npen, nrect);
                            graphics.DrawLine(npen, new Point(nrect.X + margin, nrect.Top + recwidth / 2),
                                                    new Point(nrect.X + recwidth - margin, nrect.Top + recwidth / 2));
                        }
                    }
                }
                else
                {
                    if (_grid.themesenabled)
                    {
                        try
                        {
                            if (_grid.rClosed == null)
                            {
                                _grid.rClosed = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
                                _grid.treeboxwidth = System.Convert.ToInt32(WinFormsGraphics.DPIScale * 10);
                            }
                            //    node._grid.rClosed = new VisualStyleRenderer("Explorer::TreeView",2,1);
                            _grid.rClosed.DrawBackground(graphics, new Rectangle(glyphRect.X, glyphRect.Y + (glyphRect.Height / 2) - Convert.ToInt32(4 * GraphicUtils.DPIScaleY), _grid.treeboxwidth, _grid.treeboxwidth));
                        }
                        catch
                        {
                            _grid.themesenabled = false;
                        }
                    }
                    if (!_grid.themesenabled)
                    {
                        Pen npen = Pens.Black;
                        int recwidth = 8;
                        int margin = 2;
                        int leftmargin = -recwidth / 2 - 2;
                        int topmargin = 0;
                        using (Brush nbrush = new SolidBrush(cellStyle.BackColor))
                        {
                            Rectangle nrect = new Rectangle(leftmargin + glyphRect.X + glyphRect.Width / 2 - recwidth / 2,
                                                            topmargin + glyphRect.Y + glyphRect.Height / 2 - recwidth / 2,
                                                            recwidth,
                                                            recwidth);
                            graphics.FillRectangle(nbrush, nrect);
                            graphics.DrawRectangle(npen, nrect);
                            graphics.DrawLine(npen, new Point(nrect.X + margin, nrect.Top + recwidth / 2),
                                                    new Point(nrect.X + recwidth - margin, nrect.Top + recwidth / 2));
                            graphics.DrawLine(npen, new Point(nrect.X + recwidth / 2, nrect.Top + margin),
                                                    new Point(nrect.X + recwidth / 2, nrect.Top + recwidth - margin));
                        }
                    }
                }
            }

        }
        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseUp(e);

            TreeGridRow nrow = (TreeGridRow)this.DataGridView.Rows[this.RowIndex];
            TreeGridAdvancedColumn ncol = (TreeGridAdvancedColumn)DataGridView.Columns[ColumnIndex];
            if (ncol._ismousecap)
            {
                ncol._ismousecap = false;
                ncol.ClickNode(nrow);
            }
        }
        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (e.Location.X > this.InheritedStyle.Padding.Left)
            {
                base.OnMouseDown(e);
            }
            else
            {
                TreeGridRow nrow = (TreeGridRow)this.DataGridView.Rows[this.RowIndex];
                TreeGridAdvancedColumn ncol = (TreeGridAdvancedColumn)DataGridView.Columns[ColumnIndex];
                ncol._ismousecap = true;
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    public class TreeGridRow : DataGridViewRow
    {
        public TreeGridAdvancedNode Node;
        public TreeGridRow()
        {
            _imageIndex = -1;
        }
        public int ChildIndex;
        public bool IsFirstSibling
        {
            get
            {
                return (this.ChildIndex == 0);
            }
        }
        public bool HasChildren
        {
            get
            {
                if (Node == null)
                    return false;
                return (Node.Childs.Count > 0);
            }
        }
        [Browsable(false)]
        public bool IsLastSibling
        {
            get
            {
                if (Node == null)
                {
                    if (this.DataGridView != null)
                        return (Index == (this.DataGridView.Rows.Count - 1));
                    else
                        return false;
                }
                if (Node.Parent == null)
                    return (this.ChildIndex == Node.Grid.MainNode.Childs.Count - 1);

                TreeGridAdvancedNode parent = Node.Parent;
                if (parent == null)
                    return true;
                return (this.ChildIndex == parent.Childs.Count - 1);
            }
        }

        [Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageList ImageList
        {
            get
            {
                if (DataGridView != null)
                    return ((TreeGridAdvanced)DataGridView).ImageList;
                else
                    return null;
            }
        }
        internal int _imageIndex;
        [Category("Appearance"),
        Description("..."), DefaultValue(-1),
        TypeConverter(typeof(ImageIndexConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor", typeof(UITypeEditor))]
        public int ImageIndex
        {
            get { return _imageIndex; }
            set
            {
                _imageIndex = value;
                if (DataGridView != null)
                    DataGridView.InvalidateRow(this.Index);
            }
        }


        public long NodeId;
        public int Level;
        public bool IsRoot;
        public TreeGridRow Parent;
        public Image Image
        {
            get
            {
                if (_imageIndex != -1)
                {
                    if (this.ImageList != null && this._imageIndex < this.ImageList.Images.Count)
                    {
                        // get image from image index
                        return this.ImageList.Images[this._imageIndex];
                    }
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
