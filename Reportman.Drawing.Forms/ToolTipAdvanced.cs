using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Reportman.Drawing.Forms
{
    public class ToolTipAdvanced : ToolTip
    {
        Image FImage;
        public Image Image
        {
            get
            {
                return FImage;
            }
            set
            {
                FImage = value;
            }
        }
        const int BORDER_THICKNESS = 1;
        Brush myBorderBrush;
        Brush myBackColorBrush;
        public ToolTipAdvanced()
        {
            this.OwnerDraw = true;
            this.Popup += new PopupEventHandler(CustomizedToolTip_Popup);
            this.Draw += new DrawToolTipEventHandler(CustomizedToolTip_Draw);
            myBorderBrush = new SolidBrush(SystemColors.InfoText);
            myBackColorBrush = new SolidBrush(SystemColors.Info);
        }
        void CustomizedToolTip_Popup(object sender, PopupEventArgs e)
        {
            if (OwnerDraw)
            {
                Size oldSize = e.ToolTipSize;
                Control parent = e.AssociatedControl;
                if (FImage != null)
                    e.ToolTipSize = new Size(FImage.Width + BORDER_THICKNESS * 2, FImage.Height + BORDER_THICKNESS * 2);
            }
        }
        void CustomizedToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Rectangle myToolTipRectangle = new Rectangle();
            myToolTipRectangle.Size = e.Bounds.Size;
            e.Graphics.FillRectangle(myBorderBrush, myToolTipRectangle);
            Rectangle myImageRectangle = Rectangle.Inflate(myToolTipRectangle,
                    -BORDER_THICKNESS, -BORDER_THICKNESS);
            e.Graphics.FillRectangle(myBackColorBrush, myImageRectangle);
            Control parent = e.AssociatedControl;
            Image toolTipImage = FImage;
            if (FImage != null)
            {
                myImageRectangle.Width = FImage.Width;
                myImageRectangle.Height = FImage.Height;
                e.Graphics.DrawImage(toolTipImage, myImageRectangle);
            }
        }
    }
}
