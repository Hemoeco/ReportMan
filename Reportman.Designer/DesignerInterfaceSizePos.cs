using System;
using System.Collections.Generic;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Designer
{
    internal class DesignerInterfaceSizePos : DesignerInterfaceSize
    {
        private PrintPosItem FPrintPosItemObject;
        public DesignerInterfaceSizePos(SortedList<int, ReportItem> repitem, IObjectInspector objinsp) : base(repitem, objinsp)
        {
            if (repitem.Count == 1)
                FPrintPosItemObject = (PrintPosItem)repitem.Values[0];
        }
        public PrintPosItem PrintPosItemObject { get { return FPrintPosItemObject; } }
        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
            base.GetProperties(lnames, ltypes, lvalues, lhints, lcat);
            // Left
            lnames.Add(Translator.TranslateStr(553));
            ltypes.Add(Translator.TranslateStr(556));
            lhints.Add("refcommon.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add(PrintPosItemObject.PosX);
            // Top
            lnames.Add(Translator.TranslateStr(552));
            ltypes.Add(Translator.TranslateStr(556));
            lhints.Add("refcommon.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add(PrintPosItemObject.PosY);
            // Align
            lnames.Add(Translator.TranslateStr(623));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refcommon.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add((int)PrintPosItemObject.Align);

        }
        public static PrintItemAlign StringToPrintItemAlign(string text)
        {
            if (text == Translator.TranslateStr(621))
                return PrintItemAlign.Bottom;
            else
                if (text == Translator.TranslateStr(622))
                return PrintItemAlign.Right;
            else
                    if (text == (Translator.TranslateStr(621) + "/" + Translator.TranslateStr(622)))
                return PrintItemAlign.BottomRight;
            else
                        if (text == Translator.TranslateStr(1224))
                return PrintItemAlign.LeftRight;
            else
                            if (text == Translator.TranslateStr(1225))
                return PrintItemAlign.TopBottom;
            else
                                if (text == Translator.TranslateStr(1226))
                return PrintItemAlign.AllClient;
            else
                return PrintItemAlign.None;
        }
        public static string PrintItemAlignToString(PrintItemAlign align)
        {
            string aresult = "";
            switch (align)
            {
                case PrintItemAlign.None:
                    aresult = Translator.TranslateStr(294);
                    break;
                case PrintItemAlign.Bottom:
                    aresult = Translator.TranslateStr(621);
                    break;
                case PrintItemAlign.Right:
                    aresult = Translator.TranslateStr(622);
                    break;
                case PrintItemAlign.BottomRight:
                    aresult = Translator.TranslateStr(621) + "/" + Translator.TranslateStr(622);
                    break;
                case PrintItemAlign.LeftRight:
                    aresult = Translator.TranslateStr(1224);
                    break;
                case PrintItemAlign.TopBottom:
                    aresult = Translator.TranslateStr(1225);
                    break;
                case PrintItemAlign.AllClient:
                    aresult = Translator.TranslateStr(1226);
                    break;
            }
            return aresult;
        }
        public override void GetPropertyValues(string pname, Strings lpossiblevalues)
        {
            // Align
            if (pname == Translator.TranslateStr(623))
            {
                lpossiblevalues.Add(Translator.TranslateStr(294));
                lpossiblevalues.Add(Translator.TranslateStr(621));
                lpossiblevalues.Add(Translator.TranslateStr(622));
                lpossiblevalues.Add(Translator.TranslateStr(621) + "/" + Translator.TranslateStr(622));
                lpossiblevalues.Add(Translator.TranslateStr(1224));
                lpossiblevalues.Add(Translator.TranslateStr(1225));
                lpossiblevalues.Add(Translator.TranslateStr(1226));
                return;
            }
            base.GetPropertyValues(pname, lpossiblevalues);
        }
        public override void SetProperty(string pname, Variant newvalue)
        {
            // Left/PosX
            if (pname == Translator.TranslateStr(553))
            {
                PrintPosItemObject.PosX = newvalue;
                return;
            }
            // Top/PosY
            if (pname == Translator.TranslateStr(552))
            {
                PrintPosItemObject.PosY = newvalue;
                return;
            }
            // Align
            if (pname == Translator.TranslateStr(623))
            {
                PrintPosItemObject.Align = (PrintItemAlign)(int)newvalue;
                return;
            }
            // inherited
            base.SetProperty(pname, newvalue);
        }
        public override void SetItem(ReportItem nitem)
        {
            FPrintPosItemObject = (PrintPosItem)nitem;
            base.SetItem(nitem);
        }
        public override Variant GetProperty(string pname)
        {
            // Left/PosX
            if (pname == Translator.TranslateStr(553))
                return PrintPosItemObject.PosX;
            // Top/PosY
            if (pname == Translator.TranslateStr(552))
                return PrintPosItemObject.PosY;
            // Align
            if (pname == Translator.TranslateStr(623))
                return (int)PrintPosItemObject.Align;
            // inherited
            return base.GetProperty(pname);
        }
    }


}
