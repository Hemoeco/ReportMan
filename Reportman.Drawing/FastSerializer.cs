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
using System.Data;
using System.Text;
using System.IO;

namespace Reportman.Drawing
{
    public class FastSerializer
    {
        enum TypeData
        {
            Char, String, Int32, Int16, Byte, Double, Float, Decimal, Single, ByteArray,
            DateTime, Int64, Boolean, Object, TimeSpan
        };
        private static TypeData TypeToTypeData(System.Type ntype)
        {
            TypeData coltype = TypeData.Int32;
            string typestring = ntype.ToString();
            switch (typestring)
            {
                case "System.Int32":
                    coltype = TypeData.Int32;
                    break;
                case "System.Boolean":
                    coltype = TypeData.Boolean;
                    break;
                case "System.Int16":
                    coltype = TypeData.Int16;
                    break;
                case "System.Int64":
                    coltype = TypeData.Int64;
                    break;
                case "System.Char":
                    coltype = TypeData.Char;
                    break;
                case "System.Byte":
                    coltype = TypeData.Byte;
                    break;
                case "System.String":
                    coltype = TypeData.String;
                    break;
                case "System.Double":
                    coltype = TypeData.Double;
                    break;
                case "System.Float":
                    coltype = TypeData.Float;
                    break;
                case "System.Single":
                    coltype = TypeData.Single;
                    break;
                case "System.Decimal":
                    coltype = TypeData.Decimal;
                    break;
                case "System.DateTime":
                    coltype = TypeData.DateTime;
                    break;
                case "System.TimeSpan":
                    coltype = TypeData.TimeSpan;
                    break;
                case "System.Byte[]":
                    coltype = TypeData.ByteArray;
                    break;
                case "System.Object":
                    coltype = TypeData.Object;
                    break;
                default:
                    throw new Exception("Data type not supported in FastSerializer: " + typestring);
            }
            return coltype;

        }
        private static System.Type TypeDataToType(TypeData ntype)
        {
            System.Type nresult;
            switch (ntype)
            {
                case TypeData.Boolean:
                    nresult = typeof(bool);
                    break;
                case TypeData.Int32:
                    nresult = typeof(Int32);
                    break;
                case TypeData.Int16:
                    nresult = typeof(Int16);
                    break;
                case TypeData.Int64:
                    nresult = typeof(Int64);
                    break;
                case TypeData.Char:
                    nresult = typeof(char);
                    break;
                case TypeData.Byte:
                    nresult = typeof(byte);
                    break;
                case TypeData.String:
                    nresult = typeof(string);
                    break;
                case TypeData.Double:
                    nresult = typeof(double);
                    break;
                case TypeData.Float:
                    nresult = typeof(float);
                    break;
                case TypeData.Single:
                    nresult = typeof(float);
                    break;
                case TypeData.Decimal:
                    nresult = typeof(decimal);
                    break;
                case TypeData.DateTime:
                    nresult = typeof(DateTime);
                    break;
                case TypeData.TimeSpan:
                    nresult = typeof(TimeSpan);
                    break;
                case TypeData.ByteArray:
                    nresult = typeof(byte[]);
                    break;
                case TypeData.Object:
                    nresult = typeof(object);
                    break;
                default:
                    throw new Exception("Data type not supported in FastSerializer: " + ntype.ToString());
            }
            return nresult;
        }
#if PocketPC
#else
        [ThreadStatic]
#endif
        static FastDataLocalStorage localstorage;

        static byte[] bytenull
        {
            get
            {
                if (localstorage == null)
                    localstorage = new FastDataLocalStorage();
                return localstorage.bytenull;
            }
        }
        static byte[] bytenonull
        {
            get
            {
                if (localstorage == null)
                    localstorage = new FastDataLocalStorage();
                return localstorage.bytenonull;
            }
        }
        static byte[] signature
        {
            get
            {
                if (localstorage == null)
                    localstorage = new FastDataLocalStorage();
                return localstorage.signature;
            }
        }
        static UTF8Encoding nencoder
        {
            get
            {
                if (localstorage == null)
                    localstorage = new FastDataLocalStorage();
                return localstorage.nencoder;
            }
        }


        public static MemoryStream SerializeDataSet(DataSet ndata)
        {
            MemoryStream nstream = new MemoryStream();
            BinaryWriter nwriter = new BinaryWriter(nstream);
            nstream.Write(signature, 0, 4);

            WriteInteger(nstream, ndata.Tables.Count);
            byte[] bytenull = new byte[1];
            bytenull[0] = 0;
            byte[] bytenonull = new byte[1];
            bytenonull[0] = 1;


            foreach (DataTable ntable in ndata.Tables)
            {
                WriteString(nstream, ntable.TableName);
                TypeData[] coltypes = new TypeData[ntable.Columns.Count];
                int i = 0;
                int colcount = ntable.Columns.Count;
                WriteInteger(nstream, colcount);
                foreach (DataColumn ncol in ntable.Columns)
                {
                    coltypes[i] = TypeToTypeData(ncol.DataType);
                    WriteInteger(nstream, (int)coltypes[i]);
                    WriteString(nstream, ncol.ColumnName);
                    i++;
                }
                bool hasprimary = false;
                foreach (Constraint nconst in ntable.Constraints)
                {
                    if (nconst is UniqueConstraint)
                    {
                        UniqueConstraint cprim = (UniqueConstraint)nconst;
                        if (cprim.IsPrimaryKey)
                        {
                            hasprimary = true;
                            WriteInteger(nstream, cprim.Columns.Length);
                            foreach (DataColumn ncol in cprim.Columns)
                            {
                                WriteString(nstream, ncol.ColumnName);
                            }
                        }
                    }
                }
                if (!hasprimary)
                    WriteInteger(nstream, 0);
                WriteInteger(nstream, ntable.Rows.Count);
                foreach (DataRow xrow in ntable.Rows)
                {
                    for (i = 0; i < colcount; i++)
                    {
                        object nvalue = xrow[i];
                        WriteValue(nstream, nwriter, coltypes[i], nvalue);
                    }
                }
            }
            return nstream;
        }
        private static void WriteValue(MemoryStream nstream, BinaryWriter nwriter, TypeData ntype, object nvalue)
        {
            if (nvalue == DBNull.Value)
            {
                nstream.Write(bytenull, 0, 1);
                return;
            }
            byte[] nres = null;

            switch (ntype)
            {
                case TypeData.Int32:
                    nstream.Write(bytenonull, 0, 1);
                    nwriter.Write((int)nvalue);
                    //nres = StreamUtil.IntToByteArray((int)nvalue);
                    break;
                case TypeData.Int64:
                    nstream.Write(bytenonull, 0, 1);
                    nwriter.Write((long)nvalue);
                    //nres = StreamUtil.Int64ToByteArray((Int64)nvalue);
                    break;
                case TypeData.Int16:
                    nstream.Write(bytenonull, 0, 1);
                    nwriter.Write((short)nvalue);
                    //nres = StreamUtil.ShortToByteArray((short)nvalue);
                    break;
                case TypeData.Char:
                    nres = StreamUtil.ShortToByteArray((short)(char)nvalue);
                    break;
                case TypeData.Byte:
                    nres = new byte[1];
                    nres[0] = (byte)nvalue;
                    break;
                case TypeData.String:
                    nstream.Write(bytenonull, 0, 1);
                    WriteString(nstream, (string)nvalue);
                    break;
                case TypeData.Double:
                    nstream.Write(bytenonull, 0, 1);
                    nwriter.Write((double)nvalue);
                    break;
                case TypeData.Float:
                    nstream.Write(bytenonull, 0, 1);
                    nwriter.Write((float)nvalue);
                    break;
                case TypeData.Single:
                    nstream.Write(bytenonull, 0, 1);
                    nwriter.Write((Single)nvalue);
                    break;
                case TypeData.Boolean:
                    nstream.Write(bytenonull, 0, 1);
                    nwriter.Write((bool)nvalue);
                    break;
                case TypeData.Decimal:
                    nstream.Write(bytenonull, 0, 1);
                    nwriter.Write((decimal)nvalue);
                    break;
                case TypeData.TimeSpan:
                    long nticks = ((TimeSpan)nvalue).Ticks;
                    nstream.Write(bytenonull, 0, 1);
                    nwriter.Write(nticks);
                    break;
                case TypeData.DateTime:
                    DateTime dvalue = ((DateTime)nvalue);
                    if ((dvalue.Hour == 0) && (dvalue.Minute == 0) && (dvalue.Second == 0) && (dvalue.Millisecond == 0))
                        nres = new byte[4];
                    else
                        if (dvalue.Millisecond == 0)
                        nres = new byte[7];
                    else
                        nres = new byte[9];
                    byte[] ax = StreamUtil.ShortToByteArray((short)dvalue.Year);
                    nres[0] = ax[0];
                    nres[1] = ax[1];
                    nres[2] = (byte)dvalue.Month;
                    nres[3] = (byte)dvalue.Day;
                    if ((dvalue.Hour != 0) || (dvalue.Minute != 0) || (dvalue.Second != 0) || (dvalue.Millisecond != 0))
                    {
                        nres[4] = (byte)dvalue.Hour;
                        nres[5] = (byte)dvalue.Minute;
                        nres[6] = (byte)dvalue.Second;
                        if (dvalue.Millisecond != 0)
                        {
                            short Millisecond = (short)dvalue.Millisecond;
                            if ((Millisecond > 999) || (Millisecond < 0))
                                Millisecond = 0;
                            ax = StreamUtil.ShortToByteArray(Millisecond);
                            nres[7] = ax[0];
                            nres[8] = ax[1];
                        }
                    }
                    break;
                case TypeData.ByteArray:
                    byte[] avalue = (byte[])nvalue;
                    nstream.Write(bytenonull, 0, 1);
                    WriteInteger(nstream, avalue.Length);
                    nstream.Write(avalue, 0, avalue.Length);
                    break;
                case TypeData.Object:
                    string realtype = nvalue.GetType().ToString();
                    if (realtype == null)
                    {
                        nstream.Write(bytenull, 0, 1);
                    }
                    else
                    {
                        System.Type ntypex = System.Type.GetType(realtype);
                        if (ntypex != null)
                        {
                            TypeData newtype = TypeToTypeData(ntypex);
                            if (newtype == TypeData.Object)
                                throw new Exception("Unsupported object datacolumn containint object");
                            nstream.Write(bytenonull, 0, 1);
                            WriteString(nstream, realtype);
                            WriteValue(nstream, nwriter, newtype, nvalue);
                        }
                        else
                        {
                            nstream.Write(bytenull, 0, 1);
                        }
                    }
                    break;
                default:
                    throw new Exception("Data type not supported in FastSerializer");
            }
            if (nres != null)
            {
                byte nlen = (byte)nres.Length;
                while (nlen > 1)
                {
                    if (nres[nlen - 1] == 0)
                    {
                        nlen--;
                    }
                    else
                        break;
                }
                byte[] blen = new byte[1];
                blen[0] = nlen;
                nstream.Write(blen, 0, 1);
                nstream.Write(nres, 0, nlen);
                //nstream.Write(nres, 0, nres.Length);
            }
        }


        private static object ReadValue(byte[] nbytes, BinaryReader nreader, TypeData ntype, ref int index)
        {
            //byte[] nres = null;
            byte nlen = nbytes[index];
            index++;
            if (nlen == 0)
            {
                return DBNull.Value;
            }
            switch (ntype)
            {
                case TypeData.Int32:
                    nreader.BaseStream.Seek(index, SeekOrigin.Begin);
                    int iresult = nreader.ReadInt32();
                    index = index + ((int)nreader.BaseStream.Position - index);
                    return iresult;
                //int iresult = StreamUtil.ByteArrayToInt(nbytes, index, nlen);
                //index = index + nlen;
                //return iresult;
                case TypeData.Int64:
                    //              long i64result = StreamUtil.ByteArrayToInt64(nbytes, index, nlen);
                    //              index = index + nlen;
                    //              return i64result;
                    nreader.BaseStream.Seek(index, SeekOrigin.Begin);
                    long i64result = nreader.ReadInt64();
                    index = index + ((int)nreader.BaseStream.Position - index);
                    return i64result;

                case TypeData.Int16:
                    //              int i16result = StreamUtil.ByteArrayToShort(nbytes, index, nlen);
                    //              index = index + nlen;
                    //              return i16result;
                    nreader.BaseStream.Seek(index, SeekOrigin.Begin);
                    short i16result = nreader.ReadInt16();
                    index = index + ((int)nreader.BaseStream.Position - index);
                    return i16result;
                case TypeData.Char:
                    char nchar = (char)StreamUtil.ByteArrayToShort(nbytes, index, nlen);
                    index = index + nlen;
                    return nchar;
                case TypeData.Byte:
                    index = index + 1;
                    return nbytes[index - 1];
                case TypeData.String:
                    return ReadString(nbytes, ref index);
                case TypeData.Double:
                    nreader.BaseStream.Seek(index, SeekOrigin.Begin);
                    Double doublevalue = nreader.ReadDouble();
                    index = index + ((int)nreader.BaseStream.Position - index);
                    return doublevalue;
                case TypeData.Float:
                    nreader.BaseStream.Seek(index, SeekOrigin.Begin);
                    float floatvalue = nreader.ReadSingle();
                    index = index + ((int)nreader.BaseStream.Position - index);
                    return floatvalue;
                case TypeData.Single:
                    nreader.BaseStream.Seek(index, SeekOrigin.Begin);
                    Single singlevalue = nreader.ReadSingle();
                    index = index + ((int)nreader.BaseStream.Position - index);
                    return singlevalue;
                case TypeData.Boolean:
                    nreader.BaseStream.Seek(index, SeekOrigin.Begin);
                    bool boolvalue = nreader.ReadBoolean();
                    index = index + ((int)nreader.BaseStream.Position - index);
                    return boolvalue;
                case TypeData.Decimal:
                    nreader.BaseStream.Seek(index, SeekOrigin.Begin);
                    decimal decimalvalue = nreader.ReadDecimal();
                    index = index + ((int)nreader.BaseStream.Position - index);
                    return decimalvalue;
                case TypeData.TimeSpan:
                    nreader.BaseStream.Seek(index, SeekOrigin.Begin);
                    long itimeresult = nreader.ReadInt64();
                    index = index + ((int)nreader.BaseStream.Position - index);
                    return new TimeSpan(itimeresult);
                case TypeData.DateTime:
                    //              nwriter.Write((DateTime)nvalue);
                    short Year = StreamUtil.ByteArrayToShort(nbytes, index, 2);
                    short Millisecond = 0;
                    if (nlen > 8)
                        Millisecond = StreamUtil.ByteArrayToShort(nbytes, index + 7, 2);
                    else
                        if (nlen > 7)
                        Millisecond = StreamUtil.ByteArrayToShort(nbytes, index + 7, 1);
                    byte seconds = 0;
                    if (nlen > 6)
                        seconds = nbytes[index + 6];
                    byte minutes = 0;
                    if (nlen > 5)
                        minutes = nbytes[index + 5];
                    byte hours = 0;
                    if (nlen > 4)
                        hours = nbytes[index + 4];
                    DateTime dvalue = new DateTime(Year, nbytes[index + 2], nbytes[index + 3], hours,
                      minutes, seconds, Millisecond);
                    index = index + nlen;
                    return dvalue;
                case TypeData.ByteArray:
                    int blength = StreamUtil.ByteArrayToInt(nbytes, index, 4);
                    index = index + 4;
                    byte[] bbvalue = new byte[blength];
                    if (blength > 0)
                    {
                        Array.Copy(nbytes, index, bbvalue, 0, blength);
                        index = index + blength;
                    }
                    return bbvalue;
                case TypeData.Object:
                    string realtype = ReadString(nbytes, ref index);
                    TypeData newtype = TypeToTypeData(System.Type.GetType(realtype));
                    if (newtype == TypeData.Object)
                        throw new Exception("Unsupported object datacolumn containint object");
                    return ReadValue(nbytes, nreader, newtype, ref index);
                default:
                    throw new Exception("Data type not supported in FastSerializer");
            }
        }
        private static void WriteInteger(MemoryStream nstream, int nvalue)
        {
            byte[] nbyte = StreamUtil.IntToByteArray(nvalue);
            nstream.Write(nbyte, 0, 4);
        }
        private static void WriteString(MemoryStream nstream, string nvalue)
        {
            byte[] nbytes = nencoder.GetBytes(nvalue);
            WriteInteger(nstream, nbytes.Length);
            nstream.Write(nbytes, 0, nbytes.Length);
            //WriteInteger(nstream, nvalue.Length);
            //WriteIntString(nstream,nvalue);
        }
        private static void WriteIntString(MemoryStream nstream, string nvalue)
        {
            byte[] abuf = new byte[2];
            foreach (char xchar in nvalue)
            {
                abuf[0] = (byte)xchar;
                abuf[1] = (byte)(((int)xchar) >> 8);
                nstream.Write(abuf, 0, 2);
            }
        }
        public static string ReadString(byte[] nbytes, ref int index)
        {
            int alen = StreamUtil.ByteArrayToInt(nbytes, index, 4);
            index = index + 4;
            if (alen > 0)
            {
                string astring = nencoder.GetString(nbytes, index, alen);
                index = index + alen;
                return astring;
            }
            else
                return "";
            /*          int nchars = StreamUtil.ByteArrayToInt(nbytes, index, 4);
                      index = index + 4;
                      if (nchars == 0)
                        return "";
                      StringBuilder nbuilder = new StringBuilder();
                      short nchar;
                      for (int i = 0; i < nchars; i++)
                      {
                        nchar = (short)((short)nbytes[index] + (((short)nbytes[index + 1]) << 8));
                        nbuilder.Append((char)nchar);
                        index = index + 2;
                      }
                      return nbuilder.ToString();*/
        }
        public static bool IsFastSerialized(byte[] nbytes)
        {
            if (nbytes.Length < 8)
                return false;
            if ((nbytes[0] != 10) || (nbytes[1] != 11) || (nbytes[2] != 12) || (nbytes[3] != 13))
                return false;
            else
                return true;

        }
        public static DataSet DeSerializeDataSet(byte[] nbytes)
        {
            BinaryReader nreader = new BinaryReader(new MemoryStream(nbytes));
            DataSet ndataset = new DataSet();
            int index = 0;
            if (nbytes.Length < 8)
                throw new Exception("Incorrect header in DeserializeDataSet");
            if ((nbytes[0] != 10) || (nbytes[1] != 11) || (nbytes[2] != 12) || (nbytes[3] != 13))
                throw new Exception("Incorrect header in DeserializeDataSet");
            index = 4;
            int ndatatables = StreamUtil.ByteArrayToInt(nbytes, index, 4);
            index = index + 4;
            for (int indextable = 0; indextable < ndatatables; indextable++)
            {
                string tablename = ReadString(nbytes, ref index);
                DataTable newtable = new DataTable(tablename);
                newtable.CaseSensitive = true;
                int colcount = StreamUtil.ByteArrayToInt(nbytes, index, 4);
                index = index + 4;
                TypeData[] coltypes = new TypeData[colcount];
                for (int indexcol = 0; indexcol < colcount; indexcol++)
                {
                    TypeData ntype = (TypeData)StreamUtil.ByteArrayToInt(nbytes, index, 4);
                    coltypes[indexcol] = ntype;
                    index = index + 4;
                    string colname = ReadString(nbytes, ref index);
                    DataColumn newcol = newtable.Columns.Add(colname, TypeDataToType(ntype));
                }
                // Constraint
                int colsprim = StreamUtil.ByteArrayToInt(nbytes, index, 4);
                index = index + 4;
                if (colsprim < 0)
                    throw new Exception("Incorrect format colsprim");
                DataColumn[] colprim = new DataColumn[colsprim];
                for (int indexprim = 0; indexprim < colsprim; indexprim++)
                {
                    string colprimname = ReadString(nbytes, ref index);
                    colprim[indexprim] = newtable.Columns[colprimname];
                    if (colprim[indexprim] == null)
                        throw new Exception("Column not found in table " +
                                            tablename + " column " + colprim[indexprim]);
                }
                // Read rows
                int rowcount = StreamUtil.ByteArrayToInt(nbytes, index, 4);
                index = index + 4;
                if (rowcount > 0)
                {
                    object[] rowvalues = new object[colcount];
                    //for (int acol = 0; acol < colcount; acol++)
                    //rowvalues[acol] = DBNull.Value;
                    for (int rowindex = 0; rowindex < rowcount; rowindex++)
                    {
                        for (int colindex = 0; colindex < colcount; colindex++)
                        {
                            rowvalues[colindex] = ReadValue(nbytes, nreader, coltypes[colindex], ref index);
                        }
                        newtable.Rows.Add(rowvalues);
                    }
                }
                if (colprim.Length > 0)
                    newtable.Constraints.Add("PRIM" + newtable.TableName, colprim, true);
                ndataset.Tables.Add(newtable);
            }
            return ndataset;
        }
        public static string ReadString(BinaryReader nreader)
        {
            byte[] nbytes = nreader.ReadBytes(4);
            int alen = StreamUtil.ByteArrayToInt(nbytes, 0, 4);
            if (alen > 0)
            {
                nbytes = nreader.ReadBytes(alen);
                string astring = nencoder.GetString(nbytes, 0, alen);
                return astring;
            }
            else
                return "";
        }
        private static object ReadValue(BinaryReader nreader, TypeData ntype)
        {
            //byte[] nres = null;
            byte nlen = nreader.ReadByte();
            if (nlen == 0)
            {
                return DBNull.Value;
            }
            switch (ntype)
            {
                case TypeData.Int32:
                    int iresult = nreader.ReadInt32();
                    return iresult;
                //int iresult = StreamUtil.ByteArrayToInt(nbytes, index, nlen);
                //index = index + nlen;
                //return iresult;
                case TypeData.Int64:
                    long i64result = nreader.ReadInt64();
                    return i64result;
                case TypeData.Int16:
                    short i16result = nreader.ReadInt16();
                    return i16result;
                case TypeData.Char:
                    char nchar = nreader.ReadChar();
                    return nchar;
                case TypeData.Byte:
                    return nreader.ReadByte();
                case TypeData.String:
                    return ReadString(nreader);
                case TypeData.Double:
                    Double doublevalue = nreader.ReadDouble();
                    return doublevalue;
                case TypeData.Float:
                    float floatvalue = nreader.ReadSingle();
                    return floatvalue;
                case TypeData.Single:
                    Single singlevalue = nreader.ReadSingle();
                    return singlevalue;
                case TypeData.Boolean:
                    bool boolvalue = nreader.ReadBoolean();
                    return boolvalue;
                case TypeData.Decimal:
#if WINDOWS_PHONE
					nreader.BaseStream.Seek(index, SeekOrigin.Begin);
					Double doublevalue2 = nreader.ReadDouble();
					index = index + ((int)nreader.BaseStream.Position - index);
					return System.Convert.ToDecimal(doublevalue2);
#else
                    decimal decimalvalue = nreader.ReadDecimal();
                    return decimalvalue;
#endif
                case TypeData.TimeSpan:
                    long itimeresult = nreader.ReadInt64();
                    return new TimeSpan(itimeresult);
                case TypeData.DateTime:
                    if (nlen < 4)
                        throw new Exception("Invalid date time deserializing datatable");
                    byte[] nbytes = nreader.ReadBytes(2);
                    short Year = StreamUtil.ByteArrayToShort(nbytes, 0, 2);
                    byte month = nreader.ReadByte();
                    byte day = nreader.ReadByte();

                    byte hours = 0;
                    if (nlen > 4)
                        hours = nreader.ReadByte();
                    byte minutes = 0;
                    if (nlen > 5)
                        minutes = nreader.ReadByte();
                    byte seconds = 0;
                    if (nlen > 6)
                        seconds = nreader.ReadByte();
                    short Millisecond = 0;
                    if (nlen == 7)
                    {
                        Millisecond = nreader.ReadByte();
                    }
                    else
                        if (nlen > 7)
                    {
                        nbytes = nreader.ReadBytes(2);
                        Millisecond = StreamUtil.ByteArrayToShort(nbytes, 0, 2);
                    }

                    DateTime dvalue = new DateTime(Year, month, day, hours,
                        minutes, seconds, Millisecond);
                    return dvalue;

                /*
                short Year = StreamUtil.ByteArrayToShort(nbytes, index, 2);
                short Millisecond = 0;
                if (nlen > 8)
                    Millisecond = StreamUtil.ByteArrayToShort(nbytes, index + 7, 2);
                else
                    if (nlen > 7)
                    Millisecond = StreamUtil.ByteArrayToShort(nbytes, index + 7, 1);
                byte seconds = 0;
                if (nlen > 6)
                    seconds = nbytes[index + 6];
                byte minutes = 0;
                if (nlen > 5)
                    minutes = nbytes[index + 5];
                byte hours = 0;
                if (nlen > 4)
                    hours = nbytes[index + 4];
                DateTime dvalue = new DateTime(Year, nbytes[index + 2], nbytes[index + 3], hours,
                  minutes, seconds, Millisecond);
                index = index + nlen;
                return dvalue;*/

                case TypeData.ByteArray:
                    byte[] nbytes2 = nreader.ReadBytes(4);
                    int blength = StreamUtil.ByteArrayToInt(nbytes2, 0, 4);
                    byte[] bbvalue = nreader.ReadBytes(blength);
                    return bbvalue;
                case TypeData.Object:
                    string realtype = ReadString(nreader);
                    TypeData newtype = TypeToTypeData(System.Type.GetType(realtype));
                    if (newtype == TypeData.Object)
                        throw new Exception("Unsupported object MyData.DataColumn containint object");
                    return ReadValue(nreader, newtype);
                default:
                    throw new Exception("Data type not supported in FastSerializer");
            }
            //if (nres != null)
            //  nstream.Write(nres, 0, nres.Length);
        }

        public static DataSet DeSerializeDataSet(Stream nsource)
        {
            BinaryReader nreader = new BinaryReader(nsource);
            byte[] nbytes = nreader.ReadBytes(4);
            DataSet ndataset = new DataSet();
            int versionformat = 0;
            if (nbytes.Length < 4)
                throw new Exception("Incorrect header in DeserializeDataSet");
            if ((nbytes[0] != 10) || (nbytes[1] != 11) || (nbytes[2] != 12))
                throw new Exception("Incorrect header in DeserializeDataSet");
            versionformat = nbytes[3] - 13;
            if (versionformat < 0)
                throw new Exception("Incorrect header in DeserializeDataSet");
            nbytes = nreader.ReadBytes(4);
            int ndatatables = StreamUtil.ByteArrayToInt(nbytes, 0, 4);
            for (int indextable = 0; indextable < ndatatables; indextable++)
            {
                string tablename = ReadString(nreader);
                DataTable newtable = new DataTable(tablename);
                nbytes = nreader.ReadBytes(4);
                int colcount = StreamUtil.ByteArrayToInt(nbytes, 0, 4);
                TypeData[] coltypes = new TypeData[colcount];
                for (int indexcol = 0; indexcol < colcount; indexcol++)
                {
                    nbytes = nreader.ReadBytes(4);
                    TypeData ntype = (TypeData)StreamUtil.ByteArrayToInt(nbytes, 0, 4);
                    coltypes[indexcol] = ntype;
                    string colname = ReadString(nreader);
                    DataColumn newcol = newtable.Columns.Add(colname, TypeDataToType(ntype));
                }
                // MyData.Constraint
                nbytes = nreader.ReadBytes(4);
                int colsprim = StreamUtil.ByteArrayToInt(nbytes, 0, 4);
                if (colsprim < 0)
                    throw new Exception("Incorrect format colsprim");
                DataColumn[] colprim = new DataColumn[colsprim];
                for (int indexprim = 0; indexprim < colsprim; indexprim++)
                {
                    string colprimname = ReadString(nreader);
                    colprim[indexprim] = newtable.Columns[colprimname];
                    if (colprim[indexprim] == null)
                        throw new Exception("Column not found in table " +
                                            tablename + " column " + colprim[indexprim]);
                }
                // Read rows
                nbytes = nreader.ReadBytes(4);
                int rowcount = StreamUtil.ByteArrayToInt(nbytes, 0, 4);
                if (rowcount > 0)
                {
                    object[] rowvalues = new object[colcount];
                    for (int rowindex = 0; rowindex < rowcount; rowindex++)
                    {
                        for (int colindex = 0; colindex < colcount; colindex++)
                        {
                            rowvalues[colindex] = ReadValue(nreader, coltypes[colindex]);
                        }
                        DataRow newrow = newtable.Rows.Add(rowvalues);
                        if (versionformat > 0)
                        {
                            DataRowState nstate = (DataRowState)ReadValue(nreader, TypeData.Byte);
                            switch (nstate)
                            {
                                case DataRowState.Added:
                                    newrow.SetAdded();
                                    break;
                                case DataRowState.Deleted:
                                    newrow.Delete();
                                    break;
                                case DataRowState.Modified:
                                    newrow.SetModified();
                                    break;
                            }
                        }
                    }
                }
                if (colprim.Length > 0)
                    newtable.Constraints.Add("PRIM" + newtable.TableName, colprim, true);
                ndataset.Tables.Add(newtable);
            }
            return ndataset;
        }
    }
    class FastDataLocalStorage
    {

        public byte[] bytenull;
        public byte[] bytenonull;
        public byte[] signature;
        public UTF8Encoding nencoder;

        public FastDataLocalStorage()
        {
            bytenull = new byte[1];
            bytenonull = new byte[1];
            signature = new byte[4] { 10, 11, 12, 13 };
            nencoder = new UTF8Encoding();

            bytenull[0] = 0;
            bytenonull[0] = 1;
        }

    }
}
