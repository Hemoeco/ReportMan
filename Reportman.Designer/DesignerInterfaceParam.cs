using System;
using System.Collections.Generic;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Designer
{
    internal class DesignerInterfaceParam : DesignerInterface
    {
        private Param FParamItem;
        public DesignerInterfaceParam(SortedList<int, ReportItem> repitem, IObjectInspector objinsp) : base(repitem, objinsp)
        {
            if (repitem.Count == 1)
                FParamItem = (Param)repitem.Values[0];
        }
        public override void SetItem(ReportItem nitem)
        {
            FParamItem = (Param)nitem;
            base.SetItem(nitem);
        }
        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
            lnames.Clear();
            ltypes.Clear();
            lhints.Clear();
            lcat.Clear();
            if (lvalues != null)
                lvalues.Clear();
            // Name
            lnames.Add(Translator.TranslateStr(544));
            ltypes.Add(Translator.TranslateStr(200));
            lhints.Add("refparam.html");
            lcat.Add(Translator.TranslateStr(722));
            if (lvalues != null)
                lvalues.Add(FParamItem.Alias);
            // Param type
            lnames.Add(Translator.TranslateStr(193));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refparam.html");
            lcat.Add(Translator.TranslateStr(722));
            if (lvalues != null)
                lvalues.Add((int)FParamItem.ParamType);
            // Value
            lnames.Add(Translator.TranslateStr(194));
            ltypes.Add(Translator.TranslateStr(135));
            lhints.Add("refparam.html");
            lcat.Add(Translator.TranslateStr(722));
            if (lvalues != null)
                lvalues.Add(FParamItem.Value);
            // Visible
            lnames.Add(Translator.TranslateStr(183));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refparam.html");
            lcat.Add(Translator.TranslateStr(722));
            if (lvalues != null)
                lvalues.Add(FParamItem.Visible);
            // Description
            lnames.Add(Translator.TranslateStr(197));
            ltypes.Add(Translator.TranslateStr(200));
            lhints.Add("refparam.html");
            lcat.Add(Translator.TranslateStr(722));
            if (lvalues != null)
                lvalues.Add(FParamItem.Description);
            // Hint
            lnames.Add(Translator.TranslateStr(1382));
            ltypes.Add(Translator.TranslateStr(200));
            lhints.Add("refparam.html");
            lcat.Add(Translator.TranslateStr(722));
            if (lvalues != null)
                lvalues.Add(FParamItem.Hint);
            // Validation
            lnames.Add(Translator.TranslateStr(1401));
            ltypes.Add(Translator.TranslateStr(200));
            lhints.Add("refparam.html");
            lcat.Add(Translator.TranslateStr(722));
            if (lvalues != null)
                lvalues.Add(FParamItem.Validation);
            // Error Message
            lnames.Add(Translator.TranslateStr(1403));
            ltypes.Add(Translator.TranslateStr(200));
            lhints.Add("refparam.html");
            lcat.Add(Translator.TranslateStr(722));
            if (lvalues != null)
                lvalues.Add(FParamItem.ErrorMessage);
            // Datasets
            lnames.Add(Translator.TranslateStr(198));
            ltypes.Add(Translator.TranslateStr(198));
            lhints.Add("refparam.html");
            lcat.Add(Translator.TranslateStr(722));
            if (lvalues != null)
                lvalues.Add(FParamItem.Datasets.ToSemiColon());
            // Search for string 
            lnames.Add(Translator.TranslateStr(946));
            ltypes.Add(Translator.TranslateStr(200));
            lhints.Add("refparam.html");
            lcat.Add(Translator.TranslateStr(722));
            if (lvalues != null)
                lvalues.Add(FParamItem.Search);

        }
        public override void GetPropertyValues(string pname, Strings lpossiblevalues)
        {
            // Alignment
            if (pname == Translator.TranslateStr(193))
            {
                lpossiblevalues.Add(Translator.TranslateStr(557)); // String
                lpossiblevalues.Add(Translator.TranslateStr(559)); // Integer
                lpossiblevalues.Add(Translator.TranslateStr(887)); // Float
                lpossiblevalues.Add(Translator.TranslateStr(888)); // Date
                lpossiblevalues.Add(Translator.TranslateStr(890)); // Time
                lpossiblevalues.Add(Translator.TranslateStr(889)); // DateTime
                lpossiblevalues.Add(Translator.TranslateStr(556)); // Currency
                lpossiblevalues.Add(Translator.TranslateStr(891)); // Bool
                lpossiblevalues.Add(Translator.TranslateStr(942)); // Expression Before open
                lpossiblevalues.Add(Translator.TranslateStr(943)); // Expression After open
                lpossiblevalues.Add(Translator.TranslateStr(951)); // Sustitution parameter
                lpossiblevalues.Add(Translator.TranslateStr(961)); // Param list
                lpossiblevalues.Add(Translator.TranslateStr(1368)); // Param list multiple
                lpossiblevalues.Add(Translator.TranslateStr(1422)); // string sustituion expression
                lpossiblevalues.Add(Translator.TranslateStr(1442)); // string sustitution list
                lpossiblevalues.Add(Translator.TranslateStr(1443)); // Initial expression
                lpossiblevalues.Add(Translator.TranslateStr(575));// Unkonwn
                return;
            }
        }

        public override void SetProperty(string pname, Variant newvalue)
        {
            // Name
            if (pname == Translator.TranslateStr(544))
            {
                FParamItem.Name = newvalue.ToString();
                return;
            }
            // Param type
            if (pname == Translator.TranslateStr(193))
            {
                FParamItem.ParamType = (ParamType)(int)newvalue;
                return;
            }
            // Value
            if (pname == Translator.TranslateStr(194))
            {
                switch (FParamItem.ParamType)
                {
                    case ParamType.Date:
                        newvalue = newvalue.AsDateTime;
                        FParamItem.Value = newvalue;
                        FParamItem.ParamType = ParamType.Date;
                        break;
                    case ParamType.DateTime:
                        newvalue = newvalue.AsDateTime;
                        FParamItem.Value = newvalue;
                        FParamItem.ParamType = ParamType.DateTime;
                        break;
                    default:
                        FParamItem.Value = newvalue;
                        break;
                }
                return;
            }
            // Visible
            if (pname == Translator.TranslateStr(183))
            {
                FParamItem.Visible = (bool)newvalue;
                return;
            }
            // Description
            if (pname == Translator.TranslateStr(197))
            {
                FParamItem.Description = newvalue.ToString();
                return;
            }
            // Hint
            if (pname == Translator.TranslateStr(1382))
            {
                FParamItem.Description = newvalue.ToString();
                return;
            }
            // Validation
            if (pname == Translator.TranslateStr(1401))
            {
                FParamItem.Validation = newvalue.ToString();
                return;
            }
            // Error message
            if (pname == Translator.TranslateStr(1403))
            {
                FParamItem.ErrorMessage = newvalue.ToString();
                return;
            }
            // Search
            if (pname == Translator.TranslateStr(946))
            {
                FParamItem.Search = newvalue.ToString();
                return;
            }
            // Datasets
            if (pname == Translator.TranslateStr(198))
            {
                FParamItem.Datasets = Strings.FromSemiColon(newvalue); ;
                return;
            }
            base.SetProperty(pname, newvalue);
        }
        public override Variant GetProperty(string pname)
        {
            // Name
            if (pname == Translator.TranslateStr(544))
            {
                return FParamItem.Name;
            }
            // Param type
            if (pname == Translator.TranslateStr(193))
            {
                return (int)FParamItem.ParamType;
            }
            // Value
            if (pname == Translator.TranslateStr(194))
            {
                return FParamItem.Value;
            }
            // Visible
            if (pname == Translator.TranslateStr(183))
            {
                return FParamItem.Visible;
            }
            // Description
            if (pname == Translator.TranslateStr(197))
            {
                return FParamItem.Description;
            }
            // Hint
            if (pname == Translator.TranslateStr(1382))
            {
                return FParamItem.Hint;
            }
            // Validation
            if (pname == Translator.TranslateStr(1401))
            {
                return FParamItem.Validation;
            }
            // Error message
            if (pname == Translator.TranslateStr(1403))
            {
                return FParamItem.ErrorMessage;
            }
            // Error message
            if (pname == Translator.TranslateStr(946))
            {
                return FParamItem.Search;
            }

            // Datasets
            if (pname == Translator.TranslateStr(198))
            {
                return FParamItem.Datasets.ToSemiColon();
            }
            return base.GetProperty(pname);
        }
    }
}
