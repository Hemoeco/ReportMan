using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Reportman.Drawing.Forms
{
    public partial class SignatureControl : UserControl
    {
        public enum SignatureStyle { Curve, Line };
        Bitmap nbitmap;
        System.Timers.Timer ntimer;
        System.Windows.Forms.Timer timerprin;
        Thread threadcapture;
        System.Threading.Timer ntimer2;
        int pathcount;
        bool capturedmouse;
        SortedList<int, Point> LinePath;
        List<SortedList<int,Point>> Segments;
        SortedList<int, Point> Original;
        Brush brushsign;
        Pen pensign;
        object flag = 2;
        public Bitmap Signature
        {
            get
            {
                return nbitmap;
            }
        }
        bool fempty;
        public bool Empty
        {
            get
            {
                return fempty;
            }
        }
        public SignatureStyle Style;
        public SignatureControl()
        {
            InitializeComponent();

            Segments = new List<SortedList<int, Point>>();
            Style = SignatureStyle.Curve;
            LinePath = new SortedList<int, Point>();
            Original = new SortedList<int, Point>();
            ntimer = new System.Timers.Timer(10);
            timerprin = new System.Windows.Forms.Timer();
            timerprin.Enabled = false;
            timerprin.Interval = 50;
            ntimer.Elapsed += new System.Timers.ElapsedEventHandler(TimerTick);
            timerprin.Tick += new EventHandler(TimerPrin);
            nbitmap = new Bitmap(this.Width, this.Height);
            picture.Image = nbitmap;
            brushsign = new SolidBrush(Color.White);
            pensign = new Pen(Color.Black, 2.0f);
            pensign.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            fempty = true;
        }
        public void Clear()
        {
            Segments.Clear();
            pathcount = 0;
            fempty = true;
            ClearPicture();
        }
        private void ClearPicture()
        {
            if (nbitmap != null)
            {
                picture.Image = null;
                nbitmap.Dispose();
                nbitmap = new Bitmap(this.Width, this.Height);
                using (Graphics gr = Graphics.FromImage(nbitmap))
                {
                    gr.FillRectangle(brushsign, new Rectangle(0, 0, nbitmap.Width+1, nbitmap.Height+1));
                }
                picture.Image = nbitmap;
            }
        }

        private void TimerPrin(object sender, EventArgs args)
        {
            SortedList<int, Point> nlist = new SortedList<int, Point>();
            Monitor.Enter(flag);
            try
            {
                if (LinePath.Count > 2)
                {
                    for (int i = 0; i < LinePath.Count; i++)
                    {
                        nlist.Add(LinePath.Keys[i], LinePath.Values[i]);
                        if (Original.IndexOfKey(LinePath.Keys[i])<0)
                            Original.Add(LinePath.Keys[i], LinePath.Values[i]);
                    }
                    LinePath.Clear();
                    LinePath.Add(nlist.Keys[nlist.Count - 1], nlist.Values[nlist.Count - 1]);
                }
            }
            finally
            {
                Monitor.Exit(flag);
            }
            if (nlist.Count > 0)
                DrawList(nlist);
        }
        public void TimerCall(object state)
        {
            Point npos = Cursor.Position;

            bool doadd = false;
            Monitor.Enter(flag);
            try
            {
                if (LinePath.Count > 1)
                {
                    Point oldpos = LinePath[pathcount - 1];
                    if ((oldpos.X != npos.X) || (oldpos.Y != npos.Y))
                        doadd = true;
                }
                else
                    doadd = true;
                if (doadd)
                {
                    LinePath.Add(pathcount, npos);
                    pathcount++;
                }
            }
            finally
            {
                Monitor.Exit(flag);
            }
        }

        private void TimerTick(object sender, System.Timers.ElapsedEventArgs args)
        {
            TimerCall(this);
        }
        private void SignatureControl_Resize(object sender, EventArgs e)
        {
            if (nbitmap == null)
                return;
            if ((this.Height != nbitmap.Height) || (this.Width != nbitmap.Width))
            {
                Clear();
            }
        }
        private void StartThreadCapture()
        {
            capturedmouse = true;
            timerprin.Enabled = true;
            threadcapture = new Thread(BeginCapture);
            threadcapture.Start();
        }
        private void DrawList(SortedList<int, Point> nlist)
        {


            // Draw line path into the bitmap
            using (Graphics gr = Graphics.FromImage(nbitmap))
            {
                if (nlist.Count > 1)
                {
                    //gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    fempty = false;
                    int i = 0;
                    switch (Style)
                    {
                        case SignatureStyle.Curve:
                            Point[] npoints = new Point[nlist.Count];
                            foreach (Point npoint in nlist.Values)
                            {
                                Point spoint = this.PointToClient(npoint);
                                npoints[i] = spoint;
                                i++;
                            }
                            gr.DrawCurve(pensign, npoints);
                            break;
                        case SignatureStyle.Line:
                            for (i = 0; i < nlist.Count - 1; i++)
                            {
                                Point spoint = this.PointToClient(nlist.Values[i]);
                                Point dpoint = this.PointToClient(nlist.Values[i + 1]);

                                gr.DrawLine(pensign, spoint, dpoint);
                            }
                            break;
                    }
                }
                picture.Image = null;
                picture.Image = nbitmap;
            }
        }
        private void EndThreadCapture()
        {
            ntimer.Enabled = false;
            if (ntimer2 != null)
            {
                ntimer2.Dispose();
                ntimer2 = null;
            }
            capturedmouse = false;
            if (threadcapture != null)
            {
                if (!threadcapture.Join(2000))
                {
                    threadcapture.Abort();
                }
                threadcapture = null;
                for (int i = 0; i < LinePath.Count; i++)
                {
                    if (Original.IndexOfKey(LinePath.Keys[i]) < 0)
                        Original.Add(LinePath.Keys[i], LinePath.Values[i]);

                }
                LinePath.Clear();
                //DrawList(LinePath);
                ClearPicture();
                // Redraw all so the spline curve is smoother
                Segments.Add(Original);
                Original = new SortedList<int, Point>();
                foreach (SortedList<int,Point> nlist in Segments)
                    DrawList(nlist);
            }
        }
        private void BeginCapture()
        {
            ntimer.Enabled = false;
            if (ntimer2 != null)
            {
                ntimer2.Dispose();
                ntimer2 = null;
            }
            LinePath.Clear();
            //ntimer.Enabled = true;
            ntimer2 = new System.Threading.Timer(TimerCall, this, 0, 10);
            while (capturedmouse)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
        private void picture_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                EndThreadCapture();
                StartThreadCapture();
            }
            else
            {
                if (e.Button == MouseButtons.Right)
                    Clear();
            }
        }

        private void picture_MouseUp(object sender, MouseEventArgs e)
        {
            EndThreadCapture();
        }
    }
}
