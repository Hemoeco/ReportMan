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

#if REPMAN_DOTNET2
using System.Collections.Generic;
#endif
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Reporting.Forms
{
    /// <summary>
    /// Window control used by parameters form, you can use it to build a custom parameters form
    /// </summary>
    public class ParamsControl : ScrollableControl
    {
		SortedList CheckListBoxes;
        /// <summary>
        /// Required by designer
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Dispose resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows forms designed generated code

        /// <summary>
        /// Required by Windows forms designer
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
        private Report FReport;
		private Report FOriginalReport;
		private ControlInfoList ControlList; 
		private EventHandler eventclick;
        /// <summary>
        /// Maximum label width in pixels, default 500
        /// </summary>
        public int MaxLabelWidth;
        /// <summary>
        /// Maximum control width in pixels, default 250
        /// </summary>
        public int ControlWidth;
        /// <summary>
        /// Constructor
        /// </summary>
        public ParamsControl()
        {
            InitializeComponent();
            MaxLabelWidth = 500;
            ControlWidth = 250;
            AutoScroll = true;
            ControlList=new ControlInfoList();
			eventclick=new EventHandler(ClickEvent);
			CheckListBoxes=new SortedList();
        }
		private void ClickEvent(object sender,EventArgs e)
		{
			ControlInfo cinfo=(ControlInfo)((Button)sender).Tag;
            if (SearchForm.SearchParamValue(cinfo.param, FReport))
            {
                if (cinfo.control is TextBox)
                {
                    ((TextBox)cinfo.control).Text = cinfo.param.Value.ToString();
                }
                else
                    if (cinfo.control is NumericUpDown)
                    {
                        ((NumericUpDown)cinfo.control).Value =cinfo.param.Value;
                    }
            }
		}
        /// <summary>
        /// Set the report, creates internal controls to alter the parameters
        /// </summary>
        /// <param name="rp">Report to be modified</param>
        public void SetReport(Report rp)
        {
			FOriginalReport=rp;
            FReport = new Report();
			FReport.Params=FOriginalReport.Params.Clone(FReport);
			FReport.DataInfo=FOriginalReport.DataInfo.Clone(FReport);
			FReport.DatabaseInfo=FOriginalReport.DatabaseInfo.Clone(FReport);
			FReport.Language=FOriginalReport.Language;
            FReport.InitializeParams();
            CreateControls();
        }
        /// <summary>
        /// Set the focus to the first parameter control.
        /// </summary>
        public void FocusFirst()
        {
            if (ControlList.Count > 0)
            {
                ControlList[0].control.Focus();
            }
        }
        /// <summary>
        /// Call this method when you want to stored user selection values into the Report parameters
        /// </summary>
        /// <returns>Returns true if the parameters were correctly assigned</returns>
        public bool AcceptParams()
        {
			bool aresult=true;
            foreach (ControlInfo cinfo in ControlList)
            {
                Param p = cinfo.param;
                switch (p.ParamType)
                {
                    case ParamType.Bool:
                        p.Value=((CheckBox)cinfo.control).Checked;
                        break;
                    case ParamType.String:
                    case ParamType.Subst:
                    case ParamType.SubstExpre:
                        p.Value=((TextBox)cinfo.control).Text;
                        break;
                    case ParamType.Date:
                        p.Value=((DateTimePicker)cinfo.control).Value;
                        break;
                    case ParamType.Time:
                        p.Value=((DateTimePicker)cinfo.control).Value;
                        break;
                    case ParamType.DateTime:
                        DateTime nvalue=((DateTimePicker)cinfo.control).Value;
                        TimeSpan tspan = new TimeSpan(nvalue.Hour, nvalue.Minute, nvalue.Second);
                        nvalue = nvalue.Subtract(tspan);
                        DateTime atime = ((DateTimePicker)cinfo.control2).Value;
                        tspan = new TimeSpan(atime.Hour, atime.Minute, atime.Second);
                        nvalue=nvalue.Add(tspan);
                        p.Value = nvalue;
                        break;
                    case ParamType.Integer:
                        p.Value=System.Convert.ToInt32(((NumericUpDown)cinfo.control).Value);
                        break;
                    case ParamType.Currency:
                        p.Value=((NumericUpDown)cinfo.control).Value;
                        break;
                    case ParamType.Double:
                        p.Value=System.Convert.ToDouble(((TextBox)cinfo.control).Text);
                        break;
                    case ParamType.List:
                    case ParamType.SubsExpreList: 
                        int index = ((ComboBox)cinfo.control).SelectedIndex;
                        if (index < 0)
                            index = 0;
                        if (index < p.Values.Count)
                            p.Value = p.Values[index];
                        else
                            p.Value = "";
                        break;
                    case ParamType.Multiple:
                        p.Selected.Clear();
                        foreach (int chk in ((CheckedListBox)cinfo.control).CheckedIndices)
                        {
                            p.Selected.Add(p.Values[chk]);
                        }
                                    // Selection by value instead of by index
                                    /*int index2 = p.Values.IndexOf(sitem);
                                    if (index2 >= 0)
                                        checkcontrol.SetItemChecked(index, true);
                                    else
                                    {
                                        if (DoubleUtil.IsNumeric(sitem, System.Globalization.NumberStyles.Integer))
                                        {
                                            int xindex = System.Convert.ToInt32(sitem);
                                            if (xindex < checkcontrol.Items.Count && xindex >= 0)
                                                checkcontrol.SetItemChecked(xindex, true);
                                        }
                                    }*/
                        break;
                }
            }
			// Check Expression
			Param iparam;
			string incorrectparam=FReport.CheckParameters();
			if (incorrectparam.Length>0)
			{
				aresult=false;
				iparam=FReport.Params[incorrectparam];
				foreach (ControlInfo ninfo in ControlList)
				{
					if (ninfo.param==iparam)
					{
						if (ninfo.control.Visible && ninfo.control.Enabled)
						{
							ninfo.control.Focus();
						}
						break;
					}
				}
				MessageBox.Show(iparam.ErrorMessage,iparam.Description);
			}
			if (aresult)
			{
				FOriginalReport.Params=FReport.Params.Clone(FOriginalReport);
			}
			return aresult;
		}
        private void contextmenuclickselectall(object sender,EventArgs e)
        {
//            CheckedListBox control = (CheckedListBox)((MenuItem)sender).Tag;
			CheckedListBox control = (CheckedListBox)this.CheckListBoxes[((MenuItem)sender).Parent.MenuItems[0].Text];
			for (int i = 0; i < control.Items.Count; i++)
                control.SetItemChecked(i, true);
        }
        private void contextmenuclickclear(object sender, EventArgs e)
        {
//            CheckedListBox control = (CheckedListBox)((MenuItem)sender).Tag;
            CheckedListBox control = (CheckedListBox)this.CheckListBoxes[((MenuItem)sender).Parent.MenuItems[0].Text];
			for (int i = 0; i < control.Items.Count; i++)
                control.SetItemChecked(i, false);
        }
        private void CreateControls()
        {
            using (Graphics gr=this.CreateGraphics())
            {
                // Measure texts
                int labelwidth = 0;
                foreach (Param p in FReport.Params)
                {
                    if (p.UserVisible)
                    {
                        int nwidth = (int)Math.Round(gr.MeasureString(p.Description, this.Font).Width);
                        if (nwidth > labelwidth)
                            labelwidth = nwidth;
                        if (labelwidth > MaxLabelWidth)
                            labelwidth = MaxLabelWidth;
                    }
                }
                bool focused = false;
                int TOP_GAP= Convert.ToInt32(10*GraphicUtils.DPIScale);
                int MIDDLE_GAP = Convert.ToInt32(3 * GraphicUtils.DPIScale);
                int BOTTOM_GAP = Convert.ToInt32(10 * GraphicUtils.DPIScale); ;
                int BUTTON_WIDTH = Convert.ToInt32(30 * GraphicUtils.DPIScale);
                int LEFT_GAP = Convert.ToInt32(3 * GraphicUtils.DPIScale); ;
                int GAP_LABEL = Convert.ToInt32(30 * GraphicUtils.DPIScale); ;
                int BUTTON_GAP = Convert.ToInt32(2 * GraphicUtils.DPIScale); ;
                int RIGHT_GAP = BUTTON_WIDTH+ Convert.ToInt32(30 * GraphicUtils.DPIScale) +BUTTON_GAP;
                int posy = TOP_GAP;
                int lastheight = 0;
                foreach (Param p in FReport.Params)
                {
                    if (p.UserVisible)
                    {
                        ControlInfo cinfo = new ControlInfo();
                        cinfo.param = p;
                        cinfo.label = new Label();
                        cinfo.label.AutoSize = false;
                        cinfo.label.Text = p.Description;
                        cinfo.label.Width = labelwidth+LEFT_GAP;
                        cinfo.label.Left = 0;
                        cinfo.label.Top = posy;
                        cinfo.label.Parent = this;
                        switch (p.ParamType)
                        {
                            case ParamType.Bool:
                                cinfo.control = new CheckBox();
                                ((CheckBox)cinfo.control).Checked = (bool)p.LastValue;
                                break;
                            case ParamType.String:
                            case ParamType.Subst:
                            case ParamType.SubstExpre:
                            case ParamType.ExpreA:
                            case ParamType.ExpreB:
                                cinfo.control = new TextBox();
                                ((TextBox)cinfo.control).Text = (string)p.LastValue;
                                break;
                            case ParamType.Date:
                                cinfo.control = new DateTimePicker();
                                ((DateTimePicker)cinfo.control).Value = (DateTime)p.LastValue;
                                ((DateTimePicker)cinfo.control).Format = DateTimePickerFormat.Short;
                                break;
                            case ParamType.Time:
                                cinfo.control = new DateTimePicker();
                                ((DateTimePicker)cinfo.control).Value = (DateTime)p.LastValue;
                                ((DateTimePicker)cinfo.control).Format = DateTimePickerFormat.Custom;
                                ((DateTimePicker)cinfo.control).ShowUpDown = true;
                                ((DateTimePicker)cinfo.control).CustomFormat = "hh:mm:ss";
                                break;
                            case ParamType.DateTime:
                                cinfo.control = new DateTimePicker();
                                ((DateTimePicker)cinfo.control).Value = (DateTime)p.LastValue;
                                ((DateTimePicker)cinfo.control).Format = DateTimePickerFormat.Short;
                                cinfo.control2 = new DateTimePicker();
                                ((DateTimePicker)cinfo.control2).Value = (DateTime)p.LastValue;
                                ((DateTimePicker)cinfo.control2).Format = DateTimePickerFormat.Custom;
                                ((DateTimePicker)cinfo.control2).ShowUpDown = true;
                                ((DateTimePicker)cinfo.control2).CustomFormat = "hh:mm:ss";
                                break;
                            case ParamType.Integer:
                                cinfo.control = new NumericUpDown();
                                ((NumericUpDown)cinfo.control).Minimum = System.Int32.MinValue;
                                ((NumericUpDown)cinfo.control).Maximum = System.Int32.MaxValue;
                                ((NumericUpDown)cinfo.control).Value = (int)p.LastValue;
                                break;
                            case ParamType.Currency:
                                cinfo.control = new NumericUpDown();
                                ((NumericUpDown)cinfo.control).Minimum = System.Decimal.MinValue;
                                ((NumericUpDown)cinfo.control).Maximum = System.Decimal.MaxValue;
                                ((NumericUpDown)cinfo.control).Value = (decimal)p.LastValue;
                                break;
                            case ParamType.Double:
                                cinfo.control = new TextBox();
                                ((TextBox)cinfo.control).Text = ((double)p.LastValue).ToString();
                                break;
                            case ParamType.List:
                            case ParamType.SubsExpreList:
                                cinfo.control = new ComboBox();
                                ((ComboBox)cinfo.control).DropDownStyle = ComboBoxStyle.DropDownList;
                                foreach (string sitem in p.Items)
                                    ((ComboBox)cinfo.control).Items.Add(sitem);
                                ((ComboBox)cinfo.control).SelectedIndex = p.Values.IndexOf((string)p.Value);
                                if (((ComboBox)cinfo.control).SelectedIndex==-1)
                                    if (p.Value.IsInteger())
                                    {
                                        int idxcombo = p.LastValue;
                                        if (p.Value<=((ComboBox)cinfo.control).Items.Count)
                                            ((ComboBox)cinfo.control).SelectedIndex=idxcombo;
                                    }
                                break;
                            case ParamType.Multiple:
                                p.UpdateLookupValues();
                                CheckedListBox checkcontrol=new CheckedListBox();
                                cinfo.control = checkcontrol;
                                // Add menu to select/deselect all
                                ContextMenu nmenu = new ContextMenu();
								MenuItem firstmenu = nmenu.MenuItems.Add(p.Alias);
								firstmenu.Visible=false;
                                MenuItem mselectall=nmenu.MenuItems.Add(Translator.TranslateStr(1445));
                                MenuItem mclear = nmenu.MenuItems.Add(Translator.TranslateStr(1446));
								this.CheckListBoxes.Add(p.Alias,cinfo.control);
//								mselectall.Tag = cinfo.control;
//                                mclear.Tag = cinfo.control;
                                mselectall.Click += new EventHandler(contextmenuclickselectall);
                                mclear.Click += new EventHandler(contextmenuclickclear);
                                cinfo.control.ContextMenu = nmenu;
                                foreach (string sitem in p.Items)
                                    ((CheckedListBox)cinfo.control).Items.Add(sitem);
                                foreach (string sitem in  p.Selected)
                                {
//                                    int index = System.Convert.ToInt32(sitem);
//                                    if (index<((CheckedListBox)cinfo.control).Items.Count)
//                                    {
//                                        ((CheckedListBox)cinfo.control).SetItemChecked(index,true);
//                                    }
                                    // Selection by value instead of by index
                                    int index = p.Values.IndexOf(sitem);
                                    if (index>=0)
                                        checkcontrol.SetItemChecked(index, true);
                                }
                                // Adjust Height up to 10 items
                                if (checkcontrol.Items.Count<10)
                                    checkcontrol.Height = checkcontrol.ItemHeight * (checkcontrol.Items.Count + 1);
                                else
                                    checkcontrol.Height = checkcontrol.ItemHeight * (11);
                                break;
                        }
                        cinfo.control.Left = labelwidth + LEFT_GAP + GAP_LABEL;
                        cinfo.control.Enabled = !p.IsReadOnly;
                        if (cinfo.control2 != null)
                        {
                            cinfo.control2.Enabled = !p.IsReadOnly;
                            cinfo.control.Width = ControlWidth/2;
                            cinfo.control2.Top = posy;
                            cinfo.control2.Parent = this;
                            cinfo.control2.Width = (ControlWidth /2)-LEFT_GAP;
                            cinfo.control2.Left = labelwidth + LEFT_GAP + GAP_LABEL+(ControlWidth / 2) + LEFT_GAP;
                        }
                        else
                            cinfo.control.Width = ControlWidth;
                        cinfo.control.Parent = this;
                        cinfo.control.Top = posy;
                        cinfo.label.Height = cinfo.control.Height;
                        cinfo.label.TextAlign = ContentAlignment.MiddleRight;
                        cinfo.control.Tag = cinfo;
                        cinfo.button = new Button();
						cinfo.button.Tag=cinfo;
						cinfo.button.Click+=eventclick;
                        cinfo.button.Text="...";
                        cinfo.button.Width=BUTTON_WIDTH;
                        cinfo.button.Height = cinfo.control.Height;
                        cinfo.button.Left = cinfo.control.Left + cinfo.control.Width+BUTTON_GAP;
                        if (cinfo.control2!=null)
                            cinfo.button.Left = cinfo.button.Left + cinfo.control2.Width+MIDDLE_GAP;
                        cinfo.button.Top = posy;
                        cinfo.button.Visible = (p.SearchDataset.Length > 0);
                        cinfo.button.Parent=this;
                        if (cinfo.control.Enabled)
                            if (!focused)
                                cinfo.control.Focus();

                        cinfo.PosY = posy;
                        lastheight = cinfo.control.Height;
                        posy = posy + cinfo.control.Height + MIDDLE_GAP;

                        ControlList.Add(cinfo);
                    }
                }
                this.Width = labelwidth + LEFT_GAP + GAP_LABEL + ControlWidth + LEFT_GAP + RIGHT_GAP;
                this.Height = posy + lastheight + MIDDLE_GAP + BOTTOM_GAP;
            }
        }
    }
}
