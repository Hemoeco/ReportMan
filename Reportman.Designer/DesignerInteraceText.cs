using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reportman.Reporting;
using Reportman.Drawing;

namespace Reportman.Designer
{
    internal class DesignerInterfaceText : DesignerInterfaceSizePos
    {
        private PrintItemText FPrintItemText;
        public DesignerInterfaceText(SortedList<int, ReportItem> repitem, IObjectInspector objinsp)
            : base(repitem, objinsp)
        {
            if (repitem.Count == 1)
                FPrintItemText = (PrintItemText)repitem.Values[0];
        }
        public PrintItemText PrintItemTextObject { get { return FPrintItemText; } }
        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
            base.GetProperties(lnames, ltypes, lvalues, lhints, lcat);
            // Text H.Alignment
            lnames.Add(Translator.TranslateStr(628));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemText.Alignment);
            // Text V.Alignment
            lnames.Add(Translator.TranslateStr(629));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemText.VAlignment);
            // Font Name
            lnames.Add(Translator.TranslateStr(560));
            ltypes.Add(Translator.TranslateStr(560));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.WFontName);
            // Linux (postcript) Font Name
            lnames.Add(Translator.TranslateStr(561));
            ltypes.Add(Translator.TranslateStr(561));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.LFontName);
            // Type1 Font (PDF Font)
            lnames.Add(Translator.TranslateStr(562));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemText.Type1Font);
            // Font Step (dot matrix and esc/p export)
            lnames.Add(Translator.TranslateStr(1039));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemText.PrintStep);
            // Font Size
            lnames.Add(Translator.TranslateStr(563));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.FontSize);
            // Font Color
            lnames.Add(Translator.TranslateStr(564));
            ltypes.Add(Translator.TranslateStr(558));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.FontColor);
            // Font Style
            lnames.Add(Translator.TranslateStr(566));
            ltypes.Add(Translator.TranslateStr(566));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.FontStyle);
            // RightToLeft
            lnames.Add(Translator.TranslateStr(954));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.RightToLeft);
            // Back Color
            lnames.Add(Translator.TranslateStr(565));
            ltypes.Add(Translator.TranslateStr(558));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.BackColor);
            // Transparent
            lnames.Add(Translator.TranslateStr(567));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.Transparent);
            // Cut Text
            lnames.Add(Translator.TranslateStr(625));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.CutText);
            // Word Wrap
            lnames.Add(Translator.TranslateStr(626));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.WordWrap);
            // Word Break
            //            lnames.Add(Translator.TranslateStr(626));
            //            ltypes.Add(Translator.TranslateStr(568));
            //            lhints.Add("refcommontext.html");
            //            lcat.Add(Translator.TranslateStr(1202));
            //           if (lvalues != null)
            //               lvalues.Add(FPrintItemText.WordBreak);
            // Single line
            lnames.Add(Translator.TranslateStr(627));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.SingleLine);
            // Font Rotation
            lnames.Add(Translator.TranslateStr(551));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refcommontext.html");
            lcat.Add(Translator.TranslateStr(1202));
            if (lvalues != null)
                lvalues.Add(FPrintItemText.FontRotation / 10);
        }
        public override void GetPropertyValues(string pname, Strings lpossiblevalues)
        {
            // if pname=SrpSRightToLeft then
            // begin
            //  GetBidiDescriptions(lpossiblevalues);
            //  exit;
            // end;
            // Alignment
            if (pname == Translator.TranslateStr(628))
            {
                //lpossiblevalues.Add(Translator.TranslateStr(630));
                lpossiblevalues.Add(Translator.TranslateStr(631));
                lpossiblevalues.Add(Translator.TranslateStr(632));
                lpossiblevalues.Add(Translator.TranslateStr(633));
                lpossiblevalues.Add(Translator.TranslateStr(1113));
                return;
            }
            // VAlignment
            if (pname == Translator.TranslateStr(629))
            {
                //lpossiblevalues.Add(Translator.TranslateStr(630));
                lpossiblevalues.Add(Translator.TranslateStr(634));
                lpossiblevalues.Add(Translator.TranslateStr(635));
                lpossiblevalues.Add(Translator.TranslateStr(633));
                return;
            }
            // Type1Font
            if (pname == Translator.TranslateStr(562))
            {
                lpossiblevalues.Add("Helvetica");
                lpossiblevalues.Add("Courier");
                lpossiblevalues.Add("Times Roman");
                lpossiblevalues.Add("Symbol");
                lpossiblevalues.Add("ZafDingbats");
                lpossiblevalues.Add("Truetype Link");
                lpossiblevalues.Add("Truetype Embed");
                return;
            }
            // FontStep
            if (pname == Translator.TranslateStr(1039))
            {
                lpossiblevalues.Add(Translator.TranslateStr(1038));
                lpossiblevalues.Add("20 cpi");
                lpossiblevalues.Add("17 cpi");
                lpossiblevalues.Add("15 cpi");
                lpossiblevalues.Add("12 cpi");
                lpossiblevalues.Add("10 cpi");
                lpossiblevalues.Add("6 cpi");
                lpossiblevalues.Add("5 cpi");
                return;
            }

            base.GetPropertyValues(pname, lpossiblevalues);
        }
        public static PrintStepType StringToPrintStep(string nvalue)
        {
            PrintStepType aresult = PrintStepType.BySize;
            switch (nvalue)
            {
                case "5 cpi":
                    aresult = PrintStepType.cpi5;
                    break;
                case "6 cpi":
                    aresult = PrintStepType.cpi6;
                    break;
                case "10 cpi":
                    aresult = PrintStepType.cpi10;
                    break;
                case "12 cpi":
                    aresult = PrintStepType.cpi12;
                    break;
                case "15 cpi":
                    aresult = PrintStepType.cpi15;
                    break;
                case "17 cpi":
                    aresult = PrintStepType.cpi17;
                    break;
                case "20 cpi":
                    aresult = PrintStepType.cpi20;
                    break;
            }
            return aresult;
        }
        public static TextAlignType PrintStringToAlignment(string nvalue)
        {
            TextAlignType aresult = TextAlignType.Left;
            if (nvalue == Translator.TranslateStr(630))
                aresult = TextAlignType.Left;
            if (nvalue == Translator.TranslateStr(631))
                aresult = TextAlignType.Right;
            if (nvalue == Translator.TranslateStr(632))
                aresult = TextAlignType.Right;
            if (nvalue == Translator.TranslateStr(633))
                aresult = TextAlignType.Center;
            if (nvalue == Translator.TranslateStr(1113))
                aresult = TextAlignType.Justify;
            return aresult;
        }
        public static TextAlignVerticalType PrintStringToVAlignment(string nvalue)
        {
            TextAlignVerticalType aresult = TextAlignVerticalType.Top;
            if (nvalue == Translator.TranslateStr(630))
                aresult = TextAlignVerticalType.Top;
            if (nvalue == Translator.TranslateStr(634))
                aresult = TextAlignVerticalType.Top;
            if (nvalue == Translator.TranslateStr(633))
                aresult = TextAlignVerticalType.Center;
            if (nvalue == Translator.TranslateStr(635))
                aresult = TextAlignVerticalType.Bottom;
            return aresult;
        }
        public static PDFFontType Type1FontStringToType1Font(string nvalue)
        {
            PDFFontType aresult = PDFFontType.Helvetica;
            return aresult;
        }

        public override void SetProperty(string pname, Variant newvalue)
        {
            // Alignment
            if (pname == Translator.TranslateStr(628))
            {
                FPrintItemText.Alignment = (TextAlignType)(int)newvalue;
                return;
            }
            // V. Alignment
            if (pname == Translator.TranslateStr(629))
            {
                FPrintItemText.VAlignment = (TextAlignVerticalType)(int)newvalue;
                return;
            }
            // Font Name
            if (pname == Translator.TranslateStr(560))
            {
                FPrintItemText.WFontName = newvalue.ToString();
                return;
            }
            // L. Font Name
            if (pname == Translator.TranslateStr(561))
            {
                FPrintItemText.LFontName = newvalue.ToString();
                return;
            }
            // Type1Font
            if (pname == Translator.TranslateStr(562))
            {
                if (newvalue.VarType == VariantType.Integer)
                {
                    FPrintItemText.Type1Font = (PDFFontType)newvalue.AsInteger;
                }
                else
                    FPrintItemText.Type1Font = Type1FontStringToType1Font(newvalue);
                return;
            }
            // Font Step (dot matrix and esc/p export)
            if (pname == Translator.TranslateStr(1039))
            {
                FPrintItemText.PrintStep = StringToPrintStep(newvalue);
                return;
            }
            // Font Size
            if (pname == Translator.TranslateStr(563))
            {
                FPrintItemText.FontSize = (short)newvalue;
                return;
            }
            // Font Color
            if (pname == Translator.TranslateStr(564))
            {
                FPrintItemText.FontColor = newvalue;
                return;
            }
            // Font Style
            if (pname == Translator.TranslateStr(566))
            {
                FPrintItemText.FontStyle = newvalue;
                return;
            }
            // Back Color
            if (pname == Translator.TranslateStr(565))
            {
                FPrintItemText.BackColor = newvalue;
                return;
            }
            // RightToLeft
            if (pname == Translator.TranslateStr(954))
            {
                FPrintItemText.RightToLeft = newvalue;
                return;
            }
            // Transparent
            if (pname == Translator.TranslateStr(567))
            {
                FPrintItemText.Transparent = newvalue;
                return;
            }
            // Cut Text
            if (pname == Translator.TranslateStr(625))
            {
                FPrintItemText.CutText = newvalue;
                return;
            }
            // Word Wrap
            if (pname == Translator.TranslateStr(626))
            {
                FPrintItemText.WordWrap = newvalue;
                return;
            }
            // Word Break
            //            lnames.Add(Translator.TranslateStr(626));
            //            ltypes.Add(Translator.TranslateStr(568));
            //            lhints.Add("refcommontext.html");
            //            lcat.Add(Translator.TranslateStr(1202));
            //           if (lvalues != null)
            //               lvalues.Add(FPrintItemText.WordBreak);
            // Single line
            // Word Wrap
            if (pname == Translator.TranslateStr(627))
            {
                FPrintItemText.SingleLine = newvalue;
                return;
            }
            // Font Rotation
            // Word Wrap
            if (pname == Translator.TranslateStr(551))
            {
                FPrintItemText.FontRotation = (short)(newvalue * 10);
                return;
            }
            // inherited
            base.SetProperty(pname, newvalue);
        }
        public static int StringToFontStyle(string newvalue)
        {
            int aresult = 0;
            return aresult;
        }
        public override void SetItem(ReportItem nitem)
        {
            if (nitem is PrintItemText)
                FPrintItemText = (PrintItemText)nitem;
            base.SetItem(nitem);
        }
        public override Variant GetProperty(string pname)
        {
            // Text H.Alignment
            if (pname == Translator.TranslateStr(628))
                return (int)FPrintItemText.Alignment;
            // Text V.Alignment
            if (pname == Translator.TranslateStr(629))
                return (int)FPrintItemText.VAlignment;
            // Font Name
            if (pname == Translator.TranslateStr(560))
                return FPrintItemText.WFontName;
            // Linux (postcript) Font Name
            if (pname == Translator.TranslateStr(561))
                return FPrintItemText.LFontName;
            // Type1 Font (PDF Font)
            if (pname == Translator.TranslateStr(562))
                return (int)FPrintItemText.Type1Font;
            // Font Step (dot matrix and esc/p export)
            if (pname == Translator.TranslateStr(1039))
                return (int)FPrintItemText.PrintStep;
            // Font Size
            if (pname == Translator.TranslateStr(563))
                return FPrintItemText.FontSize;
            // Font Color
            if (pname == Translator.TranslateStr(564))
                return FPrintItemText.FontColor;
            // Font Style
            if (pname == Translator.TranslateStr(566))
                return FPrintItemText.FontStyle;
            // RightToLeft
            if (pname == Translator.TranslateStr(954))
                return FPrintItemText.RightToLeft;
            // RightToLeft
            //            lnames.Add(Translator.TranslateStr(954));
            //            ltypes.Add(Translator.TranslateStr(568));
            //            lhints.Add("refcommontext.html");
            //lcat.Add(Translator.TranslateStr(1202));
            //            if (lvalues != null)
            //                lvalues.Add(FPrintItemText.RightToLeft);
            // Back Color
            if (pname == Translator.TranslateStr(565))
                return FPrintItemText.BackColor;
            // Transparent
            if (pname == Translator.TranslateStr(567))
                return FPrintItemText.Transparent;
            // Cut Text
            if (pname == Translator.TranslateStr(625))
                return FPrintItemText.CutText;
            // Word Wrap
            if (pname == Translator.TranslateStr(626))
                return FPrintItemText.WordWrap;
            // Word Break
            //            lnames.Add(Translator.TranslateStr(626));
            //            ltypes.Add(Translator.TranslateStr(568));
            //            lhints.Add("refcommontext.html");
            //            lcat.Add(Translator.TranslateStr(1202));
            //           if (lvalues != null)
            //               lvalues.Add(FPrintItemText.WordBreak);
            // Single line
            if (pname == Translator.TranslateStr(627))
                return FPrintItemText.SingleLine;
            // Font Rotation
            if (pname == Translator.TranslateStr(551))
                return FPrintItemText.FontRotation / 10;

            // inherited
            return base.GetProperty(pname);
        }
    }

}
