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
using Reportman;
using Reportman.Drawing;
using Reportman.Reporting;
#if FORMS
using Reportman.Drawing.Forms;
using Reportman.Reporting.Forms;
using System.Windows.Forms;
#else
#endif
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;

namespace Reportman.Commands
{
    /// <summary>
    /// This class implements command line execution of reports
    /// </summary>
    public class PrintReport
    {
        static private Strings GetParams()
        {
            Strings aresult = new Strings
            {
                "Report Manager Dot Net Printreport",
                "Usage:",
                "printreport.exe [options] reportfile|-stdin",
                "-preview Show preview window",
                "-previewWPF Show preview window using Windows Presentation Foundation",
                "-showparams Show parameters window window",
                "-showdata DATASETNAME Open and show the dataset",
                "-testconnection CONNECTIONNAME Test a connection",
                "-showfields DATASETNAME filename Open and save field info",
                "-showprogress Show progress window",
                "-pdf pdffilename Output to pdf file",
                "-m metafilename Output to report metafile",
                "-deletereport Delete the report file after completion",
                "-systempreview Show System preview instead Report Manager preview",
                "-u Save pdf/metafile as uncompressed",
                "-eval exp Evaluate expression and show result",
                "-syntax exp Syntax check expression",
                "-printdialog Show print dialog before printing",
                "-printername Use the printer device",
                "-getprinters List available printers",
                "-paramPARAMNAME=Value Assigns a value to aparameter",
                "-syncexecution Do not process de report asynchronously",
                "-throw Throw exception on error, instead of message box",
                "-getproviders filename Get the registered data provider names"
            };
            return aresult;
        }
        static private void PrintParams()
        {
            Strings astrings = GetParams();
            for (int i = 0; i < astrings.Count; i++)
                System.Console.WriteLine(astrings[i]);
        }

        /// <summary>
        /// PrintReport application gets a Report file (.rep) as input, process it
        /// using Reportman library and outputs the result to screen, printer, pdf
        /// and other formats.<br></br>
        /// Usage:
        /// <code>
        /// printreport.exe [options] reportfile|-stdin
        /// </code>
        /// This means options are optional, but a report file or -stdin must be specified.
        /// <list type="table">
        /// <listheader><term>Option</term>
        /// <description>Description</description></listheader>
        /// <item><term><nobr>-preview</nobr></term>
        /// <description>Show preview window, that is output to screen using Windows Forms</description>
        /// </item>
        /// <item><term><nobr>-previewWPF</nobr></term>
        /// <description>Show preview window, that is output to screen using Windows Presentation Foundation</description>
        /// </item>
        /// <item><term><nobr>-showparams</nobr></term>
        /// <description>Show parameters window before executing the report</description>
        /// </item>
        /// <item><term><nobr>-showprogress</nobr>
        /// </term><description>While calculating the report, show a progress 
        /// window, that is for the impatients</description>
        /// </item>
        /// <item><term><nobr>-pdf filename</nobr></term>
        /// <description>Generate a pdf file, file name must be specified
        /// </description></item>
        /// <item><term><nobr>-m filename</nobr></term>
        /// <description>Generate a report metafile, file name must be specified
        /// </description></item>
        /// <item><term><nobr>-u</nobr></term>
        /// <description>The pdf/metafile output will be uncompressed, have effect
        /// only used with -pdf option</description></item>
        /// <item><term><nobr>-deletereport</nobr></term>
        /// <description>Delete the report file after execution, handle with
        /// care</description></item><item><term><nobr>-systempreview</nobr></term>
        /// <description>Output to screen but using framework preview 
        /// (low performance)</description></item><item>
        /// <term><nobr>-syncexecution</nobr></term>
        /// <description>Do not process report asynchronously</description>
        /// </item><item><term><nobr>-throw</nobr></term>
        /// <description>Throw expecion on error, by default a window with 
        /// error message will pop up</description></item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// <ul>
        /// <li>Report files can be designed and tested using
        /// <a href="http://reportman.sourceforge.net">Report Manager Designer</a>.
        /// </li>
        /// <li>The default output is the default printer.</li>
        /// <li>If an error occurs a window is shown to the user,
        /// unless you use -throw option</li>
        /// </ul>
        /// </remarks>
        /// <example>
        /// This command will generate a pdf document executing a report definition file:
        /// <code>
        /// printreport.exe myreport.rep -pdf mydocument.pdf
        /// </code>
        /// </example>
        /// <example>
        /// This command will show the preview window executing a report definition file:
        /// <code>
        /// printreport.exe myreport.rep -pdf mydocument.pdf
        /// </code>
        /// </example>
        /// <param name="args">Main entry point is a serie of strings, the strings are interpreted as command line options</param>
        [STAThread]
        public static void Main(string[] args)
        {
            string printername = "";
            // AssemblyResolver.HandleUnresolvedAssemblies();


            // Bug in .Net 1.x, don't enable, but you can use a .manifest file
#if FORMS
            Application.EnableVisualStyles();
#endif
            bool dothrow = false;



            Report rp = new Report();
            try
            {
#if FORMS
                bool showprintdialog = false;
                bool preview = false;
                bool previewWPF = false;
                bool systempreview = false;
#endif
                bool asyncexecution = true;
                bool stdin = false;
                bool pdf = false;
                bool metafile = false;
                string filename = "";
                string pdffilename = "";
                string fieldsfilename = "";
                bool deletereport = false;
                bool showprogress = false;
                bool showparams = false;
                bool compressedpdf = true;
                bool showdata = false;
                bool showfields = false;
                bool testconnection = false;
                string connectionname = "";
                bool doprint = true;
                bool evaluatetext = false;
                bool syntaxcheck = false;
                bool doread = true;
                string evaltext = "";
                string dataset = "";
                SortedList<string, string> ParamValues = new SortedList<string, string>();
                bool showproviders = false;
                string providersfilename = "";
                try
                {

                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i].Trim().Length > 0)
                        {
                            switch (args[i].ToUpper())
                            {
                                case "-STDIN":
                                    stdin = true;
                                    break;
#if FORMS
                                case "-PREVIEW":
                                    preview = true;
                                    break;
                                case "-PREVIEWWPF":
                                    previewWPF = true;
                                    break;
                                case "-SYSTEMPREVIEW":
                                    systempreview = true;
                                    break;
                                case "-PRINTDIALOG":
                                    showprintdialog = true;
                                    break;
#endif
                                case "-SHOWPARAMS":
                                    showparams = true;
                                    break;
                                case "-SYNCEXECUTION":
                                    asyncexecution = false;
                                    break;
                                case "-U":
                                    compressedpdf = false;
                                    break;
                                case "-THROW":
                                    dothrow = true;
                                    break;
                                case "-PDF":
                                    pdf = true;
                                    if (args.GetUpperBound(0) > i)
                                    {
                                        i++;
                                        pdffilename = args[i];
                                    }
                                    break;
                                case "-M":
                                    metafile = true;
                                    if (args.GetUpperBound(0) > i)
                                    {
                                        i++;
                                        pdffilename = args[i];
                                    }
                                    break;
                                case "-SHOWDATA":
                                    showdata = true;
                                    doprint = false;
                                    if (args.GetUpperBound(0) > i)
                                    {
                                        i++;
                                        dataset = args[i];
                                    }
                                    break;
                                case "-TESTCONNECTION":
                                    doprint = false;
                                    testconnection = true;
                                    if (args.GetUpperBound(0) > i)
                                    {
                                        i++;
                                        connectionname = args[i];
                                    }
                                    break;
                                case "-EVAL":
                                    doprint = false;
                                    evaluatetext = true;
                                    if (args.GetUpperBound(0) > i)
                                    {
                                        i++;
                                        evaltext = args[i];
                                    }
                                    break;
                                case "-SYNTAX":
                                    doprint = false;
                                    syntaxcheck = true;
                                    if (args.GetUpperBound(0) > i)
                                    {
                                        i++;
                                        evaltext = args[i];
                                    }
                                    break;
                                case "-SHOWFIELDS":
                                    showfields = true;
                                    doprint = false;
                                    if (args.GetUpperBound(0) > i)
                                    {
                                        i++;
                                        dataset = args[i];
                                    }
                                    if (args.GetUpperBound(0) > i)
                                    {
                                        i++;
                                        fieldsfilename = args[i];
                                    }
                                    break;
                                case "-GETPROVIDERS":
                                    showproviders = true;
                                    doprint = false;
                                    doread = false;
                                    if (args.GetUpperBound(0) > i)
                                    {
                                        i++;
                                        providersfilename = args[i];
                                    }
                                    break;
#if NETCOREAPP2_0
#else
                                case "-GETPRINTERS":
                                    System.Text.StringBuilder nprinters = new System.Text.StringBuilder();
                                    nprinters.AppendLine("Installed printers:");
                                    foreach (string pname in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                                        nprinters.AppendLine(pname);
#if FORMS
                                    MessageBox.Show(nprinters.ToString());
#else
                                    System.Console.WriteLine(nprinters.ToString());
#endif
                                    break;
#endif
                                case "-PRINTERNAME":
                                    if (args.GetUpperBound(0) > i)
                                    {
                                        i++;
                                        printername = args[i];
                                    }
                                    else
                                        throw new Exception("A printer name must be provided after -printername");
                                    break;
                                case "-SHOWPROGRESS":
                                    showprogress = true;
                                    break;
                                case "-DELETEREPORT":
                                    deletereport = true;
                                    break;
                                default:
                                    // Get parameter names and values
                                    string argname = args[i];
                                    if (argname[0] == '-')
                                    {
                                        bool correctparam = false;
                                        if (argname.Length > 7)
                                        {
                                            if (argname.Substring(0, 6).ToUpper() == "-PARAM")
                                            {
                                                argname = argname.Substring(6, argname.Length - 6);
                                                int indexequ = argname.IndexOf('=');
                                                if (indexequ < 0)
                                                {
                                                    throw new Exception("Invalid syntax in -param, -paramPARAMNAME=value syntax must be used");
                                                }
                                                string paramvalue = argname.Substring(indexequ + 1, argname.Length - indexequ - 1);
                                                argname = argname.Substring(0, indexequ);
                                                ParamValues.Add(argname, paramvalue);
                                                System.Console.WriteLine("Decoded parameter: " + argname + " value " + paramvalue);
                                                correctparam = true;
                                            }
                                        }
                                        if (!correctparam)
                                            throw new Exception("Invalid argument:" + args[i]);
                                    }
                                    else
                                    {

                                        if (filename.Length > 0)
                                        {
                                            filename = args[i];
                                        }
                                        else
                                            filename = args[i];
                                    }
                                    break;
                            }
                        }
                    }
                    AddCustomFactories();

                    if (showproviders)
                    {
                        string messageproviders = "";

                        int indexp;
#if NETCOREAPP2_0
                        DataTable atable = new DataTable();
#else
                        DataTable atable = DbProviderFactories.GetFactoryClasses();
#endif
                        if (providersfilename.Length == 0)
                        {
                            bool firebirdfound = false;
                            bool mysqlfound = false;
                            bool sqlitefound = false;
                            for (indexp = 0; indexp < atable.Rows.Count; indexp++)
                            {
                                if (messageproviders.Length != 0)
                                    messageproviders = messageproviders + (char)13 + (char)10;
                                string nprovider = atable.Rows[indexp][2].ToString();
                                if (nprovider == DatabaseInfo.FIREBIRD_PROVIDER)
                                    firebirdfound = true;
                                if (nprovider == DatabaseInfo.MYSQL_PROVIDER)
                                    mysqlfound = true;
                                if (nprovider == DatabaseInfo.SQLITE_PROVIDER)
                                    sqlitefound = true;
                                messageproviders = messageproviders + atable.Rows[indexp][2].ToString();
                            }
                            if (!firebirdfound)
                                messageproviders = DatabaseInfo.FIREBIRD_PROVIDER + (char)13 + (char)10 + messageproviders;
                            if (!mysqlfound)
                                messageproviders = DatabaseInfo.MYSQL_PROVIDER + (char)13 + (char)10 + messageproviders;
                            if (!sqlitefound)
                                messageproviders = DatabaseInfo.SQLITE_PROVIDER + (char)13 + (char)10 + messageproviders;
#if FORMS
                            MessageBox.Show(messageproviders, "Data providers");
#else
                            System.Console.WriteLine("Data providers");
                            System.Console.WriteLine(messageproviders);
#endif
                        }
                        else
                        {
                            FileStream providersstream = new FileStream(providersfilename, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                            try
                            {
                                bool firebirdfound = false;
                                bool mysqlfound = false;
                                bool sqlitefound = false;
                                Strings nstrings = new Strings();
                                for (indexp = 0; indexp < atable.Rows.Count; indexp++)
                                {
                                    string nprovider = atable.Rows[indexp][2].ToString();
                                    if (nprovider == DatabaseInfo.FIREBIRD_PROVIDER)
                                        firebirdfound = true;
                                    if (nprovider == DatabaseInfo.MYSQL_PROVIDER)
                                        mysqlfound = true;
                                    if (nprovider == DatabaseInfo.SQLITE_PROVIDER)
                                        sqlitefound = true;
                                    nstrings.Add(nprovider);
                                }
                                if (!firebirdfound)
                                    nstrings.Insert(0, DatabaseInfo.FIREBIRD_PROVIDER);
                                if (!mysqlfound)
                                    nstrings.Insert(0, DatabaseInfo.MYSQL_PROVIDER);
                                if (!sqlitefound)
                                    nstrings.Insert(0, DatabaseInfo.SQLITE_PROVIDER);

                                foreach (string nstring in nstrings)
                                {
                                    StreamUtil.SWriteLine(providersstream, nstring);
                                }
                            }
                            finally
                            {
                                providersstream.Close();
                            }
                        }
                    }
                    if (doread)
                    {
                        if (stdin)
                        {
                            Stream astream = System.Console.OpenStandardInput();
                            rp.LoadFromStream(astream, 8192);
                        }
                        else
                        {
                            if (filename.Length == 0)
                                throw new Exception("You must provide a report filename");
                            rp.LoadFromFile(filename);
                        }

                        foreach (string nparamname in ParamValues.Keys)
                        {
                            string nvalue = ParamValues[nparamname];
                            if (rp.Params.IndexOf(nparamname) < 0)
                            {
                                System.Console.WriteLine("Warning: parameter not found created: " + nparamname);
                                Reportman.Reporting.Param xparam = new Reportman.Reporting.Param(rp)
                                {
                                    Alias = nparamname,
                                    Visible = false
                                };
                                rp.Params.Add(xparam);
                                //throw new Exception("Parameter " + nparamname + " not found");
                            }
                            rp.Params[nparamname].Value = nvalue;
                        }
                    }
                    if (showdata)
                    {
                        rp.PrintOnlyIfDataAvailable = false;
                        PrintOutPDF printpdf3 = new PrintOutPDF();
                        rp.BeginPrint(printpdf3);
#if FORMS
                        DataShow.ShowData(rp, dataset, null);
#else
                        rp.DataInfo[dataset].Connect();
                        ReportDataset ndataset = rp.DataInfo[dataset].Data;
                        foreach (DataColumn ncol in ndataset.Columns)
                        {
                            string nstring = ncol.ColumnName.PadLeft(30);
                            if (nstring.Length > 30)
                                nstring = nstring.Substring(0, 30);
                            System.Console.Write(nstring);
                        }
                        System.Console.WriteLine();
                        while (!ndataset.Eof)
                        {
                            foreach (DataColumn ncol in ndataset.Columns)
                            {
                                string nstring = ndataset.CurrentRow[ncol.ColumnName].ToString().PadLeft(30);
                                if (nstring.Length > 30)
                                    nstring = nstring.Substring(0, 30);
                                System.Console.Write(nstring);
                            }
                            System.Console.WriteLine();
                            ndataset.Next();
                        }
#endif
                    }
                    if (testconnection)
                    {
                        int conindex = rp.DatabaseInfo.IndexOf(connectionname);
                        if (conindex < 0)
                            throw new Exception("Connection name not found:" + connectionname);
                        rp.DatabaseInfo[conindex].Connect();
#if FORMS
                        MessageBox.Show("Connexion successfull:" + connectionname);
#else
                        System.Console.WriteLine("Connexion successfull:" + connectionname);
#endif
                    }
                    if (showfields)
                    {
                        int index = rp.DataInfo.IndexOf(dataset);
                        if (index < 0)
                            throw new Exception("Dataset not found:" + dataset);
                        rp.DataInfo[index].Connect();
                        FileStream fstream = new FileStream(fieldsfilename,
                                System.IO.FileMode.Create, System.IO.FileAccess.Write,
                                System.IO.FileShare.None);
                        try
                        {
                            rp.DataInfo[index].GetFieldsInfo(fstream);
                        }
                        finally
                        {
                            fstream.Close();
                        }
                    }
                    if ((evaluatetext) || (syntaxcheck))
                    {
                        rp.PrintOnlyIfDataAvailable = false;
                        PrintOutPDF printpdf2 = new PrintOutPDF();
                        rp.BeginPrint(printpdf2);
                        if (evaluatetext)
                        {
                            try
                            {
                                Variant aresult = rp.Evaluator.EvaluateText(evaltext);
#if FORMS
                                MessageBox.Show("Result:" + aresult.ToString());
#else
                                System.Console.WriteLine("Result:" + aresult.ToString());
#endif
                            }

                            catch (EvalException e)
                            {
#if FORMS
                                MessageBox.Show("Error Line: " + e.SourceLine.ToString() +
                     " Error position:" + e.SourcePos.ToString() + " - " + e.Message);
#else
                                System.Console.WriteLine("Error Line: " + e.SourceLine.ToString() +
                     " Error position:" + e.SourcePos.ToString() + " - " + e.Message);
#endif
                            }
                            catch (Exception E)
                            {
#if FORMS
                                MessageBox.Show("Error: " + E.Message);
#else
                                System.Console.WriteLine("Error: " + E.Message);
#endif
                            }
                        }
                        else
                        {
                            try
                            {
                                rp.Evaluator.CheckSyntax(evaltext);
#if FORMS
                                MessageBox.Show("Syntax check ok");
#else
                                System.Console.WriteLine("Syntax check ok");
#endif
                            }
                            catch (Exception E)
                            {
#if FORMS
                                MessageBox.Show("Error: " + E.Message);
#else
                                System.Console.WriteLine("Error: " + E.Message);
#endif
                            }
                        }
                    }
                    // Ask for parameters?
                    if (doprint)
                        if (showparams)
                        {
#if FORMS
                            doprint = ParamsForm.ShowParams(rp);
#else
                            throw new Exception("Show params not supported in console mode");
#endif
                        }
                    if (doprint)
                    {
                        if (showprogress)
#if FORMS
                        {
                            ReportProgressForm fprogres = new ReportProgressForm();
                            fprogres.SetMetaFile(rp.MetaFile);
                            fprogres.Show();
                        }
#else
                            throw new Exception("Show progress not supported in console mode");
#endif
                        if (pdf)
                        {
                            rp.AsyncExecution = false;
                            PrintOutPDF printpdf = new PrintOutPDF
                            {
                                FileName = pdffilename,
                                Compressed = compressedpdf
                            };
                            printpdf.Print(rp.MetaFile);
                        }
                        else
                            if (metafile)
                        {
                            rp.AsyncExecution = false;
                            rp.TwoPass = true;
                            PrintOutPDF printpdf = new PrintOutPDF
                            {
                                FileName = "",
                                Compressed = compressedpdf
                            };
                            printpdf.Print(rp.MetaFile);
                            rp.MetaFile.SaveToFile(pdffilename, compressedpdf);
                        }
                        else
                        {
                            rp.AsyncExecution = asyncexecution;
#if FORMS
                            if (previewWPF)
                            {
                                Reportman.WPF.PrintOutWPF prwpf = new WPF.PrintOutWPF();
                                prwpf.Print(rp.MetaFile);
                                Reportman.WPF.PreviewWindow.PreviewDocument(prwpf.Document);

                                /*
                                rp.AsyncExecution = false;
                                rp.TwoPass = true;
                                PrintOutPDF printpdf = new PrintOutPDF();
                                printpdf.FileName = "";
                                printpdf.Print(rp.MetaFile);
                                Reportman.WPF.PreviewWindow.PreviewMetaFile(rp.MetaFile);
                                */
                            }
                            else
                            {
                                PrintOutReportWinForms prw = new PrintOutReportWinForms(rp);
                                prw.Preview = preview;
                                prw.ShowInTaskbar = true;
                                prw.SystemPreview = systempreview;
                                prw.ShowPrintDialog = showprintdialog;
                                prw.Print(rp.MetaFile);
                            }
#else
#if NETCOREAPP2_0
                            throw new Exception("Output to printer not supported in .Net core");
#else
                            PrintOutPrint prw = new PrintOutPrint();
                                if (printername.Length > 0)
                                    PrintOutNet.DefaultPrinterName = printername;

                                prw.Print(rp.MetaFile);
#endif
#endif

                            //                                PrintOutWinForms prw = new PrintOutWinForms();
                            //							prw.OptimizeWMF = WMFOptimization.Gdiplus;
                        }
                    }
                }
                finally
                {
                    if (deletereport)
                        if (filename.Length > 0)
                            System.IO.File.Delete(filename);
                }

            }
            catch (Exception E)
            {
                if (!dothrow)
                {
                    int i;
                    string amessage = E.Message + (char)13 + (char)10;
                    for (i = 0; i < args.Length; i++)
                    {
                        amessage = amessage + (char)13 + (char)10 + "Arg" + i.ToString() + ":" + args[i];
                    }
                    Strings astrings = GetParams();
                    for (i = 0; i < astrings.Count; i++)
                    {
                        amessage = amessage + (char)13 + (char)10 + astrings[i];
                    }

#if FORMS
                    MessageBox.Show(amessage, "Error");
#else
                    System.Console.WriteLine(amessage);
#endif
                    System.Console.WriteLine(E.StackTrace);
                }
                else
                {
                    PrintParams();
                    throw;
                }
                //				Variant.WriteStringToUTF8Stream(amessage,System.Console.OpenStandardError());
            }
        }
        static void AddCustomFactories()
        {
            if (DatabaseInfo.CustomProviderFactories.Count == 0)
            {
                try
                {
                    DatabaseInfo.CustomProviderFactories.Add(DatabaseInfo.FIREBIRD_PROVIDER, FirebirdSql.Data.FirebirdClient.FirebirdClientFactory.Instance);
                    Reportman.Reporting.DatabaseInfo.CustomProviderFactories.Add(Reportman.Reporting.DatabaseInfo.FIREBIRD_PROVIDER2, FirebirdSql.Data.FirebirdClient.FirebirdClientFactory.Instance);
                }
                catch (Exception E)
                {
                    System.Console.WriteLine("Error in Firebird provider: " + E.Message);
                }
                try
                {
#if MONODEBUG
#else
                    DatabaseInfo.CustomProviderFactories.Add(DatabaseInfo.MYSQL_PROVIDER, MySql.Data.MySqlClient.MySqlClientFactory.Instance);
#endif

                }
                catch (Exception E)
                {
                    System.Console.WriteLine("Error in MySQL factory: " + E.Message);
                }
                try
                {
#if NETCOREAPP2_0
                    DatabaseInfo.CustomProviderFactories.Add(DatabaseInfo.SQLITE_PROVIDER, Microsoft.Data.Sqlite.SqliteFactory.Instance);
#else
                    DatabaseInfo.CustomProviderFactories.Add(DatabaseInfo.SQLITE_PROVIDER, System.Data.SQLite.SQLiteFactory.Instance);
#endif
                }
                catch (Exception E)
                {
                    System.Console.WriteLine("Error in Sqlite factory: " + E.Message);
                }
            }
        }
    }

}
