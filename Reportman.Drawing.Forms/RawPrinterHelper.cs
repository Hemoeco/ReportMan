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
using System.Drawing.Printing;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using Reportman.Drawing;

namespace Reportman.Drawing.Forms
{
    /// <summary>
    /// Utility class to send raw data to a printer queue, becase .Net library does not provide this functionalyty,
    /// it uses the pinvoke to call library functions
    /// </summary>
	public class RawPrinterHelper
	{
		// Structure and API declarions:
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		private class DOCINFOA
		{
			[MarshalAs(UnmanagedType.LPStr)]
			public string pDocName;
			[MarshalAs(UnmanagedType.LPStr)]
			public string pOutputFile;
			[MarshalAs(UnmanagedType.LPStr)]
			public string pDataType;
		}
		[DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, long pd);

		[DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool ClosePrinter(IntPtr hPrinter);

		[DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

		[DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool EndDocPrinter(IntPtr hPrinter);

		[DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool StartPagePrinter(IntPtr hPrinter);

		[DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool EndPagePrinter(IntPtr hPrinter);

		[DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 byteCount, out Int32 dwWritten);

		// SendBytesToPrinter()
		// When the function is given a printer name and an unmanaged array
		// of bytes, the function sends those bytes to the print queue.
		// Returns true on success, false on failure.
		// TODO: See whay this does not compile on Mono
		// [CustomPermissionAttribute(SecurityAction.LinkDemand)]
		private static bool SendIntBytesToPrinter(string printerName, IntPtr pBytes, Int32 byteCount)
		{
			Int32 dwError;
			Int32 dwWritten = 0;
			IntPtr hPrinter = new IntPtr(0);
			DOCINFOA di = new DOCINFOA();
			bool bSuccess = false; // Assume failure unless you specifically succeed.

			di.pDocName = "Raw Doc";
			di.pDataType = "RAW";

			// Open the printer.
			if (OpenPrinter(printerName, out hPrinter, 0))
			{
				// Start a document.
				if (StartDocPrinter(hPrinter, 1, di))
				{
					// Start a page.
					if (StartPagePrinter(hPrinter))
					{
						// Write your bytes.
						bSuccess = WritePrinter(hPrinter, pBytes, byteCount, out dwWritten);
						EndPagePrinter(hPrinter);
					}
					EndDocPrinter(hPrinter);
				}
				ClosePrinter(hPrinter);
			}
			// If you did not succeed, GetLastError may give more information
			// about why not.
			if (bSuccess == false)
			{
				dwError = Marshal.GetLastWin32Error();
				throw new NamedException("SendBytesToPrinter failed, Win32 Error:"+
					dwError.ToString(),dwError.ToString());
			}
			return bSuccess;
		}
		// TODO: See whay this does not compile on Mono
		// [CustomPermissionAttribute(SecurityAction.LinkDemand)]
        /// <summary>
        /// Sends a full file to the printer in raw mode, useful for dot matrix printers or pos device
        /// </summary>
        /// <param name="printerName">Printer name</param>
        /// <param name="fileName">Full parh to the file to send</param>
        /// <returns>Returns true if sucessful</returns>
		public static bool SendFileToPrinter(string printerName, string fileName)
		{
			// Open the file.
			FileStream fs = new FileStream(fileName, FileMode.Open);
			// Create a BinaryReader on the file.
			BinaryReader br = new BinaryReader(fs);
			// Dim an array of bytes big enough to hold the file's contents.
			Byte[] bytes = new Byte[fs.Length];
			bool bSuccess = false;
			// Your unmanaged pointer.
			IntPtr pUnmanagedBytes = new IntPtr(0);
			int nLength;

			nLength = Convert.ToInt32(fs.Length);
			// Read the contents of the file into the array.
			bytes = br.ReadBytes(nLength);
			// Allocate some unmanaged memory for those bytes.
			pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
			// Copy the managed byte array into the unmanaged array.
			Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
			// Send the unmanaged bytes to the printer.
			bSuccess = SendIntBytesToPrinter(printerName, pUnmanagedBytes, nLength);
			// Free the unmanaged memory that you allocated earlier.
			Marshal.FreeCoTaskMem(pUnmanagedBytes);
			return bSuccess;
		}
		// TODO: See whay this does not compile on Mono
		// [CustomPermissionAttribute(SecurityAction.LinkDemand)]
        /// <summary>
        /// Sends a string directly to printer, in raw mode, useful for dot matrix printers or pos devices
        /// </summary>
        /// <param name="printerName">Printer name</param>
        /// <param name="text">Text to send</param>
        /// <returns>Returns true if successful</returns>
		public static bool SendStringToPrinter(string printerName, string text)
		{
			Int32 byteCount;
			// How many characters are in the string?
			byteCount = text.Length;
			// Assume that the printer is expecting ANSI text, and then convert
			// the string to ANSI text.
			//pBytes = Marshal.StringToCoTaskMemAnsi(text);
            byte[] abytes = StreamUtil.StringToByteArray(text, text.Length, true);
			// Send the converted ANSI string to the printer.
//			SendBytesToPrinter(printerName, pBytes, byteCount);
            IntPtr ip =  Marshal.AllocHGlobal(abytes.Length);
            System.Runtime.InteropServices.Marshal.Copy(abytes, 0, ip, abytes.Length);

            //using (FileStream nstream = new FileStream("c:\\test.txt", FileMode.Create))
            //{
              //  nstream.Write(abytes, 0, abytes.Length);
            //}


            SendIntBytesToPrinter(printerName, ip, abytes.Length);
//            Marshal.FreeCoTaskMem(pBytes);
			return true;
		}
        /// <summary>
        /// Sends a bytes to printer, in raw mode, useful for dot matrix printers or pos devices
        /// </summary>
        /// <param name="printerName">Printer name</param>
        /// <param name="text">Text to send</param>
        /// <returns>Returns true if successful</returns>
        public static bool SendBytesToPrinter(string printerName,byte[] abytes)
        {
            Int32 byteCount;
            // How many characters are in the string?
            byteCount = abytes.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            //pBytes = Marshal.StringToCoTaskMemAnsi(text);
            // Send the converted ANSI string to the printer.
            //			SendBytesToPrinter(printerName, pBytes, byteCount);
            IntPtr ip = Marshal.AllocHGlobal(abytes.Length);
            System.Runtime.InteropServices.Marshal.Copy(abytes, 0, ip, abytes.Length);

            //using (FileStream nstream = new FileStream("c:\\test.txt", FileMode.Create))
            //{
            //  nstream.Write(abytes, 0, abytes.Length);
            //}


            SendIntBytesToPrinter(printerName, ip, abytes.Length);
            //            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
        // TODO: See whay this does not compile on Mono
        // [CustomPermissionAttribute(SecurityAction.LinkDemand)]
        /// <summary>
        /// Sends a string directly to printer, in raw mode, useful for dot matrix printers or pos devices
        /// </summary>
        /// <param name="printerName">Printer name</param>
        /// <param name="text">Text to send</param>
        /// <returns>Returns true if successful</returns>
        public static System.Threading.Tasks.Task SendStringToPrinterAsync(string printerName, string text)
        {
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
            {
                Int32 byteCount;
                // How many characters are in the string?
                byteCount = text.Length;
                // Assume that the printer is expecting ANSI text, and then convert
                // the string to ANSI text.
                //pBytes = Marshal.StringToCoTaskMemAnsi(text);
                byte[] abytes = StreamUtil.StringToByteArray(text, text.Length, true);
                // Send the converted ANSI string to the printer.
                //			SendBytesToPrinter(printerName, pBytes, byteCount);
                IntPtr ip = Marshal.AllocHGlobal(abytes.Length);
                System.Runtime.InteropServices.Marshal.Copy(abytes, 0, ip, abytes.Length);

                //using (FileStream nstream = new FileStream("c:\\test.txt", FileMode.Create))
                //{
                //  nstream.Write(abytes, 0, abytes.Length);
                //}

                SendIntBytesToPrinter(printerName, ip, abytes.Length);
            });
            return task;
        }



    }

}
