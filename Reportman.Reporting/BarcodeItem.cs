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
	public enum BarcodeType
	{
		Code_2_5_interleaved,
		Code_2_5_industrial, Code_2_5_matrix, Code39, Code39Extended, Code128A,
		Code128B, Code128C, Code128, Code93, Code93Extended, CodeMSI, CodePostNet,
		CodeCodabar, CodeEAN8, CodeEAN13, CodePDF417, CodeQR
	};
	public class BarcodeItem : PrintPosItem
	{
		private int[] modules = { 0, 1, 2, 3 };
		private const int DEF_DRAWWIDTH = 500;
		private bool FUpdated;
		Variant FValue;
		public string Expression;
		public int Modul;
		public double Ratio;
		public BarcodeType BarType;
		public bool Checksum;
		public string DisplayFormat;
		public short Rotation;
		public int BColor;
		public int BackColor;
		public bool Transparent;
		// PDF417
		public int NumColumns;
		public int NumRows;
		public int ECCLevel;
		public bool Truncated;
		public string CurrentText;
		public BarcodeItem(BaseReport rp) : base(rp)
		{
			Height = DEF_DRAWWIDTH;
			Width = Height;
			Ratio = 2.0;
			Modul = 10;
			BarType = BarcodeType.CodeEAN13;
			Expression = "'5449000000996'";
			ECCLevel = -1;
			DisplayFormat = "";
			BackColor = GraphicUtils.IntegerFromColor(Color.White);
		}
		protected override string GetClassName()
		{
			return "TRPBARCODE";
		}
		private void Evaluate()
		{
			if (FUpdated)
				return;
			Evaluator fevaluator;
			try
			{
				fevaluator = Report.Evaluator;
				FValue = fevaluator.EvaluateText(Expression);
				FUpdated = true;
			}
			catch (Exception E)
			{
				throw new ReportException(E.Message + ":Barcode", this, "Expression");
			}
		}
		public override void SubReportChanged(SubReportEvent newstate, string newgroup)
		{
			base.SubReportChanged(newstate, newgroup);
			FUpdated = false;
		}
		private string GetText()
		{
			string expre;
			string aresult;
			expre = Expression.Trim();
			if (expre.Length == 0)
				return "";

			try
			{
				Evaluate();
				aresult = FValue.ToString(DisplayFormat, ParamType.Unknown, true);
			}
			catch (Exception E)
			{
				throw new ReportException(E.Message + ":Barcode",
					this, "Expression");
			}
			return aresult;
		}
		override protected void DoPrint(PrintOut adriver, int aposx, int aposy,
			int newwidth, int newheight, MetaFile metafile, Point MaxExtent,
			ref bool PartialPrint)
		{
			string data;
			base.DoPrint(adriver, aposx, aposy, newwidth, newheight,
				metafile, MaxExtent, ref PartialPrint);
			CurrentText = GetText();
			try
			{
#if OMIT_ZXING
#else
				if (BarType == BarcodeType.CodeQR)
				{
					ZXing.QrCode.QRCodeWriter qrCode = new ZXing.QrCode.QRCodeWriter();
					int widthPixels = 0;
					int heightPixels = 0;
					ZXing.Common.BitMatrix qrResult = qrCode.encode(CurrentText, ZXing.BarcodeFormat.QR_CODE, widthPixels, heightPixels);
					int barcodeHeight = qrResult.Height;
					int barcodeWidth = qrResult.Width;
					int squareWidth = Width / barcodeWidth;
					int squareHeight = Height / barcodeHeight;
					// Center barcode in rectangle
					if (squareWidth > squareHeight)
					{
						int dif = squareWidth - squareHeight;
						aposx = aposx + (dif * barcodeWidth) / 2;
						squareWidth = squareHeight;
					}
					else
					{
						int dif = squareHeight - squareWidth;
						aposy = aposy + (dif * barcodeHeight) / 2;
						squareHeight = squareWidth;
					}
					Point origin = new Point(aposx + squareWidth * barcodeWidth / 2, aposy + squareHeight * barcodeHeight / 2);
					for (int rowIndex = 0; rowIndex < barcodeHeight; rowIndex++)
					{
						ZXing.Common.BitArray bits = qrResult.getRow(rowIndex, null);
						for (int column = 0; column < bits.Size; column++)
						{
							int xadd = squareWidth * column - squareWidth * barcodeWidth / 2;
							int yadd = squareHeight * rowIndex - squareHeight * barcodeHeight / 2;
							Point a = new Point(xadd, yadd);
							Point b = new Point(xadd, yadd + squareHeight);
							Point c = new Point(xadd + squareWidth, yadd + squareHeight);
							Point d = new Point(xadd + squareWidth, yadd);
							// a,b,c,d builds the rectangle we want to draw

							if (Rotation != 0)
							{
								double alpha = (double)Rotation / 10 * Math.PI / 180.0;
								a = Translate2D(Rotate2D(a, alpha), origin);
								b = Translate2D(Rotate2D(b, alpha), origin);
								c = Translate2D(Rotate2D(c, alpha), origin);
								d = Translate2D(Rotate2D(d, alpha), origin);
								int left = Math.Min(Math.Min(a.X, b.X), Math.Min(c.X, d.X));
								int top = Math.Min(Math.Min(a.Y, b.Y), Math.Min(c.Y, d.Y));
								int right = Math.Max(Math.Max(a.X, b.X), Math.Max(c.X, d.X));
								int bottom = Math.Max(Math.Max(a.Y, b.Y), Math.Max(c.Y, d.Y));
								a = new Point(left, top);
								c = new Point(right, bottom);
							}
							else
							{
								a = Translate2D(a, origin);
								b = Translate2D(b, origin);
								c = Translate2D(c, origin);
								d = Translate2D(d, origin);
							}


							bool drawElement = true;


							int PenColor;
							bool bitValue = bits[column];
							if (bitValue)
								PenColor = BColor;
							else
							{
								// PenColor = 0xFFFFFF;
								PenColor = BackColor;
								if (Transparent)
								{
									drawElement = false;
								}
							}
							if (drawElement)
							{
								MetaObjectDraw metaobj = new MetaObjectDraw();
								metaobj.MetaType = MetaObjectType.Draw;

								metaobj.Top = a.Y;
								metaobj.Left = a.X;
								metaobj.Width = c.X - a.X;
								metaobj.Height = c.Y - a.Y;

								metaobj.DrawStyle = ShapeType.Rectangle;
								metaobj.BrushStyle = (int)BrushType.Solid;
								metaobj.PenStyle = (int)PenType.Solid;
								metaobj.PenWidth = 0;
								metaobj.PenColor = PenColor;
								metaobj.BrushColor = PenColor;
								metafile.Pages[metafile.CurrentPage].Objects.Add(metaobj);
							}
						}
						aposy = aposy + squareHeight;
					}


				}

				else
				{
					data = CalculateBarcode();
					// draw the barcode
					DoLines(data, aposx, aposy, metafile);
				}
#endif
			}
			catch (Exception E)
			{
				throw new ReportException(E.Message + ":" + Translator.TranslateStr(573),
					this, "Barcode");
			}
		}
#if NETSTANDARD2_0
#if OMIT_ZXING
#else
		public static ZXing.Result DetectBarcode(byte[] nbitmap)
		{
			System.Collections.Generic.List<ZXing.BarcodeFormat> Fmts =
			   new System.Collections.Generic.List<ZXing.BarcodeFormat> { ZXing.BarcodeFormat.All_1D };


			ZXing.BarcodeReader reader = new ZXing.BarcodeReader
			{
				AutoRotate = true,
				TryInverted = true,
				Options =
				{
                    //PossibleFormats = Fmts,
                    TryHarder = true,
                    //PureBarcode = false
                }
			};
			var resultado = reader.Decode(nbitmap);
			return resultado;
		}
		public static ZXing.Result DetectBarcode(string fileName)
		{
			byte[] content;

			using (var mstream = StreamUtil.FileToMemoryStream(fileName))
			{
				content = mstream.ToArray();
			}
			return DetectBarcode(content);
		}
#endif
#else

		public static ZXing.Result DetectBarcode(Bitmap nbitmap)
		{
			System.Collections.Generic.List<ZXing.BarcodeFormat> Fmts =
			   new System.Collections.Generic.List<ZXing.BarcodeFormat> { ZXing.BarcodeFormat.All_1D };


			ZXing.BarcodeReader reader = new ZXing.BarcodeReader
			{
				AutoRotate = true,
				TryInverted = true,
				Options =
				{
                    //PossibleFormats = Fmts,
                    TryHarder = true,
                    //PureBarcode = false
                }
			};
			var resultado = reader.Decode(nbitmap);
			return resultado;
		}
		public static ZXing.Result DetectBarcode(string fileName)
		{
			using (Bitmap nbitmap = new Bitmap(fileName))
			{
				return DetectBarcode(nbitmap);
			}
		}
#endif
		public string CalculateBarcode()
		{
			string data;
			data = "";
			if (BarType == BarcodeType.CodePDF417)
			{
				GenerateCodeWords();
				return CurrentText;
			}
			// Calculate the with of lines
			MakeModules();
			switch (BarType)
			{
				case BarcodeType.Code_2_5_interleaved:
					data = Code_2_5_interleaved();
					break;
				case BarcodeType.Code_2_5_industrial:
					data = Code_2_5_industrial();
					break;
				case BarcodeType.Code_2_5_matrix:
					data = Code_2_5_matrix();
					break;
				case BarcodeType.Code39:
					data = Code_39();
					break;
				case BarcodeType.Code39Extended:
					data = Code_39Extended();
					break;
				case BarcodeType.Code128A:
				case BarcodeType.Code128B:
				case BarcodeType.Code128C:
				case BarcodeType.Code128:
					data = Code_128();
					break;
				case BarcodeType.Code93:
					data = Code_93();
					break;
				case BarcodeType.Code93Extended:
					data = Code_93Extended();
					break;
				case BarcodeType.CodeMSI:
					data = Code_MSI();
					break;
				case BarcodeType.CodePostNet:
					data = Code_PostNet();
					break;
				case BarcodeType.CodeCodabar:
					data = Code_Codabar();
					break;
				case BarcodeType.CodeEAN8:
					data = Code_EAN8();
					break;
				case BarcodeType.CodeEAN13:
					data = Code_EAN13();
					break;
			}
			return data;
		}
		void MakeModules()
		{
			switch (BarType)
			{
				case BarcodeType.Code_2_5_interleaved:
				case BarcodeType.Code_2_5_industrial:
				case BarcodeType.Code39:
				case BarcodeType.Code39Extended:
				case BarcodeType.CodeEAN8:
				case BarcodeType.CodeEAN13:
					if (Ratio < 2.0)
						Ratio = 2.0;
					if (Ratio > 3.0)
						Ratio = 3.0;
					break;
				case BarcodeType.Code_2_5_matrix:
					if (Ratio < 2.25)
						Ratio = 2.25;
					if (Ratio > 3.25)
						Ratio = 3.25;
					break;
			}
			modules[0] = Modul;
			modules[1] = (int)Math.Round(Modul * Ratio);
			modules[2] = (modules[1] * 3) / 2;
			modules[3] = modules[1] * 2;
		}
		private void GenerateCodeWords()
		{

		}
		private string GetEAN(string nr)
		{
			int i, fak, sum;
			string tmp;
			sum = 0;
			string result;
			tmp = nr.Substring(0, nr.Length - 1);
			fak = tmp.Length;
			for (i = 0; i < tmp.Length; i++)
			{
				if ((fak % 2) == 0)
					sum = sum + System.Convert.ToInt32("" + tmp[i]) * 1;
				else
					sum = sum + System.Convert.ToInt32("" + tmp[i]) * 3;
				fak--;
			}
			if ((sum % 10) == 0)
				result = tmp + "0";
			else
				result = tmp + (10 - (sum % 10)).ToString();
			return result;
		}
		private string Code_128()
		{
			int achecksum = 0;
			string startcode = "";
			string aresult = "";
			int i = 0;
			string cadc;
			int idx;
			BarcodeType btyp;
			string text = CurrentText;
			while (i < text.Length)
			{
				btyp = BarType;
				if (btyp == BarcodeType.Code128)
					btyp = Code128.FindNewType(text, i);
				switch (btyp)
				{
					case BarcodeType.Code128A:
						achecksum = 103;
						startcode = Code128.StartA;
						break;
					case BarcodeType.Code128B:
						achecksum = 104;
						startcode = Code128.StartB;
						break;
					case BarcodeType.Code128C:
						achecksum = 105;
						startcode = Code128.StartC;
						break;
				}
				// Start code
				if (i == 0)
					aresult = aresult + DoConvert(startcode);
				// Look for EAN control
				int idxC = 1;

				if (text[i] == (char)0xBF)
				{
					aresult = aresult + DoConvert("411131");
					achecksum = achecksum + 102 * (i + 1);
					i++;
					idxC++;
					if (i >= text.Length)
						break;
				}
				switch (btyp)
				{
					case BarcodeType.Code128A:
					case BarcodeType.Code128B:
						while (i < text.Length)
						{
							idx = Code128.FindCodeAB(btyp, text[i]);
							if (idx < 0)
								idx = Code128.FindCodeAB(btyp, ' ');
							aresult = aresult + DoConvert(Code128.table[idx].data);
							achecksum = achecksum + idx * (i + 1);
							i++;
							// Look for EAN control
							if (i < text.Length)
								if (text[i] == (char)0xBF)
								{
									aresult = aresult + DoConvert("411131");
									achecksum = achecksum + 102 * (i + 1);
									i++;
									continue;
								}
						}
						break;
					case BarcodeType.Code128C:
						cadc = "";
						while (i < text.Length)
						{
							cadc = cadc + text[i];
							if (cadc.Length > 1)
							{
								idx = Code128.FindCodeC(cadc);
								if (idx < 0)
								{
									idx = Code128.FindCodeC("00");
								}
								achecksum = achecksum + idx * idxC;
								idxC++;
								aresult = aresult + DoConvert(Code128.table[idx].data);
								cadc = "";
							}
							i++;
							// Look for EAN control
							if (i < text.Length)
								if (text[i] == (char)0xBF)
								{
									aresult = aresult + DoConvert("411131");
									achecksum = achecksum + 102 * idxC;
									idxC++;
									i++;
									continue;
								}
						}
						break;
				}

			}
			achecksum = achecksum % 103;
			aresult = aresult + DoConvert(Code128.table[achecksum].data);
			aresult = aresult + DoConvert(Code128.Stop);
			return aresult;
		}
		private string Code_MSI()
		{
			return "";
		}
		private string Code_PostNet()
		{
			return "";
		}
		private string Code_Codabar()
		{
			return "";
		}
		private static char[,] tabelle_ParityEAN13 = new char[10, 6] {
		{'A', 'A', 'A', 'A', 'A', 'A'},    // 0
	      {'A', 'A', 'B', 'A', 'B', 'B'},    // 1
	      {'A', 'A', 'B', 'B', 'A', 'B'},    // 2
	      {'A', 'A', 'B', 'B', 'B', 'A'},    // 3
	      {'A', 'B', 'A', 'A', 'B', 'B'},    // 4
	      {'A', 'B', 'B', 'A', 'A', 'B'},    // 5
	      {'A', 'B', 'B', 'B', 'A', 'A'},    // 6
	      {'A', 'B', 'A', 'B', 'A', 'B'},    // 7
	      {'A', 'B', 'A', 'B', 'B', 'A'},    // 8
	      {'A', 'B', 'B', 'A', 'B', 'A'}     // 9
	   };
		// Pattern for Barcode EAN A
		//       L1   S1   L2   S2
		private static char[,] tabelle_EAN_A = new char[10, 4] {
	   {'2', '6', '0', '5'},    // 0
	   {'1', '6', '1', '5'},    // 1
	   {'1', '5', '1', '6'},    // 2
	   {'0', '8', '0', '5'},    // 3
	   {'0', '5', '2', '6'},    // 4
	   {'0', '6', '2', '5'},    // 5
	   {'0', '5', '0', '8'},    // 6
	   {'0', '7', '0', '6'},    // 7
	   {'0', '6', '0', '7'},    // 8
	   {'2', '5', '0', '6'}     // 9
	    };
		// Pattern for Barcode EAN B
		//       L1   S1   L2   S2
		private static char[,] tabelle_EAN_B = new char[10, 4] {
	   {'0', '5', '1', '7'},    // 0
	   {'0', '6', '1', '6'},    // 1
	   {'1', '6', '0', '6'},    // 2
	   {'0', '5', '3', '5'},    // 3
	   {'1', '7', '0', '5'},    // 4
	   {'0', '7', '1', '5'},    // 5
	   {'3', '5', '0', '5'},    // 6
	   {'1', '5', '2', '5'},    // 7
	   {'2', '5', '1', '5'},    // 8
	   {'1', '5', '0', '7'}     // 9
	    };
		// Pattern for Barcode EAN  C
		//       S1   L1   S2   L2
		private char[,] tabelle_EAN_C = new char[10, 4] {
	   {'7', '1', '5', '0' },    // 0
	   {'6', '1', '6', '0' },    // 1
	   {'6', '0', '6', '1' },    // 2
	   {'5', '3', '5', '0' },    // 3
	   {'5', '0', '7', '1' },    // 4
	   {'5', '1', '7', '0' },    // 5
	   {'5', '0', '5', '3' },    // 6
	   {'5', '2', '5', '1' },    // 7
	   {'5', '1', '5', '2' },    // 8
	   {'7', '0', '5', '1' }     // 9
	    };
		private string Code_EAN13()
		{
			int i, j, LK;
			string tmp;
			string retresult = string.Empty; //Default result string ( a String empty)
			if (Checksum)
			{
				tmp = "0000000000000" + CurrentText;
				tmp = GetEAN(tmp.Substring(tmp.Length - 12, 12) + "0");
			}
			else
				tmp = CurrentText;

			// Startcode
			string result = "505";
			//Avoid to process a code that is shorter to 13 characters
			if (tmp.Length >= 13)
			{
				LK = ((int)tmp[0]) - ((int)'0');
				tmp = tmp.Substring(1, 12);
				int index;
				for (i = 0; i < 6; i++)
				{
					index = ((int)tmp[i]) - ((int)'0');
					switch (tabelle_ParityEAN13[LK, i])
					{
						case 'A':
							for (j = 0; j < 4; j++)
								result = result + tabelle_EAN_A[index, j];
							break;
						case 'B':
							for (j = 0; j < 4; j++)
								result = result + tabelle_EAN_B[index, j];
							break;
						case 'C':
							for (j = 0; j < 4; j++)
								result = result + tabelle_EAN_C[index, j];
							break;
					}
				}
				result = result + "05050";
				for (i = 6; i < 12; i++)
					for (j = 0; j < 4; j++)
					{
						index = ((int)tmp[i]) - ((int)'0');
						result = result + tabelle_EAN_C[index, j];
					}
				// Stopcode
				result += "505";
				retresult = result;
			} //End of check of a shorter code than 13
			return retresult;
		}
		private string Code_EAN8()
		{
			int i, j, index;
			String tmp;
			String retResult = String.Empty; //Default result string ( a string empty)

			if (Checksum)
			{
				tmp = "00000000" + CurrentText;
				tmp = GetEAN(tmp.Substring(tmp.Length - 7, 7) + "0");
			}
			else
				tmp = CurrentText;

			// Startcode
			String result = "505";
			//Check to avoid the processing of code shorter than 8 characters
			if (tmp.Length >= 8)
			{
				for (i = 0; i < 4; i++)
					for (j = 0; j < 4; j++)
					{
						index = ((int)tmp[i]) - ((int)'0');
						result = result + tabelle_EAN_A[index, j];
					}
				result = result + "05050";

				for (i = 4; i < 8; i++)
					for (j = 0; j < 4; j++)
					{
						index = ((int)tmp[i]) - ((int)'0');
						result = result + tabelle_EAN_C[index, j];
					}
				// Stopcode
				result += "505";
				retResult = result;
			} //End of code lengh Check 
			return retResult;
		}
		// Pattern for Barcode 2 of 5
		private static char[,] tabelle_2_5 = new char[10, 5] {
	  {'0', '0', '1', '1', '0'},    // 0
	  {'1', '0', '0', '0', '1'},    // 1
	  {'0', '1', '0', '0', '1'},    // 2
	  {'1', '1', '0', '0', '0'},    // 3
	  {'0', '0', '1', '0', '1'},    // 4
	  {'1', '0', '1', '0', '0'},    // 5
	  {'0', '1', '1', '0', '0'},    // 6
	  {'0', '0', '0', '1', '1'},    // 7
	  {'1', '0', '0', '1', '0'},    // 8
	  {'0', '1', '0', '1', '0'}     // 9
	  };
		private string Code_2_5_interleaved()
		{
			int i, j, index;
			char c;
			String FText;
			FText = CurrentText;
			String result = "5050";   // Startcode

			for (i = 0; i < FText.Length / 2; i++)
				for (j = 0; j < 5; j++)
				{
					index = ((int)FText[i * 2]) - ((int)'0');
					if (tabelle_2_5[index, j] == '1')
						c = '6';
					else
						c = '5';
					result = result + c;
					index = ((int)FText[i * 2 + 1]) - ((int)'0');
					if (tabelle_2_5[index, j] == '1')
						c = '1';
					else
						c = '0';
					result = result + c;
				}
			result = result + "605";    // Stopcode
			return result;
		}
		private string Code_2_5_industrial()
		{
			int i, j, index;
			String FText;
			String result = "606050";   // Startcode
			FText = CurrentText;
			for (i = 0; i < FText.Length; i++)
				for (j = 0; j < 5; j++)
				{
					index = ((int)FText[i]) - ((int)'0');
					if (tabelle_2_5[index, j] == '1')
						result = result + "60";
					else
						result = result + "50";
				}
			result = result + "605060";   // Stopcode
			return result;
		}
		private string Code_2_5_matrix()
		{
			int i, j, index;
			char c;
			String FText;
			String result = "705050";   // Startcode
			FText = CurrentText;
			for (i = 0; i < FText.Length; i++)
			{
				for (j = 0; j < 5; j++)
				{
					index = ((int)FText[i * 2 + 1]) - ((int)'0');
					if (tabelle_2_5[index, j] == '1')
						c = '1';
					else
						c = '0';
					//			  if odd(j) then
					if ((j % 2) != 0)
						c = (char)((int)c + 5);
					result = result + c;
				}
				result = result + "0";
			}
			result = result + "70505";   // Stopcode
			return result;
		}
		private struct Code39
		{
			public char c;
			public String data;
			public short chk;
			public Code39(char c1, String data1, short chk1)
			{
				c = c1;
				data = data1;
				chk = chk1;
			}
		}
		private static Code39[] tabelle_39 = new Code39[44]
			{ new Code39('0',"505160605",0),
			   new Code39('1',"605150506",1 ),
			 new Code39('2',"506150506",2 ),
			   new Code39('3',"606150505",3 ),
			   new Code39('4',"505160506",4 ),
			   new Code39('5',"605160505",5 ),
			   new Code39('6',"506160505",6 ),
			   new Code39('7',"505150606",7 ),
			   new Code39('8',"605150605",8 ),
			   new Code39('9',"506150605",9 ),
			   new Code39('A',"605051506",10),
			   new Code39('B',"506051506",11),
			   new Code39('C',"606051505",12),
			   new Code39('D',"505061506",13),
			new Code39('E',"605061505",14),
			new Code39('F',"506061505",15),
			new Code39('G',"505051606",16),
			new Code39('H',"605051605",17),
		  new Code39('I',"506051605",18),
			new Code39('J',"505061605",19),
			new Code39('K',"605050516",20),
			new Code39('L',"506050516",21),
			new Code39('M',"606050515",22),
			new Code39('N',"505060516",23),
			new Code39('O',"605060515",24),
			new Code39('P',"506060515",25),
			new Code39('Q',"505050616",26),
			new Code39('R',"605050615",27),
			new Code39('S',"506050615",28),
			new Code39('T',"505060615",29),
			new Code39('U',"615050506",30),
			new Code39('V',"516050506",31),
			new Code39('W',"616050505",32),
			new Code39('X',"515060506",33),
			new Code39('Y',"615060505",34),
			new Code39('Z',"516060505",35),
			new Code39('-',"515050606",36),
			new Code39('.',"615050605",37),
			new Code39(' ',"516050605",38),
			new Code39('*',"515060605",0 ),
			new Code39('$',"515151505",39),
			new Code39('/',"515150515",40),
			new Code39('+',"515051515",41),
			new Code39('%',"505151515",42)
			};
		private int FindCode39IDX(char z)
		{
			int i;
			int result = -1;
			for (i = 0; i < 44; i++)
			{
				if (tabelle_39[i].c == z)
				{
					result = i;
					break;
				}
			}
			return result;
		}
		private string Code_39()
		{
			String result;
			int i, idx;
			int checksum;
			String FText = CurrentText;

			checksum = 0;
			// Startcode
			result = tabelle_39[FindCode39IDX('*')].data + '0';

			FText = CurrentText;
			for (i = 0; i < FText.Length; i++)
			{
				idx = FindCode39IDX(FText[i]);
				if (idx >= 0)
				{
					result = result + tabelle_39[idx].data + "0";
					checksum = checksum + tabelle_39[idx].chk;
				}
			}
			// Calculate Checksum Data
			if (Checksum)
			{
				checksum = checksum % 43;
				for (i = 0; i < 44; i++)
					if (checksum == tabelle_39[i].chk)
					{
						result = result + tabelle_39[i].data + "0";
						return result;
					}
			}
			result = result + tabelle_39[FindCode39IDX('*')].data;
			return result;
		}


		private static String[] code39x = new String[128] {
		  "%U", "$A", "$B", "$C", "$D", "$E", "$F", "$G",
		  "$H", "$I", "$J", "$K", "$L", "$M", "$N", "$O",
		  "$P", "$Q", "$R", "$S", "$T", "$U", "$V", "$W",
		  "$X", "$Y", "$Z", "%A", "%B", "%C", "%D", "%E",
		  " ", "/A", "/B", "/C", "/D", "/E", "/F", "/G",
		  "/H", "/I", "/J", "/K", "/L", "/M", "/N", "/O",
		  "0",  "1",  "2",  "3",  "4",  "5",  "6",  "7",
		  "8",  "9", "/Z", "%F", "%G", "%H", "%I", "%J",
		  "%V",  "A",  "B",  "C",  "D",  "E",  "F",  "G",
		  "H",  "I",  "J",  "K",  "L",  "M",  "N",  "O",
		  "P",  "Q",  "R",  "S",  "T",  "U",  "V",  "W",
		  "X",  "Y",  "Z", "%K", "%L", "%M", "%N", "%O",
		  "%W", "+A", "+B", "+C", "+D", "+E", "+F", "+G",
		  "+H", "+I", "+J", "+K", "+L", "+M", "+N", "+O",
		  "+P", "+Q", "+R", "+S", "+T", "+U", "+V", "+W",
		  "+X", "+Y", "+Z", "%P", "%Q", "%R", "%S", "%T"
		 };

		private string Code_39Extended()
		{
			String result;
			String save = CurrentText;
			String FText = "";
			int i;
			for (i = 0; i < save.Length; i++)
			{
				if ((int)save[i] <= 127)
				{
					FText = FText + code39x[(int)save[i]];
				}
			}
			CurrentText = FText;
			try
			{
				result = Code_39();
			}
			finally
			{
				CurrentText = save;
			}
			return result;
		}

		private struct Code93
		{
			public char c;
			public string data;
			public Code93(char c1, String data1)
			{
				c = c1;
				data = data1;
			}
			public static Code93[] table = new Code93[47] {
				 new Code93('0',"131112"),
				 new Code93('1',"111213"),
				 new Code93('2',"111312"),
				 new Code93('3',"111411"),
				 new Code93('4',"121113"),
				 new Code93('5',"121212"),
				 new Code93('6',"121311"),
				 new Code93('7',"111114"),
				 new Code93('8',"131211"),
				 new Code93('9',"141111"),
				 new Code93('A',"211113"),
				 new Code93('B',"211212"),
				 new Code93('C',"211311"),
				 new Code93('D',"221112"),
				 new Code93('E',"221211"),
				 new Code93('F',"231111"),
				 new Code93('G',"112113"),
				 new Code93('H',"112212"),
				 new Code93('I',"112311"),
				 new Code93('J',"122112"),
				 new Code93('K',"132111"),
				 new Code93('L',"111123"),
				 new Code93('M',"111222"),
				 new Code93('N',"111321"),
				 new Code93('O',"121122"),
				 new Code93('P',"131121"),
				 new Code93('Q',"212112"),
				 new Code93('R',"212211"),
				 new Code93('S',"211122"),
				 new Code93('T',"211221"),
				 new Code93('U',"221121"),
				 new Code93('V',"222111"),
				 new Code93('W',"112122"),
				 new Code93('X',"112221"),
				 new Code93('Y',"122121"),
				 new Code93('Z',"123111"),
				 new Code93('-',"121131"),
				 new Code93('.',"311112"),
				 new Code93(' ',"311211"),
				 new Code93('$',"321111"),
				 new Code93('/',"112131"),
				 new Code93('+',"113121"),
				 new Code93('%',"211131"),
				 new Code93('[',"121221"),   // only used for Extended Code 93
	             new Code93(']',"312111"),   // only used for Extended Code 93
	             new Code93('{',"311121"),   // only used for Extended Code 93
	             new Code93('}',"122211")    // only used for Extended Code 93
           };
		}
		private int Find_Code93(char c)
		{
			int i;
			int result = -1;
			for (i = 0; i < 47; i++)
			{
				if (Code93.table[i].c == c)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		//	converts a string from '321' to the internal representation '715'
		//	i need this function because some pattern tables have a different
		//	format :

		//	'00111'
		//	converts to '05161'

		private String DoConvert(String s)
		{
			int i, v;
			String t;
			t = "";
			for (i = 0; i < s.Length; i++)
			{
				v = (int)s[i] - 1;
				if ((i % 2) == 0)
					v = v + 5;
				t = t + (char)v;
			}
			return t;
		}
		private String Code_93()
		{
			int i, idx;
			int checkC, checkK,   // Checksums
			weightC, weightK;
			String FText;

			FText = CurrentText;

			String result = DoConvert("111141");   // Startcode

			for (i = 0; i < FText.Length; i++)
			{
				idx = Find_Code93(FText[i]);
				if (idx < 0)
					throw new NamedException("Code93 bad Data: " + FText, FText);
				result = result + DoConvert(Code93.table[idx].data);
			}
			checkC = 0;
			checkK = 0;

			weightC = 1;
			weightK = 2;

			for (i = FText.Length - 1; i >= 0; i--)
			{
				idx = Find_Code93(FText[i]);
				checkC = checkC + idx * weightC;
				checkK = checkK + idx * weightK;

				weightC++;
				if (weightC > 20)
					weightC = 1;
				weightK++;
				if (weightK > 15)
					weightC = 1;
			}
			checkK = checkK + checkC;

			checkC = checkC % 47;
			checkK = checkK % 47;

			result = result + DoConvert(Code93.table[checkC].data) +
			 DoConvert(Code93.table[checkK].data);

			result = result + DoConvert("1111411");   // Stopcode
			return result;
		}
		private static String[] code93x = new String[128]  {
			  "]U", "[A", "[B", "[C", "[D", "[E", "[F", "[G",
			  "[H", "[I", "[J", "[K", "[L", "[M", "[N", "[O",
			  "[P", "[Q", "[R", "[S", "[T", "[U", "[V", "[W",
			  "[X", "[Y", "[Z", "]A", "]B", "]C", "]D", "]E",
			   " ", "{A", "{B", "{C", "{D", "{E", "{F", "{G",
			  "{H", "{I", "{J", "{K", "{L", "{M", "{N", "{O",
			  "0",  "1",  "2",  "3",  "4",  "5",  "6",  "7",
			   "8",  "9", "{Z", "]F", "]G", "]H", "]I", "]J",
			  "]V",  "A",  "B",  "C",  "D",  "E",  "F",  "G",
			   "H",  "I",  "J",  "K",  "L",  "M",  "N",  "O",
			   "P",  "Q",  "R",  "S",  "T",  "U",  "V",  "W",
			   "X",  "Y",  "Z", "]K", "]L", "]M", "]N", "]O",
			  "]W", "}A", "}B", "}C", "}D", "}E", "}F", "}G",
			  "}H", "}I", "}J", "}K", "}L", "}M", "}N", "}O",
			  "}P", "}Q", "}R", "}S", "}T", "}U", "}V", "}W",
			  "}X", "}Y", "}Z", "]P", "]Q", "]R", "]S", "]T"
			 };
		private struct Code128
		{
			public char b;
			public char a;
			public string c;
			public string data;
			public Code128(char a1, char b1, string c1, string data1)
			{
				a = a1;
				b = b1;
				c = c1;
				data = data1;
			}
			public static string StartA = "211412";
			public static string StartB = "211214";
			public static string StartC = "211232";
			public static string Stop = "2331112";
			public static int FindCodeAB(BarcodeType btype, char c)
			{
				int aresult = -1;
				for (int i = 0; i < 103; i++)
				{
					char tocompare;
					if (btype == BarcodeType.Code128A)
						tocompare = table[i].a;
					else
						tocompare = table[i].b;
					if (tocompare == c)
					{
						aresult = i;
						break;
					}
				}
				return aresult;
			}
			public static int FindCodeC(string c)
			{
				int aresult = -1;
				for (int i = 0; i < 103; i++)
				{
					if (table[i].c == c)
					{
						aresult = i;
						break;
					}
				}
				return aresult;
			}
			public static BarcodeType FindNewType(string text, int i)
			{
				BarcodeType btype = BarcodeType.Code128B;
				if (text[i] == (char)0xBF)
					i++;
				string acopy = text.Substring(i);
				int index = acopy.IndexOf((char)0xBF);
				if (index >= 0)
				{
					acopy = acopy.Substring(0, index);
				}
				// Look for 128C
				if ((acopy.Length % 2) == 0)
				{
					btype = BarcodeType.Code128C;
					foreach (char c in acopy)
					{
						if (!StringUtil.IsDigit(c))
						{
							btype = BarcodeType.Code128B;
							break;
						}
					}
				}
				return btype;
			}
			public static Code128[] table = new Code128[103]  {
				new Code128(' ',' ',"00","212222"),
				new Code128('!','!',"01","222122" ),
				new Code128('"','"',"02","222221" ),
				new Code128('#','#',"03","121223" ),
				new Code128('$','$',"04","121322" ),
				new Code128('%','%',"05","131222" ),
				new Code128('&','&',"06","122213" ),
				new Code128('\'','\'',"07","122312" ),
				new Code128('(','(',"08","132212" ),
				new Code128(')',')',"09","221213" ),
				new Code128('*','*',"10","221312" ),
				new Code128('+','+',"11","231212" ),
				new Code128((char)44,(char)44,"12","112232" ),
				new Code128('-','-',"13","122132" ),
				new Code128('.','.',"14","122231" ),
				new Code128('/','/',"15","113222" ),
				new Code128('0','0',"16","123122" ),
				new Code128('1','1',"17","123221" ),
				new Code128('2','2',"18","223211" ),
				new Code128('3','3',"19","221132" ),
				new Code128('4','4',"20","221231" ),
				new Code128('5','5',"21","213212" ),
				new Code128('6','6',"22","223112" ),
				new Code128('7','7',"23","312131" ),
				new Code128('8','8',"24","311222" ),
				new Code128('9','9',"25","321122" ),
				new Code128(':',':',"26","321221" ),
				new Code128(';',';',"27","312212" ),
				new Code128('<','<',"28","322112" ),
				new Code128('=','=',"29","322211" ),
				new Code128('>','>',"30","212123" ),
				new Code128('?','?',"31","212321" ),
				new Code128('@','@',"32","232121" ),
				new Code128('A','A',"33","111323" ),
				new Code128('B','B',"34","131123" ),
				new Code128('C','C',"35","131321" ),
				new Code128('D','D',"36","112313" ),
				new Code128('E','E',"37","132113" ),
				new Code128('F','F',"38","132311" ),
				new Code128('G','G',"39","211313" ),
				new Code128('H','H',"40","231113" ),
				new Code128('I','I',"41","231311" ),
				new Code128('J','J',"42","112133" ),
				new Code128('K','K',"43","112331" ),
				new Code128('L','L',"44","132131" ),
				new Code128('M','M',"45","113123" ),
				new Code128('N','N',"46","113321" ),
				new Code128('O','O',"47","133121" ),
				new Code128('P','P',"48","313121" ),
				new Code128('Q','Q',"49","211331" ),
				new Code128('R','R',"50","231131" ),
				new Code128('S','S',"51","213113" ),
				new Code128('T','T',"52","213311" ),
				new Code128('U','U',"53","213131" ),
				new Code128('V','V',"54","311123" ),
				new Code128('W','W',"55","311321" ),
				new Code128('X','X',"56","331121" ),
				new Code128('Y','Y',"57","312113" ),
				new Code128('Z','Z',"58","312311" ),
				new Code128('[','[',"59","332111" ),
				new Code128('\\','\\',"60","314111" ),
				new Code128(']',']',"61","221411" ),
				new Code128('^','^',"62","431111" ),
				new Code128('_','_',"63","111224" ),
				new Code128(' ','`',"64","111422" ),
				new Code128(' ','a',"65","121124" ),
				new Code128(' ','b',"66","121421" ),
				new Code128(' ','c',"67","141122" ),
				new Code128(' ','d',"68","141221" ),
				new Code128(' ','e',"69","112214" ),
				new Code128(' ','f',"70","112412" ),
				new Code128(' ','g',"71","122114" ),
				new Code128(' ','h',"72","122411" ),
				new Code128(' ','i',"73","142112" ),
				new Code128(' ','j',"74","142211" ),
				new Code128(' ','k',"75","241211" ),
				new Code128(' ','l',"76","221114" ),
				new Code128(' ','m',"77","413111" ),
				new Code128(' ','n',"78","241112" ),
				new Code128(' ','o',"79","134111" ),
				new Code128(' ','p',"80","111242" ),
				new Code128(' ','q',"81","121142" ),
				new Code128(' ','r',"82","121241" ),
				new Code128(' ','s',"83","114212" ),
				new Code128(' ','t',"84","124112" ),
				new Code128(' ','u',"85","124211" ),
				new Code128(' ','v',"86","411212" ),
				new Code128(' ','w',"87","421112" ),
				new Code128(' ','x',"88","421211" ),
				new Code128(' ','y',"89","212141" ),
				new Code128(' ','z',"90","214121" ),
				new Code128(' ','{',"91","412121" ),
				new Code128(' ','|',"92","111143" ),
				new Code128(' ','}',"93","111341" ),
				new Code128(' ','~',"94","131141" ),
				new Code128(' ',' ',"95","114113" ),
				new Code128('�', '�',"96","114311" ), //FNC3
	            new Code128('�','�',"97","411113" ), // FNC2
	            new Code128('�','�',"98","411311" ), // Shift B
	            new Code128(' ',' ',"99","113141" ),
				new Code128(' ',' ',"  ","114131" ),
				new Code128('�','�',"  ","311141" ),
				new Code128((char)0xBF,(char)0xBF,"  ","411131" ) // FNC1
	        };

		}

		private string Code_93Extended()
		{
			String save, result;
			int i;
			String FText;
			//	CharToOem(PChar(FText), save);
			save = CurrentText;
			FText = "";


			for (i = 0; i < save.Length; i++)
			{
				if ((int)save[i] <= 127)
					FText = FText + code93x[(int)save[i]];
			}
			CurrentText = FText;
			try
			{
				result = Code_93();
			}
			finally
			{
				CurrentText = save;
			}
			return result;
		}
		Point Translate2D(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}
		// half means a black line with 2/5 height (used for PostNet)
		private enum LineType { White, Black, Half };
		private Point Rotate2D(Point p, double alpha)
		{
			double sinus = Math.Sin(alpha);
			double cosinus = Math.Cos(alpha);
			int x = Convert.ToInt32(Math.Round(p.X * cosinus + p.Y * sinus));
			int y = Convert.ToInt32(Math.Round(-p.X * sinus + p.Y * cosinus));
			Point result = new Point(x, y);
			return result;
		}
		public void DoLines(string data, int Left, int Top, MetaFile metafile)
		{
			int i;
			LineType lt;
			int xadd, awidth, aheight;
			// Edges of a line (rectangle)
			Point a, b, c, d, origin;
			//	  double alpha;
			int PenWidth;
			int PenColor;
			int BrushColor;
			if (BarType == BarcodeType.CodePDF417)
			{
				//Draw2DBarcode(FLeft,FTop,meta);
				return;
			}
			xadd = 0;
			origin = new Point(Left, Top);
			PenWidth = 0;
			for (i = 0; i < data.Length; i++)
			{
				switch (data[i])
				{
					case '0':
						awidth = modules[0];
						lt = LineType.White;
						break;
					case '1':
						awidth = modules[1];
						lt = LineType.White;
						break;
					case '2':
						awidth = modules[2];
						lt = LineType.White;
						break;
					case '3':
						awidth = modules[3];
						lt = LineType.White;
						break;
					case '5':
						awidth = modules[0];
						lt = LineType.Black;
						break;
					case '6':
						awidth = modules[1];
						lt = LineType.Black;
						break;
					case '7':
						awidth = modules[2];
						lt = LineType.Black;
						break;
					case '8':
						awidth = modules[3];
						lt = LineType.Black;
						break;
					case 'A':
						awidth = modules[0];
						lt = LineType.Half;
						break;
					case 'B':
						awidth = modules[1];
						lt = LineType.Half;
						break;
					case 'C':
						awidth = modules[2];
						lt = LineType.Half;
						break;
					case 'D':
						awidth = modules[3];
						lt = LineType.Half;
						break;
					default:
						throw new NamedException(Translator.TranslateStr(582) + ":" + data, data);
				}
				if ((lt == LineType.Black) || (lt == LineType.Half))
					PenColor = BColor;
				else
					PenColor = 0xFFFFFF;
				if ((lt != LineType.White) || ((lt == LineType.White) && (!Transparent)))
				{
					BrushColor = PenColor;
					if (lt == LineType.Half)
						aheight = PrintHeight * 2 / 5;
					else
						aheight = PrintHeight;
					a = new Point(xadd, 0);
					b = new Point(xadd, aheight);
					c = new Point(xadd + awidth, aheight);
					d = new Point(xadd + awidth, 0);
					// a,b,c,d builds the rectangle we want to draw



					if (Rotation != 0)
					{
						double alpha = (double)Rotation / 10 * Math.PI / 180.0;
						a = Translate2D(Rotate2D(a, alpha), origin);
						b = Translate2D(Rotate2D(b, alpha), origin);
						c = Translate2D(Rotate2D(c, alpha), origin);
						d = Translate2D(Rotate2D(d, alpha), origin);
						int left = Math.Min(Math.Min(a.X, b.X), Math.Min(c.X, d.X));
						int top = Math.Min(Math.Min(a.Y, b.Y), Math.Min(c.Y, d.Y));
						int right = Math.Max(Math.Max(a.X, b.X), Math.Max(c.X, d.X));
						int bottom = Math.Max(Math.Max(a.Y, b.Y), Math.Max(c.Y, d.Y));
						a = new Point(left, top);
						c = new Point(right, bottom);
					}
					else
					{
						a = Translate2D(a, origin);
						b = Translate2D(b, origin);
						c = Translate2D(c, origin);
						d = Translate2D(d, origin);
					}

					MetaObjectDraw metaobj = new MetaObjectDraw();
					metaobj.MetaType = MetaObjectType.Draw;

					metaobj.Top = a.Y;
					metaobj.Left = a.X;
					metaobj.Width = c.X - a.X;
					metaobj.Height = c.Y - a.Y;



					metaobj.DrawStyle = ShapeType.Rectangle;
					metaobj.BrushStyle = (int)BrushType.Solid;
					metaobj.PenStyle = (int)PenType.Clear;
					metaobj.PenWidth = PenWidth;
					metaobj.PenColor = PenColor;
					metaobj.BrushColor = BrushColor;
					metafile.Pages[metafile.CurrentPage].Objects.Add(metaobj);
				}

				xadd = xadd + awidth;
			}

		}
	}
}