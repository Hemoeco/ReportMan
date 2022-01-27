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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Reportman.Drawing;
using System.Threading;

namespace Reportman.Drawing.Forms
{
	/// <summary>
	/// Form to show report processing progres
	/// </summary>
	public class ReportProgressForm : System.Windows.Forms.Form
	{
        private PrintOut intdriver;
        private MetaFile intmeta;
        public bool multithread;


        private System.Windows.Forms.Button BCancel;
        private IContainer components;
		private System.Windows.Forms.Label LProgress;
		private bool internalcancel;
		public MetaFile metafile;

        private System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Timer timerclose;
		MetaFileWorkProgress workevent;
        /// <summary>
        /// Funtion to set the metafile to link to the form
        /// </summary>
        /// <param name="meta"></param>
		public void SetMetaFile(MetaFile meta)
		{
			metafile = meta;
			workevent = new MetaFileWorkProgress(this.WorkProgress);
			metafile.OnWorkProgress += workevent;
		}
        /// <summary>
        /// Constructor
        /// </summary>
		public ReportProgressForm()
		{
			InitializeComponent();
		}
		static void WorkProgressDoCancel(int records, int pagecount, ref bool docancel)
		{
			docancel = true;
		}
        private void WorkProgressInt(int records, int pagecount, ref bool docancel)
        {
            string atext = "";
            if (records > 0)
                atext = atext + Translator.TranslateStr(684) + ": " +
                records.ToString("##,##0");
            atext = atext + " " + Translator.TranslateStr(1414) + ": " +
                (pagecount).ToString("#,##0");
            LProgress.Text = atext;
            Application.DoEvents();
            docancel = internalcancel;
            if (metafile.Finished)
                Visible = false;

        }
		public void WorkProgress(int records, int pagecount, ref bool docancel)
		{

            if (this.InvokeRequired)
            {
                object[] nparams = new object[3];
                nparams[0] = records;
                nparams[1] = pagecount;
                nparams[2] = docancel;
                this.Invoke(new MetaFileWorkProgress(WorkProgressInt), nparams);
                docancel = this.internalcancel;
            }
            else
                WorkProgressInt(records, pagecount, ref docancel);
		}
        /// <summary>
        /// Free resources
        /// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Generated code by Desginer
		/// <summary>
		/// Necessary code by Designer
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportProgressForm));
            this.BCancel = new System.Windows.Forms.Button();
            this.LProgress = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timerclose = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // BCancel
            // 
            this.BCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BCancel.Location = new System.Drawing.Point(120, 72);
            this.BCancel.Name = "BCancel";
            this.BCancel.Size = new System.Drawing.Size(104, 24);
            this.BCancel.TabIndex = 0;
            this.BCancel.Text = "Cancel";
            this.BCancel.Click += new System.EventHandler(this.BCancel_Click);
            // 
            // LProgress
            // 
            this.LProgress.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.LProgress.Location = new System.Drawing.Point(16, 8);
            this.LProgress.Name = "LProgress";
            this.LProgress.Size = new System.Drawing.Size(312, 32);
            this.LProgress.TabIndex = 1;
            this.LProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timerclose
            // 
            this.timerclose.Interval = 10;
            this.timerclose.Tick += new System.EventHandler(this.timerclose_Tick);
            // 
            // ReportProgressForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(338, 104);
            this.Controls.Add(this.LProgress);
            this.Controls.Add(this.BCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportProgressForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.ReportProgress_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.ReportProgress_Closing);
            this.ResumeLayout(false);

		}
		#endregion

		private void BCancel_Click(object sender, System.EventArgs e)
		{
			internalcancel = true;
		}

		private void ReportProgress_Load(object sender, System.EventArgs e)
		{
            // Translate items
            BCancel.Text = Translator.TranslateStr(271);
            Text = Translator.TranslateStr(1390);
		}

		private void ReportProgress_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            if (!multithread)
            {
                metafile.OnWorkProgress -= workevent;
                metafile.OnWorkProgress += new MetaFileWorkProgress(ReportProgressForm.WorkProgressDoCancel);
            }
            else
            {
                metafile.OnWorkProgress += new MetaFileWorkProgress(ReportProgressForm.WorkProgressDoCancel);
            }
		}
        public static void CalculateReport(MetaFile meta, PrintOut driver)
        {
            using (ReportProgressForm nform = new ReportProgressForm())
            {
                nform.intdriver = driver;
                nform.intmeta = meta;
                nform.timer1.Enabled = true;
                
                nform.ShowDialog();
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            SetMetaFile(intmeta);
            intdriver.Print(intmeta);
        }

        private void timerclose_Tick(object sender, EventArgs e)
        {
            timerclose.Enabled = false;
            Close();
        }
	}
    public class CalcReportProgress
    {
        public PrintOut driver;
        object flag = 0;
        public Thread threadcalc;
        public MetaFile meta;
        private string ErrorMessage;
        ReportProgressForm ndia = null;
        public static bool CalculateReport(MetaFile meta,PrintOut printdriver,IWin32Window winowner)
        {
            CalcReportProgress nprogres = new CalcReportProgress();
            nprogres.driver = printdriver;
            nprogres.meta = meta;
            nprogres.ErrorMessage = "";
            nprogres.threadcalc = new Thread(new ThreadStart(nprogres.Start));
            nprogres.threadcalc.Start();
            if (nprogres.threadcalc.Join(4000))
            {
                if (nprogres.ErrorMessage.Length > 0)
                    throw new Exception(nprogres.ErrorMessage);
                return meta.Finished;
            }
            // Else create form and show it
            ReportProgressForm ndia = new ReportProgressForm();
            try
            {
                nprogres.ndia = ndia;
                ndia.multithread = true;
                ndia.metafile = meta;
                MetaFileWorkProgress workevent = new MetaFileWorkProgress(ndia.WorkProgress);
                meta.OnWorkProgress += workevent;
                try
                {
                    ndia.ShowDialog(winowner);
                }
                finally
                {
                    meta.OnWorkProgress -= workevent;
                }
            }
            finally
            {
                Monitor.Enter(nprogres.flag);
                try
                {
                    nprogres.ndia = null;
                    ndia.Dispose();
                }
                finally
                {
                    Monitor.Exit(nprogres.flag);
                }
            }
            if (!nprogres.threadcalc.Join(1000))
            {
                nprogres.threadcalc.Abort();
            }
            nprogres.threadcalc = null;
            if (nprogres.ErrorMessage.Length > 0)
                throw new Exception(nprogres.ErrorMessage);
            return meta.Finished;
        }
        private void Start()
        {
            try
            {
                driver.Print(meta);
            }
            catch (Exception E)
            {
                ErrorMessage = E.Message;
            }
            Monitor.Enter(flag);
            try
            {
                if (ndia!=null)
                    ndia.Invoke(new EventHandler(hanclose));
                    
            }
            finally
            {
                Monitor.Exit(flag);
            }
        }
        private void hanclose(object sender, EventArgs args)
        {
            ndia.Close();
        }

    }
}
