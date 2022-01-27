namespace Reportman.Drawing.Forms
{
    partial class PrintersConfiguration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bok = new System.Windows.Forms.Button();
            this.bcancel = new System.Windows.Forms.Button();
            this.lprinterdevice = new System.Windows.Forms.Label();
            this.comboprinters = new System.Windows.Forms.ComboBox();
            this.gconfigfile = new System.Windows.Forms.GroupBox();
            this.textconfigfile = new System.Windows.Forms.TextBox();
            this.radiosystemconfig = new System.Windows.Forms.RadioButton();
            this.radiouserconfig = new System.Windows.Forms.RadioButton();
            this.combotextdriver = new System.Windows.Forms.ComboBox();
            this.ltextdriver = new System.Windows.Forms.Label();
            this.checkoem = new System.Windows.Forms.CheckBox();
            this.gtextdriveroptions = new System.Windows.Forms.GroupBox();
            this.goperations = new System.Windows.Forms.GroupBox();
            this.lsample2 = new System.Windows.Forms.Label();
            this.lsample1 = new System.Windows.Forms.Label();
            this.textescapeopendrawer = new System.Windows.Forms.TextBox();
            this.textescapecutpaper = new System.Windows.Forms.TextBox();
            this.checkopendrawer = new System.Windows.Forms.CheckBox();
            this.checkcutpaper = new System.Windows.Forms.CheckBox();
            this.ldefinedprinters = new System.Windows.Forms.ListView();
            this.colid = new System.Windows.Forms.ColumnHeader();
            this.colname = new System.Windows.Forms.ColumnHeader();
            this.colplainname = new System.Windows.Forms.ColumnHeader();
            this.gconfigfile.SuspendLayout();
            this.gtextdriveroptions.SuspendLayout();
            this.goperations.SuspendLayout();
            this.SuspendLayout();
            // 
            // bok
            // 
            this.bok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bok.Location = new System.Drawing.Point(354, 357);
            this.bok.Name = "bok";
            this.bok.Size = new System.Drawing.Size(124, 37);
            this.bok.TabIndex = 0;
            this.bok.Text = "OK";
            this.bok.UseVisualStyleBackColor = true;
            this.bok.Click += new System.EventHandler(this.bok_Click);
            // 
            // bcancel
            // 
            this.bcancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bcancel.Location = new System.Drawing.Point(509, 357);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(124, 37);
            this.bcancel.TabIndex = 1;
            this.bcancel.Text = "Cancel";
            this.bcancel.UseVisualStyleBackColor = true;
            this.bcancel.Click += new System.EventHandler(this.bcancel_Click);
            // 
            // lprinterdevice
            // 
            this.lprinterdevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lprinterdevice.AutoSize = true;
            this.lprinterdevice.Location = new System.Drawing.Point(351, 9);
            this.lprinterdevice.Name = "lprinterdevice";
            this.lprinterdevice.Size = new System.Drawing.Size(93, 13);
            this.lprinterdevice.TabIndex = 3;
            this.lprinterdevice.Text = "Use printer device";
            // 
            // comboprinters
            // 
            this.comboprinters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboprinters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboprinters.FormattingEnabled = true;
            this.comboprinters.Location = new System.Drawing.Point(354, 32);
            this.comboprinters.Name = "comboprinters";
            this.comboprinters.Size = new System.Drawing.Size(429, 21);
            this.comboprinters.TabIndex = 4;
            this.comboprinters.SelectedIndexChanged += new System.EventHandler(this.comboprinters_SelectedIndexChanged);
            // 
            // gconfigfile
            // 
            this.gconfigfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gconfigfile.Controls.Add(this.textconfigfile);
            this.gconfigfile.Controls.Add(this.radiosystemconfig);
            this.gconfigfile.Controls.Add(this.radiouserconfig);
            this.gconfigfile.Location = new System.Drawing.Point(2, 400);
            this.gconfigfile.Name = "gconfigfile";
            this.gconfigfile.Size = new System.Drawing.Size(783, 81);
            this.gconfigfile.TabIndex = 5;
            this.gconfigfile.TabStop = false;
            this.gconfigfile.Text = "Configuration fle";
            // 
            // textconfigfile
            // 
            this.textconfigfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textconfigfile.Location = new System.Drawing.Point(20, 55);
            this.textconfigfile.Name = "textconfigfile";
            this.textconfigfile.ReadOnly = true;
            this.textconfigfile.Size = new System.Drawing.Size(757, 20);
            this.textconfigfile.TabIndex = 2;
            // 
            // radiosystemconfig
            // 
            this.radiosystemconfig.AutoSize = true;
            this.radiosystemconfig.Location = new System.Drawing.Point(272, 19);
            this.radiosystemconfig.Name = "radiosystemconfig";
            this.radiosystemconfig.Size = new System.Drawing.Size(224, 17);
            this.radiosystemconfig.TabIndex = 1;
            this.radiosystemconfig.TabStop = true;
            this.radiosystemconfig.Text = "System configuration, will apply to all users";
            this.radiosystemconfig.UseVisualStyleBackColor = true;
            this.radiosystemconfig.CheckedChanged += new System.EventHandler(this.radiouserconfig_CheckedChanged);
            // 
            // radiouserconfig
            // 
            this.radiouserconfig.AutoSize = true;
            this.radiouserconfig.Location = new System.Drawing.Point(20, 19);
            this.radiouserconfig.Name = "radiouserconfig";
            this.radiouserconfig.Size = new System.Drawing.Size(111, 17);
            this.radiouserconfig.TabIndex = 0;
            this.radiouserconfig.TabStop = true;
            this.radiouserconfig.Text = "User configuration";
            this.radiouserconfig.UseVisualStyleBackColor = true;
            this.radiouserconfig.CheckedChanged += new System.EventHandler(this.radiouserconfig_CheckedChanged);
            // 
            // combotextdriver
            // 
            this.combotextdriver.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.combotextdriver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combotextdriver.FormattingEnabled = true;
            this.combotextdriver.Location = new System.Drawing.Point(146, 23);
            this.combotextdriver.Name = "combotextdriver";
            this.combotextdriver.Size = new System.Drawing.Size(277, 21);
            this.combotextdriver.TabIndex = 9;
            this.combotextdriver.SelectedIndexChanged += new System.EventHandler(this.combotextdriver_SelectedIndexChanged);
            // 
            // ltextdriver
            // 
            this.ltextdriver.AutoSize = true;
            this.ltextdriver.Location = new System.Drawing.Point(6, 26);
            this.ltextdriver.Name = "ltextdriver";
            this.ltextdriver.Size = new System.Drawing.Size(70, 13);
            this.ltextdriver.TabIndex = 8;
            this.ltextdriver.Text = "Device driver";
            // 
            // checkoem
            // 
            this.checkoem.AutoSize = true;
            this.checkoem.Location = new System.Drawing.Point(9, 50);
            this.checkoem.Name = "checkoem";
            this.checkoem.Size = new System.Drawing.Size(183, 17);
            this.checkoem.TabIndex = 10;
            this.checkoem.Text = "Character set conversion to OEM";
            this.checkoem.UseVisualStyleBackColor = true;
            this.checkoem.CheckedChanged += new System.EventHandler(this.checkoem_CheckedChanged);
            // 
            // gtextdriveroptions
            // 
            this.gtextdriveroptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gtextdriveroptions.Controls.Add(this.ltextdriver);
            this.gtextdriveroptions.Controls.Add(this.checkoem);
            this.gtextdriveroptions.Controls.Add(this.combotextdriver);
            this.gtextdriveroptions.Location = new System.Drawing.Point(354, 59);
            this.gtextdriveroptions.Name = "gtextdriveroptions";
            this.gtextdriveroptions.Size = new System.Drawing.Size(429, 85);
            this.gtextdriveroptions.TabIndex = 11;
            this.gtextdriveroptions.TabStop = false;
            this.gtextdriveroptions.Text = "Text driver options";
            // 
            // goperations
            // 
            this.goperations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.goperations.Controls.Add(this.lsample2);
            this.goperations.Controls.Add(this.lsample1);
            this.goperations.Controls.Add(this.textescapeopendrawer);
            this.goperations.Controls.Add(this.textescapecutpaper);
            this.goperations.Controls.Add(this.checkopendrawer);
            this.goperations.Controls.Add(this.checkcutpaper);
            this.goperations.Location = new System.Drawing.Point(354, 150);
            this.goperations.Name = "goperations";
            this.goperations.Size = new System.Drawing.Size(429, 150);
            this.goperations.TabIndex = 12;
            this.goperations.TabStop = false;
            this.goperations.Text = "Operations after print";
            // 
            // lsample2
            // 
            this.lsample2.AutoSize = true;
            this.lsample2.Location = new System.Drawing.Point(6, 124);
            this.lsample2.Name = "lsample2";
            this.lsample2.Size = new System.Drawing.Size(273, 13);
            this.lsample2.TabIndex = 5;
            this.lsample2.Text = "Example, TM88 Open Drawer: #27#112#48#40#200#4";
            // 
            // lsample1
            // 
            this.lsample1.AutoSize = true;
            this.lsample1.Location = new System.Drawing.Point(6, 96);
            this.lsample1.Name = "lsample1";
            this.lsample1.Size = new System.Drawing.Size(285, 13);
            this.lsample1.TabIndex = 4;
            this.lsample1.Text = "Example, TM200 Open Drawer: #27#112#48#160#160#4";
            // 
            // textescapeopendrawer
            // 
            this.textescapeopendrawer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textescapeopendrawer.Location = new System.Drawing.Point(144, 61);
            this.textescapeopendrawer.Name = "textescapeopendrawer";
            this.textescapeopendrawer.Size = new System.Drawing.Size(276, 20);
            this.textescapeopendrawer.TabIndex = 3;
            this.textescapeopendrawer.TextChanged += new System.EventHandler(this.textescapeopendrawer_TextChanged);
            // 
            // textescapecutpaper
            // 
            this.textescapecutpaper.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textescapecutpaper.Location = new System.Drawing.Point(144, 29);
            this.textescapecutpaper.Name = "textescapecutpaper";
            this.textescapecutpaper.Size = new System.Drawing.Size(276, 20);
            this.textescapecutpaper.TabIndex = 2;
            this.textescapecutpaper.TextChanged += new System.EventHandler(this.textescapecutpaper_TextChanged);
            // 
            // checkopendrawer
            // 
            this.checkopendrawer.AutoSize = true;
            this.checkopendrawer.Location = new System.Drawing.Point(8, 63);
            this.checkopendrawer.Name = "checkopendrawer";
            this.checkopendrawer.Size = new System.Drawing.Size(87, 17);
            this.checkopendrawer.TabIndex = 1;
            this.checkopendrawer.Text = "Open drawer";
            this.checkopendrawer.UseVisualStyleBackColor = true;
            this.checkopendrawer.CheckedChanged += new System.EventHandler(this.checkopendrawer_CheckedChanged);
            // 
            // checkcutpaper
            // 
            this.checkcutpaper.AutoSize = true;
            this.checkcutpaper.Location = new System.Drawing.Point(8, 31);
            this.checkcutpaper.Name = "checkcutpaper";
            this.checkcutpaper.Size = new System.Drawing.Size(72, 17);
            this.checkcutpaper.TabIndex = 0;
            this.checkcutpaper.Text = "Cut paper";
            this.checkcutpaper.UseVisualStyleBackColor = true;
            this.checkcutpaper.CheckedChanged += new System.EventHandler(this.checkcutpaper_CheckedChanged);
            // 
            // ldefinedprinters
            // 
            this.ldefinedprinters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ldefinedprinters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colid,
            this.colname,
            this.colplainname});
            this.ldefinedprinters.FullRowSelect = true;
            this.ldefinedprinters.HideSelection = false;
            this.ldefinedprinters.Location = new System.Drawing.Point(2, 1);
            this.ldefinedprinters.MultiSelect = false;
            this.ldefinedprinters.Name = "ldefinedprinters";
            this.ldefinedprinters.Size = new System.Drawing.Size(343, 393);
            this.ldefinedprinters.TabIndex = 13;
            this.ldefinedprinters.UseCompatibleStateImageBehavior = false;
            this.ldefinedprinters.View = System.Windows.Forms.View.Details;
            this.ldefinedprinters.SelectedIndexChanged += new System.EventHandler(this.ldefinedprinters_SelectedIndexChanged_1);
            // 
            // colid
            // 
            this.colid.Text = "Id";
            this.colid.Width = 25;
            // 
            // colname
            // 
            this.colname.Text = "Print Id";
            this.colname.Width = 200;
            // 
            // colplainname
            // 
            this.colplainname.Text = "Name";
            this.colplainname.Width = 400;
            // 
            // PrintersConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 488);
            this.Controls.Add(this.ldefinedprinters);
            this.Controls.Add(this.goperations);
            this.Controls.Add(this.gtextdriveroptions);
            this.Controls.Add(this.gconfigfile);
            this.Controls.Add(this.comboprinters);
            this.Controls.Add(this.lprinterdevice);
            this.Controls.Add(this.bcancel);
            this.Controls.Add(this.bok);
            this.Name = "PrintersConfiguration";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Printer configuration";
            this.Load += new System.EventHandler(this.PrintersConfiguration_Load);
            this.gconfigfile.ResumeLayout(false);
            this.gconfigfile.PerformLayout();
            this.gtextdriveroptions.ResumeLayout(false);
            this.gtextdriveroptions.PerformLayout();
            this.goperations.ResumeLayout(false);
            this.goperations.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bok;
        private System.Windows.Forms.Button bcancel;
        private System.Windows.Forms.Label lprinterdevice;
        private System.Windows.Forms.ComboBox comboprinters;
        private System.Windows.Forms.GroupBox gconfigfile;
        private System.Windows.Forms.TextBox textconfigfile;
        private System.Windows.Forms.RadioButton radiosystemconfig;
        private System.Windows.Forms.RadioButton radiouserconfig;
        private System.Windows.Forms.ComboBox combotextdriver;
        private System.Windows.Forms.Label ltextdriver;
        private System.Windows.Forms.CheckBox checkoem;
        private System.Windows.Forms.GroupBox gtextdriveroptions;
        private System.Windows.Forms.GroupBox goperations;
        private System.Windows.Forms.Label lsample1;
        private System.Windows.Forms.TextBox textescapeopendrawer;
        private System.Windows.Forms.TextBox textescapecutpaper;
        private System.Windows.Forms.CheckBox checkopendrawer;
        private System.Windows.Forms.CheckBox checkcutpaper;
        private System.Windows.Forms.Label lsample2;
        private System.Windows.Forms.ListView ldefinedprinters;
        private System.Windows.Forms.ColumnHeader colid;
        private System.Windows.Forms.ColumnHeader colname;
        private System.Windows.Forms.ColumnHeader colplainname;
    }
}