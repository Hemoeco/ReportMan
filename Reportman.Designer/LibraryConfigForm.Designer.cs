namespace Reportman.Designer
{
    partial class LibraryConfigForm
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
            this.panelbottom = new System.Windows.Forms.Panel();
            this.bcancel = new System.Windows.Forms.Button();
            this.bok = new System.Windows.Forms.Button();
            this.libConfig = new Reportman.Designer.LibraryConfig();
            this.panelbottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelbottom
            // 
            this.panelbottom.Controls.Add(this.bcancel);
            this.panelbottom.Controls.Add(this.bok);
            this.panelbottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelbottom.Location = new System.Drawing.Point(0, 444);
            this.panelbottom.Name = "panelbottom";
            this.panelbottom.Size = new System.Drawing.Size(622, 50);
            this.panelbottom.TabIndex = 0;
            // 
            // bcancel
            // 
            this.bcancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bcancel.Location = new System.Drawing.Point(225, 8);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(115, 33);
            this.bcancel.TabIndex = 3;
            this.bcancel.Text = "Cancel";
            this.bcancel.Click += new System.EventHandler(this.bcancel_Click);
            // 
            // bok
            // 
            this.bok.Location = new System.Drawing.Point(12, 9);
            this.bok.Name = "bok";
            this.bok.Size = new System.Drawing.Size(112, 32);
            this.bok.TabIndex = 2;
            this.bok.Text = "OK";
            this.bok.Click += new System.EventHandler(this.bok_Click);
            // 
            // libConfig
            // 
            this.libConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.libConfig.Location = new System.Drawing.Point(0, 0);
            this.libConfig.Name = "libConfig";
            this.libConfig.Size = new System.Drawing.Size(622, 444);
            this.libConfig.TabIndex = 1;
            // 
            // LibraryConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 494);
            this.Controls.Add(this.libConfig);
            this.Controls.Add(this.panelbottom);
            this.Name = "LibraryConfigForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configure Libraries";
            this.panelbottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelbottom;
        private System.Windows.Forms.Button bcancel;
        private System.Windows.Forms.Button bok;
        private LibraryConfig libConfig;
    }
}