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
using System.Text;
using System.Globalization;
using Reportman.Drawing;
using System.Threading;
#if REPMAN_ZLIB
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
#endif
using System.Collections.Generic;

namespace Reportman.Reporting
{
	/// <summary>
	/// Class used to read report template from a file (or stream) saved in xml format
	/// </summary>
	public class ReportReader
	{
		BaseReport areport;
		string astring;
		int position;
		string propname, proptype, propsize;
		string propvalue;
		string compclass;
		NumberFormatInfo numberformat;
		const int C_MAXDATAWIDTH = 40;
		/// <summary>
		/// Create the report reader instance, you must supply the report where the
		/// template will be loaded
		/// </summary>
		/// <param name="rp"></param>
		public ReportReader(BaseReport rp)
		{
			areport = rp;
			numberformat = new NumberFormatInfo();
			numberformat.NumberDecimalSeparator = ".";
			numberformat.NumberGroupSeparator = "";
		}
		private void ReadBuf(Stream astream, ref byte[] buf, int count)
		{
			if (astream.Read(buf, 0, count) < count)
				throw new UnNamedException("Error reading from stream in Report Reader");
		}
		private void InvalidFormatException()
		{
			throw new UnNamedException("Invalid format");
		}
		/// <summary>
		/// Converts not dot net report to a dot net report
		/// </summary>
		/// <param name="rp"></param>
		/// <param name="driver"></param>
		/// <param name="provider_factory"></param>
		public static void ConvertToDotNet(Report rp, DriverType driver, string provider_factory)
		{
			foreach (DataInfo dinfo in rp.DataInfo)
			{
				DatabaseInfo baseinfo = rp.DatabaseInfo[dinfo.DatabaseAlias];
				if (baseinfo != null)
				{
					if (baseinfo.Driver == DriverType.BDE)
					{
						if (dinfo.BDEType == DatasetType.Table)
						{
							dinfo.SQL = "SELECT * FROM " + dinfo.BDETable;

							string sqlwhere = "";
							if (dinfo.DataSource.Length > 0)
							{
								Strings masterfields = Strings.FromSemiColon(dinfo.BDEMasterFields.ToUpper());
								Strings indexfieldnames = Strings.FromSemiColon(dinfo.BDEIndexFields.ToUpper());
								int i = 0;
								foreach (string nombre in masterfields)
								{
									if (sqlwhere.Length == 0)
										sqlwhere = " WHERE ";
									else
										sqlwhere = sqlwhere + " AND ";
									sqlwhere = sqlwhere + indexfieldnames[i] + "=@" + nombre;
									// Add param, 
									i++;
									// Obtain parameter name
									string aparam = nombre;
									int index2 = rp.Params.IndexOf(aparam);
									// The param must exist
									Param nparam;
									if (index2 < 0)
									{
										nparam = new Param(rp);
										nparam.Value = "";
										nparam.Alias = aparam;
										rp.Params.Add(nparam);
									}
									else
										nparam = rp.Params[index2];
									// The param must be assigned to dataset
									index2 = nparam.Datasets.IndexOf(dinfo.Alias);
									if (index2 < 0)
										nparam.Datasets.Add(dinfo.Alias);
								}
							}
							dinfo.SQL = dinfo.SQL + sqlwhere;

							if (dinfo.BDEIndexFields.Length > 0)
							{
								Strings sortlist = Strings.FromSemiColon(dinfo.BDEIndexFields);
								if (sortlist.Count > 0)
								{
									string orderby = " ORDER BY " + sortlist[0];
									for (int i = 1; i < sortlist.Count; i++)
										orderby = orderby + "," + sortlist[i];
									dinfo.SQL = dinfo.SQL + orderby;
								}
							}
						}

					}
				}
				string sql = dinfo.SQL;
				int index = sql.IndexOf(':');
				while (index >= 0)
				{
					// Obtain parameter name
					StringBuilder paramname = new StringBuilder();
					int index2 = index + 1;
					while (index2 < sql.Length)
					{
						char caracter = sql[index2];
						if (!(StringUtil.IsAlpha(caracter) && caracter != ' ' && caracter != ')'
							  && caracter != '.' && caracter != '(' && caracter != ':' && caracter != ';' && caracter != '='))
							break;
						paramname.Append(caracter);
						index2++;
					}
					string aparam = paramname.ToString().ToUpper();
					index2 = rp.Params.IndexOf(aparam);
					// The param must exist
					Param nparam;
					if (index2 < 0)
					{
						nparam = new Param(rp);
						nparam.Value = "";
						nparam.Alias = aparam;
						rp.Params.Add(nparam);
					}
					else
						nparam = rp.Params[index2];
					// The param must be assigned to dataset
					index2 = nparam.Datasets.IndexOf(dinfo.Alias);
					if (index2 < 0)
						nparam.Datasets.Add(dinfo.Alias);
					// Replace ':' with '@'
					sql = sql.Substring(0, index) + '@' + aparam + sql.Substring(index + 1 + aparam.Length, sql.Length - index - 1 - aparam.Length);
					index = sql.IndexOf(':');
				}
				dinfo.SQL = sql;
			}
			foreach (DatabaseInfo dbinfo in rp.DatabaseInfo)
			{
				if (dbinfo.Driver != DriverType.Mybase)
				{
					if (dbinfo.Driver == DriverType.IBX)
						if (provider_factory.Length == 0)
							provider_factory = "FirebirdSql.Data.Firebird";
					dbinfo.Driver = driver;
					dbinfo.ProviderFactory = provider_factory;
				}
			}
			// All expression components, search for display format

			foreach (ReportItem nitem in rp.Components)
			{
				if (nitem is ExpressionItem)
				{
					ExpressionItem eitem = (ExpressionItem)nitem;
					string dformat = eitem.DisplayFormat;
					if (dformat.Length > 0)
					{
						dformat = dformat.Replace("mmmm", "MMMM");
						dformat = dformat.Replace("mmm", "MMM");
						dformat = dformat.Replace("/mm", "/MM");
						dformat = dformat.Replace(":nn", ":mm");
						dformat = dformat.Replace("hh:", "HH:");
						dformat = dformat.Replace("0,.00", ",0.00");
						eitem.DisplayFormat = dformat;
					}
				}
			}
		}
		/// <summary>
		/// Load a report template from a stream with xml report template format
		/// </summary>
		/// <param name="astream"></param>
		public void LoadFromStream(Stream astream)
		{
			MemoryStream memstream = StreamUtil.StreamToMemoryStream(astream);
			memstream.Seek(0, System.IO.SeekOrigin.Begin);
			ReadXML(memstream);

			// Check if the database is no dot net and if 
		}

		/// <summary>
		/// Load a report template from a stream with xml report template format
		/// </summary>
		/// <param name="astream"></param>
		/// <param name="bufsize"></param>
		public void LoadFromStream(Stream astream, int bufsize)
		{
			MemoryStream memstream = StreamUtil.StreamToMemoryStream(astream,
				bufsize);
			memstream.Seek(0, System.IO.SeekOrigin.Begin);
			ReadXML(memstream);
		}
		/// <summary>
		/// Load a report template from a file with xml report template format
		/// </summary>
		/// <param name="afilename"></param>
		public void LoadFromFile(string afilename)
		{
			FileStream stream = new FileStream(afilename, System.IO.FileMode.Open,
				System.IO.FileAccess.Read);
			try
			{
				LoadFromStream(stream);
			}
			finally
			{
				stream.Close();
			}
		}
		public List<PrintPosItem> ReadFromString(string contents)
		{
			List<PrintPosItem> nresult = new List<PrintPosItem>();
			astring = contents;
			position = 0;
			FindNextName();
			while (propname != "/SECTION")
			{
				// Read Section props
				if (propname == "COMPONENT")
				{
					FindNextName();
					if (propname != "NAME")
						InvalidFormatException();
					string compname = GetAsString();
					FindNextName();
					FindNextName();
					if (propname != "CLASSNAME")
						InvalidFormatException();
					compclass = GetAsString();
					PrintPosItem comp = CreateComponent();
					comp.Name = compname;
					nresult.Add(comp);
					FindNextName();
					while (propname != "/COMPONENT")
					{
						// Read component props
						ReadCompProp(comp);
						FindNextName();
					}
				}
				else
				  if (propname == "SECTION")
				{
					FindNextName();
				}
				else
					if (propname == "/COMPONENT")
				{
					FindNextName();
				}
				else
				{
					throw new Exception("Invalid XML name " + propname);
				}
			}
			return nresult;
		}

		private void ReadXML(MemoryStream astream)
		{
			MemoryStream memstream = new MemoryStream();

			byte[] buf = new byte[astream.Length];
			int asize = (int)astream.Length;
			ReadBuf(astream, ref buf, asize);
			memstream.Write(buf, 0, buf.Length);
			memstream.Seek(0, System.IO.SeekOrigin.Begin);

			if (buf[0] == 'x')
			{
#if REPMAN_ZLIB
				byte[] bufuncomp = new byte[100000];
				ICSharpCode.SharpZipLib.Zip.Compression.Inflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
				ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream zstream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(memstream, inf);
				MemoryStream mems = new MemoryStream();
				int readed = zstream.Read(bufuncomp, 0, 100000);
				while (readed > 0)
				{
					mems.Write(bufuncomp, 0, readed);
					readed = zstream.Read(bufuncomp, 0, 100000);
				}
				mems.Seek(0, System.IO.SeekOrigin.Begin);
				asize = (int)mems.Length;
				buf = new byte[asize];
				ReadBuf(mems, ref buf, asize);
#else
				throw new UnNamedException("REPMAN_ZLIB not defined. ZLib compression not supported");
#endif
			}
			if (asize == 0)
				throw new UnNamedException("Zero Length stream");
			char[] achar = new char[asize];
			for (int i = 0; i < asize; i++)
				achar[i] = (char)buf[i];
			astring = new string(achar, 0, asize - 1);

			IntReadFromString();
		}
		private void FindNextName()
		{
			int abegin, aend;
			int typepos;
			int endpos;
			string props;

			propname = "";
			proptype = "";
			propsize = "";
			abegin = 0;
			aend = 0;
			while (position < astring.Length)
			{
				if (astring[position] == '<')
				{
					if (abegin > 0)
						InvalidFormatException();
					abegin = position + 1;
				}
				else
					if (astring[position] == '>')
				{
					if (abegin == 0)
						InvalidFormatException();
					aend = position;
					position++;
					break;
				}
				position++;
			}
			if (aend == 0)
				InvalidFormatException();
			propname = astring.Substring(abegin, aend - abegin).Trim();
			typepos = propname.IndexOf(" ");
			props = "";
			if (typepos >= 0)
			{
				props = propname.Substring(typepos + 1, propname.Length - (typepos + 1));
				propname = propname.Substring(0, typepos);
			}
			typepos = props.IndexOf("size");
			if (typepos >= 0)
			{
				propsize = props.Substring(typepos + 6, props.Length - (typepos + 6));
				endpos = propsize.IndexOf("\"");
				if (endpos <= 0)
					InvalidFormatException();
				propsize = propsize.Substring(0, endpos);
				props = props.Substring(endpos + 1, props.Length - (endpos + 1)).Trim();
			}
			typepos = props.IndexOf("type");
			if (typepos >= 0)
			{
				proptype = props.Substring(typepos + 6, props.Length - (typepos + 6));
				endpos = proptype.IndexOf("\"");
				if (endpos <= 0)
					InvalidFormatException();
				proptype = proptype.Substring(0, endpos);
			}
			propvalue = "";
			StringBuilder avalue = new StringBuilder("");
			while (position < astring.Length)
			{
				if (astring[position] == '<')
					break;
				else
				{
					avalue.Append(astring[position]);
					position++;
				}
			}
			propvalue = avalue.ToString();
			if (propname.Length == 0)
				InvalidFormatException();
		}
		private void FillStringsNoTrim(Strings list)
		{
			string avalue = GetAsString();
			int index = avalue.IndexOf((char)10);
			while (index >= 0)
			{
				list.Add(avalue.Substring(0, index));
				avalue = avalue.Substring(index + 1, avalue.Length - (index + 1));
				index = avalue.IndexOf((char)10);
			}
			if (avalue.Length > 0)
				list.Add(avalue);
		}
		private void FillStrings(Strings list)
		{
			string avalue = GetAsString();
			int index = avalue.IndexOf((char)10);
			while (index >= 0)
			{
				list.Add(avalue.Substring(0, index).Trim());
				avalue = avalue.Substring(index + 1, avalue.Length - (index + 1)).Trim();
				index = avalue.IndexOf((char)10);
			}
			avalue = avalue.Trim();
			if (avalue.Length > 0)
				list.Add(avalue);
		}
		private string GetAsString()
		{
			string anumber;
			string aresult = "";
			int i = 0;
			while (i < propvalue.Length)
			{
				if ((StringUtil.IsAlpha(propvalue[i])) || (propvalue[i] == '#'))
				{
					if (propvalue[i] == '#')
					{
						anumber = "0";
						i++;
						while (i < propvalue.Length)
						{
							if (propvalue[i] == '#')
							{
								i++;
								break;
							}
							else
							{
								anumber = anumber + propvalue[i];
								i++;
							}
						}
						aresult = aresult + (char)System.Convert.ToInt32(anumber);
					}
					else
					{
						aresult = aresult + propvalue[i];
						i++;
					}
				}
				else
					i++;
			}
			return (aresult);
		}
		private int GetAsInteger()
		{
			return (System.Convert.ToInt32(propvalue));
		}
		private DateTime GetAsDateTime()
		{
			DateTime adate = ((new DateTime(1899, 12, 30)).Add(DateUtil.DelphiDateTimeToTimeSpan(GetAsDouble())));
			return adate;
		}
		private double GetAsDouble()
		{
			return (System.Convert.ToDouble(propvalue, numberformat));
		}
		private bool GetAsBool()
		{
			return (propvalue == "True");
		}
		private void IntReadFromString()
		{
			position = astring.IndexOf("<REPORT");
			if (position < 0)
				throw new UnNamedException("Report must begin with <REPORT" + (char)10 + astring);
			FindNextName();
			FindNextName();
			while (propname.Length > 0)
			{
				if (propname == "/REPORT")
					break;
				if (propname == "DATABASEINFO")
				{
					FindNextName();
					if (propname != "ALIAS")
						InvalidFormatException();
					DatabaseInfo dbitem = new DatabaseInfo(areport);
					dbitem.Alias = GetAsString();
					areport.DatabaseInfo.Add(dbitem);
					FindNextName();
					while (propname != "/DATABASEINFO")
					{
						ReadPropDBInfo(dbitem);
						FindNextName();
					}
				}
				else
					if (propname == "DATAINFO")
				{
					FindNextName();
					if (propname != "ALIAS")
						InvalidFormatException();
					DataInfo ditem = new DataInfo(areport);
					ditem.Alias = GetAsString();
					areport.DataInfo.Add(ditem);
					FindNextName();
					while (propname != "/DATAINFO")
					{
						ReadPropDataInfo(ditem);
						FindNextName();
					}
				}
				else
						if (propname == "PARAMETER")
				{
					FindNextName();
					if (propname != "NAME")
						InvalidFormatException();
					Param aparam = new Param(areport);
					aparam.Alias = GetAsString();
					//                            aparam.Name = GetAsString();
					areport.Params.Add(aparam);
					FindNextName();
					while (propname != "/PARAMETER")
					{
						ReadPropParam(aparam);
						FindNextName();
					}
				}
				else
							if (propname == "SUBREPORT")
				{
					FindNextName();
					if (propname != "NAME")
						InvalidFormatException();
					SubReport subrep = new SubReport(areport);
					subrep.Name = GetAsString();
					areport.SubReports.Add(subrep);
					FindNextName();
					while (propname != "/SUBREPORT")
					{
						// Read subreport props
						if (propname == "SECTION")
						{
							FindNextName();
							if (propname != "NAME")
								InvalidFormatException();
							Section sec = new Section(areport);
							subrep.Sections.Add(sec);
							sec.Name = GetAsString();
							FindNextName();
							while (propname != "/SECTION")
							{
								// Read Section props
								if (propname == "COMPONENT")
								{
									FindNextName();
									if (propname != "NAME")
										InvalidFormatException();
									string compname = GetAsString();
									FindNextName();
									FindNextName();
									if (propname != "CLASSNAME")
										InvalidFormatException();
									compclass = GetAsString();
									PrintPosItem comp = CreateComponent();
									comp.Name = compname;

									sec.Components.Add(comp);
									comp.Section = sec;
									FindNextName();
									while (propname != "/COMPONENT")
									{
										// Read component props
										ReadCompProp(comp);
										FindNextName();
									}
								}
								else
									ReadPropSection(sec);
								FindNextName();
							}
						}
						else
							ReadPropSubReport(subrep);
						FindNextName();
					}
				}
				else
					ReadPropReport();
				FindNextName();
			}
			// Reload links
			for (int i = 0; i < areport.SubReports.Count; i++)
			{
				SubReport subrep = areport.SubReports[i];
				if (subrep.ParentSub.Length > 0)
				{
					subrep.ParentSubReport = (SubReport)areport.Components[subrep.ParentSub];
					if (subrep.ParentSubReport == null)
						throw new NamedException("ParentSubReport not found:" + subrep.ParentSub, subrep.ParentSub);
				}
				if (subrep.ParentSec.Length > 0)
				{
					subrep.ParentSection = (Section)areport.Components[subrep.ParentSec];
					if (subrep.ParentSection == null)
						throw new NamedException("ParentSection not found:" + subrep.ParentSec, subrep.ParentSec);
				}
				for (int j = 0; j < subrep.Sections.Count; j++)
				{
					Section sec = subrep.Sections[j];
					if (sec.ChildSubReportName.Length > 0)
					{
						sec.ChildSubReport = (SubReport)areport.Components[sec.ChildSubReportName];
						if (sec.ChildSubReport == null)
							throw new NamedException("ChildSubReport not found:" + sec.ChildSubReportName, sec.ChildSubReportName);
					}
					if (sec.SubReportName.Length > 0)
					{
						sec.SubReport = (SubReport)areport.Components[sec.SubReportName];
						if (sec.SubReport == null)
							throw new NamedException("SubReport not found:" + sec.SubReportName, sec.SubReportName);
					}
				}
			}
		}
		private void ReadPropReport()
		{
			switch (propname)
			{
				case "WFONTNAME":
					areport.WFontName = GetAsString();
					break;
				case "LFONTNAME":
					areport.LFontName = GetAsString();
					break;
				case "GRIDVISIBLE":
					areport.GridVisible = GetAsBool();
					break;
				case "GRIDLINES":
					areport.GridLines = GetAsBool();
					break;
				case "GRIDENABLED":
					areport.GridEnabled = GetAsBool();
					break;
				case "GRIDCOLOR":
					areport.GridColor = GetAsInteger();
					break;
				case "GRIDWIDTH":
					areport.GridWidth = GetAsInteger();
					break;
				case "GRIDHEIGHT":
					areport.GridHeight = GetAsInteger();
					break;
				case "PAGEORIENTATION":
					areport.PageOrientation = (OrientationType)GetAsInteger();
					break;
				case "PAGESIZE":
					areport.PageSize = (PageSizeType)GetAsInteger();
					break;
				case "PAGESIZEQT":
					areport.PageSizeIndex = GetAsInteger();
					break;
				case "PAGEHEIGHT":
					areport.PageHeight = GetAsInteger();
					break;
				case "PAGEWIDTH":
					areport.PageWidth = GetAsInteger();
					break;
				case "CUSTOMPAGEHEIGHT":
					areport.CustomPageHeight = GetAsInteger();
					break;
				case "CUSTOMPAGEWIDTH":
					areport.CustomPageWidth = GetAsInteger();
					break;
				case "PAGEBACKCOLOR":
					areport.PageBackColor = GetAsInteger();
					break;
				case "PREVIEWSTYLE":
					areport.AutoScale = (AutoScaleType)GetAsInteger();
					break;
				case "PREVIEWMARGINS":
					areport.PreviewMargins = GetAsBool();
					break;
				case "PREVIEWWINDOW":
					areport.PreviewWindow = (PreviewWindowStyleType)GetAsInteger();
					break;
				case "LEFTMARGIN":
					areport.LeftMargin = GetAsInteger();
					break;
				case "TOPMARGIN":
					areport.TopMargin = GetAsInteger();
					break;
				case "RIGHTMARGIN":
					areport.RightMargin = GetAsInteger();
					break;
				case "BOTTOMMARGIN":
					areport.BottomMargin = GetAsInteger();
					break;
				case "PRINTERSELECT":
					areport.PrinterSelect = (PrinterSelectType)GetAsInteger();
					break;
				case "LANGUAGE":
					areport.Language = GetAsInteger();
					break;
				case "COPIES":
					areport.Copies = GetAsInteger();
					break;
				case "COLLATECOPIES":
					areport.CollateCopies = GetAsBool();
					break;
				case "TWOPASS":
					areport.TwoPass = GetAsBool();
					break;
				case "PRINTERFONTS":
					areport.PrinterFonts = (PrinterFontsType)GetAsInteger();
					break;
				case "PRINTONLYIFDATAAVAILABLE":
					areport.PrintOnlyIfDataAvailable = GetAsBool();
					break;
				case "STREAMFORMAT":
					areport.StreamFormat = (StreamFormatType)GetAsInteger();
					break;
				case "REPORTACTIONDRAWERBEFORE":
					areport.ActionBefore = GetAsBool();
					break;
				case "REPORTACTIONDRAWERAFTER":
					areport.ActionAfter = GetAsBool();
					break;
				case "PREVIEWABOUT":
					areport.PreviewAbout = GetAsBool();
					break;
				case "TYPE1FONT":
					areport.Type1Font = (PDFFontType)GetAsInteger();
					break;
				case "FONTSIZE":
					areport.FontSize = (short)GetAsInteger();
					break;
				case "FONTROTATION":
					areport.FontRotation = (short)GetAsInteger();
					break;
				case "FONTSTYLE":
					areport.FontStyle = GetAsInteger();
					break;
				case "FONTCOLOR":
					areport.FontColor = GetAsInteger();
					break;
				case "BACKCOLOR":
					areport.BackColor = GetAsInteger();
					break;
				case "TRANSPARENT":
					areport.Transparent = GetAsBool();
					break;
				case "CUTTEXT":
					areport.CutText = GetAsBool();
					break;
				case "ALIGNMENT":
					TextAlignType newalign = TextAlignType.Left;
					int aalign = GetAsInteger();
					if ((aalign & MetaFile.AlignmentFlags_AlignRight) > 0)
						newalign = TextAlignType.Right;
					else
						if ((aalign & MetaFile.AlignmentFlags_AlignHCenter) > 0)
						newalign = TextAlignType.Center;
					else
							if ((aalign & MetaFile.AlignmentFlags_AlignHJustify) > 0)
						newalign = TextAlignType.Justify;
					areport.Alignment = newalign;
					break;
				case "VALIGNMENT":
					TextAlignVerticalType vnewalign = TextAlignVerticalType.Top;
					int valign = GetAsInteger();
					if ((valign & MetaFile.AlignmentFlags_AlignBottom) > 0)
						vnewalign = TextAlignVerticalType.Bottom;
					else
						if ((valign & MetaFile.AlignmentFlags_AlignVCenter) > 0)
						vnewalign = TextAlignVerticalType.Center;
					areport.VAlignment = vnewalign;
					break;
				case "WORDWRAP":
					areport.WordWrap = GetAsBool();
					break;
				case "SINGLELINE":
					areport.SingleLine = GetAsBool();
					break;
				case "BIDIMODES":
					break;
				case "MULTIPAGE":
					areport.MultiPage = GetAsBool();
					break;
				case "PRINTSTEP":
					areport.PrintStep = (PrintStepType)GetAsInteger();
					break;
				case "PAPERSOURCE":
					areport.PaperSource = GetAsInteger();
					break;
				case "DUPLEX":
					areport.Duplex = GetAsInteger();
					break;
				case "FORCEPAPERNAME":
					areport.ForcePaperName = GetAsString();
					break;
				case "LINESPERINCH":
					areport.LinesPerInch = (short)GetAsInteger();
					break;
				default:
					if (propname[0] != '/')
						throw new NamedException("Property not supported in Report: " + propname, propname);
					break;
			}
		}
		private void ReadPropSubReport(SubReport subrep)
		{
			switch (propname)
			{
				case "ALIAS":
					subrep.Alias = GetAsString();
					break;
				case "PARENTSUBREPORT":
					subrep.ParentSub = GetAsString().ToUpper();
					break;
				case "PARENTSECTION":
					subrep.ParentSec = GetAsString().ToUpper();
					break;
				case "PRINTONLYIFDATAAVAILABLE":
					subrep.PrintOnlyIfDataAvailable = GetAsBool();
					break;
				case "REOPENONPRINT":
					subrep.ReOpenOnPrint = GetAsBool();
					break;
				default:
					if (propname[0] != '/')
						throw new NamedException("Property not supported in SubReport: " + propname, propname);
					break;
			}
		}
		private PrintPosItem CreateComponent()
		{
			PrintPosItem aresult;
			switch (propvalue)
			{
				case "TRPEXPRESSION":
					aresult = new ExpressionItem(areport);
					break;
				case "TRPLABEL":
					aresult = new LabelItem(areport);
					break;
				case "TRPCHART":
					aresult = new ChartItem(areport);
					break;
				case "TRPSHAPE":
					aresult = new ShapeItem(areport);
					break;
				case "TRPIMAGE":
					aresult = new ImageItem(areport);
					break;
				case "TRPBARCODE":
					aresult = new BarcodeItem(areport);
					break;
				default:
					throw new NamedException("Class not supported:" + propvalue, propvalue);
			}
			return (aresult);
		}
		private void BinToStream(Stream astream)
		{
			int alen = System.Convert.ToInt32(propsize);
			if (alen == 0)
				return;
			string bin2 = propvalue;
			byte[] abufdest = new byte[alen];
			int readed;
			readed = StringUtil.HexToBytes(bin2, abufdest);
			if (readed != alen)
				throw new UnNamedException("Expected: " + propsize + " Found: " + readed.ToString());
			astream.Write(abufdest, 0, readed);
			astream.Seek(0, System.IO.SeekOrigin.Begin);
		}
		private void ReadPropSection(Section sec)
		{
			try
			{
				switch (propname)
				{
					case "CHANGEEXPRESSION":
						sec.ChangeExpression = GetAsString();
						break;
					case "BEGINPAGEEXPRESSION":
						sec.BeginPageExpression = GetAsString();
						break;
					case "SKIPEXPREV":
						sec.SkipExpreV = GetAsString();
						break;
					case "SKIPEXPREH":
						sec.SkipExpreH = GetAsString();
						break;
					case "SKIPTOPAGEEXPRE":
						sec.SkipToPageExpre = GetAsString();
						break;
					case "BACKEXPRESSION":
						sec.BackExpression = GetAsString();
						break;
					case "PRINTCONDITION":
						sec.PrintCondition = GetAsString();
						break;
					case "DOAFTERPRINT":
						sec.DoAfterPrint = GetAsString();
						break;
					case "DOBEFOREPRINT":
						sec.DoBeforePrint = GetAsString();
						break;
					case "WIDTH":
						sec.Width = GetAsInteger();
						break;
					case "HEIGHT":
						sec.Height = GetAsInteger();
						break;
					case "SUBREPORT":
						sec.SubReportName = GetAsString().ToUpper();
						break;
					case "GROUPNAME":
						sec.GroupName = GetAsString();
						break;
					case "CHANGEBOOL":
						sec.ChangeBool = GetAsBool();
						break;
					case "PAGEREPEAT":
						sec.PageRepeat = GetAsBool();
						break;
					case "FORCEPRINT":
						sec.ForcePrint = GetAsBool();
						break;
					case "SKIPPAGE":
						sec.SkipPage = GetAsBool();
						break;
					case "ALIGNBOTTOM":
						sec.AlignBottom = GetAsBool();
						break;
					case "SECTIONTYPE":
						sec.SectionType = (SectionType)GetAsInteger();
						break;
					case "AUTOEXPAND":
						sec.AutoExpand = GetAsBool();
						break;
					case "AUTOCONTRACT":
						sec.AutoContract = GetAsBool();
						break;
					case "HORZDESP":
						sec.HorzDesp = GetAsBool();
						break;
					case "VERTDESP":
						sec.VertDesp = GetAsBool();
						break;
					case "EXTERNALFILENAME":
						sec.ExternalFilename = GetAsString();
						break;
					case "EXTERNALCONNECTION":
						sec.ExternalConnection = GetAsString();
						break;
					case "EXTERNALTABLE":
						sec.ExternalTable = GetAsString();
						break;
					case "EXTERNALFIELD":
						sec.ExternalField = GetAsString();
						break;
					case "EXTERNALSEARCHFIELD":
						sec.ExternalSearchField = GetAsString();
						break;
					case "EXTERNALSEARCHVALUE":
						sec.ExternalSearchValue = GetAsString();
						break;
					case "STREAMFORMAT":
						sec.StreamFormat = (StreamFormatType)GetAsInteger();
						break;
					case "CHILDSUBREPORT":
						sec.ChildSubReportName = GetAsString().ToUpper();
						break;
					case "SKIPRELATIVEH":
						sec.SkipRelativeH = GetAsBool();
						break;
					case "SKIPRELATIVEV":
						sec.SkipRelativeV = GetAsBool();
						break;
					case "SKIPTYPE":
						sec.SkipType = (SkipType)GetAsInteger();
						break;
					case "ININUMPAGE":
						sec.IniNumPage = GetAsBool();
						break;
					case "GLOBAL":
						sec.Global = GetAsBool();
						break;
					case "BACKSTYLE":
						sec.BackStyle = (BackStyleType)GetAsInteger();
						break;
					case "DPIRES":
						sec.dpires = GetAsInteger();
						break;
					case "DRAWSTYLE":
						sec.DrawStyle = (ImageDrawStyleType)GetAsInteger();
						break;
					case "CACHEDIMAGE":
						if (propvalue != "False")
							sec.SharedImage = (SharedImageType)GetAsInteger();
						break;
					case "STREAM":
						BinToStream(sec.Stream);
						break;
					default:
						if (propname[0] != '/')
							throw new NamedException("Property not suported in Section:" + propname, propname);
						break;
				}
			}
			catch (Exception)
			{

			}
		}
		private void ReadPropDBInfo(DatabaseInfo dbitem)
		{
			switch (propname)
			{
				case "CONFIGFILE":
					//					dbitem.ConfigFile = GetAsString();
					break;
				case "LOADPARAMS":
					//					dbitem.LoadParams = GetAsBool();
					break;
				case "LOADDRIVERPARAMS":
					//					dbitem.LoadDriverParams = GetAsBool();
					break;
				case "LOGINPROMPT":
					//					dbitem.LoginPrompt = GetAsBool();
					break;
				case "PROVIDERFACTORY":
					dbitem.ProviderFactory = GetAsString();
					break;
				case "DRIVER":
					dbitem.Driver = (DriverType)GetAsInteger();
					break;
				case "DOTNETDRIVER":
					dbitem.DotNetDriver = (DotNetDriverType)GetAsInteger();
					break;
				case "REPORTTABLE":
					dbitem.ReportTable = GetAsString();
					break;
				case "REPORTSEARCHFIELD":
					dbitem.ReportSearchField = GetAsString();
					break;
				case "REPORTFIELD":
					dbitem.ReportField = GetAsString();
					break;
				case "REPORTGROUPSTABLE":
					dbitem.ReportGroupsTable = GetAsString();
					break;
				case "ADOCONNECTIONSTRING":
					dbitem.ConnectionString = GetAsString();
					break;
				default:
					if (propname[0] != '/')
						throw new NamedException("Property not suported in Databaseinfo:" + propname, propname);
					break;
			}
		}
		private void ReadPropDataInfo(DataInfo ditem)
		{
			switch (propname)
			{
				case "DATABASEALIAS":
					ditem.DatabaseAlias = GetAsString();
					break;
				case "SQL":
					ditem.SQL = GetAsString();
					break;
				case "DATASOURCE":
					ditem.DataSource = GetAsString();
					break;
				case "MYBASEFILENAME":
					ditem.MyBaseFilename = GetAsString();
					break;
				case "MYBASEFIELDS":
					ditem.MyBaseFields = GetAsString();
					break;
				case "MYBASEINDEXFIELDS":
					ditem.MyBaseIndexFields = GetAsString();
					break;
				case "MYBASEMASTERFIELDS":
					ditem.MyBaseMasterFields = GetAsString();
					break;
				case "BDEINDEXFIELDS":
					ditem.BDEIndexFields = GetAsString();
					break;
				case "BDEINDEXNAME":
					ditem.BDEIndexName = GetAsString();
					break;
				case "BDETABLE":
					ditem.BDETable = GetAsString();
					break;
				case "BDETYPE":
					ditem.BDEType = (DatasetType)GetAsInteger();
					break;
				case "BDEFILTER":
					ditem.BDEFilter = GetAsString();
					break;
				case "BDEMASTERFIELDS":
					ditem.BDEMasterFields = GetAsString();
					break;
				case "BDEFIRSTRANGE":
					ditem.BDEFirstRange = GetAsString();
					break;
				case "BDELASTRANGE":
					ditem.BDELastRange = GetAsString();
					break;
				case "DATAUNIONS":
					FillStrings(ditem.DataUnions);
					break;
				case "GROUPUNION":
					ditem.GroupUnion = GetAsBool();
					break;
				case "PARALLELUNION":
					ditem.ParallelUnion = GetAsBool();
					break;
				case "OPENONSTART":
					ditem.OpenOnStart = GetAsBool();
					break;
				default:
					if (propname[0] != '/')
						throw new NamedException("Property not suported in Datainfo:" + propname, propname);
					break;
			}
		}
		private void ReadPropParam(Param aparam)
		{
			switch (propname)
			{
				case "DESCRIPTION":
					aparam.Descriptions = GetAsString();
					break;
				case "HINT":
					aparam.Hints = GetAsString();
					break;
				case "ERRORMESSAGE":
					aparam.ErrorMessages = GetAsString();
					break;
				case "VALIDATION":
					aparam.Validation = GetAsString();
					break;
				case "SEARCH":
					aparam.Search = GetAsString();
					break;
				case "VISIBLE":
					aparam.Visible = GetAsBool();
					break;
				case "ISREADONLY":
					aparam.IsReadOnly = GetAsBool();
					break;
				case "NEVERVISIBLE":
					aparam.NeverVisible = GetAsBool();
					break;
				case "ALLOWNULLS":
					aparam.AllowNulls = GetAsBool();
					break;
				case "PARAMTYPE":
					aparam.ParamType = (ParamType)GetAsInteger();
					break;
				case "DATASETS":
					FillStrings(aparam.Datasets);
					break;
				case "ITEMS":
					FillStrings(aparam.Items);
					break;
				case "VALUES":
					FillStrings(aparam.Values);
					break;
				case "SELECTED":
					FillStrings(aparam.Selected);
					break;
				case "LOOKUPDATASET":
					aparam.LookupDataset = GetAsString();
					break;
				case "SEARCHPARAM":
					aparam.SearchParam = GetAsString();
					break;
				case "SEARCHDATASET":
					aparam.SearchDataset = GetAsString();
					break;
				case "VALUE":
					switch (aparam.ParamType)
					{
						case ParamType.String:
						case ParamType.ExpreA:
						case ParamType.ExpreB:
						case ParamType.Subst:
						case ParamType.List:
						case ParamType.SubsExpreList:
						case ParamType.InitialValue:
						case ParamType.Unknown:
							aparam.Value = GetAsString();
							break;
						case ParamType.Integer:
							aparam.Value = GetAsInteger();
							break;
						case ParamType.Double:
							aparam.Value = GetAsDouble();
							break;
						case ParamType.Currency:
							aparam.Value = GetAsDouble();
							break;
						case ParamType.Date:
						case ParamType.Time:
						case ParamType.DateTime:
							aparam.Value = GetAsDateTime();
							break;
						case ParamType.Bool:
							aparam.Value = GetAsBool();
							break;
					}
					break;
				default:
					if (propname[0] != '/')
						throw new NamedException("Property not suported in Param:" + propname, propname);
					break;
			}
		}
		private void ReadCompProp(PrintPosItem comp)
		{
			bool assigned = false;
			switch (propname)
			{
				case "PRINTCONDITION":
					comp.PrintCondition = GetAsString();
					assigned = true;
					break;
				case "DOAFTERPRINT":
					comp.DoAfterPrint = GetAsString();
					assigned = true;
					break;
				case "DOBEFOREPRINT":
					comp.DoBeforePrint = GetAsString();
					assigned = true;
					break;
				case "WIDTH":
					comp.Width = GetAsInteger();
					assigned = true;
					break;
				case "HEIGHT":
					comp.Height = GetAsInteger();
					assigned = true;
					break;
				case "POSX":
					comp.PosX = GetAsInteger();
					assigned = true;
					break;
				case "POSY":
					comp.PosY = GetAsInteger();
					assigned = true;
					break;
				case "ALIGN":
					comp.Align = (PrintItemAlign)GetAsInteger();
					assigned = true;
					break;
				case "NAME":
					comp.Name = GetAsString();
					assigned = true;
					break;
			}
			if (assigned)
				return;
			if (comp is PrintItemText)
			{
				PrintItemText compt = (PrintItemText)comp;
				switch (propname)
				{
					case "WFONTNAME":
						compt.WFontName = GetAsString();
						assigned = true;
						break;
					case "LFONTNAME":
						compt.LFontName = GetAsString();
						assigned = true;
						break;
					case "TYPE1FONT":
						compt.Type1Font = (PDFFontType)GetAsInteger();
						assigned = true;
						break;
					case "FONTSIZE":
						compt.FontSize = (short)GetAsInteger();
						assigned = true;
						break;
					case "FONTROTATION":
						compt.FontRotation = (short)GetAsInteger();
						assigned = true;
						break;
					case "FONTSTYLE":
						compt.FontStyle = GetAsInteger();
						assigned = true;
						break;
					case "FONTCOLOR":
						compt.FontColor = GetAsInteger();
						assigned = true;
						break;
					case "BACKCOLOR":
						compt.BackColor = GetAsInteger();
						assigned = true;
						break;
					case "TRANSPARENT":
						compt.Transparent = GetAsBool();
						assigned = true;
						break;
					case "CUTTEXT":
						compt.CutText = GetAsBool();
						assigned = true;
						break;
					case "ALIGNMENT":
						TextAlignType newalign = TextAlignType.Left;
						int aalign = GetAsInteger();
						if ((aalign & MetaFile.AlignmentFlags_AlignRight) > 0)
							newalign = TextAlignType.Right;
						else
							if ((aalign & MetaFile.AlignmentFlags_AlignHCenter) > 0)
							newalign = TextAlignType.Center;
						else
								if ((aalign & MetaFile.AlignmentFlags_AlignHJustify) > 0)
							newalign = TextAlignType.Justify;
						compt.Alignment = newalign;
						assigned = true;
						break;
					case "VALIGNMENT":
						TextAlignVerticalType vnewalign = TextAlignVerticalType.Top;
						int valign = GetAsInteger();
						if ((valign & MetaFile.AlignmentFlags_AlignBottom) > 0)
							vnewalign = TextAlignVerticalType.Bottom;
						else
							if ((valign & MetaFile.AlignmentFlags_AlignVCenter) > 0)
							vnewalign = TextAlignVerticalType.Center;
						compt.VAlignment = vnewalign;
						assigned = true;
						break;
					case "WORDWRAP":
						compt.WordWrap = GetAsBool();
						assigned = true;
						break;
					case "SINGLELINE":
						compt.SingleLine = GetAsBool();
						assigned = true;
						break;
					case "WORDBREAK":
						compt.SingleLine = GetAsBool();
						assigned = true;
						break;
					case "INTERLINE":
						compt.InterLine = GetAsInteger();
						assigned = true;
						break;
					case "BIDIMODES":
						assigned = true;
						break;
					case "BIDIMODE":
						int nbidi = GetAsInteger();
						if (nbidi != 0)
							compt.RightToLeft = true;
						assigned = true;
						break;
					case "MULTIPAGE":
						compt.MultiPage = GetAsBool();
						assigned = true;
						break;
					case "PRINTSTEP":
						compt.PrintStep = (PrintStepType)GetAsInteger();
						assigned = true;
						break;
				}
			}
			if (assigned)
				return;
			if (comp is LabelItem)
			{
				LabelItem compl = (LabelItem)comp;
				switch (propname)
				{
					case "WIDETEXT":
						FillStringsNoTrim(compl.AllStrings);
						assigned = true;
						break;
				}
			}
			if (comp is ExpressionItem)
			{
				ExpressionItem compe = (ExpressionItem)comp;
				switch (propname)
				{
					case "EXPRESSION":
						compe.Expression = GetAsString();
						assigned = true;
						break;
					case "AGINIVALUE":
						compe.AgIniValue = GetAsString();
						assigned = true;
						break;
					case "EXPORTEXPRESSION":
						compe.ExportExpression = GetAsString();
						assigned = true;
						break;
					case "DATATYPE":
						compe.DataType = (ParamType)GetAsInteger();
						assigned = true;
						break;
					case "DISPLAYFORMAT":
						compe.DisplayFormat = GetAsString();
						assigned = true;
						break;
					case "IDENTIFIER":
						compe.Identifier = GetAsString();
						assigned = true;
						break;
					case "GROUPNAME":
						compe.GroupName = GetAsString();
						assigned = true;
						break;
					case "AGGREGATE":
						compe.Aggregate = (Aggregate)GetAsInteger();
						assigned = true;
						break;
					case "AGTYPE":
						compe.AgType = (AggregateType)GetAsInteger();
						assigned = true;
						break;
					case "AUTOEXPAND":
						compe.AutoExpand = GetAsBool();
						assigned = true;
						break;
					case "AUTOCONTRACT":
						compe.AutoContract = GetAsBool();
						assigned = true;
						break;
					case "PRINTONLYONE":
						compe.PrintOnlyOne = GetAsBool();
						assigned = true;
						break;
					case "PRINTNULLS":
						compe.PrintNulls = GetAsBool();
						assigned = true;
						break;
					case "EXPORTDISPLAYFORMAT":
						compe.ExportDisplayFormat = GetAsString();
						assigned = true;
						break;
					case "EXPORTLINE":
						compe.ExportLine = GetAsInteger();
						assigned = true;
						break;
					case "EXPORTPOSITION":
						compe.ExportPosition = GetAsInteger();
						assigned = true;
						break;
					case "EXPORTSIZE":
						compe.ExportSize = GetAsInteger();
						assigned = true;
						break;
					case "EXPORTDONEWLINE":
						compe.ExportDoNewLine = GetAsBool();
						assigned = true;
						break;
				}
			}
			if (assigned)
				return;
			if (comp is ShapeItem)
			{
				ShapeItem comps = (ShapeItem)comp;
				switch (propname)
				{
					case "SHAPE":
						comps.Shape = (ShapeType)GetAsInteger();
						assigned = true;
						break;
					case "BRUSHSTYLE":
						comps.BrushStyle = (BrushType)GetAsInteger();
						assigned = true;
						break;
					case "BRUSHCOLOR":
						comps.BrushColor = GetAsInteger();
						assigned = true;
						break;
					case "PENSTYLE":
						comps.PenStyle = (PenType)GetAsInteger();
						assigned = true;
						break;
					case "PENCOLOR":
						comps.PenColor = GetAsInteger();
						assigned = true;
						break;
					case "PENWIDTH":
						comps.PenWidth = GetAsInteger();
						assigned = true;
						break;
				}
			}
			if (assigned)
				return;
			if (comp is ImageItem)
			{
				ImageItem compi = (ImageItem)comp;
				switch (propname)
				{
					case "EXPRESSION":
						compi.Expression = GetAsString();
						assigned = true;
						break;
					case "DPIRES":
						compi.dpires = GetAsInteger();
						assigned = true;
						break;
					case "DRAWSTYLE":
						compi.DrawStyle = (ImageDrawStyleType)GetAsInteger();
						assigned = true;
						break;
					case "CACHEDIMAGE":
						if (propvalue != "False")
							compi.SharedImage = (SharedImageType)GetAsInteger();
						assigned = true;
						break;
					case "STREAM":
						BinToStream(compi.Stream);
						assigned = true;
						break;
					case "ROTATION":
						compi.Rotation = (short)GetAsInteger();
						assigned = true;
						break;
					case "COPYMODE":
						compi.CopyMode = GetAsInteger();
						assigned = true;
						break;
				}
			}
			if (assigned)
				return;
			if (comp is BarcodeItem)
			{
				BarcodeItem compb = (BarcodeItem)comp;
				switch (propname)
				{
					case "EXPRESSION":
						compb.Expression = GetAsString();
						assigned = true;
						break;
					case "MODUL":
						compb.Modul = GetAsInteger();
						assigned = true;
						break;
					case "RATIO":
						compb.Ratio = GetAsDouble();
						assigned = true;
						break;
					case "TYP":
						compb.BarType = (BarcodeType)GetAsInteger();
						assigned = true;
						break;
					case "CHECKSUM":
						compb.Checksum = GetAsBool();
						assigned = true;
						break;
					case "TRANSPARENT":
						compb.Transparent = GetAsBool();
						assigned = true;
						break;
					case "DISPLAYFORMAT":
						compb.DisplayFormat = GetAsString();
						assigned = true;
						break;
					case "ROTATION":
						compb.Rotation = (short)GetAsInteger();
						assigned = true;
						break;
					case "BCOLOR":
						compb.BColor = GetAsInteger();
						assigned = true;
						break;
					case "BACKCOLOR":
						compb.BackColor = GetAsInteger();
						assigned = true;
						break;
					case "NUMCOLUMNS":
						compb.NumColumns = GetAsInteger();
						assigned = true;
						break;
					case "NUMROWS":
						compb.NumRows = GetAsInteger();
						assigned = true;
						break;
					case "ECCLEVEL":
						compb.ECCLevel = GetAsInteger();
						assigned = true;
						break;
					case "TRUNCATED":
						compb.Truncated = GetAsBool();
						assigned = true;
						break;
				}
			}
			if (assigned)
				return;
			if (comp is ChartItem)
			{
				ChartItem compc = (ChartItem)comp;
				switch (propname)
				{
					case "VALUEEXPRESSION":
						compc.ValueExpression = GetAsString();
						assigned = true;
						break;
					case "VALUEXEXPRESSION":
						compc.ValueXExpression = GetAsString();
						assigned = true;
						break;
					case "GETVALUECONDITION":
						compc.GetValueCondition = GetAsString();
						assigned = true;
						break;
					case "CHANGESERIEEXPRESSION":
						compc.ChangeSerieExpression = GetAsString();
						assigned = true;
						break;
					case "CAPTIONEXPRESSION":
						compc.CaptionExpression = GetAsString();
						assigned = true;
						break;
					case "COLOREXPRESSION":
						compc.ColorExpression = GetAsString();
						assigned = true;
						break;
					case "SERIECOLOREXPRESSION":
						compc.SerieColorExpression = GetAsString();
						assigned = true;
						break;
					case "SERIECAPTION":
						compc.SerieCaption = GetAsString();
						assigned = true;
						break;
					case "CLEAREXPRESSION":
						compc.ClearExpression = GetAsString();
						assigned = true;
						break;
					case "IDENTIFIER":
						compc.Identifier = GetAsString();
						assigned = true;
						break;
					case "CHANGESERIEBOOL":
						compc.ChangeSerieBool = GetAsBool();
						assigned = true;
						break;
					case "CLEAREXPRESSIONBOOL":
						compc.ClearExpressionBool = GetAsBool();
						assigned = true;
						break;
					case "CHARTTYPE":
						compc.ChartStyle = (ChartType)GetAsInteger();
						assigned = true;
						break;
					case "DRIVER":
						compc.Driver = (ChartDriver)GetAsInteger();
						assigned = true;
						break;
					case "VIEW3D":
						compc.View3d = GetAsBool();
						assigned = true;
						break;
					case "VIEW3DWALLS":
						compc.View3dWalls = GetAsBool();
						assigned = true;
						break;
					case "PERSPECTIVE":
						compc.Perspective = GetAsInteger();
						assigned = true;
						break;
					case "ELEVATION":
						compc.Elevation = GetAsInteger();
						assigned = true;
						break;
					case "ROTATION":
						compc.Rotation = GetAsInteger();
						assigned = true;
						break;
					case "ZOOM":
						compc.Zoom = GetAsInteger();
						assigned = true;
						break;
					case "HORZOFFSET":
						compc.HorzOffset = GetAsInteger();
						assigned = true;
						break;
					case "VERTOFFSET":
						compc.VertOffset = GetAsInteger();
						assigned = true;
						break;
					case "TILT":
						compc.Tilt = GetAsInteger();
						assigned = true;
						break;
					case "ORTHOGONAL":
						compc.Orthogonal = GetAsBool();
						assigned = true;
						break;
					case "MULTIBAR":
						compc.MultiBar = (BarType)GetAsInteger();
						assigned = true;
						break;
					case "RESOLUTION":
						compc.Resolution = GetAsInteger();
						assigned = true;
						break;
					case "SHOWLEGEND":
						compc.ShowLegend = GetAsBool();
						assigned = true;
						break;
					case "SHOWHINT":
						compc.ShowHint = GetAsBool();
						assigned = true;
						break;
					case "MARKSTYLE":
						compc.MarkStyle = GetAsInteger();
						assigned = true;
						break;
					case "HORZFONTSIZE":
						compc.HorzFontSize = GetAsInteger();
						assigned = true;
						break;
					case "VERTFONTSIZE":
						compc.VertFontSize = GetAsInteger();
						assigned = true;
						break;
					case "HORZFONTROTATION":
						compc.HorzFontRotation = GetAsInteger();
						assigned = true;
						break;
					case "VERTFONTROTATION":
						compc.VertFontRotation = GetAsInteger();
						assigned = true;
						break;
					case "AUTORANGE":
						compc.AutoRange = (Series.AutoRangeAxis)GetAsInteger();
						assigned = true;
						break;
					case "YMIN":
						compc.AxisYInitial = GetAsDouble();
						assigned = true;
						break;
					case "YMAX":
						compc.AxisYFinal = GetAsDouble();
						assigned = true;
						break;
				}
			}
			if (assigned)
				return;
			if (propname[0] != '/')
				throw new UnNamedException("Property not suported in " + compclass + ":" + propname);
		}
	}
	/// <summary>
	/// Class used to write a report template to a file (or stream) in xml format
	/// </summary>
	public class ReportWriter
	{
		const int C_MAXDATAWIDTH = 40;
		BaseReport areport;
		static NumberFormatInfo numberformat;
		static string ALPHACHARS;
		static object flag = 2;
		private static void AddRangeToSBuilder(StringBuilder sbuilder, char firstchar, char lastchar)
		{
			char current = firstchar;
			while (current <= lastchar)
			{
				sbuilder.Append(current);
				current = (char)(((int)current) + 1);
			}
		}
		static ReportWriter()
		{
			Monitor.Enter(flag);
			try
			{
				if (numberformat == null)
				{
					numberformat = new NumberFormatInfo();
					numberformat.NumberDecimalSeparator = ".";
					numberformat.NumberGroupSeparator = "";
					StringBuilder sbuilder = new StringBuilder();
					AddRangeToSBuilder(sbuilder, '0', '9');
					AddRangeToSBuilder(sbuilder, 'A', 'Z');
					AddRangeToSBuilder(sbuilder, 'a', 'z');
					sbuilder.Append("_ .()=;:");
					ALPHACHARS = sbuilder.ToString();
				}
			}
			finally
			{
				Monitor.Exit(flag);
			}
		}
		/// <summary>
		/// Create the report reader instance, you must supply the report where the
		/// template will be loaded
		/// </summary>
		/// <param name="rp"></param>
		public ReportWriter(BaseReport rp)
		{
			areport = rp;
		}


		private static bool RpIsAlpha(char c)
		{
#if REPMAN_DOTNET1
			foreach (char a in ALPHACHARS)
			{
				if (a==c)
					return true;
			}
			return false;
#else
			return (ALPHACHARS.Contains(c.ToString()));
#endif
		}
		private static string RpBoolToStr(bool avalue)
		{
			if (avalue)
				return "True";
			else
				return "False";
		}
		private static string StringToRpString(String astring)
		{
			if (astring == null)
				astring = "";
			int alen = 0;
			string asubs;
			StringBuilder aresult = new StringBuilder();
			foreach (char c in astring)
			{
				if (RpIsAlpha(c))
				{
					aresult.Append(c);
					alen++;
					if (alen > C_MAXDATAWIDTH)
					{
						alen = 0;
						aresult.Append((char)13);
						aresult.Append((char)10);
					}
				}
				else
				{
					asubs = '#' + ((int)c).ToString() + '#';
					aresult.Append(asubs);
					alen = alen + asubs.Length;
					if (alen > C_MAXDATAWIDTH)
					{
						alen = 0;
						aresult.Append((char)13);
						aresult.Append((char)10);
					}
				}
			}
			return aresult.ToString();
		}

		private static void WritePropertyS(string propname, string propvalue, Stream astream)
		{
			string astring = "<" + propname + " type=\"String\">" + StringToRpString(propvalue) + "</" + propname + ">";
			StreamUtil.SWriteLine(astream, astring);
		}
		private static void WritePropertyI(string propname, int propvalue, Stream astream)
		{
			string astring = "<" + propname + " type=\"Integer\">" + propvalue.ToString() + "</" + propname + ">";
			StreamUtil.SWriteLine(astream, astring);
		}
		private static void WritePropertyBool(string propname, bool propvalue, Stream astream)
		{
			string astring = "<" + propname + " type=\"Boolean\">" + RpBoolToStr(propvalue) + "</" + propname + ">";
			StreamUtil.SWriteLine(astream, astring);
		}
		private static void WritePropertyB(string propname, Stream propvalue, Stream astream)
		{
			string astring = "<" + propname + " type=\"Binary\" size=\"" + propvalue.Length.ToString() + "\">" +
				StringToRpString(StreamUtil.StreamToHex(propvalue)) + "</" + propname + ">";
			StreamUtil.SWriteLine(astream, astring);
		}
		private static string RpDoubleToStr(double avalue)
		{
			return avalue.ToString(numberformat);
		}
		private static void WritePropertyD(string propname, double propvalue, Stream astream)
		{
			string astring = "<" + propname + " type=\"Double\">" +
				RpDoubleToStr(propvalue) + "</" + propname + ">";
			StreamUtil.SWriteLine(astream, astring);
		}
		private static void WritePropertyDateTime(string propname, DateTime propvalue, Stream astream)
		{
			string astring = "<" + propname + " type=\"DateTime\">" +
				RpDoubleToStr(DateUtil.DateTimeToDelphiDate(propvalue)) + "</" + propname + ">";
			StreamUtil.SWriteLine(astream, astring);
		}
		private void WriteReportPropsXML(BaseReport areport, Stream astream)
		{
			WritePropertyS("WFONTNAME", areport.WFontName, astream);
			WritePropertyS("LFONTNAME", areport.LFontName, astream);
			WritePropertyBool("GRIDVISIBLE", areport.GridVisible, astream);
			WritePropertyBool("GRIDLINES", areport.GridLines, astream);
			WritePropertyBool("GRIDENABLED", areport.GridEnabled, astream);
			WritePropertyI("GRIDCOLOR", areport.GridColor, astream);
			WritePropertyI("GRIDWIDTH", areport.GridWidth, astream);
			WritePropertyI("GRIDHEIGHT", areport.GridHeight, astream);
			WritePropertyI("PAGEORIENTATION", (int)areport.PageOrientation, astream);
			WritePropertyI("PAGESIZE", (int)areport.PageSize, astream);
			WritePropertyI("PAGESIZEQT", areport.PageSizeIndex, astream);
			WritePropertyI("PAGEHEIGHT", areport.PageHeight, astream);
			WritePropertyI("PAGEWIDTH", areport.PageWidth, astream);
			WritePropertyI("CUSTOMPAGEHEIGHT", areport.CustomPageHeight, astream);
			WritePropertyI("CUSTOMPAGEWIDTH", areport.CustomPageWidth, astream);
			WritePropertyI("PAGEBACKCOLOR", (int)areport.PageBackColor, astream);
			WritePropertyI("PREVIEWSTYLE", (int)areport.AutoScale, astream);
			WritePropertyBool("PREVIEWMARGINS", areport.PreviewMargins, astream);
			WritePropertyI("PREVIEWWINDOW", (int)areport.PreviewWindow, astream);
			WritePropertyI("LEFTMARGIN", areport.LeftMargin, astream);
			WritePropertyI("TOPMARGIN", areport.TopMargin, astream);
			WritePropertyI("RIGHTMARGIN", areport.RightMargin, astream);
			WritePropertyI("BOTTOMMARGIN", areport.BottomMargin, astream);
			WritePropertyI("PRINTERSELECT", (int)areport.PrinterSelect, astream);
			WritePropertyI("LANGUAGE", areport.Language, astream);
			WritePropertyI("COPIES", areport.Copies, astream);
			WritePropertyBool("COLLATECOPIES", areport.CollateCopies, astream);
			WritePropertyBool("TWOPASS", areport.TwoPass, astream);
			WritePropertyI("PRINTERFONTS", (int)areport.PrinterFonts, astream);
			WritePropertyBool("PRINTONLYIFDATAAVAILABLE", areport.PrintOnlyIfDataAvailable, astream);
			WritePropertyI("STREAMFORMAT", (int)areport.StreamFormat, astream);
			WritePropertyBool("REPORTACTIONDRAWERBEFORE", areport.ActionBefore, astream);
			WritePropertyBool("REPORTACTIONDRAWERAFTER", areport.ActionAfter, astream);
			WritePropertyBool("PREVIEWABOUT", areport.PreviewAbout, astream);
			WritePropertyI("TYPE1FONT", (int)areport.Type1Font, astream);
			WritePropertyI("FONTSIZE", areport.FontSize, astream);
			WritePropertyI("FONTROTATION", areport.FontRotation, astream);
			WritePropertyI("FONTSTYLE", areport.FontStyle, astream);
			WritePropertyI("FONTCOLOR", areport.FontColor, astream);
			WritePropertyI("BACKCOLOR", areport.BackColor, astream);
			WritePropertyBool("TRANSPARENT", areport.Transparent, astream);
			WritePropertyBool("CUTTEXT", areport.CutText, astream);
			int aalign = 0;
			switch (areport.Alignment)
			{
				case TextAlignType.Right:
					aalign = MetaFile.AlignmentFlags_AlignRight;
					break;
				case TextAlignType.Center:
					aalign = MetaFile.AlignmentFlags_AlignHCenter;
					break;
				case TextAlignType.Justify:
					aalign = MetaFile.AlignmentFlags_AlignHJustify;
					break;
			}
			WritePropertyI("ALIGNMENT", aalign, astream);
			aalign = 0;
			switch (areport.VAlignment)
			{
				case TextAlignVerticalType.Bottom:
					aalign = MetaFile.AlignmentFlags_AlignBottom;
					break;
				case TextAlignVerticalType.Center:
					aalign = MetaFile.AlignmentFlags_AlignVCenter;
					break;
			}
			WritePropertyI("VALIGNMENT", aalign, astream);
			WritePropertyBool("WORDWRAP", areport.WordWrap, astream);
			WritePropertyBool("SINGLELINE", areport.SingleLine, astream);
			//            WritePropertyS("BIDIMODES",areport.BidiModes.Text,astream);
			WritePropertyBool("MULTIPAGE", areport.MultiPage, astream);
			WritePropertyI("PRINTSTEP", (int)areport.PrintStep, astream);
			WritePropertyI("PAPERSOURCE", areport.PaperSource, astream);
			WritePropertyI("DUPLEX", areport.Duplex, astream);
			WritePropertyS("FORCEPAPERNAME", areport.ForcePaperName, astream);
			WritePropertyI("LINESPERINCH", areport.LinesPerInch, astream);
		}
		private static void WriteComponentXML(PrintPosItem comp, Stream astream)
		{
			StreamUtil.SWriteLine(astream, "<COMPONENT>");

			WritePropertyS("NAME", comp.Name, astream);
			WritePropertyS("CLASSNAME", comp.ClassName, astream);
			WritePropertyI("WIDTH", comp.Width, astream);
			WritePropertyI("HEIGHT", comp.Height, astream);
			WritePropertyS("PRINTCONDITION", comp.PrintCondition, astream);
			WritePropertyS("DOBEFOREPRINT", comp.DoBeforePrint, astream);
			WritePropertyS("DOAFTERPRINT", comp.DoAfterPrint, astream);
			// CommonPos
			WritePropertyI("POSX", comp.PosX, astream);
			WritePropertyI("POSY", comp.PosY, astream);
			WritePropertyI("ALIGN", (int)comp.Align, astream);
			// Common text component
			if (comp is PrintItemText)
			{
				PrintItemText compt = (PrintItemText)comp;
				WritePropertyS("WFONTNAME", compt.WFontName, astream);
				WritePropertyS("LFONTNAME", compt.LFontName, astream);
				int bidi = 0;
				if (compt.RightToLeft)
					bidi = 2;
				WritePropertyI("BIDIMODE", bidi, astream);
				WritePropertyI("TYPE1FONT", (int)compt.Type1Font, astream);
				WritePropertyI("FONTSIZE", compt.FontSize, astream);
				WritePropertyI("FONTROTATION", compt.FontRotation, astream);
				WritePropertyI("FONTSTYLE", compt.FontStyle, astream);
				WritePropertyI("FONTCOLOR", compt.FontColor, astream);
				WritePropertyI("BACKCOLOR", compt.BackColor, astream);
				WritePropertyBool("TRANSPARENT", compt.Transparent, astream);

				WritePropertyBool("CUTTEXT", compt.CutText, astream);
				int aalign = 0;
				switch (compt.Alignment)
				{
					case TextAlignType.Right:
						aalign = MetaFile.AlignmentFlags_AlignRight;
						break;
					case TextAlignType.Center:
						aalign = MetaFile.AlignmentFlags_AlignHCenter;
						break;
					case TextAlignType.Justify:
						aalign = MetaFile.AlignmentFlags_AlignHJustify;
						break;
				}
				WritePropertyI("ALIGNMENT", aalign, astream);
				aalign = 0;
				switch (compt.VAlignment)
				{
					case TextAlignVerticalType.Bottom:
						aalign = MetaFile.AlignmentFlags_AlignBottom;
						break;
					case TextAlignVerticalType.Center:
						aalign = MetaFile.AlignmentFlags_AlignVCenter;
						break;
				}
				WritePropertyI("VALIGNMENT", aalign, astream);
				WritePropertyI("INTERLINE", compt.InterLine, astream);
				WritePropertyBool("WORDWRAP", compt.WordWrap, astream);
				WritePropertyBool("WORDBREAK", compt.WordBreak, astream);
				WritePropertyBool("SINGLELINE", compt.SingleLine, astream);
				int intbidi = 0;
				if (compt.RightToLeft)
					intbidi = 2;
				WritePropertyI("BIDIMODE", intbidi, astream);
				//                WritePropertyBool("RIGHTTOLEFT",compt.RightToLeft,astream);
				WritePropertyBool("MULTIPAGE", compt.MultiPage, astream);
				WritePropertyI("PRINTSTEP", (int)compt.PrintStep, astream);
			}
			// TRpLabel
			if (comp is LabelItem)
			{
				LabelItem compl = (LabelItem)comp;
				WritePropertyS("WIDETEXT", compl.AllStrings.Text, astream);
			}
			else
			// TRpExpression
			if (comp is ExpressionItem)
			{
				ExpressionItem compe = (ExpressionItem)comp;
				WritePropertyS("EXPRESSION", compe.Expression, astream);
				WritePropertyS("AGINIVALUE", compe.AgIniValue, astream);
				WritePropertyS("EXPORTEXPRESSION", compe.ExportExpression, astream);
				WritePropertyI("DATATYPE", (int)compe.DataType, astream);
				WritePropertyS("DISPLAYFORMAT", compe.DisplayFormat, astream);
				WritePropertyS("IDENTIFIER", compe.Identifier, astream);
				WritePropertyI("AGGREGATE", (int)compe.Aggregate, astream);
				WritePropertyS("GROUPNAME", compe.GroupName, astream);
				WritePropertyI("AGTYPE", (int)compe.AgType, astream);
				WritePropertyBool("AUTOEXPAND", compe.AutoExpand, astream);
				WritePropertyBool("AUTOCONTRACT", compe.AutoContract, astream);
				WritePropertyBool("PRINTONLYONE", compe.PrintOnlyOne, astream);
				WritePropertyBool("PRINTNULLS", compe.PrintNulls, astream);
				WritePropertyS("EXPORTDISPLAYFORMAT", compe.ExportDisplayFormat, astream);
				WritePropertyI("EXPORTLINE", compe.ExportLine, astream);
				WritePropertyI("EXPORTPOSITION", compe.ExportPosition, astream);
				WritePropertyI("EXPORTSIZE", compe.ExportSize, astream);
				WritePropertyBool("EXPORTDONEWLINE", compe.ExportDoNewLine, astream);
			}
			else
			// TRpShape            
			if (comp is ShapeItem)
			{
				ShapeItem comps = (ShapeItem)comp;
				WritePropertyI("SHAPE", (int)comps.Shape, astream);
				WritePropertyI("BRUSHSTYLE", (int)comps.BrushStyle, astream);
				WritePropertyI("BRUSHCOLOR", comps.BrushColor, astream);
				WritePropertyI("PENSTYLE", (int)comps.PenStyle, astream);
				WritePropertyI("PENCOLOR", comps.PenColor, astream);
				WritePropertyI("PENWIDTH", comps.PenWidth, astream);
			}
			else
			// TRpImage
			if (comp is ImageItem)
			{
				ImageItem compi = (ImageItem)comp;
				WritePropertyS("EXPRESSION", compi.Expression, astream);
				if (compi.Stream != null)
					if (compi.Stream.Length > 0)
					{
						compi.Stream.Seek(0, SeekOrigin.Begin);
						WritePropertyB("STREAM", compi.Stream, astream);
					}
				WritePropertyI("ROTATION", compi.Rotation, astream);
				WritePropertyI("DRAWSTYLE", (int)compi.DrawStyle, astream);
				WritePropertyI("DPIRES", compi.dpires, astream);
				WritePropertyI("COPYMODE", compi.CopyMode, astream);
				WritePropertyI("CACHEDIMAGE", (int)compi.SharedImage, astream);
			}
			else
			// TRpChart
			if (comp is ChartItem)
			{
				ChartItem compc = (ChartItem)comp;
				WritePropertyS("VALUEEXPRESSION", compc.ValueExpression, astream);
				WritePropertyS("VALUEXEXPRESSION", compc.ValueXExpression, astream);
				WritePropertyS("GETVALUECONDITION", compc.GetValueCondition, astream);
				WritePropertyS("CHANGESERIEEXPRESSION", compc.ChangeSerieExpression, astream);
				WritePropertyS("CAPTIONEXPRESSION", compc.CaptionExpression, astream);
				WritePropertyS("COLOREXPRESSION", compc.ColorExpression, astream);
				WritePropertyS("SERIECOLOREXPRESSION", compc.SerieColorExpression, astream);
				WritePropertyS("SERIECAPTION", compc.SerieCaption, astream);
				WritePropertyS("CLEAREXPRESSION", compc.ClearExpression, astream);
				//  WritePropertyI('SERIES',Integer(compc.Series),Stream);
				WritePropertyBool("CHANGESERIEBOOL", compc.ChangeSerieBool, astream);
				WritePropertyI("CHARTTYPE", (int)compc.ChartStyle, astream);
				WritePropertyS("IDENTIFIER", compc.Identifier, astream);
				WritePropertyBool("CLEAREXPRESSIONBOOL", compc.ClearExpressionBool, astream);
				WritePropertyI("DRIVER", (int)compc.Driver, astream);
				WritePropertyBool("VIEW3D", compc.View3d, astream);
				WritePropertyBool("VIEW3DWALLS", compc.View3dWalls, astream);
				WritePropertyI("PERSPECTIVE", compc.Perspective, astream);
				WritePropertyI("ELEVATION", compc.Elevation, astream);
				WritePropertyI("ROTATION", compc.Rotation, astream);
				WritePropertyI("ZOOM", compc.Rotation, astream);
				WritePropertyI("HORZOFFSET", compc.HorzOffset, astream);
				WritePropertyI("VERTOFFSET", compc.VertOffset, astream);
				WritePropertyI("TILT", compc.Tilt, astream);
				WritePropertyBool("ORTHOGONAL", compc.Orthogonal, astream);
				WritePropertyI("MULTIBAR", (int)compc.MultiBar, astream);
				WritePropertyI("RESOLUTION", compc.Resolution, astream);
				WritePropertyBool("SHOWLEGEND", compc.ShowLegend, astream);
				WritePropertyBool("SHOWHINT", compc.ShowHint, astream);
				WritePropertyI("MARKSTYLE", compc.MarkStyle, astream);
				WritePropertyI("HORZFONTSIZE", compc.HorzFontSize, astream);
				WritePropertyI("VERTFONTSIZE", compc.VertFontSize, astream);
				WritePropertyI("HORZFONTROTATION", compc.HorzFontRotation, astream);
				WritePropertyI("VERTFONTROTATION", compc.VertFontRotation, astream);
				WritePropertyI("AUTORANGE", (int)compc.AutoRange, astream);
				WritePropertyD("YMIN", compc.AxisYInitial, astream);
				WritePropertyD("YMAX", compc.AxisYFinal, astream);
			}
			else
			// TRpBarcode
			if (comp is BarcodeItem)
			{
				BarcodeItem compb = (BarcodeItem)comp;
				WritePropertyS("EXPRESSION", compb.Expression, astream);
				WritePropertyI("MODUL", compb.Modul, astream);
				WritePropertyD("RATIO", compb.Ratio, astream);
				WritePropertyI("TYP", (int)compb.BarType, astream);
				WritePropertyBool("CHECKSUM", compb.Checksum, astream);
				WritePropertyBool("TRANSPARENT", compb.Transparent, astream);
				WritePropertyS("DISPLAYFORMAT", compb.DisplayFormat, astream);
				WritePropertyI("ROTATION", compb.Rotation, astream);
				WritePropertyI("BCOLOR", compb.BColor, astream);
				WritePropertyI("BACKCOLOR", compb.BackColor, astream);
				WritePropertyI("NUMCOLUMNS", compb.NumColumns, astream);
				WritePropertyI("NUMROWS", compb.NumRows, astream);
				WritePropertyI("ECCLEVEL", compb.ECCLevel, astream);
				WritePropertyBool("TRUNCATED", compb.Truncated, astream);
			}
			StreamUtil.SWriteLine(astream, "</COMPONENT>");

		}
		private void WriteDatabaseInfoXML(DatabaseInfo dbinfo, Stream astream)
		{
			WritePropertyS("ALIAS", dbinfo.Alias, astream);
			//            WritePropertyS("CONFIGFILE",dbinfo.Configfile,astream);
			//            WritePropertyBool("LOADPARAMS",dbinfo.LoadParams,astream);
			//            WritePropertyBool("LOADDRIVERPARAMS",dbinfo.LoadDriverParams,astream);
			//            WritePropertyBool("LOGINPROMPT",dbinfo.LoginPrompt,astream);
			WritePropertyI("DRIVER", (int)dbinfo.Driver, astream);
			WritePropertyS("REPORTTABLE", dbinfo.ReportTable, astream);
			WritePropertyS("REPORTSEARCHFIELD", dbinfo.ReportSearchField, astream);
			WritePropertyS("REPORTFIELD", dbinfo.ReportField, astream);
			WritePropertyS("REPORTGROUPSTABLE", dbinfo.ReportGroupsTable, astream);
			WritePropertyS("ADOCONNECTIONSTRING", dbinfo.ConnectionString, astream);
			WritePropertyI("DOTNETDRIVER", (int)dbinfo.DotNetDriver, astream);
			WritePropertyS("PROVIDERFACTORY", dbinfo.ProviderFactory, astream);
		}
		private void WriteDataInfoXML(DataInfo dinfo, Stream astream)
		{
			WritePropertyS("ALIAS", dinfo.Alias, astream);
			WritePropertyS("DATABASEALIAS", dinfo.DatabaseAlias, astream);
			WritePropertyS("SQL", dinfo.SQL, astream);
			WritePropertyS("DATASOURCE", dinfo.DataSource, astream);
			WritePropertyS("MYBASEFILENAME", dinfo.MyBaseFilename, astream);
			WritePropertyS("MYBASEFIELDS", dinfo.MyBaseFields, astream);
			WritePropertyS("MYBASEINDEXFIELDS", dinfo.MyBaseIndexFields, astream);
			WritePropertyS("MYBASEMASTERFIELDS", dinfo.MyBaseMasterFields, astream);
			WritePropertyS("BDEINDEXFIELDS", dinfo.BDEIndexFields, astream);
			WritePropertyS("BDEINDEXNAME", dinfo.BDEIndexName, astream);
			WritePropertyS("BDETABLE", dinfo.BDETable, astream);
			WritePropertyI("BDETYPE", (int)dinfo.BDEType, astream);
			WritePropertyS("BDEFILTER", dinfo.BDEFilter, astream);
			WritePropertyS("BDEMASTERFIELDS", dinfo.BDEMasterFields, astream);
			WritePropertyS("BDEFIRSTRANGE", dinfo.BDEFirstRange, astream);
			WritePropertyS("BDELASTRANGE", dinfo.BDELastRange, astream);
			WritePropertyS("DATAUNIONS", dinfo.DataUnions.Text, astream);
			WritePropertyBool("GROUPUNION", dinfo.GroupUnion, astream);
			WritePropertyBool("OPENONSTART", dinfo.OpenOnStart, astream);
			WritePropertyBool("PARALLELUNION", dinfo.ParallelUnion, astream);
		}
		private void WriteParamXML(Param aparam, Stream astream)
		{
			WritePropertyS("NAME", aparam.Alias, astream);
			WritePropertyS("DESCRIPTION", aparam.Descriptions, astream);
			WritePropertyS("HINT", aparam.Hints, astream);
			WritePropertyS("ERRORMESSAGE", aparam.ErrorMessage, astream);
			WritePropertyS("VALIDATION", aparam.Validation, astream);
			WritePropertyS("SEARCH", aparam.Search, astream);
			WritePropertyBool("VISIBLE", aparam.Visible, astream);
			WritePropertyBool("ISREADONLY", aparam.IsReadOnly, astream);
			WritePropertyBool("NEVERVISIBLE", aparam.NeverVisible, astream);
			WritePropertyBool("ALLOWNULLS", aparam.AllowNulls, astream);
			WritePropertyI("PARAMTYPE", (int)aparam.ParamType, astream);
			WritePropertyS("DATASETS", aparam.Datasets.Text, astream);
			WritePropertyS("ITEMS", aparam.Items.Text, astream);
			WritePropertyS("VALUES", aparam.Values.Text, astream);
			WritePropertyS("SELECTED", aparam.Selected.Text, astream);
			WritePropertyS("LOOKUPDATASET", aparam.LookupDataset, astream);
			WritePropertyS("SEARCHDATASET", aparam.SearchDataset, astream);
			WritePropertyS("SEARCHPARAM", aparam.SearchParam, astream);
			switch (aparam.ParamType)
			{
				case ParamType.String:
				case ParamType.ExpreA:
				case ParamType.ExpreB:
				case ParamType.Subst:
				case ParamType.List:
				case ParamType.SubsExpreList:
				case ParamType.InitialValue:
				case ParamType.Unknown:
					WritePropertyS("VALUE", aparam.Value.ToString(), astream);
					break;
				case ParamType.Integer:
					if (aparam.Value.VarType == VariantType.String)
					{
						if (DoubleUtil.IsNumeric(aparam.Value.ToString(), NumberStyles.Integer))
						{
							int valorInt = Convert.ToInt32(aparam.Value.ToString());
							aparam.Value = valorInt;
							WritePropertyI("VALUE", valorInt, astream);
						}
						else
							WritePropertyS("VALUE", aparam.Value.ToString(), astream);
					}
					else
						WritePropertyI("VALUE", aparam.Value, astream);
					break;
				case ParamType.Double:
					if (aparam.Value.VarType == VariantType.String)
					{
						if (DoubleUtil.IsNumeric(aparam.Value.ToString(), NumberStyles.Float))
						{
							double valorInt = Convert.ToDouble(aparam.Value.ToString());
							aparam.Value = valorInt;
							WritePropertyD("VALUE", valorInt, astream);
						}
						else
							WritePropertyS("VALUE", aparam.Value.ToString(), astream);
					}
					else
						WritePropertyD("VALUE", aparam.Value, astream);
					break;
				case ParamType.Currency:
					if (aparam.Value.VarType == VariantType.String)
					{
						if (DoubleUtil.IsNumeric(aparam.Value.ToString(), NumberStyles.Currency))
						{
							double valorInt = Convert.ToDouble(aparam.Value.ToString());
							aparam.Value = valorInt;
							WritePropertyD("VALUE", valorInt, astream);
						}
						else
							WritePropertyS("VALUE", aparam.Value.ToString(), astream);
					}
					else
						WritePropertyD("VALUE", aparam.Value, astream);
					break;
				case ParamType.Date:
				case ParamType.Time:
				case ParamType.DateTime:
					WritePropertyDateTime("VALUE", aparam.Value, astream);
					break;
				case ParamType.Bool:
					WritePropertyBool("VALUE", aparam.Value, astream);
					break;
			}

		}
		private void WriteSubReportXML(SubReport subrep, Stream astream)
		{
			WritePropertyS("NAME", subrep.Name, astream);
			WritePropertyS("ALIAS", subrep.Alias, astream);
			if (subrep.ParentSubReport != null)
				WritePropertyS("PARENTSUBREPORT", subrep.ParentSubReport.Name, astream);
			if (subrep.ParentSection != null)
				WritePropertyS("PARENTSECTION", subrep.ParentSection.Name, astream);
			WritePropertyBool("PRINTONLYIFDATAAVAILABLE", subrep.PrintOnlyIfDataAvailable, astream);
			WritePropertyBool("REOPENONPRINT", subrep.ReOpenOnPrint, astream);
		}
		private void WriteSectionXML(Section asection, Stream astream)
		{
			StreamUtil.SWriteLine(astream, "<SECTION>");

			WritePropertyS("NAME", asection.Name, astream);
			WritePropertyI("WIDTH", asection.Width, astream);
			WritePropertyI("HEIGHT", asection.Height, astream);
			WritePropertyS("PRINTCONDITION", asection.PrintCondition, astream);
			WritePropertyS("DOBEFOREPRINT", asection.DoBeforePrint, astream);
			WritePropertyS("DOAFTERPRINT", asection.DoAfterPrint, astream);
			if (asection.SubReport != null)
				WritePropertyS("SUBREPORT", asection.SubReport.Name, astream);
			WritePropertyS("GROUPNAME", asection.GroupName, astream);
			WritePropertyS("CHANGEEXPRESSION", asection.ChangeExpression, astream);
			WritePropertyS("BEGINPAGEEXPRESSION", asection.BeginPageExpression, astream);
			WritePropertyS("SKIPEXPREV", asection.SkipExpreV, astream);
			WritePropertyS("SKIPEXPREH", asection.SkipExpreH, astream);
			WritePropertyS("SKIPTOPAGEEXPRE", asection.SkipToPageExpre, astream);
			WritePropertyS("BACKEXPRESSION", asection.BackExpression, astream);
			WritePropertyBool("CHANGEBOOL", asection.ChangeBool, astream);
			WritePropertyBool("PAGEREPEAT", asection.PageRepeat, astream);
			WritePropertyBool("FORCEPRINT", asection.ForcePrint, astream);
			WritePropertyBool("SKIPPAGE", asection.SkipPage, astream);
			WritePropertyBool("ALIGNBOTTOM", asection.AlignBottom, astream);
			WritePropertyI("SECTIONTYPE", (int)asection.SectionType, astream);
			WritePropertyBool("AUTOEXPAND", asection.AutoExpand, astream);
			WritePropertyBool("AUTOCONTRACT", asection.AutoContract, astream);
			WritePropertyBool("HORZDESP", asection.HorzDesp, astream);
			WritePropertyBool("VERTDESP", asection.VertDesp, astream);
			WritePropertyS("EXTERNALFILENAME", asection.ExternalFilename, astream);
			WritePropertyS("EXTERNALCONNECTION", asection.ExternalConnection, astream);
			WritePropertyS("EXTERNALTABLE", asection.ExternalTable, astream);
			WritePropertyS("EXTERNALFIELD", asection.ExternalField, astream);
			WritePropertyS("EXTERNALSEARCHFIELD", asection.ExternalSearchField, astream);
			WritePropertyS("EXTERNALSEARCHVALUE", asection.ExternalSearchValue, astream);
			WritePropertyI("STREAMFORMAT", (int)asection.StreamFormat, astream);
			if (asection.ChildSubReport != null)
			{
				WritePropertyS("CHILDSUBREPORT", asection.ChildSubReport.Name, astream);
			}
			WritePropertyBool("SKIPRELATIVEH", asection.SkipRelativeH, astream);
			WritePropertyBool("SKIPRELATIVEV", asection.SkipRelativeV, astream);
			WritePropertyI("SKIPTYPE", (int)asection.SkipType, astream);
			WritePropertyBool("ININUMPAGE", asection.IniNumPage, astream);
			WritePropertyBool("GLOBAL", asection.Global, astream);
			WritePropertyI("DPIRES", asection.dpires, astream);
			WritePropertyI("CACHEDIMAGE", (int)asection.SharedImage, astream);
			WritePropertyI("BACKSTYLE", (int)asection.BackStyle, astream);
			WritePropertyI("DRAWSTYLE", (int)asection.DrawStyle, astream);
			if (asection.Stream != null)
			{
				if (asection.Stream.Length > 0)
				{
					asection.Stream.Seek(0, SeekOrigin.Begin);
					WritePropertyB("STREAM", asection.Stream, astream);
				}
			}
			foreach (PrintPosItem aitem in asection.Components)
			{
				WriteComponentXML(aitem, astream);
			}
			StreamUtil.SWriteLine(astream, "</SECTION>");

		}
		public static string WriteComponents(List<PrintPosItem> nitems)
		{
			string nresult = "";
			using (MemoryStream astream = new MemoryStream())
			{
				StreamUtil.SWriteLine(astream, "<SECTION>");
				foreach (PrintPosItem aitem in nitems)
				{
					WriteComponentXML(aitem, astream);
				}
				StreamUtil.SWriteLine(astream, "</SECTION>");
				byte[] acontent = astream.ToArray();
				nresult = StreamUtil.ByteArrayToString(acontent, acontent.Length);
			}
			return nresult;
		}
		/// <summary>
		/// Save the report template to a stream, in xml format
		/// </summary>
		/// <param name="astream"></param>
		public void SaveToStream(Stream astream)
		{
			// Check if the format should be compressed
			if (areport.StreamFormat == StreamFormatType.XMLZlib)
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
		private void IntSaveToStream(Stream astream)
		{
			// Write header
			string astring = "<?xml version=\"1.0\" standalone=\"no\"?>";
			StreamUtil.SWriteLine(astream, astring);
			astring = "<!DOCTYPE REPORT_MANAGER_2>";
			StreamUtil.SWriteLine(astream, astring);
			// Write XML Report properties
			astring = "<REPORT>";
			StreamUtil.SWriteLine(astream, astring);
			WriteReportPropsXML(areport, astream);

			// Write database info list
			foreach (DatabaseInfo dbinfo in areport.DatabaseInfo)
			{
				astring = "<DATABASEINFO>";
				StreamUtil.SWriteLine(astream, astring);
				WriteDatabaseInfoXML(dbinfo, astream);
				astring = "</DATABASEINFO>";
				StreamUtil.SWriteLine(astream, astring);
			}
			// Write datainfo list
			foreach (DataInfo dinfo in areport.DataInfo)
			{
				astring = "<DATAINFO>";
				StreamUtil.SWriteLine(astream, astring);
				WriteDataInfoXML(dinfo, astream);
				astring = "</DATAINFO>";
				StreamUtil.SWriteLine(astream, astring);
			}
			// Write parameter list
			foreach (Param aparam in areport.Params)
			{
				astring = "<PARAMETER>";
				StreamUtil.SWriteLine(astream, astring);
				WriteParamXML(aparam, astream);
				astring = "</PARAMETER>";
				StreamUtil.SWriteLine(astream, astring);
			}
			// Write subreports
			foreach (SubReport asubrep in areport.SubReports)
			{
				astring = "<SUBREPORT>";
				StreamUtil.SWriteLine(astream, astring);
				WriteSubReportXML(asubrep, astream);
				foreach (Section asection in asubrep.Sections)
				{
					WriteSectionXML(asection, astream);
				}
				astring = "</SUBREPORT>";
				StreamUtil.SWriteLine(astream, astring);
			}
			astring = "</REPORT>";
			StreamUtil.SWriteLine(astream, astring);
		}
		/// <summary>
		/// Save the report template to a file, in xml report template format
		/// </summary>
		/// <param name="afilename"></param>
		public void SaveToFile(string afilename)
		{
			FileStream astream = new FileStream(afilename, System.IO.FileMode.Create,
				System.IO.FileAccess.Write);
			try
			{
				SaveToStream(astream);
			}
			finally
			{
				astream.Close();
			}
		}
	}
}