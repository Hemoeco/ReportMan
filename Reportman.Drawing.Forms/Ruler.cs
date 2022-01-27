using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using Reportman.Drawing;
using System.Threading;

namespace Reportman.Drawing.Forms
{
    public enum RulerStyle {Horizontal,Vertical};
    public enum RulerBorder {Single,None};
    public class Ruler : Control
    {
        public Ruler()
        {
            InitializeComponent();
            DoInit();
            bitmapvalid = false;
            FBackgroundColor = Color.White;
            FForegroundColor = Color.Black;
            FRulerScale = 1.0;
            FBorder = RulerBorder.Single;
            FMetrics = Units.Cms;
            System.Drawing.Graphics gr = this.CreateGraphics();
            try
            {
                dpix = (int)gr.DpiX;
                dpiy = (int)gr.DpiY;
            }
            finally
            {
                gr.Dispose();
            }
        }
        private RulerStyle FStyle;
        private RulerBorder FBorder;
        private Units FMetrics;
        private Color FBackgroundColor;
        private Color FForegroundColor;
        private double FRulerScale;
        private bool bitmapvalid;
        private SolidBrush brush;
        private SolidBrush brushpen;
        private Pen pen;
        private Bitmap bitmap;
        private int dpix;
        private int dpiy;

        [DefaultValue(RulerStyle.Horizontal)]
        public RulerStyle Style
        {
            get
            {
                return FStyle;
            }
            set
            {
                if (FStyle != value)
                {
                    FStyle = value;
                    bitmapvalid = false;
                    Invalidate();
                }
            }
        }
        [DefaultValue(RulerBorder.Single)]
        public RulerBorder Border
        {
            get
            {
                return FBorder;
            }
            set
            {
                if (FBorder != value)
                {
                    FBorder = value;
                    bitmapvalid = false;
                    Invalidate();
                }
            }
        }
        [DefaultValue(typeof(double),"1.0")]
        public double RulerScale
        {
            get
            {
                return FRulerScale;
            }
            set
            {
                if (FRulerScale != value)
                {
                    FRulerScale = value;
                    if (FRulerScale < 0.01)
                        FRulerScale = 0.01;
                    bitmapvalid = false;
                    Invalidate();
                }
            }
        }
        [DefaultValue(Units.Cms)]
        public Units Metrics
        {
            get
            {
                return FMetrics;
            }
            set
            {
                if (FMetrics != value)
                {
                    FMetrics = value;
                    bitmapvalid = false;
                    Invalidate();
                }
            }
        }
        [DefaultValue(typeof(Color),"White")]
        public Color BackgroundColor
        {
            get
            {
                return FBackgroundColor;
            }
            set
            {
                if (FBackgroundColor != value)
                {
                    FBackgroundColor = value;
                    bitmapvalid = false;
                    Invalidate();
                }
            }
        }
        [DefaultValue(typeof(Color),"Black")]
        public Color ForeGroundColor
        {
            get
            {
                return FForegroundColor;
            }
            set
            {
                if (FForegroundColor != value)
                {
                    FForegroundColor = value;
                    bitmapvalid = false;
                    Invalidate();
                }
            }
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (!bitmapvalid)
                RedrawBitmap();
            base.OnPaint(pe);
            pe.Graphics.DrawImage(bitmap, new Point(0, 0));
        }
        private void RedrawBitmap()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
            if (brush != null)
            {
                brush.Dispose();
                brush = null;
            }
            if (brushpen != null)
            {
                brushpen.Dispose();
                brushpen = null;
            }
            if (pen != null)
            {
                pen.Dispose();
                pen = null;
            }
            bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            brush = new SolidBrush(FBackgroundColor);
            pen = new Pen(FForegroundColor);
            brushpen = new SolidBrush(FForegroundColor);

            using (System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bitmap))
            {
                gr.FillRectangle(brush,new Rectangle(0,0,bitmap.Width,bitmap.Height));
                if (Border == RulerBorder.Single)
                {
                    gr.DrawRectangle(pen,new Rectangle(0,0,bitmap.Width-1,bitmap.Height-1));
                }
                PaintRuler(gr, FMetrics, FRulerScale, FStyle, brushpen,pen, Font, 
                    new Rectangle(0, 0, bitmap.Width - 1, bitmap.Height - 1),dpix,dpiy);
            }
            bitmapvalid = true;
        }
        private static int LogicalToDevice(int origin,int destination,int value)
        {
            int aresult=value;
            if (origin==0)
                return aresult;
            aresult=(int)Math.Round((double)value*((double)destination/origin));
            return aresult;
        }

        public static void PaintRuler(Graphics gr, Units Metrics, double Scale, RulerStyle Style,
            Brush brush,Pen pen, Font font, Rectangle rect, int dpix, int dpiy)
        {
            double onecent,onethousand;
            int h1,h2,h3;
            int windowheight,windowwidth;
            if (Metrics ==Units.Cms)
            {
                onecent = 100.0 / System.Convert.ToDouble(Twips.CMS_PER_INCH);
                onethousand=onecent*10;
            }
            else
            {
                onecent=100;
                onethousand=1000;
            }
            int textpos;
            dpix = (int)Math.Round((double)dpix * Scale);
            dpiy = (int)Math.Round((double)dpiy * Scale);
            windowwidth = (int)Math.Round(1000.0 * rect.Right / dpix);
            windowheight = (int)Math.Round(1000.0 * rect.Bottom / dpiy);
            if (Scale>=1)
            {
                h1 = (int)Math.Round(120.0 / Scale * 1.5);
                h2 = (int)Math.Round(60.0 / Scale * 1.5);
                h3 = (int)Math.Round(30.0 / Scale * 1.5);
            }
            else
            {
                h1=240;
                h2=120;
                h3=60;
            }
            int originx = windowwidth;
            int originy = windowheight;
            int destinationx = rect.Right;
            int destinationy = rect.Bottom;
            int clength;
            int cheight;
            string nvalue;
            int x,value,avaluex,avaluey,avaluex2,avaluey2;
            double i;
            if (Style == RulerStyle.Horizontal)
            {
                textpos = rect.Bottom - font.Height - (int)Math.Round((double)h2*dpiy/Twips.TWIPS_PER_INCH);
                i=0.0;
                clength=windowwidth;
                cheight=windowheight;
                x=0;
                while (i<clength)
                {
                    value=x % 10;
                    if (value==0)
                    {
                        // One number
                        avaluex=(int)Math.Round(i);
                        avaluey=textpos;
                        avaluex=LogicalToDevice(originx,destinationx,avaluex);
                        nvalue = ((int)Math.Round(i / onethousand)).ToString();
                        gr.DrawString(nvalue,font,brush,new PointF(avaluex,avaluey));
                        avaluey = cheight;
                        avaluey2 = cheight - h1;
                        avaluex2 = avaluex;
                        avaluey = LogicalToDevice(originy, destinationy, avaluey);
                        avaluey2 = LogicalToDevice(originy, destinationy, avaluey2);
                        gr.DrawLine(pen, avaluex, avaluey, avaluex2, avaluey2);
                    }
                    else
                    if (value==5)
                    {
                        // 0.5 units
                        avaluex=(int)Math.Round(i);
                        avaluey=cheight;
                        avaluey2=cheight-h2;
                        avaluex=LogicalToDevice(originx,destinationx,avaluex);
                        avaluex2=avaluex;
                        avaluey=LogicalToDevice(originy,destinationy,avaluey);
                        avaluey2=LogicalToDevice(originy,destinationy,avaluey2);
                        gr.DrawLine(pen,avaluex,avaluey,avaluex2,avaluey2);
                    }
                    else
                    {
                        // 0.1 units
                        avaluex = (int)Math.Round(i);
                        avaluey = cheight;
                        avaluey2 = cheight - h3;
                        avaluex = LogicalToDevice(originx, destinationx, avaluex);
                        avaluex2 = avaluex;
                        avaluey = LogicalToDevice(originy, destinationy, avaluey);
                        avaluey2 = LogicalToDevice(originy, destinationy, avaluey2);
                        gr.DrawLine(pen, avaluex, avaluey, avaluex2, avaluey2);
                    }

                    i=i+onecent;
                    x++;
                }
            }
            else
            {
                textpos = rect.Right - (int)Math.Round((double)h2*dpiy/Twips.TWIPS_PER_INCH);
                i=0.0;
                clength=windowheight;
                cheight=windowwidth;
                x=0;
                while (i < clength)
                {
                    value = x % 10;
                    if (value == 0)
                    {
                        // One number
                        nvalue = ((int)Math.Round(i / onethousand)).ToString();
                        avaluex = textpos-(int)Math.Round(gr.MeasureString(nvalue, font).Width)-2;
                        avaluey = (int)Math.Round(i);
                        avaluey = LogicalToDevice(originy, destinationy, avaluey);
                        gr.DrawString(nvalue, font, brush, new PointF(avaluex, avaluey));
                        avaluex2 = cheight;
                        avaluex = cheight - h1;
                        avaluex = LogicalToDevice(originx, destinationx, avaluex);
                        avaluex2 = LogicalToDevice(originx, destinationx, avaluex2);
                        avaluey2 = avaluey;
                        gr.DrawLine(pen, avaluex, avaluey, avaluex2, avaluey2);
                    }
                    else
                        if (value == 5)
                        {
                            // 0.5 units
                            avaluey = (int)Math.Round(i);
                            avaluex2 = cheight;
                            avaluex = cheight - h2;
                            avaluey = LogicalToDevice(originy, destinationy, avaluey);
                            avaluey2 = avaluey;
                            avaluex = LogicalToDevice(originx, destinationx, avaluex);
                            avaluex2 = LogicalToDevice(originx, destinationx, avaluex2);
                            gr.DrawLine(pen, avaluex, avaluey, avaluex2, avaluey2);
                        }
                        else
                        {
                            // 0.1 units
                            avaluey = (int)Math.Round(i);
                            avaluex2 = cheight;
                            avaluex = cheight - h3;
                            avaluey = LogicalToDevice(originy, destinationy, avaluey);
                            avaluey2 = avaluey;
                            avaluex = LogicalToDevice(originx, destinationx, avaluex);
                            avaluex2 = LogicalToDevice(originx, destinationx, avaluex2);
                            gr.DrawLine(pen, avaluex, avaluey, avaluex2, avaluey2);
                        }

                    i = i + onecent;
                    x++;
                }

            }
        }

        private void DoInit()
        {
#if REPMAN_COMPACT
#else
            LayoutEventHandler resizeevent;
            resizeevent = new LayoutEventHandler(MyResizeHandler);
            Layout += resizeevent;
#endif
        }
#if REPMAN_COMPACT
#else
        private void MyResizeHandler(object sender, LayoutEventArgs e)
        {
            bitmapvalid = false;
            Invalidate();
        }
#endif
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (bitmap != null)
                bitmap.Dispose();
            if (brush != null)
                brush.Dispose();
            if (brushpen != null)
                brushpen.Dispose();
            if (pen != null)
                pen.Dispose();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component designer generated code

        /// <summary>
        /// Required method for the designer
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }
}
