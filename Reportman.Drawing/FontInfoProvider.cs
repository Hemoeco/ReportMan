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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Reportman.Drawing
{
	public struct GlyphInfo
	{
		public int Width;
		public int GlyphIndex;
	}
	public class PDFFont
	{
		public PDFFontType Name;
		public string WFontName;
		public string LFontName;
		public string FontName;
		public int Size;
		public int Color;
		public int Style;
		public bool Italic;
		public bool Underline;
		public bool Bold;
		public bool StrikeOut;
		public bool Transparent;
		public int BackColor;
		public PDFFont()
		{
			Name = PDFFontType.Courier;
			Size = 10;
			BackColor = 0xFFFFFF;
		}
	}
	public class AdvFontData
	{
		public byte[] Data;
		public uint DirectoryOffset;
	}
	public class TTFontData
	{
		public bool Embedded;
		public AdvFontData FontData;
		public string PostcriptName;
		public string Encoding;
		public int Ascent, Descent, Leading, CapHeight, Flags, FontWeight;
		public int LineSpacing;
		public int EmHeight;
		public int MaxWidth, AvgWidth;
		public double StemV;
		public string FontFamily, FontStretch;
		public double ItalicAngle;
		public Rectangle FontBBox;
		public string FaceName;
		public string StyleName;
		public bool Type1;
		public bool HaveKerning;
		public string ObjectName;
		public long ObjectIndex;
		public long ObjectIndexParent;
		public long DescriptorIndex;
		public long ToUnicodeIndex;
		public int FirstLoaded;
		public int LastLoaded;
		public bool IsUnicode;
		public SortedList<char, int> Glyphs;
		public SortedList<char, int> Widths;
		public SortedList<ulong, int> Kernings;
		public static SortedList<string, AdvFontData> FontDatas;
		public SortedList<char, GlyphInfo> CacheWidths;
		public bool IsBold;
		public bool IsItalic;

		public LogFontFt LogFont;

		public TTFontData()
		{
			Flags = 32;
			FontWeight = 0;
			MaxWidth = 0;
			AvgWidth = 0;
			StemV = 0;
			ItalicAngle = 0;
			FaceName = "";
			StyleName = "";
			CapHeight = 0;
			FirstLoaded = 65536;
			LastLoaded = -1;
			Widths = new SortedList<char, int>();
			Kernings = new SortedList<ulong, int>();
			Glyphs = new SortedList<char, int>();
		}
	}
	public abstract class FontInfoProvider
	{
		public abstract void FillFontData(PDFFont pdfFont, TTFontData fontData);
		public abstract int GetCharWidth(PDFFont pdfFont, TTFontData fontData,
				 char charCode);
		public abstract int GetKerning(PDFFont pdfFont, TTFontData fontData,
				 char leftChar, char rightChar);
		public abstract MemoryStream GetFontStream(TTFontData data);
	}
}
