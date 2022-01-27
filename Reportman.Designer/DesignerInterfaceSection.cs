using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Reportman.Reporting;
using Reportman.Drawing;

namespace Reportman.Designer
{
  internal class DesignerInterfaceSection:DesignerInterfaceSize
  {
    private Section FSection;
    public DesignerInterfaceSection(SortedList<int, ReportItem> repitem, IObjectInspector objinsp) :base (repitem,objinsp)
    {
        if (repitem.Count == 1)
          FSection = (Section)repitem.Values[0];
    }
    public Section PrintPosItemObject { get { return FSection; } }

    public override void GetPropertyValues(string pname, Strings lpossiblevalues)
    {
        // Skip type
        if (pname == Translator.TranslateStr(926))
        {
            lpossiblevalues.Add(Translator.TranslateStr(924)); // Skip Before
            lpossiblevalues.Add(Translator.TranslateStr(925)); // Skip After
            return;
        }
        // Child subreport
        if (pname == Translator.TranslateStr(834))
        {
            lpossiblevalues.Add("");
            foreach (SubReport nsub in FSection.Report.SubReports)
            {
              if (nsub!=FSection.SubReport)
              {
                lpossiblevalues.Add(nsub.GetDisplayName(false));
              }
            }
            return;
        }
        // BackStyle
        if (pname == Translator.TranslateStr(1250))
        {
          FillBackStyleDescriptions(lpossiblevalues);
            return;
        }
        // DrawStyle
        if (pname == Translator.TranslateStr(667))
        {
            FillDrawStyleDescriptions(lpossiblevalues);
            return;
        }
        // DrawStyle
        if (pname == Translator.TranslateStr(1409))
        {
          FillSharedImageDescriptions(lpossiblevalues);
          return;
        }
        base.GetPropertyValues(pname, lpossiblevalues);
    }
    public static void FillBackStyleDescriptions(Strings nvalues)
    {
      nvalues.Add(Translator.TranslateStr(1251));
      nvalues.Add(Translator.TranslateStr(1252));
      nvalues.Add(Translator.TranslateStr(1253));
    }
    public static void FillSharedImageDescriptions(Strings nvalues)
    {
      nvalues.Add(Translator.TranslateStr(294));
      nvalues.Add(Translator.TranslateStr(1420));
      nvalues.Add(Translator.TranslateStr(1421));
    }
    public static void FillDrawStyleDescriptions(Strings nvalues)
    {
      nvalues.Add(Translator.TranslateStr(636));
      nvalues.Add(Translator.TranslateStr(637));
      nvalues.Add(Translator.TranslateStr(638));
      nvalues.Add(Translator.TranslateStr(668));
      nvalues.Add(Translator.TranslateStr(1326));
    }
    public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
    {
      base.GetProperties(lnames, ltypes, lvalues, lhints, lcat);
      if ((FSection.SectionType == SectionType.PageFooter) || (FSection.SectionType == SectionType.PageHeader)
         || (FSection.SectionType == SectionType.GroupFooter) || (FSection.SectionType == SectionType.GroupHeader))
      {
        lnames.Add(Translator.TranslateStr(486));
        ltypes.Add(Translator.TranslateStr(568));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.Global);
      }
      if (FSection.SectionType!=SectionType.PageFooter)
      {
        // AutoExpand
        lnames.Add(Translator.TranslateStr(610));
        ltypes.Add(Translator.TranslateStr(568));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.AutoExpand);
        // AutoContract
        lnames.Add(Translator.TranslateStr(611));
        ltypes.Add(Translator.TranslateStr(568));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.AutoContract);
      }
      if ((FSection.SectionType== SectionType.GroupHeader) || (FSection.SectionType== SectionType.GroupFooter))
      {
        // IniNumPage
        lnames.Add(Translator.TranslateStr(940));
        ltypes.Add(Translator.TranslateStr(568));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.IniNumPage);
        // Group Name
        lnames.Add(Translator.TranslateStr(277));
        ltypes.Add(Translator.TranslateStr(557));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.GroupName);
        // Group Expression
        lnames.Add(Translator.TranslateStr(615));
        ltypes.Add(Translator.TranslateStr(571));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.ChangeExpression);
        // Change bool
        lnames.Add(Translator.TranslateStr(616));
        ltypes.Add(Translator.TranslateStr(568));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.ChangeBool);
        if (FSection.SectionType == SectionType.GroupHeader)
        {
          // Page repeat
          lnames.Add(Translator.TranslateStr(617));
          ltypes.Add(Translator.TranslateStr(568));
          lhints.Add("refsection.html");
          lcat.Add(Translator.TranslateStr(280));
          if (lvalues!=null)
            lvalues.Add(FSection.PageRepeat);
          // Footer at report end
          lnames.Add(Translator.TranslateStr(919));
          ltypes.Add(Translator.TranslateStr(568));
          lhints.Add("refsection.html");
          lcat.Add(Translator.TranslateStr(280));
          if (lvalues!=null)
            lvalues.Add(FSection.ForcePrint);
        }

      }
      if ((FSection.SectionType== SectionType.GroupHeader) || (FSection.SectionType== SectionType.GroupFooter)
         || (FSection.SectionType== SectionType.Detail))
      {
        // Begin page
        lnames.Add(Translator.TranslateStr(618));
        ltypes.Add(Translator.TranslateStr(571));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.BeginPageExpression);
        // Skip page
        lnames.Add(Translator.TranslateStr(619));
        ltypes.Add(Translator.TranslateStr(568));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.SkipPage);
        // Align bottom
        lnames.Add(Translator.TranslateStr(620));
        ltypes.Add(Translator.TranslateStr(568));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.AlignBottom);
        // Horz desp
        lnames.Add(Translator.TranslateStr(308));
        ltypes.Add(Translator.TranslateStr(568));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.HorzDesp);
        // Vert desp
        lnames.Add(Translator.TranslateStr(1124));
        ltypes.Add(Translator.TranslateStr(568));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.VertDesp);
        // Skip type
        lnames.Add(Translator.TranslateStr(926));
        ltypes.Add(Translator.TranslateStr(569));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add((int)FSection.SkipType);
        // Skip to page
        lnames.Add(Translator.TranslateStr(927));
        ltypes.Add(Translator.TranslateStr(571));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.SkipToPageExpre);
        // Skip Expre H
        lnames.Add(Translator.TranslateStr(922));
        ltypes.Add(Translator.TranslateStr(571));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.SkipExpreH);
        // Horz relative skip
        lnames.Add(Translator.TranslateStr(920));
        ltypes.Add(Translator.TranslateStr(568));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.SkipRelativeH);
        // Skip Expre V
        lnames.Add(Translator.TranslateStr(923));
        ltypes.Add(Translator.TranslateStr(571));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.SkipExpreV);
        // Vert relative skip
        lnames.Add(Translator.TranslateStr(921));
        ltypes.Add(Translator.TranslateStr(568));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.SkipRelativeV);
        // Child subreport
        lnames.Add(Translator.TranslateStr(834));
        ltypes.Add(Translator.TranslateStr(961));
        lhints.Add("refsection.html");
        lcat.Add(Translator.TranslateStr(280));
        if (lvalues!=null)
          lvalues.Add(FSection.ChildSubReportName);
      }
      if (FSection.SectionType == SectionType.PageFooter)
      {
          // Print after report end
          lnames.Add(Translator.TranslateStr(919));
          ltypes.Add(Translator.TranslateStr(568));
          lhints.Add("refsection.html");
          lcat.Add(Translator.TranslateStr(280));
          if (lvalues!=null)
            lvalues.Add(FSection.ForcePrint);
      }
      // External section
      lnames.Add(Translator.TranslateStr(830));
      ltypes.Add(Translator.TranslateStr(830));
      lhints.Add("refsection.html");
      lcat.Add(Translator.TranslateStr(280));
      if (lvalues!=null)
        lvalues.Add(FSection.ExternalFilename);
      // External Data
      lnames.Add(Translator.TranslateStr(861));
      ltypes.Add(Translator.TranslateStr(861));
      lhints.Add("refsection.html");
      lcat.Add(Translator.TranslateStr(280));
      if (lvalues!=null)
        lvalues.Add(FSection.GetExternalDataDescription());
      // Back image expression
      lnames.Add(Translator.TranslateStr(1248));
      ltypes.Add(Translator.TranslateStr(571));
      lhints.Add("refsection.html");
      lcat.Add(Translator.TranslateStr(280));
      if (lvalues!=null)
        lvalues.Add(FSection.BackExpression);
      // Image
      lnames.Add(Translator.TranslateStr(639));
      ltypes.Add(Translator.TranslateStr(639));
      lhints.Add("refsection.html");
      lcat.Add(Translator.TranslateStr(280));
      if (lvalues!=null)
      {
        Variant nvar = new Variant();
        MemoryStream nstream = null;
        if (FSection.Stream!=null)
            nstream = new MemoryStream();
        else
            nstream = FSection.Stream; ;
        nvar.SetStream(nstream);
        lvalues.Add(nvar);
      }
      // DPI Resolution
      lnames.Add(Translator.TranslateStr(666));
      ltypes.Add(Translator.TranslateStr(559));
      lhints.Add("refsection.html");
      lcat.Add(Translator.TranslateStr(280));
      if (lvalues!=null)
        lvalues.Add(FSection.dpires);
      // BackStyle
      lnames.Add(Translator.TranslateStr(1250));
      ltypes.Add(Translator.TranslateStr(569));
      lhints.Add("refsection.html");
      lcat.Add(Translator.TranslateStr(280));
      if (lvalues!=null)
        lvalues.Add((int)FSection.BackStyle);
      // DrawStyle
      lnames.Add(Translator.TranslateStr(667));
      ltypes.Add(Translator.TranslateStr(569));
      lhints.Add("refsection.html");
      lcat.Add(Translator.TranslateStr(280));
      if (lvalues!=null)
        lvalues.Add((int)FSection.DrawStyle);
      // Shared image mode
      lnames.Add(Translator.TranslateStr(1409));
      ltypes.Add(Translator.TranslateStr(569));
      lhints.Add("refsection.html");
      lcat.Add(Translator.TranslateStr(280));
      if (lvalues!=null)
        lvalues.Add((int)FSection.SharedImage);
    }
    public override Variant GetProperty(string pname)
    {
      if (pname==Translator.TranslateStr(486))
        return FSection.Global;
      if (pname==Translator.TranslateStr(610))
        return FSection.AutoExpand;
      if (pname==Translator.TranslateStr(611))
        return FSection.AutoContract;
      if (pname==Translator.TranslateStr(940))
        return FSection.IniNumPage;
      if (pname==Translator.TranslateStr(277))
        return FSection.GroupName;
      if (pname==Translator.TranslateStr(615))
        return FSection.ChangeExpression;
      if (pname==Translator.TranslateStr(616))
        return FSection.ChangeBool;
      if (pname==Translator.TranslateStr(617))
        return FSection.PageRepeat;
      if (pname==Translator.TranslateStr(619))
        return FSection.ForcePrint;
      if (pname==Translator.TranslateStr(618))
        return FSection.BeginPageExpression;
      if (pname==Translator.TranslateStr(619))
        return FSection.SkipPage;
      if (pname==Translator.TranslateStr(620))
        return FSection.AlignBottom;
      if (pname==Translator.TranslateStr(308))
        return FSection.HorzDesp;
      if (pname==Translator.TranslateStr(1124))
        return FSection.VertDesp;
      if (pname==Translator.TranslateStr(926))
        return ((int)FSection.SkipType);
      if (pname==Translator.TranslateStr(927))
        return FSection.SkipToPageExpre;
      if (pname==Translator.TranslateStr(922))
        return FSection.SkipExpreH;
      if (pname==Translator.TranslateStr(920))
        return FSection.SkipRelativeH;
      if (pname==Translator.TranslateStr(923))
        return FSection.SkipExpreV;
      if (pname==Translator.TranslateStr(921))
        return FSection.SkipRelativeV;
        if (pname == Translator.TranslateStr(834))
        {
            if (FSection.ChildSubReport == null)
                return "";
            else
            {
                return FSection.ChildSubReport.GetDisplayName(false);
            }
        }
      if (pname==Translator.TranslateStr(919))
        return FSection.ForcePrint;
      if (pname == Translator.TranslateStr(830))
        return FSection.ExternalFilename;
      if (pname == Translator.TranslateStr(861))
        return FSection.GetExternalDataDescription();
      if (pname == Translator.TranslateStr(1248))
        return FSection.BackExpression;
      if (pname == Translator.TranslateStr(639))
      {
          Variant nvar = new Variant();
          MemoryStream nstream = null;
          if (FSection.Stream != null)
              nstream = new MemoryStream();
          else
              nstream = FSection.Stream; ;
          nvar.SetStream(nstream);
          return nvar;
      }
      if (pname == Translator.TranslateStr(666))
        return FSection.dpires;
      if (pname == Translator.TranslateStr(1250))
        return ((int)FSection.BackStyle);
      if (pname == Translator.TranslateStr(667)) 
        return ((int)FSection.DrawStyle);
      if (pname == Translator.TranslateStr(1409))
        return ((int)FSection.SharedImage); 
      return base.GetProperty(pname);
    }

    public override void SetItem(ReportItem nitem)
    {
      FSection = (Section)nitem;
      base.SetItem(nitem);
    }

    public override void SetProperty(string pname, Variant newvalue)
    {
      if (pname == Translator.TranslateStr(486))
      {
        FSection.Global = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(610))
      {
        FSection.AutoExpand = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(611))
      {
        FSection.AutoContract = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(940))
      {
        FSection.IniNumPage = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(277))
      {
        FSection.GroupName = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(615))
      {
        FSection.ChangeExpression = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(616))
      {
        FSection.ChangeBool = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(617))
      {
        FSection.PageRepeat = newvalue; 
        return;
      }
      if (pname == Translator.TranslateStr(619))
      {
        FSection.ForcePrint = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(618))
      {
        FSection.BeginPageExpression = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(619))
      {
        FSection.SkipPage = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(620))
      {
        FSection.AlignBottom = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(308))
      {
        FSection.HorzDesp = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(1124))
      {
        FSection.VertDesp = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(926))
      {
        FSection.SkipType = (SkipType)(int)newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(927))
      {
        FSection.SkipToPageExpre = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(922))
      {
        FSection.SkipExpreH = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(920))
      {
        FSection.SkipRelativeH = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(923))
      {
        FSection.SkipExpreV = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(921))
      {
        FSection.SkipRelativeV = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(834))
      {
        SubReport nsub = null;
        Strings alist = new Strings();
        GetPropertyValues(Translator.TranslateStr(834), alist);
        string nname = "";
        if (!newvalue.IsNull)
        {
            if (newvalue.IsInteger())
            {
                int index = newvalue;
                if (index < alist.Count)
                    nname = alist[index];
            }
        }
        if (nname.Length>0)
        {
            foreach (SubReport newsub in FSection.Report.SubReports)
            {
                if (newsub.GetDisplayName(false) == nname)
                {
                    nsub = newsub;
                    break;
                }
            }
        }
        if (nsub != null)
        {
          FSection.ChildSubReport = nsub;
          FSection.ChildSubReportName = nsub.Name;
        }
        else
        {
          FSection.ChildSubReportName = "";
          FSection.ChildSubReport = null;
        }
        return;
      }
      if (pname == Translator.TranslateStr(919))
      {
        FSection.ForcePrint = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(830))
      {
        FSection.ExternalFilename = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(861))
      {
        //FSection.GetExternalDataDescription();
        return;
      }
      if (pname == Translator.TranslateStr(1248))
      {
        FSection.BackExpression = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(639))
      {
          FSection.Stream.SetLength(0);
          byte[] narray = newvalue.GetStream().ToArray();
          FSection.Stream.Write(narray, 0, narray.Length);
          return;
      }
      if (pname == Translator.TranslateStr(666))
      {
        FSection.dpires = newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(1250))
      {
         FSection.BackStyle = (BackStyleType)(int)newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(667))
      {
         FSection.DrawStyle = (ImageDrawStyleType)(int)newvalue;
        return;
      }
      if (pname == Translator.TranslateStr(1409))
      {
        FSection.SharedImage = (SharedImageType)(int)newvalue;
        return;
      }
      
      // inherited
      base.SetProperty(pname, newvalue);
    }

  }
}
