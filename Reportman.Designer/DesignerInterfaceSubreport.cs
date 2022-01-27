using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reportman.Reporting;
using Reportman.Drawing;

namespace Reportman.Designer
{
    internal class DesignerInterfaceSubReport : DesignerInterface
    {
        SubReport fsubreport;
        public DesignerInterfaceSubReport(SortedList<int, ReportItem> repitem, IObjectInspector objinsp) : base(repitem, objinsp)
        {
            if (repitem.Count == 1)
                fsubreport = (SubReport)repitem.Values[0];
        }
        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {

            base.GetProperties(lnames, ltypes, lvalues, lhints, lcat);
            fsubreport = (SubReport)ReportItemObject;
            // Main dataset
            lnames.Add(Translator.TranslateStr(275));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refsubreport.html");
            //lcat.Add(Translator.TranslateStr(280));
            if (lvalues != null)
                lvalues.Add(fsubreport.Alias);
            // Print only if data available
            lnames.Add(Translator.TranslateStr(800));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refsubreport.html");
            //lcat.Add(Translator.TranslateStr(280));
            if (lvalues != null)
                lvalues.Add(fsubreport.PrintOnlyIfDataAvailable);

        }
        public override void GetPropertyValues(string pname, Strings lpossiblevalues)
        {
            if (pname == Translator.TranslateStr(275))
            {
                fsubreport = (SubReport)ReportItemObject;
                lpossiblevalues.Add("");
                foreach (DataInfo dinfo in fsubreport.Report.DataInfo)
                {
                    lpossiblevalues.Add(dinfo.Alias);
                }
                return;
            }
            base.GetPropertyValues(pname, lpossiblevalues);
        }
        public override void SetItem(ReportItem nitem)
        {
            fsubreport = (SubReport)nitem;
            base.SetItem(nitem);
        }
        public override Variant GetProperty(string pname)
        {
            if (pname == Translator.TranslateStr(275))
            {
                return fsubreport.Alias;
            }
            if (pname == Translator.TranslateStr(800))
            {
                return fsubreport.PrintOnlyIfDataAvailable;
            }

            return base.GetProperty(pname);
        }
        public override void SetProperty(string pname, Variant newvalue)
        {
            if (pname == Translator.TranslateStr(275))
            {
                if (newvalue.VarType == VariantType.String)
                {
                    fsubreport.Alias = newvalue;
                }
                else
                {
                    if (newvalue == 0)
                        fsubreport.Alias = "";
                    else
                        fsubreport.Alias = this.fsubreport.Report.DataInfo[(int)newvalue - 1].Alias;
                }

                // Refresh the interface of structure
                FObjectInspector.Structure.RefreshSubReport(fsubreport);
                return;
            }
            if (pname == Translator.TranslateStr(800))
            {
                fsubreport.PrintOnlyIfDataAvailable = newvalue;
                return;
            }
            base.SetProperty(pname, newvalue);
        }
    }

}
