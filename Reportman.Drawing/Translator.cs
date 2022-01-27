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
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
#if NETSTANDARD2_0
#else
using System.Drawing;
#endif

namespace Reportman.Drawing
{
    /// <summary>
    /// Translator component helps in translating aplications to multiple languages
    /// using current user regional configuration loads the specified file
    /// with the regional extension for the file
    /// </summary>
#if NETSTANDARD2_0
#else
    [ToolboxBitmapAttribute(typeof(Translator), "translator.ico")]
#endif
    public class Translator : System.ComponentModel.Component
	{
        private static bool DefaultStringsLoaded = false;
		private static string[] DefaultStrings;
        private static string[] ResourceNamesExec;
        private static Assembly ExecAssembly;
        private static Assembly ReportmanAssembly;
        private static string[] ResourceNames;
        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private bool FAutoLocale;
		private bool FActive;
		private string FFilename;
		private ArrayList FContent;
        private bool FLookForResource;
		private void DoInit()
		{
			const int DEFAULT_TRANS_SIZE = 1000;

			FActive = false;
			FFilename = "";
            FLookForResource = true;
			FContent = new ArrayList(DEFAULT_TRANS_SIZE);
		}
        public bool LookForResource
        {
            get { return FLookForResource; }
            set { FLookForResource = value; }
        }
        /// <summary>
        /// Translator is a component to help the translation of applications in multiple languages.
        /// It can open a translation file created from Report Manager Translation utility.
        /// It's capable of searching for a file with a file extension according to the regional
        /// settings, that is for example if regional setting is Spanish Mexico, it will search for
        /// a file with extension .ESM, but if not found, it will search for a file with extension .ES.
        /// </summary>
        /// <param name="container">The container of the Translator, when used in Windows.Forms designer</param>
		public Translator(System.ComponentModel.IContainer container)
		{
			//
			// Required for Windows.Forms designer
			//
			container.Add(this);
			InitializeComponent();

			DoInit();
		}
        /// <summary>
        /// Simple constructor for the Translator component
        /// </summary>
		public Translator()
		{
			//
			// Required for Windows.Forms designer
			//
			InitializeComponent();

			DoInit();
		}

		/// <summary> 
		/// Dispose used elements.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (components != null)
					{
						components.Dispose();
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		private string FindLocalFilename()
		{
			// Reads the file into the array
			if (FFilename.Length < 1)
				throw new UnNamedException("No Filename property asigned to Translator");
			string afilename = FFilename;

			if (afilename.Length < 1)
			{
				// If no filename assigned get executable name
				Process currentProcess = Process.GetCurrentProcess();
				afilename = currentProcess.Modules[0].FileName;
			}
			if (FAutoLocale)
			{

				//string extens = System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName;
                string extens = System.Globalization.CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName;
				afilename = System.IO.Path.ChangeExtension(afilename, extens);
				FileInfo afileinfo = new FileInfo(afilename);
				if (!afileinfo.Exists)
				{
					extens = extens.Substring(0,2);
					afilename = System.IO.Path.ChangeExtension(afilename, extens);
					afileinfo = new FileInfo(afilename);
					if (!afileinfo.Exists)
					{
						// Default english
						extens = "en";
						afilename = System.IO.Path.ChangeExtension(afilename, extens);
					}
				}
			}
			return afilename;

		}
        public static Stream FindResource(bool searchonly,ref bool found,string nfilename)
        {
            bool resourcefound=false;
            bool atreportman = false;
			Monitor.Enter(flag);
            try
            {
                if (ResourceNamesExec == null)
                {
                    ExecAssembly = System.Reflection.Assembly.GetEntryAssembly();
                    if (ExecAssembly != null)
                    {
                        ResourceNamesExec = ExecAssembly.GetManifestResourceNames();
                    }
                    else
                        ResourceNamesExec = new string[0];
                }
            }
            finally
            {
                Monitor.Exit(flag);
            }
            //CultureInfo nculture = Thread.CurrentThread.CurrentUICulture
            //string extens2 = System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower();
            //string extens3 = System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName.ToLower();
            System.Globalization.CultureInfo currentc = System.Globalization.CultureInfo.CurrentCulture;
            string extens3 = currentc.ThreeLetterWindowsLanguageName.ToUpper();
            string extens2 = extens3.Substring(0, 2);
            string resname="";
            int maxlength = nfilename.Length+4;
            foreach (string s in ResourceNamesExec)
            {
                if (s.Length >= maxlength)
                {
                    if (s.Substring(s.Length - maxlength, maxlength).ToUpper() == nfilename.ToUpper() + "." + extens3)
                    {
                        resname = s;
                        resourcefound = true;
                        break;
                    }
                    if (s.Substring(s.Length - maxlength +1, maxlength-1).ToUpper() == nfilename.ToUpper() + "." + extens2)
                    {
                        resname=s;
                        resourcefound=true;
                        break;
                    }
                }
            }
            if ((!resourcefound) && (nfilename == "reportmanres")) 
            {
                atreportman = true;
    			Monitor.Enter(flag);
                try
                {
                    
                        ReportmanAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                        ResourceNames = ReportmanAssembly.GetManifestResourceNames();
                }
                finally
                {
                    Monitor.Exit(flag);
                }
                foreach (string s in ResourceNames)
                {
                    if (s.Length >= maxlength)
                    {
                        if (s.Substring(s.Length - maxlength, maxlength).ToUpper() == nfilename.ToUpper() + "." + extens3)
                        {
                            resname = s;
                            resourcefound = true;
                            break;
                        }
                        if (s.Substring(s.Length - maxlength+1, maxlength - 1).ToUpper() == nfilename.ToUpper() + "." + extens2)
                        {
                            resname = s;
                            resourcefound = true;
                            break;
                        }
                    }
                }
            }
            Stream nstream=null;
            // Execution assembly first
            if ((!searchonly) && resourcefound)
            {
                if (atreportman)
                    nstream = ReportmanAssembly.GetManifestResourceStream(resname);
                else
                    nstream = ExecAssembly.GetManifestResourceStream(resname);
            }
            found=resourcefound;
            return nstream;            
        }
        public Stream FindResource()
        {
            bool found = false;
            return FindResource(false,ref found,FFilename);
        }
        /// <summary>
		/// Check if the resource file exists.
		/// </summary>
		/// <returns>
		/// Returns true if the resource file exists or a resource can be found
		/// </returns>
		public bool FileExists()
		{
            bool resourcefound = false;
            // Check first for a resource
            if (FLookForResource)
            {
                FindResource(true,ref resourcefound,FFilename);
            }
            if (resourcefound)
                return true;
            string afilename = FindLocalFilename();
			FileInfo afileinfo = new FileInfo(afilename);
			return (afileinfo.Exists);
		}
		/// <summary>
		/// Call this method to reload content from resource file
		/// </summary>
		public void UpdateContent()
		{
            bool resourcefound=false;
            // Check first for a resource
            if (FLookForResource)
            {
                Stream nstream = FindResource(false, ref resourcefound,FFilename);
                if (nstream != null)
                {
                    using (nstream)
                    {
                        LoadFromStream(nstream);
                    }
                }
            }
            if (!resourcefound)
            {
                string afilename = FindLocalFilename();
                FileInfo afileinfo = new FileInfo(afilename);
                if (!afileinfo.Exists)
                {
                    throw new NamedException("File not found:" + afilename, afilename);
                }
                LoadFromFile(afilename);
            }
		}
		/// <summary>
		/// Set this property to true to open the resource file.
		/// Before obtaining translations, this property must be set.
		/// </summary>
		public bool Active
		{
			get
			{
				return FActive;
			}
			set
			{
				FActive = value;
				if (FActive)
					UpdateContent();
				else
					FContent.Clear();
			}
		}
		/// <summary>
		/// When this property is set to true, the provided file name will
		/// extended with the current user regional extension
		/// </summary>
		public bool AutoLocale
		{
			get
			{
				return FAutoLocale;
			}
			set
			{
				bool oldactive = FActive;
				Active = false;
				FAutoLocale = value;
				Active = oldactive;
			}
		}
		/// <summary>
		/// Resource file to load strings from, you usually provide
		/// the file name without extension, so with, Autolocale set
		/// to true, the extension will be added based on the current
		/// user regional settings.
		/// </summary>
		public string Filename
		{
			get
			{
				return FFilename;
			}
			set
			{
				bool oldactive = FActive;
				Active = false;
				FFilename = value;
				Active = oldactive;
			}
		}
		/// <summary>
		/// Number of items in the resource file
		/// </summary>
		public int Count
		{
			get
			{
				return FContent.Count;
			}
		}
		/// <summary>
		/// You can access translation providing an integer index.
		/// The Active property must be set to true before accessing translations
		/// </summary>
		public string this[int index]
		{
			get
			{
				return (String)FContent[index];
			}
		}
		/// <summary>
		/// You can add elements to the resource using this function.
		/// </summary>
		/// <returns>
		/// Returns the index for the element added.
		/// </returns>
		public int AddString(string astring)
		{
			FContent.Add(astring);
			return FContent.Count-1;
		}
		/// <summary>
		/// Load the resource from any Stream.
		/// </summary>
		public void LoadFromStream(Stream astream)
		{
			const int READ_BUFSIZE = 8192;
			int readed;
			// Load the strings from a stream
			byte[] abuffer = new byte[READ_BUFSIZE];
			string astring = "";
			bool foundlf;
			int i;

			// Clear strings array
			FContent.Clear();

			foundlf = false;
			astream.Seek(0, System.IO.SeekOrigin.Begin);
			readed = astream.Read(abuffer, 0, READ_BUFSIZE);
			while (readed > 0)
			{
				for (i = 0; i < (readed / 2); i++)
				{
					if (((char)((abuffer[(i * 2) + 1] << 8) + abuffer[(i * 2)])) == (char)10)
					{
						if (foundlf)
						{
							foundlf = false;
							astring = astring + (char)10;
						}
						else
							foundlf = true;
					}
					else
					{
						if (foundlf)
						{
							AddString(astring);
							astring = "";
							foundlf = false;
						}
						astring = astring + (char)((abuffer[(i * 2) + 1] << 8) + abuffer[(i * 2)]);
					}
				}
				readed = astream.Read(abuffer, 0, READ_BUFSIZE);
			}
			if (astring.Length > 0)
			{
				AddString(astring);
			}
			FActive = true;
		}
        /// <summary>
		/// Load the resource from a file.
		/// </summary>
		public void LoadFromFile(String filename)
		{
			// Load the strings from a stream
			FileStream astream = new FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			try
			{
				LoadFromStream(astream);
			}
			finally
			{
				astream.Close();
			}
		}
		private static Translator atrans=null;
		private static string currentdefaultpath = "";
		private static object flag=1;

		/// <summary> 
		/// TranslateStr is a class function, can be called without instantiating any
		/// Translator, it will look for reportmanres.xxx resource files
		/// the file must be in path, else a default string will be returned
		/// 
		/// <sample>
		/// this.Text=Reportman.Translator.TranslateStr(4);
		/// </sample>
		/// </summary>
		public static string TranslateStr(int index)
		{
			const string DEFAULT_STRING="XXXXXXX";
			
			Monitor.Enter(flag);
			try
			{
                if (!DefaultStringsLoaded)
                    LoadDefaultStrings();
                if (atrans == null)
				{
					atrans = new Translator();
					atrans.Filename = "reportmanres";
					atrans.AutoLocale = true;
                    if (atrans.FileExists())
                    {
                        atrans.Active = true;
                    }
				}
			}
			finally
			{
				Monitor.Exit(flag);
			}
			string newvalue = "";
			if (atrans.Active)
			{
					if (atrans.Count>index)
						newvalue = atrans[index];
					else
					{
						if (index < DefaultStrings.GetLength(0))
							newvalue = DefaultStrings[index];
						else
						{
							newvalue = DEFAULT_STRING;
						}
					}
			}
			else
			{
				if (index < DefaultStrings.GetLength(0))
					newvalue = DefaultStrings[index];
				else
				{
					newvalue = DEFAULT_STRING;
				}
			}
			if (index == 448)
				newvalue = newvalue.Replace("%s", "{0}");
			return newvalue;
		}
        /// <summary>
        /// You can override the default path to the search of translation files, useful for example
        /// when your translation files are not in the application directory, for example in web
        /// applications
        /// </summary>
        /// <param name="apath"></param>
		public static void SetDefaultTranslatorPath(string apath)
		{
			if (currentdefaultpath == apath)
				return;
			currentdefaultpath = apath;
			Monitor.Enter(flag);
			try
			{
				if (atrans != null)
				{
					atrans.Dispose();
					atrans = null;
				}
				atrans = new Translator();
				atrans.Filename = currentdefaultpath + "reportmanres";
				atrans.AutoLocale = true;
				if (atrans.FileExists())
				{
					atrans.Active = true;
				}
			}
			finally
			{
				Monitor.Exit(flag);
			}
		}
        /// <summary>
        /// Fills a list with language descriptions
        /// </summary>        
        public static void GetLanguageDescriptions(IList alist)
        {
            alist.Clear();
            alist.Add(TranslateStr(264)); // English
            alist.Add(TranslateStr(265)); // Spanish
            alist.Add(TranslateStr(266)); // Catalan
            alist.Add(TranslateStr(953)); // French
            alist.Add(TranslateStr(1218)); // Portuguesse
            alist.Add(TranslateStr(1219)); // German
            alist.Add(TranslateStr(1220)); // Italian
            alist.Add(TranslateStr(1258)); // Turkish
            alist.Add(TranslateStr(1383)); // Lithuanian
            alist.Add(TranslateStr(1387)); // Greek
            alist.Add(TranslateStr(1388)); // 10 -Hungar 
            alist.Add(TranslateStr(1384)); // Danish
            alist.Add(TranslateStr(1385)); // Dutch
            alist.Add(TranslateStr(1386)); // Spanish Mexico
            //alist.Add(TranslateStr(1385)); // Spanish Argentina
        }
        private static void LoadDefaultStrings()
        {
            if (DefaultStringsLoaded)
                return;
            DefaultStringsLoaded = true;
            using (Translator tr=new Translator())
            {
                using (MemoryStream mstream=new MemoryStream())
                {
                    mstream.Write(resource.reportmanres,0,resource.reportmanres.Length);
                    mstream.Seek(0,SeekOrigin.Begin);
                    tr.LoadFromStream(mstream);
                }
                DefaultStrings = new string[tr.Count];
                for (int i = 0; i < tr.Count; i++)
                {
                    DefaultStrings[i] = tr[i];
                }                    
            }
        }

#region Generated code by Desginer
        /// <summary>
        /// Necessary method to allow design
        /// </summary>
        private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
#endregion
	}
}
