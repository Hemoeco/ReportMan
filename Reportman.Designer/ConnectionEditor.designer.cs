namespace Reportman.Designer
{
    partial class ConnectionEditor
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
            this.btestconnection = new System.Windows.Forms.Button();
            this.bcancel = new System.Windows.Forms.Button();
            this.BOK = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel1.Controls.Add(this.btestconnection, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.bcancel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.BOK, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.MemoSQL, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(637, 506);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // btestconnection
            // 
            this.btestconnection.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btestconnection.Location = new System.Drawing.Point(253, 473);
            this.btestconnection.Name = "btestconnection";
            this.btestconnection.Size = new System.Drawing.Size(130, 30);
            this.btestconnection.TabIndex = 12;
            this.btestconnection.Text = "Test Connection";
            this.btestconnection.UseVisualStyleBackColor = true;
            this.btestconnection.Click += new System.EventHandler(this.Btestconnection_Click);
            // 
            // bcancel
            // 
            this.bcancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bcancel.Location = new System.Drawing.Point(465, 473);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(130, 30);
            this.bcancel.TabIndex = 11;
            this.bcancel.Text = "Cancel";
            this.bcancel.UseVisualStyleBackColor = true;
            // 
            // BOK
            // 
            this.BOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BOK.Location = new System.Drawing.Point(41, 473);
            this.BOK.Name = "BOK";
            this.BOK.Size = new System.Drawing.Size(130, 30);
            this.BOK.TabIndex = 10;
            this.BOK.Text = "OK";
            this.BOK.UseVisualStyleBackColor = true;
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
            this.MemoSQL.Size = new System.Drawing.Size(631, 464);
            this.MemoSQL.TabIndex = 13;
            // 
            // ConnectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ConnectionEditor";
            this.Size = new System.Drawing.Size(637, 506);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btestconnection;
        private System.Windows.Forms.Button bcancel;
        private System.Windows.Forms.Button BOK;
        private System.Windows.Forms.TextBox MemoSQL;
    }
}
