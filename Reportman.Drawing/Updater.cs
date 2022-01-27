using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Reflection;
using Reportman.Drawing;

namespace Reportman.Drawing
{
    public delegate void CopyProgress(string filename, int file,
     int filecount, int position, int size, ref bool docancel);
    public class VersionInfo
    {
#if PocketPC
#else
        public static Version GetAssemblyVersion(string filename)
        {

            Version xversion;
            AssemblyName nname = AssemblyName.GetAssemblyName(filename);
            xversion = nname.Version;



            return xversion;
        }
        public static Version GetFileVersion(string filename)
        {
            // To allow the open of any file including .Net versions newer than
            // the compiled version FileVersion is used instead
            System.Diagnostics.FileVersionInfo finfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(filename);
            Version aversion = new Version(finfo.FileMajorPart, finfo.FileMinorPart, finfo.FileBuildPart, finfo.FilePrivatePart);

            return aversion;
        }
#endif
        public static Version GetAssemblyVersion()
        {
            return Assembly.GetCallingAssembly().GetName().Version;
        }
        public static bool RequireUpgrade(Version oldversion, Version newversion)
        {
            if (newversion.Major > oldversion.Major)
                return true;
            if (newversion.Major < oldversion.Major)
                return false;
            if (newversion.Minor > oldversion.Minor)
                return true;
            if (newversion.Minor < oldversion.Minor)
                return false;

            if (newversion.Build > oldversion.Build)
                return true;
            if (newversion.Build < oldversion.Build)
                return false;
            if (newversion.Revision > oldversion.Revision)
                return true;
            if (newversion.Revision < oldversion.Revision)
                return false;
            return false;
        }
        public static bool RequireUpgrade(string oldfilename, string newfilename)
        {
            Version oldversion = GetAssemblyVersion(oldfilename);
            Version newversion = GetAssemblyVersion(newfilename);
            return VersionInfo.RequireUpgrade(oldversion, newversion);
        }
    }
    public class Updater
    {
        string FFilePath;
        public bool PerformBackup;
        public CopyProgress OnProgress;
        public bool ExcludeExecutingAssembly;
        public string FilePath
        {
            get { return FFilePath; }
        }
        public Updater(string fpath)
        {
            FFilePath = fpath;
            PerformBackup = false;
            ExcludeExecutingAssembly = false;
        }
        public static DataTable GetModifiedFiles(DataTable olderfiles, DataTable files, string filesdir, bool copycontent, bool compareDates = false)
        {
            DataTable xtable = CreateFilesTable();
            try
            {
                foreach (DataRow newrow in files.Rows)
                {
                    bool doupdate = false;
                    DataRow xrow = olderfiles.Rows.Find(newrow["FULLPATH"]);
                    if ((xrow == null) || (!compareDates))
                    {
                        doupdate = true;
                    }
                    else
                    {
                        TimeSpan nspan = (DateTime)newrow["MODIFIED"] - (DateTime)xrow["MODIFIED"];
                        // Actualización cuando la diferencia es mayor de 3 segundos
                        if (Math.Abs(nspan.TotalSeconds) > 3)
                            doupdate = true;
                    }
                    if (doupdate)
                    {
                        xrow = xtable.NewRow();
                        foreach (DataColumn ncol in files.Columns)
                        {
                            xrow[ncol.ColumnName] = newrow[ncol.ColumnName];
                        }
                        if (!compareDates)
                        {
                            xrow["MODIFIED"] = System.DateTime.Now;
                        }
                        if (copycontent)
                        {
                            string fullname = Path.Combine(filesdir, newrow["PATH"].ToString());
                            fullname = Path.Combine(fullname, newrow["FILE"].ToString());
                            using (FileStream fstream = new FileStream(fullname, FileMode.Open, FileAccess.Read))
                            {
                                using (MemoryStream mstream = new MemoryStream())
                                {
                                    const int BUFSIZE = 8192;
                                    byte[] buf = new byte[BUFSIZE];
                                    int readed;
                                    do
                                    {
                                        readed = fstream.Read(buf, 0, BUFSIZE);
                                        mstream.Write(buf, 0, readed);
                                    } while (readed > 0);
                                    xrow["STREAM"] = mstream.ToArray();
                                }
                            }
                        }
                        xtable.Rows.Add(xrow);
                    }
                }
            }
            catch
            {
                xtable.Dispose();
                throw;
            }
            return xtable;
        }
        public static DataTable CreateFilesTable()
        {
            DataTable xtable = new DataTable();
            xtable.Columns.Add("FULLPATH", System.Type.GetType("System.String"));
            xtable.Columns.Add("PATH", System.Type.GetType("System.String"));
            xtable.Columns.Add("FILE", System.Type.GetType("System.String"));
            xtable.Columns.Add("STREAM", System.Type.GetType("System.Byte[]"));
            xtable.Columns.Add("MODIFIED", System.Type.GetType("System.DateTime"));

            xtable.Constraints.Add("PRIMPATH", xtable.Columns[0], true);
            return xtable;
        }
        public void FillFiles(DataTable xtable, string sourcedir, string subdir, bool copycontent)
        {
            string[] nfilescontent = Directory.GetFiles(sourcedir, "*.*", SearchOption.AllDirectories);

            foreach (string fname in nfilescontent)
            {
                FileInfo ninfo = new FileInfo(fname);
                if (System.IO.Path.GetFileName(fname).ToUpper() != "THUMBS.DB")
                {
                    DataRow xrow = xtable.NewRow();
                    //xrow["MODIFIED"] = ninfo.LastWriteTimeUtc;
                    xrow["MODIFIED"] = ninfo.LastWriteTime;
                    string dirname = Path.GetDirectoryName(fname);
                    int findex = fname.IndexOf(sourcedir);
                    if (findex >= 0)
                    {
                        dirname = dirname.Substring(findex + sourcedir.Length, dirname.Length - findex - sourcedir.Length);
                    }
                    if (dirname == "\\")
                        dirname = "";
                    if (dirname.Length > 1)
                    {
                        if (dirname[0] == '\\')
                            dirname = dirname.Substring(1, dirname.Length - 1);
                    }

                    xrow["PATH"] = dirname;
                    xrow["FILE"] = Path.GetFileName(fname);
                    xrow["FULLPATH"] = Path.Combine(dirname, xrow["FILE"].ToString()).ToUpper();
                    if (copycontent)
                    {
                        using (FileStream fstream = new FileStream(fname, FileMode.Open, FileAccess.Read))
                        {
                            using (MemoryStream mstream = new MemoryStream())
                            {
                                const int BUFSIZE = 8192;
                                byte[] buf = new byte[BUFSIZE];
                                int readed;
                                do
                                {
                                    readed = fstream.Read(buf, 0, BUFSIZE);
                                    mstream.Write(buf, 0, readed);
                                } while (readed > 0);
                                xrow["STREAM"] = mstream.ToArray();
                            }
                        }
                    }
                    xtable.Rows.Add(xrow);
                }
            }
        }
        public DataTable GetFiles(string sourcedir, bool copycontent)
        {
            DataTable xtable = CreateFilesTable();
            try
            {
                FillFiles(xtable, sourcedir, "", copycontent);
            }
            catch
            {
                xtable.Dispose();
                throw;
            }
            return xtable;
        }
        public void Update(DataTable files)
        {
            if (files == null)
                throw new Exception("No se actualizaron archivos porque no se proporcionaron");
            DateTime mmfirst = System.DateTime.Now;
            string excludefilename = "";
            if (ExcludeExecutingAssembly)
            {
                excludefilename = System.Reflection.Assembly.GetExecutingAssembly().Location;
                excludefilename = Path.GetFileName(excludefilename);
                excludefilename = excludefilename.ToUpper();
            }
            const int BUFSIZE = 8192;
            byte[] buf = new byte[BUFSIZE];

            string backpath = "";
            // Create temp dir
            if (PerformBackup)
            {
                backpath = FFilePath + "CC" + System.DateTime.Now.ToString("ddMMyyyyHH_mm_ss");
                Directory.CreateDirectory(backpath);
            }
            int countfile = 1;

            // Check if all the files are ready to upgrade
            foreach (DataRow frow in files.Rows)
            {
                DateTime datemodified = (DateTime)frow["MODIFIED"];
                string npath = Path.Combine(FFilePath, frow["PATH"].ToString());
                Directory.CreateDirectory(npath);
                string filename = npath + Path.DirectorySeparatorChar + frow["FILE"].ToString();
                FileInfo nfinfo = new FileInfo(filename);
                bool docancel = false;
                if (OnProgress != null)
                    OnProgress(filename, countfile, files.Rows.Count, 0, ((byte[])frow["STREAM"]).Length, ref docancel);
                bool requireupgrade = true;
                if (nfinfo.Exists)
                {
                    //              if (nfinfo.LastWriteTimeUtc >= datemodified)
                    if (nfinfo.LastWriteTime >= datemodified)
                        requireupgrade = false;
                }
                else
                    requireupgrade = false;

                if ((excludefilename.Length > 0) && requireupgrade)
                {
                    if (frow["FILE"].ToString().ToUpper() == excludefilename)
                        requireupgrade = false;
                }
                if (requireupgrade)
                {
                    if (StreamUtil.FileInUse(filename))
                    {
                        throw new Exception("File in use: " + filename);
                    }
                }
            }
            foreach (DataRow xrow in files.Rows)
            {
                // Only update if version newer
                DateTime datemodified = (DateTime)xrow["MODIFIED"];
                string npath = Path.Combine(FFilePath, xrow["PATH"].ToString());
                Directory.CreateDirectory(npath);
                string filename = npath + Path.DirectorySeparatorChar + xrow["FILE"].ToString();
                FileInfo nfinfo = new FileInfo(filename);
                bool docancel = false;
                if (OnProgress != null)
                    OnProgress(filename, countfile, files.Rows.Count, 0, ((byte[])xrow["STREAM"]).Length, ref docancel);
                bool doupdate = true;
                if (nfinfo.Exists)
                {
#if PocketPC
/*            DateTime nlastwritetime;
            if (OnGetLastWriteTime != null)
                nlastwritetime = OnGetLastWriteTime(filename);
            else
                nlastwritetime = nfinfo.LastWriteTime;
            if (nlastwritetime >= datemodified)
                doupdate = false;*/
            doupdate = true;
#else
                    //          if (nfinfo.LastWriteTimeUtc >= datemodified)
                    if (nfinfo.LastWriteTime >= datemodified)
                        doupdate = false;
#endif
                }
                if (excludefilename.Length > 0)
                {
                    if (xrow["FILE"].ToString().ToUpper() == excludefilename)
                        doupdate = false;
                }
                if (doupdate)
                {
                    // Backup file
                    //System.Threading.Thread.Sleep(1000);

                    if (PerformBackup)
                    {
                        string nbackpath = Path.Combine(backpath, xrow["PATH"].ToString());
                        Directory.CreateDirectory(nbackpath);
                        if (File.Exists(filename))
                            File.Move(filename, nbackpath + Path.DirectorySeparatorChar + xrow["FILE"].ToString());
                    }
                    DateTime mmlast;
                    TimeSpan difmilis;
                    byte[] original = (byte[])xrow["STREAM"];
                    using (FileStream fstream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                    {
                        int index = 0;
                        int totalwritten = 0;
                        do
                        {
                            int towrite = BUFSIZE;
                            if ((original.Length - index) < BUFSIZE)
                                towrite = (original.Length - index);
                            fstream.Write(original, index, towrite);
                            totalwritten = totalwritten + towrite;
                            index = index + towrite;
                            if (OnProgress != null)
                            {

                                mmlast = System.DateTime.Now;
                                difmilis = mmlast - mmfirst;
                                if (difmilis.TotalMilliseconds > 500)
                                {
                                    OnProgress(filename, countfile, files.Rows.Count, totalwritten, original.Length, ref docancel);
                                    mmfirst = System.DateTime.Now;
                                }
                            }
                        } while (index < original.Length);
                    }
                    OnProgress(filename, countfile, files.Rows.Count, original.Length, original.Length, ref docancel);

                    nfinfo = new FileInfo(filename);
#if PocketPC
          if (OnSetLastWriteTime != null)
              OnSetLastWriteTime(filename, datemodified);
          else
              throw new Exception("OnSetLastWriteTime event must be provided");
          //nfinfo.LastWriteTime = datemodified;
#else
                    //          nfinfo.LastWriteTimeUtc = datemodified;
                    nfinfo.LastWriteTime = datemodified;
#endif
                    countfile++;
                }
            }
        }
    }
}
