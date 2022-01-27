using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reportman.Reporting;
using Reportman.Drawing;

namespace Reportman.Designer
{
    class DesignerInterfaceChart:DesignerInterfaceText
    {
        private ChartItem FPrintItemChart;
        private string VerticalAxisAutoRangeName = "Y Auto";
        private string VerticalAxisMinimum = "Y Min";
        private string VerticalAxisMaximum = " Y Max";

        public DesignerInterfaceChart(SortedList<int, ReportItem> repitem, IObjectInspector objinsp)
            : base(repitem,objinsp)
        {
            VerticalAxisAutoRangeName = Translator.TranslateStr(1449);
            VerticalAxisMinimum = Translator.TranslateStr(1450);
            VerticalAxisMaximum = Translator.TranslateStr(1451);
            if (repitem.Count == 1)
                FPrintItemChart = (ChartItem)repitem.Values[0];

        }
        public ChartItem PrintItemChart { get { return FPrintItemChart; } }
        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
            base.GetProperties(lnames, ltypes, lvalues, lhints, lcat);

            // Expression
            lnames.Add(Translator.TranslateStr(571));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.ValueExpression);
            // Identifier
            lnames.Add(Translator.TranslateStr(304));
            ltypes.Add(Translator.TranslateStr(557));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.Identifier);
            // ChartType
            lnames.Add(Translator.TranslateStr(712));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemChart.ChartStyle);
            // GetValue Condition
            lnames.Add(Translator.TranslateStr(717));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.GetValueCondition);
            // Change Serie Expression
            lnames.Add(Translator.TranslateStr(714));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.ChangeSerieExpression);
            // Change Serie Bool Expression
            lnames.Add(Translator.TranslateStr(715));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.ChangeSerieBool);

            // Clear Chart Expression
            lnames.Add(Translator.TranslateStr(966));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.ClearExpression);

            // Change Serie Bool Expression
            lnames.Add(Translator.TranslateStr(967));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.ClearExpressionBool);

            // Caption Expression
            lnames.Add(Translator.TranslateStr(716));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.CaptionExpression);
            // Expression X
            lnames.Add(Translator.TranslateStr(571)+" X");
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.ValueXExpression);


            // Serie Caption Expression
            lnames.Add(Translator.TranslateStr(1331));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.SerieCaption);

            // Value Color Expression
            lnames.Add(Translator.TranslateStr(1365));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.ColorExpression);

            // Serie Color Expression
            lnames.Add(Translator.TranslateStr(1364));
            ltypes.Add(Translator.TranslateStr(571));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1207));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.SerieColorExpression);

            // Chart Driver
            lnames.Add(Translator.TranslateStr(67));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemChart.Driver);

            // View3d
            lnames.Add(Translator.TranslateStr(904));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.View3d);

            // View3dWalls
            lnames.Add(Translator.TranslateStr(905));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.View3dWalls);

            // Perspective
            lnames.Add(Translator.TranslateStr(906));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.Perspective);

            // Elevation
            lnames.Add(Translator.TranslateStr(907));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.Elevation);

            // Rotation
            lnames.Add(Translator.TranslateStr(908));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.Rotation);

            // Orthogonal
            lnames.Add(Translator.TranslateStr(909));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.Orthogonal);

            // Zoom
            lnames.Add(Translator.TranslateStr(910));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.Zoom);

            // Horizontal Offset
            lnames.Add(Translator.TranslateStr(911));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.HorzOffset);

            // Vertical Offset
            lnames.Add(Translator.TranslateStr(912));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.VertOffset);

            // Tilt
            lnames.Add(Translator.TranslateStr(913));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.Tilt);

            // DPIRes
            lnames.Add(Translator.TranslateStr(666));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.Resolution);


            // MultiBar
            lnames.Add(Translator.TranslateStr(914));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemChart.MultiBar);

            // Show Hint
            lnames.Add(Translator.TranslateStr(1228));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.ShowHint);

            // Show Legend
            lnames.Add(Translator.TranslateStr(1229));
            ltypes.Add(Translator.TranslateStr(568));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.ShowLegend);

            // Mark Type
            lnames.Add(Translator.TranslateStr(1351));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemChart.MarkStyle);

            // Vertical axis font size
            lnames.Add(Translator.TranslateStr(1361));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.VertFontSize);
            // Horizontal axis font size
            lnames.Add(Translator.TranslateStr(1363));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.HorzFontSize);
            // Vertical axis font rotation
            lnames.Add(Translator.TranslateStr(1362));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.VertFontRotation);
            // Horizontal axis font rotation
            lnames.Add(Translator.TranslateStr(1376));
            ltypes.Add(Translator.TranslateStr(559));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.HorzFontRotation);
            // Vertical axis autorange
            lnames.Add(VerticalAxisAutoRangeName);
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add((int)FPrintItemChart.AutoRange);
            // Vertical axis Min
            lnames.Add(VerticalAxisMinimum);
            ltypes.Add(Translator.TranslateStr(1171));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.AxisYInitial);
            // Vertical axis Max
            lnames.Add(VerticalAxisMaximum);
            ltypes.Add(Translator.TranslateStr(1171));
            lhints.Add("refchart.html");
            lcat.Add(Translator.TranslateStr(1208));
            if (lvalues != null)
                lvalues.Add(FPrintItemChart.AxisYFinal);

        }
        public override Variant GetProperty(string pname)
        {
            // Expression
            if (pname == Translator.TranslateStr(571))
                return FPrintItemChart.ValueExpression;
            // Expression
            if (pname == Translator.TranslateStr(571)+" X")
                return FPrintItemChart.ValueXExpression;
            // Identifier
            if (pname == Translator.TranslateStr(304))
                return FPrintItemChart.Identifier;
            // ChartType
            if (pname == Translator.TranslateStr(712))
                return (int)FPrintItemChart.ChartStyle;
            // GetValue Condition
            if (pname == Translator.TranslateStr(717))
                return FPrintItemChart.GetValueCondition;
            // Change Serie Expression
            if (pname == Translator.TranslateStr(714))
                return FPrintItemChart.ChangeSerieExpression;
            // Change Serie Bool Expression
            if (pname == Translator.TranslateStr(715))
                return FPrintItemChart.ChangeSerieBool;
            // Clear Chart Expression
            if (pname == Translator.TranslateStr(966))
                return FPrintItemChart.ClearExpression;
            // Change Serie Bool Expression
            if (pname == Translator.TranslateStr(967))
                return FPrintItemChart.ClearExpressionBool;
            // Caption Expression
            if (pname == Translator.TranslateStr(716))
                return FPrintItemChart.CaptionExpression;
            // Serie Caption Expression
            if (pname == Translator.TranslateStr(1331))
                return FPrintItemChart.SerieCaption;
            // Value Color Expression
            if (pname == Translator.TranslateStr(1365))
                return FPrintItemChart.ColorExpression;
            // Serie Color Expression
            if (pname == Translator.TranslateStr(1364))
                return FPrintItemChart.SerieColorExpression;
            // Chart Driver
            if (pname == Translator.TranslateStr(67))
                return (int)FPrintItemChart.Driver;
            // View3d
            if (pname == Translator.TranslateStr(904))
                return FPrintItemChart.View3d;
            // View3dWalls
            if (pname == Translator.TranslateStr(905))
                return FPrintItemChart.View3dWalls;
            // Perspective
            if (pname == Translator.TranslateStr(906))
                return FPrintItemChart.Perspective;
            // Elevation
            if (pname == Translator.TranslateStr(907))
                return FPrintItemChart.Elevation;
            // Rotation
            if (pname == Translator.TranslateStr(908))
                return FPrintItemChart.Rotation;
            // Orthogonal
            if (pname == Translator.TranslateStr(909))
                return FPrintItemChart.Orthogonal;
            // Zoom
            if (pname == Translator.TranslateStr(910))
                return FPrintItemChart.Zoom;
            // Horizontal Offset
            if (pname == Translator.TranslateStr(911))
                return FPrintItemChart.HorzOffset;
            // Vertical Offset
            if (pname == Translator.TranslateStr(912))
                return FPrintItemChart.VertOffset;
            // Tilt
            if (pname == Translator.TranslateStr(913))
                return FPrintItemChart.Tilt;
            // DPIRes
            if (pname == Translator.TranslateStr(666))
                return FPrintItemChart.Resolution;
            // MultiBar
            if (pname == Translator.TranslateStr(914))
                return (int)FPrintItemChart.MultiBar;
            // Show Hint
            if (pname == Translator.TranslateStr(1228))
                return FPrintItemChart.ShowHint;
            // Show Legend
            if (pname == Translator.TranslateStr(1229))
                return FPrintItemChart.ShowLegend;
            // Mark Type
            if (pname == Translator.TranslateStr(1351))
                return FPrintItemChart.MarkStyle;
            // Vertical axis font size
            if (pname == Translator.TranslateStr(1361))
                return FPrintItemChart.VertFontSize;
            // Horizontal axis font size
            if (pname == Translator.TranslateStr(1363))
                return FPrintItemChart.HorzFontSize;
            // Vertical axis font rotation
            if (pname == Translator.TranslateStr(1362))
                return FPrintItemChart.VertFontRotation;
            // Horizontal axis font rotation
            if (pname == Translator.TranslateStr(1376))
                return FPrintItemChart.HorzFontRotation;
            // Axis autorange
            if (pname == VerticalAxisAutoRangeName)
                return (int)FPrintItemChart.AutoRange;
            // Axis Y Initial
            if (pname == VerticalAxisMinimum)
                return FPrintItemChart.AxisYInitial;
            // Axis Y Initial
            if (pname == VerticalAxisMaximum)
                return FPrintItemChart.AxisYFinal;

            return base.GetProperty(pname);
        }
        public override void SetProperty(string pname, Variant newvalue)
        {
            // Expression
            if (pname == Translator.TranslateStr(571))
            {
                FPrintItemChart.ValueExpression = newvalue;
            }
            else
            // Expression X
            if (pname == Translator.TranslateStr(571)+" X")
            {
                FPrintItemChart.ValueXExpression = newvalue;
            }
            else
            // Identifier
            if (pname == Translator.TranslateStr(304))
            {
                FPrintItemChart.Identifier = newvalue;
            }
            else
            // ChartType
            if (pname == Translator.TranslateStr(712))
            {
                FPrintItemChart.ChartStyle = (ChartType)(int)newvalue;
            }
            else
            // GetValue Condition
            if (pname == Translator.TranslateStr(717))
            {
                FPrintItemChart.GetValueCondition = newvalue;
            }
            else
            // Change Serie Expression
            if (pname == Translator.TranslateStr(714))
            {
                FPrintItemChart.ChangeSerieExpression = newvalue;
            }
            else
            // Change Serie Expression
            if (pname == Translator.TranslateStr(715))
            {
                FPrintItemChart.ChangeSerieBool = newvalue;
            }
            else
            // Clear Chart Expression
            if (pname == Translator.TranslateStr(966))
            {
                FPrintItemChart.ClearExpression = newvalue.ToString();
            }
            else
            // Change Serie Bool Expression
            if (pname == Translator.TranslateStr(967))
            {
                FPrintItemChart.ClearExpressionBool = newvalue;
            }
            else
            // Caption Expression
            if (pname == Translator.TranslateStr(716))
            {
                FPrintItemChart.CaptionExpression = newvalue;
            }
            else
            // Serie Caption Expression
            if (pname == Translator.TranslateStr(1331))
            {
                FPrintItemChart.SerieCaption = newvalue;
            }
            else
            // Value Color Expression
            if (pname == Translator.TranslateStr(1365))
            {
                FPrintItemChart.ColorExpression = newvalue;
            }
            else
            // Serie Color Expression
            if (pname == Translator.TranslateStr(1364))
            {
                FPrintItemChart.SerieColorExpression = newvalue;
            }
            else
            // Chart Driver
            if (pname == Translator.TranslateStr(67))
            {
                FPrintItemChart.Driver = (ChartDriver)(int)newvalue;
            }
            else
            // View3d
            if (pname == Translator.TranslateStr(904))
            {
                FPrintItemChart.View3d = newvalue;
            }
            else
            // View3dWalls
            if (pname == Translator.TranslateStr(905))
            {
                FPrintItemChart.View3dWalls = newvalue;
            }
            else
            // Perspective
            if (pname == Translator.TranslateStr(906))
            {
                FPrintItemChart.Perspective = newvalue;
            }
            else
            // Elevation
            if (pname == Translator.TranslateStr(907))
            {
                FPrintItemChart.Elevation = newvalue;
            }
            else
            // Rotation
            if (pname == Translator.TranslateStr(908))
            {
                FPrintItemChart.Rotation = newvalue;
            }
            else
            // Orthogonal
            if (pname == Translator.TranslateStr(909))
            {
                FPrintItemChart.Orthogonal = newvalue;
            }
            else
            // Zoom
            if (pname == Translator.TranslateStr(910))
            {
                FPrintItemChart.Zoom = newvalue;
            }
            else
            // Horizontal Offset
            if (pname == Translator.TranslateStr(911))
            {
                FPrintItemChart.HorzOffset = newvalue;
            }
            else
            // Vertical Offset
            if (pname == Translator.TranslateStr(912))
            {
                FPrintItemChart.VertOffset = newvalue;
            }
            else
            // Tilt
            if (pname == Translator.TranslateStr(913))
            {
                FPrintItemChart.Tilt = newvalue;
            }
            else
            // DPIRes
            if (pname == Translator.TranslateStr(666))
            {
                FPrintItemChart.Resolution = newvalue;
            }
            else
            // MultiBar
            if (pname == Translator.TranslateStr(914))
            {
                FPrintItemChart.MultiBar = (BarType)(int)newvalue;
            }
            else
            // Show Hint
            if (pname == Translator.TranslateStr(1228))
            {
                FPrintItemChart.ShowHint = newvalue;
            }
            else
            // Show Legend
            if (pname == Translator.TranslateStr(1229))
            {
                FPrintItemChart.ShowLegend = newvalue;
            }
            else
            // Mark Type
            if (pname == Translator.TranslateStr(1351))
            {
                FPrintItemChart.MarkStyle = newvalue;
            }
            else
            // Vertical axis font size
            if (pname == Translator.TranslateStr(1361))
            {
                FPrintItemChart.VertFontSize = newvalue;
            }
            else
            // Horizontal axis font size
            if (pname == Translator.TranslateStr(1363))
            {
                FPrintItemChart.HorzFontSize = newvalue;
            }
            else
            // Vertical axis font rotation
            if (pname == Translator.TranslateStr(1362))
            {
                FPrintItemChart.VertFontRotation = newvalue;
            }
            else
            // Horizontal axis font rotation
            if (pname == Translator.TranslateStr(1376))
            {
                FPrintItemChart.HorzFontRotation = newvalue;
            }
            else
            // Axis AutoRange
            if (pname == VerticalAxisAutoRangeName)
            {
                FPrintItemChart.AutoRange = (Series.AutoRangeAxis)(int)newvalue;
            }
            else
            // ChartType
            if (pname == VerticalAxisMinimum)
            {
                FPrintItemChart.AxisYInitial = newvalue;
            }
            else
            // ChartType
            if (pname == VerticalAxisMaximum)
            {
                FPrintItemChart.AxisYFinal = newvalue;
            }
            else
                // inherited
                base.SetProperty(pname, newvalue);
            if (FPrintItemChart.DrawHelper != null)
            {
                ((ChartDrawHelper)FPrintItemChart.DrawHelper).Bitmap = null;
            }
        }
        
        public override void GetPropertyValues(string pname, Strings lpossiblevalues)
        {
            // ChartType
            if (pname == Translator.TranslateStr(712))
            {
                lpossiblevalues.Add(Translator.TranslateStr(895)); // Line
                lpossiblevalues.Add(Translator.TranslateStr(897)); // Bar
                lpossiblevalues.Add(Translator.TranslateStr(896)); // Point
                lpossiblevalues.Add(Translator.TranslateStr(898)); // Horizontal Bar
                lpossiblevalues.Add(Translator.TranslateStr(899)); // Area
                lpossiblevalues.Add(Translator.TranslateStr(900)); // Pie
                lpossiblevalues.Add(Translator.TranslateStr(901)); // Arrow
                lpossiblevalues.Add(Translator.TranslateStr(902)); // Bubble
                lpossiblevalues.Add(Translator.TranslateStr(903)); // Gannt
                // Splines, CandleStick, Pyramid, Polar, PointFigure, Funnel, Kagi, Doughnut, Radar, Renko, ErrorBar
                lpossiblevalues.Add("Splines");
                lpossiblevalues.Add("CandleStick");
                lpossiblevalues.Add("Pyramid");
                lpossiblevalues.Add("Polar");
                lpossiblevalues.Add("PointFigure");
                lpossiblevalues.Add("Funnel");
                lpossiblevalues.Add("Kagi");
                lpossiblevalues.Add("Doughnut");
                lpossiblevalues.Add("Radar");
                lpossiblevalues.Add("Renko"); 
                lpossiblevalues.Add("ErrorBar"); 
                return;
            }
            // Driver
            if (pname == Translator.TranslateStr(67))
            {
                lpossiblevalues.Add(Translator.TranslateStr(95)); // Default
                lpossiblevalues.Add(Translator.TranslateStr(893)); // Engine
                lpossiblevalues.Add(Translator.TranslateStr(894)); // TeeChart
                return;
            }
            // MultiBar
            if (pname == Translator.TranslateStr(914))
            {
                lpossiblevalues.Add(Translator.TranslateStr(915)); // None
                lpossiblevalues.Add(Translator.TranslateStr(916)); // Side
                lpossiblevalues.Add(Translator.TranslateStr(917)); // Stacked
                lpossiblevalues.Add(Translator.TranslateStr(918)); // Stacked100
                return;
            }
            // Mark Type
            if (pname == Translator.TranslateStr(1351))
            {
                lpossiblevalues.Add(Translator.TranslateStr(1352)); 
                lpossiblevalues.Add(Translator.TranslateStr(1353)); 
                lpossiblevalues.Add(Translator.TranslateStr(1354)); 
                lpossiblevalues.Add(Translator.TranslateStr(1355));
                lpossiblevalues.Add("Valor 2 dec");
                lpossiblevalues.Add("Valor 3 dec");
                lpossiblevalues.Add("Porc. 1 dec");
                return;
            }
            // Mark Type
            if (pname == VerticalAxisAutoRangeName)
            {
                lpossiblevalues.Add(Translator.TranslateStr(1452));
                lpossiblevalues.Add(Translator.TranslateStr(1453));
                lpossiblevalues.Add(Translator.TranslateStr(1454));
                lpossiblevalues.Add(Translator.TranslateStr(1455));
                lpossiblevalues.Add(Translator.TranslateStr(1456));
                return;
            }
            base.GetPropertyValues(pname, lpossiblevalues);
        }


    }
    class ChartDrawHelper
    {
        public Series ChartSeries;
        public System.Drawing.Bitmap Bitmap;
        ChartItem ChartItem;
        public ChartDrawHelper(ChartItem nChartItem)
        {
            ChartItem = nChartItem;
            BuildSampleSeries();
        }
        List<SeriesItem> DefaultSeries = new List<SeriesItem>();
        void BuildSampleSeries()
        {
            ChartSeries = new Series();
            SeriesItem SerieA = new SeriesItem();
            SerieA.Color = ChartItem.SeriesColors[0];
            SerieA.Caption = "2015";
            SerieA.Values.Add(10.5);
            SerieA.ValueCaptions.Add("Hard Disk");
            SerieA.Colors.Add(SerieA.Color);
            SerieA.Values.Add(20);
            SerieA.ValueCaptions.Add("CPU");
            SerieA.Colors.Add(SerieA.Color);
            SerieA.Values.Add(15);
            SerieA.ValueCaptions.Add("Graphics");
            SerieA.Colors.Add(SerieA.Color);
            SerieA.Values.Add(14);
            SerieA.ValueCaptions.Add("Display");
            SerieA.Colors.Add(SerieA.Color);

            SeriesItem SerieB = new SeriesItem();
            SerieB.Color = ChartItem.SeriesColors[1];
            SerieB.Caption = "2016";
            SerieB.Values.Add(12.5);
            SerieB.ValueCaptions.Add("Hard Disk");
            SerieB.Colors.Add(SerieB.Color);
            SerieB.Values.Add(21);
            SerieB.ValueCaptions.Add("CPU");
            SerieB.Colors.Add(SerieB.Color);
            SerieB.Values.Add(14);
            SerieB.ValueCaptions.Add("Graphics");
            SerieB.Colors.Add(SerieB.Color);
            SerieB.Values.Add(16.34);
            SerieB.ValueCaptions.Add("Display");
            SerieB.Colors.Add(SerieB.Color);

            DefaultSeries.Add(SerieA);
            DefaultSeries.Add(SerieB);

            ChartSeries.SeriesItems.Add(SerieA);
            ChartSeries.SeriesItems.Add(SerieB);
        }
        public void UpdateSeries()
        {

            ChartSeries.AutoRange = ChartItem.AutoRange;
            ChartSeries.Effect3D = ChartItem.View3d;
            ChartSeries.FontSize = ChartItem.VertFontSize;
            ChartSeries.HorzFontRotation = ChartItem.HorzFontRotation;
            ChartSeries.VertFontRotation = ChartItem.VertFontRotation;
            ChartSeries.ShowHint = ChartItem.ShowHint;
            ChartSeries.ShowLegend = ChartItem.ShowLegend;
            ChartSeries.Resolution = ChartItem.Resolution;
            ChartSeries.MarkStyle = ChartItem.MarkStyle;
            ChartSeries.MultiBar = ChartItem.MultiBar;
            ChartSeries.PrintWidth = ChartItem.Width;
            ChartSeries.PrintHeight = ChartItem.Height;
            ChartSeries.Clear();
            if ((ChartItem.ChartStyle == ChartType.Renko) || (ChartItem.ChartStyle == ChartType.Pie)
                  || (ChartItem.ChartStyle == ChartType.Doughnut))
            {
                ChartSeries.SeriesItems.Add(DefaultSeries[0]);
            }
            else
            {
                foreach (var serie in DefaultSeries)
                {
                    ChartSeries.SeriesItems.Add(serie);
                }
            }
            foreach (SeriesItem nitem in ChartSeries.SeriesItems)
            {
                nitem.ChartStyle = ChartItem.ChartStyle;
            }
        }
    }
}


