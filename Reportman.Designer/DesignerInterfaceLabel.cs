using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reportman.Reporting;
using Reportman.Drawing;

namespace Reportman.Designer
{
    class DesignerInterfaceLabel : DesignerInterfaceText
    {
        private LabelItem FPrintItemLabel;
        public DesignerInterfaceLabel(SortedList<int, ReportItem> repitem, IObjectInspector objinsp)
            : base(repitem,objinsp)
        {
            if (repitem.Count >= 0)
                FPrintItemLabel = (LabelItem)repitem.Values[0];

        }
        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
            base.GetProperties(lnames, ltypes, lvalues, lhints, lcat);

            // Text
            lnames.Add(Translator.TranslateStr(570));
            ltypes.Add(Translator.TranslateStr(557));
            lhints.Add("reflabel.html");
            lcat.Add(Translator.TranslateStr(1203));
            if (lvalues != null)
                lvalues.Add(FPrintItemLabel.Text);

        }
        public override Variant GetProperty(string pname)
        {
            // Expression
            if (pname == Translator.TranslateStr(570))
                return FPrintItemLabel.Text;

            return base.GetProperty(pname);
        }
        public override void SetProperty(string pname, Variant newvalue)
        {
            // Text
            if (pname == Translator.TranslateStr(570))
            {
                FPrintItemLabel.Text = newvalue;
            }
            else
                // inherited
                base.SetProperty(pname, newvalue);
        }


    }
}
