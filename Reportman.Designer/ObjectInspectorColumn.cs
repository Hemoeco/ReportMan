using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Reportman.Drawing;
using Reportman.Drawing.Forms;
using Reportman.Reporting;

namespace Reportman.Designer
{
    public enum ObjectInspectorCellType
    {
        Text, Expression, Integer, Decimal, Boolean, DropDown, DropDownList, Color, Image, FontName, FontStyle, SQL,
        ConnectionString
    };

    public class ObjectInspectorColumn : DataGridViewColumn
    {
        public FrameMainDesigner FrameMain;
        public ObjectInspectorColumn()
            : base(new ObjectInspectorCell())
        {
        }
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                base.CellTemplate = value;
            }
        }
    }

    public class ObjectInspectorCell : DataGridViewTextBoxCell
    {
        DataGridViewComboBoxEditingControl ComboBoxPicker;
        CheckBoxPickerControl CheckBoxPicker;
        DataGridViewTextBoxEditingControl TextBoxPicker;
        EllipsisEditingControl EllipsisPicker;
        PanelColorPicker ColorPickerc;
        PanelImagePicker ImagePickerc;
        NumericUpDownPickerControl NumericPicker;
        EventHandler ComboPickerClick;

        private EllipsisClick ClickFontNameEvent;
        private EllipsisClick ClickExpressionEvent;
        private EllipsisClick ClickSQLEvent;
        private EllipsisClick ClickConnectionStringEvent;
        private EventHandler ClickFontStyleEvent;
        public ObjectInspectorCell()
            : base()
        {
            ClickFontNameEvent = new EllipsisClick(ClickFontName);
            ClickExpressionEvent = new EllipsisClick(ClickExpression);
            ClickSQLEvent = new EllipsisClick(ClickSQL);
            ClickConnectionStringEvent = new EllipsisClick(ClickConnectionString);
            ClickFontStyleEvent = new EventHandler(ClickFontStyle);
            ComboPickerClick = new EventHandler(ClickCombo);
        }
        private void ClickCombo(object sender, EventArgs args)
        {
            //if (DataGridView == null)
            //    return;
            //DataGridView.EndEdit();
            //((DataTable)DataGridView.DataSource).AcceptChanges();
        }
        public FrameMainDesigner GetMainDesigner()
        {
            FrameMainDesigner ndia = ((ObjectInspectorColumn)OwningColumn).FrameMain;
            return ndia;
        }
        public override void DetachEditingControl()
        {
            if (DataGridView.EditingControl is DataGridViewComboBoxEditingControl)
            {
                ((DataGridViewComboBoxEditingControl)DataGridView.EditingControl).Click -= ComboPickerClick;
            }
            base.DetachEditingControl();
        }
        public override void InitializeEditingControl(int rowIndex, object
            initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                                          dataGridViewCellStyle);
            Reportman.Reporting.Variant nvalue = (Variant)GetColumnValue("VALUE", rowIndex);
            ObjectInspectorCellType ntype = (ObjectInspectorCellType)GetColumnValue("TYPEENUM", rowIndex);
            if (ntype == ObjectInspectorCellType.Image)
                nvalue = (Variant)GetColumnValue("VALUEBIN", rowIndex);

            if (DataGridView.EditingControl is DataGridViewComboBoxEditingControl)
            {
                ComboBoxPicker = DataGridView.EditingControl as DataGridViewComboBoxEditingControl;
                Strings list = (Strings)GetColumnValue("VALUELIST", rowIndex);
                ComboBoxPicker.Items.Clear();
                foreach (string s in list)
                {
                    ComboBoxPicker.Items.Add(s);
                }
                if (ntype == ObjectInspectorCellType.DropDown)
                {
                    ComboBoxPicker.DropDownStyle = ComboBoxStyle.DropDown;

                    ComboBoxPicker.Text = nvalue.ToString();
                }
                else
                {
                    ComboBoxPicker.DropDownStyle = ComboBoxStyle.DropDownList;

                    if (!(nvalue.IsNull))
                    {
                        int index = -1;
                        if (nvalue.IsString())
                        {
                            index = ComboBoxPicker.Items.IndexOf(nvalue.ToString());
                        }
                        else
                            index = nvalue;

                        if ((index < ComboBoxPicker.Items.Count) && (index >= 0))
                            ComboBoxPicker.SelectedIndex = index;
                        else
                            ComboBoxPicker.SelectedIndex = -1;
                    }
                    else
                        ComboBoxPicker.SelectedIndex = -1;
                }
                ComboBoxPicker.Click += ComboPickerClick;
            }
            else
                if (DataGridView.EditingControl is DataGridViewTextBoxEditingControl)
            {
                TextBoxPicker = DataGridView.EditingControl as DataGridViewTextBoxEditingControl;
                switch (ntype)
                {
                    case ObjectInspectorCellType.FontStyle:
                        TextBoxPicker.ReadOnly = true;
                        TextBoxPicker.BackColor = SystemColors.Info;
                        TextBoxPicker.ForeColor = SystemColors.InfoText;
                        TextBoxPicker.Click += ClickFontStyleEvent;
                        if (nvalue.IsNull)
                            nvalue = 0;
                        TextBoxPicker.Text = GraphicUtils.StringFontStyleFromInteger((int)nvalue);
                        break;
                    default:
                        TextBoxPicker.Text = nvalue.ToString();
                        break;
                }
            }
            else
                    if (DataGridView.EditingControl is PanelColorPicker)
            {
                ColorPickerc = DataGridView.EditingControl as PanelColorPicker;
                if (nvalue.IsNull)
                {
                    ColorPickerc.TextDisplayed = false;
                    ColorPickerc.Color = GraphicUtils.IntegerFromColor(Color.White);
                }
                else
                {
                    ColorPickerc.TextDisplayed = true;
                    int valorInt = nvalue;
                    Color ncolor = GraphicUtils.ColorFromInteger(valorInt);
                    ColorPickerc.Color = valorInt;
                }
            }
            else
                        if (DataGridView.EditingControl is PanelImagePicker)
            {
                ImagePickerc = DataGridView.EditingControl as PanelImagePicker;
                ImagePickerc.xrow = ((DataRowView)DataGridView.Rows[rowIndex].DataBoundItem).Row;
                if (nvalue.IsNull)
                {
                    ImagePickerc.MemStream = new System.IO.MemoryStream();
                }
                else
                {
                    ImagePickerc.MemStream = nvalue.GetStream();
                }
            }
            else
                        if (DataGridView.EditingControl is CheckBoxPickerControl)
            {
                CheckBoxPicker = DataGridView.EditingControl as CheckBoxPickerControl;
                if (nvalue.IsNull)
                    CheckBoxPicker.CheckState = CheckState.Indeterminate;
                else
                    CheckBoxPicker.Checked = (bool)nvalue;
            }
            else
                            if (DataGridView.EditingControl is EllipsisEditingControl)
            {
                EllipsisPicker = DataGridView.EditingControl as EllipsisEditingControl;
                if (!EllipsisPicker.AssignedEvent)
                {
                    EllipsisPicker.AssignedEvent = true;
                    EllipsisPicker.ButtonClick += EllipsisPicker_ButtonClick;
                }

                EllipsisPicker.Text = nvalue.ToString();
                EllipsisPicker.Data = ntype;

            }
            else
                                if (DataGridView.EditingControl is NumericUpDownPickerControl)
            {
                NumericPicker = DataGridView.EditingControl as NumericUpDownPickerControl;
                switch (ntype)
                {
                    case ObjectInspectorCellType.Decimal:
                        NumericPicker.DataType = TextBoxDataType.Numeric;
                        break;
                    case ObjectInspectorCellType.Integer:
                        NumericPicker.DataType = TextBoxDataType.Integer;
                        break;

                }
                if (nvalue.IsNull)
                {
                    NumericPicker.Text = "";
                }
                else
                {
                    switch (ntype)
                    {
                        case ObjectInspectorCellType.Decimal:
                            NumericPicker.Text = ((decimal)nvalue).ToString("##0.0000");
                            break;
                        case ObjectInspectorCellType.Integer:
                            NumericPicker.Text = nvalue.ToString();
                            break;

                    }
                }

            }
        }

        private bool EllipsisPicker_ButtonClick(EllipsisEditingControl sender, ref string text)
        {
            switch ((ObjectInspectorCellType)sender.Data)
            {
                case ObjectInspectorCellType.FontName:
                    return ClickFontName(sender, ref text);
                case ObjectInspectorCellType.Expression:
                    return ClickExpression(sender, ref text);
                case ObjectInspectorCellType.SQL:
                    return ClickSQL(sender, ref text);
                case ObjectInspectorCellType.ConnectionString:
                    return ClickConnectionString(sender, ref text);
                default:
                    throw new Exception("Ellipsisclick not implemented for " + sender.Tag.ToString());
            }
        }

        private Font CreateOriginalFont()
        {
            string fontname = "";
            int fstyle = 0;
            int fontsize = 10;
            for (int i = 0; i < DataGridView.Rows.Count; i++)
            {
                string pname = GetColumnValue("NAME", i).ToString();
                if (pname == Translator.TranslateStr(560))
                {
                    fontname = GetColumnValue("VALUE", i).ToString();
                    if (fontname.Length == 0)
                        fontname = "Arial";
                }
                else
                    if (pname == Translator.TranslateStr(563))
                {
                    Variant nvar = (Variant)GetColumnValue("VALUE", i);
                    if (nvar.IsNull)
                        nvar = 10;
                    fontsize = nvar;
                }
                else
                        if (pname == Translator.TranslateStr(566))
                {
                    Variant nvar = (Variant)GetColumnValue("VALUE", i);
                    if (nvar.IsNull)
                        nvar = 0;
                    fstyle = nvar;
                }
            }
            FontStyle nfstyle = GraphicUtils.FontStyleFromInteger(fstyle);
            Font nfont = new Font(fontname, fontsize, nfstyle);
            return nfont;
        }
        private void SetNewFont(string fontname, int fontsize, int fstyle)
        {
            for (int i = 0; i < DataGridView.Rows.Count; i++)
            {
                string pname = GetColumnValue("NAME", i).ToString();
                if (pname == Translator.TranslateStr(560))
                {
                    SetValue(i, fontname);
                }
                else
                    if (pname == Translator.TranslateStr(563))
                {
                    SetValue(i, fontsize);
                }
                else
                        if (pname == Translator.TranslateStr(566))
                {
                    SetValue(i, fstyle);
                }
            }
        }
        private bool ClickExpression(EllipsisEditingControl sender, ref string expression)
        {
            if (ExpressionDlg.ShowDialog(ref expression, GetMainDesigner()))
            {
                return true;
            }
            else
                return false;
        }
        DataGridView CurrentDataGridView
        {
            get
            {
                if (DataGridView != null)
                    return DataGridView;
                else
                    return IntDataGridView;
            }
        }
        DataGridView IntDataGridView;
        private bool ClickSQL(EllipsisEditingControl sender, ref string sql)
        {
            IntDataGridView = sender.m_dataGridView;
            DesignerInterfaceDataInfo dinfo = (DesignerInterfaceDataInfo)GetColumnValue("INTERFACE");
            string datainfoalias = dinfo.GetProperty(Translator.TranslateStr(518));
            if (SQLEditor.ShowDialog(ref sql, GetMainDesigner(), datainfoalias))
            {
                return true;
            }
            else
                return false;
        }
        private bool ClickConnectionString(EllipsisEditingControl sender, ref string connection_string)
        {
            IntDataGridView = sender.m_dataGridView;
            DesignerInterfaceDbInfo dinfo = (DesignerInterfaceDbInfo)GetColumnValue("INTERFACE");
            string datainfoalias = dinfo.GetProperty(Translator.TranslateStr(400));
            if (ConnectionEditor.ShowDialog(ref connection_string, GetMainDesigner(), datainfoalias))
            {
                return true;
            }
            else
                return false;
        }

        private bool ClickFontName(EllipsisEditingControl sender, ref string fontname)
        {
            bool aresult = false;
            using (FontDialog ndialog = new FontDialog())
            {
                // Search for other properties
                Font nfont = CreateOriginalFont();
                ndialog.Font = new Font(fontname, nfont.Size, nfont.Style);
                if (ndialog.ShowDialog() == DialogResult.OK)
                {
                    fontname = ndialog.Font.Name;
                    SetNewFont(fontname, (int)Math.Round(ndialog.Font.Size), GraphicUtils.IntegerFromFontStyle(ndialog.Font.Style));
                    aresult = true;
                }
            }
            return aresult;
        }
        private void ClickFontStyle(object sender, EventArgs e)
        {
            using (FontDialog ndialog = new FontDialog())
            {
                // Search for other properties
                Font nfont = CreateOriginalFont();
                ndialog.Font = new Font(nfont.FontFamily, nfont.Size, nfont.Style);
                if (ndialog.ShowDialog() == DialogResult.OK)
                {
                    SetNewFont(ndialog.Font.FontFamily.ToString(), (int)Math.Round(ndialog.Font.Size), GraphicUtils.IntegerFromFontStyle(ndialog.Font.Style));
                }
            }
        }
        private object GetColumnValue(string columnname)
        {
            return GetColumnValue(columnname, CurrentDataGridView.CurrentRow.Index);
        }
        private object GetColumnValue(string columnname, int rowindex)
        {
            object aresult = new Variant();
            DataGridView ngridview = CurrentDataGridView;
            if (ngridview != null)
            {
                DataGridViewRow nrview = ngridview.Rows[rowindex];
                if (nrview != null)
                    if (nrview.DataBoundItem != null)
                    {
                        DataRow nrow = ((DataRowView)nrview.DataBoundItem).Row;

                        if (columnname == "VALUE")
                        {

                            aresult = Variant.VariantFromObject(nrow[columnname]);
                        }
                        else
                            aresult = nrow[columnname];
                    }
            }
            return aresult;
        }

        private void SetValue(Variant nvalue)
        {
            SetValue(DataGridView.CurrentRow.Index, nvalue);
        }
        private void SetValue(int rowindex, Variant nvalue)
        {
            if (DataGridView != null)
            {
                DataGridViewRow nrview = DataGridView.Rows[rowindex];
                if (nrview != null)
                    if (nrview.DataBoundItem != null)
                    {
                        DataRow nrow = ((DataRowView)nrview.DataBoundItem).Row;
                        nrow.BeginEdit();
                        try
                        {
                            nrow["VALUE"] = nvalue;
                        }
                        finally
                        {
                            nrow.EndEdit();
                        }
                    }
            }
        }

        public override Type EditType
        {
            get
            {
                Type ntype = typeof(DataGridViewTextBoxEditingControl);
                //Return the type of the editing contol that ComboBox uses.
                if (DataGridView != null)
                {
                    ObjectInspectorCellType celltype = (ObjectInspectorCellType)GetColumnValue("TYPEENUM");
                    switch (celltype)
                    {
                        case ObjectInspectorCellType.Decimal:
                        case ObjectInspectorCellType.Integer:
                            ntype = typeof(NumericUpDownPickerControl);
                            break;
                        case ObjectInspectorCellType.DropDownList:
                        case ObjectInspectorCellType.DropDown:
                            ntype = typeof(DataGridViewComboBoxEditingControl);
                            break;
                        case ObjectInspectorCellType.Text:
                        case ObjectInspectorCellType.FontStyle:
                            ntype = typeof(DataGridViewTextBoxEditingControl);
                            break;
                        case ObjectInspectorCellType.Color:
                            ntype = typeof(PanelColorPicker);
                            break;
                        case ObjectInspectorCellType.Image:
                            ntype = typeof(PanelImagePicker);
                            break;
                        case ObjectInspectorCellType.Boolean:
                            ntype = typeof(CheckBoxPickerControl);
                            break;
                        case ObjectInspectorCellType.FontName:
                            ntype = typeof(EllipsisEditingControl);
                            break;
                        case ObjectInspectorCellType.Expression:
                            ntype = typeof(EllipsisEditingControl);
                            break;
                        case ObjectInspectorCellType.SQL:
                            ntype = typeof(EllipsisEditingControl);
                            break;
                        case ObjectInspectorCellType.ConnectionString:
                            ntype = typeof(EllipsisEditingControl);
                            break;
                    }
                }
                return ntype;
            }
        }

        public override Type ValueType
        {
            get
            {
                // Return the type of the value that ComboBox contains.
                return typeof(int);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                // Use the current date and time as the default value.
                return 0;
            }
        }

        protected override void Paint(Graphics graphics,
                                        Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                                        DataGridViewElementStates elementState, object value,
                                        object formattedValue, string errorText,
                                        DataGridViewCellStyle cellStyle,
                                        DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                        DataGridViewPaintParts paintParts)
        {
            //            formattedValue = null;
            bool painted = false;
            string property_name = GetColumnValue("NAME", rowIndex).ToString();
            Variant nvalue = (Variant)GetColumnValue("VALUE", rowIndex);
            if (!nvalue.IsNull)
            {
                ObjectInspectorCellType celltype = (ObjectInspectorCellType)GetColumnValue("TYPEENUM", rowIndex);
                if (celltype == ObjectInspectorCellType.Image)
                    nvalue = (Variant)GetColumnValue("VALUEBIN", rowIndex);
                switch (celltype)
                {
                    case ObjectInspectorCellType.DropDownList:
                    case ObjectInspectorCellType.DropDown:
                        Strings list = (Strings)GetColumnValue("VALUELIST", rowIndex);
                        int index = -1;
                        if (nvalue.IsString())
                        {
                            index = list.IndexOf(nvalue);
                        }
                        else
                            index = nvalue;
                        if (index < list.Count)
                        {
                            if (index >= 0)
                                formattedValue = list[index];
                            else
                                formattedValue = "";
                        }
                        break;
                    case ObjectInspectorCellType.Text:
                        break;
                    case ObjectInspectorCellType.Decimal:
                        Decimal dvalue = nvalue;
                        formattedValue = dvalue.ToString("##,##0.000");
                        break;
                    case ObjectInspectorCellType.Integer:
                        Decimal ivalue = nvalue;
                        formattedValue = ivalue.ToString("##,##0");
                        break;
                    case ObjectInspectorCellType.Color:
                        Color c = GraphicUtils.ColorFromInteger(nvalue);
                        formattedValue = c.ToArgb().ToString("x8");
                        cellStyle.BackColor = c;
                        cellStyle.ForeColor = GraphicUtils.GetInvertedBlackWhite(c);
                        break;
                    case ObjectInspectorCellType.Image:
                        System.IO.MemoryStream memstream = nvalue.GetStream();
                        formattedValue = StringUtil.GetSizeAsString(memstream.Length);
                        break;
                    case ObjectInspectorCellType.FontStyle:
                        formattedValue = GraphicUtils.StringFontStyleFromInteger(nvalue);
                        cellStyle.BackColor = SystemColors.Info;
                        cellStyle.ForeColor = SystemColors.InfoText;
                        break;
                    case ObjectInspectorCellType.Boolean:
                        bool? boolvalue = null;
                        if (!nvalue.IsNull)
                            boolvalue = (bool)nvalue;
                        ButtonState nstate;
                        if (boolvalue == null)
                            nstate = ButtonState.Inactive;
                        else
                        if ((bool)boolvalue)
                            nstate = ButtonState.Checked;
                        else
                            nstate = ButtonState.Normal;
                        base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, "", "",
                                   "", cellStyle, advancedBorderStyle, paintParts);
                        // Make square cell Bounds
                        //                        int checkwidth=(int)Math.Round((double)cellStyle.Font.Size*(double)96/(double)72);
                        int checkwidth = 13;
                        int x1 = cellBounds.Left;
                        int y1 = cellBounds.Top;
                        int width1 = cellBounds.Width;
                        int height1 = cellBounds.Height;
                        if (height1 > width1)
                        {
                            y1 = height1 - (checkwidth / 2);
                            height1 = width1;
                        }
                        else
                        {
                            x1 = x1 + width1 / 2 - (checkwidth / 2);
                            y1 = y1 + (height1 - checkwidth) / 2;
                            width1 = height1;
                        }
                        Rectangle nbounds = new Rectangle(x1, y1, checkwidth, checkwidth);
                        ControlPaint.DrawCheckBox(graphics, nbounds, nstate);
                        painted = true;
                        break;
                }
            }
            if (!painted)
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue,
                           errorText, cellStyle, advancedBorderStyle, paintParts);


            /*           Rectangle ColorBoxRect = new Rectangle();
                       RectangleF TextBoxRect = new RectangleF();
                       GetDisplayLayout(cellBounds, ref ColorBoxRect, ref TextBoxRect);

                       //// Draw the cell background, if specified.
                       if ((paintParts & DataGridViewPaintParts.Background) ==
                           DataGridViewPaintParts.Background)
                       {
                           SolidBrush cellBackground;
                           if (value != null && value.GetType() == typeof(Color))
                           {
                               cellBackground = new SolidBrush((Color)value);
                           }
                           else
                           {
                               cellBackground = new SolidBrush(cellStyle.BackColor);
                           }
                           graphics.FillRectangle(cellBackground, ColorBoxRect);
                           graphics.DrawRectangle(Pens.Black, ColorBoxRect);
                           Color lclcolor = (Color)value;
                           graphics.DrawString(lclcolor.Name.ToString(), cellStyle.Font, System.Drawing.Brushes.Black, TextBoxRect);

                           cellBackground.Dispose();
                       }*/

        }

        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter formattedValueTypeConverter, System.ComponentModel.TypeConverter valueTypeConverter)
        {
            object aresult = null;
            ObjectInspectorCellType celltype = (ObjectInspectorCellType)GetColumnValue("TYPEENUM");

            switch (celltype)
            {
                case ObjectInspectorCellType.DropDownList:
                case ObjectInspectorCellType.DropDown:
                    if (formattedValue.ToString().Length == 0)
                        aresult = DBNull.Value;
                    else
                    {
                        Strings list = (Strings)GetColumnValue("VALUELIST");
                        int index = list.IndexOf(formattedValue.ToString());
                        if (index < 0)
                            index = 0;
                        aresult = index;
                    }
                    break;
                case ObjectInspectorCellType.Text:
                case ObjectInspectorCellType.Expression:
                case ObjectInspectorCellType.SQL:
                case ObjectInspectorCellType.ConnectionString:
                    aresult = formattedValue.ToString();
                    break;
                case ObjectInspectorCellType.Color:
                    if (formattedValue.ToString().Length == 0)
                        aresult = DBNull.Value;
                    else
                    {
                        int colorvalue = 0;
                        if (int.TryParse(formattedValue.ToString(), System.Globalization.NumberStyles.HexNumber, null, out colorvalue))
                        {
                            aresult = colorvalue;
                        }
                    }
                    break;
                case ObjectInspectorCellType.Image:
                    if (formattedValue is Variant)
                    {
                        Variant imvar = (Variant)formattedValue;
                        if (imvar.VarType == VariantType.Binary)
                        {
                            System.IO.MemoryStream nstream = ((Variant)formattedValue).GetStream();
                            aresult = StringUtil.GetSizeAsString(nstream.Length);
                        }
                        else
                            aresult = formattedValue;
                    }
                    else
                        aresult = "";

                    break;
                case ObjectInspectorCellType.FontName:
                    aresult = formattedValue.ToString();
                    break;
                case ObjectInspectorCellType.FontStyle:
                    if (formattedValue.ToString().Length == 0)
                        aresult = DBNull.Value;
                    else
                        aresult = GraphicUtils.IntegerFromStringFontStyle(formattedValue.ToString());
                    break;
                case ObjectInspectorCellType.Decimal:
                    if (formattedValue.ToString().Length == 0)
                        aresult = DBNull.Value;
                    else
                        aresult = System.Convert.ToDecimal(formattedValue);
                    break;
                case ObjectInspectorCellType.Boolean:
                    if (formattedValue.ToString().Length == 0)
                        aresult = DBNull.Value;
                    else
                        aresult = System.Convert.ToBoolean(formattedValue);
                    break;
                case ObjectInspectorCellType.Integer:
                    if (formattedValue.ToString().Length == 0)
                        aresult = DBNull.Value;
                    else
                        aresult = (int)Math.Round(System.Convert.ToDecimal(formattedValue));
                    break;
            }
            if (aresult == null)
            {
                aresult = base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
            }
            return aresult;
        }

        protected virtual void GetDisplayLayout(Rectangle CellRect, ref Rectangle colorBoxRect, ref RectangleF textBoxRect)
        {
            const int DistanceFromEdge = 2;

            colorBoxRect.X = CellRect.X + DistanceFromEdge;
            colorBoxRect.Y = CellRect.Y + 1;
            colorBoxRect.Size = new Size((int)(1.5 * 17), CellRect.Height - (2 * DistanceFromEdge));

            // The text occupies the middle portion.
            textBoxRect = RectangleF.FromLTRB(colorBoxRect.X + colorBoxRect.Width + 5, colorBoxRect.Y + 2, CellRect.X + CellRect.Width - DistanceFromEdge, colorBoxRect.Y + colorBoxRect.Height);
        }
    }


    class CheckBoxPickerControl : CheckBox, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;

        public CheckBoxPickerControl()
        {
            CheckAlign = ContentAlignment.MiddleCenter;

            CheckedChanged += new EventHandler(nvaluechange);
        }
        private void nvaluechange(object sender, EventArgs ev)
        {
            NotifyDataGridViewOfValueChange();
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
        // property.
        public object EditingControlFormattedValue
        {
            get
            {
                return this;
            }
            set
            {
                this.Checked = false;
            }
        }

        // Implements the 
        // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.Checked;
        }

        // Implements the 
        // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
        // property.
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
        // method.
        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the ColorPicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                //                case Keys.Left:
                //                case Keys.Up:
                //                case Keys.Down:
                //                case Keys.Right:
                //                case Keys.Home:
                //                case Keys.End:
                //                case Keys.PageDown:
                //                case Keys.PageUp:
                //                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }
        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
        // method.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        // Implements the IDataGridViewEditingControl
        // .RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlDataGridView property.
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlValueChanged property.
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        protected virtual void NotifyDataGridViewOfValueChange()
        {
            this.valueChanged = true;
            if (this.dataGridView != null)
            {
                this.dataGridView.NotifyCurrentCellDirty(true);
            }
        }

        protected override void OnLeave(EventArgs eventargs)
        {
            // Notify the DataGridView that the contents of the cell
            // have changed.
            base.OnLeave(eventargs);
            NotifyDataGridViewOfValueChange();
        }

    }
    class NumericUpDownPickerControl : TextBoxAdvanced, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;

        public NumericUpDownPickerControl()
        {
            TextAlign = HorizontalAlignment.Right;
            TextChanged += new EventHandler(nvaluechange);
        }
        private void nvaluechange(object sender, EventArgs ev)
        {
            NotifyDataGridViewOfValueChange();
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
        // property.
        public object EditingControlFormattedValue
        {
            get
            {
                return Text;
            }
            set
            {
                this.Text = "";
            }
        }

        // Implements the 
        // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.Text;
        }

        // Implements the 
        // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
        // property.
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
        // method.
        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the ColorPicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                //                case Keys.Up:
                //                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                    //                case Keys.PageDown:
                    //                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }
        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
        // method.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        // Implements the IDataGridViewEditingControl
        // .RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlDataGridView property.
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlValueChanged property.
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        protected virtual void NotifyDataGridViewOfValueChange()
        {
            this.valueChanged = true;
            if (this.dataGridView != null)
            {
                this.dataGridView.NotifyCurrentCellDirty(true);
            }
        }

        protected override void OnLeave(EventArgs eventargs)
        {
            // Notify the DataGridView that the contents of the cell
            // have changed.
            base.OnLeave(eventargs);
            NotifyDataGridViewOfValueChange();
        }

    }
    internal class PanelColorPicker : Panel, IDataGridViewEditingControl
    {
        int rowIndex;
        bool FTextDisplayed;
        bool valueChanged;
        DataGridView dataGridView;
        public bool TextDisplayed
        {
            get
            {
                return FTextDisplayed;
            }
            set
            {
                FTextDisplayed = value;
                UpdateText();
                Invalidate();
            }
        }
        protected override void OnClick(EventArgs e)
        {
            using (ColorDialog ndia = new ColorDialog())
            {
                ndia.Color = GraphicUtils.ColorFromInteger(FBackColor);
                if (ndia.ShowDialog() == DialogResult.OK)
                {
                    FBackColor = GraphicUtils.IntegerFromColor(ndia.Color);
                    BackColor = ndia.Color;
                    NotifyDataGridViewOfValueChange();
                }
            }
            base.OnClick(e);
        }
        private int FBackColor;
        public int Color
        {
            get
            {
                return FBackColor;
            }
            set
            {
                FBackColor = value;
                BackColor = GraphicUtils.ColorFromInteger(FBackColor);
                UpdateText();
                Invalidate();
            }
        }
        private void UpdateText()
        {

        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
        // property.
        public object EditingControlFormattedValue
        {
            get
            {
                return FBackColor;
            }
            set
            {
                if (value is Color)
                {
                    FBackColor = GraphicUtils.IntegerFromColor((Color)value);
                    BackColor = (Color)value;
                }
                else
                {
                    FBackColor = Convert.ToInt32(value);
                    BackColor = GraphicUtils.ColorFromInteger(FBackColor);
                }
            }
        }

        // Implements the 
        // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            //string resultado = GraphicUtils.ColorFromInteger(FBackColor).ToArgb().ToString("x8");
            string resultado = BackColor.B.ToString("x2") + BackColor.G.ToString("x2") + BackColor.R.ToString("x2");
            return resultado;
        }

        // Implements the 
        // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
        // property.
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
        // method.
        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the ColorPicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                //                case Keys.Left:
                //                case Keys.Up:
                //                case Keys.Down:
                //                case Keys.Right:
                //                case Keys.Home:
                //                case Keys.End:
                //                case Keys.PageDown:
                //                case Keys.PageUp:
                //                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }
        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
        // method.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        // Implements the IDataGridViewEditingControl
        // .RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlDataGridView property.
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlValueChanged property.
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        protected virtual void NotifyDataGridViewOfValueChange()
        {
            this.valueChanged = true;
            if (this.dataGridView != null)
            {
                this.dataGridView.NotifyCurrentCellDirty(true);
            }
        }


    }


    internal class PanelImagePicker : Panel, IDataGridViewEditingControl
    {
        int rowIndex;
        bool FTextDisplayed;
        bool valueChanged;
        DataGridView dataGridView;
        System.IO.MemoryStream FMemStream;
        public DataRow xrow;
        public bool TextDisplayed
        {
            get
            {
                return FTextDisplayed;
            }
            set
            {
                FTextDisplayed = value;
                UpdateText();
                Invalidate();
            }
        }
        public static string GetImageFilters()
        {
            string nfilter = "All image files|*.png;*.jpeg;*.jpg;*.bmp;*.gif";
            return nfilter;

        }
        protected override void OnClick(EventArgs e)
        {
            using (OpenFileDialog ndia = new OpenFileDialog())
            {
                ndia.Filter = GetImageFilters();
                if (ndia.ShowDialog() == DialogResult.OK)
                {
                    FMemStream = StreamUtil.FileToMemoryStream(ndia.FileName);
                    xrow["VALUEBIN"] = Variant.VariantFromObject(FMemStream);
                    //xrow["VALUE"] = Variant.VariantFromObject(StringUtil.GetSizeAsString(FMemStream.Length));

                    NotifyDataGridViewOfValueChange();
                }
            }
            base.OnClick(e);
        }
        public System.IO.MemoryStream MemStream
        {
            get
            {
                return FMemStream;
            }
            set
            {
                FMemStream = value;
                UpdateText();
                Invalidate();
            }
        }
        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
        // method.
        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the ColorPicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                //                case Keys.Up:
                //                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                    //                case Keys.PageDown:
                    //                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }
        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
        // method.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        private void UpdateText()
        {

        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
        // property.
        public object EditingControlFormattedValue
        {
            get
            {
                return StringUtil.GetSizeAsString(FMemStream.Length);
            }
            set
            {
                MemStream = (System.IO.MemoryStream)value;
            }
        }

        // Implements the 
        // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            long nlen = 0;
            if (FMemStream != null)
                nlen = FMemStream.Length;
            return StringUtil.GetSizeAsString(nlen);
        }

        // Implements the 
        // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
        // property.
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }


        // Implements the IDataGridViewEditingControl
        // .RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlDataGridView property.
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlValueChanged property.
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        protected virtual void NotifyDataGridViewOfValueChange()
        {
            this.valueChanged = true;
            if (this.dataGridView != null)
            {
                this.dataGridView.NotifyCurrentCellDirty(true);
            }
        }


    }


}
