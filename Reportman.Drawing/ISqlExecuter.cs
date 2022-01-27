using System.Data;

namespace Reportman.Drawing
{
    public interface IDbCommandExecuter
    {
        DataTable Open(IDbCommand ncommand);
    }
    public delegate void ISqlExecuterProgressEvent(int current, int total);
    public delegate void ISqlExecuterPartialFillEvent(object sender, ISqlExecuterPartialFillArgs args);
    public interface ISqlExecuter
    {
        int ExecuteInmediate(string sql);
        void Execute(string sql);
        void Execute(System.Data.Common.DbCommand ncommand);
        System.Data.Common.DbCommand CreateCommand(string cadsql);
        void StartTransaction(IsolationLevel nisolation);
        void Commit();
        void Rollback();
        void RollbackInmediate();
        DataTable OpenInmediate(DataSet ndataset, string sql, string tablename);
        void Open(DataSet ndataset, string sql, string tablename);
        void Open(DataSet ndataset, System.Data.Common.DbCommand command, string tablename);
        void Open(DataSet ndataset, string sql, string tablename, int maxrecords, ISqlExecuterPartialFillEvent eventpartial);
        void BeginInsertBlock();
        void EndInsertBlock();
        void Flush();
        void Flush(ISqlExecuterProgressEvent pgevent);
        long GetGenerator(string generatorName, int increment);
        object GetValueFromSql(string sql);
        void AddExternalColumnsToLastCommand(string externalcolumns, string deletes);
        void AddCustomOperation(int operation, string data, byte[] binarydata);
        void Connect();
        void Disconnect();
    }
    public class ISqlExecuterPartialFillArgs
    {
        public int TotalCount;
        public DataTable Table;
        public ISqlExecuterPartialFillArgs(int nTotalCount, DataTable nTable)
        {
            TotalCount = nTotalCount;
            Table = nTable;
        }
    }

}
