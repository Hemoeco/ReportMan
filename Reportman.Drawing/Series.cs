using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Drawing
{
    public class Series
    {
        public enum AutoRangeAxis { Default = 0, AutoBoth = 1, AutoUpper=2, AutoLower = 3, None = 4 };
        public AutoRangeAxis AutoRange;
        public double LowValue;
        public double HighValue;
        public bool Logaritmic;
        public double LogBase;
        public bool Inverted;
        public float FontSize;
        public float Resolution = 100;
        public bool ShowLegend;
        public int HorzFontRotation;
        public int VertFontRotation;
        public int VertFontSize;
        public int HorzFontSize;
        public bool ShowHint;
        public int PrintWidth;
        public int PrintHeight;
        public bool Effect3D;
        public int MarkStyle;
        public BarType MultiBar;
        public List<SeriesItem> SeriesItems = new List<SeriesItem>();
        public Series()
        {
            LowValue = 0.0;
            HighValue = 0.0;
            LogBase = 0.0;
            Inverted = false;
            AutoRange = AutoRangeAxis.Default;
            Effect3D = false;
        }
        public void Clear()
        {
            SeriesItems.Clear();
        }
    }
    public enum ChartType
    {
        Line, Bar, Point, Horzbar, Area, Pie, Arrow,
        Bubble, Gantt, Splines, CandleStick, Pyramid, Polar, PointFigure, Funnel, Kagi, Doughnut, Radar, Renko, ErrorBar
    };
    public enum ChartDriver { Default, Engine, Teechart };
    public enum BarType { None, Side, Stacked, Stacked100 };
    public class SeriesItem
    {
        public Doubles Values;
        public List<object> ValuesX;
        public Integers Colors;
        public Strings ValueCaptions;
        public int Color;
        public string Caption;
        public double MaxValue;
        public double MinValue;
        public ChartType ChartStyle;
        public string FunctionName;
        public string FunctionParams;
        public SeriesItem()
        {
            Color = -1;
            MaxValue = -10e300;
            MinValue = +10e300;
            Values = new Doubles();
            ValuesX = new List<object>();
            Colors = new Integers();
            ValueCaptions = new Strings();
            Caption = "";
            ChartStyle = ChartType.Bar;

        }
        public void SetLastValueColor(int newcolor)
        {
            if (Colors.Count == 0)
                return;
            Colors[Colors.Count - 1] = newcolor;
        }
    }
}
