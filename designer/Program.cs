using System;
using System.Threading;
using System.Windows.Forms;

namespace designer
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Adds the event handler  to the event.
            try
            {
                MyExceptionHandler eh = new MyExceptionHandler();
                Application.ThreadException += new ThreadExceptionEventHandler(eh.OnThreadException);
                // Reportman.Drawing.AssemblyResolver.HandleUnresolvedAssemblies();
                AddCustomFactories();

                //var nresult = Reportman.Reporting.BarcodeItem.DetectBarcode("c:\\dades\\prova18.png");
                // Creates an instance of the methods that will handle the exception.


                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Reportman.Designer.MainForm());
            }
            catch(Exception ex)
            {
                MyExceptionHandler.ShowThreadExceptionDialog(ex);
            }

        }
        static void AddCustomFactories()
        {
            if (Reportman.Reporting.DatabaseInfo.CustomProviderFactories.Count == 0)
            {
                try
                {
                    Reportman.Reporting.DatabaseInfo.CustomProviderFactories.Add(Reportman.Reporting.DatabaseInfo.FIREBIRD_PROVIDER, FirebirdSql.Data.FirebirdClient.FirebirdClientFactory.Instance);
                    Reportman.Reporting.DatabaseInfo.CustomProviderFactories.Add(Reportman.Reporting.DatabaseInfo.FIREBIRD_PROVIDER2, FirebirdSql.Data.FirebirdClient.FirebirdClientFactory.Instance);
                }
                catch (Exception E)
                {
                    System.Console.WriteLine("Error in Firebird provider: " + E.Message);
                }
                try
                {
#if MONODEBUG
#else
                    Reportman.Reporting.DatabaseInfo.CustomProviderFactories.Add(Reportman.Reporting.DatabaseInfo.MYSQL_PROVIDER, MySql.Data.MySqlClient.MySqlClientFactory.Instance);
#endif

                }
                catch (Exception E)
                {
                    System.Console.WriteLine("Error in MySQL factory: " + E.Message);
                }
                try
                {
#if MONODEBUG
                    //DatabaseInfo.CustomProviderFactories.Add(DatabaseInfo.SQLITE_PROVIDER,Mono.Data.Sqlite.SqliteFactory.Instance);
#else
                    Reportman.Reporting.DatabaseInfo.CustomProviderFactories.Add(Reportman.Reporting.DatabaseInfo.SQLITE_PROVIDER, System.Data.SQLite.SQLiteFactory.Instance);
#endif
                }
                catch (Exception E)
                {
                    System.Console.WriteLine("Error in Sqlite factory: " + E.Message);
                }
                
            }
        }
        // Creates a class to handle the exception event.
        internal class MyExceptionHandler
        {
            // Handles the exception event.
            public void OnThreadException(object sender, ThreadExceptionEventArgs t)
            {
                DialogResult result = DialogResult.Cancel;
                try
                {
                    result = ShowThreadExceptionDialog(t.Exception);
                }
                catch
                {
                    try
                    {
                        MessageBox.Show("Fatal Error", "Fatal Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                    }
                    finally
                    {
                        Application.Exit();
                    }
                }

                // Exits the program when the user clicks Abort.
                if (result == DialogResult.Abort)
                    Application.Exit();
            }

            // Creates the error message and displays it.
            public static DialogResult ShowThreadExceptionDialog(Exception e)
            {
                string errorMsg = "Exception raised:\n\n";
                errorMsg = errorMsg + e.Message + "\n\nStack call:\n" + e.StackTrace;
                ThreadExceptionDialog ndia = new ThreadExceptionDialog(e);

                DialogResult aresult = ndia.ShowDialog();
                //DialogResult aresult = MessageBox.Show(errorMsg, "Application Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                return aresult;
            }
        }
    }
}