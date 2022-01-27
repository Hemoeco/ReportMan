using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Reportman.Drawing;

namespace Reportman.Reporting
{
    public class ExpressionItem : PrintItemText
    {
        private Doubles FValues;
        private EvalIdenExpression FIdenExpression;
        private Variant FExportValue;
        private bool FIsPartial;
        private bool FForcedPartial;
        private string FOldString;
        private bool FUpdated;
        private int FDataCount;
        private Variant FSumValue;
        private string FExpression;
        public string Expression
        {
            get { return FExpression; }
            set { FExpression = value; UpdateIsPageCount(); }
        }
        public string GroupName;
        public Aggregate Aggregate;
        public AggregateType AgType;
        public string Identifier;
        public bool AutoExpand;
        public bool AutoContract;
        public string DisplayFormat;
        public string ExportDisplayFormat;
        public ParamType DataType;
        public Variant Value;
        public Variant ExportValue;
        public Variant SumValue;
        public int DataCount;
        public bool Updated;
        public string AgIniValue;
        public bool PrintOnlyOne;
        public bool PrintNulls;
        public bool IsPartial
        {
            get { return FIsPartial; }
        }
        public int PartialPos;
        public string ExportExpression;
        public int ExportLine, ExportPosition, ExportSize;
        public bool ExportDoNewLine;
        public bool IsPageCount;
        public bool IsGroupPageCount;
        public int LastMetaIndex;
        protected override string GetClassName()
        {
            return "TRPEXPRESSION";
        }
        override protected void DoPrint(PrintOut adriver, int aposx, int aposy,
            int newwidth, int newheight, MetaFile metafile, Point MaxExtent,
            ref bool PartialPrint)
        {
            int newposition;
            string avalue;
            base.DoPrint(adriver, aposx, aposy, newwidth, newheight,
                metafile, MaxExtent, ref PartialPrint);
            LastMetaIndex = -1;

            TextObjectStruct TextObj = GetTextObject();
            if (PrintOnlyOne)
            {
                if (FOldString == TextObj.Text)
                    return;
            }
            FOldString = TextObj.Text;
            if (MultiPage || FForcedPartial)
            {

                MaxExtent.X = PrintWidth;
                newposition = MetaFile.CalcTextExtent(Report.Driver, MaxExtent, TextObj);
                if (newposition < TextObj.Text.Length)
                {
                    if (!FIsPartial)
                        PartialPos = 0;
                    FIsPartial = true;
                    PartialPrint = true;
                    PartialPos = PartialPos + newposition;
                    TextObj.Text = TextObj.Text.Substring(0, newposition);
                }
                else
                {
                    FIsPartial = false;
                    FForcedPartial = false;
                }
            }
            MetaPage apage = metafile.Pages[metafile.CurrentPage];
            MetaObjectText aobj = new MetaObjectText();
            aobj.MetaType = MetaObjectType.Text;
            aobj.Left = aposx;
            aobj.Top = aposy;
            aobj.Width = PrintWidth;
            aobj.Height = PrintHeight;
            aobj.Alignment = TextObj.Alignment;
            aobj.PrintStep = PrintStep;
            aobj.BackColor = BackColor;
            aobj.Transparent = Transparent;
            aobj.CutText = CutText;
            aobj.FontColor = FontColor;
            aobj.FontRotation = FontRotation;
            aobj.Type1Font = Type1Font;
            aobj.FontSize = FontSize;
            aobj.FontStyle = (short)FontStyle;
            aobj.TextP = apage.AddString(TextObj.Text);
            aobj.TextS = TextObj.Text.Length;
            aobj.WordWrap = WordWrap;
            aobj.LFontNameP = apage.AddString(LFontName);
            aobj.LFontNameS = LFontName.Length;
            aobj.WFontNameP = apage.AddString(WFontName);
            aobj.WFontNameS = WFontName.Length;
            apage.Objects.Add(aobj);

            LastMetaIndex = metafile.Pages[metafile.CurrentPage].Objects.Count - 1;
            // Is Total pages variable?
            if (IsPageCount)
            {
                Report.AddTotalPagesItem(metafile.CurrentPage, metafile.Pages[metafile.CurrentPage].Objects.Count - 1, DisplayFormat);
            }
            if (ExportValue.VarType != VariantType.Null)
            {
                try
                {
                    avalue = FExportValue.ToString(ExportDisplayFormat, ParamType.Unknown, true);

                    MetaObjectExport nobj = new MetaObjectExport();
                    nobj.MetaType = MetaObjectType.Export;
                    nobj.Left = aposx;
                    nobj.Top = aposy;
                    nobj.Width = PrintWidth;
                    nobj.Height = PrintHeight;
                    nobj.TextExpP = apage.AddString(avalue);
                    nobj.TextExpS = avalue.Length;
                    nobj.Line = ExportLine;
                    nobj.Position = ExportPosition;
                    nobj.DoNewLine = ExportDoNewLine;
                    nobj.Size = ExportSize;
                    apage.Objects.Add(nobj);
                }
                catch (Exception E)
                {
                    throw new ReportException(E.Message + "Expression-" + Name + " ExportDisplayFormat",
                        this, "ExportDisplayFormat");
                }
            }
        }
        public EvalIdenExpression IdenExpression
        {
            get { return FIdenExpression; }
        }
        public void UpdateIsPageCount()
        {
            IsPageCount = false;
            IsGroupPageCount = false;
            string astring = Expression.Trim().ToUpper();
            if (astring == "PAGECOUNT")
                IsPageCount = true;
            else
                if (astring == "GROUPPAGECOUNT")
                IsGroupPageCount = true;
        }
        public ExpressionItem(BaseReport rp)
            : base(rp)
        {
            FExportValue = new Variant();
            FValues = new Doubles();
            FOldString = "";
            PrintNulls = true;
            Height = 275;
            AgIniValue = "0";
            Width = 1440;
            FIdenExpression = new EvalIdenExpression(Report.Evaluator);
            FIdenExpression.ExpreItem = this;
            DataType = ParamType.Unknown;
            ExportSize = 20;
            ExportSize = 1;
            FOldString = "";
            FExpression = "";
            DisplayFormat = "";
            GroupName = "";
            ExportDisplayFormat = "";
            Identifier = "";
            ExportExpression = "";
        }
        public void Evaluate()
        {
            if (FUpdated)
                return;
            Evaluator fevaluator;
            try
            {
                fevaluator = Report.Evaluator;
                Value = fevaluator.EvaluateText(Expression);
                FUpdated = true;
            }
            catch (Exception E)
            {
                throw new ReportException(E.Message + ":Expression-" + Name + " Prop:Expression", this, "Expression");
            }
            FExportValue = new Variant();
            if (ExportExpression.Length > 0)
            {
                try
                {
                    fevaluator = Report.Evaluator;
                    Value = fevaluator.EvaluateText(ExportExpression);
                }
                catch (Exception E)
                {
                    throw new ReportException(E.Message + ":ExportExpression-" + Name + " Prop:ExportExpression", this, "ExportExpression");
                }

            }
        }
        private string GetText()
        {
            string expre;
            string aresult;
            expre = Expression.Trim();
            if (expre.Length == 0)
                return "";

            // Is Total pages variable?
            if (IsPageCount || IsGroupPageCount)
            {
                // 20 spaces
                return "                    ";
            }
            try
            {
                Evaluate();
                aresult = Value.ToString(DisplayFormat, DataType, PrintNulls);
            }
            catch (Exception E)
            {
                throw new ReportException(E.Message + ":Expression-" + Name + " Prop:Expression",
                    this, "Expression");
            }
            if (IsPartial)
            {
                if (aresult[PartialPos] == ' ')
                    if (aresult.Length >= (PartialPos - 2))
                        PartialPos++;
                aresult = aresult.Substring(PartialPos, aresult.Length - PartialPos);
            }
            return aresult;
        }
        public override void SubReportChanged(SubReportEvent newstate, string newgroup)
        {
            base.SubReportChanged(newstate, newgroup);

            Evaluator eval;
            eval = Report.Evaluator;
            switch (newstate)
            {
                case SubReportEvent.Start:
                    FExportValue = new Variant();
                    FIsPartial = false;
                    FForcedPartial = false;
                    FOldString = "";
                    FUpdated = false;
                    FDataCount = 0;
                    if (Aggregate != Aggregate.None)
                    {
                        try
                        {
                            // Update with the initial value
                            eval.Expression = AgIniValue;
                            eval.Evaluate();
                            Value = eval.Result;
                            FSumValue = Value;
                            FUpdated = true;
                        }
                        catch (Exception E)
                        {
                            throw new ReportException(E.Message + ":Expression-" + Name + " Prop:AgIniValue", this, "AgIniValue");
                        }
                    }
                    break;
                case SubReportEvent.SubReportStart:
                    FExportValue = new Variant();
                    FIsPartial = false;
                    FForcedPartial = false;
                    FOldString = "";
                    FUpdated = false;
                    FDataCount = 0;
                    if ((Aggregate != Aggregate.None) && (Aggregate != Aggregate.General))
                    {
                        try
                        {
                            // Update with the initial value
                            eval.Expression = AgIniValue;
                            eval.Evaluate();
                            Value = eval.Result;
                            FSumValue = Value;
                            FUpdated = true;
                        }
                        catch (Exception E)
                        {
                            throw new ReportException(E.Message + ":Expression-" + Name + " Prop:AgIniValue", this, "AgIniValue");
                        }
                    }
                    break;
                case SubReportEvent.DataChange:
                    FIsPartial = false;
                    FForcedPartial = false;
                    FUpdated = false;
                    FDataCount++;
                    if (Aggregate != Aggregate.None)
                    {
                        try
                        {
                            eval.Expression = Expression;
                            eval.Evaluate();
                            // Do the operation
                            switch (AgType)
                            {
                                case AggregateType.Summary:
                                    if (eval.Result.VarType != VariantType.Null)
                                        Value = Value + eval.Result;
                                    break;
                                case AggregateType.Minimum:
                                    if (eval.Result.VarType != VariantType.Null)
                                    {
                                        if (Value > eval.Result)
                                            Value = eval.Result;
                                    }
                                    break;
                                case AggregateType.Maximum:
                                    if (eval.Result.VarType != VariantType.Null)
                                    {
                                        if (Value < eval.Result)
                                            Value = eval.Result;
                                    }
                                    break;
                                case AggregateType.Average:
                                    if (eval.Result.VarType == VariantType.Null)
                                    {
                                        FDataCount--;
                                    }
                                    else
                                    {
                                        FSumValue = FSumValue + eval.Result;
                                        Value = FSumValue / FDataCount;
                                    }
                                    break;
                                case AggregateType.StandardDeviation:
                                    if (eval.Result.VarType == VariantType.Null)
                                    {
                                        FDataCount--;
                                    }
                                    else
                                    {
                                        FValues.Add(eval.Result);
                                        Value = DoubleUtil.StandardDeviation(FValues);
                                    }
                                    break;

                            }
                            FUpdated = true;
                        }
                        catch (Exception E)
                        {
                            throw new ReportException(E.Message + ":Expression-" + Name + " Prop:Expression", this, "Expression");
                        }
                    }
                    break;
                case SubReportEvent.GroupChange:
                    FIsPartial = false;
                    FForcedPartial = false;
                    FUpdated = false;
                    FOldString = "";
                    if (Aggregate == Aggregate.Group)
                    {
                        if (GroupName.ToUpper() == newgroup.ToUpper())
                        {
                            // Update with the initial value
                            try
                            {
                                // Update with the initial value
                                eval.Expression = AgIniValue;
                                eval.Evaluate();
                                Value = eval.Result;
                                FSumValue = Value;
                                FUpdated = true;
                            }
                            catch (Exception E)
                            {
                                throw new ReportException(E.Message + ":Expression-" + Name + " Prop:AgIniValue", this, "AgIniValue");
                            }
                        }
                    }
                    break;
                case SubReportEvent.PageChange:
                    FOldString = "";
                    if (Aggregate == Aggregate.None)
                    {
                        // Page variable must be recalculated
                        FUpdated = false;
                    }
                    if (Aggregate == Aggregate.Page)
                    {
                        // Update with the initial value
                        try
                        {
                            // Update with the initial value
                            eval.Expression = AgIniValue;
                            eval.Evaluate();
                            Value = eval.Result;
                            FSumValue = Value;
                            FUpdated = true;
                        }
                        catch (Exception E)
                        {
                            throw new ReportException(E.Message + ":Expression-" + Name + " Prop:AgIniValue", this, "AgIniValue");
                        }
                        SubReportChanged(SubReportEvent.DataChange, "");
                    }
                    break;
                case SubReportEvent.InvalidateValue:
                    FIsPartial = false;
                    FForcedPartial = false;
                    FOldString = "";
                    FUpdated = false;
                    break;
            }
        }
        private TextObjectStruct GetTextObject()
        {
            int aalign;
            TextObjectStruct aresult = new TextObjectStruct();
            aresult.Text = GetText();
            aresult.LFontName = LFontName;
            aresult.WFontName = WFontName;
            aresult.FontSize = FontSize;
            aresult.FontRotation = FontRotation;
            aresult.FontStyle = (short)FontStyle;
            aresult.Type1Font = Type1Font;
            aresult.FontColor = FontColor;
            aresult.CutText = CutText;
            aalign = PrintAlignment | VPrintAlignment;
            if (SingleLine)
                aalign = aalign | MetaFile.AlignmentFlags_SingleLine;
            aresult.Alignment = aalign;
            aresult.WordWrap = WordWrap;
            aresult.RightToLeft = RightToLeft;
            aresult.PrintStep = PrintStep;
            return aresult;
        }
        override public Point GetExtension(PrintOut adriver, Point MaxExtent, bool ForcePartial)
        {
            TextObjectStruct atext;
            atext = GetTextObject();
            // Items printed only one time, have no extension
            if (PrintOnlyOne)
            {
                if (FOldString == atext.Text)
                {
                    return new Point(0, 0);
                }
            }

            int aposition;
            bool IsPartial;
            IsPartial = false;
            Point aresult = base.GetExtension(adriver, MaxExtent, ForcePartial);
            if ((MultiPage) || ForcePartial)
            {
                MaxExtent.X = aresult.X;
                aposition = MetaFile.CalcTextExtent(adriver, MaxExtent, atext);
                if (aposition < atext.Text.Length)
                    IsPartial = true;
                atext.Text = atext.Text.Substring(0, aposition);
                aresult = adriver.TextExtent(atext, aresult);
                if (IsPartial)
                    aresult.Y = MaxExtent.Y;
            }
            else
                aresult = adriver.TextExtent(atext, aresult);
            LastExtent = aresult;
            return aresult;
        }
    }
}
