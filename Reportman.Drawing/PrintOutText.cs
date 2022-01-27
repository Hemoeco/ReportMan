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
using System.Drawing;


namespace Reportman.Drawing
{
    public class PrintOutText : PrintOut, IDisposable
    {
        SortedList<PrintStepType, PrintStepType> allowedsteps;
        private int FLinesPerInch;
        private StringBuilder currpage;
        private bool FDrawerBefore;
        private bool FDrawerAfter;
        public int FPageWidth;
        private int PageQt;
        public int FPageHeight;
        public const int DEFAULT_LINESPERINCH = 6;
        private List<PrintLine> Lines;
        private PrinterSelectType FPrinterSelect;
        public string ForceDriverName;
        public string FPrinterDriver;
        private bool LoadOEMConvert;
        private bool OEMConvert;
        public bool FullPlain;
        public int blacklines;
        public int whitelines;
        private PrintLine PreviousLine;
        private LineInfos linfos;
        bool masterselect;
        bool limitedmaster;
        bool condensedmaster;
        public StringBuilder PrintResult;
        private SortedList<PrinterRawOperation, string> escapecodes;
        byte master10;
        byte master12;
        byte mastercond;
        byte masterwide;
        int linefeeds;
        StringBuilder FResult;
        /// <summary>
        /// Constructo and initialization
        /// </summary>
        public PrintOutText()
            : base()
        {
            PrintResult = new StringBuilder();
            linfos = new LineInfos();
            FLinesPerInch = DEFAULT_LINESPERINCH;
            Lines = new List<PrintLine>();
            ForceDriverName = "";
            OEMConvert = false;
            FullPlain = false;
            LoadOEMConvert = true;
            FPrinterDriver = "PLAIN";
            escapecodes = new SortedList<PrinterRawOperation, string>();
            master10 = 0;
            master12 = 1;
            mastercond = 4;
            masterwide = 32;
            linefeeds = 0;
        }
        public string GetEscape(PrinterRawOperation op)
        {
            int index = escapecodes.IndexOfKey(op);
            if (index >= 0)
                return escapecodes[op];
            else
                return "";
        }
        /// <summary>
        /// Draw all objects of the page to current PDF file page
        /// </summary>
        /// <param name="meta">MetaFile containing the page</param>
        /// <param name="page">MetaPage to be drawn</param>
        override public void DrawPage(MetaFile meta, MetaPage page)
        {
            for (int i = 0; i < page.Objects.Count; i++)
            {
                DrawObject(meta, page, page.Objects[i]);
            }

        }
        public void DrawObject(MetaFile meta, MetaPage page, MetaObject aobj)
        {
            int posx, posy;
            Rectangle rec;
            int aalign;
            string astring;
            PrintStepType fontstep;
            bool red;
            posx = aobj.Left;
            posy = aobj.Top;
            switch (aobj.MetaType)
            {
                case MetaObjectType.Text:
                    MetaObjectText obj = (MetaObjectText)aobj;
                    aalign = obj.Alignment;
                    rec = new Rectangle(posx, posy, obj.Width, obj.Height);
                    astring = page.GetText(obj);
                    fontstep = FontSizeToStep(obj.FontSize, obj.PrintStep);
                    red = GraphicUtils.ColorFromInteger(obj.FontColor) == Color.Red;
                    TextRect(rec, astring, aalign, obj.CutText,
                    obj.WordWrap, obj.RightToLeft, fontstep, obj.FontStyle, red);

                    break;
            }
        }
        public void TextRect(Rectangle arect, string text, int alignment, bool clipping,
            bool wordbreak, bool righttoleft, PrintStepType fontstep, int fontstyle, bool red)
        {
            text = text.Replace("" + (char)13 + (char)10, "" + (char)10);
            Rectangle recsize;
            int posx, posy;
            bool singleline;
            string astring = "";
            Strings lwords, lwidths;
            int alinesize, alinedif, currpos;
            Rectangle arec;

            singleline = (alignment & MetaFile.AlignmentFlags_SingleLine) > 0;
            if (singleline)
                wordbreak = false;
            recsize = arect;
            CalculateTextExtent(text, ref recsize, wordbreak, singleline, fontstep, clipping);
            posy = arect.Top;
            if ((alignment & MetaFile.AlignmentFlags_AlignBottom) > 0)
                posy = arect.Bottom - recsize.Height;
            if ((alignment & MetaFile.AlignmentFlags_AlignVCenter) > 0)
                posy = arect.Top + (((arect.Bottom - arect.Top) - recsize.Height) / 2);
            for (int i = 0; i < linfos.Count; i++)
            {
                posx = arect.Left;
                LineInfo linfo = linfos[i];
                if ((alignment & MetaFile.AlignmentFlags_AlignRight) > 0)
                    posx = arect.Right - linfo.Width;
                // Align horz
                if ((alignment & MetaFile.AlignmentFlags_AlignHCenter) > 0)
                    posx = arect.Left + (((arect.Right - arect.Left) - linfo.Width) / 2);
                if (linfo.Size <= 0)
                    astring = "";
                else
                    astring = text.Substring(linfo.Position, linfo.Size);
                // Justify
                if ((alignment & MetaFile.AlignmentFlags_AlignHJustify) > 0)
                {
                    lwords = Strings.FromSeparator(' ', astring);
                    lwords.RemoveBlanks();
                    alinesize = 0;
                    lwidths = new Strings();
                    foreach (string nword in lwords)
                    {
                        arec = new Rectangle(0, arect.Top, (int)Math.Round(nword.Length * StepToTwips(fontstep)), arect.Height);
                        if (righttoleft)
                            lwidths.Add((-(arec.Right - arec.Left)).ToString());
                        else
                            lwidths.Add((arec.Right - arec.Left).ToString());
                        alinesize = alinesize + (arec.Right - arec.Left);
                    }
                    alinedif = arect.Right - arect.Left - alinesize;
                    if (alinedif > 0)
                    {
                        if (lwords.Count > 0)
                            alinedif = alinedif / (lwords.Count - 1);
                        if (righttoleft)
                        {
                            currpos = arect.Right;
                            alinedif = -alinedif;
                        }
                        else
                            currpos = posx;
                        int idx = 0;
                        foreach (string nword in lwords)
                        {
                            DoTextOut(currpos, posx + linfo.TopPos, nword,
                                linfo.Width, fontstep, righttoleft, fontstyle, red);
                            currpos = currpos + System.Convert.ToInt32(lwidths[idx]) + alinedif;

                            idx++;
                        }
                    }
                    else
                        DoTextOut(posx, posy + linfo.TopPos, astring, linfo.Width, fontstep, righttoleft, fontstyle, red);
                }
                else
                    DoTextOut(posx, posy + linfo.TopPos, astring, linfo.Width, fontstep, righttoleft, fontstyle, red);

            }
        }
        public int GetLineIndex(int posy)
        {
            int amax;
            int nresult = 0;
            if (FPageHeight <= 0)
                return nresult;
            amax = Lines.Count;
            // Lines go on base 0 that is from 0 to 65 in a 66 line paper
            nresult = (int)Math.Round(System.Convert.ToDouble(posy) / FPageHeight * (amax));
            if (nresult < 0)
                nresult = 0;
            if (nresult > amax)
                nresult = amax;
            return nresult;
        }
        public int GetColumnNumber(int posx, PrintStepType fontstep)
        {
            int nresult = (int)Math.Round(System.Convert.ToDouble(posx) / StepToTwips(fontstep));
            if (nresult < 0)
                nresult = 0;
            return nresult;
        }
        public string GetBlankLine(PrintStepType fontstep)
        {
            StringBuilder nresult = new StringBuilder();
            int charcount = (int)Math.Round(System.Convert.ToDouble(FPageWidth) / StepToTwips(fontstep));
            for (int i = 0; i < charcount; i++)
                nresult.Append(" ");
            return nresult.ToString();
        }
        public void DoTextOut(int x, int y, string text, int linewidth, PrintStepType fontstep, bool rightoleft, int fontstyle, bool red)
        {
            string astring;
            int atpos;
            int lineindex, index;
            int columnnumber;
            int toposition;

            astring = text;
            if (rightoleft)
                astring = PDFCanvas.DoReverseString(astring);
            lineindex = GetLineIndex(y);
            PrintLine nline = Lines[lineindex];
            if (nline.texts.Count < 1)
            {
                nline.FontStep = fontstep;
                nline.Value = GetBlankLine(fontstep);
            }
            PosText npostext = new PosText();
            columnnumber = GetColumnNumber(x, nline.FontStep);
            atpos = columnnumber;
            index = nline.texts.IndexOfKey(atpos);
            bool doinsert = false;
            if (index < 0)
            {
                npostext = new PosText();
                npostext.bold = ((fontstyle & 1) > 0);
                npostext.underline = ((fontstyle & 4) > 0);
                npostext.italic = ((fontstyle & 2) > 0);
                npostext.strokeout = ((fontstyle & 8) > 0);
                npostext.red = red;
                npostext.regular = (!npostext.bold) && (!npostext.strokeout) && (!npostext.italic) &&
                    (!npostext.strokeout);
                npostext.Position = columnnumber;
                doinsert = true;
            }
            else
            {
                npostext = nline.texts[atpos];
            }
            toposition = astring.Length;
            // Trunc strings
            if (columnnumber + toposition >= nline.Value.Length)
                toposition = nline.Value.Length - columnnumber - 1;
            npostext.nSize = toposition;
            string oldvalue = nline.Value;
            nline.Value = "";
            if (columnnumber > 0)
            {
                if (columnnumber > oldvalue.Length)
                {
                    nline.Value = oldvalue;
                    return;
                }
                else
                    nline.Value = oldvalue.Substring(0, columnnumber);
            }
            nline.Value = nline.Value + astring.Substring(0, toposition);
            if (nline.Value.Length < oldvalue.Length)
                nline.Value = nline.Value + oldvalue.Substring(nline.Value.Length, oldvalue.Length - nline.Value.Length);
            if (doinsert)
                nline.texts.Add(atpos, npostext);
            else
                nline.texts[atpos] = npostext;
        }
        /// <summary>
        /// The driver should do initialization here, a print driver should start a print document,
        /// while a preview driver should initialize a bitmap
        /// </summary>
        public override void NewDocument(MetaFile meta)
        {
            PrintResult = new StringBuilder();
            whitelines = 0;
            blacklines = 0;
            PreviousLine = null;
            FLinesPerInch = meta.LinesPerInch;
            FDrawerBefore = meta.OpenDrawerBefore;
            FDrawerAfter = meta.OpenDrawerAfter;
            FPageWidth = meta.CustomX;
            FPageHeight = meta.CustomY;
            FResult = new StringBuilder();
            FPrinterSelect = meta.PrinterSelect;
            UpdatePrinterConfig();

            RecalcSize();

        }
        public enum OemConvertOverride { None, False, True };

        public OemConvertOverride OverrideOemConvert = OemConvertOverride.None;

        private void UpdatePrinterConfig()
        {
            if (ForceDriverName.Length == 0)
            {
                FPrinterDriver = PrinterConfig.GetDriverName(FPrinterSelect);
            }
            else
                FPrinterDriver = ForceDriverName;
            if (OverrideOemConvert != OemConvertOverride.None)
            {
                if (OverrideOemConvert == OemConvertOverride.False)
                    OEMConvert = false;
                else
                    OEMConvert = true;
            }
            else
            if (LoadOEMConvert)
            {
                OEMConvert = PrinterConfig.GetOEMConvert(FPrinterSelect);
            }

            allowedsteps = new SortedList<PrintStepType, PrintStepType>
            {
                { PrintStepType.cpi5, PrintStepType.cpi10 },
                { PrintStepType.cpi6, PrintStepType.cpi10 },
                { PrintStepType.cpi10, PrintStepType.cpi10 },
                { PrintStepType.cpi12, PrintStepType.cpi10 },
                { PrintStepType.cpi15, PrintStepType.cpi10 },
                { PrintStepType.cpi17, PrintStepType.cpi10 },
                { PrintStepType.cpi20, PrintStepType.cpi10 }
            };

            FillEscapes();

        }
        public void FillEscapes()
        {
            Type rtype = typeof(PrinterRawOperation);


            escapecodes.Clear();
            foreach (string s in Enum.GetNames(rtype))
            {
                escapecodes.Add((PrinterRawOperation)Enum.Parse(rtype, s), "");
            }
            masterselect = false;
            limitedmaster = false;
            condensedmaster = false;

            switch (FPrinterDriver)
            {
                case "EPSON":
                    // Init Printer-Line spacing to 1/6
                    escapecodes[PrinterRawOperation.InitPrinter] = "" + (char)27 + (char)64;
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.LineSpace6] = "" + (char)27 + "2";
                    escapecodes[PrinterRawOperation.LineSpace8] = "" + (char)27 + "0";
                    escapecodes[PrinterRawOperation.LineSpace7_72] = "" + (char)27 + "1";
                    escapecodes[PrinterRawOperation.LineSpacen_216] = "" + (char)27 + "3";
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    escapecodes[PrinterRawOperation.FF] = "" + (char)12;
                    escapecodes[PrinterRawOperation.Underline] = "" + (char)27 + (char)45 + (char)1;
                    // Underline off-Bold off-Italic off
                    escapecodes[PrinterRawOperation.Normal] = "" + (char)27 + (char)45 + (char)0;
                    // Set 10 or 12 cpi, enabled-disable double wide, enable-disable condensed
                    escapecodes[PrinterRawOperation.cpi5] = "" + (char)18 + (char)14;
                    escapecodes[PrinterRawOperation.cpi10] = "" + (char)20 + (char)18;
                    escapecodes[PrinterRawOperation.cpi17] = "" + (char)20 + (char)15;
                    escapecodes[PrinterRawOperation.EndPrint] = "" + (char)27 + (char)64;


                    // Open drawer
                    escapecodes[PrinterRawOperation.Pulse] = "" + (char)27 + (char)112 + (char)0 + (char)100 + (char)100;
                    break;
                case "EPSON-MASTER":
                    // Init Printer-Line spacing to 1/6 - Draft mode
                    escapecodes[PrinterRawOperation.InitPrinter] = "" + (char)27 + (char)64 + (char)27 + "x" + "0";
                    //har)27+"["+"4"+"q";
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.LineSpace6] = "" + (char)27 + "2";
                    escapecodes[PrinterRawOperation.LineSpace8] = "" + (char)27 + "0";
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    escapecodes[PrinterRawOperation.FF] = "" + (char)12;
                    masterselect = true;
                    limitedmaster = false;
                    condensedmaster = false;

                    // Open drawer
                    escapecodes[PrinterRawOperation.Pulse] = "" + (char)27 + (char)112 + (char)0 + (char)100 + (char)100;
                    escapecodes[PrinterRawOperation.EndPrint] = "" + (char)27 + (char)64;
                    break;
                case "EPSON-ESCP":
                    // Init Printer-Line spacing to 1/6 - Draft mode - Bidirectional print
                    escapecodes[PrinterRawOperation.InitPrinter] = "" + (char)27 + (char)64 + (char)27 + "x" + "1" + (char)27 + "U" + (char)0;
                    escapecodes[PrinterRawOperation.LineSpace6] = "" + (char)27 + "2";
                    escapecodes[PrinterRawOperation.LineSpace7_72] = "" + (char)27 + "1";
                    escapecodes[PrinterRawOperation.LineSpace8] = "" + (char)27 + "0";
                    escapecodes[PrinterRawOperation.LineSpacen_216] = "" + (char)27 + "3";
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    escapecodes[PrinterRawOperation.FF] = "" + (char)12;
                    escapecodes[PrinterRawOperation.Bold] = "" + (char)27 + "E";
                    escapecodes[PrinterRawOperation.Underline] = "" + (char)27 + (char)45 + (char)1;
                    escapecodes[PrinterRawOperation.Italic] = "" + (char)27 + "4";
                    // Underline off-Bold off-Italic off
                    escapecodes[PrinterRawOperation.Normal] = "" + (char)27 + (char)45 + (char)0 +
                     (char)27 + "F" + (char)27 + "5";
                    // Set 10 or 12 cpi, enabled-disable double wide, enable-disable condensed
                    escapecodes[PrinterRawOperation.cpi5] = "" + (char)27 + "P" + (char)14 + (char)18;
                    escapecodes[PrinterRawOperation.cpi6] = "" + (char)27 + "M" + (char)14 + (char)18;
                    escapecodes[PrinterRawOperation.cpi10] = "" + (char)27 + "P" + (char)20 + (char)18;
                    escapecodes[PrinterRawOperation.cpi12] = "" + (char)27 + "M" + (char)20 + (char)18;
                    // 15 cpi not supported in LX models
                    //  escapecodes[rpescape15cpi]=""+(char)27+"g"+(char)20;
                    escapecodes[PrinterRawOperation.cpi17] = "" + (char)27 + "P" + (char)20 + (char)15;
                    escapecodes[PrinterRawOperation.cpi20] = "" + (char)27 + "M" + (char)20 + (char)15;

                    escapecodes[PrinterRawOperation.EndPrint] = "" + (char)27 + (char)64;

                    // Open drawer
                    escapecodes[PrinterRawOperation.Pulse] = "" + (char)27 + (char)112 + (char)0 + (char)100 + (char)100;
                    break;
                case "EPSON-ESCPQ":
                    // Init Printer-Line spacing to 1/6 - Draft mode - Bidirectional print
                    escapecodes[PrinterRawOperation.InitPrinter] = "" + (char)27 + (char)64 + (char)27 + "x" + "1" + (char)27 + "U" + (char)0;
                    escapecodes[PrinterRawOperation.LineSpace6] = "" + (char)27 + "2";
                    escapecodes[PrinterRawOperation.LineSpace8] = "" + (char)27 + "0";
                    escapecodes[PrinterRawOperation.LineSpacen_180] = "" + (char)27 + "3";
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    escapecodes[PrinterRawOperation.FF] = "" + (char)12;
                    escapecodes[PrinterRawOperation.Bold] = "" + (char)27 + "E";
                    escapecodes[PrinterRawOperation.Underline] = "" + (char)27 + (char)45 + (char)1;
                    escapecodes[PrinterRawOperation.Italic] = "" + (char)27 + "4";
                    // Underline off-Bold off-Italic off
                    escapecodes[PrinterRawOperation.Normal] = "" + (char)27 + (char)45 + (char)0 +
                     (char)27 + "F" + (char)27 + "5";
                    // Set 10 or 12 cpi, enabled-disable double wide, enable-disable condensed
                    escapecodes[PrinterRawOperation.cpi5] = "" + (char)27 + "P" + (char)14 + (char)18;
                    escapecodes[PrinterRawOperation.cpi6] = "" + (char)27 + "M" + (char)14 + (char)18;
                    escapecodes[PrinterRawOperation.cpi10] = "" + (char)27 + "P" + (char)20 + (char)18;
                    escapecodes[PrinterRawOperation.cpi12] = "" + (char)27 + "M" + (char)20 + (char)18;
                    // 15 cpi not supported in LX models
                    //  escapecodes[rpescape15cpi]=""+(char)27+"g"+(char)20;
                    escapecodes[PrinterRawOperation.cpi17] = "" + (char)27 + "P" + (char)20 + (char)15;
                    escapecodes[PrinterRawOperation.cpi20] = "" + (char)27 + "M" + (char)20 + (char)15;

                    escapecodes[PrinterRawOperation.EndPrint] = "" + (char)27 + (char)64;

                    // Open drawer
                    escapecodes[PrinterRawOperation.Pulse] = "" + (char)27 + (char)112 + (char)0 + (char)100 + (char)100;
                    break;
                case "IBMPROPRINTER":
                    // Init Printer-Line spacing to 1/6 - Draft mode
                    escapecodes[PrinterRawOperation.InitPrinter] = "" + (char)20 + (char)20 + (char)27 + (char)64;
                    escapecodes[PrinterRawOperation.LineSpace6] = "" + (char)27 + "2";
                    escapecodes[PrinterRawOperation.LineSpace8] = "" + (char)27 + "0";
                    escapecodes[PrinterRawOperation.LineSpace7_72] = "" + (char)27 + "1";
                    escapecodes[PrinterRawOperation.LineSpacen_216] = "" + (char)27 + "3";
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    escapecodes[PrinterRawOperation.FF] = "" + (char)12;
                    escapecodes[PrinterRawOperation.Bold] = "" + (char)27 + "E";
                    escapecodes[PrinterRawOperation.Underline] = "" + (char)27 + (char)45 + (char)1;
                    escapecodes[PrinterRawOperation.Italic] = "" + (char)27 + "4";
                    // Underline off-Bold off-Italic off
                    escapecodes[PrinterRawOperation.Normal] = "" + (char)27 + (char)45 + (char)0 +
                     (char)27 + "F" + (char)27 + "5";
                    // Set 10 or 12 cpi, enabled-disable double wide, enable-disable condensed
                    escapecodes[PrinterRawOperation.cpi5] = "" + (char)27 + (char)18 + (char)14 + (char)18;
                    escapecodes[PrinterRawOperation.cpi6] = "" + (char)27 + ":" + (char)14 + (char)18;
                    escapecodes[PrinterRawOperation.cpi10] = "" + (char)27 + (char)18 + (char)20 + (char)18;
                    escapecodes[PrinterRawOperation.cpi12] = "" + (char)20 + (char)18 + (char)27 + ":";
                    escapecodes[PrinterRawOperation.cpi17] = "" + (char)20 + (char)27 + (char)18 + (char)15;
                    escapecodes[PrinterRawOperation.cpi20] = "" + (char)20 + (char)27 + ":" + (char)15;

                    escapecodes[PrinterRawOperation.EndPrint] = "" + (char)20 + (char)20 + (char)27 + (char)64;
                    break;
                case "EPSONTMU210":
                    // Init Printer-Line spacing to 1/6 - Draft mode
                    escapecodes[PrinterRawOperation.InitPrinter] = "" + (char)27 + (char)64;
                    escapecodes[PrinterRawOperation.LineSpace6] = "" + (char)27 + "2";
                    escapecodes[PrinterRawOperation.Linespacen_60] = "" + (char)27 + "3";
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    //escapecodes[PrinterRawOperation.FF]=""+(char)12;
                    masterselect = true;
                    limitedmaster = true;
                    condensedmaster = false;

                    // Can select red font
                    escapecodes[PrinterRawOperation.RedFont] = "" + (char)27 + "r" + (char)1;
                    escapecodes[PrinterRawOperation.BlackFont] = "" + (char)27 + "r" + (char)0;

                    escapecodes[PrinterRawOperation.EndPrint] = "" + (char)27 + (char)64;
                    // Open drawer
                    escapecodes[PrinterRawOperation.Pulse] = "" + (char)27 + (char)112 + (char)0 + (char)100 + (char)100;
                    break;
                case "EPSONTMU210CUT":
                    escapecodes[PrinterRawOperation.InitPrinter] = "" + (char)27 + (char)64;
                    escapecodes[PrinterRawOperation.LineSpace6] = "" + (char)27 + "2";
                    escapecodes[PrinterRawOperation.Linespacen_60] = "" + (char)27 + "3";
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    //escapecodes[PrinterRawOperation.FF]=""+(char)12;
                    masterselect = true;
                    limitedmaster = true;
                    condensedmaster = false;
                    // Can select red font
                    escapecodes[PrinterRawOperation.RedFont] = "" + (char)27 + "r" + (char)1;
                    escapecodes[PrinterRawOperation.BlackFont] = "" + (char)27 + "r" + (char)0;
                    // Cut paper
                    escapecodes[PrinterRawOperation.EndPrint] = "" + (char)27 + "m" + (char)27 + (char)64;
                    // Open drawer
                    escapecodes[PrinterRawOperation.Pulse] = "" + (char)27 + (char)112 + (char)0 + (char)100 + (char)100;
                    break;
                case "EPSONTM88II":
                    // Init Printer-Line spacing to 1/6 - Draft mode
                    escapecodes[PrinterRawOperation.InitPrinter] = "" + (char)27 + (char)64;
                    escapecodes[PrinterRawOperation.LineSpace6] = "" + (char)27 + "2";
                    escapecodes[PrinterRawOperation.Linespacen_60] = "" + (char)27 + "3";
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    //escapecodes[PrinterRawOperation.FF]=""+(char)12;
                    masterselect = true;
                    limitedmaster = true;
                    condensedmaster = true;

                    escapecodes[PrinterRawOperation.EndPrint] = "" + (char)27 + (char)64;
                    // Open drawer
                    escapecodes[PrinterRawOperation.Pulse] = "" + (char)27 + (char)112 + (char)0 + (char)100 + (char)100;
                    break;

                case "EPSONTM88IICUT":
                    // Init Printer-Line spacing to 1/6 - Draft mode
                    escapecodes[PrinterRawOperation.InitPrinter] = "" + (char)27 + (char)64;
                    escapecodes[PrinterRawOperation.LineSpace6] = "" + (char)27 + "2";
                    escapecodes[PrinterRawOperation.Linespacen_60] = "" + (char)27 + "3";
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    //escapecodes[PrinterRawOperation.FF]=""+(char)12;
                    masterselect = true;
                    limitedmaster = true;
                    condensedmaster = true;
                    // Cut paper
                    escapecodes[PrinterRawOperation.EndPrint] = "" + (char)27 + "m" + (char)27 + (char)64;
                    // Open drawer
                    escapecodes[PrinterRawOperation.Pulse] = "" + (char)27 + (char)112 + (char)0 + (char)100 + (char)100;
                    break;
                case "HP-PCL":
                    // Init printer + 6 lines per inch
                    escapecodes[PrinterRawOperation.InitPrinter] = "" + (char)27 + (char)64;

                    escapecodes[PrinterRawOperation.LineSpace6] = "" + (char)27 + (char)38 + (char)108 + (char)54 + (char)68;
                    escapecodes[PrinterRawOperation.LineSpace8] = "" + (char)27 + (char)38 + (char)108 + (char)56 + (char)68;

                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    escapecodes[PrinterRawOperation.FF] = "" + (char)27 + (char)38 + (char)108 + (char)48 + (char)72; // Form feed and eject page
                    escapecodes[PrinterRawOperation.Bold] = "" + (char)27 + (char)40 + (char)115 + (char)51 + (char)66;
                    escapecodes[PrinterRawOperation.Underline] = "" + (char)27 + (char)38 + (char)100 + (char)48 + (char)68;
                    escapecodes[PrinterRawOperation.Italic] = "" + (char)27 + (char)40 + (char)115 + (char)49 + (char)83;
                    // Underline off-Bold off-Italic off
                    escapecodes[PrinterRawOperation.Normal] = "" + (char)27 + (char)38 + (char)100 + (char)64 +
                      (char)27 + (char)40 + (char)115 + (char)48 + (char)66 +
                      (char)27 + (char)40 + (char)115 + (char)48 + (char)83;
                    // Set 10 or 12 cpi, enabled-disable double wide, enable-disable condensed
                    escapecodes[PrinterRawOperation.cpi5] = "" + (char)27 + (char)40 + (char)115 + (char)5 + (char)72;
                    escapecodes[PrinterRawOperation.cpi6] = "" + (char)27 + (char)40 + (char)115 + (char)6 + (char)72;
                    escapecodes[PrinterRawOperation.cpi10] = "" + (char)27 + (char)38 + (char)107 + (char)48 + (char)83;
                    escapecodes[PrinterRawOperation.cpi15] = "" + (char)27 + (char)38 + (char)107 + (char)52 + (char)83;
                    escapecodes[PrinterRawOperation.cpi17] = "" + (char)27 + (char)40 + (char)115 + (char)17 + (char)72;
                    escapecodes[PrinterRawOperation.cpi20] = "" + (char)27 + (char)40 + (char)115 + (char)20 + (char)72;
                    break;
                case "PLAIN":
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    break;
                case "PLAINFULL":
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    escapecodes[PrinterRawOperation.CR] = "" + (char)13;
                    //                    FFullPlain = true;
                    break;
                case "VT100":
                    // Init Printer-Line spacing to 1/6 - Draft mode
                    //  escapecodes[PrinterRawOperation.InitPrinter]=""+(char)27+(char)64+(char)27+"2"+(char)27+"x"+(char)0;
                    escapecodes[PrinterRawOperation.LineFeed] = "" + (char)10;
                    //  escapecodes[PrinterRawOperation.CR]=""+(char)13;
                    //  escapecodes[PrinterRawOperation.FF]=""+(char)12;
                    //  escapecodes[rpescapenormal]=""+(char)27+"[0m";
                    // Set 10 or 12 cpi, enabled-disable double wide, enable-disable condensed
                    escapecodes[PrinterRawOperation.cpi15] = "" + (char)27 + "(char)" + "6";
                    escapecodes[PrinterRawOperation.cpi10] = "" + (char)27 + "(char)" + "5";
                    break;
            }
            if (escapecodes[PrinterRawOperation.cpi5].Length > 0)
                allowedsteps[PrintStepType.cpi5] = PrintStepType.cpi5;
            if (escapecodes[PrinterRawOperation.cpi6].Length > 0)
                allowedsteps[PrintStepType.cpi6] = PrintStepType.cpi6;
            if (escapecodes[PrinterRawOperation.cpi12].Length > 0)
                allowedsteps[PrintStepType.cpi12] = PrintStepType.cpi12;
            if (escapecodes[PrinterRawOperation.cpi15].Length > 0)
                allowedsteps[PrintStepType.cpi15] = PrintStepType.cpi15;
            if (escapecodes[PrinterRawOperation.cpi17].Length > 0)
                allowedsteps[PrintStepType.cpi17] = PrintStepType.cpi17;
            if (escapecodes[PrinterRawOperation.cpi20].Length > 0)
                allowedsteps[PrintStepType.cpi20] = PrintStepType.cpi20;
            if (masterselect)
            {
                allowedsteps[PrintStepType.cpi15] = PrintStepType.cpi17;
                if (limitedmaster)
                {
                    if (condensedmaster)
                    {
                        allowedsteps[PrintStepType.cpi10] = PrintStepType.cpi12;
                        allowedsteps[PrintStepType.cpi12] = PrintStepType.cpi12;
                        allowedsteps[PrintStepType.cpi5] = PrintStepType.cpi6;
                        allowedsteps[PrintStepType.cpi6] = PrintStepType.cpi6;
                        allowedsteps[PrintStepType.cpi20] = PrintStepType.cpi17;
                        allowedsteps[PrintStepType.cpi17] = PrintStepType.cpi17;
                    }
                    else
                    {
                        allowedsteps[PrintStepType.cpi15] = PrintStepType.cpi12;
                        allowedsteps[PrintStepType.cpi17] = PrintStepType.cpi12;
                        allowedsteps[PrintStepType.cpi20] = PrintStepType.cpi12;
                        allowedsteps[PrintStepType.cpi12] = PrintStepType.cpi12;
                    }
                }

            }
            else
            {
                if (escapecodes[PrinterRawOperation.cpi5].Length == 0)
                {
                    if (escapecodes[PrinterRawOperation.cpi6].Length > 0)
                        allowedsteps[PrintStepType.cpi5] = PrintStepType.cpi6;
                    else
                        allowedsteps[PrintStepType.cpi5] = PrintStepType.cpi10;
                }
                if (escapecodes[PrinterRawOperation.cpi6].Length == 0)
                {
                    allowedsteps[PrintStepType.cpi6] = PrintStepType.cpi10;
                }
                if (escapecodes[PrinterRawOperation.cpi12].Length == 0)
                {
                    if (escapecodes[PrinterRawOperation.cpi15].Length > 0)
                        allowedsteps[PrintStepType.cpi12] = PrintStepType.cpi15;
                    else
                    {
                        if (escapecodes[PrinterRawOperation.cpi17].Length > 0)
                            allowedsteps[PrintStepType.cpi12] = PrintStepType.cpi17;
                        else
                        {
                            if (escapecodes[PrinterRawOperation.cpi20].Length > 0)
                                allowedsteps[PrintStepType.cpi12] = PrintStepType.cpi20;
                            else
                                allowedsteps[PrintStepType.cpi12] = PrintStepType.cpi10;
                        }
                    }
                }
                if (escapecodes[PrinterRawOperation.cpi15].Length == 0)
                {
                    if (escapecodes[PrinterRawOperation.cpi17].Length > 0)
                        allowedsteps[PrintStepType.cpi15] = PrintStepType.cpi17;
                    else
                    {
                        if (escapecodes[PrinterRawOperation.cpi20].Length > 0)
                            allowedsteps[PrintStepType.cpi15] = PrintStepType.cpi20;
                        else
                            allowedsteps[PrintStepType.cpi15] = PrintStepType.cpi10;
                    }
                }
                if (escapecodes[PrinterRawOperation.cpi17].Length == 0)
                {
                    if (escapecodes[PrinterRawOperation.cpi20].Length > 0)
                        allowedsteps[PrintStepType.cpi17] = PrintStepType.cpi20;
                    else
                    {
                        if (escapecodes[PrinterRawOperation.cpi15].Length > 0)
                            allowedsteps[PrintStepType.cpi17] = PrintStepType.cpi15;
                        else
                            if (escapecodes[PrinterRawOperation.cpi12].Length > 0)
                            allowedsteps[PrintStepType.cpi17] = PrintStepType.cpi12;
                        else
                            allowedsteps[PrintStepType.cpi17] = PrintStepType.cpi10;
                    }
                }
                if (escapecodes[PrinterRawOperation.cpi20].Length == 0)
                {
                    if (escapecodes[PrinterRawOperation.cpi17].Length > 0)
                        allowedsteps[PrintStepType.cpi20] = PrintStepType.cpi17;
                    else
                    {
                        if (escapecodes[PrinterRawOperation.cpi15].Length > 0)
                            allowedsteps[PrintStepType.cpi20] = PrintStepType.cpi15;
                        else
                            if (escapecodes[PrinterRawOperation.cpi12].Length > 0)
                            allowedsteps[PrintStepType.cpi20] = PrintStepType.cpi12;
                        else
                            allowedsteps[PrintStepType.cpi20] = PrintStepType.cpi10;
                    }
                }
            }
        }

        public override void EndDocument(MetaFile meta)
        {
            string init;
            if (FDrawerAfter)
            {
                init = GetEscape(PrinterRawOperation.Pulse);
                if (init.Length > 0)
                    PrintResult.Append(init);
            }
            if (Reportman.Drawing.PrinterConfig.GetCutPaperOption(FPrinterSelect))
            {
                init = Reportman.Drawing.PrinterConfig.GetCutPaperOperation(FPrinterSelect);
                if (init.Length == 0)
                    init = GetEscape(PrinterRawOperation.CutPaper);
                if (init.Length > 0)
                {
                    if (init.Length > 0)
                        PrintResult.Append(init);

                }
            }
            if (Reportman.Drawing.PrinterConfig.GetOpenDrawerOption(FPrinterSelect))
            {
                init = Reportman.Drawing.PrinterConfig.GetOpenDrawerOperation(FPrinterSelect);
                if (init.Length == 0)
                    init = GetEscape(PrinterRawOperation.OpenDrawer);
                if (init.Length > 0)
                {
                    init = GetEscape(PrinterRawOperation.OpenDrawer);
                    if (init.Length > 0)
                        PrintResult.Append(init);

                }
            }
        }
        public void WriteCurrentPage(bool cutclearlines)
        {
            int lastline = Lines.Count - 1;
            if (cutclearlines)
            {
                while (lastline > 0)
                {
                    if (Lines[lastline].texts.Count == 0)
                    {
                        lastline--;
                    }
                    else
                        break;
                }
            }
            for (int i = 0; i <= lastline; i++)
            {
                string codedstring = EncodeLine(Lines[i], i);
                if (FullPlain)
                {
                    codedstring = codedstring.Trim();
                    codedstring = codedstring + GetEscape(PrinterRawOperation.LineFeed);
                }
                if (Lines[i].texts.Count == 0)
                {
                    whitelines++;
                }
                else
                    blacklines++;
                currpage.Append(codedstring);
            }
            if ((lastline < Lines.Count - 1) && (!cutclearlines))
            {
                string escapeff = GetEscape(PrinterRawOperation.FF);
                string escapelf = GetEscape(PrinterRawOperation.LineFeed);
                if ((escapeff.Length > 0) && (lastline < Lines.Count - 1))
                {
                    currpage.Append(escapeff);
                }
                else
                {
                    while (lastline < Lines.Count - 1)
                    {
                        lastline++;
                        currpage.Append(escapelf);
                        linefeeds++;
                    }
                }
            }
        }
        public string GetFontStepEscape(PrintStepType nstep)
        {
            if (masterselect)
            {
                byte aselect;
                byte condcode;
                string nresult = "";
                if (limitedmaster)
                    condcode = 0;
                else
                    condcode = mastercond;
                aselect = 0;
                if (limitedmaster && condensedmaster)
                {
                    switch (nstep)
                    {
                        case PrintStepType.cpi20:
                            aselect = 0;
                            break;
                        case PrintStepType.cpi17:
                            aselect = master12;
                            break;
                        case PrintStepType.cpi15:
                            aselect = 0;
                            break;
                        case PrintStepType.cpi12:
                            aselect = master10;
                            break;
                        case PrintStepType.cpi10:
                            aselect = 0;
                            break;
                        case PrintStepType.cpi6:
                            aselect = (byte)(master10 | masterwide);
                            break;
                        case PrintStepType.cpi5:
                            aselect = 0;
                            break;
                    }
                }
                else
                {
                    switch (nstep)
                    {
                        case PrintStepType.cpi20:
                            aselect = (byte)(master12 | condcode);
                            break;
                        case PrintStepType.cpi17:
                            aselect = (byte)(master10 | condcode);
                            break;
                        case PrintStepType.cpi15:
                            break;
                        case PrintStepType.cpi12:
                            aselect = master12;
                            break;
                        case PrintStepType.cpi10:
                            aselect = master10;
                            break;
                        case PrintStepType.cpi6:
                            aselect = (byte)(master12 | masterwide);
                            break;
                        case PrintStepType.cpi5:
                            aselect = (byte)(master10 | masterwide);
                            break;
                    }

                }
                nresult = "" + (char)27 + "!" + (char)aselect;


                return nresult;
            }
            string s = "";
            switch (nstep)
            {
                case PrintStepType.cpi5:
                    s = GetEscape(PrinterRawOperation.cpi5);
                    break;
                case PrintStepType.cpi6:
                    s = GetEscape(PrinterRawOperation.cpi6);
                    break;
                case PrintStepType.cpi10:
                    s = GetEscape(PrinterRawOperation.cpi10);
                    break;
                case PrintStepType.cpi12:
                    s = GetEscape(PrinterRawOperation.cpi12);
                    break;
                case PrintStepType.cpi15:
                    s = GetEscape(PrinterRawOperation.cpi15);
                    break;
                case PrintStepType.cpi17:
                    s = GetEscape(PrinterRawOperation.cpi17);
                    break;
                case PrintStepType.cpi20:
                    s = GetEscape(PrinterRawOperation.cpi20);
                    break;
            }
            return s;
        }
        public string EncodeLine(PrintLine Line, int index)
        {
            StringBuilder nline = new StringBuilder();

            bool changestep = false;
            if (PreviousLine == null)
                changestep = true;
            else
                if (PreviousLine.FontStep != Line.FontStep)
                changestep = true;
            if (changestep)
            {
                nline.Append(GetFontStepEscape(Line.FontStep));
                PreviousLine = new PrintLine();
                PreviousLine.FontStep = Line.FontStep;
            }
            int linepos = 0;
            foreach (PosText npos in Line.texts.Values)
            {
                while (linepos < npos.Position)
                {
                    nline.Append(" ");
                    linepos++;
                }
                if (!npos.regular)
                {
                    if (npos.underline)
                        nline.Append(GetEscape(PrinterRawOperation.Underline));
                    if (npos.bold)
                        nline.Append(GetEscape(PrinterRawOperation.Bold));
                    if (npos.red)
                        nline.Append(GetEscape(PrinterRawOperation.RedFont));
                    if (npos.strokeout)
                        nline.Append(GetEscape(PrinterRawOperation.StrikeOut));
                    if (npos.italic)
                        nline.Append(GetEscape(PrinterRawOperation.Italic));
                }
                string newstring = Line.Value.Substring(npos.Position, npos.nSize);
                if (OEMConvert)
                {
                    Encoding nencode = Encoding.GetEncoding(850);
                    Encoding oriencoding = Encoding.GetEncoding("utf-8");

                    byte[] nbytes = Encoding.Convert(oriencoding, nencode, oriencoding.GetBytes(newstring));
                    newstring = StreamUtil.ByteArrayToString(nbytes, nbytes.Length);
                }
                nline.Append(newstring);
                linepos = linepos + newstring.Length;
                if (!npos.regular)
                {
                    nline.Append(GetEscape(PrinterRawOperation.Normal));
                    if (npos.red)
                        nline.Append(GetEscape(PrinterRawOperation.BlackFont));
                }
            }
            string escapcr = GetEscape(PrinterRawOperation.CR);
            nline.Append(escapcr);
            string escapeff = GetEscape(PrinterRawOperation.FF);
            string escapelf = GetEscape(PrinterRawOperation.LineFeed);
            if (escapeff.Length > 0)
            {
                if (index == Lines.Count - 1)
                    nline.Append(escapelf);
                else
                    nline.Append(escapelf);
                linefeeds++;
            }
            else
                nline.Append(escapelf);


            return nline.ToString();
        }
        public override void EndPage(MetaFile meta)
        {
            PrintResult.Append(currpage.ToString());
        }
        public override Point GetPageSize(out int indexqt)
        {
            indexqt = 0;
            if (FPageWidth == 0)
            {
                FPageWidth = (int)Math.Round((double)MetaFile.PageSizeArray[0, 0] / 1000 * Twips.TWIPS_PER_INCH);
                FPageHeight = (int)Math.Round((double)MetaFile.PageSizeArray[0, 1] / 1000 * Twips.TWIPS_PER_INCH);

            }
            return new Point(FPageWidth, FPageHeight);
        }
        public override Point GraphicExtent(System.IO.MemoryStream astream, Point extent, int dpi)
        {
            throw new NotImplementedException();
        }
        public override Point SetPageSize(PageSizeDetail psize)
        {
            int newwidth, newheight;
            // Sets the page size for the pdf file, first if it's a qt page
            PageQt = psize.Index;
            if (psize.Custom)
            {
                PageQt = -1;
                newwidth = psize.CustomWidth;
                newheight = psize.CustomHeight;
            }
            else
            {
                newwidth = (int)Math.Round((double)MetaFile.PageSizeArray[psize.Index, 0] / 1000 * Twips.TWIPS_PER_INCH);
                newheight = (int)Math.Round((double)MetaFile.PageSizeArray[psize.Index, 1] / 1000 * Twips.TWIPS_PER_INCH);
            }
            if (FOrientation == OrientationType.Landscape)
            {
                FPageWidth = newheight;
                FPageHeight = newwidth;
            }
            else
            {
                FPageWidth = newwidth;
                FPageHeight = newheight;
            }
            return new Point(FPageWidth, FPageHeight);
        }
        public static PrintStepType FontSizeToStep(short FontSize, PrintStepType select)
        {
            PrintStepType aresult = PrintStepType.cpi10;
            if (select == PrintStepType.BySize)
            {
                switch (FontSize)
                {
                    case 8:
                        aresult = PrintStepType.cpi17;
                        break;
                    case 9:
                        aresult = PrintStepType.cpi15;
                        break;
                    case 10:
                        aresult = PrintStepType.cpi12;
                        break;
                    case 11:
                    case 12:
                        aresult = PrintStepType.cpi10;
                        break;
                    case 13:
                    case 14:
                    case 15:
                        aresult = PrintStepType.cpi6;
                        break;
                    default:
                        if (FontSize < 8)
                            aresult = PrintStepType.cpi20;
                        else
                        if (FontSize > 15)
                            aresult = PrintStepType.cpi5;
                        break;
                }
            }
            else
                aresult = select;
            return aresult;
        }
        public override Point TextExtent(TextObjectStruct aobj, Point extent)
        {
            bool singleline;
            Rectangle rect;
            PrintStepType fontstep;
            Point maxextent = new Point(extent.X, extent.Y); ;
            if (aobj.FontRotation != 0)
                return extent;
            if (aobj.CutText)
            {
                maxextent = extent;
            }

            // single line
            singleline = ((aobj.Alignment & MetaFile.AlignmentFlags_SingleLine) > 0);
            fontstep = FontSizeToStep(aobj.FontSize, aobj.PrintStep);
            rect = new Rectangle(0, 0, extent.X, 0);

            string ntext = aobj.Text.Replace("" + (char)13 + (char)10, "" + (char)10);


            CalculateTextExtent(ntext, ref rect, aobj.WordWrap, singleline, fontstep, aobj.CutText);

            extent = new Point(rect.Right, rect.Bottom);
            if (aobj.CutText)
            {
                if (maxextent.Y < extent.Y)
                    extent = new Point(extent.X, maxextent.Y);
            }
            return extent;
        }
        public void WriteInit()
        {
            //string init = GetEscape(PrinterRawOperation.InitPrinter);
            //f (init.Length>0)
            //currpage.Append(init);
            /*if (FDrawerBefore)
            {
                init = GetEscape(PrinterRawOperation.Pulse);
                if (init.Length>0)
                    currpage.Append(init);
            }*/
        }
        public void WriteInterLine()
        {
            string s = "";
            switch (FLinesPerInch)
            {
                case 600:
                    s = GetEscape(PrinterRawOperation.LineSpace6);
                    break;
                case 800:
                    s = GetEscape(PrinterRawOperation.LineSpace8);
                    break;
                default:
                    s = GetEscape(PrinterRawOperation.LineSpace7_72);
                    if ((FLinesPerInch != 972) || (s.Length == 0))
                    {
                        s = GetEscape(PrinterRawOperation.LineSpacen_216);
                        if (s.Length > 0)
                        {
                            byte nlines = (byte)Math.Round(1.0m / ((decimal)FLinesPerInch / (decimal)100) * (decimal)216);
                            s = s + (char)nlines;
                        }
                        else
                        {
                            s = GetEscape(PrinterRawOperation.LineSpacen_180);
                            if (s.Length > 0)
                            {
                                byte nlines2 = (byte)Math.Round(1.0m / ((decimal)FLinesPerInch / (decimal)100) * (decimal)180);
                                s = s + (char)nlines2;
                            }
                            else
                            {
                                s = GetEscape(PrinterRawOperation.Linespacen_60);
                                if (s.Length > 0)
                                {
                                    byte nlines3 = (byte)Math.Round(1.0m / ((decimal)FLinesPerInch / (decimal)100) * (decimal)180);
                                    s = s + (char)nlines3;
                                }
                            }

                        }

                    }
                    break;
            }
            if (s.Length > 0)
                currpage.Append(s);
        }
        public override bool Print(MetaFile meta)
        {
            currpage = new StringBuilder();
            bool aresult = base.Print(meta);
            int FCurrentPage = FromPage - 1;
            meta.RequestPage(FCurrentPage);
            if (meta.Pages.CurrentCount < FromPage)
                return false;
            SetPageSize(meta.Pages[0].PageDetail);
            SetOrientation(meta.Orientation);




            MetaPage apage;
            while (meta.Pages.CurrentCount > FCurrentPage)
            {
                apage = meta.Pages[FCurrentPage];
                if (FCurrentPage == (FromPage - 1))
                {
                    WriteInit();
                    WriteInterLine();
                    WritePageSize();
                }
                else
                {
                    if ((FCurrentPage <= (ToPage - 1)) && (FCurrentPage >= (FromPage - 1)))
                        NewPage(meta, apage);
                }
                DrawPage(meta, apage);
                FCurrentPage++;
                meta.RequestPage(FCurrentPage);
                if ((FCurrentPage > (ToPage - 1)) || (FCurrentPage >= meta.Pages.CurrentCount))
                {
                    bool cutclearlines = false;
                    if ((FPrinterDriver == "EPSONTMU210") ||
                       (FPrinterDriver == "EPSONTMU210CUT") ||
                        (FPrinterDriver == "EPSONTM88II") ||
                       (FPrinterDriver == "EPSONTM88IICUT"))
                        cutclearlines = true;

                    WriteCurrentPage(cutclearlines);
                    EndPage(meta);
                    break;
                }
                else
                {
                    WriteCurrentPage(false);
                    EndPage(meta);
                }
            }
            EndDocument(meta);
            return true;
        }
        public override void NewPage(MetaFile meta, MetaPage page)
        {
            currpage = new StringBuilder();
            RecalcSize();
            base.NewPage(meta, page);
        }
        public void RecalcSize()
        {
            int numberoflines = (int)Math.Round(Twips.TwipsToInch(FPageHeight) * ((decimal)FLinesPerInch / 100));
            Lines.Clear();
            for (int i = 0; i < numberoflines; i++)
            {
                Lines.Add(new PrintLine());
            }
            linefeeds = 0;
        }
        public void WritePageSize()
        {
            // Write interline

            string s = "";
            // Set line space
            char plinecount = (char)(byte)Lines.Count;
            switch (FPrinterDriver)
            {
                case "EPSON":
                case "EPSON-ESCPQ":
                case "EPSON-ESCP":
                case "EPSON-MASTER":
                case "IBMPROPRINTER":
                    s = s + (char)27 + 'C' + plinecount;
                    break;
                case "HP-PCL":
                    s = s + (char)27 + (char)38 + (char)108 + plinecount + (char)80;
                    break;
            }
            if (s.Length > 0)
                currpage.Append(s);
            // Select regular font
            s = GetEscape(PrinterRawOperation.Normal);
            if (s.Length > 0)
                currpage.Append(s);
        }
        public PrintStepType NearestFontStep(PrintStepType fontstep)
        {
            return allowedsteps[fontstep];
        }
        /*function TRpTextDriver.NearestFontStep(FontStep:TRpFontStep):TRpFontStep;
        var
         maxallowed:TRpFontStep;
         i:TRpFontStep;
        begin
         maxallowed:=rpcpi10;
         for i:=Low(TRpFontStep) to High(TRpFontStep) do
         begin
          if allowedsizes[i] then
          begin
           maxallowed:=i;
           break;
          end;
         end;
         Result:=maxallowed;
         i:=FontStep;
         while i>maxallowed do
         begin
          if allowedsizes[i] then
          begin
           Result:=i;
           break;
          end;
          dec(i);
         end;
        end;


                 */
        public double StepToTwips(PrintStepType step)
        {
            double aresult = (double)Twips.TWIPS_PER_INCH / 10.0;

            switch (step)
            {
                case PrintStepType.cpi20:
                    aresult = (double)Twips.TWIPS_PER_INCH / 20.0;
                    break;
                case PrintStepType.cpi17:
                    aresult = (double)Twips.TWIPS_PER_INCH / 17.14;
                    break;
                case PrintStepType.cpi15:
                    aresult = (double)Twips.TWIPS_PER_INCH / 15.0;
                    break;
                case PrintStepType.cpi12:
                    aresult = (double)Twips.TWIPS_PER_INCH / 12.0;
                    break;
                case PrintStepType.cpi10:
                    aresult = (double)Twips.TWIPS_PER_INCH / 10.0;
                    break;
                case PrintStepType.cpi6:
                    aresult = (double)Twips.TWIPS_PER_INCH / 6.0;
                    break;
                case PrintStepType.cpi5:
                    aresult = (double)Twips.TWIPS_PER_INCH / 5.0;
                    break;
            }

            return aresult;
        }
        public double CalcCharWidth(char charcode, PrintStepType step)
        {
            if ((charcode == (char)0) || (charcode == (char)13) || (charcode == (char)10))
                return 0;
            else
                return StepToTwips(step);
        }
        bool IsSign(char c)
        {
            return ((c == ',') || (c == '.') || (c == '-') || (c == ' ') || (c == ':') || (c == ';'));
        }
        public void CalculateTextExtent(string text, ref Rectangle rect, bool wordbreak, bool singleline, PrintStepType fontstep, bool doclip)
        {
            bool dolineinfo = true;
            // Calculate leading and line spacing
            int linespacing = (int)Math.Round((double)Twips.TWIPS_PER_INCH / ((double)FLinesPerInch / 100.0));
            int leading = 0;
            int maxrightindex = -1;

            fontstep = NearestFontStep(fontstep);

            StringBuilder currentline = new StringBuilder();
            double currentwidth = 0;
            double maxwidth = 0;
            double newsize = 0;
            int linebreakpos = 0;
            bool wasspace = false;
            int infocount = 0;
            double lastsize = 0;
            double lastsizewithoutspace = 0;
            int lastindexwithoutspace = 0;
            int currenttoppos = 0;
            double recwidth = (double)(rect.Width);
            if (dolineinfo)
                linfos.Clear();
            // Replace cr/lf for only cfs
            //string astring = text.Replace("" + (char)13 + (char)10, "" + (char)10);
            string astring = text;
            if (singleline)
            {
                astring = astring.Replace("" + (char)13, " ");
                astring = astring.Replace("" + (char)10, " ");
            }

            int i = 0;
            int startposition = 0;
            LineInfo linfo = new LineInfo();
            while (i < astring.Length)
            {
                // Check for LF
                if ((astring[i] == (char)10))
                {
                    // Add the line
                    int cutindex = i;
                    if (wasspace)
                    {
                        cutindex = lastindexwithoutspace + 1;
                        currentwidth = lastsizewithoutspace;
                    }
                    if (maxrightindex >= 0)
                        cutindex = maxrightindex;
                    linfo.LastLine = true;
                    linfo.Position = startposition;
                    linfo.Step = fontstep;
                    linfo.Size = cutindex - startposition;
                    if (linfo.Size < 0)
                        linfo.Size = 0;
                    linfo.Width = (int)Math.Round(currentwidth);
                    if (currentwidth > maxwidth)
                        maxwidth = currentwidth;
                    linfo.TopPos = currenttoppos - leading;
                    linfo.Height = linespacing;
                    currenttoppos = currenttoppos + linespacing;
                    bool doinsert = true;
                    if (doclip)
                    {
                        if ((linfo.Height * linfos.Count + linfo.Height) > rect.Height)
                            doinsert = false;
                    }
                    if (doinsert)
                    {
                        linfos.Add(linfo);
                        infocount++;
                    }
                    currentwidth = 0;
                    startposition = i + 1;
                    maxrightindex = -1;
                }
                else
                {
                    newsize = CalcCharWidth(astring[i], fontstep);
                    // If the character fits inside the line
                    if ((currentwidth + newsize <= recwidth) || (!wordbreak))
                    {
                        if (currentwidth + newsize > recwidth)
                            if (maxrightindex < 0)
                                maxrightindex = i - 1;
                        if (astring[i] == ' ')
                        {
                            if (!wasspace)
                            {
                                wasspace = true;
                                lastsizewithoutspace = currentwidth;
                                if (i > startposition)
                                {
                                    lastindexwithoutspace = i - 1;
                                    linebreakpos = i - 1;
                                }
                                else
                                    lastindexwithoutspace = 0;
                            }
                        }
                        else
                        {
                            wasspace = false;
                            if (IsSign(astring[i]))
                            {
                                linebreakpos = i;
                                lastsize = currentwidth + newsize;
                            }
                        }
                        currentwidth = currentwidth + newsize;
                    }
                    // When the character does not fit
                    else
                    {
                        if (wordbreak)
                        {
                            // Add the line
                            if ((currentwidth == 0) && (startposition == i))
                                i++;
                            int cutindex = i;
                            if (linebreakpos > 0)
                            {
                                cutindex = linebreakpos + 1;
                                i = linebreakpos;
                                currentwidth = lastsize;
                            }
                            else
                                i--;
                            if (wasspace)
                            {
                                cutindex = lastindexwithoutspace + 1;
                                currentwidth = lastsizewithoutspace;
                            }
                            if (maxrightindex >= 0)
                                cutindex = maxrightindex;
                            linfo.LastLine = false;
                            linfo.Position = startposition;
                            linfo.Step = fontstep;
                            linfo.Size = cutindex - startposition;
                            if (linfo.Size < 0)
                                linfo.Size = 0;
                            linfo.Width = (int)Math.Round((currentwidth));
                            if (currentwidth > maxwidth)
                                maxwidth = currentwidth;
                            linfo.TopPos = currenttoppos - leading;
                            linfo.Height = linespacing;
                            currenttoppos = currenttoppos + linespacing;
                            bool doinsert = true;
                            if (doclip)
                            {
                                if ((linfo.Height * linfos.Count + linfo.Height) > rect.Height)
                                    doinsert = false;
                            }
                            if (doinsert)
                            {
                                linfos.Add(linfo);
                                infocount++;
                            }
                            currentwidth = 0;
                            linebreakpos = 0;
                            startposition = i + 1;
                            maxrightindex = -1;
                            // Skip spaces
                            while (startposition < astring.Length - 1)
                            {
                                if (astring[startposition] == ' ')
                                {
                                    startposition++;
                                    i = startposition - 1;
                                }
                                else
                                    break;
                            }
                        }
                        else
                        {
                            if (maxrightindex < 0)
                                maxrightindex = i - 1;
                        }
                    }
                }
                i++;
            }
            // Check for LF
            if (startposition < astring.Length)
            {
                // Add the line
                int cutindex = i;
                if (wasspace)
                {
                    cutindex = lastindexwithoutspace + 1;
                    currentwidth = lastsizewithoutspace;
                }
                if (maxrightindex >= 0)
                    cutindex = maxrightindex;
                linfo.LastLine = true;
                linfo.Position = startposition;
                linfo.Step = fontstep;
                linfo.Size = cutindex - startposition;
                if (linfo.Size < 0)
                    linfo.Size = 0;
                linfo.Width = (int)Math.Round(currentwidth);
                if (currentwidth > maxwidth)
                    maxwidth = currentwidth;
                linfo.TopPos = currenttoppos - leading;
                linfo.Height = linespacing;
                currenttoppos = currenttoppos + linespacing;
                bool doinsert = true;
                if (doclip)
                {
                    if ((linfo.Height * linfos.Count + linfo.Height) > rect.Height)
                        doinsert = false;
                }
                if (doinsert)
                {
                    linfos.Add(linfo);
                    infocount++;
                }
                maxrightindex = -1;
            }
            int totalheight = 0;
            if (infocount > 0)
                totalheight = infocount * linespacing + leading;
            Rectangle arec = new Rectangle(rect.Left, rect.Top,
                                           (int)Math.Round((maxwidth)),
                                           totalheight);
            rect = arec;

        }
        public void CalculateTextExtent2(string text, ref Rectangle rect, bool WordBreak, bool singleline, PrintStepType fontstep)
        {
            string astring;
            int i;
            double asize;
            Rectangle arec;
            int position;
            LineInfo info;
            double maxwidth;
            double newsize;
            double recwidth;
            int linebreakpos;
            bool nextline;
            double alastsize;
            bool lockspace;
            bool createsnewline;
            bool DescPos = false;

            astring = text;
            fontstep = NearestFontStep(fontstep);
            arec = new Rectangle(0, 0, rect.Width, 0);

            asize = 0.0;
            createsnewline = false;
            position = 0;
            linebreakpos = -1;
            maxwidth = 0;
            recwidth = rect.Right - rect.Left;
            nextline = false;
            i = 0;
            alastsize = 0;
            lockspace = false;
            linfos.Clear();

            while (i < astring.Length)
            {
                newsize = CalcCharWidth(astring[i], fontstep);
                if (!((astring[i] == ' ') || (astring[i] == (char)10) || (astring[i] == (char)13)))
                {
                    lockspace = false;
                    if (WordBreak)
                    {
                        if ((asize + newsize) > recwidth)
                        {
                            if (linebreakpos >= 0)
                            {
                                i = linebreakpos;
                            }
                            else
                                i--;
                            nextline = true;
                            asize = alastsize;
                            linebreakpos = -1;
                        }
                        else
                        {
                            if ((astring[i] == '.') || (astring[i] == ',') || (astring[i] == '-') || (astring[i] == ' '))
                            {
                                linebreakpos = i;
                                if (astring[i] == ' ')
                                {
                                    if (!lockspace)
                                    {
                                        alastsize = asize;
                                        lockspace = true;
                                    }
                                }
                                else
                                {
                                    alastsize = asize + newsize;
                                }
                            }
                            asize = asize + newsize;
                        }
                    }
                    else
                        asize = asize + newsize;
                }
                else
                {
                    asize = asize + newsize;
                }
                if (!singleline)
                {
                    if (astring[i] == (char)10)
                    {
                        nextline = true;
                        DescPos = true;
                        createsnewline = true;
                    }
                }
                if (asize > maxwidth)
                    maxwidth = asize;
                if (nextline)
                {
                    info = new LineInfo();
                    nextline = false;
                    info.Position = position;
                    info.LastLine = createsnewline;
                    info.Size = i - position;
                    if (DescPos)
                    {
                        DescPos = false;
                        info.Size = info.Size - 2;
                    }
                    info.Width = (int)Math.Round(asize);
                    info.Height = (int)Math.Round((double)Twips.TWIPS_PER_INCH / ((double)FLinesPerInch / 100.0));
                    info.TopPos = arec.Bottom;
                    arec = new Rectangle(arec.Left, arec.Top, arec.Width, arec.Height + info.Height);
                    asize = 0;
                    position = i + 1;
                    linfos.Add(info);
                    createsnewline = false;
                    if (i < astring.Length - 1)
                    {
                        if (astring[i + 1] == ' ')
                        {
                            i++;
                            position = i + 1;
                        }
                    }
                }
                i++;
            }
            arec = new Rectangle(arec.Left, arec.Top, (int)Math.Round(maxwidth + 1), arec.Height);
            if (position < astring.Length)
            {
                info = new LineInfo();
                info.Position = position;
                info.Size = astring.Length - position;
                info.Width = (int)Math.Round(asize + 1);
                info.Height = (int)Math.Round(System.Convert.ToDouble(Twips.TWIPS_PER_INCH) / (System.Convert.ToDouble(FLinesPerInch) / 100));
                info.TopPos = arec.Bottom;
                arec = new Rectangle(arec.Left, arec.Top, arec.Width, arec.Height + info.Height);
                linfos.Add(info);
            }
            rect = new Rectangle(arec.Left, arec.Top, arec.Width, linfos.Count * (int)Math.Round(System.Convert.ToDouble(Twips.TWIPS_PER_INCH) / (System.Convert.ToDouble(FLinesPerInch) / 100)));
        }
    }
    public class PrintLine
    {
        public SortedList<int, PosText> texts;
        public PrintStepType FontStep;
        public string Value;
        public PrintLine()
        {
            Value = "";
            texts = new SortedList<int, PosText>();
            FontStep = PrintStepType.cpi10;
        }
    }
    public struct PosText
    {
        public int Position;
        public int nSize;
        public bool underline;
        public bool italic;
        public bool red;
        public bool bold;
        public bool strokeout;
        public bool regular;
    }
}
