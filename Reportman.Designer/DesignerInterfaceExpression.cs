using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Designer
{
    class DesignerInterfaceExpression : DesignerInterfaceText
    {
        private ExpressionItem FPrintItemExpression;
        public DesignerInterfaceExpression(SortedList<int, ReportItem> repitem, IObjectInspector objinsp)
            : base(repitem,objinsp)
        {
            if (repitem.Count >= 0)
                FPrintItemExpression = (ExpressionItem)repitem.Values[0];
        }
        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
            base.GetProperties(lnames, ltypes, lvalues, lhints, lcat);

            // Expression
            lnames.Add(Translator.TranslateStr(571));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refexpression.html");
            lcat.Add(Translator.TranslateStr(571));
            if (lvalues != null)
                lvalues.Add(FPrintItemExpression.Expression);

            // DataType
            lnames.Add(Translator.TranslateStr(892));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refexpression.html");
            lcat.Add(Translator.TranslateStr(571));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemExpression.DataType);

            // DisplayFormat
            lnames.Add(Translator.TranslateStr(574));
            ltypes.Add(Translator.TranslateStr(557));
            lhints.Add("refexpression.html");
            lcat.Add(Translator.TranslateStr(571));
            if (lvalues != null)
                lvalues.Add(FPrintItemExpression.DisplayFormat);

            // Multipage
            lnames.Add(Translator.TranslateStr(958));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refexpression.html");
            lcat.Add(Translator.TranslateStr(571));
            if (lvalues != null)
                lvalues.Add(FPrintItemExpression.MultiPage);

            // Print Nulls
            lnames.Add(Translator.TranslateStr(941));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refexpression.html");
            lcat.Add(Translator.TranslateStr(571));
            if (lvalues != null)
                lvalues.Add(FPrintItemExpression.PrintNulls);
            // Identifier
            lnames.Add(Translator.TranslateStr(304));
            ltypes.Add(Translator.TranslateStr(557));
            lhints.Add("refexpression.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemExpression.Identifier);

            // Aggregate
            lnames.Add(Translator.TranslateStr(293));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refexpression.html");
            lcat.Add(Translator.TranslateStr(571));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemExpression.Aggregate);

            // Aggregate group
            lnames.Add(Translator.TranslateStr(296));
            ltypes.Add(Translator.TranslateStr(961));
            lhints.Add("refexpression.html");
            lcat.Add(Translator.TranslateStr(571));
            if (lvalues != null)
                lvalues.Add(FPrintItemExpression.GroupName);

            // Aggregate Type
            lnames.Add(Translator.TranslateStr(297));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refexpression.html");
            lcat.Add(Translator.TranslateStr(571));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemExpression.AgType);

            // Aggregate Initial Expression
            lnames.Add(Translator.TranslateStr(303));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refexpression.html");
            lcat.Add(Translator.TranslateStr(571));
            if (lvalues != null)
                lvalues.Add(FPrintItemExpression.AgIniValue);
            // Print Only One
            lnames.Add(Translator.TranslateStr(576));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refexpression.html");
            lcat.Add(Translator.TranslateStr(571));
            if (lvalues != null)
                lvalues.Add(FPrintItemExpression.PrintOnlyOne);

        }
        public override void GetPropertyValues(string pname, Strings lpossiblevalues)
        {
            // Group Name
            if (pname == Translator.TranslateStr(296))
            {
                lpossiblevalues.Add(""); // None
                foreach (Section nsection in FPrintItemExpression.Section.SubReport.Sections)
                {
                    if (nsection.SectionType == SectionType.GroupHeader)
                    {
                        lpossiblevalues.Add(nsection.GroupName);
                    }
                }
                return;
            }
            // Data Type
            if (pname == Translator.TranslateStr(892))
            {
                lpossiblevalues.Add(Translator.TranslateStr(886)); // Unknown
                lpossiblevalues.Add(Translator.TranslateStr(557)); // String
                lpossiblevalues.Add(Translator.TranslateStr(559)); // Integer
                lpossiblevalues.Add(Translator.TranslateStr(887)); // Float
                lpossiblevalues.Add(Translator.TranslateStr(556)); // Currency
                lpossiblevalues.Add(Translator.TranslateStr(888)); // Date
                lpossiblevalues.Add(Translator.TranslateStr(889)); // DateTime
                lpossiblevalues.Add(Translator.TranslateStr(890)); // Time
                lpossiblevalues.Add(Translator.TranslateStr(891)); // Boolean

                return;
            }
            // Aggregate
            if (pname == Translator.TranslateStr(293))
            {
                lpossiblevalues.Add(Translator.TranslateStr(294)); // None 0
                lpossiblevalues.Add(Translator.TranslateStr(296)); // Group 1
                lpossiblevalues.Add(Translator.TranslateStr(269)); // Page 2
                lpossiblevalues.Add(Translator.TranslateStr(295)); // General 3

                return;
            }
            // Aggregate Type
            if (pname == Translator.TranslateStr(297))
            {
                lpossiblevalues.Add(Translator.TranslateStr(298)); // Sum 0
                lpossiblevalues.Add(Translator.TranslateStr(299)); // Minimum 1
                lpossiblevalues.Add(Translator.TranslateStr(300)); // Maximum 2
                lpossiblevalues.Add(Translator.TranslateStr(301)); // Average 3
                lpossiblevalues.Add(Translator.TranslateStr(302)); // Std Dev 4

                return;
            }

            base.GetPropertyValues(pname, lpossiblevalues);
        }

        public override Variant GetProperty(string pname)
        {
            // Expression
            if (pname == Translator.TranslateStr(571))
                return FPrintItemExpression.Expression;
            // DataType
            if (pname == Translator.TranslateStr(892))
                return (int)FPrintItemExpression.DataType;
            // DisplayFormat
            if (pname == Translator.TranslateStr(574))
                return FPrintItemExpression.DisplayFormat;
            // MultiPage
            if (pname == Translator.TranslateStr(958))
                return FPrintItemExpression.MultiPage;
            // PrintNulls
            if (pname == Translator.TranslateStr(941))
                return FPrintItemExpression.PrintNulls;
            // Identifier
            if (pname == Translator.TranslateStr(304))
                return FPrintItemExpression.Identifier;
            // Aggregate
            if (pname == Translator.TranslateStr(293))
                return (int)FPrintItemExpression.Aggregate;
            // Aggregate Group
            if (pname == Translator.TranslateStr(296))
                return FPrintItemExpression.GroupName;
            // Aggregate Type
            if (pname == Translator.TranslateStr(297))
                return (int)FPrintItemExpression.AgType;
            // Aggregate Initial Expression
            if (pname == Translator.TranslateStr(303))
                return FPrintItemExpression.AgIniValue;
            // Print Only One
            if (pname == Translator.TranslateStr(576))
                return FPrintItemExpression.PrintOnlyOne;

            return base.GetProperty(pname);
        }
        public override void SetProperty(string pname, Variant newvalue)
        {
            // Expression
            if (pname == Translator.TranslateStr(571))
            {
                FPrintItemExpression.Expression = newvalue;
            }
            else
            // DataType
            if (pname == Translator.TranslateStr(892))
            {
                FPrintItemExpression.DataType = (ParamType)(int)newvalue;
            }
            else
            // DisplayFormat
            if (pname == Translator.TranslateStr(574))
            {
                FPrintItemExpression.DisplayFormat = newvalue;
            }
            else
            // MultiPage
            if (pname == Translator.TranslateStr(958))
            {
                FPrintItemExpression.MultiPage = newvalue;
            }
            else
            // PrintNulls
            if (pname == Translator.TranslateStr(941))
            {
                FPrintItemExpression.PrintNulls = newvalue;
            }
            else
            // Identifier
            if (pname == Translator.TranslateStr(304))
            {
                FPrintItemExpression.Identifier = newvalue;
            }
            else
            // Aggregate
            if (pname == Translator.TranslateStr(293))
            {
                FPrintItemExpression.Aggregate = (Aggregate)(int)newvalue;
            }
            else
            // Group name
            if (pname == Translator.TranslateStr(296))
            {
                if (newvalue.IsInteger())
                {
                    FPrintItemExpression.AgType = (AggregateType)(int)newvalue;
                    Strings lpossiblevalues = new Strings
                    {
                        "" // None
                    };
                    foreach (Section nsection in FPrintItemExpression.Section.SubReport.Sections)
                    {
                        if (nsection.SectionType == SectionType.GroupHeader)
                        {
                            lpossiblevalues.Add(nsection.GroupName);
                        }
                    }
                    FPrintItemExpression.GroupName = lpossiblevalues[(int)newvalue];
                }
                else
                    FPrintItemExpression.GroupName = newvalue;
            }
            else
            // Aggregate Type
            if (pname == Translator.TranslateStr(297))
            {
                FPrintItemExpression.AgType = (AggregateType)(int)newvalue;
            }
            else
            // Group Initial Expression
            if (pname == Translator.TranslateStr(303))
            {
                FPrintItemExpression.AgIniValue = newvalue;
            }
            else
            if (pname == Translator.TranslateStr(576))
            {
                FPrintItemExpression.PrintOnlyOne = newvalue;
            }
            else
                // inherited
                base.SetProperty(pname, newvalue);
        }
    }
}
