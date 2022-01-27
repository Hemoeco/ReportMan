using System;
using System.Collections.Generic;
using System.Linq;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Designer
{
    internal class DesignerInterfaceImage : DesignerInterfaceSizePos
    {
        private ImageItem FPrintItemImage;
        public DesignerInterfaceImage(SortedList<int, ReportItem> repitem, IObjectInspector objinsp)
            : base(repitem, objinsp)
        {
            if (repitem.Count == 1)
                FPrintItemImage = (ImageItem)repitem.Values[0];
        }
        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
            base.GetProperties(lnames, ltypes, lvalues, lhints, lcat);

            // DrawStyle
            lnames.Add(Translator.TranslateStr(667));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refimage.html");
            lcat.Add(Translator.TranslateStr(639));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemImage.DrawStyle);
            // Expression
            lnames.Add(Translator.TranslateStr(571));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refimage.html");
            lcat.Add(Translator.TranslateStr(639));
            if (lvalues != null)
                lvalues.Add(FPrintItemImage.Expression);
            // Image
            lnames.Add(Translator.TranslateStr(639));
            ltypes.Add(Translator.TranslateStr(639));
            lhints.Add("refimage.html");
            lcat.Add(Translator.TranslateStr(639));
            Variant nvar = new Variant();
            nvar.SetStream(FPrintItemImage.Stream);
            if (lvalues != null)
                lvalues.Add(nvar);
            // DPIRes
            lnames.Add(Translator.TranslateStr(666));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refimage.html");
            lcat.Add(Translator.TranslateStr(639));
            if (lvalues != null)
                lvalues.Add(FPrintItemImage.dpires);
            // Cached
            lnames.Add(Translator.TranslateStr(1409));
            ltypes.Add(Translator.TranslateStr(1409));
            lhints.Add("refimage.html");
            lcat.Add(Translator.TranslateStr(639));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemImage.SharedImage);
        }
        public override Variant GetProperty(string pname)
        {
            // DrawStyle
            if (pname == Translator.TranslateStr(667))
            {
                return (int)FPrintItemImage.DrawStyle;
            }
            // Expression
            if (pname == Translator.TranslateStr(571))
            {
                return FPrintItemImage.Expression;
            }
            // Image
            if (pname == Translator.TranslateStr(639))
            {
                Variant nvar = new Variant();
                nvar.SetStream(FPrintItemImage.Stream);
                return nvar;
            }
            // DPIRes
            if (pname == Translator.TranslateStr(666))
            {
                return FPrintItemImage.dpires;
            }
            // Cached
            if (pname == Translator.TranslateStr(1409))
            {
                return (int)FPrintItemImage.SharedImage;
            }

            return base.GetProperty(pname);
        }
        public override void GetPropertyValues(string pname, Strings lpossiblevalues)
        {
            // DrawStyle
            if (pname == Translator.TranslateStr(667))
            {
                lpossiblevalues.Add(Translator.TranslateStr(633)); // Center
                lpossiblevalues.Add(Translator.TranslateStr(637)); // Stretch
                lpossiblevalues.Add(Translator.TranslateStr(638)); // Full
                lpossiblevalues.Add(Translator.TranslateStr(668)); // Tile
                lpossiblevalues.Add(Translator.TranslateStr(1326)); // Tiled dpi
                return;
            }
            // Cached
            if (pname == Translator.TranslateStr(1409))
            {
                lpossiblevalues.Add(Translator.TranslateStr(294)); // None
                lpossiblevalues.Add(Translator.TranslateStr(1420)); // Fixed
                lpossiblevalues.Add(Translator.TranslateStr(1421)); // Variable
            }
            base.GetPropertyValues(pname, lpossiblevalues);
        }

        public override void SetProperty(string pname, Variant newvalue)
        {
            // DrawStyle
            if (pname == Translator.TranslateStr(667))
            {
                FPrintItemImage.DrawStyle = (ImageDrawStyleType)(int)newvalue;
                return;
            }
            // Expression
            if (pname == Translator.TranslateStr(571))
            {
                FPrintItemImage.Expression = newvalue.ToString();
                return;
            }
            // Image
            if (pname == Translator.TranslateStr(639))
            {
                FPrintItemImage.Stream.SetLength(0);
                byte[] narray = newvalue.GetStream().ToArray();
                FPrintItemImage.Stream.Write(narray, 0, narray.Length);
                return;
            }
            // DPIRes
            if (pname == Translator.TranslateStr(666))
            {
                FPrintItemImage.dpires = (int)newvalue;
                return;
            }
            // Cached
            if (pname == Translator.TranslateStr(1409))
            {
                FPrintItemImage.SharedImage = (SharedImageType)(int)newvalue;
            }
            // inherited
            base.SetProperty(pname, newvalue);
        }
    }

}
