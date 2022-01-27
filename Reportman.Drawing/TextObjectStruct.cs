using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Drawing
{
    /// <summary>
    /// This structure is used to store information about a block of text properties, alignment and content, 
    /// it is used by some functions to measure texts or pass text and format information in a single variable
    /// </summary>
	public struct TextObjectStruct
    {
        /// <summary>Text content</summary>
		public string Text;
        /// <summary>Family name of the font when the text is drawn in Linux</summary>
        public string LFontName;
        /// <summary>Family name of the font when the text is drawn in Microsoft Windows</summary>
        public string WFontName;
        /// <summary>Font size</summary>
        public short FontSize;
        /// <summary>Font rotation</summary>
        public short FontRotation;
        /// <summary>Font style as a short</summary>
        public short FontStyle;
        /// <summary>Font color</summary>
        public int FontColor;
        /// <summary>Font type when exporting to Adobe PDF</summary>
        public PDFFontType Type1Font;
        /// <summary>Flag to indicate if the text must be clip to the box</summary>
        public bool CutText;
        /// <summary>Text alignment as an integer</summary>
        public int Alignment;
        /// <summary>Flag to indicate if the text must wrap multiple lines</summary>
        public bool WordWrap;
        /// <summary>Flag to indicate if the text must be dran right to left</summary>
        public bool RightToLeft;
        /// <summary>Print step, for dot matrix printers</summary>
        public PrintStepType PrintStep;
        /// <summary>
        /// Returns a TextObjectStruct from a MetaObject
        /// </summary>
        /// <param name="apage">Parent page of the MetaObject</param>
        /// <param name="aobj">MetaObject with data</param>
        public static TextObjectStruct FromMetaObjectText(MetaPage apage, MetaObjectText aobj)
        {
            TextObjectStruct aresult = new TextObjectStruct();
            aresult.Text = apage.GetText(aobj);
            aresult.LFontName = apage.GetLFontNameText(aobj);
            aresult.WFontName = apage.GetWFontNameText(aobj);
            aresult.WordWrap = aobj.WordWrap;
            aresult.Type1Font = aobj.Type1Font;
            aresult.RightToLeft = aobj.RightToLeft;
            aresult.FontColor = aobj.FontColor;
            aresult.FontRotation = aobj.FontRotation;
            aresult.FontStyle = aobj.FontStyle;
            aresult.PrintStep = aobj.PrintStep;
            aresult.CutText = aobj.CutText;
            aresult.FontSize = aobj.FontSize;
            aresult.Alignment = aobj.Alignment;
            return aresult;
        }
    }
}
