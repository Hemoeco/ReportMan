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
using Reportman.Drawing;
using System.Data;
using System.Collections;
using System.Data.Common;
using System.Collections.Generic;

namespace Reportman.Reporting
{
    /// <summary>
    /// A dataset definition could represent a table inside a database or
    /// a sql query, in .Net version, it's always a Query. <see cref="Variant">DataInfo</see>
    /// </summary>
    public enum DatasetType {
        /// <summary>The DataInfo will open (execute) a query</summary>
        Query,
        /// <summary>The DataInfo will open a table inside a database</summary>
        Table
    };
    /// <summary>
    /// The DriverType is part of a report database information definition, indicating the technology to be used
    /// to connect to the database, in .Net version you can choose DotNet or DotNet2 depending on the version you 
    /// are using. <see cref="Variant">DatabaseInfo</see>
    /// </summary>
    public enum DriverType
	{
        /// <summary>Borland database driver optimized for using disconnected DataSets. Not available in .Net</summary>
        DBExpress = 0,
        /// <summary>In memory database driver, it can be used to read DataSets into memory for further processing</summary>
        /// <remarks>When used in .Net it's capable of reading DataSets</remarks>
        Mybase = 1,
        /// <summary>Borland Interbase Express database driver, can access also Firebird databases.
        /// Not available in .Net</summary>
        IBX = 2,
        /// <summary>Borland Database Engine database driver, can access Paradox,Dbase, and some SQL Databases in native mode.
        /// Not available in .Net</summary>
        BDE = 3,
        /// <summary>Borland Interface to Microsoft DAO, can access any OleDB (including Microsoft Jet) or ODBC databases.
        /// Not available in .Net</summary>
        ADO = 4,
        /// <summary>Database access objects to access Interbase or 4rd databases known as Interbase Objects.
        /// Not available in .Net</summary>
        IBO = 5,
        /// <summary>Opensource Database Objects to access multiple sql database technologies (zeoslib).
        /// Not available in .Net</summary>
        ZEOS = 6,
        /// <summary>Driver used to connect to databases in .Net 1.x, because no abstraction is provided in .Net 1.x, 
        /// you must select alse the driver (object instances) to be used. <see cref="Variant">DotNetDriverType</see>
        /// Available only in .Net 1.x</summary>
        DotNet = 7,
        /// <summary>Driver used to connect to databases in .Net 2.x, it's used with ProviderFactory property inside
        /// DatabaseInfo to perform the connection<see cref="Variant">DatabaseInfo</see>
        /// Available only in .Net 2.x</summary>
        DotNet2 = 8
    };
    /// <summary>
    /// The DotNetDriverType indicates the database provider to be used when you select DriverType.DotNet, that
    /// is .Net 1.x, in this versions of .Net there were no abstraction for Database access, so you must select
    /// and pre-link into your executable references to the database client library. 
    /// This type is not used in .Net 2.x because the functionality of database providers, 
    /// configured in machine.config or application.config<see cref="Variant">DatabaseInfo</see>
    /// </summary>
    public enum DotNetDriverType
	{
        /// <summary>OleDb .Net 1.x Data Provider</summary>
		OleDb,
        /// <summary>Odbc .Net 1.x Data Provider</summary>
        Odbc,
        /// <summary>Firebird for .Net 1.x Data Provider</summary>
        Firebird,
        /// <summary>SqlServer .Net 1.x Data Provider</summary>
        Sql,
        /// <summary>PostgreSql .Net 1.x Data Provider</summary>
        PostgreSql,
        /// <summary>MySql .Net 1.x Data Provider</summary>
        MySql,
        /// <summary>SQLite .Net 1.x Data Provider</summary>
        SQLite,
        /// <summary>Oracle .Net 1.x Data Provider</summary>
        Oracle,
        /// <summary>DB2 .Net 1.x Data Provider</summary>
        DB2,
        /// <summary>Sybase .Net 1.x Data Provider</summary>
        Sybase,
        /// <summary>SqlCE .Net 1.x Data Provider</summary>
        SqlCE
	};
    /// <summary>
    /// DatabaseInfo stores information about database connectivity, a Report have a collection
    /// of connection definitions. Each connection can use diferent connectivity technology 
    /// (database providers). <see cref="Variant">Report</see><see cref="Variant">DatabaseInfos</see>
    /// </summary>
	public class DatabaseInfo : ReportItem,ICloneable
	{
        public static string FIREBIRD_PROVIDER = "Firebird.Data.FirebirdClient";
        public static string FIREBIRD_PROVIDER2 = "FirebirdSql.Data.Firebird";
        public static string MYSQL_PROVIDER = "MySql.Data.MySqlClient";
        public static string SQLITE_PROVIDER = "System.Data.SQLite";
        private System.Data.IDbConnection FConnection;
        private System.Data.IDbConnection FExternalConnection;
        private System.Data.IDbTransaction IntTransaction;
        /// <summary>
        /// Obtain current active transaction to execute querys
        /// </summary>
        public System.Data.IDbTransaction CurrentTransaction
        {
            get
            {
                if (Transaction != null)
                    return Transaction;
                else
                    return IntTransaction;
            }
        }
        public static List<string> GetDriverDescriptions()
        {
            List<string> alist = new List<string>
            {
                "Borland DBExpress",
                "B.MyBase and text files",
                "Interbase Express",
                "Borland Database Engine",
                "Microsoft DAO",
                "Interbase Objects",
                "Zeos Database Objects",
                "Dot Net Connection",
                "Dot Net 2 Connection"
            };
            return alist;
        }
        /// <summary>
        /// You can assign a transaction, so this transaction will be used instead a default created one
        /// </summary>
        public System.Data.IDbTransaction Transaction;
        /// <summary>
        /// You can assign a Connection, so this connection will be used, if you provide a connection, 
        /// all the settings inside DatabaseInfo related to perform connection will be ignored and all
        /// querys will be executed against the provided connection
        /// </summary>
        public System.Data.IDbConnection Connection
        {
            get
            {
                if (FExternalConnection != null)
                    return FExternalConnection;
                else
                    return FConnection;
            }
            set
            {
                if (value == null)
                {
                    FConnection = null;
                    FExternalConnection = null;
                }
                else
                {
                    if (FConnection != value)
                    {
                        FExternalConnection = value;
                    }
                }
            }
        }
        public IDbCommandExecuter SqlExecuter;
        protected override string GetClassName()
        {
            return "TRPDATABASEINFOITEM";
        }
        /// <summary>
        /// Clone the DatabaseInfo item
        /// </summary>
        /// <returns>A new DatabaseInfo item with same data as the original</returns>
		public object Clone()
		{
			DatabaseInfo ninfo=new DatabaseInfo(Report);
			ninfo.Alias=Alias;
			ninfo.Driver=Driver;
			ninfo.ProviderFactory=ProviderFactory;
			ninfo.ReportTable=ReportTable;
			ninfo.ReportSearchField=ReportSearchField;
			ninfo.Name=Name;
			ninfo.ReportField=ReportField;
			ninfo.ReportGroupsTable=ReportGroupsTable;
			ninfo.ConnectionString=ConnectionString;
            ninfo.FExternalConnection = FExternalConnection;
            ninfo.Transaction = Transaction;
            ninfo.DotNetDriver = DotNetDriver;
            ninfo.TransIsolation = TransIsolation;
			return ninfo;
		}			
        /// <summary>
        /// Clone the DatabaseInfo item
        /// </summary>
        /// <param name="areport">Report to assign to the item</param>
        /// <returns>A new DatabaseInfo item with same data as the original</returns>
        public DatabaseInfo Clone(Report areport)
		{
			DatabaseInfo ninfo=(DatabaseInfo)Clone();
			ninfo.Report=areport;
			return ninfo;
		}			
        /// <summary>DatabaseInfo item name</summary>
		public string Alias;
        /// <summary>DatabaseInfo driver type</summary>
        public DriverType Driver;
        /// <summary>Provider factory, only for .Net 2.x driver</summary>
        public string ProviderFactory;
        /// <summary>Report table name when loading report items from a connection</summary>
        public string ReportTable;
        /// <summary>Report search field when loading report items from a connection</summary>
        public string ReportSearchField;
        /// <summary>Report field when loading report items from a connection</summary>
        public string ReportField;
        /// <summary>Report groups table when loading report items from a connection</summary>
        public string ReportGroupsTable;
        /// <summary>Connection string, for ADO and .Net drivers</summary>
        public string ConnectionString;
        /// <summary>DotNet driver type</summary>
        public DotNetDriverType DotNetDriver;
        /// <summary>
        /// Default isolation level if a transaction have not assigned, all the querys related
        /// to this connection will run inside the same transaction
        /// </summary>
        public System.Data.IsolationLevel TransIsolation;
        /// <summary>
        /// Constructor
        /// </summary>
		public DatabaseInfo(BaseReport rp)
			: base(rp)
		{
			Driver = DriverType.DotNet2;
			ReportTable = "REPMAN_REPORTS";
			ReportGroupsTable = "REPMAN_GROUPS";
			ReportSearchField = "REPORT_NAME";
			ReportField = "REPORT";
			Alias = "";
			this.ProviderFactory = "";
			ReportTable = ""; ReportSearchField = ""; ReportField = ""; ReportGroupsTable = "";
			ConnectionString = "";
			//TransIsolation = System.Data.IsolationLevel.ReadCommitted;
           	TransIsolation = System.Data.IsolationLevel.RepeatableRead;
        }
        /// <summary>
        /// Disconnect from database, also dispose any transaction
        /// </summary>
		public void DisConnect()
		{
            if (DriverType.Mybase == Driver)
                return;
            if (Connection != null)
			{
				if (IntTransaction != null)
				{
					IntTransaction.Commit();
                    IntTransaction.Dispose();
					IntTransaction = null;
				}
                if (FConnection != null)
                {
                    FConnection.Close();
                    FConnection.Dispose();
                    FConnection = null;
                }
			}
		}
        public static SortedList<string, DbProviderFactory> CustomProviderFactories = new SortedList<string, DbProviderFactory>();
        /// <summary>
        /// Connect to the database
        /// </summary>
        public void Connect()
		{
            if (DriverType.Mybase == Driver)
                return;
            if (SqlExecuter != null)
                return;
                
            if (Connection != null)
            {
                if (Transaction == null)
                    if (IntTransaction==null)
                        IntTransaction = Connection.BeginTransaction(TransIsolation);
                return;
            }
			string UsedConnectionString=ConnectionString;
			int index=Report.Params.IndexOf("ADOCONNECTIONSTRING");
			if (index>0)
				UsedConnectionString=Report.Params[index].Value.ToString();
            index = Report.Params.IndexOf(Alias+"_ADOCONNECTIONSTRING");
            if (index > 0)
                UsedConnectionString = Report.Params[index].Value.ToString();
			
            if (Driver == DriverType.IBX)
                this.ProviderFactory = FIREBIRD_PROVIDER;
			if (this.ProviderFactory.Length == 0)
				throw new UnNamedException("Provider factory not supplied");
            DbProviderFactory afactory = null;
            if (CustomProviderFactories.IndexOfKey(ProviderFactory) >= 0)
                afactory = CustomProviderFactories[ProviderFactory];
            if (afactory == null)
#if NETSTANDARD2_0
                throw new Exception("You must provide in .netstandard a DatabaseInfo.CustomProviderFactory for the name: " + ProviderFactory);
#else
                afactory = DbProviderFactories.GetFactory(this.ProviderFactory);
#endif
            if (afactory==null)
				throw new NamedException("System.Data.Common.DbProviderFactories Factory not found:" + this.ProviderFactory.ToString(), DotNetDriver.ToString());
			DbConnection aconnection=null;
			aconnection = afactory.CreateConnection();
			aconnection.ConnectionString = UsedConnectionString;
			aconnection.Open();
			FConnection = aconnection;
            if (Transaction==null)
                IntTransaction = FConnection.BeginTransaction(TransIsolation);
/*			switch (DotNetDriver)
			{
#if REPMAN_OLEDB
				case DotNetDriverType.OleDb:
					System.Data.OleDb.OleDbConnection OleDbConn;
					OleDbConn = new OleDbConnection(UsedConnectionString);
					OleDbConn.Open();
					FConnection = OleDbConn;
					break;
#endif
#if REPMAN_ODBC
				case DotNetDriverType.Odbc:
					System.Data.Odbc.OdbcConnection OdbcConn;
					OdbcConn = new OdbcConnection(UsedConnectionString);
					OdbcConn.Open();
					FConnection = OdbcConn;
					break;
#endif
#if REPMAN_FIREBIRD
				case DotNetDriverType.Firebird:
					FbConnection fbconn = new FbConnection(UsedConnectionString);
					fbconn.Open();
					FConnection = fbconn;
					break;
#endif
#if REPMAN_ORACLE
				case DotNetDriverType.Oracle:
					OracleConnection oraconn = new OracleConnection(UsedConnectionString);
					oraconn.Open();
					FConnection = oraconn;
					break;
#endif
#if REPMAN_POSTGRESQL
				case DotNetDriverType.PostgreSql:
					Npgsql.NpgsqlConnection pgsqlconn=new Npgsql.NpgsqlConnection(UsedConnectionString);
					pgsqlconn.Open();
					FConnection=pgsqlconn;
					break;
#endif
#if REPMAN_SQLITE
				case DotNetDriverType.SQLite:
					Mono.Data.SqliteClient.SqliteConnection sqliconn=new Mono.Data.SqliteClient.SqliteConnection(UsedConnectionString);
					sqliconn.Open();
					FConnection=sqliconn;
					break;
#endif
#if REPMAN_SYBASE
				case DotNetDriverType.Sybase:
					Mono.Data.SybaseClient.SybaseConnection sybaseconn=new Mono.Data.SybaseClient.SybaseConnection(UsedConnectionString);
					sybaseconn.Open();
					FConnection=sybaseconn;
					break;
#endif
#if REPMAN_DB2
				case DotNetDriverType.DB2:
					IBM.Data.DB2.DB2Connection db2conn=new IBM.Data.DB2.DB2Connection(UsedConnectionString);
					db2conn.Open();
					FConnection=db2conn;
					break;
#endif
#if REPMAN_MYSQL
				case DotNetDriverType.MySql:
					MySql.Data.MySqlClient.MySqlConnection myconn=new MySql.Data.MySqlClient.MySqlConnection(UsedConnectionString);
					myconn.Open();
					FConnection=myconn;
					break;
#endif
#if REPMAN_SQL
				case DotNetDriverType.Sql:
					SqlConnection sqconn = new SqlConnection(UsedConnectionString);
					sqconn.Open();
					FConnection = sqconn;
					break;
#endif
#if REPMAN_SQLCE
				case DotNetDriverType.SqlCE:
					SqlCeConnection sqceconn=new SqlCeConnection(UsedConnectionString);
					sqceconn.Open();
					FConnection=sqceconn;
					break;
#endif
				default:
					throw new NamedException("Database driver not supported:" + DotNetDriver.ToString(),DotNetDriver.ToString());
			}
#if NETSTANDARD2_0
#else
            if (Connection != null)
				Transaction = Connection.BeginTransaction(TransIsolation);
#endif*/
		}
        /// <summary>
        /// Obrains a IDataReader from a sql sentence
        /// </summary>
        /// <param name="sqlsentence">A valid sql sentence</param>
        /// <param name="dataalias">Parameters related to this alias will be used</param>
        /// <param name="aparams">Report parameters</param>
        /// <param name="onlyexec">Execute only, not open the query</param>
        /// <returns>A valid IDataReader</returns>
		public IDataReader GetDataReaderFromSQL(string sqlsentence, string dataalias,
			Params aparams, bool onlyexec)
		{
			IDataReader adatareader = null;
			Connect();
			IDbCommand Command = Connection.CreateCommand();
			Command.Transaction = Transaction;

			// Assign parameters, string substitution only
			for (int i = 0; i < aparams.Count; i++)
			{
				int index;
				Param aparam = aparams[i];
				switch (aparam.ParamType)
				{
					case ParamType.Subst:
					case ParamType.Multiple:
						index = aparam.Datasets.IndexOf(dataalias);
						if (index >= 0)
							sqlsentence = sqlsentence.Replace(aparam.Search, aparam.Value);
						break;
					case ParamType.SubstExpre:
                    case ParamType.SubsExpreList:
                        int index2 = aparam.Datasets.IndexOf(Alias);
                        if (index2 >= 0)
                        {
                            using (Evaluator eval = new Evaluator())
                            {
                                string nvalue = aparam.Value.ToString();
                                eval.Expression = nvalue;
                                nvalue = eval.Evaluate().ToString();

                                sqlsentence = sqlsentence.Replace(aparam.Search, nvalue);
                            }
                        } 
                        break;                        
				}
			}
			Command.CommandText = sqlsentence;
			// Assign parameters
			for (int i = 0; i < aparams.Count; i++)
			{
				Param aparam = aparams[i];
				int index = aparam.Datasets.IndexOf(dataalias);
				if (index >= 0)
				{
					if ((aparam.ParamType != ParamType.Subst) &&
					 (aparam.ParamType != ParamType.Multiple) &&
                     (aparam.ParamType != ParamType.SubstExpre) && (aparam.ParamType != ParamType.SubsExpreList))
					{
						System.Data.IDataParameter dbparam = Command.CreateParameter();
						dbparam.ParameterName = "@" + aparam.Alias;
						dbparam.Direction = ParameterDirection.Input;
						dbparam.DbType = aparam.Value.GetDbType();
						dbparam.Value = aparam.Value;
						Command.Parameters.Add(dbparam);
					}
				}
			}
			adatareader = Command.ExecuteReader();
			return adatareader;
		}
	}

}
