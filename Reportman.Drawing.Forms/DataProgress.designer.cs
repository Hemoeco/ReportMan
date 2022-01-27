namespace Reportman.Reporting.Forms
{
    partial class DataProgress
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
            this.components = new System.ComponentModel.Container();
            this.lprogress = new System.Windows.Forms.Label();
            this.progbar = new System.Windows.Forms.ProgressBar();
            this.bcancel = new System.Windows.Forms.Button();
            this.timerexecute = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lprogress
            // 
            this.lprogress.AutoSize = true;
            this.lprogress.Location = new System.Drawing.Point(22, 18);
            this.lprogress.Name = "lprogress";
            this.lprogress.Size = new System.Drawing.Size(46, 17);
            this.lprogress.TabIndex = 0;
            this.lprogress.Text = "label1";
            // 
            // progbar
            // 
            this.progbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progbar.Location = new System.Drawing.Point(25, 53);
            this.progbar.Name = "progbar";
            this.progbar.Size = new System.Drawing.Size(515, 56);
            this.progbar.TabIndex = 1;
            // 
            // bcancel
            // 
            this.bcancel.Location = new System.Drawing.Point(206, 137);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(122, 33);
            this.bcancel.TabIndex = 2;
            this.bcancel.Text = "Cancel";
            this.bcancel.UseVisualStyleBackColor = true;
            this.bcancel.Click += new System.EventHandler(this.bcancel_Click);
            // 
            // timerexecute
            // 
            this.timerexecute.Interval = 1;
            this.timerexecute.Tick += new System.EventHandler(this.timerexecute_Tick);
            // 
            // DataProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 197);
            this.Controls.Add(this.bcancel);
            this.Controls.Add(this.progbar);
            this.Controls.Add(this.lprogress);
            this.Name = "DataProgress";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Progress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lprogress;
        private System.Windows.Forms.ProgressBar progbar;
        private System.Windows.Forms.Button bcancel;
        private System.Windows.Forms.Timer timerexecute;
    }
}