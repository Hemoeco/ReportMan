using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Designer
{
    class DesignerInterfaceBarcode : DesignerInterfaceSizePos
    {
        private BarcodeItem FPrintItemBarcode;
        public DesignerInterfaceBarcode(SortedList<int, ReportItem> repitem, IObjectInspector objinsp)
            : base(repitem, objinsp)
        {
            if (repitem.Count == 1)
                FPrintItemBarcode = (BarcodeItem)repitem.Values[0];
        }
        public BarcodeItem PrintItemShapeObject { get { return FPrintItemBarcode; } }

        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
            base.GetProperties(lnames, ltypes, lvalues, lhints, lcat);

            // Expression
            lnames.Add(Translator.TranslateStr(571));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add(FPrintItemBarcode.Expression);
            // Barcode Type
            lnames.Add(Translator.TranslateStr(577));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemBarcode.BarType);
            // Checksum
            lnames.Add(Translator.TranslateStr(579));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add(FPrintItemBarcode.Checksum);
            // Modul
            lnames.Add(Translator.TranslateStr(580));
            ltypes.Add(Translator.TranslateStr(556));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add(FPrintItemBarcode.Modul);
            // Ratio
            lnames.Add(Translator.TranslateStr(581));
            ltypes.Add(Translator.TranslateStr(1171));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add(FPrintItemBarcode.Ratio);
            // Display Format
            lnames.Add(Translator.TranslateStr(574));
            ltypes.Add(Translator.TranslateStr(557));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add(FPrintItemBarcode.DisplayFormat);
            // Rotation
            lnames.Add(Translator.TranslateStr(908));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add(Convert.ToInt32(FPrintItemBarcode.Rotation / 10 / 90));
            // Brush Color
            lnames.Add(Translator.TranslateStr(564));
            ltypes.Add(Translator.TranslateStr(558));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemBarcode.BColor);
            // Back Color
            lnames.Add(Translator.TranslateStr(565));
            ltypes.Add(Translator.TranslateStr(558));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemBarcode.BackColor);
            // Transparent
            lnames.Add(Translator.TranslateStr(567));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add(FPrintItemBarcode.Transparent);
            // ECC Level Check 
            lnames.Add(Translator.TranslateStr(1335));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemBarcode.ECCLevel + 1);
            // Num Rows
            lnames.Add(Translator.TranslateStr(1336));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemBarcode.NumRows);
            // Num Cols
            lnames.Add(Translator.TranslateStr(1337));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemBarcode.NumColumns);
            // Truncate
            lnames.Add(Translator.TranslateStr(1338));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refbarcode.html");
            lcat.Add(Translator.TranslateStr(1209));
            if (lvalues != null)
                lvalues.Add(FPrintItemBarcode.Truncated);
        }
        public override Variant GetProperty(string pname)
        {
            // Expression
            if (pname == Translator.TranslateStr(571))
                return FPrintItemBarcode.Expression;
            // Barcode Type
            if (pname == Translator.TranslateStr(577))
                return (int)FPrintItemBarcode.BarType;
            // Checksum
            if (pname == Translator.TranslateStr(579))
                return FPrintItemBarcode.Checksum;
            // Transparent
            if (pname == Translator.TranslateStr(567))
                return FPrintItemBarcode.Transparent;
            // Modul
            if (pname == Translator.TranslateStr(580))
                return FPrintItemBarcode.Modul;
            // Ratio
            if (pname == Translator.TranslateStr(581))
                return FPrintItemBarcode.Ratio;
            // Display Format
            if (pname == Translator.TranslateStr(574))
                return FPrintItemBarcode.DisplayFormat;
            // Rotation
            if (pname == Translator.TranslateStr(908))
                return Convert.ToInt32(FPrintItemBarcode.Rotation / 10 / 90);
            // Brush Color
            if (pname == Translator.TranslateStr(564))
                return FPrintItemBarcode.BColor;
            // Back Color
            if (pname == Translator.TranslateStr(565))
                return FPrintItemBarcode.BackColor;
            // ECC Level Check 
            if (pname == Translator.TranslateStr(1335))
                return FPrintItemBarcode.ECCLevel + 1;
            // Num Rows
            if (pname == Translator.TranslateStr(1336))
                return FPrintItemBarcode.NumRows;
            // Num Cols
            if (pname == Translator.TranslateStr(1337))
                return FPrintItemBarcode.NumColumns;
            // Truncate
            if (pname == Translator.TranslateStr(1338))
                return FPrintItemBarcode.Truncated;

            return base.GetProperty(pname);
        }
        public override void SetProperty(string pname, Variant newvalue)
        {
            // Expression
            if (pname == Translator.TranslateStr(571))
            {
                FPrintItemBarcode.Expression = newvalue;
            }
            else
            // Barcode Type
            if (pname == Translator.TranslateStr(577))
            {
                FPrintItemBarcode.BarType = (BarcodeType)(int)newvalue;
            }
            else
            // Checksum
            if (pname == Translator.TranslateStr(579))
            {
                FPrintItemBarcode.Checksum = newvalue;
            }
            else
            if (pname == Translator.TranslateStr(567))
            {
                FPrintItemBarcode.Transparent = newvalue;
            }
            else
            // Modul
            if (pname == Translator.TranslateStr(580))
            {
                FPrintItemBarcode.Modul = newvalue;
            }
            else
            // Ratio
            if (pname == Translator.TranslateStr(581))
            {
                FPrintItemBarcode.Ratio = newvalue;
            }
            else
            // Display Format
            if (pname == Translator.TranslateStr(574))
            {
                FPrintItemBarcode.DisplayFormat = newvalue;
            }
            else
            // Rotation
            if (pname == Translator.TranslateStr(908))
            {
                switch (newvalue.AsInteger)
                {
                    case 1:
                        FPrintItemBarcode.Rotation = 900;
                        break;
                    case 2:
                        FPrintItemBarcode.Rotation = 1800;
                        break;
                    case 3:
                        FPrintItemBarcode.Rotation = 2700;
                        break;
                    default:
                        FPrintItemBarcode.Rotation = 0;
                        break;
                }
            }
            else
            // Brush Color
            if (pname == Translator.TranslateStr(564))
            {
                FPrintItemBarcode.BColor = newvalue;
            }
            else
            // Back Color
            if (pname == Translator.TranslateStr(565))
            {
                FPrintItemBarcode.BackColor = newvalue;
            }
            else
            // ECC Level Check 
            if (pname == Translator.TranslateStr(1335))
            {
                FPrintItemBarcode.ECCLevel = newvalue - 1;
            }
            else
            // Num Rows
            if (pname == Translator.TranslateStr(1336))
            {
                FPrintItemBarcode.NumRows = newvalue;
            }
            else
            // Num Cols
            if (pname == Translator.TranslateStr(1337))
            {
                FPrintItemBarcode.NumColumns = newvalue;
            }
            else
            // Truncate
            if (pname == Translator.TranslateStr(1338))
            {
                FPrintItemBarcode.Truncated = newvalue;
            }
            else
                // inherited
                base.SetProperty(pname, newvalue);
        }

        public override void GetPropertyValues(string pname, Strings lpossiblevalues)
        {
            // Barcode Type
            if (pname == Translator.TranslateStr(577))
            {
                lpossiblevalues.Add("Code 2 5 interleaved");
                lpossiblevalues.Add("Code 2 5 industrial");
                lpossiblevalues.Add("Code 2 5 matrix");
                lpossiblevalues.Add("Code 39");
                lpossiblevalues.Add("Code 39 Extended");
                lpossiblevalues.Add("Code 128A");
                lpossiblevalues.Add("Code 128B");
                lpossiblevalues.Add("Code 128C");
                lpossiblevalues.Add("Code128");
                lpossiblevalues.Add("Code 93");
                lpossiblevalues.Add("Code 93 Extended");
                lpossiblevalues.Add("Code MSI");
                lpossiblevalues.Add("Code PostNet");
                lpossiblevalues.Add("Code CodaBar");
                lpossiblevalues.Add("Code EAN8");
                lpossiblevalues.Add("Code EAN13");
                lpossiblevalues.Add("Code PDF417");
                lpossiblevalues.Add("Code QR");
            }
            else
            // Rotation
            if (pname == Translator.TranslateStr(908))
            {
                lpossiblevalues.Add("0");
                lpossiblevalues.Add("90");
                lpossiblevalues.Add("180");
                lpossiblevalues.Add("270");
            }
            else
            // ECC Level
            if (pname == Translator.TranslateStr(1335))
            {
                lpossiblevalues.Add("Auto");
                lpossiblevalues.Add("Level 0");
                lpossiblevalues.Add("Level 1");
                lpossiblevalues.Add("Level 2");
                lpossiblevalues.Add("Level 3");
                lpossiblevalues.Add("Level 4");
                lpossiblevalues.Add("Level 5");
                lpossiblevalues.Add("Level 6");
                lpossiblevalues.Add("Level 7");
                lpossiblevalues.Add("Level 8");
            }
            else
                base.GetPropertyValues(pname, lpossiblevalues);
        }


    }

}
