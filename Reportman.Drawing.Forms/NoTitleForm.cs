using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Reportman.Drawing.Forms
{
    public partial class NoTitleForm : Form
    {
        ReSize resize = new ReSize();     // ReSize Class "/\" To Help Resize Form <None Style>
        private const int DEFAULT_GRIP_SIZE = 16;      // Grip size
        private const int DEFAULT_CAPTION_HEIGHT = 32;   // Caption bar height;
        private const int DEFAULT_BORDER_SIZE = 8;

        public bool AllowWindowEffects = true;

        public NoTitleForm()
        {
            InitializeComponent();
            FBorderColor = Color.DarkBlue;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //SetStyle(ControlStyles.Opaque, true);
            //            BackColor = Color.Magenta;
            //            TransparencyKey = BackColor;
            timeradjust.Tick += Timeradjust_Tick;
            
        }

        protected override void OnLoad(EventArgs e)
        {
            RestoreSavedBounds();

            base.OnLoad(e);
        }
        const int WS_MINIMIZEBOX = 0x20000;
        const int CS_DBLCLKS = 0x8;
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                parms.Style &= ~0x00C00000; // remove WS_CAPTION
                parms.Style |= WS_MINIMIZEBOX;
                parms.Style |= CS_DBLCLKS;
                //parms.Style |= 0x00040000;  // include WS_SIZEBOX
                return parms;
            }
        }
        bool NoTitleMaximized = false;
        Rectangle restored_bounds;
        bool firsttimerestored = true;

        System.Windows.Forms.Timer timeradjust = new System.Windows.Forms.Timer();
        protected virtual void OnFormWindowStateChanged(EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Normal:
                    timeradjust.Interval = 10;
                    timeradjust.Enabled = false;
                    timeradjust.Enabled = true;
                    //AfterAdjustWindowToNormal();    
                    if (CurrentToolStrip != null)
                    {
                        /*                        CurrentToolStrip.Capture = true;
                                                toolbarorigin = Cursor.Position;
                                                moving = true;*/
                        //CurrentToolStrip.Capture = true;
                        //CurrentToolStrip.Capture = false;
                        //SendMessage(Handle, WM_NCLBUTTONUP, HT_CAPTION, 0);                        
                        //SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                    }
                    break;
            }
        }
        private void Timeradjust_Tick(object sender, EventArgs e)
        {
            timeradjust.Enabled = false;
            AfterAdjustWindowToNormal();    
        }

        public void AfterAdjustWindowToNormal()
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            // if first time look for DefaultBounds
            if (botonmax != null)
                botonmax.Image = Properties.Resources.maximizar_19;
            if (firsttimerestored)
            {
                firsttimerestored = false;
                if (DefaultRestoreBounds.Left >= -100)
                    SetBounds(DefaultRestoreBounds.Left, DefaultRestoreBounds.Top, DefaultRestoreBounds.Width, DefaultRestoreBounds.Height);
            }
            AdjustToolStripOffset();

        }

        FormWindowState oldstate;
        string oldtext = "";
        bool InFullScreen = false;
        public virtual void SetFullScreen(bool newvalue)
        {
            
            if (newvalue)
            {
                List<Control> hijos = new List<Control>();
                foreach (Control ncontrol in Controls)
                    hijos.Add(ncontrol);
                Controls.Clear();
                oldtext = Text;
                oldstate = WindowState;
                //Text = "";
                if (WindowState == FormWindowState.Normal)
                {
                    SaveBounds();
                    InFullScreen = newvalue;
                }
                else
                {
                    InFullScreen = newvalue;
                    SwitchMaximizeMinimize();
                    Application.DoEvents();
                }
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Normal;
                Rectangle workarea = Screen.FromControl(this).Bounds;
                SetBounds(workarea.Left, workarea.Top, workarea.Width, workarea.Height);
                //WindowState = FormWindowState.Maximized;
                foreach (Control ncontrol in hijos)
                    Controls.Add(ncontrol);
            }
            else
            {
                Text = oldtext;
                if (oldstate == FormWindowState.Normal)
                {
                    InFullScreen = newvalue;
                    FormBorderStyle = FormBorderStyle.Sizable;
                    RestoreSavedBounds();
                }
                else
                {
                    RestoreSavedBounds();
                    InFullScreen = newvalue;
                    SwitchMaximizeMinimize();
                }
            }
        }


        public void SwitchMaximizeMinimize()
        {
            if (AllowWindowEffects)
            {
                //Point ps = this.PointToClient(new Point(Left, Top));
                //Rectangle workarea = Screen.FromHandle(this.Handle).WorkingArea;
                //this.MaximizedBounds = new Rectangle(ps.X,ps.Y,workarea.Width-ps.X*2,workarea.Height-ps.Y*2);

                if (WindowState == FormWindowState.Maximized)
                {
                    FormBorderStyle = FormBorderStyle.Sizable;
                    // if first time look for DefaultBounds
                    if (botonmax != null)
                        botonmax.Image = Properties.Resources.maximizar_19;
                    WindowState = FormWindowState.Normal;
                    AfterAdjustWindowToNormal();
                }
                else
                {
                    /*Rectangle rec = Screen.GetWorkingArea(this);
                    MaximizedBounds = new Rectangle(-8,-8,rec.Width+16,rec.Width+16);*/
                    FormBorderStyle = FormBorderStyle.None;                     
                    MaximizedBounds = Screen.GetWorkingArea(this);
                    if (botonmax != null)
                        botonmax.Image = Properties.Resources.restaurar_19;
                    //SetFullScreen(true);
                    WindowState = FormWindowState.Maximized;
                    AdjustToolStripOffset();
                }
                return;
            }
            
            if (!FShowTitle)
            {
                if (NoTitleMaximized)
                {
                    NoTitleMaximized = false;
                    if (botonmax != null)
                        botonmax.Image = Properties.Resources.maximizar_19;
                    FormBorderStyle = FormBorderStyle.Sizable;
                    SetBounds(restored_bounds.Left, restored_bounds.Top, restored_bounds.Width, restored_bounds.Height);
                    AdjustPadding();
                }
                else
                {
                    restored_bounds = this.Bounds;
                    NoTitleMaximized = true;
                    if (botonmax != null)
                        botonmax.Image = Properties.Resources.restaurar_19;
                    AdjustPadding();
                    Rectangle workarea = Screen.FromControl(this).WorkingArea;
                    if (AllowWindowEffects)
                    {
                        FormBorderStyle = FormBorderStyle.None;
                        SetBounds(workarea.Left, workarea.Top, workarea.Width, workarea.Height); 
                        /*
                        Point ps = this.PointToClient(new Point(Left, Top));
                        SetBounds(0,0, workarea.Width, workarea.Height);
                        Point npoint = PointToScreen(new Point(0, 0));
                        Rectangle newbounds = new Rectangle(ps.X, ps.Y, workarea.Width - ps.X * 2, workarea.Height - ps.Y * 2);
                        SetBounds(newbounds.X, newbounds.Y, newbounds.Width, newbounds.Height);*/
                        //ClientSize = new Size(workarea.Width, workarea.Height);
                    }
                    else
                    {
                        SetBounds(workarea.Left, workarea.Top, workarea.Width, workarea.Height);
                    }
                }
            }
        }

        bool FShowTitle = true;
        [DefaultValue(true)]
        public bool ShowTitle
        {
            get
            {
                return FShowTitle;
            }
            set
            {
                if (FShowTitle != value)
                {
                    FShowTitle = value;
                    if (FShowTitle)
                    {
                        FormBorderStyle = FormBorderStyle.Sizable;
                        AdjustPadding();
                    }
                    else
                    {
                        FormBorderStyle = FormBorderStyle.Sizable;
                        ControlBox = false;
                        Text = "";
                        this.SetStyle(ControlStyles.ResizeRedraw, true);
                        AdjustPadding();
                    }
                }
            }

        }
        private void AdjustPadding()
        {
            if (!FShowTitle)
            {
                int padding_width = Convert.ToInt32(FBorderSize * Reportman.Drawing.GraphicUtils.DPIScale);
                if (NoTitleMaximized)
                    this.Padding = new Padding(0, 0, 0, 0);
                else
                {
                    if (!AllowWindowEffects)
                        this.Padding = new Padding(padding_width, padding_width, padding_width, padding_width);
                }
                    
            }
            else
            {
                this.Padding = new Padding(0, 0, 0, 0);
            }
        }
        int FGripSize = DEFAULT_GRIP_SIZE;
        int FCaptionHeight = DEFAULT_CAPTION_HEIGHT;

        [DefaultValue(DEFAULT_GRIP_SIZE)]
        public int GripSize
        {
            get
            {
                return FGripSize;
            }
            set
            {
                FGripSize = value;
                Invalidate();
            }
        }
        [DefaultValue(DEFAULT_CAPTION_HEIGHT)]
        public int CaptionHeight
        {
            get
            {
                return FCaptionHeight;
            }
            set
            {
                FCaptionHeight = value;
                Invalidate();
            }
        }
        int FBorderSize = DEFAULT_BORDER_SIZE;

        [DefaultValue(DEFAULT_BORDER_SIZE)]
        public int BorderSize
        {
            get
            {
                return FBorderSize;
            }
            set
            {
                FBorderSize = value;
                Invalidate();
            }
        }
        bool FDrawBorder = true;
        [DefaultValue(true)]
        public bool DrawBorder
        {
            get
            {
                return FDrawBorder;
            }
            set
            {
                FDrawBorder= value;
                Invalidate();
            }
        }
        bool FDrawGrip = false;
        [DefaultValue(false)]
        public bool DrawGrip
        {
            get
            {
                return FDrawGrip;
            }
            set
            {
                FDrawGrip = value;
                Invalidate();
            }
        }
        bool FDrawTitle = true;
        [DefaultValue(true)]
        public bool DrawTitle
        {
            get
            {
                return FDrawTitle;
            }
            set
            {
                FDrawTitle = value;
                Invalidate();
            }
        }
        Color FBorderColor;
        [DefaultValue(typeof(Color), "DarkBlue")]
        public Color BorderColor
        {
            get
            {
                return FBorderColor;
            }
            set
            {
                FBorderColor = value;
                Invalidate();
            }
        }
        protected override void OnResize(EventArgs e)
        {
            //Invalidate();
            //AdjustButtonGap();

            switch (WindowState)
            {
                case FormWindowState.Normal:
                    break;
            }

            base.OnResize(e);
        }
        protected override void OnPaint(PaintEventArgs e)
            //this if you want to draw   (if)
        {
            if (!FShowTitle)
            {
                Color theColor = FBorderColor;
                int BORDER_SIZE = Convert.ToInt32(FBorderSize * Reportman.Drawing.GraphicUtils.DPIScale); ;
                int cGrip = Convert.ToInt32(FGripSize * Reportman.Drawing.GraphicUtils.DPIScale);
                int cCaption = Convert.ToInt32(FCaptionHeight * Reportman.Drawing.GraphicUtils.DPIScale);

                if ((FDrawBorder) && (!NoTitleMaximized))
                {

                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                                                 theColor, BORDER_SIZE, ButtonBorderStyle.Solid,
                                                 theColor, BORDER_SIZE, ButtonBorderStyle.Solid,
                                                 theColor, BORDER_SIZE, ButtonBorderStyle.Solid,
                                                 theColor, BORDER_SIZE, ButtonBorderStyle.Solid);
                    //e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;
                    //e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    //e.Graphics.FillRectangle(new SolidBrush(theColor), new RectangleF(0, 0, BorderSize, Height));

                }
                if (FDrawGrip && (!NoTitleMaximized))
                {
                    Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
                    ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
                }
                if (FDrawTitle)
                {
   
                    Rectangle rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
                    e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
                }


            }
            base.OnPaint(e);
        }


        //
        //override  WndProc  
        //
        protected override void WndProc(ref Message m)
        {
            if ((!FShowTitle) && (!NoTitleMaximized))
            {
                //****************************************************************************

                int x = (int)(m.LParam.ToInt64() & 0xFFFF);               //get x mouse position
                int y = (int)((m.LParam.ToInt64() & 0xFFFF0000) >> 16);   //get y mouse position  you can gave (x,y) it from "MouseEventArgs" too
                Point pt = PointToClient(new Point(x, y));

                if (m.Msg == 0x84)
                {
                    switch (resize.getMosuePosition(pt, this))
                    {
                        case "l": m.Result = (IntPtr)10; return;  // the Mouse on Left Form
                        case "r": m.Result = (IntPtr)11; return;  // the Mouse on Right Form
                        case "a": m.Result = (IntPtr)12; return;
                        case "la": m.Result = (IntPtr)13; return;
                        case "ra": m.Result = (IntPtr)14; return;
                        case "u": m.Result = (IntPtr)15; return;
                        case "lu": m.Result = (IntPtr)16; return;
                        case "ru": m.Result = (IntPtr)17; return; // the Mouse on Right_Under Form
                        case "": m.Result = pt.Y < FCaptionHeight /*mouse on title Bar*/ ? (IntPtr)2 : (IntPtr)1; return;

                    }
                }

            }
            if (m.Msg == WM_NCCALCSIZE)
            {
                if (m.WParam.Equals(IntPtr.Zero))
                {

                    RECT rc = (RECT)m.GetLParam(typeof(RECT));
                    Rectangle r = rc.ToRectangle();
                    //r.Inflate(8, 8);
                    Marshal.StructureToPtr(new RECT(r), m.LParam, true);
                }
                else
                {
                    NCCALCSIZE_PARAMS csp = (NCCALCSIZE_PARAMS)m.GetLParam(typeof(NCCALCSIZE_PARAMS));
                    Rectangle r = csp.rgrc0.ToRectangle();
                    //r.Inflate(8, 8);
                    csp.rgrc0 = new RECT(r);
                    Marshal.StructureToPtr(csp, m.LParam, true);
                }
                m.Result = IntPtr.Zero;
            }


            FormWindowState org = this.WindowState;


            base.WndProc(ref m);


            if (this.WindowState != org)
                this.OnFormWindowStateChanged(EventArgs.Empty);


        }
        const int WM_NCCALCSIZE = 0x83;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left, top, right, bottom;

            public RECT(Rectangle rc)
            {
                this.left = rc.Left;
                this.top = rc.Top;
                this.right = rc.Right;
                this.bottom = rc.Bottom;
            }

            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(left, top, right, bottom);
            }

        }
        [StructLayout(LayoutKind.Sequential)]
        private struct NCCALCSIZE_PARAMS
        {
            public RECT rgrc0, rgrc1, rgrc2;
            public WINDOWPOS lppos;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPOS
        {
            public IntPtr hWnd, hWndInsertAfter;
            public int x, y, cx, cy, flags;
        }

        ToolStripButton botonmax;
        ToolStripButton botonmin;
        ToolStripLabel buttongap;
        ToolStrip CurrentToolStrip;
        public void AddToolStripButtons(ToolStrip toolStrip1,bool center)
        {
            CurrentToolStrip = toolStrip1;
            ToolStripButton botonclose = new ToolStripButton();
            botonmin = new ToolStripButton();
            botonmin.DisplayStyle = ToolStripItemDisplayStyle.Image;
            botonmin.Image = Properties.Resources.minimizar_19;
            botonmin.Click += Botonmin_Click;
            botonmin.Alignment = ToolStripItemAlignment.Right;
            CustomToolStripRenderer nrender = new CustomToolStripRenderer();
            nrender.bclose = botonclose;
            toolStrip1.Renderer = nrender;

            botonmax = new ToolStripButton();
            botonmax.DisplayStyle = ToolStripItemDisplayStyle.Image;
            if (NoTitleMaximized)
            {
                botonmax.Image = Properties.Resources.restaurar_19;
            }
            else
            {
                botonmax.Image = Properties.Resources.maximizar_19;
            }
            botonmax.Alignment = ToolStripItemAlignment.Right;
            botonmax.Click += Botonmax_Click;

            botonclose.DisplayStyle = ToolStripItemDisplayStyle.Image;
            botonclose.Name = "BotonClose";
            botonclose.Image = Properties.Resources.cerrar_19;
            botonclose.Click += Botonclose_Click; ;
            botonclose.Alignment = ToolStripItemAlignment.Right;
            toolStrip1.MouseDown += ToolStrip1_MouseDown;
            toolStrip1.MouseMove += ToolStrip1_MouseMove;
            toolStrip1.MouseUp += ToolStrip1_MouseUp;
            toolStrip1.DoubleClick += ToolStrip1_DoubleClick;
            toolStrip1.ItemAdded += ToolStrip1_ItemAdded; ;
            toolStrip1.ItemRemoved += ToolStrip1_ItemRemoved;

            int separatorwidth = Convert.ToInt32(4* GraphicUtils.DPIScale);
            botonmin.Padding = new Padding(separatorwidth, 0, separatorwidth, 0);
            botonmax.Padding = new Padding(separatorwidth, 0, separatorwidth, 0);
            botonclose.Padding = new Padding(separatorwidth, 0, separatorwidth, 0);


            toolStrip1.Items.Add(botonclose);
            toolStrip1.Items.Add(botonmax);
            toolStrip1.Items.Add(botonmin);
            if (center)
            {
                buttongap = new ToolStripLabel();
//                buttongap.DisplayStyle = ToolStripItemDisplayStyle.;
                buttongap.AutoSize = true;
                toolStrip1.Items.Insert(0, buttongap);
                buttongap.Height = botonclose.Height;
                AdjustButtonGap();
            }
            else
            {
                AdjustToolStripOffset();
            }

        }
        private void AdjustToolStripOffset()
        {
            if (CurrentToolStrip == null)
                return;
            ToolStripItem firstitem = null;
            foreach (ToolStripItem nitem in CurrentToolStrip.Items)
            {
                if (nitem.Name == "bguardar")
                {
                    firstitem = nitem;
                    break;
                }
            }
            if (firstitem == null)
                return;
            if ((NoTitleMaximized) || (WindowState == FormWindowState.Maximized))
            {
                firstitem.Margin = new Padding(0, 0, 0, 0);
            }
            else
            {
                firstitem.Margin = new Padding(Convert.ToInt32(20 * GraphicUtils.DPIScale),0,0,0);
            }
        }

        private void ToolStrip1_ItemRemoved(object sender, ToolStripItemEventArgs e)
        {
            AdjustButtonGap();
        }

        private void ToolStrip1_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            AdjustButtonGap();
        }

        private void ToolStrip1_Resize(object sender, EventArgs e)
        {
            AdjustButtonGap();
        }

        private void AdjustButtonGap()
        {
            if (buttongap != null)
            {
                ToolStrip nstrip = buttongap.GetCurrentParent();
                if (nstrip != null)
                {
                    if (buttongap.Padding.Left != 0)
                    {
                        buttongap.Padding = new Padding(0, 0, 0, 0);
                        nstrip.SuspendLayout();
                        nstrip.ResumeLayout();
//                        Application.DoEvents();
                    }
                    int indexant = nstrip.Items.IndexOf(botonmin) - 1;
                    ToolStripItem lastitem = nstrip.Items[indexant-2];
                    int diference = botonmin.Bounds.Left - lastitem.Bounds.Right+buttongap.Padding.Left;
                    if (diference > 0)
                        buttongap.Padding = new Padding(diference / 2, 0, 0, 0);
                    else
                        buttongap.Padding = new Padding(0, 0, 0, 0);
                }
            }
        }
        private void ToolStrip1_DoubleClick(object sender, EventArgs e)
        {
            Point newpos = Cursor.Position;
            ToolStrip nstrip = (ToolStrip)sender;
            Point pclient = nstrip.PointToClient(newpos);
            if (pclient.X<botonmin.Bounds.Left)
            {
                int nindex = nstrip.Items.IndexOf(botonmin) - 3;
                int offset = 0;
                while (nstrip.Items[nindex].Bounds.Left == 0)
                {
                    offset = offset + nstrip.Items[nindex].Width;
                    nindex--;
                    if (nindex == 0)
                        break;
                }
                ToolStripItem lastitem = nstrip.Items[nindex];
                if (pclient.X>lastitem.Bounds.Right+offset)
                    SwitchMaximizeMinimize();
                else
                {
                    ToolStripItem firstitem = null;
                    foreach (ToolStripItem nitem in CurrentToolStrip.Items)
                    {
                        if (nitem.Name == "bguardar")
                        {
                            firstitem = nitem;
                            break;
                        }
                    }
                    if (firstitem != null)
                    {
                        if (firstitem.Margin.Left > 1)
                        {
                            if (pclient.X < firstitem.Margin.Left)
                                SwitchMaximizeMinimize();
                        }
                    }
                }

            }

        }
        enum BoundsEnum { None = 0,Maximized = 1,Restored = 2};
        private string GetIniFileName()
        {
            string exename = System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            exename = exename + "position";
            string filename = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            filename = System.IO.Path.Combine(filename, exename);
            filename = System.IO.Path.ChangeExtension(filename, ".ini");
            return filename;
        }
        public void SaveBounds()
        {
            if (InFullScreen)
                return;
            IniFile inif = new IniFile(GetIniFileName());
            switch (WindowState)
            {
                case FormWindowState.Maximized:
                    inif.WriteInteger("POSITION", "STATUS", 1);
                    inif.WriteInteger("POSITION", "LEFT", RestoreBounds.Left);
                    inif.WriteInteger("POSITION", "TOP", RestoreBounds.Top);
                    inif.WriteInteger("POSITION", "WIDTH", RestoreBounds.Width);
                    inif.WriteInteger("POSITION", "HEIGHT", RestoreBounds.Height);
                    break;
                case FormWindowState.Normal:
                    inif.WriteInteger("POSITION", "STATUS", 2);
                    inif.WriteInteger("POSITION", "LEFT", Left);
                    inif.WriteInteger("POSITION", "TOP", Top);
                    inif.WriteInteger("POSITION", "WIDTH", Width);
                    inif.WriteInteger("POSITION", "HEIGHT", Height);
                    break;
            }
            try
            {
                inif.SaveToFile(GetIniFileName());
            }
            catch
            {

            }
        }
        Rectangle DefaultRestoreBounds;
        public void RestoreSavedBounds()
        {
            IniFile inif = new IniFile(GetIniFileName());
            int status = inif.ReadInteger("POSITION", "STATUS", 0);
            if ((status < 0) || (status > 2))
            {
                status = 0;
            }
            BoundsEnum currentstatus = (BoundsEnum)status;
            if (InFullScreen)
            {
                currentstatus = BoundsEnum.Restored;
            }
            int newleft = inif.ReadInteger("POSITION", "LEFT", int.MinValue);
            int newtop = inif.ReadInteger("POSITION", "TOP", -1);
            int newwidth = inif.ReadInteger("POSITION", "WIDTH", -1);
            int newheight = inif.ReadInteger("POSITION", "HEIGHT", -1);
            DefaultRestoreBounds = new Rectangle(int.MinValue, int.MinValue, int.MinValue, int.MinValue);
            if ((newleft >= -50) && (newtop>=-20))
            {
                // Check inside work area
                Rectangle workarea = Screen.GetWorkingArea(this);
                if ((newleft<workarea.Right-300) &&
                    (newtop < workarea.Height - 300))
                {
                    DefaultRestoreBounds = new Rectangle(newleft, newtop, newwidth, newheight);
                }
            }
            if (currentstatus != BoundsEnum.None)
            {
                switch (currentstatus)
                {
                    case BoundsEnum.Maximized:
                        if (WindowState != FormWindowState.Maximized)
                            SwitchMaximizeMinimize();
                        break;
                    case BoundsEnum.Restored:
                        if (WindowState != FormWindowState.Normal)
                            SwitchMaximizeMinimize();
                        else
                        {
                            if (DefaultRestoreBounds.Left>-200)
                            {
                                SetBounds(DefaultRestoreBounds.Left, DefaultRestoreBounds.Top, DefaultRestoreBounds.Width, DefaultRestoreBounds.Height);
                                firsttimerestored = false;
                            }
                        }
                        break;
                    case BoundsEnum.None:
                        break;
                }
            }
            else
            {
                if (WindowState == FormWindowState.Normal)
                    SwitchMaximizeMinimize();
            }
        }
        private void ToolStrip1_MouseUp(object sender, MouseEventArgs e)
        {
            ToolStrip nstrip = (ToolStrip)sender;
            if ((nstrip.Capture) && (moving))
            {
                nstrip.Capture = false;
                moving = true;
            }
        }

        private void ToolStrip1_MouseMove(object sender, MouseEventArgs e)
        {
            ToolStrip nstrip = (ToolStrip)sender;
            if ((nstrip.Capture) && (moving))
            {

                Point newlocation = e.Location;
                int x1 = newlocation.X;
                int y1 = newlocation.Y;
                int x2 = ToolStripMouseDownOrigin.X;
                int y2 = ToolStripMouseDownOrigin.Y;
                double dist = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
                dist = dist * Reportman.Drawing.GraphicUtils.DPIScale;
                if (dist > 5)
                {
                    moving = false;
                    ReleaseCapture();
                    if (WindowState == FormWindowState.Maximized)
                    {
                        Point screenpoint = this.PointToScreen(new Point(e.X, e.Y));
                        int originalWidth = Width;
                        SwitchMaximizeMinimize();
                        Left = screenpoint.X - Width /2;
                        Top = screenpoint.Y;
                        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                    }
                    else
                        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);



                    /*Point newlocation = e.Location;
                    int difx = newlocation.X - toolbarorigin.X;
                    int dify = newlocation.Y - toolbarorigin.Y;
                    SetBounds(Left + difx, Top + dify, Width, Height);*/
                    //toolbarorigin = e.Location;
                }
            }
        }
        Point toolbarorigin;
        bool moving = false;


        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int WM_NCLBUTTONUP = 0xA2;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        Point ToolStripMouseDownOrigin;
        private void ToolStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            if (NoTitleMaximized)
                return;
            ToolStrip nstrip = (ToolStrip)sender;
            Point pclient = e.Location;
            ToolStripMouseDownOrigin = e.Location;

            int nindex = nstrip.Items.IndexOf(botonmin) - 3;
            ToolStripItem lastitem = nstrip.Items[nindex];

            ToolStripItem firstitem = null;
            foreach (ToolStripItem nitem in nstrip.Items)
            {
                if (nitem.Visible)
                {
                    firstitem = nitem;
                    break;
                }
            }
            int leftpos = 0;
            if (firstitem != null)
            {
                leftpos = firstitem.Bounds.Left;
            }
            if (pclient.X < botonmin.Bounds.Left)
            {
                if ((pclient.X > lastitem.Bounds.Right) || (pclient.X<leftpos))
                {
                    
                    toolbarorigin = e.Location;
                    nstrip.Capture = true;
                    moving = true;
                }
            }

        }

        private void Botonclose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Botonmin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Botonmax_Click(object sender, EventArgs e)
        {
            SwitchMaximizeMinimize();
        }
    }
    public interface IDisplayDisable
    {
        bool DisplayDisable();
    }
    public class ToolStripButtonAllowDisable:ToolStripButton,IDisplayDisable
    {
        public bool DisplayDisable()
        {
            return true;
        }
    }
    public class ToolStripDropDownButtonAllowDisable:ToolStripDropDownButton,IDisplayDisable
    {
        public bool DisplayDisable()
        {
            return true;
        }
    }
    class CustomToolStripRenderer:ToolStripProfessionalRenderer
    {
        public ToolStripButton bclose;
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item == bclose)
            {
                if (e.Item.Selected)
                {
                    //e.Item.Image = Properties.Resources.cerrar_19_blanco;
                    e.Graphics.FillRectangle(Brushes.Red, new Rectangle(0, 0, e.Item.Width, e.Item.Height));
                    return;
                }

                //else
                   // e.Item.Image = Properties.Resources.cerrar_19;

            }
            base.OnRenderButtonBackground(e);

        }
        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            base.OnRenderItemCheck(e);
        }
        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderItemBackground(e);
        }
        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
#if DON6
            if (!(e.Item is IDisplayDisable))
            {
                base.OnRenderItemImage(e);
                return;
            }
            else
            {
                if (!((IDisplayDisable)e.Item).DisplayDisable())
                {
                    base.OnRenderItemImage(e);
                    return;
                }

            }
            if (e.Item == bclose)
            {
                if (e.Item.Selected)
                {
                    //e.Item.Image = Properties.Resources.cerrar_19_blanco;
                    Image nimage = Properties.Resources.cerrar_19_blanco;
                    e.Graphics.DrawImage(nimage, new Rectangle(e.Item.Padding.Left, 0, e.Item.Width - e.Item.Padding.Left - e.Item.Padding.Right, e.Item.Height), new Rectangle(0, 0, nimage.Width, nimage.Height), GraphicsUnit.Pixel);
                    return;
                }

            }
            else
            {
                if ((!e.Item.Selected) && (e.Item.Enabled))
                {
                    System.Threading.Monitor.Enter(e.Image);
                    try
                    {
                        //base.OnRenderItemImage(e);
                        // Scale the image before painting it
                        //float xtransform = (float)e.ImageRectangle.Width/e.Image.Width;
                        //float ytransform = (float)e.ImageRectangle.Height/e.Image.Height;
                        using (Bitmap newImage = new Bitmap(e.ImageRectangle.Width, e.ImageRectangle.Height, e.Graphics))
                        {
                            using (Graphics ng = Graphics.FromImage(newImage))
                            {
                                ng.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                ng.DrawImage(e.Item.Image, new Rectangle(0, 0, e.ImageRectangle.Width, e.ImageRectangle.Height));
                            }
                            ControlPaint.DrawImageDisabled(e.Graphics, newImage, e.ImageRectangle.Left,
                                e.ImageRectangle.Top, e.Item.BackColor);
                        }
                        //    e.Graphics.TranslateTransform(e.ImageRectangle.Left, e.ImageRectangle.Top);
                        //e.Graphics.ScaleTransform(xtransform, ytransform);
                        //ControlPaint.DrawImageDisabled(e.Graphics, e.Item.Image, 0,
                        //    0, e.Item.BackColor);
                    }
                    finally
                    {
                        System.Threading.Monitor.Exit(e.Image);
                    }

                    //ControlPaint.DrawImageDisabled(e.Graphics, e.Item.Image, e.ImageRectangle.Left, e.ImageRectangle.Top, e.Item.BackColor);
                    //bool oldEnabled = e.Item.Enabled;
                    //e.Item.Enabled = false;
                    //base.OnRenderItemImage(e);
                    //e.Item.Enabled = oldEnabled;
                    return;
                }
            }
            if (e.Image != null)
            {
                System.Threading.Monitor.Enter(e.Image);
                try
                {
                    base.OnRenderItemImage(e);
                }
                finally
                {
                    System.Threading.Monitor.Exit(e.Image);
                }
            }
            else
            {
                base.OnRenderItemImage(e);
            }
#else
            base.OnRenderItemImage(e);
#endif
        }
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
#if DON6
            if (!(e.Item is IDisplayDisable))
            {
                base.OnRenderItemText(e);
                return;
            }
            else
            {
                if (!((IDisplayDisable)e.Item).DisplayDisable())
                {
                    base.OnRenderItemText(e);
                    return;
                }

            }
            if ((!e.Item.Selected) && (e.Item.Enabled))
            {
                //e.Item.Enabled = false;
                e.TextColor = Color.Gray;
                e.TextFormat = TextFormatFlags.Left;
                base.OnRenderItemText(e);
                //e.Item.Enabled = false;
            }
            else
#endif
                base.OnRenderItemText(e);
        }
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //base.OnRenderToolStripBorder(e);
        }
    }
    class ReSize
    {


        private bool Above, Right, Under, Left, Right_above, Right_under, Left_under, Left_above;




        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="thickness">set thickness of form border</param>
        public ReSize(int thickness,int thicknessarea)
        {
        }

        /// <summary>
        /// Constructor set thickness of form border=1
        /// </summary>
        public ReSize()
        {
        }

        //Get Mouse Position
        public string getMosuePosition(Point mouse, NoTitleForm form)
        {
            int Area = form.BorderSize;
            int Thickness = form.BorderSize;
            bool above_underArea = mouse.X > Area && mouse.X < (form.ClientRectangle.Width-form.Padding.Right) - Area; /* |\AngleArea(Left_Above)\(=======above_underArea========)/AngleArea(Right_Above)/| */ //Area===>(==)
            bool right_left_Area = mouse.Y > Area && mouse.Y < (form.ClientRectangle.Height-form.Padding.Bottom) - Area;

            bool _Above = mouse.Y <= Thickness+form.Padding.Top;  //Mouse in Above All Area
            bool _Right = mouse.X >= (form.ClientRectangle.Width-form.Padding.Left) - Thickness;
            bool _Under = mouse.Y >= (form.ClientRectangle.Height-form.Padding.Bottom) - Thickness;
            bool _Left = mouse.X <= Thickness+form.Padding.Left;

            Above = _Above && (above_underArea); if (Above) return "a";   /*Mouse in Above All Area WithOut Angle Area */
            Right = _Right && (right_left_Area); if (Right) return "r";
            Under = _Under && (above_underArea); if (Under) return "u";
            Left = _Left && (right_left_Area); if (Left) return "l";


            Right_above =/*Right*/ (_Right && (!right_left_Area)) && /*Above*/ (_Above && (!above_underArea)); if (Right_above) return "ra";     /*if Mouse  Right_above */
            Right_under =/* Right*/((_Right) && (!right_left_Area)) && /*Under*/(_Under && (!above_underArea)); if (Right_under) return "ru";     //if Mouse  Right_under 
            Left_under = /*Left*/((_Left) && (!right_left_Area)) && /*Under*/ (_Under && (!above_underArea)); if (Left_under) return "lu";      //if Mouse  Left_under
            Left_above = /*Left*/((_Left) && (!right_left_Area)) && /*Above*/(_Above && (!above_underArea)); if (Left_above) return "la";      //if Mouse  Left_above

            return "";

        }

    }
    /*
    
        [DllImport("user32.dll")]    
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private void toolStrip1_MouseMove(object sender, MouseEventArgs e)
        {
            Point npoint = new Point(e.X, e.Y);
            Point screenpoint = toolStrip1.PointToScreen(npoint);
            IntPtr wparam = new IntPtr(0);
            long lparam = screenpoint.X;
            lparam = lparam + (screenpoint.Y << 16);
            //int x = (int)(m.LParam.ToInt64() & 0xFFFF);               //get x mouse position
            //int y = (int)((m.LParam.ToInt64() & 0xFFFF0000) >> 16);   //get y mouse position  you can gave (x,y) it from "MouseEventArgs" too

            SendMessage(Handle, 0x84, wparam, new IntPtr(lparam));
        }*/

}
