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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Reportman.Drawing
{
    public class IniFile
    {
        string fname;
        Strings lines;
        NumberFormatInfo numberfor;
        public SortedList<string ,IniSection> sections;
        FileInfo finfo;
        public IniFile(string filename)
        {
            fname = filename;
            lines = new Strings();
            finfo = new FileInfo(fname);
            if (finfo.Exists)
                lines.LoadFromFile(filename);
            sections = new SortedList<string, IniSection>();
            ParseText();
        }
        public IniFile(Stream inistream)
        {
            lines = new Strings();
            lines.LoadFromStream(inistream);
            sections = new SortedList<string, IniSection>();
            ParseText();
        }
        public string ReadString(string sectionname, string valuename, string defaultvalue)
        {
            string aresult = defaultvalue;
            string asec=sectionname.ToUpper();
            if (sections.IndexOfKey(asec) >= 0)
            {
                IniSection inisec = sections[asec];
                string aval = valuename.ToUpper();
                if (inisec.Values.IndexOfKey(aval) >= 0)
                {
                    aresult = inisec.Values[aval];
                }
            }
            return aresult;
        }
        public void WriteString(string sectionname, string valuename, string newvalue)
        {
            string asec = sectionname.ToUpper();
            IniSection inisec;
            if (sections.IndexOfKey(asec) < 0)
            {
                inisec = new IniSection();
                sections.Add(asec,inisec);
            }
            else
                inisec = sections[asec];
            string aval = valuename.ToUpper();
            if (inisec.Values.IndexOfKey(aval) >= 0)
            {
                inisec.Values[aval] = newvalue;
            }
            else
                inisec.Values.Add(aval, newvalue);
        }
        public DateTime ReadDateTime(string sectionname, string valuename, DateTime defaultvalue)
        {
            DateTime aresult = defaultvalue;
            string asec = sectionname.ToUpper();
            if (sections.IndexOfKey(asec) >= 0)
            {
                IniSection inisec = sections[asec];
                string aval = valuename.ToUpper();
                if (inisec.Values.IndexOfKey(aval) >= 0)
                {
                    try
                    {
                        string avalue = inisec.Values[aval];
                        if (avalue.Length > 0)
                        {
                            int year = System.Convert.ToInt32(avalue.Substring(0, 4));
                            int month = System.Convert.ToInt32(avalue.Substring(4, 2));
                            int day = System.Convert.ToInt32(avalue.Substring(6, 2));

                            int hour = System.Convert.ToInt32(avalue.Substring(9, 2));
                            int minute = System.Convert.ToInt32(avalue.Substring(11, 2));
                            int second = System.Convert.ToInt32(avalue.Substring(13, 2));

                            aresult = new DateTime(year, month, day, hour, minute, second);
                        }
                        else
                            aresult = defaultvalue;
                    }
                    catch
                    {
                        aresult = defaultvalue;
                    }
                }
            }
            return aresult;
        }
        public void WriteDateTime(string sectionname, string valuename, DateTime newvalue)
        {
            string asec = sectionname.ToUpper();
            IniSection inisec;
            if (sections.IndexOfKey(asec) < 0)
            {
                inisec = new IniSection();
                sections.Add(asec, inisec);
            }
            else
                inisec = sections[asec];
            string aval = valuename.ToUpper();
            string datestring = newvalue.ToString("yyyyMMdd HHmmss");
            if (inisec.Values.IndexOfKey(aval) >= 0)
            {
                inisec.Values[aval] = datestring;
            }
            else
                inisec.Values.Add(aval, datestring);
        }
        public void WriteInteger(string sectionname, string valuename, int intvalue)
        {
            string newvalue = intvalue.ToString();
            string asec = sectionname.ToUpper();
            IniSection inisec;
            if (sections.IndexOfKey(asec) < 0)
            {
                inisec = new IniSection();
                sections.Add(asec, inisec);
            }
            else
                inisec = sections[asec];
            string aval = valuename.ToUpper();
            if (inisec.Values.IndexOfKey(aval) >= 0)
            {
                inisec.Values[aval] = newvalue;
            }
            else
                inisec.Values.Add(aval, newvalue);
        }
        public void WriteDecimal(string sectionname, string valuename, decimal decvalue)
        {
            CheckNumberFormat();
            valuename = valuename.ToUpper();
            string newvalue = decvalue.ToString(numberfor);
            string asec = sectionname.ToUpper();
            IniSection inisec;
            if (sections.IndexOfKey(asec) < 0)
            {
                inisec = new IniSection();
                sections.Add(asec, inisec);
            }
            else
                inisec = sections[asec];
            string aval = valuename.ToUpper();
            if (inisec.Values.IndexOfKey(aval) >= 0)
            {
                inisec.Values[aval] = newvalue;
            }
            else
                inisec.Values.Add(valuename, newvalue);
        }
        public void WriteBool(string sectionname, string valuename, bool boolvalue)
        {
            int defint = 0;
            if (boolvalue)
                defint = 1;

            WriteInteger(sectionname, valuename, defint);
        }
        public int ReadInteger(string sectionname, string valuename, int defaultvalue)
        {
            string sresult = ReadString(sectionname, valuename,defaultvalue.ToString());
            if (sresult.Length == 0)
                return defaultvalue;
            else
                return System.Convert.ToInt32(sresult);
        }
        public void CheckNumberFormat()
        {
            if (numberfor == null)
            {
                numberfor = new NumberFormatInfo();
                numberfor.NumberDecimalSeparator = ".";
                numberfor.NumberGroupSeparator = "";
            }
        }
        public decimal ReadDecimal(string sectionname, string valuename, decimal defaultvalue)
        {
            CheckNumberFormat();
            string sresult = ReadString(sectionname, valuename, defaultvalue.ToString(numberfor));
            if (sresult.Length == 0)
                return defaultvalue;
            else
                return System.Convert.ToDecimal(sresult,numberfor);
        }
        public bool ReadBool(string sectionname, string valuename, bool defaultvalue)
        {
            int defint=0;
            if (defaultvalue)
                defint = 1;

            int intresult = ReadInteger(sectionname, valuename, defint);
            return (intresult==1);
        }
        private void ParseText()
        {
            IniSection currentsection=null;
            foreach (string linex in lines)
            {
                string line = linex.Trim();
                // A section ?
                if (line.Length > 0)
                {
                    if (line[0] == '[')
                    {
                        string secname = line.Substring(1, line.Length - 1).ToUpper();
                        int index1 = secname.IndexOf(']');
                        if (index1 > 0)
                        {
                            secname = secname.Substring(0, index1);
                        }
                        currentsection = new IniSection();
                        if (sections.IndexOfKey(secname)<0)
                            sections.Add(secname,currentsection);
                    }
                    else
                    {
                        // Must be a value
                        if (currentsection != null)
                        {
                            string nvalue = "";
                            string valuename = line.ToUpper();
                            int index2 = line.IndexOf('=');
                            if (index2 > 0)
                            {
                                nvalue = line.Substring(index2+1,line.Length-index2-1);
                                valuename = line.Substring(0,index2).ToUpper();
                            }
                            if (currentsection.Values.IndexOfKey(valuename)<0)
                                currentsection.Values.Add(valuename, nvalue);
                        }
                    }
                }
            }
        }
        public void SaveToStream(Stream nstream)
        {
            Strings nstring = new Strings();
            foreach (string secname in sections.Keys)
            {
                IniSection inisec = sections[secname];
                nstring.Add("[" + secname + "]");
                foreach (string nkey in inisec.Values.Keys)
                {
                    nstring.Add(nkey +"="+inisec.Values[nkey]);
                }
            }
            string ntext = nstring.Text;
            byte[] content = StreamUtil.StringToByteArray(ntext, ntext.Length);
            nstream.Write(content, 0, content.Length);

        }
        public void SaveToFile(string filename)
        {
            string apath = Path.GetDirectoryName(filename);
            if (!Directory.Exists(apath))
              Directory.CreateDirectory(apath);
            using (FileStream fstream=new FileStream(filename,FileMode.Create,FileAccess.Write))
            {
                SaveToStream(fstream);
            }
        }
    }

    public class IniSection
    {
        public SortedList<string, string> Values;
        public IniSection()
        {
            Values = new SortedList<string, string>();
        }
    }
}
