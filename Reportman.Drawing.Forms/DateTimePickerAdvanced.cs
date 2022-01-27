//Copyright 2013 by Peter Ringering.
//Feel free to use this code how ever you wish, but please don't remove this comment block.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Reportman.Drawing.Forms
{
    public partial class DateTimePickerAdvanced : UserControl
    {
        #region Variables, Properties and Events
        private Rectangle m_rectButton;
        private System.Windows.Forms.VisualStyles.ComboBoxState m_eButtonState = System.Windows.Forms.VisualStyles.ComboBoxState.Normal;
        System.Globalization.DateTimeFormatInfo ninfo;

        private string m_szDateEntryFormat = "MM/dd/yyyy";
        private string m_szDateDisplayFormat = "MM/dd/yyyy";
        private string m_szTimeEntryFormat = "";
        private string m_szTimeDisplayFormat = "";
        private string m_szCombinedEntryFormat = "";
        private string m_szCombinedDisplayFormat = "";
        private string m_szDateTimeEntryPattern = ""; //Used as the pattern when entering data.
        private bool m_bValidDateFormat = true;
        private bool m_bValidTimeFormat = true;
        private DateTime m_dteMaxDate = DateTime.MaxValue;
        private DateTime m_dteMinDate = DateTime.MinValue;

        protected const string NOSUPPORTDATECHARS = "fFgKz%\\";

        private MonthCalendar m_ctlCalendar = null;
        private PopupWindow m_wndCalendar = null;

        /// <summary>
        /// Get to check if both the date and time formats are valid.
        /// </summary>
        public bool ValidFormatStrings { get { return m_bValidDateFormat && m_bValidTimeFormat; } }
        /// <summary>
        /// Gets the pattern in 0's how the date and time are entered.
        /// </summary>
        public string DateTimeEntryPattern
        {
            get { return m_szDateTimeEntryPattern; }
        }
        public System.Globalization.DateTimeFormatInfo DateFormatInfo
        {
            get
            {
                if (ninfo == null)
                {
                    ninfo = System.Globalization.DateTimeFormatInfo.CurrentInfo;
                }
                return ninfo;
            }
        }
        public bool EnterAsTab { get; set; }

        public BeforeEnterTabEvent BeforeEnterTab;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (EnterAsTab)
            {
                if (keyData == (Keys.Enter))
                {
                    bool cancelled = false;
                    BeforeEnterTab(ref cancelled);
                    if (!cancelled)
                        SendKeys.Send("{TAB}");
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (EnterAsTab)
            {
                if (e.KeyCode == Keys.Return)
                    e.SuppressKeyPress = true;
            }
            base.OnKeyDown(e);
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            int maxheight = Math.Max(txtDate.Height, btnCalendar.Height);

            height = maxheight;
            btnCalendar.SetBounds(width - btnCalendar.Width, btnCalendar.Top,btnCalendar.Width, btnCalendar.Height);
            txtDate.SetBounds(0, (maxheight - txtDate.Height) / 2, width - btnCalendar.Width, txtDate.Height);
            
            base.SetBoundsCore(x, y, width, height, specified);
        }
        public string CustomFormat
        {
            get
            {
                string nresult = DateFormat;
                if (TimeFormat.Length>0)
                {
                    if (DateFormat.Length > 0)
                        nresult = nresult + " " + TimeFormat;
                    else
                        nresult = TimeFormat;
                }
                return nresult;
            }
            set
            {
                string newvalue = value;
                if (value.IndexOf("/")>=0)
                {
                    string[] formats = newvalue.Split(' ');
                    DateFormat = formats[0];
                    if (formats.Length > 1)
                        TimeFormat = formats[formats.Length - 1];
                    else
                        TimeFormat = "";
                }
                else
                {
                    DateFormat = "";
                    TimeFormat = newvalue;
                }
            }
        }
        /// <summary>
        /// Gets and sets the displayed date format string.
        /// </summary>
        public string DateFormat
        {
            get
            {
                return m_szDateDisplayFormat;
            }
            set
            {
                m_szDateDisplayFormat = value;
                ValidateDateFormat();
                SetDateTimeEntryPattern();
                ReformatDateControl(m_szCombinedDisplayFormat);
            }
        }
        /// <summary>
        /// Gets and sets the displayed time format string.
        /// </summary>
        public string TimeFormat
        {
            get
            {
                return m_szTimeDisplayFormat;
            }
            set
            {
                m_szTimeDisplayFormat = value;
                ValidateTimeFormat();
                SetDateTimeEntryPattern();
                ReformatDateControl(m_szCombinedDisplayFormat);
            }
        }
        /// <summary>
        /// Gets the displayed combined date and time format string.
        /// </summary>
        public string DateTimeFormat { get { return m_szCombinedDisplayFormat; } }

        /// <summary>
        /// Gets and sets the displayed date and time in the text part of the control.  Computer will beep if set value is not a valid date or time.
        /// </summary>
        public override string Text
        {
            get
            {

                if (txtDate.Text == m_szDateTimeEntryPattern)
                    return "";

                return txtDate.Text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (this.ContainsFocus)
                        txtDate.Text = m_szDateTimeEntryPattern;
                    else
                        txtDate.Text = "";
                }
                else
                {
                    if (IsDateValid(value) == IsDateValidResult.Valid)
                    {
                        if (m_bValidDateFormat && m_bValidTimeFormat)
                            txtDate.Text = GBLMethods.FormatDate(GBLMethods.CDate(value, m_szCombinedDisplayFormat), m_szCombinedDisplayFormat);
                        else if (!DesignMode)
                            GBLMethods.MessageBeep(MessageBeepTypes.Default);
                    }
                    else if (!DesignMode)
                        GBLMethods.MessageBeep(MessageBeepTypes.Default);
                }
            }
        }
        /// <summary>
        /// Gets the displayed text in the textbox as is.
        /// </summary>
        public string FormattedText
        {
            get
            {
                return txtDate.Text;
            }
        }
        /// <summary>
        /// Gets and sets the DateTime value in the control.  Computer will beep if the control has an invalid date.
        /// </summary>
        public DateTime Value
        {
            get
            {
                DateTime dteValue = DateTime.MinValue;
                if (txtDate.Text == m_szDateTimeEntryPattern)
                    return dteValue;
                if (txtDate.Text.Length == 0)
                    return dteValue;
                if (this.ValidDate == IsDateValidResult.Valid)
                    dteValue = GBLMethods.CDate(txtDate.Text, m_szCombinedEntryFormat);
                else if (!DesignMode)
                    GBLMethods.MessageBeep(MessageBeepTypes.Default);

                return dteValue;
            }
            set
            {
                if (m_bValidDateFormat && m_bValidTimeFormat)
                {
                    if (value == DateTime.MinValue && DesignMode)
                        txtDate.Text = m_szDateTimeEntryPattern;
                    else if (value == DateTime.MinValue)
                    {
                        if (this.ContainsFocus)
                            txtDate.Text = m_szDateTimeEntryPattern;
                        else
                            txtDate.Text = "";
                        //txtDate.Text = "";
                        //txtDate.Text = m_szDateTimeEntryPattern;
                    }
                    else
                        txtDate.Text = GBLMethods.FormatDate(value, m_szCombinedDisplayFormat);
                    OnValueChanged(new EventArgs());
                }
                else if (!DesignMode)
                    GBLMethods.MessageBeep(MessageBeepTypes.Default);
            }
        }
        /// <summary>
        /// Gets and sets the textbox's background color.
        /// </summary>
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                txtDate.BackColor = value;
            }
        }
        /// <summary>
        /// Gets and sets the textbox's foreground color.
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                txtDate.ForeColor = value;
            }
        }
        /// <summary>
        /// Gets and sets the textbox's font.
        /// </summary>
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                txtDate.Font = value;
            }
        }
        /// <summary>
        /// Gets and sets the textbox's context menu strip.
        /// </summary>
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return txtDate.ContextMenuStrip;
            }
            set
            {
                txtDate.ContextMenuStrip = value;
            }
        }
        /// <summary>
        /// Gets and sets the textbox's selection start.
        /// </summary>
        public int SelectionStart
        {
            get
            {
                return txtDate.SelectionStart;
            }
            set
            {
                txtDate.SelectionStart = value;
            }
        }
        /// <summary>
        /// Gets and sets the textbox's selection length.
        /// </summary>
        public int SelectionLength
        {
            get
            {
                return txtDate.SelectionLength;
            }
            set
            {
                txtDate.SelectionLength = value;
            }
        }
        /// <summary>
        /// Gets and sets the maximum date value allowed.
        /// </summary>
        public DateTime MaxDate
        {
            get { return m_dteMaxDate; }
            set { m_dteMaxDate = value; }
        }
        /// <summary>
        /// Gets and sets the minimum date value allowed.
        /// </summary>
        public DateTime MinDate
        {
            get { return m_dteMinDate; }
            set { m_dteMinDate = value; }
        }
        /// <summary>
        /// Gets to check if the value in the date textbox is a valid date.
        /// </summary>
        public IsDateValidResult ValidDate
        {
            get
            {
                return IsDateValid(txtDate.Text);
            }
        }
        //-----------------------------------------------------------------------------
        public delegate void ValueChangedEventHandler(Object sender, EventArgs e);
        /// <summary>
        /// Fired when the date and/or time value changes.
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// Fires ValueChanged Event when the date and/or time value changes.
        /// </summary>
        protected virtual void OnValueChanged(EventArgs eventargs)
        {
            if (ValueChanged != null)
                ValueChanged(this, eventargs);
        }
        //-----------------------------------------------------------------------------
        #endregion

        #region Constructor/Setup
        public DateTimePickerAdvanced()
        {
            InitializeComponent();
            //m_szCombinedEntryFormat = m_szDateEntryFormat + m_szTimeEntryFormat;
            m_rectButton = btnCalendar.ClientRectangle;

            m_ctlCalendar = new MonthCalendar();
            m_ctlCalendar.MaxSelectionCount = 1;
            m_wndCalendar = new PopupWindow(m_ctlCalendar);

            btnCalendar.Paint += btnCalendar_Paint;
            btnCalendar.MouseMove += btnCalendar_MouseMove;
            btnCalendar.MouseLeave += btnCalendar_MouseLeave;

            btnCalendar.Click += btnCalendar_Click;
            txtDate.KeyDown += txtDate_KeyDown;
            txtDate.KeyUp += TxtDate_KeyUp;
            m_wndCalendar.Closed += m_wndCalendar_Closed;
            m_ctlCalendar.DateChanged += m_ctlCalendar_DateChanged;
            m_ctlCalendar.DateSelected += m_ctlCalendar_DateSelected;
            txtDate.Resize += TxtDate_Resize;

            txtDate.KeyPress += txtDate_KeyPress;

            ValidateDateFormat();
            ValidateTimeFormat();
            SetDateTimeEntryPattern();
        }


        private void TxtDate_Resize(object sender, EventArgs e)
        {
            btnCalendar.Height = txtDate.Height;
            this.Height = txtDate.Height;
        }

        private void ValidateDateFormat()
        {
            if (string.IsNullOrEmpty(m_szDateDisplayFormat))
            {
                m_szDateEntryFormat = m_szDateDisplayFormat;
                m_bValidDateFormat = true;
                return;
            }
            //Get rid of date format characters that we don't support.
            m_szDateDisplayFormat = GBLMethods.StripText(m_szDateDisplayFormat, NOSUPPORTDATECHARS + "hHmst");
            bool bValid = GBLMethods.ValidateFormatString(m_szDateDisplayFormat);
            if (bValid)
            {
                m_szDateEntryFormat = m_szDateDisplayFormat;

                //Non-numeric date format strings dddd and MMMM--> becomes MM/dd/yyyy string.
                if (m_szDateEntryFormat.Contains("dddd") || m_szDateEntryFormat.Contains("MMMM"))
                    m_szDateEntryFormat = "MM/dd/yyyy";

                ScrubFormatSegment(ref m_szDateEntryFormat, "MM");
                ScrubFormatSegment(ref m_szDateEntryFormat, "dd");
                ScrubFormatSegment(ref m_szDateEntryFormat, "yyyy");

                m_bValidDateFormat = true;
            }
            else
            {
                m_bValidDateFormat = false;
                m_szDateEntryFormat = "";
                MessageBox.Show("Invalid date format string", "Invalid Format String", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void ValidateTimeFormat()
        {
            if (string.IsNullOrEmpty(m_szTimeDisplayFormat))
            {
                m_szTimeEntryFormat = m_szTimeDisplayFormat;
                m_bValidTimeFormat = true;
                return;
            }
            //Get rid of time format characters that we don't support.
            m_szTimeDisplayFormat = GBLMethods.StripText(m_szTimeDisplayFormat, NOSUPPORTDATECHARS + "Mdy");
            bool bValid = GBLMethods.ValidateFormatString(m_szTimeDisplayFormat);
            if (bValid)
            {
                m_szTimeEntryFormat = m_szTimeDisplayFormat;
                ScrubFormatSegment(ref m_szTimeEntryFormat, "hh");
                ScrubFormatSegment(ref m_szTimeEntryFormat, "HH");
                ScrubFormatSegment(ref m_szTimeEntryFormat, "mm");
                ScrubFormatSegment(ref m_szTimeEntryFormat, "ss");
                ScrubFormatSegment(ref m_szTimeEntryFormat, "tt");

                m_bValidTimeFormat = true;
            }
            else
            {
                m_bValidTimeFormat = false;
                m_szTimeEntryFormat = "";
                MessageBox.Show("Invalid time format string", "Invalid Format String", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void ScrubFormatSegment(ref string szFormat, string szFormatSegment)
        {
            int nFirstSegIndex = -1;
            int nLastSegIndex = -1;
            GetSegmentFirstLastPosition(szFormat, szFormatSegment[0], out nFirstSegIndex, out nLastSegIndex);

            if (nFirstSegIndex < 0)
                //Format segment doesn't exist in Format--we're done.
                return;

            if ((nLastSegIndex - nFirstSegIndex) + 1 == szFormatSegment.Length)
                //Format's segment is the right length--we're done.
                return;

            //Format has too many or too few chars in segment.  Cut out the old segment and replace it with szFormatSegment.
            szFormat = GBLMethods.LeftStr(szFormat, nFirstSegIndex) 
                + szFormatSegment 
                + GBLMethods.RightStr(szFormat, (szFormat.Length - nLastSegIndex) - 1);
        }
        private void GetSegmentFirstLastPosition(string szFormat, char cSegmentChar, out int nFirstSegmentIndex, out int nLastSegmentIndex)
        {
            nFirstSegmentIndex = szFormat.IndexOf(cSegmentChar);
            nLastSegmentIndex = szFormat.LastIndexOf(cSegmentChar, szFormat.Length - 1);
        }
        private void SetDateTimeEntryPattern()
        {
            if (!m_bValidDateFormat || !m_bValidTimeFormat)
            {
                m_szCombinedDisplayFormat = m_szCombinedEntryFormat = m_szDateTimeEntryPattern = txtDate.Text = "";
                ResizeControl(false);
                return;
            }
            if (string.IsNullOrEmpty(m_szDateEntryFormat) && string.IsNullOrEmpty(m_szTimeEntryFormat))
            {
                m_szCombinedDisplayFormat = m_szCombinedEntryFormat = m_szDateTimeEntryPattern = txtDate.Text = "";
                ResizeControl(false);
                return;
            }
            m_szCombinedEntryFormat = m_szDateEntryFormat + " " + m_szTimeEntryFormat;
            m_szCombinedEntryFormat = m_szCombinedEntryFormat.Trim();

            m_szCombinedDisplayFormat = m_szDateDisplayFormat + " " + m_szTimeDisplayFormat;
            m_szCombinedDisplayFormat = m_szCombinedDisplayFormat.Trim();

            string szNullDate = m_szCombinedEntryFormat;
            string szSegments = "MdyHhms";
            foreach (char cSegChar in szSegments)
                szNullDate = szNullDate.Replace(cSegChar, '0');

            szNullDate = szNullDate.Replace("tt", "AM");

            m_szDateTimeEntryPattern = szNullDate;
            if (DesignMode)
                if (txtDate.TextLength <= 0)
                    txtDate.Text = m_szDateTimeEntryPattern;

            bool bShowButton = !string.IsNullOrEmpty(m_szDateEntryFormat);
            ResizeControl(bShowButton);
        }
        private void ResizeControl(bool bShowButton)
        {
            btnCalendar.Visible = bShowButton;

            if (bShowButton)
            {
                txtDate.Width = (this.Width - btnCalendar.Width);
                btnCalendar.Left = txtDate.Width;
            }
            else
                txtDate.Width = this.Width;
        }
        protected override void OnEnter(EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDate.Text))
                txtDate.Text = m_szDateTimeEntryPattern;
            else
                ReformatDateControl(m_szCombinedEntryFormat);

            base.OnEnter(e);
        }
        protected override void OnLeave(EventArgs e)
        {
            if (txtDate.Text == m_szDateTimeEntryPattern)
                txtDate.Text = "";
            else
                ReformatDateControl(m_szCombinedDisplayFormat);

            base.OnLeave(e);
        }
        private void ReformatDateControl(string szCombinedFormat)
        {
            if (this.ValidDate == IsDateValidResult.Valid)
                if (m_bValidDateFormat && m_bValidTimeFormat && txtDate.TextLength > 0 && txtDate.Text != m_szDateTimeEntryPattern)
                    txtDate.Text = GBLMethods.FormatDate(GBLMethods.CDate(txtDate.Text, szCombinedFormat), szCombinedFormat);
        }
        #endregion

        #region Drop Down Button
        void btnCalendar_Paint(object sender, PaintEventArgs e)
        {
            if (!ComboBoxRenderer.IsSupported)
            {
                Image nimage = Properties.Resources.dropdown_7;
                int imwidth = Convert.ToInt32(nimage.Width * GraphicUtils.DPIScale);
                int imheight = Convert.ToInt32(nimage.Height * GraphicUtils.DPIScale);
                Rectangle destination = btnCalendar.ClientRectangle;
                int posx = destination.Left + (destination.Width - imwidth) / 2;
                int posy = destination.Top + (destination.Height - imheight) / 2;
                destination = new Rectangle(posx, posy, imwidth, imheight);
                e.Graphics.DrawImage(nimage,destination,new Rectangle(0,0,nimage.Width,nimage.Height), GraphicsUnit.Pixel);
                return;
            }

            m_rectButton = btnCalendar.ClientRectangle;
            ComboBoxRenderer.DrawDropDownButton(e.Graphics, m_rectButton, m_eButtonState);
        }
        void btnCalendar_MouseMove(object sender, MouseEventArgs e)
        {
            m_eButtonState = System.Windows.Forms.VisualStyles.ComboBoxState.Normal;

            if (m_rectButton.Contains(e.Location))
                m_eButtonState = System.Windows.Forms.VisualStyles.ComboBoxState.Hot;

            if (!this.Enabled)
                m_eButtonState = System.Windows.Forms.VisualStyles.ComboBoxState.Disabled;

            btnCalendar.Invalidate();
        }
        void btnCalendar_MouseLeave(object sender, EventArgs e)
        {
            m_eButtonState = System.Windows.Forms.VisualStyles.ComboBoxState.Normal;

            if (!this.Enabled)
                m_eButtonState = System.Windows.Forms.VisualStyles.ComboBoxState.Disabled;

            btnCalendar.Invalidate();
        }
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (this.Enabled)
                m_eButtonState = System.Windows.Forms.VisualStyles.ComboBoxState.Normal;
            else
                m_eButtonState = System.Windows.Forms.VisualStyles.ComboBoxState.Disabled;

            btnCalendar.Enabled = this.Enabled;
        }
        #endregion

        #region Calendar Control
        void m_wndCalendar_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            txtDate.Focus();
        }
        private void TxtDate_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Up))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
            void txtDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (EnterAsTab)
            {
                if (e.KeyCode == Keys.Return)
                { 
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }
            }
            if (e.KeyCode == Keys.F4 && !e.Alt && !e.Control)
            {
                btnCalendar_Click(sender, new EventArgs());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Delete)
            {
                //if (CheckAllSelectedResetControl())
                    // OnValueChanged(new EventArgs());
                txtDate.Text = m_szDateTimeEntryPattern;
                //Value = DateTime.MinValue;
                //OnValueChanged(new EventArgs());

                //else
                //    GBLMethods.MessageBeep(MessageBeepTypes.Default);

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                if (txtDate.SelectionStart > 0)
                {
                    if (txtDate.Text.Length <= txtDate.SelectionStart)
                        txtDate.SelectionStart = txtDate.SelectionStart - 1;
                    
                    char cChar = txtDate.Text[txtDate.SelectionStart];
                    DateSegment nsegment = GetActiveSegment(cChar, txtDate.SelectionStart);
                    if (nsegment != null)
                    {
                        if (nsegment is Reportman.Drawing.Forms.DateSegmentDay)
                        {
                            int day = 0;
                            if (int.TryParse(nsegment.GetCurrentValue(),out day))
                            {
                                day = day - 1;
                                if (day <= 0)
                                    day = 1;
                                nsegment.SetCurrentValue(day);
                                OnValueChanged(new EventArgs());
                            }
                        }
                        else
                        if (nsegment is Reportman.Drawing.Forms.DateSegmentMonth)
                        {
                            int month = 0;
                            if (int.TryParse(nsegment.GetCurrentValue(), out month))
                            {
                                month = month - 1;
                                if (month <= 1)
                                    month = 1;
                                nsegment.SetCurrentValue(month);
                                OnValueChanged(new EventArgs());
                            }
                        }
                        else
                        if (nsegment is Reportman.Drawing.Forms.DateSegmentYear)
                        {
                            int year = 0;
                            if (int.TryParse(nsegment.GetCurrentValue(), out year))
                            {
                                if (year == 0)
                                    year = System.DateTime.Today.Year;
                                else
                                    year = year -1;
                                if (year < 0000)
                                    year = 0000;
                                nsegment.SetCurrentValue(year);
                                OnValueChanged(new EventArgs());
                            }
                        }
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                }

            }
            if (e.KeyCode == Keys.Up)
            {
                if (txtDate.SelectionStart > 0)
                {
                    if (txtDate.Text.Length <= txtDate.SelectionStart)
                        txtDate.SelectionStart = txtDate.SelectionStart - 1;

                    char cChar = txtDate.Text[txtDate.SelectionStart];
                    DateSegment nsegment = GetActiveSegment(cChar, txtDate.SelectionStart);
                    if (nsegment != null)
                    {
                        if (nsegment is Reportman.Drawing.Forms.DateSegmentDay)
                        {
                            int day = 0;
                            if (int.TryParse(nsegment.GetCurrentValue(), out day))
                            {
                                day = day + 1;
                                if (day >= 31)
                                    day = 31;
                                nsegment.SetCurrentValue(day);
                                OnValueChanged(new EventArgs());
                            }
                        }
                        else
                        if (nsegment is Reportman.Drawing.Forms.DateSegmentMonth)
                        {
                            int month = 0;
                            if (int.TryParse(nsegment.GetCurrentValue(), out month))
                            {
                                month = month + 1;
                                if (month >= 12)
                                    month = 12;
                                nsegment.SetCurrentValue(month);
                                OnValueChanged(new EventArgs());
                            }
                        }
                        else
                        if (nsegment is Reportman.Drawing.Forms.DateSegmentYear)
                        {
                            int year = 0;
                            if (int.TryParse(nsegment.GetCurrentValue(), out year))
                            {
                                if (year == 0)
                                    year = System.DateTime.Today.Year;
                                else
                                    year = year + 1;
                                if (year >= 9999)
                                    year  = 9999;
                                nsegment.SetCurrentValue(year);
                                OnValueChanged(new EventArgs());
                            }
                        }
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                }

            }
        }
        void btnCalendar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(m_szDateTimeEntryPattern))
                return;

            m_wndCalendar.ResizeToFit();
            DateTime dteCurrent = DateTime.Today;
            dteCurrent = GBLMethods.CDate(txtDate.Text, m_szCombinedEntryFormat);
            if (dteCurrent.Year < 100)
                dteCurrent = DateTime.Today;
            m_ctlCalendar.SetDate(dteCurrent);
            m_ctlCalendar.MinDate = m_dteMinDate;
            m_ctlCalendar.MaxDate = m_dteMaxDate;

            m_wndCalendar.Show(this, 0, txtDate.Height);
            m_ctlCalendar.Focus();
        }
        void m_ctlCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            m_wndCalendar.Hide();
            txtDate.Focus();
        }

        void m_ctlCalendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            if (string.IsNullOrEmpty(m_szDateTimeEntryPattern))
                return;

            bool bChanged = true;
            if (txtDate.Text != m_szDateTimeEntryPattern)
            {
                DateTime dteCurrent = GBLMethods.CDate(txtDate.Text, m_szCombinedEntryFormat);
                bChanged = (e.Start != dteCurrent);
            }
            if (bChanged)
            {
                txtDate.Text = GBLMethods.FormatDate(e.Start, m_szCombinedEntryFormat);
                OnValueChanged(new EventArgs());
            }
        }
        #endregion

        #region Validation
        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);

            if (txtDate.Text == m_szDateTimeEntryPattern)
                return;

            IsDateValidResult eResult = this.ValidDate;
            if (eResult != IsDateValidResult.Valid)
            {
                e.Cancel = true;
                ShowValFailMsg(eResult);
                txtDate.SelectAll();
            }
        }
        public void ShowValFailMsg(IsDateValidResult eResult)
        {
            string szErrorText = "";
            switch (eResult)
            {
                case IsDateValidResult.Invalid:
                    szErrorText = "You have entered an invalid date.  Please enter a valid date or hit Delete to reset the date to empty.";
                    break;
                case IsDateValidResult.GreaterThanMaxDate:
                    szErrorText = string.Format("You have entered a date that is greater than the maximum allowed date: {0}.  Please enter a valid date before {0}."
                        , m_dteMaxDate.ToShortDateString());
                    break;
                case IsDateValidResult.LessThanMinDate:
                    szErrorText = string.Format("You have entered a date that is less than the minimum allowed date: {0}.  Please enter a valid date after {0}."
                        , m_dteMinDate.ToShortDateString());
                    break;
            }
            MessageBox.Show("Fecha/hora no válida, pulse suprimir para reiniciar o seleccione una fecha/hora", "Fecha no válida", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        public bool IsFocused()
        {
            if (this.txtDate.Focused)
                return true;
            else
                return false;
        }

        protected internal IsDateValidResult IsDateValid(string szDateText)
        {
            if (string.IsNullOrEmpty(szDateText))
                return IsDateValidResult.Valid;

            if (szDateText == m_szDateTimeEntryPattern)
                return IsDateValidResult.Valid;
            DateTime dteDate;
            if (!DateTime.TryParseExact(szDateText, m_szCombinedDisplayFormat, DateFormatInfo, System.Globalization.DateTimeStyles.None, out dteDate))
            {
                if (!DateTime.TryParse(szDateText, out dteDate))
                {
                    //    throw ex;
                    return IsDateValidResult.Invalid;
                }
            }
            //if (!DateTime.TryParse(szDateText, out dteDate))
            //    return IsDateValidResult.Invalid;

            if (dteDate < m_dteMinDate)
                return IsDateValidResult.LessThanMinDate;
            else if (dteDate > m_dteMaxDate)
                return IsDateValidResult.GreaterThanMaxDate;

            return IsDateValidResult.Valid;
        }
        #endregion

        #region Process User Keyed Char
        void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true; //This class will process all keypress characters.
            if (!ProcessChar(e.KeyChar))
                GBLMethods.MessageBeep(MessageBeepTypes.Default);

            OnValueChanged(new EventArgs());

            OnKeyPress(e);
        }
        private bool ProcessChar(char cChar)
        {
            if (string.IsNullOrEmpty(m_szDateTimeEntryPattern))
                return false;

            switch (cChar)
            {
                case ' ':
                    if (CheckAllSelectedResetControl())
                        return true;
                    
                    if (txtDate.SelectionStart >= m_szCombinedEntryFormat.Length)
                        return false;

                    char cSegmentChar = m_szCombinedEntryFormat[txtDate.SelectionStart];
                    int nFirstSegIndex = -1;
                    int nLastSegIndex = -1;
                    GetSegmentFirstLastPosition(m_szCombinedEntryFormat, cSegmentChar, out nFirstSegIndex, out nLastSegIndex);
                    ReplaceDateCharAdvance(m_szDateTimeEntryPattern[txtDate.SelectionStart], nLastSegIndex);

                    return true;
                case '\b':
                    if (CheckAllSelectedResetControl())
                        return true;

                    if (txtDate.SelectionStart > 0)
                    {
                        if (GetActiveSegment(cChar, txtDate.SelectionStart - 1) == null)
                            //We're backspacing in the divider area.  Set selection to go back 1 now.
                            txtDate.SelectionStart--;

                        string szLeft = "";
                        if (txtDate.SelectionStart > 1)
                            szLeft = GBLMethods.LeftStr(txtDate.Text, txtDate.SelectionStart - 1);

                        char cNullChar = m_szDateTimeEntryPattern[txtDate.SelectionStart - 1];
                        string szNewText = szLeft
                            + cNullChar
                            + GBLMethods.RightStr(txtDate.Text, txtDate.TextLength - txtDate.SelectionStart);

                        int nSelStart = txtDate.SelectionStart;
                        txtDate.Text = szNewText;
                        txtDate.SelectionStart = nSelStart - 1;  //MS always resets SelectionStart when you change Text property.
                    }
                    return true;
            }
            DateSegment objDateSegment = GetActiveSegment(cChar);
            if (objDateSegment == null)
                return false;

            return objDateSegment.SegmentProcessChar();
        }
        internal bool CheckAllSelectedResetControl()
        {
            if (txtDate.SelectedText == txtDate.Text)
            {
                txtDate.Text = m_szDateTimeEntryPattern;
                txtDate.SelectionStart = txtDate.SelectionLength = 0;
                return true;
            }
            return false;
        }
        internal DateSegment GetActiveSegment(char cChar)
        {
            return GetActiveSegment(cChar, txtDate.SelectionStart);
        }
        internal DateSegment GetActiveSegment(char cChar, int nStart)
        {
            if (nStart >= m_szCombinedEntryFormat.Length)
                return null;

            char cSegmentChar = m_szCombinedEntryFormat[nStart];
            switch (cSegmentChar)
            {
                case 'M':
                    return GetDateSegment(enumDateSegments.Month, cChar, nStart);
                case 'd':
                    return GetDateSegment(enumDateSegments.Day, cChar, nStart);
                case 'y':
                    return GetDateSegment(enumDateSegments.Year, cChar, nStart);
                case 'h':
                case 'H':
                    return GetDateSegment(enumDateSegments.Hour, cChar, nStart);
                case 'm':
                    return GetDateSegment(enumDateSegments.Minute, cChar, nStart);
                case 's':
                    return GetDateSegment(enumDateSegments.Second, cChar, nStart);
                case 't':
                    return GetDateSegment(enumDateSegments.AMPM, cChar, nStart);
            }
            return null;
        }
        internal DateSegment GetDateSegment(enumDateSegments eSegmentType)
        {
            return GetDateSegment(eSegmentType, 'Z', txtDate.SelectionStart); //This will cause ValNumeric to return false, if the class's client runs SegmentProcessChar.
        }
        internal DateSegment GetDateSegment(enumDateSegments eSegmentType, char cChar, int nStart)
        {
            char cSegmentChar = m_szCombinedEntryFormat[nStart];
            int nFirstSegIndex = 0;
            int nLastSegIndex = 0;
            
            switch (eSegmentType)
            {
                case enumDateSegments.Month:
                    GetSegmentFirstLastPosition(m_szCombinedEntryFormat, 'M', out nFirstSegIndex, out nLastSegIndex);
                    return new DateSegmentMonth(this, cChar, nFirstSegIndex, nLastSegIndex, txtDate, cSegmentChar);
                case enumDateSegments.Day:
                    GetSegmentFirstLastPosition(m_szCombinedEntryFormat, 'd', out nFirstSegIndex, out nLastSegIndex);
                    return new DateSegmentDay(this, cChar, nFirstSegIndex, nLastSegIndex, txtDate, cSegmentChar);
                case enumDateSegments.Year:
                    GetSegmentFirstLastPosition(m_szCombinedEntryFormat, 'y', out nFirstSegIndex, out nLastSegIndex);
                    return new DateSegmentYear(this, cChar, nFirstSegIndex, nLastSegIndex, txtDate, cSegmentChar);
                case enumDateSegments.Hour:
                    GetSegmentFirstLastPosition(m_szCombinedEntryFormat, cSegmentChar, out nFirstSegIndex, out nLastSegIndex);
                    return new DateSegmentHour(this, cChar, nFirstSegIndex, nLastSegIndex, txtDate, cSegmentChar);
                case enumDateSegments.Minute:
                case enumDateSegments.Second:
                     GetSegmentFirstLastPosition(m_szCombinedEntryFormat, cSegmentChar, out nFirstSegIndex, out nLastSegIndex);
                    return new DateSegmentMinuteSecond(this, cChar, nFirstSegIndex, nLastSegIndex, txtDate, cSegmentChar);
                case enumDateSegments.AMPM:
                    GetSegmentFirstLastPosition(m_szCombinedEntryFormat, 't', out nFirstSegIndex, out nLastSegIndex);
                    return new DateSegmentAMPM(this, cChar, nFirstSegIndex, nLastSegIndex, txtDate, cSegmentChar);

            }
            return null;
        }
        internal void ReplaceDateCharAdvance(char cNewChar, int nSegmentEnd)
        {
            string szNewText = GBLMethods.LeftStr(txtDate.Text, txtDate.SelectionStart)
                + cNewChar
                + GBLMethods.RightStr(txtDate.Text, (txtDate.TextLength - txtDate.SelectionStart) - 1);

            int nSelStart = txtDate.SelectionStart;
            txtDate.Text = szNewText;

            txtDate.SelectionStart = nSelStart + 1;
            if (txtDate.SelectionStart > nSegmentEnd)
                if (txtDate.SelectionStart < txtDate.TextLength - 1)
                    txtDate.SelectionStart++;
        }
        #endregion

        #region Miscellaneous
        /// <summary>
        /// Selects all the text in the textbox.
        /// </summary>
        public void SelectAll()
        {
            txtDate.SelectAll();
        }
        #endregion
    }
    public enum IsDateValidResult
    {
        Valid = 0,
        Invalid = 1,
        LessThanMinDate = 2,
        GreaterThanMaxDate = 3,
    }
    internal enum MessageBeepTypes
    {
        Default,
        Hand,
        Question,
        Exclamation,
        Asterick,
    }
    internal class GBLMethods
    {
        internal const Int32
            SM_CXVSCROLL = 2,
            SM_CYHSCROLL = 3;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(int nIndex);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern Boolean MessageBeep(UInt32 messageBeep);

        #region String Conversion Methods

        internal static bool CBool(string szValue)
        {
            szValue = szValue.ToUpper();
            if (szValue == "TRUE")
                return true;
            if (szValue == "FALSE")
                return false;
            if (szValue.Trim().Length == 0)
                return false;
            int nVal = 0;
            int.TryParse(szValue, out nVal);
            return (nVal != 0);
        }
        internal static long CLong(string szValue)
        {
            szValue = szValue.Trim();
            long lReturn = 0;
            long.TryParse(szValue, out lReturn);
            return lReturn;
        }
        internal static uint CUInt(string szValue)
        {
            uint uiReturn = 0;
            szValue = szValue.Trim();
            uint.TryParse(szValue, out uiReturn);
            return uiReturn;
        }
        internal static int CInt(string szValue)
        {
            int nReturn = 0;
            szValue = szValue.Trim();
            int.TryParse(szValue, out nReturn);
            return nReturn;
        }
        internal static decimal CDecimal(string szValue)
        {
            szValue = szValue.Trim();
            decimal dReturn = 0;
            decimal.TryParse(szValue, out dReturn);
            return dReturn;
        }
        internal static double CDouble(string szValue)
        {
            szValue = szValue.Trim();
            double dReturn = 0;
            double.TryParse(szValue, out dReturn);
            return dReturn;
        }
        internal static byte CByte(string szValue)
        {
            szValue = szValue.Trim();
            byte bytReturn = 0;
            byte.TryParse(szValue, out bytReturn);
            return bytReturn;
        }
        internal static DateTime CDate(string szValue)
        {
            return CDate(szValue, "MM/dd/yyyy");
        }
        internal static DateTime CDate(string szValue, string szFormat)
        {
            szValue = szValue.Trim();
            DateTime dteReturn = DateTime.Today;
            //DateTime.TryParse(szValue, out dteReturn);
            try
            {
                if (szValue.Length == 0)
                    return dteReturn;
                if (!DateTime.TryParseExact(szValue,szFormat,null,new System.Globalization.DateTimeStyles(),out dteReturn))
                    DateTime.TryParse(szValue, out dteReturn);
                //dteReturn = DateTime.ParseExact(szValue, szFormat, null);
            }
            catch (Exception)
            {
                DateTime.TryParse(szValue, out dteReturn);
            }

            return dteReturn;
        }
        #endregion

        #region String Methods

        internal static string LeftStr(string param, int length)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            if (string.IsNullOrEmpty(param))
                return "";

            string result = param.Substring(0, length);
            //return the result of the operation
            return result;
        }
        internal static string RightStr(string param, int length)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            if (string.IsNullOrEmpty(param))
                return "";

            int nStart = param.Length - length;
            string result = param.Substring(nStart, length);
            //return the result of the operation
            return result;
        }

        internal static string MidStr(string param, int startIndex, int length)
        {
            //start at the specified index in the string ang get N number of
            //characters depending on the lenght and assign it to a variable
            if (string.IsNullOrEmpty(param))
                return "";

            string result = param.Substring(startIndex, length);
            //return the result of the operation
            return result;
        }

        internal static string MidStr(string param, int startIndex)
        {
            //start at the specified index and return all characters after it
            //and assign it to a variable
            if (string.IsNullOrEmpty(param))
                return "";

            string result = param.Substring(startIndex);
            //return the result of the operation
            return result;
        }
        internal static string StrDup(string szDupString, int nCount)
        {
            StringBuilder objBuilder = new StringBuilder();
            for (int nCounter = 0; nCounter < nCount; nCounter++)
                objBuilder.Append(szDupString);

            return objBuilder.ToString();
        }
        internal static Size GetTextDims(Control ctlControl, string szText)
        {
            return ctlControl.CreateGraphics().MeasureString(szText, ctlControl.Font).ToSize();
        }
        internal static string StripText(string szText, string szStripString)
        {
            if (szText == "")
                return szText;

            string szReturn = szText;
            foreach (char cChar in szStripString)
                szReturn = szReturn.Replace(cChar.ToString(), "");

            return szReturn;
        }
        #endregion


        internal static void MessageBeep(MessageBeepTypes eMessageBeepType)
        {
            UInt32 uiBeepCode = 0xFFFFFFFF;

            switch (eMessageBeepType)
            {
                case MessageBeepTypes.Hand:
                    uiBeepCode = 0x00000010;
                    break;
                case MessageBeepTypes.Question:
                    uiBeepCode = 0x00000020;
                    break;
                case MessageBeepTypes.Exclamation:
                    uiBeepCode = 0x00000030;
                    break;
                case MessageBeepTypes.Asterick:
                    uiBeepCode = 0x00000040;
                    break;
            }
            MessageBeep(uiBeepCode);
        }
        internal static int GetLastDay(int nMonth, int nYear)
        {
            //int nToday = 
            int nLastDay = 0;
            switch (nMonth)
            {
                case 1:	//January
                case 3:	//March
                case 5:	//May
                case 7:	//July
                case 8:	//August
                case 10:	//October
                case 12:	//December
                    nLastDay = 31;
                    break;
                case 4: //April
                case 6: //June
                case 9: //September
                case 11: //November
                    nLastDay = 30;
                    break;
                case 2: //February (Who came up with this kind of a wacky month???)
                    if (nYear % 4 == 0 && nYear % 100 != 0 || nYear % 400 == 0)
                        //Leap Year
                        nLastDay = 29;
                    else
                        nLastDay = 28;
                    break;
            }
            return nLastDay;
        }
        internal static string FormatDate(DateTime dteDate, string szFormat)
        {
            string szReturn = "";
            if (szFormat.Length > 0)
            {
                string szStrFormat = "{0:" + szFormat + "}";
                szReturn = string.Format(szStrFormat, dteDate);
            }

            return szReturn;
        }
        internal static bool ValidateFormatString(string szFormat)
        {
            string szStrFormat = "{0:" + szFormat + "}";
            DateTime dteNow = DateTime.Now;
            string szDate = "";
            try
            {
                szDate = string.Format(szStrFormat, dteNow);
                dteNow = DateTime.ParseExact(szDate, szFormat, null);
            }
            catch (Exception)
            {
                return false;
            }

            //if (!DateTime.TryParse(szDate, out dteNow))
            //    return false;

            return true;
        }
    }
    internal enum enumDateSegments
    {
        Month = 0,
        Day = 1,
        Year = 2,
        Hour = 3,
        Minute = 4,
        Second = 5,
        AMPM = 6,
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    internal class DateSegment
    {
        protected int m_nSegmentStart = 0;
        protected int m_nSegmentEnd = 0;
        protected TextBox m_txtDate = null;
        protected char m_cChar = '0';
        protected DateTimePickerAdvanced m_ctlControl = null;
        protected char m_cFormatChar = 'M';

        public DateSegment(DateTimePickerAdvanced ctlControl, char cChar, int nSegmentStart, int nSegmentEnd, TextBox txtDate, char cFormatChar)
        {
            m_ctlControl = ctlControl;
            m_cChar = cChar;
            m_nSegmentStart = nSegmentStart;
            m_nSegmentEnd = nSegmentEnd;
            m_txtDate = txtDate;
            m_cFormatChar = cFormatChar;
        }
        public virtual bool SegmentProcessChar()
        {
            m_ctlControl.CheckAllSelectedResetControl();
            m_ctlControl.ReplaceDateCharAdvance(m_cChar, m_nSegmentEnd);
            return true;
        }
        protected bool ValNumeric()
        {
            if (m_cChar < '0' || m_cChar > '9')
                return false;

            return true;
        }
        protected string GetNewSegmentText()
        {
            string szOldText = GBLMethods.MidStr(m_txtDate.Text, m_nSegmentStart, (m_nSegmentEnd - m_nSegmentStart) + 1);
            int nCharIndex = m_txtDate.SelectionStart - m_nSegmentStart;
            string szNewValue = GBLMethods.LeftStr(szOldText, nCharIndex)
                + m_cChar
                + GBLMethods.RightStr(szOldText, (szOldText.Length - nCharIndex) - 1);
            return szNewValue;
        }
        public string GetCurrentValue()
        {
            string szValue = "";
            if (m_nSegmentStart >= 0 && m_nSegmentEnd >= 0)
                szValue = GBLMethods.MidStr(m_txtDate.Text, m_nSegmentStart, (m_nSegmentEnd - m_nSegmentStart) + 1);
            return szValue;
        }
        public virtual void SetCurrentValue(int newvalue)
        {
            
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    internal class DateSegmentMonth : DateSegment
    {
        public DateSegmentMonth(DateTimePickerAdvanced ctlControl, char cChar, int nSegmentStart, int nSegmentEnd, TextBox txtDate, char cFormatChar)
            : base(ctlControl, cChar, nSegmentStart, nSegmentEnd, txtDate, cFormatChar)
        {
        }
        public override void SetCurrentValue(int newvalue)
        {
            if ((newvalue>=1) && (newvalue<=12))
            {
                if (m_nSegmentStart >= 0 && m_nSegmentEnd >= 0)
                {
                    string newtext = m_txtDate.Text.Substring(0, m_nSegmentStart);
                    newtext = newtext + newvalue.ToString("00");
                    newtext = newtext + m_txtDate.Text.Substring(m_nSegmentEnd+1,m_txtDate.Text.Length-(m_nSegmentEnd + 1));
                    m_txtDate.Text = newtext;
                                        
                    m_txtDate.SelectionStart = m_nSegmentStart;
                    m_txtDate.SelectionLength = m_nSegmentEnd - m_nSegmentStart +1;
                }
            }
        }
        public override bool SegmentProcessChar()
        {
            if (!ValNumeric())
                return false;

            if (m_txtDate.SelectionStart == m_nSegmentStart)
            {
                if (m_cChar < '0' || m_cChar > '1')
                    return false;
            }
            else
            {
                int nNewValue = GBLMethods.CInt(GetNewSegmentText());
                if (nNewValue > 12)
                    return false;
            }
            return base.SegmentProcessChar();
        }
        public int GetMonthValue()
        {
            return GBLMethods.CInt(GetCurrentValue());
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    internal class DateSegmentDay : DateSegment
    {
        public DateSegmentDay(DateTimePickerAdvanced ctlControl, char cChar, int nSegmentStart, int nSegmentEnd, TextBox txtDate, char cFormatChar)
            : base(ctlControl, cChar, nSegmentStart, nSegmentEnd, txtDate, cFormatChar)
        {
        }
        public override void SetCurrentValue(int newvalue)
        {
            if ((newvalue >= 1) && (newvalue <= 31))
            {
                if (m_nSegmentStart >= 0 && m_nSegmentEnd >= 0)
                {
                    string newtext = m_txtDate.Text.Substring(0, m_nSegmentStart);
                    newtext = newtext + newvalue.ToString("00");
                    newtext = newtext + m_txtDate.Text.Substring(m_nSegmentEnd + 1, m_txtDate.Text.Length - (m_nSegmentEnd + 1));
                    m_txtDate.Text = newtext;

                    m_txtDate.SelectionStart = m_nSegmentStart;
                    m_txtDate.SelectionLength = m_nSegmentEnd - m_nSegmentStart + 1;
                }
            }
        }
        public override bool SegmentProcessChar()
        {
            if (!ValNumeric())
                return false;

            DateSegmentMonth objMonth = m_ctlControl.GetDateSegment(enumDateSegments.Month) as DateSegmentMonth;
            int nMonth = objMonth.GetMonthValue();
            DateSegmentYear objYear = m_ctlControl.GetDateSegment(enumDateSegments.Year) as DateSegmentYear;
            int nYear = objYear.GetYearValue();

            if (m_txtDate.SelectionStart == m_nSegmentStart)
            {
                if (m_cChar < '0' || m_cChar > '3')
                    return false;
                // Allows writtint a 3 even on february later can be modified
                //if (nMonth == 2 && m_cChar == '3')
                //    return false;
            }
            else
            {
                int nNewValue = GBLMethods.CInt(GetNewSegmentText());
                if (nNewValue > 31)
                    return false;

                // Allways allo last day of month to allow change de month later
                //if (nYear > 0 && nMonth > 0)
                //    if (nNewValue > GBLMethods.GetLastDay(nMonth, nYear))
                //        return false;
            }
            return base.SegmentProcessChar();
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    internal class DateSegmentYear : DateSegment
    {
        public DateSegmentYear(DateTimePickerAdvanced ctlControl, char cChar, int nSegmentStart, int nSegmentEnd, TextBox txtDate, char cFormatChar)
            : base(ctlControl, cChar, nSegmentStart, nSegmentEnd, txtDate, cFormatChar)
        {
        }
        public override void SetCurrentValue(int newvalue)
        {
            if ((newvalue >= 0) && (newvalue <= 9999))
            {
                if (m_nSegmentStart >= 0 && m_nSegmentEnd >= 0)
                {
                    string newtext = m_txtDate.Text.Substring(0, m_nSegmentStart);
                    newtext = newtext + newvalue.ToString("0000");
                    newtext = newtext + m_txtDate.Text.Substring(m_nSegmentEnd + 1, m_txtDate.Text.Length - (m_nSegmentEnd + 1));
                    m_txtDate.Text = newtext;

                    m_txtDate.SelectionStart = m_nSegmentStart;
                    m_txtDate.SelectionLength = m_nSegmentEnd - m_nSegmentStart + 1;
                }
            }
        }
        public override bool SegmentProcessChar()
        {
            if (!ValNumeric())
                return false;

            return base.SegmentProcessChar();
        }
        public int GetYearValue()
        {
            return GBLMethods.CInt(GetCurrentValue());
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    internal class DateSegmentHour : DateSegment
    {
        public DateSegmentHour(DateTimePickerAdvanced ctlControl, char cChar, int nSegmentStart, int nSegmentEnd, TextBox txtDate, char cFormatChar)
            : base(ctlControl, cChar, nSegmentStart, nSegmentEnd, txtDate, cFormatChar)
        {
        }
        public override bool SegmentProcessChar()
        {
            if (!ValNumeric())
                return false;

            int nNewHour = GBLMethods.CInt(GetNewSegmentText());
            if (m_cFormatChar == 'h')
            {
                if (m_txtDate.SelectionStart == m_nSegmentStart)
                {
                    if (m_cChar < '0' || m_cChar > '1')
                        return false;
                }
                else
                    if (nNewHour > 12)
                    return false;
            }
            else
            {
                if (m_txtDate.SelectionStart == m_nSegmentStart)
                {
                    if (m_cChar < '0' || m_cChar > '2')
                        return false;
                }
                else
                    if (nNewHour > 23)
                    return false;
            }
            return base.SegmentProcessChar();
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    internal class DateSegmentMinuteSecond : DateSegment
    {
        public DateSegmentMinuteSecond(DateTimePickerAdvanced ctlControl, char cChar, int nSegmentStart, int nSegmentEnd, TextBox txtDate, char cFormatChar)
            : base(ctlControl, cChar, nSegmentStart, nSegmentEnd, txtDate, cFormatChar)
        {
        }
        public override bool SegmentProcessChar()
        {
            if (!ValNumeric())
                return false;

            int nNewValue = GBLMethods.CInt(GetNewSegmentText());
            if (nNewValue > 59)
                return false;

            return base.SegmentProcessChar();
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    internal class DateSegmentAMPM : DateSegment
    {
        public DateSegmentAMPM(DateTimePickerAdvanced ctlControl, char cChar, int nSegmentStart, int nSegmentEnd, TextBox txtDate, char cFormatChar)
            : base(ctlControl, cChar, nSegmentStart, nSegmentEnd, txtDate, cFormatChar)
        {
        }
        public override bool SegmentProcessChar()
        {
            string szChar = "";
            szChar += m_cChar;
            szChar = szChar.ToUpper();
            m_cChar = szChar[0];

            if (m_txtDate.SelectionStart == m_nSegmentStart)
            {
                if (!(m_cChar == 'A' || m_cChar == 'P'))
                    return false;
            }
            else
                if (m_cChar != 'M')
                return false;

            return base.SegmentProcessChar();
        }
    }
    public class DateDropButton : Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            ControlPaint.DrawComboButton(pevent.Graphics, this.Bounds, ButtonState.Flat);
            base.OnPaint(pevent);
        }
    }
    public class PopupWindow : System.Windows.Forms.ToolStripDropDown
    {
        private System.Windows.Forms.Control _content;
        private System.Windows.Forms.ToolStripControlHost _host;

        public PopupWindow(System.Windows.Forms.Control content)
        {
            //Basic setup...
            this.AutoSize = true;
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;

            this._content = content;
            this._host = new System.Windows.Forms.ToolStripControlHost(content);

            //Positioning and Sizing
            this.Padding = Padding.Empty;
            this.Margin = Padding.Empty;

            //this.MinimumSize = content.MinimumSize;
            //this.MaximumSize = content.Size;
            this.Size = content.Size;
            content.Location = new Point(0, 0);

            //Add the host to the list
            this.Items.Add(this._host);
        }
        public void ResizeToFit()
        {
            this.Size = _content.Size;
        }
    }
}
