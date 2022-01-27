using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.ComponentModel;

namespace Reportman.Drawing.Forms
{
    public delegate void BeforeEnterTabEvent(ref bool cancel);
    public enum ColumnDataType { Text, Integer, Numeric, Double, Date, DateTime, Time, ComboBox,ComboBoxList,Boolean,Password};
    public delegate bool DataColumnButtonClickEvent(DataGridViewColumn ncolumn,ref object value);
    public class DataGridViewColumnAdvanced:DataGridViewColumn
    {
        private ColumnDataType FDataType;
        private int FMaxInputLength;
        private Image FImageButton;
        public DataColumnButtonClickEvent ButtonClick;
        public float ImageButtonScale = 1.0f;
        public ColumnDataType DataType
        {
            get { 
                return FDataType; 
            }
            set
            {
                FDataType = value;
            }
        }
        bool FReadOnlyInput;
        public bool ReadOnlyInput
        {
            get { return FReadOnlyInput; }
            set
            {
                FReadOnlyInput = value;
                
            }
        }
        public ISearchWindow SearchWindow;
        public int MaxInputLength
        {
            get
            {
                return FMaxInputLength;
            }
            set
            {
                FMaxInputLength = value;
                if (FMaxInputLength < 0)
                    FMaxInputLength = 0;
            }

        }
        public static int ImageWidth
        {
            get
            {
                return Convert.ToInt32(20 * Reportman.Drawing.GraphicUtils.DPIScaleY);
            }
        }
        public Image ImageButton
        {
            get { return FImageButton; }
            set
            {
                FImageButton = value;
            }
        }
        public DataGridViewColumnAdvanced()
            : base(new DataGridViewCellAdvanced())
        {
            FDataType = ColumnDataType.Text;
            
        }

        public override int GetPreferredWidth(DataGridViewAutoSizeColumnMode autoSizeColumnMode, bool fixedHeight)
        {
            int nwidth = base.GetPreferredWidth(autoSizeColumnMode, fixedHeight);;
            return nwidth;
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

    public class DataGridViewCellAdvanced : DataGridViewTextBoxCell
    {
        //DataGridViewComboBoxEditingControl ComboBoxPicker;
//        CheckBoxPickerControl CheckBoxPicker;
        //DataGridViewTextBoxEditingControl TextBoxPicker;
        //EllipsisEditingControl EllipsisPicker;
        //ColorPickerControl ColorPickerc;
        //NumericUpDownPickerControl NumericPicker;
        //private EventHandler ButtonClicEvent;
        //private EventHandler ClickFontStyleEvent;
        public DataGridViewCellAdvanced()
            : base()
        {
            //ButtonClicEvent = new EventHandler(ButtonClick);
        }
        private void ButtonClick(object sender, EventArgs args)
        {
/*            DataGridViewColumn col = GetColumn();
            if (!(col is DataGridViewColumnAdvanced))
                return;
            DataGridViewColumnAdvanced ncolumn = (DataGridViewColumnAdvanced)GetColumn();
            if (ncolumn.ButtonClick != null)
                ncolumn.ButtonClick(ncolumn, new DataGridViewColumnEventArgs(ncolumn),);*/
        }
        private DataGridViewColumn GetColumn()
        {
            DataGridViewColumn ncol = null;
            if (DataGridView != null)
            {
                ncol= DataGridView.Columns[ColumnIndex];
            }
            return ncol;
        }
        public override Type EditType
        {
            get
            {
                Type ntype = typeof(AdvancedEditingControl);
                //Return the type of the editing contol that ComboBox uses.
/*                if (DataGridView != null)
                {
                    ObjectInspectorCellType celltype = (ObjectInspectorCellType)GetColumnValue("TYPEENUM");
                    switch (celltype)
                    {
                        case ObjectInspectorCellType.Decimal:
                        case ObjectInspectorCellType.Integer:
                            ntype = typeof(NumericUpDownPickerControl);
                            break;
                        case ObjectInspectorCellType.DropDownList:
                            ntype = typeof(DataGridViewComboBoxEditingControl);
                            break;
                        case ObjectInspectorCellType.Text:
                        case ObjectInspectorCellType.FontStyle:
                            ntype = typeof(DataGridViewTextBoxEditingControl);
                            break;
                        case ObjectInspectorCellType.Color:
                            ntype = typeof(ColorPickerControl);
                            break;
                        case ObjectInspectorCellType.Boolean:
                            ntype = typeof(CheckBoxPickerControl);
                            break;
                        case ObjectInspectorCellType.FontName:
                            ntype = typeof(EllipsisEditingControl);
                            break;
                    }
                }*/
                return ntype;
            }
        }
        protected override void  Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
 	        base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }
        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter valueTypeConverter, System.ComponentModel.TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            DataGridViewColumnAdvanced ncol = (DataGridViewColumnAdvanced)GetColumn();
            if (ncol.DataType == ColumnDataType.Password)
            {
                if (value == null)
                    return "";
                else
                    if (value == DBNull.Value)
                        return "";
                    else
                        if (value.ToString().Length == 0)
                            return "";
                        else
                            return "" + (char)0x25CF + (char)0x25CF + (char)0x25CF + (char)0x25CF + (char)0x25CF + (char)0x25CF;
            }
            else
                return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
        }

    }
    public partial class AdvancedEditingControl : UserControl, IDataGridViewEditingControl
    {
        private TextBoxAdvanced textcontrol;
        private DateTimePickerNullable datecontrol1;
        private DateTimePickerAdvanced datecontrol2;
        //private DateTimePickerNullable datecontrol2;
        private PictureBox picbo;
        private ColumnDataType controldatatype;
        private ColumnDataType FDataType;
        private bool disabledchange;
        private int FMaxInputLength;
        private bool FReadOnlyInput;
        public bool ReadOnlyInput
        {
            get
            {
                return FReadOnlyInput;
            }
            set
            {
                ReadOnlyInput = value;
                UpdateReaonlyInput();
            }
        }
        ISearchWindow SearchWindow;
        public static bool NewDatePicker = false;

        public void DoKeyDown(object sender, KeyEventArgs args)
        {
            OnKeyDown(args);
        }
        public void DoKeyPress(object sender, KeyPressEventArgs args)
        {
            OnKeyPress(args);
        }
        public void DoKeyUp(object sender, KeyEventArgs args)
        {
            OnKeyUp(args);
        }
        public float ImageButtonScale = 1.0f;

        public Image ImageButton
        {
            get { return picbo.Image; }
            set 
            { 
                picbo.Image = value;
                picbo.Visible = picbo.Image!= null;
            picbo.SizeMode = PictureBoxSizeMode.Zoom;
            picbo.Width = DataGridViewColumnAdvanced.ImageWidth;
                CreateMainControl();
                ResizeControls();
            }

        }
        DataGridView m_dataGridView = null;
        int m_rowIndex = 0;
        bool m_valueChanged = false;
        //string m_prevText = null;
        public Control MainControl;
        object prevvalue;
        public new string Text
        {
            get
            {
                return textcontrol.Text;
            }
            set
            {
                textcontrol.Text = value;
            }
        }
        public AdvancedEditingControl()
        {
            picbo = new PictureBox();
            picbo.Click += new EventHandler(PicboxButton_Click);
            picbo.SizeMode = PictureBoxSizeMode.Zoom;
            picbo.Width = DataGridViewColumnAdvanced.ImageWidth;
            picbo.Dock = DockStyle.Right;
            picbo.Visible = false;
            Controls.Add(picbo);
        }
        private void MValueChange(object sender, EventArgs ev)
        {
            NotifyChange();
        }
        public void SaveCurrentValue()
        {
            NotifyChange();
        }
        void MLostFocus(object sender, EventArgs e)
        {
            NotifyChange();
        }
        void MDoubleClick(object sender, EventArgs e)
        {
            if (m_dataGridView is DataGridViewAdvanced)
            {
                DataGridViewAdvanced ngrid = ((DataGridViewAdvanced)m_dataGridView);
                // aceptamos el valor
                ngrid.FinishEdit();
                ngrid.DoDoubleClick(MainControl);
            }
        }

        private void UpdateReaonlyInput()
        {
            switch (FDataType)
            {
                case ColumnDataType.Double:
                case ColumnDataType.Integer:
                case ColumnDataType.Numeric:
                case ColumnDataType.Text:
                case ColumnDataType.Password:
                    if (textcontrol != null)
                    {
                        textcontrol.ReadOnly = FReadOnlyInput;
                    }
                    break;
            }

        }
        private void SetValue(object newvalue)
        {
            CreateControl();
            bool oldsearchenabled = false; 
            switch (FDataType)
            {
                case ColumnDataType.Double:
                case ColumnDataType.Integer:
                case ColumnDataType.Numeric:
                    if (textcontrol.SearchWindow != null)
                    {
                        oldsearchenabled = textcontrol.SearchWindow.Enabled;
                        textcontrol.SearchWindow.Enabled = false;
                    }
                    try
                    {
                        textcontrol.Text = newvalue.ToString();
                    }
                    finally
                    {
                        if (textcontrol.SearchWindow != null)
                        {
                            textcontrol.SearchWindow.Enabled = oldsearchenabled;
                        }
                    }
                    break;
                case ColumnDataType.Text:
                case ColumnDataType.Password:
                    if (textcontrol.SearchWindow != null)
                    {
                        oldsearchenabled = textcontrol.SearchWindow.Enabled;
                        textcontrol.SearchWindow.Enabled = false;
                    }
                    try
                    {
                        textcontrol.Text = newvalue.ToString();
                    }
                    finally
                    {
                        if (textcontrol.SearchWindow != null)
                        {
                            textcontrol.SearchWindow.Enabled = oldsearchenabled;
                        }
                    }
                    break;
                case ColumnDataType.Date:
                    if (NewDatePicker)
                    {
                        if (newvalue != DBNull.Value)
                            datecontrol2.Value = System.Convert.ToDateTime(newvalue).Date;
                        else
                            datecontrol2.Value = DateTime.MinValue;
                    }
                    else
                    {
                        if (newvalue != DBNull.Value)
                            datecontrol1.Value = System.Convert.ToDateTime(newvalue).Date;
                        else
                            datecontrol1.Value = DateTime.MinValue;
                    }
                    break;
                case ColumnDataType.Time:
                    if (NewDatePicker)
                    {
                        if (newvalue != DBNull.Value)
                            datecontrol2.Value = System.Convert.ToDateTime(newvalue);
                        else
                            datecontrol2.Value = DateTime.MinValue;
                    }
                    else
                    {
                        if (newvalue != DBNull.Value)
                            datecontrol1.Value = System.Convert.ToDateTime(newvalue);
                        else
                            datecontrol1.Value = DateTime.MinValue;
                    }
                    break;
                case ColumnDataType.DateTime:
                    if (NewDatePicker)
                    {
                        if (newvalue != DBNull.Value)
                            datecontrol2.Value = System.Convert.ToDateTime(newvalue);
                        else
                            datecontrol2.Value = DateTime.MinValue;
                    }
                    else
                    {
                        if (newvalue != DBNull.Value)
                            datecontrol1.Value = System.Convert.ToDateTime(newvalue);
                        else
                            datecontrol1.Value = DateTime.MinValue;
                    }
                    break;
            }
        }
        public object NewValue
        {
            get { return GetValue(); }
            set
            {
                disabledchange = true;
                try
                {
                    SetValue(value);
                }
                finally
                {
                    disabledchange = false;
                }

                NotifyChange();
            }
        }
        private object GetValue()
        {
            object nresult = null;
            switch (FDataType)
            {
                case ColumnDataType.Text:
                case ColumnDataType.Password:
                    nresult = textcontrol.Text;
                    break;
                case ColumnDataType.Integer:
                case ColumnDataType.Double:
                case ColumnDataType.Numeric:
                    nresult = textcontrol.Text;
                    break;
                case ColumnDataType.Date:
                    if (NewDatePicker)
                    {
                        if (datecontrol2.Value == DateTime.MinValue)
                            nresult = DBNull.Value;
                        else
                            nresult = datecontrol2.Value.Date;
                    }
                    else
                    {
                        if (datecontrol1.Value == DateTime.MinValue)
                            nresult = DBNull.Value;
                        else
                            nresult = datecontrol1.Value.Date;
                    }
                    break;
                case ColumnDataType.Time:
                    if (NewDatePicker)
                    {
                        if (datecontrol2.Value == DateTime.MinValue)
                            nresult = DBNull.Value;
                        else
                            nresult = datecontrol2.Value;
                    }
                    else
                    {
                        if (datecontrol1.Value == DateTime.MinValue)
                            nresult = DBNull.Value;
                        else
                            nresult = datecontrol1.Value;
                    }
                    break;
                case ColumnDataType.DateTime:
                    if (NewDatePicker)
                    {
                        if (datecontrol2.Value == DateTime.MinValue)
                            nresult = DBNull.Value;
                        else
                            nresult = datecontrol2.Value;
                    }
                    else
                    {
                        if (datecontrol1.Value == DateTime.MinValue)
                            nresult = DBNull.Value;
                        else
                            nresult = datecontrol1.Value;
                    }
                    break;
                /*                case ColumnDataType.DateTime:
                                    if (datecontrol1.Value == DateTime.MinValue)
                                        nresult = DBNull.Value;
                                    else
                                        nresult = datecontrol1.Value.Add(datecontrol2.Value - datecontrol2.Value.Date);
                                    break;*/
            }
            return nresult;
        }
        private void SetNewValue(object nval)
        {
            switch (FDataType)
            {
                case ColumnDataType.Text:
                case ColumnDataType.Password:
                    textcontrol.Text = nval.ToString();
                    break;
                case ColumnDataType.Integer:
                case ColumnDataType.Double:
                case ColumnDataType.Numeric:
                    textcontrol.Text = nval.ToString();
                    break;
                case ColumnDataType.Date:
                case ColumnDataType.Time:
                case ColumnDataType.DateTime:
                    if (NewDatePicker)
                    {
                        if (nval.ToString() != "")
                            datecontrol2.Value = (DateTime)nval;
                    }
                    else
                    {
                        if (nval.ToString() != "")
                            datecontrol1.Value = (DateTime)nval;
                    }
                    break;
                /*case ColumnDataType.DateTime:
                    datecontrol1.Value = ((DateTime)nval).Date;
                    datecontrol2.Value = (DateTime)nval;
                    break;*/
            }
        }
        private void PicboxButton_Click(object sender, EventArgs e)
        {
            DataGridViewColumnAdvanced ncol = GetColumn();
            if (ncol == null)
                return;
            if (ncol.ButtonClick != null)
            {
                object nvalue = GetValue();
                bool aresult = ncol.ButtonClick(GetColumn(), ref nvalue);
                if (aresult)
                {
                    //SetValue(nvalue);
                    //NotifyChange();
                }
            }
        }

        private void NotifyChange()
        {
            if (disabledchange)
                return;
            if (!GetValue().Equals(prevvalue))
            {
                m_valueChanged = true;
                m_dataGridView.NotifyCurrentCellDirty(true);
                prevvalue = GetValue();
            }
        }
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Do nothing
        }
        public Cursor EditingControlCursor
        {
            get
            {
                return Cursors.IBeam;
            }
        }
        public Cursor EditingPanelCursor
        {
            get
            {
                return Cursors.IBeam;
            }
        }
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return m_dataGridView;
            }
            set
            {
                m_dataGridView = value;
            }
        }
        public object EditingControlFormattedValue
        {
            get
            {
                return GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Display);
            }
            set
            {
                SetNewValue(value);
            }
        }
        public int EditingControlRowIndex
        {
            get
            {
                return m_rowIndex;
            }
            set
            {
                m_rowIndex = value;
            }
        }

        public bool EditingControlValueChanged
        {
            get
            {
                return m_valueChanged;
            }
            set
            {
                m_valueChanged = value;
            }
        }
        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            if (FDataType == ColumnDataType.Date)
            {
                switch (keyData)
                {
                    case Keys.Down:
                        return false;
                    case Keys.Up:
                        return false;
                    default:
                        return true;
                }

            }

            switch (keyData)
            {
                case Keys.Tab:
                    return true;
                case Keys.Home:
                    return true;
                case Keys.End:
                    return true;
                case Keys.Left:
                    if ((this.textcontrol.SelectionLength == 0)
                        && (this.textcontrol.SelectionStart==0))
                        return false;
                    else
                        return true;
                case Keys.Right:
                    if ((this.textcontrol.SelectionLength == 0)
                        && (this.textcontrol.SelectionStart == this.textcontrol.Text.Length))
                        return false;
                    else
                        return true;
                case Keys.Delete:
//                    this.textcontrol.Text = "";
                    return true;
                case Keys.Enter:
                    NotifyChange();
                    return false;
                case Keys.Up:
                    return false;
                case Keys.Down:
                    return false;
                default:
                    if (FDataType == ColumnDataType.Text)
                        return true;
                    else
                        return false;
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            object nvalue = GetValue();
            if (nvalue == null)
                return null;
            string fvalue = "";
            switch (FDataType)
            {
                case ColumnDataType.Text:
                case ColumnDataType.Password:
                    fvalue = nvalue.ToString();
                    break;
                case ColumnDataType.Double:
                case ColumnDataType.Integer:
                case ColumnDataType.Numeric:
                    fvalue = nvalue.ToString();
                    break;
                case ColumnDataType.Date:
                    if (nvalue == DBNull.Value)
                        fvalue = "";
                    else
                        fvalue = ((DateTime)nvalue).ToString("dd/MM/yyyy");
                    break;
                case ColumnDataType.Time:
                    if (nvalue == DBNull.Value)
                        fvalue = "";
                    else
                        fvalue = ((DateTime)nvalue).ToString("HH:mm:ss");
                    break;
                case ColumnDataType.DateTime:
                    if (nvalue == DBNull.Value)
                        fvalue = "";
                    else
                        fvalue = ((DateTime)nvalue).ToString("dd/MM/yyyy HH:mm:ss");
                    break;
            }
            return fvalue;
        }
        public void CreateMainControl()
        {
            if (controldatatype != FDataType)
            {
                MainControl = null;
            }
            else
            {
                if (MainControl is TextBoxAdvanced)
                {
                    if (((TextBoxAdvanced)MainControl).MaxLength != FMaxInputLength)
                        MainControl = null;
                }
            }
            if (MainControl == null)
            {
                controldatatype = FDataType;
                switch (FDataType)
                {
                    case ColumnDataType.Text:
                    case ColumnDataType.Password:
                    case ColumnDataType.Numeric:
                    case ColumnDataType.Integer:
                    case ColumnDataType.Double:
                        if (textcontrol == null)
                        {
                            textcontrol = new TextBoxAdvanced();
                            textcontrol.BorderStyle = BorderStyle.None;
                            textcontrol.Font = this.EditingControlDataGridView.Font;
                            textcontrol.Multiline = true;
                            textcontrol.MinimumSize = new System.Drawing.Size(0,textcontrol.Height );
                            textcontrol.Multiline = false;
                            //textcontrol.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
                            textcontrol.LostFocus += new EventHandler(MLostFocus);
                            textcontrol.TextChanged += new EventHandler(MValueChange);
                            textcontrol.KeyDown += new KeyEventHandler(DoKeyDown);
                            textcontrol.KeyUp += new KeyEventHandler(DoKeyUp);
                            textcontrol.KeyPress += new KeyPressEventHandler(DoKeyPress);
                            textcontrol.DoubleClick += new EventHandler(MDoubleClick);
                            textcontrol.ReadOnly = FReadOnlyInput;
                            MainControl = textcontrol;
                            textcontrol.DataType = (TextBoxDataType)FDataType;
                            textcontrol.MaxLength = FMaxInputLength;
                            if (FDataType == ColumnDataType.Password)
                                textcontrol.UseSystemPasswordChar = true;
                            else
                                textcontrol.UseSystemPasswordChar = false;
                            Controls.Add(textcontrol);
                        }
                        else
                        {
                            textcontrol.Visible = true;
                            textcontrol.DataType = (TextBoxDataType)FDataType;
                            textcontrol.ReadOnly = FReadOnlyInput;
                            textcontrol.MaxLength = FMaxInputLength;
                            if (FDataType == ColumnDataType.Password)
                                textcontrol.UseSystemPasswordChar = true;
                            else
                                textcontrol.UseSystemPasswordChar = false;
                            MainControl = textcontrol;
                        }
                        DataGridViewColumnAdvanced ncol = GetColumn();

                        textcontrol.SearchWindow = ncol.SearchWindow;
                        if (textcontrol.SearchWindow != null)
                            textcontrol.DataType = TextBoxDataType.Text;

                        ResizeControls();
                        break;
                    case ColumnDataType.Date:
                    case ColumnDataType.Time:
                    case ColumnDataType.DateTime:
                        if (NewDatePicker)
                        {
                            datecontrol2 = new DateTimePickerAdvanced();
                            datecontrol2.ValueChanged += new EventHandler(Datecontrol1_ValueChanged);
                            //datecontrol2 = new DateTimePickerNullable();
                            //datecontrol2.ValueChanged += new EventHandler(datecontrol1_ValueChanged);
                            datecontrol2.DateFormat = "dd/MM/yyyy";
                            if (FDataType == ColumnDataType.DateTime)
                                datecontrol2.TimeFormat = "HH:mm:ss";
                            else
                                if (FDataType == ColumnDataType.Time)
                                {
                                    datecontrol2.DateFormat = "";
                                    datecontrol2.TimeFormat = "HH:mm:ss";
                            }

                            //datecontrol2.Format = DateTimePickerFormat.Custom;
                            //datecontrol2.CustomFormat = "hh:mm:ss";
                            //datecontrol2.KeyDown += new KeyEventHandler(DoKeyDown);
                            //datecontrol2.KeyUp += new KeyEventHandler(DoKeyDown);
                            //datecontrol2.KeyPress += new KeyPressEventHandler(DoKeyPress);
                            datecontrol2.KeyDown += new KeyEventHandler(DoKeyDown);
                            datecontrol2.KeyUp += new KeyEventHandler(DoKeyDown);
                            datecontrol2.KeyPress += new KeyPressEventHandler(DoKeyPress);
                            datecontrol2.LostFocus += new EventHandler(MLostFocus);
                            datecontrol2.ValueChanged += new EventHandler(MValueChange);
                            //datecontrol2.LostFocus += new EventHandler(mLostFocus);
                            //datecontrol2.ValueChanged += new EventHandler(mValueChange);
                            datecontrol2.DoubleClick += new EventHandler(MDoubleClick);
                            //datecontrol2.DoubleClick += new EventHandler(mDoubleClick);
                            Controls.Add(datecontrol2);
                            MainControl = datecontrol2;
                        }
                        else
                        {
                            if (datecontrol1 == null)
                            {
                                datecontrol1 = new DateTimePickerNullable();
                                datecontrol1.ValueChanged += new EventHandler(Datecontrol1_ValueChanged);
                            }
                                //datecontrol2 = new DateTimePickerNullable();
                                //datecontrol2.ValueChanged += new EventHandler(datecontrol1_ValueChanged);
                                datecontrol1.Format = DateTimePickerFormat.Custom;
                                if (FDataType == ColumnDataType.DateTime)
                                    datecontrol1.CustomFormat = "dd/MM/yyyy HH:mm:ss";
                                else
                                    if (FDataType == ColumnDataType.Time)
                                    datecontrol1.CustomFormat = "HH:mm:ss";
                                else
                                        if (FDataType == ColumnDataType.Date)
                                    datecontrol1.CustomFormat = "dd/MM/yyyy";

                                //datecontrol2.Format = DateTimePickerFormat.Custom;
                                //datecontrol2.CustomFormat = "hh:mm:ss";
                                //datecontrol2.KeyDown += new KeyEventHandler(DoKeyDown);
                                //datecontrol2.KeyUp += new KeyEventHandler(DoKeyDown);
                                //datecontrol2.KeyPress += new KeyPressEventHandler(DoKeyPress);
                                datecontrol1.KeyDown += new KeyEventHandler(DoKeyDown);
                                datecontrol1.KeyUp += new KeyEventHandler(DoKeyDown);
                                datecontrol1.KeyPress += new KeyPressEventHandler(DoKeyPress);
                                datecontrol1.LostFocus += new EventHandler(MLostFocus);
                                datecontrol1.ValueChanged += new EventHandler(MValueChange);
                                //datecontrol2.LostFocus += new EventHandler(mLostFocus);
                                //datecontrol2.ValueChanged += new EventHandler(mValueChange);
                                datecontrol1.DoubleClick += new EventHandler(MDoubleClick);
                                //datecontrol2.DoubleClick += new EventHandler(mDoubleClick);
                                Controls.Add(datecontrol1);
                                //Controls.Add(datecontrol2);
                                MainControl = datecontrol1;
                            
                        }
                        //datecontrol1.Visible = ((FDataType == ColumnDataType.Date) || (FDataType == ColumnDataType.DateTime));
                        //datecontrol2.Visible = ((FDataType == ColumnDataType.Time) || (FDataType == ColumnDataType.DateTime));
                        /*if (FDataType == ColumnDataType.Time)
                            MainControl = datecontrol2;
                        else
                            MainControl = datecontrol1;*/                        
                            if(MainControl!=null)
                                MainControl.Visible = true;
                        ResizeControls();
                        break;
                }
            }
            else
            {

            }
        }

        void Datecontrol1_ValueChanged(object sender, EventArgs e)
        {
            if (!disabledchange)
                m_dataGridView.NotifyCurrentCellDirty(true);
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
            CreateMainControl();
            ResizeControls();
        }
        private void ResizeControls()
        {
            if (MainControl != null)
            {
                MainControl.Top = (this.Height - MainControl.Height) / 2;
                int offset = 0;
                if (picbo.Visible)
                    offset = picbo.Width;
                //if (FDataType == ColumnDataType.DateTime)
                //{
                //   datecontrol1.Width = (this.Width - offset) / 2;
                //datecontrol2.Width = datecontrol1.Width;
                //datecontrol2.Left = datecontrol1.Width;
                //datecontrol2.Top = datecontrol1.Top;
                //}
                //else
                {
                    MainControl.Width = this.Width - offset;
                }
            }
        }

        private DataGridViewColumnAdvanced GetColumn()
        {
            DataGridViewColumnAdvanced nresult = null;
            if (m_dataGridView == null)
                return nresult;
            if (m_dataGridView.CurrentCell == null)
                return nresult;
            if (m_dataGridView.CurrentCell.ColumnIndex < 0)
                return nresult;
            return (DataGridViewColumnAdvanced)m_dataGridView.Columns[m_dataGridView.CurrentCell.ColumnIndex];
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            DataGridViewColumnAdvanced ncol = GetColumn();
            if (FDataType != ncol.DataType)
            {
                FDataType = ncol.DataType;
                if (MainControl!=null)
                {
                    MainControl.Visible=false;
                    MainControl = null;
                }
            }
            if (FReadOnlyInput != ncol.ReadOnlyInput)
            {
                FReadOnlyInput = ncol.ReadOnlyInput;
                UpdateReaonlyInput();
            }
            SearchWindow = ncol.SearchWindow;
            if (SearchWindow != null)
            {
                FDataType = ColumnDataType.Text;
                FMaxInputLength = 0;
            }
            else
            {
                FMaxInputLength = ncol.MaxInputLength;
            }

            CreateMainControl();
            if (MainControl is TextBoxAdvanced)
            {
                ((TextBoxAdvanced)MainControl).SearchWindow = SearchWindow;
             }
            disabledchange = true;
            try
            {
                prevvalue = DBNull.Value;
                if (this.m_dataGridView.CurrentCell.Value != null)
                    prevvalue = this.m_dataGridView.CurrentCell.Value;
                SetValue(prevvalue);
                prevvalue = GetValue();
            }
            finally
            {
                disabledchange = false;
            }
            if (this.textcontrol!=null)
                if (selectAll)
                    this.textcontrol.SelectAll();
            if (MainControl != null)
                if (MainControl.Visible)
                    MainControl.Focus();
            ImageButtonScale = ncol.ImageButtonScale;
            ImageButton = ncol.ImageButton;
        }
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }
        public override bool Focused
        {
            get
            {
                if (MainControl != null)
                    return MainControl.Focused;
                else
                    return base.Focused;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (textcontrol!=null)
                textcontrol.Dispose();
            if (datecontrol1!=null)
                datecontrol1.Dispose();
            if (datecontrol2 != null)
                datecontrol2.Dispose();
            //            if (datecontrol2!=null)
            //                datecontrol2.Dispose();
            if (picbo!=null)
                picbo.Dispose();

            base.Dispose(disposing);
        }
    }
    public enum SearchWindowKeyOperation { Up, Down, PageUp, PageDown,Return };
    public delegate void ShowSearchWindowEvent(object sender, ShowSearchWindowArgs args);
    public class ShowSearchWindowArgs
    {
        public Control Window;
        public ShowSearchWindowArgs(Control ncontrol)
        {
            Window = ncontrol;
        }
    }
    public interface ISearchWindow
    {
        void ChangeSearchString(string newvalue);
        void Deactivate();
        void KeyOperation(SearchWindowKeyOperation key_operation);
        Control CreateWindow();
        bool Click(Point clientpoint);
        // Property declaration:
        bool Enabled
        {
            get;
            set;
        }
    }

}
