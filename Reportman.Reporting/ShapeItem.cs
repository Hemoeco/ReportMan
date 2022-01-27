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
using System.IO;
using System.Drawing;
using Reportman.Drawing;

namespace Reportman.Reporting
{
	public class ShapeItem : PrintPosItem
	{
		private const int DEF_DRAWWIDTH = 500;
		public BrushType BrushStyle;
		public int BrushColor;
		public PenType PenStyle;
		public int Color;
		public ShapeType Shape;
		public int PenWidth;
		public int PenColor;
        public string BrushColorExpression;
		public ShapeItem(BaseReport rp)
			: base(rp)
		{
			BrushColor = 0xFFFFFF;
			Height = DEF_DRAWWIDTH;
			Width = Height;
            BrushColorExpression = "";
		}
        protected override string GetClassName()
        {
            return "TRPSHAPE";
        }
        override protected void DoPrint(PrintOut adriver, int aposx, int aposy,
			int newwidth, int newheight, MetaFile metafile, Point MaxExtent,
			ref bool PartialPrint)
		{
			base.DoPrint(adriver, aposx, aposy, newwidth, newheight,
				metafile, MaxExtent, ref PartialPrint);
			MetaObjectDraw metaobj = new MetaObjectDraw();
			metaobj.MetaType = MetaObjectType.Draw;
			metaobj.Top = aposy; metaobj.Left = aposx;
			metaobj.Width = PrintWidth; metaobj.Height = PrintHeight;
			metaobj.DrawStyle = Shape;
			metaobj.BrushStyle = (int)BrushStyle;
			metaobj.PenStyle = (int)PenStyle;
			metaobj.PenWidth = PenWidth;
			metaobj.PenColor = PenColor;
            if (BrushColorExpression.Length > 0)
            {
                try
                {
                    metaobj.BrushColor = Report.Evaluator.EvaluateText(BrushColorExpression);
                }
                catch
                {
                    metaobj.BrushColor = BrushColor;
                }
            }
            else
                metaobj.BrushColor = BrushColor;
			metafile.Pages[metafile.CurrentPage].Objects.Add(metaobj);
		}
        override public Point GetExtension(PrintOut adriver, Point MaxExtent, bool ForcePartial)
		{
			Point aresult = base.GetExtension(adriver, MaxExtent,ForcePartial);
			if (Shape==ShapeType.HorzLine)
			{
				aresult.Y = PenWidth;
			}
			LastExtent = aresult;
			return aresult;
		}
	}
}
