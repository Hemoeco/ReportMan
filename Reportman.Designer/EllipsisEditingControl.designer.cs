namespace Reportman.Designer
{
	partial class EllipsisEditingControl
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

		#region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textcontrol = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textcontrol
            // 
            this.textcontrol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textcontrol.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textcontrol.Location = new System.Drawing.Point(0, 1);
            this.textcontrol.Name = "textcontrol";
            this.textcontrol.Size = new System.Drawing.Size(81, 13);
            this.textcontrol.TabIndex = 0;
            // 
            // browseButton
            // 
            this.browseButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.browseButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.browseButton.Location = new System.Drawing.Point(83, 0);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(25, 24);
            this.browseButton.TabIndex = 1;
            this.browseButton.Text = "...";
            this.browseButton.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.browseButton.UseVisualStyleBackColor = false;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // EllipsisEditingControl
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.textcontrol);
            this.Name = "EllipsisEditingControl";
            this.Size = new System.Drawing.Size(108, 24);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		#endregion

		private System.Windows.Forms.TextBox textcontrol;
        private System.Windows.Forms.Button browseButton;
	}
}
