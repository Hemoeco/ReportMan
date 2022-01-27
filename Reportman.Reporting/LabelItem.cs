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
using System.Drawing;
using System.IO;
using Reportman.Drawing;

namespace Reportman.Reporting
{
	public enum Aggregate { None, Group, Page, General };
	public enum AggregateType { Summary, Minimum, Maximum, Average, StandardDeviation };
	public class LabelItem : PrintItemText
	{
		Strings FAllStrings;
		public LabelItem(BaseReport rp)
			: base(rp)
		{
			FAllStrings = new Strings();
		}
		public Strings AllStrings { get { return FAllStrings; } }
		public string Text
		{
			get
			{
				int lang = Report.Language+1;
				if (lang < 0)
					lang = 0;
                if (lang < FAllStrings.Count)
                    return FAllStrings[lang];
                else
                    if (FAllStrings.Count > 0)
                        return FAllStrings[0];
                    else
                        return "";

			}
			set
			{
				int lang = Report.Language;
				if (lang<0)
					lang=0;
				string defaultvalue = "";
				if (FAllStrings.Count > 0)
					defaultvalue = FAllStrings[0];
				while (lang > (FAllStrings.Count-1))
				{
					FAllStrings.Add(defaultvalue);
				}
				FAllStrings[lang] = value;
			}
		}
        protected override string GetClassName()
        {
            return "TRPLABEL";
        }
        override protected void DoPrint(PrintOut adriver, int aposx, int aposy,
			int newwidth, int newheight, MetaFile metafile, Point MaxExtent,
			ref bool PartialPrint)
		{
			int aalign;
			base.DoPrint(adriver, aposx, aposy, newwidth, newheight,
				metafile, MaxExtent, ref PartialPrint);
			MetaPage apage = metafile.Pages[metafile.CurrentPage];
			MetaObjectText metaobj = new MetaObjectText();
			metaobj.TextP = apage.AddString(Text);
			metaobj.TextS = Text.Length;
			metaobj.LFontNameP = apage.AddString(LFontName);
			metaobj.LFontNameS = LFontName.Length;
			metaobj.WFontNameP = apage.AddString(WFontName);
			metaobj.WFontNameS = WFontName.Length;
			metaobj.FontSize = FontSize;
			metaobj.BackColor = BackColor;
			metaobj.FontRotation = FontRotation;
			metaobj.FontStyle = (short)FontStyle;
			metaobj.FontColor = FontColor;
			metaobj.Type1Font = Type1Font;
			metaobj.CutText = CutText;
			metaobj.Transparent = Transparent;
			metaobj.WordWrap = WordWrap;
			metaobj.Top = aposy;
			metaobj.Left = aposx;
			metaobj.Width = PrintWidth;
			metaobj.Height = PrintHeight;
			metaobj.RightToLeft=RightToLeft;
			metaobj.PrintStep = PrintStep;
			aalign = PrintAlignment | VPrintAlignment;
			if (SingleLine)
				aalign = aalign | MetaFile.AlignmentFlags_SingleLine;
			metaobj.Alignment = aalign;
			apage.Objects.Add(metaobj);
		}
		private TextObjectStruct GetTextObject()
		{
			int aalign;
			TextObjectStruct aresult = new TextObjectStruct();
			aresult.Text = Text;
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
			aresult.RightToLeft=RightToLeft;
			aresult.PrintStep = PrintStep;
			return aresult;
		}
        override public Point GetExtension(PrintOut adriver, Point MaxExtent, bool ForcePartial)
        {
            TextObjectStruct atext;
            atext = GetTextObject();
            Point aresult = base.GetExtension(adriver,MaxExtent,ForcePartial);
            aresult = adriver.TextExtent(atext, aresult);
            LastExtent = aresult;
            return aresult;
        }
    }
	public class EvalIdenExpression : IdenFunction
	{
		public EvalIdenExpression(Evaluator eval)
			: base(eval)
		{ }
		public ExpressionItem ExpreItem;
		protected override Variant GetValue()
		{
            ExpreItem.Evaluate();
			return ExpreItem.Value;
		}
	}
}
