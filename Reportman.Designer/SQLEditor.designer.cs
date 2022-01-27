namespace Reportman.Designer
{
    partial class SQLEditor
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BOK = new System.Windows.Forms.Button();
            this.bcancel = new System.Windows.Forms.Button();
            this.bshowdata = new System.Windows.Forms.Button();
            this.MemoSQL = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.bshowdata, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.bcancel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.BOK, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.MemoSQL, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(719, 505);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // BOK
            // 
            this.BOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BOK.Location = new System.Drawing.Point(54, 472);
            this.BOK.Name = "BOK";
            this.BOK.Size = new System.Drawing.Size(130, 30);
            this.BOK.TabIndex = 10;
            this.BOK.Text = "OK";
            this.BOK.UseVisualStyleBackColor = true;
            this.BOK.Click += new System.EventHandler(this.BOK_Click);
            // 
            // bcancel
            // 
            this.bcancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bcancel.Location = new System.Drawing.Point(533, 472);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(130, 30);
            this.bcancel.TabIndex = 11;
            this.bcancel.Text = "Cancel";
            this.bcancel.UseVisualStyleBackColor = true;
            this.bcancel.Click += new System.EventHandler(this.bcancel_Click);
            // 
            // bshowdata
            // 
            this.bshowdata.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bshowdata.Location = new System.Drawing.Point(293, 472);
            this.bshowdata.Name = "bshowdata";
            this.bshowdata.Size = new System.Drawing.Size(130, 30);
            this.bshowdata.TabIndex = 12;
            this.bshowdata.Text = "Show data";
            this.bshowdata.UseVisualStyleBackColor = true;
            this.bshowdata.Click += new System.EventHandler(this.bshowdata_Click);
            // 
            // MemoSQL
            // 
            this.MemoSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.MemoSQL, 3);
            this.MemoSQL.Location = new System.Drawing.Point(3, 3);
            this.MemoSQL.Multiline = true;
            this.MemoSQL.Name = "MemoSQL";
            this.MemoSQL.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.MemoSQL.Size = new System.Drawing.Size(713, 463);
            this.MemoSQL.TabIndex = 13;
            // 
            // SQLEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SQLEditor";
            this.Size = new System.Drawing.Size(719, 505);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button bshowdata;
        private System.Windows.Forms.Button bcancel;
        private System.Windows.Forms.Button BOK;
        private System.Windows.Forms.TextBox MemoSQL;
    }
}
