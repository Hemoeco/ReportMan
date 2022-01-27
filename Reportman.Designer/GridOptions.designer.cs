namespace Reportman.Designer
{
    partial class GridOptions
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
            this.lhorzontal = new System.Windows.Forms.Label();
            this.textwidth = new Reportman.Drawing.Forms.TextBoxAdvanced();
            this.textheight = new Reportman.Drawing.Forms.TextBoxAdvanced();
            this.lvertical = new System.Windows.Forms.Label();
            this.lcombogridstyle = new System.Windows.Forms.Label();
            this.combostyle = new System.Windows.Forms.ComboBox();
            this.lcolor = new System.Windows.Forms.Label();
            this.bcolor = new System.Windows.Forms.Button();
            this.bok = new System.Windows.Forms.Button();
            this.bcancel = new System.Windows.Forms.Button();
            this.colordialog1 = new System.Windows.Forms.ColorDialog();
            this.checkenabled = new System.Windows.Forms.CheckBox();
            this.lenabled = new System.Windows.Forms.Label();
            this.lvisible = new System.Windows.Forms.Label();
            this.checkvisible = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lhorzontal
            // 
            this.lhorzontal.AutoSize = true;
            this.lhorzontal.Location = new System.Drawing.Point(12, 20);
            this.lhorzontal.Name = "lhorzontal";
            this.lhorzontal.Size = new System.Drawing.Size(35, 13);
            this.lhorzontal.TabIndex = 0;
            this.lhorzontal.Text = "Width";
            // 
            // textwidth
            // 
            this.textwidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textwidth.BarCodeBeginChar = '$';
            this.textwidth.BarCodeEndChar = '%';
            this.textwidth.DataType = Reportman.Drawing.Forms.TextBoxDataType.Numeric;
            this.textwidth.Location = new System.Drawing.Point(148, 17);
            this.textwidth.Name = "textwidth";
            this.textwidth.ReadBarCode = false;
            this.textwidth.Size = new System.Drawing.Size(222, 20);
            this.textwidth.TabIndex = 1;
            this.textwidth.Text = "0";
            // 
            // textheight
            // 
            this.textheight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textheight.BarCodeBeginChar = '$';
            this.textheight.BarCodeEndChar = '%';
            this.textheight.DataType = Reportman.Drawing.Forms.TextBoxDataType.Numeric;
            this.textheight.Location = new System.Drawing.Point(148, 48);
            this.textheight.Name = "textheight";
            this.textheight.ReadBarCode = false;
            this.textheight.Size = new System.Drawing.Size(222, 20);
            this.textheight.TabIndex = 3;
            this.textheight.Text = "0";
            // 
            // lvertical
            // 
            this.lvertical.AutoSize = true;
            this.lvertical.Location = new System.Drawing.Point(12, 51);
            this.lvertical.Name = "lvertical";
            this.lvertical.Size = new System.Drawing.Size(38, 13);
            this.lvertical.TabIndex = 2;
            this.lvertical.Text = "Height";
            // 
            // lcombogridstyle
            // 
            this.lcombogridstyle.AutoSize = true;
            this.lcombogridstyle.Location = new System.Drawing.Point(12, 83);
            this.lcombogridstyle.Name = "lcombogridstyle";
            this.lcombogridstyle.Size = new System.Drawing.Size(30, 13);
            this.lcombogridstyle.TabIndex = 4;
            this.lcombogridstyle.Text = "Style";
            // 
            // combostyle
            // 
            this.combostyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.combostyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combostyle.FormattingEnabled = true;
            this.combostyle.Items.AddRange(new object[] {
            "Points",
            "Lines"});
            this.combostyle.Location = new System.Drawing.Point(148, 80);
            this.combostyle.Name = "combostyle";
            this.combostyle.Size = new System.Drawing.Size(222, 21);
            this.combostyle.TabIndex = 5;
            // 
            // lcolor
            // 
            this.lcolor.AutoSize = true;
            this.lcolor.Location = new System.Drawing.Point(12, 124);
            this.lcolor.Name = "lcolor";
            this.lcolor.Size = new System.Drawing.Size(31, 13);
            this.lcolor.TabIndex = 6;
            this.lcolor.Text = "Color";
            // 
            // bcolor
            // 
            this.bcolor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bcolor.Location = new System.Drawing.Point(148, 117);
            this.bcolor.Name = "bcolor";
            this.bcolor.Size = new System.Drawing.Size(222, 26);
            this.bcolor.TabIndex = 7;
            this.bcolor.UseVisualStyleBackColor = true;
            this.bcolor.Click += new System.EventHandler(this.bcolor_Click);
            // 
            // bok
            // 
            this.bok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bok.Location = new System.Drawing.Point(15, 235);
            this.bok.Name = "bok";
            this.bok.Size = new System.Drawing.Size(107, 33);
            this.bok.TabIndex = 15;
            this.bok.Text = "OK";
            this.bok.UseVisualStyleBackColor = true;
            // 
            // bcancel
            // 
            this.bcancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bcancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bcancel.Location = new System.Drawing.Point(247, 235);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(126, 33);
            this.bcancel.TabIndex = 16;
            this.bcancel.Text = "Cancel";
            this.bcancel.UseVisualStyleBackColor = true;
            // 
            // checkenabled
            // 
            this.checkenabled.AutoSize = true;
            this.checkenabled.Location = new System.Drawing.Point(148, 162);
            this.checkenabled.Name = "checkenabled";
            this.checkenabled.Size = new System.Drawing.Size(15, 14);
            this.checkenabled.TabIndex = 8;
            this.checkenabled.UseVisualStyleBackColor = true;
            // 
            // lenabled
            // 
            this.lenabled.AutoSize = true;
            this.lenabled.Location = new System.Drawing.Point(12, 162);
            this.lenabled.Name = "lenabled";
            this.lenabled.Size = new System.Drawing.Size(46, 13);
            this.lenabled.TabIndex = 12;
            this.lenabled.Text = "Enabled";
            // 
            // lvisible
            // 
            this.lvisible.AutoSize = true;
            this.lvisible.Location = new System.Drawing.Point(12, 195);
            this.lvisible.Name = "lvisible";
            this.lvisible.Size = new System.Drawing.Size(37, 13);
            this.lvisible.TabIndex = 14;
            this.lvisible.Text = "Visible";
            // 
            // checkvisible
            // 
            this.checkvisible.AutoSize = true;
            this.checkvisible.Location = new System.Drawing.Point(148, 194);
            this.checkvisible.Name = "checkvisible";
            this.checkvisible.Size = new System.Drawing.Size(15, 14);
            this.checkvisible.TabIndex = 13;
            this.checkvisible.UseVisualStyleBackColor = true;
            // 
            // GridOptions
            // 
            this.AcceptButton = this.bok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 280);
            this.Controls.Add(this.lvisible);
            this.Controls.Add(this.checkvisible);
            this.Controls.Add(this.lenabled);
            this.Controls.Add(this.checkenabled);
            this.Controls.Add(this.bcancel);
            this.Controls.Add(this.bok);
            this.Controls.Add(this.bcolor);
            this.Controls.Add(this.lcolor);
            this.Controls.Add(this.combostyle);
            this.Controls.Add(this.lcombogridstyle);
            this.Controls.Add(this.textheight);
            this.Controls.Add(this.lvertical);
            this.Controls.Add(this.textwidth);
            this.Controls.Add(this.lhorzontal);
            this.Name = "GridOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Grid";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lhorzontal;
        private Reportman.Drawing.Forms.TextBoxAdvanced textwidth;
        private Reportman.Drawing.Forms.TextBoxAdvanced textheight;
        private System.Windows.Forms.Label lvertical;
        private System.Windows.Forms.Label lcombogridstyle;
        private System.Windows.Forms.ComboBox combostyle;
        private System.Windows.Forms.Label lcolor;
        private System.Windows.Forms.Button bcolor;
        private System.Windows.Forms.Button bok;
        private System.Windows.Forms.Button bcancel;
        private System.Windows.Forms.ColorDialog colordialog1;
        private System.Windows.Forms.CheckBox checkenabled;
        private System.Windows.Forms.Label lenabled;
        private System.Windows.Forms.Label lvisible;
        private System.Windows.Forms.CheckBox checkvisible;
    }
}