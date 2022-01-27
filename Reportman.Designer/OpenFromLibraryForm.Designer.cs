namespace Reportman.Designer
{
    partial class OpenFromLibraryForm
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
            this.panelclient = new System.Windows.Forms.Panel();
            this.openFromLibrary1 = new Reportman.Designer.OpenFromLibrary();
            this.panellib = new System.Windows.Forms.Panel();
            this.comboLibrary = new System.Windows.Forms.ComboBox();
            this.LabelLibrary = new System.Windows.Forms.Label();
            this.panelclient.SuspendLayout();
            this.panellib.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelclient
            // 
            this.panelclient.Controls.Add(this.openFromLibrary1);
            this.panelclient.Controls.Add(this.panellib);
            this.panelclient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelclient.Location = new System.Drawing.Point(0, 0);
            this.panelclient.Name = "panelclient";
            this.panelclient.Size = new System.Drawing.Size(807, 504);
            this.panelclient.TabIndex = 2;
            // 
            // openFromLibrary1
            // 
            this.openFromLibrary1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openFromLibrary1.Location = new System.Drawing.Point(0, 37);
            this.openFromLibrary1.Name = "openFromLibrary1";
            this.openFromLibrary1.ShowOkCancel = true;
            this.openFromLibrary1.Size = new System.Drawing.Size(807, 467);
            this.openFromLibrary1.TabIndex = 1;
            this.openFromLibrary1.Load += new System.EventHandler(this.openFromLibrary1_Load);
            // 
            // panellib
            // 
            this.panellib.Controls.Add(this.comboLibrary);
            this.panellib.Controls.Add(this.LabelLibrary);
            this.panellib.Dock = System.Windows.Forms.DockStyle.Top;
            this.panellib.Location = new System.Drawing.Point(0, 0);
            this.panellib.Name = "panellib";
            this.panellib.Size = new System.Drawing.Size(807, 37);
            this.panellib.TabIndex = 0;
            // 
            // comboLibrary
            // 
            this.comboLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboLibrary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLibrary.FormattingEnabled = true;
            this.comboLibrary.Location = new System.Drawing.Point(162, 7);
            this.comboLibrary.Name = "comboLibrary";
            this.comboLibrary.Size = new System.Drawing.Size(633, 21);
            this.comboLibrary.TabIndex = 1;
            this.comboLibrary.SelectedIndexChanged += new System.EventHandler(this.comboLibrary_SelectedIndexChanged);
            // 
            // LabelLibrary
            // 
            this.LabelLibrary.AutoSize = true;
            this.LabelLibrary.Location = new System.Drawing.Point(12, 9);
            this.LabelLibrary.Name = "LabelLibrary";
            this.LabelLibrary.Size = new System.Drawing.Size(38, 13);
            this.LabelLibrary.TabIndex = 0;
            this.LabelLibrary.Text = "Library";
            // 
            // OpenFromLibraryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 504);
            this.Controls.Add(this.panelclient);
            this.Name = "OpenFromLibraryForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OpenFromLibraryForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OpenFromLibraryForm_FormClosed);
            this.Load += new System.EventHandler(this.OpenFromLibraryForm_Load);
            this.panelclient.ResumeLayout(false);
            this.panellib.ResumeLayout(false);
            this.panellib.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelclient;
        private System.Windows.Forms.Panel panellib;
        private System.Windows.Forms.Label LabelLibrary;
        private OpenFromLibrary openFromLibrary1;
        private System.Windows.Forms.ComboBox comboLibrary;
    }
}