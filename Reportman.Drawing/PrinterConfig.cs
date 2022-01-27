using System;
using System.Threading;
using System.IO;

namespace Reportman.Drawing
{
    /// <summary>
    /// Printer configuration class to obtain default printer settigns
    /// </summary>
    public class PrinterConfig
    {
        public static bool PersistentConfiguration = true;
        private static IniFile config = null;
        public static object flag = 0;
        public static bool ForceSystemConfig = false;
        private static string filename;
        private static void CheckLoaded()
        {
            Monitor.Enter(flag);
            try
            {
                if ((config == null) || (!PersistentConfiguration))
                {
                    filename = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    filename = filename + Path.DirectorySeparatorChar + "reportman.ini";
                    if (!ForceSystemConfig)
                    {
                        FileInfo ninfo = new FileInfo(filename);
                        if (!ninfo.Exists)
                        {
                            filename = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                            filename = filename + Path.DirectorySeparatorChar + "reportman.ini";
                        }
                    }
                    config = new IniFile(filename);
                }
            }
            finally
            {
                Monitor.Exit(flag);
            }

        }
        public static string ConfigFile()
        {
            CheckLoaded();
            return filename;
        }
        public static void ReloadParameters()
        {
            Monitor.Enter(flag);
            try
            {
                config = null;
            }
            finally
            {
                Monitor.Exit(flag);
            }
            CheckLoaded();
        }
        public static string GetDriverName(PrinterSelectType printselect)
        {
            string defvalue = "";
            switch (printselect)
            {
                case PrinterSelectType.Characterprinter:
                    defvalue = "EPSON";
                    break;
                case PrinterSelectType.PlainPrinter:
                    defvalue = "PLAIN";
                    break;
                case PrinterSelectType.PlainFullPrinter:
                    defvalue = "PLAINFULL";
                    break;
            }
            string valuename = "Printer" + ((int)printselect).ToString();
            CheckLoaded();
            return config.ReadString("PrinterDriver", valuename, defvalue);
        }
        public static string GetPrinterName(PrinterSelectType printselect)
        {
            string defvalue = "";
            string valuename = "Printer" + ((int)printselect).ToString();
            CheckLoaded();
            return config.ReadString("PrinterNames", valuename, defvalue);
        }
        public static string DecodeEscapeString(string source)
        {
            string nresult = source;
            string newstring = "";
            int idx = 0;
            while (idx < nresult.Length)
            {
                char newchar = nresult[idx];
                if (newchar == '#')
                {
                    idx++;
                    string number = "";
                    while (idx < nresult.Length)
                    {
                        if (char.IsDigit(nresult[idx]))
                        {
                            number = number + nresult[idx];
                            idx++;
                        }
                        else
                            break;
                    }
                    if (number.Length > 0)
                    {
                        int idxchar = Convert.ToInt32(number);
                        char xchar = (char)idxchar;
                        newstring = newstring + xchar;
                    }
                }
                else
                {
                    newstring = newstring + nresult[idx];
                    idx++;
                }
            }
            return newstring;
        }
        public static string GetCutPaperOperation(PrinterSelectType printselect)
        {
            string defvalue = "";
            string valuename = "Printer" + ((int)printselect).ToString();
            CheckLoaded();
            string nresult = config.ReadString("CutPaper", valuename, defvalue);
            nresult = DecodeEscapeString(nresult);
            return nresult;
        }
        public static bool GetOpenDrawerOption(PrinterSelectType printselect)
        {
            string valuename = "Printer" + ((int)printselect).ToString();
            CheckLoaded();
            return config.ReadBool("OpenDrawerOn", valuename, false);
        }
        public static bool GetCutPaperOption(PrinterSelectType printselect)
        {
            string valuename = "Printer" + ((int)printselect).ToString();
            CheckLoaded();
            return config.ReadBool("CutPaperOn", valuename, false);
        }
        public static string GetOpenDrawerOperation(PrinterSelectType printselect)
        {
            string defvalue = "";
            string valuename = "Printer" + ((int)printselect).ToString();
            CheckLoaded();
            string nresult = config.ReadString("OpenDrawer", valuename, defvalue);
            nresult = DecodeEscapeString(nresult);
            return nresult;
        }
        public static bool GetOEMConvert(PrinterSelectType printselect)
        {
            string valuename = "Printer" + ((int)printselect).ToString();
            CheckLoaded();
            return config.ReadBool("PrinterEscapeOem", valuename, true);
        }

        public static Strings GetTextOnlyPrintDrivers()
        {
            Strings drivernames = new Strings
            {
                " ",
                "PLAIN",
                "EPSON",
                "EPSON-MASTER",
                "EPSON-ESCP",
                "EPSON-ESCPQ",
                "IBMPROPRINTER",
                "EPSONTMU210",
                "EPSONTMU210CUT",
                "EPSONTM88IICUT",
                "EPSONTM88II",
                "HP-PCL",
                "VT100",
                "PLAINFULL"
            };
            return drivernames;
        }
        public static Strings GetConfigurablePrinters()
        {
            Strings configs = new Strings
            {
                Translator.TranslateStr(467),
                Translator.TranslateStr(468),
                Translator.TranslateStr(469),
                Translator.TranslateStr(470),
                Translator.TranslateStr(471),
                Translator.TranslateStr(472),
                Translator.TranslateStr(473),
                Translator.TranslateStr(474),
                Translator.TranslateStr(475),
                Translator.TranslateStr(476),
                Translator.TranslateStr(477),
                Translator.TranslateStr(478),
                Translator.TranslateStr(479),
                Translator.TranslateStr(480),
                Translator.TranslateStr(481),
                Translator.TranslateStr(482),
                Translator.TranslateStr(1343),
                Translator.TranslateStr(1344)
            };

            // More configurable printers
            for (int i = 1; i <= 50; i++)
                configs.Add("Printer" + i.ToString());

            return configs;
        }
    }
}
