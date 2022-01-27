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
    /// This form is used by report parameters window to help user in selecting parameter values, possible
    /// if the parameter provide a lookup dataset
    /// </summary>
    public class SearchForm : Form
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pbottom = new System.Windows.Forms.Panel();
            this.bcancel = new System.Windows.Forms.Button();
            this.bok = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pallclient = new System.Windows.Forms.Panel();
            this.dgrid = new System.Windows.Forms.DataGrid();
            this.ptop = new System.Windows.Forms.Panel();
            this.bsearch = new System.Windows.Forms.Button();
            this.textsearch = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pbottom.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pallclient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrid)).BeginInit();
            this.ptop.SuspendLayout();
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
            this.bcancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bcancel.Location = new System.Drawing.Point(221, 7);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.pallclient);
            this.panel1.Controls.Add(this.ptop);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(546, 312);
            this.panel1.TabIndex = 1;
            // 
            // pallclient
            // 
            this.pallclient.Controls.Add(this.dgrid);
            this.pallclient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pallclient.Location = new System.Drawing.Point(0, 40);
            this.pallclient.Name = "pallclient";
            this.pallclient.Size = new System.Drawing.Size(546, 272);
            this.pallclient.TabIndex = 1;
            // 
            // dgrid
            // 
            this.dgrid.DataMember = "";
            this.dgrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgrid.Location = new System.Drawing.Point(0, 0);
            this.dgrid.Name = "dgrid";
            this.dgrid.ReadOnly = true;
            this.dgrid.Size = new System.Drawing.Size(546, 272);
            this.dgrid.TabIndex = 0;
            this.dgrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgrid_KeyDown);
            // 
            // ptop
            // 
            this.ptop.Controls.Add(this.bsearch);
            this.ptop.Controls.Add(this.textsearch);
            this.ptop.Dock = System.Windows.Forms.DockStyle.Top;
            this.ptop.Location = new System.Drawing.Point(0, 0);
            this.ptop.Name = "ptop";
            this.ptop.Size = new System.Drawing.Size(546, 40);
            this.ptop.TabIndex = 0;
            // 
            // bsearch
            // 
            this.bsearch.Location = new System.Drawing.Point(424, 8);
            this.bsearch.Name = "bsearch";
            this.bsearch.Size = new System.Drawing.Size(112, 24);
            this.bsearch.TabIndex = 1;
            this.bsearch.Text = "Search";
            this.bsearch.Click += new System.EventHandler(this.bsearch_Click);
            // 
            // textsearch
            // 
            this.textsearch.Location = new System.Drawing.Point(8, 8);
            this.textsearch.Name = "textsearch";
            this.textsearch.Size = new System.Drawing.Size(400, 20);
            this.textsearch.TabIndex = 0;
            this.textsearch.TextChanged += new System.EventHandler(this.textsearch_TextChanged);
            this.textsearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textsearch_KeyDown);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SearchForm
            // 
            this.AcceptButton = this.bok;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(546, 360);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pbottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Name = "SearchForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
#if REPMAN_DOTNET1
#else
            this.Shown += new System.EventHandler(this.SearchForm_Shown);
#endif
            this.Load += new System.EventHandler(this.SearchForm_Load);
            this.pbottom.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.pallclient.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrid)).EndInit();
            this.ptop.ResumeLayout(false);
            this.ptop.PerformLayout();
            this.ResumeLayout(false);

		}

        #endregion

        private System.Windows.Forms.Panel pbottom;
				private System.Windows.Forms.Button bok;
				private System.Windows.Forms.Panel panel1;
				private System.Windows.Forms.Panel ptop;
				private System.Windows.Forms.Panel pallclient;
				private System.Windows.Forms.DataGrid dgrid;
				private System.Windows.Forms.TextBox textsearch;
				private System.Windows.Forms.Button bsearch;
        private Timer timer1;
				private System.Windows.Forms.Button bcancel;
        /// <summary>
        /// Constructor
        /// </summary>
        public SearchForm()
        {
            InitializeComponent();
            bok.Text = Translator.TranslateStr(93);
            bcancel.Text = Translator.TranslateStr(94);
						bsearch.Text=Translator.TranslateStr(1375);
        }
				private Report report;
				private Param param;
        private void SearchForm_Load(object sender, EventArgs e)
        {
        }
        private void bok_Click(object sender, EventArgs e)
        {
            if (dgrid.DataSource != null)
            {
                if (dgrid.CurrentRowIndex >= 0)
                    param.Value=Variant.VariantFromObject(((DataTable)dgrid.DataSource).Rows[dgrid.CurrentRowIndex][0]);
            }
            DialogResult = DialogResult.OK;
        }
        /// <summary>
        /// Search for a parameter value using Report configuration
        /// </summary>
        /// <param name="aparam">Parameter to perform lookup</param>
        /// <param name="areport">Report containing the parameter</param>
        /// <returns>Returns true if the user accepted the parameter value</returns>
		public static bool SearchParamValue(Param aparam,Report areport)
		{
			bool aresult=false;
			using (SearchForm nform=new SearchForm())
			{
                nform.Text = Translator.TranslateStr(1374);
				nform.param=aparam;
				nform.report=areport;
                if (aparam.SearchParam.Length < 1)
                {
                    nform.ptop.Visible = false;
                }
                if (nform.ShowDialog() == DialogResult.OK)
				{
					aresult=true;
				}
			}
			return aresult;
		}

			private void bsearch_Click(object sender, EventArgs e)
			{
				// Execute the query again
				string searchname=param.SearchDataset;
				DataInfo dinfo=report.DataInfo[searchname];
				DatabaseInfo dbinfo=report.DatabaseInfo[dinfo.DatabaseAlias];
                if (param.SearchParam.Length > 0)
                {
                    report.Params[param.SearchParam].Value = textsearch.Text;
                    ptop.Visible = true;
                }
                else
                    ptop.Visible = false;
				IDataReader areader=dbinfo.GetDataReaderFromSQL(dinfo.SQL,dinfo.Alias,report.Params,false);
				PagedDataTable atable=new PagedDataTable();
				atable.CurrentReader=areader;
                if (dgrid.DataSource == null)
                {
                    dgrid.TableStyles.Add(DataShow.CreateDataGridStyle(atable,dgrid));
                }
				dgrid.DataSource=atable;
			}

        private void textsearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgrid.DataSource == null)
                return;
            if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down))
            {
                CurrencyManager cmx = (CurrencyManager)dgrid.BindingContext[dgrid.DataSource];
                if (e.KeyCode == Keys.Up)
                    cmx.Position = cmx.Position - 1;
                else
                    cmx.Position = cmx.Position + 1;
                e.Handled = true;
            }
        }

        private void SearchForm_Shown(object sender, EventArgs e)
        {
            if (ptop.Visible)
            {
                textsearch.Focus();
                timer1.Enabled = true;
            }
            else
            {
                bsearch_Click(this, new EventArgs());
                dgrid.Focus();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            bsearch_Click(this, new EventArgs());
        }

        private void textsearch_TextChanged(object sender, EventArgs e)
        {
            if (ptop.Visible)
            {
                timer1.Enabled = false;
                timer1.Enabled = true;
            }
        }

        private void dgrid_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Enter))
            {
                e.Handled = true;
                bok_Click(this, new EventArgs());
            }
        }
		}
}