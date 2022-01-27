using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reportman.Drawing.Forms
{
    public partial class TransformImageForm : Form
    {
        public TransformImageForm()
        {
            InitializeComponent();
            labelAltoOriginal.Font = new Font(labelAltoOriginal.Font, FontStyle.Bold);
            labelAnchoOriginal.Font = labelAltoOriginal.Font;
            labelFormatoOriginal.Font = labelAltoOriginal.Font;
            labelSize.Font = labelAltoOriginal.Font;
            labelOriginalSize.Font = labelAltoOriginal.Font;
        }
        Image NewImage;
        Image OriginalImage;
        System.IO.MemoryStream NewImageStream;
        System.IO.MemoryStream OriginalImageStream;
        bool Modified = false;
        public void Inicializar(System.IO.MemoryStream nOriginalStream, int? defaultSize)
        {
            OriginalImageStream = nOriginalStream;
            OriginalImage = Image.FromStream(OriginalImageStream);
            NewImage = OriginalImage;
            labelAnchoOriginal.Text = OriginalImage.Width.ToString("N0");
            labelAltoOriginal.Text = OriginalImage.Height.ToString("N0");
            EditNuevoAlto.Value = OriginalImage.Height;
            EditNuevoAncho.Value = OriginalImage.Width;
            comboFormat.SelectedIndex = 0;
            AspectRatio = (decimal)OriginalImage.Width / OriginalImage.Height;
            if (System.Drawing.Imaging.ImageFormat.Jpeg.Equals(OriginalImage.RawFormat))
            {
                comboFormat.SelectedIndex = 1;
                labelFormatoOriginal.Text = "JPEG";
            }
            else
            if (System.Drawing.Imaging.ImageFormat.Gif.Equals(OriginalImage.RawFormat))
            {
                comboFormat.SelectedIndex = 2;
                labelFormatoOriginal.Text = "GIF";
            }
            else
            {
                labelFormatoOriginal.Text = "PNG";
            }
            picImage.Image = NewImage;
            labelSize.Text = "Tamaño: " + Reportman.Drawing.StringUtil.GetSizeAsString(OriginalImageStream.Length);
            labelOriginalSize.Text = labelSize.Text;
            EditNuevoAlto.ValueChanged += EditNuevoAlto_ValueChanged;
            EditNuevoAncho.ValueChanged += EditNuevoAncho_ValueChanged;
            comboFormat.SelectedIndexChanged += ComboFormat_SelectedIndexChanged;
            editCalidad.ValueChanged += EditCalidad_ValueChanged;

            if (defaultSize != null)
            {
                if (OriginalImage.Width>OriginalImage.Height)
                {
                    if (OriginalImage.Width > defaultSize)
                        EditNuevoAncho.Value = Convert.ToInt32(defaultSize);
                }
                else
                {
                    if (OriginalImage.Height > defaultSize)
                        EditNuevoAlto.Value = Convert.ToInt32(defaultSize);
                }
            }
        }

        private void EditCalidad_ValueChanged(object sender, EventArgs e)
        {
            RecalcularImagen();
        }

        private void ComboFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecalcularImagen();
        }
        private void RecalcularImagen()
        {
            NewImageStream = new System.IO.MemoryStream();
            // Se mira si el tamaño es distinto
            if ((OriginalImage.Width != EditNuevoAncho.Value) || (OriginalImage.Height != EditNuevoAlto.Value))
            {
                int alto = Convert.ToInt32(EditNuevoAlto.Value);
                int ancho = Convert.ToInt32(EditNuevoAncho.Value);
                NewImage = new Bitmap(ancho, alto);
                using (Graphics gr = Graphics.FromImage(NewImage))
                {
                    gr.DrawImage(OriginalImage, new Rectangle(0, 0, ancho, alto ));
                }

            }
            else
                NewImage = OriginalImage;
            NewImageStream = new System.IO.MemoryStream();
            WriteImage();
        }
        private void WriteImage()
        {
            NewImageStream = new System.IO.MemoryStream();
            switch (comboFormat.SelectedIndex)
            {
                case 1:
                    System.Drawing.Imaging.ImageCodecInfo icodec = GraphicUtils.GetImageCodec("image/jpeg");
                    System.Drawing.Imaging.EncoderParameters eparams = new System.Drawing.Imaging.EncoderParameters(1);
                    System.Drawing.Imaging.EncoderParameter qParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality,
                        Convert.ToInt64(editCalidad.Value));
                    eparams.Param[0] = qParam;
                    NewImage.Save(NewImageStream, icodec, eparams);
                    break;
                case 2:
                    NewImage.Save(NewImageStream, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                default:
                    NewImage.Save(NewImageStream, System.Drawing.Imaging.ImageFormat.Png);
                    break;
            }
            NewImageStream.Seek(0, System.IO.SeekOrigin.Begin);
            NewImage = Image.FromStream(NewImageStream);
            picImage.Image = NewImage;
            labelSize.Text = Reportman.Drawing.StringUtil.GetSizeAsString(NewImageStream.Length);
        }
        private void EditNuevoAncho_ValueChanged(object sender, EventArgs e)
        {
            if (CambiandoAspecto)
                return;
            if (checkAspecto.Checked)
            {
                CambiandoAspecto = true;
                try
                {
                    EditNuevoAlto.Value = Convert.ToInt32(EditNuevoAncho.Value / AspectRatio);
                }
                finally
                {
                    CambiandoAspecto = false;
                }
            }
            RecalcularImagen();
        }

        bool CambiandoAspecto = false;
        decimal AspectRatio = 1;
        private void EditNuevoAlto_ValueChanged(object sender, EventArgs e)
        {
            if (CambiandoAspecto)
                return;
            if (checkAspecto.Checked)
            {
                CambiandoAspecto = true;
                try
                {
                    EditNuevoAncho.Value = Convert.ToInt32(EditNuevoAlto.Value * AspectRatio);
                }
                finally
                {
                    CambiandoAspecto = false;
                }
            }
            RecalcularImagen();
        }

        public static System.IO.MemoryStream ShowTransformImage(System.IO.MemoryStream imageStream,IWin32Window owner,int? defaultSize)
        {
            System.IO.MemoryStream result = null;
            using (TransformImageForm dia = new TransformImageForm())
            {
                dia.Inicializar(imageStream,defaultSize);
                if (dia.ShowDialog(owner) == DialogResult.OK)
                {
                    result = dia.NewImageStream;
                }
            }
            return result;
        }
        private void Bcancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;            
        }

        private void BAceptar_Click(object sender, EventArgs e)
        {
            if (NewImageStream == null)
                NewImageStream = OriginalImageStream;
            DialogResult = DialogResult.OK;
        }
    }
}
