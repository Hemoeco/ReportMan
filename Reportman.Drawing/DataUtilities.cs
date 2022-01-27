using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Reportman.Drawing
{
    /// <summary>
    /// DataTable and DataSet utilities
    /// </summary>
    public class DataUtilities
    {
        static Dictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>();
        /// <summary>
        /// Copy a DataTable, the standard Copy() command does not work for byte[] columns
        /// </summary>
        /// <param name="ntable"></param>
        /// <returns></returns>
        public static DataTable Copy(DataTable ntable)
        {
            DataTable newtable = ntable.Clone();

            int colcount = newtable.Columns.Count;
            object[] values = new object[colcount];
            foreach (DataRow xrow in ntable.Rows)
            {
                for (int i = 0; i < colcount; i++)
                    values[i] = xrow[i];
                newtable.Rows.Add(values);
            }
            return newtable;
        }
        static DataUtilities()
        {
            typeMap[typeof(byte)] = DbType.Byte;
            typeMap[typeof(sbyte)] = DbType.SByte;
            typeMap[typeof(short)] = DbType.Int16;
            typeMap[typeof(ushort)] = DbType.UInt16;
            typeMap[typeof(int)] = DbType.Int32;
            typeMap[typeof(uint)] = DbType.UInt32;
            typeMap[typeof(long)] = DbType.Int64;
            typeMap[typeof(ulong)] = DbType.UInt64;
            typeMap[typeof(float)] = DbType.Single;
            typeMap[typeof(double)] = DbType.Double;
            typeMap[typeof(decimal)] = DbType.Decimal;
            typeMap[typeof(bool)] = DbType.Boolean;
            typeMap[typeof(string)] = DbType.String;
            typeMap[typeof(char)] = DbType.StringFixedLength;
            typeMap[typeof(Guid)] = DbType.Guid;
            typeMap[typeof(DateTime)] = DbType.DateTime;
#if PocketPC
#else
            typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
#endif
            typeMap[typeof(byte[])] = DbType.Binary;
            typeMap[typeof(byte?)] = DbType.Byte;
            typeMap[typeof(sbyte?)] = DbType.SByte;
            typeMap[typeof(short?)] = DbType.Int16;
            typeMap[typeof(ushort?)] = DbType.UInt16;
            typeMap[typeof(int?)] = DbType.Int32;
            typeMap[typeof(uint?)] = DbType.UInt32;
            typeMap[typeof(long?)] = DbType.Int64;
            typeMap[typeof(ulong?)] = DbType.UInt64;
            typeMap[typeof(float?)] = DbType.Single;
            typeMap[typeof(double?)] = DbType.Double;
            typeMap[typeof(decimal?)] = DbType.Decimal;
            typeMap[typeof(bool?)] = DbType.Boolean;
            typeMap[typeof(char?)] = DbType.StringFixedLength;
            typeMap[typeof(Guid?)] = DbType.Guid;
            typeMap[typeof(DateTime?)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            //typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
        }
        public static System.Data.DbType TypeToDbType(System.Type ntype)
        {
            return typeMap[ntype];
        }
        public static DataTable GroupBy(List<DataTable> sources, string groupCols, string sumCols)
        {
            List<string> groupColumns = groupCols.Split(';').ToList();
            List<string> sumColumns = sumCols.Split(';').ToList();
            DataTable result = (DataTable)sources[0].Clone();
            DataView nview = new DataView(result, "", String.Join(",", groupColumns), DataViewRowState.CurrentRows);
            var keys = new object[groupColumns.Count];
            foreach (DataTable table in sources)
            {
                foreach (DataRow row in table.Rows)
                {
                    DataRow newrow = null;
                    for (int i = 0; i < groupColumns.Count; i++)
                    {
                        string key = groupColumns[i];
                        keys[i] = row[key];
                    }
                    DataRowView[] findRows = nview.FindRows(keys);
                    if (findRows.Length > 0)
                        newrow = findRows[0].Row;
                    else
                    {
                        newrow = result.NewRow();
                        foreach (string key in groupColumns)
                        {
                            newrow[key] = row[key];
                        }
                        result.Rows.Add(newrow);
                    }
                    for (int i = 0; i < sumColumns.Count; i++)
                    {
                        string key = sumColumns[i];
                        object source = row[key];
                        object destination = newrow[key];
                        if (destination == DBNull.Value)
                            destination = source;
                        else
                        {
                            if (source != DBNull.Value)
                            {
                                switch (destination.GetType().ToString())
                                {
                                    case "System.Decimal":
                                        destination = Convert.ToDecimal(destination) + Convert.ToDecimal(source);
                                        break;
                                    case "System.Double":
                                        destination = Convert.ToDouble(destination) + Convert.ToDouble(source);
                                        break;
                                    case "System.Int32":
                                    case "System.Int16":
                                        destination = Convert.ToInt32(destination) + Convert.ToInt32(source);
                                        break;
                                    case "System.String":
                                        destination = destination.ToString() + source.ToString();
                                        break;
                                    default:
                                        throw new Exception("Sum not implemented for type " + destination.GetType().ToString());
                                }
                            }
                        }
                        newrow[key] = destination;
                    }
                }
            }
            return result;
        }


    }
}
