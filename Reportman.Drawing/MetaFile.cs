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
using System.Drawing;
using System.Threading;
#if REPMAN_ZLIB
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
#endif
using System.Collections.Generic;
#if NETSTANDARD2_0
#else
using System.Drawing.Imaging;
#endif

namespace Reportman.Drawing
{

    /// <summary>
    /// When the Report is sent to a printer, there is an option to use the printer fonts, if the printer fonts
    /// are used, in some cases the print is faster (dot matrix printers)
    /// </summary>
    public enum PrinterFontsType { 
        /// <summary>The printer fonts are not used, or used only if the print driver forces it</summary>
        Default,
        /// <summary>The printer fonts are allways used, some print drivers does not have this option available</summary>
        Always,
        /// <summary>Do not use printer fonts</summary>
        Never,
        /// <summary>Use printer fonts, but if necessary recalculate the report to accomodate printer fonts,
        /// this is the recommended option when working with dot matrix printers, but number of pages in preview and 
        /// number of printed can differ</summary>
        Recalculate
    };
    /// <summary>
    /// When previewing pages of a report, visualization scale can be adjusted to predetermined sizes
    /// </summary>
    public enum AutoScaleType {
        /// <summary>The page will fit the width of the window application</summary>
        Wide,
        /// <summary>Real size, using dots per inch of screen</summary>
        Real,
        /// <summary>The entire page will be scaled to be visible inside the window application</summary>
        EntirePage,
        /// <summary>The page will have a custom scale</summary>
        Custom,
        /// <summary>The page will fit the height of the window</summary>
        Height
    };
    /// <summary>
    /// Event launched when the report processing stops by user or is aborted
    /// </summary>
	public delegate void MetaStopWork();
    /// <summary>
    /// Event launched when the asynchronous report processing produces an error
    /// </summary>
    /// <param name="errormessage">Error message containing error information</param>
	public delegate void MetaFileWorkAsyncError(string errormessage);
    /// <summary>
    /// Event launched when reading a MetaFile, so, on large metafiles (or when low bandwidth)
    /// you can show load progress to the user
    /// </summary>
    /// <param name="pages">Current page being read</param>
    /// <param name="pagecount">Total page count</param>
    /// <param name="docancel">Set this reference variable to true to cancel the MetaFile loading</param>
	public delegate void MetaFileLoadProgress(int pages, int pagecount, ref bool docancel);
    /// <summary>
    /// Event launched when the loading of a MetaFile causes an error
    /// </summary>
    /// <param name="errormessage">Error message containing error information</param>
    public delegate void MetaFileLoadError(string errormessage);
    /// <summary>
    /// Event launched when a page is being requested
    /// </summary>
    /// <param name="pageindex">Page requested</param>
    /// <returns></returns>
	public delegate bool RequestPageEvent(int pageindex);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="records">Records processed</param>
    /// <param name="pagecount">Current number number of pages</param>
    /// <param name="docancel">Set this reference variable to true to cancel report processing</param>
	public delegate void MetaFileWorkProgress(int records, int pagecount, ref bool docancel);
    /// <summary>
    /// Event launched when saving a MetaFile, so on large MetaFiles (or low bandwidth)
    /// you can show save progress to the user
    /// </summary>
    /// <param name="currentpage">Current page being saved</param>
    /// <param name="pagecount">Total number of pages to save</param>
    /// <param name="docancel">Set this reference variable to true to cancel the MetaFile saving</param>
	public delegate void MetaFileSaveWorkProgress(int currentpage,int pagecount,ref bool docancel);
    /// <summary>
    /// Brush types, when drawing shapes, the filling area can be stablished to a specified pattern
    /// </summary>
	public enum BrushType
	{
        /// <summary>Solid, default pattern. HatchStyle.SolidDiamond</summary>
		Solid,
        /// <summary>No pattern is used, that is like a transparent color</summary>
        Clear,
        /// <summary>Pattern of horizontal lines. HatchStyle.Horizontal</summary>
        Horizontal,
        /// <summary>Pattern of vertical lines. HatchStyle.Vertical</summary>
        Vertical,
        /// <summary>Pattern of diagonal lines. HatchStyle.LightUpwardDiagonal</summary>
        ADiagonal,
        /// <summary>Pattern of reverse diagonal lines. HatchStyle.LightDownwardDiagonal</summary>
        BDiagonal,
        /// <summary>Pattern of crossed lines. HatchStyle.Cross</summary>
        ACross,
        /// <summary>Pattern of diagonal crossed lines. HatchStyle.DiagonalCross</summary>
        BCross,
        /// <summary>Pattern of dense points type 1. HatchStyle.Percent10</summary>
        Dense1,
        /// <summary>Pattern of dense points type 2. HatchStyle.Percent20</summary>
        Dense2,
        /// <summary>Pattern of dense points type 3. HatchStyle.Percent30</summary>
        Dense3,
        /// <summary>Pattern of dense points type 4. HatchStyle.Percent40</summary>
        Dense4,
        /// <summary>Pattern of dense points type 5. HatchStyle.Percent50</summary>
        Dense5,
        /// <summary>Pattern of dense points type 6. HatchStyle.Percent60</summary>
        Dense6,
        /// <summary>Pattern of dense points type 7. HatchStyle.Percent70</summary>
        Dense7
	};
    /// <summary>
    /// Pen types, used when drawing shapes
    /// </summary>
	public enum PenType { 
        /// <summary>Solid normal pen</summary>
        Solid,
        /// <summary>Dashed pen</summary>
        Dash,
        /// <summary>Doted pen</summary>
        Dot,
        /// <summary>Dash Dot pen</summary>
        DashDot,
        /// <summary>Dash Dot Dot pen</summary>
        DashDotDot,
        /// <summary>Transparent pen</summary>
        Clear
    };
    /// <summary>
    /// Drawing styles, determines the way the image adapt to a bounding box
    /// </summary>
	public enum ImageDrawStyleType { 
        /// <summary>The image is drawn, but it's clipped inside the rectangle, part of the image can be not visible</summary>
        Crop,
        /// <summary>The image is adapted to the bounding box size</summary>
        Stretch,
        /// <summary>The image is drawn without clipping, so bounding box size is ignored, dots per inch is used to calculate size</summary>
        Full,
        /// <summary>The image is drawn as many times as necessary to fill the rectangle left to right and top to bottom, scaling will depend on ouput device</summary>
        Tile,
        /// <summary>The image is drawn as many times as necessary to fill the rectangle left to right and top to bottom, the dots per inch image attribute will remain</summary>
        Tiledpi
    };
    /// <summary>
    /// Available shape types
    /// </summary>
	public enum ShapeType
	{
        /// <summary>Rectangle</summary>
		Rectangle,
        /// <summary>Square, if the bounding box is a rectangle the smaller distance will be used to draw the rectangle</summary>
        Square,
        /// <summary>Rectangle with rounded corners</summary>
        RoundRect,
        /// <summary>Like square but with rounded corners</summary>
        RoundSquare,
        /// <summary>An ellipse will be drawn filling the bounding box</summary>
        Ellipse,
        /// <summary>A circle will be drawn filling the bounding box, smaller distance will be used when the bounding box is a rectangle</summary>
        Circle,
        /// <summary>Horizontal line</summary>
        HorzLine,
        /// <summary>Vertical line</summary>
        VertLine,
        /// <summary>Oblique line</summary>
        Oblique1,
        /// <summary>Oblique reversed line</summary>
        Oblique2
	};
    /// <summary>
    /// When printing to dot matrix devices, a fixed pitch font will be selected, the font to be selected can be specified
    /// using the PrintStep
    /// </summary>
	public enum PrintStepType { 
        /// <summary>The font size will determine the step, smaller sizes will select cpi20</summary>
        BySize,
        /// <summary>20 character per inch font</summary>
        cpi20,
        /// <summary>20 character per inch font</summary>
        cpi17,
        /// <summary>17 character per inch font</summary>
        cpi15,
        /// <summary>15 character per inch font</summary>
        cpi12,
        /// <summary>10 character per inch font</summary>
        cpi10,
        /// <summary>6 character per inch font</summary>
        cpi6,
        /// <summary>5 character per inch font</summary>
        cpi5
    };
    /// <summary>
    /// When exporting a document to PDF, there are some standard predefinded fonts you can use,
    /// using this fonts redude file size
    /// </summary>
	public enum PDFFontType { 
        /// <summary>Helvetica font, same look as Microsoft Arial, variable pitch</summary>
        Helvetica,
        /// <summary>Courier font, classic fixed pitch font</summary>
        Courier,
        /// <summary>Times Roman  font, classic variable pitch font</summary>
        TimesRoman,
        /// <summary>Symbol font, provided to print standard symbols</summary>
        Symbol,
        /// <summary>ZafDingbats font, provided to print other symbols</summary>
        ZafDingbats,
        /// <summary>Linked will store a reference to the font in the Adobe PDF file, the file
        /// size will not be increased, but destination computer (computer that opens the pdf file)
        /// must have the referenced installed</summary>
        Linked,
        /// <summary>Embedded will store the font data in the Adobe PDF file, the file
        /// size will be increased, but ensures the destination computer (computer that opens the pdf file)
        /// will render correctly the document</summary>
        Embedded
    };
    /// <summary>
    /// Report Manager allow automatic printer selection when processing reports, so you can assign
    /// a predefined printer to a report, then assign on each computer a diferent printer to predefined
    /// printers.
    /// </summary>
    /// <remarks>
    /// This is useful for automatic output to ticket printer without the need for showing a print dialog
    /// or setting it as default printer.
    /// </remarks>
    public enum PrinterSelectType
	{
        /// <summary>Default printer</summary>
		DefaultPrinter,
        /// <summary>Report Printer, user predefined printer</summary>
        ReportPrinter,
        /// <summary>Ticket Printer, user predefined printer, by default the text driver TM-U210 will be used</summary>
        TicketPrinter,
        /// <summary>Graphic Printer, user predefined printer</summary>
        Graphicprinter,
        /// <summary>Character Printer, user predefined printer, by default the text driver EPSON will be used</summary>
        Characterprinter,
        /// <summary>Report Printer 2, user predefined printer</summary>
        ReportPrinter2,
        /// <summary>Ticket Printer 2, user predefined printer, by default the text driver TM-U210 will be used</summary>
        TicketPrinter2,
        /// <summary>User Printer 1, user predefined printer</summary>
        UserPrinter1,
        /// <summary>User Printer 2, user predefined printer</summary>
        UserPrinter2,
        /// <summary>User Printer 3, user predefined printer</summary>
        UserPrinter3,
        /// <summary>User Printer 4, user predefined printer</summary>
        UserPrinter4,
        /// <summary>User Printer 5, user predefined printer</summary>
        UserPrinter5,
        /// <summary>User Printer 6, user predefined printer</summary>
        UserPrinter6,
        /// <summary>User Printer 7, user predefined printer</summary>
        UserPrinter7,
        /// <summary>User Printer 8, user predefined printer</summary>
        UserPrinter8,
        /// <summary>User Printer 9, user predefined printer</summary>
        UserPrinter9,
        /// <summary>Plain Printer, by default PLAIN text driver will be used</summary>
        PlainPrinter,
        /// <summary>Plain Full Printer, by default PLAIN text driver without form feed will be used</summary>
        PlainFullPrinter,
        /// <summary>Other printers</summary>
        Printer1,Printer2,Printer3,Printer4,Printer5,Printer6,Printer7,Printer8,Printer9,Printer10,
        Printer11, Printer12, Printer13, Printer14, Printer15, Printer16, Printer17, Printer18, Printer19, Printer20,
        Printer21, Printer22, Printer23, Printer24, Printer25, Printer26, Printer27, Printer28, Printer29, Printer30,
        Printer31, Printer32, Printer33, Printer34, Printer35, Printer36, Printer37, Printer38, Printer39, Printer40,
        Printer41, Printer42, Printer43, Printer44, Printer45, Printer46, Printer47, Printer48, Printer49, Printer50
	};
    /// <summary>
    /// Page orientation
    /// </summary>
	public enum OrientationType { 
        /// <summary>The predefined orientation in selected printer will be used</summary>
        Default,
        /// <summary>Portrait orientaion will be used (Vertical)</summary>
        Portrait,
        /// <summary>Landscape orientaion will be used (Horizontal)</summary>
        Landscape
    };
    /// <summary>
    /// Preview window size the first time it's shown
    /// </summary>
	public enum PreviewWindowStyleType { 
        /// <summary>
        /// Non maximized, normal size
        /// </summary>
        Normal, 
        /// <summary>
        /// Maximized size
        /// </summary>
        Maximized };
	enum MetaSeparator { FileHeader, PageHeader, ObjectHeader, StreamHeader };
    /// <summary>
    /// MetaFiles are composed by pages containing objects, each object can have a type that determines
    /// the action when drawing (or printing) 
    /// </summary>
	public enum MetaObjectType { 
        /// <summary>Text object contain formatting and text information</summary>
        Text,
        /// <summary>Draw object contain basic poligon drawing (lines, rectangles...)</summary>
        Draw,
        /// <summary>Image object contain image and image formatting information</summary>
        Image,
        /// <summary>Poligon object contain a series of drawing actions, this is for future use</summary>
        Polygon,
        /// <summary>Export object contain information for exporting the metafile to custom formats</summary>
        Export
    };
    /// <summary>
    /// Structure containing a reference to pagecount object, the number of pages is not known until the
    /// report is finished, so when the report is finished, all the references to PAGECOUNT special variable
    /// are updated
    /// </summary>
	public  struct TotalPage
	{
        /// <summary>Page index in page collection inside the MetaFile</summary>
		public int PageIndex;
        /// <summary>Object index in object array inside the MetaPage</summary>
        public int ObjectIndex;
        /// <summary>Display format used in the display of the total page number</summary>
        public string DisplayFormat;
	}
	public class TotalPages:List<TotalPage>
	{

	}
    /// <summary>
    /// The PageSizeDetail structure contains advanced page information to select page size on a printer.
    /// </summary>
	public struct PageSizeDetail
	{
        /// <summary>Index into the PageSizeArray structure in print drivers, containing page sizes</summary>
		public int Index;
        /// <summary>The page size does not match predefined pages and custom size must be used</summary>
        public bool Custom;
        /// <summary>When Custom is true this width will be used</summary>
        public int CustomWidth;
        /// <summary>When Custom is true this height will be used</summary>
        public int CustomHeight;
        /// <summary>Physical width of the page</summary>
        public int PhysicWidth;
        /// <summary>Physical height of the page</summary>
        public int PhysicHeight;
        /// <summary>Paper source, this is an index, that must match an index in a specific printer driver</summary>
        public int PaperSource;
        /// <summary>When working with custom sizes you can force the selection of a page size by name</summary>
        public string ForcePaperName;
        /// <summary>
        /// Duplex
        /// </summary>
		public int Duplex;
	}
    /// <summary>
    /// Enumeration indicating the text alignment, that is how the text is drawn inside the defined box
    /// <see cref="Variant">PrintItemText</see>
    /// </summary>
    public enum TextAlignType {
        /// <summary>Align the text to the left of the bounding box</summary>
        Left,
        /// <summary>Align the text to the right of the bounding box</summary>
        Right,
        /// <summary>Align the text to the center of the bounding box</summary>
        Center,
        /// <summary>Align the text to the left and the right of the bounding box, inserting spaces
        /// to align on both sides</summary>
        Justify
    };
    /// <summary>
    /// Enumeration indicating vertical text alignment, that is how the full text, after horizontal alignment is done, is drawn inside the defined box
    /// <see cref="Variant">PrintItemText</see>
    /// </summary>
    public enum TextAlignVerticalType {
        /// <summary>Align the text to the top of the bounding box</summary>
        Top,
        /// <summary>Align the text to the bottom of the bounding box</summary>
        Bottom,
        /// <summary>Align the text to the center of the bounding box</summary>
        Center
    };

	/// <summary>
	/// Class used engine to read/write metafiles.
	/// </summary>
	public class MetaFile: IDisposable,ICloneable
	{
		private MemoryStream FSharedStream;
		private byte[] sign2_4 ={(byte)'R',(byte)'P',(byte)'M',(byte)'E',(byte)'T',(byte)'A',
					  	   (byte)'F',(byte)'I',(byte)'L',(byte)'E',(byte)'0',(byte)'9',0};
		private byte[] sign2_2 ={(byte)'R',(byte)'P',(byte)'M',(byte)'E',(byte)'T',(byte)'A',
								   (byte)'F',(byte)'I',(byte)'L',(byte)'E',(byte)'0',(byte)'7',0};
        /// <summary>Stream used to save image streams shared between multiple pages</summary>
		public MemoryStream SharedStream { get { return FSharedStream; } }
        /// <summary>
        /// Flag available for information of metafile status, default false, empty.
        /// </summary>
        public bool Empty;
        /// <summary>
        /// Array of page sizes
        /// </summary>
		public static int[,] PageSizeArray = new int[149, 2] {
			{ 8268 , 11693},  // psA4
			{ 7165 , 10118},  // psB5
			{ 8500 , 11000},  // psLetter
			{ 8500 , 14000},  // psLegal
			{ 7500 , 10000},  // psExecutive
			{ 33110 , 46811}, // psA0
			{ 23386 , 33110}, // psA1
			{ 16535 , 23386}, // psA2
			{ 11693 , 16535}, // psA3
			{ 5827 , 8268},   // psA5
			{ 4134 , 5827},   // psA6
			{ 2913 , 4134},   // psA7
			{ 2047 , 2913},   // psA8
			{ 1457 , 2047},   // psA9
			{ 40551 , 57323}, // psB0
			{ 28661 , 40551}, // psB1
			{ 1260 , 1772},   // psB10
			{ 20276 , 28661}, // psB2
			{ 14331 , 20276}, // psB3
			{ 10118 , 14331}, // psB4
			{ 5039 , 7165},   // psB6
			{ 3583 , 5039},   // psB7
			{ 2520 , 3583},   // psB8
			{ 1772 , 2520},   // psB9
			{ 6417 , 9016},   // psC5E
			{ 4125 , 9500},   // psComm10E
			{ 4331 , 8661},   // psDLE
			{ 8250 , 13000},  // psFolio
			{ 17000 , 11000}, // psLedger
			{ 11000 , 17000}, // psTabloid
			{ -1 , -1},        // psNPageSize
																								 // Windows equivalents begins at 31
			{ 8500 , 11000}, // Letter 8 12 x 11 in
			{ 8500 , 11000}, // Letter Small 8 12 x 11 in
			{ 11000 , 17000},  // Tabloid 11 x 17 in
			{ 17000 , 11000},  // Ledger 17 x 11 in
			{ 8500 , 14000},  // Legal 8 12 x 14 in
			{ 55000 , 8500},  // Statement 5 12 x 8 12 in
			{ 7500 , 10500}, // Executive 7 14 x 10 12 in
			{ 11693 , 16535}, // A3 297 x 420 mm                     
			{ 8268 , 11693},      // A4 210 x 297 mm                     
			{ 8268 , 11693},// A4 Small 210 x 297 mm               
			{ 5827 , 8268}, // A5 148 x 210 mm                     
			{ 10118 , 14331},    // B4 (JIS} 250 x 354                  
			{ 7165 , 10118}, // B5 (JIS} 182 x 257 mm               
			{ 8250 , 13000}, // Folio 8 12 x 13 in                  
			{ 8465 , 10827}, // Quarto 215 x 275 mm                 
			{ 10000 , 14000}, // 10x14 in                            
		{ 11000 , 17000},// 11x17 in                            
		{ 8500 , 11000}, // Note 8 12 x 11 in                   
		{ 3875 , 8875},// Envelope #9 3 78 x 8 78             
		{ 4125 , 9500},// Envelope #10 4 18 x 9 12            
		{ 4500 , 10375},// Envelope #11 4 12 x 10 38           
		{ 4276 , 11000},// Envelope #12 4 \276 x 11            
		{ 5000 , 11500},// Envelope #14 5 x 11 12              
		{ 16969 , 21969},// C size sheet 431 x 558 mm                       
		{ 21969 , 33976},// D size sheet 558 x 863 mm                      
		{ 33976 , 43976},// E size sheet 863 x 1117 mm                       
		{ 4331 , 8661},// Envelope DL 110 x 220mm             
		{ 6378 , 9016},// Envelope C5 162 x 229 mm            
		{ 12756 , 18031},// Envelope C3  324 x 458 mm           
		{ 9016 , 12756},// Envelope C4  229 x 324 mm           
		{ 4488 , 6378},// Envelope C6  114 x 162 mm           
		{ 4488 , 9016},// Envelope C65 114 x 229 mm           
		{ 9843 , 13898},// Envelope B4  250 x 353 mm           
		{ 6929 , 9843},// Envelope B5  176 x 250 mm           
		{ 6929 , 4921},// Envelope B6  176 x 125 mm           
		{ 4331 , 9056},// Envelope 110 x 230 mm               
		{ 3875 , 7500}, // Envelope Monarch 3.875 x 7.5 in     
		{ 3625 , 6500},// 6 34 Envelope 3 58 x 6 12 in        
		{ 14875 , 11000},// US Std Fanfold 14 78 x 11 in        
		{ 8500 , 12000},// German Std Fanfold 8 12 x 12 in    
		{ 8500 , 13000},// German Legal Fanfold 8 12 x 13 in  
		{ 9843 , 13898},// B4 (ISO} 250 x 353 mm               
		{ 3937 , 5827},// Japanese Postcard 100 x 148 mm      
		{ 9000 , 11000}, // 9 x 11 in                           
		{ 10000 , 11000}, // 10 x 11 in                          
		{ 15000 , 11000}, // 15 x 11 in                          
		{ 8661 , 8661}, // Envelope Invite 220 x 220 mm        
		{ 100 , 100}, // RESERVED--DO NOT USE                
		{ 100 , 100}, // RESERVED--DO NOT USE                
		{ 9275 , 12000}, // Letter Extra 9 \275 x 12 in         
		{ 9275 , 15000}, // Legal Extra 9 \275 x 15 in          
		{ 11690 , 18000}, // Tabloid Extra 11.69 x 18 in         
		{ 9270 , 12690}, // A4 Extra 9.27 x 12.69 in            
		{ 8275 , 11000},  // Letter Transverse 8 \275 x 11 in    
		{ 8268 , 11693},  // A4 Transverse 210 x 297 mm          
		{ 9275 , 12000}, // Letter Extra Transverse 9\275 x 12 in  
		{ 8937 , 14016},     // SuperASuperAA4 227 x 356 mm       
		{ 12008 , 19172},    // SuperBSuperBA3 305 x 487 mm       
		{ 8500 , 12690},    // Letter Plus 8.5 x 12.69 in          
		{ 8268 , 12992},    // A4 Plus 210 x 330 mm                
		{ 5828 , 8268},    // A5 Transverse 148 x 210 mm          
		{ 7166 , 10118},    // B5 (JIS} Transverse 182 x 257 mm    
		{ 13071 , 17520},    // A3 Extra 322 x 445 mm               
		{ 6850 , 9252},    // A5 Extra 174 x 235 mm               
		{ 7913 , 10867},    // B5 (ISO} Extra 201 x 276 mm         
		{ 16536 , 23386},    // A2 420 x 594 mm                     
		{ 11693 , 16535},    // A3 Transverse 297 x 420 mm          
		{ 13071 , 17520},     // A3 Extra Transverse 322 x 445 mm    
		{ 7874 , 5827}, // Japanese Double Postcard 200 x 148 mm 
		{ 4173 ,5827},  // A6 105 x 148 mm                 
		{ 9449 , 13071},  // Japanese Envelope Kaku #2 240 x 332 mm       
		{ 8504 , 10906},  // Japanese Envelope Kaku #3 216 x 277 mm     
		{ 4724 , 9252},  // Japanese Envelope Chou #3 120 x 235 mm      
		{ 3543 , 8071},  // Japanese Envelope Chou #4  90 x 205 mm    
		{ 11000 , 8500},  // Letter Rotated 11 x 8 1/2 11 in 
		{ 16535 , 11693},  // A3 Rotated 420 x 297 mm         
		{ 11693 , 8268},  // A4 Rotated 297 x 210 mm         
		{ 8268 , 5828},  // A5 Rotated 210 x 148 mm         
		{ 14331 , 10118},  // B4 (JIS} Rotated 364 x 257 mm   
		{ 10118 , 7165},  // B5 (JIS} Rotated 257 x 182 mm   
		{ 5827 , 3937}, // Japanese Postcard Rotated 148 x 100 mm 
		{ 5827 , 7874}, // Double Japanese Postcard Rotated 148 x 200 mm 
		{ 5827 , 4173}, // A6 Rotated 148 x 105 mm         
		{ 13071 , 9449},  // Japanese Envelope Kaku #2 Rotated
		{ 10906 , 8504},  // Japanese Envelope Kaku #3 Rotated
		{ 9252 , 4724},  // Japanese Envelope Chou #3 Rotated
		{ 8071 , 3543},  // Japanese Envelope Chou #4 Rotated
		{ 5039 , 7165},  // B6 (JIS} 128 x 182 mm           
		{ 7165 , 5039},  // B6 (JIS} Rotated 182 x 128 mm   
		{ 12000 , 11000},  // 12 x 11 in                      
		{ 4134 , 9252},  // Japanese Envelope You #4 105 x 235 mm       
		{ 9252 , 4134},  // Japanese Envelope You #4 Rotated
		{ 5748 , 8465},  // PRC 16K 146 x 215 mm            
		{ 3819 , 5945},  // PRC 32K 97 x 151 mm             
		{ 3819 , 5945},  // PRC 32K(Big} 97 x 151 mm        
		{ 4134 , 6496},  // PRC Envelope #1 102 x 165 mm    
		{ 4134 , 6929},  // PRC Envelope #2 102 x 176 mm    
		{ 4921 , 5929},  // PRC Envelope #3 125 x 176 mm    
		{ 4331 , 8189},  // PRC Envelope #4 110 x 208 mm    
		{ 4331 , 8661}, // PRC Envelope #5 110 x 220 mm    
		{ 4724 , 9055}, // PRC Envelope #6 120 x 230 mm    
		{ 6299 , 9055}, // PRC Envelope #7 160 x 230 mm    
		{ 4724 , 12165}, // PRC Envelope #8 120 x 309 mm    
		{ 9016 , 12756}, // PRC Envelope #9 229 x 324 mm    
		{ 12756 , 18031}, // PRC Envelope #10 324 x 458 mm   
		{ 8465 , 5748}, // PRC 16K Rotated                 
		{ 5945 , 3819}, // PRC 32K Rotated                 
		{ 5945 , 3819}, // PRC 32K(Big} Rotated            
		{ 6496 , 4134}, // PRC Envelope #1 Rotated 165 x 102 mm
		{ 6929 , 4134}, // PRC Envelope #2 Rotated 176 x 102 mm
		{ 6929 , 4921}, // PRC Envelope #3 Rotated 176 x 125 mm
		{ 8189 , 4331}, // PRC Envelope #4 Rotated 208 x 110 mm
		{ 8661 , 4331}, // PRC Envelope #5 Rotated 220 x 110 mm
		{ 9055 , 4724}, // PRC Envelope #6 Rotated 230 x 120 mm
		{ 9055 , 6299}, // PRC Envelope #7 Rotated 230 x 160 mm
		{ 12165 , 4724}, // PRC Envelope #8 Rotated 309 x 120 mm
		{ 12756 , 9016}, // PRC Envelope #9 Rotated 324 x 229 mm
		{ 18031 , 12756} // PRC Envelope #10 Rotated 458 x 324 mm 
		};
        /// <summary>Constant used for storing all alignment information into a single integer value</summary>
		public const int AlignmentFlags_AlignLeft = 1;
        /// <summary>Constant used for storing all alignment information into a single integer value</summary>
        public const int AlignmentFlags_AlignRight = 2;
        /// <summary>Constant used for storing all alignment information into a single integer value</summary>
        public const int AlignmentFlags_AlignHJustify = 1024;
        /// <summary>Constant used for storing all alignment information into a single integer value</summary>
        public const int AlignmentFlags_AlignHCenter = 4;
        /// <summary>Constant used for storing all alignment information into a single integer value</summary>
        public const int AlignmentFlags_AlignVCenter = 32;
        /// <summary>Constant used for storing all alignment information into a single integer value</summary>
        public const int AlignmentFlags_AlignBottom = 16;
        /// <summary>Constant used for storing all alignment information into a single integer value</summary>
        public const int AlignmentFlags_SingleLine = 64;
        private bool Searched;
        private SortedList ObjectsFound;
        private SortedList PagesFound;
        private FileStream finternalfile;
		private bool AbortingThread;
		private bool FVersion2_2;
		private bool FReading;
		private FlexStream intflexstream;
		private Thread FReadThread;
		private Stream FIntStream;
		private int facount;
		private bool FFinished;
        /// <summary>Maximum number of pages that can contain a MetaFile (32000)</summary>
		public const int MAX_NUMBER_PAGES = 32000;
        /// <summary>ForwardOnly is set to true when there is no need to store processed pages in memory
        /// that is usable for long reports with lot of pages that are printet directly to printer so
        /// the memory consumption is minimized</summary>
		public bool ForwardOnly;
        /// <summary>Indicates if the MetaFile processing have been finished, the report processing can
        /// be done asynchronously</summary>
		public bool Finished
		{
			get
			{
				return FFinished;
			}
		}
        /// <summary>Indicates if the MetaFile is still reading from original stream, the report reading can
        /// be done asynchronously, so you can show the first page while receiving the other pages using a
        /// low bandwidth connection</summary>
        public bool Reading
		{
			get { return FReading; }
		}
        /// <summary>Default page size index in PageSizeArray</summary>
		public int PageSizeIndex;
        /// <summary>Width in twips of the page</summary>
        public int CustomX;
        /// <summary>Height in twips of the page</summary>
        public int CustomY;
        /// <summary>True if the program should open the Pos drawer before printing</summary>
        public bool OpenDrawerBefore;
        /// <summary>True if the program should open the Pos drawer after printing</summary>
        public bool OpenDrawerAfter;
        /// <summary>Page orientation</summary>
        public OrientationType Orientation;
        /// <summary>Default page back color</summary>
        public int BackColor;
        /// <summary>Default paper source, this integer value is understandable by the print driver</summary>
        public int PaperSource;
        /// <summary>Default number of copies to print</summary>
        public int Copies;
        /// <summary>Default lines per inch in the page, have sense only when printing to Pos devices</summary>
        public short LinesPerInch;
        /// <summary>Default collate copies setting</summary>
        public bool CollateCopies;
        /// <summary>Default duplex setting</summary>
        public byte Duplex;
        /// <summary>Printer to select when printing the MetaFile</summary>
        public PrinterSelectType PrinterSelect;
        /// <summary>Default scale type when previewing the MetaFile</summary>
        public AutoScaleType AutoScale;
        /// <summary>Default window size when previewing the MetaFile</summary>
        public PreviewWindowStyleType PreviewWindow;
        /// <summary>True if the about box in preview window should be visible</summary>
        public bool PreviewAbout;
        /// <summary>True if the hard margins should be visible in preview</summary>
        public bool PreviewMargins;
        /// <summary>Collection of pages</summary>
		public MetaPages Pages;
        /// <summary>Event to monitor loading of MetaFile</summary>
		public event MetaFileLoadProgress OnLoadProgress;
        /// <summary>Event to monitor report processing</summary>
        public event MetaFileWorkProgress OnWorkProgress;
        /// <summary>Release resources (database connections if Report Active </summary>
        public event MetaStopWork OnRelease;
        /// <summary>Event to monitor saving of MetaFile</summary>
        public event MetaFileSaveWorkProgress OnSaveWorkProgress;
        /// <summary>Event to monitor user cancels the work</summary>
        public event MetaStopWork OnStopWork;
        /// <summary>Event to monitor error while processing the report</summary>
        public event MetaFileWorkAsyncError OnWorkAsyncError;
        /// <summary>Event to monitor when a page have been requested</summary>
        public event RequestPageEvent OnRequestPage;
        /// <summary>Event to monitor when the printing starts</summary>
        public event BeginPrintOut OnBeginPrint;
        /// <summary>Event to monitor error while loading a report</summary>
        public event MetaFileLoadError OnLoadError;
        /// <summary>Current page index, page being processed actually, the report can be processed asynchronously</summary>
		public int CurrentPage;
        /// <summary>If set to true, the MetaFile will be read asynchronously</summary>
		public bool AsyncReading;
        /// <summary>Default printer fonts selection</summary>
        public PrinterFontsType PrinterFonts;
        public object Clone()
        {
            MetaFile newmetafile = new MetaFile();
            using (System.IO.MemoryStream mstream = new System.IO.MemoryStream())
            {
                this.SaveToStream(mstream, false);
                mstream.Seek(0, SeekOrigin.Begin);
                newmetafile.LoadFromStream(mstream, false);
            }
            return newmetafile;
        }

        /// <summary>
        /// Free resources
        /// </summary>
        virtual public void Dispose()
		{
			Clear();
			if (intflexstream!=null)
			{
				intflexstream.Dispose();
				intflexstream=null;
			}
			if (finternalfile!=null)
			{
				finternalfile.Dispose();
				finternalfile=null;
			}
			if (FSharedStream!=null)
			{
				FSharedStream.Dispose();
				FSharedStream=null;
			}
			if (FSharedStream!=null)
			{
				FSharedStream.Dispose();
				FSharedStream=null;
			}
		}
        /// <summary>
        /// Constructor
        /// </summary>
		public MetaFile()
		{
			// Standard sizes
			CustomX = 12047;
			CustomY = 17039;
			OpenDrawerBefore = false;
			OpenDrawerAfter = false;
			Pages = new MetaPages(this);
			FSharedStream = new MemoryStream();
            ObjectsFound = new SortedList();
            PagesFound = new SortedList();
		}
    /// <summary>
    /// Release resources
    /// </summary>
    public void Release()
    {
      if (OnRelease != null)
        OnRelease();
    }
        /// <summary>
        /// Clear MetaFile, that is clear all pages and objects
        /// </summary>
		public void Clear()
		{
			FFinished = false;
			Pages.Clear();
			FSharedStream.SetLength(0);
		}
        /// <summary>
        /// Forces stop of the asynchronous reading of MetaFile
        /// </summary>
		public void StopWork()
		{
			if (FReading)
			{
				FReading = false;
				if (FReadThread != null)
				{
					AbortingThread = true;
					FReadThread.Abort();
					FReadThread = null;
				}
			}
			if (OnStopWork != null)
				OnStopWork();
		}
        /// <summary>
        /// Fires the event OnWorkProgress
        /// </summary>
        /// <param name="records">Number of records processed</param>
        /// <param name="pagecount">Current page count</param>
        /// <param name="docancel">Set this reference variable to true to cancel report processing</param>
		public void WorkProgress(int records, int pagecount, ref bool docancel)
		{
			if (OnWorkProgress != null)
				OnWorkProgress(records, pagecount, ref docancel);
		}
        /// <summary>
        /// Fires a OnWorkAsyncError
        /// </summary>
        /// <param name="message">Error message</param>
		public void WorkAsyncError(string message)
		{
			if (OnWorkAsyncError != null)
				OnWorkAsyncError(message);
		}
		static private void ReadBuf(Stream astream, ref byte[] buf, int count)
		{
			if (astream.Read(buf, 0, count) < count)
				throw new Exception(Translator.TranslateStr(521));
		}
        /// <summary>
        /// Load a MetaFile from a stream
        /// </summary>
        /// <param name="astream">Source stream</param>
        /// <param name="ClearFirst">Set it to true to clear the MetaFile before reading</param>
		public void LoadFromStream(Stream astream, bool ClearFirst)
		{
			try
			{
				intflexstream = new FlexStream(astream);
				IntLoadFromStream(intflexstream, ClearFirst);
			}
			catch (Exception E)
			{
				if (OnLoadError != null)
					OnLoadError(E.Message);
				throw;
			}
		}
        /// <summary>
        /// Save the MetaFile to a file
        /// </summary>
        /// <param name="filename">File name </param>
        /// <param name="compressed">Set to true to compress the MetaFile</param>
		public void SaveToFile(string filename,bool compressed)
		{
			FileStream fstream = new FileStream(filename, FileMode.Create);
			try
			{
				SaveToStream(fstream, compressed);
			}
			finally
			{
				fstream.Close();
			}
		}
		private void IntSaveToStream(Stream astream)
		{
			// Write the signature
			astream.Write(sign2_4, 0, sign2_4.Length);
			int separator = (int)MetaSeparator.FileHeader;
			astream.Write(StreamUtil.IntToByteArray(separator), 0, 4);
			// Metafile Report header
			astream.Write(StreamUtil.IntToByteArray(PageSizeIndex),0,4);
			astream.Write(StreamUtil.IntToByteArray(CustomX),0,4);
			astream.Write(StreamUtil.IntToByteArray(CustomY),0,4);
			int ainteger=(int)(Orientation);
			astream.Write(StreamUtil.IntToByteArray(ainteger),0,4);
			astream.Write(StreamUtil.IntToByteArray(BackColor),0,4);
			astream.Write(StreamUtil.ShortToByteArray(PaperSource),0,2);
			astream.Write(StreamUtil.ShortToByteArray(Copies),0,2);
			astream.Write(StreamUtil.ShortToByteArray(LinesPerInch),0,2);
			astream.Write(StreamUtil.BoolToByteArray(CollateCopies),0,1);
			astream.Write(StreamUtil.ByteToByteArray(Duplex),0,1);
			ainteger=(int)PrinterSelect;
			astream.Write(StreamUtil.IntToByteArray(ainteger),0,4);
			ainteger=(int)AutoScale;
			astream.Write(StreamUtil.IntToByteArray(ainteger),0,4);
			ainteger=(int)PreviewWindow;
			astream.Write(StreamUtil.IntToByteArray(ainteger),0,4);
			astream.Write(StreamUtil.BoolToByteArray(OpenDrawerBefore),0,1);
			astream.Write(StreamUtil.BoolToByteArray(OpenDrawerAfter),0,1);
			ainteger=0;
			if (PreviewAbout)
				ainteger=1;
			astream.Write(StreamUtil.IntToByteArray(ainteger),0,4);
			ainteger=0;
			if (PreviewMargins)
				ainteger=1;
			astream.Write(StreamUtil.IntToByteArray(ainteger), 0, 4);

			// Now write pages
			// First write pagecount
			astream.Write(StreamUtil.IntToByteArray(Pages.Count), 0, 4);
			for (int i = 0; i < Pages.Count; i++)
			{
				// Write main stream (shared images stream)
				if (i == 0)
				{
					if (FSharedStream.Length > 0)
					{
						separator = (int)MetaSeparator.StreamHeader;
						astream.Write(StreamUtil.IntToByteArray(separator), 0, 4);
						long ssize = FSharedStream.Length;
						astream.Write(StreamUtil.LongToByteArray(ssize), 0, 8);
						FSharedStream.Seek(0, SeekOrigin.Begin);
						FSharedStream.WriteTo(astream);
					}
				}
				separator = (int)MetaSeparator.PageHeader;
				astream.Write(StreamUtil.IntToByteArray(separator), 0, 4);

				// Save the page to the stream
				Pages[i].SaveToStream(astream);

				// Work progress
				if (OnSaveWorkProgress != null)
				{
					bool docancel;
					docancel=false;
					OnSaveWorkProgress(i, Pages.Count, ref docancel);
					if (docancel)
						throw new UnNamedException("Operation aborted");
				}
			}
		}
        /// <summary>
        /// Save the MetaFile to a stream
        /// </summary>
        /// <param name="astream">Destination stream</param>
        /// <param name="compressed">Set it to true to compress the stream</param>
		public void SaveToStream(Stream astream, bool compressed)
		{
			// Check the report is fully calculated or readed
		    RequestPage(MAX_NUMBER_PAGES);
			if (compressed)
			{
#if REPMAN_ZLIB
				ICSharpCode.SharpZipLib.Zip.Compression.Deflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater();
				ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream zstream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(astream, inf, 131072);
				try
				{
					IntSaveToStream(zstream);
				}
				finally
				{
					zstream.Finish();
				}
#else
				throw new Exception("REPMAN_ZLIB not defined. ZLib compression not supported");
#endif
			}
			else
				IntSaveToStream(astream);
		}
		private void IntLoadFromStream(Stream astream, bool ClearFirst)
		{
			if (FReading)
			{
				AbortingThread = true;
				FReadThread.Abort();
				FIntStream = null;
			}
			else
				FReading = true;
			FFinished = false;
			FReadThread = null;
			try
			{
				if (ClearFirst)
					Clear();

				byte[] buf = new byte[13];


				if (astream.Read(buf, 0, 13) < 13)
					throw new Exception(Translator.TranslateStr(520));
				FVersion2_2 = false;
				if (StreamUtil.CompareArrayContent(buf, sign2_2))
					FVersion2_2 = true;
				else
					if (!StreamUtil.CompareArrayContent(buf, sign2_4))
						throw new Exception(Translator.TranslateStr(520));
				ReadBuf(astream, ref buf, 4);
				MetaSeparator separator = MetaSeparator.FileHeader;
				if (StreamUtil.ByteArrayToInt(buf, 4) != (int)separator)
					throw new Exception(Translator.TranslateStr(521));

				ReadBuf(astream, ref buf, 4);
				PageSizeIndex = StreamUtil.ByteArrayToInt(buf, 4);
				ReadBuf(astream, ref buf, 4);
				CustomX = StreamUtil.ByteArrayToInt(buf, 4);
				ReadBuf(astream, ref buf, 4);
				CustomY = StreamUtil.ByteArrayToInt(buf, 4);
				ReadBuf(astream, ref buf, 4);
				Orientation = (OrientationType)StreamUtil.ByteArrayToInt(buf, 4);
				ReadBuf(astream, ref buf, 4);
				BackColor = StreamUtil.ByteArrayToInt(buf, 4);
				ReadBuf(astream, ref buf, 2);
				PaperSource = StreamUtil.ByteArrayToInt(buf, 2);
				ReadBuf(astream, ref buf, 2);
				Copies = StreamUtil.ByteArrayToInt(buf, 2);
				ReadBuf(astream, ref buf, 2);
				LinesPerInch = (short)StreamUtil.ByteArrayToInt(buf, 2);
				ReadBuf(astream, ref buf, 1);
				CollateCopies = StreamUtil.ByteArrayToInt(buf, 1) != 0;
				if (LinesPerInch < 0)
					LinesPerInch = 6;
				ReadBuf(astream, ref buf, 1);
				Duplex = (byte)StreamUtil.ByteArrayToInt(buf, 1);
				ReadBuf(astream, ref buf, 4);
				PrinterSelect = (PrinterSelectType)StreamUtil.ByteArrayToInt(buf, 4);
				ReadBuf(astream, ref buf, 4);
                AutoScale = (AutoScaleType)StreamUtil.ByteArrayToInt(buf, 4);
				ReadBuf(astream, ref buf, 4);
				PreviewWindow = (PreviewWindowStyleType)StreamUtil.ByteArrayToInt(buf, 4);
				ReadBuf(astream, ref buf, 1);
				OpenDrawerBefore = !(StreamUtil.ByteArrayToInt(buf, 1) == 0);
				ReadBuf(astream, ref buf, 1);
				OpenDrawerAfter = !(StreamUtil.ByteArrayToInt(buf, 1) == 0);
				ReadBuf(astream, ref buf, 4);
				PreviewAbout = !(StreamUtil.ByteArrayToInt(buf, 4) == 0);
				ReadBuf(astream, ref buf, 4);
				PreviewMargins = !(StreamUtil.ByteArrayToInt(buf, 4) == 0);
				// Read pagecount
				ReadBuf(astream, ref buf, 4);
				int acount = StreamUtil.ByteArrayToInt(buf, 4);
                if (acount == 0)
                    return;
				ReadBuf(astream, ref buf, 4);
				separator = MetaSeparator.PageHeader;
				bool docancel = false;
				if (OnLoadProgress != null)
					OnLoadProgress(0, acount, ref docancel);
				if (docancel)
					throw new Exception(Translator.TranslateStr(503));
				int bytesread = 4;
				while (bytesread > 0)
				{
                    int newseparator = StreamUtil.ByteArrayToInt(buf, 4);
                    if (newseparator != (int)separator)
					{
						if (newseparator == (int)MetaSeparator.StreamHeader)
						{
							// Stream
							buf = new byte[9];
							ReadBuf(astream, ref buf, 8);
							long asize = StreamUtil.ByteArrayToLong(buf,8);
							if (asize < 0)
								throw new Exception(Translator.TranslateStr(523));
							if (asize > 0)
							{
								buf = new byte[asize];
								ReadBuf(astream, ref buf, (int)asize);
								FSharedStream = new MemoryStream((int)asize);
								FSharedStream.Write(buf, 0, (int)asize);
							}
							buf = new byte[9];
							ReadBuf(astream, ref buf, 4);
                            newseparator = StreamUtil.ByteArrayToInt(buf, 4);
                        }
					}
					if (newseparator!= (int)separator)
					{
						throw new Exception(Translator.TranslateStr(522));
					}
					// Read the main stream
					//if (acount < (Pages.CurrentCount - 1))
					//	throw new Exception(Translator.TranslateStr(522));
					MetaPage apage = new MetaPage(this);
					apage.Version2_2 = FVersion2_2;
					apage.UpdatedPageSize = false;
					apage.Orientation = Orientation;
					apage.PageDetail.Index = PageSizeIndex;
					apage.PageDetail.Custom = true;
					apage.PageDetail.CustomWidth = CustomX;
					apage.PageDetail.CustomHeight = CustomY;
					apage.PageDetail.PhysicWidth = CustomX;
					apage.PageDetail.PhysicHeight = CustomY;
					apage.PageDetail.PaperSource = 0;
					apage.PageDetail.ForcePaperName = "";
					apage.PageDetail.Duplex = 0;

					apage.LoadFromStream(astream);

					Pages.Add(apage);
					if (AsyncReading && (acount > 1))
					{
						docancel = false;
						if (OnLoadProgress != null)
							OnLoadProgress(Pages.CurrentCount, acount, ref docancel);
						if (docancel)
							throw new Exception(Translator.TranslateStr(503));
						FIntStream = astream;
						facount = acount;
						FReadThread = new System.Threading.Thread(new ThreadStart(IntLoadPages));
						try
						{
							AbortingThread = false;
							FReadThread.Start();
						}
						catch (Exception)
						{
							FReadThread = null;
							throw;
						}
						break;
					}
					bytesread = astream.Read(buf, 0, 4);
					if ((bytesread > 0) && (bytesread < 4))
						throw new Exception(Translator.TranslateStr(522));
					docancel = false;
					if (OnLoadProgress != null)
						OnLoadProgress(Pages.CurrentCount, acount, ref docancel);
					if (docancel)
						throw new Exception(Translator.TranslateStr(503));
				}
			}
			finally
			{
				if (FReadThread == null)
				{
					FReading = false;
					FFinished = true;
				}
			}

		}
		void IntLoadPages()
		{
			try
			{
				byte[] buf = new byte[13];
				bool docancel;
				MetaSeparator separator = MetaSeparator.PageHeader;

				int bytesread = FIntStream.Read(buf, 0, 4);
				if ((bytesread > 0) && (bytesread < 4))
					throw new Exception(Translator.TranslateStr(522));
				while (bytesread > 0)
				{
					if (StreamUtil.ByteArrayToInt(buf, 4) != (int)separator)
						throw new Exception(Translator.TranslateStr(522));
					if (facount < (Pages.CurrentCount - 1))
						throw new Exception(Translator.TranslateStr(522));
					MetaPage apage = new MetaPage(this);
					apage.Version2_2 = FVersion2_2;
					apage.UpdatedPageSize = false;
					apage.Orientation = Orientation;
					apage.PageDetail.Index = PageSizeIndex;
					apage.PageDetail.Custom = true;
					apage.PageDetail.CustomWidth = CustomX;
					apage.PageDetail.CustomHeight = CustomY;
					apage.PageDetail.PhysicWidth = CustomX;
					apage.PageDetail.PhysicHeight = CustomY;
					apage.PageDetail.PaperSource = 0;
					apage.PageDetail.ForcePaperName = "";
					apage.PageDetail.Duplex = 0;

					apage.LoadFromStream(FIntStream);

					Pages.Add(apage);
					bytesread = FIntStream.Read(buf, 0, 4);
					if ((bytesread > 0) && (bytesread < 4))
						throw new Exception(Translator.TranslateStr(522));
					docancel = false;
					if (OnLoadProgress != null)
						OnLoadProgress(Pages.CurrentCount, facount, ref docancel);
					if (docancel)
						throw new Exception(Translator.TranslateStr(503));
				}
				FReading = false;
				FFinished = true;
				FIntStream = null;
				intflexstream = null;
				if (finternalfile != null)
				{
					finternalfile.Close();
					finternalfile = null;
				}
			}
			catch (Exception E)
			{
				FReading = false;
				FFinished = false;
				FIntStream = null;
				intflexstream = null;
				if (finternalfile != null)
				{
					finternalfile.Close();
					finternalfile = null;
				}
				// Not throw exceptions when asynchronous reading and message handled?
				if (!AbortingThread)
				{
					if (OnLoadError != null)
						OnLoadError(E.Message);
					else
						throw;
				}
			}
		}
        /// <summary>
        /// Load the stream from a file
        /// </summary>
        /// <param name="filename">File name to load</param>
		public void LoadFromFile(string filename)
		{
			finternalfile = new FileStream(filename, FileMode.Open);
			try
			{
				LoadFromStream(finternalfile, true);
			}
			finally
			{
				if (!FReading)
				{
					if (finternalfile!=null)
					{
						finternalfile.Close();
						finternalfile = null;
					}
				}
			}
		}
        /// <summary>
        /// Updates all references to total pages to the total page count. This is useful to
        /// update the page count when concatenating MetaFiles
        /// </summary>
        /// <param name="alist">Total pages list</param>
		public void UpdateTotalPages(TotalPages alist)
		{
			int index, i;
			TotalPage aobject;
			MetaPage apage;
			string astring;
			int oldtexts;
			for (i = 0; i < alist.Count; i++)
			{
				aobject = alist[i];
				MetaObjectText objx;
				apage = Pages[aobject.PageIndex];
				if (apage != null)
				{
                    objx = (MetaObjectText)apage.Objects[aobject.ObjectIndex];
                    index = objx.TextP-1;
					if (aobject.DisplayFormat.Length > 0)
						astring = Pages.CurrentCount.ToString(aobject.DisplayFormat);
					else
						astring = Pages.CurrentCount.ToString();
					oldtexts = 9;
					objx.TextS = astring.Length;
					astring = astring + "                                      ";
					for (int j = 0; j < oldtexts; j++)
					{
						apage.Pool[index + j] = astring[j];
					}
				}
			}
		}
		static private bool IsDelimiter(char achar)
		{
			bool aresult = false;
			switch (achar)
			{
				case ' ':
				case '.':
				case ',':
				case '-':
				case '/':
				case '\\':
				case '=':
				case ')':
				case '(':
				case '*':
					aresult = true;
					break;
			}
			return aresult;
		}
        /// <summary>
        /// Updates all references to total pages to the total page count. This is useful to
        /// update the page count when concatenating MetaFiles
        /// </summary>
        /// <param name="alist">Total pages list</param>
        /// <param name="pcount">Page count</param>
        public void UpdateTotalPagesPCount(TotalPages alist, int pcount)
		{
			int i, index;
			TotalPage aobject;
			MetaPage apage;
			string astring;
			int oldtexts;
			MetaObjectText objx;

			for (i = 0; i < alist.Count; i++)
			{
				aobject = alist[i];
				apage = Pages[aobject.PageIndex];
				if (apage != null)
				{
                    objx = (MetaObjectText)apage.Objects[aobject.ObjectIndex];
                    index = objx.TextP-1;
					if (aobject.DisplayFormat.Length > 0)
						astring = pcount.ToString(aobject.DisplayFormat);
					else
						astring = pcount.ToString();
					oldtexts = 9;
					objx.TextS = astring.Length;
//					apage.Objects[aobject.ObjectIndex] = objx;
					astring = astring + "                                      ";
					for (int j = 0; j < oldtexts; j++)
					{
						apage.Pool[index + j] = astring[j];
					}
				}
			}
		}
        /// <summary>
        /// Calculates the extent of a text, given a output driver
        /// </summary>
        /// <param name="adriver">Report processing driver</param>
        /// <param name="maxextent">Maximum extent</param>
        /// <param name="objt">Text content and options like font, alignment</param>
        /// <returns>Returns the current text position if the text does not fit in the bounding box</returns>
		public static int CalcTextExtent(PrintOut adriver, Point maxextent,
			TextObjectStruct objt)
		{
			Point newextent;
			int currentpos, lasttested, oldcurrentpos;
			string originalstring;
			int minpos, maxpos;

			currentpos = objt.Text.Length;
			originalstring = objt.Text;
			newextent = maxextent;
			newextent = adriver.TextExtent(objt, newextent);
			lasttested = currentpos;
			minpos = -1;
			maxpos = currentpos;
			oldcurrentpos = -1;
			// Speed enhacement to cut at least lot of size testing
			while (minpos < maxpos)
			{
				// The first test is performed
				currentpos = (minpos + maxpos) / 2;
				// Word Break
				while (currentpos > 0)
				{
					currentpos--;
					if (IsDelimiter(originalstring[currentpos]))
						break;
				}
				if (oldcurrentpos == currentpos)
					break;
				oldcurrentpos = currentpos;
				objt.Text = originalstring.Substring(0, currentpos);
				newextent = maxextent;
				newextent = adriver.TextExtent(objt, newextent);
				if (newextent.Y <= maxextent.Y)
					minpos = currentpos;
				else
				{
					lasttested = currentpos;
					maxpos = lasttested;
				}
			}
			currentpos = lasttested;
			objt.Text = originalstring.Substring(0, currentpos);
			newextent = maxextent;
			newextent = adriver.TextExtent(objt, newextent);
			while (newextent.Y > maxextent.Y)
			{
				while (currentpos > 0)
				{
					currentpos--;
					if (currentpos < 0)
						break;
					if (IsDelimiter(objt.Text[currentpos]))
						break;
				}
				if (currentpos < 0)
					break;
				objt.Text = originalstring.Substring(0, currentpos);
				newextent = maxextent;
				newextent = adriver.TextExtent(objt, newextent);
			}
			if (currentpos < 0)
				return objt.Text.Length;
			else
				return currentpos;
		}
        /// <summary>
        /// Finish the report processing
        /// </summary>
		public void Finish()
		{
			FFinished = true;
		}
        /// <summary>
        /// Finish the report processing
        /// </summary>
        public void UnFinish()
        {
            FFinished = false;
        }
        /// <summary>
        /// Wait until the page requested is already calculated
        /// </summary>
        /// <param name="pageindex">Page index to wait for</param>
        /// <returns>Returns true if the page exists</returns>
		public bool RequestPage(int pageindex)
		{
            if (Empty)
                return true;
			if (Pages.CurrentCount > pageindex)
				return FFinished;
            if (FFinished)
                return true;
			if (OnRequestPage != null)
				FFinished = OnRequestPage(pageindex);
			if (FReading)
			{
				while ((FReading) && (Pages.CurrentCount <= pageindex))
					Thread.Sleep(System.TimeSpan.FromMilliseconds(100));

			}
			return FFinished;
		}
        /// <summary>
        /// Starts printing
        /// </summary>
        /// <param name="driver">Report preocessing driver </param>
		public void BeginPrint(PrintOut driver)
		{
			if (!FFinished)
				if (OnBeginPrint != null)
					OnBeginPrint(driver);
		}
        /// <summary>
        /// Returns true if the object was found in the previous search
        /// </summary>
        /// <param name="aobject">MetaObject to search</param>
        public bool ObjectFound(MetaObject aobject)
        {
            if (!Searched)
                return false;
            string tosearch = aobject.Id.ToString("0000000000000000000");
            return ObjectsFound[tosearch] != null;
        }
        /// <summary>
        /// Returns the next page index where the last search occur
        /// </summary>
        public int NextPageFound(int currpage)
        {
            string tosearch;
            int index;
            if (!Searched)
                return 0;
            if (PagesFound.Count==0)
                return 0;
            tosearch = currpage.ToString("00000000");
            index=PagesFound.IndexOfKey(tosearch);
            if (index>=PagesFound.Count-1)
            {
                index=0;
            }
            else
                index++;
            return System.Convert.ToInt32(PagesFound.GetKey(index));
        }
        /// <summary>
        /// Executes a word search in the metafile, a list of coincidences will be updated
        /// </summary>
        /// <param name="svalue">String value to search in the entire MetaFile</param>
        public void DoSearch(string svalue)
        {
            ObjectsFound.Clear();
            PagesFound.Clear();
            svalue = svalue.ToUpper();
            if (svalue.Length == 0)
            {
                Searched = false;
                return;
            }
            Searched = true;
            int id = 1;

            MetaPage apage;
            MetaObject aobj;
            RequestPage(MAX_NUMBER_PAGES);


            bool foundinpage;
            svalue = svalue.ToUpper();
            for (int i=0;i<Pages.Count;i++)
            {
                apage=Pages[i];
                foundinpage = false;
                for (int j=0;j<apage.Objects.Count;j++)
                {
                    aobj=apage.Objects[j];
                    if (aobj.MetaType == MetaObjectType.Text)
                    {
                        aobj.Id = 0;
                        string atext = apage.GetText((MetaObjectText)aobj).ToUpper();
                        if (atext.IndexOf(svalue) >= 0)
                        {
                            aobj.Id = id;
                            id++;
                            foundinpage = true;
                            ObjectsFound.Add(aobj.Id.ToString("0000000000000000000"), aobj);
                        }
                    }
                }
                if (foundinpage)
                    PagesFound.Add(i.ToString("00000000"),apage);
            }
        }
        public void AddMetaFiles(List<MetaFile> nlist)
        {
            foreach (MetaFile nf in nlist)
            {
                using (MemoryStream nstream = new MemoryStream())
                {
                    nf.SaveToStream(nstream, false);
                    nstream.Seek(0, SeekOrigin.Begin);
                    LoadFromStream(nstream, false);
                }
            }
        }
	}
	class FlexStream : Stream
	{
		const int UNCOMP_BUF_SIZE = 100000;
		MetaStream metas;
#if REPMAN_ZLIB
		long FPosition;
		bool feof;
		byte[] bufuncomp;
		MemoryStream mems;
		ICSharpCode.SharpZipLib.Zip.Compression.Inflater inf;
		InflaterInputStream zstream;
#endif
		public FlexStream(Stream stream)
			: base()
		{
			metas = new MetaStream(stream);
#if REPMAN_ZLIB
			if (metas.Compressed)
			{
				inf = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
				zstream = new InflaterInputStream(metas, inf);
				bufuncomp = new byte[UNCOMP_BUF_SIZE];
				mems = new MemoryStream();
			}
#endif
		}
		override public bool CanRead
		{
			get { return metas.CanRead; }
		}
		override public bool CanWrite
		{
			get { return false; }
		}
		override public bool CanSeek
		{
			get { return false; }
		}
		override public long Length
		{
			get { throw new Exception("Property not supported Length - FlexStream"); }
		}
		override public long Position
		{
			get { throw new Exception("Property not supported Position - FlexStream"); }
			set { throw new Exception("Property not supported Position - FlexStream"); }
		}
		override public void Flush()
		{
			throw new Exception("Method not supported Flush - FlexStream");
		}
		override public void SetLength(long asize)
		{
			throw new Exception("Method not supported SetLength - FlexStream");
		}
		override public long Seek(long index, System.IO.SeekOrigin origin)
		{
			throw new Exception("Method not supported Seek - FlexStream");
		}
		override public void Write(byte[] buf, int index, int count)
		{
			throw new Exception("Method not supported Write - FlexStream");
		}
		override public int Read(byte[] buf, int index, int count)
		{
			if (!metas.Compressed)
				return metas.Read(buf, index, count);
#if REPMAN_ZLIB
			{
				mems.Seek(FPosition, System.IO.SeekOrigin.Begin);
				int partial = 0;
				int readed = mems.Read(buf, 0, count);
				while ((readed > 0) || (!feof))
				{
					partial = partial + readed;
					FPosition = FPosition + readed;
					if (partial == count)
						break;
					int needed = count - partial;
					readed = zstream.Read(bufuncomp, 0, UNCOMP_BUF_SIZE);
					while ((readed > 0))
					{
						mems.Write(bufuncomp, 0, readed);
						if (needed <= 0)
							break;
						readed = zstream.Read(bufuncomp, 0, UNCOMP_BUF_SIZE);
						needed = needed - readed;
					}
					if (readed == 0)
						feof = true;
					mems.Seek(FPosition, System.IO.SeekOrigin.Begin);
					readed = mems.Read(buf, index + partial, count - partial);
				}
				return partial;
			}
#else
			throw new Exception("REPMAN_ZLIB not defined. ZLib compression not supported");
#endif
		}
	}
}
