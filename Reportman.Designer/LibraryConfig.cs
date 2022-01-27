using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Designer
{
    public partial class LibraryConfig : UserControl
    {
        public ReportLibraryConfigCollection config = new ReportLibraryConfigCollection();
        public LibraryConfig()
        {
            InitializeComponent();

            List<string> driverDescriptions = DatabaseInfo.GetDriverDescriptions();
            foreach (string value in driverDescriptions)
            {
                comboDriver.Items.Add(value);
            }
            bnew.Text = Translator.TranslateStr(1102);
            bnew.ToolTipText = Translator.TranslateStr(1103);
            bdelete.Text = Translator.TranslateStr(1104);
            bdelete.ToolTipText = Translator.TranslateStr(1105);
            checkLoadParams.Text = Translator.TranslateStr(145);
            checkLoadDriverParams.Text = Translator.TranslateStr(146);
            checkLoginPrompt.Text = Translator.TranslateStr(144);
            labelDriver.Text = Translator.TranslateStr(147);
            labelReportTable.Text = Translator.TranslateStr(1115);
            labelReportField.Text = Translator.TranslateStr(1116);
            labelReportSearchField.Text = Translator.TranslateStr(1117);
            labelReportGroupsTable.Text = Translator.TranslateStr(1118);
            labelConnectionString.Text = Translator.TranslateStr(1119);
        }
        public void FillProviders()
        {
            comboProvider.Items.Clear();
            DataTable atable = System.Data.Common.DbProviderFactories.GetFactoryClasses();
            for (int indexp = 0; indexp < atable.Rows.Count; indexp++)
            {
                string nprovider = atable.Rows[indexp][2].ToString();
                comboProvider.Items.Add(nprovider);
            }
            foreach (string factory in DatabaseInfo.CustomProviderFactories.Keys)
            {
                string nprovider = factory;
                if (comboProvider.Items.IndexOf(nprovider)<0)
                    comboProvider.Items.Add(nprovider);

            }
        }

        public void Initialize()
        {
            this.bindingSource1.CurrentChanged += BindingSource1_CurrentChanged;
            FillProviders();
            string configFilePath = ReportLibraryConfig.GetConfigFilename();
            config.LoadFromFile(configFilePath);
            this.bindingSource1.DataSource = config;
            this.textReportTable.DataBindings.Add("Text", this.bindingSource1, "ReportTable");
            this.textReportField.DataBindings.Add("Text", this.bindingSource1, "ReportField");
            this.comboProvider.DataBindings.Add("Text", this.bindingSource1, "ProviderName");
            this.textReportSearchField.DataBindings.Add("Text", this.bindingSource1, "ReportSearchField");
            this.textReportGroupTable.DataBindings.Add("Text", this.bindingSource1, "ReportGroupsTable");
            this.comboDriver.DataBindings.Add("SelectedIndex", this.bindingSource1, "DriverIndex");
            this.textADOConnection.DataBindings.Add("Text", this.bindingSource1, "ADOConnectionString");

            //textReportTable.DataBindings.Add())
            /*foreach (ReportLibraryConfig item in config)
            {
                LConnections.Items.Add(item);
            }
            if (LConnections.Items.Count > 0)
                LConnections.SelectedIndex = 0;*/

        }

        private void BindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            bconnect.Enabled = bindingSource1.Current != null;
        }

        public void Save()
        {
            string configFilePath = ReportLibraryConfig.GetConfigFilename();
            config.SaveToFile(configFilePath);
        }

        private void bconnect_Click(object sender, EventArgs e)
        {
            ReportLibraryConfig libraryItem =  (ReportLibraryConfig)this.bindingSource1.Current;
            var factory = libraryItem.GetFactory();
            using (System.Data.Common.DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = libraryItem.ADOConnectionString;
                connection.Open();
                connection.Close();
            }
            MessageBox.Show(this.FindForm(), Translator.TranslateStr(406));
        }
    }
}
