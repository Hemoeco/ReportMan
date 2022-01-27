namespace Reportman.Drawing.Forms
{
    partial class InputBox
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.datepicker = new System.Windows.Forms.DateTimePicker();
            this.EditText = new System.Windows.Forms.TextBox();
            this.ltext = new System.Windows.Forms.Label();
            this.maintable = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.bok = new System.Windows.Forms.Button();
            this.bcancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.maintable.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // datepicker
            // 
            this.datepicker.CustomFormat = "dd/MM/yy";
            this.datepicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datepicker.Location = new System.Drawing.Point(12, 3);
            this.datepicker.Name = "datepicker";
            this.datepicker.Size = new System.Drawing.Size(86, 20);
            this.datepicker.TabIndex = 9;
            this.datepicker.Visible = false;
            // 
            // EditText
            // 
            this.EditText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditText.Location = new System.Drawing.Point(3, 29);
            this.EditText.Name = "EditText";
            this.EditText.Size = new System.Drawing.Size(632, 20);
            this.EditText.TabIndex = 8;
            // 
            // ltext
            // 
            this.ltext.AutoSize = true;
            this.ltext.Dock = System.Windows.Forms.DockStyle.Top;
            this.ltext.Location = new System.Drawing.Point(3, 0);
            this.ltext.Name = "ltext";
            this.ltext.Size = new System.Drawing.Size(632, 26);
            this.ltext.TabIndex = 7;
            this.ltext.Text = "label1 kkd dfdsafd dsfds fdsfds fdsfds fdsfds fdsfdsf fdsfds fdsfds fdsfdsa fsdaf" +
                "dsf fdasfsda fdasfds fdsafd adf sad dfdsaf s dsfd dfaaf dsf ";
            // 
            // maintable
            // 
            this.maintable.AutoSize = true;
            this.maintable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.maintable.ColumnCount = 1;
            this.maintable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.maintable.Controls.Add(this.tableLayoutPanel2, 0, 3);
            this.maintable.Controls.Add(this.ltext, 0, 0);
            this.maintable.Controls.Add(this.EditText, 0, 1);
            this.maintable.Controls.Add(this.panel1, 0, 2);
            this.maintable.Dock = System.Windows.Forms.DockStyle.Top;
            this.maintable.Location = new System.Drawing.Point(0, 0);
            this.maintable.Name = "maintable";
            this.maintable.RowCount = 4;
            this.maintable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.maintable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.maintable.Size = new System.Drawing.Size(638, 142);
            this.maintable.TabIndex = 10;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.bok, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.bcancel, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 100);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(632, 39);
            this.tableLayoutPanel2.TabIndex = 12;
            // 
            // bok
            // 
            this.bok.Dock = System.Windows.Forms.DockStyle.Left;
            this.bok.Location = new System.Drawing.Point(3, 3);
            this.bok.Name = "bok";
            this.bok.Size = new System.Drawing.Size(129, 33);
            this.bok.TabIndex = 5;
            this.bok.Text = "OK";
            this.bok.UseVisualStyleBackColor = true;
            this.bok.Click += new System.EventHandler(this.bok_Click);
            // 
            // bcancel
            // 
            this.bcancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.bcancel.Location = new System.Drawing.Point(510, 3);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(119, 33);
            this.bcancel.TabIndex = 6;
            this.bcancel.Text = "Cancel";
            this.bcancel.UseVisualStyleBackColor = true;
            this.bcancel.Click += new System.EventHandler(this.bcancel_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(3, 55);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(114, 36);
            this.panel1.TabIndex = 9;
            // 
            // InputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 151);
            this.Controls.Add(this.datepicker);
            this.Controls.Add(this.maintable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "InputBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Input Box";
            this.Load += new System.EventHandler(this.InputBox_Load);
            this.Shown += new System.EventHandler(this.InputBox_Shown);
            this.maintable.ResumeLayout(false);
            this.maintable.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker datepicker;
        private System.Windows.Forms.TextBox EditText;
        private System.Windows.Forms.Label ltext;
        private System.Windows.Forms.TableLayoutPanel maintable;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button bok;
        private System.Windows.Forms.Button bcancel;
        private System.Windows.Forms.Panel panel1;
    }
}