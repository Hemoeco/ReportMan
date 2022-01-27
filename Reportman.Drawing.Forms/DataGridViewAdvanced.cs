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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections;
using System.Threading;
using System.Data;
using System.Drawing.Drawing2D;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Reportman.Drawing.Forms
{
    public delegate void NextColumnFocusEvent(object sender,int currentcolumn,ref int nextcolumn);
    public delegate void DoubleClickControlEvent(object sender,Control ncontrol);
    /// <summary>
    /// Datagrid view Intro is tab
    /// </summary>
	public class DataGridViewAdvanced:DataGridView
	{
        public delegate void SelectNextRowAfterEnterKeyEvent(object sender, DataGridViewRow gridRow);
        public static bool DoubleBufferedPerformance = true;
        public static DataGridViewCellBorderStyle DefaultCellBorderStyle = DataGridViewCellBorderStyle.Single;
        private SortedList<int, Control> controlskeydown;
        private int taggedcontrols;
        private Graphics gr;
        public SelectNextRowAfterEnterKeyEvent OnSelectNextRowAfterEnterKey;

        float zerosize;
        /// <summary>
        /// Creates a new DataGridView with advanced features
        /// </summary>
        public DataGridViewAdvanced()
        {
            FAdjustColumnsFontChange = true;
            CellBorderStyle = DefaultCellBorderStyle;
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                DoubleBuffered = false;
            else
                DoubleBuffered = DoubleBufferedPerformance;
            InitializeComponent();
            FCreatePopUpForImage = true;
            controlskeydown = new SortedList<int, Control>();
            gr=this.CreateGraphics();
            /*using (Font origfont = new Font("Microsoft Sans Serif", 8.25f))
            {
                zerosize = gr.MeasureString("0000",origfont).Width;
            }*/
            zerosize = gr.MeasureString("0000", Font).Width;
            onlygridkeys = new SortedList<Keys, Keys>
            {
                { Keys.F1, Keys.F1 },
                { Keys.F2, Keys.F2 },
                { Keys.F3, Keys.F3 },
                { Keys.F4, Keys.F4 },
                { Keys.F5, Keys.F5 },
                { Keys.F6, Keys.F6 },
                { Keys.F7, Keys.F7 },
                { Keys.F8, Keys.F8 },
                { Keys.F9, Keys.F9 },
                { Keys.F10, Keys.F10 },
                { Keys.F11, Keys.F11 },
                { Keys.F12, Keys.F12 },
                { Keys.PageDown, Keys.PageDown },
                { Keys.PageUp, Keys.PageUp }
            };


        }
        public NextColumnFocusEvent NextColumnFocusEnter;
        public  DoubleClickControlEvent DoubleClickControl;
        private bool FValidateRowOnColumnChange;
        private bool FEnterAsTab;
        private bool FSkipReadOnly;
        private bool FAllowUserToInsertRows;
        private bool FCreatePopUpForImage;
        private bool FKeyPreview;
        private SortedList<Keys, Keys> onlygridkeys;
        /// <summary>
        /// Allow de user to insert a row in the middle of the grid
        /// </summary>
        [DefaultValue(false)]
        public bool AllowUserToInsertRows
        {
            get
            {
                return FAllowUserToInsertRows;
            }
            set
            {
                FAllowUserToInsertRows = value;
            }
        }
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
        }
        protected override void OnColumnWidthChanged(DataGridViewColumnEventArgs e)
        {

            base.OnColumnWidthChanged(e);
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }
        /// <summary>
        /// Allow keys pressed inside editing control passed to the grid itself
        /// </summary>
        [DefaultValue(false)]
        public bool KeyPreview
        {
            get
            {
                return FKeyPreview;
            }
            set
            {
                FKeyPreview = value;
            }
        }
        private bool FDoubleClicOpenImage;
        /// <summary>
        /// Allow de user to double clic to see an image
        /// </summary>
        [DefaultValue(false)]
        public bool DoubleClicOpenImage
        {
            get
            {
                return FDoubleClicOpenImage;
            }
            set
            {
                FDoubleClicOpenImage = value;
            }
        }
        /// <summary>
        /// Allow de user to double clic to see an image
        /// </summary>
        [DefaultValue(true)]
        public bool CreatePopUpForImage
        {
            get
            {
                return FCreatePopUpForImage;
            }
            set
            {
                FCreatePopUpForImage = value;
            }
        }
        private bool FAdjustColumnsFontChange;
        /// <summary>
        /// Adjust the column when the font chages
        /// </summary>
        [DefaultValue(true)]
        public bool AdjustColumnsFontChange
        {
            get
            {
                return FAdjustColumnsFontChange;
            }
            set
            {
                FAdjustColumnsFontChange = value;
            }
        }
        /// <summary>
        /// Force EndEdit after each column channge
        /// </summary>
        [DefaultValue(false)]
        public bool ValidateRowOnColumnChange
        {
            get
            {
                return FValidateRowOnColumnChange;
            }
            set
            {
                FValidateRowOnColumnChange = value;
            }
        }
        /// <summary>
        /// The enter and return keys will be handled as tab keys to provide horizontal
        /// displacement entering new data
        /// </summary>
        [DefaultValue(false)]
        public bool EnterAsTab
        {
            get
            {
                return FEnterAsTab;
            }
            set
            {
                FEnterAsTab = value;
            }
        }
        /// <summary>
        /// When advancing with keyboard, skip readonly columns for faster data enter
        /// </summary>
        [DefaultValue(false)]
        public bool SkipReadOnly
        {
            get
            {
                return FSkipReadOnly;
            }
            set
            {
                FSkipReadOnly = value;
            }
        }

        /*        
                /// New handler while inside editor
                /// </summary>
                /// <param name="keyData"></param>
                /// <returns></returns>
                protected override bool ProcessDialogKey(Keys keyData)
                {
                    if ((keyData == Keys.Return) || (keyData == Keys.Enter))
                    {
                        SendKeys.Send(((char)Keys.Tab).ToString());
                        return true;
                    }
                    else
                        return base.ProcessDialogKey(keyData);
                }
                /// <summary>
                /// New handler while outside editor
                /// </summary>
                /// <param name="e"></param>
                protected override void OnKeyDown(KeyEventArgs e)
                {
                    if ((e.KeyData == Keys.Return) || (e.KeyData == Keys.Enter))
                        SendKeys.Send(((char)Keys.Tab).ToString());
                    else
                      base.OnKeyDown(e);
                }
         */
        public List<string> DecimalKeyPadExceptions = new List<string>();
        /// <summary>
        /// New handler while inside editor
        /// </summary>
        /// <param name="keyData">Key data</param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (CurrentCell == null)
            {
                return base.ProcessDialogKey(keyData);
            }
             if ( (((keyData & Keys.KeyCode)== Keys.Return) || ((keyData & Keys.KeyCode)== Keys.Enter) && (FEnterAsTab)) 
                  || ((keyData & Keys.KeyCode) == Keys.Tab))
             {
                 if (ProcessKey(keyData))
                 {
                     return true;
                 }
             }
             if ((keyData & Keys.KeyCode) == Keys.Insert)
             {
                 if (AllowUserToInsertRows)
                 {
                     if (SelectedRows.Count==1)
                     {
                         DataGridViewRow nrow = SelectedRows[0];
                         this.Rows.Insert(nrow.Index, 1);
                     }
                 }
             }
            if (keyData == Keys.Decimal)
            {
                bool doconvert = true;
                if (DecimalKeyPadExceptions.Count>0)
                {
                    string data_property_name = this.Columns[CurrentCell.ColumnIndex].DataPropertyName;
                    if (data_property_name.Length >= 0)
                        if (DecimalKeyPadExceptions.IndexOf(data_property_name) >= 0)
                            doconvert = false;
                }
                if (doconvert)
                {
                    string defsep = Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
                    SendKeys.Send(defsep);
                    return true;
                }
            }
            /*DataGridViewCell ncell = CurrentCell;
            if (ncell != null)
            {
                DataGridViewColumn ncol = Columns[ncell.ColumnIndex];
                string nformat = ncol.DefaultCellStyle.Format;
                if (nformat.Length > 0)
                {
                    if (nformat[0] == 'N')
                    {
                        keyData = KeyUtil.ConvertKeyPad(keyData);
                        if ((int)keyData <= 255)
                        {
                            char nchar=(char)keyData;
                            if (nformat == "N0")
                            {
                                if (!char.IsDigit(nchar))
                                    return true;
                            }
                            else
                            {
                                if (char.IsDigit(nchar))
                                    return false;
                                string defsep = Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
                                if (defsep.IndexOf(nchar) >= 0)
                                    return false;
                                else
                                    return true;
                            }
                        }
                    }
                }
            }*/
            return base.ProcessDialogKey(keyData);
        }
        /// <summary>
        /// Preprocessing of messages
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool PreProcessMessage(ref Message msg)
        {
            Keys keydata;
           
            if (msg.Msg == 256)
            {
                Keys nkey = (Keys)msg.WParam & Keys.KeyCode;
                nkey = nkey | Control.ModifierKeys;
                return ProcessKey(nkey);
            }
            else
                if ((msg.Msg == 258) || (msg.Msg == 257))
                {
                    keydata = (Keys)msg.WParam & Keys.KeyCode;
                    keydata = keydata | Control.ModifierKeys;
                    if ((((((keydata & Keys.KeyCode) == Keys.Enter) || (keydata & Keys.KeyCode) == Keys.Return)) && FEnterAsTab)
                         || ((keydata & Keys.KeyCode) == Keys.Tab))
                    {
                        msg.Msg = 0;
                        return true;
                    }
                }
            return base.PreProcessMessage(ref msg);
        }
        public void ScaleColumns()
        {
            if (Reportman.Drawing.GraphicUtils.DPIScale == 1.0f)
                return;
            foreach (DataGridViewColumn ncol in Columns)
            {
                ncol.Width = Convert.ToInt32(ncol.Width * Reportman.Drawing.GraphicUtils.DPIScale);
            }
        }
        private bool ProcessKey(Keys keydata)
        {
            if  ((( ( ((keydata & Keys.KeyCode)== Keys.Enter) || (keydata & Keys.KeyCode)==Keys.Return)) && FEnterAsTab)
                 || ((keydata & Keys.KeyCode)==Keys.Tab))
            {
                if (CurrentCell == null)
                    return false;
                int rowindex = CurrentCell.RowIndex;
                bool nextrow = false;

                int current = CurrentCell.ColumnIndex;

                if ((keydata & Keys.Shift) > 0)
                {
                    current--;
                    while (current >= 0)
                    {
                        if (Columns[current].Visible && ((!FSkipReadOnly) || (!Columns[current].ReadOnly)))
                            break;
                        current--;
                    }
                    if (current < 0)
                    {
                        current = ColumnCount-1;
                        rowindex--;
                        if (rowindex >=0)
                        {
                            while (current >= 0)
                            {
                                if (Columns[current].Visible && ((!FSkipReadOnly) || (!Columns[current].ReadOnly)))
                                    break;
                                current--;
                            }
                        }
                        else
                            return true;
                    }
                }
                else
                {
                    int nextcolumn = -1;
                    // Assigned next column?
                    if (NextColumnFocusEnter != null)
                    {
                        if ((keydata & Keys.KeyCode)!=Keys.Tab)
                            NextColumnFocusEnter(this, current, ref nextcolumn);
                    }
                    current++;
                    if (nextcolumn < 0)
                    {
                        while (current < ColumnCount)
                        {
                            if (Columns[current].Visible && ((!FSkipReadOnly) || (!Columns[current].ReadOnly)))
                                break;
                            current++;
                        }
                    }
                    else
                    {
                        while (current < ColumnCount)
                        {
                            if (current == nextcolumn)
                                break;
                            current++;
                        }
                    }
                    if (current >= ColumnCount)
                    {
                        current = 0;
                        rowindex++;
                        nextrow = true;
                        if (rowindex < RowCount)
                        {
                            if (nextcolumn >= 0)
                            {
                                while (current < ColumnCount)
                                {
                                    if (current == nextcolumn)
                                        break;
                                    current++;
                                }
                            }
                            else
                            {
                                while (current < ColumnCount)
                                {
                                    if (Columns[current].Visible && ((!FSkipReadOnly) || (!Columns[current].ReadOnly)))
                                        break;
                                    current++;
                                }
                            }
                        }
                        else
                            return true;
                    }
                }
                if (ValidateRowOnColumnChange)
                {
                    FinishEdit();
                }
                if (current < ColumnCount)
                {
                    bool doselect=true;
                    if (CurrentCell.RowIndex != rowindex)
                    {
                        try
                        {
//                            bool isnewrow = false;
//                            if (CurrentRow.DataBoundItem == null)
//                                isnewrow = true;
//                            else
//                                if (((DataRowView)CurrentRow.DataBoundItem).Row==null)
//                                    isnewrow = true;

//                            if (((DataRowView)this.BindingContext[this.DataSource].Current).Row!=null)
//                            if (!isnewrow)
//                                FinishEdit();
                        }
                        catch(Exception ex)
                        {
                            OnDataError(true,new DataGridViewDataErrorEventArgs(ex,CurrentCell.ColumnIndex,CurrentCell.RowIndex,DataGridViewDataErrorContexts.Commit));
                            doselect=false;
                        }
                    }
                    if (doselect)
                    {
                        if ((CurrentCell.ColumnIndex!=current) || (CurrentCell.RowIndex!=rowindex))
                            if (rowindex<RowCount)
                            {
                                if (nextrow && (OnSelectNextRowAfterEnterKey != null))
                                {
                                    OnSelectNextRowAfterEnterKey(this, this.Rows[rowindex-1]);
                                }
                                CurrentCell = this[current, rowindex];
                            }
                    }
                }
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Variable del dise�ador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean resources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        protected override void OnLostFocus(EventArgs e)
        {
             base.OnLostFocus(e);
        }
        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = true;
            base.OnDataError(displayErrorDialogIfNoHandler, e);
        }
        public DataRow GetCurrentDataRow()
        {
            DataRow result = null;
            if (CurrentCell != null)
            {
                if (CurrentRow != null)
                {
                    if (CurrentRow.DataBoundItem != null)
                    {
                        if (CurrentRow.DataBoundItem is DataRowView)
                        {
                            result=((DataRowView)CurrentRow.DataBoundItem).Row;
                        }
                    }
                }
            }

            return result;
        }
        public DataColumn GetCurrentDataColumn()
        {
            DataColumn ncol = null;
            DataRow nrow=GetCurrentDataRow();
            if (nrow!=null)
                if (CurrentCell != null)
                {
                    if (CurrentCell.ColumnIndex>=0)
                    {
                        DataGridViewColumn vcol = Columns[CurrentCell.ColumnIndex];
                        if (vcol is DataGridViewImageColumn)
                        if (vcol.DataPropertyName.ToString().Length>0)
                        {
                            string colname=vcol.DataPropertyName.ToString();
                            if (nrow.Table != null)
                            {
                                if (nrow.Table.Columns.IndexOf(colname) >= 0)
                                {
                                    ncol = nrow.Table.Columns[colname];
                                }
                            }
                        }
                    }
                }

            return ncol;
        }
        public DataGridViewColumn GetCurrentColumn()
        {
            DataGridViewColumn vcol = null;
            DataRow nrow = GetCurrentDataRow();
            if (nrow != null)
                if (CurrentCell != null)
                {
                    if (CurrentCell.ColumnIndex >= 0)
                        vcol = Columns[CurrentCell.ColumnIndex];
                }

            return vcol;
        }
        public DataGridViewColumn GetDataColumn(string propname)
        {
            DataGridViewColumn vcol = null;
            foreach (DataGridViewColumn ncol in Columns)
            {
                if (ncol.DataPropertyName == propname)
                {
                    vcol = ncol;
                    break;
                }
            }
            return vcol;
        }
        public DataTable GetDataTable()
        {
            if (DataSource is DataView)
                return ((DataView)DataSource).Table;
            if (DataSource is DataTable)
                return (DataTable)DataSource;
            if (DataSource is BindingSource)
                    return ((DataView)((BindingSource)DataSource).DataSource).Table;

            return null;
        }
        private void CopyClic(object sender, EventArgs args)
        {
            DataRow nrow = GetCurrentDataRow();
            DataColumn ncol = GetCurrentDataColumn();
            if (nrow == null)
                return;
            if (ncol == null)
                return;
            if (nrow[ncol] != DBNull.Value)
            {
                byte[] data=(byte[])nrow[ncol];
                MemoryStream mems=new MemoryStream(data);
                Image nimage = Image.FromStream(mems);
                Clipboard.SetImage(nimage);
            }
        }
        private void DeleteClic(object sender, EventArgs args)
        {
            DataRow nrow = GetCurrentDataRow();
            DataColumn ncol = GetCurrentDataColumn();
            if (nrow == null)
                return;
            if (ncol == null)
                return;
            nrow[ncol] = DBNull.Value;
        }
        private void PasteClic(object sender, EventArgs args)
        {
            DataRow nrow = GetCurrentDataRow();
            DataColumn ncol = GetCurrentDataColumn();
            if (nrow == null)
                return;
            if (ncol == null)
                return;
            if (!Clipboard.ContainsImage())
            {
                MessageBox.Show("No hay una imagen en el portapapeles");
                return;
            }
            Image nimage = Clipboard.GetImage();
            MemoryStream mems = new MemoryStream();
            ImageCodecInfo icodec = GraphicUtils.GetImageCodec("image/jpeg");
            if (icodec != null)
            {
                EncoderParameters eparams = new EncoderParameters(1);
                System.Drawing.Imaging.EncoderParameter qParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality,
                    (long)100);
                eparams.Param[0] = qParam;
                nimage.Save(mems, icodec, eparams);
            }
            else
               nimage.Save(mems, System.Drawing.Imaging.ImageFormat.Bmp);
            nrow[ncol] = mems.ToArray();
            InvalidateCell(CurrentCell);
        }
        private void OpenImageFromFile(string archivo)
        {
            DataRow nrow = GetCurrentDataRow();
            DataColumn ncol = GetCurrentDataColumn();
            if (nrow == null)
                return;
            if (ncol == null)
                return;
            Image nimage = Image.FromFile(archivo);
            MemoryStream mems;
            using (FileStream fs = new FileStream(archivo, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                mems = StreamUtil.StreamToMemoryStream(fs);
            }
            nrow[ncol] = mems.ToArray();
            InvalidateCell(CurrentCell);
        }
        private void OpenImageClic(object sender, EventArgs args)
        {
            DataRow nrow = GetCurrentDataRow();
            DataColumn ncol = GetCurrentDataColumn();
            if (nrow == null)
                return;
            if (ncol == null)
                return;
            OpenFileDialog ndialog = new OpenFileDialog();
            ndialog.Title = Translator.TranslateStr(42);
            ndialog.Filter = Translator.TranslateStr(706) + "|*.bmp;" +
                "*.jpg;*.jpeg;" +
                "*.gif;*.png";
            if (ndialog.ShowDialog() != DialogResult.OK)
                return;
            OpenImageFromFile(ndialog.FileName);
        }
        public void PopMenuOpening(object sender,CancelEventArgs ev)
        {
            DataRow nrow = GetCurrentDataRow();
            if (nrow == null)
            {
                ev.Cancel = true;
                return;
            }
            DataGridViewColumn vcolumn=(DataGridViewColumn)((ContextMenuStrip)sender).Tag;
            // Select the current column
            CurrentCell = this[vcolumn.Index, CurrentCell.RowIndex];
            ContextMenuStrip nsender = (ContextMenuStrip)sender;
            foreach (ToolStripMenuItem nitem in nsender.Items)
            {
                bool desabilitar=false;
                if (ReadOnly)
                    desabilitar = true;
                if (vcolumn.ReadOnly)
                    desabilitar = true;
                // Pegar
                if (nitem.Text == Translator.TranslateStr(11))
                    nitem.Enabled = !desabilitar;
                // Abrir
                if (nitem.Text == Translator.TranslateStr(40))
                    nitem.Enabled = !desabilitar;
                // Limpiar
                if (nitem.Text == Translator.TranslateStr(150))
                    nitem.Enabled = !desabilitar;
            }

            DataColumn ncol = GetCurrentDataColumn();
            if (nrow == null)
                return;
            if (ncol == null)
                return;
            long imsize = 0;
            if (nrow[ncol] != DBNull.Value)                
            {
                if (nrow[ncol] is byte[])
                {
                    byte[] data = (byte[])nrow[ncol];
                    imsize = data.GetUpperBound(0);
                }
            }
            ToolStripMenuItem mimagesize = (ToolStripMenuItem)nsender.Items[nsender.Items.Count - 1];
            if (imsize > (1024 * 1024))
            {
                mimagesize.Text = (System.Convert.ToDecimal(imsize) / (1024 * 1024)).ToString("N2") + " Megabytes";
            }
            else
                mimagesize.Text = (System.Convert.ToDecimal(imsize) / (1024)).ToString("N2") + " Kbytes";
 
        }        
        // But� dret sobre un 
        public ContextMenuStrip CreateImagePopUp(DataGridViewColumn ncolumn)
        {            
            // Menu de imagen
            ContextMenuStrip nmenu = new ContextMenuStrip();
            nmenu.Tag = ncolumn;
            nmenu.Opening+=new CancelEventHandler(PopMenuOpening);
            // Copiar
            ToolStripMenuItem nitem;
            nitem = new ToolStripMenuItem(Translator.TranslateStr(10));
            nitem.Tag = this;
            nitem.Click += new EventHandler(CopyClic);
            nmenu.Items.Add(nitem);
            // Pegar
            nitem = new ToolStripMenuItem(Translator.TranslateStr(11));
            nitem.Tag = this;
            nitem.Click += new EventHandler(PasteClic);
            nmenu.Items.Add(nitem);
            // Abrir
            nitem = new ToolStripMenuItem(Translator.TranslateStr(42));
            nitem.Tag = this;
            nitem.Click += new EventHandler(OpenImageClic);
            nmenu.Items.Add(nitem);
            // Eliminar
            nitem = new ToolStripMenuItem(Translator.TranslateStr(150));
            nitem.Tag = this;
            nitem.Click += new EventHandler(DeleteClic);
            nmenu.Items.Add(nitem);

            // Image size
            nitem = new ToolStripMenuItem(Translator.TranslateStr(150));
            nitem.Enabled = false;
            nitem.Tag = this;
            nmenu.Items.Add(nitem);
            
            return nmenu;
        }
        public void OpenImage()
        {
            DataRow nrow = GetCurrentDataRow();
            DataColumn ncol = GetCurrentDataColumn();
            if (nrow == null)
                return;
            if (ncol == null)
                return;


            if (nrow[ncol] != DBNull.Value)
            {
                if (nrow[ncol] is byte[])
                {
                    string archivo = Path.GetTempFileName();
                    archivo = Path.ChangeExtension(archivo, ".bmp");
                    byte[] data = (byte[])nrow[ncol];
                    MemoryStream mems = new MemoryStream(data);
                    Image nimage = Image.FromStream(mems);
                    nimage.Save(archivo, System.Drawing.Imaging.ImageFormat.Bmp);
                    System.Diagnostics.Process proceso = new System.Diagnostics.Process();
                    proceso.StartInfo.FileName = archivo;
                    proceso.Start();
                }
            }
        }
        /// <summary>
        /// Implemented to allow double clic for opening images
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            if (FDoubleClicOpenImage)
            {
                if (CurrentCell!=null)
                    if (CurrentCell is DataGridViewImageCell)
                    {
                            OpenImage();
                    }
            }
            base.OnDoubleClick(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (CreatePopUpForImage)
                {
                    DataGridViewColumn ncol = GetCurrentColumn();
                    if (ncol is DataGridViewImageColumn)
                    {
                        if (ncol.ContextMenuStrip == null)
                            ncol.ContextMenuStrip = CreateImagePopUp(ncol);
                        ncol.ContextMenuStrip.Show(this.PointToScreen(new Point(e.X, e.Y)));
                    }
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnCurrentCellDirtyStateChanged(EventArgs e)
        {
            // Si se trata de un CheckBoxCell aceptamos su valor
            if (CurrentCell != null)
            {
                if (CurrentCell.ColumnIndex >= 0)
                {
                    if (Columns[CurrentCell.ColumnIndex] is DataGridViewCheckBoxColumn)
                    {
                        if (IsCurrentCellDirty)
                        {
                            // No aceptar el valor en filas nuevas porque deja de funcionar
                            // el datagrid
                            if (!Rows[CurrentCell.RowIndex].IsNewRow)
                               CommitEdit(DataGridViewDataErrorContexts.Commit);
                        }
                    }
                }
            }
            base.OnCurrentCellDirtyStateChanged(e);
        }
        float oldzerosize = 0;
        protected override void OnFontChanged(EventArgs e)
        {
            // Adjust size of column if property enable
            if (FAdjustColumnsFontChange)
            {
                float newsize = gr.MeasureString("0000",Font).Width;
                if (zerosize != 0)
                {
                    float relation = newsize / zerosize;
                    if ((relation > 1.05f) || (relation < 0.95f))
                    {
                        if (Math.Abs(oldzerosize-newsize)>2)
                        {
                            // Original widhts based on 8.25 size
                            for (int i = 0; i < Columns.Count; i++)
                            {
                                Columns[i].Width = System.Convert.ToInt32(Columns[i].Width * relation);

                            }
                            oldzerosize = newsize;
                        }
                    }
                }
            }
            try
            {
                // Error index out of bounds when parent changes
                base.OnFontChanged(e);
            }
            catch
            {

            }
        }
        private void GridAvKeyDown(object sender, KeyEventArgs e)
        {
            if (onlygridkeys.IndexOfKey(e.KeyCode)>=0)
            {
                 OnKeyDown(e);
            }
            
        }
        private void GridAvKeyPress(object sender, KeyPressEventArgs args)
        {
//            OnKeyPress(args);
        }
        public void InsertRowDataBound()
        {
            DataTable tabla=null;
            if (DataSource is DataView)
            {
                tabla = ((DataView)DataSource).Table;
            }
            else
                if (DataSource is DataTable)
                    tabla = (DataTable)DataSource;
                else if (DataSource is BindingSource)
                    tabla = ((DataView) ((BindingSource)DataSource).DataSource).Table;
                

            DataRow nrow = tabla.NewRow();
            tabla.Rows.Add(nrow);
            DataGridViewRow nrowview = null;
            foreach (DataGridViewRow nv in Rows)
            {
                if (nv.DataBoundItem != null)
                    if (((DataRowView)nv.DataBoundItem).Row == nrow)
                    {
                        nrowview = nv;
                        break;
                    }
            }

            if (nrowview != null)
            {
                int coln;
                for (coln = 0; coln < Columns.Count; coln++)
                {
                    if (Columns[coln].Visible)
                    {
                        break;
                    }
                }
                if (coln < Columns.Count)
                    CurrentCell = this[coln, nrowview.Index];
            }
        }
        public void DeleteDataBoundSelection(bool remove)
        {
            List<DataRow> lrows = new List<DataRow>();
            // Se elimina una fila si la hubiera
            // Primero mira si hay varias seleccionadas
           if( this.Rows.Count==0)
                return;
            if (CurrentRow != null)
            {
                DataGridViewRow nvrow = this.Rows[CurrentRow.Index];
                if (nvrow.DataBoundItem == null)
                {
                    this.BindingContext[this.DataSource].CancelCurrentEdit();
                    if (SelectedRows.Count == 0)
                        return;
                }
            }
            if (SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow rview in SelectedRows)
                {
                    DataRowView rview2 = (DataRowView)rview.DataBoundItem;
                    if (rview2 != null)
                    {
                        lrows.Add(rview2.Row);
                    }
                }
            }
            else
            {
                DataGridViewCell ncell = CurrentCell;
                if (ncell != null)
                {
                    DataGridViewRow rviewx = CurrentRow;
                    if (rviewx != null)
                    {
                        if (rviewx.DataBoundItem != null)
                        {
                            DataRowView nrview = (DataRowView)rviewx.DataBoundItem;
                            if (nrview.Row != null)
                            {
                                lrows.Add(nrview.Row);
                            }
                        }
                    }
                }
            }
            if (CurrentRow != null)
            {
                if (!SelectedRows.Contains(CurrentRow))
                    FinishEdit();
            }
            //this.BindingContext[this.DataSource].EndCurrentEdit();
            object dsource = DataSource;
            DataSource = null;
            try
            {
                for (int i = 0; i < lrows.Count; i++)
                {
                    if (remove)
                        lrows[i].Table.Rows.Remove(lrows[i]);
                    else
                        lrows[i].Delete();
                }
            }
            finally
            {
                DataSource = dsource;
            }
        }
        public void PasteFromClipBoardDataBound(bool assignnulls)
        {
            if (CurrentCell == null)
                return;
            int initialcolumn = CurrentCell.ColumnIndex;
            int initialrow = CurrentCell.RowIndex;
            DataTable ntable = null;
            if (DataSource is DataView)
                ntable = ((DataView)DataSource).Table;
            else
                if (DataSource is DataTable)
                    ntable = (DataTable)DataSource;
                else
                    if (DataSource is BindingSource)
                    {
                        BindingSource nbinding = (BindingSource)DataSource;
                        while (nbinding.DataSource is BindingSource)
                        {
                            nbinding = (BindingSource)nbinding.DataSource;
                        }
                        if (nbinding.DataSource is DataView)
                        {
                            ntable = ((DataView)nbinding.DataSource).Table;
                        }
                    }
            else
            if (DataSource is DataSet)
            {
                DataSet ndataset = (DataSet)DataSource;
                if (ndataset.Tables.IndexOf(DataMember) >= 0)
                    ntable = ndataset.Tables[DataMember];
            }
            if (ntable == null)
                return;
            object olddsource = DataSource;
            bool makevisible = false;
            //            DataSource = null;
            //            try
            SuspendLayout();
            try
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split('\n');
                if (lines.Length > 1000)
                {
                    if (Visible)
                    {
                        makevisible = true;
                        Visible = false;
                    }
                }
                foreach (string line in lines)
                {
                    if (line.Length > 0)
                    {
                        DataGridViewRow nvrow = this.Rows[initialrow];
                        DataRow nrow;
                        bool added = false;
                        // DataView nv = (DataView)nvrow.DataBoundItem;
                        if (nvrow.DataBoundItem == null)
                        {
                            //nrow = ntable.NewRow();
                            this.BindingContext[this.DataSource,DataMember].CancelCurrentEdit();
                            this.BindingContext[this.DataSource,DataMember].AddNew();
                            nrow = ((DataRowView)this.BindingContext[this.DataSource,DataMember].Current).Row;
                            added = true;
                        }
                        else
                            nrow = ((DataRowView)nvrow.DataBoundItem).Row;
                        string[] words = line.Split('\t');
                        int col = initialcolumn - 1;
                        int upbound = words.GetUpperBound(0);
                        for (int i = 0; i <= upbound; i++)
                        {
                            string word = words[i].Trim();
                            while (col < Columns.Count)
                            {
                                col++;
                                if (col < Columns.Count)
                                    if (Columns[col].Visible)
                                        break;
                            }
                            if (col < Columns.Count)
                            {
                                if (!Columns[col].ReadOnly)
                                {
                                    // Binary
                                    string propname = Columns[col].DataPropertyName;
                                    if (Columns[col] is DataGridViewImageColumn)
                                    {
                                        if (word.Length > 0)
                                        {
                                           
                                            nrow[propname] = Convert.FromBase64String(word);
                                        }
                                        else
                                            nrow[propname] = DBNull.Value;
                                    }
                                    else
                                    {
                                        if (word.Length > 0)
                                        {
                                            if (nrow[propname].ToString() != word)
                                                nrow[propname] = word;
                                        }
                                        else
                                        {
                                            if (assignnulls)
                                                nrow[propname] = DBNull.Value;
                                        }
                                    }
                                }
                            }
                        }
                        if (added)
                        {
                            this.BindingContext[this.DataSource].EndCurrentEdit();
                            //ntable.Rows.Add(nrow);
                        }
                        initialrow++;
                    }
                }
                Invalidate();
            }
            finally
            {
                if (makevisible)
                    Visible = true;
                ResumeLayout();
            }
            //            finally
            //            {
            //                DataSource = olddsource;
            //            }

        }


        public EventHandler NewRowLeave;

        protected override void OnRowLeave(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex>=0)
                if (Rows[e.RowIndex].IsNewRow)
                {
                    if (NewRowLeave != null)
                    {
                        base.OnRowLeave(e);
                        NewRowLeave(this, new EventArgs());
                        return;
                    }
                }
            base.OnRowLeave(e);
        }

        public void PasteFromClipBoardDataBoundAdd(bool assignnulls)
        {
            if (CurrentCell == null)
                return;
            int initialcolumn = CurrentCell.ColumnIndex;
            int initialrow = CurrentCell.RowIndex;
            object source_origin = DataSource;
            if (DataSource is BindingSource)
            {
                BindingSource bsource = (BindingSource)DataSource;
                source_origin = bsource.DataSource;
            }


            DataTable ntable = null;
            if (source_origin is DataView)
                ntable = ((DataView)source_origin).Table;
            else
                if (source_origin is DataTable)
                    ntable = (DataTable)source_origin;
            if (ntable == null)
                return;
            object oldsource = DataSource;
            //            DataSource = null;
            //            try
            SuspendLayout();
            // this.BindingContext[this.DataSource].SuspendBinding();
            try
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split('\n');
                // Count visible, not readonly columns
                SortedList<int,int> validcolumns = new SortedList<int, int>();
                SortedList<int, int> validtablecolumns = new SortedList<int, int>();
                int idxvalidcol = 0;
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (Columns[i].Visible)
                    {
                        if (!Columns[i].ReadOnly)
                        {
                            validcolumns.Add(idxvalidcol, i);
                            int idxcol = ntable.Columns.IndexOf(Columns[i].DataPropertyName);
                            validtablecolumns.Add(idxvalidcol, idxcol);
                        }
                        idxvalidcol++;
                    }
                }
                DataSource = null;
                object[] nvalues = new object[validtablecolumns.Count];
                int idxrow = 0;
                foreach (string line in lines)
                {
                    if (line.Length > 0)
                    {
                        DataRow nrow = ntable.NewRow();
                        string[] words = line.Split('\t');
                        int col = 0;
                        int upbound = words.GetUpperBound(0);
                        if (upbound!=nvalues.Length-1)
                            nvalues = new object[upbound-1]; 
                        for (int i = 0; i <= upbound; i++)
                        {
                            string word = words[i].Trim();

                            int idxvalid = validtablecolumns.IndexOfKey(i);
                            if (idxvalid >= 0)
                            {
                                if (col < validcolumns.Count)
                                {
                                    if (word.Length > 0)
                                    {
                                        nrow[validtablecolumns[i]] = word;
                                    }
                                    else
                                    {
                                        nrow[validtablecolumns[i]] = DBNull.Value;
                                    }
                                }
                            }
                        }
                        idxrow++;
                        ntable.Rows.Add(nrow);
                    }
                }
                Invalidate();
            }
            finally
            {
                //this.BindingContext[this.DataSource].ResumeBinding();
                DataSource = oldsource;
                ResumeLayout();
            }
            //            finally
            //            {
            //                DataSource = olddsource;
            //            }

        }

        /// <summary>
        /// Look for an active row or cell edition and finish it
        /// </summary>
        public void FinishEdit()
        {
            if (CurrentRow == null)
                return;
            if (this.DataSource==null)
                return;
            if (CurrentRow != null)
            {
                if (this.BindingContext[this.DataSource].Position < 0)
                    return;
                DataGridViewRow nvrow = this.Rows[CurrentRow.Index];
                if (nvrow.DataBoundItem == null)
                {
                    DataGridViewCell ncell = CurrentCell;

                    if (this.Rows.Count >= 1)
                    {
                        this.BindingContext[this.DataSource].CancelCurrentEdit();
                    }
                    //this.BindingContext[this.DataSource].EndCurrentEdit();
                }
                else
                {
                    DataGridViewCell ncell = CurrentCell;
                    if (ncell != null)
                    {
                        if (ncell.IsInEditMode)
                        {
                            if (EditingControl is AdvancedEditingControl)
                            {
                                AdvancedEditingControl econtrol = (AdvancedEditingControl)EditingControl;
                                econtrol.SaveCurrentValue();
                            }
                            this.EndEdit();
                        }
                    }
                    this.BindingContext[this.DataSource].EndCurrentEdit();
                }
            }
        }
        
        public void PasteFromClipBoardDataBoundNewRows()
        {
            DataTable ntable = null;
            if (DataSource is DataView)
                ntable = ((DataView)DataSource).Table;
            else
                if (DataSource is DataTable)
                    ntable = (DataTable)DataSource;
            if (ntable == null)
                return;
            object olddsource = DataSource;
            DataSource = null;
            try
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split('\n');
                foreach (string line in lines)
                {
                    DataRow nrow = ntable.NewRow();
                    string[] words = line.Split('\t');
                    int col = -1;
                    int upbound = words.GetUpperBound(0);
                    for (int i = 0; i <= upbound; i++)
                    {
                        string word = words[i];
                        while (col < Columns.Count)
                        {
                            col++;
                            if (col < Columns.Count)
                                if (Columns[col].Visible)
                                    break;
                        }
                        if (col < Columns.Count)
                            if (word.Length > 0)
                            {
                                if (!Columns[col].ReadOnly)
                                    nrow[Columns[col].DataPropertyName] = word;
                            }
                    }
                    ntable.Rows.Add(nrow);
                }
            }
            finally
            {
                DataSource = olddsource;
            }
        }
        public void CopySelectionToClipBoard()
        {
            DataGridViewSelectedCellCollection  collect = SelectedCells;
            SortedList<int,DataGridViewColumn> acolumns = new SortedList<int,DataGridViewColumn>();
            SortedList<int, DataGridViewRow> arows = new SortedList<int, DataGridViewRow>();
            int index;
            foreach (DataGridViewCell ncell in collect)
            {
                if (!Rows[ncell.RowIndex].IsNewRow)
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
            }
            int i = 0;
            StringBuilder nresult = new StringBuilder();
            foreach (KeyValuePair<int,DataGridViewRow> npair in arows)
            {
                if (i > 0)
                    nresult.Append("\n");
                int j = 0;
                foreach (KeyValuePair<int,DataGridViewColumn> colpari in acolumns)
                {
                    if (j>0)
                       nresult.Append("\t");
                    object nvalor = this[colpari.Key,npair.Key].Value;
                    if (nvalor == null)
                        nresult.Append("");
                    else
                    {
                        object avalue = nvalor;
                        if (nvalor is byte[])
                        {
                            string nbase64 = Convert.ToBase64String((byte[])nvalor);
                            nresult.Append(nbase64);
                        }
                        else
                            nresult.Append(nvalor.ToString());
                    }
                    j++;
                }
                i++;
            }
            Clipboard.SetDataObject(nresult.ToString(), false);            
        }
        public int IndexOfDataRow(DataRow nrow)
        {
            // Optimization
            // Look for the current row first
            if (CurrentCell != null)
            {
                if (CurrentRow!=null)
                    if (CurrentRow.DataBoundItem!=null)
                        if (((DataRowView)CurrentRow.DataBoundItem).Row == nrow)
                        {
                            return CurrentRow.Index;
                        }
            }
            int index = -1;
            foreach (DataGridViewRow rv in Rows)
            {
                if (rv.DataBoundItem is DataRowView)
                {
                    if (((DataRowView)rv.DataBoundItem).Row == nrow)
                    {
                        index = rv.Index;
                        break;
                    }
                }
            }


            return index;
        }
        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            int index = -1;
            if (e.Control.Tag != null)
                index = controlskeydown.IndexOfKey((int)e.Control.Tag);
            if (index < 0)
            {
                e.Control.KeyDown += new KeyEventHandler(GridAvKeyDown);
                e.Control.KeyPress += new KeyPressEventHandler(GridAvKeyPress);
                taggedcontrols++;
                e.Control.Tag = taggedcontrols;
                controlskeydown.Add(taggedcontrols, e.Control);
            }

            base.OnEditingControlShowing(e);
        }
        public static Point GetNextCell(DataGridView ngrid, int curcol, int currow)
        {
            int ncol = curcol;
            int nrow = currow;

            do
            {
                ncol = ncol + 1;
                if (ncol >= ngrid.Columns.Count)
                {
                    ncol = 0;
                    nrow++;
                    if (nrow >= ngrid.Rows.Count)
                        nrow = 0;
                }
                if ((nrow == currow) && (ncol == curcol))
                    return new Point(-1, -1);
            }
            while (!ngrid.Columns[ncol].Visible);

            return new Point(ncol, nrow);
        }
        public Point GetNextCell(int curcol, int currow)
        {
            return GetNextCell(this, curcol, currow);
        }
        public void DoDoubleClick(Control ncontrol)
        {
            if (DoubleClickControl!=null)
                DoubleClickControl(ncontrol,ncontrol);
        }
        public void SelectCellDataBound(DataRow nrow, string columnnames)
        {
            if (DataSource == null)
                return;
            if (!(DataSource is DataView))
            {
                if (DataSource is BindingSource)
                {
                    if (((DataView)((BindingSource)DataSource).DataSource).Table == null)
                        return;
                }
                else
                    return;
            }

            int colindex=-1;
            int rowindex = -1;
            Strings lnames = Strings.FromSemiColon(columnnames);
            lnames.RemoveBlanks();
            foreach (DataGridViewColumn ncol in Columns)
            {
                if (ncol.Visible)
                    if (lnames.IndexOf(ncol.DataPropertyName) >= 0)
                    {
                        colindex = ncol.Index;
                        break;
                    }
            }
            foreach (DataGridViewRow rv in Rows)
            {
                DataRowView vrow = (DataRowView)rv.DataBoundItem;
                if (vrow!=null)
                    if (vrow.Row != null)
                {
                    if (vrow.Row == nrow)
                        rowindex = rv.Index;
                }
            }
            if ((colindex >= 0) && (rowindex >= 0))
                CurrentCell = this[colindex, rowindex]; 
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
        public void InvalidateRowDatabound(DataRow xrow)
        {
            if (CurrentRow != null)
            {
                if (CurrentRow.DataBoundItem!=null)
                {
                    DataRowView nv = (DataRowView)CurrentRow.DataBoundItem;
                    if (nv.Row == xrow)
                    {
                        InvalidateRow(CurrentRow.Index);
                        return;
                    }
                }
            }
            foreach (DataGridViewRow gridrow in Rows)
            {
                DataRowView nv = (DataRowView)gridrow.DataBoundItem;
                if (nv != null)
                {
                    if (nv.Row == xrow)
                    {
                        InvalidateRow(gridrow.Index);
                        return;
                    }
                }
            }
        }
        public static void FindText(DataGridView grid,string ntext)
        {
            if (grid.CurrentCell == null)
                return;
            // Busca desde la celda seleccionada hacia la derecha y abajo
            string busca = ntext.ToUpper().Replace(',','.');

            if (busca.Length == 0)
                return;
            int currow = grid.CurrentCell.RowIndex;
            int currcol = grid.CurrentCell.ColumnIndex;

            int newrow = currow;
            int newcol = currcol;
            Point p;
            do
            {
                p = DataGridViewAdvanced.GetNextCell(grid,newcol, newrow);
                if (p.X >= 0)
                {
                    string nvalue = "";
                    if (grid[p.X, p.Y].Value!=null)
                        nvalue = grid[p.X, p.Y].Value.ToString().ToUpper().Replace(',','.');
                    if (nvalue.IndexOf(busca) >= 0)
                    {
                        grid.CurrentCell = grid[p.X, p.Y];
                        grid.FirstDisplayedScrollingColumnIndex = p.X;
                        break;
                    }

                    newcol = p.X;
                    newrow = p.Y;
                    if (p.Y == currow)
                        if (p.X == currcol)
                            break;
                }

            }
            while (p.X >= 0);    

        }


        private const int WM_SCROLL = 276; // Horizontal scroll
        private const int WM_VSCROLL = 277; // Vertical scroll
        private const int SB_LINEUP = 0; // Scrolls one line up
        private const int SB_LINELEFT = 0;// Scrolls one cell left
        private const int SB_LINEDOWN = 1; // Scrolls one line down
        private const int SB_LINERIGHT = 1;// Scrolls one cell right
        private const int SB_PAGEUP = 2; // Scrolls one page up
        private const int SB_PAGELEFT = 2;// Scrolls one page left
        private const int SB_PAGEDOWN = 3; // Scrolls one page down
        private const int SB_PAGERIGTH = 3; // Scrolls one page right
        private const int SB_PAGETOP = 6; // Scrolls to the upper left
        private const int SB_LEFT = 6; // Scrolls to the left
        private const int SB_PAGEBOTTOM = 7; // Scrolls to the upper right
        private const int SB_RIGHT = 7; // Scrolls to the right
        private const int SB_ENDSCROLL = 8; // Ends scroll
        private const int VK_PRIOR = 0x21; // Page up
        private const int VK_NEXT = 0x22; // PageDOWN
        private const int WM_KEYDOWN = 0x100; // PageDOWN

        [DllImport("user32.dll",CharSet=CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg,IntPtr wParam, IntPtr lParam);

        public void ScrollPageDown()
        {
            SendMessage(this.Handle, WM_KEYDOWN, (IntPtr)VK_NEXT,(IntPtr)0);
        }
        public void ScrollPageUp()
        {
            SendMessage(this.Handle, WM_KEYDOWN, (IntPtr)VK_PRIOR, (IntPtr)0);
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
        private static string ExcelColumnToName(int idx)
        {
            string nresult = ((char)(idx + (int)'A')).ToString();
            return nresult+":"+nresult;
        }
        public void SaveToCSV(string filename,string listseparator,Encoding encoding)
        {
            List<int> ColumnIndexes = new List<int>();
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].Visible)
                    ColumnIndexes.Add(i);
            }
            StringBuilder allbuilder = new StringBuilder();
            StringBuilder linebuilder = new StringBuilder();
            // Column titles
            for (int i = 0; i < ColumnIndexes.Count; i++)
            {
                if (i > 0)
                    linebuilder.Append(listseparator);
                linebuilder.Append(StringUtil.DoubleQuoteStr(Columns[ColumnIndexes[i]].HeaderText));
            }
            allbuilder.AppendLine(linebuilder.ToString());
            System.Globalization.NumberFormatInfo ninfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
            //ninfo.CurrencyDecimalSeparator = ".";
            //ninfo.CurrencyGroupSeparator = "";
            // Grid values
            foreach (DataGridViewRow grow in Rows)
            {
                linebuilder = new StringBuilder();
                // Row values
                for (int i = 0; i < ColumnIndexes.Count; i++)
                {
                    DataGridViewColumn ncol = Columns[ColumnIndexes[i]];
                    if (i > 0)
                        linebuilder.Append(listseparator);
                    object nvalue = grow.Cells[ColumnIndexes[i]].Value;
                    if (nvalue == null)
                        nvalue = DBNull.Value;
                    if (nvalue != DBNull.Value)
                    {
                        string numberformat = "";
                        switch (ncol.DefaultCellStyle.Format)
                        {
                            case "N0":
                                numberformat = "####0";
                                break;
                            case "N1":
                                numberformat = "####0.0";
                                break;
                            case "N2":
                                numberformat = "####0.00";
                                break;
                            case "N3":
                                numberformat = "####0.000";
                                break;
                            case "N4":
                                numberformat = "####0.0000";
                                break;
                        }
                        if (numberformat.Length > 0)
                        {
                            decimal decvalue = System.Convert.ToDecimal(nvalue);
                            linebuilder.Append(decvalue.ToString(numberformat, ninfo));
                        }
                        else
                        {
                            if ((nvalue is DateTime) && (ncol.DefaultCellStyle.Format.Length>0))
                            {
                                linebuilder.Append(((DateTime)nvalue).ToString(ncol.DefaultCellStyle.Format));
                            }
                            else
                                linebuilder.Append(StringUtil.DoubleQuoteStr(nvalue.ToString()));
                        }
                    }
                }
                allbuilder.AppendLine(linebuilder.ToString());
            }


            using (StreamWriter nwrite = new StreamWriter(filename, false, encoding, 4096))
            {
                nwrite.Write(allbuilder.ToString());
            }

        }
        public void SaveToExcel(string filename)
        {
            SaveToExcel(this, filename, "GiveAway", false);
        }
        public static bool SaveToExcel(DataGridView gridav, string filename, string title, bool showSaveDialog)
        { 
            if (showSaveDialog)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Excel|*.xlsx";
                dialog.Title = "Excel";
                if ((filename != null) && (filename.Length>0))
                    dialog.FileName = filename;
                if (dialog.ShowDialog(gridav.FindForm()) != DialogResult.OK)
                    return false;
                filename = dialog.FileName;
            }
            using (ClosedXML.Excel.XLWorkbook workbook = new ClosedXML.Excel.XLWorkbook())
            {
                ClosedXML.Excel.IXLWorksheet worksheet = workbook.Worksheets.Add(title);
                int linia = 1;
                int columna = 1;
                float anchoLetra = gridav.CreateGraphics().MeasureString("l", gridav.Font).Width;
                foreach (DataGridViewColumn gridcolumn in gridav.Columns)
                {
                    if (gridcolumn.Visible)
                    {
                        ClosedXML.Excel.IXLColumns cols = worksheet.Columns(columna, columna);
                        cols.Width = gridcolumn.Width / anchoLetra;
                        worksheet.Cell(linia, columna).Value = gridcolumn.HeaderText;
                        columna++;
                    }
                }
                linia++;
                foreach (DataGridViewRow gridrow in gridav.Rows)
                {
                    columna = 1;
                    foreach (DataGridViewColumn gridcolumn in gridav.Columns)
                    {
                        if (gridcolumn.Visible)
                        {  
                            object value = gridrow.Cells[gridcolumn.Index].Value;
                            if (value != null)
                            {
                                switch (value.GetType().ToString())
                                {
                                    case "System.Byte[]":
                                        value = DBNull.Value;
                                        break;
                                    default:
                                        break;
                                }
                                if (value != DBNull.Value)
                                    worksheet.Cell(linia, columna).Value = value;
                            }
                            columna++;
                        }
                    }
                    linia++;
                }
                workbook.SaveAs(filename);
            }
            return true;
        }
        public void SaveToExcelOleAutomation(string filename)
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
                param2[1] = i+1;
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



        #region Windows Designer generated code

        /// <summary>
        /// Required by Windows forms designer
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
	}
    public class ScrollTextPanel : Panel
    {
        System.Windows.Forms.Timer ntimer = new System.Windows.Forms.Timer();
        float position = 0;
        StringFormat nstringformat;
        public ScrollTextPanel()
        {
            nstringformat = new StringFormat(StringFormat.GenericDefault);
            nstringformat.Alignment = StringAlignment.Near;
            nstringformat.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox;
            nstringformat.LineAlignment = StringAlignment.Near;
            DoubleBuffered = true;
            ntimer.Enabled = false;
            ntimer.Interval = 33;
            ntimer.Tick += new EventHandler(TimerTick);
        }

        void TimerTick(object sender, EventArgs e)
        {
            position = position + Speed;
            Invalidate();
        }
        public float Speed = 2.0f;
        bool FScrollText;
        public bool ScrollText
        {
            get
            {
                return FScrollText;
            }
            set
            {
                FScrollText = value;
                if (FScrollText)
                {
                    ntimer.Enabled = true;
                    Invalidate();
                }
                else
                {
                    ntimer.Enabled = false;
                    Invalidate();
                }
            }
        }
        public string ScrollString = "";
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!ntimer.Enabled)
                return;
            if (ScrollString.Length == 0)
                return;

            SizeF nsizef = new SizeF(Width, Height);

            SizeF xsize = e.Graphics.MeasureString(ScrollString, Font, nsizef, nstringformat);
            int labelwidth = System.Convert.ToInt32(xsize.Width);
            if (labelwidth==0)
                return;

            int labelposition = System.Convert.ToInt32(position);
            position = position % (Width+labelwidth);

            PointF npoint = new PointF(Width-position,(Height-System.Convert.ToInt32(xsize.Height))/2);

            using (SolidBrush nbrush = new SolidBrush(ForeColor))
            {
                e.Graphics.DrawString(ScrollString, Font, nbrush, npoint, nstringformat);
            }
        }        
       
    }
    public class TransparentPanel : Panel
    {
        public TransparentPanel():base()
        {
            // SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //BackColor = Color.FromArgb(50, 100, 100, 100);
            //DoubleBuffered = true;
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
        /*protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                    cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED



                //cp.Style = cp.Style | 0x04000000 | 0x02000000; // WS_CLIPSIBLINGS WS_CLIPCHILDREN
                return cp;
            }
        }*/
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var sb = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.FillRectangle(sb, this.DisplayRectangle);
        }
    }
    public class PanelAdvanced: Panel
    {
        protected override void OnSizeChanged(EventArgs e)
        {
#if MONO
                    base.OnSizeChanged(e);
#else
            if (this.IsHandleCreated)
            {

                this.BeginInvoke((MethodInvoker)delegate
                {

                    base.OnSizeChanged(e);

                });

            }
#endif
        }
    } 

}
