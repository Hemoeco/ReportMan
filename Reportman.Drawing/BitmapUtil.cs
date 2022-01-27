using System;
using System.IO;
using System.Linq;
#if NETSTANDARD2_0
#else
using System.Drawing;
#endif

namespace Reportman.Drawing
{
    public static class BitmapUtil
    {
        /// <summary>
        /// Obtain information about a jpeg stream
        /// </summary>
        /// <param name="astream">Input stream</param>
        /// <param name="width">Ouput parameter, width in pixels</param>
        /// <param name="height">Ouput parameter, height in pixels</param>
        /// <returns>Returns false if it's not a jpeg</returns>
        public static bool GetJPegInfo(Stream astream, out int width,
            out int height)
        {
            width = 0;
            height = 0;
            byte[] c1 = new byte[2];
            byte[] c2 = new byte[2];
            int readed;
            byte marker;
            bool aresult = false;
            // Checks it's a jpeg image
            readed = astream.Read(c1, 0, 1);
            if (readed < 1)
            {
                astream.Seek(0, System.IO.SeekOrigin.Begin);
                return aresult;
            }
            if (c1[0] != 0xFF)
            {
                astream.Seek(0, System.IO.SeekOrigin.Begin);
                return aresult;
            }
            readed = astream.Read(c2, 0, 1);
            if (readed < 1)
            {
                astream.Seek(0, System.IO.SeekOrigin.Begin);
                return aresult;
            }
            if (c2[0] != M_SOI)
            {
                astream.Seek(0, System.IO.SeekOrigin.Begin);
                return aresult;
            }
            aresult = true;
            // Read segments until M_SOS
            do
            {
                marker = NextMarker(astream);
                switch (marker)
                {
                    case M_SOF0:        // Baseline }
                    case M_SOF1:        // Extended sequential, Huffman }
                    case M_SOF2:        // Progressive, Huffman }
                    case M_SOF3:        // Lossless, Huffman }
                    case M_SOF5:        // Differential sequential, Huffman }
                    case M_SOF6:        // Differential progressive, Huffman }
                    case M_SOF7:        // Differential lossless, Huffman }
                    case M_SOF9:        // Extended sequential, arithmetic }
                    case M_SOF10:       // Progressive, arithmetic }
                    case M_SOF11:       // Lossless, arithmetic }
                    case M_SOF13:       // Differential sequential, arithmetic }
                    case M_SOF14:       // Differential progressive, arithmetic }
                    case M_SOF15:       // Differential lossless, arithmetic }
                        process_SOFn(astream, out height, out width);
                        // Exit, no more info need
                        marker = M_SOS;
                        break;
                    default:
                        skip_variable(astream);
                        break;
                }
            }
            while ((marker != M_SOS) && (marker != M_EOI));
            astream.Seek(0, System.IO.SeekOrigin.Begin);
            return aresult;
        }
        /// <summary>
        /// Obtain information about a bitmap stream
        /// </summary>
        /// <param name="astream">Input stream</param>
        /// <param name="width">Output parameter, with of the bitmap in pixels</param>
        /// <param name="height">Output parameter, height of the bitmap in pixels</param>
        /// <param name="imagesize">Size in bytes of the image information part</param>
        /// <param name="MemBits">Bits containing information converted to Adobe PDF compatible form</param>
        /// <param name="indexed">Output parameter, returns true if the image is paletized</param>
        /// <param name="bitsperpixel">Output parameter, number of bits of information for each pixel</param>
        /// <param name="usedcolors">Output parameter, valid for indexed bitmaps, number of colors used from the palette</param>
        /// <param name="palette">Output parameter, palette in Adobe PDF compatible form, valid only in indexed bitmaps</param>
        /// <returns>Returns false if the stream is not abitmap</returns>
		public static bool GetBitmapInfo(Stream sourcestream, out int width,
            out int height, out int imagesize, MemoryStream MemBits,
            out bool indexed, out int bitsperpixel, out int usedcolors,
            out string palette, out bool isgif, out string mask, MemoryStream smask)
        {
            mask = "";
            bool aresult = false;
            MemoryStream newstream = null;
            try
            {
                const int MAX_BITMAPHEADERSIZE = 32000;
                Stream astream = sourcestream;

                isgif = false;
                byte[] buf = new byte[16];
                width = 0;
                height = 0;
                imagesize = 0;
                indexed = false;
                bool iscoreheader = false;
                bitsperpixel = 8;
                usedcolors = 0;
                palette = "";
                int readed, index;
                BitmapFileHeader fileheader = new BitmapFileHeader();
                BitmapInfoHeader infoheader = new BitmapInfoHeader();
                BitmapCoreHeader coreheader = new BitmapCoreHeader();
                // Read the file header
                readed = astream.Read(buf, 0, 14);
                if (readed != 14)
                {
                    astream.Seek(0, System.IO.SeekOrigin.Begin);
                    return aresult;
                }
                isgif = (((char)buf[0] == 'G') && ((char)buf[1] == 'I') && ((char)buf[2] == 'F'));
                if (isgif)
                {
                    astream.Seek(0, System.IO.SeekOrigin.Begin);
                    return false;
#if NETSTANDARD2_0
                    throw new Exception("GIF Images not supported in .Net Standard");
#else
                    /*using (Image nimage = Image.FromStream(astream))
                    {
                        using (Bitmap nbitmap = new Bitmap(nimage.Width,nimage.Height,System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                        {
                            using (var gr = Graphics.FromImage(nbitmap))
                            {
                                gr.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, nimage.Width, nimage.Height));
                                gr.DrawImage(nimage, new Rectangle(0, 0, nimage.Width, nimage.Height), new Rectangle(0, 0, nimage.Width, nimage.Height),
                                    GraphicsUnit.Pixel);
                            }
                            astream.Seek(0, System.IO.SeekOrigin.Begin);
                            newstream = new MemoryStream();
                            nbitmap.Save(newstream, System.Drawing.Imaging.ImageFormat.Bmp);
                            nbitmap.Save("c:\\datos\\prueba.bmp");
                        }
                        newstream.Seek(0, System.IO.SeekOrigin.Begin);
                        isgif = false;
                        astream = newstream;
                    }*/
                    return true;
                    //readed = astream.Read(buf, 0, 14);
                    //if (readed != 14)
                    //
                    //    astream.Seek(0, System.IO.SeekOrigin.Begin);
                    //    return aresult;
                    // }
#endif
                }

                if (((char)buf[0] != 'B') || ((char)buf[1] != 'M'))
                {
                    return aresult;
                }
                fileheader.bfSize = StreamUtil.ByteArrayToUInt(buf, 2, 4);
                fileheader.bfOffBits = StreamUtil.ByteArrayToUInt(buf, 10, 4);
                // Read the size of bitmap info
                readed = astream.Read(buf, 0, 4);
                if (readed != 4)
                {
                    astream.Seek(0, System.IO.SeekOrigin.Begin);
                    return aresult;
                }
                uint bsize = StreamUtil.ByteArrayToUInt(buf, 0, 4);
                if ((bsize < 2) || (bsize > MAX_BITMAPHEADERSIZE))
                {
                    astream.Seek(0, System.IO.SeekOrigin.Begin);
                    return aresult;
                }
                if (bsize < 15)
                    iscoreheader = true;
                else
                    buf = new byte[bsize + 1];
                readed = astream.Read(buf, 0, (int)(bsize - 4));
                if (readed != (int)(bsize - 4))
                {
                    astream.Seek(0, System.IO.SeekOrigin.Begin);
                    return aresult;
                }
                if (iscoreheader)
                {
                    coreheader.bcSize = bsize;
                    coreheader.bcWidth = StreamUtil.ByteArrayToUInt(buf, 0, 4);
                    coreheader.bcHeight = StreamUtil.ByteArrayToUInt(buf, 4, 4);
                    coreheader.bcPlanes = StreamUtil.ByteArrayToUShort(buf, 8, 2);
                    coreheader.bcBitCount = StreamUtil.ByteArrayToUShort(buf, 10, 2);
                    bitsperpixel = coreheader.bcBitCount;
                    width = (int)coreheader.bcWidth;
                    height = (int)coreheader.bcHeight;
                    imagesize = width * height * 3;
                    if (MemBits == null)
                    {
                        aresult = true;
                        return aresult;
                    }
                }
                else
                {
                    infoheader.biSize = bsize;
                    infoheader.biWidth = StreamUtil.ByteArrayToInt(buf, 0, 4);
                    infoheader.biHeight = StreamUtil.ByteArrayToInt(buf, 4, 4);
                    infoheader.biPlanes = StreamUtil.ByteArrayToUShort(buf, 8, 2);
                    infoheader.biBitCount = StreamUtil.ByteArrayToUShort(buf, 10, 2);
                    infoheader.biClrUsed = (uint)StreamUtil.ByteArrayToInt(buf, 28, 4);
                    width = infoheader.biWidth;
                    height = infoheader.biHeight;
                    imagesize = width * height * 3;
                    bitsperpixel = infoheader.biBitCount;
                    if (bitsperpixel < 16)
                        usedcolors = (int)infoheader.biClrUsed;
                    if (MemBits == null)
                    {
                        aresult = true;
                        return aresult;
                    }
                }
                // Obtain the DIBBits
                int y, scanwidth;
                int toread;
                byte divider;
                int origwidth;
                int acolor, numcolors;
                byte[] bufcolors;
                // Read color entries
                switch (bitsperpixel)
                {
                    case 1:
                        numcolors = 2;
                        break;
                    case 4:
                        numcolors = 16;
                        break;
                    case 8:
                        numcolors = 256;
                        break;
                    case 16:
                    case 15:
                    case 24:
                    case 32:
                        numcolors = 0;
                        break;
                    default:
                        throw new UnNamedException("Bad bitcount in GetBitmapInfo");
                }
                bool createIndexedSMap = false;
                if (numcolors > 0)
                {
                    if (iscoreheader)
                    {
                        usedcolors = numcolors;
                        bufcolors = new byte[usedcolors * 3];
                        readed = astream.Read(bufcolors, 0, usedcolors * 3);
                        if (readed != (usedcolors * 3))
                        {
                            throw new UnNamedException("Invalid bitmap palette");
                        }
                        palette = "";
                        for (y = 0; y < usedcolors; y++)
                        {
                            index = y * 3;
                            acolor = (((int)(bufcolors[index + 2])) << 16) + (((int)(bufcolors[index + 1])) << 8) +
                                (int)bufcolors[index];
                            if (palette == "")
                                palette = "<" + acolor.ToString("X").PadLeft(6, '0');
                            else
                                palette = palette + " " + acolor.ToString("X").PadLeft(6, '0');
                        }
                        if (usedcolors > 0)
                            palette = palette + ">";
                    }
                    else
                    {
                        if (usedcolors == 0)
                            usedcolors = numcolors;
                        bufcolors = new byte[usedcolors * 4];
                        readed = astream.Read(bufcolors, 0, usedcolors * 4);
                        if (readed != (usedcolors * 4))
                        {
                            throw new UnNamedException("Invalid bitmap palette");
                        }
                        palette = "";

                        for (y = 0; y < usedcolors; y++)
                        {
                            index = y * 4;
                            acolor = (((int)(bufcolors[index + 2])) << 16) + (((int)(bufcolors[index + 1])) << 8) +
                                (int)bufcolors[index];
                            if (palette == "")
                                palette = "<" + acolor.ToString("X").PadLeft(6, '0');
                            else
                                palette = palette + " " + acolor.ToString("X").PadLeft(6, '0');
                            byte transbyte = bufcolors[index + 3];
                            if ((transbyte != 255) || (transbyte != 0))
                            {
                                createIndexedSMap = true;
                            }
                            if (bufcolors[index + 3] == 0)
                            {
                                if (mask == "")
                                {
                                    mask = "[ ";
                                }
                                mask = mask + y.ToString() + " " + y.ToString();
                            }
                        }
                        if (usedcolors > 0)
                            palette = palette + ">";
                        if (mask.Length > 0)
                            mask = mask + " ]";
                    }
                }
                // Go to position bits
                astream.Seek(fileheader.bfOffBits, System.IO.SeekOrigin.Begin);
                if (numcolors > 0)
                {
                    switch (numcolors)
                    {
                        case 2:
                            divider = 8;
                            break;
                        case 16:
                            divider = 2;
                            break;
                        case 256:
                            divider = 1;
                            break;
                        default:
                            divider = 1;
                            break;
                    }
                    scanwidth = (int)width / divider;
                    indexed = true;
                    if ((width % divider) > 0)
                        scanwidth = scanwidth + 1;
                    // bitmap file format is aligned on double word
                    // the alignment must be removed from datafile
                    origwidth = scanwidth;
                    while ((scanwidth % 4) > 0)
                        scanwidth = scanwidth + 1;
                    buf = new byte[scanwidth];
                    byte[] bufMask = null;
                    if (createIndexedSMap)
                    {
                        bufMask = new byte[width];
                    }
                    MemBits.SetLength(height * origwidth);
                    MemBits.Seek(0, System.IO.SeekOrigin.Begin);

                    for (y = height - 1; y >= 0; y--)
                    {
                        astream.Read(buf, 0, scanwidth);
                        MemBits.Seek(y * origwidth, System.IO.SeekOrigin.Begin);
                        MemBits.Write(buf, 0, origwidth);
                        if (createIndexedSMap)
                        {

                        }
                    }
                    MemBits.Seek(0, System.IO.SeekOrigin.Begin);
                    imagesize = (int)MemBits.Length;
                    aresult = true;
                }
                else
                {

                    MemBits.SetLength(imagesize);
                    int module;
                    if (bitsperpixel == 32)
                    {
                        scanwidth = width * 4;
                        toread = 0;
                        module = 4;
                    }
                    else
                        if ((bitsperpixel == 16) || (bitsperpixel == 15))
                    {
                        scanwidth = width * 2;
                        toread = 0;
                        // Align to 32 bit
                        toread = 4 - (scanwidth % 4);
                        if (toread == 4)
                            toread = 0;
                        module = 2;
                    }
                    else
                    {
                        scanwidth = width * 3;
                        // Align to 32 bit
                        toread = 4 - (scanwidth % 4);
                        if (toread == 4)
                            toread = 0;
                        module = 3;
                    }
                    MemBits.Seek(0, System.IO.SeekOrigin.Begin);
                    scanwidth = scanwidth + toread;
                    buf = new byte[scanwidth];
                    int linewidth = width * 3;
                    byte[] bufdest = new byte[linewidth];
                    byte[] bufdestMask = null;
                    if ((bitsperpixel > 24) && (smask != null))
                        bufdestMask = new byte[width * height];
                    for (y = height - 1; y >= 0; y--)
                    {
                        readed = astream.Read(buf, 0, scanwidth);
                        if (readed != scanwidth)
                            throw new UnNamedException("Bad bitmap stream");
                        MemBits.Seek((width * 3) * y, System.IO.SeekOrigin.Begin);
                        if (bitsperpixel > 16)
                        {
                            for (int h = 0; h < width; h++)
                            {
                                bufdest[h * 3] = buf[module * h + 2];
                                bufdest[h * 3 + 1] = buf[module * h + 1];
                                bufdest[h * 3 + 2] = buf[module * h];
                                if ((bitsperpixel > 24) && (smask != null))
                                {
                                    bufdestMask[y * width + h] = buf[module * h + 3];
                                }
                            }
                        }
                        else
                        {
                            if (bitsperpixel == 15)
                            {
                                // 5-5-5
                                for (int h = 0; h < width; h++)
                                {
                                    ushort num = (ushort)buf[module * h];
                                    ushort num2 = (ushort)((ushort)buf[module * h + 1] << 8);
                                    num = (ushort)(num | num2);
                                    byte rcolor = (byte)(num & 0x1F);
                                    byte gcolor = (byte)((num & 0x3FF) >> 5);
                                    byte bcolor = (byte)((num & 0x7FFF) >> 10);
                                    rcolor = (byte)Math.Round((double)rcolor / 31 * 255);
                                    gcolor = (byte)Math.Round((double)gcolor / 31 * 255);
                                    bcolor = (byte)Math.Round((double)bcolor / 31 * 255);
                                    bufdest[h * 3] = bcolor;
                                    bufdest[h * 3 + 1] = gcolor;
                                    bufdest[h * 3 + 2] = rcolor;
                                }
                            }
                            else
                            {
                                // 5-6-5
                                for (int h = 0; h < width; h++)
                                {
                                    ushort num = (ushort)buf[module * h];
                                    ushort num2 = (ushort)((ushort)buf[module * h + 1] << 8);
                                    num = (ushort)(num | num2);
                                    byte rcolor = (byte)(num & 0x1F);
                                    byte gcolor = (byte)((num & 0x7FF) >> 5);
                                    byte bcolor = (byte)(num >> 11);
                                    rcolor = (byte)Math.Round((double)rcolor / 31 * 255);
                                    gcolor = (byte)Math.Round((double)gcolor / 63 * 255);
                                    bcolor = (byte)Math.Round((double)bcolor / 31 * 255);
                                    bufdest[h * 3] = bcolor;
                                    bufdest[h * 3 + 1] = gcolor;
                                    bufdest[h * 3 + 2] = rcolor;
                                }
                            }
                        }
                        MemBits.Write(bufdest, 0, linewidth);
                    }
                    if ((bitsperpixel > 24) && (smask != null))
                    {
                        smask.Write(bufdestMask, 0, bufdestMask.Length);
                    }
                    MemBits.Seek(0, System.IO.SeekOrigin.Begin);
                    aresult = true;
                }
                // Adobe PDF counts from 0 to usedcolors
                if (usedcolors > 0)
                    usedcolors--;
            }
            finally
            {
                if (newstream != null)
                    newstream.Dispose();
            }
            return aresult;
        }
        private struct BitmapFileHeader
        {
            public uint bfSize;
            public uint bfOffBits;
        }
        private struct BitmapInfoHeader
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biClrUsed;
        }
        private struct BitmapCoreHeader
        {
            public uint bcSize;
            public uint bcWidth;
            public uint bcHeight;
            public ushort bcPlanes;
            public ushort bcBitCount;
        }


        private const byte M_SOF0 = 0xC0;        // Start Of Frame N 
        private const byte M_SOF1 = 0xC1;        // N indicates which compression process 
        private const byte M_SOF2 = 0xC2;        // Only SOF0-SOF2 are now in common use 
        private const byte M_SOF3 = 0xC3;
        private const byte M_SOF5 = 0xC5;        // NB: codes C4 and CC are NOT SOF markers }
        private const byte M_SOF6 = 0xC6;
        private const byte M_SOF7 = 0xC7;
        private const byte M_SOF9 = 0xC9;
        private const byte M_SOF10 = 0xCA;
        private const byte M_SOF11 = 0xCB;
        private const byte M_SOF13 = 0xCD;
        private const byte M_SOF14 = 0xCE;
        private const byte M_SOF15 = 0xCF;
        private const byte M_SOI = 0xD8;        // (beginning of datastream) }
        private const byte M_EOI = 0xD9;       //{ (end of datastream) }
        private const byte M_SOS = 0xDA;        // (begins compressed data) }
        private const byte M_COM = 0xFE;        // Comment }

        private static byte NextMarker(Stream astream)
        {
            byte[] c1 = new byte[2];
            int readed;

            // Find 0xFF byte; count and skip any non-FFs. }
            readed = astream.Read(c1, 0, 1);
            if (readed < 1)
                throw new UnNamedException("Invalid JPEG");
            while (c1[0] != 0xFF)
            {
                readed = astream.Read(c1, 0, 1);
                if (readed < 1)
                    throw new UnNamedException("Invalid JPEG");
            }
            // Get marker code byte, swallowing any duplicate FF bytes.  Extra FFs
            // are legal as pad bytes, so don't count them in discarded_bytes. }
            do
            {
                readed = astream.Read(c1, 0, 1);
                if (readed < 1)
                    throw new UnNamedException("Invalid JPEG");
            } while (c1[0] == 0xFF);
            return c1[0];
        }
        // Skip over an unknown or uninteresting variable-length marker
        private static void skip_variable(Stream astream)
        {
            int alength, readed;
            byte[] c1 = new byte[2];

            //{ Get the marker parameter length count }
            readed = astream.Read(c1, 0, 2);
            if (readed < 2)
                throw new UnNamedException("Invalid JPEG");
            alength = ((int)c1[1]) + (((int)c1[0]) << 8);
            // Length includes itself, so must be at least 2 }
            if (alength < 2)
                throw new UnNamedException("Invalid JPEG");
            alength = alength - 2;
            // Skip over the remaining bytes }
            byte[] abuf = new byte[alength];
            readed = astream.Read(abuf, 0, alength);
            if (readed < alength)
                throw new UnNamedException("Invalid JPEG");
        }
        private static void process_SOFn(Stream astream, out int height,
            out int width)
        {
            int alength, readed;
            byte[] c1 = new byte[2];
            //{ Get the marker parameter length count }
            readed = astream.Read(c1, 0, 2);
            if (readed < 2)
                throw new UnNamedException("Invalid JPEG");
            // data_precission skiped
            readed = astream.Read(c1, 0, 1);
            if (readed < 1)
                throw new UnNamedException("Invalid JPEG");
            // Height
            readed = astream.Read(c1, 0, 2);
            if (readed < 2)
                throw new UnNamedException("Invalid JPEG");
            alength = ((int)c1[1]) + (((int)c1[0]) << 8);
            height = alength;
            // Width
            readed = astream.Read(c1, 0, 2);
            if (readed < 2)
                throw new UnNamedException("Invalid JPEG");
            alength = ((int)c1[1]) + (((int)c1[0]) << 8);
            width = alength;
        }
        public static string GetFileExtension(this System.Drawing.Imaging.ImageFormat imageFormat)
        {
            var extension = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()
                .Where(ie => ie.FormatID == imageFormat.Guid)
                .Select(ie => ie.FilenameExtension
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .First()
                    .Trim('*')
                    .ToLower())
                .FirstOrDefault();

            return extension ?? string.Format(".{0}", imageFormat.ToString().ToLower());
        }

    }
}
