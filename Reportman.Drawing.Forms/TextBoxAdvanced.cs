using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using Reportman.Drawing;

namespace Reportman.Drawing.Forms
{
    public enum TextBoxDataType { Text, Integer, Numeric, Double };
    public class TextBoxAdvanced : TextBox, IMessageFilter
    {
        public List<AutoCompleteInfo> AutoCompleteListTop;
        static SortedList<char, char> validnumeric;
        static SortedList<char, char> validinteger;
        static SortedList<char, char> validdouble;
        private Control ComboParentForm; // Or use type "Form" 
        private ListBox listBoxChild;
        private Control searchChild;
        private int IgnoreTextChange;
        private bool MsgFilterActive = false;
        public int AutoCompleteWidth = 0;
        public List<KeyValuePair<string, string>> AutoCompleteList;
        public int AutoCompleteMaxVisibleLines = 20;
        private bool FIncludeGoogleTerms;
        public bool IncludeGoogleTerms
        {
            set
            {
                FIncludeGoogleTerms = value;
                if (value)
                {
                    if (GoogleTermsImage == null)
                        GoogleTermsImage = Properties.Resources.search16;
                    if (NotGoogleTermsImage == null)
                        NotGoogleTermsImage = Properties.Resources.search16;
                }
            }
            get
            {
                return FIncludeGoogleTerms;
            }
        }
        public Image GoogleTermsImage;
        public Image NotGoogleTermsImage;
        public ISearchWindow SearchWindow;
        static char decimalsep;
        protected string _waterMarkText = ""; //The watermark text
        protected Color _waterMarkColor; //Color of the watermark when the control does not have focus
        protected Color _waterMarkActiveColor; //Color of the watermark when the control has focus

        private Panel waterMarkContainer; //Container to hold the watermark
        private Font waterMarkFont; //Font of the watermark
        private SolidBrush waterMarkBrush; //Brush for the watermark
        public StringEvent OnDropDownClicked;
        static TextBoxAdvanced()
        {
            decimalsep = (char)0;
            validnumeric = new SortedList<char, char>();
            validdouble = new SortedList<char, char>();

            string defsep = Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
            validnumeric.Add('\n', '\n');
            validnumeric.Add('0', '0');
            validnumeric.Add('1', '1');
            validnumeric.Add('2', '2');
            validnumeric.Add('3', '3');
            validnumeric.Add('4', '4');
            validnumeric.Add('5', '5');
            validnumeric.Add('6', '6');
            validnumeric.Add('7', '7');
            validnumeric.Add('8', '8');
            validnumeric.Add('9', '9');
            validnumeric.Add('\b', '\b');
            validnumeric.Add('-', '-');

            validinteger = new SortedList<char, char>();
            foreach (char c1 in validnumeric.Keys)
                validinteger.Add(c1, c1);
            foreach (char c in defsep)
            {
                if (decimalsep == (char)0)
                    decimalsep = c;
                validnumeric.Add(c, c);
            }

            foreach (char c2 in validnumeric.Keys)
                validdouble.Add(c2, c2);
            validdouble.Add('e', 'e');

            // Control-X
            validdouble.Add((char)24, (char)24);
            validnumeric.Add((char)24, (char)24);
            validinteger.Add((char)24, (char)24);

            // Control->Z
            validdouble.Add((char)26, (char)26);
            validnumeric.Add((char)26, (char)26);
            validinteger.Add((char)26, (char)26);

            // Control->C
            validdouble.Add((char)3, (char)3);
            validnumeric.Add((char)3, (char)3);
            validinteger.Add((char)3, (char)3);

        }

        public TextBoxAdvanced()
            : base()
        {
            FBarCodeBeginChar = '$';
            FBarCodeEndChar = '%';




            //Sets some default values to the watermark properties
            _waterMarkColor = Color.LightGray;
            _waterMarkActiveColor = Color.Gray;
            waterMarkFont = this.Font;
            waterMarkBrush = new SolidBrush(_waterMarkActiveColor);
            waterMarkContainer = null;

            //Draw the watermark, so we can see it in design time
            DrawWaterMark();

            //Eventhandlers which contains function calls. 
            //Either to draw or to remove the watermark
            this.Enter += new EventHandler(ThisHasFocus);
            this.Leave += new EventHandler(ThisWasLeaved);
            this.TextChanged += new EventHandler(ThisTextChanged);

        }
        public object SelectedDropDown
        {
            get
            {

                if (listBoxChild == null)
                    return null;
                if (listBoxChild.Visible)
                    return null;
                if (listBoxChild.SelectedIndex < 0)
                    return null;
                return listBoxChild.Items[listBoxChild.SelectedIndex];
            }
        }        /// <summary>
                 /// Removes the watermark if it should
                 /// </summary>
        private void RemoveWaterMark()
        {
            if (waterMarkContainer != null)
            {
                this.Controls.Remove(waterMarkContainer);
                waterMarkContainer = null;
            }
        }
        /// <summary>
        /// Draws the watermark if the text length is 0
        /// </summary>
        private void DrawWaterMark()
        {
            if ((this.waterMarkContainer == null) && (this.TextLength <= 0) && (_waterMarkText != null) && (_waterMarkText.Length > 0))
            {
                waterMarkContainer = new Panel(); // Creates the new panel instance
                waterMarkContainer.Paint += new PaintEventHandler(WaterMarkContainer_Paint);
                waterMarkContainer.Invalidate();
                waterMarkContainer.Click += new EventHandler(WaterMarkContainer_Click);
                this.Controls.Add(waterMarkContainer); // adds the control
            }
        }

        private void WaterMarkContainer_Paint(object sender, PaintEventArgs e)
        {
            //Setting the watermark container up
            waterMarkContainer.Location = new Point(2, 0); // sets the location
            waterMarkContainer.Height = this.Height; // Height should be the same as its parent
            waterMarkContainer.Width = this.Width; // same goes for width and the parent
            waterMarkContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right; // makes sure that it resizes with the parent control



            if (this.ContainsFocus)
            {
                //if focused use normal color
                waterMarkBrush = new SolidBrush(this._waterMarkActiveColor);
            }

            else
            {
                //if not focused use not active color
                waterMarkBrush = new SolidBrush(this._waterMarkColor);
            }

            //Drawing the string into the panel 
            Graphics g = e.Graphics;
            g.DrawString(this._waterMarkText, waterMarkFont, waterMarkBrush, new PointF(-2f, 1f));//Take a look at that point
            //The reason I'm using the panel at all, is because of this feature, that it has no limits
            //I started out with a label but that looked very very bad because of its paddings 

        }
        private void ThisHasFocus(object sender, EventArgs e)
        {
            //if focused use focus color
            waterMarkBrush = new SolidBrush(this._waterMarkActiveColor);

            //The watermark should not be drawn if the user has already written some text
            if (this.TextLength <= 0)
            {
                RemoveWaterMark();
                DrawWaterMark();
            }
        }

        private void ThisWasLeaved(object sender, EventArgs e)
        {
            //if the user has written something and left the control
            if (this.TextLength > 0)
            {
                //Remove the watermark
                RemoveWaterMark();
            }
            else
            {
                //But if the user didn't write anything, Then redraw the control.
                this.Invalidate();
            }
        }

        private void ThisTextChanged(object sender, EventArgs e)
        {
            //If the text of the textbox is not empty
            if (this.TextLength > 0)
            {
                //Remove the watermark
                RemoveWaterMark();
            }
            else
            {
                //But if the text is empty, draw the watermark again.
                DrawWaterMark();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //Draw the watermark even in design time
            DrawWaterMark();
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            //Check if there is a watermark
            if (waterMarkContainer != null)
                //if there is a watermark it should also be invalidated();
                waterMarkContainer.Invalidate();
        }

        private void WaterMarkContainer_Click(object sender, EventArgs e)
        {
            this.Focus(); //Makes sure you can click wherever you want on the control to gain focus
        }
        [Category("Watermark attribtues")]
        [Description("Sets the text of the watermark")]
        public string WaterMark
        {
            get { return this._waterMarkText; }
            set
            {
                this._waterMarkText = value;
                DrawWaterMark();
                this.Invalidate();
            }
        }

        [Category("Watermark attribtues")]
        [Description("When the control gaines focus, this color will be used as the watermark's forecolor")]
        public Color WaterMarkActiveForeColor
        {
            get { return this._waterMarkActiveColor; }

            set
            {
                this._waterMarkActiveColor = value;
                this.Invalidate();
            }
        }

        [Category("Watermark attribtues")]
        [Description("When the control looses focus, this color will be used as the watermark's forecolor")]
        public Color WaterMarkForeColor
        {
            get { return this._waterMarkColor; }

            set
            {
                this._waterMarkColor = value;
                this.Invalidate();
            }
        }

        [Category("Watermark attribtues")]
        [Description("The font used on the watermark. Default is the same as the control")]
        public Font WaterMarkFont
        {
            get
            {
                return this.waterMarkFont;
            }

            set
            {
                this.waterMarkFont = value;
                this.Invalidate();
            }
        }


        private TextBoxDataType FDataType;
        public TextBoxDataType DataType
        {
            get { return FDataType; }
            set
            {
                FDataType = value;
            }
        }
        private bool FReadBarCode;
        public bool ReadBarCode
        {
            get { return FReadBarCode; }
            set
            {
                FReadBarCode = value;
            }
        }
        protected override void OnLostFocus(EventArgs e)
        {
            if (listBoxChild != null && !listBoxChild.Focused)
                HideTheList();
            if (SearchWindow != null)
                SearchWindow.Deactivate();
            base.OnLostFocus(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            HideTheList();

            base.OnMouseDown(e);
        }
        private char FBarCodeBeginChar;
        public char BarCodeBeginChar
        {
            get { return FBarCodeBeginChar; }
            set
            {
                FBarCodeBeginChar = value;
            }
        }
        public bool PreFilterMessage(ref Message m)
        {
            if (!Focused)
                return false;
            if (m.Msg == 0x201) // Mouse click: WM_LBUTTONDOWN
            {
                var Pos = new Point((int)(m.LParam.ToInt32() & 0xFFFF), (int)(m.LParam.ToInt32() >> 16));

                var Ctrl = Control.FromHandle(m.HWnd);
                if (Ctrl != null)
                {
                    Point screenPoint = Ctrl.PointToScreen(Pos);
                    // Convert the point into our parent control's coordinates ...
                    Pos = ComboParentForm.PointToClient(screenPoint);

                    // ... because we need to hide the list if user clicks on something other than the list-box
                    if (ComboParentForm != null)
                    {
                        if (listBoxChild != null)
                        {
                            if (listBoxChild.Visible)
                            {
                                if (Pos.X < listBoxChild.Left || Pos.X > listBoxChild.Right || Pos.Y < listBoxChild.Top || Pos.Y > listBoxChild.Bottom)
                                {
                                    this.HideTheList();
                                }
                            }
                        }
                        if (searchChild != null)
                        {
                            if (searchChild.Visible)
                            {
                                if (Pos.X < searchChild.Left || Pos.X > searchChild.Right || Pos.Y < searchChild.Top || Pos.Y > searchChild.Bottom)
                                {
                                    SearchWindow.Deactivate();
                                }
                                else
                                {
                                    return SearchWindow.Click(searchChild.PointToClient(screenPoint));
                                }
                            }
                        }
                    }
                }
            }
            else
            if (m.Msg == 0x020A)
            {
                // Mouse wheel
                if (listBoxChild != null && listBoxChild.Visible)
                {
                    long wheelcode = m.WParam.ToInt64();
                    wheelcode = wheelcode >> 16;
                    long wheels = wheelcode / 120;
                    long incrementtodo = -wheelcode / 120;
                    long NewIx = listBoxChild.SelectedIndex + incrementtodo;
                    if (NewIx < 0)
                        NewIx = 0;
                    if (NewIx > listBoxChild.Items.Count - 1)
                        NewIx = listBoxChild.Items.Count - 1;
                    // Keep the index valid!
                    if (NewIx >= 0)
                        listBoxChild.SelectedIndex = Convert.ToInt32(NewIx);
                    return true;
                }
                if (searchChild != null && searchChild.Visible)
                {
                    long wheelcode = m.WParam.ToInt64();
                    wheelcode = wheelcode >> 16;
                    long wheels = wheelcode / 120;
                    long incrementtodo = -wheelcode / 120;
                    if (incrementtodo > 0)
                        SearchWindow.KeyOperation(SearchWindowKeyOperation.Down);
                    else
                        SearchWindow.KeyOperation(SearchWindowKeyOperation.Up);
                    return true;
                }
            }
            else
            if ((m.Msg == 0x100) || (m.Msg == 0x101)) // WM_KEYDOWN WM_KEYUP
            {
                if (listBoxChild != null && listBoxChild.Visible)
                {
                    int keycode = m.WParam.ToInt32();
                    switch (keycode)
                    {
                        case 0x1B: // Escape key
                            if (m.Msg == 0x100)
                                this.HideTheList();
                            return true;
                        case 34: // Page down
                        case 33: // Page up
                        case 0x26: // up key
                        case 0x28: // right key
                                   // Change selection
                            if (m.Msg == 0x100)
                            {
                                int increment = 1;
                                if (keycode == 0x26)
                                    increment = -1;
                                else
                                    if (keycode == 34)
                                    increment = Convert.ToInt32(listBoxChild.Height / listBoxChild.ItemHeight);
                                else
                                    if (keycode == 33)
                                    increment = -Convert.ToInt32(listBoxChild.Height / listBoxChild.ItemHeight);

                                int NewIx = listBoxChild.SelectedIndex + increment;
                                if (NewIx < 0)
                                    NewIx = 0;
                                if (NewIx > listBoxChild.Items.Count - 1)
                                    NewIx = listBoxChild.Items.Count - 1;
                                // Keep the index valid!
                                if (NewIx >= 0)
                                    listBoxChild.SelectedIndex = NewIx;
                            }
                            return true;

                        case 0x0D: // return (use the currently selected item)
                            if (m.Msg == 0x100)
                            {
                                CopySelection();
                            }
                            return false;
                    }
                }
                if (searchChild != null && searchChild.Visible)
                {
                    int keycode = m.WParam.ToInt32();
                    switch (keycode)
                    {
                        case 0x1B: // Escape key
                            SearchWindow.Deactivate();
                            return true;
                        case 34: // Page down
                        case 33: // Page up
                        case 0x26: // up key
                        case 0x28: // right key
                                   // Change selection
                            if (m.Msg == 0x100)
                            {
                                if (keycode == 0x26)
                                    SearchWindow.KeyOperation(SearchWindowKeyOperation.Up);
                                else
                                if (keycode == 34)
                                    SearchWindow.KeyOperation(SearchWindowKeyOperation.PageDown);
                                else
                                if (keycode == 33)
                                    SearchWindow.KeyOperation(SearchWindowKeyOperation.PageUp);
                                else
                                    SearchWindow.KeyOperation(SearchWindowKeyOperation.Down);
                            }
                            return true;
                        case 0x0D: // return (use the currently selected item)
                            SearchWindow.KeyOperation(SearchWindowKeyOperation.Return);
                            return false;
                    }
                }
            }

            return false;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (FReadBarCode)
            {
                if (Text.Length > 1)
                {
                    if (Text[0] == FBarCodeBeginChar)
                    {
                        int index = 1;
                        while (index < Text.Length)
                        {
                            if (Text[index] == FBarCodeEndChar)
                            {
                                Text = Text.Substring(1, index - 1);
                                break;
                            }
                            index++;
                        }
                    }
                }
            }
            HandletextChanged(e);
            base.OnTextChanged(e);
        }
        char[] separators = { '-', ' ' };
        private void Webclient_DownloadDataCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            System.Net.WebClient nwebclient = (System.Net.WebClient)sender;
            nwebclient.Dispose();
            if (e.Error != null)
                return;
            if (Text.Trim().Length == 0)
            {
                HideDropDown();
                return;
            }
            System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
            xmldoc.LoadXml(e.Result);



            listBoxChild.BeginUpdate();
            try
            {
                listBoxChild.Items.Clear();

                string SearchText = StringUtil.RemoveDiacritics(Text.Trim().ToUpper());
                List<AutoCompleteInfo> matchestop = GetMatchesTop(SearchText);
                if (matchestop.Count != 0)
                {
                    foreach (AutoCompleteInfo compinfo in matchestop)
                    {
                        listBoxChild.Items.Add(compinfo);
                    }
                }
                foreach (System.Xml.XmlNode node in xmldoc.ChildNodes)
                {

                    foreach (System.Xml.XmlNode completenode in node.ChildNodes)
                    {
                        foreach (System.Xml.XmlNode suggestionnode in completenode.ChildNodes)
                        {
                            listBoxChild.Items.Add(new GoogleSuggestion(suggestionnode.Attributes["data"].Value.ToString()));
                        }

                    }

                }
                int exact_match = -1;
                List<string> matches = GetMatches(SearchText, ref exact_match);
                if (matches.Count != 0)
                {
                    listBoxChild.Items.AddRange(matches.ToArray());
                    if (exact_match >= 0)
                        listBoxChild.SelectedIndex = exact_match;
                }
            }
            finally
            {
                listBoxChild.EndUpdate();
            }
            UpdateListPosition();
        }
        private void SearchGoogleTerms()
        {
            System.Net.WebClient webclient = new System.Net.WebClient();
            webclient.DownloadStringCompleted += Webclient_DownloadDataCompleted;
            string SearchText = Text.Trim();
            if (SearchText.Length > 0)
            {
                string tosearch = System.Uri.EscapeDataString(SearchText);
                string searchuri = "http://suggestqueries.google.com/complete/search?q=" + tosearch + "&client=toolbar&hl=es";
                //webclient.CancelAsync();
                webclient.DownloadStringAsync(new Uri(searchuri));
            }
            else
                HideDropDown();
        }
        private List<AutoCompleteInfo> GetMatchesTop(string pattern)
        {
            List<AutoCompleteInfo> matches = new List<AutoCompleteInfo>();
            if (AutoCompleteListTop == null)
            {
                return matches;
            }
            string[] words = pattern.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int idx = 0;
            foreach (AutoCompleteInfo compinfo in AutoCompleteListTop)
            {
                bool match = true;
                string searchvalue = compinfo.Text.ToUpper() + " " + compinfo.Address.ToUpper();
                foreach (string word in words)
                {
                    if (searchvalue.IndexOf(word) < 0)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    matches.Add(compinfo);
                    idx++;
                }
            }
            return matches;
        }
        private List<string> GetMatches(string pattern, ref int exact_match)
        {
            List<string> matches = new List<string>();
            string[] words = pattern.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int idx = 0;
            foreach (KeyValuePair<string, string> keypair in AutoCompleteList)
            {
                bool match = true;
                foreach (string word in words)
                {
                    if (keypair.Value.IndexOf(word) < 0)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    if (keypair.Value == pattern)
                        exact_match = idx;
                    matches.Add(keypair.Key);
                    idx++;
                }
            }
            return matches;
        }
        System.Windows.Forms.Timer timerlookup;
        private void Timerlookup_Tick(object sender, EventArgs e)
        {
            timerlookup.Enabled = false;
            if (!Focused)
                return;
            HandleTextSearchInt();
        }
        void HandleTextSearchInt()
        {
            if (SearchWindow == null)
                return;
            string SearchText = Text;
            if ((SearchText.Length == 0) || (!SearchText.Contains(" ")))
            {
                SearchWindow.Deactivate();
                return;
            }
            if (!SearchWindow.Enabled)
                return;
            searchChild = SearchWindow.CreateWindow();
            if (searchChild == null)
                return;
            CreateMessageFilter();
            Point PutItHere = new Point(this.Left, this.Bottom);
            Control TheControlToMove = this;

            PutItHere = this.Parent.PointToScreen(PutItHere);

            TheControlToMove = searchChild;
            ComboParentForm = this.Parent;

            if (!(this.Parent is TableLayoutPanel))
            {
                ComboParentForm = FindForm(this);
            }
            if (ComboParentForm == null)
            {
                SearchWindow.Deactivate();
                return;
            }

            PutItHere = ComboParentForm.PointToClient(PutItHere);

            TheControlToMove.Left = PutItHere.X;
            TheControlToMove.Top = PutItHere.Y;
            int newWidth = this.Width;
            int margin = Convert.ToInt32(20 * Reportman.Drawing.GraphicUtils.DPIScale);
            if (AutoCompleteWidth == 0)
            {
                newWidth = ComboParentForm.ClientSize.Width - PutItHere.X - margin;
            }
            else
                newWidth = AutoCompleteWidth;
            TheControlToMove.Width = Math.Min(ComboParentForm.ClientSize.Width - TheControlToMove.Left, newWidth);
            int numitems = AutoCompleteMaxVisibleLines;
            int itemHeight = Convert.ToInt32(25 * Reportman.Drawing.GraphicUtils.DPIScale);
            int TotalItemHeight = itemHeight * numitems;
            int previousheight = TheControlToMove.Height;
            int newheight = Math.Min(ComboParentForm.ClientSize.Height - TheControlToMove.Top - margin, TotalItemHeight);
            if (newheight < previousheight)
            {
                // Try to place above
                int suggested_height = previousheight;
                int new_y = TheControlToMove.Top - this.Height - previousheight;
                if (new_y < 0)
                {
                    suggested_height = suggested_height + new_y;
                    new_y = 0;
                }
                if (suggested_height > newheight)
                {
                    newheight = suggested_height;
                    PutItHere = new Point(TheControlToMove.Left, new_y);
                    TheControlToMove.Top = PutItHere.Y;
                }

            }
            TheControlToMove.Height = newheight;
            TheControlToMove.Parent = ComboParentForm;
            TheControlToMove.BringToFront();
            if (!TheControlToMove.Visible)
            {
                TheControlToMove.Show();
                TheControlToMove.PerformLayout();
            }
            SearchWindow.ChangeSearchString(SearchText);

        }
        void HandleTextSearch()
        {
            if (timerlookup == null)
            {
                timerlookup = new System.Windows.Forms.Timer();
                timerlookup.Interval = 500;
                timerlookup.Tick += Timerlookup_Tick;
            }
            timerlookup.Enabled = false;
            if (SearchWindow != null)
            {
                if (SearchWindow.Enabled)
                {

                    timerlookup.Enabled = true;
                }
            }
        }

        void HandletextChanged(EventArgs e)
        {
            string SearchText = Text;
            if (SearchWindow != null)
            {
                HandleTextSearch();
                return;
            }
            if (AutoCompleteList == null)
                return;
            if (IgnoreTextChange > 0)
            {
                IgnoreTextChange = 0;
                return;
            }

            InitListControl();



            if (listBoxChild == null)
                return;




            // Don't show the list when nothing has been typed
            if (!string.IsNullOrEmpty(SearchText))
            {
                if (IncludeGoogleTerms)
                {
                    SearchGoogleTerms();
                }
                else
                {
                    SearchText = StringUtil.RemoveDiacritics(Text.Trim().ToUpper());
                    int exact_match = -1;
                    List<string> matches = GetMatches(SearchText, ref exact_match);
                    if (matches.Count == 0)
                        listBoxChild.Items.Clear();
                    else
                    {
                        listBoxChild.BeginUpdate();
                        listBoxChild.Items.Clear();
                        listBoxChild.Items.AddRange(matches.ToArray());
                        if (exact_match >= 0)
                            listBoxChild.SelectedIndex = exact_match;
                        listBoxChild.EndUpdate();
                    }
                }
            }
            else
            {
                listBoxChild.Items.Clear();
                HideDropDown();
            }
            if (!IncludeGoogleTerms)
                UpdateListPosition();
        }
        private void UpdateListPosition()
        {
            if (listBoxChild.Items.Count > 0)
            {
                if (listBoxChild.Parent is TableLayoutPanel)
                {
                    int numitems = (listBoxChild.Items.Count + 1);
                    if (numitems > AutoCompleteMaxVisibleLines)
                        numitems = AutoCompleteMaxVisibleLines;
                    int TotalItemHeight = listBoxChild.ItemHeight * numitems;
                    listBoxChild.Height = TotalItemHeight;
                    listBoxChild.Visible = true;
                }
                else
                {
                    Point PutItHere = new Point(this.Left, this.Bottom);
                    Control TheControlToMove = this;

                    PutItHere = this.Parent.PointToScreen(PutItHere);

                    TheControlToMove = listBoxChild;
                    PutItHere = ComboParentForm.PointToClient(PutItHere);

                    TheControlToMove.Left = PutItHere.X;
                    TheControlToMove.Top = PutItHere.Y;
                    int newWidth = this.Width;
                    if (AutoCompleteWidth == 0)
                        newWidth = this.Width;
                    else
                        newWidth = AutoCompleteWidth;
                    TheControlToMove.Width = Math.Min(ComboParentForm.ClientSize.Width - TheControlToMove.Left, newWidth);
                    int numitems = (listBoxChild.Items.Count + 1);
                    if (numitems > AutoCompleteMaxVisibleLines)
                        numitems = AutoCompleteMaxVisibleLines;
                    int TotalItemHeight = listBoxChild.ItemHeight * numitems;
                    TheControlToMove.Height = Math.Min(ComboParentForm.ClientSize.Height - TheControlToMove.Top, TotalItemHeight);
                    if (!TheControlToMove.Visible)
                        TheControlToMove.Show();
                }
            }
            else
                HideTheList();
        }
        private void HideTheList()
        {
            if (listBoxChild != null)
            {
                listBoxChild.Hide();
            }
        }

        private char FBarCodeEndChar;
        public char BarCodeEndChar
        {
            get { return FBarCodeEndChar; }
            set
            {
                FBarCodeEndChar = value;
            }
        }
        private Form FindForm(Control ncontrol)
        {
            if (ncontrol.Parent != null)
            {
                if (ncontrol.Parent is Form)
                    return (Form)ncontrol.Parent;
                else
                    return FindForm(ncontrol.Parent);
            }
            else
                return null;
        }
        private void FreeMessageFilter()
        {
            // Setup a messaage filter so we can listen to the keyboard
            if (MsgFilterActive)
            {
                Application.RemoveMessageFilter(this);
                MsgFilterActive = false;
            }
        }
        protected override void OnLeave(EventArgs e)
        {
            FreeMessageFilter();
            base.OnLeave(e);
        }
        private void CreateMessageFilter()
        {
            // Setup a messaage filter so we can listen to the keyboard
            if (!MsgFilterActive)
            {
                Application.AddMessageFilter(this);
                MsgFilterActive = true;
            }
        }
        void InitListControl()
        {
            if (listBoxChild == null)
            {
                // Find parent - or keep going up until you find the parent form
                ComboParentForm = this.Parent;
                //if (!(this.Parent is TableLayoutPanel))
                {
                    ComboParentForm = FindForm(this);
                }
                if (ComboParentForm != null)
                {
                    CreateMessageFilter();
                    listBoxChild = listBoxChild = new ListBox();
                    listBoxChild.TabStop = false;
                    listBoxChild.Visible = false;
                    if (IncludeGoogleTerms)
                    {
                        listBoxChild.DrawMode = DrawMode.OwnerDrawFixed;
                        listBoxChild.DrawItem += ListBoxChild_DrawItem;
                        listBoxChild.ItemHeight = Reportman.Drawing.GraphicUtils.ScaleToDPI(20);
                        listBoxChild.SelectedIndexChanged += ListBoxChild_SelectedIndexChanged;
                        listBoxChild.MouseMove += ListBoxChild_MouseMove;
                    }
                    listBoxChild.Click += ListBox1_Click;
                    ComboParentForm.Controls.Add(listBoxChild);
                    if (ComboParentForm is TableLayoutPanel)
                    {
                        ((TableLayoutPanel)ComboParentForm).SetRow(listBoxChild, 1);
                        listBoxChild.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
                        listBoxChild.Margin = new Padding(Margin.Left, 0, Margin.Right, 0);
                    }
                    else
                        ComboParentForm.Controls.SetChildIndex(listBoxChild, 0); // Put it at the front
                }
            }
        }
        private void ListBoxChild_MouseMove(object sender, EventArgs e)
        {
            listBoxChild.Invalidate(true);
        }

        private void ListBoxChild_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxChild.Invalidate();
        }
        public void HideDropDown()
        {
            if (listBoxChild != null)
            {
                listBoxChild.Visible = false;
            }
        }
        private void ListBoxChild_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            Brush drawbrush = null;

            bool selected = false;
            if (e.Index == listBoxChild.SelectedIndex)
                selected = true;
            Point currentpos = Cursor.Position;
            bool mouseover = false;
            currentpos = listBoxChild.PointToClient(currentpos);
            if (e.Bounds.Contains(currentpos))
                mouseover = true;
            if ((mouseover) && (!selected))
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(204, 229, 255)), e.Bounds);
                drawbrush = new SolidBrush(ForeColor);
            }
            else
            {
                if (selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
                    drawbrush = new SolidBrush(SystemColors.HighlightText);
                }
                else
                {
                    drawbrush = new SolidBrush(ForeColor);
                }
            }
            StringFormat format = new StringFormat(StringFormatFlags.NoWrap);
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            int offset = GraphicUtils.ScaleToDPI(18);
            int imwidth = GraphicUtils.ScaleToDPI(16);
            Rectangle StringBounds = new Rectangle(e.Bounds.Left + offset, e.Bounds.Top, e.Bounds.Width - offset, e.Bounds.Height);
            Image imageToDraw = null;
            if (listBoxChild.Items[e.Index] is GoogleSuggestion)
                imageToDraw = GoogleTermsImage;
            else
                if (listBoxChild.Items[e.Index] is AutoCompleteInfo)
            {
                AutoCompleteInfo compinfo = (AutoCompleteInfo)listBoxChild.Items[e.Index];
                imageToDraw = compinfo.Icon;
            }
            else
                imageToDraw = NotGoogleTermsImage;

            e.Graphics.DrawString(listBoxChild.Items[e.Index].ToString(), Font, drawbrush, StringBounds, format);
            int imoffset = (e.Bounds.Height - imwidth) / 2;

            e.Graphics.DrawImage(imageToDraw, new Rectangle(e.Bounds.Left, e.Bounds.Top + imoffset, imwidth, imwidth));
        }


        /// <summary>
        /// Copy the selection from the list-box into the combo box
        /// </summary>
        private void CopySelection()
        {
            if (listBoxChild.SelectedItem != null)
            {
                //this.SelectedItem = listBoxChild.SelectedItem;
                Text = listBoxChild.SelectedItem.ToString();
                HideTheList();
                this.SelectAll();
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            if (MsgFilterActive)
                Application.RemoveMessageFilter(this);

            base.OnHandleCreated(e);
        }
        private void ListBox1_Click(object sender, EventArgs e)
        {
            var ThisList = sender as ListBox;

            if (ThisList != null)
            {
                if (OnDropDownClicked == null)
                {                // Copy selection to the combo box
                    CopySelection();
                    this.Focus();
                    SendKeys.Send("{Enter}");
                }
                else
                {
                    object newitem = "";
                    if (ThisList.SelectedIndex >= 0)
                        newitem = ThisList.Items[ThisList.SelectedIndex];
                    OnDropDownClicked(this, newitem);
                }
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
            switch (FDataType)
            {
                case TextBoxDataType.Numeric:
                case TextBoxDataType.Double:
                    if (e.KeyCode == Keys.Decimal)
                    {
                        // No more than one comma admitted
                        string defsep = Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
                        if (defsep.Length > 0)
                        {
                            if (defsep[0] != '.')
                            {
                                SendKeys.Send(defsep);
                                e.SuppressKeyPress = true;
                            }
                        }
                    }
                    break;
            }

            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            int index;
            switch (FDataType)
            {
                case TextBoxDataType.Numeric:
                case TextBoxDataType.Double:
                case TextBoxDataType.Integer:
                    if (e.KeyChar != (char)22)
                    {

                        SortedList<char, char> lvalidate;
                        if (FDataType == TextBoxDataType.Numeric)
                            lvalidate = validnumeric;
                        else
                            if (FDataType == TextBoxDataType.Double)
                            lvalidate = validdouble;
                        else
                            lvalidate = validinteger;
                        index = lvalidate.IndexOfKey(e.KeyChar);
                        if (index < 0)
                            e.KeyChar = (char)0;
                        // only 1 - symbol
                        if (e.KeyChar == '-')
                        {
                            // only at the beginning
                            if (SelectionStart <= 0)
                            {
                                index = Text.IndexOf('-');
                                if (index >= 0)
                                    if (SelectionStart + SelectionLength < index)
                                        e.KeyChar = (char)0;
                            }
                            else
                                e.KeyChar = (char)0;
                        }
                        // only 1 e symbol
                        if (e.KeyChar == 'e')
                        {
                            index = Text.IndexOf('-');
                            if (index >= 0)
                                e.KeyChar = (char)0;
                        }
                        // only 1 decimalsep symbol
                        if (decimalsep != (char)0)
                        {
                            if (e.KeyChar == decimalsep)
                            {
                                index = Text.IndexOf(decimalsep);
                                if (index >= 0)
                                    e.KeyChar = (char)0;
                            }
                        }
                    }
                    break;

            }
            base.OnKeyPress(e);
        }

    }
}
