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
using System.Threading;
using System.Collections.Generic;


namespace Reportman.Drawing
{
    /// <summary>
    /// Event launched when there is no data available to print. Should return
    /// true if the action have been handled and no futher processing must happen
    /// </summary>
    /// <param name="pageindex">Page requested</param>
    /// <returns></returns>
    public delegate bool NoDataToPrintEvent();
    /// <summary>
    /// Event triggered when a driver starts printing
    /// </summary>
    /// <param name="driver"></param>
	public delegate void BeginPrintOut(PrintOut driver);
    /// <summary>
    /// Base class for print drivers define the funtions to be implemented by
    /// any print driver (Report processing driver)
    /// </summary>
	public abstract class PrintOut
	{
        public static string DefaultPrinterName = "";
        public string ForcePrinterName = "";
        public bool Previewing;
        /// <summary>
        /// Array of predefined page sizes
        /// </summary>
  		public static int[,] PageSizeArray=new int[149,2]
    		{
      { 8268, 11693},  // psA4
      { 7165, 10118},  // psB5
      { 8500, 11000},  // psLetter
      { 8500, 14000},  // psLegal
      { 7500, 10000},  // psExecutive
      { 33110, 46811}, // psA0
      { 23386, 33110}, // psA1
      { 16535, 23386}, // psA2
      { 11693, 16535}, // psA3
      { 5827, 8268},   // psA5
      { 4134, 5827},   // psA6
      { 2913, 4134},   // psA7
      { 2047, 2913},   // psA8
      { 1457, 2047},   // psA9
      { 40551, 57323}, // psB0
      { 28661, 40551}, // psB1
      { 1260, 1772},   // psB10
      { 20276, 28661}, // psB2
      { 14331, 20276}, // psB3
      { 10118, 14331}, // psB4
      { 5039, 7165},   // psB6
      { 3583, 5039},   // psB7
      { 2520, 3583},   // psB8
      { 1772, 2520},   // psB9
      { 6417, 9016},   // psC5E
      { 4125, 9500},   // psComm10E
      { 4331, 8661},   // psDLE
      { 8250, 13000},  // psFolio
      { 17000, 11000}, // psLedger
      { 11000, 17000}, // psTabloid
      { -1, -1},        // psNPageSize
                                    // Windows equivalents begins at 31
      { 8500, 11000}, // Letter 8 12 x 11 in
      { 8500, 11000}, // Letter Small 8 12 x 11 in
      { 11000, 17000},  // Tabloid 11 x 17 in
      { 17000, 11000},  // Ledger 17 x 11 in
      { 8500, 14000},  // Legal 8 12 x 14 in
      { 55000, 8500},  // Statement 5 12 x 8 12 in
      { 7500, 10500}, // Executive 7 14 x 10 12 in
      { 11693, 16535}, // A3 297 x 420 mm                     
      { 8268, 11693},      // A4 210 x 297 mm                     
      { 8268, 11693},// A4 Small 210 x 297 mm               
      { 5827, 8268}, // A5 148 x 210 mm                     
      { 10118, 14331},    // B4 (JIS) 250 x 354                  
      { 7165, 10118}, // B5 (JIS) 182 x 257 mm               
      { 8250, 13000}, // Folio 8 12 x 13 in                  
      { 8465, 10827}, // Quarto 215 x 275 mm                 
      { 10000, 14000}, // 10x14 in                            
    { 11000, 17000},// 11x17 in                            
    { 8500, 11000}, // Note 8 12 x 11 in                  
    { 3875, 8875},// Envelope #9 3 78 x 8 78             
    { 4125, 9500},// Envelope #10 4 18 x 9 12            
    { 4500, 10375},// Envelope #11 4 12 x 10 38           
    { 4276, 11000},// Envelope #12 4 \276 x 11            
    { 5000, 11500},// Envelope #14 5 x 11 12              
    { 16969, 21969},// C size sheet 431 x 558 mm                       
    { 21969, 33976},// D size sheet 558 x 863 mm                      
    { 33976, 43976},// E size sheet 863 x 1117 mm                       
    { 4331, 8661},// Envelope DL 110 x 220mm             
    { 6378, 9016},// Envelope C5 162 x 229 mm            
    { 12756, 18031},// Envelope C3  324 x 458 mm           
    { 9016, 12756},// Envelope C4  229 x 324 mm           
    { 4488, 6378},// Envelope C6  114 x 162 mm           
    { 4488, 9016},// Envelope C65 114 x 229 mm           
    { 9843, 13898},// Envelope B4  250 x 353 mm           
    { 6929, 9843},// Envelope B5  176 x 250 mm           
    { 6929, 4921},// Envelope B6  176 x 125 mm           
    { 4331, 9056},// Envelope 110 x 230 mm               
    { 3875, 7500}, // Envelope Monarch 3.875 x 7.5 in     
    { 3625, 6500},// 6 34 Envelope 3 58 x 6 12 in        
    { 14875, 11000},// US Std Fanfold 14 78 x 11 in        
    { 8500, 12000},// German Std Fanfold 8 12 x 12 in    
    { 8500, 13000},// German Legal Fanfold 8 12 x 13 in  
    { 9843, 13898},// B4 (ISO) 250 x 353 mm               
    { 3937, 5827},// Japanese Postcard 100 x 148 mm      
    { 9000, 11000}, // 9 x 11 in                           
    { 10000, 11000}, // 10 x 11 in                          
    { 15000, 11000}, // 15 x 11 in                          
    { 8661, 8661}, // Envelope Invite 220 x 220 mm        
    { 100, 100}, // RESERVED--DO NOT USE                
    { 100, 100}, // RESERVED--DO NOT USE                
    { 9275, 12000}, // Letter Extra 9 \275 x 12 in         
    { 9275, 15000}, // Legal Extra 9 \275 x 15 in          
    { 11690, 18000}, // Tabloid Extra 11.69 x 18 in         
    { 9270, 12690}, // A4 Extra 9.27 x 12.69 in            
    { 8275, 11000},  // Letter Transverse 8 \275 x 11 in    
    { 8268, 11693},  // A4 Transverse 210 x 297 mm          
    { 9275, 12000}, // Letter Extra Transverse 9\275 x 12 in  
    { 8937, 14016},     // SuperASuperAA4 227 x 356 mm      
    { 12008, 19172},    // SuperBSuperBA3 305 x 487 mm       
    { 8500, 12690},    // Letter Plus 8.5 x 12.69 in          
    { 8268, 12992},    // A4 Plus 210 x 330 mm                
    { 5828, 8268},    // A5 Transverse 148 x 210 mm          
    { 7166, 10118},    // B5 (JIS) Transverse 182 x 257 mm    
    { 13071, 17520},    // A3 Extra 322 x 445 mm               
    { 6850, 9252},    // A5 Extra 174 x 235 mm               
    { 7913, 10867},    // B5 (ISO) Extra 201 x 276 mm         
    { 16536, 23386},    // A2 420 x 594 mm                     
    { 11693, 16535},    // A3 Transverse 297 x 420 mm          
    { 13071, 17520},     // A3 Extra Transverse 322 x 445 mm    
    { 7874, 5827}, // Japanese Double Postcard 200 x 148 mm 
    { 4173,5827},  // A6 105 x 148 mm                 }
    { 9449, 13071},  // Japanese Envelope Kaku #2 240 x 332 mm       
    { 8504, 10906},  // Japanese Envelope Kaku #3 216 x 277 mm     
    { 4724, 9252},  // Japanese Envelope Chou #3 120 x 235 mm      
    { 3543, 8071},  // Japanese Envelope Chou #4  90 x 205 mm    
    { 11000, 8500},  // Letter Rotated 11 x 8 1/2 11 in 
    { 16535, 11693},  // A3 Rotated 420 x 297 mm         
    { 11693, 8268},  // A4 Rotated 297 x 210 mm         
    { 8268, 5828},  // A5 Rotated 210 x 148 mm         
    { 14331, 10118},  // B4 (JIS) Rotated 364 x 257 mm   
    { 10118, 7165},  // B5 (JIS) Rotated 257 x 182 mm   
    { 5827, 3937}, // Japanese Postcard Rotated 148 x 100 mm 
    { 5827, 7874}, // Double Japanese Postcard Rotated 148 x 200 mm 
    { 5827, 4173}, // A6 Rotated 148 x 105 mm         
    { 13071, 9449},  // Japanese Envelope Kaku #2 Rotated
    { 10906, 8504},  // Japanese Envelope Kaku #3 Rotated
    { 9252, 4724},  // Japanese Envelope Chou #3 Rotated
    { 8071, 3543},  // Japanese Envelope Chou #4 Rotated
    { 5039, 7165},  // B6 (JIS) 128 x 182 mm           
    { 7165, 5039},  // B6 (JIS) Rotated 182 x 128 mm   
    { 12000, 11000},  // 12 x 11 in                      
    { 4134, 9252},  // Japanese Envelope You #4 105 x 235 mm       
    { 9252, 4134},  // Japanese Envelope You #4 Rotated
    { 5748, 8465},  // PRC 16K 146 x 215 mm            
    { 3819, 5945},  // PRC 32K 97 x 151 mm             
    { 3819, 5945},  // PRC 32K(Big) 97 x 151 mm        
    { 4134, 6496},  // PRC Envelope #1 102 x 165 mm    
    { 4134, 6929},  // PRC Envelope #2 102 x 176 mm    
    { 4921, 5929},  // PRC Envelope #3 125 x 176 mm    
    { 4331, 8189},  // PRC Envelope #4 110 x 208 mm    
    { 4331, 8661}, // PRC Envelope #5 110 x 220 mm    
    { 4724, 9055}, // PRC Envelope #6 120 x 230 mm    
    { 6299, 9055}, // PRC Envelope #7 160 x 230 mm    
    { 4724, 12165}, // PRC Envelope #8 120 x 309 mm    
    { 9016, 12756}, // PRC Envelope #9 229 x 324 mm    
    { 12756, 18031}, // PRC Envelope #10 324 x 458 mm   
    { 8465, 5748}, // PRC 16K Rotated                 
    { 5945, 3819}, // PRC 32K Rotated                 
    { 5945, 3819}, // PRC 32K(Big) Rotated            
    { 6496, 4134}, // PRC Envelope #1 Rotated 165 x 102 mm
    { 6929, 4134}, // PRC Envelope #2 Rotated 176 x 102 mm
    { 6929, 4921}, // PRC Envelope #3 Rotated 176 x 125 mm
    { 8189, 4331}, // PRC Envelope #4 Rotated 208 x 110 mm
    { 8661, 4331}, // PRC Envelope #5 Rotated 220 x 110 mm
    { 9055, 4724}, // PRC Envelope #6 Rotated 230 x 120 mm
    { 9055, 6299}, // PRC Envelope #7 Rotated 230 x 160 mm
    { 12165, 4724}, // PRC Envelope #8 Rotated 309 x 120 mm
    { 12756, 9016}, // PRC Envelope #9 Rotated 324 x 229 mm
    { 18031, 12756} // PRC Envelope #10 Rotated 458 x 324 mm

		};
        /// <summary>
        /// Returns page size name for provided index
        /// </summary>
        /// <param name="index">Index inside page size array</param>
        /// <returns>String result, containing page size name</returns>
        public static string PageSizeName(int index)
        {
            string aresult = "";
            switch (index)
            {
                case 0:
                    aresult = "A4";
                    break;
                case 1:
                    aresult = "B5";
                    break;
                case 2:
                    aresult = "Letter";
                    break;
                case 3:
                    aresult = "Legal";
                    break;
                case 4:
                    aresult = "Executive";
                    break;
                case 5:
                    aresult = "A0";
                    break;
                case 6:
                    aresult = "A1";
                    break;
                case 7:
                    aresult = "A2";
                    break;
                case 8:
                    aresult = "A3";
                    break;
                case 9:
                    aresult = "A5";
                    break;
                case 10:
                    aresult = "A6";
                    break;
                case 11:
                    aresult = "A7";
                    break;
                case 12:
                    aresult = "A8";
                    break;
                case 13:
                    aresult = "A9";
                    break;
                case 14:
                    aresult = "B0";
                    break;
                case 15:
                    aresult = "B1";
                    break;
                case 16:
                    aresult = "B10";
                    break;
                case 17:
                    aresult = "B2";
                    break;
                case 18:
                    aresult = "B3";
                    break;
                case 19:
                    aresult = "B4";
                    break;
                case 20:
                    aresult = "B6";
                    break;
                case 21:
                    aresult = "B7";
                    break;
                case 22:
                    aresult = "B8";
                    break;
                case 23:
                    aresult = "B9";
                    break;
                case 24:
                    aresult = "C5E";
                    break;
                case 25:
                    aresult = "C10E";
                    break;
                case 26:
                    aresult = "DLE";
                    break;
                case 27:
                    aresult = "Folio";
                    break;
                case 28:
                    aresult = "Ledger";
                    break;
                case 29:
                    aresult = "Tabloid";
                    break;
                case 30:
                    aresult = "Custom";
                    break;
                // Windows
                case 31:
                    aresult = "Letter";
                    break;
                // Windows
                case 32:
                    aresult = "Letter Small";
                    break;
                case 33:
                    aresult = "Tabloid";
                    break;
                case 34:
                    aresult = "Ledger";
                    break;
                case 35:
                    aresult = "Legal";
                    break;
                case 36:
                    aresult = "Statement";
                    break;
                case 37:
                    aresult = "Executive";
                    break;
                case 38:
                    aresult = "A3";
                    break;
                case 39:
                    aresult = "A4";
                    break;
                case 40:
                    aresult = "A4 Small";
                    break;
                case 41:
                    aresult = "A5";
                    break;
                case 42:
                    aresult = "B4";
                    break;
                case 43:
                    aresult = "B5";
                    break;
                case 44:
                    aresult = "Folio";
                    break;
                case 45:
                    aresult = "Quarto";
                    break;
                case 46:
                    aresult = "10x14 in";
                    break;
                case 47:
                    aresult = "11x17 in";
                    break;
                case 48:
                    aresult = "Note";
                    break;
                case 49:
                    aresult = "Envelope #9";
                    break;
                case 50:
                    aresult = "Envelope #10";
                    break;
                case 51:
                    aresult = "Envelope #11";
                    break;
                case 52:
                    aresult = "Envelope #12";
                    break;
                case 53:
                    aresult = "Envelope #14";
                    break;
                case 54:
                    aresult = "C";
                    break;
                case 55:
                    aresult = "D";
                    break;
                case 56:
                    aresult = "E";
                    break;
                case 57:
                    aresult = "Envelope DL";
                    break;
                case 58:
                    aresult = "Envelope C5";
                    break;
                case 59:
                    aresult = "Envelope C3";
                    break;
                case 60:
                    aresult = "Envelope C4";
                    break;
                case 61:
                    aresult = "Envelope C6";
                    break;
                case 62:
                    aresult = "Envelope C65";
                    break;
                case 63:
                    aresult = "Envelope B4";
                    break;
                case 64:
                    aresult = "Envelope B5";
                    break;
                case 65:
                    aresult = "Envelope B6";
                    break;
                case 66:
                    aresult = "Envelope";
                    break;
                case 67:
                    aresult = "Envelope Monarch";
                    break;
                case 68:
                    aresult = "Envelope 6 34";
                    break;
                case 69:
                    aresult = "US Std Fanfold";
                    break;
                case 70:
                    aresult = "German Std Fanfold";
                    break;
                case 71:
                    aresult = "German Legal Fanfold";
                    break;
                case 72:
                    aresult = "B4 ISO";
                    break;
                case 73:
                    aresult = "Japanese Postcard";
                    break;
                case 74:
                    aresult = "9x11 in";
                    break;
                case 75:
                    aresult = "10x11 in";
                    break;
                case 76:
                    aresult = "15x11 in";
                    break;
                case 77:
                    aresult = "Envelope invite";
                    break;
                case 78:
                    aresult = "---";
                    break;
                case 79:
                    aresult = "---";
                    break;
                case 80:
                    aresult = "Letter Extra 1";
                    break;
                case 81:
                    aresult = "Letter Extra 2";
                    break;
                case 82:
                    aresult = "Tabloid Extra";
                    break;
                case 83:
                    aresult = "A4 Extra";
                    break;
                case 84:
                    aresult = "Letter trans.";
                    break;
                case 85:
                    aresult = "A4 trans.";
                    break;
                case 86:
                    aresult = "Letter Extra trans.";
                    break;
                case 87:
                    aresult = "SuperASuperAA4";
                    break;
                case 88:
                    aresult = "SuperVSuperBA3";
                    break;
                case 89:
                    aresult = "Letter Plus";
                    break;
                case 90:
                    aresult = "A4 Plus";
                    break;
                case 91:
                    aresult = "A5 trans.";
                    break;
                case 92:
                    aresult = "B5 trans.";
                    break;
                case 93:
                    aresult = "A3 Extra";
                    break;
                case 94:
                    aresult = "A5 Extra";
                    break;
                case 95:
                    aresult = "B5 Extra";
                    break;
                case 96:
                    aresult = "A2";
                    break;
                case 97:
                    aresult = "A3 trans.";
                    break;
                case 98:
                    aresult = "A3 Extra trans.";
                    break;
                case 99:
                    aresult = "Japanese Double Postcard";
                    break;
                case 100:
                    aresult = "A6";
                    break;
                case 101:
                    aresult = "Japanese Envelope Kaku #2";
                    break;
                case 102:
                    aresult = "Japanese Envelope Kaku #3";
                    break;
                case 103:
                    aresult = "Japanese Envelope Chou #3";
                    break;
                case 104:
                    aresult = "Japanese Envelope Chou #4";
                    break;
                case 105:
                    aresult = "Letter Rotated";
                    break;
                case 106:
                    aresult = "A3 Rotated";
                    break;
                case 107:
                    aresult = "A4 Rotated";
                    break;
                case 108:
                    aresult = "A5 Rotated";
                    break;
                case 109:
                    aresult = "B4 Rotated";
                    break;
                case 110:
                    aresult = "B5 Rotated";
                    break;
                case 111:
                    aresult = "Japanese Postcard Rotated";
                    break;
                case 112:
                    aresult = "Double Japanese Postcard Rotated";
                    break;
                case 113:
                    aresult = "A6 Rotated";
                    break;
                case 114:
                    aresult = "Japanese Envelope Kaku #2 Rotated";
                    break;
                case 115:
                    aresult = "Japanese Envelope Kaku #3 Rotated";
                    break;
                case 116:
                    aresult = "Japanese Envelope Chou #3 Rotated";
                    break;
                case 117:
                    aresult = "Japanese Envelope Chou #4 Rotated";
                    break;
                case 119:
                    aresult = "B6";
                    break;
                case 120:
                    aresult = "B6 Rotated";
                    break;
                case 121:
                    aresult = "12x11 in";
                    break;
                case 122:
                    aresult = "Japanese Envelope You #4";
                    break;
                case 123:
                    aresult = "Japanese Envelope You #4 Rotated";
                    break;
                case 124:
                    aresult = "PRC 16K";
                    break;
                case 125:
                    aresult = "PRC 32K";
                    break;
                case 126:
                    aresult = "PRC 32K (Big)";
                    break;
                case 127:
                    aresult = "PRC Envelope #1";
                    break;
                case 128:
                    aresult = "PRC Envelope #2";
                    break;
                case 129:
                    aresult = "PRC Envelope #3";
                    break;
                case 130:
                    aresult = "PRC Envelope #4";
                    break;
                case 131:
                    aresult = "PRC Envelope #5";
                    break;
                case 132:
                    aresult = "PRC Envelope #6";
                    break;
                case 133:
                    aresult = "PRC Envelope #7";
                    break;
                case 134:
                    aresult = "PRC Envelope #8";
                    break;
                case 135:
                    aresult = "PRC Envelope #9";
                    break;
                case 136:
                    aresult = "PRC Envelope #10";
                    break;
                case 137:
                    aresult = "PRC 16K Rotated";
                    break;
                case 138:
                    aresult = "PRC 32K Rotated";
                    break;
                case 139:
                    aresult = "PRC 32K (Big) Rotated";
                    break;
                case 140:
                    aresult = "PRC Envelope #1 Rotated";
                    break;
                case 141:
                    aresult = "PRC Envelope #2 Rotated";
                    break;
                case 142:
                    aresult = "PRC Envelope #3 Rotated";
                    break;
                case 143:
                    aresult = "PRC Envelope #4 Rotated";
                    break;
                case 144:
                    aresult = "PRC Envelope #5 Rotated";
                    break;
                case 145:
                    aresult = "PRC Envelope #6 Rotated";
                    break;
                case 146:
                    aresult = "PRC Envelope #7 Rotated";
                    break;
                case 147:
                    aresult = "PRC Envelope #8 Rotated";
                    break;
                case 148:
                    aresult = "PRC Envelope #9 Rotated";
                    break;
                case 149:
                    aresult = "PRC Envelope #10 Rotated";
                    break;
                default:
                    aresult = "Unknown";
                    break;
            }
            return aresult;
        }
        /// <summary>
        /// Maximum number of pages allowed (this is to avoid stack overflow recursions)
        /// </summary>
		protected int PRINTOUT_MAX_PAGES = 100000;
        /// <summary>
        /// Starting page, by default 1
        /// </summary>
        public int FromPage;
        /// <summary>
        /// End page, by default, the maximum
        /// </summary>
        public int ToPage;
        /// <summary>
        /// Number of copies, by default 1
        /// </summary>
        public int Copies;
        /// <summary>
        /// Default orientation, by default Portrait
        /// </summary>
        protected OrientationType FOrientation;
        /// <summary>
        /// Throw exception when there is no data available to print
        /// </summary>
        public bool EmptyReportThrowException;
        /// <summary>
        /// Event to control 
        /// </summary>
        public NoDataToPrintEvent NoData;
        /// <summary>
        /// Internally determines if the searched texts must be drawn as a selection.
        /// Used in preview.
        /// </summary>
        protected bool DrawFound;
        /// <summary>
        /// Constructor, set default values only
        /// </summary>
        protected PrintOut()
		{
            DrawFound = true;
			FromPage = 1;
			ToPage = PRINTOUT_MAX_PAGES;
			Copies = 0;
			FOrientation=OrientationType.Portrait;
		}
        /// <summary>
        /// Free memory consumed by graphics resources
        /// </summary>
        public virtual void Dispose()
        {

        }
        /// <summary>
        /// The driver should do initialization here, a print driver should start a print document,
        /// while a preview driver should initialize a bitmap
        /// </summary>
        public virtual void NewDocument(MetaFile meta)
        {
        }
        /// <summary>
        /// The driver should do cleanup here, a print driver should finish print document.
        /// </summary>
        public virtual void EndDocument(MetaFile meta)
        {
        }
        /// <summary>
        /// The driver should start a new page
        /// </summary>
        public virtual void NewPage(MetaFile meta, MetaPage page)
        {
        }
        /// <summary>
        /// The driver should end current page
        /// </summary>
        public virtual void EndPage(MetaFile meta)
        {
        }
        /// <summary>
        /// The driver should draw the page
        /// </summary>
        public abstract void DrawPage(MetaFile meta, MetaPage page);
        /// <summary>
        /// The driver must return the text extent in twips
        /// </summary>
        public abstract Point TextExtent(TextObjectStruct aobj, Point extent);
        /// <summary>
        /// The driver must return the image extent in twips
        /// </summary>
        public abstract Point GraphicExtent(MemoryStream astream, Point extent,
			int dpi);
        /// <summary>
        /// The driver must select a printer
        /// </summary>
        public virtual void SelectPrinter(PrinterSelectType PrinterSelect)
        {
        }
        /// <summary>
        /// Sets page orientation
        /// </summary>
        virtual public void SetOrientation(OrientationType PageOrientation)
        {
            FOrientation = PageOrientation;
        }
        virtual public void DrawChart(Series nseries, MetaFile ametafile, int posx, int posy, object achart)
        {

        }
        /// <summary>
        /// The driver must set a new page size
        /// </summary>
        public abstract Point SetPageSize(PageSizeDetail psize);
        /// <summary>
        /// The driver must return the default page size
        /// </summary>
        public abstract Point GetPageSize(out int indexqt);
        /// <summary>
        /// Print initialization, it calls Metafile.BeginPrint, and NewDocument for this driver
        /// </summary>
        virtual public bool PreparePrint(MetaFile meta)
		{
            bool prepareresult = false;
            bool shownodata = false;
            try
            {
                meta.BeginPrint(this);
                prepareresult = true;
            }
            catch (Exception E)
            {
                if (!EmptyReportThrowException)
                {
                     if (E is NoDataToPrintException)
                         shownodata = true;
                     else
                         throw;

                }
                     else
                         throw;
            }
            if (shownodata)
            {
                if (NoData != null)
                {
                    if (!NoData())
                    {
                        ProcessNoDataToPrint();
                    }
                }
                else
                    ProcessNoDataToPrint();
            }
			if (prepareresult)
                NewDocument(meta);
            return prepareresult;
		}
        /// <summary>
        /// This procedure is called when the user does not handle the
        /// no data to print event. Depending on the ouput driver the output
        /// will be a message to the user.
        /// </summary>
        virtual protected void ProcessNoDataToPrint()
        {

        }
        /// <summary>
        /// Print method, it the PreparePrint, the driver should override this to perform additional
        /// actions
        /// </summary>
        virtual public bool Print(MetaFile meta)
		{
            bool aresult = PreparePrint(meta);
            return aresult;
		}

	}
    public enum PrinterRawOperation  { CutPaper,OpenDrawer,LineFeed,CR,FF,TearOff,InitPrinter,Pulse,EndPrint,RedFont,BlackFont,
        Normal,Bold,Underline,Italic, StrikeOut, LineSpace6,LineSpace8,LineSpace7_72,LineSpacen_216,LineSpacen_180,Linespacen_60,
        cpi20,cpi17,cpi15,cpi12,cpi10,cpi6,cpi5};


}
