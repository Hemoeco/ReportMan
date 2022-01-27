using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Reportman.Drawing.Forms
{
    public delegate void StringEvent(object sender, object nitem);
    public class ComboBoxAdvanced : ComboBox
    {
        public List<KeyValuePair<string, string>> AutoCompleteList;
        private ImageList imageList;
        public ImageList ImageList
        {
            get { return imageList; }
            set {
                imageList = value;
                if (imageList != null)
                {
                    DrawMode = DrawMode.OwnerDrawFixed;
                }
                else
                    DrawMode = DrawMode.Normal;
            }
        }
        public ComboBoxAdvanced() : base()
        {
        }
        protected override void OnDrawItem(DrawItemEventArgs ea)
        {
            if (imageList == null)
            {
                base.OnDrawItem(ea);
                return;
            }
            ea.DrawBackground();
            ea.DrawFocusRectangle();

            ComboBoxAdvancedItem item;
            Size imageSize = imageList.ImageSize;
            Rectangle bounds = ea.Bounds;

            try
            {
                item = (ComboBoxAdvancedItem)Items[ea.Index];

                if (item.ImageIndex != -1)
                {
                    imageList.Draw(ea.Graphics, bounds.Left, bounds.Top,
          item.ImageIndex);
                    ea.Graphics.DrawString(item.Text, ea.Font, new
          SolidBrush(ea.ForeColor), bounds.Left + imageSize.Width, bounds.Top);
                }
                else
                {
                    ea.Graphics.DrawString(item.Text, ea.Font, new
          SolidBrush(ea.ForeColor), bounds.Left, bounds.Top);
                }
            }
            catch
            {
                if (ea.Index != -1)
                {
                    ea.Graphics.DrawString(Items[ea.Index].ToString(), ea.Font, new
          SolidBrush(ea.ForeColor), bounds.Left, bounds.Top);
                }
                else
                {
                    ea.Graphics.DrawString(Text, ea.Font, new
          SolidBrush(ea.ForeColor), bounds.Left, bounds.Top);
                }
            }

            base.OnDrawItem(ea);
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
    }
    public class CheckBoxAdvanced:CheckBox
    {
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
    }
    public class ToolStripComboBoxAdvanced : ToolStripComboBox
    {
        Size FNewSize;
        public new Size Size
        {
            set
            {
                FNewSize = value;
                base.Size = new Size(Convert.ToInt32(Math.Round(Reportman.Drawing.GraphicUtils.DPIScaleX * FNewSize.Width)),
                     Convert.ToInt32(Math.Round(Reportman.Drawing.GraphicUtils.DPIScaleY * FNewSize.Height)));
            }
            get
            {
                return FNewSize;
            }
        }
    }
    public class ToolStripTextBoxAdvanced: ToolStripTextBox, IMessageFilter
    {
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg,
            int wParam, string lParam);
        private Control ComboParentForm; // Or use type "Form" 
        private ListBox listBoxChild;
        private int IgnoreTextChange;
        private bool MsgFilterActive = false;
        public int AutoCompleteWidth = 0;
        public List<KeyValuePair<string, string>> AutoCompleteList;
        public List<AutoCompleteInfo> AutoCompleteListTop;
        public int AutoCompleteMaxVisibleLines = 20;
        public bool IncludeGoogleTerms = false;
        public Image GoogleTermsImage = Properties.Resources.search16;
        public Image NotGoogleTermsImage = Properties.Resources.search16;
        string FCueBannerText = "";
        public string CueBannerText
        {
            get
            {
                return FCueBannerText;
            }
            set
            {
                FCueBannerText = value;
                UpdateCueBanner();
            }
        }
        private void Control_HandleCreated(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FCueBannerText))
                UpdateCueBanner();
        }
        private void UpdateCueBanner()
        {
            SendMessage(this.Control.Handle, EM_SETCUEBANNER, 1, FCueBannerText);
        }
        public ToolStripTextBoxAdvanced()
        {
            // Set up all the events we need to handle
            TextChanged += ComboListMatcher_TextChanged;
            //SelectionChangeCommitted += ComboListMatcher_SelectionChangeCommitted;
            LostFocus += ComboListMatcher_LostFocus;
            MouseDown += ComboListMatcher_MouseDown;
            //HandleDestroyed += ComboListMatcher_HandleDestroyed;
            Control.HandleCreated += Control_HandleCreated;
        }
        public override Size GetPreferredSize(Size constrainingSize)
        {
            Size newSize = base.GetPreferredSize(constrainingSize);
            return newSize;
        }
        /* Size FNewSize;
         public new Size Size
         {
             set
             {
                 FNewSize = value;
                 base.Size = new Size(Convert.ToInt32(Math.Round(Reportman.Drawing.GraphicUtils.DPIScaleX * FNewSize.Width)),
                      Convert.ToInt32(Math.Round(Reportman.Drawing.GraphicUtils.DPIScaleY * FNewSize.Height)));
             }
             get
             {
                 return FNewSize;
             }
         }*/
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
            string[] words = pattern.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int idx = 0;
            foreach (AutoCompleteInfo compinfo in AutoCompleteListTop)
            {
                bool match = true;
                string searchvalue = compinfo.Text.ToUpper() +" "+ compinfo.Address.ToUpper();
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


        void ComboListMatcher_HandleDestroyed(object sender, EventArgs e)
        {
            if (MsgFilterActive)
                Application.RemoveMessageFilter(this);
        }



        private void ComboListMatcher_MouseDown(object sender, MouseEventArgs e)
        {
            HideTheList();
        }

        void ComboListMatcher_LostFocus(object sender, EventArgs e)
        {
            if (listBoxChild != null && !listBoxChild.Focused)
                HideTheList();
        }

        void ComboListMatcher_SelectionChangeCommitted(object sender, EventArgs e)
        {
            IgnoreTextChange++;
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
        
        void InitListControl()
        {
            if (listBoxChild == null)
            {
                // Find parent - or keep going up until you find the parent form
                //ComboParentForm = this.Parent;
                if (this.Parent != null)
                {
                    if ((Parent as ToolStrip).Parent != null)
                        ComboParentForm = FindForm((Parent as ToolStrip).Parent);
                }
                if (ComboParentForm != null)
                {
                    // Setup a messaage filter so we can listen to the keyboard
                    if (!MsgFilterActive)
                    {
                        Application.AddMessageFilter(this);
                        MsgFilterActive = true;
                    }

                    listBoxChild = new ListBoxNonSelectable();
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
                        ((TableLayoutPanel)ComboParentForm).SetRow(listBoxChild,1);
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
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(204,229,255)), e.Bounds);
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

        void ExecuteSearch()
        {

        }
        void ComboListMatcher_TextChanged(object sender, EventArgs e)
        {
            if (IgnoreTextChange > 0)
            {
                IgnoreTextChange = 0;
                return;
            }
            if (AutoCompleteList == null)
                return;

            InitListControl();

            if (listBoxChild == null)
                return;

            string SearchText = Text;


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
                    Point PutItHere = new Point(this.Bounds.Left, this.Bounds.Bottom);
                    Control TheControlToMove;

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
                    {
                        TheControlToMove.Show();
                        //TheControlToMove.BringToFront();
                    }
                }
            }
            else
                HideTheList();
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

        private void ListBox1_Click(object sender, EventArgs e)
        {
            var ThisList = sender as ListBox;

            if (OnDropDownClicked == null)
            {
                if (ThisList != null)
                {
                    // Copy selection to the combo box
                    CopySelection();
                    this.Focus();
                    SendKeys.Send("{Enter}");
                }
            }
        }

        private void HideTheList()
        {
            if (listBoxChild != null)
            {
        /*        listBoxChild.Parent = null;
                listBoxChild.Dispose();
                listBoxChild = null;*/
                listBoxChild.Hide();
            }
        }
        public StringEvent OnDropDownClicked;
        public bool PreFilterMessage(ref Message m)
        {

            if (m.Msg == 0x201) // Mouse click: WM_LBUTTONDOWN
            {
                var Pos = new Point((int)(m.LParam.ToInt32() & 0xFFFF), (int)(m.LParam.ToInt32() >> 16));

                var Ctrl = Control.FromHandle(m.HWnd);
                if (Ctrl != null)
                {
                    if (ComboParentForm != null)
                    {
                        // Convert the point into our parent control's coordinates ...
                        Point newPos = ComboParentForm.PointToClient(Ctrl.PointToScreen(Pos));

                        // ... because we need to hide the list if user clicks on something other than the list-box
                        if (ComboParentForm != null)
                        {
                            if (listBoxChild != null)
                            {
                                if (listBoxChild.Visible)
                                {
                                    if (newPos.X < listBoxChild.Left || newPos.X > listBoxChild.Right
                                        || newPos.Y < listBoxChild.Top || newPos.Y > listBoxChild.Bottom)
                                    {
                                        this.HideTheList();
                                    }
                                    else
                                    {
                                        Point pointlist = listBoxChild.PointToClient(Ctrl.PointToScreen(Pos));
                                        int newidex = listBoxChild.IndexFromPoint(pointlist);
                                        listBoxChild.SelectedIndex = newidex;
                                        if (newidex >= 0)
                                        {
                                            if (OnDropDownClicked != null)
                                            {
                                                try

                                                {
                                                    if (listBoxChild.Items[listBoxChild.SelectedIndex] is AutoCompleteInfo)
                                                    {
                                                        AutoCompleteInfo compinfo = (AutoCompleteInfo)listBoxChild.Items[listBoxChild.SelectedIndex];
                                                        OnDropDownClicked(this, compinfo);
                                                    }
                                                    else
                                                        OnDropDownClicked(this, listBoxChild.Items[listBoxChild.SelectedIndex].ToString());
                                                }
                                                catch
                                                {
                                                }
                                            }
                                            else
                                            {
                                                CopySelection();
                                                m.Msg = 0x100;
                                                m.WParam = (IntPtr)0x0D;
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            // Capture the mouse and freeit on mouse upp
                                            //listBoxChild.Capture = true;
                                            return true;
                                        }
                                    }
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
                }
            }
           else
            if (m.Msg == 0x100) // WM_KEYDOWN
            {
                if (listBoxChild != null && listBoxChild.Visible)
                {
                    int keycode = m.WParam.ToInt32();
                    switch (keycode)
                    {
                        case 0x1B: // Escape key
                            this.HideTheList();
                            return true;
                        case 34: // Page down
                        case 33: // Page up
                        case 0x26: // up key
                        case 0x28: // right key
                                   // Change selection
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
                            if (NewIx>=0)
                                listBoxChild.SelectedIndex = NewIx;
                            return true;

                        case 0x0D: // return (use the currently selected item)
                            CopySelection();
                            return false;
                    }
                }
            }

            return false;
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
        }
    }
    public class ComboListMatcher : TextBox, IMessageFilter
    {
        private Control ComboParentForm; // Or use type "Form" 
        private ListBox listBoxChild;
        private int IgnoreTextChange;
        private bool MsgFilterActive = false;
        public int AutoCompleteWidth = 0;
        public List<KeyValuePair<string, string>> AutoCompleteList;
        public int AutoCompleteMaxVisibleLines = 20;

        public ComboListMatcher()
        {
            // Set up all the events we need to handle
            TextChanged += ComboListMatcher_TextChanged;
            //SelectionChangeCommitted += ComboListMatcher_SelectionChangeCommitted;
            LostFocus += ComboListMatcher_LostFocus;
            MouseDown += ComboListMatcher_MouseDown;
            HandleDestroyed += ComboListMatcher_HandleDestroyed;
        }
        char[] separators = { '-', ' ' };
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

        void ComboListMatcher_HandleDestroyed(object sender, EventArgs e)
        {
            if (MsgFilterActive)
                Application.RemoveMessageFilter(this);
        }

        ~ComboListMatcher()
        {
        }

        private void ComboListMatcher_MouseDown(object sender, MouseEventArgs e)
        {
            HideTheList();
        }

        void ComboListMatcher_LostFocus(object sender, EventArgs e)
        {
            if (listBoxChild != null && !listBoxChild.Focused)
                HideTheList();
        }

        void ComboListMatcher_SelectionChangeCommitted(object sender, EventArgs e)
        {
            IgnoreTextChange++;
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

        void InitListControl()
        {
            if (listBoxChild == null)
            {
                // Find parent - or keep going up until you find the parent form
                ComboParentForm = this.Parent;
                if (!(this.Parent is TableLayoutPanel))
                {
                    ComboParentForm = FindForm(this);
                }
                if (ComboParentForm != null)
                {
                    // Setup a messaage filter so we can listen to the keyboard
                    if (!MsgFilterActive)
                    {
                        Application.AddMessageFilter(this);
                        MsgFilterActive = true;
                    }

                    listBoxChild = new ListBoxNonSelectable();
                    listBoxChild.TabStop = false;
                    listBoxChild.Visible = false;
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


        void ComboListMatcher_TextChanged(object sender, EventArgs e)
        {
            if (IgnoreTextChange > 0)
            {
                IgnoreTextChange = 0;
                return;
            }
            if (AutoCompleteList == null)
                return;

            InitListControl();

            if (listBoxChild == null)
                return;

            string SearchText = Text;


            // Don't show the list when nothing has been typed
            if (!string.IsNullOrEmpty(SearchText))
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
            else
            {
                listBoxChild.Items.Clear();
            }

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
                    listBoxChild.BringToFront();
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
                    {
                        TheControlToMove.Show();
                        //TheControlToMove.BringToFront();
                    }
                }
            }
            else
                HideTheList();
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

        private void ListBox1_Click(object sender, EventArgs e)
        {
            var ThisList = sender as ListBox;

            if (ThisList != null)
            {
                // Copy selection to the combo box
                CopySelection();
            }
        }

        private void HideTheList()
        {
            if (listBoxChild != null)
            {
                /*listBoxChild.Parent = null;
                listBoxChild.Dispose();
                listBoxChild = null;*/
                listBoxChild.Hide();
            }
        }

        public bool PreFilterMessage(ref Message m)
        {

            if (m.Msg == 0x201) // Mouse click: WM_LBUTTONDOWN
            {
                var Pos = new Point((int)(m.LParam.ToInt32() & 0xFFFF), (int)(m.LParam.ToInt32() >> 16));

                var Ctrl = Control.FromHandle(m.HWnd);
                if (Ctrl != null)
                {
                    if (ComboParentForm != null)
                    {
                        // Convert the point into our parent control's coordinates ...
                        Pos = ComboParentForm.PointToClient(Ctrl.PointToScreen(Pos));

                        // ... because we need to hide the list if user clicks on something other than the list-box
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
                    }
                }
            }
            else
            if (m.Msg == 0x020A)
            {
                // Mouse wheel
                if (listBoxChild != null && listBoxChild.Visible)
                {
                    int wheelcode = m.WParam.ToInt32();
                    wheelcode = wheelcode >> 16;
                    int wheels = wheelcode / 120;
                    int incrementtodo = -wheelcode / 120;
                    int NewIx = listBoxChild.SelectedIndex + incrementtodo;
                    if (NewIx < 0)
                        NewIx = 0;
                    if (NewIx > listBoxChild.Items.Count - 1)
                        NewIx = listBoxChild.Items.Count - 1;
                    // Keep the index valid!
                    if (NewIx >= 0)
                        listBoxChild.SelectedIndex = NewIx;
                }
            }
            else
            if (m.Msg == 0x100) // WM_KEYDOWN
            {
                if (listBoxChild != null && listBoxChild.Visible)
                {
                    int keycode = m.WParam.ToInt32();
                    switch (keycode)
                    {
                        case 0x1B: // Escape key
                            this.HideTheList();
                            return true;
                        case 34: // Page down
                        case 33: // Page up
                        case 0x26: // up key
                        case 0x28: // right key
                                   // Change selection
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
                            return true;

                        case 0x0D: // return (use the currently selected item)
                            CopySelection();
                            return false;
                    }
                }
            }

            return false;
        }
    }
    public class ComboBoxAdvancedItem
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private int _imageIndex;
        public int ImageIndex
        {
            get { return _imageIndex; }
            set { _imageIndex = value; }
        }

        public ComboBoxAdvancedItem()
            : this("")
        {
        }

        public ComboBoxAdvancedItem(string text)
            : this(text, -1)
        {
        }

        public ComboBoxAdvancedItem(string text, int imageIndex)
        {
            _text = text;
            _imageIndex = imageIndex;
        }

        public override string ToString()
        {
            return _text;
        }
    }
    public class FormUtils
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();
        public static Form GetActiveForm()
        {
            Form result = Form.ActiveForm;
            if (result == null)
            {
                if (Application.OpenForms.Count > 0)
                    result = Application.OpenForms[Application.OpenForms.Count - 1];
            }
            return result;
        }
        public static bool IsParent(Control parent,Control child)
        {
            if (parent == child.Parent)
                return true;
            else
            {
                if (child.Parent == null)
                    return false;
                else
                    return IsParent(parent, child.Parent);
            }
        }
        public static Control FocusedControl(Control parent)
        {
            var focusedControl = FocusedControl();
            if (focusedControl != null)
            {
                bool wasParent = IsParent(parent, focusedControl);
                if (!wasParent)
                    focusedControl = null;
            }
            /*if (focusedControl == null)
            {
                focusedControl = FindFocusedControl(parent);
            }*/
            return focusedControl;
        }
        /*private static Control FindFocusedControl(Control control)
        {
            ContainerControl container = control as ContainerControl;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as ContainerControl;
            }
            return control;
        }*/
        public static Control FocusedControl()
        {
            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Control focused = null;
                IntPtr handle = GetFocus();
                if (handle != IntPtr.Zero)
                {
                    focused = Control.FromHandle(handle);
                }

                return focused;
            }
            else
                return null;
        }
        public static Padding PaddingDPI(Padding source)
        {
            return new Padding(GraphicUtils.ScaleToDPI(source.Left), GraphicUtils.ScaleToDPI(source.Top),
                GraphicUtils.ScaleToDPI(source.Right), GraphicUtils.ScaleToDPI(source.Bottom));
        }

        public static void ScaleImageList(ImageList imlist)
        {

            int newx = System.Convert.ToInt32(imlist.ImageSize.Width * Reportman.Drawing.GraphicUtils.DPIScaleX);
            int newy = System.Convert.ToInt32(imlist.ImageSize.Height * Reportman.Drawing.GraphicUtils.DPIScaleY);
            if ((newx == imlist.ImageSize.Width) && (newy == imlist.ImageSize.Height))
                return;
            //int newx = System.Convert.ToInt32(imlist.ImageSize.Width * 1.20);
            //int newy = System.Convert.ToInt32(imlist.ImageSize.Height * 1.2);
            List<Image> lista = new List<Image>();
            foreach (Image im in imlist.Images)
                lista.Add(im);
            imlist.ImageSize = new Size(newx, newy);
            imlist.Images.Clear();
            foreach (Image im in lista)
                imlist.Images.Add(im);
        }
        public static bool IsChildFocused(Control parentcontrol)
        {
            Control focused = FocusedControl();
            if (focused==null)
                return false;
            while (focused != null)
            {
                if (focused == parentcontrol)
                    return true;
                else
                    focused = focused.Parent;
            }
            return false;
        }


    }
    public class CustomPaintControl: Control
    {
        public PaintEventHandler OnCustomPaint;
        public CustomPaintControl()
            : base()
        {
            DoubleBuffered = true;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (OnCustomPaint != null)
                OnCustomPaint(this,e);
        }
        
    }

    public class UnSelectableButton : Button
    {
       private const int WM_MOUSEACTIVATE = 0x0021;
       const int MA_NOACTIVATE = 3;
       public UnSelectableButton()
          : base()
       {
          SetStyle(ControlStyles.Selectable, false);
       }
       protected override void WndProc(ref Message m)
       {
          if (m.Msg == WM_MOUSEACTIVATE)
             m.Result = (IntPtr)MA_NOACTIVATE;
          base.WndProc(ref m);
       }
    }

    public class ToolStripAdvanced : ToolStrip
    {
        Size FNewImageScalingSize;
        public new Size ImageScalingSize
        {
            set
            {
                FNewImageScalingSize = value;
                base.ImageScalingSize = new Size(Convert.ToInt32(Math.Round(Reportman.Drawing.GraphicUtils.DPIScaleX * FNewImageScalingSize.Width)),
                     Convert.ToInt32(Math.Round(Reportman.Drawing.GraphicUtils.DPIScaleY * FNewImageScalingSize.Height)));
            }
            get
            {
                return FNewImageScalingSize;
            }
        }
        //Size FNewSize;
        // La barra se distorsiona en resoluciones altas
        /*public new Size Size
        {
            set
            {
                FNewSize = value;
                base.Size = new Size(Convert.ToInt32(Math.Round(Reportman.Drawing.GraphicUtils.DPIScaleX * FNewSize.Width)),
                     Convert.ToInt32(Math.Round(Reportman.Drawing.GraphicUtils.DPIScaleY * FNewSize.Height)));
            }
            get
            {
                return FNewSize;
            }
        }*/
        const uint WM_LBUTTONDOWN = 0x201;
        const uint WM_LBUTTONUP = 0x202;

        static private bool down = false;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONUP && !down)
            {
                m.Msg = (int)WM_LBUTTONDOWN; base.WndProc(ref m);
                m.Msg = (int)WM_LBUTTONUP;
            }

            if (m.Msg == WM_LBUTTONDOWN) down = true;
            if (m.Msg == WM_LBUTTONUP) down = false;

            base.WndProc(ref m);
        }
    }

    public class ListBoxNonSelectable:ListBox
    {
        public ListBoxNonSelectable():base()
        {
            SetStyle(ControlStyles.Selectable, false);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.Opaque, true);
            //SetStyle(ControlStyles.UserMouse, true);
            //this.DoubleBuffered = true;
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            /*this.SetStyle(
           ControlStyles.OptimizedDoubleBuffer |
           ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint,
           true);*/

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.Opaque, true);
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }
        private const int WS_EX_NOACTIVATE = 0x08000000;
        protected override CreateParams CreateParams
        { 

            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                //cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                //cp.Style = cp.Style & ~0x200000;
                return cp;
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }
    }
    class GoogleSuggestion
    {
        string Text;
        public GoogleSuggestion(string ntext)
        {
            Text = ntext;
        }
        public override string ToString()
        {
            return Text;
        }
    }
    public class AutoCompleteInfo
    {
        public string Text;
        public string Address;
        public Image Icon;
        public AutoCompleteInfo(string nText,string nAddress,Image nIcon)
        {
            Text = nText;
            Address = nAddress;
            Icon = nIcon;
        }
        public override string ToString()
        {
            return Text + "  "+Address;
        }
    }


}
