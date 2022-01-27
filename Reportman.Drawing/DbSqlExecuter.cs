using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Reportman.Drawing;
using System.Data.Common;


namespace Reportman.Drawing
{

    public class DbSqlExecuter:ISqlExecuter
    {
        enum SQType { Execute, Open,StartTransaction,Commit,Rollback };
        DataTable commands;
        DataTable commandparams;
        DataView vcommandparams;
        SortedList<int, List<object[]>> objparamvalues = new SortedList<int, List<object[]>>();
        object[] nvalues = new object[9];
        object[] pvalues = new object[4];
        DbCommand commandgen;
        DbCommand command_execute;
        public DbTransaction transaction;
        DbConnection connection;
        DbDataAdapter dataadapter;
        DbProviderFactory factory;
        public DbSqlExecuter(DbConnection nconnection,DbProviderFactory nfactory)
        {
            factory = nfactory;
            connection = nconnection;

            commands = new DataTable("COMMANDS");
            commands.Columns.Add("ID", System.Type.GetType("System.Int32"));
            commands.Columns.Add("TYPE", System.Type.GetType("System.Int32"));
            commands.Columns.Add("SQL", System.Type.GetType("System.String"));
            commands.Columns.Add("DATASET", System.Type.GetType("System.Object"));
            commands.Columns.Add("TABLENAME", System.Type.GetType("System.String"));
            commands.Columns.Add("MAXRECORDS", System.Type.GetType("System.Int32"));
            commands.Columns.Add("EVENTOBJECT", System.Type.GetType("System.Object"));
            commands.Columns.Add("PARAMS", System.Type.GetType("System.Int32"));
            commands.Columns.Add("INSERT_BLOCK", System.Type.GetType("System.Int32"));

            commandparams = new DataTable("COMMAND_PARAMS");
            commandparams.Columns.Add("ID", System.Type.GetType("System.Int32"));
            commandparams.Columns.Add("NAME", System.Type.GetType("System.String"));
            commandparams.Columns.Add("TYPE", System.Type.GetType("System.Int32"));
            commandparams.Columns.Add("VALUE", System.Type.GetType("System.Object"));

            vcommandparams = new DataView(commandparams, "", "ID", System.Data.DataViewRowState.CurrentRows);
            commandgen = factory.CreateCommand();
            commandgen.Connection = connection;
            command_execute = factory.CreateCommand();
            command_execute.Connection = connection;

            dataadapter = factory.CreateDataAdapter();
            dataadapter.SelectCommand = commandgen;
        }
        int CurrentInsertBlock = 0;
        int InsertBlockSequence = 0;
        int ExecutingInsertBlock = 0;
        public void BeginInsertBlock()
        {
            InsertBlockSequence++;
            CurrentInsertBlock = InsertBlockSequence;
        }
        public void EndInsertBlock()
        {
            CurrentInsertBlock = 0;
        }
        public object GetValueFromSql(string sql)
        {
            object result = DBNull.Value;
            using (DataTable ntable = OpenInmediate(null, sql,"VALUE"))
            {
                if (ntable.Rows.Count > 0)
                    result = ntable.Rows[0][0];
            }
            return result;
        }
        public long GetGenerator(string generatorName, int increment)
        {
            object valor = GetValueFromSql("SELECT GEN_ID(" + generatorName + ", " + increment.ToString() + " FROM RDB$DATABASE");
            if (valor == DBNull.Value)
                throw new Exception("Error getting generator value of generator " + generatorName);
            return Convert.ToInt64(valor);
        }


        public DataTable OpenInmediate(DataSet ndataset, string sql, string tablename)
        {
            commandgen.CommandText = sql;
            if (ndataset != null)
            {
                dataadapter.Fill(ndataset, tablename);
                return ndataset.Tables[tablename];
            }
            else
            {
                DataTable newTable = new DataTable();
                dataadapter.Fill(newTable);
                return newTable;
            }
        }
        public void Open(DataSet ndataset, string sql, string tablename,int maxrecords,ISqlExecuterPartialFillEvent nevent)
        {
            AddOperation(SQType.Open, sql, ndataset, tablename, maxrecords, nevent);
        }
        public void Open(DataSet ndataset, string sql, string tablename)
        {
            AddOperation(SQType.Open, sql, ndataset, tablename);
        }
        public void Open(DataSet ndataset, System.Data.Common.DbCommand nCommand , string tablename)
        {
            throw new Exception("Not Implemented");
        }

        private void AddOperation(SQType ntype, string sql)
        {
            AddOperation(ntype, sql, null, null, 0, null);
        }
        private void AddOperation(SQType ntype)
        {
            AddOperation(ntype, null, null, null, 0, null);
        }
        private void AddOperation(SQType ntype, string sql, DataSet ndataset, string tablename
            )
        {
            AddOperation(ntype, sql, ndataset, tablename, 0, null);
        }
        int commandId = 0;
        private void AddOperation(SQType ntype, string sql, DataSet ndataset, string tablename, int maxrecords,
            ISqlExecuterPartialFillEvent nevent)
        {
            AddOperation(ntype, sql, ndataset, tablename, maxrecords, nevent, null);
        }
        private void AddOperation(SQType ntype, string sql, DataSet ndataset, string tablename, int maxrecords,
            ISqlExecuterPartialFillEvent nevent,System.Data.Common.DbCommand ncommand)
        {
            commandId++;
            //nvalues = new object[8];
            nvalues[0] = commandId;
            nvalues[1] = (int)ntype;
            nvalues[2] = sql;
            if (ndataset == null)
                nvalues[3] = DBNull.Value;
            else
                nvalues[3] = ndataset;
            if (tablename != null)
                nvalues[4] = tablename;
            else
                nvalues[4] = DBNull.Value;
            nvalues[5] = maxrecords;
            nvalues[6] = DBNull.Value;
            if (nevent != null)
                nvalues[6] = new EventStorage(nevent);
            nvalues[8] = CurrentInsertBlock;
            if (ncommand == null)
            {
                nvalues[7] = 0;
                commands.Rows.Add(nvalues);
            }
            else
            {
                nvalues[7] = ncommand.Parameters.Count;
                commands.Rows.Add(nvalues);
                List<object[]> paramlist = new List<object[]>();
                foreach (System.Data.Common.DbParameter nparam in ncommand.Parameters)
                {
                    pvalues = new object[4];
                    pvalues[0] = commandId;
                    pvalues[1] = nparam.ParameterName;
                    //pvalues[2] = Convert.ToInt32(nparam.DbType);
                    pvalues[2] = Convert.ToInt32(nparam.DbType);
                    pvalues[3] = nparam.Value;
                    commandparams.Rows.Add(pvalues);
                    paramlist.Add(pvalues);
                }
                objparamvalues.Add(commandId, paramlist);
            }
        }
        private class EventStorage
        {
            public ISqlExecuterPartialFillEvent Event;
            public EventStorage(ISqlExecuterPartialFillEvent nvent)
            {
                Event = nvent;
            }
        }
        public void Execute(string sql)
        {
            AddOperation(SQType.Execute, sql);
        }
        public void Execute(System.Data.Common.DbCommand ncommand)
        {
            AddOperation(SQType.Execute, ncommand.CommandText,null,"",0,null,ncommand);
        }
        public System.Data.Common.DbCommand CreateCommand(string cadsql)
        {
            DbCommand result = factory.CreateCommand();
            result.CommandText = cadsql;
            result.Connection = connection;
            return result;
        }

        public void RollbackInmediate()
        {
            if (transaction != null)
            {
                transaction.Rollback();
                commandgen.Transaction = null;
                command_execute.Transaction = null;
            }
            else
                throw new Exception("No transaction active");
        }
        public void Commit()
        {
            AddOperation(SQType.Commit);
        }
        public void StartTransaction()
        {
            AddOperation(SQType.StartTransaction);
        }
        public void Rollback()
        {
            AddOperation(SQType.Rollback);
        }
        public void StartTransaction(IsolationLevel nlevel)
        {
            int idxlevel = (int)nlevel;
            AddOperation(SQType.StartTransaction, idxlevel.ToString());
        }
        public int ExecuteInmediate(string sql)
        {
            Flush();
            commandgen.CommandText = sql;
            return commandgen.ExecuteNonQuery();
        }
        public void Flush()
        {
            Flush(null);
        }
        public void Flush(ISqlExecuterProgressEvent nevent)
        {
            try
            {
                DateTime lasttime = System.DateTime.MinValue;
                DateTime currenttime;
                if (nevent != null)
                {
                    lasttime = System.DateTime.Now;
                }
                for (int i = 0; i< commands.Rows.Count; i++)
                {
                    DataRow xrow = commands.Rows[i];
                    SQType ntype = (SQType)xrow["TYPE"];
                    switch (ntype)
                    {
                        case SQType.Commit:
                            if (transaction != null)
                            {
                                transaction.Commit();
                                transaction = null;
                                commandgen.Transaction = null;
                                command_execute.Transaction = null;
                            }
                            //else
                            //    throw new Exception("No transaction active");
                            break;
                        case SQType.Rollback:
                            if (transaction != null)
                            {
                                transaction.Rollback();
                                commandgen.Transaction = null;
                                command_execute.Transaction = null;
                                transaction = null;
                            }
                            //else
                            //    throw new Exception("No transaction active");
                            break;
                        case SQType.StartTransaction:
                            if (transaction == null)
                            {
                                IsolationLevel nlevel = (IsolationLevel)System.Convert.ToInt32(xrow["SQL"]);
                                transaction = connection.BeginTransaction(nlevel);
                                commandgen.Transaction = transaction;
                                command_execute.Transaction = transaction;
                            }
                            else
                            {


                            }
                            //else
                            //    throw new Exception("No transaction active");
                            break;
                        case SQType.Open:
                            try
                            {
                                commandgen.Parameters.Clear();
                                commandgen.CommandText = xrow["SQL"].ToString();
                                int numparams = Convert.ToInt32(xrow["PARAMS"]);
                                commandgen.Parameters.Clear();
                                if (numparams > 0)
                                {
                                    /*DataRowView[] vparams = vcommandparams.FindRows(xrow["ID"]);
                                    foreach (DataRowView nrv in vparams)
                                    {
                                        FbParameter nparam = command.CreateParameter();
                                        nparam.ParameterName = nrv["NAME"].ToString();
                                        nparam.DbType = (System.Data.DbType)nrv["TYPE"];
                                        nparam.Value = nrv["VALUE"];
                                        command.Parameters.Add(nparam);
                                    }*/
                                    foreach (object[] obparams in objparamvalues[Convert.ToInt32(xrow["ID"])])
                                    {
                                        string paramname = obparams[1].ToString();
                                        DbType ftype = (DbType)obparams[2];
                                        DbParameter nparam = factory.CreateParameter();
                                        nparam.DbType = ftype;
                                        nparam.ParameterName = paramname;
                                        commandgen.Parameters.Add(nparam);
                                        nparam.Value = obparams[3];
                                    }
                                }

                                int maxrecords = 0;
                                if (xrow["MAXRECORDS"] != DBNull.Value)
                                    maxrecords = Convert.ToInt32(xrow["MAXRECORDS"]);
                                DataSet ndataset = (DataSet)xrow["DATASET"];
                                if (maxrecords==0)
                                    dataadapter.Fill(ndataset, xrow["TABLENAME"].ToString());
                                else
                                {
                                    int totalrecordcount = 0;
                                    //bool waspartial = false;
                                    using (DbDataReader intreader = commandgen.ExecuteReader())
                                    {
                                        // Partial fill 
                                        string tablename = xrow["TABLENAME"].ToString();
                                        DataTable intdatatable = new DataTable(tablename);
                                        int nfieldcount = intreader.FieldCount;
                                        for (int i2 = 0; i2 < nfieldcount; i2++)
                                        {
                                            Type ntypet = intreader.GetFieldType(i2);
                                            intdatatable.Columns.Add(intreader.GetName(i2), ntypet);
                                        }
                                        int dorecordcount = maxrecords;
                                        object[] nobjarray = new object[nfieldcount];
                                        while (intreader.Read())
                                        {
                                            totalrecordcount++;
                                            for (int i3 = 0; i3 < nfieldcount; i3++)
                                            {
                                                nobjarray[i3] = intreader[i3];
                                            }
                                            intdatatable.Rows.Add(nobjarray);
                                            dorecordcount--;
                                            if (dorecordcount < 0)
                                            {
                                                //waspartial = true;
                                                EventStorage evobject = (EventStorage)xrow["EVENTOBJECT"];
                                                evobject.Event(this, new ISqlExecuterPartialFillArgs(totalrecordcount, intdatatable));
                                                intdatatable.Rows.Clear();
                                                dorecordcount = maxrecords;
                                            }
                                        }
                                        //if (waspartial)
                                        //{
                                            if (intdatatable.Rows.Count > 0)
                                            {
                                                //SendPartialTable(intdatatable);
                                                EventStorage evobject = (EventStorage)xrow["EVENTOBJECT"];
                                                evobject.Event(this, new ISqlExecuterPartialFillArgs(totalrecordcount, intdatatable));

                                                intdatatable.Rows.Clear();
                                            }
                                            intdatatable.Dispose();
                                            intdatatable = null;
                                        //}
                                        //else
                                        //{
                                        //    ndataset.Tables.Add(intdatatable);
                                        //}


                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                throw new Exception("Error processing command: "+ex.Message+(char)10+commandgen.CommandText);
                            }
                            break;
                        case SQType.Execute:
                            try
                            {
                                if (command_execute.Transaction == null)
                                {
                                    if (transaction == null)
                                    {
                                        IsolationLevel nlevel = IsolationLevel.Snapshot;
                                        transaction = connection.BeginTransaction(nlevel);
                                        command_execute.Transaction = transaction;
                                    }
                                    else
                                    {
                                        command_execute.Transaction = transaction;

                                    }
                                }

                                int current_commandId = Convert.ToInt32(xrow["ID"]);
                                int numparams = Convert.ToInt32(xrow["PARAMS"]);
                                int insertblock = Convert.ToInt32(xrow["INSERT_BLOCK"]);
                                //FbCommand oldcommand = command;
                                //command = new FbCommand(xrow["SQL"].ToString(),oldcommand.Connection,oldcommand.Transaction);

                                if ((insertblock == 0) || (insertblock != ExecutingInsertBlock))
                                {

                                    command_execute.CommandText = xrow["SQL"].ToString();
                                    command_execute.Parameters.Clear();
                                    if (numparams > 0)
                                    {
                                        /*DataRowView[] vparams = vcommandparams.FindRows(xrow["ID"]);
                                        foreach (DataRowView nrv in vparams)
                                        {
                                            FbParameter nparam = command.CreateParameter();
                                            nparam.ParameterName = nrv["NAME"].ToString();
                                            nparam.DbType = (System.Data.DbType)nrv["TYPE"];
                                            nparam.Value = nrv["VALUE"];
                                            command.Parameters.Add(nparam);
                                        }*/

                                        foreach (object[] obparams in objparamvalues[current_commandId])
                                        {
                                            DbParameter nparam = factory.CreateParameter();
                                            nparam.DbType = (DbType)obparams[2];
                                            nparam.ParameterName = obparams[1].ToString();
                                            command_execute.Parameters.Add(nparam);
                                            nparam.Value = obparams[3];

                                            //string paramname = obparams[1].ToString();
                                            //FbDbType ftype = ;
                                            //FbParameter nparam = command.Parameters.Add(paramname, ftype);
                                        }
                                    }
                                    if (ExecutingInsertBlock != insertblock)
                                        ExecutingInsertBlock = insertblock;
                                }
                                else
                                {
                                    if (numparams > 0)
                                    {

                                        int idxparam = 0;
                                        foreach (object[] obparams in objparamvalues[current_commandId])
                                        {
                                            command_execute.Parameters[idxparam].Value = obparams[3];
                                            idxparam++;
                                        }
                                    }
                                }
                                command_execute.ExecuteNonQuery();
                            }
                            catch(Exception ex)
                            {
                                //command.Transaction.Commit();
                                throw new Exception("Error processing command: "+ex.Message+(char)10+command_execute.CommandText);
                            } 
                            break;
                    }
                    if (nevent != null)
                    {
                        currenttime = System.DateTime.Now;
                        TimeSpan dif = currenttime - lasttime;
                        if (dif.TotalSeconds > 0.5)
                        {
                            lasttime = currenttime;
                            nevent(i, commands.Rows.Count);
                        }
                    }

                }
            }
            finally
            {
                commands.Rows.Clear();
                commandId = 0;
                commandparams.Rows.Clear();
                objparamvalues.Clear();
            }
        }
        /*public static FbDbType DbTypeToFDbType(DbType valor)
        {
            FbDbType aresult;
            switch (valor)
            {
                case DbType.AnsiString:
                case DbType.String:
                    aresult = FbDbType.VarChar;
                    break;
                case DbType.AnsiStringFixedLength:
                    aresult = FbDbType.Char;
                    break;
                case DbType.Int16:
                case DbType.Byte:
                    aresult = FbDbType.SmallInt;
                    break;
                case DbType.Int32:
                    aresult = FbDbType.Integer;
                    break;
                case DbType.DateTime:
                    aresult = FbDbType.TimeStamp;
                    break;
                case DbType.Date:
                    aresult = FbDbType.Date;
                    break;
                case DbType.Time:
                    aresult = FbDbType.Time;
                    break;
                case DbType.Single:
                    aresult = FbDbType.Float;
                    break;
                case DbType.Double:
                    aresult = FbDbType.Double;
                    break;
                case DbType.Decimal:
                case DbType.Currency:
                    aresult = FbDbType.Decimal;
                    break;
                case DbType.Int64:
                    aresult = FbDbType.BigInt;
                    break;
                case DbType.Binary:
                    aresult = FbDbType.Binary;
                    break;
                default:
                    throw new Exception("Tipo no soportado:" + valor.ToString());
            }
            return aresult;
        }*/
        public void AddExternalColumnsToLastCommand(string external_columns, string deletions)
        {
            throw new Exception("Operation not implemented, add externalcolumns to last commmand");
        }
        public void AddCustomOperation(int operation, string data, byte[] binarydata)
        {
            throw new Exception("AddCustom Operation not supported in FbSqlExecuter");
        }
        public void Connect()
        {
            connection.Open();
        }
        public void Disconnect()
        {
            connection.Close();
        }
    }

}
