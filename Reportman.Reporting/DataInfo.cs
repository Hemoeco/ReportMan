using System;
using System.Collections.Generic;
using Reportman.Drawing;
using System.Data;
using System.IO;

namespace Reportman.Reporting
{
    /// <summary>
    /// DatabaInfo stores information about a dataset, a Report have a collection
    /// of dataset definitions. Each dataset is related to one connection (DatabaseInfo)<see cref="Variant">Report</see><see cref="Variant">DataInfos</see>
    /// </summary>
    public class DataInfo : ReportItem, IDisposable, ICloneable
    {
        Strings masterfieldslist;
        Strings sharedfieldslist;
        object[] mastervalues;

        private Evaluator internalevaluator;
        private Evaluator GetEvaluator()
        {
            if (internalevaluator == null)
            {
                internalevaluator = new Evaluator();
            }
            return internalevaluator;
        }
        private string OldSQLUsed;
        private bool connecting;
        /// <summary>DataInfo name (alias)</summary>
		public string Alias;
        /// <summary>DatabaseInfo alias related to this dataset</summary>
		public string DatabaseAlias;
        /// <summary>Sql sentence, for parameters precede them by double quotes in native drivers or by @ symbol in .Net drivers</summary>
		public string SQL;
        /// <summary>A master dataset can be assigned so the query is executed each time the parameters of the
        /// query change, the parameters with the same name as master dataset fields will be checked</summary>
		public string DataSource;
        /// <summary>Filename to loadwhen MyBase driver is selected</summary>
		public string MyBaseFilename;
        /// <summary>Field definition file when Mybase driver is selected</summary>
        public string MyBaseFields;
        /// <summary>Index fields when MyBase driver is selected</summary>
        public string MyBaseIndexFields;
        /// <summary>Master fields when MyBase driver is selected</summary>
        public string MyBaseMasterFields;
        /// <summary>Index fields when BDE driver is selected</summary>
        public string BDEIndexFields;
        /// <summary>Index name when BDE driver is selected</summary>
        public string BDEIndexName;
        /// <summary>Index table name when BDE driver is selected and BDEType is Table</summary>
        public string BDETable;
        /// <summary>BDE dataset type (table,query) when BDE driver is selected</summary>
        public DatasetType BDEType;
        /// <summary>BDE filter BDE driver is selected</summary>
        public string BDEFilter;
        /// <summary>BDE master fields BDE driver is selected</summary>
        public string BDEMasterFields;
        /// <summary>BDE first range filter BDE driver is selected</summary>
        public string BDEFirstRange;
        /// <summary>BDE last range filter BDE driver is selected</summary>
        public string BDELastRange;
        /// <summary>When Mybase driver is selected, you can fill the dataset (in memory dataset) with other datasets</summary>
        public Strings DataUnions;
        /// <summary>When Mybase driver is selected, you can fill the dataset (in memory dataset) with other datasets, with grouping option</summary>
        public bool GroupUnion;
        /// <summary>When Mybase driver is selected, and union is performed, 
        /// </summary>
        public bool ParallelUnion;
        /// <summary>By default all datasets are open on start, set this property to false when you want to open the dataset at runtime manually</summary>
        public bool OpenOnStart;
        /// <summary>Current IDataReader</summary>
		public IDataReader DataReader;
        /// <summary>Current in memory two record buffer table</summary>
        public ReportDataset Data;
        /// <summary>Current IDbCommand</summary>
		public System.Data.IDbCommand Command;
        /// <summary>Set this property to override the sql sentence at runtime, set is before executing the report</summary>
        public string SQLOverride;
        public DataView DataViewOverride;
        /// <summary>
        /// Free resources
        /// </summary>
		override public void Dispose()
        {
            base.Dispose();
            if (Data != null)
            {
                Data.Dispose();
                Data = null;
            }
            // TODO: Check for assigned connections and dispose them
            // That is check if they implements IDisposable and call Dispose()
        }
        protected override string GetClassName()
        {
            return "TRPDATAINFOITEM";
        }
        /// <summary>
        /// Clone the DataInfo item
        /// </summary>
        /// <param name="areport">The new owner of the DataInfo</param>
        /// <returns>A new DataInfo item</returns>
		public DataInfo Clone(Report areport)
        {
            DataInfo ninfo = (DataInfo)Clone();
            ninfo.Report = areport;
            return ninfo;
        }
        /// <summary>
        /// Clone the DataInfo item
        /// </summary>
        /// <returns>A new DataInfo item</returns>
        public object Clone()
        {
            DataInfo ninfo = new DataInfo(Report);
            ninfo.Alias = Alias;
            ninfo.BDEFilter = BDEFilter;
            ninfo.BDEFirstRange = this.BDEFirstRange;
            ninfo.BDEIndexFields = this.BDEIndexFields;
            ninfo.BDEIndexName = this.BDEIndexName;
            ninfo.BDELastRange = this.BDELastRange;
            ninfo.BDEMasterFields = this.BDEMasterFields;
            ninfo.BDETable = this.BDETable;
            ninfo.BDEType = this.BDEType;
            ninfo.DatabaseAlias = this.DatabaseAlias;
            ninfo.DataSource = this.DataSource;
            ninfo.DataUnions = (Strings)this.DataUnions.Clone();
            ninfo.GroupUnion = this.GroupUnion;
            ninfo.MyBaseFields = this.MyBaseFields;
            ninfo.MyBaseFilename = this.MyBaseFilename;
            ninfo.MyBaseIndexFields = this.MyBaseIndexFields;
            ninfo.MyBaseMasterFields = this.MyBaseMasterFields;
            ninfo.Name = Name;
            ninfo.OpenOnStart = this.OpenOnStart;
            ninfo.Report = Report;
            ninfo.SQL = this.SQL;
            return ninfo;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rp">The owner</param>
        public DataInfo(BaseReport rp)
            : base(rp)
        {
            OpenOnStart = true;
            DataUnions = new Strings();
            Alias = ""; DatabaseAlias = ""; SQL = ""; DataSource = ""; SQLOverride = "";
            MyBaseFilename = ""; MyBaseFields = ""; MyBaseIndexFields = ""; MyBaseMasterFields = "";
            BDEIndexFields = ""; BDEIndexName = ""; BDETable = "";
            BDEFilter = ""; BDEMasterFields = ""; BDEFirstRange = ""; BDELastRange = "";
            Data = new ReportDataset();
        }
        /// <summary>
        /// Obtain the related DatabaseInfoItem, searching by DatabaseAlias
        /// </summary>
        /// <returns>The DatabaseInfo or throws an exception</returns>
		public DatabaseInfo GetDbItem()
        {
            int index = -1;
            DatabaseInfos infos = Report.DatabaseInfo;
            for (int i = 0; i < infos.Count; i++)
            {
                if (infos[i].Alias == DatabaseAlias)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
                return infos[index];
            else
                throw new NamedException("Dabase Alias not found: " + DatabaseAlias, DatabaseAlias);
        }
        /// <summary>
        /// Obtain field information from the dataset, useful for the designer
        /// </summary>
        /// <param name="astream">Stream to fill with the information</param>
		public void GetFieldsInfo(Stream astream)
        {
            int i;
            for (i = 0; i < Data.Columns.Count; i++)
            {
                StreamUtil.SWriteLine(astream, Data.Columns[i].ColumnName);
                StreamUtil.SWriteLine(astream, Data.Columns[i].DataType.ToString());
                StreamUtil.SWriteLine(astream, Data.Columns[i].MaxLength.ToString());
            }
        }
        public void GoFirstMem()
        {
            if (Data.CurrentView == null)
                return;
            Data.First();
        }
        /// <summary>
        /// Open the dataset and read the first record into the data variable
        /// </summary>
		public void Connect()
        {
            if (connecting)
                throw new NamedException("Circular datalinks not allowed:" + Alias, Alias);
            if ((Data.CurrentReader != null) || (Data.CurrentView != null))
                return;
            connecting = true;
            try
            {
                DatabaseInfo dbitem = GetDbItem();
                dbitem.Connect();
                DataReader = null;
                Data.CurrentReader = null;
                Data.CurrentView = null;
                Data.UpdateColumns = true;

                Params aparams = Report.Params;


                DataInfo mastersource = null;
                // Open first the master source
                if (DataSource.Length > 0)
                {
                    int index = Report.DataInfo.IndexOf(DataSource);
                    if (index < 0)
                        throw new NamedException("Master source not found: "
                         + DataSource + " in " + Alias, DataSource);
                    mastersource = Report.DataInfo[index];
                    Report.UpdateParamsBeforeOpen(index, true);
                    mastersource.Connect();
                }
                if ((dbitem.Driver == DriverType.Mybase) || (DataViewOverride != null))
                {
#if REPMAN_COMPACT
                    throw new Exception("Driver Mybase not supported in Compact framework");
#else
                    if (DataViewOverride != null)
                    {
                        Data.CurrentView = DataViewOverride;

                        if (mastersource != null)
                        {
                            UpdateParams(mastersource.Data, Command);
                            Data.ViewFilter = mastervalues;

                            mastersource.Data.OnDataChange += new DataChange(OnDataChange);
                        }
                    }
                    else
                    {
                        Strings primarycolumns = new Strings();
                        masterfieldslist = Strings.FromSemiColon(MyBaseMasterFields.ToUpper());
                        masterfieldslist.RemoveBlanks();
                        mastervalues = new object[masterfieldslist.Count];

                        for (int ixvalue = 0; ixvalue < masterfieldslist.Count; ixvalue++)
                            mastervalues[ixvalue] = DBNull.Value;
                        DataSet intdataset = dbitem.Report.DatabaseInfo.MemoryDataSet;
                        DataTable inttable = null;
                        int indextable = intdataset.Tables.IndexOf(Alias);
                        if (indextable >= 0)
                        {
                            inttable = intdataset.Tables[indextable];
                            intdataset.Tables.Remove(inttable);
                            inttable.Dispose();
                            inttable = null;
                        }
                        if (this.DataUnions.Count > 0)
                        {
                            DataView primview = null;

                            SortedList<string, string> fieldsunion = null;
                            if (ParallelUnion)
                                fieldsunion = new SortedList<string, string>();
                            int idxunion = 0;
                            foreach (string ntable in DataUnions)
                            {
                                string field_prefix = "";
                                if (idxunion > 0)
                                    field_prefix = "Q" + (idxunion + 1).ToString("00") + "_";
                                if (ParallelUnion)
                                    fieldsunion.Clear();
                                // Create structure
                                Strings nnames = Strings.FromSeparator('-', ntable);
                                nnames.RemoveBlanks();
                                string tablename = nnames[0];
                                if (nnames.Count < 2)
                                    sharedfieldslist = new Strings();
                                else
                                {
                                    sharedfieldslist = Strings.FromSemiColon(nnames[1].ToUpper());
                                    sharedfieldslist.RemoveBlanks();
                                }
                                if (Report.DataInfo[tablename].DataReader == null)
                                    Report.DataInfo[tablename].Connect();
                                else
                                {

                                    if (Report.UsedDataReaders.IndexOfKey(tablename) >= 0)
                                    {
                                        Report.DataInfo[tablename].DisConnect();
                                        Report.DataInfo[tablename].Connect();
                                    }
                                    else
                                        Report.UsedDataReaders.Add(tablename, tablename);
                                }
                                IDataReader FCurrentReader = Report.DataInfo[tablename].DataReader;
                                DataView FCurrentDataView = null;
                                bool isempty;
                                if (FCurrentReader == null)
                                {
                                    FCurrentDataView = Report.DataInfo[tablename].Data.CurrentView;
                                    if (FCurrentDataView == null)
                                        throw new Exception("No datareader in table: " + tablename);
                                    isempty = FCurrentDataView.Count == 0;
                                }
                                else
                                {
                                    isempty = Report.DataInfo[tablename].Data.Eof;

                                    if (Report.UsedDataReaders.IndexOfKey(tablename) < 0)
                                        Report.UsedDataReaders.Add(tablename, tablename);
                                }

                                //bool isempty = Report.DataInfo[tablename].Data.Eof;
                                if ((idxunion == 0) || ParallelUnion)
                                {
                                    if (idxunion == 0)
                                        inttable = new DataTable();
                                    DataTable adatatable = null;
                                    if (FCurrentReader != null)
                                        adatatable = FCurrentReader.GetSchemaTable();
                                    else
                                    {
                                        adatatable = new DataTable();
                                        adatatable.Columns.Add("ColumnName", System.Type.GetType("System.String"));
                                        adatatable.Columns.Add("DataType", System.Type.GetType("System.Type"));
                                        object[] values = new object[2];
                                        foreach (DataColumn ncol in FCurrentDataView.Table.Columns)
                                        {
                                            values[0] = ncol.ColumnName;
                                            values[1] = ncol.DataType;
                                            adatatable.Rows.Add(values);
                                        }
                                    }
                                    DataColumn col;

                                    for (int i = 0; i < adatatable.Rows.Count; i++)
                                    {
                                        string acolname;
                                        string originalcolname;
                                        DataRow nrow = adatatable.Rows[i];
                                        acolname = nrow["ColumnName"].ToString().ToUpper();
                                        if (acolname.Length < 1)
                                            acolname = "Column" + i.ToString();
                                        originalcolname = acolname;
                                        if (sharedfieldslist.IndexOf(acolname) < 0)
                                            acolname = field_prefix + acolname;
                                        if (ParallelUnion)
                                            fieldsunion.Add(originalcolname, acolname);
                                        if (inttable.Columns.IndexOf(acolname) < 0)
                                        {
                                            col = inttable.Columns.Add(acolname, (Type)nrow["DataType"]);
                                            if ((col.DataType.ToString() == "System.String") && (FCurrentReader != null))
                                            {
                                                int maxlength = (int)nrow["ColumnSize"];
                                                col.MaxLength = maxlength;
                                            }
                                            col.Caption = acolname;
                                        }
                                    }
                                }
                                // Add rows
                                SortedList<int, int> fieldintunion = null;
                                int colcount = 0;
                                if (FCurrentReader != null)
                                    colcount = FCurrentReader.FieldCount;
                                else
                                    colcount = FCurrentDataView.Table.Columns.Count;
                                object[] nobject = new object[colcount];

                                if (ParallelUnion)
                                {
                                    // Create primary key for share union
                                    if (sharedfieldslist.Count > 0)
                                    {
                                        if (idxunion == 0)
                                        {
                                            DataColumn[] primcols = new DataColumn[sharedfieldslist.Count];
                                            int idxcol = 0;
                                            foreach (string primcolname in sharedfieldslist)
                                            {
                                                primcols[idxcol] = inttable.Columns[primcolname];
                                                primarycolumns.Add(primcolname);
                                                idxcol++;
                                            }
                                            primview = new DataView(inttable, "", sharedfieldslist.ToCharSeparated(','), DataViewRowState.CurrentRows);
                                            //inttable.Constraints.Add("PRIM" + inttable.TableName, primcols,true);
                                        }
                                    }
                                    fieldintunion = new SortedList<int, int>();


                                    if (nobject.Length != inttable.Columns.Count)
                                        nobject = new object[inttable.Columns.Count];
                                    if (FCurrentReader != null)
                                    {
                                        for (int idxreader = 0; idxreader < FCurrentReader.FieldCount; idxreader++)
                                        {
                                            fieldintunion.Add(idxreader, inttable.Columns.IndexOf(fieldsunion[FCurrentReader.GetName(idxreader)]));
                                        }
                                    }
                                    else
                                    {
                                        for (int idxreader = 0; idxreader < FCurrentDataView.Table.Columns.Count; idxreader++)
                                        {
                                            fieldintunion.Add(idxreader, inttable.Columns.IndexOf(fieldsunion[FCurrentDataView.Table.Columns[idxreader].ColumnName]));
                                        }
                                    }
                                }
                                object[] primkeys = null;
                                if (primarycolumns.Count > 0)
                                    primkeys = new object[primarycolumns.Count];
                                bool addrow = !isempty;
                                int secuentialindex = 0;
                                int dataviewindex = 0;
                                while (addrow)
                                {
                                    int xcol = 0;
                                    if (ParallelUnion)
                                    {
                                        int idxcolprim = 0;
                                        foreach (string colnameprim in sharedfieldslist)
                                        {
                                            if (FCurrentReader != null)
                                            {
                                                primkeys[idxcolprim] = FCurrentReader[colnameprim];
                                            }
                                            else
                                                primkeys[idxcolprim] = FCurrentDataView[dataviewindex][colnameprim];

                                            idxcolprim++;
                                        }
                                        //DataRow foundrow = inttable.Rows.Find(primkeys);
                                        DataRow foundrow = null;
                                        if (primview != null)
                                        {
                                            int index = primview.Find(primkeys);
                                            if (index >= 0)
                                                foundrow = primview[index].Row;
                                        }
                                        else
                                        {
                                            if (secuentialindex < inttable.Rows.Count)
                                                foundrow = inttable.Rows[secuentialindex];
                                        }

                                        if (foundrow == null)
                                        {
                                            if (FCurrentReader != null)
                                            {
                                                foreach (int xindex in fieldintunion.Keys)
                                                {
                                                    nobject[fieldintunion[xindex]] = FCurrentReader[xindex];
                                                }
                                            }
                                            else
                                            {
                                                foreach (int xindex in fieldintunion.Keys)
                                                {
                                                    nobject[fieldintunion[xindex]] = FCurrentDataView[dataviewindex][xindex];
                                                }
                                            }
                                            inttable.Rows.Add(nobject);
                                        }
                                        else
                                        {
                                            foundrow.BeginEdit();
                                            if (FCurrentReader != null)
                                            {
                                                foreach (int xindex in fieldintunion.Keys)
                                                {
                                                    foundrow[fieldintunion[xindex]] = FCurrentReader[xindex];
                                                }
                                            }
                                            else
                                            {
                                                foreach (int xindex in fieldintunion.Keys)
                                                {
                                                    foundrow[fieldintunion[xindex]] = FCurrentDataView[dataviewindex][xindex];
                                                }
                                            }
                                            foundrow.EndEdit();
                                        }
                                    }
                                    else
                                    {
                                        if (FCurrentReader != null)
                                        {
                                            while (xcol < FCurrentReader.FieldCount)
                                            {
                                                nobject[xcol] = FCurrentReader[xcol];
                                                xcol++;
                                            }
                                        }
                                        else
                                        {
                                            while (xcol < colcount)
                                            {
                                                nobject[xcol] = FCurrentDataView[dataviewindex][xcol];
                                                xcol++;
                                            }
                                        }
                                        inttable.Rows.Add(nobject);
                                    }
                                    secuentialindex++;
                                    dataviewindex++;
                                    if (FCurrentReader != null)
                                        addrow = FCurrentReader.Read();
                                    else
                                        addrow = dataviewindex < FCurrentDataView.Count;
                                }

                                idxunion++;
                            }
                            if (inttable != null)
                            {
                                Strings lsorting = Strings.FromSemiColon(this.MyBaseIndexFields);
                                Strings lnew = new Strings();
                                Strings nmaster = Strings.FromSemiColon(this.MyBaseMasterFields);
                                for (int ix = 0; ix < nmaster.Count; ix++)
                                    lnew.Add(lsorting[ix]);
                                string sortingstring = lnew.ToCharSeparated(',');
                                DataView intview = null;

                                // Resort table for master/child relations, correctly sorted
                                if (MyBaseIndexFields.Length > 0)
                                {
                                    Strings nsort = Strings.FromSemiColon(this.MyBaseIndexFields);
                                    string nsorting = nsort.ToCharSeparated(',');
                                    intview = new DataView(inttable, "", nsorting, DataViewRowState.CurrentRows);
                                    DataTable oldtable = inttable;
                                    inttable = oldtable.Clone();
                                    object[] nobj = new object[oldtable.Columns.Count];
                                    foreach (DataRowView xv in intview)
                                    {
                                        for (int idx = 0; idx < nobj.Length; idx++)
                                        {
                                            nobj[idx] = xv[idx];
                                        }
                                        inttable.Rows.Add(nobj);
                                    }
                                    intview.Dispose();
                                    oldtable.Dispose();
                                    intview = new DataView(inttable, "", sortingstring, DataViewRowState.CurrentRows);
                                }
                                else
                                    intview = new DataView(inttable, "", sortingstring, DataViewRowState.CurrentRows);


                                intdataset.Tables.Add(inttable);
                                Data.CurrentView = intview;

                                if (mastersource != null)
                                {
                                    UpdateParams(mastersource.Data, Command);
                                    Data.ViewFilter = mastervalues;

                                    mastersource.Data.OnDataChange += new DataChange(OnDataChange);
                                }

                            }
                        }
                    }
#endif
                }
                else
                {
                    IDataReader areader;
                    if (dbitem.SqlExecuter == null)
                        Command = dbitem.Connection.CreateCommand();
                    else
#if NETSTANDARD2_0
                    Command = dbitem.Connection.CreateCommand();
#else
                    Command = new System.Data.SqlClient.SqlCommand();
#endif
                    Command.Transaction = dbitem.CurrentTransaction;
                    string sqlsentence;
                    if (SQLOverride.Length != 0)
                        sqlsentence = SQLOverride;
                    else
                        sqlsentence = SQL;
                    // Assign parameters, string substitution only
                    for (int i = 0; i < aparams.Count; i++)
                    {
                        Param aparam = aparams[i];
                        switch (aparam.ParamType)
                        {
                            case ParamType.Subst:
                            case ParamType.Multiple:
                                int index = aparam.Datasets.IndexOf(Alias);
                                if (index >= 0)
                                    sqlsentence = sqlsentence.Replace(aparam.Search, aparam.Value);
                                break;
                            case ParamType.SubstExpre:
                            case ParamType.SubsExpreList:
                                int index2 = aparam.Datasets.IndexOf(Alias);
                                if (index2 >= 0)
                                {

                                    string nvalue = aparam.LastValue.ToString();
                                    //        GetEvaluator().Expression = nvalue;
                                    //      nvalue = GetEvaluator().Evaluate().ToString();

                                    sqlsentence = sqlsentence.Replace(aparam.Search, nvalue);
                                }
                                break;
                        }
                    }

                    Command.CommandText = sqlsentence;
                    OldSQLUsed = sqlsentence;
                    // Assign parameters
                    for (int i = 0; i < aparams.Count; i++)
                    {
                        Param aparam = aparams[i];
                        int index = aparam.Datasets.IndexOf(Alias);
                        if (index >= 0)
                        {
                            if ((aparam.ParamType != ParamType.Subst) &&
                             (aparam.ParamType != ParamType.Multiple) && (aparam.ParamType != ParamType.SubsExpreList)
                                 && (aparam.ParamType != ParamType.SubstExpre))
                            {
                                System.Data.IDataParameter dbparam = Command.CreateParameter();
                                //dbparam.ParameterName = "@" + aparam.Alias;
                                // SQL Server does not like "@" prefix
                                dbparam.ParameterName = aparam.Alias;
                                dbparam.Direction = ParameterDirection.Input;
                                dbparam.DbType = aparam.Value.GetDbType();
                                dbparam.Value = aparam.LastValue.AsObject();
                                Command.Parameters.Add(dbparam);
                            }
                        }
                    }
                    if (mastersource != null)
                    {
                        UpdateParams(mastersource.Data, Command);
                        mastersource.Data.OnDataChange += new DataChange(OnDataChange);
                    }
                    if (dbitem.SqlExecuter != null)
                    {
                        Data.CurrentView = new DataView(dbitem.SqlExecuter.Open(Command));
                    }
                    else
                    {
                        areader = Command.ExecuteReader();
                        DataReader = areader;
                        Data.CurrentReader = DataReader;
                    }

                }

            }
            finally
            {
                connecting = false;
            }
        }
        private void OnDataChange(ReportDataset master)
        {
            if (UpdateParams(master, Command))
            {
                if ((Data.CurrentView != null) && (masterfieldslist != null))
                {
                    Data.ViewFilter = mastervalues;
                }
                else
                {
                    if (DataReader != null)
                    {
                        DataReader.Close();
                        DataReader = null;
                    }
                    DatabaseInfo dbitem = GetDbItem();

                    if (dbitem.SqlExecuter != null)
                    {
                        Data.CurrentView = new DataView(dbitem.SqlExecuter.Open(Command));
                    }
                    else
                    {
                        IDataReader areader = Command.ExecuteReader();
                        DataReader = areader;
                        Data.UpdateColumns = (OldSQLUsed != Command.CommandText);
                        Data.CurrentReader = areader;
                    }
                }
            }
        }
        /// <summary>
        /// Update the parameters inside the command, the master dataset will be used to fill the parameters
        /// with matching names
        /// </summary>
        /// <param name="master">Master dataset</param>
        /// <param name="ncommand">Command</param>
        /// <returns></returns>
		public bool UpdateParams(ReportDataset master, IDbCommand ncommand)
        {
            if ((Data.CurrentView != null) && (masterfieldslist != null))
            {
                bool nresult = false;
                int idx = 0;
                foreach (string nmasterf in masterfieldslist)
                {
                    object nvalue = DBNull.Value;
                    if (!master.Eof)
                    {
                        nvalue = master.CurrentRow[nmasterf];
                    }
                    if (!nvalue.Equals(mastervalues[idx]))
                    {
                        nresult = true;
                        mastervalues[idx] = nvalue;
                    }
                    idx++;
                }
                return nresult;
            }
            else
            {
                bool aresult = false;
                for (int i = 0; i < ncommand.Parameters.Count; i++)
                {
                    IDbDataParameter param = (IDbDataParameter)ncommand.Parameters[i];
                    string paramname = param.ParameterName;
                    if (paramname.Length > 0)
                    {
                        if (paramname[0] == '@')
                            paramname = param.ParameterName.Substring(1, param.ParameterName.Length - 1);
                        int index = master.Columns.IndexOf(paramname);
                        if (index >= 0)
                        {
                            if (master.Eof)
                            {
                                if (param.Value != null)
                                    aresult = true;
                                param.Value = null;
                            }
                            else
                                if (!master.CurrentRow[paramname].Equals(param.Value))
                            {
                                aresult = true;
                                param.Value = master.CurrentRow[paramname];
                            }
                        }
                    }
                }
                return aresult;
            }
        }
        /// <summary>
        /// Close the dataset, and free the datareader
        /// </summary>
		public void DisConnect()
        {
            if (DataReader != null)
            {
                DataReader.Close();
                Command = null;
                DataReader = null;
                Data.CurrentReader = null;
            }
            if (Data.CurrentView != null)
            {
                Data.CurrentView = null;
            }
            if (Data.CurrentReader != null)
            {
                Data.CurrentReader = null;
            }
        }
        public void DisConnectMem()
        {

            if (Data != null)
            {
                if (Data.CurrentView != null)
                    Data.CurrentView = null;
            }
        }
    }
}
