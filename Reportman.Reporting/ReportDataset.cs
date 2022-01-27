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
using System.Data;
using System.Collections.Generic;
#if REPMAN_DOTNET2
using System.Xml;
#endif

namespace Reportman.Reporting
{
    /// <summary>
    /// ReportDataset is used internally by the reporting engine.
    /// Allow navigation in a IDataReader, using a two record buffer, so you can go back one record, but
    /// not two. The navigation is done forward, and does not consume memory (like IDataReader).
    /// </summary>
    /// <remarks>
    /// A two record buffer is needed to do report grouping, the reporting engine must go back one record
    /// when a group expression changes, and print group footers. This class is a helper to perform this task,
    /// but you can use it also to access to information inside a DataReader using a 
    /// DataTable approach instead a DataReader approach, you will obtain a performance improvement
    /// when the number of records is high because the records are not stored in memory
    /// </remarks>
    public class ReportDataset : DataTable
	{
        public delegate void UpdateDataEvent(object sender,DataRow xrow);
        public delegate void CreateTableEvent(object sender, DataTable xtable);

        public UpdateDataEvent OnUpdateData;
        public CreateTableEvent OnCreateTable;

        private bool FEof;
		private bool FInternalEof;
		private int FLastFetched;
		private int FBuffer;
		private int FCurrent;
		private IDataReader FCurrentReader;
        private DataView FCurrentView;
        private DataRowView[] CurrentRowSet;
        private object[] FViewFilter;
        public SortedList<string, int> ColumnSizes;
        public ReportDataset()
        {
            ColumnSizes = new SortedList<string, int>();
        }
        public int CurrentRowCount
        {
            get
            {
                int ncount=0;
                if (FCurrentReader != null)
                    ncount = Rows.Count;
                else
                    if (FCurrentView != null)
                    {
                        if (CurrentRowSet != null)
                            ncount = CurrentRowSet.Length;
                        else
                            ncount = CurrentView.Count;
                    }
                return ncount;
            }
        }
        /// <summary>
        /// Assign this event if you are interested in knowing when the current record data changes
        /// </summary>
		public event DataChange OnDataChange;
        /// <summary>
        /// When a DataReader is assigned, by default columns in the DataTable are created from datareader information, 
        /// you can turn off the column creation setting the property to false, that is useful to enhace performance when
        /// you assign diferent DataReaders but with the same columns
        /// </summary>
        public bool UpdateColumns;
        /// <summary>
        /// Indicates if a DataReader have been assigned, this is a read only property
        /// </summary>
		public bool Active
		{
			get { return (FCurrentReader != null) || (FCurrentView !=null); }
		}
		private void DoUpdateData()
		{
			if (UpdateColumns)
			{
				Clear();
				Columns.Clear();
			}
            ColumnSizes.Clear();
			Rows.Clear();
			BeginLoadData();
			FEof = false;
			FInternalEof = false;
			if (FCurrentReader == null)
			{
                if (FCurrentView == null)
                {
                    FEof = true;
                    FInternalEof = true;
                    return;
                }
			}
			int i;
			if (Columns.Count == 0)
			{
                if (FCurrentReader != null)
                {
                    DataTable adatatable = FCurrentReader.GetSchemaTable();
                    DataColumn col;

                    for (i = 0; i < adatatable.Rows.Count; i++)
                    {
                        string acolname;
                        DataRow nrow = adatatable.Rows[i];
                        //					col=Columns.Add();
                        acolname = nrow["ColumnName"].ToString().ToUpper();
                        if (adatatable.Columns.IndexOf("COLUMNSIZE") >= 0)
                        {
                            if (nrow["COLUMNSIZE"] != DBNull.Value)
                            {
                                if (ColumnSizes.IndexOfKey(acolname) >= 0)
                                    throw new Exception("Repeated column: " + acolname);
                                ColumnSizes.Add(acolname, System.Convert.ToInt32(nrow["COLUMNSIZE"]));
                            }
                        }
                        if (acolname.Length < 1)
                            acolname = "Column" + i.ToString();
                        //					col.ColumnName=acolname;
                        //					col.DataType=(Type)nrow["DataType"];
                        //					col.Caption=acolname;
                        //					col.Caption=acolname;
                        col = Columns.Add(acolname, (Type)nrow["DataType"]);
                        if (col.DataType.ToString() == "System.String")
                        {
                            int maxlength = (int)nrow["ColumnSize"];
                            col.MaxLength = maxlength;
                        }
                        col.Caption = acolname;
                    }
                }
                else
                {
                    foreach (DataColumn colview in FCurrentView.Table.Columns)
                        Columns.Add(colview.ColumnName, colview.DataType);
                }
                if (OnCreateTable != null)
                    OnCreateTable(this, this);
			}
			FCurrent = 0;
			FBuffer = -1;
			FLastFetched = 0;
			DataRow arow;
            if (FCurrentReader != null)
            {
                if (FCurrentReader.Read())
                {
                    arow = NewRow();
                    Rows.Add(arow);
                    for (i = 0; i < Columns.Count; i++)
                        arow[i] = FCurrentReader[i];
                    if (OnUpdateData != null)
                        OnUpdateData(this,arow);
                }
                else
                {
                    FEof = true;
                    FInternalEof = true;
                }
                FViewFilter = null;
                UpdateViewRows();
            }
            else
            {
                if (FCurrentView.Count == 0)
                {
                    FEof = true;
                    FInternalEof = true;
                }
            }
		}
        /// <summary>
        /// Returns current active row, if you never call Prior, it's the last fetched row in the DataReader
        /// </summary>
        public DataRow CurrentRow
		{
			get
			{
                if (FCurrentView != null)
                {
                    if (CurrentRowSet != null)
                    {
                        if (FCurrent < CurrentRowSet.Length)
                            return CurrentRowSet[FCurrent].Row;
                        else
                        {
                            if (CurrentRowSet.Length > 0)
                                return CurrentRowSet[CurrentRowSet.Length - 1].Row;
                            else
                                return null;
                        }
                    }
                    else
                    {
                        if (FCurrent < FCurrentView.Count)
                            return FCurrentView[FCurrent].Row;
                        else
                        {
                            if (FCurrentView.Count>0)
                                return FCurrentView[FCurrentView.Count-1].Row;
                            else
                                return null;
                        }
                    }
                }
                else
                {
                    if (Rows.Count > FCurrent)
                        return Rows[FCurrent];
                    else
                        return null;
                }
			}
		}
        /// <summary>
        /// Because there is a two record buffer you can access the row that is not active
        /// </summary>
        public DataRow OtherRow
		{
			get
			{
				DataRow aresult = null;
                if (FCurrentView != null)
                {
                    if (CurrentRowSet != null)
                    {
                        if (FCurrent > 0)
                        {
                            aresult = CurrentRowSet[FCurrent - 1].Row;
                        }
                        else
                            aresult = CurrentRowSet[0].Row;
                    }
                    else
                    {
                        if (FCurrent > 0)
                        {
                            aresult = FCurrentView[FCurrent - 1].Row;
                        }
                        else
                            aresult = FCurrentView[0].Row;
                    }
                }
                else
                {
                    if (Rows.Count < 2)
                    {
                        aresult = Rows[FCurrent];
                    }
                    else
                    {
                        aresult = Rows[FBuffer];
                    }
                }
				return aresult;
			}
		}
        /// <summary>
        /// This is the main property of the class, you must assign it to provide data to the DataTable.
        /// One record is fetched (if available) when you assign this property.
        /// </summary>
        public IDataReader CurrentReader
		{
			get
			{
				return FCurrentReader;
			}
			set
			{
				if (FCurrentReader != value)
				{
					FCurrentReader = value;
					DoUpdateData();
				}
			}
		}
        public object[] ViewFilter
        {
            get { return FViewFilter; }
            set
            {
                FViewFilter = value;
                UpdateViewRows();
            }
        }
        /// <summary>
        /// This is the main property of the class, you must assign it to provide data to the DataTable.
        /// One record is fetched (if available) when you assign this property.
        /// </summary>
        public DataView CurrentView
        {
            get
            {
                return FCurrentView;
            }
            set
            {
                if (FCurrentView != value)
                {
                    FCurrentView = value;
                    DoUpdateData();
                }
            }
        }
        /// <summary>
        /// Returns true when there are no more records to fetch, or also when the assigned DataReader does not returned any data
        /// </summary>
        public bool Eof
		{
			get
			{
                if ((FCurrentReader == null) && (FCurrentView == null))
                    return true;
				return FInternalEof;
			}
		}
        /// <summary>
        /// Go to first record only for inmemeory views
        /// </summary>
        public void First()
        {
            if (FCurrentView == null)
                throw new Exception("First only supported memeory");
            FCurrent = 0;
            if (CurrentRowSet != null)
            {
                FEof = CurrentRowSet.Length == 0;
                FInternalEof = FEof;
            }
            else
            {
                FEof = CurrentView.Count == 0;
                FInternalEof = FEof;
            }
        }
        /// <summary>
        /// Moves to the next record in the internal DataReader, storing the contents in the DataTable.
        /// If you called Prior, then only changes the row returned by CurrentRow
        /// </summary>
        public bool Next()
		{
            if (FCurrentView != null)
            {
                if (CurrentRowSet != null)
                {
                    if (FCurrent < (CurrentRowSet.Length-1))
                    {
                        FCurrent++;
                        if (OnDataChange != null)
                            OnDataChange(this);
//                        if ((FEof) && (FCurrent == CurrentRowSet.Length - 1))
//                            return false;
//                        else
                            return true;
                    }
                    else
                    {
                        FInternalEof = true;
                        FEof = true;
                        return false;
                    }
                }
                else
                {
                    if (FCurrent < FCurrentView.Count-1)
                    {
                        FCurrent++;
                        if (OnDataChange != null)
                            OnDataChange(this);
                        return true;
                    }
                    else
                    {
                        FInternalEof = true;
                        FEof = true;
                        return false;
                    }
                }
            }
			if (FCurrentReader == null)
				return false;
			if (FLastFetched != FCurrent)
			{
				FCurrent = FLastFetched;
				FInternalEof = FEof;
                if (OnDataChange != null)
                  OnDataChange(this);
                return true;
			}
			if (FEof)
				return false;
			FEof = !FCurrentReader.Read();
			if (FEof)
			{
				FInternalEof = true;
				return false;
			}
			int i;
			DataRow arow;
			if (Rows.Count < 2)
			{
				FLastFetched = Rows.Count;
				FBuffer = FCurrent;
				FCurrent = FLastFetched;
				arow = NewRow();
				Rows.Add(arow);
			}
			else
			{
				arow = Rows[FBuffer];
				FLastFetched = FBuffer;
				FBuffer = FCurrent;
				FCurrent = FLastFetched;
			}
			for (i = 0; i < Columns.Count; i++)
				arow[i] = FCurrentReader[i];
            if (OnUpdateData != null)
                OnUpdateData(this, arow);

            if (OnDataChange != null)
				OnDataChange(this);
			return true;
		}
        /// <summary>
        /// Moves to the prior record in the DataTable, you can only go back one record
        /// </summary>
        public bool Prior()
		{
            if (FCurrentView != null)
            {
                if (FCurrent == 0)
                    return false;
                FCurrent--;
                if (OnDataChange != null)
                    OnDataChange(this);
                return true;
            }
			if (FCurrentReader == null)
				return false;
			if (Rows.Count < 2)
				return false;
			if (FLastFetched == FCurrent)
			{
				FCurrent = FBuffer;
			}
			else
			{
				FCurrent = FLastFetched;
			}
            if (OnDataChange != null)
                OnDataChange(this);
            return true;
		}
        private void UpdateViewRows()
        {
            CurrentRowSet = null;
            if (FViewFilter != null)
            {
                CurrentRowSet = FCurrentView.FindRows(FViewFilter);
                FCurrent = 0;
                if (CurrentRowSet.Length == 0)
                {
                    FEof = true;
                    FInternalEof = true;
                }
                else
                {
                    FEof = false;
                    FInternalEof = false;
                }
            }
        }
    }
    /// <summary>
    /// Delegate used by ReportDataset to trigger an event when CurrentRow data changes
    /// </summary>
    public delegate void DataChange(ReportDataset Data);
}
