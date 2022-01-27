using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Reportman.Drawing.Forms;
using Reportman.Reporting;
using Reportman.Drawing;

namespace Reportman.Designer
{
    internal partial class ObjectInspector : DataGridViewAdvanced, IObjectInspector
    {
        DataGridViewColumn ColLabel;
        DataGridViewColumn ColProperty;
        DesignerInterface CurrentInterface;
        public PropertyChanged OnPropertyChange { get; set; }
        DataRowChangeEventHandler rowchangeevent;
        DataSet data;
        String SDouble;
        String SInteger;
        String SString;
        string SDateTime;
        string SDate;
        string STime;
        bool selectingobject;
        ComboBox FComboSelection;
        public ComboBox ComboSelection
        {
            get
            {
                return FComboSelection;
            }
            set
            {
                FComboSelection = value;
            }
        }
        public Control GetControl()
        {
            return this;
        }
        public FrameStructure Structure { get; set; }
        public EditSubReport SubReportEdit { get; set; }
        public SortedList<int, ReportItem> CurrentList = new SortedList<int, ReportItem>();
        public ObjectInspector()
        {
            InitializeComponent();
            rowchangeevent = new DataRowChangeEventHandler(RowChange);
            ColumnHeadersVisible = false;
        }
        private void FormatCell(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0)
                return;
            DataGridViewRow dgview = Rows[e.RowIndex];
            DataRowView rview = (DataRowView)dgview.DataBoundItem;
            DataRow nrow = rview.Row;
            if (nrow["TYPE"].ToString() == SDouble)
            {
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
        private DataTable FindDataTable(string tablename, DesignerInterface obj)
        {
            // Table name differen for different secions types
            if (obj.ReportItemObject is Section)
            {
                Section nsec = (Section)obj.ReportItemObject;
                tablename = tablename + nsec.SectionType.ToString();
            }
            Reportman.Drawing.Strings pnames = new Strings();
            DataTable properties = data.Tables[tablename];

            bool assignproperties = true;
            if (properties == null)
            {
                Reportman.Reporting.Variants pvalues = null;
                if (obj.SelectionList.Count == 1)
                {
                    assignproperties = false;
                    pvalues = new Variants();
                }
                properties = new DataTable(tablename);
                properties.Columns.Add("NAME", System.Type.GetType("System.String"));
                properties.Columns.Add("TYPE", System.Type.GetType("System.String"));
                properties.Columns.Add("TYPEENUM", System.Type.GetType("System.Int32"));
                properties.Columns.Add("VALUE", System.Type.GetType("System.Object"));
                properties.Columns.Add("VALUELIST", System.Type.GetType("System.Object"));
                properties.Columns.Add("INTERFACE", System.Type.GetType("System.Object"));
                properties.Columns.Add("VALUEBIN", System.Type.GetType("System.Object"));
                DataColumn acol = properties.Columns.Add("TWIPS", System.Type.GetType("System.Boolean"));
                acol.DefaultValue = false;

                properties.Constraints.Add("IPRIMNAME", properties.Columns["NAME"], true);
                Reportman.Drawing.Strings ptypes = new Strings();
                Reportman.Drawing.Strings lhints = new Strings();
                Reportman.Drawing.Strings lcat = new Strings();
                obj.GetProperties(pnames, ptypes, pvalues, lhints, lcat);
                properties.Rows.Clear();
                for (int i = 0; i < pnames.Count; i++)
                {
                    bool isbinary = false;
                    DataRow nrow = properties.NewRow();
                    nrow["NAME"] = pnames[i];
                    nrow["TYPE"] = ptypes[i];
                    nrow["INTERFACE"] = obj;
                    nrow["VALUELIST"] = DBNull.Value;
                    nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.Text);
                    if (ptypes[i] == Translator.TranslateStr(571))
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.Expression);
                    }
                    else
                    if (ptypes[i] == Translator.TranslateStr(1099))
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.ConnectionString);
                    }
                    else
                    if (ptypes[i] == "SQL")
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.SQL);
                    }
                    else
                        if (ptypes[i] == Translator.TranslateStr(556))
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.Decimal);
                        nrow["TWIPS"] = true;
                    }
                    else
                        if (ptypes[i] == Translator.TranslateStr(1171))
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.Decimal);
                        nrow["TWIPS"] = false;
                    }
                    else
                            if (ptypes[i] == Translator.TranslateStr(569))
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.DropDownList);
                        Strings alist = new Strings();
                        obj.GetPropertyValues(pnames[i], alist);
                        nrow["VALUELIST"] = alist;
                    }
                    else if (ptypes[i] == Translator.TranslateStr(961))
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.DropDown);
                        Strings alist = new Strings();
                        obj.GetPropertyValues(pnames[i], alist);
                        nrow["VALUELIST"] = alist;
                    }
                    else
                        if (ptypes[i] == Translator.TranslateStr(568))
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.Boolean);
                    }
                    else
                            if (ptypes[i] == Translator.TranslateStr(558))
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.Color);
                    }
                    else
                                if (ptypes[i] == Translator.TranslateStr(639))
                    {
                        isbinary = true;
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.Image);
                    }
                    else
                                if (ptypes[i] == Translator.TranslateStr(560))
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.FontName);
                    }
                    else
                            if (ptypes[i] == Translator.TranslateStr(566))
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.FontStyle);
                    }
                    else
                                // Font size
                                if (ptypes[i] == Translator.TranslateStr(559))
                    {
                        nrow["TYPEENUM"] = System.Convert.ToInt32(ObjectInspectorCellType.Integer);
                    }
                    if (pvalues != null)
                    {
                        if (isbinary)
                        {
                            nrow["VALUEBIN"] = pvalues[i];
                            System.IO.MemoryStream mstream = pvalues[i].GetStream();
                            nrow["VALUE"] = StringUtil.GetSizeAsString(mstream.Length);

                        }
                        else
                        {
                            if ((bool)nrow["TWIPS"])
                                nrow["VALUE"] = Twips.UnitsFromTwips(pvalues[i]);
                            else
                                nrow["VALUE"] = pvalues[i];
                        }
                    }
                    properties.Rows.Add(nrow);
                }
                properties.RowChanging += rowchangeevent;
                data.Tables.Add(properties);
            }
            if (assignproperties)
            {
                properties.RowChanging -= rowchangeevent;
                for (int i = 0; i < properties.Rows.Count; i++)
                {
                    DataRow nrow = properties.Rows[i];
                    string ntype = nrow["TYPE"].ToString();
                    ObjectInspectorCellType cellType = ObjectInspectorCellType.Text;
                    if (nrow["TYPEENUM"] != DBNull.Value)
                        cellType = (ObjectInspectorCellType)nrow["TYPEENUM"];
                    if ((ntype == Translator.TranslateStr(569)) || (ntype == Translator.TranslateStr(961)))
                    {
                        Strings alist = new Strings();
                        obj.GetPropertyValues(nrow["NAME"].ToString(), alist);
                        nrow["VALUELIST"] = alist;
                    }
                    Variant nvalue = obj.GetPropertyMulti(nrow["NAME"].ToString());
                    if (cellType == ObjectInspectorCellType.Color)
                    {
                        if (!nvalue.IsNull)
                            nrow["VALUE"] = (int)nvalue;
                    }
                    else
                    if (nvalue.VarType == VariantType.Binary)
                    {
                        nrow["VALUEBIN"] = nvalue;
                        nrow["VALUE"] = StringUtil.GetSizeAsString(nvalue.GetStream().Length);
                    }
                    else
                    {

                        if (nvalue.IsNull)
                        {
                            nrow["VALUE"] = DBNull.Value;
                        }
                        else
                        {
                            if ((bool)nrow["TWIPS"])
                                nrow["VALUE"] = Twips.UnitsFromTwips(nvalue);
                            else
                                nrow["VALUE"] = nvalue;
                            if (nrow["TYPE"].ToString() == Translator.TranslateStr(961))
                            {
                                Strings alist = new Strings();
                                obj.GetPropertyValues(nrow["NAME"].ToString(), alist);
                                nrow["VALUELIST"] = alist;
                            }
                        }
                    }
                }
                properties.RowChanging += rowchangeevent;
            }
            return properties;
        }
        private FrameMainDesigner FrameMain;
        public void Initialize(FrameMainDesigner nFrameMain)
        {
            FrameMain = nFrameMain;
            SString = Translator.TranslateStr(571);
            SInteger = Translator.TranslateStr(556);
            SDouble = Translator.TranslateStr(556);
            SDate = Translator.TranslateStr(888);
            SDateTime = Translator.TranslateStr(889);
            STime = Translator.TranslateStr(890);
            data = new DataSet();

            AutoGenerateColumns = false;
            SkipReadOnly = true;
            ColumnCount = 0;
            Columns.Add(new DataGridViewTextBoxColumn());
            ObjectInspectorColumn ncolumn = new ObjectInspectorColumn();
            ncolumn.FrameMain = FrameMain;
            Columns.Add(ncolumn);

            EnterAsTab = false;
            ColLabel = Columns[0];
            ColProperty = Columns[1];
            ColProperty.Resizable = DataGridViewTriState.False;
            ColLabel.Frozen = true;
            ColLabel.ReadOnly = true;
            ColLabel.DefaultCellStyle.BackColor = SystemColors.ButtonFace;
            ColLabel.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            ColProperty.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ColLabel.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ColLabel.DataPropertyName = "NAME";
            ColProperty.DataPropertyName = "VALUE";
            ColProperty.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            AllowUserToOrderColumns = false;
            AllowUserToResizeRows = false;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.EditMode = DataGridViewEditMode.EditOnEnter;
            this.RowHeadersVisible = false;
            this.MultiSelect = false;

            ColLabel.SortMode = DataGridViewColumnSortMode.NotSortable;
            ColProperty.SortMode = DataGridViewColumnSortMode.NotSortable;

            ColProperty.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            CellFormatting += new DataGridViewCellFormattingEventHandler(FormatCell);


            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            //            Font = new Font(Font.FontFamily, 6);
            int nheight = (int)Math.Round(Font.Size * 1.4 * (double)WinFormsGraphics.ScreenDPI() / (double)72);
            nheight = nheight + 5;
            if (nheight < 10)
                nheight = nheight + 2;
            RowTemplate.Height = nheight;
        }
        public void SetObject(DesignerInterface obj)
        {
            CurrentList.Clear();
            if (DataSource != null)
                if (CurrentRow != null)
                {
                    if (CurrentRow.DataBoundItem != null)
                        RowChange(this, new DataRowChangeEventArgs(((DataRowView)CurrentRow.DataBoundItem).Row, DataRowAction.Change));
                    DataSource = null;
                }
            CurrentInterface = obj;
            if (CurrentInterface == null)
            {
                Visible = false;
                return;
            }
            else
                Visible = true;
            Section nsection = null;
            bool issection = false;
            selectingobject = true;
            try
            {

                PrintPosItem nitem = null;
                if (CurrentInterface is DesignerInterfaceSection)
                {
                    issection = true;
                    nsection = (Section)CurrentInterface.SelectionList.Values[0];
                }
                else
                    if (CurrentInterface is DesignerInterfaceSizePos)
                {
                    nitem = (PrintPosItem)CurrentInterface.SelectionList.Values[0];
                    nsection = nitem.Section;
                    if (CurrentInterface.SelectionList.Count != 1)
                        nitem = null;
                }
                ComboSelection.Items.Clear();
                ComboSelection.SelectedIndex = -1;
                int selindex = 1;
                int newindex = 0;
                if (nsection != null)
                {
                    ComboSelection.Items.Add(nsection.Name);
                    CurrentList.Add(0, nsection);
                    foreach (PrintPosItem xitem in nsection.Components)
                    {
                        ComboSelection.Items.Add(xitem.Name);
                        CurrentList.Add(selindex, xitem);
                        if (nitem == xitem)
                            newindex = selindex;
                        selindex++;
                    }
                    if (issection)
                        ComboSelection.SelectedIndex = 0;
                    else
                        if (CurrentInterface.SelectionList.Count == 1)
                        ComboSelection.SelectedIndex = newindex;
                    if (Structure != null)
                        Structure.SelectItem(nsection, true);
                }
                else
                {
                    if (CurrentInterface.SelectionList.Count == 1)
                    {
                        ComboSelection.Items.Add(CurrentInterface.SelectionList.Values[0].Name);
                        ComboSelection.SelectedIndex = 0;
                        CurrentList.Add(0, CurrentInterface.SelectionList.Values[0]);
                        if (Structure != null)
                            Structure.SelectItem(CurrentInterface.SelectionList.Values[0], true);
                    }
                }


                DataTable properties = FindDataTable(obj.SelectionClassName, obj);
                DataSource = properties;
            }
            finally
            {
                selectingobject = false;
            }
        }
        public void SetObjectFromCombo()
        {
            if (selectingobject)
                return;
            if (CurrentList.Count == 0)
                return;
            if (ComboSelection.SelectedIndex > CurrentList.Count - 1)
                return;
            ReportItem ritem = CurrentList[ComboSelection.SelectedIndex];
            if (!(ritem is PrintItem))
                return;

            PrintItem nitem = (PrintItem)ritem;
            SubReportEdit.SelectPrintItem(nitem);
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Agregar código de dibujo personalizado aquí

            // Llamando a la clase base OnPaint
            base.OnPaint(pe);
        }
        private void RowChange(object sender, DataRowChangeEventArgs args)
        {
            object newValue = DBNull.Value;
            bool executeonpropchange = false;
            if (args.Row["VALUE"] != DBNull.Value)
            {
                if (CurrentInterface != null)
                {
                    if ((bool)args.Row["TWIPS"])
                    {
                        CurrentInterface.SetPropertyMulti(args.Row["NAME"].ToString(), Twips.TwipsFromUnits(Variant.VariantFromObject(args.Row["VALUE"])));
                        executeonpropchange = true;
                    }
                    else
                    {
                        bool isbinary = false;
                        if ((int)args.Row["TYPEENUM"] == System.Convert.ToInt32(ObjectInspectorCellType.Image))
                            isbinary = true;
                        if (!isbinary)
                        {
                            object newValueObject = args.Row["VALUE"];
                            string propName = args.Row["NAME"].ToString();
                            object interfaceParamObject = args.Row["INTERFACE"];
                            if ((interfaceParamObject is DesignerInterfaceParam) && (propName == Translator.TranslateStr(194)))
                            {
                                var interfaceParam = (DesignerInterfaceParam)interfaceParamObject;
                                var param = (Param)interfaceParam.ReportItemObject;
                                switch (param.ParamType)
                                {
                                    case ParamType.Date:
                                        DateTime newDate = Convert.ToDateTime(newValueObject);
                                        newDate = newDate.Date;
                                        newValueObject = newDate;
                                        break;
                                    case ParamType.DateTime:
                                        DateTime newDate2 = Convert.ToDateTime(newValueObject);
                                        newValueObject = newDate2;
                                        break;
                                    case ParamType.Time:
                                        DateTime newDate3 = Convert.ToDateTime(newValueObject);
                                        newValueObject = newDate3;
                                        break;
                                }
                            }
                            CurrentInterface.SetPropertyMulti(args.Row["NAME"].ToString(), Variant.VariantFromObject(newValueObject));
                            executeonpropchange = true;
                        }
                    }

                }
                newValue = args.Row["VALUE"];
            }
            if (args.Row["VALUEBIN"] != DBNull.Value)
            {
                if (CurrentInterface != null)
                {
                    bool isbinary = false;
                    if ((int)args.Row["TYPEENUM"] == System.Convert.ToInt32(ObjectInspectorCellType.Image))
                        isbinary = true;
                    if (isbinary)
                    {
                        CurrentInterface.SetPropertyMulti(args.Row["NAME"].ToString(), Variant.VariantFromObject(args.Row["VALUEBIN"]));
                        executeonpropchange = true;
                    }
                }
                newValue = args.Row["VALUEBIN"];
            }
            if ((executeonpropchange) && (OnPropertyChange != null))
            {
                OnPropertyChange(args.Row["NAME"].ToString(), newValue);
            }
        }
    }
}
