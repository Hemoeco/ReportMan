using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Reporting
{
    class VariableGraph : IdenVariable
    {
        public BaseReport Report;
        public ChartItem NewChart;
        public VariableGraph(Evaluator eval, BaseReport rp)
            : base(eval)
        {
            Report = rp;
        }
        public void Clear()
        {
            NewChart.Clear();
        }
        public void NewValue(double newvalue, bool seriechange, string valuecaption, string seriecaption)
        {
            NewChart.NewValue(newvalue, seriechange, valuecaption, seriecaption);
        }
        public void NewValueXY(double newvalueX, double newvalue, bool seriechange, string valuecaption, string seriecaption)
        {
            NewChart.NewValueXY(newvalueX, newvalue, seriechange, valuecaption, seriecaption);
        }

        public void NewFunction(string functionName, string functionParams, string serieCaption)
        {
            NewChart.NewFunction(functionName, functionParams, serieCaption);
        }
        public void GraphicBounds(bool autol, bool autoh, double lvalue, double hvalue, bool logaritmic,
                double logBase, bool inverted)
        {
            NewChart.GraphicBounds(autol, autoh, lvalue, hvalue, logaritmic, logBase, inverted);
        }
        public void GraphicColor(int newcolor)
        {
            NewChart.GraphicColor(newcolor);
        }
        public void GraphicSerieColor(int newcolor)
        {
            NewChart.GraphicSerieColor(newcolor);
        }

    }
}
