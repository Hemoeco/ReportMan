#region Copyright
/*
 *  Report Manager:  Database Reporting tool for .Net and Mono
 *
 *     The contents of this file are subject to the MPL License
 *     with optional use of GPL or LGPL licenses.
 *     You may not use this file except in compliance with the
 *     Licenses. You may obtain copies of the Licenses at:
 *     http://reportman.sourceforge.net/license
 *
 *     Software is distributed on an "AS IS" basis,
 *     WITHOUT WARRANTY OF ANY KIND, either
 *     express or implied.  See the License for the specific
 *     language governing rights and limitations.
 *
 *  Copyright (c) 1994 - 2008 Toni Martir (toni@reportman.es)
 *  All Rights Reserved.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Designer
{
    internal class DesignerInterface
    {
      public virtual void SetPropertyMulti(string pname, Variant newvalue)
      {
        if (SelectionList.Count == 1)
        {
          SetProperty(pname, newvalue);
        }
        else
        {
          foreach (ReportItem nitem in SelectionList.Values)
          {
            SetItem(nitem);
            SetProperty(pname, newvalue);
          }
        }
      }

      public Variant GetPropertyMulti(string pname)
      {
        if (SelectionList.Count == 1)
        {
          return GetProperty(pname);
        }
        else
        {
          Variant nvar = new Variant();
          bool firstpass = true;
          foreach (ReportItem nitem in SelectionList.Values)
          {
            SetItem(nitem);
            if (firstpass)
            {
              nvar = GetProperty(pname);
              firstpass = false;
            }
            else
              if (!nvar.Equals(GetProperty(pname)))
              {
                return new Variant();
              }
          }
          return nvar;
        }
      }
      public static DesignerInterface GetFromOject(SortedList<int, ReportItem> litems,IObjectInspector objinsp)
      {
        string baseclass = "";
        foreach (ReportItem nitem in litems.Values)
        {
          if (baseclass.Length==0)
          {
            baseclass = nitem.GetType().ToString();
            baseclass = baseclass.Substring(20, baseclass.Length - 20);
          }
          else
          {
            string atypename = nitem.GetType().ToString();
            atypename = atypename.Substring(20, atypename.Length - 20);
            if (atypename != baseclass)
            {

              switch (atypename)
              {
                case "LabelItem":
                  if ((nitem is LabelItem) ||  (nitem is ExpressionItem) || (nitem is ChartItem))
                    baseclass = "PrintItemText";
                  break;
                case "BarcodeItem":
                    if (!(nitem is BarcodeItem))
                        baseclass = "PrintPosItem";
                    break;
                 case "ExpressionItem":
                  if ((nitem is LabelItem) || (nitem is ChartItem) || (nitem is ExpressionItem))
                    baseclass = "PrintItemText";
                  break;
                case "ChartItem":
                  if ((nitem is LabelItem) || (nitem is ChartItem) || (nitem is ChartItem))
                    baseclass = "PrintItemText";
                  break;
                case "PrintItemText":
                  if (!((nitem is LabelItem) && (nitem is ChartItem) && (nitem is ExpressionItem)))
                        baseclass = "PrintPosItem";
                  break;
                case "SubReport":
                  baseclass = "SubReport";
                  break;
                case "DatabaseInfo":
                  baseclass = "DatabaseInfo";
                  break;
                case "DataInfo":
                  baseclass = "DataInfo";
                  break;
                default:
                  baseclass = "PrintPosItem";
                  break;
              }
            }
          }
          if (baseclass == "PrintPosItem")
            break;
        }
        DesignerInterface nresult = null;
        switch (baseclass)
        {
          case "LabelItem":
            nresult = new DesignerInterfaceLabel(litems,objinsp);
            break;
          case "PrintItemText":
                nresult = new DesignerInterfaceText(litems, objinsp);
                break;
          case "ExpressionItem":
            nresult = new DesignerInterfaceExpression(litems, objinsp);
            break;
          case "ChartItem":
            nresult = new DesignerInterfaceChart(litems, objinsp);
            break;
          case "ShapeItem":
            nresult = new DesignerInterfaceShape(litems, objinsp);
            break;
          case "PrintPosItem":
            nresult = new DesignerInterfaceSizePos(litems, objinsp);
            break;
          case "BarcodeItem":
            nresult = new DesignerInterfaceBarcode(litems, objinsp);
            break;
          case "Section":
            nresult = new DesignerInterfaceSection(litems, objinsp);
            break;
          case "SubReport":
            nresult = new DesignerInterfaceSubReport(litems, objinsp);
            break;
          case "DatabaseInfo":
            nresult = new DesignerInterfaceDbInfo(litems, objinsp);
            break;
          case "DataInfo":
            nresult = new DesignerInterfaceDataInfo(litems, objinsp);
            break;
          case "Param":
            nresult = new DesignerInterfaceParam(litems,objinsp);
            break;
          case "ImageItem":
            nresult = new DesignerInterfaceImage(litems, objinsp);
            break;
        }
        nresult.SelectionClassName = baseclass;
        return nresult;
      }
        private SortedList<int,ReportItem> FReportItems;
      public SortedList<int, ReportItem> SelectionList
      {
        get { return FReportItems; }
      }
      public string SelectionClassName;
        private ReportItem FReportItemObject;
        protected IObjectInspector FObjectInspector;
      public DesignerInterface(SortedList<int, ReportItem> repitem, IObjectInspector objinsp)
        {
          FReportItems = repitem;
            FObjectInspector = objinsp;
          if (repitem.Count == 1)
              FReportItemObject = repitem.Values[0];
        }
        public ReportItem ReportItemObject { get { return FReportItemObject; } }
        public virtual void GetProperties(Strings lnames,Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
        }
        public virtual void GetPropertyValues(string pname, Strings lpossiblevalues)
        {
            throw new Exception(Translator.TranslateStr(675)+":"+pname);
        }
        public virtual void SetItem(ReportItem nitem)
        {
        }
        public virtual Variant GetProperty(string pname)
        {
            throw new Exception(Translator.TranslateStr(674) + ":" + pname);
        }
        public virtual MemoryStream GetProperty(string pname, ref MemoryStream memvalue)
        {
            throw new Exception(Translator.TranslateStr(674) + ":" + pname);
        }
        public virtual void SetProperty(string pname, Variant newvalue)
        {
            throw new Exception(Translator.TranslateStr(674) + ":" + pname);
        }
        public virtual void SetProperty(string pname, MemoryStream newvalue)
        {
            throw new Exception(Translator.TranslateStr(642) + ":" + pname);
        }
    }

}
