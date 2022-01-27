namespace Reportman.Designer
{
    partial class ExpressionDlg
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
            this.BAdd = new System.Windows.Forms.Button();
            this.LParams = new System.Windows.Forms.Label();
            this.LItems = new System.Windows.Forms.ListBox();
            this.LCategory = new System.Windows.Forms.ListBox();
            this.MemoExpre = new System.Windows.Forms.TextBox();
            this.LModel = new System.Windows.Forms.Label();
            this.LHelp = new System.Windows.Forms.Label();
            this.BCancel = new System.Windows.Forms.Button();
            this.BOK = new System.Windows.Forms.Button();
            this.BConectar = new System.Windows.Forms.Button();
            this.BCheckSyn = new System.Windows.Forms.Button();
            this.BShowResult = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.BAdd, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.LParams, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.LItems, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.LCategory, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.MemoExpre, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LModel, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.LHelp, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.BCancel, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.BOK, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.BConectar, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.BCheckSyn, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.BShowResult, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(495, 383);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // BAdd
            // 
            this.BAdd.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BAdd.Location = new System.Drawing.Point(249, 145);
            this.BAdd.Name = "BAdd";
            this.BAdd.Size = new System.Drawing.Size(117, 30);
            this.BAdd.TabIndex = 13;
            this.BAdd.Text = "Add";
            this.BAdd.UseVisualStyleBackColor = true;
            this.BAdd.Click += new System.EventHandler(this.BAdd_Click);
            // 
            // LParams
            // 
            this.LParams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LParams.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.LParams, 4);
            this.LParams.Location = new System.Drawing.Point(3, 333);
            this.LParams.Name = "LParams";
            this.LParams.Size = new System.Drawing.Size(489, 13);
            this.LParams.TabIndex = 7;
            this.LParams.Text = "Params";
            // 
            // LItems
            // 
            this.LItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.LItems, 2);
            this.LItems.FormattingEnabled = true;
            this.LItems.Location = new System.Drawing.Point(249, 181);
            this.LItems.Name = "LItems";
            this.LItems.Size = new System.Drawing.Size(243, 134);
            this.LItems.TabIndex = 4;
            this.LItems.SelectedIndexChanged += new System.EventHandler(this.LItems_SelectedIndexChanged);
            // 
            // LCategory
            // 
            this.LCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.LCategory, 2);
            this.LCategory.FormattingEnabled = true;
            this.LCategory.Items.AddRange(new object[] {
            "Database fields",
            "Functions",
            "Variables",
            "Constants",
            "Operators"});
            this.LCategory.Location = new System.Drawing.Point(3, 181);
            this.LCategory.Name = "LCategory";
            this.LCategory.Size = new System.Drawing.Size(240, 134);
            this.LCategory.TabIndex = 3;
            this.LCategory.SelectedIndexChanged += new System.EventHandler(this.LCategory_SelectedIndexChanged);
            // 
            // MemoExpre
            // 
            this.MemoExpre.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.MemoExpre, 4);
            this.MemoExpre.Location = new System.Drawing.Point(3, 3);
            this.MemoExpre.Multiline = true;
            this.MemoExpre.Name = "MemoExpre";
            this.MemoExpre.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.MemoExpre.Size = new System.Drawing.Size(489, 136);
            this.MemoExpre.TabIndex = 1;
            // 
            // LModel
            // 
            this.LModel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LModel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.LModel, 2);
            this.LModel.Location = new System.Drawing.Point(249, 320);
            this.LModel.Name = "LModel";
            this.LModel.Size = new System.Drawing.Size(243, 13);
            this.LModel.TabIndex = 5;
            this.LModel.Text = "LModel";
            this.LModel.Click += new System.EventHandler(this.Label1_Click);
            // 
            // LHelp
            // 
            this.LHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LHelp.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.LHelp, 2);
            this.LHelp.Location = new System.Drawing.Point(3, 320);
            this.LHelp.Name = "LHelp";
            this.LHelp.Size = new System.Drawing.Size(240, 13);
            this.LHelp.TabIndex = 6;
            this.LHelp.Text = "LHelp";
            // 
            // BCancel
            // 
            this.BCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.SetColumnSpan(this.BCancel, 2);
            this.BCancel.Location = new System.Drawing.Point(312, 349);
            this.BCancel.Name = "BCancel";
            this.BCancel.Size = new System.Drawing.Size(117, 30);
            this.BCancel.TabIndex = 8;
            this.BCancel.Text = "Cancel";
            this.BCancel.UseVisualStyleBackColor = true;
            this.BCancel.Click += new System.EventHandler(this.BCancel_Click);
            // 
            // BOK
            // 
            this.BOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.SetColumnSpan(this.BOK, 2);
            this.BOK.Location = new System.Drawing.Point(58, 349);
            this.BOK.Name = "BOK";
            this.BOK.Size = new System.Drawing.Size(130, 30);
            this.BOK.TabIndex = 9;
            this.BOK.Text = "OK";
            this.BOK.UseVisualStyleBackColor = true;
            this.BOK.Click += new System.EventHandler(this.BOK_Click);
            // 
            // BConectar
            // 
            this.BConectar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BConectar.Location = new System.Drawing.Point(3, 145);
            this.BConectar.Name = "BConectar";
            this.BConectar.Size = new System.Drawing.Size(117, 30);
            this.BConectar.TabIndex = 10;
            this.BConectar.Text = "Connect";
            this.BConectar.UseVisualStyleBackColor = true;
            this.BConectar.Click += new System.EventHandler(this.BConectar_Click);
            // 
            // BCheckSyn
            // 
            this.BCheckSyn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BCheckSyn.Location = new System.Drawing.Point(126, 145);
            this.BCheckSyn.Name = "BCheckSyn";
            this.BCheckSyn.Size = new System.Drawing.Size(117, 30);
            this.BCheckSyn.TabIndex = 12;
            this.BCheckSyn.Text = "Syntax Check";
            this.BCheckSyn.UseVisualStyleBackColor = true;
            this.BCheckSyn.Click += new System.EventHandler(this.BCheckSyn_Click);
            // 
            // BShowResult
            // 
            this.BShowResult.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BShowResult.Location = new System.Drawing.Point(372, 145);
            this.BShowResult.Name = "BShowResult";
            this.BShowResult.Size = new System.Drawing.Size(120, 30);
            this.BShowResult.TabIndex = 11;
            this.BShowResult.Text = "Show Result";
            this.BShowResult.UseVisualStyleBackColor = true;
            this.BShowResult.Click += new System.EventHandler(this.BShowResult_Click);
            // 
            // ExpressionDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ExpressionDlg";
            this.Size = new System.Drawing.Size(495, 383);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox LItems;
        private System.Windows.Forms.ListBox LCategory;
        private System.Windows.Forms.TextBox MemoExpre;
        private System.Windows.Forms.Label LModel;
        private System.Windows.Forms.Label LHelp;
        private System.Windows.Forms.Label LParams;
        private System.Windows.Forms.Button BCancel;
        private System.Windows.Forms.Button BOK;
        private System.Windows.Forms.Button BShowResult;
        private System.Windows.Forms.Button BConectar;
        private System.Windows.Forms.Button BCheckSyn;
        private System.Windows.Forms.Button BAdd;
    }
}
