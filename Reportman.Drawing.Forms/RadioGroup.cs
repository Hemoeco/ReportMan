using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reportman.Drawing.Forms
{
    public delegate void StandardEvent (object sender,EventArgs args);
    public partial class RadioGroup : Reportman.Drawing.Forms.GroupBoxAdvanced
    {
        public EventHandler SelectedIndexChanged;
        public bool FHorizontal;
        private bool FAutoSize;
        private int FSelectedIndex;
        List<RadioButton> buttons;
        public int SelectedIndex
        {
            get
            {
                return FSelectedIndex;
            }
            set
            {
                bool diferent = false;
                if (value < 0)
                {
                    foreach (RadioButton bt in buttons)
                    {
                        bt.Checked = false;
                    }
                    if (FSelectedIndex!=-1)
                        diferent = true;
                    FSelectedIndex = -1;
                }
                else
                if (value < buttons.Count)
                {
                    buttons[value].Checked = true;
                    if (FSelectedIndex!=value)
                        diferent=true;
                    FSelectedIndex = value;
                }
                if (diferent)
                    if (SelectedIndexChanged != null)
                        SelectedIndexChanged(this, new EventArgs());
            }
        }
        public bool AutoAdjustSize
        {
            get { return FAutoSize; }
            set 
            { 
                FAutoSize = value;
                DoResize();            
            }
        }
        public bool Horizontal
        {
            get { return FHorizontal; }
            set
            {
                FHorizontal = value;
                DoResize();
            }
        }
        public void SetCaptions(Strings captions)
        {
            int FOldItemIndex = FSelectedIndex;
            foreach (RadioButton rbold in buttons)
            {
                Controls.Remove(rbold);
                rbold.Dispose();
            }
            buttons.Clear();
            foreach (string captionname in captions)
            {
                RadioButton nradio = new RadioButton();
                nradio.Text = captionname;
                if (FOldItemIndex == buttons.Count)
                    nradio.Checked = true;
                nradio.CheckedChanged += new EventHandler(radiochange);
                Controls.Add(nradio);
                buttons.Add(nradio);
            }
            DoResize();
        }
        private void radiochange(object sender, EventArgs nargs)
        {
            int idx=0;
            int newSelectedIndex = -1;
            foreach (RadioButton nbutton in buttons)
            {
                if (nbutton.Checked)
                {
                    newSelectedIndex = idx;
                    break;
                }
                idx++;
            }
            if (newSelectedIndex != FSelectedIndex)
            {
                FSelectedIndex = newSelectedIndex;
                if (SelectedIndexChanged != null)
                    SelectedIndexChanged(this, new EventArgs());
            }
        }
        public RadioGroup()
        {
            InitializeComponent();
            buttons = new List<RadioButton>();
            BackColor = Color.Transparent;
            FSelectedIndex = -1;
        }
        private void DoResize()
        {
            const int TOP_GAP = 3;
            const int BOTTOM_GAP = 3;
            const int LEFT_GAP = 10;
            int INTER_GAP = 0;
            int INTER_GAP_VERTICAL =0;
            SizeF maxsize = new SizeF(0.0f,0.0f);
            using (Graphics gr = this.CreateGraphics())
            {
                foreach (RadioButton rb in buttons)
                {
                    SizeF newsize = gr.MeasureString(rb.Text, Font);
                    if (newsize.Height < 17)
                        newsize.Height = 17;
                    if (Horizontal)
                        newsize.Width = newsize.Width + 20;
                    if (newsize.Width > maxsize.Width)
                        maxsize = new SizeF(newsize.Width, maxsize.Height);
                    if (newsize.Height> maxsize.Height)
                        maxsize = new SizeF(maxsize.Width, newsize.Height);
                }
                // Measure the caption
                SizeF newsize2 = gr.MeasureString(Text, Font);
                if (!Horizontal)
                    if (newsize2.Width > maxsize.Width)
                        maxsize = new SizeF(newsize2.Width, maxsize.Height);
                if (newsize2.Height > maxsize.Height)
                    maxsize = new SizeF(maxsize.Width, newsize2.Height);
            }
            Size imaxsize = new Size(System.Convert.ToInt32(maxsize.Width+20),System.Convert.ToInt32(maxsize.Height));
            INTER_GAP = 5;
            INTER_GAP_VERTICAL = imaxsize.Height/3*3;
            int TopPos = imaxsize.Height+TOP_GAP;
            int LeftPos = LEFT_GAP;
            if (FAutoSize)
            {
                foreach (RadioButton radio in buttons)
                {
                    radio.SetBounds(LeftPos, TopPos, imaxsize.Width, imaxsize.Height);
                    if (Horizontal)
                    {
                        LeftPos = LeftPos + imaxsize.Width + INTER_GAP;
                    }
                    else
                    {
                        TopPos = TopPos + imaxsize.Height + INTER_GAP_VERTICAL;
                    }
                }
                if (Horizontal)
                    SetBounds(Left, Top, LeftPos + LEFT_GAP * 2, TOP_GAP + imaxsize.Height * 2 + BOTTOM_GAP * 2);
                else
                    SetBounds(Left, Top, imaxsize.Width + LEFT_GAP * 2, TopPos + BOTTOM_GAP);
            }
            else
            {
                if (buttons.Count == 0)
                    return;
                INTER_GAP = (Width - LEFT_GAP * 2) / buttons.Count;
                INTER_GAP_VERTICAL = (Height - TOP_GAP - imaxsize.Height - BOTTOM_GAP) / buttons.Count;
                TopPos = TOP_GAP;
                if (Text.Length > 0)
                    TopPos = TopPos + imaxsize.Height;
                //TopPos = TopPos + (Height - TopPos) / 2- imaxsize.Height/2;
                foreach (RadioButton radio in buttons)
                {
                    if (Horizontal)
                    {
                        radio.SetBounds(LeftPos, TopPos, imaxsize.Width, imaxsize.Height);
                        LeftPos = LeftPos + INTER_GAP;
                    }
                    else
                    {
                        radio.SetBounds(LeftPos, TopPos, imaxsize.Width, imaxsize.Height);
                        TopPos = TopPos + INTER_GAP_VERTICAL;
                    }
                }
            }
        }
        protected override void OnFontChanged(EventArgs e)
        {
            DoResize();
            // Resize the control
            base.OnFontChanged(e);
        }
        protected override void OnParentChanged(EventArgs e)
        {
            DoResize();
            base.OnParentChanged(e);
        }
        protected override void OnResize(EventArgs e)
        {
            if (!FAutoSize)
                base.OnResize(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            BackColor = Color.Transparent;
            base.OnPaint(e);
        }
    }
}
