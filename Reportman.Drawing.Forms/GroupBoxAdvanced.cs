using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reportman.Drawing.Forms
{
    public partial class GroupBoxAdvanced : GroupBox
    {
        private Color FBorderColor;
        private Color FTextColor;
        public GroupBoxAdvanced()
        {
            FBorderColor = SystemColors.ActiveBorder;
            FTextColor = SystemColors.ActiveCaption;
            InitializeComponent();
        }
        [DefaultValue(typeof(Color), "ActiveBorder")]
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
        [DefaultValue(typeof(Color), "ActiveCaption")]
        public Color TextColor
        {
            get
            {
                return FTextColor;
            }
            set
            {
                FTextColor = value;
                Invalidate();
            }

        }
        protected override void OnPaint(PaintEventArgs e)
        {
/*            Size tSize = TextRenderer.MeasureText(Text, Font);
            Rectangle borderRect = e.ClipRectangle;
            borderRect.Y = (borderRect.Y + (tSize.Height / 2));
            borderRect.Height = (borderRect.Height - (tSize.Height / 2));
            Rectangle textRect = new Rectangle(6, 0, tSize.Width, tSize.Height);
            e.Graphics.SetClip(textRect, System.Drawing.Drawing2D.CombineMode.Exclude);
            ControlPaint.DrawBorder(e.Graphics, borderRect, FBorderColor,
                ButtonBorderStyle.Solid);
            e.Graphics.SetClip(e.ClipRectangle);
            e.Graphics.FillRectangle(new SolidBrush(BackColor), textRect);
            e.Graphics.DrawString(Text, Font, new
                SolidBrush(FTextColor), textRect);
 */
            SizeF tSize = new SizeF(0, 0);
            try
            {
                e.Graphics.FillRectangle(new SolidBrush(BackColor), this.Bounds);
                tSize = e.Graphics.MeasureString(Text, Font);

                e.Graphics.DrawString(Text, Font, new
                    SolidBrush(FTextColor), new PointF(0.0f, 0.0f));
            }
            catch
            {
            }
            try
            {
                e.Graphics.SetClip(new RectangleF(0.0f, 0.0f, tSize.Width, tSize.Height), System.Drawing.Drawing2D.CombineMode.Exclude);
            }
            catch
            {
            }

            try
            {
                Rectangle borderRect = new Rectangle(0, (int)Math.Round(tSize.Height / 2.0), Width, (int)Math.Round((float)Height - tSize.Height / 2.0));
                ControlPaint.DrawBorder(e.Graphics, borderRect, FBorderColor,
                    ButtonBorderStyle.Solid);
            }
            catch
            {
            }


            //base.OnPaint(e);
 
        }
    }
    
}
