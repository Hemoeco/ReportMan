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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Designer
{
    /// <summary>
    /// Utility for resizing any item
    /// </summary>
    public class RuntimeResize:IDisposable
    {
        private RuntimeSquare TopBar;
        private RuntimeSquare LeftBar;
        private RuntimeSquare RightBar;
        private RuntimeSquare BottomBar;
        private Control FParent;
        private int FLeft,FWidth,FHeight,FTop;
        private RuntimeCapture Q1, Q2, Q3, Q4;
        private ResizeDif DifEvent;

        public ResizeAction Action;
        public ResizeEventGetBounds OnGetBounds;
        public ResizeEventSetBounds OnSetBounds;
        public ResizeEventSetBounds OnNewBounds;
        public bool GridEnabled;
        public int GridWidth;
        public int GridHeight;
        public int GridOffsetX;
        public int GridOffsetY;
      public double GridScale;
      public RuntimeResize()
        {
            TopBar = new RuntimeSquare();
            LeftBar = new RuntimeSquare();
            BottomBar = new RuntimeSquare();
            RightBar = new RuntimeSquare();
            Action = ResizeAction.Resize;
            DifEvent = new ResizeDif(DifProc);
            Q1 = new RuntimeCapture();
            Q1.Cursor = Cursors.SizeNWSE;
            Q2 = new RuntimeCapture();
            Q2.Cursor = Cursors.SizeNESW;
            Q3 = new RuntimeCapture();
            Q3.Cursor = Cursors.SizeNWSE;
            Q4 = new RuntimeCapture();
            Q4.Cursor = Cursors.SizeNESW;
            Q1.OnDif = DifEvent;
            Q2.OnDif = DifEvent;
            Q3.OnDif = DifEvent;
            Q4.OnDif = DifEvent;
            GridWidth = 100;
            GridHeight = 100;
            GridEnabled = false;
            GridScale = 1;
        }
        public void Dispose()
        {
            TopBar.Dispose();
            LeftBar.Dispose();
            RightBar.Dispose();
            BottomBar.Dispose();
            Q1.Dispose();
            Q2.Dispose();
            Q3.Dispose();
            Q4.Dispose();
        }
        public void SetBounds(int Left, int Top, int Width, int Height,bool finish)
        {
            if (finish)
            {
                FLeft = Left;
                FTop = Top;
                FWidth = Width;
                FHeight = Height;
                if (OnSetBounds != null)
                    OnSetBounds(new Rectangle(Left, Top, Width, Height));
            }
            TopBar.SetBounds(Left, Top, Width, 1);
            LeftBar.SetBounds(Left, Top, 1, Height);
            BottomBar.SetBounds(Left, Top+Height-1, Width, 1);
            RightBar.SetBounds(Left+Width-1, Top, 1, Height);
            int squarewidth = 8;          
            if (Action == ResizeAction.Resize)
            {
                Q2.Visible = true;
                Q3.Visible = true;
                Q4.Visible = true;
            }
            else
            {
                Q2.Visible = false;
                Q3.Visible = false;
                Q4.Visible = false;
                squarewidth = 16;
            }
            int squaremid=squarewidth / 2;
            Q1.SetBounds(Left - squaremid, Top - squaremid, squarewidth, squarewidth);
            Q2.SetBounds(Left + Width - squaremid, Top - squaremid, squarewidth, squarewidth);
            Q3.SetBounds(Left + Width - squaremid, Top + Height - squaremid, squarewidth, squarewidth);
            Q4.SetBounds(Left - squaremid, Top +Height - squaremid, squarewidth, squarewidth);
        }
        public void UpdatePos()
        {
            if (OnGetBounds==null)
                return;
        }
        public void BringToFront()
        {
            TopBar.BringToFront();
            LeftBar.BringToFront();
            RightBar.BringToFront();
            BottomBar.BringToFront();
            Q1.BringToFront();
            Q2.BringToFront();
            Q3.BringToFront();
            Q4.BringToFront();
        }
        public Control Parent
        {
            get { return FParent; }
            set
            {
                FParent = value;
                TopBar.Parent = FParent;
                LeftBar.Parent = FParent;
                RightBar.Parent = FParent;
                BottomBar.Parent = FParent;
                Q1.Parent = FParent;
                Q2.Parent = FParent;
                Q3.Parent = FParent;
                Q4.Parent = FParent;
                SetBounds(FLeft, FTop, FWidth, FHeight,true);
            }

        }
        private void DifProc(RuntimeCapture sender, int difx, int dify, bool finish,bool start,bool mouseup)
        {
            if (start)
            {
                return;
            }
          int newleft = 0;
          int newtop = 0;
          int newwidth = 0;
          int newheight = 0;
          bool dosetbounds = true;
          bool alignleft = false;
          bool aligntop = false;
          bool alignbottom = false;
          bool alignright = false;


            if (sender == Q1)
            {
              newleft = FLeft + difx;
              newwidth = FWidth - difx;
              newtop = FTop + dify;
              newheight = FHeight - dify;
              alignleft = true;
              aligntop = true;
            }
            if (sender == Q2)
            {
              newleft = FLeft;
              newtop = FTop + dify;
              newwidth = FWidth + difx;
              newheight = FHeight - dify;
              aligntop = true;
              alignright = true;
            }
            if (sender == Q3)
            {
              newleft = FLeft;
              newtop = FTop;
              newwidth = FWidth + difx;
              newheight = FHeight + dify;
              alignright = true;
              alignbottom = true;
            }
            if (sender == Q4)
            {
              newleft = FLeft + difx;
              newtop = FTop;
              newwidth = FWidth - difx;
              newheight = FHeight + dify;
              alignleft = true;
              alignbottom = true;
            }
            if (newwidth < 0)
            {
                dosetbounds = false;
              newleft = newleft - newwidth;
              newwidth = -newwidth;
            }
            if (newheight < 0)
            {
                dosetbounds = false;
              newtop = newtop + newheight;
              newheight = -newheight;
            }
            if (dosetbounds)
            {
                // Align to grid
              if (GridEnabled)
              {
                if (alignleft)
                {
                  newleft = newleft - GridOffsetX;
                  int nleft = TwipsGraphics.AlignToGridPixels(newleft, GridWidth, GridHeight, GridScale);
                  newwidth = newwidth + newleft - nleft;
                  newleft = nleft;
                  newleft = newleft + GridOffsetX;
                }
                if (aligntop)
                {
                  newtop = newtop - GridOffsetY;
                  int ntop = TwipsGraphics.AlignToGridPixels(newtop, GridWidth, GridHeight, GridScale);
                  newheight = newheight + newtop - ntop;
                  newtop = ntop;
                  newtop = newtop + GridOffsetY;
                }
                if (alignright)
                {
                  newwidth = newwidth - GridOffsetX + newleft;
                  newwidth = TwipsGraphics.AlignToGridPixels(newwidth, GridWidth, GridHeight, GridScale);
                  newwidth = newwidth + GridOffsetX - newleft;
                }
                if (alignbottom)
                {
                  newheight = newheight - GridOffsetY + newtop;
                  newheight = TwipsGraphics.AlignToGridPixels(newheight, GridWidth, GridHeight, GridScale);
                  newheight = newheight + GridOffsetY - newtop;
                }
              }
                SetBounds(newleft, newtop, newwidth, newheight, finish);
                if (mouseup)
                {
                    if (OnNewBounds != null)
                        OnNewBounds(new Rectangle(newleft, newtop, newwidth, newheight));
                }
            }
          }
    }
    class RuntimeSquare : Control
    {
        private Pen MyPen;
        private Brush MyBrush;
        public RuntimeSquare():base()
        {
            BackColor = SystemColors.WindowText;
            ForeColor = SystemColors.Window;
            SetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint,true);            
            MyPen = new Pen(SystemColors.WindowText);
            MyBrush = Brushes.White;
        }
        protected override void Dispose(bool disposing)
        {
            MyPen.Dispose();
            MyBrush.Dispose();
            base.Dispose(disposing);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if ((Width>2 ) && (Height>2))
                e.Graphics.FillRectangle(MyBrush, new Rectangle(0, 0, Width-1, Height-1));
            e.Graphics.DrawRectangle(MyPen, new Rectangle(0, 0, Width-1, Height-1));
            base.OnPaint(e);
        }
    }
    class RuntimeCapture : RuntimeSquare
    {
        private int originalx, originaly;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            originalx = e.X;
            originaly = e.Y;
            Capture = true;
            if (OnDif != null)
                OnDif(this, e.X, e.Y, false, true,false);
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!Capture)
                return;
            int difx = e.X - originalx;
            int dify = e.Y - originaly;
            if (OnDif!=null)
                OnDif(this,difx,dify,true,false,false);
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!Capture)
                return;
            int difx = e.X - originalx;
            int dify = e.Y - originaly;
            if (OnDif!=null)
                OnDif(this,difx,dify,true,false,true);
            base.OnMouseUp(e);
            Capture = false;
        }
        public ResizeDif OnDif;
    }
    /// <summary>
    /// Action to do
    /// </summary>
    public enum ResizeAction { Resize, Move };
    /// <summary>
    /// Event to retrieve original object bounds
    /// </summary>
    /// <returns></returns>
    public delegate Rectangle ResizeEventGetBounds();
    /// <summary>
    /// Event to set new bounds, can return validated new bounds
    /// </summary>
    /// <param name="newvalue"></param>
    /// <returns></returns>
    public delegate Rectangle ResizeEventSetBounds(Rectangle newvalue);

    delegate void ResizeDif(RuntimeCapture sender, int difx, int dify, bool finish,bool start,bool mouseup);
}
