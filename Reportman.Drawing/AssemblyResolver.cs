using System;
using System.IO;

namespace Reportman.Drawing
{
    public static class AssemblyResolver
    {
        public static void HandleUnresolvedAssemblies()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }
        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string sqlitename = "System.Data.SQLite";
            if (args.Name.Length >= sqlitename.Length)
            {
                if (args.Name.Substring(0, sqlitename.Length) == "System.Data.SQLite")
                {
                    string pathToWhereYourNativeFolderLives = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    pathToWhereYourNativeFolderLives = Path.GetDirectoryName(pathToWhereYourNativeFolderLives);
                    //var path = Path.Combine(pathToWhereYourNativeFolderLives, "Native");
                    string path = pathToWhereYourNativeFolderLives;
                    if (IntPtr.Size == 8) // or for .NET4 use Environment.Is64BitProcess
                    {
                        path = Path.Combine(path, "Win64");
                    }
                    else
                    {
                        path = Path.Combine(path, "Win32");
                    }
                    string dirpath = path;
                    string dllpath = Path.Combine(path, "System.Data.SQLite.dll");
                    if (!File.Exists(dllpath))
                    {
                        throw new Exception("System.Data.SQLite.dll could not be found in " + dllpath);
                    }

                    System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(dllpath);
                    return assembly;
                }
                sqlitename = "PDFLibNet";
                if (args.Name.Length >= sqlitename.Length)
                {
                    if (args.Name.Substring(0, sqlitename.Length) == "PDFLibNet")
                    {
                        string pathToWhereYourNativeFolderLives = System.Reflection.Assembly.GetExecutingAssembly().Location;
                        pathToWhereYourNativeFolderLives = Path.GetDirectoryName(pathToWhereYourNativeFolderLives);
                        //var path = Path.Combine(pathToWhereYourNativeFolderLives, "Native");
                        string path = pathToWhereYourNativeFolderLives;
                        if (IntPtr.Size == 8) // or for .NET4 use Environment.Is64BitProcess
                        {
                            path = Path.Combine(path, "Win64");
                        }
                        else
                        {
                            path = Path.Combine(path, "Win32");
                        }
                        string dirpath = path;
                        string dllpath = Path.Combine(path, "PDFLibNet.dll");
                        if (!File.Exists(dllpath))
                        {
                            string zipfile = Path.ChangeExtension(dllpath, ".zip");
                            if (File.Exists(zipfile))
                            {
                                string filepath = Path.GetDirectoryName(zipfile);
                                ICSharpCode.SharpZipLib.Zip.FastZip nzip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                                nzip.ExtractZip(zipfile, filepath, "");
                            }
                        }
                        if (!File.Exists(dllpath))
                        {
                            throw new Exception("PDFLibNet could not be found in " + dllpath);
                        }

                        System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(dllpath);
                        return assembly;
                    }
                }
            }

            return null;
        }
    }
}
