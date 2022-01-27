using System;
using System.Collections.Generic;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Designer
{
    internal class DesignerInterfaceSize : DesignerInterface
    {
        private PrintItem FPrintItemObject;
        public DesignerInterfaceSize(SortedList<int, ReportItem> repitem, IObjectInspector objinsp) : base(repitem, objinsp)
        {
            if (repitem.Count == 1)
                FPrintItemObject = (PrintItem)repitem.Values[0];
        }
        public PrintItem PrintItemObject { get { return FPrintItemObject; } }
        private int FLeft;
        private int FTop;
        private int FWidth;
        private int FHeight;
        public int Left { get { return FLeft; } }
        public int Top { get { return FTop; } }
        public int Width { get { return FWidth; } }
        public int Height { get { return FHeight; } }
        private double FScale;
        private bool FSelected;
        protected void SetSelected(bool newvalue)
        {
            FSelected = newvalue;
        }
        public void SetBounds(int newleft, int newwidth, int newtop, int newheight)
        {
            FLeft = newleft;
            FWidth = newwidth;
            FTop = newtop;
            FHeight = newheight;
        }
        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
            lnames.Clear();
            ltypes.Clear();
            lhints.Clear();
            lcat.Clear();
            if (lvalues != null)
                lvalues.Clear();
            // PrintCondition
            lnames.Add(Translator.TranslateStr(614));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refcommon.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add(PrintItemObject.PrintCondition);
            // Before Print
            lnames.Add(Translator.TranslateStr(613));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refcommon.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add(PrintItemObject.DoBeforePrint);
            // After Print
            lnames.Add(Translator.TranslateStr(612));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refcommon.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add(PrintItemObject.DoAfterPrint);

            // Width
            lnames.Add(Translator.TranslateStr(554));
            ltypes.Add(Translator.TranslateStr(556));
            lhints.Add("refcommon.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add(PrintItemObject.Width);
            // Height
            lnames.Add(Translator.TranslateStr(555));
            ltypes.Add(Translator.TranslateStr(556));
            lhints.Add("refcommon.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add(PrintItemObject.Height);
        }
        public override void SetItem(ReportItem nitem)
        {
            FPrintItemObject = (PrintItem)nitem;
            base.SetItem(nitem);
        }
        public override Variant GetProperty(string pname)
        {
            // Print condition
            if (pname == Translator.TranslateStr(614))
                return PrintItemObject.PrintCondition;
            // Before print
            if (pname == Translator.TranslateStr(613))
                return PrintItemObject.DoBeforePrint;
            // After print
            if (pname == Translator.TranslateStr(612))
                return PrintItemObject.DoAfterPrint;
            // Width
            if (pname == Translator.TranslateStr(554))
                return PrintItemObject.Width;
            // Height
            if (pname == Translator.TranslateStr(555))
                return PrintItemObject.Height;
            // inherited
            return base.GetProperty(pname);
        }
        public override void SetProperty(string pname, Variant newvalue)
        {
            // Print condition
            if (pname == Translator.TranslateStr(614))
            {
                PrintItemObject.PrintCondition = newvalue.ToString();
                return;
            }
            // Before print
            if (pname == Translator.TranslateStr(613))
            {
                PrintItemObject.DoBeforePrint = newvalue.ToString();
                return;
            }
            // After print
            if (pname == Translator.TranslateStr(612))
            {
                PrintItemObject.DoAfterPrint = newvalue.ToString();
                return;
            }
            // Width
            if (pname == Translator.TranslateStr(554))
            {
                PrintItemObject.Width = newvalue;
                return;
            }
            // Height
            if (pname == Translator.TranslateStr(555))
            {
                PrintItemObject.Height = newvalue;
                return;
            }
            // inherited
            base.SetProperty(pname, newvalue);
        }
    }

}
