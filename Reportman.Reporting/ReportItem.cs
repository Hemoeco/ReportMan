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
using System.Collections;
using Reportman.Drawing;
using System.ComponentModel;
using System.Collections.Generic;

namespace Reportman.Reporting
{
    /// <summary>
    /// Enumeration indicating if the image should be shared, so only one instance of the image is stored
    /// while generating the report (ReportMetafile), if the image is repeated in the report, it's recommended
    /// to share it, so the image is stored only once, and not for each page (a internal reference is done).
    /// <see cref="Variant">ImageItem</see>
    /// </summary>
    /// <remarks>
    /// When exporting the Report to Adobe PDF format, it's recommended to share images that are repeated
    /// in each page, so the resulting file is smaller
    /// </remarks>
	public enum SharedImageType {
        /// <summary>The image will not be shared, recommended for variable images</summary>
        None,
        /// <summary>The image will be shared, for better performance, the reporting engine will assume that
        /// the image does not change in later drawings, recommended for page header logos for example</summary>
        Fixed,
        /// <summary>The image will be shared, but the engine must check it, so if the image changes while
        /// the report is processing, the new image will be stored and also shared until another change.
        /// This is recommended when the image will be printed in more than one page, then it changes and again 
        /// repeats in more pages. For example in a categorized price list, printing an image for each category (wines,
        /// cheese). Checking this option will optimize the size of the resulting MetaFile (or Adobe PDF file)
        /// but will hurt performance, specially for large images, because the engine must check for changes in the image
        /// each time is drawn</summary>
        Variable
    };
    /// <summary>
    /// Enumeration indicating the alignment of a Report Item related to his parent. This is useful when the 
    /// parent Section have properties like AutoExpand or AutoContract set, you can place a Rectangle 
    /// and set Align to AllClient, the rectangle will be adapted at run time to fill the section. You can also
    /// align to bottom or expand lines from top to bottom.
    /// <see cref="Variant">PrintPosItem</see><see cref="Variant">Section</see>
    /// </summary>
    public enum PrintItemAlign { 
        /// <summary>No alignment, default will be used</summary>
        None,
        /// <summary>Align to bottom of the parent</summary>
        Bottom,
        /// <summary>Align to the right of the parent</summary>
        Right,
        /// <summary>Align to the right and bottom of the parent</summary>
        BottomRight,
        /// <summary>Expand from left to right of the parent</summary>
        LeftRight,
        /// <summary>Expand from top to bottom of the parent</summary>
        TopBottom,
        /// <summary>Expand to all area of the parent</summary>
        AllClient
    };
    /// <summary>
    /// Base classs for Report items providing common base functionality, and a relation to the owner (Report).
    /// <see cref="Variant">Report</see>
    /// <see cref="Variant">Section</see>
    /// </summary>
    [TypeConverter(typeof(ReportItemTypeConverter))]
    public class ReportItem:IDisposable
	{
        private string FName;
        /// <summary>
        /// Name of the print item
        /// </summary>        
        [Browsable(false)]
        public string Name
		{
			get
			{
				return (FName);
			}
			set
			{
                if (FName.Length > 0)
                {
                    if (Report.Components.IndexOfKey(Name) >= 0)
                    {
                        if (Report.Components[Name] == this)
                        {
                            Report.RemoveComponent(this);
                        }
                    }
                }
				FName = value;
                if (FName.Length > 0)
                {
                    if (Report.Components.IndexOfKey(FName.ToUpper())>=0)
                    {
                        FName = Report.FindNewName(this);
                    }
                    Report.AddComponent(this);
                }
			}
		}
		private BaseReport FReport;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rp">Report, the owner of the item</param>
		public ReportItem(BaseReport rp)
		{
			FReport = rp;
			FName = "";
		}
        /// <summary>
        /// Report that owns the item
        /// </summary>
        [Browsable(false)]
        public BaseReport Report
		{
			get { return FReport; }
			set {FReport=value;}
		}
        /// <summary>
        /// Internal function to determine the object type name
        /// </summary>
        /// <returns>Object type name</returns>
        protected virtual string GetClassName()
        {
            return "";
        }
        /// <summary>
        /// Provide the class name, that is the object type name
        /// </summary>
        [Browsable(false)]
        public string ClassName
        {
            get
            {
                return GetClassName();
            }
        }
        /// <summary>
        /// Disposes any memory used
        /// </summary>
        public virtual void Dispose()
        {

        }
    }
    /// <summary>
    /// Basic class providing more basic functionality this item provides printing capability and some events
    /// </summary>
	public class PrintItem : ReportItem
	{
        /// <summary>Width in twips of the printable item</summary>
        [Category("Position")]
        public int Width { get; set; }
        /// <summary>Height in twips of the printable item</summary>
        [Category("Position")]
        public int Height { get; set; }
        /// <summary>Width in twips of the printable item, variable at runtime</summary>
        public int PrintWidth;
        /// <summary>Height in twips of the printable item, variable at runtime</summary>
        public int PrintHeight;
        /// <summary>Expression evaluated before printing the item</summary>
        public string DoBeforePrint;
        /// <summary>Expression evaluated after printing the item</summary>
        public string DoAfterPrint;
        /// <summary>When this property is not empty, it will be evaluated by the reporting engine,
        /// if the result is false, the item will not print</summary>
        public string PrintCondition;
        /// <summary>Visible will determine if the item must be shown at design time</summary>
		public bool Visible;
        /// <summary>Last calculated extent at runtime, while processing the report</summary>
        public Point LastExtent;
    /// <summary>
    /// Selecction index
    /// </summary>
    public int SelectionIndex;
    public Rectangle SelectionRectangle;
        /// <summary>
        /// Constructor
        /// </summary>
		public PrintItem(BaseReport rp)
			: base(rp)
		{
			Visible = true;
			Height = 0;
			Width = 0;
			PrintCondition = "";
			DoAfterPrint = "";
			DoBeforePrint = "";
		}
        /// <summary>
        /// Print
        /// </summary>
        /// <returns>Returns true if the print condition is empty or evaluated to true</returns>
		public bool EvaluatePrintCondition()
		{
			if (PrintCondition.Length == 0)
				return true;
			Evaluator fevaluator;
            bool nresult = false;
			fevaluator = Report.Evaluator;
			Variant aresult;
			try
			{
				fevaluator.Expression = PrintCondition;
				fevaluator.Evaluate();
				aresult = fevaluator.Result;
                nresult = aresult;
			}
			catch (Exception E)
			{
				throw new ReportException(E.Message + ":" + Name + " Prop:PrintCondition " + PrintCondition, this, "PrintCondition");
			}
			return nresult;
        }
        /// <summary>
        /// This event is generated while report is processing to allow the report items to update contents
        /// </summary>
        /// <param name="newstate">New state for the subreport</param>
        /// <param name="newgroup">New group if apply</param>
		public virtual void SubReportChanged(SubReportEvent newstate, string newgroup)
		{
		}
        /// <summary>
        /// This procedure will return the current size of the printed item
        /// </summary>
        /// <param name="adriver">Report processing driver</param>
        /// <param name="MaxExtent">Maximum extension</param>
        /// <returns></returns>
		public virtual Point GetExtension(PrintOut adriver, Point MaxExtent,bool ForcePartial)
		{
			Point aresult = new Point();
			aresult.X = Width;
			aresult.Y = Height;
			LastExtent = aresult;
			return aresult;
		}
        /// <summary>
        /// This procedure will print the item into the MetaFile, internal implementation
        /// </summary>
        /// <param name="adriver">Report processing driver</param>
        /// <param name="aposx">Horizontal position in twips</param>
        /// <param name="aposy">Vertical position in twips</param>
        /// <param name="newwidth">Width of the bounding box in twips</param>
        /// <param name="newheight">Height of the bounding box in twips</param>
        /// <param name="metafile">Destination MetaFile</param>
        /// <param name="MaxExtent">Maximum extension</param>
        /// <param name="PartialPrint">Returns true if some text will expand multiple pages</param>
		protected virtual void DoPrint(PrintOut adriver, int aposx, int aposy,
			int newwidth, int newheight, MetaFile metafile, Point MaxExtent,
			ref bool PartialPrint)
		{
			if (newwidth >= 0)
				PrintWidth = newwidth;
			else
				PrintWidth = Width;
			if (newheight >= 0)
				PrintHeight = newheight;
			else
				PrintHeight = Height;
			PartialPrint = false;
		}
        /// <summary>
        /// This procedure will print the item into the MetaFile
        /// </summary>
        /// <param name="adriver">Report processing driver</param>
        /// <param name="aposx">Horizontal position in twips</param>
        /// <param name="aposy">Vertical position in twips</param>
        /// <param name="newwidth">Width of the bounding box in twips</param>
        /// <param name="newheight">Height of the bounding box in twips</param>
        /// <param name="metafile">Destination MetaFile</param>
        /// <param name="MaxExtent">Maximum extension</param>
        /// <param name="PartialPrint">Returns true if some text will expand multiple pages</param>
        public void Print(PrintOut adriver, int aposx, int aposy,
			int newwidth, int newheight, MetaFile metafile, Point MaxExtent,
			ref bool PartialPrint)
		{
			if (!EvaluatePrintCondition())
				return;
			ExecuteBeforePrint();

			DoPrint(adriver, aposx, aposy, newwidth, newheight, metafile, MaxExtent, ref PartialPrint);

			ExecuteAfterPrint();
		}
        /// <summary>
        /// Internal implementation for actions to do before printing the item, in this base class
        /// the DoBeforePrint expression will be evaluated
        /// </summary>
		protected void ExecuteBeforePrint()
		{
			// Do Before print and doafter print
			Evaluator fevaluator;
			if (DoBeforePrint.Length > 0)
			{
				try
				{
					fevaluator = Report.Evaluator;
					fevaluator.Expression = DoBeforePrint;
					fevaluator.Evaluate();
				}
				catch (Exception E)
				{
					throw new ReportException(E.Message + ":BeforePrint " + Name,
						this, "BeforePrint");
				}
			}

		}
        /// <summary>
        /// Internal implementation for actions to do after printing the item, in this base class
        /// the DoAfterPrint expression will be evaluated
        /// </summary>
        protected void ExecuteAfterPrint()
		{
			Evaluator fevaluator;
			if (DoAfterPrint.Length > 0)
			{
				try
				{
					fevaluator = Report.Evaluator;
					fevaluator.Expression = DoAfterPrint;
					fevaluator.Evaluate();
				}
				catch (Exception E)
				{
					throw new ReportException(E.Message + ":AfterPrint " + Name,
						this, "AfterPrint");
				}
			}
		}
	}
    /// <summary>
    /// Collections of ReportItems
    /// </summary>
#if REPMAN_DOTNET1
	public class ReportItems
	{
		SortedList FItems;
		SortedListStringIndexer FIndexer;
		
		public int Count
		{
			get
			{
				return FItems.Count;
			}
		}
		public ReportItems()
		{
			FItems = new SortedList();
		}
		public void Clear()
		{
			FItems.Clear();
		}
		public void Add(string key,ReportItem obj)
		{
			FItems.Add(key,obj);
		}
		/// <summary>
		/// Returns the report component by index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ReportItem this[int index]
		{
			get
			{ 
				CheckRange(index); 
				string key=(string)FItems.GetKey(index);
				return (ReportItem)FItems[key]; 
			}
			set 
			{ 
				CheckRange(index); 
				string key=(string)FItems.GetKey(index);
				FItems[key] = value; 
			}
		}
		/// <summary>
		/// Returns the index of a key in the list
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int IndexOfKey(string key)
		{
			return FItems.IndexOfKey(key);
		}
		/// <summary>
		/// Removes a key
		/// </summary>
		/// <param name="key"></param>
		public void Remove(string key)
		{
			FItems.Remove(key);
			FIndexer=new SortedListStringIndexer(FItems);
		}
		/// <summary>
		/// Returns the report component by name
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ReportItem this[string key]
		{
			get 
			{ 
				return (ReportItem)FItems[key]; 
			}
			set 
			{ 
				FItems[key] = value; 
			}
		}
		/// <summary>
		/// Returns the key of an index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public SortedListStringIndexer Keys
		{
			get 
			{ 
				return FIndexer; 
			}
		}
#else
    public class ReportItems : System.Collections.Generic.SortedList<string,ReportItem>
    {
        /// <summary>
        /// Returns the report component by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ReportItem this[int index]
        {
            get 
            { 
                CheckRange(index); 
                return this[Keys[index]]; 
            }
            set 
            { 
                CheckRange(index); 
                this[Keys[index]] = value; 
            }
        }
#endif
        private void CheckRange(int index)
        {
            if ((index < 0) || (index >= Count))
                throw new UnNamedException("Index out of range on ReportItems collection");
        }
        /// <summary>
        /// IEnumerable Interface Implementation:
        ///   Declaration of the GetEnumerator() method 
        ///   required by IEnumerable
        /// </summary>
        /// <returns></returns>
        public new IEnumerator GetEnumerator()
		{
			return new ReportItemEnumerator(this);
		}
        /// <summary>
        /// Inner class implements IEnumerator interface:
        /// </summary>
        public class ReportItemEnumerator : IEnumerator
        {
            private int position = -1;
            private ReportItems t;
            /// <summary>
            /// Constructor for a enumerator of report items
            /// </summary>
            /// <param name="t"></param>
            public ReportItemEnumerator(ReportItems t)
            {
                this.t = t;
            }

            /// <summary>
            /// Go to next element in the lis
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (position < t.Count - 1)
                {
                    position++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Declare the Reset method required by IEnumerator
            /// </summary>
            public void Reset()
            {
                position = -1;
            }
            /// <summary>
            /// Declare the Current property required by IEnumerator
            /// </summary>
            public object Current
            {
                get
                {
                    return t[position];
                }
            }
        }
    }
    /// <summary>
    /// Base print item providing position and alignment properties
    /// </summary>
    public class PrintPosItem : PrintItem
    {
        /// <summary>
        /// Hidden flag
        /// </summary>
        public bool Hidden;
        /// <summary>
        /// Horizontal print position in twips, related to the parent
        /// </summary>
        public int PosX;
        /// <summary>
        /// Vertical print position in twips, related to the parent
        /// </summary>
        public int PosY;
        /// <summary>
        /// Item alignment, when priting the
        /// parent section in multiple pages, the Z order (back/front) of
        /// the item and the alignment will determine if it will be printed
        /// at the last page, that is when aligned to bottom or bottom/right and
        /// in the front
        /// </summary>
        public PrintItemAlign Align;
        /// <summary>
        /// Internal flag determining the behaviour 
        /// </summary>
        public bool PartialFlag;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <summary>
        /// Parent section
        /// </summary>
        public Section Section;
        /// <summary>
        /// Constructor
        /// </summary>
        public PrintPosItem(BaseReport rp)
            : base(rp)
        {

        }
        /// <summary>
        /// This procedure will return the current size of the printed item
        /// </summary>
        /// <param name="adriver">Report processing driver</param>
        /// <param name="MaxExtent">Maximum extension</param>
        /// <returns></returns>
        public override Point GetExtension(PrintOut adriver, Point MaxExtent, bool ForcePartial)
        {
            Point aresult = new Point();
            if (Align!=PrintItemAlign.LeftRight)
                aresult.X = Width;
            if (Align != PrintItemAlign.TopBottom)
                aresult.Y = Height;
            LastExtent = aresult;
            return aresult;
        }

    }

    /// <summary>
    /// Collections of PrintItems
    /// </summary>
#if REPMAN_DOTNET1
	public class PrintItems
	{
		PrintItem[] FItems;
		const int FIRST_ALLOCATION_OBJECTS = 50;
		int FCount;
		public PrintItems()
		{
			FCount = 0;
			FItems = new PrintItem[FIRST_ALLOCATION_OBJECTS];
		}
		public void Clear()
		{
			for (int i = 0; i < FCount; i++)
				FItems[i] = null;
			FCount = 0;
		}
		private void CheckRange(int index)
		{
			if ((index < 0) || (index >= FCount))
				throw new UnNamedException("Index out of range on PrintItems collection");
		}
		public PrintItem this[int index]
		{
			get { CheckRange(index); return FItems[index]; }
			set { CheckRange(index); FItems[index] = value; }
		}
		public int Count { get { return FCount; } }
		public void Add(PrintItem obj)
		{
			if (FCount > (FItems.Length - 2))
			{
				PrintItem[] nobjects = new PrintItem[FCount];
				System.Array.Copy(FItems, 0, nobjects, 0, FCount);
				FItems = new PrintItem[FItems.Length * 2];
				System.Array.Copy(nobjects, 0, FItems, 0, FCount);
			}
			FItems[FCount] = obj;
			FCount++;
		}
	}
#else
	public class PrintItems:System.Collections.Generic.List<PrintItem>
	{

	}
#endif
    /// <summary>
    /// Collections of PrintPosItems
    /// </summary>
#if REPMAN_DOTNET1
	public class PrintPosItems
	{
		PrintPosItem[] FItems;
		const int FIRST_ALLOCATION_OBJECTS = 50;
		int FCount;
		public PrintPosItems()
		{
			FCount = 0;
			FItems = new PrintPosItem[FIRST_ALLOCATION_OBJECTS];
		}
		public void Clear()
		{
			for (int i = 0; i < FCount; i++)
				FItems[i] = null;
			FCount = 0;
		}
		private void CheckRange(int index)
		{
			if ((index < 0) || (index >= FCount))
				throw new UnNamedException("Index out of range on PrintItems collection");
		}
		public PrintPosItem this[int index]
		{
			get { CheckRange(index); return FItems[index]; }
			set { CheckRange(index); FItems[index] = value; }
		}
		public int Count { get { return FCount; } }
		public void Add(PrintPosItem obj)
		{
			if (FCount > (FItems.Length - 2))
			{
				PrintPosItem[] nobjects = new PrintPosItem[FCount];
				System.Array.Copy(FItems, 0, nobjects, 0, FCount);
				FItems = new PrintPosItem[FItems.Length * 2];
				System.Array.Copy(nobjects, 0, FItems, 0, FCount);
			}
			FItems[FCount] = obj;
			FCount++;
		}
		// IEnumerable Interface Implementation:
		//   Declaration of the GetEnumerator() method 
		//   required by IEnumerable
		public IEnumerator GetEnumerator()
		{
			return new PrintItemPosEnumerator(this);
		}
		// Inner class implements IEnumerator interface:
		public class PrintItemPosEnumerator : IEnumerator
		{
			private int position = -1;
			private PrintPosItems t;

			public PrintItemPosEnumerator(PrintPosItems t)
			{
				this.t = t;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < t.Count - 1)
				{
					position++;
					return true;
				}
				else
				{
					return false;
				}
			}

			// Declare the Reset method required by IEnumerator:
			public void Reset()
			{
				position = -1;
			}

			// Declare the Current property required by IEnumerator:
			public object Current
			{
				get
				{
					return t[position];
				}
			}
		}
	}
#else
	public class PrintPosItems:System.Collections.Generic.List<PrintPosItem>
	{

	}
#endif
    /// <summary>
    /// Base class for any report item containing text properties, ExpressionItem and LabelItem are examples
    /// </summary>
	public class PrintItemText : PrintPosItem
	{
        /// <summary>Font family name for Microsoft Windows operating system</summary>
        public string WFontName;
        /// <summary>Font family name for Linux operating system</summary>
        public string LFontName;
        /// <summary>Font size in standard points unit, that is 72 points=1 inchess</summary>
		public short FontSize;
        /// <summary>Rotation angle in degrees for the text. Rotation in reverse clock wise, internally stored as 
        /// an integer in degrees*10 scale.</summary>
        public short FontRotation;
        /// <summary>Combination of a set of possible effects for the font: bold, italic, underline and strikeout, 
        /// all the styles are compatible. Internally stored as an Integer</summary>
        public int FontStyle;
        /// <summary>Font color, internally stored as a quad byte 0x0BGR integer</summary>
        public int FontColor;
        /// <summary>Background color for the printed text, if Transparent property is set to false</summary>
        public int BackColor;
        /// <summary>If false a background color (Back Color) will be used for the text</summary>
        public bool Transparent;
        /// <summary>If true, the text will be clipped to the defined box, else the text will expand outside the box, 
        /// if word wrap is true, the widh of the box will be preserved.</summary>
        public bool CutText;
        /// <summary>Will break the sentences in more lines if the lines does not fit in current box width</summary>
        public bool WordWrap;
        /// <summary>Still not implemented, for future use, to break words when they not fit on single line</summary>
        public bool WordBreak;
        /// <summary>Still not implemented, for future use, when multiple lines are drawn represents the space 
        /// between lines</summary>
        public int InterLine;
        /// <summary>Horizontal alignment for the text inside the box defined by the component. The text can be 
        /// aligned Left (also None), Right and Center. The right toleft property can alter the meaning of 
        /// this property</summary>
        public TextAlignType Alignment;
        /// <summary>Vertical alignment for the text inside the box defined by the component. The text can be 
        /// aligned top (also None), bottom and Center..</summary>
        public TextAlignVerticalType VAlignment;
        /// <summary>Will force printing of text in a single line even if it contains line feeds. </summary>
        public bool SingleLine;
        /// <summary>Font used when exporting to Adobe PDF file format</summary>
		public PDFFontType Type1Font;
        /// <summary>If true, then the print value (usually a string) will print in diferent pages if needed, 
        /// usually you set autoexpand and autocontract to true in the parent section and word 
        /// wrap property to true</summary>
        public bool MultiPage;
        /// <summary>Font used when printing to Pos devices</summary>
        public PrintStepType PrintStep;
        /// <summary>
        /// RightToLeft for arabic texts
        /// </summary>
        public bool RightToLeft;
        /// <summary>
        /// Constructor
        /// </summary>
		public PrintItemText(BaseReport rp)
			: base(rp)
		{
			Transparent = true;
			FontSize = 10;
			BackColor = 0xFFFFFF;
			WFontName = "Arial";
			LFontName = "Helvetica";
		}
        /// <summary>
        /// Returns the horizontal alignment converted to an integer value
        /// </summary>
		public int PrintAlignment
		{
			get
			{
				// Inverse the alignment for BidiMode Full
				int aresult = 0;
				if (Alignment == TextAlignType.Right)
					aresult = MetaFile.AlignmentFlags_AlignRight;
				else
					if (Alignment == TextAlignType.Center)
						aresult = MetaFile.AlignmentFlags_AlignHCenter;
					else
						if (Alignment == TextAlignType.Justify)
							aresult = MetaFile.AlignmentFlags_AlignHJustify;
				return aresult;
			}
		}
        /// <summary>
        /// Returns the vertical alignment converted to an integer value
        /// </summary>
        public int VPrintAlignment
		{
			get
			{
				// Inverse the alignment for BidiMode Full
				int aresult = 0;
				if (VAlignment == TextAlignVerticalType.Center)
					aresult = MetaFile.AlignmentFlags_AlignVCenter;
				else
					if (VAlignment == TextAlignVerticalType.Bottom)
						aresult = MetaFile.AlignmentFlags_AlignBottom;
				return aresult;
			}
		}
	}
    class ReportItemTypeConverter: ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection props = base.GetProperties(context, value, attributes);
            List<PropertyDescriptor> list = new List<PropertyDescriptor>(props.Count);
            foreach (PropertyDescriptor prop in props)
            {
                switch (prop.Name)
                {
                    case "Width":                        
                        list.Add(new DisplayNamePropertyDescriptor(
                            prop, Translator.TranslateStr(554), "Twips", Translator.TranslateStr(280)));
                        break;
                    case "Height":
                        list.Add(new DisplayNamePropertyDescriptor(
                            prop, Translator.TranslateStr(555), "Twips", Translator.TranslateStr(280)));
                        break;
                    default:
                        list.Add(prop);
                        break;
                }
            }
            return new PropertyDescriptorCollection(list.ToArray(), true);
        }
    }
    class DisplayNamePropertyDescriptor : PropertyDescriptor
    {
        private readonly string displayName;
        private readonly PropertyDescriptor parent;
        private readonly string categoryName;
        private readonly string sType;
        public DisplayNamePropertyDescriptor(
            PropertyDescriptor parent, string displayName,string sType,string categoryName) : base(parent)
        {
            this.displayName = displayName;
            this.parent = parent;
            this.categoryName = categoryName;
            this.sType = sType;
        }
        public override string DisplayName
        { get { return displayName; } }
        public override string Category
        {
            get { return categoryName;  }
        }
        public override bool ShouldSerializeValue(object component)
        { return parent.ShouldSerializeValue(component); }

        public override void SetValue(object component, object value)
        {
            switch (sType)
            {
                case "Twips":
                    int twipsValue = Twips.TwipsFromUnits(Variant.VariantFromObject(Convert.ToDecimal(value)));
                    parent.SetValue(component, twipsValue);
                    break;
                default:
                    parent.SetValue(component, value);
                    break;
            }
            switch (parent.Name)
            {
                case "Width":
                    break;
                case "Height":
                    break;
            }

        }
        public override object GetValue(object component)
        {
            switch (sType)
            {
                case "Twips":
                    int twipsValue = Convert.ToInt32(parent.GetValue(component));
                    decimal unitValue = Convert.ToDecimal(Twips.UnitsFromTwips(Convert.ToInt32(twipsValue)));
                    unitValue = Math.Round(unitValue, 6);
                    return unitValue;
                default:
                    return parent.GetValue(component);
            }
        }
        public override void ResetValue(object component)
        {
            parent.ResetValue(component);
        }
        public override bool CanResetValue(object component)
        {
            return parent.CanResetValue(component);
        }
        public override bool IsReadOnly
        {
            get { return parent.IsReadOnly; }
        }
        public override void AddValueChanged(object component, EventHandler handler)
        {
            parent.AddValueChanged(component, handler);
        }
        public override void RemoveValueChanged(object component, EventHandler handler)
        {
            parent.RemoveValueChanged(component, handler);
        }
        public override bool SupportsChangeEvents
        {
            get { return parent.SupportsChangeEvents; }
        }
        public override Type PropertyType
        {
            get { return parent.PropertyType; }
        }
        DecimalConverter decimalConverter = new DecimalConverter();
        public override TypeConverter Converter
        {
            get
            {
                switch (sType)
                {
                    case "Twips":
                        //Twips.TwipsFromUnits(Variant.VariantFromObject(args.Row["VALUE"])
                        return this.decimalConverter;
                    default:
                        return parent.Converter;
                }

            }
        }
        public override Type ComponentType
        {
            get {
                switch (sType)
                {
                    case "Twips":
                        //Twips.TwipsFromUnits(Variant.VariantFromObject(args.Row["VALUE"])
                        return Type.GetType("System.Decimal");
                    default:
                        return parent.ComponentType;
                }

            }
        }
        public override string Description
        {
            get { return parent.Description; }
        }
        public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
        {
            return parent.GetChildProperties(instance, filter);
        }
        public override string Name
        {
            get { return parent.Name; }
        }

    }

}
