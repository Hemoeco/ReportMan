using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reportman.Reporting.Forms
{
    public delegate void DataProgressEventHandler(object sender,DataProgress nform);
    public partial class DataProgress : Form
    {
        bool cancelled;
        DataProgressEventHandler OnExecute;
        public DataProgress()
        {
            InitializeComponent();
        }
        public static void ExecuteProgress(DataProgressEventHandler OnExecute)
        {
            DataProgress ndia = new DataProgress();
            ndia.OnExecute = OnExecute;
            ndia.timerexecute.Enabled = true;
            ndia.ShowDialog();
        }
        public void ShowProgress(object sender, int records, int count, ref bool docancel)
        {
            lprogress.Text = "Records: " + records.ToString("##,##") + " of " + count.ToString("###,##");
            if (progbar.Value > count)
                progbar.Value = count;
            progbar.Maximum = count;
            progbar.Value = records;
            Application.DoEvents();
            docancel = cancelled;

        }
        private void timerexecute_Tick(object sender, EventArgs e)
        {
            timerexecute.Enabled = false;
            try
            {
                OnExecute(this, this);
            }
            finally
            {
                Close();
            }
        }

        private void bcancel_Click(object sender, EventArgs e)
        {
            cancelled = true;
        }
    }
}
