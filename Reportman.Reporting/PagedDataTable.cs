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
#if REPMAN_DOTNET2
using System.Xml;
#endif

namespace Reportman.Reporting
{
	public class PagedDataTable : DataTable
	{
		private bool FEof;
		private bool FInternalEof;
		private IDataReader FCurrentReader;
		public event PagedDataChange OnDataChange;
		public bool UpdateColumns;
		public int PageSize;
		public bool Active
		{
			get { return (FCurrentReader != null); }
		}
		public PagedDataTable():base()
		{
			PageSize=500;
		}
		private void DoUpdateData()
		{
			if (UpdateColumns)
			{
				Clear();
				Columns.Clear();
			}
			Rows.Clear();
			BeginLoadData();
			FEof = false;
			FInternalEof = false;
			if (FCurrentReader == null)
			{
				FEof = true;
				FInternalEof = true;
				return;
			}
			int i;
			if (Columns.Count == 0)
			{
				DataTable adatatable = FCurrentReader.GetSchemaTable();
				DataColumn col;

				for (i = 0; i < adatatable.Rows.Count; i++)
				{
					string acolname;
					DataRow nrow = adatatable.Rows[i];
					//					col=Columns.Add();
					acolname = nrow["ColumnName"].ToString().ToUpper();
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
			int x=0;
			while (x<PageSize)
			{
				x++;
				if (!Next())
					break;
			}
		}
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
		public bool Eof
		{
			get
			{
				return FInternalEof;
			}
		}
		public bool Next()
		{
			if (FCurrentReader == null)
				return false;
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
			arow = NewRow();
			for (i = 0; i < Columns.Count; i++)
				arow[i] = FCurrentReader[i];
			Rows.Add(arow);
			if (OnDataChange != null)
				OnDataChange(this);
			return true;
		}
	}
	public delegate void PagedDataChange(PagedDataTable Data);
}
