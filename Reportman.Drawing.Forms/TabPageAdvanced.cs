#region Copyright
/* Code based on Magic Library tab control
 * Crownwood.Magic.Controls.TabControl 
 * 
 * 
 * 
 * 
 * 
 * 
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Reportman.Drawing.Forms
{
   public class TabPageAdvanced:PanelAdvanced
   {
              // Enumeration of property change events
        public enum Property
        {
            Title,
            Control,
            ImageIndex,
            ImageList,
            Icon,
            IconFrame,
            Selected,
            CanClose,
            DrawIconHightlight,
            TabWidth,
            TitleAlignment
        }

        // Declare the property change event signature
        public delegate void PropChangeHandler(TabPageAdvanced page, Property prop, object oldValue);

        // Public events
        public event PropChangeHandler PropertyChanged;

        // Instance fields
        protected string _title;
        protected Control _control;
        protected int _imageIndex;
        protected ImageList _imageList;
        protected Image _icon;
        protected bool _selected;
		protected Control _startFocus;
		protected bool _shown;
   
        public TabPageAdvanced()
        {

            InternalConstruct("Page", null, null, -1, null);
        }
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        bool _Composited = true;
        public bool Composited
        {
            get
            {
                return _Composited;
            }
            set
            {
                if (_Composited != value)
                {
                    _Composited = value;
                    int newexstyle = initialparams.ExStyle;
                    if (Composited)
                    {
                        newexstyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                    }
                    int wl = GetWindowLong(this.Handle, newexstyle);
                    SetWindowLong(this.Handle, newexstyle, wl);
                    UpdateStyles();

                }
            }
        }
        CreateParams initialparams;
        protected override CreateParams CreateParams
        {
            get
            {
                initialparams = base.CreateParams;
                CreateParams cp = base.CreateParams;
                if (Composited)
                {
                    cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                }


                //cp.Style = cp.Style | 0x04000000 | 0x02000000; // WS_CLIPSIBLINGS WS_CLIPCHILDREN
                return cp;
            }
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
        
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
        public TabPageAdvanced(string title)
        {
            InternalConstruct(title, null, null, -1, null);
        }
        bool _processing;
        Image oldIcon;
        Image localprogessimage;
        public bool Processing
        {
            get
            {
                return _processing;
            }
            set
            {
                if (_processing != value)
                {
                    if (value)
                    {
                        if (Alerting)
                            Alerting = false;
                        _processing = value;
                        oldIcon = Icon;
                        if (localprogessimage == null)
                            localprogessimage = (Image)Properties.Resources.progress_wheel.Clone();
                        _icon = localprogessimage;
                        OnPropertyChanged(Property.IconFrame, Icon);
                        ImageAnimator.Animate(localprogessimage, OnFrameChanged);
                    }
                    else
                    {
                        _processing = value;
                        ImageAnimator.StopAnimate(localprogessimage, OnFrameChanged);
                        _icon = oldIcon;
                        OnPropertyChanged(Property.Icon, Icon);
                    }
                }
            }
        }
        private void OnFrameChanged(object sender, EventArgs e)
        {
            try
            {
                ImageAnimator.UpdateFrames();
                OnPropertyChanged(Property.IconFrame, Icon);
            }
            catch
            {

            }
        }
        bool _alerting;
        Image localalertingimage;
        Image _AlertingIcon = Properties.Resources.flag_finish;
        
        public Image AlertingIcon
        {
            get
            {
                return _AlertingIcon;
            }
            set
            {
                if (_AlertingIcon != value)
                {
                    _AlertingIcon = value;
                    if (localalertingimage != null)
                    {
                        localalertingimage.Dispose();
                        localalertingimage = null;
                    }
                }
            }
        }
        public static Image DefaultAlertingIcon
        {
            get
            {
                return Properties.Resources.flag_finish;
            }
        }
        public static Image DefaultProgressIcon
        {
            get
            {
                return Properties.Resources.progress_wheel;
            }
        }
        public bool Alerting
        {
            get
            {
                return _alerting;
            }
            set
            {
                if (_alerting != value)
                {
                    if (value)
                    {
                        if (Processing)
                            Processing = false;
                        _alerting = value;
                        oldIcon = Icon;
                        if (localalertingimage == null)
                            localalertingimage = (Image)_AlertingIcon.Clone();
                        _icon = localalertingimage;
                        OnPropertyChanged(Property.IconFrame, Icon);
                        ImageAnimator.Animate(localalertingimage, OnFrameChangedFinish);
                    }
                    else
                    {
                        _alerting = value;
                        ImageAnimator.StopAnimate(localalertingimage, OnFrameChangedFinish);
                        _icon = oldIcon;
                        OnPropertyChanged(Property.IconFrame, Icon);
                    }
                }
            }
        }
        private void OnFrameChangedFinish(object sender, EventArgs e)
        {
            if (Icon == null)
                return;
            ImageAnimator.UpdateFrames();
            OnPropertyChanged(Property.IconFrame, Icon);
        }
        public TabPageAdvanced(string title, Control control)
        {
            InternalConstruct(title, control, null, -1, null);
        }
			
        public TabPageAdvanced(string title, Control control, int imageIndex)
        {
            InternalConstruct(title, control, null, imageIndex, null);
        }

        public TabPageAdvanced(string title, Control control, ImageList imageList, int imageIndex)
        {
            InternalConstruct(title, control, imageList, imageIndex, null);
        }

        public TabPageAdvanced(string title, Control control, Image icon)
        {
            InternalConstruct(title, control, null, -1, icon);
        }

        protected void InternalConstruct(string title, 
                                         Control control, 
                                         ImageList imageList, 
                                         int imageIndex,
                                         Image icon)
        {
            // Assign parameters to internal fields
            _title = title;
            _control = control;
            _imageIndex = imageIndex;
            _imageList = imageList;
            _icon = icon;
            _canClose = true;
            _drawIconHightlight = false;

            // Appropriate defaults
            _selected = false;
			_startFocus = null;
#if DON6
            BackColor = Color.White;
#else
#endif
        }

        [DefaultValue("Page")]
        [Localizable(true)]
        public string Title
        {
            get { return _title; }
		   
            set 
            { 
                if (_title != value)
                {
                    string oldValue = _title;
                    _title = value; 

                    OnPropertyChanged(Property.Title, oldValue);
                }
            }
        }
        bool _canClose;
        [DefaultValue(true)]
        public bool CanClose
        {
            get { return _canClose; }

            set
            {
                if (_canClose != value)
                {
                    bool oldValue = _canClose;
                    _canClose = value;

                    OnPropertyChanged(Property.CanClose, oldValue);
                }
            }
        }
        StringAlignment _TitleAlignment = StringAlignment.Center;
        [DefaultValue(StringAlignment.Center)]
        public StringAlignment TitleAlignment
        {
            get { return _TitleAlignment; }

            set
            {
                if (_TitleAlignment != value)
                {
                    StringAlignment oldValue = _TitleAlignment;
                    _TitleAlignment = value;

                    OnPropertyChanged(Property.TitleAlignment, oldValue);
                }
            }
        }
        int _TabWidth;
        [DefaultValue(0)]
        public int TabWidth
        {
            get { return _TabWidth; }

            set
            {
                if (_TabWidth != value)
                {
                    int oldValue = _TabWidth;
                    _TabWidth = value;

                    OnPropertyChanged(Property.TabWidth, oldValue);
                }
            }
        }



        bool _drawIconHightlight;
        [DefaultValue(false)]
        public bool DrawIconHightlight
        {
            get { return _drawIconHightlight; }

            set
            {
                if (_drawIconHightlight != value)
                {
                    bool oldValue = _drawIconHightlight;
                    _drawIconHightlight = value;

                    OnPropertyChanged(Property.DrawIconHightlight, oldValue);
                }
            }
        }

        [DefaultValue(null)]
        public Control Control
        {
            get { return _control; }

            set 
            { 
                if (_control != value)
                {
                    Control oldValue = _control;
                    _control = value; 

                    OnPropertyChanged(Property.Control, oldValue);
                }
            }
        }

        [DefaultValue(-1)]
        public int ImageIndex
        {
            get { return _imageIndex; }
		
            set 
            { 
                if (_imageIndex != value)
                {
                    int oldValue = _imageIndex;
                    _imageIndex = value; 

                    OnPropertyChanged(Property.ImageIndex, oldValue);
                }
            }
        }

        [DefaultValue(null)]
        public ImageList ImageList
        {
            get { return _imageList; }
		
            set 
            { 
                if (_imageList != value)
                {
                    ImageList oldValue = _imageList;
                    _imageList = value; 

                    OnPropertyChanged(Property.ImageList, oldValue);
                }
            }
        }

        [DefaultValue(null)]
        public Image Icon
        {
            get { return _icon; }
		
            set
            {
                if (_icon != value)
                {
                    if ((Processing) || (Alerting))
                    {
                        oldIcon = value;
                    }
                    else
                    {
                        oldIcon = value;
                        Image oldValue = _icon;
                        _icon = value;

                        OnPropertyChanged(Property.Icon, oldValue);
                    }

                }
            }
        }

        [DefaultValue(true)]
        public bool Selected
        {
            get { return _selected; }

            set
            {
                if (_selected != value)
                {
                    bool oldValue = _selected;
                    _selected = value;

                    OnPropertyChanged(Property.Selected, oldValue);
                }
            }
        }

        [DefaultValue(null)]
        public Control StartFocus
        {
            get { return _startFocus; }
            set { _startFocus = value; }
        }

        public virtual void OnPropertyChanged(Property prop, object oldValue)
        {
            // Any attached event handlers?
            if (PropertyChanged != null)
                PropertyChanged(this, prop, oldValue);
        }
        
        internal bool Shown
        {
            get { return _shown; }
            set { _shown = value; }
        }

   }
}
