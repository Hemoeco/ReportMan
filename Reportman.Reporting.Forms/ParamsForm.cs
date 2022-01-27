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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Reportman.Drawing;
using Reportman.Reporting;

namespace Reportman.Reporting.Forms
{
    /// <summary>
    /// Report parameters form, it's used when the user click on show parameters icon on preview window.
    /// You can show the parameters window before executing a report. See example.
    /// </summary>
    /// <example>
    /// <code>
    /// Report rp=new Report();
    /// rp.LoadFromFile("test.rep");
    /// if ParamsForm.ShowParams(rp) 
    /// {
    ///     PrintOutReportWinForms prw=new PrintOutReportWinForms(rp);
    ///     prw.Preview=true;
    ///     prw.ShowPrintDialog=true;
    ///     prw.Print(rp.MetaFile);    
    /// }
    /// </code>
    /// </example>
    public class ParamsForm : Form
    {
        /// <summary>
        /// Required by Windows Forms designer
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Free resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Windows forms designed generated code

        /// <summary>
        /// Necessari for Windows forms designer
        /// </summary>
        private void InitializeComponent()
        {
            this.pbottom = new System.Windows.Forms.Panel();
            this.bcancel = new System.Windows.Forms.Button();
            this.bok = new System.Windows.Forms.Button();
            this.panelparent = new System.Windows.Forms.Panel();
            this.pbottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbottom
            // 
            this.pbottom.Controls.Add(this.bcancel);
            this.pbottom.Controls.Add(this.bok);
            this.pbottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pbottom.Location = new System.Drawing.Point(0, 312);
            this.pbottom.Name = "pbottom";
            this.pbottom.Size = new System.Drawing.Size(546, 48);
            this.pbottom.TabIndex = 0;
            // 
            // bcancel
            // 
            this.bcancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bcancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bcancel.Location = new System.Drawing.Point(424, 7);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(115, 33);
            this.bcancel.TabIndex = 1;
            this.bcancel.Text = "Cancel";
            // 
            // bok
            // 
            this.bok.Location = new System.Drawing.Point(8, 8);
            this.bok.Name = "bok";
            this.bok.Size = new System.Drawing.Size(112, 32);
            this.bok.TabIndex = 0;
            this.bok.Text = "OK";
            this.bok.Click += new System.EventHandler(this.bok_Click);
            // 
            // panelparent
            // 
            this.panelparent.AutoScroll = true;
            this.panelparent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelparent.Location = new System.Drawing.Point(0, 0);
            this.panelparent.Name = "panelparent";
            this.panelparent.Size = new System.Drawing.Size(546, 312);
            this.panelparent.TabIndex = 1;
            // 
            // ParamsForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(546, 360);
            this.Controls.Add(this.panelparent);
            this.Controls.Add(this.pbottom);
            this.Name = "ParamsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.ParamsForm_Load);
            this.pbottom.ResumeLayout(false);
            this.ResumeLayout(false);

		}

        #endregion

        private System.Windows.Forms.Panel pbottom;
		private System.Windows.Forms.Button bok;
		private System.Windows.Forms.Button bcancel;
        private Panel panelparent;
        private ParamsControl PControl;
        /// <summary>
        /// Constuctor
        /// </summary>
        public ParamsForm()
        {
            InitializeComponent();
            bok.Text = Translator.TranslateStr(93);
            bcancel.Text = Translator.TranslateStr(94);
						this.AcceptButton=bok;
        }

        private void ParamsForm_Load(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// Use this procedure to show the parameters window to the user
        /// </summary>
        /// <param name="rp">The Report containing the parameters</param>
        /// <returns>Returns true if the user accept parameter changes</returns>
        public static bool ShowParams(Report rp,IWin32Window parent)
		{
			return ShowParams(rp,"");
		}
        /// <summary>
        /// Use this procedure to show the parameters window to the user
        /// </summary>
        /// <param name="rp">The Report containing the parameters</param>
        /// <param name="caption">Caption for the report parameters window</param>
        /// <returns>Returns true if the user accept parameter changes</returns>
        public static bool ShowParams(Report rp,string caption)
        {
            bool doshow = false;
            foreach (Param p in rp.Params)
            {
                if (p.UserVisible)
                {
                    doshow = true;
                    break;
                }
            }
            if (!doshow)
                return true;
            int MAX_HEIGHT = Screen.PrimaryScreen.Bounds.Height-100;
            bool aresult=false;
            using (ParamsForm fparams = new ParamsForm())
            {
								fparams.Text=caption;
								if (fparams.Text.Length<1)
									fparams.Text=Translator.TranslateStr(238);								
                fparams.PControl = new ParamsControl();
                fparams.PControl.SetReport(rp);
                fparams.PControl.Parent = fparams.panelparent;
                fparams.Width= fparams.PControl.Width;
                int nheight = fparams.PControl.Height + fparams.pbottom.Height+Convert.ToInt32(20*Reportman.Drawing.GraphicUtils.DPIScale);
                if (nheight > MAX_HEIGHT)
                    nheight = MAX_HEIGHT;
                fparams.Height = nheight;
                fparams.PControl.Dock = DockStyle.Fill;
                aresult = fparams.ShowDialog() == DialogResult.OK;
            }
            return aresult;
        }

        private void bok_Click(object sender, EventArgs e)
        {
            // Save Params
					if (PControl.AcceptParams())
					{
						DialogResult = DialogResult.OK;				
					}
        }

#if REPMAN_DOTNET1
#else
		private void ParamsForm_Shown(object sender, EventArgs e)
        {
            if (PControl == null)
                return;
            PControl.FocusFirst();
        }
#endif
	}
}
