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
using System.Text;
using System.Threading;
using System.Drawing;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace Reportman.Drawing
{
	/// <summary>
	/// Line information when measuring pdf texts
	/// </summary>
	public struct LineInfo
	{
		public int Position;
		public int Size;
		public int Width;
		public int Height;
		public int TopPos;
		public PrintStepType Step;
		public bool LastLine;
	}
	struct PageInfo
	{
		public int PageWidth;
		public int PageHeight;
	}
	public class LineInfos
	{
		LineInfo[] FObjects;
		const int FIRST_ALLOCATION_OBJECTS = 50;
		int FCount;
		public LineInfos()
		{
			FCount = 0;
			FObjects = new LineInfo[FIRST_ALLOCATION_OBJECTS];
		}
		public void Clear()
		{
			FCount = 0;
		}
		private void CheckRange(int index)
		{
			if ((index < 0) || (index >= FCount))
				throw new Exception("Index out of range on LineInfos collection");
		}
		public LineInfo this[int index]
		{
			get { CheckRange(index); return FObjects[index]; }
			set { CheckRange(index); FObjects[index] = value; }
		}
		public int Count { get { return FCount; } }
		public void Add(LineInfo obj)
		{
			if (FCount > (FObjects.Length - 2))
			{
				LineInfo[] nobjects = new LineInfo[FCount];
				System.Array.Copy(FObjects, 0, nobjects, 0, FCount);
				FObjects = new LineInfo[FObjects.Length * 2];
				System.Array.Copy(nobjects, 0, FObjects, 0, FCount);
			}
			FObjects[FCount] = obj;
			FCount++;
		}
	}
	class PageInfos
	{
		PageInfo[] FObjects;
		const int FIRST_ALLOCATION_OBJECTS = 50;
		int FCount;
		public PageInfos()
		{
			FCount = 0;
			FObjects = new PageInfo[FIRST_ALLOCATION_OBJECTS];
		}
		public void Clear()
		{
			FCount = 0;
		}
		private void CheckRange(int index)
		{
			if ((index < 0) || (index >= FCount))
				throw new Exception("Index out of range on PageInfos collection");
		}
		public PageInfo this[int index]
		{
			get { CheckRange(index); return FObjects[index]; }
			set { CheckRange(index); FObjects[index] = value; }
		}
		public int Count { get { return FCount; } }
		public void Add(PageInfo obj)
		{
			if (FCount > (FObjects.Length - 2))
			{
				PageInfo[] nobjects = new PageInfo[FCount];
				System.Array.Copy(FObjects, 0, nobjects, 0, FCount);
				FObjects = new PageInfo[FObjects.Length * 2];
				System.Array.Copy(nobjects, 0, FObjects, 0, FCount);
			}
			FObjects[FCount] = obj;
			FCount++;
		}
	}
	public class MemStreams : IDisposable
	{
		const long MAX_MEM_SIZE = 100000000;
		MemoryStream[] FItems;
		long TotalSize = 0;
		SortedList<int, string> FTempFiles = new SortedList<int, string>();
		SortedList<int, long> FFileSizes = new SortedList<int, long>();
		const int FIRST_ALLOCATION_OBJECTS = 50;
		int FCount;
		public MemStreams()
		{
			FCount = 0;
			FItems = new MemoryStream[FIRST_ALLOCATION_OBJECTS];
		}
		public void Clear()
		{
			for (int i = 0; i < FCount; i++)
			{
				RemoveTempFile(i);
				FItems[i] = null;
			}
			FCount = 0;
			TotalSize = 0;
		}
		private void CheckRange(int index)
		{
			if ((index < 0) || (index >= FCount))
				throw new Exception("Index out of range on PrintItems collection");
		}
		private void RemoveTempFile(int idx)
		{
			if (FTempFiles.IndexOfKey(idx) >= 0)
			{
				if (File.Exists(FTempFiles[idx]))
					File.Delete(FTempFiles[idx]);
				FTempFiles.Remove(idx);
			}
		}
		public MemoryStream this[int index]
		{
			get
			{
				CheckRange(index);
				MemoryStream mstream = FItems[index];
				if (FTempFiles.IndexOfKey(index) >= 0)
				{
					mstream = Reportman.Drawing.StreamUtil.FileToMemoryStream(FTempFiles[index]);
				}
				return mstream;
			}
			set
			{
				CheckRange(index);
				FItems[index] = value;
				RemoveTempFile(index);
				TotalSize = TotalSize - FFileSizes[index];
				TotalSize = TotalSize - value.Length;
			}
		}
		public int Count { get { return FCount; } }
		public void Add(MemoryStream obj)
		{
			TotalSize = TotalSize + obj.Length;
			if (FCount > (FItems.Length - 2))
			{
				MemoryStream[] nobjects = new MemoryStream[FCount];
				System.Array.Copy(FItems, 0, nobjects, 0, FCount);
				FItems = new MemoryStream[FItems.Length * 2];
				System.Array.Copy(nobjects, 0, FItems, 0, FCount);
			}
			FItems[FCount] = obj;
			FFileSizes.Add(FCount, obj.Length);
			TotalSize = TotalSize + obj.Length;
			if (TotalSize > MAX_MEM_SIZE)
			{
				string tmpfilename = System.IO.Path.GetTempFileName();
				Reportman.Drawing.StreamUtil.MemoryStreamToFile(obj, tmpfilename);
				FItems[Count] = null;
				FTempFiles.Add(FCount, tmpfilename);
			}
			FCount++;
		}
		public void Dispose()
		{
			while (FTempFiles.Count > 0)
				RemoveTempFile(FTempFiles.Keys[0]);
		}
	}
	public class PDFCanvas
	{
		public static string ENDSTREAM = "" + (char)13 + (char)10 + "endstream";
		private FontInfoProvider FInfoProvider;
		FontInfoProvider FFDefInfoProvider;
		public FontInfoProvider FDefInfoProvider
		{
			get
			{
				if (FFDefInfoProvider == null)
				{
#if NETSTANDARD2_0
					FFDefInfoProvider = new FontInfoFt();
#else
					if ((System.Environment.OSVersion.Platform == PlatformID.Unix) || (System.Environment.OSVersion.Platform == PlatformID.MacOSX))
						FFDefInfoProvider = new FontInfoFt();
					else
						FFDefInfoProvider = new FontInfoGDI();
#endif
				}
				return FFDefInfoProvider;
			}
		}
		private PDFFont FFont;
		public PDFFile File;
		private int FResolution;
		private LineInfos Lines;
		private SortedList FFontData;
		public int Resolution;
		public LineInfos LineInfo
		{
			get
			{
				return Lines;
			}
		}
		public PDFFont Font { get { return FFont; } }
		public PDFCanvas()
		{
			OldPenColor = -1;
			OldBrushColor = -1;
			Resolution = Twips.TWIPS_PER_INCH;
			FFontData = new SortedList();
			FFont = new PDFFont();
			FResolution = Twips.TWIPS_PER_INCH;
			Lines = new LineInfos();

		}
		public int PenColor;
		public int PenStyle;
		public int PenWidth;
		public int BrushColor;
		public int BrushStyle;

		public int OldBrushColor;
		public int OldPenColor;
		public int SavedPenColor;
		public int SavedBrushColor;

		public SortedList FontData
		{
			get { return FFontData; }
		}
		public FontInfoProvider InfoProvider
		{
			get
			{
				if (FInfoProvider != null)
					return FInfoProvider;
				else
					return FDefInfoProvider;
			}
			set
			{
				if (value == null)
					FInfoProvider = FDefInfoProvider;
				else
					FInfoProvider = value;
			}
		}
		private bool translatedy;
		public string UnitsToTextX(int Value)
		{
			double nvalue = ((double)(Value) / FResolution) * PDFFile.CONS_PDFRES;
			string aresult = nvalue.ToString("##0.00");
#if REPMAN_COMPACT
			string decseparator=CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
#else
			string decseparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
#endif
			return aresult.Replace(decseparator, ".");
		}
		string UnitsToTextY(int Value)
		{
			double nvalue;
			if (translatedy)
				nvalue = ((double)(-Value) / FResolution) * PDFFile.CONS_PDFRES;
			else
				nvalue = ((double)(File.PageHeight - Value) / FResolution) * PDFFile.CONS_PDFRES;
			string aresult = nvalue.ToString("##0.00");
#if REPMAN_COMPACT
			string decseparator = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
#else
			string decseparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
#endif
			return aresult.Replace(decseparator, ".");
		}
		string UnitsToTextText(int Value, int FSize)
		{
			double nvalue;
			if (translatedy)
				nvalue = (((double)(-Value) / FResolution) * PDFFile.CONS_PDFRES) - FSize;
			else
				nvalue = (((double)(File.PageHeight - Value) / FResolution) * PDFFile.CONS_PDFRES) - FSize;
			string aresult = nvalue.ToString("##0.00");
#if REPMAN_COMPACT
			string decseparator=CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
#else
			string decseparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
#endif
			return aresult.Replace(decseparator, ".");
		}
		public static string NumberToText(double Value)
		{
			string aresult = Value.ToString("##0.00");
#if REPMAN_COMPACT
			string decseparator=CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
#else
			string decseparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
#endif
			return aresult.Replace(decseparator, ".");
		}
		string RGBToFloats(int acolor)
		{
#if REPMAN_COMPACT
			string decseparator=CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
#else
			string decseparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
#endif
			string aresult;
			byte r, g, b;
			r = (byte)acolor;
			aresult = ((double)r / 256).ToString("0.00");
			g = (byte)(acolor >> 8);
			aresult = aresult + " " + ((double)g / 256).ToString("0.00");
			b = (byte)(acolor >> 16);
			aresult = aresult + " " + ((double)b / 256).ToString("0.00");
			return aresult.Replace(decseparator, ".");
		}
		private void SetDash()
		{
			switch (PenStyle)
			{
				// Dash
				case 1:
					StreamUtil.SWriteLine(File.STempStream, "[16 8] 0 d");
					break;
				// Dot
				case 2:
					StreamUtil.SWriteLine(File.STempStream, "[1] 0 d");
					break;
				// Dash Dot
				case 3:
					StreamUtil.SWriteLine(File.STempStream, "[8 7 2 7] 0 d");
					break;
				// Dash Dot Dot
				case 4:
					StreamUtil.SWriteLine(File.STempStream, "[8 4 2 4 2 4] 0 d");
					break;
				// Clear
				case 5:
					break;
				default:
					StreamUtil.SWriteLine(File.STempStream, "[] 0 d");
					break;
			}
		}
		public void Line(int x1, int y1, int x2, int y2)
		{
			if (PenStyle == 5)
				return;
			SetDash();
			int LineWidth = 1;
			if (PenWidth > 0)
				LineWidth = PenWidth;
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(LineWidth) + " w");

			WritePenColor(PenColor);
			WriteBrushColor(PenColor);
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x1) + ' ' + UnitsToTextY(y1) + " m");
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x2) + ' ' + UnitsToTextY(y2) + " l");
			// S-Solid,  D-Dashed, B-Beveled, I-Inset, U-Underline
			StreamUtil.SWriteLine(File.STempStream, "S");

		}
		public void WritePenColor(int NewColor)
		{
			bool dowrite = true;
			if (File.Optimized)
			{
				dowrite = (OldPenColor != NewColor);
			}
			if (dowrite)
			{
				StreamUtil.SWriteLine(File.STempStream, RGBToFloats(NewColor) + " RG");
				OldPenColor = NewColor;
			}
		}
		public void WriteBrushColor(int NewColor)
		{
			bool dowrite = true;
			if (File.Optimized)
			{
				dowrite = OldBrushColor != NewColor;
			}
			if (dowrite)
			{
				StreamUtil.SWriteLine(File.STempStream, RGBToFloats(NewColor) + " rg");
				OldBrushColor = NewColor;
			}
		}
		public TTFontData GetTTFontData()
		{
			if (!((FFont.Name == PDFFontType.Linked) || (FFont.Name == PDFFontType.Embedded)))
				return null;
			if (InfoProvider == null)
				return null;
			return UpdateFonts();
		}
		public TTFontData UpdateFonts()
		{
			string searchname;
			TTFontData adata;

			if (!((FFont.Name == PDFFontType.Linked) || (FFont.Name == PDFFontType.Embedded)))
				return null;
			if (InfoProvider == null)
				return null;
			searchname = FFont.FontName + FFont.Style.ToString("00000");
			adata = FFontData[searchname] as TTFontData;
			if (adata == null)
			{
				adata = new TTFontData();
				adata.Embedded = false;
				adata.ObjectName = FFont.FontName + FFont.Style.ToString();
				FFontData.Add(searchname, adata);
				adata.Embedded = (FFont.Name == PDFFontType.Embedded);
				InfoProvider.FillFontData(FFont, adata);
			}
			return adata;
		}
		public void Ellipse(int X1, int Y1, int X2, int Y2)
		{
			int LineWidth;
			double W, H;
			string opfill;
			if ((PenStyle == 5) && (BrushStyle == 2))
				return;
			SetDash();
			W = X2 - X1;
			H = Y2 - Y1;
			LineWidth = 1;
			if (PenWidth > 0)
				LineWidth = PenWidth;
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(LineWidth) + " w");
			WritePenColor(PenColor);
			WriteBrushColor(BrushColor);

			// Draws a ellipse in 4 pass
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(X1) + " " +
				UnitsToTextY(Y1 + ((int)(H / 2))) + " m");
			StreamUtil.SWriteLine(File.STempStream,
				UnitsToTextX(X1) + " " + UnitsToTextY(Y1 + ((int)(H / 2)) - (int)Math.Round((double)H / 2 * 11 / 20)) + " " +
				UnitsToTextX(X1 + ((int)(W / 2)) - (int)Math.Round((double)W / 2 * 11 / 20)) + " " + UnitsToTextY(Y1) + " " +
				UnitsToTextX(X1 + ((int)(W / 2))) + " " + UnitsToTextY(Y1) + " c");
			StreamUtil.SWriteLine(File.STempStream,
				UnitsToTextX(X1 + ((int)(W / 2)) + (int)Math.Round((double)W / 2 * 11 / 20)) + ' ' + UnitsToTextY(Y1) + " " +
				UnitsToTextX(X1 + (int)W) + " " + UnitsToTextY(Y1 + ((int)(H / 2)) - (int)Math.Round((double)H / 2 * 11 / 20)) + " " +
				UnitsToTextX(X1 + (int)W) + " " + UnitsToTextY(Y1 + ((int)(H / 2))) + " c");
			StreamUtil.SWriteLine(File.STempStream,
				UnitsToTextX(X1 + (int)W) + " " + UnitsToTextY(Y1 + ((int)(H / 2)) + (int)Math.Round((double)H / 2 * 11 / 20)) + " " +
				UnitsToTextX(X1 + ((int)(W / 2)) + (int)Math.Round((double)W / 2 * 11 / 20)) + " " + UnitsToTextY(Y1 + (int)H) + " " +
				UnitsToTextX(X1 + ((int)(W / 2))) + " " + UnitsToTextY(Y1 + (int)H) + " c");
			StreamUtil.SWriteLine(File.STempStream,
				UnitsToTextX(X1 + ((int)(W / 2)) - (int)Math.Round((double)W / 2 * 11 / 20)) + " " + UnitsToTextY(Y1 + (int)H) + " " +
				UnitsToTextX(X1) + " " + UnitsToTextY(Y1 + ((int)(H / 2)) + (int)Math.Round((double)H / 2 * 11 / 20)) + " " +
				UnitsToTextX(X1) + " " + UnitsToTextY(Y1 + ((int)(H / 2))) + " c");

			opfill = "B";
			if (PenStyle == 5)
				opfill = "f";
			// Bsclear
			if (BrushStyle == 1)
				StreamUtil.SWriteLine(File.STempStream, "S");
			else
				// BsSolid
				StreamUtil.SWriteLine(File.STempStream, opfill);
		}
		public void RoundedRectangle(int x1, int y1, int x2, int y2, int radius)
		{
			int LineWidth;
			string opfill;
			if ((PenStyle == 5) && (BrushStyle == 2))
				return;
			SetDash();
			LineWidth = 1;
			if (PenWidth > 0)
				LineWidth = PenWidth;
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(LineWidth) + " w");

			WritePenColor(PenColor);
			WriteBrushColor(BrushColor);

			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x1) + " " + UnitsToTextY(y2 - radius) + " m");
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x1) + " " + UnitsToTextY(y1 + radius) + " l");
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x1) + " " + UnitsToTextY(y1) +
				" " + UnitsToTextX(x1 + radius) + " " + UnitsToTextY(y1) + " v");



			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x2 - radius) + " " + UnitsToTextY(y1) + " l");
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x2) + " " + UnitsToTextY(y1) +
				" " + UnitsToTextX(x2) + " " + UnitsToTextY(y1 + radius) + " v");


			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x2) + " " + UnitsToTextY(y2 - radius) + " l");
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x2) + " " + UnitsToTextY(y2) +
				" " + UnitsToTextX(x2 - radius) + " " + UnitsToTextY(y2) + " v");

			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x1 + radius) + " " + UnitsToTextY(y2) + " l");
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x1) + " " + UnitsToTextY(y2) +
				" " + UnitsToTextX(x1) + " " + UnitsToTextY(y2 - radius) + " v");


			opfill = "B";
			if (PenStyle == 5)
				opfill = "f";
			// Bsclear
			if (BrushStyle == 1)
				StreamUtil.SWriteLine(File.STempStream, "S");
			else
				// BsSolid
				StreamUtil.SWriteLine(File.STempStream, opfill);
		}
		public void Rectangle(int x1, int y1, int x2, int y2)
		{
			int LineWidth;
			string opfill;
			if ((PenStyle == 5) && (BrushStyle == 2))
				return;
			SetDash();
			LineWidth = 1;
			if (PenWidth > 0)
				LineWidth = PenWidth;
			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(LineWidth) + " w");

			WritePenColor(PenColor);
			WriteBrushColor(BrushColor);

			StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(x1) + " " + UnitsToTextY(y1) +
				" " + UnitsToTextX(x2 - x1) + " " + UnitsToTextX(-(y2 - y1)) + " re");
			opfill = "B";
			if (PenStyle == 5)
				opfill = "f";
			// Bsclear
			if (BrushStyle == 1)
				StreamUtil.SWriteLine(File.STempStream, "S");
			else
				// BsSolid
				StreamUtil.SWriteLine(File.STempStream, opfill);
		}
		public void SaveGraph()
		{
			StreamUtil.SWriteLine(File.STempStream, "q");
			SavedPenColor = OldPenColor;
			SavedBrushColor = OldBrushColor;
		}
		public void RestoreGraph()
		{
			StreamUtil.SWriteLine(File.STempStream, "Q");
			translatedy = false;
			OldPenColor = SavedPenColor;
			OldBrushColor = SavedBrushColor;
		}
		public static string DoReverseString(string astring)
		{
			string aresult = "";
			for (int i = 0; i < astring.Length; i++)
				aresult = astring[i] + aresult;
			return aresult;
		}
		public static string Type1FontTopdfFontName(PDFFontType Type1Font, bool oblique,
			bool bold, string WFontName, int FontStyle)
		{
			int avalue;
			string searchname;
			string aresult;
			if ((Type1Font == PDFFontType.Linked) ||
					(Type1Font == PDFFontType.Embedded))
			{
				searchname = WFontName + FontStyle.ToString();
				aresult = searchname;
			}
			else
			{
				avalue = 0;
				switch (Type1Font)
				{
					case PDFFontType.Helvetica:
						avalue = 0;
						break;
					case PDFFontType.Courier:
						avalue = 4;
						break;
					case PDFFontType.TimesRoman:
						avalue = 8;
						break;
					case PDFFontType.Symbol:
						avalue = 12;
						break;
					case PDFFontType.ZafDingbats:
						avalue = 13;
						break;
				}
				if ((Type1Font == PDFFontType.Helvetica) ||
					(Type1Font == PDFFontType.Courier) ||
					(Type1Font == PDFFontType.TimesRoman))
				{
					if (bold)
						avalue = avalue + 1;
					if (oblique)
						avalue = avalue + 2;
				}
				aresult = (avalue + 1).ToString();
			}
			return aresult;
		}

		public void TextOut(int X, int Y, string Text, int LineWidth,
		 int Rotation, bool RightToLeft)
		{
			double rotrad, fsize;
			string rotstring;
			int PosLine, PosLineX1, PosLineY1, PosLineX2, PosLineY2;
			string astring;
			TTFontData adata;
			//bool havekerning;
			int leading, linespacing;

			//havekerning = false;
			adata = GetTTFontData();
			if (adata != null)
			{
				//if (adata.HaveKerning)
				//    havekerning = true;
				//linespacing = adata.Ascent - adata.Descent + adata.Leading;
				leading = adata.Leading;
			}
			else
			{
				GetStdLineSpacing(out linespacing, out leading);
			}
			leading = (int)Math.Round((((double)leading) / 100000.0) * FResolution * FFont.Size * 1.25);
			// linespacing = (int)Math.Round((((double)linespacing) / 100000.0) * FResolution * FFont.Size * 1.25);
			Y = Y + leading;

			File.CheckPrinting();
			if (Rotation != 0)
			{
				SaveGraph();
			}
			try
			{
				WritePenColor(FFont.Color);
				WriteBrushColor(FFont.Color);

				StreamUtil.SWriteLine(File.STempStream, "BT");
				StreamUtil.SWriteLine(File.STempStream, "/F" +
				 Type1FontTopdfFontName(FFont.Name, FFont.Italic, FFont.Bold, FFont.FontName, FFont.Style) + " " +
					FFont.Size.ToString() + " Tf");
				// Rotates
				if (Rotation != 0)
				{
					rotstring = "1 0 0 1 " +
						UnitsToTextX(X) + " " +
						UnitsToTextText(Y, FFont.Size);
					StreamUtil.SWriteLine(File.STempStream, rotstring + " cm");
					rotrad = (double)Rotation / 10 * (2 * Math.PI / 360);
					rotstring = NumberToText(Math.Cos(rotrad)) + " " +
						NumberToText(Math.Sin(rotrad)) + " " +
						NumberToText(-Math.Sin(rotrad)) + " " +
						NumberToText(Math.Cos(rotrad)) + " 0 0";
					StreamUtil.SWriteLine(File.STempStream, rotstring + " cm");
				}
				else
					StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(X) + " " + UnitsToTextText(Y, FFont.Size) + " Td");
				astring = Text;
				if (RightToLeft)
				{
					astring = DoReverseString(astring);
				}
				// Kerning disable for GDI+ compatibility
				//
				//if (havekerning)
				//	StreamUtil.SWriteLine(File.STempStream, PDFCompatibleTextWithKerning(astring, adata, FFont) + " TJ");
				// else
				StreamUtil.SWriteLine(File.STempStream, PDFCompatibleText(astring, adata, FFont) + " Tj");
				StreamUtil.SWriteLine(File.STempStream, "ET");
			}
			finally
			{
				if (Rotation != 0)
				{
					RestoreGraph();
				}
			}
			// Underline and strikeout
			if (FFont.Underline)
			{
				PenStyle = 0;
				PenWidth = (int)Math.Round(((double)FFont.Size / PDFFile.CONS_PDFRES * FResolution) * PDFFile.CONS_UNDERLINEWIDTH);
				PenColor = FFont.Color;
				if (Rotation == 0)
				{
					PosLine = (int)Math.Round(PDFFile.CONS_UNDERLINEPOS * ((double)FFont.Size / PDFFile.CONS_PDFRES * FResolution));
					Line(X, Y + PosLine, X + LineWidth, Y + PosLine);
				}
				else
				{
					Y = Y + (int)Math.Round(PDFFile.CONS_UNDERLINEPOS * ((double)FFont.Size / PDFFile.CONS_PDFRES * FResolution));
					rotrad = (double)Rotation / 10 * (2 * Math.PI / 360);
					fsize = (double)PDFFile.CONS_UNDERLINEPOS * FFont.Size / PDFFile.CONS_PDFRES * FResolution - (double)FFont.Size / PDFFile.CONS_PDFRES * FResolution;
					PosLineX1 = (int)-Math.Round(fsize * Math.Cos(rotrad));
					PosLineY1 = (int)-Math.Round(fsize * Math.Sin(rotrad));
					PosLineX2 = (int)Math.Round(LineWidth * Math.Cos(rotrad));
					PosLineY2 = (int)-Math.Round(LineWidth * Math.Sin(rotrad));
					Line(X + PosLineX1, Y + PosLineY1, X + PosLineX2, Y + PosLineY2);
					Y = Y - (int)Math.Round(PDFFile.CONS_UNDERLINEPOS * ((double)FFont.Size / PDFFile.CONS_PDFRES * FResolution));
				}
			}
			if (FFont.StrikeOut)
			{
				PenStyle = 0;
				PenWidth = (int)Math.Round(((double)FFont.Size / PDFFile.CONS_PDFRES * FResolution) * PDFFile.CONS_UNDERLINEWIDTH);
				PenColor = FFont.Color;
				if (Rotation == 0)
				{
					PosLine = (int)Math.Round(PDFFile.CONS_STRIKEOUTPOS * ((double)FFont.Size / PDFFile.CONS_PDFRES * FResolution));
					Line(X, Y + PosLine, X + LineWidth, Y + PosLine);
				}
				else
				{
					Y = Y + (int)Math.Round(PDFFile.CONS_UNDERLINEPOS * ((double)FFont.Size / PDFFile.CONS_PDFRES * FResolution));
					rotrad = (double)Rotation / 10 * (2 * Math.PI / 360);
					fsize = PDFFile.CONS_UNDERLINEPOS * (double)FFont.Size / PDFFile.CONS_PDFRES * FResolution - (double)FFont.Size / PDFFile.CONS_PDFRES * FResolution;
					PosLineX1 = -(int)Math.Round(fsize * Math.Cos(rotrad));
					PosLineY1 = (int)-Math.Round(fsize * Math.Sin(rotrad));
					PosLineX2 = (int)Math.Round(LineWidth * Math.Cos(rotrad));
					PosLineY2 = (int)-Math.Round(LineWidth * Math.Sin(rotrad));
					fsize = (1 - PDFFile.CONS_STRIKEOUTPOS) * (double)FFont.Size / PDFFile.CONS_PDFRES * FResolution;
					PosLineX1 = X + PosLineX1;
					PosLineY1 = Y + PosLineY1;
					PosLineX2 = X + PosLineX2;
					PosLineY2 = Y + PosLineY2;
					PosLineX1 = PosLineX1 - (int)Math.Round(fsize * Math.Sin(rotrad));
					PosLineY1 = PosLineY1 - (int)Math.Round(fsize * Math.Cos(rotrad));
					PosLineX2 = PosLineX2 - (int)Math.Round(fsize * Math.Sin(rotrad));
					PosLineY2 = PosLineY2 - (int)Math.Round(fsize * Math.Cos(rotrad));
					Line(PosLineX1, PosLineY1, PosLineX2, PosLineY2);
				}
			}
		}
		public static string WideCharToHex(char achar)
		{
			return IntToHex((int)achar); ;
		}
		public static string IntToHex(int nvalue)
		{
			StringBuilder nresult = new StringBuilder(nvalue.ToString("X"));
			while (nresult.Length < 4)
				nresult.Insert(0, "0");
			return nresult.ToString();
		}
		public string PDFCompatibleTextWithKerning(string astring, TTFontData adata, PDFFont pdffont)
		{
			int i;
			int kerningvalue;
			string aresult;

			if (astring.Length < 1)
				return "[]";
			if (adata.IsUnicode)
			{
				aresult = "[<";
				for (i = 0; i < astring.Length; i++)
				{
					aresult = aresult + IntToHex(adata.CacheWidths[astring[i]].GlyphIndex);
					if (i < (astring.Length - 1))
					{
						kerningvalue = InfoProvider.GetKerning(FFont, adata, astring[i], astring[i + 1]);
						if (kerningvalue != 0)
						{
							aresult = aresult + "> " + kerningvalue.ToString() + " <";
						}
					}
				}
				aresult = aresult + ">]";
			}
			else
			{
				aresult = "[(";
				for (i = 0; i < astring.Length; i++)
				{
					switch (astring[i])
					{
						case '(':
						case ')':
						case '\\':
							aresult = aresult + "\\" + astring[i];
							break;
						default:
							// Euro symbol exception
							if (astring[i] == (char)8364)
								aresult = aresult + (char)128;
							else
								aresult = aresult + astring[i];
							break;
					}
					if (i < (astring.Length - 1))
					{
						kerningvalue = InfoProvider.GetKerning(FFont, adata, astring[i], astring[i + 1]);
						if (kerningvalue != 0)
						{
							aresult = aresult + ") " + kerningvalue.ToString() + " (";
						}
					}
				}
				aresult = aresult + ")";
			}
			return aresult;
		}
		public static string PDFCompatibleText(string astring, TTFontData adata, PDFFont pdffont)
		{
			int i;
			bool isunicode = false;
			string aresult;

			if (adata != null)
				isunicode = adata.IsUnicode;
			if (isunicode)
			{
				aresult = "<";
				for (i = 0; i < astring.Trim().Length; i++)
					aresult = aresult + IntToHex(adata.CacheWidths[astring[i]].GlyphIndex);
				aresult = aresult + ">";
			}
			else
			{
				aresult = "(";
				for (i = 0; i < astring.Length; i++)
				{
					switch (astring[i])
					{
						case '(':
						case ')':
						case '\\':
							aresult = aresult + "\\" + astring[i];
							break;
						default:
							// Euro symbol exception
							if (astring[i] == (char)8364)
								aresult = aresult + (char)128;
							else
								aresult = aresult + astring[i];
							break;
					}
				}
				aresult = aresult + ")";
			}
			return aresult;
		}
		void GetStdLineSpacing(out int linespacing, out int leading)
		{
			switch (FFont.Name)
			{
				case PDFFontType.Helvetica:
					linespacing = 1270;
					leading = 150;
					break;
				case PDFFontType.Courier:
					linespacing = 1265;
					leading = 133;
					break;
				case PDFFontType.TimesRoman:
					linespacing = 1257;
					leading = 150;
					break;
				case PDFFontType.Symbol:
					linespacing = 1450;
					leading = 255;
					break;
				case PDFFontType.ZafDingbats:
					linespacing = 1200;
					leading = 150;
					break;
				default:
					linespacing = 1270;
					leading = 200;
					break;
			}
		}
		bool IsSeparator(char c)
		{
			return ((c == (char)10) || (c == (char)13) || (c == ' '));
		}
		bool IsSpecial(char c)
		{
			return ((c == (char)10) || (c == (char)13) || (c == (char)0));
		}
		bool IsSign(char c)
		{
			return ((c == ',') || (c == '.') || (c == '-') || (c == ' ') || (c == ':') || (c == ';'));
		}
		const int Default_Font_Width = 600;
		static int[] Helvetica_Widths = new int[] {
																									 278,278,355,556,556,889,667,191,333,333,389,584,278,333,
																									 278,278,556,556,556,556,556,556,556,556,556,556,278,278,584,584,
																									 584,556,1015,667,667,722,722,667,611,778,722,278,500,667,556,833,
																									 722,778,667,778,722,667,611,722,667,944,667,667,611,278,278,278,
																									 469,556,333,556,556,500,556,556,278,556,556,222,222,500,222,833,
																									 556,556,556,556,333,500,278,556,500,722,500,500,500,334,260,334,
																									 584,0,556,0,222,556,333,1000,556,556,333,1000,667,333,1000,0,
																									 611,0,0,222,222,333,333,350,556,1000,333,1000,500,333,944,0,
																									 500,667,0,333,556,556,556,556,260,556,333,737,370,556,584,0,
																									 737,333,400,584,333,333,333,556,537,278,333,333,365,556,834,834,
																									 834,611,667,667,667,667,667,667,1000,722,667,667,667,667,278,278,
																									 278,278,722,722,778,778,778,778,778,584,778,722,722,722,722,667,
																									 667,611,556,556,556,556,556,556,889,500,556,556,556,556,278,278,
																									 278,278,556,556,556,556,556,556,556,584,611,556,556,556,556,500,
																									 556,500};

		static int[] Helvetica_Bold_Widths = new int[] {
																												278,333,474,556,556,889,722,238,333,333,389,584,278,333,
																												278,278,556,556,556,556,556,556,556,556,556,556,333,333,584,584,
																												584,611,975,722,722,722,722,667,611,778,722,278,556,722,611,833,
																												722,778,667,778,722,667,611,722,667,944,667,667,611,333,278,333,
																												584,556,333,556,611,556,611,556,333,611,611,278,278,556,278,889,
																												611,611,611,611,389,556,333,611,556,778,556,556,500,389,280,389,
																												584,0,556,0,278,556,500,1000,556,556,333,1000,667,333,1000,0,
																												611,0,0,278,278,500,500,350,556,1000,333,1000,556,333,944,0,
																												500,667,0,333,556,556,556,556,280,556,333,737,370,556,584,0,
																												737,333,400,584,333,333,333,611,556,278,333,333,365,556,834,834,
																												834,611,722,722,722,722,722,722,1000,722,667,667,667,667,278,278,
																												278,278,722,722,778,778,778,778,778,584,778,722,722,722,722,667,
																												667,611,556,556,556,556,556,556,889,556,556,556,556,556,278,278,
																												278,278,611,611,611,611,611,611,611,584,611,611,611,611,611,556,
																												611,556};
		static int[] Helvetica_Italic_Widths = new int[] {
																													278,278,355,556,556,889,667,191,333,333,389,584,278,333,
																													278,278,556,556,556,556,556,556,556,556,556,556,278,278,584,584,
																													584,556,1015,667,667,722,722,667,611,778,722,278,500,667,556,833,
																													722,778,667,778,722,667,611,722,667,944,667,667,611,278,278,278,
																													469,556,333,556,556,500,556,556,278,556,556,222,222,500,222,833,
																													556,556,556,556,333,500,278,556,500,722,500,500,500,334,260,334,
																													584,0,556,0,222,556,333,1000,556,556,333,1000,667,333,1000,0,
																													611,0,0,222,222,333,333,350,556,1000,333,1000,500,333,944,0,
																													500,667,0,333,556,556,556,556,260,556,333,737,370,556,584,0,
																													737,333,400,584,333,333,333,556,537,278,333,333,365,556,834,834,
																													834,611,667,667,667,667,667,667,1000,722,667,667,667,667,278,278,
																													278,278,722,722,778,778,778,778,778,584,778,722,722,722,722,667,
																													667,611,556,556,556,556,556,556,889,500,556,556,556,556,278,278,
																													278,278,556,556,556,556,556,556,556,584,611,556,556,556,556,500,
																													556,500};

		static int[] Helvetica_BoldItalic_Widths = new int[] {
																															 278,333,474,556,556,889,722,238,333,333,389,584,278,333,
																															 278,278,556,556,556,556,556,556,556,556,556,556,333,333,584,584,
																															 584,611,975,722,722,722,722,667,611,778,722,278,556,722,611,833,
																															 722,778,667,778,722,667,611,722,667,944,667,667,611,333,278,333,
																															 584,556,333,556,611,556,611,556,333,611,611,278,278,556,278,889,
																															 611,611,611,611,389,556,333,611,556,778,556,556,500,389,280,389,
																															 584,0,556,0,278,556,500,1000,556,556,333,1000,667,333,1000,0,
																															 611,0,0,278,278,500,500,350,556,1000,333,1000,556,333,944,0,
																															 500,667,0,333,556,556,556,556,280,556,333,737,370,556,584,0,
																															 737,333,400,584,333,333,333,611,556,278,333,333,365,556,834,834,
																															 834,611,722,722,722,722,722,722,1000,722,667,667,667,667,278,278,
																															 278,278,722,722,778,778,778,778,778,584,778,722,722,722,722,667,
																															 667,611,556,556,556,556,556,556,889,556,556,556,556,556,278,278,
																															 278,278,611,611,611,611,611,611,611,584,611,611,611,611,611,556,
																															 611,556};

		static int[] Times_Roman_Widths = new int[] {
																										 250,333,408,500,500,833,778,180,333,333,500,564,250,333,
																										 250,278,500,500,500,500,500,500,500,500,500,500,278,278,564,564,
																										 564,444,921,722,667,667,722,611,556,722,722,333,389,722,611,889,
																										 722,722,556,722,667,556,611,722,722,944,722,722,611,333,278,333,
																										 469,500,333,444,500,444,500,444,333,500,500,278,278,500,278,778,
																										 500,500,500,500,333,389,278,500,500,722,500,500,444,480,200,480,
																										 541,0,500,0,333,500,444,1000,500,500,333,1000,556,333,889,0,
																										 611,0,0,333,333,444,444,350,500,1000,333,980,389,333,722,0,
																										 444,722,0,333,500,500,500,500,200,500,333,760,276,500,564,0,
																										 760,333,400,564,300,300,333,500,453,250,333,300,310,500,750,750,
																										 750,444,722,722,722,722,722,722,889,667,611,611,611,611,333,333,
																										 333,333,722,722,722,722,722,722,722,564,722,722,722,722,722,722,
																										 556,500,444,444,444,444,444,444,667,444,444,444,444,444,278,278,
																										 278,278,500,500,500,500,500,500,500,564,500,500,500,500,500,500,
																										 500,500};
		static int[] Times_Roman_Italic_Widths = new int[] {
																													250,333,420,500,500,833,778,214,333,333,500,675,250,333,
																													250,278,500,500,500,500,500,500,500,500,500,500,333,333,675,675,
																													675,500,920,611,611,667,722,611,611,722,722,333,444,667,556,833,
																													667,722,611,722,611,500,556,722,611,833,611,556,556,389,278,389,
																													422,500,333,500,500,444,500,444,278,500,500,278,278,444,278,722,
																													500,500,500,500,389,389,278,500,444,667,444,444,389,400,275,400,
																													541,0,500,0,333,500,556,889,500,500,333,1000,500,333,944,0,
																													556,0,0,333,333,556,556,350,500,889,333,980,389,333,667,0,
																													389,556,0,389,500,500,500,500,275,500,333,760,276,500,675,0,
																													760,333,400,675,300,300,333,500,523,250,333,300,310,500,750,750,
																													750,500,611,611,611,611,611,611,889,667,611,611,611,611,333,333,
																													333,333,722,667,722,722,722,722,722,675,722,722,722,722,722,556,
																													611,500,500,500,500,500,500,500,667,444,444,444,444,444,278,278,
																													278,278,500,500,500,500,500,500,500,675,500,500,500,500,500,444,
																													500,444};
		static int[] Times_Roman_Bold_Widths = new int[] {
																													250,333,555,500,500,1000,833,278,333,333,500,570,250,333,
																													250,278,500,500,500,500,500,500,500,500,500,500,333,333,570,570,
																													570,500,930,722,667,722,722,667,611,778,778,389,500,778,667,944,
																													722,778,611,778,722,556,667,722,722,1000,722,722,667,333,278,333,
																													581,500,333,500,556,444,556,444,333,500,556,278,333,556,278,833,
																													556,500,556,556,444,389,333,556,500,722,500,500,444,394,220,394,
																													520,0,500,0,333,500,500,1000,500,500,333,1000,556,333,1000,0,
																													667,0,0,333,333,500,500,350,500,1000,333,1000,389,333,722,0,
																													444,722,0,333,500,500,500,500,220,500,333,747,300,500,570,0,
																													747,333,400,570,300,300,333,556,540,250,333,300,330,500,750,750,
																													750,500,722,722,722,722,722,722,1000,722,667,667,667,667,389,389,
																													389,389,722,722,778,778,778,778,778,570,778,722,722,722,722,722,
																													611,556,500,500,500,500,500,500,722,444,444,444,444,444,278,278,
																													278,278,500,556,500,500,500,500,500,570,500,556,556,556,556,500,
																													556,500};
		static int[] Times_Roman_BoldItalic_Widths = new int[] {
																																250,389,555,500,500,833,778,278,333,333,500,570,250,333,
																																250,278,500,500,500,500,500,500,500,500,500,500,333,333,570,570,
																																570,500,832,667,667,667,722,667,667,722,778,389,500,667,611,889,
																																722,722,611,722,667,556,611,722,667,889,667,611,611,333,278,333,
																																570,500,333,500,500,444,500,444,333,500,556,278,278,500,278,778,
																																556,500,500,500,389,389,278,556,444,667,500,444,389,348,220,348,
																																570,0,500,0,333,500,500,1000,500,500,333,1000,556,333,944,0,
																																611,0,0,333,333,500,500,350,500,1000,333,1000,389,333,722,0,
																																389,611,0,389,500,500,500,500,220,500,333,747,266,500,606,0,
																																747,333,400,570,300,300,333,576,500,250,333,300,300,500,750,750,
																																750,500,667,667,667,667,667,667,944,667,667,667,667,667,389,389,
																																389,389,722,722,722,722,722,722,722,570,722,722,722,722,722,611,
																																611,500,500,500,500,500,500,500,722,444,444,444,444,444,278,278,
																																278,278,500,556,500,500,500,500,500,570,500,556,556,556,556,444,
																																500,444};

		double CalcCharWidth(char charcode, TTFontData fontdata)
		{
			byte intvalue;
			int defaultwidth;
			int[] aarray;
			aarray = null;
			defaultwidth = Default_Font_Width;
			bool isdefault = true;
			if (IsSpecial(charcode))
			{
				return 0.0;
			}
			if (FFont.Name == PDFFontType.Linked || FFont.Name == PDFFontType.Embedded)
			{
				// Ask for font size
				double x = InfoProvider.GetCharWidth(Font, fontdata, charcode);
				return (x * FFont.Size / 1000);
			}
			switch (FFont.Name)
			{
				case PDFFontType.Helvetica:
					isdefault = false;
					aarray = Helvetica_Widths;
					if (FFont.Bold)
					{
						if (FFont.Italic)
							aarray = Helvetica_BoldItalic_Widths;
						else
							aarray = Helvetica_Bold_Widths;
					}
					else
					{
						if (FFont.Italic)
							aarray = Helvetica_Italic_Widths;
						else
							aarray = Helvetica_Widths;
					}
					break;
				case PDFFontType.TimesRoman:
					isdefault = false;
					aarray = Times_Roman_Widths;
					if (FFont.Bold)
					{
						if (FFont.Italic)
							aarray = Times_Roman_BoldItalic_Widths;
						else
							aarray = Times_Roman_Bold_Widths;
					}
					else
					{
						if (FFont.Italic)
							aarray = Times_Roman_Italic_Widths;
						else
							aarray = Times_Roman_Widths;
					}
					break;
			}
			intvalue = (byte)charcode;
			if (isdefault || (intvalue < 32))
				return defaultwidth;
			double aresult = aarray[intvalue - 32];
			aresult = aresult * FFont.Size / 1000;
			return aresult;
		}
		const int AlignmentFlags_SingleLine = 64;
		const int AlignmentFlags_AlignHCenter = 4;
		const int AlignmentFlags_AlignHJustify = 1024;
		const int AlignmentFlags_AlignTop = 8;
		const int AlignmentFlags_AlignBottom = 16;
		const int AlignmentFlags_AlignVCenter = 32;
		const int AlignmentFlags_AlignLeft = 1;
		const int AlignmentFlags_AlignRight = 2;
		public void Translate(int X, int Y)
		{
			string transstring = "1 0 0 1 " +
				UnitsToTextX(X) + " " +
				UnitsToTextY(Y);
			translatedy = true;

			StreamUtil.SWriteLine(File.STempStream, transstring + " cm");
		}
		public void Rotate(double radiants)
		{
			string rotstring = NumberToText(Math.Cos(radiants)) + " " +
				NumberToText(Math.Sin(radiants)) + " " +
				NumberToText(-Math.Sin(radiants)) + " " +
				NumberToText(Math.Cos(radiants)) + " 0 0";
			StreamUtil.SWriteLine(File.STempStream, rotstring + " cm");
		}
		public void TextRect(Rectangle arect, string Text, int Alignment, bool Clipping,
			bool wordbreak, int Rotation, bool RightToLeft)
		{
			// Replace cr/lf for only cfs
			//Text = Text.Replace("" + (char)13 + (char)10, "" + (char)10);
			// Kill tabs replace with spaces
			Text = Text.Replace("\t", " ");
			// Remove chars and tabs
			Rectangle recsize;
			int i, index;
			int posx, posy;
			double currpos, alinedif;
			bool singleline;
			string astring;
			int alinesize;
			Strings lwords;
			Integers lwidths;
			Rectangle arec;
			string aword;
			bool isunicode = false;
			TTFontData adata = GetTTFontData();
			if (!(adata == null))
			{
				isunicode = adata.IsUnicode;
			}

			if (!isunicode)
				Text = UnicodeToWin1252(Text);

			File.CheckPrinting();

			if (Clipping || (Rotation != 0))
			{
				SaveGraph();
			}
			try
			{
				if (Clipping)
				{
					// Clipping rectangle
					StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(arect.Left) + " " + UnitsToTextY(arect.Top) +
						" " + UnitsToTextX(arect.Width) + " " + UnitsToTextX(-(arect.Height)) + " re");
					StreamUtil.SWriteLine(File.STempStream, "h"); // ClosePath
					StreamUtil.SWriteLine(File.STempStream, "W"); // Clip
					StreamUtil.SWriteLine(File.STempStream, "n"); // NewPath
				}
				if (Rotation != 0)
				{
					int X = arect.Left;
					int Y = arect.Top;
					Translate(X, Y);
					double rotrad = (double)Rotation / 10 * (2 * Math.PI / 360);
					Rotate(rotrad);
					arect = new Rectangle(0, 0, arect.Width, arect.Height);
				}
				singleline = (Alignment & AlignmentFlags_SingleLine) > 0;
				if (singleline)
					wordbreak = false;
				// Calculates text extent and apply alignment
				recsize = arect;
				TextExtent(Text, ref recsize, wordbreak, singleline, true);
				// Align bottom or center
				posy = arect.Top;
				if ((Alignment & AlignmentFlags_AlignBottom) > 0)
				{
					posy = arect.Top + arect.Height - recsize.Height;
				}
				if ((Alignment & AlignmentFlags_AlignVCenter) > 0)
				{
					posy = arect.Top + (int)((arect.Height - recsize.Height) / 2);
				}
				for (i = 0; i < Lines.Count; i++)
				{
					posx = arect.Left;
					// Aligns horz.
					if ((Alignment & AlignmentFlags_AlignRight) > 0)
					{
						// recsize.right contains the width of the full text
						posx = arect.Left + arect.Width - Lines[i].Width;
					}
					// Aligns horz.
					if ((Alignment & AlignmentFlags_AlignHCenter) > 0)
					{
						posx = arect.Left + (int)(((arect.Width) - Lines[i].Width) / 2);
					}
					astring = Text.Substring(Lines[i].Position, Lines[i].Size);
					if (((Alignment & AlignmentFlags_AlignHJustify) > 0) && (!Lines[i].LastLine))
					{
						// Calculate the sizes of the words, then
						// share space between words
						lwords = new Strings();
						aword = "";
						index = 0;
						while (index < astring.Length)
						{
							if (astring[index] != ' ')
							{
								aword = aword + astring[index];
							}
							else
							{
								if (aword.Length > 0)
								{
									lwords.Add(aword);
								}
								aword = "";
							}
							index++;
						}
						if (aword.Length > 0)
						{
							lwords.Add(aword);
						}
						// Calculate all words size
						alinesize = 0;
						lwidths = new Integers();
						for (index = 0; index < lwords.Count; index++)
						{
							arec = arect;
							TextExtent(lwords[index], ref arec, false, true, false);
							int nwidth;
							if (RightToLeft)
								nwidth = -(arec.Width);
							else
								nwidth = arec.Width;
							lwidths.Add(nwidth);
							alinesize = alinesize + nwidth;
						}
						alinedif = arect.Width - alinesize;
						if (alinedif > 0)
						{
							if (lwords.Count > 1)
								alinedif = alinedif / (lwords.Count - 1);
							if (RightToLeft)
							{
								currpos = arect.Width;
								alinedif = -alinedif;
							}
							else
								currpos = posx;
							if ((!Font.Transparent) && (lwords.Count > 0))
							{
								int PreviousBrushColor = BrushColor;
								int PreviousBrushStyle = BrushStyle;
								int PreviousPenStyle = PenStyle;
								int PreviousPenColor = PenColor;
								BrushColor = Font.BackColor;
								PenColor = Font.BackColor;
								PenStyle = 5;
								BrushStyle = 0;
								Rectangle(Convert.ToInt32(currpos), posy + Lines[i].TopPos, Convert.ToInt32(currpos) + Lines[i].Width, posy + Lines[i].TopPos + Lines[i].Height);
								PenColor = PreviousPenColor;
								PenStyle = PreviousPenStyle;
								BrushColor = PreviousBrushColor;
								BrushStyle = PreviousBrushStyle;
							}
							for (index = 0; index < lwords.Count; index++)
							{
								TextOut(Convert.ToInt32(currpos), posy + Lines[i].TopPos, lwords[index], Lines[i].Width, 0, RightToLeft);
								currpos = currpos + lwidths[index] + alinedif;
							}
						}
					}
					else
					{
						if (!Font.Transparent)
						{
							int PreviousBrushColor = BrushColor;
							int PreviousBrushStyle = BrushStyle;
							int PreviousPenStyle = PenStyle;
							int PreviousPenColor = PenColor;
							BrushColor = Font.BackColor;
							PenColor = Font.BackColor;
							PenStyle = 5;
							BrushStyle = 0;
							Rectangle(posx, posy + Lines[i].TopPos, posx + Lines[i].Width, posy + Lines[i].TopPos + Lines[i].Height);
							PenColor = PreviousPenColor;
							PenStyle = PreviousPenStyle;
							BrushColor = PreviousBrushColor;
							BrushStyle = PreviousBrushStyle;
						}

						TextOut(posx, posy + Lines[i].TopPos, astring, Lines[i].Width, 0, RightToLeft);
					}
				}
			}
			finally
			{
				if (Clipping || (Rotation != 0))
				{
					RestoreGraph();
				}
			}
		}
		public void DrawImage(Rectangle rec, MemoryStream abitmap, int dpires,
			bool tile, bool clip, long internal_imageindex)
		{
			MemoryStream astream;
			int imagesize;
			int bitmapwidth, bitmapheight;
			MemoryStream fimagestream;

			int aheight, awidth;
			Rectangle arect;
			bool isjpeg;
			bool indexed = false;
			int bitsperpixel = 8;
			int imageindex;
			int numcolors = 0;
			string palette = "";
			string mask = "";
			bool newstream = true;
			MemoryStream imageMaskStream;
			arect = rec;
			File.CheckPrinting();
			if (File.CalculateOnly)
			{
				fimagestream = null;
				imageMaskStream = null;
			}
			else
			{
				fimagestream = new MemoryStream();
				imageMaskStream = new MemoryStream();
			}
			try
			{
				abitmap.Seek(0, System.IO.SeekOrigin.Begin);
				bool isgif = false;
				isjpeg = BitmapUtil.GetJPegInfo(abitmap, out bitmapwidth, out bitmapheight);
				if (isjpeg)
				{
					// Read image dimensions
					//				fimagestream.SetSize(abitmap.size);
					if (!File.CalculateOnly)
					{
						abitmap.Seek(0, System.IO.SeekOrigin.Begin);
						abitmap.WriteTo(fimagestream);
						fimagestream.Seek(0, System.IO.SeekOrigin.Begin);
						imagesize = (int)fimagestream.Length;
					}
					else
						imagesize = 0;
				}
				else
				{
					bool isBitmap = BitmapUtil.GetBitmapInfo(abitmap, out bitmapwidth, out bitmapheight,
						out imagesize, fimagestream, out indexed, out bitsperpixel, out numcolors, out palette, out isgif, out mask, imageMaskStream);
					if ((isgif) || (!isBitmap))
					{
						// #if NETSTANDARD2_0
						//throw new Exception("Bitmap format not supported in .net standard");
						//#else
						isgif = false;
						//abitmap.Seek(0, System.IO.SeekOrigin.Begin);
						//fimagestream.SetLength(abitmap.Length);
						//abitmap.WriteTo(fimagestream);
						//fimagestream.Seek(0, System.IO.SeekOrigin.Begin);
						//imagesize = (int)fimagestream.Length;

						Image newimage = null;
						// Png /gif etc not supported so trye save image as Bitmap
						try
						{
							abitmap.Seek(0, System.IO.SeekOrigin.Begin);
							newimage = Image.FromStream(abitmap);

							// Transparent color correctio
							/*Bitmap newbitmap = new Bitmap(newimage.Width, newimage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                            using (Graphics gr = Graphics.FromImage(newbitmap))
                            {
                                gr.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, newbitmap.Width, newbitmap.Height));
                                gr.DrawImage(newimage, new Rectangle(0, 0, newbitmap.Width , newbitmap.Height ));
                            }
                            newimage.Dispose();
                            newimage = newbitmap;                        */
						}
						catch
						{

						}
						if (newimage != null)
						{
							MemoryStream newbitmapstream = new MemoryStream();
							newimage.Save(newbitmapstream, System.Drawing.Imaging.ImageFormat.Bmp);
							abitmap = newbitmapstream;
							abitmap.Seek(0, System.IO.SeekOrigin.Begin);
							if (imageMaskStream != null)
							{
								imageMaskStream = new MemoryStream();
							}
							BitmapUtil.GetBitmapInfo(abitmap, out bitmapwidth, out bitmapheight,
								out imagesize, fimagestream, out indexed, out bitsperpixel, out numcolors, out palette, out isgif, out mask, imageMaskStream);
						}
						// #endif
					}
				}
				if (imageMaskStream != null)
				{
					if (imageMaskStream.Length == 0)
					{
						imageMaskStream.Dispose();
						imageMaskStream = null;
					}
				}
				if (dpires != 0)
				{
					var arec = new Rectangle(rec.Left, rec.Top, rec.Width, rec.Height);
					rec = new Rectangle(rec.Left, rec.Top,
						(int)Math.Round((double)bitmapwidth / dpires * FResolution),
						(int)Math.Round((double)bitmapheight / dpires * FResolution));
					if (rec.Width < arec.Width)
						rec = new Rectangle(rec.Left + (arec.Width - rec.Width) / 2, rec.Top,
							rec.Width, rec.Height);
					if (rec.Height < arec.Height)
						rec = new Rectangle(rec.Left, rec.Top + (arec.Height - rec.Height) / 2,
							rec.Width, rec.Height);
					arect = rec;
				}
				if (internal_imageindex >= 0)
				{
					if (File.ImageIndexes.IndexOfKey(internal_imageindex.ToString()) >= 0)
					{
						imageindex = (int)File.ImageIndexes[internal_imageindex.ToString()];
						newstream = false;
					}
					else
					{
						File.ImageCount = File.ImageCount + 1;
						imageindex = File.ImageCount;
						File.ImageIndexes.Add(internal_imageindex.ToString(), imageindex);
					}
				}
				else
				{
					File.ImageCount = File.ImageCount + 1;
					imageindex = File.ImageCount;
				}
				StreamUtil.SWriteLine(File.STempStream, "q");
				if (clip)
				{
					// Clipping rectangle
					StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(arect.Left) + " " + UnitsToTextY(arect.Top) +
						' ' + UnitsToTextX(arect.Width - arect.Left) + ' ' + UnitsToTextX(-(arect.Height - arect.Top)) + " re");
					StreamUtil.SWriteLine(File.STempStream, "h"); // ClosePath
					StreamUtil.SWriteLine(File.STempStream, "W"); // Clip
					StreamUtil.SWriteLine(File.STempStream, "n"); // NewPath
				}
				awidth = rec.Width;
				aheight = rec.Height;
				if (awidth <= 0)
					tile = false;
				if (aheight <= 0)
					tile = false;
				do
				{
					rec = new Rectangle(arect.Left, rec.Top, awidth, aheight);
					do
					{
						/*if (newstream && (imageMaskStream != null))
                        {
							StreamUtil.SWriteLine(File.STempStream, "AIS false");
						}*/
						StreamUtil.SWriteLine(File.STempStream, "q");
						// Translate
						StreamUtil.SWriteLine(File.STempStream, "1 0 0 1 "
							+ UnitsToTextX(rec.Left) +
							" " + UnitsToTextY(rec.Top + rec.Height) + " cm");
						// Scale
						StreamUtil.SWriteLine(File.STempStream, UnitsToTextX(rec.Width) +
							" 0 0  " + UnitsToTextX(rec.Height) + " 0 0 cm");
						StreamUtil.SWriteLine(File.STempStream, "/Im" + imageindex.ToString() + " Do");
						StreamUtil.SWriteLine(File.STempStream, "Q");
						if (!tile)
							break;
						rec = new Rectangle(rec.Left + awidth, rec.Top, rec.Left + awidth, rec.Height);
					} while (rec.Width <= arect.Width + awidth);
					if (!tile)
						break;
					rec = new Rectangle(rec.Left, rec.Top + aheight, rec.Width, rec.Top + aheight);
				} while (rec.Height <= arect.Height + aheight);
				if (newstream)
				{
					System.IO.MemoryStream maskStream = null;
					string imageName = "Im" + File.ImageCount.ToString();
					if (!File.CalculateOnly)
					{
						string imageMaskName = "";
						if (imageMaskStream != null)
						{
							File.ImageCount = File.ImageCount + 1;
							imageMaskName = "Im" + File.ImageCount.ToString();

							// Saves the bitmap to temp bitmaps
							maskStream = new MemoryStream();
							StreamUtil.SWriteLine(maskStream, "<< /Type /XObject");
							StreamUtil.SWriteLine(maskStream, "/Subtype /Image");
							StreamUtil.SWriteLine(maskStream, "/Width " + bitmapwidth.ToString());
							StreamUtil.SWriteLine(maskStream, "/Height " + bitmapheight.ToString());
							StreamUtil.SWriteLine(maskStream, "/ColorSpace /DeviceGray");
							StreamUtil.SWriteLine(maskStream, "/BitsPerComponent 8");
							imageMaskName = "Im" + File.ImageCount.ToString();
							StreamUtil.SWriteLine(maskStream, "/Name /" + imageMaskName);

#if REPMAN_ZLIB
							long lengthPositionMask = 0;
							if (File.Compressed)
							{
								byte[] bytesLength = ASCIIEncoding.ASCII.GetBytes("/Length ");
								maskStream.Write(bytesLength, 0, bytesLength.Length);
								lengthPositionMask = maskStream.Position;
								StreamUtil.SWriteLine(maskStream, "             ");
								StreamUtil.SWriteLine(maskStream, "/Length1 " + imageMaskStream.Length.ToString());
								StreamUtil.SWriteLine(maskStream, "/Filter [/FlateDecode]");
							}
							else
								StreamUtil.SWriteLine(maskStream, "/Length " + imageMaskStream.Length.ToString());
#endif

							StreamUtil.SWriteLine(maskStream, ">>");
							StreamUtil.SWriteLine(maskStream, "stream");
							imageMaskStream.Seek(0, System.IO.SeekOrigin.Begin);
#if REPMAN_ZLIB
							if (File.Compressed)
							{
								// StreamUtil.CompressStream(fimagestream, astream, false);		
								CancellationTokenSource cancelSource = new CancellationTokenSource();
								var ntask = StreamUtil.CompressStreamTask(imageMaskStream, maskStream, false, true, cancelSource);
								ntask.ConfigureAwait(false);
								File.CompressionTasks.Add(new CompressionTask(ntask, cancelSource, maskStream, lengthPositionMask));
							}
							else
#endif
								imageMaskStream.WriteTo(maskStream);

						}
						// Saves the bitmap to temp bitmaps
						astream = new MemoryStream();
						StreamUtil.SWriteLine(astream, "<< /Type /XObject");
						StreamUtil.SWriteLine(astream, "/Subtype /Image");
						StreamUtil.SWriteLine(astream, "/Width " + bitmapwidth.ToString());
						StreamUtil.SWriteLine(astream, "/Height " + bitmapheight.ToString());
						if (indexed)
						{
							StreamUtil.SWriteLine(astream, "/ColorSpace");
							StreamUtil.SWriteLine(astream, "[/Indexed");
							StreamUtil.SWriteLine(astream, "/DeviceRGB " + numcolors.ToString());
							StreamUtil.SWriteLine(astream, palette);
							StreamUtil.SWriteLine(astream, "]");
							StreamUtil.SWriteLine(astream, "/BitsPerComponent " + bitsperpixel.ToString());
							if (mask.Length > 0)
							{
								StreamUtil.SWriteLine(astream, "/Mask " + mask);

							}
						}
						else
						{
							StreamUtil.SWriteLine(astream, "/ColorSpace /DeviceRGB");
							StreamUtil.SWriteLine(astream, "/BitsPerComponent 8");
						}
						StreamUtil.SWriteLine(astream, "/Name /" + imageName);
						if (maskStream != null)
						{
							// StreamUtil.SWriteLine(astream, "/SMask " + " 26 0 R");
							byte[] maskBuf = ASCIIEncoding.ASCII.GetBytes("/SMask ");
							astream.Write(maskBuf, 0, maskBuf.Length);
							long imageMaskPosition = astream.Position;
							File.Masks.Add(File.ImageCount - 1, new StreamPosition(astream, imageMaskPosition));
							StreamUtil.SWriteLine(astream, "               0 R");
						}
						long lengthPosition = 0;
						if (isjpeg)
						{
							StreamUtil.SWriteLine(astream, "/Length " + fimagestream.Length.ToString());
							StreamUtil.SWriteLine(astream, "/Filter [/DCTDecode]");
						}
						else
							if (isgif)
						{
							StreamUtil.SWriteLine(astream, "/Length " + fimagestream.Length.ToString());
							StreamUtil.SWriteLine(astream, "/Filter [/LZWDecode]");
						}
						else
						{
#if REPMAN_ZLIB
							if (File.Compressed)
							{
								byte[] bytesLength = ASCIIEncoding.ASCII.GetBytes("/Length ");
								astream.Write(bytesLength, 0, bytesLength.Length);
								lengthPosition = astream.Position;
								StreamUtil.SWriteLine(astream, "             ");
								StreamUtil.SWriteLine(astream, "/Length1 " + fimagestream.Length.ToString());
								StreamUtil.SWriteLine(astream, "/Filter [/FlateDecode]");
							}
							else
#endif
								StreamUtil.SWriteLine(astream, "/Length " + fimagestream.Length.ToString());
						}
						StreamUtil.SWriteLine(astream, ">>");
						StreamUtil.SWriteLine(astream, "stream");
						fimagestream.Seek(0, System.IO.SeekOrigin.Begin);
#if REPMAN_ZLIB
						if ((File.Compressed) && (!isjpeg) && (!isgif))
						{
							// StreamUtil.CompressStream(fimagestream, astream, false);		
							CancellationTokenSource cancelSource = new CancellationTokenSource();
							var ntask = StreamUtil.CompressStreamTask(fimagestream, astream, false, true, cancelSource);
							ntask.ConfigureAwait(false);
							File.CompressionTasks.Add(new CompressionTask(ntask, cancelSource, astream, lengthPosition));
						}
						else
#endif
							fimagestream.WriteTo(astream);
						File.BitmapStreams.Add(astream);
						if (maskStream != null)
							File.BitmapStreams.Add(maskStream);
					}
				}
			}
			finally
			{
			}
		}
		public void TextExtent(string Text, ref Rectangle rect, bool wordbreak, bool singleline, bool dolineinfo)
		{
			// Calculate leading and line spacing
			bool havekerning = false;
			TTFontData adata = GetTTFontData();
			//double kerningamount;
			int linespacing;
			int leading;
			if (adata != null)
			{
				if (adata.HaveKerning)
					havekerning = true;
				linespacing = adata.Ascent - adata.Descent + adata.Leading;
				leading = adata.Leading;
			}
			else
			{
				GetStdLineSpacing(out linespacing, out leading);
			}
			leading = (int)Math.Round((((double)leading) / 100000.0) * FResolution * FFont.Size * 1.25);
			linespacing = (int)Math.Round((((double)linespacing) / 100000.0) * FResolution * FFont.Size * 1.25);


			StringBuilder currentline = new StringBuilder();
			double currentwidth = 0;
			double maxwidth = 0;
			double newsize = 0;
			int linebreakpos = 0;
			bool wasspace = false;
			int infocount = 0;
			double lastsize = 0;
			double lastsizewithoutspace = 0;
			int lastindexwithoutspace = 0;
			int currenttoppos = 0;
			double recwidth = (double)(rect.Width) / FResolution * PDFFile.CONS_PDFRES;
			if (dolineinfo)
				Lines.Clear();
			// Replace cr/lf for only cfs
			//string astring = Text.Replace(""+(char)13+(char)10,""+(char)10);
			// Kill tabs replace with spaces
			string astring = Text;
			astring = astring.Replace("\t", " ");
			int i = 0;
			int startposition = 0;
			LineInfo linfo = new LineInfo();
			while (i < astring.Length)
			{
				// Skip cr chars
				//                if (astring[i] == (char)13)
				//{
				//i++;
				//if (i >= astring.Length)
				//break;
				//}
				// Check for LF
				if ((astring[i] == (char)10))
				{
					// Add the line
					int cutindex = i;
					if (wasspace)
					{
						cutindex = lastindexwithoutspace + 1;
						currentwidth = lastsizewithoutspace;
					}
					linfo.LastLine = true;
					linfo.Position = startposition;
					linfo.Step = PrintStepType.cpi10;
					linfo.Size = cutindex - startposition;
					if (i > 0)
						if (astring[i - 1] == (char)13)
							linfo.Size = linfo.Size - 1;
					if (linfo.Size < 0)
						linfo.Size = 0;
					linfo.Width = (int)Math.Round((currentwidth * FResolution / PDFFile.CONS_PDFRES));
					if (currentwidth > maxwidth)
						maxwidth = currentwidth;
					linfo.TopPos = currenttoppos - leading;
					linfo.Height = linespacing;
					currenttoppos = currenttoppos + linespacing;
					if (dolineinfo)
						Lines.Add(linfo);
					infocount++;
					currentwidth = 0;
					startposition = i + 1;
				}
				else
				{
					newsize = CalcCharWidth(astring[i], adata);
					// Check Kerning pairs to reduce new size
					if (havekerning)
					{
						if (i < (astring.Length - 1))
						{
							/*
                            // Kerning is not supported by GDI+ so disable it (PDF and GDI+)
                            kerningamount = InfoProvider.GetKerning(Font, adata, astring[i], astring[i + 1]);
                            newsize = newsize - (kerningamount * (double)FFont.Size / 1000);
                            */
						}
					}
					// If the character fits inside the line
					if ((currentwidth + newsize <= recwidth) || (!wordbreak))
					{
						if (astring[i] == ' ')
						{
							if (!wasspace)
							{
								wasspace = true;
								lastsizewithoutspace = currentwidth;
								if (i > startposition)
								{
									lastindexwithoutspace = i - 1;
									linebreakpos = i - 1;
									lastsize = currentwidth;
								}
								else
									lastindexwithoutspace = 0;
							}
						}
						else
						{
							wasspace = false;
							if (IsSign(astring[i]))
							{
								linebreakpos = i;
								lastsize = currentwidth + newsize;
							}
						}
						currentwidth = currentwidth + newsize;
					}
					// When the character does not fit
					else
					{
						if (wordbreak)
						{
							// Add the line
							if ((currentwidth == 0) && (startposition == i))
								i++;
							int cutindex = i;
							if (linebreakpos > 0)
							{
								cutindex = linebreakpos + 1;
								i = linebreakpos;
								currentwidth = lastsize;
							}
							else
								i--;
							if (wasspace)
							{
								cutindex = lastindexwithoutspace + 1;
								currentwidth = lastsizewithoutspace;
							}
							linfo.LastLine = false;
							linfo.Position = startposition;
							linfo.Step = PrintStepType.cpi10;
							linfo.Size = cutindex - startposition;
							if (linfo.Size < 0)
								linfo.Size = 0;
							linfo.Width = (int)Math.Round((currentwidth * FResolution / PDFFile.CONS_PDFRES));
							if (currentwidth > maxwidth)
								maxwidth = currentwidth;
							linfo.TopPos = currenttoppos - leading;
							linfo.Height = linespacing;
							currenttoppos = currenttoppos + linespacing;
							if (dolineinfo)
								Lines.Add(linfo);
							infocount++;
							currentwidth = 0;
							linebreakpos = 0;
							startposition = i + 1;
							// Skip spaces
							while (startposition < astring.Length - 1)
							{
								if (astring[startposition] == ' ')
								{
									startposition++;
									i = startposition - 1;
								}
								else
									break;
							}
						}
					}
				}
				i++;
			}
			// Check for LF
			if (startposition < astring.Length)
			{
				// Add the line
				int cutindex = i;
				if (wasspace)
				{
					cutindex = lastindexwithoutspace + 1;
					currentwidth = lastsizewithoutspace;
				}
				linfo.LastLine = true;
				linfo.Position = startposition;
				linfo.Step = PrintStepType.cpi10;
				linfo.Size = cutindex - startposition;
				if (linfo.Size < 0)
					linfo.Size = 0;
				linfo.Width = (int)Math.Round((currentwidth * FResolution / PDFFile.CONS_PDFRES));
				if (currentwidth > maxwidth)
					maxwidth = currentwidth;
				linfo.TopPos = currenttoppos - leading;
				linfo.Height = linespacing;
				currenttoppos = currenttoppos + linespacing;
				if (dolineinfo)
					Lines.Add(linfo);
				infocount++;
			}
			int totalheight = 0;
			if (infocount > 0)
				totalheight = infocount * linespacing + leading;
			Rectangle arec = new Rectangle(rect.Left, rect.Top,
										   (int)Math.Round((maxwidth * FResolution / PDFFile.CONS_PDFRES)),
										   totalheight);
			rect = arec;
		}

		/*		public void TextExtent(string Text, ref Rectangle rect, bool wordbreak, bool singleline, bool dolineinfo)
				{
					string astring;
					double asize;
					Rectangle arec;
					int position, i;
					LineInfo info = new LineInfo();
					int offset = 0;

					double maxwidth, newsize, recwidth;
					int linebreakpos;
					bool nextline;
					double alastsize;
					bool lockspace;
					bool createsnewline;
					bool havekerning;
					TTFontData adata;
					int kerningamount;
					int linespacing;
					int leading;
					// Text extent for the simple strings, wide strings not supported
					havekerning = false;
					adata = GetTTFontData();
					if (adata != null)
					{
						if (adata.HaveKerning)
							havekerning = true;
						linespacing = adata.Ascent - adata.Descent + adata.Leading;
						leading = adata.Leading;
					}
					else
					{
						GetStdLineSpacing(out linespacing, out leading);
					}
					leading = (int)Math.Round((((double)leading) / 100000.0) * FResolution * FFont.Size * 1.25);
					linespacing = (int)Math.Round((((double)linespacing) / 100000.0) * FResolution * FFont.Size * 1.25);

					createsnewline = false;
					astring = Text;
					arec = new Rectangle(0, 0, rect.Width, 0);
					asize = 0;
					if (dolineinfo)
						Lines.Clear();

					position = 0;
					linebreakpos = 0;
					maxwidth = 0;
					recwidth = (double)(rect.Width - rect.Left) / FResolution * PDFFile.CONS_PDFRES;
					nextline = false;
					i = 0;
					alastsize = 0;
					lockspace = false;
					bool incomplete;
					while (i < astring.Length)
					{
						incomplete = false;
						newsize = CalcCharWidth(astring[i], adata);
						if (havekerning)
						{
							if (i < (astring.Length - 1))
							{
								kerningamount = InfoProvider.GetKerning(Font, adata, astring[i], astring[i + 1]);
								newsize = newsize - (kerningamount * (double)FFont.Size / 1000);
							}
						}
						if (!IsSeparator(astring[i]))
							lockspace = false;
						if (wordbreak)
						{
							if (asize + newsize > recwidth)
							{
								if (linebreakpos > 0)
								{
									i = linebreakpos;
									nextline = true;
									asize = alastsize;
									linebreakpos = 0;
								}
								else
								{
									nextline = true;
									incomplete = true;
									linebreakpos = 0;
								}
							}
							else
							{
								if (IsSign(astring[i]))
								{
									linebreakpos = i;
									if (astring[i] == ' ')
									{
										if (!lockspace)
										{
											alastsize = asize;
											lockspace = true;

										}
										asize=asize+newsize;
									}
									else
									{
										asize = asize + newsize;
										alastsize = asize;
									}
								}
								else
									asize = asize + newsize;
							}
						}
						else
							asize = asize + newsize;
						if ((!singleline) && (i<astring.Length))
						{
							if (astring[i] == (char)10)
							{
								nextline = true;
								offset = 1;
								createsnewline = true;
							}
							else
							if (astring[i] == (char)13)
							{
								if (i<(astring.Length-1))
								{
									if (astring[i+1] == (char)10)
									{
										nextline = true;
										offset = 2;
										i++;
										createsnewline = true;
									}
								}
							}
						}
						if (asize > maxwidth)
							maxwidth = asize;
						if (nextline)
						{
							nextline = false;
							info.Position = position;
							info.Size = i - position - offset;
							info.Width = (int)Math.Round((asize) / PDFFile.CONS_PDFRES * FResolution);
							//   info.height:=Round((Font.Size)/CONS_PDFRES*FResolution);
							info.Height = linespacing;
							info.TopPos = arec.Height - leading;
							info.LastLine = createsnewline;
							info.Step = PrintStepType.cpi10;
							arec = new Rectangle(arec.Left, arec.Top, arec.Width, arec.Height + info.Height);
							asize = 0;
							if (incomplete)
								i--;
							position = i + 1;
							if (dolineinfo)
								Lines.Add(info);
							createsnewline = false;
							// Skip only one blank char
							if (!incomplete)
								if (i < astring.Length - 1)
									if (astring[i + 1] == ' ')
									{
										i++;
										position = i + 1;
									}
						}
						i++;
					}
					arec = new Rectangle(arec.Left, arec.Top,
						(int)Math.Round((maxwidth + 1) / PDFFile.CONS_PDFRES * FResolution), arec.Height);
					if (position <= astring.Length - 1)
					{
						info.Position = position;
						info.Size = astring.Length - position - offset;
						info.Width = (int)Math.Round((asize + 1) / PDFFile.CONS_PDFRES * FResolution);
						info.Height = linespacing;
						info.TopPos = arec.Height - leading;
						info.Step = PrintStepType.cpi10;
						arec = new Rectangle(arec.Left, arec.Top, arec.Width, arec.Height + info.Height);
						info.LastLine = true;
						if (dolineinfo)
							Lines.Add(info);
					}
					arec = new Rectangle(arec.Left, arec.Top, arec.Width, arec.Height + leading);
					rect = arec;
				}
		 */
		static PDFCanvas()
		{
#if NETSTANDARD2_0
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
		}
		static string UnicodeToWin1252(string source)
		{

			Encoder enc = Encoding.GetEncoding(1252).GetEncoder();
			Byte[] pbytes = new byte[source.Length * 2];
			int bytesused;

			//enc.Convert(source.ToCharArray(),0,source.Length,pbytes,0,source.Length,true,out charsused,out bytesused,out completed);

			bytesused = enc.GetBytes(source.ToCharArray(), 0, source.Length, pbytes, 0, true);

			StringBuilder st = new StringBuilder();
			char c;
			for (int i = 0; i < bytesused; i++)
			{
				c = (char)pbytes[i];
				st.Append(c);
			}
			return st.ToString();
		}
	}
	public class PDFFile : IDisposable
	{
		private const string LINE_FEED = "\r\n";
		private long FResourceNum, FCatalogNum;
		private long FOutlinesNum;
		private int FCurrentSetPageObject;
		private Strings FPages;
		private MemoryStream FSTempStream;
		private MemoryStream FTempStream;
		private Stream FMainPDF;
		public string FileName = "";
		private PDFCanvas FCanvas;
		private bool FPrinting;
		private int FFontCount;
		private long FObjectCount;
		private PageInfos FPageInfos;
		private int FResolution;
		private long FStreamSize1;
		private int FStreamSize2;
		private Longs FObjectOffsets;
		public SortedList ImageIndexes;
		public long FObjectOffset;
		public bool Optimized;
		private int FPage;
		private long FParentNum;
		private Strings FFontList;
		private MemStreams FBitmapStreams;
		public List<CompressionTask> CompressionTasks;
		public const int POINTS_PER_INCH = 72;
		public bool Compressed;
		public bool OptimizeSize = true;
		public int ImageCount;
		public const int CONS_PDFRES = POINTS_PER_INCH;
		public const double CONS_UNDERLINEPOS = 1.1;
		public const double CONS_STRIKEOUTPOS = 0.7;
		public const double CONS_UNDERLINEWIDTH = 0.1;
		private const string CONS_UNICODEPREDIX = "";
		public Stream MainPDF { get { return FMainPDF; } }
		public int PageHeight;
		public int PageWidth;
		public MemStreams BitmapStreams { get { return FBitmapStreams; } }
		public string DocTitle, DocAuthor, DocCreator, DocKeywords, DocSubject, DocProducer;
		/// <summary>
		/// The pdf is not generated but all size calculations are done
		/// </summary>
		public bool CalculateOnly;
		public SortedList<int, StreamPosition> Masks = new SortedList<int, StreamPosition>();
		virtual public void Dispose()
		{
#if REPMAN_DOTNET1
#else
			if (FMainPDF != null)
			{
#if REPMAN_COMPACT
#else
				FMainPDF.Dispose();
#endif
				FMainPDF = null;
			}
			if (FTempStream != null)
			{
#if REPMAN_COMPACT
#else
				FTempStream.Dispose();
#endif
				FTempStream = null;
			}
			if (FSTempStream != null)
			{
#if REPMAN_COMPACT
#else
				FSTempStream.Dispose();
#endif
				FSTempStream = null;
			}
			FBitmapStreams.Dispose();
			foreach (var task in CompressionTasks)
			{
				task.CancelTask();
			}
			CompressionTasks.Clear();
#endif
		}
		public PDFFile()
		{
			Optimized = false;
			DocTitle = "Report Manager Document";
			DocAuthor = "Report Manager engine";
			DocCreator = DocAuthor;
			DocKeywords = "";
			DocSubject = "";
			DocProducer = DocAuthor;
			FResolution = Twips.TWIPS_PER_INCH;
			FPageInfos = new PageInfos();
			ImageIndexes = new SortedList();
			FPages = new Strings();
			FCanvas = new PDFCanvas();
			FCanvas.File = this;
			FCanvas.Resolution = FResolution;
			PageWidth = 12048;
			PageHeight = 17039;
			FMainPDF = null;
			FTempStream = new MemoryStream();
			FSTempStream = new MemoryStream();
			FBitmapStreams = new MemStreams();
			CompressionTasks = new List<CompressionTask>();
			FObjectOffsets = new Longs();
			FFontList = new Strings();
			FOutlinesNum = 0;
		}
		public void CheckPrinting()
		{
			if (!FPrinting)
				throw new Exception("Not printing (PDFFile.CheckPrinting)");
		}
		public int Resolution
		{
			get { return FResolution; }
			set { FResolution = value; FCanvas.Resolution = value; }
		}
		public bool Printing
		{
			get { return FPrinting; }
		}
		public MemoryStream STempStream
		{
			get { return FSTempStream; }
		}
		public PDFCanvas Canvas
		{
			get { return FCanvas; }
		}
		void AddToOffset(long offset)
		{
			FObjectOffset = FObjectOffset + offset;
			FObjectOffsets.Add(FObjectOffset);
		}
		void CreateFont(string subtype, string basefont, string encoding)
		{
			FFontCount = FFontCount + 1;
			FObjectCount = FObjectCount + 1;
			FFontList.Add(FObjectCount.ToString());
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
			StreamUtil.SWriteLine(FTempStream, "<< /Type /Font");
			StreamUtil.SWriteLine(FTempStream, "/Subtype /" + subtype);
			StreamUtil.SWriteLine(FTempStream, "/Name /F" + FFontCount.ToString());
			StreamUtil.SWriteLine(FTempStream, "/BaseFont /" + basefont);
			StreamUtil.SWriteLine(FTempStream, "/Encoding /" + encoding);
			StreamUtil.SWriteLine(FTempStream, ">>");
			StreamUtil.SWriteLine(FTempStream, "endobj");
			AddToOffset(FTempStream.Length);
			FTempStream.WriteTo(FMainPDF);
		}
		void SetOutLine()
		{
			FObjectCount = FObjectCount + 1;
			FOutlinesNum = FObjectCount;
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
			StreamUtil.SWriteLine(FTempStream, "<< /Type /Outlines");
			StreamUtil.SWriteLine(FTempStream, "/Count 0");
			StreamUtil.SWriteLine(FTempStream, ">>");
			StreamUtil.SWriteLine(FTempStream, "endobj");
			AddToOffset(FTempStream.Length);
			FTempStream.WriteTo(FMainPDF);
		}
		void SetPages()
		{
			int i, PageObjNum;
			FObjectCount = FObjectCount + 1;
			FParentNum = FObjectCount;
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
			StreamUtil.SWriteLine(FTempStream, "<< /Type /Pages");
			StreamUtil.SWriteLine(FTempStream, "/Kids [");

			PageObjNum = 2;
			for (i = 1; i <= FPage; i++)
			{
				StreamUtil.SWriteLine(FTempStream, (FObjectCount + i + 1 + ImageCount).ToString() + " 0 R");
				FPages.Add(PageObjNum.ToString());
				PageObjNum = PageObjNum + 2;
			}
			StreamUtil.SWriteLine(FTempStream, "]");
			StreamUtil.SWriteLine(FTempStream, "/Count " + FPage.ToString());
			StreamUtil.SWriteLine(FTempStream, ">>");
			StreamUtil.SWriteLine(FTempStream, "endobj");
			AddToOffset(FTempStream.Length);
			FTempStream.WriteTo(FMainPDF);
		}
		void SetFontType()
		{
			int i;
			TTFontData adata;
			int index, acount;
			string awidths;

			CreateFont("Type1", "Helvetica", "WinAnsiEncoding");
			CreateFont("Type1", "Helvetica-Bold", "WinAnsiEncoding");
			CreateFont("Type1", "Helvetica-Oblique", "WinAnsiEncoding");
			CreateFont("Type1", "Helvetica-BoldOblique", "WinAnsiEncoding");
			CreateFont("Type1", "Courier", "WinAnsiEncoding");
			CreateFont("Type1", "Courier-Bold", "WinAnsiEncoding");
			CreateFont("Type1", "Courier-Oblique", "WinAnsiEncoding");
			CreateFont("Type1", "Courier-BoldOblique", "WinAnsiEncoding");
			CreateFont("Type1", "Times-Roman", "WinAnsiEncoding");
			CreateFont("Type1", "Times-Bold", "WinAnsiEncoding");
			CreateFont("Type1", "Times-Italic", "WinAnsiEncoding");
			CreateFont("Type1", "Times-BoldItalic", "WinAnsiEncoding");
			CreateFont("Type1", "Symbol", "WinAnsiEncoding");
			CreateFont("Type1", "ZapfDingbats", "WinAnsiEncoding");
			// Writes font files
			for (i = 0; i < FCanvas.FontData.Count; i++)
			{
				adata = (TTFontData)Canvas.FontData.GetByIndex(i);
				if (adata.Embedded)
				{
					// Writes font resource data
					FObjectCount = FObjectCount + 1;
					FTempStream.SetLength(0);
					StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
					MemoryStream fontcontent = Canvas.InfoProvider.GetFontStream(adata);

					System.IO.MemoryStream fontcontentstream = new MemoryStream();
#if REPMAN_ZLIB
					if (Compressed)
					{
						StreamUtil.CompressStream(fontcontent, fontcontentstream, OptimizeSize);
					}
					else
#endif
						fontcontent.WriteTo(fontcontentstream);

					StreamUtil.SWriteLine(FTempStream, "<< /Length " + fontcontentstream.Length.ToString());
					StreamUtil.SWriteLine(FTempStream, "/Length1 " + fontcontent.Length.ToString());
					adata.ObjectIndex = FObjectCount;
#if REPMAN_ZLIB
					if (Compressed)
						StreamUtil.SWriteLine(FTempStream, "/Filter [/FlateDecode]");
#endif
					StreamUtil.SWriteLine(FTempStream, ">>");
					StreamUtil.SWriteLine(FTempStream, "stream");
					fontcontent.Seek(0, System.IO.SeekOrigin.Begin);

					FTempStream.Write(fontcontentstream.GetBuffer(), 0, Convert.ToInt32(fontcontentstream.Length));

					StreamUtil.SWriteLine(FTempStream, PDFCanvas.ENDSTREAM);
					StreamUtil.SWriteLine(FTempStream, "endobj");
					AddToOffset(FTempStream.Length);
					FTempStream.WriteTo(FMainPDF);
				}
				else
				{
					adata.ObjectIndex = 0;
				}
				// Writes font descriptor
				FObjectCount = FObjectCount + 1;
				FTempStream.SetLength(0);
				StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
				adata.DescriptorIndex = FObjectCount;
				StreamUtil.SWriteLine(FTempStream, "<< /Type /FontDescriptor");
				if (adata.IsUnicode)
				{
					StreamUtil.SWriteLine(FTempStream, "/FontName /" + adata.PostcriptName);
					StreamUtil.SWriteLine(FTempStream, "/FontFamily(" + adata.FontFamily + ")");
				}
				else
					StreamUtil.SWriteLine(FTempStream, "/FontName /" + adata.PostcriptName);
				StreamUtil.SWriteLine(FTempStream, "/Flags " + adata.Flags.ToString());
				StreamUtil.SWriteLine(FTempStream, "/FontBBox [" +
					adata.FontBBox.Left.ToString() + " " +
					adata.FontBBox.Height.ToString() + " " +
					adata.FontBBox.Width.ToString() + " " +
					adata.FontBBox.Top.ToString() + "]");
				StreamUtil.SWriteLine(FTempStream, "/ItalicAngle " + ((int)Math.Round(adata.ItalicAngle)).ToString());
				StreamUtil.SWriteLine(FTempStream, "/Ascent " + adata.Ascent.ToString());
				StreamUtil.SWriteLine(FTempStream, "/Descent " + adata.Descent.ToString());
				StreamUtil.SWriteLine(FTempStream, "/Leading " + adata.Leading.ToString());
				StreamUtil.SWriteLine(FTempStream, "/CapHeight " + adata.CapHeight.ToString());
				StreamUtil.SWriteLine(FTempStream, "/StemV " + ((int)Math.Round(adata.StemV)).ToString());
				if (adata.AvgWidth != 0)
					StreamUtil.SWriteLine(FTempStream, "/AvgWidth " + adata.AvgWidth.ToString());
				StreamUtil.SWriteLine(FTempStream, "/MaxWidth " + adata.MaxWidth.ToString());
				StreamUtil.SWriteLine(FTempStream, "/FontStretch /Normal");
				if (adata.FontWeight > 0)
					StreamUtil.SWriteLine(FTempStream, "/FontWeight " + adata.FontWeight.ToString());
				if (adata.Embedded)
				{
					if (adata.Type1)
						StreamUtil.SWriteLine(FTempStream, "/FontFile " +
							adata.ObjectIndex.ToString() + " 0 R");
					else
						StreamUtil.SWriteLine(FTempStream, "/FontFile2 " +
							adata.ObjectIndex.ToString() + " 0 R");
				}
				StreamUtil.SWriteLine(FTempStream, ">>");
				StreamUtil.SWriteLine(FTempStream, "endobj");
				AddToOffset(FTempStream.Length);
				FTempStream.WriteTo(FMainPDF);

				// To unicode stream
				if (adata.IsUnicode)
				{
					// First Build the string
					StringBuilder cmaphead = new StringBuilder("/CIDInit /ProcSet findresource begin" + LINE_FEED +
						"12 dict begin " + LINE_FEED +
						"begincmap" + LINE_FEED +
						"/CIDSystemInfo" + LINE_FEED +
						"<< /Registry (TTX+0)" + LINE_FEED +
						"/Ordering (T42UV)" + LINE_FEED +
						"/Supplement 0" + LINE_FEED +
						">> def" + LINE_FEED +
						"/CMapName /TTX+0 def" + LINE_FEED +
						"/CMapType 2 def" + LINE_FEED +
						"1 begincodespacerange" + LINE_FEED +
						"<0000><FFFF>" + LINE_FEED +
						"endcodespacerange" + LINE_FEED);
					int currentindex = 0;
					int nsize = 0;
					while (currentindex < adata.CacheWidths.Count)
					{
						nsize = adata.CacheWidths.Count - currentindex;
						if (nsize <= 0)
							break;
						if (nsize > 100)
							nsize = 100;
						cmaphead.Append(nsize.ToString() +
							" beginbfchar" + LINE_FEED);
						for (int idx = 0; idx < nsize; idx++)
						{
							char nkey = adata.CacheWidths.Keys[currentindex + idx];
							int nvalue = adata.CacheWidths[nkey].GlyphIndex;

							string fromTo = "<" + PDFCanvas.IntToHex(nvalue) + "> ";
							cmaphead.Append(fromTo + " <" + PDFCanvas.IntToHex((int)nkey) + ">" + LINE_FEED);
						}
						cmaphead.Append("endbfchar" + LINE_FEED);
						currentindex = currentindex + nsize;
					}
					/*
										int currentindex = adata.FirstLoaded;
										int nextindex = adata.FirstLoaded;
										while (currentindex<=adata.LastLoaded)
										{
											index = currentindex;
											int aunicodecount = 0;
											while (index<=adata.LastLoaded)
											{
												nextindex = index;
												if (adata.Glyphs.IndexOfKey((char)index)>=0)
												{
													aunicodecount++;
													if (aunicodecount>=2)
														break;
												}
												index++;
											}
											if (aunicodecount>0)
											{
												cmaphead.Append(aunicodecount.ToString()+
													" beginbfchar"+LINE_FEED);
												for (index=currentindex;index<=nextindex;index++)
												{
													if (adata.Glyphs.IndexOfKey((char)index)>=0)
													{
														string fromTo = "<"+ PDFCanvas.IntToHex((int)adata.Glyphs[(char)index])+"> ";
														cmaphead.Append(fromTo+" <"+PDFCanvas.IntToHex((int)index)+">"+LINE_FEED);
													}
												}
												cmaphead.Append("endbfchar" +LINE_FEED);
											}
											currentindex = nextindex+1;
										}*/
					cmaphead.Append("endcmap" + LINE_FEED +
						"CMapName currentdict /CMap defineresource pop" + LINE_FEED +
						"end end" + LINE_FEED);
					FObjectCount = FObjectCount + 1;
					adata.ToUnicodeIndex = FObjectCount;
					FTempStream.SetLength(0);
					StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
					StreamUtil.SWriteLine(FTempStream, "<< /Length " + cmaphead.Length.ToString());
#if REPMAN_ZLIB
					if (Compressed)
						StreamUtil.SWriteLine(FTempStream, "/Filter [/FlateDecode]");
#endif
					StreamUtil.SWriteLine(FTempStream, ">>");
					StreamUtil.SWriteLine(FTempStream, "stream");
#if REPMAN_ZLIB
					if (Compressed)
					{
						using (MemoryStream nstream = new MemoryStream())
						{
							string nstring = cmaphead.ToString();
							byte[] nbyte = StreamUtil.StringToByteArray(nstring, nstring.Length, false);
							nstream.Write(nbyte, 0, nbyte.Length);
							nstream.Seek(0, SeekOrigin.Begin);
							StreamUtil.CompressStream(nstream, FTempStream, OptimizeSize);
						}
					}
					else
#endif
						StreamUtil.SWriteLine(FTempStream, cmaphead.ToString());
					StreamUtil.SWriteLine(FTempStream, PDFCanvas.ENDSTREAM);
					StreamUtil.SWriteLine(FTempStream, "endobj");
					AddToOffset(FTempStream.Length);
					FTempStream.WriteTo(FMainPDF);
				}
			}
			// Creates the fonts of the font list
			for (i = 0; i < Canvas.FontData.Count; i++)
			{
				adata = (TTFontData)Canvas.FontData.GetByIndex(i);
				if (adata.IsUnicode)
				{
					FObjectCount = FObjectCount + 1;
					FTempStream.SetLength(0);
					adata.ObjectIndexParent = FObjectCount;
					StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
					StreamUtil.SWriteLine(FTempStream, "<< /Type /Font");
					StreamUtil.SWriteLine(FTempStream, "/Subtype /Type0");
					StreamUtil.SWriteLine(FTempStream, "/Name /F" + adata.ObjectName);
					StreamUtil.SWriteLine(FTempStream, "/BaseFont /" + CONS_UNICODEPREDIX + adata.PostcriptName);
					StreamUtil.SWriteLine(FTempStream, "/Encoding /Identity-H");
					StreamUtil.SWriteLine(FTempStream, "/DescendantFonts [ " + (FObjectCount + 1).ToString() + " 0 R ]");
					StreamUtil.SWriteLine(FTempStream, "/ToUnicode " +
								adata.ToUnicodeIndex.ToString() + " 0 R");

					StreamUtil.SWriteLine(FTempStream, ">>");
					StreamUtil.SWriteLine(FTempStream, "endobj");
					AddToOffset(FTempStream.Length);
					FTempStream.WriteTo(FMainPDF);

					FObjectCount = FObjectCount + 1;
					FTempStream.SetLength(0);
					StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
					StreamUtil.SWriteLine(FTempStream, "<< /Type /Font");
					if (adata.Type1)
						StreamUtil.SWriteLine(FTempStream, "/Subtype /CIDFontType1");
					else
						StreamUtil.SWriteLine(FTempStream, "/Subtype /CIDFontType2");
					StreamUtil.SWriteLine(FTempStream, "/BaseFont /" + CONS_UNICODEPREDIX + adata.PostcriptName);
					StreamUtil.SWriteLine(FTempStream, "/FontDescriptor " +
						adata.DescriptorIndex.ToString() + " 0 R");
					StreamUtil.SWriteLine(FTempStream, "/FontFamily(" + adata.FontFamily + ")");
					StreamUtil.SWriteLine(FTempStream, "/CIDSystemInfo<</Ordering(Identity)/Registry(Adobe)/Supplement 0>>");
					StreamUtil.SWriteLine(FTempStream, "/DW 1000");
					StreamUtil.SWriteLine(FTempStream, "/W [");
					awidths = "";
					/*					index = adata.FirstLoaded;
										acount = 0;
										do
										{
											if (adata.Glyphs.IndexOfKey((char)index) >= 0)
											{
												awidths = awidths + adata.Glyphs[(char)index].ToString() + "[" + adata.Widths[(char)index].ToString() + "] ";
												acount = acount + 1;
												if ((acount % 8) == 7)
													awidths = awidths + LINE_FEED;
											}
											index++;
										}
										while (index <= adata.LastLoaded);
					 */
					index = 0;
					acount = 0;
					while (index < adata.CacheWidths.Count)
					{
						char nkey = adata.CacheWidths.Keys[index];
						int nvalue = adata.CacheWidths[nkey].GlyphIndex;
						int nwidth = adata.CacheWidths[nkey].Width;
						awidths = awidths + nvalue.ToString() + "[" + nwidth.ToString() + "] ";
						acount++;
						if ((acount % 8) == 7)
							awidths = awidths + LINE_FEED;

						index++;
					}
					StreamUtil.SWriteLine(FTempStream, awidths);
					StreamUtil.SWriteLine(FTempStream, "]");
					StreamUtil.SWriteLine(FTempStream, "/CDIToGDIMap /Identity");

					StreamUtil.SWriteLine(FTempStream, ">>");
					StreamUtil.SWriteLine(FTempStream, "endobj");
					AddToOffset(FTempStream.Length);
					FTempStream.WriteTo(FMainPDF);
				}
				else
				{
					FObjectCount = FObjectCount + 1;
					FTempStream.SetLength(0);
					adata.ObjectIndexParent = FObjectCount;
					StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
					StreamUtil.SWriteLine(FTempStream, "<< /Type /Font");
					if (adata.Type1)
						StreamUtil.SWriteLine(FTempStream, "/Subtype /Type1");
					else
						StreamUtil.SWriteLine(FTempStream, "/Subtype /TrueType");
					StreamUtil.SWriteLine(FTempStream, "/Name /F" + adata.ObjectName);
					StreamUtil.SWriteLine(FTempStream, "/BaseFont /" + adata.PostcriptName);
					StreamUtil.SWriteLine(FTempStream, "/FirstChar " + adata.FirstLoaded.ToString());
					StreamUtil.SWriteLine(FTempStream, "/LastChar " + adata.LastLoaded.ToString());
					awidths = "[";
					if (adata.LastLoaded > 0)
					{
						index = adata.FirstLoaded;
						do
						{
							if (adata.Widths.IndexOfKey((char)index) >= 0)
								awidths = awidths + adata.Widths[(char)index].ToString() + " ";
							else
								awidths = awidths + "0 ";
							index++;
							if ((index % 8) == 7)
								awidths = awidths + LINE_FEED;
						}
						while (index <= adata.LastLoaded);
						awidths = awidths + "]";
						StreamUtil.SWriteLine(FTempStream, "/Widths " + awidths);
					}
					StreamUtil.SWriteLine(FTempStream, "/FontDescriptor " +
						adata.DescriptorIndex.ToString() + " 0 R");
					StreamUtil.SWriteLine(FTempStream, "/Encoding /" + adata.Encoding);
					StreamUtil.SWriteLine(FTempStream, ">>");
					StreamUtil.SWriteLine(FTempStream, "endobj");
					AddToOffset(FTempStream.Length);
					FTempStream.WriteTo(FMainPDF);
				}
			}
		}
		void StartStream()
		{
			// Starting of the stream
			FObjectCount = FObjectCount + 1;
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
			StreamUtil.SWriteLine(FTempStream, "<< /Length " + (FObjectCount + 1).ToString() + " 0 R");
#if REPMAN_ZLIB
			if (Compressed)
				StreamUtil.SWriteLine(FTempStream, "/Filter [/FlateDecode]");
#endif
			StreamUtil.SWriteLine(FTempStream, " >>");
			FStreamSize1 = FTempStream.Length;
			StreamUtil.SWriteLine(FTempStream, "stream");
			FSTempStream.SetLength(0);
		}
		void EndStream()
		{
			FSTempStream.Seek(0, System.IO.SeekOrigin.Begin);
			long TempSize;
#if REPMAN_ZLIB
			if (Compressed)
			{
				StreamUtil.CompressStream(FSTempStream, FTempStream, OptimizeSize);
			}
			else
#endif
				FSTempStream.WriteTo(FTempStream);

			FSTempStream.SetLength(0);

			StreamUtil.SWriteLine(FTempStream, PDFCanvas.ENDSTREAM);
			StreamUtil.SWriteLine(FTempStream, "endobj");
			FStreamSize2 = 6;
			AddToOffset(FTempStream.Length);
			FTempStream.WriteTo(FMainPDF);

			TempSize = FTempStream.Length - FStreamSize1 - FStreamSize2 - "Stream".Length - PDFCanvas.ENDSTREAM.Length - 6;
			FObjectCount = FObjectCount + 1;
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
			StreamUtil.SWriteLine(FTempStream, TempSize.ToString());
			StreamUtil.SWriteLine(FTempStream, "endobj");
			AddToOffset(FTempStream.Length);
			FTempStream.WriteTo(FMainPDF);
		}

		public void BeginDoc()
		{
			if (FileName.Length == 0)
			{
				FMainPDF = new MemoryStream();
			}
			else
			{
				FMainPDF = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite);
			}

			const string PDF_HEADER = "%PDF-1.4";
			PageInfo aobj;

			FPageInfos.Clear();
			ImageIndexes.Clear();

			aobj = new PageInfo();
			aobj.PageWidth = PageWidth;
			aobj.PageHeight = PageHeight;
			FPageInfos.Add(aobj);

			FBitmapStreams.Clear();
			FPrinting = true;
			FMainPDF.SetLength(0);
			FObjectOffsets.Clear();
			FObjectCount = 0;
			FObjectOffset = 0;
			FPages.Clear();
			FFontList.Clear();
			FFontCount = 0;
			FCurrentSetPageObject = 0;
			ImageCount = 0;
			FPage = 1;
			// Writes the header
			StreamUtil.SWriteLine(FMainPDF, PDF_HEADER);
			AddToOffset(PDF_HEADER.Length);
			// Writes Doc info
			FObjectCount = FObjectCount + 1;
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
			StreamUtil.SWriteLine(FTempStream, "<<");
			StreamUtil.SWriteLine(FTempStream, "/Producer (" + DocProducer + ")");
			StreamUtil.SWriteLine(FTempStream, "/Author (" + DocAuthor + ")");
			//			SWriteLine(FTempStream,"/CreationDate (D:"+FormatDateTime('YYYYMMDDHHmmSS',now)+')');
			StreamUtil.SWriteLine(FTempStream, "/Creator (" + DocCreator + ")");
			StreamUtil.SWriteLine(FTempStream, "/Keywords (" + DocKeywords + ")");
			StreamUtil.SWriteLine(FTempStream, "/Subject (" + DocSubject + ")");
			StreamUtil.SWriteLine(FTempStream, "/Title (" + DocTitle + ")");
			StreamUtil.SWriteLine(FTempStream, "/ModDate ()");
			StreamUtil.SWriteLine(FTempStream, ">>");
			StreamUtil.SWriteLine(FTempStream, "endobj");
			AddToOffset(FTempStream.Length);
			FTempStream.WriteTo(FMainPDF);
			StartStream();
		}
		public void NewPage(int NPageWidth, int NPageHeight)
		{
			long TempSize;
			PageInfo aobj;

			CheckPrinting();

			PageWidth = NPageWidth;
			PageHeight = NPageHeight;
			aobj = new PageInfo();
			aobj.PageWidth = NPageWidth;
			aobj.PageHeight = NPageHeight;
			FPageInfos.Add(aobj);

			FPage = FPage + 1;

			FSTempStream.Seek(0, System.IO.SeekOrigin.Begin);
#if REPMAN_ZLIB
			if (Compressed)
			{
				StreamUtil.CompressStream(FSTempStream, FTempStream, OptimizeSize);
			}
			else
#endif
				FSTempStream.WriteTo(FTempStream);

			FSTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, PDFCanvas.ENDSTREAM);
			StreamUtil.SWriteLine(FTempStream, "endobj");
			FStreamSize2 = 6;
			AddToOffset(FTempStream.Length);
			FTempStream.WriteTo(FMainPDF);
			TempSize = FTempStream.Length - FStreamSize1 - FStreamSize2 - "Stream".Length - PDFCanvas.ENDSTREAM.Length - 6;
			FObjectCount = FObjectCount + 1;
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
			StreamUtil.SWriteLine(FTempStream, TempSize.ToString());
			StreamUtil.SWriteLine(FTempStream, "endobj");
			AddToOffset(FTempStream.Length);
			FTempStream.WriteTo(FMainPDF);

			FObjectCount = FObjectCount + 1;
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, (FObjectCount).ToString() + " 0 obj");
			StreamUtil.SWriteLine(FTempStream, "<< /Length " + (FObjectCount + 1).ToString() + " 0 R");
#if REPMAN_ZLIB
			if (Compressed)
				StreamUtil.SWriteLine(FTempStream, "/Filter [/FlateDecode]");
#endif
			StreamUtil.SWriteLine(FTempStream, " >>");

			FStreamSize1 = FTempStream.Length;
			StreamUtil.SWriteLine(FTempStream, "stream");
		}
		void SetArray()
		{
			int i;
			TTFontData adata;
			FObjectCount = FObjectCount + 1;
			FResourceNum = FObjectCount;
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
			StreamUtil.SWriteLine(FTempStream, "<< /ProcSet [ /PDF /Text /ImageC]");
			StreamUtil.SWriteLine(FTempStream, "/XObject << ");
			for (i = 1; i <= ImageCount; i++)
			{
				if (!Masks.ContainsKey(i - 1))
				{
					StreamUtil.SWriteLine(FTempStream, "/Im" + i.ToString() + " " + (FObjectCount + i).ToString() + " 0 R");
				}
			}
			StreamUtil.SWriteLine(FTempStream, ">>");
			StreamUtil.SWriteLine(FTempStream, "/Font << ");

			for (i = 1; i <= FFontCount; i++)
				StreamUtil.SWriteLine(FTempStream, "/F" + i.ToString() + " " + FFontList[i - 1] + " 0 R ");
			for (i = 0; i < Canvas.FontData.Count; i++)
			{
				adata = (TTFontData)Canvas.FontData.GetByIndex(i);
				StreamUtil.SWriteLine(FTempStream, "/F" + adata.ObjectName +
					" " + adata.ObjectIndexParent.ToString() + " 0 R ");
			}
			StreamUtil.SWriteLine(FTempStream, ">>");
			StreamUtil.SWriteLine(FTempStream, ">>");
			StreamUtil.SWriteLine(FTempStream, "endobj");
			AddToOffset(FTempStream.Length);
			FTempStream.WriteTo(FMainPDF);
		}
		private void WaitCompressionTasks()
		{
			if (CompressionTasks.Count == 0)
				return;
			List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
			foreach (var comp in CompressionTasks)
			{
				tasks.Add(comp.Task);
				comp.Task.ConfigureAwait(false);
			}
			System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
			foreach (var comp in CompressionTasks)
			{
				if (comp.Task.Exception != null)
				{
					throw new Exception("Error compressing image: " + comp.Task.Exception.Message);
				}
				string stringLength = comp.Task.Result.CompressedBytes.ToString();
				byte[] bytesLength = ASCIIEncoding.ASCII.GetBytes(stringLength.ToString());
				comp.PositionStream.Seek(comp.StreamPosition, SeekOrigin.Begin);
				comp.PositionStream.Write(bytesLength, 0, bytesLength.Length);
				comp.PositionStream.Seek(0, SeekOrigin.Begin);
			}
		}
		public void EndDoc()
		{
			int i;
			CheckPrinting();
			FPrinting = false;
			// Writes the trailing zone
			EndStream();
			SetOutLine();
			SetFontType();
			SetPages();
			SetArray();
			// Wait for tasks
			WaitCompressionTasks();
			for (i = 1; i <= ImageCount; i++)
			{
				WriteBitmap(i);
			}
			for (i = 1; i <= FPage; i++)
			{
				SetPageObject(i);
			}
			SetCatalog();
			SetXref();
			StreamUtil.SWriteLine(FMainPDF, "%%EOF");

			// Save to disk if filename assigned
			if (FMainPDF is FileStream)
			{
				((System.IO.FileStream)FMainPDF).Close();
				FMainPDF = null;
			}
			else
				FMainPDF.Seek(0, System.IO.SeekOrigin.Begin);
			FBitmapStreams.Clear();
		}
		void SetCatalog()
		{
			FObjectCount = FObjectCount + 1;
			FCatalogNum = FObjectCount;
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
			StreamUtil.SWriteLine(FTempStream, "<< /Type /Catalog");
			StreamUtil.SWriteLine(FTempStream, "/Pages " + FParentNum.ToString() + " 0 R");
			StreamUtil.SWriteLine(FTempStream, "/Outlines " + FOutlinesNum.ToString() + " 0 R");
			StreamUtil.SWriteLine(FTempStream, ">>");
			StreamUtil.SWriteLine(FTempStream, "endobj");
			AddToOffset(FTempStream.Length);
			FTempStream.WriteTo(FMainPDF);
		}
		string GetOffsetNumber(string offset)
		{
			long x, y;
			x = offset.Length;
			string aresult = "";
			for (y = 1; y <= 10 - x; y++)
				aresult = aresult + "0";
			aresult = aresult + offset;
			return aresult;
		}
		void SetXref()
		{
			int i;
			FObjectCount = FObjectCount + 1;
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, "xref");
			StreamUtil.SWriteLine(FTempStream, "0 " + FObjectCount.ToString());
			StreamUtil.SWriteLine(FTempStream, "0000000000 65535 f");

			for (i = 0; i <= FObjectCount - 2; i++)
				StreamUtil.SWriteLine(FTempStream, GetOffsetNumber(FObjectOffsets[i].ToString()) + " 00000 n");

			StreamUtil.SWriteLine(FTempStream, "trailer");
			StreamUtil.SWriteLine(FTempStream, "<< /Size " + FObjectCount.ToString());
			StreamUtil.SWriteLine(FTempStream, "/Root " + FCatalogNum.ToString() + " 0 R");
			StreamUtil.SWriteLine(FTempStream, "/Info 1 0 R");
			StreamUtil.SWriteLine(FTempStream, ">>");
			StreamUtil.SWriteLine(FTempStream, "startxref");
			StreamUtil.SWriteLine(FTempStream, FMainPDF.Length.ToString());
			FTempStream.WriteTo(FMainPDF);
		}
		void SetPageObject(int index)
		{
			PageInfo aobj;

			aobj = FPageInfos[index - 1];

			FObjectCount = FObjectCount + 1;
			FTempStream.SetLength(0);
			StreamUtil.SWriteLine(FTempStream, FObjectCount.ToString() + " 0 obj");
			StreamUtil.SWriteLine(FTempStream, "<< /Type /Page");
			StreamUtil.SWriteLine(FTempStream, "/Parent " + FParentNum.ToString() + " 0 R");
			StreamUtil.SWriteLine(FTempStream, "/MediaBox [ 0 0 " +
				FCanvas.UnitsToTextX(aobj.PageWidth) + " " + FCanvas.UnitsToTextX(aobj.PageHeight) + "]");
			StreamUtil.SWriteLine(FTempStream, "/Contents " + FPages[FCurrentSetPageObject] + " 0 R");
			StreamUtil.SWriteLine(FTempStream, "/Resources " + FResourceNum.ToString() + " 0 R");
			StreamUtil.SWriteLine(FTempStream, ">>");
			StreamUtil.SWriteLine(FTempStream, "endobj");
			AddToOffset(FTempStream.Length);
			FTempStream.WriteTo(FMainPDF);
			FCurrentSetPageObject = FCurrentSetPageObject + 1;
		}

		void WriteBitmap(int index)
		{
			if (!CalculateOnly)
			{
				FObjectCount = FObjectCount + 1;
				FTempStream.SetLength(0);
				string resIndex = FObjectCount.ToString();
				StreamUtil.SWriteLine(FTempStream, resIndex + " 0 obj");
				if (Masks.ContainsKey(index))
				{
					string resIndex2 = (FObjectCount + 1).ToString();
					byte[] bytesIndex = ASCIIEncoding.ASCII.GetBytes(resIndex2);
					var streamPos = Masks[index];
					streamPos.Stream.Seek(streamPos.Position, SeekOrigin.Begin);
					streamPos.Stream.Write(bytesIndex, 0, bytesIndex.Length);
					streamPos.Stream.Seek(0, SeekOrigin.Begin);
				}
				FBitmapStreams[index - 1].WriteTo(FTempStream);
				StreamUtil.SWriteLine(FTempStream, PDFCanvas.ENDSTREAM);
				StreamUtil.SWriteLine(FTempStream, "endobj");
				AddToOffset(FTempStream.Length);
				FTempStream.WriteTo(FMainPDF);
			}
		}
	}
	public class StreamPosition
	{
		public Stream Stream;
		public long Position;
		public StreamPosition(System.IO.Stream astream, long aposition)
		{
			Stream = astream;
			Position = aposition;
		}
	}
	public class CompressionTask
	{
		public CompressionTask(System.Threading.Tasks.Task<TaskCompressResult> nTask, CancellationTokenSource nCancelSource, Stream nPositionStream, long nStreamPosition)
		{
			Task = nTask;
			CancelSource = nCancelSource;
			PositionStream = nPositionStream;
			StreamPosition = nStreamPosition;
		}
		public System.Threading.Tasks.Task<TaskCompressResult> Task;
		public CancellationTokenSource CancelSource;
		public Stream PositionStream;
		public long StreamPosition;
		public void CancelTask()
		{
			if (CancelSource != null)
			{
				CancelSource.Cancel();
			}
		}
	}
}
