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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Reportman.Drawing.Forms
{
    public class CancelArgs
    {
        public bool Cancel = false;
        public TabPageAdvanced Page;
        public CancelArgs(TabPageAdvanced npage)
        {
            Page = npage;
        }
    }
    public delegate void CancelEvent(object sender,CancelArgs args);

   public enum VisualStyle
   {
      IDE = 0,
      Plain = 1,
      Chrome
   }
   public enum Direction
   {
      Vertical = 0,
      Horizontal = 1
   }

   public enum Edge
   {
      Top,
      Left,
      Bottom,
      Right,
      None
   }
    public partial class TabControlAdvanced : Panel
    {
        //      public TabControlAdvanced()
        //      {
        //         InitializeComponent();
        //      }
        public static Image FinishFlag
        {
            get
            {
                return Properties.Resources.flag_finish;
            }
        }
        public static Image ProgresWheel
        {
            get
            {
                return Properties.Resources.progress_wheel;
            }
        }
        // Enumeration of appearance styles
        public enum VisualAppearance
        {
            MultiDocument = 0,
            MultiForm = 1,
            MultiBox = 2
        }
        public Color AlertingColor = Color.FromArgb(200, 150, 150);
        public static TabControlAdvanced GetTabControlAdvanced(TabPageAdvanced npage)
        {
            Control ncontrol = npage.Parent;
            while (ncontrol != null)
            {
                if (ncontrol is TabControlAdvanced)
                {
                    break;
                }
                else
                {
                    ncontrol = ncontrol.Parent;
                }
            }
            if (ncontrol == null)
                return null;
            else
                return (TabControlAdvanced)ncontrol;
        }
        // Enumeration of modes that control display of the tabs area
        public enum HideTabsModes
        {
            ShowAlways,
            HideAlways,
            HideUsingLogic,
            HideWithoutMouse
        }

        // Indexes into the menu images strip
        protected enum ImageStrip
        {
            LeftEnabled = 0,
            LeftDisabled = 1,
            RightEnabled = 2,
            RightDisabled = 3,
            Close = 4,
            Error = 5,
            DropDown = 6
        }

        // Enumeration of Indexes into positioning constants array
        protected enum PositionIndex
        {
            BorderTop = 0,
            BorderLeft = 1,
            BorderBottom = 2,
            BorderRight = 3,
            ImageGapTop = 4,
            ImageGapLeft = 5,
            ImageGapBottom = 6,
            ImageGapRight = 7,
            TextOffset = 8,
            TextGapLeft = 9,
            TabsBottomGap = 10,
            ButtonOffset = 11,
        }

        // Helper class for handling multiline calculations
        protected class MultiRect
        {
            protected Rectangle _rect;
            protected int _index;

            public MultiRect(Rectangle rect, int index)
            {
                _rect = rect;
                _index = index;
            }

            public int Index
            {
                get { return _index; }
            }

            public Rectangle Rect
            {
                get { return _rect; }
                set { _rect = value; }
            }

            public int X
            {
                get { return _rect.X; }
                set { _rect.X = value; }
            }

            public int Y
            {
                get { return _rect.Y; }
                set { _rect.Y = value; }
            }

            public int Width
            {
                get { return _rect.Width; }
                set { _rect.Width = value; }
            }

            public int Height
            {
                get { return _rect.Height; }
                set { _rect.Height = value; }
            }
        }
        protected class HostPanel : Panel
        {
            public HostPanel()
            {
                // Prevent flicker with double buffering and all painting inside WM_PAINT
                SetStyle(ControlStyles.DoubleBuffer, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.UserPaint, true);
            }

            protected override void OnResize(EventArgs e)
            {
                // Update size of each child to match ourself
                foreach (Control c in this.Controls)
                    c.Size = this.Size;

                base.OnResize(e);
            }
        }


        // Class constants for sizing/positioning each style
        protected static int[,] _position = {
                                                {3, 1, 1, 1, 1, 2, 1, 1, 2, 1, 3, 2},	// IDE
                                                {6, 2, 2, 3, 3, 1, 1, 0, 1, 1, 2, 0},   // Plain

                                                //                                                {3, 1, 1, 1, 1, 2, 1, 1, 2, 1, 3, 2}	// Chrome
#if DON6
                                                {3, 1, 1, 1, 1, 6, 1, -4, 2, 1, 1, 0}   // Chrome
#else
                                                {3, 1, 1, 1, 1, 1, 1, 0, 2, 1, 1, 0}   // Chrome
#endif
        };

        // Class constants
        protected static int _plainBorder = 3;
        protected static int _plainBorderDouble = 6;
        protected static int _tabsAreaStartInset = 5;
        protected static int _tabsAreaEndInset = 5;
        protected static float _alphaIDE = 1.5F;
        protected static int _buttonGap = 3;
        protected static int _buttonWidth = 14;
        protected static int _buttonHeight = 14;
        protected static int _unScaledButtonWidth = 14;
        protected static int _originalButtonWidth = 14;
        protected static int _originalButtonHeight = 14;
        protected static int _imageButtonWidth = 12;
        protected static int _imageButtonHeight = 12;
        protected static int _multiBoxAdjust = 2;
        protected readonly Rectangle _nullPosition = new Rectangle(-999, -999, 0, 0);

        static TabControlAdvanced()
        {
            _buttonWidth = Convert.ToInt32(_buttonWidth * Reportman.Drawing.GraphicUtils.DPIScale);
            _buttonHeight = Convert.ToInt32(_buttonHeight * Reportman.Drawing.GraphicUtils.DPIScale);
            _buttonGap = Convert.ToInt32(_buttonGap * Reportman.Drawing.GraphicUtils.DPIScale);

            _originalButtonHeight = Convert.ToInt32(_originalButtonHeight * Reportman.Drawing.GraphicUtils.DPIScale);
            _originalButtonWidth = Convert.ToInt32(_originalButtonWidth * Reportman.Drawing.GraphicUtils.DPIScale);
            _imageButtonHeight = Convert.ToInt32(_imageButtonHeight * Reportman.Drawing.GraphicUtils.DPIScale);
            _imageButtonWidth = Convert.ToInt32(_imageButtonWidth * Reportman.Drawing.GraphicUtils.DPIScale);

            _MouseOffsetTriggerReorder = Convert.ToInt32(5 * Reportman.Drawing.GraphicUtils.DPIScale);
            //_imageButtonWidth = Convert.ToInt32(_imageButtonWidth * Reportman.Drawing.GraphicUtils.DPIScale);
            //_imageButtonHeight = Convert.ToInt32(_imageButtonHeight * Reportman.Drawing.GraphicUtils.DPIScale);

            // Create a strip of images by loading an embedded bitmap resource
            _internalImages = new ImageList();
            _internalImages.ImageSize = new Size(_imageButtonWidth, _imageButtonHeight);
            _internalImages.ImageSize = new Size(_imageButtonWidth, _imageButtonHeight);
            _internalImages.Images.Add(Properties.Resources.left_1);
            _internalImages.Images.Add(Properties.Resources.left_2_empty);
            _internalImages.Images.Add(Properties.Resources.right_3);
            _internalImages.Images.Add(Properties.Resources.right_4_empty);
            _internalImages.Images.Add(Properties.Resources.close_5);
            _internalImages.Images.Add(Properties.Resources.close_6);
            _internalImages.Images.Add(Properties.Resources.dropdown_7);

        }

        // Class state
        protected static ImageList _internalImages;

        // Instance fields - size/positioning
        protected int _textHeight;
        int _imageWidth;
        int _imageHeight;

        public int ImageWidth
        {
            get
            {
                return _imageWidth;
            }
            set
            {
                _imageWidth = value;
                Invalidate();
            }
        }
        bool _autoShrinkPages;
        public bool AutoShrinkPages
        {
            get
            {
                return _autoShrinkPages;
            }
            set
            {
                _autoShrinkPages = value;
                Recalculate();
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                //cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED


                //cp.Style = cp.Style | 0x04000000 | 0x02000000; // WS_CLIPSIBLINGS WS_CLIPCHILDREN
                return cp;
            }
        }
#if DON6
        public bool EmptyMoveForm = true;
#else
        public bool EmptyMoveForm = false;
#endif

        bool _autoHidePaging;
        public bool AutoHidePaging
        {
            get
            {
                return _autoHidePaging;
            }
            set
            {
                _autoHidePaging = value;
                Recalculate();
            }
        }
        int _autoShrinkMinimum;
        public int AutoShrinkMinimum
        {
            get
            {
                return _autoShrinkMinimum;
            }
            set
            {
                _autoShrinkMinimum = value;
                Recalculate();
            }
        }
        public int ImageMargin = 0;
        public int ImageHeight
        {
            get
            {
                return _imageHeight;
            }
            set
            {
                _imageHeight = value;
                Invalidate();
            }
        }

        protected int _imageGapTopExtra;
        protected int _imageGapBottomExtra;
        protected Rectangle _pageRect;
        protected Rectangle _pageAreaRect;
        protected Rectangle _tabsAreaRect;

        // Instance fields - state
        protected int _ctrlTopOffset;			// How far from top edge embedded controls should offset
        protected int _ctrlLeftOffset;			// How far from left edge embedded controls should offset
        protected int _ctrlRightOffset;			// How far from right edgeembedded controls should offset
        protected int _ctrlBottomOffset;	    // How far from bottom edge embedded controls should offset
        protected int _styleIndex;				// Index into position array
        protected int _pageSelected;			// index of currently selected page (-1 is none)
        protected int _startPage;				// index of first page to draw, used when scrolling pages
        protected int _hotTrackPage;			// which page is currently displayed as being tracked
        protected bool _hotTrackPageHightlightClose;			// close is displayed
        protected int _topYPos;                 // Y position of first line in multiline mode
        protected int _bottomYPos;              // Y position of last line in multiline mode
        protected int _leaveTimeout;            // How long from leaving to timeout occuring
        protected bool _dragFromControl;        // Must drag away from whole control before drag events generated
        protected bool _mouseOver;              // Mouse currently over the control (or child pages)
        protected bool _multiline;              // should tabs that cannot fit on a line create new lines
        protected bool _multilineFullWidth;     // when in multiline mode, all lines are extended to end 
        protected bool _shrinkPagesToFit;		// pages are shrunk so they all fit in control width
        protected bool _changed;				// Flag for use when updating contents of collection
        protected bool _positionAtTop;			// display tabs at top or bottom of the control
        protected bool _showClose;				// should the close button be displayed
        protected bool _showDropDown;				// should the dropdown tabs be displayed
        protected bool _showCloseIndividual;				// should the close button be displayed
        protected bool _showArrows;				// should then scroll arrow be displayed
        protected bool _insetPlain;				// Show the inset border for controls
        protected bool _insetBorderPagesOnly;   // Remove the border entirely for Plain mode
        protected bool _selectedTextOnly;	    // Only draw text for selected tab
        protected bool _rightScroll;			// Should the right scroll button be enabled
        protected bool _leftScroll;				// Should the left scroll button be enabled
        protected bool _dimUnselected;			// should unselected pages be drawn slightly dimmed
        protected bool _boldSelected;			// should selected page use a bold font
        protected bool _hotTrack;				// should mouve moving over text hot track it
        protected bool _hoverSelect;			// select a page when he mouse hovers over it
        protected bool _recalculate;			// flag to indicate recalculation is needed before painting
        protected bool _leftMouseDown;			// Is the left mouse button down
        protected bool _leftMouseDownDrag;		// Has a drag operation begun
        protected bool _ignoreDownDrag;         // When pressed the left button cannot generate two drags
        protected bool _defaultColor;			// Is the background color the default one?
        protected bool _defaultFont;            // Is the Font the default one?
        protected bool _recordFocus;			// Record the control with focus when leaving a page
        protected bool _idePixelArea;           // Place a one pixel border at top/bottom of tabs area
        protected bool _idePixelBorder;         // Place a one pixel border around control
        protected ContextMenuStrip _contextMenu;       // Context menu to show on right mouse up
        protected Point _leftMouseDownPos;		// Initial mouse down position for left mouse button
        protected Color _hotTextColor;			// color for use when drawing text as hot
        protected Color _textColor;				// color for use when text not hot
        protected Color _textInactiveColor;	    // color for use when text not hot and not the active tab
        protected Color _backIDE;				// background drawing color when in IDE appearance
        protected Color _buttonActiveColor;		// color for drawing buttons images when active
        protected Color _buttonInactiveColor;	// color for drawing buttons images when inactive
        protected Color _backLight;				// light variation of the back color
        protected Color _backLightLight;		// lightlight variation of the back color
        protected Color _backDark;				// dark variation of the back color
        protected Color _backDarkDark;			// darkdark variation of the back color
        protected VisualStyle _style;			// which style of use 
        protected HideTabsModes _hideTabsMode;  // Decide when to hide/show tabs area
        protected Timer _overTimer;             // Time when mouse has left control
        protected HostPanel _hostPanel;         // Hosts the page instance control/form
        protected VisualAppearance _appearance;	// which appearance style
        protected ImageList _imageList;			// collection of images for use is tabs
        protected ArrayList _tabRects;			// display rectangles for associated page
        protected TabPageCollection _tabPages;	// collection of pages

        // Instance fields - buttons
        protected InertButton _closeButton;
        protected InertButton _dropDownButton;
        protected InertButton _leftArrow;
        protected InertButton _rightArrow;

        public delegate void DoubleClickTabHandler(TabControlAdvanced sender, TabPageAdvanced page);

        // Exposed events
        public event CancelEvent ClosePressed;
        public event CancelEvent SelectionChanging;
        public event EventHandler SelectionChanged;
        public event EventHandler PageGotFocus;
        public event EventHandler PageLostFocus;
        public event CancelEventHandler PopupMenuDisplay;
        public event MouseEventHandler PageDragStart;
        public event MouseEventHandler PageDragMove;
        public event MouseEventHandler PageDragEnd;
        public event MouseEventHandler PageDragQuit;
        public event DoubleClickTabHandler DoubleClickTab;


        public TabControlAdvanced()
        {

            InitializeComponent();

            // Prevent flicker with double buffering and all painting inside WM_PAINT
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);

            // Create collections
            _tabRects = new ArrayList();
            _tabPages = new TabPageCollection();

            // Hookup to collection events
            _tabPages.Clearing += new CollectionClear(OnClearingPages);
            _tabPages.Cleared += new CollectionClear(OnClearedPages);
            _tabPages.Inserting += new CollectionChange(OnInsertingPage);
            _tabPages.Inserted += new CollectionChange(OnInsertedPage);
            _tabPages.Removing += new CollectionChange(OnRemovingPage);
            _tabPages.Removed += new CollectionChange(OnRemovedPage);

            // Define the default state of the control
            _startPage = -1;
            _pageSelected = -1;
            _hotTrackPage = -1;
            _imageList = null;
            _insetPlain = true;
            _multiline = false;
            _multilineFullWidth = false;
            _dragFromControl = true;
            _mouseOver = false;
            _leftScroll = false;
            _defaultFont = true;
            _defaultColor = true;
            _rightScroll = false;
            _hoverSelect = false;
            _leftMouseDown = false;
            _ignoreDownDrag = true;
            _selectedTextOnly = false;
            _leftMouseDownDrag = false;
            _insetBorderPagesOnly = false;
            _hideTabsMode = HideTabsModes.ShowAlways;
            _recordFocus = true;
            _styleIndex = 1;
            _leaveTimeout = 200;
            _ctrlTopOffset = 0;
            _ctrlLeftOffset = 0;
            _ctrlRightOffset = 0;
            _ctrlBottomOffset = 0;
            _style = VisualStyle.IDE;
            _buttonActiveColor = Color.FromArgb(128, this.ForeColor);
            _buttonInactiveColor = _buttonActiveColor;
            _textColor = TabControlAdvanced.DefaultForeColor;
            //_textInactiveColor = Color.FromArgb(128, _textColor);
            _textInactiveColor = SystemColors.InactiveCaptionText;
            _hotTextColor = SystemColors.Highlight;

            // Create the panel that hosts each page control. This is done to prevent the problem where a 
            // hosted Control/Form has 'AutoScaleBaseSize' defined. In which case our attempt to size it the
            // first time is ignored and the control sizes itself to big and would overlap the tabs area.
            _hostPanel = new HostPanel();
            _hostPanel.Location = new Point(-1, -1);
            _hostPanel.Size = new Size(0, 0);
            _hostPanel.MouseEnter += new EventHandler(OnPageMouseEnter);
            _hostPanel.MouseLeave += new EventHandler(OnPageMouseLeave);

            // Create hover buttons
            _closeButton = new InertButton(_internalImages, (int)ImageStrip.Close);
            _dropDownButton = new InertButton(_internalImages, (int)ImageStrip.DropDown);
            _leftArrow = new InertButton(_internalImages, (int)ImageStrip.LeftEnabled, (int)ImageStrip.LeftDisabled);
            _rightArrow = new InertButton(_internalImages, (int)ImageStrip.RightEnabled, (int)ImageStrip.RightDisabled);

            // We want our buttons to have very thin borders
            _closeButton.BorderWidth = _leftArrow.BorderWidth = _rightArrow.BorderWidth = 1;
            _dropDownButton.BorderWidth = 1;

            // Hookup to the button events
            _closeButton.Click += new EventHandler(OnCloseButton);
            _dropDownButton.Click += new EventHandler(OnDropDownButton);
            _leftArrow.Click += new EventHandler(OnLeftArrow);
            _rightArrow.Click += new EventHandler(OnRightArrow);


            int arrowsize;
            // Set their fixed sizes
            //_originalButtonWidth = Convert.ToInt32(_originalButtonWidth *1.25);
            //_originalButtonHeight = Convert.ToInt32(_originalButtonHeight *1.25);
            bool dpiAware = WinFormsGraphics.IsWindowsFormsDPIAware();

            if (dpiAware)
                arrowsize = Convert.ToInt32(_unScaledButtonWidth);
            else
                arrowsize = Convert.ToInt32(_originalButtonWidth);
            _leftArrow.Size = _rightArrow.Size = _closeButton.Size = _dropDownButton.Size = new Size(arrowsize, arrowsize);

            // Add child controls
            Controls.AddRange(new Control[] { _closeButton, _leftArrow, _rightArrow, _hostPanel, _dropDownButton });
            


            // Grab some contant values
            _imageWidth = Convert.ToInt32(16 * Reportman.Drawing.GraphicUtils.DPIScale);
            _imageHeight = Convert.ToInt32(16 * Reportman.Drawing.GraphicUtils.DPIScale);

            // Default to having a MultiForm usage
            SetAppearance(VisualAppearance.MultiForm);

            // Need a timer so that when the mouse leaves, a fractionaly delay occurs before
            // noticing and hiding the tabs area when the appropriate style is set
            _overTimer = new Timer();
            _overTimer.Interval = _leaveTimeout;
            _overTimer.Tick += new EventHandler(OnMouseTick);

            // Need notification when the MenuFont is changed
            Microsoft.Win32.SystemEvents.UserPreferenceChanged +=
                new UserPreferenceChangedEventHandler(OnPreferenceChanged);

            // Define the default Font, BackColor and Button images
            DefineFont(SystemInformation.MenuFont);
            DefineBackColor(SystemColors.Control);
            DefineButtonImages();

            Recalculate();
        }


        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual TabPageCollection TabPages
        {
            get { return _tabPages; }
        }

        [Category("Appearance")]
        public override Font Font
        {
            get { return base.Font; }

            set
            {
                if (value != null)
                {
                    if (value != base.Font)
                    {
                        _defaultFont = (value == SystemInformation.MenuFont);

                        DefineFont(value);

                        _recalculate = true;
                        Invalidate();
                    }
                }
            }
        }

        private bool ShouldSerializeFont()
        {
            return !_defaultFont;
        }

        [Category("Appearance")]
        public override Color ForeColor
        {
            get { return _textColor; }

            set
            {
                if (_textColor != value)
                {
                    _textColor = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        private bool ShouldSerializeForeColor()
        {
            return _textColor != TabControlAdvanced.DefaultForeColor;
        }

        [Category("Appearance")]
        public override Color BackColor
        {
            get { return base.BackColor; }

            set
            {
                if (this.BackColor != value)
                {
                    _defaultColor = (value == SystemColors.Control);

                    DefineBackColor(value);

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        private bool ShouldSerializeBackColor()
        {
            return this.BackColor != SystemColors.Control;
        }

        [Category("Appearance")]
        public virtual Color ButtonActiveColor
        {
            get { return _buttonActiveColor; }

            set
            {
                if (_buttonActiveColor != value)
                {
                    _buttonActiveColor = value;
                    DefineButtonImages();
                }
            }
        }

        private bool ShouldSerializeButtonActiveColor()
        {
            return _buttonActiveColor != Color.FromArgb(128, this.ForeColor);
        }

        public void ResetButtonActiveColor()
        {
            ButtonActiveColor = Color.FromArgb(128, this.ForeColor);
        }

        [Category("Appearance")]
        public virtual Color ButtonInactiveColor
        {
            get { return _buttonInactiveColor; }

            set
            {
                if (_buttonInactiveColor != value)
                {
                    _buttonInactiveColor = value;
                    DefineButtonImages();
                }
            }
        }

        private bool ShouldSerializeButtonInactiveColor()
        {
            return _buttonInactiveColor != Color.FromArgb(128, this.ForeColor);
        }

        public void ResetButtonInactiveColor()
        {
            ButtonInactiveColor = Color.FromArgb(128, this.ForeColor);
        }

        [Category("Appearance")]
        [DefaultValue(typeof(VisualAppearance), "MultiForm")]
        public virtual VisualAppearance Appearance
        {
            get { return _appearance; }

            set
            {
                if (_appearance != value)
                {
                    SetAppearance(value);

                    Recalculate();
                    Invalidate();
                }
            }
        }

        public void ResetAppearance()
        {
            Appearance = VisualAppearance.MultiForm;
        }

        [Category("Appearance")]
        [DefaultValue(typeof(VisualStyle), "IDE")]
        public virtual VisualStyle Style
        {
            get { return _style; }

            set
            {
                if (_style != value)
                {
                    _style = value;

                    // Define the correct style indexer
                    SetStyleIndex();

                    Recalculate();
                    Invalidate();
                }
            }
        }

        public void ResetStyle()
        {
            Style = VisualStyle.IDE;
        }

        [Category("Behavour")]
        public virtual ContextMenuStrip ContextPopupMenu
        {
            get { return _contextMenu; }
            set { _contextMenu = value; }
        }

        protected bool ShouldSerializeContextPopupMenu()
        {
            return _contextMenu != null;
        }

        public void ResetContextPopupMenu()
        {
            ContextPopupMenu = null;
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public virtual bool HotTrack
        {
            get { return _hotTrack; }

            set
            {
                if (_hotTrack != value)
                {
                    _hotTrack = value;

                    if (!_hotTrack)
                    {
                        _hotTrackPage = -1;
                    }

                    _recalculate = true;
                    Invalidate();
                }
            }
        }
        private bool _allowTabReordering = false;
        private bool _reorderingtab;
        private static int _MouseOffsetTriggerReorder;
        [Category("Appearance")]
        [DefaultValue(false)]
        public virtual bool AllowTabReordering
        {
            get { return _allowTabReordering; }

            set
            {
                if (_allowTabReordering != value)
                {
                    _allowTabReordering = value;
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool AllowLastTabReordering = false;


        public void ResetHotTrack()
        {
            HotTrack = false;
        }

        [Category("Appearance")]
        public virtual Color HotTextColor
        {
            get { return _hotTextColor; }

            set
            {
                if (_hotTextColor != value)
                {
                    _hotTextColor = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        private bool ShouldSerializeHotTextColor()
        {
            return _hotTextColor != SystemColors.ActiveCaption;
        }

        public void ResetHotTextColor()
        {
            HotTextColor = SystemColors.ActiveCaption;
        }

        [Category("Appearance")]
        public virtual Color TextColor
        {
            get { return _textColor; }

            set
            {
                if (_textColor != value)
                {
                    _textColor = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        private bool ShouldSerializeTextColor()
        {
            return _textColor != TabControlAdvanced.DefaultForeColor;
        }

        public void ResetTextColor()
        {
            TextColor = TabControlAdvanced.DefaultForeColor;
        }

        [Category("Appearance")]
        public virtual Color TextInactiveColor
        {
            get { return _textInactiveColor; }

            set
            {
                if (_textInactiveColor != value)
                {
                    _textInactiveColor = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        private bool ShouldSerializeTextInactiveColor()
        {
            return _textInactiveColor != Color.FromArgb(128, TabControlAdvanced.DefaultForeColor);
        }

        public void TextTextInactiveColor()
        {
            TextInactiveColor = Color.FromArgb(128, TabControlAdvanced.DefaultForeColor);
        }

        [Browsable(false)]
        public virtual Rectangle TabsAreaRect
        {
            get { return _tabsAreaRect; }
        }

        [Category("Appearance")]
        public virtual ImageList ImageList
        {
            get { return _imageList; }

            set
            {
                if (_imageList != value)
                {
                    _imageList = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        private bool ShouldSerializeImageList()
        {
            return _imageList != null;
        }

        public void ResetImageList()
        {
            ImageList = null;
        }

        [Category("Appearance")]
        public virtual bool PositionTop
        {
            get { return _positionAtTop; }

            set
            {
                if (_positionAtTop != value)
                {
                    _positionAtTop = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        protected bool ShouldSerializePositionTop()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    return _positionAtTop != false;
                case VisualAppearance.MultiDocument:
                default:
                    return _positionAtTop != true;
            }
        }

        public void ResetPositionTop()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    PositionTop = false;
                    break;
                case VisualAppearance.MultiDocument:
                default:
                    PositionTop = true;
                    break;
            }
        }

        [Category("Appearance")]
        public virtual bool ShowClose
        {
            get { return _showClose; }

            set
            {
                if (_showClose != value)
                {
                    _showClose = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }
        [Category("Appearance")]
        public virtual bool ShowDropDown
        {
            get { return _showDropDown; }

            set
            {
                if (_showDropDown != value)
                {
                    _showDropDown = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }
        [Category("Appearance")]
        public virtual bool ShowCloseIndividual
        {
            get { return _showCloseIndividual; }

            set
            {
                if (_showCloseIndividual != value)
                {
                    _showCloseIndividual = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        protected bool ShouldSerializeShowClose()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    return _showClose != false;
                case VisualAppearance.MultiDocument:
                default:
                    return _showClose != true;
            }
        }

        public void ResetShowClose()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    ShowClose = false;
                    break;
                case VisualAppearance.MultiDocument:
                default:
                    ShowClose = true;
                    break;
            }
        }

        [Category("Appearance")]
        public virtual bool ShowArrows
        {
            get { return _showArrows; }

            set
            {
                if (_showArrows != value)
                {
                    _showArrows = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        protected bool ShouldSerializeShowArrows()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    return _showArrows != false;
                case VisualAppearance.MultiDocument:
                default:
                    return _showArrows != true;
            }
        }

        public void ResetShowArrows()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    ShowArrows = false;
                    break;
                case VisualAppearance.MultiDocument:
                default:
                    ShowArrows = true;
                    break;
            }
        }

        [Category("Appearance")]
        public virtual bool ShrinkPagesToFit
        {
            get { return _shrinkPagesToFit; }

            set
            {
                if (_shrinkPagesToFit != value)
                {
                    _shrinkPagesToFit = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        protected bool ShouldSerializeShrinkPagesToFit()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    return _shrinkPagesToFit != true;
                case VisualAppearance.MultiDocument:
                default:
                    return _shrinkPagesToFit != false;
            }
        }

        public void ResetShrinkPagesToFit()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    ShrinkPagesToFit = true;
                    break;
                case VisualAppearance.MultiDocument:
                default:
                    ShrinkPagesToFit = false;
                    break;
            }
        }

        [Category("Appearance")]
        public virtual bool BoldSelectedPage
        {
            get { return _boldSelected; }

            set
            {
                if (_boldSelected != value)
                {
                    _boldSelected = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        protected bool ShouldSerializeBoldSelectedPage()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    return _boldSelected != false;
                case VisualAppearance.MultiDocument:
                default:
                    return _boldSelected != true;
            }
        }

        public void ResetBoldSelectedPage()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    BoldSelectedPage = false;
                    break;
                case VisualAppearance.MultiDocument:
                default:
                    BoldSelectedPage = true;
                    break;
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public virtual bool MultilineFullWidth
        {
            get { return _multilineFullWidth; }

            set
            {
                if (_multilineFullWidth != value)
                {
                    _multilineFullWidth = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        public void ResetMultilineFullWidth()
        {
            MultilineFullWidth = false;
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public virtual bool Multiline
        {
            get { return _multiline; }

            set
            {
                if (_multiline != value)
                {
                    _multiline = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        public void ResetMultiline()
        {
            Multiline = false;
        }

        [Category("Appearance")]
        [DefaultValue(0)]
        public virtual int ControlLeftOffset
        {
            get { return _ctrlLeftOffset; }

            set
            {
                if (_ctrlLeftOffset != value)
                {
                    _ctrlLeftOffset = value;

                    Recalculate();
                    Invalidate();
                }
            }
        }

        public void ResetControlLeftOffset()
        {
            ControlLeftOffset = 0;
        }

        [Category("Appearance")]
        [DefaultValue(0)]
        public virtual int ControlTopOffset
        {
            get { return _ctrlTopOffset; }

            set
            {
                if (_ctrlTopOffset != value)
                {
                    _ctrlTopOffset = value;

                    Recalculate();
                    Invalidate();
                }
            }
        }

        public void ResetControlTopOffset()
        {
            ControlTopOffset = 0;
        }

        [Category("Appearance")]
        [DefaultValue(0)]
        public virtual int ControlRightOffset
        {
            get { return _ctrlRightOffset; }

            set
            {
                if (_ctrlRightOffset != value)
                {
                    _ctrlRightOffset = value;

                    Recalculate();
                    Invalidate();
                }
            }
        }

        public void ResetControlRightOffset()
        {
            ControlRightOffset = 0;
        }

        [Category("Appearance")]
        [DefaultValue(0)]
        public virtual int ControlBottomOffset
        {
            get { return _ctrlBottomOffset; }

            set
            {
                if (_ctrlBottomOffset != value)
                {
                    _ctrlBottomOffset = value;

                    Recalculate();
                    Invalidate();
                }
            }
        }

        public void ResetControlBottomOffset()
        {
            ControlBottomOffset = 0;
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public virtual bool InsetPlain
        {
            get { return _insetPlain; }

            set
            {
                if (_insetPlain != value)
                {
                    _insetPlain = value;

                    Recalculate();
                    Invalidate();
                }
            }
        }

        public void ResetInsetPlain()
        {
            InsetPlain = true;
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public virtual bool InsetBorderPagesOnly
        {
            get { return _insetBorderPagesOnly; }

            set
            {
                if (_insetBorderPagesOnly != value)
                {
                    _insetBorderPagesOnly = value;

                    Recalculate();
                    Invalidate();
                }
            }
        }

        public void ResetInsetBorderPagesOnly()
        {
            InsetBorderPagesOnly = true;
        }

        [Category("Appearance")]
        public virtual bool IDEPixelBorder
        {
            get { return _idePixelBorder; }

            set
            {
                if (_idePixelBorder != value)
                {
                    _idePixelBorder = value;

                    Recalculate();
                    Invalidate();
                }
            }
        }

        protected bool ShouldSerializeIDEPixelBorder()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    return _idePixelBorder != false;
                case VisualAppearance.MultiDocument:
                default:
                    return _idePixelBorder != true;
            }
        }

        public void ResetIDEPixelBorder()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                case VisualAppearance.MultiForm:
                    IDEPixelBorder = false;
                    break;
                case VisualAppearance.MultiDocument:
                default:
                    IDEPixelBorder = true;
                    break;
            }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public virtual bool IDEPixelArea
        {
            get { return _idePixelArea; }

            set
            {
                if (_idePixelArea != value)
                {
                    _idePixelArea = value;

                    Recalculate();
                    Invalidate();
                }
            }
        }

        public void ResetIDEPixelArea()
        {
            IDEPixelArea = true;
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public virtual bool SelectedTextOnly
        {
            get { return _selectedTextOnly; }

            set
            {
                if (_selectedTextOnly != value)
                {
                    _selectedTextOnly = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        public void ResetSelectedTextOnly()
        {
            SelectedTextOnly = false;
        }

        [Category("Behavour")]
        [DefaultValue(200)]
        public int MouseLeaveTimeout
        {
            get { return _leaveTimeout; }

            set
            {
                if (_leaveTimeout != value)
                {
                    _leaveTimeout = value;
                    _overTimer.Interval = value;
                }
            }
        }

        public void ResetMouseLeaveTimeout()
        {
            _leaveTimeout = 200;
        }

        [Category("Behavour")]
        [DefaultValue(true)]
        public bool DragFromControl
        {
            get { return _dragFromControl; }
            set { _dragFromControl = value; }
        }

        public void ResetDragFromControl()
        {
            DragFromControl = true;
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public virtual HideTabsModes HideTabsMode
        {
            get { return _hideTabsMode; }

            set
            {
                if (_hideTabsMode != value)
                {
                    _hideTabsMode = value;

                    Recalculate();
                    Invalidate();
                }
            }
        }

        protected bool ShouldSerializeHideTabsMode()
        {
            return HideTabsMode != HideTabsModes.ShowAlways;
        }

        public void ResetHideTabsMode()
        {
            HideTabsMode = HideTabsModes.ShowAlways;
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public virtual bool HoverSelect
        {
            get { return _hoverSelect; }

            set
            {
                if (_hoverSelect != value)
                {
                    _hoverSelect = value;

                    _recalculate = true;
                    Invalidate();
                }
            }
        }

        public void ResetHoverSelect()
        {
            HoverSelect = false;
        }

        [Category("Behavour")]
        [DefaultValue(true)]
        public virtual bool RecordFocus
        {
            get { return _recordFocus; }

            set
            {
                if (_recordFocus != value)
                    _recordFocus = value;
            }
        }

        public void ResetRecordFocus()
        {
            RecordFocus = true;
        }

        [Browsable(false)]
        [DefaultValue(-1)]
        public virtual int SelectedIndex
        {
            get { return _pageSelected; }

            set
            {
                if ((value >= 0) && (value < _tabPages.Count))
                {
                    if (_pageSelected != value)
                    {
                        // Raise selection changing event
                        CancelArgs args = new CancelArgs(_tabPages[value]);
                        OnSelectionChanging(this, args);
                        if (args.Cancel)
                            throw new Exception("Can not change tab, cancelled");
                        // Any page currently selected?
                        if (_pageSelected != -1)
                            DeselectPage(_tabPages[_pageSelected]);

                        _pageSelected = value;

                        if (_pageSelected != -1)
                        {
                            SelectPage(_tabPages[_pageSelected]);

                            // If newly selected page is scrolled off the left hand side
                            if (_pageSelected < _startPage)
                                _startPage = _pageSelected;  // then bring it into view
                        }

                        // Change in selection causes tab pages sizes to change
                        if (_boldSelected)
                        {
                            Recalculate();
                            Invalidate();
                        }

                        // Raise selection change event
                        OnSelectionChanged(EventArgs.Empty);

                        Invalidate();
                    }
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public virtual TabPageAdvanced SelectedTab
        {
            get
            {
                // If nothing is selected we return null
                if (_pageSelected == -1)
                    return null;
                else
                    return _tabPages[_pageSelected];
            }

            set
            {
                // Cannot change selection to be none of the tabs
                if (value != null)
                {
                    // Get the requested page from the collection
                    int index = _tabPages.IndexOf(value);

                    // If a valid known page then using existing property to perform switch
                    if (index != -1)
                        this.SelectedIndex = index;
                }
            }
        }

        public virtual void MakePageVisible(TabPageAdvanced page)
        {
            MakePageVisible(_tabPages.IndexOf(page));
        }

        public virtual void MakePageVisible(int index)
        {
            // Only relevant if we do not shrink all pages to fit and not in multiline
            if (!_shrinkPagesToFit && !_multiline)
            {
                // Range check the request page
                if ((index >= 0) && (index < _tabPages.Count))
                {
                    // Is requested page before those shown?
                    if (index < _startPage)
                    {
                        // Define it as the new start page
                        _startPage = index;

                        _recalculate = true;
                        Invalidate();
                    }
                    else
                    {
                        // Find the last visible position
                        int xMax = GetMaximumDrawPos();

                        Rectangle rect = (Rectangle)_tabRects[index];

                        // Is the page drawn off over the maximum position?
                        if (rect.Right >= xMax)
                        {
                            // Need to find the new start page to bring this one into view
                            int newStart = index;

                            // Space left over for other tabs to be drawn inside
                            int spaceLeft = xMax - rect.Width - _tabsAreaRect.Left - _tabsAreaStartInset;

                            do
                            {
                                // Is there a previous tab to check?
                                if (newStart == 0)
                                    break;

                                Rectangle rectStart = (Rectangle)_tabRects[newStart - 1];

                                // Is there enough space to draw it?
                                if (rectStart.Width > spaceLeft)
                                    break;

                                // Move to new tab and reduce available space left
                                newStart--;
                                spaceLeft -= rectStart.Width;

                            } while (true);

                            // Define the new starting page
                            _startPage = newStart;

                            _recalculate = true;
                            Invalidate();
                        }
                    }
                }
            }
        }

        protected override bool ProcessMnemonic(char key)
        {
            int total = _tabPages.Count;
            int index = this.SelectedIndex + 1;

            for (int count = 0; count < total; count++, index++)
            {
                // Range check the index
                if (index >= total)
                    index = 0;

                TabPageAdvanced page = _tabPages[index];

                // Find position of first mnemonic character
                int position = page.Title.IndexOf('&');

                // Did we find a mnemonic indicator?
                if (IsMnemonic(key, page.Title))
                {
                    // Select this page
                    this.SelectedTab = page;

                    return true;
                }
            }

            return false;
        }

        protected override void OnResize(EventArgs e)
        {
            Recalculate();
            Invalidate();

            base.OnResize(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Recalculate();
            Invalidate();

            base.OnSizeChanged(e);
        }

        public virtual void OnPopupMenuDisplay(CancelEventArgs e)
        {
            // Has anyone registered for the event?
            if (PopupMenuDisplay != null)
                PopupMenuDisplay(this, e);
        }

        public virtual void OnSelectionChanging(object sender, CancelArgs args)
        {
            // Has anyone registered for the event?
            if (SelectionChanging != null)
                SelectionChanging(this, args);
        }

        public virtual void OnSelectionChanged(EventArgs e)
        {
            if (SelectedTab != null)
            {
                if (SelectedTab.Alerting)
                    SelectedTab.Alerting = false;
            }
            // Has anyone registered for the event?
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

        public virtual void OnClosePressed(CancelArgs e)
        {
            if (_reorderingtab)
                return;
            // Has anyone registered for the event?
            if (ClosePressed != null)
                ClosePressed(this, e);
        }

        public virtual void OnPageGotFocus(EventArgs e)
        {
            // Has anyone registered for the event?
            if (PageGotFocus != null)
                PageGotFocus(this, e);
        }

        public virtual void OnPageLostFocus(EventArgs e)
        {
            // Has anyone registered for the event?
            if (PageLostFocus != null)
                PageLostFocus(this, e);
        }

        public virtual void OnPageDragStart(MouseEventArgs e)
        {
            // Has anyone registered for the event?
            if (PageDragStart != null)
                PageDragStart(this, e);
        }

        public virtual void OnPageDragMove(MouseEventArgs e)
        {
            // Has anyone registered for the event?
            if (PageDragMove != null)
                PageDragMove(this, e);
        }

        public virtual void OnPageDragEnd(MouseEventArgs e)
        {
            // Has anyone registered for the event?
            if (PageDragEnd != null)
                PageDragEnd(this, e);
        }

        public virtual void OnPageDragQuit(MouseEventArgs e)
        {
            // Has anyone registered for the event?
            if (PageDragQuit != null)
                PageDragQuit(this, e);
        }

        public virtual void OnDoubleClickTab(TabPageAdvanced page)
        {
            // Has anyone registered for the event?
            if (DoubleClickTab != null)
                DoubleClickTab(this, page);
        }

        protected virtual void OnCloseButton(object sender, EventArgs e)
        {
            OnClosePressed(new CancelArgs(SelectedTab));
        }
        ContextMenuStrip dropmenu;
        protected virtual void OnDropDownButton(object sender, EventArgs e)
        {
            if (TabPages.Count == 0)
                return;
            if (dropmenu == null)
            {
                dropmenu = new ContextMenuStrip();
            }
            dropmenu.Items.Clear();
            foreach (TabPageAdvanced ntab in TabPages)
            {
                dropmenu.ImageList = ntab.ImageList;
                ToolStripMenuItem nitem = new ToolStripMenuItem(ntab.Title);
                dropmenu.Items.Add(nitem);
                nitem.Tag = ntab;
                nitem.Click += new EventHandler(Nitem_Click);
                nitem.ImageIndex = ntab.ImageIndex;
                if (ntab == SelectedTab)
                    nitem.Checked = true;
            }
            dropmenu.Show(_dropDownButton, new Point(_dropDownButton.Width, _dropDownButton.Height), ToolStripDropDownDirection.BelowLeft);
        }

        void Nitem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem nitem = (ToolStripMenuItem)sender;
            TabPageAdvanced ntab = (TabPageAdvanced)nitem.Tag;
            if (TabPages.IndexOf(ntab) >= 0)
                SelectedTab = ntab;
        }

        protected virtual void OnLeftArrow(object sender, EventArgs e)
        {
            // Set starting page back one
            _startPage--;

            _recalculate = true;
            Invalidate();
        }

        protected virtual void OnRightArrow(object sender, EventArgs e)
        {
            // Set starting page forward one
            _startPage++;

            _recalculate = true;
            Invalidate();
        }

        protected virtual void DefineFont(Font newFont)
        {
            // Use base class for storage of value
            base.Font = newFont;

            // Update internal height value using Font
            _textHeight = newFont.Height;

            // Is the font height bigger than the image height?
            if (_imageHeight >= _textHeight)
            {
                // No, do not need extra spacing around the image to fit in text
                _imageGapTopExtra = 0;
                _imageGapBottomExtra = 0;
            }
            else
            {
                // Yes, need to make the image area bigger so that its height calculation
                // matchs that height of the text
                int extraHeight = _textHeight - _imageHeight;

                // Split the extra height between the top and bottom of image
                _imageGapTopExtra = extraHeight / 2;
                _imageGapBottomExtra = extraHeight - _imageGapTopExtra;
            }
        }

        protected virtual void DefineBackColor(Color newColor)
        {
            base.BackColor = newColor;

            // Calculate the modified colors from this base
            _backLight = ControlPaint.Light(newColor);
            _backLightLight = ControlPaint.LightLight(newColor);
            _backDark = ControlPaint.Dark(newColor);
            _backDarkDark = ControlPaint.DarkDark(newColor);

#if DON6
            _backIDE = Color.White;
#else
            _backIDE = ColorHelper.TabBackgroundFromBaseColor(newColor);
#endif
        }

        protected virtual void DefineButtonImages()
        {
            ImageAttributes ia = new ImageAttributes();

            ColorMap activeMap = new ColorMap();
            ColorMap inactiveMap = new ColorMap();

            // Define the color transformations needed
            activeMap.OldColor = Color.Black;
            activeMap.NewColor = _buttonActiveColor;
            inactiveMap.OldColor = Color.White;
            inactiveMap.NewColor = _buttonInactiveColor;

            // Create remap attributes for use by button
            ia.SetRemapTable(new ColorMap[] { activeMap, inactiveMap }, ColorAdjustType.Bitmap);

            // Pass attributes to the buttons
            _leftArrow.ImageAttributes = ia;
            _rightArrow.ImageAttributes = ia;
            _closeButton.ImageAttributes = ia;
            _dropDownButton.ImageAttributes = ia;
        }

        protected virtual void SetAppearance(VisualAppearance appearance)
        {
            switch (appearance)
            {
                case VisualAppearance.MultiForm:
                case VisualAppearance.MultiBox:
                    _shrinkPagesToFit = true;					// shrink tabs to fit width
                    _positionAtTop = false;						// draw tabs at bottom of control
                    _showClose = false;							// do not show the close button
                    _showDropDown = false;							// do not show the close button
                    _showCloseIndividual = false;							// do not show the close button
                    _showArrows = false;						// do not show the scroll arrow buttons
                    _boldSelected = false;						// do not show selected pages in bold
                    _idePixelArea = true;                       // show a one pixel border at top or bottom
                    IDEPixelBorder = false;                     // do not show a one pixel border round control
                    break;
                case VisualAppearance.MultiDocument:
                    _shrinkPagesToFit = false;					// shrink tabs to fit width
                    _positionAtTop = true;						// draw tabs at bottom of control
                    _showClose = false;							// do not show the close button
                    _showDropDown = false;							// do not show the close button
                    _showCloseIndividual = true;							// do not show the close button
                    _showArrows = true;						    // do not show the scroll arrow buttons
                    _boldSelected = true;						// do not show selected pages in bold
                    _idePixelArea = true;                       // show a one pixel border at top or bottom
                    IDEPixelBorder = false;                     // do not show a one pixel border round control
                    break;
            }

            // These properties are the same whichever style is selected
            _hotTrack = false;							// do not hot track paes
            _dimUnselected = true;						// draw dimmed non selected pages

            // Define then starting page for drawing
            if (_tabPages.Count > 0)
                _startPage = 0;
            else
                _startPage = -1;

            _appearance = appearance;

            // Define the correct style indexer
            SetStyleIndex();
        }

        protected virtual void SetStyleIndex()
        {
            switch (_appearance)
            {
                case VisualAppearance.MultiBox:
                    // Always pretend we are plain style
                    _styleIndex = 1;
                    break;
                case VisualAppearance.MultiForm:
                case VisualAppearance.MultiDocument:
                    _styleIndex = (int)_style;
                    break;
            }
        }

        protected virtual bool HideTabsCalculation()
        {
            bool hideTabs = false;

            switch (_hideTabsMode)
            {
                case HideTabsModes.ShowAlways:
                    hideTabs = false;
                    break;
                case HideTabsModes.HideAlways:
                    hideTabs = true;
                    break;
                case HideTabsModes.HideUsingLogic:
                    hideTabs = (_tabPages.Count <= 1);
                    break;
                case HideTabsModes.HideWithoutMouse:
                    hideTabs = !_mouseOver;
                    break;
            }

            return hideTabs;
        }

        protected virtual void Recalculate()
        {
            // Reset the need for a recalculation
            _recalculate = false;

            SizeF maxtextsize = this.CreateGraphics().MeasureString("Mg", this.Font);

            // The height of a tab button is...
            int tabButtonHeight = _position[_styleIndex, (int)PositionIndex.ImageGapTop] +
                                  _imageGapTopExtra +
                                  _imageHeight + ImageMargin +
                                  _imageGapBottomExtra +
                                  _position[_styleIndex, (int)PositionIndex.ImageGapBottom] +
                                  _position[_styleIndex, (int)PositionIndex.BorderBottom];
            if ((_position[_styleIndex, (int)PositionIndex.ImageGapTop] +
                                  _imageGapTopExtra + maxtextsize.Height) > tabButtonHeight)
                tabButtonHeight = Convert.ToInt32(Math.Round(maxtextsize.Height));
            // The height of the tabs area is...
            int tabsAreaHeight = _position[_styleIndex, (int)PositionIndex.BorderTop] +
                                 tabButtonHeight + _position[_styleIndex, (int)PositionIndex.TabsBottomGap];

            bool hideTabsArea = HideTabsCalculation();

            // Should the tabs area be hidden?
            if (hideTabsArea)
            {
                // ... then do not show the tabs or button controls
                _pageAreaRect = new Rectangle(0, 0, this.Width, this.Height);
                _tabsAreaRect = new Rectangle(0, 0, 0, 0);
            }
            else
            {
                if (_positionAtTop)
                {
                    // Create rectangle that represents the entire tabs area
                    _pageAreaRect = new Rectangle(0, tabsAreaHeight, this.Width, this.Height - tabsAreaHeight);

                    // Create rectangle that represents the entire area for pages
                    _tabsAreaRect = new Rectangle(0, 0, this.Width, tabsAreaHeight);
                }
                else
                {
                    // Create rectangle that represents the entire tabs area
                    _tabsAreaRect = new Rectangle(0, this.Height - tabsAreaHeight, this.Width, tabsAreaHeight);

                    // Create rectangle that represents the entire area for pages
                    _pageAreaRect = new Rectangle(0, 0, this.Width, this.Height - tabsAreaHeight);
                }
            }

            int xEndPos = 0;

            if (!hideTabsArea && _tabPages.Count > 0)
            {
                // The minimum size of a button includes its left and right borders for width,
                // and then fixed height which is based on the size of the image and font
                Rectangle tabPosition;

                if (_positionAtTop)
                    tabPosition = new Rectangle(0,
                                                _tabsAreaRect.Bottom - tabButtonHeight -
                                                _position[_styleIndex, (int)PositionIndex.BorderTop],
                                                _position[_styleIndex, (int)PositionIndex.BorderLeft] +
                                                _position[_styleIndex, (int)PositionIndex.BorderRight],
                                                tabButtonHeight);
                else
                    tabPosition = new Rectangle(0,
                                                _tabsAreaRect.Top +
                                                _position[_styleIndex, (int)PositionIndex.BorderTop],
                                                _position[_styleIndex, (int)PositionIndex.BorderLeft] +
                                                _position[_styleIndex, (int)PositionIndex.BorderRight],
                                                tabButtonHeight);

                // Find starting and ending positons for drawing tabs
                int xStartPos = _tabsAreaRect.Left + _tabsAreaStartInset;
                xEndPos = GetMaximumDrawPos();

                // Available width for tabs is size between start and end positions
                int xWidth = xEndPos - xStartPos;

                if (_multiline)
                    RecalculateMultilineTabs(xStartPos, xEndPos, tabPosition, tabButtonHeight);
                else
                    RecalculateSinglelineTabs(xWidth, xStartPos, tabPosition);
            }

            // Position of Controls defaults to the entire page area
            _pageRect = _pageAreaRect;

            // Adjust child controls positions depending on style
            if ((_style == VisualStyle.Plain) && (_appearance != VisualAppearance.MultiBox))
            {
                _pageRect = _pageAreaRect;

                // Shrink by having a border on left,top and right borders
                _pageRect.X += _plainBorderDouble;
                _pageRect.Width -= (_plainBorderDouble * 2) - 1;

                if (!_positionAtTop)
                    _pageRect.Y += _plainBorderDouble;

                _pageRect.Height -= _plainBorderDouble - 1;

                // If hiding the tabs then need to adjust the controls positioning
                if (hideTabsArea)
                {
                    _pageRect.Height -= _plainBorderDouble;

                    if (_positionAtTop)
                        _pageRect.Y += _plainBorderDouble;
                }
            }

            // Calcualte positioning of the child controls/forms
            int leftOffset = _ctrlLeftOffset;
            int rightOffset = _ctrlRightOffset;
            int topOffset = _ctrlTopOffset;
            int bottomOffset = _ctrlBottomOffset;

            if (_idePixelBorder && (_style != VisualStyle.Plain))
            {
                leftOffset += 2;
                rightOffset += 2;

                if (_positionAtTop || hideTabsArea)
                    bottomOffset += 2;

                if (!_positionAtTop || hideTabsArea)
                    topOffset += 2;
            }

            Point pageLoc = new Point(_pageRect.Left + leftOffset,
                                      _pageRect.Top + topOffset);

            Size pageSize = new Size(_pageRect.Width - leftOffset - rightOffset,
                                     _pageRect.Height - topOffset - bottomOffset);

            // If in Plain style and requested to only show top or bottom border
            if ((_style == VisualStyle.Plain) && _insetBorderPagesOnly)
            {
                // Then need to increase width to occupy where borders would have been 
                pageLoc.X -= _plainBorderDouble;
                pageSize.Width += _plainBorderDouble * 2;

                if (hideTabsArea || _positionAtTop)
                {
                    // Draw into the bottom border area
                    pageSize.Height += _plainBorderDouble;
                }

                if (hideTabsArea || !_positionAtTop)
                {
                    // Draw into the top border area
                    pageLoc.Y -= _plainBorderDouble;
                    pageSize.Height += _plainBorderDouble;
                }
            }

            // Position the host panel appropriately
            _hostPanel.Size = pageSize;
            _hostPanel.Location = pageLoc;

            // If we have any tabs at all
            if (_tabPages.Count > 0)
            {
                Rectangle rect = (Rectangle)_tabRects[_tabPages.Count - 1];

                // Determine is the right scrolling button should be enabled
                _rightScroll = (rect.Right > xEndPos);
            }
            else
            {
                // No pages means there can be no right scrolling
                _rightScroll = false;
            }

            // Determine if left scrolling is possible
            _leftScroll = (_startPage > 0);

            // Handle then display and positioning of buttons
            RecalculateButtons();
        }

        protected virtual void RecalculateMultilineTabs(int xStartPos, int xEndPos,
                                                        Rectangle tabPosition, int tabButtonHeight)
        {
            using (Graphics g = this.CreateGraphics())
            {
                // MultiBox style needs a pixel extra drawing room on right
                if (_appearance == VisualAppearance.MultiBox)
                    xEndPos -= 2;

                // How many tabs on this line
                int lineCount = 0;

                // Remember which line is the first displayed
                _topYPos = tabPosition.Y;

                // Next tab starting position
                int xPos = xStartPos;
                int yPos = tabPosition.Y;

                // How many full lines were there
                int fullLines = 0;

                // Line increment value
                int lineIncrement = tabButtonHeight + 1;

                // Track which line has the selection on it                                
                int selectedLine = 0;

                // Vertical adjustment
                int yAdjust = 0;

                // Create array for holding lines of tabs
                ArrayList lineList = new ArrayList
                {

                    // Add the initial line
                    new ArrayList()
                };

                // Process each tag page in turn
                for (int i = 0; i < _tabPages.Count; i++)
                {
                    // Get the tab instance for this position
                    TabPageAdvanced page = _tabPages[i];

                    // Find out the tabs total width
                    int tabWidth = GetTabPageSpace(g, page);

                    // If not the first on the line, then check if newline should be started
                    if (lineCount > 0)
                    {
                        // Does this tab extend pass end of the lines
                        if ((xPos + tabWidth) > xEndPos)
                        {
                            // Next tab position is down a line and back to the start
                            xPos = xStartPos;
                            yPos += lineIncrement;

                            // Remember which line is the last displayed
                            _bottomYPos = tabPosition.Y;

                            // Increase height of the tabs area
                            _tabsAreaRect.Height += lineIncrement;

                            // Decrease height of the control area
                            _pageAreaRect.Height -= lineIncrement;

                            // Moving areas depends on drawing at top or bottom
                            if (_positionAtTop)
                                _pageAreaRect.Y += lineIncrement;
                            else
                            {
                                yAdjust -= lineIncrement;
                                _tabsAreaRect.Y -= lineIncrement;
                            }

                            // Start a new line 
                            lineList.Add(new ArrayList());

                            // Make sure the entries are aligned to fill entire line
                            fullLines++;
                        }
                    }

                    // Limit the width of a tab to the whole line
                    if (tabWidth > (xEndPos - xStartPos))
                        tabWidth = xEndPos - xStartPos;

                    // Construct rectangle for representing this tab
                    Rectangle tabRect = new Rectangle(xPos, yPos, tabWidth, tabButtonHeight);

                    // Add this tab to the current line array
                    ArrayList thisLine = lineList[lineList.Count - 1] as ArrayList;

                    // Create entry to represent the sizing of the given page index
                    MultiRect tabEntry = new MultiRect(tabRect, i);

                    thisLine.Add(tabEntry);

                    // Track which line has the selection on it
                    if (i == _pageSelected)
                        selectedLine = fullLines;

                    // Move position of next tab along
                    xPos += tabWidth + 1;

                    // Increment number of tabs on this line
                    lineCount++;
                }

                int line = 0;

                // Do we need all lines to extend full width
                if (_multilineFullWidth)
                    fullLines++;

                // Make each full line stretch the whole line width
                foreach (ArrayList lineArray in lineList)
                {
                    // Only right fill the full lines
                    if (line < fullLines)
                    {
                        // Number of items on this line
                        int numLines = lineArray.Count;

                        // Find ending position of last entry
                        MultiRect itemEntry = (MultiRect)lineArray[numLines - 1];

                        // Is there spare room between last entry and end of line?                            
                        if (itemEntry.Rect.Right < (xEndPos - 1))
                        {
                            // Work out how much extra to give each one
                            int extra = (int)((xEndPos - itemEntry.Rect.Right - 1) / numLines);

                            // Keep track of how much items need moving across
                            int totalMove = 0;

                            // Add into each entry
                            for (int i = 0; i < numLines; i++)
                            {
                                // Get the entry class instance
                                MultiRect expandEntry = (MultiRect)lineArray[i];

                                // Move across requried amount
                                expandEntry.X += totalMove;

                                // Add extra width
                                expandEntry.Width += (int)extra;

                                // All items after this needing moving
                                totalMove += extra;
                            }

                            // Extend the last position, in case rounding errors means its short
                            itemEntry.Width += (xEndPos - itemEntry.Rect.Right - 1);
                        }
                    }

                    line++;
                }

                if (_positionAtTop)
                {
                    // If the selected line is not the bottom line
                    if (selectedLine != (lineList.Count - 1))
                    {
                        ArrayList lastLine = (ArrayList)(lineList[lineList.Count - 1]);

                        // Find y offset of last line
                        int lastOffset = ((MultiRect)lastLine[0]).Rect.Y;

                        // Move all lines below it up one
                        for (int lineIndex = selectedLine + 1; lineIndex < lineList.Count; lineIndex++)
                        {
                            ArrayList al = (ArrayList)lineList[lineIndex];

                            for (int item = 0; item < al.Count; item++)
                            {
                                MultiRect itemEntry = (MultiRect)al[item];
                                itemEntry.Y -= lineIncrement;
                            }
                        }

                        // Move selected line to the bottom
                        ArrayList sl = (ArrayList)lineList[selectedLine];

                        for (int item = 0; item < sl.Count; item++)
                        {
                            MultiRect itemEntry = (MultiRect)sl[item];
                            itemEntry.Y = lastOffset;
                        }
                    }
                }
                else
                {
                    // If the selected line is not the top line
                    if (selectedLine != 0)
                    {
                        ArrayList topLine = (ArrayList)(lineList[0]);

                        // Find y offset of top line
                        int topOffset = ((MultiRect)topLine[0]).Rect.Y;

                        // Move all lines above it down one
                        for (int lineIndex = 0; lineIndex < selectedLine; lineIndex++)
                        {
                            ArrayList al = (ArrayList)lineList[lineIndex];

                            for (int item = 0; item < al.Count; item++)
                            {
                                MultiRect itemEntry = (MultiRect)al[item];
                                itemEntry.Y += lineIncrement;
                            }
                        }

                        // Move selected line to the top
                        ArrayList sl = (ArrayList)lineList[selectedLine];

                        for (int item = 0; item < sl.Count; item++)
                        {
                            MultiRect itemEntry = (MultiRect)sl[item];
                            itemEntry.Y = topOffset;
                        }
                    }
                }

                // Now assignt each lines rectangle to the corresponding structure
                foreach (ArrayList al in lineList)
                {
                    foreach (MultiRect multiEntry in al)
                    {
                        Rectangle newRect = multiEntry.Rect;

                        // Make the vertical adjustment
                        newRect.Y += yAdjust;

                        _tabRects[multiEntry.Index] = newRect;
                    }
                }
            }
        }

        protected virtual void RecalculateSinglelineTabs(int xWidth, int xStartPos, Rectangle tabPosition)
        {
            using (Graphics g = this.CreateGraphics())
            {
                int originalWidth = xWidth;

                // Remember which lines are then first and last displayed
                _topYPos = tabPosition.Y;
                _bottomYPos = _topYPos;

                // Set the minimum size for each tab page
                for (int i = 0; i < _tabPages.Count; i++)
                {
                    // Is this page before those displayed?
                    if (i < _startPage)
                        _tabRects[i] = (object)_nullPosition;  // Yes, position off screen
                    else
                        _tabRects[i] = (object)tabPosition;	 // No, create minimum size
                }

                // Subtract the minimum tab sizes already allocated
                xWidth -= _tabPages.Count * (tabPosition.Width + 1);

                // Is there any more space left to allocate
                if (xWidth > 0)
                {
                    ArrayList listNew = new ArrayList();
                    ArrayList listOld = new ArrayList();

                    // Add all pages to those that need space allocating
                    for (int i = _startPage; i < _tabPages.Count; i++)
                        listNew.Add(_tabPages[i]);

                    // Each tab can have an allowance
                    int xAllowance;

                    do
                    {
                        // The list generated in the last iteration becomes 
                        // the to be processed in this iteration
                        listOld = listNew;

                        // List of pages that still need more space allocating
                        listNew = new ArrayList();

                        if (_shrinkPagesToFit)
                        {
                            // Each page is allowed a maximum allowance of space
                            // during this iteration. 
                            xAllowance = xWidth / _tabPages.Count;
                        }
                        else
                        {
                            // Allow each page as much space as it wants
                            xAllowance = 999;
                        }

                        // Assign space to each page that is requesting space
                        foreach (TabPageAdvanced page in listOld)
                        {
                            int index = _tabPages.IndexOf(page);

                            Rectangle rectPos = (Rectangle)_tabRects[index];

                            // Find out how much extra space this page is requesting
                            int xSpace = GetTabPageSpace(g, page) - rectPos.Width;

                            // Does it want more space than its currently allowed to have?
                            if (xSpace > xAllowance)
                            {
                                // Restrict allowed space
                                xSpace = xAllowance;

                                // Add page to ensure it gets processed next time around
                                listNew.Add(page);
                            }

                            // Give space to tab
                            rectPos.Width += xSpace;

                            _tabRects[index] = (object)rectPos;

                            // Reduce extra left for remaining tabs
                            xWidth -= xSpace;
                        }
                    } while ((listNew.Count > 0) && (xAllowance > 0) && (xWidth > 0));
                }

                // Assign the final positions to each tab now we known their sizes
                for (int i = _startPage; i < _tabPages.Count; i++)
                {
                    Rectangle rectPos = (Rectangle)_tabRects[i];

                    // Define position of tab page
                    rectPos.X = xStartPos;

                    _tabRects[i] = (object)rectPos;

                    // Next button must be the width of this one across
                    xStartPos += rectPos.Width + 1;
                }
                if ((AutoShrinkPages) && (_tabPages.Count > 1))
                {
                    int totalWidth = 0;
                    for (int i = 0; i < _tabPages.Count; i++)
                    {
                        Rectangle tabrec = (Rectangle)_tabRects[i];
                        totalWidth = totalWidth + tabrec.Width;
                    }
                    if (totalWidth > (originalWidth))
                    {
                        // It does not fit so shring all pages
                        int availableWidth = originalWidth;
                        int totalpages = _tabPages.Count;
                        int fixedWidth = availableWidth / totalpages;
                        if (!AllowLastTabReordering)
                        {
                            totalpages = totalpages - 1;
                            availableWidth = availableWidth - ((Rectangle)_tabRects[_tabPages.Count - 1]).Width * 2;
                            fixedWidth = availableWidth / totalpages;
                        }
                        if (fixedWidth > AutoShrinkMinimum)
                        {
                            Rectangle previous = (Rectangle)_tabRects[0];
                            if (previous.Width > 0)
                            {
                                _tabRects[0] = new Rectangle(previous.Left, previous.Top, fixedWidth, previous.Height);
                                for (int i = 1; i < totalpages; i++)
                                {
                                    previous = (Rectangle)_tabRects[i - 1];
                                    Rectangle newrec = new Rectangle(previous.Right + 1, previous.Top, fixedWidth, previous.Height);
                                    _tabRects[i] = newrec;
                                }
                                if (!AllowLastTabReordering)
                                {
                                    Rectangle oldrec = (Rectangle)_tabRects[totalpages];
                                    previous = (Rectangle)_tabRects[totalpages - 1];
                                    Rectangle newrec = new Rectangle(previous.Right + 1, previous.Top, oldrec.Width, previous.Height);
                                    _tabRects[totalpages] = newrec;
                                }
                            }
                        }
                    }
                }

            }
        }

        protected virtual void RecalculateButtons()
        {
            int buttonTopGap = 0;

            if (_multiline)
            {
                // The height of a tab row is
                int tabButtonHeight = _position[_styleIndex, (int)PositionIndex.ImageGapTop] +
                                      _imageGapTopExtra +
                                      _imageHeight +
                                      _imageGapBottomExtra +
                                      _position[_styleIndex, (int)PositionIndex.ImageGapBottom] +
                                      _position[_styleIndex, (int)PositionIndex.BorderBottom];

                // The height of the tabs area is...
                int tabsAreaHeight = _position[_styleIndex, (int)PositionIndex.BorderTop] +
                                      tabButtonHeight + _position[_styleIndex, (int)PositionIndex.TabsBottomGap];

                // Find offset to place button halfway down the tabs area rectangle
                buttonTopGap = _position[_styleIndex, (int)PositionIndex.ButtonOffset] +
                               (tabsAreaHeight - _buttonHeight) / 2;

                // Invert gap position when at bottom
                if (!_positionAtTop)
                    buttonTopGap = _tabsAreaRect.Height - buttonTopGap - _buttonHeight;
            }
            else
            {
                // Find offset to place button halfway down the tabs area rectangle
                buttonTopGap = _position[_styleIndex, (int)PositionIndex.ButtonOffset] +
                                (_tabsAreaRect.Height - _buttonHeight) / 2;
            }
            // Position to place next button
            int xStart = _tabsAreaRect.Right - _buttonWidth - _buttonGap;

            // Close button should be shown?
            if (_showClose)
            {
                // Define the location
                _closeButton.Location = new Point(xStart, _tabsAreaRect.Top + buttonTopGap);

                if (xStart < 1)
                    _closeButton.Hide();
                else
                    _closeButton.Show();

                xStart -= _buttonWidth;
            }
            else
                _closeButton.Hide();

            // DropDown button should be shown?
            if (_showDropDown)
            {
                // Define the location
                _dropDownButton.Location = new Point(xStart, _tabsAreaRect.Top + buttonTopGap);

                if (xStart < 1)
                    _dropDownButton.Hide();
                else
                    _dropDownButton.Show();

                xStart -= _dropDownButton.Width;
            }
            else
                _dropDownButton.Hide();

            // Arrows should be shown?
            if (_showArrows)
            {
                // Position the right arrow first as its more the right hand side
                _rightArrow.Location = new Point(xStart, _tabsAreaRect.Top + buttonTopGap);

                if (xStart < 1)
                    _rightArrow.Hide();
                else
                    _rightArrow.Show();

                xStart -= _rightArrow.Width;

                _leftArrow.Location = new Point(xStart, _tabsAreaRect.Top + buttonTopGap);

                if (xStart < 1)
                    _leftArrow.Hide();
                else
                    _leftArrow.Show();

                xStart -= _leftArrow.Width;

                // Define then enabled state of buttons
                _leftArrow.Enabled = _leftScroll;
                _rightArrow.Enabled = _rightScroll;
            }
            else
            {
                _leftArrow.Hide();
                _rightArrow.Hide();
            }

            if ((_appearance == VisualAppearance.MultiBox) || (_style == VisualStyle.Plain))
                _closeButton.BackColor = _leftArrow.BackColor = _rightArrow.BackColor = _dropDownButton.BackColor = this.BackColor;
            else
                _closeButton.BackColor = _leftArrow.BackColor = _rightArrow.BackColor = _dropDownButton.BackColor = _backIDE;
        }

        protected virtual int GetMaximumDrawPos()
        {
            int xEndPos = _tabsAreaRect.Right - _tabsAreaEndInset;

            // Showing the close button reduces available space
            if (_showClose)
                xEndPos -= _buttonWidth + _buttonGap;

            // If showing arrows then reduce space for both
            if (_showArrows)
                xEndPos -= _buttonWidth * 2;
            if (_showDropDown)
                xEndPos -= _buttonWidth;

            return xEndPos;
        }

        protected virtual int GetTabPageSpace(Graphics g, TabPageAdvanced page)
        {
            // Find the fixed elements of required space
            int width = _position[_styleIndex, (int)PositionIndex.BorderLeft] +
                        _position[_styleIndex, (int)PositionIndex.BorderRight];

            // Any icon or image provided?
            if ((((page.Icon != null) || (((_imageList != null) || (page.ImageList != null)) && (page.ImageIndex != -1))))
               || (page.TabWidth > 0))
            {
                width += _position[_styleIndex, (int)PositionIndex.ImageGapLeft] +
                         _imageWidth + ImageMargin +
                         _position[_styleIndex, (int)PositionIndex.ImageGapRight];
            }

            // Any text provided?
            if ((page.Title.Length > 0) || (_showCloseIndividual && page.CanClose))
            {
                if (!_selectedTextOnly || (_selectedTextOnly && (_pageSelected == _tabPages.IndexOf(page))))
                {
                    Font drawFont = base.Font;

                    if (_boldSelected && page.Selected)
                        drawFont = new Font(drawFont, FontStyle.Bold);

                    // Find width of the requested text
                    SizeF dimension = g.MeasureString(page.Title, drawFont);

                    // With of close icon
                    if (_showCloseIndividual)
                    {
                        dimension = new SizeF(dimension.Width + _buttonWidth + _buttonGap, dimension.Height);
                    }
                    if (page.TabWidth > 0)
                        dimension = new SizeF(page.TabWidth, dimension.Height);

                    // Convert to integral
                    width += _position[_styleIndex, (int)PositionIndex.TextGapLeft] +
                            (int)dimension.Width + 1;
                }
            }
            else
            {
                Font drawFont = base.Font;
                SizeF dimension = g.MeasureString(" ", drawFont);
                if (page.TabWidth > 0)
                    dimension = new SizeF(page.TabWidth, dimension.Height);
            }

            return width;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Does the state need recalculating before paint can occur?
            if (_recalculate)
                Recalculate();

            using (SolidBrush pageAreaBrush = new SolidBrush(this.BackColor))
            {
                // Fill backgrounds of the page and tabs areas
                e.Graphics.FillRectangle(pageAreaBrush, _pageAreaRect);

                if ((_style == VisualStyle.Plain) || (_appearance == VisualAppearance.MultiBox))
                {
                    e.Graphics.FillRectangle(pageAreaBrush, _tabsAreaRect);
                }
                else
                {
                    using (SolidBrush tabsAreaBrush = new SolidBrush(_backIDE))
                        e.Graphics.FillRectangle(tabsAreaBrush, _tabsAreaRect);
                }
            }

            // MultiBox and Chrome appearance does not have any borders
            if (_appearance != VisualAppearance.MultiBox)
            {
                bool hiddenPages = HideTabsCalculation();

                // Draw the borders
                switch (_style)
                {
                    case VisualStyle.Plain:
                        // Height for drawing the border is size of the page area extended 
                        // down to draw the bottom border inside the tabs area
                        int pageHeight = _pageAreaRect.Height + _plainBorderDouble;

                        int xDraw = _pageAreaRect.Top;

                        // Should the tabs area be hidden?
                        if (hiddenPages)
                        {
                            // Then need to readjust pageHeight
                            pageHeight -= _plainBorderDouble;
                        }
                        else
                        {
                            // If drawing at top then overdraw upwards and not down
                            if (_positionAtTop)
                                xDraw -= _plainBorderDouble;
                        }

                        if (_insetBorderPagesOnly)
                        {
                            if (!hiddenPages)
                            {
                                // Draw the outer border around the page area			
                                DrawHelper.DrawPlainRaisedBorderTopOrBottom(e.Graphics, new Rectangle(0, xDraw, this.Width, pageHeight),
                                                                            _backLightLight, base.BackColor, _backDark, _backDarkDark, _positionAtTop);
                            }
                        }
                        else
                        {
                            // Draw the outer border around the page area			
                            DrawHelper.DrawPlainRaisedBorder(e.Graphics, new Rectangle(_pageAreaRect.Left, xDraw, _pageAreaRect.Width, pageHeight),
                                                             _backLightLight, base.BackColor, _backDark, _backDarkDark);
                        }

                        // Do we have any tabs?
                        if ((_tabPages.Count > 0) && _insetPlain)
                        {
                            // Draw the inner border around the page area
                            Rectangle inner = new Rectangle(_pageAreaRect.Left + _plainBorder,
                                                            xDraw + _plainBorder,
                                                            _pageAreaRect.Width - _plainBorderDouble,
                                                            pageHeight - _plainBorderDouble);

                            if (_insetBorderPagesOnly)
                            {
                                if (!hiddenPages)
                                {
                                    DrawHelper.DrawPlainSunkenBorderTopOrBottom(e.Graphics, new Rectangle(0, inner.Top, this.Width, inner.Height),
                                                                                _backLightLight, base.BackColor, _backDark, _backDarkDark, _positionAtTop);
                                }
                            }
                            else
                            {
                                DrawHelper.DrawPlainSunkenBorder(e.Graphics, new Rectangle(inner.Left, inner.Top, inner.Width, inner.Height),
                                                                 _backLightLight, base.BackColor, _backDark, _backDarkDark);
                            }

                        }
                        break;

                    case VisualStyle.IDE:
                        // Draw the top and bottom borders to the tabs area
                        using (Pen darkdark = new Pen(_backDarkDark),
                                   dark = new Pen(_backDark),
                                   lightlight = new Pen(_backLightLight),
                                   backColor = new Pen(base.BackColor))
                        {
                            int borderGap = _position[_styleIndex, (int)PositionIndex.BorderTop];

                            if (_positionAtTop)
                            {
                                // Fill the border between the tabs and the embedded controls
                                using (SolidBrush backBrush = new SolidBrush(base.BackColor))
                                    e.Graphics.FillRectangle(backBrush, 0, _tabsAreaRect.Bottom - borderGap, _tabsAreaRect.Width, borderGap);

                                int indent = 0;

                                // Is a single pixel border required around whole area?                            
                                if (_idePixelBorder)
                                {
                                    using (Pen llFore = new Pen(ControlPaint.LightLight(this.ForeColor)))
                                        e.Graphics.DrawRectangle(dark, 0, 0, this.Width - 1, this.Height - 1);

                                    indent++;
                                }
                                else
                                {
                                    if (_idePixelArea)
                                    {
                                        // Draw top border
                                        e.Graphics.DrawLine(dark, 0, _tabsAreaRect.Top, _tabsAreaRect.Width, _tabsAreaRect.Top);
                                    }
                                }

                                // Draw bottom border
                                if (!hiddenPages)
                                    e.Graphics.DrawLine(lightlight, indent,
                                                                    _tabsAreaRect.Bottom - borderGap,
                                                                    _tabsAreaRect.Width - (indent * 2),
                                                                    _tabsAreaRect.Bottom - borderGap);
                            }
                            else
                            {
                                // Fill the border between the tabs and the embedded controls
                                using (SolidBrush backBrush = new SolidBrush(base.BackColor))
                                    e.Graphics.FillRectangle(backBrush, 0, _tabsAreaRect.Top, _tabsAreaRect.Width, borderGap);

                                int indent = 0;

                                // Is a single pixel border required around whole area?                            
                                if (_idePixelBorder)
                                {
                                    using (Pen llFore = new Pen(ControlPaint.LightLight(this.ForeColor)))
                                        e.Graphics.DrawRectangle(dark, 0, 0, this.Width - 1, this.Height - 1);

                                    indent++;
                                }
                                else
                                {
                                    if (_idePixelArea)
                                    {
                                        // Draw bottom border
                                        e.Graphics.DrawLine(backColor, 0, _tabsAreaRect.Bottom - 1, _tabsAreaRect.Width, _tabsAreaRect.Bottom - 1);
                                    }
                                }

                                // Draw top border
                                if (!hiddenPages)
                                    e.Graphics.DrawLine(darkdark, indent,
                                                                _tabsAreaRect.Top + 2,
                                                                _tabsAreaRect.Width - (indent * 2),
                                                                _tabsAreaRect.Top + 2);
                            }
                        }
                        break;
                    case VisualStyle.Chrome:
                        // Draw the top and bottom borders to the tabs area
                        /*using (Pen darkdark = new Pen(_backDarkDark),
                                   dark = new Pen(_backDark),
                                   lightlight = new Pen(_backLightLight),
                                   backColor = new Pen(base.BackColor))
                        {
                            int borderGap = _position[_styleIndex, (int)PositionIndex.BorderTop];

                            if (_positionAtTop)
                            {
                                // Fill the border between the tabs and the embedded controls
                                using (SolidBrush backBrush = new SolidBrush(base.BackColor))
                                    e.Graphics.FillRectangle(backBrush, 0, _tabsAreaRect.Bottom - borderGap, _tabsAreaRect.Width, borderGap);

                                int indent = 0;

                                // Is a single pixel border required around whole area?                            
                                if (_idePixelBorder)
                                {
                                    using (Pen llFore = new Pen(ControlPaint.LightLight(this.ForeColor)))
                                        e.Graphics.DrawRectangle(dark, 0, 0, this.Width - 1, this.Height - 1);

                                    indent++;
                                }
                                else
                                {
                                    if (_idePixelArea)
                                    {
                                        // Draw top border
                                        e.Graphics.DrawLine(dark, 0, _tabsAreaRect.Top, _tabsAreaRect.Width, _tabsAreaRect.Top);
                                    }
                                }

                                // Draw bottom border
                                if (!hiddenPages)
                                    e.Graphics.DrawLine(lightlight, indent,
                                                                    _tabsAreaRect.Bottom - borderGap,
                                                                    _tabsAreaRect.Width - (indent * 2),
                                                                    _tabsAreaRect.Bottom - borderGap);
                            }
                            else
                            {
                                // Fill the border between the tabs and the embedded controls
                                using (SolidBrush backBrush = new SolidBrush(base.BackColor))
                                    e.Graphics.FillRectangle(backBrush, 0, _tabsAreaRect.Top, _tabsAreaRect.Width, borderGap);

                                int indent = 0;

                                // Is a single pixel border required around whole area?                            
                                if (_idePixelBorder)
                                {
                                    using (Pen llFore = new Pen(ControlPaint.LightLight(this.ForeColor)))
                                        e.Graphics.DrawRectangle(dark, 0, 0, this.Width - 1, this.Height - 1);

                                    indent++;
                                }
                                else
                                {
                                    if (_idePixelArea)
                                    {
                                        // Draw bottom border
                                        e.Graphics.DrawLine(backColor, 0, _tabsAreaRect.Bottom - 1, _tabsAreaRect.Width, _tabsAreaRect.Bottom - 1);
                                    }
                                }

                                // Draw top border
                                if (!hiddenPages)
                                    e.Graphics.DrawLine(darkdark, indent,
                                                                _tabsAreaRect.Top + 2,
                                                                _tabsAreaRect.Width - (indent * 2),
                                                                _tabsAreaRect.Top + 2);
                            }
                        }*/
                        break;
                }
            }

            // Clip the drawing to prevent drawing in unwanted areas
            ClipDrawingTabs(e.Graphics);

            // Paint each tab page
            /*foreach (TabPageAdvanced page in _tabPages)
            {
                Rectangle rectTab = (Rectangle)_tabRects[_tabPages.IndexOf(page)];
                //DrawTabBorder(ref rectTab,page,e.Graphics);
            }*/

            List<TabPageAdvanced> pagestodraw = new List<TabPageAdvanced>();
            for (int i = 0; i < _tabPages.Count; i++)
            {
                if ((i == _tabPages.Count - 1) && (_reorderingtab))
                {
                    if (AllowLastTabReordering)
                        pagestodraw.Add(_tabPages[i]);
                }
                else
                    pagestodraw.Add(_tabPages[i]);
            }
            // Paint each tab page
            foreach (TabPageAdvanced page in pagestodraw)
            {
                if (!page.Selected)
                {
                    bool highlighttext = false;
                    bool highlightclose = false;
                    GetHighLightStatus(page, ref highlighttext, ref highlightclose);
                    DrawTab(page, e.Graphics, highlighttext, highlightclose);
                }
            }
            // Paint each tab page
            foreach (TabPageAdvanced page in pagestodraw)
            {
                if (page.Selected)
                {
                    bool highlighttext = false;
                    bool highlightclose = false;
                    GetHighLightStatus(page, ref highlighttext, ref highlightclose);
                    DrawTab(page, e.Graphics, highlighttext, highlightclose);
                }
            }
        }


        protected virtual Rectangle ClippingRectangle()
        {
            // Calculate how much to reduce width by for clipping rectangle
            int xReduce = _tabsAreaRect.Width - GetMaximumDrawPos();

            // Create clipping rect
            return new Rectangle(_tabsAreaRect.Left,
                                 _tabsAreaRect.Top,
                                 _tabsAreaRect.Width - xReduce,
                                 _tabsAreaRect.Height);
        }

        protected virtual void ClipDrawingTabs(Graphics g)
        {
            Rectangle clipRect = ClippingRectangle();

            // Restrict drawing to this clipping rectangle
            g.Clip = new Region(clipRect);
        }

        protected virtual void DrawTab(TabPageAdvanced page, Graphics g, bool highlightText, bool highlightClose)
        {
            Rectangle rectTab = (Rectangle)_tabRects[_tabPages.IndexOf(page)];

            if (_reorderingtab)
            {
                if (page == SelectedTab)
                {
                    Point currentScreenPos = Cursor.Position;
                    Point currentPos = this.PointToClient(currentScreenPos);
                    int newx = rectTab.Left + currentPos.X - _leftMouseDownPos.X;
                    if (newx < 0)
                        newx = 0;
                    if (newx + rectTab.Width > Width)
                        newx = Width - rectTab.Width;
                    rectTab = new Rectangle(newx, rectTab.Top, rectTab.Width, rectTab.Height);
                }
            }

            DrawTabBorder(ref rectTab, page, g);

            int xDraw = rectTab.Left + _position[_styleIndex, (int)PositionIndex.BorderLeft];
            int xMax = rectTab.Right - _position[_styleIndex, (int)PositionIndex.BorderRight];

            DrawTabImage(rectTab, page, g, xMax, ref xDraw, highlightText);
            DrawTabText(rectTab, page, g, highlightText, highlightClose, xMax, xDraw);
        }

        protected virtual void DrawTabImage(Rectangle rectTab,
                                            TabPageAdvanced page,
                                            Graphics g,
                                            int xMax,
                                            ref int xDraw, bool highlightText)
        {
            // Default to using the Icon from the page
            Image drawIcon = page.Icon;
            Image drawImage = null;
            if (drawIcon != null)
            {
                if (page.DrawIconHightlight)
                    if (!highlightText)
                        drawIcon = null;
            }

            // If there is no valid Icon and the page is requested an image list index...
            if ((drawIcon == null) && (page.ImageIndex != -1))
            {
                try
                {
                    // Default to using an image from the TabPageAdvanced
                    ImageList imageList = page.ImageList;

                    // If page does not have an ImageList...
                    if (imageList == null)
                        imageList = _imageList;   // ...then use the TabControlAdvanced one

                    // Do we have an ImageList to select from?
                    if (imageList != null)
                    {
                        // Grab the requested image
                        drawImage = imageList.Images[page.ImageIndex];
                    }
                }
                catch (Exception)
                {
                    // User supplied ImageList/ImageIndex are invalid, use an error image instead
                    drawImage = _internalImages.Images[(int)ImageStrip.Error];
                }
            }

            // Draw any image required
            if ((drawImage != null) || (drawIcon != null))
            {
                // Enough room to draw any of the image?
                if ((xDraw + _position[_styleIndex, (int)PositionIndex.ImageGapLeft]) <= xMax)
                {
                    // Move past the left image gap
                    xDraw += _position[_styleIndex, (int)PositionIndex.ImageGapLeft];

                    // Find down position for drawing the image
                    int yDraw = rectTab.Top +
                                _position[_styleIndex, (int)PositionIndex.ImageGapTop] +
                                _imageGapTopExtra;

                    int gaptop = _position[_styleIndex, (int)PositionIndex.ImageGapTop];
                    //yDraw = yDraw + ImageMargin;                
                    // Icono centrado en el rectangulo
                    yDraw = gaptop + ((rectTab.Height - gaptop) - _imageHeight) / 2;

                    // If there is enough room for all of the image?
                    if ((xDraw + _imageWidth - 1) <= xMax)
                    {
                        if (drawImage != null)
                            g.DrawImage(drawImage, new Rectangle(xDraw, yDraw, _imageWidth, _imageHeight));
                        else
                        {
                            //g.DrawIcon(drawIcon, new Rectangle(xDraw, yDraw, _imageWidth, _imageHeight));
                            System.Threading.Monitor.Enter(drawIcon);
                            try
                            {
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.DrawImage(drawIcon, new Rectangle(xDraw, yDraw, _imageWidth, _imageHeight));
                            }
                            finally
                            {
                                System.Threading.Monitor.Exit(drawIcon);
                            }
                        }

                        // Move past the image and the image gap to the right
                        xDraw += _imageWidth + ImageMargin + _position[_styleIndex, (int)PositionIndex.ImageGapRight];
                    }
                    else
                    {
                        // Calculate how much room there is
                        int xSpace = xMax - xDraw;

                        // Any room at all?
                        if (xSpace > 0)
                        {
                            if (drawImage != null)
                            {
                                // Draw only part of the image
                                g.DrawImage(drawImage,
                                            new Point[]{new Point(xDraw, yDraw),
                                                        new Point(xDraw + xSpace, yDraw),
                                                        new Point(xDraw, yDraw + _imageHeight)},
                                            new Rectangle(0, 0, xSpace,
                                            _imageHeight),
                                            GraphicsUnit.Pixel);
                            }
                            else
                            {
                                // Draw only part of the image
                                g.DrawImage(drawIcon,
                                            new Point[]{new Point(xDraw, yDraw),
                                                        new Point(xDraw + xSpace, yDraw),
                                                        new Point(xDraw, yDraw + _imageHeight)},
                                            new Rectangle(0, 0, xSpace,
                                            _imageHeight),
                                            GraphicsUnit.Pixel);
                            }
                            // All space has been used up, nothing left for text
                            xDraw = xMax;
                        }
                    }
                }
            }
        }

        protected virtual void DrawTabText(Rectangle rectTab,
                                           TabPageAdvanced page,
                                           Graphics g,
                                           bool highlightText,
                                           bool highlightClose,
                                           int xMax,
                                           int xDraw)
        {
            if (!_selectedTextOnly || (_selectedTextOnly && page.Selected))
            {
                // Any space for drawing text?
                if (xDraw < xMax)
                {
                    Color drawColor;
                    SolidBrush drawBrush;
                    Font drawFont = base.Font;

                    // Decide which base color to use
                    if (highlightText)
                        drawColor = _hotTextColor;
                    else
                    {
                        // Do we modify base color depending on selection?
                        if (_dimUnselected && !page.Selected)
                        {
                            // Reduce the intensity of the color
                            drawColor = _textInactiveColor;
                        }
                        else
                            drawColor = _textColor;
                    }


                    // Should selected items be drawn in bold?
                    if (_boldSelected && page.Selected)
                        drawFont = new Font(drawFont, FontStyle.Bold);

                    Console.WriteLine("DrawText {0}", drawColor.ToString());

                    if (Math.Abs(drawColor.GetBrightness() - BackColor.GetBrightness()) < 0.5)
                        drawColor = Color.FromArgb(drawColor.R / 2, drawColor.G / 2, drawColor.B / 2);
                    // Now the color is determined, create solid brush
                    drawBrush = new SolidBrush(drawColor);

                    // Ensure only a single line is draw from then left hand side of the
                    // rectangle and if to large for line to shows ellipsis for us
                    StringFormat drawFormat = new StringFormat();
                    drawFormat.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
                    drawFormat.Trimming = StringTrimming.EllipsisCharacter;
                    drawFormat.Alignment = page.TitleAlignment;
                    drawFormat.HotkeyPrefix = HotkeyPrefix.Show;

                    // Find the vertical drawing limits for text
                    int yStart = rectTab.Top + _position[_styleIndex, (int)PositionIndex.ImageGapTop];

                    int yEnd = rectTab.Bottom -
                            _position[_styleIndex, (int)PositionIndex.ImageGapBottom] -
                            _position[_styleIndex, (int)PositionIndex.BorderBottom];

                    // Use text offset to adjust position of text
                    yStart += _position[_styleIndex, (int)PositionIndex.TextOffset];

                    // Across the text left gap
                    xDraw += _position[_styleIndex, (int)PositionIndex.TextGapLeft];

                    // Need at least 1 pixel width before trying to draw
                    if (xDraw < xMax)
                    {
                        if ((_showCloseIndividual) && (page.CanClose))
                            xMax = xMax - _buttonGap - _buttonWidth;
                        // Find drawing rectangle
                        Rectangle drawRect = new Rectangle(xDraw, yStart, xMax - xDraw, yEnd - yStart);

                        // Finally....draw the string!
                        g.DrawString(page.Title, drawFont, drawBrush, drawRect, drawFormat);

                        //if ((_showCloseIndividual) && (page.Selected || highlightText) && (page.CanClose))
                        if ((_showCloseIndividual) && (page.CanClose))
                        {
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            int cross_width = Convert.ToInt32(6 * Reportman.Drawing.GraphicUtils.DPIScale);
                            Pen pendraw;
                            if (highlightClose)
                            {
                                pendraw = new Pen(Brushes.White);
                            }
                            else
                            {
                                if (page.Selected)
                                    pendraw = new Pen(Brushes.Gray);
                                else
                                    pendraw = new Pen(Brushes.DarkGray);
                            }
                            pendraw.EndCap = LineCap.Round;
                            pendraw.StartCap = LineCap.Round;
                            pendraw.Width = 2f * Reportman.Drawing.GraphicUtils.DPIScale;
                            // g.DrawImage(_internalImages.Images[4], new Point(xMax + _buttonGap, yStart + (yEnd - yStart - _internalImages.Images[4].Height) / 2));
                            Rectangle newrec = new Rectangle(xMax + _buttonGap + cross_width / 2, yStart + (yEnd - yStart - cross_width) / 2, cross_width, cross_width);
                            if (highlightClose)
                            {
                                //g.ResetClip();
                                //g.SmoothingMode = SmoothingMode.HighQuality;
                                //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                //g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                int circle_gap = Convert.ToInt32(8 * Reportman.Drawing.GraphicUtils.DPIScale);
                                Rectangle recellipse = new Rectangle(newrec.Left - circle_gap / 2, newrec.Top - circle_gap / 2, newrec.Width + circle_gap, newrec.Height + circle_gap);

                                GraphicsPath pathClip = new GraphicsPath();
                                pathClip.AddEllipse(recellipse);
                                g.FillPath(Brushes.Red, pathClip);


                                //Rectangle recellipse = new Rectangle(xMax + _buttonGap, yStart + (yEnd - yStart - _buttonHeight) / 2, _internalImages.Images[4].Width, _internalImages.Images[4].Height);
                                //g.FillEllipse(Brushes.Red, recellipse);
                            }
                            //g.DrawRectangle(pendraw, new Rectangle(xMax + _buttonGap, yStart + (yEnd - yStart - _buttonHeight) / 2, _internalImages.Images[4].Width, _internalImages.Images[4].Height));
                            g.DrawLine(pendraw, newrec.Left, newrec.Bottom, newrec.Right, newrec.Top);
                            g.DrawLine(pendraw, newrec.Left, newrec.Top, newrec.Right, newrec.Bottom);
                        }
                    }

                    // Cleanup resources!
                    drawBrush.Dispose();
                }
            }
        }

        protected virtual void DrawTabBorder(ref Rectangle rectTab, TabPageAdvanced page, Graphics g)
        {
            if (_appearance == VisualAppearance.MultiBox)
            {
                // Adjust the drawing upwards two pixels to 'look pretty'
                rectTab.Y -= _multiBoxAdjust;

                // Draw the same regardless of style
                DrawMultiBoxBorder(page, g, rectTab);
            }
            else
            {
                // Drawing the border is style specific
                switch (_style)
                {
                    case VisualStyle.Plain:
                        DrawPlainTabBorder(page, g, rectTab);
                        break;
                    case VisualStyle.IDE:
                        DrawIDETabBorder(page, g, rectTab);
                        break;
                    case VisualStyle.Chrome:
                        DrawChromeTabBorder(page, g, rectTab);
                        break;
                }
            }
        }

        protected virtual void DrawMultiBoxBorder(TabPageAdvanced page, Graphics g, Rectangle rectPage)
        {
            if (page.Selected)
            {
                using (SolidBrush lightlight = new SolidBrush(_backLightLight))
                    g.FillRectangle(lightlight, rectPage);

                using (Pen darkdark = new Pen(_backDarkDark))
                    g.DrawRectangle(darkdark, rectPage);
            }
            else
            {
                using (SolidBrush backBrush = new SolidBrush(this.BackColor))
                    g.FillRectangle(backBrush, rectPage.X + 1, rectPage.Y, rectPage.Width - 1, rectPage.Height);

                // Find the index into TabPageAdvanced collection for this page
                int index = _tabPages.IndexOf(page);

                // Decide if the separator should be drawn
                bool drawSeparator = (index == _tabPages.Count - 1) ||
                    (index < (_tabPages.Count - 1)) &&
                    (_tabPages[index + 1].Selected != true);

                // MultiLine mode is slighty more complex
                if (_multiline && !drawSeparator)
                {
                    // By default always draw separator
                    drawSeparator = true;

                    // If we are not the last item
                    if (index < (_tabPages.Count - 1))
                    {
                        // If the next item is selected
                        if (_tabPages[index + 1].Selected == true)
                        {
                            Rectangle thisRect = (Rectangle)_tabRects[index];
                            Rectangle nextRect = (Rectangle)_tabRects[index + 1];

                            // If we are on the same drawing line then do not draw separator
                            if (thisRect.Y == nextRect.Y)
                                drawSeparator = false;
                        }
                    }
                }

                // Draw tab separator unless the next page after us is selected
                if (drawSeparator)
                {
                    using (Pen lightlight = new Pen(_backLightLight),
                              dark = new Pen(_backDark))
                    {
                        g.DrawLine(dark, rectPage.Right, rectPage.Top + 2, rectPage.Right,
                                   rectPage.Bottom - _position[_styleIndex, (int)PositionIndex.TabsBottomGap] - 1);
                        g.DrawLine(lightlight, rectPage.Right + 1, rectPage.Top + 2, rectPage.Right + 1,
                                   rectPage.Bottom - _position[_styleIndex, (int)PositionIndex.TabsBottomGap] - 1);
                    }
                }
            }
        }

        protected virtual void DrawPlainTabBorder(TabPageAdvanced page, Graphics g, Rectangle rectPage)
        {
            using (Pen light = new Pen(_backLightLight),
                      dark = new Pen(_backDark),
                      darkdark = new Pen(_backDarkDark))
            {
                int yLeftOffset = 0;
                int yRightOffset = 0;

                using (SolidBrush backBrush = new SolidBrush(base.BackColor))
                {
                    if (page.Selected)
                    {
                        // Calculate the rectangle that covers half the top border area
                        int yBorder;

                        if (_positionAtTop)
                            yBorder = rectPage.Top + (_position[_styleIndex, (int)PositionIndex.BorderTop] / 2);
                        else
                            yBorder = rectPage.Top - (_position[_styleIndex, (int)PositionIndex.BorderTop] / 2);

                        // Construct rectangle that covers the outer part of the border
                        Rectangle rectBorder = new Rectangle(rectPage.Left, yBorder, rectPage.Width - 1, rectPage.Height);

                        // Blank out area 
                        g.FillRectangle(backBrush, rectBorder);

                        // Make the left and right border lines extend higher up
                        yLeftOffset = -2;
                        yRightOffset = -1;
                    }
                }

                if (_positionAtTop)
                {
                    // Draw the left border
                    g.DrawLine(light, rectPage.Left, rectPage.Bottom, rectPage.Left, rectPage.Top + 2);
                    g.DrawLine(light, rectPage.Left + 1, rectPage.Top + 1, rectPage.Left + 1, rectPage.Top + 2);

                    // Draw the top border
                    g.DrawLine(light, rectPage.Left + 2, rectPage.Top + 1, rectPage.Right - 2, rectPage.Top + 1);

                    // Draw the right border
                    g.DrawLine(darkdark, rectPage.Right, rectPage.Bottom - yRightOffset, rectPage.Right, rectPage.Top + 2);
                    g.DrawLine(dark, rectPage.Right - 1, rectPage.Bottom - yRightOffset, rectPage.Right - 1, rectPage.Top + 2);
                    g.DrawLine(dark, rectPage.Right - 2, rectPage.Top + 1, rectPage.Right - 2, rectPage.Top + 2);
                    g.DrawLine(darkdark, rectPage.Right - 2, rectPage.Top, rectPage.Right, rectPage.Top + 2);
                }
                else
                {
                    // Draw the left border
                    g.DrawLine(light, rectPage.Left, rectPage.Top + yLeftOffset, rectPage.Left, rectPage.Bottom - 2);
                    g.DrawLine(dark, rectPage.Left + 1, rectPage.Bottom - 1, rectPage.Left + 1, rectPage.Bottom - 2);

                    // Draw the bottom border
                    g.DrawLine(dark, rectPage.Left + 2, rectPage.Bottom - 1, rectPage.Right - 2, rectPage.Bottom - 1);
                    g.DrawLine(darkdark, rectPage.Left + 2, rectPage.Bottom, rectPage.Right - 2, rectPage.Bottom);

                    // Draw the right border
                    g.DrawLine(darkdark, rectPage.Right, rectPage.Top, rectPage.Right, rectPage.Bottom - 2);
                    g.DrawLine(dark, rectPage.Right - 1, rectPage.Top + yRightOffset, rectPage.Right - 1, rectPage.Bottom - 2);
                    g.DrawLine(dark, rectPage.Right - 2, rectPage.Bottom - 1, rectPage.Right - 2, rectPage.Bottom - 2);
                    g.DrawLine(darkdark, rectPage.Right - 2, rectPage.Bottom, rectPage.Right, rectPage.Bottom - 2);
                }
            }
        }

        protected virtual void DrawChromeTabBorder(TabPageAdvanced page, Graphics g, Rectangle rectPage)
        {
            using (Pen lightlight = new Pen(_backLightLight),
                      backColor = new Pen(base.BackColor),
                      dark = new Pen(_backDark),
                      darkdark = new Pen(_backDarkDark))
            {

                // Draw background in unselected color
                //using(SolidBrush tabsAreaBrush = new SolidBrush(_backIDE))
                //    g.FillRectangle(tabsAreaBrush, rectPage);
                Color backbrushcolor = Color.FromArgb(220, 225, 231);
                if (page.Selected)
                    backbrushcolor = Color.White;
                else
                    if (page.Alerting)
                    backbrushcolor = AlertingColor;


                using (SolidBrush tabsAreaBrush = new SolidBrush(backbrushcolor), penbrush = new SolidBrush(Color.DarkGray))
                {
                    const int penwidth = 1;
                    //                    const int TABSEP = 2;
                    //                    const int CURVESEP = 7;
                    const int TABSEP = 4;
                    const int CURVESEP = 5;
                    const int CURVEMARGIN = 2;
                    using (Pen npen = new Pen(penbrush))
                    {
                        int tab_separation = Convert.ToInt32(TABSEP * GraphicUtils.DPIScale);
                        int curve_separation = Convert.ToInt32(CURVESEP * GraphicUtils.DPIScale);
                        int curve_margin = Convert.ToInt32(CURVEMARGIN * GraphicUtils.DPIScale);
                        //int curve_separation = CURVESEP;
                        //int curve_margin = CURVEMARGIN;
                        //g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        npen.Width = penwidth * (Reportman.Drawing.GraphicUtils.DPIScale);
                        Point bottomleft = new Point(rectPage.X - curve_separation, rectPage.Y + rectPage.Height);
                        Point topleft = new Point(rectPage.X + tab_separation, rectPage.Y);
                        Point topright = new Point(rectPage.X + rectPage.Width - tab_separation, rectPage.Y);
                        Point bottomright = new Point(rectPage.X + rectPage.Width + curve_separation, rectPage.Y + rectPage.Height);

                        Point bottomleftbegin = new Point(bottomleft.X - curve_margin, bottomleft.Y);
                        Point bottomleftbegin2 = new Point(bottomleft.X, bottomleft.Y - curve_margin / 2);
                        Point bottomleftcurve1 = new Point(bottomleft.X + (topleft.X - bottomleft.X) / 6, bottomleft.Y - (bottomleft.Y - topleft.Y) / 6);
                        Point bottomleftcurve2 = new Point(bottomleft.X + (topleft.X - bottomleft.X) / 4, bottomleft.Y - (bottomleft.Y - topleft.Y) / 4);
                        Point[] leftbottomcurve = new Point[4];
                        leftbottomcurve[0] = bottomleftbegin;
                        leftbottomcurve[1] = bottomleftbegin2;
                        leftbottomcurve[2] = bottomleftcurve1;
                        leftbottomcurve[3] = bottomleftcurve2;

                        Point topleftbegin = new Point(topleft.X - (topleft.X - bottomleft.X) / 6, topleft.Y + (bottomleft.Y - topleft.Y) / 6);
                        Point topleftbegin2 = new Point(topleft.X - (topleft.X - bottomleft.X) / 8, topleft.Y + (bottomleft.Y - topleft.Y) / 8);
                        Point topleftcurve1 = new Point(topleft.X, topleft.Y + curve_margin / 2);
                        Point topleftcurve2 = new Point(topleft.X + curve_margin, topleft.Y);
                        Point[] topleftcurve = new Point[4];
                        topleftcurve[0] = topleftbegin;
                        topleftcurve[1] = topleftbegin2;
                        topleftcurve[2] = topleftcurve1;
                        topleftcurve[3] = topleftcurve2;


                        Point toprightbegin = new Point(topright.X - curve_margin, topright.Y);
                        Point toprightbegin2 = new Point(topright.X, topright.Y + curve_margin / 2);
                        Point toprightcurve1 = new Point(topright.X + (bottomright.X - topright.X) / 8, topright.Y + (bottomright.Y - topright.Y) / 8);
                        Point toprightcurve2 = new Point(topright.X + (bottomright.X - topright.X) / 6, topright.Y + (bottomright.Y - topright.Y) / 6);
                        Point[] toprightcurve = new Point[4];
                        toprightcurve[0] = toprightbegin;
                        toprightcurve[1] = toprightbegin2;
                        toprightcurve[2] = toprightcurve1;
                        toprightcurve[3] = toprightcurve2;


                        Point bottomrightbegin = new Point(bottomright.X - (bottomright.X - topright.X) / 4, bottomright.Y - (bottomright.Y - topright.Y) / 4);
                        Point bottomrightbegin2 = new Point(bottomright.X - (bottomright.X - topright.X) / 6, bottomright.Y - (bottomright.Y - topright.Y) / 6);
                        Point bottomrightcurve1 = new Point(bottomright.X, bottomright.Y - curve_margin / 2);
                        Point bottomrightcurve2 = new Point(bottomright.X + curve_margin, bottomright.Y);
                        Point[] bottomrightcurve = new Point[4];
                        bottomrightcurve[0] = bottomrightbegin;
                        bottomrightcurve[1] = bottomrightbegin2;
                        bottomrightcurve[2] = bottomrightcurve1;
                        bottomrightcurve[3] = bottomrightcurve2;

                        float cornerradius = 2.5f * GraphicUtils.DPIScale;
                        PointF[] newpoints = new PointF[6];
                        newpoints[0] = bottomleftbegin;
                        newpoints[1] = bottomleft;
                        newpoints[2] = topleft;
                        newpoints[3] = topright;
                        newpoints[4] = bottomright;
                        newpoints[5] = bottomrightcurve2;
                        GraphicsPath npath = GraphicUtils.GetRoundedLine(newpoints, cornerradius);

                        /*GraphicsPath npath = new GraphicsPath();

                        npath.AddCurve(leftbottomcurve);
                        npath.AddLine(bottomleftcurve2, topleftbegin);
                        npath.AddCurve(topleftcurve);
                        npath.AddLine(topleftcurve2, toprightbegin);
                        npath.AddCurve(toprightcurve);
                        npath.AddLine(toprightcurve2, bottomrightbegin);
                        npath.AddCurve(bottomrightcurve);*/



                        npath.CloseFigure();
                        g.FillPath(tabsAreaBrush, npath);
                        if (!page.Selected)
                            g.DrawPath(npen, npath);
                        else
                        {
                            using (Pen penwhite = new Pen(backbrushcolor))
                            {
                                penwhite.Width = penwidth * (Reportman.Drawing.GraphicUtils.DPIScale);
                                g.DrawLine(penwhite, bottomleft, bottomright);
                                npath = GraphicUtils.GetRoundedLine(newpoints, cornerradius);
                                if (_reorderingtab)
                                {
                                    PointF firstpoint = newpoints[0];
                                    PointF lastpoint = newpoints[newpoints.Length - 1];
                                    PointF firstlinepoint = new PointF(0, firstpoint.Y);
                                    PointF lastlinepoint = new PointF(Width, firstpoint.Y);
                                    g.DrawLine(npen, firstlinepoint, firstpoint);
                                    g.DrawLine(npen, lastpoint, lastlinepoint);
                                }
                                /*npath = new GraphicsPath();
                                npath.AddCurve(leftbottomcurve);
                                npath.AddLine(bottomleftcurve2, topleftbegin);
                                npath.AddCurve(topleftcurve);
                                npath.AddLine(topleftcurve2, toprightbegin);
                                npath.AddCurve(toprightcurve);
                                npath.AddLine(toprightcurve2, bottomrightbegin);
                                npath.AddCurve(bottomrightcurve);*/
                                g.DrawPath(npen, npath);
                            }
                        }
                        // Ultima pagina dibuja hasta el final
                        if (page == TabPages[TabPages.Count - 1])
                        {
                            g.DrawLine(npen, bottomrightcurve2, new Point(Width, bottomright.Y));
                            //g.DrawLine(npen, bottomright, new Point(Width,bottomright.Y));
                        }


                    }
                }


                // Find the index into TabPageAdvanced collection for this page
                /*int index = _tabPages.IndexOf(page);

                // Decide if the separator should be drawn
                bool drawSeparator = (index == _tabPages.Count - 1) ||
                                     (index < (_tabPages.Count - 1)) && 
                                     (_tabPages[index+1].Selected != true);

                // MultiLine mode is slighty more complex
                if (_multiline && !drawSeparator)
                {
                    // By default always draw separator
                    drawSeparator = true;

                    // If we are not the last item
                    if (index < (_tabPages.Count - 1))
                    {
                        // If the next item is selected
                        if (_tabPages[index+1].Selected == true)
                        {
                            Rectangle thisRect = (Rectangle)_tabRects[index];
                            Rectangle nextRect = (Rectangle)_tabRects[index+1];

                            // If we are on the same drawing line then do not draw separator
                            if (thisRect.Y == nextRect.Y)
                                drawSeparator = false;
                        }
                    }
                }
                */
                // Draw tab separator unless the next page after us is selected
                /*if (drawSeparator)
                {
                    // Reduce the intensity of the color
                    using(Pen linePen = new Pen(_textInactiveColor))
                        g.DrawLine(linePen, rectPage.Right, rectPage.Top + 2, rectPage.Right, 
                            rectPage.Bottom - _position[_styleIndex, (int)PositionIndex.TabsBottomGap] - 1);
                }*/
            }
        }
        protected virtual void DrawIDETabBorder(TabPageAdvanced page, Graphics g, Rectangle rectPage)
        {
            using (Pen lightlight = new Pen(_backLightLight),
                      backColor = new Pen(base.BackColor),
                      dark = new Pen(_backDark),
                      darkdark = new Pen(_backDarkDark))
            {
                if (page.Selected)
                {
                    // Draw background in selected color
                    using (SolidBrush pageAreaBrush = new SolidBrush(this.BackColor))
                        g.FillRectangle(pageAreaBrush, rectPage);

                    if (_positionAtTop)
                    {
                        // Overdraw the bottom border
                        g.DrawLine(backColor, rectPage.Left, rectPage.Bottom, rectPage.Right - 1, rectPage.Bottom);

                        // Draw the right border
                        g.DrawLine(darkdark, rectPage.Right, rectPage.Top, rectPage.Right, rectPage.Bottom);
                    }
                    else
                    {
                        // Draw the left border
                        g.DrawLine(lightlight, rectPage.Left, rectPage.Top - 1, rectPage.Left, rectPage.Bottom);

                        // Draw the bottom border
                        g.DrawLine(darkdark, rectPage.Left + 1, rectPage.Bottom, rectPage.Right, rectPage.Bottom);

                        // Draw the right border
                        g.DrawLine(darkdark, rectPage.Right, rectPage.Top - 1, rectPage.Right, rectPage.Bottom);

                        // Overdraw the top border
                        g.DrawLine(backColor, rectPage.Left + 1, rectPage.Top - 1, rectPage.Right - 1, rectPage.Top - 1);
                    }
                }
                else
                {
                    // Draw background in unselected color
                    using (SolidBrush tabsAreaBrush = new SolidBrush(_backIDE))
                        g.FillRectangle(tabsAreaBrush, rectPage);

                    // Find the index into TabPageAdvanced collection for this page
                    int index = _tabPages.IndexOf(page);

                    // Decide if the separator should be drawn
                    bool drawSeparator = (index == _tabPages.Count - 1) ||
                                         (index < (_tabPages.Count - 1)) &&
                                         (_tabPages[index + 1].Selected != true);

                    // MultiLine mode is slighty more complex
                    if (_multiline && !drawSeparator)
                    {
                        // By default always draw separator
                        drawSeparator = true;

                        // If we are not the last item
                        if (index < (_tabPages.Count - 1))
                        {
                            // If the next item is selected
                            if (_tabPages[index + 1].Selected == true)
                            {
                                Rectangle thisRect = (Rectangle)_tabRects[index];
                                Rectangle nextRect = (Rectangle)_tabRects[index + 1];

                                // If we are on the same drawing line then do not draw separator
                                if (thisRect.Y == nextRect.Y)
                                    drawSeparator = false;
                            }
                        }
                    }

                    // Draw tab separator unless the next page after us is selected
                    if (drawSeparator)
                    {
                        // Reduce the intensity of the color
                        using (Pen linePen = new Pen(_textInactiveColor))
                            g.DrawLine(linePen, rectPage.Right, rectPage.Top + 2, rectPage.Right,
                                rectPage.Bottom - _position[_styleIndex, (int)PositionIndex.TabsBottomGap] - 1);
                    }
                }
            }
        }

        protected virtual void OnClearingPages()
        {
            // Is a page currently selected?
            if (_pageSelected != -1)
            {
                // Deselect the page
                DeselectPage(_tabPages[_pageSelected]);

                // Remember that nothing is selected
                _pageSelected = -1;
                _startPage = -1;
            }

            // Remove all the user controls 
            foreach (TabPageAdvanced page in _tabPages)
                RemoveTabPage(page);

            // Remove all rectangles associated with TabPageAdvanced's
            _tabRects.Clear();
        }

        protected virtual void OnClearedPages()
        {
            // Must recalculate after the pages have been removed and
            // not before as that would calculate based on pages still
            // being present in the list
            Recalculate();

            // Raise selection changing event
            OnSelectionChanging(this, new CancelArgs(null));

            // Must notify a change in selection
            OnSelectionChanged(EventArgs.Empty);

            Invalidate();
        }

        protected virtual void OnInsertingPage(int index, object value)
        {
            // If a page currently selected?
            if (_pageSelected != -1)
            {
                // Is the selected page going to be after this new one in the list
                if (_pageSelected >= index)
                    _pageSelected++;  // then need to update selection index to reflect this
            }
        }

        protected virtual void OnInsertedPage(int index, object value)
        {
            bool selectPage = false;

            TabPageAdvanced page = value as TabPageAdvanced;

            // Hookup to receive TabPageAdvanced property changes
            page.PropertyChanged += new TabPageAdvanced.PropChangeHandler(OnPagePropertyChanged);

            // Add the appropriate Control/Form/TabPageAdvanced to the control
            AddTabPage(page);

            // Do we want to select this page?
            if ((_pageSelected == -1) || (page.Selected))
            {
                // Raise selection changing event
                OnSelectionChanging(this, new CancelArgs(page));

                // Any page currently selected
                if (_pageSelected != -1)
                    DeselectPage(_tabPages[_pageSelected]);

                // This becomes the newly selected page
                _pageSelected = _tabPages.IndexOf(page);

                // If no page is currently defined as the start page
                if (_startPage == -1)
                    _startPage = 0;	 // then must be added then first page

                // Request the page be selected
                selectPage = true;
            }

            // Add new rectangle to match new number of pages, this must be done before
            // the 'SelectPage' or 'OnSelectionChanged' to ensure the number of _tabRects 
            // entries matches the number of _tabPages entries.
            _tabRects.Add((object)new Rectangle());

            // Cause the new page to be the selected one
            if (selectPage)
            {
                // Must recalculate to ensure the new _tabRects entry above it correctly
                // filled in before the new page is selected, as a change in page selection
                // may cause the _tabRects values ot be interrogated.
                Recalculate();

                SelectPage(page);

                // Raise selection change event
                OnSelectionChanged(EventArgs.Empty);
            }

            Recalculate();
            Invalidate();
        }

        protected virtual void OnRemovingPage(int index, object value)
        {
            TabPageAdvanced page = value as TabPageAdvanced;

            page.PropertyChanged -= new TabPageAdvanced.PropChangeHandler(OnPagePropertyChanged);

            // Remove the appropriate Control/Form/TabPageAdvanced to the control
            RemoveTabPage(page);

            // Notice a change in selected page
            _changed = false;

            // Is this the currently selected page
            if (_pageSelected == index)
            {
                // Raise selection changing event
                OnSelectionChanging(this, new CancelArgs(page));

                _changed = true;
                DeselectPage(page);
            }
        }
        public bool Close(TabPageAdvanced npage)
        {
            if (ClosePressed != null)
            {
                ClosePressed(this, new CancelArgs(npage));
            }
            return (this.TabPages.IndexOf(npage) < 0);
        }
        protected virtual void OnRemovedPage(int index, object value)
        {
            if (_hotTrackPage == index)
            {
                _hotTrackPage = -1;
            }
            // Is first displayed page then one being removed?
            if (_startPage >= index)
            {
                // Decrement to use start displaying previous page
                _startPage--;

                // Have we tried to select off the left hand side?
                if (_startPage == -1)
                {
                    // Are there still some pages left?
                    if (_tabPages.Count > 0)
                        _startPage = 0;
                }
            }

            // Is the selected page equal to or after this new one in the list
            if (_pageSelected >= index)
            {
                // Decrement index to reflect this change
                _pageSelected--;

                // Have we tried to select off the left hand side?
                if (_pageSelected == -1)
                {
                    // Are there still some pages left?
                    if (_tabPages.Count > 0)
                        _pageSelected = 0;
                }

                // Is the new selection valid?
                if (_pageSelected != -1)
                    SelectPage(_tabPages[_pageSelected]);  // Select it
            }

            // Change in selection causes event generation
            if (_changed)
            {
                // Reset changed flag
                _changed = false;

                // Raise selection change event
                OnSelectionChanged(EventArgs.Empty);
            }

            // Remove a rectangle to match number of pages
            _tabRects.RemoveAt(0);

            Recalculate();
            Invalidate();
        }

        protected virtual void AddTabPage(TabPageAdvanced page)
        {
            // Has not been shown for the first time yet
            page.Shown = false;

            // Add user supplied control 
            if (page.Control != null)
            {
                Form controlIsForm = page.Control as Form;

                page.Control.Hide();

                // Adding a Form takes extra effort
                if (controlIsForm == null)
                {
                    // Monitor focus changes on the Control
                    page.Control.GotFocus += new EventHandler(OnPageEnter);
                    page.Control.LostFocus += new EventHandler(OnPageLeave);
                    page.Control.MouseEnter += new EventHandler(OnPageMouseEnter);
                    page.Control.MouseLeave += new EventHandler(OnPageMouseLeave);

                    // Must fill the entire hosting panel it is on
                    page.Control.Dock = DockStyle.None;

                    // Set correct size
                    page.Control.Location = new Point(0, 0);
                    page.Control.Size = _hostPanel.Size;

                    _hostPanel.Controls.Add(page.Control);
                }
                else
                {
                    // Monitor activation changes on the TabPageAdvanced
                    controlIsForm.Activated += new EventHandler(OnPageEnter);
                    controlIsForm.Deactivate += new EventHandler(OnPageLeave);
                    controlIsForm.MouseEnter += new EventHandler(OnPageMouseEnter);
                    controlIsForm.MouseLeave += new EventHandler(OnPageMouseLeave);

                    // Have to ensure the Form is not a top level form
                    controlIsForm.TopLevel = false;

                    // We are the new parent of this form
                    controlIsForm.Parent = _hostPanel;

                    // To prevent user resizing the form manually and prevent
                    // the caption bar appearing, we use the 'None' border style.
                    controlIsForm.FormBorderStyle = FormBorderStyle.None;

                    // Must fill the entire hosting panel it is on
                    controlIsForm.Dock = DockStyle.None;

                    // Set correct size
                    controlIsForm.Location = new Point(0, 0);
                    controlIsForm.Size = _hostPanel.Size;
                }

                // Need to monitor when the Form/Panel is clicked
                if ((page.Control is Form) || (page.Control is Panel))
                    page.Control.MouseDown += new MouseEventHandler(OnPageMouseDown);

                RecursiveMonitor(page.Control, true);
            }
            else
            {
                page.Hide();

                // Monitor focus changes on the TabPageAdvanced
                page.GotFocus += new EventHandler(OnPageEnter);
                page.LostFocus += new EventHandler(OnPageLeave);
                page.MouseEnter += new EventHandler(OnPageMouseEnter);
                page.MouseLeave += new EventHandler(OnPageMouseLeave);

                // Must fill the entire hosting panel it is on
                page.Dock = DockStyle.None;

                // Need to monitor when the Panel is clicked
                page.MouseDown += new MouseEventHandler(OnPageMouseDown);

                RecursiveMonitor(page, true);

                // Set correct size
                page.Location = new Point(0, 0);
                page.Size = _hostPanel.Size;

                // Add the TabPageAdvanced itself instead
                _hostPanel.Controls.Add(page);
            }
        }

        protected virtual void RemoveTabPage(TabPageAdvanced page)
        {
            // Remove user supplied control 
            if (page.Control != null)
            {
                RecursiveMonitor(page.Control, false);

                Form controlIsForm = page.Control as Form;

                // Need to unhook hooked up event
                if ((page.Control is Form) || (page.Control is Panel))
                    page.Control.MouseDown -= new MouseEventHandler(OnPageMouseDown);

                if (controlIsForm == null)
                {
                    // Unhook event monitoring
                    page.Control.GotFocus -= new EventHandler(OnPageEnter);
                    page.Control.LostFocus -= new EventHandler(OnPageLeave);
                    page.Control.MouseEnter -= new EventHandler(OnPageMouseEnter);
                    page.Control.MouseLeave -= new EventHandler(OnPageMouseLeave);

                    // Use helper method to circumvent form Close bug
                    ControlHelper.Remove(_hostPanel.Controls, page.Control);
                }
                else
                {
                    // Unhook activation monitoring
                    controlIsForm.Activated -= new EventHandler(OnPageEnter);
                    controlIsForm.Deactivate -= new EventHandler(OnPageLeave);
                    controlIsForm.MouseEnter -= new EventHandler(OnPageMouseEnter);
                    controlIsForm.MouseLeave -= new EventHandler(OnPageMouseLeave);

                    // Remove Form but prevent the Form close bug
                    ControlHelper.RemoveForm(_hostPanel, controlIsForm);
                }
            }
            else
            {
                RecursiveMonitor(page, false);

                // Need to unhook hooked up event
                page.MouseDown -= new MouseEventHandler(OnPageMouseDown);

                // Unhook event monitoring
                page.GotFocus -= new EventHandler(OnPageEnter);
                page.LostFocus -= new EventHandler(OnPageLeave);
                page.MouseEnter -= new EventHandler(OnPageMouseEnter);
                page.MouseLeave -= new EventHandler(OnPageMouseLeave);

                // Use helper method to circumvent form Close bug
                ControlHelper.Remove(_hostPanel.Controls, page);
            }
        }

        protected virtual void OnPageMouseDown(object sender, MouseEventArgs e)
        {
            Control c = sender as Control;

            // If the mouse has been clicked and it does not have 
            // focus then it should receive the focus immediately.
            if (!c.ContainsFocus)
                c.Focus();
        }

        protected virtual void RecursiveMonitor(Control top, bool monitor)
        {
            foreach (Control c in top.Controls)
            {
                if (monitor)
                {
                    // Monitor focus changes on the Control
                    c.GotFocus += new EventHandler(OnPageEnter);
                    c.LostFocus += new EventHandler(OnPageLeave);
                    c.MouseEnter += new EventHandler(OnPageMouseEnter);
                    c.MouseLeave += new EventHandler(OnPageMouseLeave);
                }
                else
                {
                    // Unmonitor focus changes on the Control
                    c.GotFocus -= new EventHandler(OnPageEnter);
                    c.LostFocus -= new EventHandler(OnPageLeave);
                    c.MouseEnter -= new EventHandler(OnPageMouseEnter);
                    c.MouseLeave -= new EventHandler(OnPageMouseLeave);
                }

                RecursiveMonitor(c, monitor);
            }
        }

        protected virtual void OnPageEnter(object sender, EventArgs e)
        {
            OnPageGotFocus(e);
        }

        protected virtual void OnPageLeave(object sender, EventArgs e)
        {
            OnPageLostFocus(e);
        }

        protected virtual void OnPageMouseEnter(object sender, EventArgs e)
        {
            _mouseOver = true;
            _overTimer.Stop();

            if (_hideTabsMode == HideTabsModes.HideWithoutMouse)
            {
                Recalculate();
                Invalidate();
            }
        }

        protected virtual void OnPageMouseLeave(object sender, EventArgs e)
        {
            _overTimer.Start();
        }

        protected virtual void OnMouseTick(object sender, EventArgs e)
        {
            _mouseOver = false;
            _overTimer.Stop();

            if (_hideTabsMode == HideTabsModes.HideWithoutMouse)
            {
                Recalculate();
                Invalidate();
            }
        }

        protected virtual void OnPagePropertyChanged(TabPageAdvanced page, TabPageAdvanced.Property prop, object oldValue)
        {
            switch (prop)
            {
                case TabPageAdvanced.Property.Control:
                    Control pageControl = oldValue as Control;

                    // Is there a Control to be removed?
                    if (pageControl != null)
                    {
                        // Use helper method to circumvent form Close bug
                        ControlHelper.Remove(this.Controls, pageControl);
                    }
                    else
                    {
                        // Use helper method to circumvent form Close bug
                        ControlHelper.Remove(this.Controls, page); // remove the whole TabPageAdvanced instead
                    }

                    // Add the appropriate Control/Form/TabPageAdvanced to the control
                    AddTabPage(page);

                    // Is a page currently selected?
                    if (_pageSelected != -1)
                    {
                        // Is the change in Control for this page?
                        if (page == _tabPages[_pageSelected])
                            SelectPage(page);   // make Control visible
                    }

                    Recalculate();
                    Invalidate();
                    break;
                case TabPageAdvanced.Property.Title:
                case TabPageAdvanced.Property.ImageIndex:
                case TabPageAdvanced.Property.ImageList:
                case TabPageAdvanced.Property.Icon:
                case TabPageAdvanced.Property.TabWidth:

                    _recalculate = true;
                    Invalidate();
                    break;
                case TabPageAdvanced.Property.IconFrame:
                    Invalidate();
                    break;
                case TabPageAdvanced.Property.Selected:
                    // Becoming selected?
                    if (page.Selected)
                    {
                        // Move selection to the new page and update page properties
                        MovePageSelection(page);
                        MakePageVisible(page);
                    }
                    break;
            }
        }

        protected virtual Control FindFocus(Control root)
        {
            // Does the root control has focus?
            if (root.Focused)
                return root;

            // Check for focus at each child control
            foreach (Control c in root.Controls)
            {
                Control child = FindFocus(c);

                if (child != null)
                    return child;
            }

            return null;
        }

        protected virtual void DeselectPage(TabPageAdvanced page)
        {
            page.Selected = false;

            // Hide any associated control
            if (page.Control != null)
            {
                // Should we remember which control had focus when leaving?
                if (_recordFocus)
                {
                    // Record current focus location on Control
                    if (page.Control.ContainsFocus)
                        page.StartFocus = FindFocus(page.Control);
                    else
                        page.StartFocus = null;
                }

                page.Control.Hide();
            }
            else
            {
                // Should we remember which control had focus when leaving?
                if (_recordFocus)
                {
                    // Record current focus location on Control
                    if (page.ContainsFocus)
                        page.StartFocus = FindFocus(page);
                    else
                        page.StartFocus = null;
                }
                page.Hide();
            }
        }

        protected virtual void SelectPage(TabPageAdvanced page)
        {
            page.Selected = true;

            // Bring the control for this page to the front
            if (page.Control != null)
                HandleShowingTabPage(page, page.Control);
            else
                HandleShowingTabPage(page, page);
        }

        protected virtual void HandleShowingTabPage(TabPageAdvanced page, Control c)
        {
            // First time this page has been displayed?
            if (!page.Shown)
            {
                // Special testing needed for Forms
                Form f = c as Form;

                // AutoScaling can cause the Control/Form to be
                if ((f != null) && (f.AutoScaleMode != System.Windows.Forms.AutoScaleMode.None))
                {
                    // Workaround the problem where a form has a defined 'AutoScaleBaseSize' value. The 
                    // first time it is shown it calculates the size of each contained control and scales 
                    // as needed. But if the contained control is Dock=DockStyle.Fill it scales up/down so 
                    // its not actually filling the space! Get around by hiding and showing to force correct 
                    // calculation.
                    c.Show();
                    c.Hide();
                }

                // Only need extra logic first time around
                page.Shown = true;
            }

            // Finally, show it!
            c.Show();

            // Restore focus to last know control to have it
            if (page.StartFocus != null)
                page.StartFocus.Focus();
            else
            {
                c.Focus();
            }
        }

        protected virtual void MovePageSelection(TabPageAdvanced page)
        {
            int pageIndex = _tabPages.IndexOf(page);

            if (!AllowLastTabReordering)
            {
                if (pageIndex == _tabPages.Count - 1)
                    _leftMouseDown = false;
            }
            if (pageIndex != _pageSelected)
            {

                // Raise selection changing event
                OnSelectionChanging(this, new CancelArgs(page));

                // Any page currently selected?
                if (_pageSelected != -1)
                    DeselectPage(_tabPages[_pageSelected]);

                _pageSelected = pageIndex;

                if (_pageSelected != -1)
                    SelectPage(_tabPages[_pageSelected]);

                // Change in selection causes tab pages sizes to change
                if (_boldSelected || _selectedTextOnly || !_shrinkPagesToFit || _multiline)
                {
                    Recalculate();
                    Invalidate();
                }

                // Raise selection change event
                OnSelectionChanged(EventArgs.Empty);

                Invalidate();
            }
        }

        // Used by the TabControlDesigner
        internal bool WantDoubleClick(IntPtr hWnd, Point mousePos)
        {
            return ControlWantDoubleClick(hWnd, mousePos, _leftArrow) ||
                   ControlWantDoubleClick(hWnd, mousePos, _rightArrow) ||
                   ControlWantDoubleClick(hWnd, mousePos, _dropDownButton) ||
                   ControlWantDoubleClick(hWnd, mousePos, _closeButton);
        }

        // Used by the TabControlDesigner
        internal void ExternalMouseTest(IntPtr hWnd, Point mousePos)
        {
            if (!(ControlMouseTest(hWnd, mousePos, _leftArrow) ||
                  ControlMouseTest(hWnd, mousePos, _rightArrow) ||
                  ControlMouseTest(hWnd, mousePos, _dropDownButton) ||
                  ControlMouseTest(hWnd, mousePos, _closeButton)))
                InternalMouseDown(mousePos);
        }

        protected virtual bool ControlWantDoubleClick(IntPtr hWnd, Point mousePos, Control check)
        {
            // Cannot have double click if control not visible
            if (check.Visible)
            {
                // Is double click for this control?
                if (check.Enabled && (hWnd == check.Handle))
                {
                    if (check == _leftArrow)
                        OnLeftArrow(null, EventArgs.Empty);

                    if (check == _rightArrow)
                        OnRightArrow(null, EventArgs.Empty);

                    return true;
                }
                else
                {
                    // Create rectangle for control position
                    Rectangle checkRect = new Rectangle(check.Location.X,
                                                        check.Location.Y,
                                                        check.Width,
                                                        check.Height);

                    // Was double click over a disabled button?
                    if (checkRect.Contains(mousePos))
                        return true;
                }
            }

            return false;
        }

        protected virtual bool ControlMouseTest(IntPtr hWnd, Point mousePos, Control check)
        {
            // Is the mouse down for the left arrow window and is it valid to click?
            if ((hWnd == check.Handle) && check.Visible && check.Enabled)
            {
                // Check if the mouse click is over the left arrow
                if (check.ClientRectangle.Contains(mousePos))
                {
                    if (check == _leftArrow)
                        OnLeftArrow(null, EventArgs.Empty);

                    if (check == _rightArrow)
                        OnRightArrow(null, EventArgs.Empty);

                    return true;
                }
            }

            return false;
        }

        protected override void OnDoubleClick(EventArgs e)
        {
           /* Point pos = TabControlAdvanced.MousePosition;

            int count = _tabRects.Count;

            for (int index = 0; index < count; index++)
            {
                // Get tab drawing rectangle
                Rectangle local = (Rectangle)_tabRects[index];

                // If drawing on the control
                if (local != _nullPosition)
                {
                    // Convert from Control to screen coordinates
                    Rectangle screen = this.RectangleToScreen(local);

                    if (screen.Contains(pos))
                    {
                        // Generate appropriate event
                        OnDoubleClickTab(_tabPages[index]);
                        break;
                    }
                }
            }*/

            base.OnDoubleClick(e);
        }




        protected override void OnMouseUp(MouseEventArgs e)
        {
            Capture = false;
            _leftMouseDown = false;
            Point mousePos = new Point(e.X, e.Y);

            if (_reorderingtab)
            {
                _reorderingtab = false;
                ExecuteReOrdertab(mousePos);
                this.Update();
                this.Invalidate();
                OnMouseMove(e);
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                // Check for page close
                for (int i = 0; i < _tabPages.Count; i++)
                {
                    Rectangle rect = (Rectangle)_tabRects[i];

                    if (rect.Contains(mousePos))
                    {
                        if ((_showCloseIndividual) && (_tabPages[i].CanClose))
                        {
                            if (mousePos.X > (rect.Right - _buttonWidth))
                            {
                                _leftMouseDown = false;
                                OnClosePressed(new CancelArgs(_tabPages[i]));
                                _hotTrackPage = -1;
                                _hotTrackPageHightlightClose = false;
                                this.Update();
                                OnMouseMove(e);
                                return;
                            }
                        }
                    }
                }
            }


            if (_leftMouseDownDrag)
            {
                // Generate event for interested parties
                if (e.Button == MouseButtons.Left)
                    OnPageDragEnd(e);
                else
                    OnPageDragQuit(e);

                _leftMouseDownDrag = false;
                _ignoreDownDrag = true;
            }

            if (e.Button == MouseButtons.Left)
            {
                // Exit any page dragging attempt
                _leftMouseDown = false;
            }
            else
            {
                // Is it the button that causes context menu to show?
                if (e.Button == MouseButtons.Right)
                {

                    // Is the mouse in the tab area
                    if (_tabsAreaRect.Contains(mousePos))
                    {
                        CancelEventArgs ce = new CancelEventArgs();

                        // Generate event giving handlers cancel to update/cancel menu
                        OnPopupMenuDisplay(ce);

                        // Still want the popup?
                        if (!ce.Cancel)
                        {
                            // Is there any attached menu to show
                            if (_contextMenu != null)
                                _contextMenu.Show(this.PointToScreen(new Point(e.X, e.Y)));
                        }
                    }
                }
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Only select a button or page when using left mouse button
            InternalMouseDown(new Point(e.X, e.Y));

            base.OnMouseDown(e);
        }
        bool moving = false;
        Point toolbarorigin;
        protected virtual void InternalMouseDown(Point mousePos)
        {
            moving = false;
            if (Control.MouseButtons == MouseButtons.Left)
            {
                _reorderingtab = false;
            }
            bool clickedonbar = true;
            // Clicked on a tab page?
            for (int i = 0; i < _tabPages.Count; i++)
            {
                Rectangle rect = (Rectangle)_tabRects[i];

                if (rect.Contains(mousePos))
                {
                    clickedonbar = false;
                    if ((_showCloseIndividual) && (_tabPages[i].CanClose))
                    {
                        if (mousePos.X > rect.Left + rect.Width - _buttonWidth)
                            return;
                    }

                    // Are the scroll buttons being shown?
                    if (_leftArrow.Visible)
                    {
                        // Ignore mouse down over then buttons area
                        if (mousePos.X >= _leftArrow.Left)
                            break;
                    }
                    else
                    {
                        // No, is the close button visible?
                        if (_closeButton.Visible)
                        {
                            // Ignore mouse down over then close button area
                            if ((mousePos.X >= _closeButton.Left) && (_tabPages[i].CanClose))
                                break;
                        }
                        else
                        {
                            if (_dropDownButton.Visible)
                            {
                                // Ignore mouse down over then dropdown button area
                                if (mousePos.X >= _dropDownButton.Left)
                                    break;
                            }
                        }
                    }

                    // Remember where the left mouse was initially pressed
                    if (Control.MouseButtons == MouseButtons.Left)
                    {
                        _leftMouseDown = true;
                        _ignoreDownDrag = false;
                        _leftMouseDownDrag = false;
                        _leftMouseDownPos = mousePos;
                        Capture = true;
                    }

                    MovePageSelection(_tabPages[i]);
                    MakePageVisible(_tabPages[i]);
                    break;
                }
            }
            if (clickedonbar && EmptyMoveForm)
            {
                toolbarorigin = mousePos;
                Capture = true;
                moving = true;
            }
        }
        private void ExecuteReOrdertab(Point mousePos)
        {
            Rectangle? rectTabSelected = null;
            int selectedIndex = -1;
            for (int i = 0; i < TabPages.Count; i++)
            {
                TabPageAdvanced page = TabPages[i];
                if (SelectedTab == page)
                {
                    rectTabSelected = (Rectangle)_tabRects[i];
                    selectedIndex = i;
                    break;
                }
            }
            if (rectTabSelected == null)
                return;
            int replaceIndex = -1;
            for (int i = 0; i < TabPages.Count; i++)
            {
                bool checktab = true;
                if (i == TabPages.Count - 1)
                    if (!AllowLastTabReordering)
                        checktab = false;
                TabPageAdvanced page = TabPages[i];
                if (SelectedTab == page)
                    checktab = false;
                if (i == selectedIndex)
                    checktab = false;
                if (checktab)
                {
                    Rectangle rectTab = (Rectangle)_tabRects[i];
                    int newx = rectTabSelected.Value.Left + mousePos.X - _leftMouseDownPos.X + rectTabSelected.Value.Width / 2;
                    if (newx < 0)
                        newx = 0;
                    if (newx + rectTabSelected.Value.Width > Width)
                        newx = Width - rectTabSelected.Value.Width;
                    if ((newx > rectTab.Left + rectTab.Width / 3) && (newx < rectTab.Left + rectTab.Width * 2 / 3))
                    {
                        replaceIndex = i;
                        break;
                    }
                }
            }
            if (replaceIndex >= 0)
            {
                Rectangle rectTabreplaced = (Rectangle)_tabRects[replaceIndex];
                Rectangle rectTabsesected = (Rectangle)_tabRects[selectedIndex];
                _leftMouseDownPos = new Point(_leftMouseDownPos.X + rectTabreplaced.Left - rectTabsesected.Left
                    , _leftMouseDownPos.Y);

                _tabPages.Switch(selectedIndex, replaceIndex);
                _pageSelected = replaceIndex;
                Recalculate();
            }
        }
        private void GetHighLightStatus(TabPageAdvanced page, ref bool highlighttext, ref bool highlightclose)
        {
            Point mousePos = Cursor.Position;
            mousePos = this.PointToClient(mousePos);
            GetHighLightStatus(page, ref highlighttext, ref highlightclose, mousePos);
        }
        private void GetHighLightStatus(TabPageAdvanced page, ref bool highlighttext, ref bool highlightclose, Point mousePos)
        {
            Rectangle rect = new Rectangle();
            highlightclose = false;
            highlighttext = false;
            int mousePage = -1;
            // Find the page this mouse point is inside
            for (int pos = 0; pos < _tabPages.Count; pos++)
            {
                rect = (Rectangle)_tabRects[pos];

                if (rect.Contains(mousePos))
                {
                    mousePage = pos;
                    break;
                }
            }
            if (mousePage < 0)
                return;
            if (_tabPages[mousePage] != page)
                return;
            highlighttext = true;
            if (rect.Contains(mousePos))
            {

                if ((_showCloseIndividual) && (_tabPages[mousePage].CanClose))
                {
                    if (mousePos.X > (rect.Right - _buttonWidth))
                    {
                        highlightclose = true;
                        if (_leftArrow.Visible)
                        {
                            // Ignore mouse down over then buttons area
                            if (mousePos.X >= _leftArrow.Left)
                                highlightclose = false;
                        }
                        else
                        {
                            // No, is the close button visible?
                            if (_closeButton.Visible)
                            {
                                // Ignore mouse down over then close button area
                                if ((mousePos.X >= _closeButton.Left) && (_tabPages[mousePage].CanClose))
                                    highlightclose = false;
                            }
                            else
                            {
                                if (_dropDownButton.Visible)
                                {
                                    // Ignore mouse down over then dropdown button area
                                    if (mousePos.X >= _dropDownButton.Left)
                                        highlightclose = false;
                                }
                            }

                        }
                    }
                }
            }
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            Point pos = this.PointToScreen(new Point(e.X,e.Y));

            bool insidetab = false;
            int count = _tabRects.Count;

            for (int index = 0; index < count; index++)
            {
                // Get tab drawing rectangle
                Rectangle local = (Rectangle)_tabRects[index];

                // If drawing on the control
                if (local != _nullPosition)
                {
                    // Convert from Control to screen coordinates
                    Rectangle screen = this.RectangleToScreen(local);

                    if (screen.Contains(pos))
                    {
                        // Generate appropriate event
                        OnDoubleClickTab(_tabPages[index]);
                        insidetab = true;
                        break;
                    }
                }
            }
            if (!insidetab)
            {
                Form nform = this.FindForm();
                if (nform != null)
                {
                    if (nform is NoTitleForm)
                    {
                        NoTitleForm notitleform = (NoTitleForm)nform;
                        if (!notitleform.ShowTitle)
                        {
                            // Check position
                            notitleform.SwitchMaximizeMinimize();
                        }
                    }
                }
            }
            base.OnMouseDoubleClick(e);
        }
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int WM_NCLBUTTONUP = 0xA2;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        protected override void OnMouseMove(MouseEventArgs e)
        {

            if ((Capture) && (moving))
            {
                moving = false;
                ReleaseCapture();
                SendMessage(this.FindForm().Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                /*Point newlocation = e.Location;
                int difx = newlocation.X - toolbarorigin.X;
                int dify = newlocation.Y - toolbarorigin.Y;
                SetBounds(Left + difx, Top + dify, Width, Height);*/
                //toolbarorigin = e.Location;
            }

            if (_leftMouseDown)
            {
                if (AllowTabReordering)
                {
                    if (!_reorderingtab)
                    {
                        if (Math.Abs(_leftMouseDownPos.X - e.X) > _MouseOffsetTriggerReorder)
                        {
                           
                            _reorderingtab = true;
                        }
                    }
                    if (_reorderingtab)
                    {
                        ExecuteReOrdertab(new Point(e.X, e.Y));
                        _recalculate = true;
                        this.Invalidate();
                        return;
                    }
                }
                if (!_leftMouseDownDrag)
                {
                    Point thisPosition = new Point(e.X, e.Y);
        
                    bool startDrag = false;

                    if (_dragFromControl)
                        startDrag = !this.ClientRectangle.Contains(thisPosition);
                    else
                    {
                        // Create starting mouse down position
                        Rectangle dragRect = new Rectangle(_leftMouseDownPos, new Size(0,0));
                        
                        // Expand by size of the double click area
                        dragRect.Inflate(SystemInformation.DoubleClickSize);
                        
                        // Drag when mouse moves outside the double click area
                        startDrag = !dragRect.Contains(thisPosition);
                    }

                    if (startDrag && !_ignoreDownDrag)
                    {
                        // Generate event for interested parties
                        OnPageDragStart(e);
		
                        // Enter dragging mode
                        _leftMouseDownDrag = true;	
                    }
                }
                else
                {
                    // Generate event for interested parties
                    OnPageDragMove(e);
                }
            }
            else
            {
                if (_hotTrack || _hoverSelect)
                {
                    int mousePage = -1;
                    bool pageChanged = false;

                    // Create a point representing current mouse position
                    Point mousePos = new Point(e.X, e.Y);
                    Rectangle rect = new Rectangle();
                    // Find the page this mouse point is inside
                    for (int pos=0; pos<_tabPages.Count; pos++)
                    {
                        rect = (Rectangle)_tabRects[pos];

                        if (rect.Contains(mousePos))
                        {
                            mousePage = pos;
                            break;
                        }
                    }

                    // Should moving over a tab cause selection changes?
                    if (_hoverSelect && !_multiline && (mousePage != -1))
                    {
                        // Has the selected page changed?
                        if (mousePage != _pageSelected)
                        {
                            // Move selection to new page
                            MovePageSelection(_tabPages[mousePage]);

                            pageChanged = true;
                        }
                    }
                    bool hightlightClose = false;
                    if (mousePage >= 0)
                    {
                        if (rect.Contains(mousePos))
                        {

                            if ((_showCloseIndividual) && (_tabPages[mousePage].CanClose))
                            {
                                if (mousePos.X > (rect.Right - _buttonWidth))
                                {
                                    hightlightClose = true;
                                    if (_leftArrow.Visible)
                                    {
                                        // Ignore mouse down over then buttons area
                                        if (mousePos.X >= _leftArrow.Left)
                                            hightlightClose = false;
                                    }
                                    else
                                    {
                                        // No, is the close button visible?
                                        if (_closeButton.Visible)
                                        {
                                            // Ignore mouse down over then close button area
                                            if ((mousePos.X >= _closeButton.Left) && (_tabPages[mousePage].CanClose))
                                                hightlightClose = false;
                                        }
                                        else
                                        {
                                            if (_dropDownButton.Visible)
                                            {
                                                // Ignore mouse down over then dropdown button area
                                                if (mousePos.X >= _dropDownButton.Left)
                                                    hightlightClose = false;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }

                    if (_hotTrack)
                    {
                        if (_hotTrackPage >= _tabPages.Count)
                        {
                            _hotTrack = false;
                            _hotTrackPage = -1;
                        }
                    }


                    if (_hotTrack && !pageChanged && ((mousePage != _hotTrackPage) || (hightlightClose != _hotTrackPageHightlightClose)))
                    {
                        Graphics g = this.CreateGraphics();

                        // Clip the drawing to prevent drawing in unwanted areas
                        ClipDrawingTabs(g);

                        // Remove highlight of old page
                        if (_hotTrackPage != -1)
                        {
                            DrawTab(_tabPages[_hotTrackPage], g, false,false);
                            if ((!_tabPages[_hotTrackPage].Selected) && (mousePage == -1))
                            {
                                DrawTab(SelectedTab, g, false,false);
                            }
                        }

                        _hotTrackPage = mousePage;
                        _hotTrackPageHightlightClose = hightlightClose;

                        // Add highlight to new page
                        if (_hotTrackPage != -1)
                        {
                                DrawTab(_tabPages[_hotTrackPage], g, true,hightlightClose);




                            if (!_tabPages[_hotTrackPage].Selected)
                            {
                                DrawTab(this.SelectedTab, g, false,false);
                            }
                        }
                        // Must correctly release resource
                        g.Dispose();
                    }
                }
            }
            
            base.OnMouseMove(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _mouseOver = true;
            _overTimer.Stop();
            
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_hotTrack)
            {
                int newTrackPage = -1;

                if (newTrackPage != _hotTrackPage)
                {
                    Graphics g = this.CreateGraphics();

                    // Clip the drawing to prevent drawing in unwanted areas
                    ClipDrawingTabs(g);

                    // Remove highlight of old page
                    if (_hotTrackPage != -1)
                    {
                        DrawTab(_tabPages[_hotTrackPage], g, false,false);
                        if (!_tabPages[_hotTrackPage].Selected)
                        {
                            DrawTab(this.SelectedTab, g, false,false);
                        }
                    }
                    else
                        if (SelectedTab != null)
                            DrawTab(this.SelectedTab, g, false,false);

                    _hotTrackPage = newTrackPage;

                    // Must correctly release resource
                    g.Dispose();
                }
            }

            _overTimer.Start();

            base.OnMouseLeave(e);
        }		

		protected virtual void OnPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			// Are we using the default menu or a user defined value?
			if (_defaultFont)
			{
				DefineFont(SystemInformation.MenuFont);

                Recalculate();
                Invalidate();
			}
		}

		protected override void OnSystemColorsChanged(EventArgs e)
		{
			// If still using the Default color when we were created
			if (_defaultColor)
			{
				DefineBackColor(TabControlAdvanced.DefaultBackColor);

                Recalculate();
                Invalidate();
			}

			base.OnSystemColorsChanged(e);
		}

   }
   public class TabPageCollection : CollectionWithEvents
   {
      public TabPageAdvanced Add(TabPageAdvanced value)
      {
         // Use base class to process actual collection operation
         base.List.Add(value as object);

         return value;
      }

      public void AddRange(TabPageAdvanced[] values)
      {
         // Use existing method to add each array entry
         foreach (TabPageAdvanced page in values)
            Add(page);
      }


        public void Remove(TabPageAdvanced value)
      {
         // Use base class to process actual collection operation
         base.List.Remove(value as object);
      }

      public void Insert(int index, TabPageAdvanced value)
      {
         // Use base class to process actual collection operation
         base.List.Insert(index, value as object);
      }

      public bool Contains(TabPageAdvanced value)
      {
         // Use base class to process actual collection operation
         return base.List.Contains(value as object);
      }

      public TabPageAdvanced this[int index]
      {
         // Use base class to process actual collection operation
         get { return (base.List[index] as TabPageAdvanced); }
      }

      public TabPageAdvanced this[string title]
      {
         get
         {
            // Search for a Page with a matching title
            foreach (TabPageAdvanced page in base.List)
               if (page.Title == title)
                  return page;

            return null;
         }
      }

      public int IndexOf(TabPageAdvanced value)
      {
         // Find the 0 based index of the requested entry
         return base.List.IndexOf(value);
      }
   }
    [ToolboxBitmap(typeof(InertButton))]
    [DefaultProperty("PopupStyle")]
    public class InertButton : Control
    {
        // Instance fields
        protected int _borderWidth;
        protected bool _mouseOver;
        protected bool _mouseCapture;
        protected bool _popupStyle;
        protected int _imageIndexEnabled;
        protected int _imageIndexDisabled;
        protected ImageList _imageList;
        protected ImageAttributes _imageAttr;
        protected MouseButtons _mouseButton;
		
        public InertButton()
        {
            InternalConstruct(null, -1, -1, null);
        }

        public InertButton(ImageList imageList, int imageIndexEnabled)
        {
            InternalConstruct(imageList, imageIndexEnabled, -1, null);
        }

        public InertButton(ImageList imageList, int imageIndexEnabled, int imageIndexDisabled)
        {
            InternalConstruct(imageList, imageIndexEnabled, imageIndexDisabled, null);
        }

        public InertButton(ImageList imageList, int imageIndexEnabled, int imageIndexDisabled, ImageAttributes imageAttr)
        {
            InternalConstruct(imageList, imageIndexEnabled, imageIndexDisabled, imageAttr);
        }
		
        public void InternalConstruct(ImageList imageList, 
                                      int imageIndexEnabled, 
                                      int imageIndexDisabled, 
                                      ImageAttributes imageAttr)
        {
            // Remember parameters
            _imageList = imageList;
            _imageIndexEnabled = imageIndexEnabled;
            _imageIndexDisabled = imageIndexDisabled;
            _imageAttr = imageAttr;

            // Set initial state
            _borderWidth = 2;
            _mouseOver = false;
            _mouseCapture = false;
            _popupStyle = true;
            _mouseButton = MouseButtons.None;

            // Prevent drawing flicker by blitting from memory in WM_PAINT
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // Prevent base class from trying to generate double click events and
            // so testing clicks against the double click time and rectangle. Getting
            // rid of this allows the user to press then button very quickly.
            SetStyle(ControlStyles.StandardDoubleClick, false);

            // Should not be allowed to select this control
            SetStyle(ControlStyles.Selectable, false);
        }

        [Category("Appearance")]
        [DefaultValue(null)]
        public ImageList ImageList
        {
            get { return _imageList; }

            set
            {
                if (_imageList != value)
                {
                    _imageList = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(-1)]
        public int ImageIndexEnabled
        {
            get { return _imageIndexEnabled; }

            set
            {
                if (_imageIndexEnabled != value)
                {
                    _imageIndexEnabled = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(-1)]
        public int ImageIndexDisabled
        {
            get { return _imageIndexDisabled; }

            set
            {
                if (_imageIndexDisabled != value)
                {
                    _imageIndexDisabled = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(null)]
        public ImageAttributes ImageAttributes
        {
            get { return _imageAttr; }
			
            set
            {
                if (_imageAttr != value)
                {
                    _imageAttr = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(2)]
        public int BorderWidth
        {
            get { return _borderWidth; }

            set
            {
                if (_borderWidth != value)
                {
                    _borderWidth = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool PopupStyle
        {
            get { return _popupStyle; }

            set
            {
                if (_popupStyle != value)
                {
                    _popupStyle = value;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!_mouseCapture)
            {
                // Mouse is over the button and being pressed, so enter the button depressed 
                // state and also remember the original button that was pressed. As we only 
                // generate an event when the same button is released.
                _mouseOver = true;
                _mouseCapture = true;
                _mouseButton = e.Button;

                // Redraw to show button state
                Invalidate();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {

            // Are we waiting for this button to go up?
            if (e.Button == _mouseButton)
            {
                // Set state back to become normal
                _mouseOver = false;
                _mouseCapture = false;

                // Redraw to show button state
                Invalidate();
            }
            else
            {
                // We don't want to lose capture of mouse
                Capture = true;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Is mouse point inside our client rectangle
            bool over = this.ClientRectangle.Contains(new Point(e.X, e.Y));

            // If entering the button area or leaving the button area...
            if (over != _mouseOver)
            {
                // Update state
                _mouseOver = over;

                // Redraw to show button state
                Invalidate();
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            // Update state to reflect mouse over the button area
            _mouseOver = true;

            // Redraw to show button state
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            // Update state to reflect mouse not over the button area
            _mouseOver = false;

            // Redraw to show button state
            Invalidate();

            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Do we have an image list for use?
            if (_imageList != null)
            {
                // Is the button disabled?
                if (!this.Enabled)
                {
                    // Do we have an image for showing when disabled?
                    if (_imageIndexDisabled != -1)
                    {
                        // Any caller supplied attributes to modify drawing?
                        if (null == _imageAttr)
                        {
                            // No, so use the simple DrawImage method
                            e.Graphics.DrawImage(_imageList.Images[_imageIndexDisabled], new Point(1,1));
                        }
                        else
                        {
                            // Yes, need to use the more complex DrawImage method instead
                            Image image = _imageList.Images[_imageIndexDisabled];

                            // Three points provided are upper-left, upper-right and 
                            // lower-left of the destination parallelogram. 
                            Point[] pts = new Point[3];
                            pts[0].X = 1;
                            pts[0].Y = 1;
                            pts[1].X = pts[0].X + image.Width;
                            pts[1].Y = pts[0].Y;
                            pts[2].X = pts[0].X;
                            pts[2].Y = pts[1].Y + image.Height;

                            e.Graphics.DrawImage(_imageList.Images[_imageIndexDisabled], 
                                                 pts,
                                                 new Rectangle(0, 0, image.Width, image.Height),
                                                 GraphicsUnit.Pixel, _imageAttr);
                        }
                    }
                    else
                    {
                        // No disbled image, how about an enabled image we can draw grayed?
                        if (_imageIndexEnabled != -1)
                        {
                            // Yes, draw the enabled image but with color drained away
                            ControlPaint.DrawImageDisabled(e.Graphics, _imageList.Images[_imageIndexEnabled], 1, 1, this.BackColor);
                        }
                        else
                        {
                            // No images at all. Do nothing.
                        }
                    }
                }
                else
                {
                    // Button is enabled, any caller supplied attributes to modify drawing?
                    if (null == _imageAttr)
                    {
                        // No, so use the simple DrawImage method
                        e.Graphics.DrawImage(_imageList.Images[_imageIndexEnabled], 
                                             (_mouseOver &&  _mouseCapture ? new Point(2,2) : 
                                             new Point(1,1)));
                    }
                    else
                    {
                        // Yes, need to use the more complex DrawImage method instead
                        Image image = _imageList.Images[_imageIndexEnabled];

                        // Three points provided are upper-left, upper-right and 
                        // lower-left of the destination parallelogram. 
                        Point[] pts = new Point[3];
                        pts[0].X = (_mouseOver && _mouseCapture) ? 2 : 1;
                        pts[0].Y = (_mouseOver && _mouseCapture) ? 2 : 1;
                        pts[1].X = pts[0].X + image.Width ;
                        pts[1].Y = pts[0].Y;
                        pts[2].X = pts[0].X;
                        pts[2].Y = pts[1].Y + image.Height;

                        e.Graphics.DrawImage(_imageList.Images[_imageIndexEnabled], 
                                             pts,
                                             new Rectangle(0, 0, image.Width, image.Height),
                                             GraphicsUnit.Pixel, _imageAttr);
                    }

                    ButtonBorderStyle bs;

                    // Decide on the type of border to draw around image
                    if (_popupStyle)
                    {
                        if (_mouseOver && this.Enabled)
                            bs = (_mouseCapture ? ButtonBorderStyle.Inset : ButtonBorderStyle.Outset);
                        else
                            bs = ButtonBorderStyle.Solid;
                    }
                    else
                    {
                        if (this.Enabled)
                            bs = ((_mouseOver && _mouseCapture) ? ButtonBorderStyle.Inset : ButtonBorderStyle.Outset);
                        else
                            bs = ButtonBorderStyle.Solid;
                    }

                    ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, 
                                            this.BackColor, _borderWidth, bs,
                                            this.BackColor, _borderWidth, bs,
                                            this.BackColor, _borderWidth, bs,
                                            this.BackColor, _borderWidth, bs);
                }
            }

            base.OnPaint(e);
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
        }
    }
    // Declare the event signatures
    public delegate void CollectionClear();
    public delegate void CollectionChange(int index, object value);

    public class CollectionWithEvents : CollectionBase
    {
       // Collection change events
       public event CollectionClear Clearing;
       public event CollectionClear Cleared;
       public event CollectionChange Inserting;
       public event CollectionChange Inserted;
       public event CollectionChange Removing;
       public event CollectionChange Removed;

       // Overrides for generating events
       protected override void OnClear()
       {
          // Any attached event handlers?
          if (Clearing != null)
             Clearing();
       }
        public void Switch(int index1, int index2)
        {
            // Use existing method to add each array entry
            object old = List[index1];
            List[index1] = List[index2];
            List[index2] = old;
        }
        protected override void OnClearComplete()
       {
          // Any attached event handlers?
          if (Cleared != null)
             Cleared();
       }

       protected override void OnInsert(int index, object value)
       {
          // Any attached event handlers?
          if (Inserting != null)
             Inserting(index, value);
       }

       protected override void OnInsertComplete(int index, object value)
       {
          // Any attached event handlers?
          if (Inserted != null)
             Inserted(index, value);
       }

       protected override void OnRemove(int index, object value)
       {
          // Any attached event handlers?
          if (Removing != null)
             Removing(index, value);
       }

       protected override void OnRemoveComplete(int index, object value)
       {
          // Any attached event handlers?
          if (Removed != null)
             Removed(index, value);
       }

       protected int IndexOf(object value)
       {
          // Find the 0 based index of the requested entry
          return base.List.IndexOf(value);
       }
    }
    public class DrawHelper
    {
       public enum CommandState
       {
          Normal,
          HotTrack,
          Pushed
       }

       protected static IntPtr _halfToneBrush = IntPtr.Zero;

       public static void DrawReverseString(Graphics g,
                                            String drawText,
                                            Font drawFont,
                                            Rectangle drawRect,
                                            Brush drawBrush,
                                            StringFormat drawFormat)
       {
          GraphicsContainer container = g.BeginContainer();

          // The text will be rotated around the origin (0,0) and so needs moving
          // back into position by using a transform
          g.TranslateTransform(drawRect.Left * 2 + drawRect.Width,
                               drawRect.Top * 2 + drawRect.Height);

          // Rotate the text by 180 degress to reverse the direction 
          g.RotateTransform(180);

          // Draw the string as normal and let then transforms do the work
          g.DrawString(drawText, drawFont, drawBrush, drawRect, drawFormat);

          g.EndContainer(container);
       }

       public static void DrawPlainRaised(Graphics g,
                                          Rectangle boxRect,
                                          Color baseColor)
       {
          using (Pen lighlight = new Pen(ControlPaint.LightLight(baseColor)),
                    dark = new Pen(ControlPaint.DarkDark(baseColor)))
          {
             g.DrawLine(lighlight, boxRect.Left, boxRect.Bottom, boxRect.Left, boxRect.Top);
             g.DrawLine(lighlight, boxRect.Left, boxRect.Top, boxRect.Right, boxRect.Top);
             g.DrawLine(dark, boxRect.Right, boxRect.Top, boxRect.Right, boxRect.Bottom);
             g.DrawLine(dark, boxRect.Right, boxRect.Bottom, boxRect.Left, boxRect.Bottom);
          }
       }

       public static void DrawPlainSunken(Graphics g,
                                          Rectangle boxRect,
                                          Color baseColor)
       {
          using (Pen lighlight = new Pen(ControlPaint.LightLight(baseColor)),
                    dark = new Pen(ControlPaint.DarkDark(baseColor)))
          {
             g.DrawLine(dark, boxRect.Left, boxRect.Bottom, boxRect.Left, boxRect.Top);
             g.DrawLine(dark, boxRect.Left, boxRect.Top, boxRect.Right, boxRect.Top);
             g.DrawLine(lighlight, boxRect.Right, boxRect.Top, boxRect.Right, boxRect.Bottom);
             g.DrawLine(lighlight, boxRect.Right, boxRect.Bottom, boxRect.Left, boxRect.Bottom);
          }
       }

       public static void DrawPlainRaisedBorder(Graphics g,
                                                Rectangle rect,
                                                Color lightLight,
                                                Color baseColor,
                                                Color dark,
                                                Color darkDark)
       {
          if ((rect.Width > 2) && (rect.Height > 2))
          {
             using (Pen ll = new Pen(lightLight),
                       b = new Pen(baseColor),
                       d = new Pen(dark),
                       dd = new Pen(darkDark))
             {
                int left = rect.Left;
                int top = rect.Top;
                int right = rect.Right;
                int bottom = rect.Bottom;

                // Draw the top border
                g.DrawLine(b, right - 1, top, left, top);
                g.DrawLine(ll, right - 2, top + 1, left + 1, top + 1);
                g.DrawLine(b, right - 3, top + 2, left + 2, top + 2);

                // Draw the left border
                g.DrawLine(b, left, top, left, bottom - 1);
                g.DrawLine(ll, left + 1, top + 1, left + 1, bottom - 2);
                g.DrawLine(b, left + 2, top + 2, left + 2, bottom - 3);

                // Draw the right
                g.DrawLine(dd, right - 1, top + 1, right - 1, bottom - 1);
                g.DrawLine(d, right - 2, top + 2, right - 2, bottom - 2);
                g.DrawLine(b, right - 3, top + 3, right - 3, bottom - 3);

                // Draw the bottom
                g.DrawLine(dd, right - 1, bottom - 1, left, bottom - 1);
                g.DrawLine(d, right - 2, bottom - 2, left + 1, bottom - 2);
                g.DrawLine(b, right - 3, bottom - 3, left + 2, bottom - 3);
             }
          }
       }

       public static void DrawPlainRaisedBorderTopOrBottom(Graphics g,
                                                           Rectangle rect,
                                                           Color lightLight,
                                                           Color baseColor,
                                                           Color dark,
                                                           Color darkDark,
                                                           bool drawTop)
       {
          if ((rect.Width > 2) && (rect.Height > 2))
          {
             using (Pen ll = new Pen(lightLight),
                       b = new Pen(baseColor),
                       d = new Pen(dark),
                       dd = new Pen(darkDark))
             {
                int left = rect.Left;
                int top = rect.Top;
                int right = rect.Right;
                int bottom = rect.Bottom;

                if (drawTop)
                {
                   // Draw the top border
                   g.DrawLine(b, right - 1, top, left, top);
                   g.DrawLine(ll, right - 1, top + 1, left, top + 1);
                   g.DrawLine(b, right - 1, top + 2, left, top + 2);
                }
                else
                {
                   // Draw the bottom
                   g.DrawLine(dd, right - 1, bottom - 1, left, bottom - 1);
                   g.DrawLine(d, right - 1, bottom - 2, left, bottom - 2);
                   g.DrawLine(b, right - 1, bottom - 3, left, bottom - 3);
                }
             }
          }
       }

       public static void DrawPlainSunkenBorder(Graphics g,
                                                Rectangle rect,
                                                Color lightLight,
                                                Color baseColor,
                                                Color dark,
                                                Color darkDark)
       {
          if ((rect.Width > 2) && (rect.Height > 2))
          {
             using (Pen ll = new Pen(lightLight),
                       b = new Pen(baseColor),
                       d = new Pen(dark),
                       dd = new Pen(darkDark))
             {
                int left = rect.Left;
                int top = rect.Top;
                int right = rect.Right;
                int bottom = rect.Bottom;

                // Draw the top border
                g.DrawLine(d, right - 1, top, left, top);
                g.DrawLine(dd, right - 2, top + 1, left + 1, top + 1);
                g.DrawLine(b, right - 3, top + 2, left + 2, top + 2);

                // Draw the left border
                g.DrawLine(d, left, top, left, bottom - 1);
                g.DrawLine(dd, left + 1, top + 1, left + 1, bottom - 2);
                g.DrawLine(b, left + 2, top + 2, left + 2, bottom - 3);

                // Draw the right
                g.DrawLine(ll, right - 1, top + 1, right - 1, bottom - 1);
                g.DrawLine(b, right - 2, top + 2, right - 2, bottom - 2);
                g.DrawLine(b, right - 3, top + 3, right - 3, bottom - 3);

                // Draw the bottom
                g.DrawLine(ll, right - 1, bottom - 1, left, bottom - 1);
                g.DrawLine(b, right - 2, bottom - 2, left + 1, bottom - 2);
                g.DrawLine(b, right - 3, bottom - 3, left + 2, bottom - 3);
             }
          }
       }

       public static void DrawPlainSunkenBorderTopOrBottom(Graphics g,
                                                           Rectangle rect,
                                                           Color lightLight,
                                                           Color baseColor,
                                                           Color dark,
                                                           Color darkDark,
                                                           bool drawTop)
       {
          if ((rect.Width > 2) && (rect.Height > 2))
          {
             using (Pen ll = new Pen(lightLight),
                       b = new Pen(baseColor),
                       d = new Pen(dark),
                       dd = new Pen(darkDark))
             {
                int left = rect.Left;
                int top = rect.Top;
                int right = rect.Right;
                int bottom = rect.Bottom;

                if (drawTop)
                {
                   // Draw the top border
                   g.DrawLine(d, right - 1, top, left, top);
                   g.DrawLine(dd, right - 1, top + 1, left, top + 1);
                   g.DrawLine(b, right - 1, top + 2, left, top + 2);
                }
                else
                {
                   // Draw the bottom
                   g.DrawLine(ll, right - 1, bottom - 1, left, bottom - 1);
                   g.DrawLine(b, right - 1, bottom - 2, left, bottom - 2);
                   g.DrawLine(b, right - 1, bottom - 3, left, bottom - 3);
                }
             }
          }
       }

       public static void DrawButtonCommand(Graphics g,
                                            VisualStyle style,
                                            Direction direction,
                                            Rectangle drawRect,
                                            CommandState state,
                                            Color baseColor,
                                            Color trackLight,
                                            Color trackBorder)
       {
          Rectangle rect = new Rectangle(drawRect.Left, drawRect.Top, drawRect.Width - 1, drawRect.Height - 1);

          // Draw background according to style
          switch (style)
          {
             case VisualStyle.Plain:
                // Draw background with back color
                using (SolidBrush backBrush = new SolidBrush(baseColor))
                   g.FillRectangle(backBrush, rect);

                // Modify according to state
                switch (state)
                {
                   case CommandState.HotTrack:
                      DrawPlainRaised(g, rect, baseColor);
                      break;
                   case CommandState.Pushed:
                      DrawPlainSunken(g, rect, baseColor);
                      break;
                }
                break;
             case VisualStyle.IDE:
                // Draw according to state
                switch (state)
                {
                   case CommandState.Normal:
                      // Draw background with back color
                      using (SolidBrush backBrush = new SolidBrush(baseColor))
                         g.FillRectangle(backBrush, rect);
                      break;
                   case CommandState.HotTrack:
                      g.FillRectangle(Brushes.White, rect);

                      using (SolidBrush trackBrush = new SolidBrush(trackLight))
                         g.FillRectangle(trackBrush, rect);

                      using (Pen trackPen = new Pen(trackBorder))
                         g.DrawRectangle(trackPen, rect);
                      break;
                   case CommandState.Pushed:
                      //TODO: draw in a darker background color
                      break;
                }
                break;
                case VisualStyle.Chrome:
                    // Draw according to state
                    switch (state)
                    {
                        case CommandState.Normal:
                            // Draw background with back color
                            using (SolidBrush backBrush = new SolidBrush(baseColor))
                                g.FillRectangle(backBrush, rect);
                            break;
                        case CommandState.HotTrack:
                            g.FillRectangle(Brushes.White, rect);

                            using (SolidBrush trackBrush = new SolidBrush(trackLight))
                                g.FillRectangle(trackBrush, rect);

                            using (Pen trackPen = new Pen(trackBorder))
                                g.DrawRectangle(trackPen, rect);
                            break;
                        case CommandState.Pushed:
                            //TODO: draw in a darker background color
                            break;
                    }
                    break;
            }
        }

       public static void DrawSeparatorCommand(Graphics g,
                                               VisualStyle style,
                                               Direction direction,
                                               Rectangle drawRect,
                                               Color baseColor)
       {
            switch (style)
            {
                case VisualStyle.IDE:
                    // Draw a single separating line
                    using (Pen dPen = new Pen(ControlPaint.Dark(baseColor)))
                    {
                        if (direction == Direction.Horizontal)
                            g.DrawLine(dPen, drawRect.Left, drawRect.Top,
                                             drawRect.Left, drawRect.Bottom - 1);
                        else
                            g.DrawLine(dPen, drawRect.Left, drawRect.Top,
                                             drawRect.Right - 1, drawRect.Top);
                    }
                    break;
                case VisualStyle.Plain:
                    // Draw a dark/light combination of lines to give an indent
                    using (Pen lPen = new Pen(ControlPaint.Dark(baseColor)),
                              llPen = new Pen(ControlPaint.LightLight(baseColor)))
                    {
                        if (direction == Direction.Horizontal)
                        {
                            g.DrawLine(lPen, drawRect.Left, drawRect.Top, drawRect.Left, drawRect.Bottom - 1);
                            g.DrawLine(llPen, drawRect.Left + 1, drawRect.Top, drawRect.Left + 1, drawRect.Bottom - 1);
                        }
                        else
                        {
                            g.DrawLine(lPen, drawRect.Left, drawRect.Top, drawRect.Right - 1, drawRect.Top);
                            g.DrawLine(llPen, drawRect.Left, drawRect.Top + 1, drawRect.Right - 1, drawRect.Top + 1);
                        }
                    }
                    break;
                case VisualStyle.Chrome:
                    // Draw a single separating line
                    using (Pen dPen = new Pen(ControlPaint.Dark(baseColor)))
                    {
                        if (direction == Direction.Horizontal)
                            g.DrawLine(dPen, drawRect.Left, drawRect.Top,
                                             drawRect.Left, drawRect.Bottom - 1);
                        else
                            g.DrawLine(dPen, drawRect.Left, drawRect.Top,
                                             drawRect.Right - 1, drawRect.Top);
                    }
                    break;
            }
          // Drawing depends on the visual style required
/*          if (style == VisualStyle.IDE)
          {
             // Draw a single separating line
             using (Pen dPen = new Pen(ControlPaint.Dark(baseColor)))
             {
                if (direction == Direction.Horizontal)
                   g.DrawLine(dPen, drawRect.Left, drawRect.Top,
                                    drawRect.Left, drawRect.Bottom - 1);
                else
                   g.DrawLine(dPen, drawRect.Left, drawRect.Top,
                                    drawRect.Right - 1, drawRect.Top);
             }
          }
          else
          {
             // Draw a dark/light combination of lines to give an indent
             using (Pen lPen = new Pen(ControlPaint.Dark(baseColor)),
                       llPen = new Pen(ControlPaint.LightLight(baseColor)))
             {
                if (direction == Direction.Horizontal)
                {
                   g.DrawLine(lPen, drawRect.Left, drawRect.Top, drawRect.Left, drawRect.Bottom - 1);
                   g.DrawLine(llPen, drawRect.Left + 1, drawRect.Top, drawRect.Left + 1, drawRect.Bottom - 1);
                }
                else
                {
                   g.DrawLine(lPen, drawRect.Left, drawRect.Top, drawRect.Right - 1, drawRect.Top);
                   g.DrawLine(llPen, drawRect.Left, drawRect.Top + 1, drawRect.Right - 1, drawRect.Top + 1);
                }
             }
          }*/
       }

    }
    public class ControlHelper
    {
       public static void RemoveAll(Control control)
       {
          if ((control != null) && (control.Controls.Count > 0))
          {
             Button tempButton = null;
             Form parentForm = control.FindForm();

             if (parentForm != null)
             {
                // Create a hidden, temporary button
                tempButton = new Button();
                tempButton.Visible = false;

                // Add this temporary button to the parent form
                parentForm.Controls.Add(tempButton);

                // Must ensure that temp button is the active one
                parentForm.ActiveControl = tempButton;
             }

             // Remove all entries from target
             control.Controls.Clear();

             if (parentForm != null)
             {
                // Remove the temporary button
                tempButton.Dispose();
                parentForm.Controls.Remove(tempButton);
             }
          }
       }

       public static void Remove(Control.ControlCollection coll, Control item)
       {
          if ((coll != null) && (item != null))
          {
             Button tempButton = null;
             Form parentForm = item.FindForm();

             if (parentForm != null)
             {
                // Create a hidden, temporary button
                tempButton = new Button();
                tempButton.Visible = false;

                // Add this temporary button to the parent form
                parentForm.Controls.Add(tempButton);

                // Must ensure that temp button is the active one
                parentForm.ActiveControl = tempButton;
             }

             // Remove our target control
             coll.Remove(item);

             if (parentForm != null)
             {
                // Remove the temporary button
                tempButton.Dispose();
                parentForm.Controls.Remove(tempButton);
             }
          }
       }

       public static void RemoveAt(Control.ControlCollection coll, int index)
       {
          if (coll != null)
          {
             if ((index >= 0) && (index < coll.Count))
             {
                Remove(coll, coll[index]);
             }
          }
       }

       public static void RemoveForm(Control source, Form form)
       {
          ContainerControl container = source.FindForm() as ContainerControl;

          if (container == null)
             container = source as ContainerControl;

          Button tempButton = new Button();
          tempButton.Visible = false;

          // Add this temporary button to the parent form
          container.Controls.Add(tempButton);

          // Must ensure that temp button is the active one
          container.ActiveControl = tempButton;

          // Remove Form parent
          form.Parent = null;

          // Remove the temporary button
          tempButton.Dispose();
          container.Controls.Remove(tempButton);
       }
    }
    public class ColorHelper
    {
       public static Color TabBackgroundFromBaseColor(Color backColor)
       {
          Color backIDE;

          // Check for the 'Classic' control color
          if ((backColor.R == 212) &&
              (backColor.G == 208) &&
              (backColor.B == 200))
          {
             // Use the exact background for this color
             backIDE = Color.FromArgb(247, 243, 233);
          }
          else
          {
             // Check for the 'XP' control color
             if ((backColor.R == 236) &&
                 (backColor.G == 233) &&
                 (backColor.B == 216))
             {
                // Use the exact background for this color
                backIDE = Color.FromArgb(255, 251, 233);
             }
             else
             {
                // Calculate the IDE background color as only half as dark as the control color
                int red = 255 - ((255 - backColor.R) / 2);
                int green = 255 - ((255 - backColor.G) / 2);
                int blue = 255 - ((255 - backColor.B) / 2);
                backIDE = Color.FromArgb(red, green, blue);
             }
          }

          return backIDE;
       }
    }
 }


