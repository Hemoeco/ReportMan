using System;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
#if NETSTANDARD2_0
using System.Globalization;
using System.Collections;
using System.ComponentModel;
#endif

namespace Reportman.Drawing
{
    public partial class GraphicUtils
    {
#if NETSTANDARD2_0
        public static SortedList<string, Color> ColorNames;
#else
        public static SortedList<string, KnownColor> ColorNames;
#endif
        private static object flag = 2;
        private static void UpdateColorNames()
        {
            Monitor.Enter(flag);
            try
            {
                if (ColorNames == null)
                {
#if NETSTANDARD2_0
                    ColorNames = new SortedList<string, Color>();
                    /*var typeToCheckTo = typeof(Color);
                    var type = typeof(SystemColors);
                    var fields = type.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).Where(p => p.PropertyType.Equals(typeToCheckTo));
                    foreach (var field in fields)
                    {
                        object value = field.GetValue(null, null);
                        ColorNames.Add(field.Name, (Color)value);
                   }*/
#else
                    ColorNames = new SortedList<string, KnownColor>();
                    KnownColor ncolor = KnownColor.ActiveBorder;
                    string[] names = Enum.GetNames(ncolor.GetType());
                    KnownColor[] values = (KnownColor[])Enum.GetValues(ncolor.GetType());
                    int i = 0;
                    foreach (string s in names)
                    {
                        ColorNames.Add(s, values[i]);
                        i++;
                    }
#endif
                }
            }
            finally
            {
                Monitor.Exit(flag);
            }
        }
        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        private static byte[] SimpleThresholdBW(byte[] sourceBuffer, int width, int height, int srcStride, int dstStride, int threshold)
        {
            byte[] destinationBuffer = new byte[dstStride * height];
            int srcIx = 0;
            int dstIx = 0;
            byte bit;
            byte pix8;

            int newpixel, i, j;

            // Iterate lines
            for (int y = 0; y < height; y++, srcIx += srcStride, dstIx += dstStride)
            {
                bit = 128;
                i = srcIx;
                j = dstIx;
                pix8 = 0;
                // Iterate pixels
                for (int x = 0; x < width; x++, i += 4)
                {
                    // Compute pixel brightness (i.e. total of Red, Green, and Blue values)
                    newpixel = sourceBuffer[i] + sourceBuffer[i + 1] + sourceBuffer[i + 2];

                    if (newpixel > threshold)
                        pix8 |= bit;
                    if (bit == 1)
                    {
                        destinationBuffer[j++] = pix8;
                        bit = 128;
                        pix8 = 0; // init next value with 0
                    }
                    else
                        bit >>= 1;
                } // line finished
                if (bit != 128)
                    destinationBuffer[j] = pix8;
            } // all lines finished
            return destinationBuffer;
        }

        public static System.Drawing.Bitmap ConvertToBitonal(System.Drawing.Bitmap original, int threshold)
        {
            System.Drawing.Bitmap source = null;

            if (original.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
                return (System.Drawing.Bitmap)original.Clone();
            else if (original.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            { // If original bitmap is not already in 32 BPP, ARGB format, then convert
                // unfortunately Clone doesn't do this for us but returns a bitmap with the same pixel format
                //source = original.Clone( new Rectangle( Point.Empty, original.Size ), PixelFormat.Format32bppArgb );
                source = new System.Drawing.Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(source))
                {
                    //g.CompositingQuality = Drawing2D.CompositingQuality.GammaCorrected;
                    //g.InterpolationMode = Drawing2D.InterpolationMode.Low;
                    //g.SmoothingMode = Drawing2D.SmoothingMode.None;
                    g.DrawImageUnscaled(original, 0, 0);
                }
            }
            else
            {
                source = original;
            }

            // Lock source bitmap in memory
            System.Drawing.Imaging.BitmapData sourceData = source.LockBits(new System.Drawing.Rectangle(0, 0, source.Width, source.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Copy image data to binary array
            int imageSize = sourceData.Stride * sourceData.Height;
            byte[] sourceBuffer = new byte[imageSize];
            System.Runtime.InteropServices.Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

            // Unlock source bitmap
            source.UnlockBits(sourceData);

            // Dispose of source if not originally supplied bitmap
            if (source != original)
            {
                source.Dispose();
            }

            // Create destination bitmap
            System.Drawing.Bitmap destination = new System.Drawing.Bitmap(sourceData.Width, sourceData.Height, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            destination.SetResolution(original.HorizontalResolution, original.VerticalResolution);

            // Lock destination bitmap in memory
            System.Drawing.Imaging.BitmapData destinationData = destination.LockBits(new System.Drawing.Rectangle(0, 0, destination.Width, destination.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);

            // Create destination buffer
            byte[] destinationBuffer = SimpleThresholdBW(
            sourceBuffer,
            sourceData.Width,
            sourceData.Height,
            sourceData.Stride,
            destinationData.Stride, threshold);

            // Copy binary image data to destination bitmap
            System.Runtime.InteropServices.Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, destinationData.Stride * sourceData.Height);

            // Unlock destination bitmap
            destination.UnlockBits(destinationData);

            // Return
            return destination;
        }


        /// <summary>
        /// Create a Color based on a 32 bit integer
        /// </summary>
        /// <param name="aint">Integer color value in the form of $00BBGGRR</param>
        /// <returns>Returs a Color usable in any System.Drawing function</returns>
        public static Color ColorFromInteger(int aint)
        {
            UpdateColorNames();
            if ((aint >= 0) || (aint < -ColorNames.Count))
            {
                byte r = (byte)(aint);
                byte g = (byte)(aint >> 8);
                byte b = (byte)(aint >> 16);
                Color ncolor = Color.FromArgb(r, g, b);
                return ncolor;
            }
            else
            {
#if NETSTANDARD2_0
                // Known colors not implemented in NET STandard
                if (-aint >= ColorNames.Count)
                {
                    return ColorFromInteger(-aint);
                }
                else
                {
                    string keycolor = ColorNames.Keys[-aint];
                    return ColorNames[keycolor];
                }
#else

                string keycolor = ColorNames.Keys[-aint];
                return Color.FromKnownColor(ColorNames[keycolor]);
#endif
            }
        }
        /// <summary>
        /// Create an integer value based on a Color        /// </summary>
        /// <param name="acolor">Color value</param>
        /// <returns>Returs an integer value  in the form of $00BBGGRR</returns>
        public static int IntegerFromColor(Color acolor)
        {
            int aresult;
            UpdateColorNames();
#if NETSTANDARD2_0
            aresult = (int)acolor.R + (int)(acolor.G << 8) + ((int)acolor.B << 16);
#else
            if (acolor.IsKnownColor)
            {
                aresult = -ColorNames.IndexOfValue(acolor.ToKnownColor());
            }
            else
                aresult = (int)acolor.R + (int)(acolor.G << 8) + ((int)acolor.B << 16);
#endif
            return aresult;
        }
        public static int IntegerFromColorA(Color acolor)
        {
            int aresult;
            aresult = (int)acolor.R + (int)(acolor.G << 8) + ((int)acolor.B << 16);
            return aresult;
        }
#if NETSTANDARD2_0
        private static Hashtable htmlSysColorTable;
        /// <include file='doc\ColorTranslator.uex' path='docs/doc[@for="ColorTranslator.FromHtml"]/*' />
        /// <devdoc>
        ///    Translates an Html color representation to
        ///    a GDI+ <see cref='System.Drawing.Color'/>.
        /// </devdoc>
        public static Color ColorFromHtml(string htmlColor)
        {
            Color c = Color.Empty;

            // empty color
            if ((htmlColor == null) || (htmlColor.Length == 0))
                return c;

            // #RRGGBB or #RGB
            if ((htmlColor[0] == '#') &&
                ((htmlColor.Length == 7) || (htmlColor.Length == 4)))
            {

                if (htmlColor.Length == 7)
                {
                    c = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 16),
                                       Convert.ToInt32(htmlColor.Substring(3, 2), 16),
                                       Convert.ToInt32(htmlColor.Substring(5, 2), 16));
                }
                else
                {
                    string r = Char.ToString(htmlColor[1]);
                    string g = Char.ToString(htmlColor[2]);
                    string b = Char.ToString(htmlColor[3]);

                    c = Color.FromArgb(Convert.ToInt32(r + r, 16),
                                       Convert.ToInt32(g + g, 16),
                                       Convert.ToInt32(b + b, 16));
                }
            }

            // special case. Html requires LightGrey, but .NET uses LightGray
            if (c.IsEmpty && String.Equals(htmlColor, "LightGrey", StringComparison.OrdinalIgnoreCase))
            {
                c = Color.LightGray;
            }

            // System color
            if (c.IsEmpty)
            {
                if (htmlSysColorTable == null)
                {
                    InitializeHtmlSysColorTable();
                }

                object o = htmlSysColorTable[htmlColor.ToLower(CultureInfo.InvariantCulture)];
                if (o != null)
                {
                    c = (Color)o;
                }
            }

            // resort to type converter which will handle named colors
            if (c.IsEmpty)
            {
                c = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(htmlColor);
            }

            return c;
        }
        private static void InitializeHtmlSysColorTable()
        {
            htmlSysColorTable = new Hashtable(0);
            /*
            htmlSysColorTable = new Hashtable(26);
            htmlSysColorTable["activeborder"] = Color.FromKnownColor(KnownColor.ActiveBorder);
            htmlSysColorTable["activecaption"] = Color.FromKnownColor(KnownColor.ActiveCaption);
            htmlSysColorTable["appworkspace"] = Color.FromKnownColor(KnownColor.AppWorkspace);
            htmlSysColorTable["background"] = Color.FromKnownColor(KnownColor.Desktop);
            htmlSysColorTable["buttonface"] = Color.FromKnownColor(KnownColor.Control);
            htmlSysColorTable["buttonhighlight"] = Color.FromKnownColor(KnownColor.ControlLightLight);
            htmlSysColorTable["buttonshadow"] = Color.FromKnownColor(KnownColor.ControlDark);
            htmlSysColorTable["buttontext"] = Color.FromKnownColor(KnownColor.ControlText);
            htmlSysColorTable["captiontext"] = Color.FromKnownColor(KnownColor.ActiveCaptionText);
            htmlSysColorTable["graytext"] = Color.FromKnownColor(KnownColor.GrayText);
            htmlSysColorTable["highlight"] = Color.FromKnownColor(KnownColor.Highlight);
            htmlSysColorTable["highlighttext"] = Color.FromKnownColor(KnownColor.HighlightText);
            htmlSysColorTable["inactiveborder"] = Color.FromKnownColor(KnownColor.InactiveBorder);
            htmlSysColorTable["inactivecaption"] = Color.FromKnownColor(KnownColor.InactiveCaption);
            htmlSysColorTable["inactivecaptiontext"] = Color.FromKnownColor(KnownColor.InactiveCaptionText);
            htmlSysColorTable["infobackground"] = Color.FromKnownColor(KnownColor.Info);
            htmlSysColorTable["infotext"] = Color.FromKnownColor(KnownColor.InfoText);
            htmlSysColorTable["menu"] = Color.FromKnownColor(KnownColor.Menu);
            htmlSysColorTable["menutext"] = Color.FromKnownColor(KnownColor.MenuText);
            htmlSysColorTable["scrollbar"] = Color.FromKnownColor(KnownColor.ScrollBar);
            htmlSysColorTable["threeddarkshadow"] = Color.FromKnownColor(KnownColor.ControlDarkDark);
            htmlSysColorTable["threedface"] = Color.FromKnownColor(KnownColor.Control);
            htmlSysColorTable["threedhighlight"] = Color.FromKnownColor(KnownColor.ControlLight);
            htmlSysColorTable["threedlightshadow"] = Color.FromKnownColor(KnownColor.ControlLightLight);
            htmlSysColorTable["window"] = Color.FromKnownColor(KnownColor.Window);
            htmlSysColorTable["windowframe"] = Color.FromKnownColor(KnownColor.WindowFrame);
            htmlSysColorTable["windowtext"] = Color.FromKnownColor(KnownColor.WindowText);*/
        }
#else
#endif
        public static Color ColorFromString(string ncolor)
        {
            if (ncolor.Length == 0)
                throw new Exception("Invalid color, ColorFromString, empty string");
            if (ncolor[0] == '#')
#if NETSTANDARD2_0
                return ColorFromHtml(ncolor);
#else
                return ColorTranslator.FromHtml(ncolor);
#endif
            if (ncolor[0] == '(')
            {
                ncolor = ncolor.Substring(1, ncolor.Length - 1);
                int index = ncolor.IndexOf(')');
                if (index == (ncolor.Length - 1))
                    ncolor = ncolor.Substring(0, ncolor.Length - 1);
                char separator = ';';
                index = ncolor.IndexOf(",");
                if (index >= 0)
                    separator = ',';
                string[] colorarray = ncolor.Split(separator);
                byte r = 0;
                byte g = 0;
                byte b = 0;
                int i = 0;
                foreach (string acolor in colorarray)
                {
                    switch (i)
                    {
                        case 0:
                            r = System.Convert.ToByte(acolor);
                            break;
                        case 1:
                            g = System.Convert.ToByte(acolor);
                            break;
                        default:
                            b = System.Convert.ToByte(acolor);
                            break;
                    }
                    i++;
                }
                return Color.FromArgb(r, g, b);
            }
            else
                return Color.FromName(ncolor);

        }
        /// <summary>
        /// Create a Color based on a 32 bit integer
        /// </summary>
        /// <param name="aint">Integer color value in the form of $00BBGGRR</param>
        /// <returns>Returs a Color usable in any System.Drawing function</returns>
        public static Color ColorFromIntegerA(int aint)
        {
            byte r = (byte)(aint);
            byte g = (byte)(aint >> 8);
            byte b = (byte)(aint >> 16);
            Color ncolor = Color.FromArgb(255, r, g, b);
            return ncolor;
        }
        /// <summary>
        /// Retuns a codec with the mime type or null if not found
        /// </summary>
        /// <param name="mimeType">Codec string, example "image/jpeg"</param>
        /// <returns></returns>
        public static System.Drawing.Imaging.ImageCodecInfo GetImageCodec(string mimeType)
        {
            System.Drawing.Imaging.ImageCodecInfo[] codecs
              = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();


            foreach (System.Drawing.Imaging.ImageCodecInfo codec in codecs)
            {
                if (String.Compare(codec.MimeType,
                                   mimeType, true) == 0)
                {
                    return codec;
                }
            }


            return null;
        }

    }
}
