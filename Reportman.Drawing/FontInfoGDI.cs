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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace Reportman.Drawing
{

    class FontInfoGDI : FontInfoProvider, IDisposable
    {
        const int TTF_PRECISION = 1000;
        private Bitmap FBitmap;
        private Graphics gr;
        private static object tflag = 2;
        private SortedList Fonts;
        private Font font;
        private FontFamily family;
        private FontStyle fontstyle;
        private float dpix;
        private string currentfont;
        public static SortedList<string, SortedList<ulong, int>> KerningsCache;
        public static SortedList<string, SortedList<char, GlyphInfo>> WidthsCache;

        private IntPtr hDC;
        public FontInfoGDI()
        {
            currentfont = "";
            FBitmap = new Bitmap(100, 100, PixelFormat.Format32bppArgb);
            dpix = FBitmap.HorizontalResolution;
            gr = Graphics.FromImage(FBitmap);
            hDC = gr.GetHdc();
            Fonts = new SortedList();
        }
        public override MemoryStream GetFontStream(TTFontData data)
        {
            //    return new MemoryStream(data.FontData.Data);
            Dictionary<int, int[]> glyps = new Dictionary<int, int[]>();
            foreach (char xchar in data.Glyphs.Keys)
            {
                int gl = data.Glyphs[xchar];
                int width = data.Widths[xchar];
                if (!glyps.ContainsKey(gl))
                    glyps[gl] = new int[] { gl, width, (int)xchar };
            }
            TrueTypeFontSubSet subset = new TrueTypeFontSubSet(data.PostcriptName, data.FontData.Data,
                glyps, data.FontData.DirectoryOffset);
            byte[] nresult = subset.Execute();

            return new MemoryStream(nresult);
        }

        public void SelectFont(PDFFont pdffont)
        {
            string fontname = "";
            if (pdffont.Italic)
                fontname = fontname + "I";
            else
                fontname = fontname + "i";
            if (pdffont.Bold)
                fontname = fontname + "B";
            else
                fontname = fontname + "b";
            fontname = fontname + "_____" + pdffont.WFontName;
            if (currentfont == fontname)
                return;
            currentfont = fontname;
            int index = Fonts.IndexOfKey(fontname);
            if (index >= 0)
                font = (Font)Fonts[fontname];
            else
            {
                fontstyle = System.Drawing.FontStyle.Regular;
                if (pdffont.Italic)
                    fontstyle = fontstyle | System.Drawing.FontStyle.Italic;
                if (pdffont.Bold)
                    fontstyle = fontstyle | System.Drawing.FontStyle.Bold;

                Font afont = new Font(pdffont.WFontName, TTF_PRECISION, fontstyle, System.Drawing.GraphicsUnit.Point);
                family = afont.FontFamily;
                Fonts.Add(fontname, afont);
                font = afont;
            }
        }
        public void Dispose()
        {
            gr.ReleaseHdc();

        }
        [DllImport("gdi32.dll")]
        static extern uint GetFontData(IntPtr hdc, uint dwTable, uint dwOffset,
           [Out] byte[] lpvBuffer, uint cbData);

        public void ReadFontData(TTFontData fontData)
        {
            Monitor.Enter(tflag);
            try
            {
                string nfam = fontData.FontFamily.ToUpper();
                if (fontData.IsBold)
                    nfam = nfam + "__b__";
                if (fontData.IsItalic)
                    nfam = nfam + "__i__";
                if (TTFontData.FontDatas == null)
                    TTFontData.FontDatas = new SortedList<string, AdvFontData>();
                if (TTFontData.FontDatas.IndexOfKey(nfam) >= 0)
                {
                    fontData.FontData = TTFontData.FontDatas[nfam];
                }
                else
                {
                    uint directoryOffset = 0;
                    byte[] fontCollectionBuffer = new byte[4];
                    uint dwTable = 0x66637474;
                    uint asize = GetFontData(hDC, 0x66637474, 0,
                        fontCollectionBuffer, 4);
                    string header = System.Text.ASCIIEncoding.ASCII.GetString(fontCollectionBuffer);
                    if (header != "ttcf")
                    {
                        dwTable = 0;
                    }
                    asize = GetFontData(hDC, dwTable, 0,
                        null, 0);
                    // uint asize = GetFontData(hDC, 0, 0,
                    //     null, 0);
                    //if ((asizeFull - asize)>0)
                    //{
                    //    directoryOffset = asizeFull - asize;
                    //}
                    if (asize > 0)
                    {
                        // Gets the raw data of the font
                        byte[] abyte = new byte[asize];
                        fontData.FontData = new AdvFontData();
                        fontData.FontData.DirectoryOffset = directoryOffset;
                        uint aresult = GetFontData(hDC, dwTable, 0, abyte, asize);
                        //if GDI_ERROR=GetFontData(adc,0,0,data.FontData.Memory,asize) then
                        //    RaiseLastOSError;
                        fontData.FontData.Data = abyte;
                        TTFontData.FontDatas.Add(nfam, fontData.FontData);
                    }
                }
            }
            finally
            {
                Monitor.Exit(tflag);
            }
        }
        public override void FillFontData(PDFFont pdfFont, TTFontData fontData)
        {
            SelectFont(pdfFont);
            fontData.PostcriptName = pdfFont.WFontName.Replace(" ", "");
            if (pdfFont.Bold)
            {
                if (pdfFont.Italic)
                {
                    fontData.PostcriptName = fontData.PostcriptName + ",BoldItalic";
                }
                else
                {
                    fontData.PostcriptName = fontData.PostcriptName + ",Bold";
                }
                fontData.IsBold = true;
            }
            else
            {
                if (pdfFont.Italic)
                {
                    fontData.PostcriptName = fontData.PostcriptName + ",Italic";
                    fontData.IsItalic = true;
                }
            }
            fontData.FontFamily = pdfFont.WFontName;
            fontData.Type1 = false;
            fontData.IsUnicode = true;
            fontData.FontStretch = "/Normal";
            fontData.Encoding = "WinAnsiEncoding";
            fontData.HaveKerning = false;
            fontData.EmHeight = family.GetEmHeight(fontstyle);
            fontData.LineSpacing = (int)Math.Round(1000.0 * family.GetLineSpacing(fontstyle) / fontData.EmHeight);
            fontData.Ascent = (int)Math.Round(1000.0 * family.GetCellAscent(fontstyle) / fontData.EmHeight);
            fontData.Descent = -(int)Math.Round(1000.0 * family.GetCellDescent(fontstyle) / fontData.EmHeight);
            fontData.Leading = fontData.LineSpacing - fontData.Ascent + fontData.Descent;



            //double multipli=(double)1.0*72/(double)dpix*(double)0.72);
            //data.FontWeight:=potm^.otmTextMetrics.tmWeight;
            //dataData.FontBBox.Left = Math.Round(data.FontBBox.Left*multipli);
            //fontData.FontBBox.Right = Round(data.FontBBox.Right*multipli);
            //data.FontBBox.Bottom:=Round(data.FontBBox.Bottom*multipli);
            //data.FontBBox.Top:=Round(data.FontBBox.Top*multipli);

            Font ft = (Font)font.Clone();
            IntPtr hFt = ft.ToHfont();
            SelectObject(hDC, hFt);
            if (fontData.Embedded)
            {
                ReadFontData(fontData);
            }
            // Assign widths list
            Monitor.Enter(tflag);
            try
            {
                if (WidthsCache == null)
                    WidthsCache = new SortedList<string, SortedList<char, GlyphInfo>>();
                if (WidthsCache.IndexOfKey(fontData.PostcriptName) < 0)
                {
                    SortedList<char, GlyphInfo> nlist = new SortedList<char, GlyphInfo>();
                    WidthsCache.Add(fontData.PostcriptName, nlist);
                    fontData.CacheWidths = nlist;
                }
                else
                    fontData.CacheWidths = WidthsCache[fontData.PostcriptName];
            }
            finally
            {
                Monitor.Exit(tflag);
            }



            GcpFlags nflags = GetFontLanguageInfo(hDC);
            fontData.HaveKerning = (nflags & GcpFlags.UseKerning) > 0;
            if (fontData.HaveKerning)
            {
                Monitor.Enter(tflag);
                try
                {
                    if (KerningsCache == null)
                        KerningsCache = new SortedList<string, SortedList<ulong, int>>();
                    if (KerningsCache.IndexOfKey(fontData.PostcriptName) >= 0)
                        fontData.Kernings = KerningsCache[fontData.PostcriptName];
                    else
                    {
                        uint MAX_KER = 50000;
                        SortedList<ulong, int> nkernin = new SortedList<ulong, int>();
                        KERNINGPAIR[] kerarray = new KERNINGPAIR[MAX_KER];
                        GCHandle kerHnd = GCHandle.Alloc(kerarray, GCHandleType.Pinned);
                        try
                        {
                            uint numkernings = GetKerningPairs(hDC, MAX_KER, kerHnd.AddrOfPinnedObject());
                            for (uint i = 0; i < numkernings; i++)
                            {
                                //string nkey = kerarray[i].wFirst.ToString("00000") +
                                //    kerarray[i].wSecond.ToString("00000");
                                ulong nkey = (ulong)(kerarray[i].wFirst << 32) + (ulong)kerarray[i].wSecond;
                                if (nkernin.IndexOfKey(nkey) < 0)
                                {
                                    int amount = (int)Math.Round((double)-kerarray[i].iKernAmount / dpix * 72);
                                    nkernin.Add(nkey, amount);
                                }
                            }

                            // Cache kernings
                            KerningsCache.Add(fontData.PostcriptName, nkernin);

                        }
                        finally
                        {
                            kerHnd.Free();

                        }
                        fontData.Kernings = nkernin;
                    }
                }
                finally
                {
                    Monitor.Exit(tflag);
                }
            }
            DeleteObject(hFt);
        }
        [DllImport("gdi32.dll")]
        //        static extern uint GetKerningPairs(IntPtr hdc, uint nNumPairs,
        //            [Out] KERNINGPAIR[] lpkrnpair);
        static extern uint GetKerningPairs(IntPtr hdc, uint nNumPairs,
                    IntPtr lpkrnpair);
        [StructLayout(LayoutKind.Sequential)]
        struct KERNINGPAIR
        {
            public ushort wFirst; // might be better off defined as char
            public ushort wSecond; // might be better off defined as char
            public int iKernAmount;

            public KERNINGPAIR(ushort wFirst, ushort wSecond, int iKernAmount)
            {
                this.wFirst = wFirst;
                this.wSecond = wSecond;
                this.iKernAmount = iKernAmount;
            }

            public override string ToString()
            {
                return (String.Format("{{First={0}, Second={1}, Amount={2}}}", wFirst, wSecond, iKernAmount));
            }
        }

        [Flags]
        public enum GcpFlags    // Win32: GCP_xxx flags, WinGDI.h 
        {
            DBCS = 0x00000001,
            ReOrder = 0x00000002,
            UseKerning = 0x00000008,
            GlyphShape = 0x00000010,
            Ligate = 0x00000020,
            Diacritic = 0x00000100,
            Kashida = 0x00000400,
            Error = 0x00008000,
            Justify = 0x00010000,
            FliGlyphs = 0x00040000,
            ClassIn = 0x00080000,
            MaxExtent = 0x00100000,
            JustifyIn = 0x00200000,
            DisplayZWG = 0x00400000,
            SymSwapOff = 0x00800000,
            NumericOverride = 0x01000000,
            NeutralOverride = 0x02000000,
            NumericsLatin = 0x04000000,
            NumericsLocal = 0x08000000,


            FliMask = 0x0000103B
        }


        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetCharacterPlacementW(IntPtr dev, string text, int count, int max,
            [In, Out] ref GpcResults results, GcpFlags flags);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct GpcResults   // Win32: GCP_RESULTS 
        {
            public int StructSize;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string OutString;
            public IntPtr Order;
            public IntPtr Dx;
            public IntPtr CaretPos;
            public IntPtr Class;
            public IntPtr Glyphs;
            public int GlyphCount;
            public int MaxFit;
        }
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern GcpFlags GetFontLanguageInfo(IntPtr dev);

        [StructLayout(LayoutKind.Sequential)]
        public struct ABC
        {
            public int abcA;
            public uint abcB;
            public int abcC;
        }
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int DeleteObject(IntPtr hObj);

        [DllImport("gdi32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool GetCharABCWidthsW(IntPtr hdc, uint uFirstChar, uint uLastChar, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct, SizeConst = 1)] ABC[] lpabc);
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        static extern uint GetGlyphIndicesW(IntPtr hdc, string lpsz, int c,
           [Out] ushort[] pgi, uint fl);
        private ABC GetCharWidthABC(char ch, Font font, Graphics gr, ref int glyphindex, ref bool glylphsupported)
        {
            glylphsupported = true;
            ABC[] _temp = new ABC[1];
            Font ft = (Font)font.Clone();
            IntPtr hFt = ft.ToHfont();
            SelectObject(hDC, hFt);
            GetCharABCWidthsW(hDC, ch, ch, _temp);
            ushort[] glys = new ushort[1];
            // flag GGI_MARK_NONEXISTING_GLYPHS
            uint numchars = GetGlyphIndicesW(hDC, "" + ch, 1, glys, 1);
            glyphindex = -1;
            if (numchars > 0)
            {
                glyphindex = glys[0];
                if (glyphindex == 0xFFFF)
                {
                    GetGlyphIndicesW(hDC, "?", 1, glys, 1);
                    GetCharABCWidthsW(hDC, '?', '?', _temp);
                    glyphindex = glys[0];
                    glylphsupported = false;
                }
            }

            DeleteObject(hFt);

            /*            short[] glys = new short[1]; 


                        GpcResults gcp = new GpcResults();

                       GCHandle glyHnd = GCHandle.Alloc( glys, GCHandleType.Pinned );
                       try
                       {

                           gcp.StructSize = Marshal.SizeOf(typeof(GpcResults));
                           gcp.OutString = new String('\0', 1 + 2);
                           gcp.Order = (System.IntPtr)null; ;
                           gcp.Dx = (System.IntPtr)null; ;
                           gcp.CaretPos = (System.IntPtr)null; ;
                           gcp.Class = (System.IntPtr)null; ;
                           gcp.Glyphs = glyHnd.AddrOfPinnedObject(); ;
                           gcp.GlyphCount = 1;
                           gcp.MaxFit = 0;
                           string astring = "" + ch + (char)0;
                           nresult = GetCharacterPlacementW(hDC, astring, (int)1, (int)0, ref gcp, GcpFlags.Diacritic);
                           if (nresult == 0)  //  0:Error;  OK e.g. 0x100085 
                           {
                               int err = Marshal.GetLastWin32Error();
                               throw new Exception("Call to GetCharacterPlacement failed, error code:" + err.ToString());
                               // todo: error handling 
                           } 

                           DeleteObject(hFt);
                       }
                       finally
                       {
                           glyHnd.Free();
                       }
                        glyphindex = glys[0];
            */
            return _temp[0];
        }
        public override int GetCharWidth(PDFFont pdffont, TTFontData fontData,
            char charcode)
        {
            SelectFont(pdffont);
            int index = fontData.Widths.IndexOfKey(charcode);
            if (index >= 0)
            {
                return (int)fontData.Widths[charcode];
            }
            /*                     string text = "" + charcode+"x";
                                 System.Drawing.StringFormat format = new System.Drawing.StringFormat();
                                 System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0,
                                                                                      10000, 10000);
                                 System.Drawing.CharacterRange[] ranges =
                                                             { new System.Drawing.CharacterRange(0,
                                                                                     text.Length) };
                                 System.Drawing.Region[] regions = new System.Drawing.Region[1];


                                 format.SetMeasurableCharacterRanges(ranges);
                                 regions = gr.MeasureCharacterRanges(text, font, rect, format);
                                 rect = regions[0].GetBounds(gr);
                                 int newwidth=(int)Math.Round(rect.Width*72/dpix*0.72);*/
            int newwidth = 0;
            int glyphindex = 0;
            bool glyphsupported = true;
            //            if (System.Environment.OSVersion.Platform != PlatformID.Unix)
            //            {
            if (fontData.CacheWidths.IndexOfKey(charcode) >= 0)
            {
                GlyphInfo ninfo = fontData.CacheWidths[charcode];
                newwidth = ninfo.Width;
                glyphindex = ninfo.GlyphIndex;
            }
            else
            {
                Monitor.Enter(tflag);
                try
                {
                    ABC nresult = GetCharWidthABC(charcode, font, gr, ref glyphindex, ref glyphsupported);
                    newwidth = (int)Math.Round((double)(nresult.abcA + nresult.abcB + nresult.abcC) / dpix * 72);
                    GlyphInfo ninfo = new GlyphInfo();
                    ninfo.GlyphIndex = glyphindex;
                    ninfo.Width = newwidth;
                    if (fontData.CacheWidths.IndexOfKey(charcode) < 0)
                    {
                        fontData.CacheWidths.Add(charcode, ninfo);
                    }
                }
                finally
                {
                    Monitor.Exit(tflag);
                }
            }
            //            }
            if (glyphsupported)
            {
                fontData.Glyphs[charcode] = glyphindex;
            }
            fontData.Widths.Add(charcode, newwidth);
            int aint = (int)charcode;
            if (fontData.FirstLoaded > aint)
                fontData.FirstLoaded = aint;
            if (fontData.LastLoaded < aint)
                fontData.LastLoaded = aint;
            return newwidth;
        }
        public override int GetKerning(PDFFont pdffont, TTFontData fontData,
            char leftChar, char rightChar)
        {
            if (fontData.Kernings == null)
                return 0;
            ulong nkey = (ulong)((int)leftChar << 32) + (ulong)rightChar;
            if (fontData.Kernings.IndexOfKey(nkey) >= 0)
                return fontData.Kernings[nkey];
            else
                return 0;
        }

    }
}