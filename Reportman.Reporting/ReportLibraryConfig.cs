using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reportman.Drawing;
using System.Data.Common;

namespace Reportman.Reporting
{
    public class ReportLibraryConfig
    {
        public ReportLibraryConfig()
        {
            AllowSorting = false;
        }
        public string ReportTable { get; set; } = "REPMAN_REPORTS";
        public string ReportField { get; set; } = "REPORT";
        public string ReportSearchField { get; set; } = "REPORT_NAME";
        public string ReportGroupsTable { get; set; } = "REPMAN_GROUPS";
        public bool AllowSorting { get; set; }
        public string ADOConnectionString { get; set; } = "";
        public bool LoadParams { get; set; } = true;
        public bool LoadDriverParams { get; set; } = true;
        public bool LoginPrompt { get; set; } = false;
        public string Alias { get; set; } = "";
        
        public int DriverIndex
        {
            get
            {
                return (int)Driver;
            }
            set
            {
                Driver = (DriverType)value;
            }
        }
        public DriverType Driver
        {
            get; set; } = DriverType.DotNet2;
        public string ProviderName { get; set; } = "";
        public DbConnection CurrentConnection;
        public override string ToString()
        {
            return Alias;
        }
        public static string GetConfigFilename()
        {
            string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
            configPath = System.IO.Path.Combine(configPath, "repmandlib.ini");

            return configPath;
        }
        public DbProviderFactory GetFactory()
        {
            // Search for custom provided factories
            int idx = DatabaseInfo.CustomProviderFactories.IndexOfKey(ProviderName);
            if (idx>=0)
            {
                return DatabaseInfo.CustomProviderFactories[ProviderName];
            }
            switch (Driver)
            {
                case DriverType.IBX:
                    
                    if (DatabaseInfo.CustomProviderFactories.IndexOfKey(DatabaseInfo.FIREBIRD_PROVIDER) >=0)
                        return DatabaseInfo.CustomProviderFactories[DatabaseInfo.FIREBIRD_PROVIDER];
                    else
                    if (DatabaseInfo.CustomProviderFactories.IndexOfKey(DatabaseInfo.FIREBIRD_PROVIDER2) >= 0)
                        return DatabaseInfo.CustomProviderFactories[DatabaseInfo.FIREBIRD_PROVIDER2];
                    else
                        throw new Exception("Firebird provider not found: " + DatabaseInfo.FIREBIRD_PROVIDER);
                case DriverType.DotNet2:
                    DbProviderFactory factory = DbProviderFactories.GetFactory(ProviderName);
                    if (factory == null)
                        throw new Exception("Can not find provider factory: " + ProviderName);
                    else
                        return factory;
                default:
                    throw new Exception("Driver not supported: " + Driver.ToString());
            }
        }

        public System.IO.MemoryStream ReadReport(string reportName)
        {
            System.IO.MemoryStream result = null;
            string sqltext = "SELECT " + ReportField + " FROM " + ReportTable +
                 " WHERE " + ReportSearchField + "=" + StringUtil.QuoteStr(reportName);
            bool closeConnection = false;
            if (CurrentConnection == null)
            {
                DbProviderFactory factory = GetFactory();
                DbConnection connection = factory.CreateConnection();
                connection.ConnectionString = ADOConnectionString;
                connection.Open();
                CurrentConnection = connection;
                closeConnection = true;
            }
            try
            {
                using (DbCommand command = CurrentConnection.CreateCommand())
                {
                    command.CommandText = sqltext;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new Exception(Translator.TranslateStr(357));
                        object value = reader[0];
                        result = new System.IO.MemoryStream();
                        if (value != DBNull.Value)
                        {
                            byte[] valorb = (byte[])value;
                            result.Write(valorb, 0, valorb.Length);
                            result.Seek(0, System.IO.SeekOrigin.Begin);
                        }
                    }
                }
            }
            finally
            {
                if (closeConnection)
                {
                    DbConnection connection = CurrentConnection;
                    CurrentConnection = null;
                    connection.Close();
                }
            }
            return result;
        }
        public System.IO.MemoryStream SaveReport(Report nreport,string reportName)
        {
            string sqltext = "UPDATE " + ReportTable +" SET "+ReportField + "=@REPORT" +
                 " WHERE " + ReportSearchField + "=" + StringUtil.QuoteStr(reportName);
            bool closeConnection = false;
            System.IO.MemoryStream newStream = new System.IO.MemoryStream();
            nreport.SaveToStream(newStream);
            if (CurrentConnection == null)
            {
                DbProviderFactory factory = GetFactory();
                DbConnection connection = factory.CreateConnection();
                connection.ConnectionString = ADOConnectionString;
                connection.Open();
                CurrentConnection = connection;
                closeConnection = true;
            }
            try
            {
                using (DbCommand command = CurrentConnection.CreateCommand())
                {
                    command.CommandText = sqltext;
                    var transaction = CurrentConnection.BeginTransaction();
                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected != 1)
                            throw new Exception(Translator.TranslateStr(357));
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            finally
            {
                if (closeConnection)
                {
                    DbConnection connection = CurrentConnection;
                    CurrentConnection = null;
                    connection.Close();
                }
            }
            return newStream;
        }
    }
    public class ReportLibrarySelection
    {
        public ReportLibraryConfig ReportLibrary;
        public string ReportName;
        public System.IO.MemoryStream Stream;
        public void Save(Report nreport)
        {
            Stream = ReportLibrary.SaveReport(nreport, ReportName);
        }
    }
    public class ReportLibraryConfigCollection : List<ReportLibraryConfig>
    {
        public void LoadFromFile(string filename)
        {
            IniFile inif = new IniFile(filename);
            Clear();
            int concount = inif.ReadInteger("REPMAN_CONNECTIONS", "COUNT", 0);
            for (int i = 0; i < concount; i++)
            {
                string conname = "REPMAN_CONNECTION" + i.ToString();
                string aname = inif.ReadString(conname, "NAME", "CONNECTION" + i.ToString());
                ReportLibraryConfig aitem = new ReportLibraryConfig();
                aitem.Alias = aname;
                Add(aitem);
                aitem.ADOConnectionString = inif.ReadString(conname, "ADOSTRING", aitem.ADOConnectionString);
                aitem.LoadParams = inif.ReadBool(conname, "LOADPARAMS", aitem.LoadParams);
                aitem.LoadDriverParams = inif.ReadBool(conname, "LOADDRIVERPARAMS", aitem.LoadDriverParams);
                aitem.LoginPrompt = inif.ReadBool(conname, "LOGINPROMPT", aitem.LoginPrompt);
                aitem.Driver = (DriverType)inif.ReadInteger(conname, "DRIVER", (int)aitem.Driver);
                aitem.ReportTable = inif.ReadString(conname, "REPORTTABLE", aitem.ReportTable);
                aitem.ReportField = inif.ReadString(conname, "REPORTFIELD", aitem.ReportField);
                aitem.ReportSearchField = inif.ReadString(conname, "REPORTSEARCHFIELD", aitem.ReportSearchField);
                aitem.ReportGroupsTable = inif.ReadString(conname, "REPORTGROUPSTABLE", aitem.ReportGroupsTable);
                aitem.ProviderName = inif.ReadString(conname, "PROVIDERFACTORY", "");
            }
        }
        public void SaveToFile(string filename)
        {
            IniFile inif = new IniFile(filename);
            inif.WriteInteger("REPMAN_CONNECTIONS", "COUNT", Count);
            for (int i = 0; i < Count; i++)
            {
                ReportLibraryConfig aitem = this[i];
                string conname = "REPMAN_CONNECTION" + i.ToString();
                inif.WriteString(conname, "NAME", aitem.Alias);
                inif.WriteString(conname, "ADOSTRING", aitem.ADOConnectionString);
                inif.WriteBool(conname, "LOADPARAMS", aitem.LoadParams);
                inif.WriteBool(conname, "LOADDRIVERPARAMS", aitem.LoadDriverParams);
                inif.WriteBool(conname, "LOGINPROMPT", aitem.LoginPrompt);
                inif.WriteInteger(conname, "DRIVER", (int)aitem.Driver);
                inif.WriteString(conname, "REPORTTABLE", aitem.ReportTable);
                inif.WriteString(conname, "REPORTFIELD", aitem.ReportField);
                inif.WriteString(conname, "REPORTSEARCHFIELD", aitem.ReportSearchField);
                inif.WriteString(conname, "REPORTGROUPSTABLE", aitem.ReportGroupsTable);
                inif.WriteString(conname, "PROVIDERFACTORY", aitem.ProviderName);
            }
            inif.SaveToFile(filename);
        }
    }
}
