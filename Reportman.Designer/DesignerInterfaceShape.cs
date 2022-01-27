using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Designer
{
    internal class DesignerInterfaceShape:DesignerInterfaceSizePos
    {
        private ShapeItem FPrintItemShape;
        public DesignerInterfaceShape(SortedList<int, ReportItem> repitem, IObjectInspector objinsp)
            : base(repitem,objinsp)
        {
          if (repitem.Count > 0)
            FPrintItemShape = (ShapeItem)repitem.Values[0];
        }
        public ShapeItem PrintItemShapeObject { get { return FPrintItemShape; } }
        public override void GetProperties(Strings lnames, Strings ltypes, Variants lvalues, Strings lhints, Strings lcat)
        {
            base.GetProperties(lnames, ltypes, lvalues, lhints, lcat);

            // Shape
            lnames.Add(Translator.TranslateStr(643));
            ltypes.Add(Translator.TranslateStr(569));
            lhints.Add("refdraw.html");
            lcat.Add(Translator.TranslateStr(643));
            if (lvalues!=null)
                lvalues.Add((int)FPrintItemShape.Shape);


            // Pen style
             lnames.Add(Translator.TranslateStr(646));
             ltypes.Add(Translator.TranslateStr(569));
             lhints.Add("refdraw.html");
             lcat.Add(Translator.TranslateStr(643));
             if (lvalues!=null)
              lvalues.Add((int)FPrintItemShape.PenStyle);

             // Pen Color
             lnames.Add(Translator.TranslateStr(647));
             ltypes.Add(Translator.TranslateStr(558));
             lhints.Add("refdraw.html");
             lcat.Add(Translator.TranslateStr(643));
             if (lvalues!=null)
              lvalues.Add(FPrintItemShape.PenColor);

             // PenWidth
             lnames.Add(Translator.TranslateStr(648));
             ltypes.Add(Translator.TranslateStr(556));
             lhints.Add("refdraw.html");
             lcat.Add(Translator.TranslateStr(643));
             if (lvalues!=null)
              lvalues.Add(FPrintItemShape.PenWidth);


             // Brush style
             lnames.Add(Translator.TranslateStr(644));
             ltypes.Add(Translator.TranslateStr(569));
             lhints.Add("refdraw.html");
             lcat.Add(Translator.TranslateStr(643));
             if (lvalues!=null)
              lvalues.Add((int)FPrintItemShape.BrushStyle);

             // Brush Color
             lnames.Add(Translator.TranslateStr(645));
             ltypes.Add(Translator.TranslateStr(558));
             lhints.Add("refdraw.html");
             lcat.Add(Translator.TranslateStr(643));
             if (lvalues!=null)
              lvalues.Add(FPrintItemShape.BrushColor);


        }
        public override Variant GetProperty(string pname)
        {
            if (pname == Translator.TranslateStr(643))
                return (int)FPrintItemShape.Shape;
            if (pname == Translator.TranslateStr(646))
                return (int)FPrintItemShape.PenStyle;
            if (pname == Translator.TranslateStr(647))
                return FPrintItemShape.PenColor;
            if (pname == Translator.TranslateStr(648))
                return FPrintItemShape.PenWidth;
            if (pname == Translator.TranslateStr(644))
                return (int)FPrintItemShape.BrushStyle;
            if (pname == Translator.TranslateStr(645))
                return FPrintItemShape.BrushColor;

            return base.GetProperty(pname);
        }
        public override void SetProperty(string pname, Variant newvalue)
        {
            // Shape
            if (pname == Translator.TranslateStr(643))
            {
                FPrintItemShape.Shape = (ShapeType)(int)newvalue;
                return;
            }
            // PenStyle
            if (pname == Translator.TranslateStr(646))
            {
                FPrintItemShape.PenStyle = (PenType)(int)newvalue;
                return;
            }
            // Brush Style
            if (pname == Translator.TranslateStr(644))
            {
                FPrintItemShape.BrushStyle = (BrushType)(int)newvalue;
                return;
            }
            // PenStyle
            if (pname == Translator.TranslateStr(646))
            {
                FPrintItemShape.PenStyle = (PenType)(int)newvalue;
                return;
            }
            // Pen Color
            if (pname == Translator.TranslateStr(647))
            {
                FPrintItemShape.PenColor = newvalue;
                return;
            }
            // Pen Width
            if (pname == Translator.TranslateStr(648))
            {
                FPrintItemShape.PenWidth = newvalue;
                return;
            }
            // Brush Color
            if (pname == Translator.TranslateStr(645))
            {
                FPrintItemShape.BrushColor = newvalue;
                return;
            }
            // inherited
            base.SetProperty(pname, newvalue);
        }

        public override void GetPropertyValues(string pname, Strings lpossiblevalues)
        {
            // Shape
            if (pname == Translator.TranslateStr(643))
            {
                lpossiblevalues.Add(Translator.TranslateStr(606)); // Rectangle
                lpossiblevalues.Add(Translator.TranslateStr(609)); // Square
                lpossiblevalues.Add(Translator.TranslateStr(607)); // RoundRect
                lpossiblevalues.Add(Translator.TranslateStr(608)); // RoundSquare
                lpossiblevalues.Add(Translator.TranslateStr(605)); // Ellipse
                lpossiblevalues.Add(Translator.TranslateStr(604)); // Circle
                lpossiblevalues.Add(Translator.TranslateStr(306)); // Horz Line
                lpossiblevalues.Add(Translator.TranslateStr(307)); // Vert Line
                lpossiblevalues.Add(Translator.TranslateStr(719)); // Oblique 1
                lpossiblevalues.Add(Translator.TranslateStr(720)); // Oblique 2
                return;
            }
            // PenStyle
            if (pname == Translator.TranslateStr(646))
            {
                lpossiblevalues.Add(Translator.TranslateStr(598)); // Solid
                lpossiblevalues.Add(Translator.TranslateStr(599)); // Dash
                lpossiblevalues.Add(Translator.TranslateStr(600)); // Dot
                lpossiblevalues.Add(Translator.TranslateStr(601)); // DashDot
                lpossiblevalues.Add(Translator.TranslateStr(602)); // DashDotDot
                lpossiblevalues.Add(Translator.TranslateStr(603)); // Clear
                return;
            }
            // BrushStyle
            if (pname == Translator.TranslateStr(644))
            {
                lpossiblevalues.Add(Translator.TranslateStr(583)); // Solid
                lpossiblevalues.Add(Translator.TranslateStr(584)); // Clear
                lpossiblevalues.Add(Translator.TranslateStr(585)); // Fill Horz.
                lpossiblevalues.Add(Translator.TranslateStr(586)); // Fill Vert
                lpossiblevalues.Add(Translator.TranslateStr(587)); // Fill Diagonal
                lpossiblevalues.Add(Translator.TranslateStr(588)); // Fill Diagonal 2
                lpossiblevalues.Add(Translator.TranslateStr(589)); // Fill Cross
                lpossiblevalues.Add(Translator.TranslateStr(590)); // Fill Diag Cross
                lpossiblevalues.Add(Translator.TranslateStr(591)); // Fill Dense 1
                lpossiblevalues.Add(Translator.TranslateStr(592)); // Fill Dense 2
                lpossiblevalues.Add(Translator.TranslateStr(593)); // Fill Dense 3
                lpossiblevalues.Add(Translator.TranslateStr(594)); // Fill Dense 4
                lpossiblevalues.Add(Translator.TranslateStr(595)); // Fill Dense 5
                lpossiblevalues.Add(Translator.TranslateStr(596)); // Fill Dense 6
                lpossiblevalues.Add(Translator.TranslateStr(597)); // Fill Dense 7
                return;
            }
            base.GetPropertyValues(pname, lpossiblevalues);
        }


    }
}
