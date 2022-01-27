using System;
using System.Linq;
using System.Text;
using System.IO;

namespace Reportman.Drawing
{
    /// <summary>
    /// String collection
    /// </summary>
	public class Strings : System.Collections.Generic.List<string>, ICloneable
    {
        /// <summary>
        /// Clone the object
        /// </summary>
        /// <returns>A object clone</returns>
		public object Clone()
        {
            Strings st = new Strings();
            foreach (string s in this)
            {
                st.Add(s);
            }
            return st;
        }
        /// <summary>
        /// Get a single string separating strings by a LINE FEED char, (char)10
        /// </summary>
		public string Text
        {
            get
            {
                StringBuilder astring = new StringBuilder();
                if (Count > 0)
                    astring.Append(this[0]);
                int i = 1;
                while (i < Count)
                {
                    astring.Append("" + (char)13 + (char)10 + this[i]);
                    i++;
                }
                return astring.ToString();
            }
            set
            {
                Fill(value);
            }
        }
        private void Fill(string astring)
        {
            Clear();
            int i = 0;
            StringBuilder partial = new StringBuilder();
            while (i < astring.Length)
            {
                if (astring[i] == (char)10)
                {
                    Add(partial.ToString());
                    partial = new StringBuilder();
                    if (i < astring.Length - 1)
                        if (astring[i + 1] == (char)13)
                            i++;
                }
                else
                {
                    partial.Append(astring[i]);
                }
                i++;
            }
            Add(partial.ToString());
        }
        /// <summary>
        /// Obtain a single string from a collection of strings stored in a single string
        /// </summary>
        /// <param name="astring">A list of strings represented by a single string separated by LF (char)10</param>
        /// <param name="index">Index to check</param>
        /// <returns>Returns a single string, obtained from index or an empty string if not found</returns>
		static public string GetStringByIndex(string astring, int index)
        {
            Strings alist;
            alist = new Strings();
            alist.Fill(astring);
            if (alist.Count < 1)
                return "";
            if (index < 0)
                index = 0;
            if (alist.Count < index)
            {
                return alist[index];
            }
            else
            {
                return alist[0];
            }
        }
        /// <summary>
        /// Generates a string collection (inside a single string) from a collection of strings an index and a string.
        /// So really, it inserts (or update) a line inside a group of lines.
        /// </summary>
        /// <param name="astring">A list of strings represented by a single string separated by LF (char)10</param>
        /// <param name="avalue">String value to insert into the list</param>
        /// <param name="index"></param>
        /// <returns>Collection of strings separated by LF with the line inserted or updated</returns>
		static public string SetStringByIndex(string astring, string avalue, int index)
        {
            Strings alist;
            string defaultvalue = "";
            alist = new Strings();
            if (index < 0)
                index = 0;
            alist.Fill(astring);
            if (alist.Count > 0)
                defaultvalue = alist[0];
            while ((alist.Count - 1) < index)
            {
                alist.Add(defaultvalue);
            }
            alist[index] = avalue;
            return alist.Text;
        }
        /// <summary>
        /// Converts a string with semicolons to a Strings collection
        /// </summary>
        /// <param name="semicolonstring"></param>
        /// Semicolon separated strings
        /// <returns></returns>
        public static Strings FromSemiColon(string semicolonstring)
        {
            return Strings.FromSeparator(';', semicolonstring);
        }
        /// <summary>
        /// Converts a string with any separator to a Strings collection
        /// </summary>
        /// <param name="separator"></param>
        /// Separator as char
        /// <returns></returns>
        /// <param name="nstring"></param>
        /// Semicolon separated strings
        /// <returns></returns>
        public static Strings FromSeparator(char separator, string nstring)
        {
            Strings aresult = new Strings();
            string partial = nstring;
            int index = partial.IndexOf(separator);
            while (index >= 0)
            {
                aresult.Add(partial.Substring(0, index));
                partial = partial.Substring(index + 1);
                index = partial.IndexOf(separator);
            }
            aresult.Add(partial);
            return aresult;
        }
        /// <summary>
        /// Remove the empty strings
        /// </summary>
        public void RemoveBlanks()
        {
            int i = 0;
            while (i < Count)
            {
                if (this[i].Length == 0)
                    this.RemoveAt(i);
                else
                    i++;
            }
        }
        /// <summary>
        /// Converts the string collection into a semicolon separated string
        /// </summary>
        /// <returns></returns>
        public string ToSemiColon()
        {
            if (Count < 1)
            {
                return "";
            }
            StringBuilder sbuilder = new StringBuilder(this[0]);
            int i;
            for (i = 1; i < Count; i++)
            {
                sbuilder.Append(';');
                sbuilder.Append(this[i]);
            }
            return sbuilder.ToString();
        }
        /// <summary>
        /// Converts the string collection into a semicolon separated string
        /// </summary>
        /// <returns></returns>
        public string ToCharSeparated(char separator)
        {
            if (Count < 1)
            {
                return "";
            }
            StringBuilder sbuilder = new StringBuilder(this[0]);
            int i;
            for (i = 1; i < Count; i++)
            {
                sbuilder.Append(separator);
                sbuilder.Append(this[i]);
            }
            return sbuilder.ToString();
        }
        public void LoadFromFile(string filename)
        {
            using (FileStream nstream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                LoadFromStream(nstream);
            }
        }
        public void LoadFromStream(Stream nstream)
        {
            using (MemoryStream mems = StreamUtil.StreamToMemoryStream(nstream))
            {
                mems.Seek(0, SeekOrigin.Begin);
                StreamReader nreader = new StreamReader(nstream, Encoding.UTF8);
                if (nreader.EndOfStream)
                {
                    string nstring = StreamUtil.ByteArrayToString(mems.ToArray(), System.Convert.ToInt32(mems.Length), true);
                    nstring = nstring.Replace((char)65279 + "", "");
                    Clear();
                    StringBuilder nline = new StringBuilder();
                    foreach (char c in nstring)
                    {
                        if (c != (char)13)
                        {
                            if (c == (char)10)
                            {
                                Add(nline.ToString());
                                nline = new StringBuilder();
                            }
                            else
                                nline.Append(c);
                        }
                    }
                    string aline = nline.ToString();
                    if (aline.Length > 0)
                        Add(aline);
                }
                else
                {
                    //string nstring = Encoding.UTF8.GetString(mems.ToArray());
                    // StreamUtil.ByteArrayToString(mems.ToArray(),System.Convert.ToInt32(mems.Length),true);
                    Clear();

                    while (!nreader.EndOfStream)
                    {
                        string nline = nreader.ReadLine().Trim();
                        if (nline.Length > 0)
                            Add(nline);
                    }
                }
            }
        }
    }
}
