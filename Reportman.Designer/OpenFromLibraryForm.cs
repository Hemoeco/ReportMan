using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Reportman.Reporting;
using Reportman.Drawing;

namespace Reportman.Designer
{
    public partial class OpenFromLibraryForm : Form
    {
        ReportLibraryConfigCollection libs;
        OpenFromLibrary.SelectionModeType SelectionType = OpenFromLibrary.SelectionModeType.Selection; 
        public OpenFromLibraryForm()
        {
            InitializeComponent();

            Text = Translator.TranslateStr(1135);
            LabelLibrary.Text = Translator.TranslateStr(1140);
            openFromLibrary1.Visible = false;
            openFromLibrary1.OnAccept += OnAccept;
            openFromLibrary1.OnCancel += OnCancel;

        }
        ReportLibrarySelection ReportSelection;
        public void OnCancel(object sender, EventArgs args)
        {
            ReportSelection = null;
            DialogResult = DialogResult.Cancel;
        }
        public void OnAccept(object sender,EventArgs args)
        {
            string ReportName = openFromLibrary1.GetSelectedReport();
            if (ReportName.Length == 0)
            {
                throw new Exception(Translator.TranslateStr(357));
            }
            ReportLibrarySelection selection = new ReportLibrarySelection();
            selection.ReportLibrary = (ReportLibraryConfig)comboLibrary.Items[comboLibrary.SelectedIndex];
            selection.ReportName = openFromLibrary1.GetSelectedReport();
            selection.Stream = selection.ReportLibrary.ReadReport(selection.ReportName);
            ReportSelection = selection;
            DialogResult = DialogResult.OK;
        }
        public static ReportLibrarySelection SelectReportFromLibraries(ReportLibraryConfigCollection libs,
              OpenFromLibrary.SelectionModeType SelectionType,IWin32Window parent)
        {
            using (OpenFromLibraryForm dia = new OpenFromLibraryForm())
            {
                dia.SelectionType = SelectionType;
                dia.libs = libs;
                dia.ShowDialog(parent);
                return dia.ReportSelection;
            }
        }

        private void bcancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void OpenFromLibraryForm_Load(object sender, EventArgs e)
        {
            foreach (var lib in libs)
            {
                comboLibrary.Items.Add(lib);
            }
            if (libs.Count > 0)
                comboLibrary.SelectedIndex = 0;
        }

        private void comboLibrary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboLibrary.SelectedIndex < 0)
                openFromLibrary1.Visible = false;
            try
            {
                ReportLibraryConfig selected = (ReportLibraryConfig)comboLibrary.Items[comboLibrary.SelectedIndex];
                var factory = selected.GetFactory();
                if (selected.CurrentConnection == null)
                {
                    System.Data.Common.DbConnection connection = factory.CreateConnection();
                    connection.ConnectionString = selected.ADOConnectionString;
                    connection.Open();
                    selected.CurrentConnection = connection;
                }
                DbSqlExecuter executer = new DbSqlExecuter(selected.CurrentConnection, factory);
                openFromLibrary1.Init(executer, selected, OpenFromLibrary.SelectionModeType.SelectionEdit);
                openFromLibrary1.Visible = true;
            }
            catch
            {
                openFromLibrary1.Visible = false;
                throw;
            }
        }

        private void openFromLibrary1_Load(object sender, EventArgs e)
        {
            if (!openFromLibrary1.Visible)
                return ;
            string selectedReport = openFromLibrary1.GetSelectedReport();
            if (selectedReport.Length > 0)
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void OpenFromLibraryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                foreach (var lib in libs)
                {
                    if (lib.CurrentConnection != null)
                        lib.CurrentConnection.Close();
                }
            }
            catch
            {

            }
        }
    }
}
