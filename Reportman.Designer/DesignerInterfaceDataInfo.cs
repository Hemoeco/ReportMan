using System;
using System.Collections.Generic;
using System.Linq;
using Reportman.Reporting;
using Reportman.Drawing;

namespace Reportman.Designer
{
    internal class DesignerInterfaceDataInfo : DesignerInterface
    {
        private DataInfo FDataInfo;
        public DesignerInterfaceDataInfo(SortedList<int, ReportItem> repitem, IObjectInspector objinsp)
          : base(repitem, objinsp)
        {
            if (repitem.Count == 1)
                FDataInfo = (DataInfo)repitem.Values[0];
        }
        public DataInfo DataInfo { get { return FDataInfo; } }
        public Strings GetAvaliableConnections()
        {
            Strings lpossiblevalues = new Strings();
            foreach (DatabaseInfo dbinfo in this.ReportItemObject.Report.DatabaseInfo)
            {
                lpossiblevalues.Add(dbinfo.Alias);
            }
            return lpossiblevalues;
        }
        public Strings GetAvaliableMasterDataSets()
        {
            Strings lpossiblevalues = new Strings
            {
                ""
            };
            foreach (DataInfo dinfo in this.ReportItemObject.Report.DataInfo)
            {
                if (dinfo.Alias != FDataInfo.Alias)
                    lpossiblevalues.Add(dinfo.Alias);
            }
            return lpossiblevalues;
        }

        public override void GetPropertyValues(string pname, Strings lpossiblevalues)
        {
            if (pname == Translator.TranslateStr(154))
            {
                Strings lconnections = GetAvaliableConnections();
                lpossiblevalues.Clear();
                lpossiblevalues.Add("");
                foreach (string nstring in lconnections)
                    lpossiblevalues.Add(nstring);
                return;
            }
            if (pname == Translator.TranslateStr(155))
            {
                Strings ldatasets = GetAvaliableMasterDataSets();
                lpossiblevalues.Clear();
                foreach (string nstring in ldatasets)
                    lpossiblevalues.Add(nstring);
                return;
            }
            base.GetPropertyValues(pname, lpossiblevalues);
        }
        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
            lnames.Clear();
            ltypes.Clear();
            lhints.Clear();
            lcat.Clear();
            if (lvalues != null)
                lvalues.Clear();
            // DataSet Name
            lnames.Add(Translator.TranslateStr(518));
            ltypes.Add(Translator.TranslateStr(557));
            lhints.Add("refdatainfo.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add(FDataInfo.Alias);
            // Connection
            lnames.Add(Translator.TranslateStr(154));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refdatainfo.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add(FDataInfo.DatabaseAlias);
            // Master dataset
            lnames.Add(Translator.TranslateStr(155));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refdatainfo.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add(FDataInfo.DataSource);
            // SQL
            lnames.Add("SQL");
            ltypes.Add("SQL");
            lhints.Add("refdatainfo.html");
            lcat.Add(Translator.TranslateStr(1201));
            if (lvalues != null)
                lvalues.Add(FDataInfo.SQL);
        }
        public override Variant GetProperty(string pname)
        {
            // Alias Name
            if (pname == Translator.TranslateStr(518))
                return FDataInfo.Alias;
            // Connection name
            if (pname == Translator.TranslateStr(154))
                return FDataInfo.DatabaseAlias;
            // Master dataset
            if (pname == Translator.TranslateStr(155))
                return FDataInfo.DataSource;
            // SQL
            if (pname == "SQL")
                return FDataInfo.SQL;
            return base.GetProperty(pname);
        }
        public override void SetProperty(string pname, Variant newvalue)
        {
            // DataSet Name
            if (pname == Translator.TranslateStr(518))
            {
                FDataInfo.Alias = newvalue.ToString();
                return;
            }
            // Connection name
            if (pname == Translator.TranslateStr(154))
            {
                Strings nstrings = GetAvaliableConnections();
                if (newvalue <= nstrings.Count)
                {
                    if (newvalue == 0)
                    {
                        FDataInfo.DatabaseAlias = "";
                    }
                    else
                        FDataInfo.DatabaseAlias = nstrings[newvalue - 1];
                }
                else
                {
                    if (newvalue == 0)
                    {
                        FDataInfo.DatabaseAlias = "";
                    }
                }
                return;
            }
            // Master DataSet
            if (pname == Translator.TranslateStr(155))
            {
                Strings nstrings = GetAvaliableMasterDataSets();
                if (newvalue.AsString.Length == 0)
                    FDataInfo.DataSource = "";
                else
                {
                    if (DoubleUtil.IsNumeric(newvalue.AsString, System.Globalization.NumberStyles.Integer))
                    {
                        int nindx = Convert.ToInt32(newvalue.AsString);
                        if (nindx < nstrings.Count)
                            if (nindx >= 0)
                                FDataInfo.DataSource = nstrings[nindx];
                    }
                }
                return;
            }
            // SQL
            if (pname == "SQL")
            {
                FDataInfo.SQL = newvalue.ToString();
                return;
            }
            // inherited
            base.SetProperty(pname, newvalue);
        }
    }

}
