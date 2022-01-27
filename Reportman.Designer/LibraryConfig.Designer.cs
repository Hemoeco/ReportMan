namespace Reportman.Designer
{
    partial class LibraryConfig
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
            this.components = new System.ComponentModel.Container();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.PanelConnection = new System.Windows.Forms.Panel();
            this.comboProvider = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textADOConnection = new System.Windows.Forms.TextBox();
            this.labelConnectionString = new System.Windows.Forms.Label();
            this.bconnect = new System.Windows.Forms.Button();
            this.textReportGroupTable = new System.Windows.Forms.TextBox();
            this.labelReportGroupsTable = new System.Windows.Forms.Label();
            this.textReportSearchField = new System.Windows.Forms.TextBox();
            this.labelReportField = new System.Windows.Forms.Label();
            this.textReportField = new System.Windows.Forms.TextBox();
            this.labelReportSearchField = new System.Windows.Forms.Label();
            this.textReportTable = new System.Windows.Forms.TextBox();
            this.comboDriver = new System.Windows.Forms.ComboBox();
            this.labelReportTable = new System.Windows.Forms.Label();
            this.labelDriver = new System.Windows.Forms.Label();
            this.checkLoginPrompt = new System.Windows.Forms.CheckBox();
            this.checkLoadDriverParams = new System.Windows.Forms.CheckBox();
            this.checkLoadParams = new System.Windows.Forms.CheckBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.LConnections = new System.Windows.Forms.ListBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.bnew = new System.Windows.Forms.ToolStripButton();
            this.bdelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.PanelConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.PanelConnection);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitter1);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.LConnections);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(481, 389);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(481, 415);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // PanelConnection
            // 
            this.PanelConnection.Controls.Add(this.comboProvider);
            this.PanelConnection.Controls.Add(this.label1);
            this.PanelConnection.Controls.Add(this.textADOConnection);
            this.PanelConnection.Controls.Add(this.labelConnectionString);
            this.PanelConnection.Controls.Add(this.bconnect);
            this.PanelConnection.Controls.Add(this.textReportGroupTable);
            this.PanelConnection.Controls.Add(this.labelReportGroupsTable);
            this.PanelConnection.Controls.Add(this.textReportSearchField);
            this.PanelConnection.Controls.Add(this.labelReportField);
            this.PanelConnection.Controls.Add(this.textReportField);
            this.PanelConnection.Controls.Add(this.labelReportSearchField);
            this.PanelConnection.Controls.Add(this.textReportTable);
            this.PanelConnection.Controls.Add(this.comboDriver);
            this.PanelConnection.Controls.Add(this.labelReportTable);
            this.PanelConnection.Controls.Add(this.labelDriver);
            this.PanelConnection.Controls.Add(this.checkLoginPrompt);
            this.PanelConnection.Controls.Add(this.checkLoadDriverParams);
            this.PanelConnection.Controls.Add(this.checkLoadParams);
            this.PanelConnection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelConnection.Location = new System.Drawing.Point(180, 0);
            this.PanelConnection.Name = "PanelConnection";
            this.PanelConnection.Size = new System.Drawing.Size(301, 389);
            this.PanelConnection.TabIndex = 2;
            // 
            // comboProvider
            // 
            this.comboProvider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboProvider.FormattingEnabled = true;
            this.comboProvider.Location = new System.Drawing.Point(137, 115);
            this.comboProvider.Name = "comboProvider";
            this.comboProvider.Size = new System.Drawing.Size(151, 21);
            this.comboProvider.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Provider name";
            // 
            // textADOConnection
            // 
            this.textADOConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textADOConnection.Location = new System.Drawing.Point(137, 242);
            this.textADOConnection.Name = "textADOConnection";
            this.textADOConnection.Size = new System.Drawing.Size(151, 20);
            this.textADOConnection.TabIndex = 15;
            // 
            // labelConnectionString
            // 
            this.labelConnectionString.AutoSize = true;
            this.labelConnectionString.Location = new System.Drawing.Point(17, 245);
            this.labelConnectionString.Name = "labelConnectionString";
            this.labelConnectionString.Size = new System.Drawing.Size(114, 13);
            this.labelConnectionString.TabIndex = 14;
            this.labelConnectionString.Text = "ADO ConnectionString";
            // 
            // bconnect
            // 
            this.bconnect.Location = new System.Drawing.Point(20, 282);
            this.bconnect.Name = "bconnect";
            this.bconnect.Size = new System.Drawing.Size(141, 26);
            this.bconnect.TabIndex = 20;
            this.bconnect.Text = "Connect";
            this.bconnect.UseVisualStyleBackColor = true;
            this.bconnect.Click += new System.EventHandler(this.bconnect_Click);
            // 
            // textReportGroupTable
            // 
            this.textReportGroupTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textReportGroupTable.Location = new System.Drawing.Point(137, 218);
            this.textReportGroupTable.Name = "textReportGroupTable";
            this.textReportGroupTable.Size = new System.Drawing.Size(151, 20);
            this.textReportGroupTable.TabIndex = 12;
            // 
            // labelReportGroupsTable
            // 
            this.labelReportGroupsTable.AutoSize = true;
            this.labelReportGroupsTable.Location = new System.Drawing.Point(17, 221);
            this.labelReportGroupsTable.Name = "labelReportGroupsTable";
            this.labelReportGroupsTable.Size = new System.Drawing.Size(78, 13);
            this.labelReportGroupsTable.TabIndex = 11;
            this.labelReportGroupsTable.Text = "R.Groups table";
            // 
            // textReportSearchField
            // 
            this.textReportSearchField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textReportSearchField.Location = new System.Drawing.Point(137, 192);
            this.textReportSearchField.Name = "textReportSearchField";
            this.textReportSearchField.Size = new System.Drawing.Size(151, 20);
            this.textReportSearchField.TabIndex = 10;
            // 
            // labelReportField
            // 
            this.labelReportField.AutoSize = true;
            this.labelReportField.Location = new System.Drawing.Point(17, 169);
            this.labelReportField.Name = "labelReportField";
            this.labelReportField.Size = new System.Drawing.Size(61, 13);
            this.labelReportField.TabIndex = 9;
            this.labelReportField.Text = "Report field";
            // 
            // textReportField
            // 
            this.textReportField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textReportField.Location = new System.Drawing.Point(137, 166);
            this.textReportField.Name = "textReportField";
            this.textReportField.Size = new System.Drawing.Size(151, 20);
            this.textReportField.TabIndex = 8;
            // 
            // labelReportSearchField
            // 
            this.labelReportSearchField.AutoSize = true;
            this.labelReportSearchField.Location = new System.Drawing.Point(17, 195);
            this.labelReportSearchField.Name = "labelReportSearchField";
            this.labelReportSearchField.Size = new System.Drawing.Size(74, 13);
            this.labelReportSearchField.TabIndex = 7;
            this.labelReportSearchField.Text = "R.Search field";
            // 
            // textReportTable
            // 
            this.textReportTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textReportTable.Location = new System.Drawing.Point(137, 140);
            this.textReportTable.Name = "textReportTable";
            this.textReportTable.Size = new System.Drawing.Size(151, 20);
            this.textReportTable.TabIndex = 6;
            // 
            // comboDriver
            // 
            this.comboDriver.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDriver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDriver.FormattingEnabled = true;
            this.comboDriver.Location = new System.Drawing.Point(137, 89);
            this.comboDriver.Name = "comboDriver";
            this.comboDriver.Size = new System.Drawing.Size(151, 21);
            this.comboDriver.TabIndex = 4;
            // 
            // labelReportTable
            // 
            this.labelReportTable.AutoSize = true;
            this.labelReportTable.Location = new System.Drawing.Point(17, 143);
            this.labelReportTable.Name = "labelReportTable";
            this.labelReportTable.Size = new System.Drawing.Size(65, 13);
            this.labelReportTable.TabIndex = 4;
            this.labelReportTable.Text = "Report table";
            // 
            // labelDriver
            // 
            this.labelDriver.AutoSize = true;
            this.labelDriver.Location = new System.Drawing.Point(17, 92);
            this.labelDriver.Name = "labelDriver";
            this.labelDriver.Size = new System.Drawing.Size(35, 13);
            this.labelDriver.TabIndex = 3;
            this.labelDriver.Text = "Driver";
            // 
            // checkLoginPrompt
            // 
            this.checkLoginPrompt.AutoSize = true;
            this.checkLoginPrompt.Location = new System.Drawing.Point(20, 59);
            this.checkLoginPrompt.Name = "checkLoginPrompt";
            this.checkLoginPrompt.Size = new System.Drawing.Size(87, 17);
            this.checkLoginPrompt.TabIndex = 2;
            this.checkLoginPrompt.Text = "Login prompt";
            this.checkLoginPrompt.UseVisualStyleBackColor = true;
            // 
            // checkLoadDriverParams
            // 
            this.checkLoadDriverParams.AutoSize = true;
            this.checkLoadDriverParams.Location = new System.Drawing.Point(20, 36);
            this.checkLoadDriverParams.Name = "checkLoadDriverParams";
            this.checkLoadDriverParams.Size = new System.Drawing.Size(134, 17);
            this.checkLoadDriverParams.TabIndex = 1;
            this.checkLoadDriverParams.Text = "Load driver parameters";
            this.checkLoadDriverParams.UseVisualStyleBackColor = true;
            // 
            // checkLoadParams
            // 
            this.checkLoadParams.AutoSize = true;
            this.checkLoadParams.Location = new System.Drawing.Point(20, 13);
            this.checkLoadParams.Name = "checkLoadParams";
            this.checkLoadParams.Size = new System.Drawing.Size(105, 17);
            this.checkLoadParams.TabIndex = 0;
            this.checkLoadParams.Text = "Load parameters";
            this.checkLoadParams.UseVisualStyleBackColor = true;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(170, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(10, 389);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // LConnections
            // 
            this.LConnections.DataSource = this.bindingSource1;
            this.LConnections.Dock = System.Windows.Forms.DockStyle.Left;
            this.LConnections.FormattingEnabled = true;
            this.LConnections.Location = new System.Drawing.Point(0, 0);
            this.LConnections.Name = "LConnections";
            this.LConnections.Size = new System.Drawing.Size(170, 389);
            this.LConnections.TabIndex = 0;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(Reportman.Reporting.ReportLibraryConfigCollection);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(19, 19);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bnew,
            this.bdelete});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(239, 26);
            this.toolStrip1.TabIndex = 0;
            // 
            // bnew
            // 
            this.bnew.Image = global::Reportman.Designer.Properties.Resources.addprops;
            this.bnew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bnew.Name = "bnew";
            this.bnew.Size = new System.Drawing.Size(117, 23);
            this.bnew.Text = "New connection";
            // 
            // bdelete
            // 
            this.bdelete.Image = global::Reportman.Designer.Properties.Resources.delete;
            this.bdelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bdelete.Name = "bdelete";
            this.bdelete.Size = new System.Drawing.Size(119, 23);
            this.bdelete.Text = "Drop connection";
            // 
            // LibraryConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "LibraryConfig";
            this.Size = new System.Drawing.Size(481, 415);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.PanelConnection.ResumeLayout(false);
            this.PanelConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Panel PanelConnection;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ListBox LConnections;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton bnew;
        private System.Windows.Forms.ToolStripButton bdelete;
        private System.Windows.Forms.Button bconnect;
        private System.Windows.Forms.TextBox textReportGroupTable;
        private System.Windows.Forms.Label labelReportGroupsTable;
        private System.Windows.Forms.TextBox textReportSearchField;
        private System.Windows.Forms.Label labelReportField;
        private System.Windows.Forms.TextBox textReportField;
        private System.Windows.Forms.Label labelReportSearchField;
        private System.Windows.Forms.TextBox textReportTable;
        private System.Windows.Forms.ComboBox comboDriver;
        private System.Windows.Forms.Label labelReportTable;
        private System.Windows.Forms.Label labelDriver;
        private System.Windows.Forms.CheckBox checkLoginPrompt;
        private System.Windows.Forms.CheckBox checkLoadDriverParams;
        private System.Windows.Forms.CheckBox checkLoadParams;
        private System.Windows.Forms.TextBox textADOConnection;
        private System.Windows.Forms.Label labelConnectionString;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.ComboBox comboProvider;
        private System.Windows.Forms.Label label1;
    }
}
