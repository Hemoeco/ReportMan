namespace Reportman.Designer
{
  partial class PropertyFrame
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

    #region Código generado por el Diseñador de componentes

    /// <summary> 
    /// Método necesario para admitir el Diseñador. No se puede modificar
    /// el contenido del método con el editor de código.
    /// </summary>
    private void InitializeComponent()
    {
            this.comboselection = new System.Windows.Forms.ComboBox();
            this.panelclient = new System.Windows.Forms.Panel();
            this.tablepanel = new System.Windows.Forms.TableLayoutPanel();
            this.bforward = new System.Windows.Forms.Button();
            this.bback = new System.Windows.Forms.Button();
            this.tablepanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboselection
            // 
            this.comboselection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tablepanel.SetColumnSpan(this.comboselection, 2);
            this.comboselection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboselection.FormattingEnabled = true;
            this.comboselection.Location = new System.Drawing.Point(3, 3);
            this.comboselection.Name = "comboselection";
            this.comboselection.Size = new System.Drawing.Size(240, 21);
            this.comboselection.TabIndex = 0;
            this.comboselection.SelectedIndexChanged += new System.EventHandler(this.comboselection_SelectedIndexChanged);
            this.comboselection.Click += new System.EventHandler(this.comboselection_Click);
            // 
            // panelclient
            // 
            this.tablepanel.SetColumnSpan(this.panelclient, 2);
            this.panelclient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelclient.Location = new System.Drawing.Point(3, 30);
            this.panelclient.Name = "panelclient";
            this.panelclient.Size = new System.Drawing.Size(240, 233);
            this.panelclient.TabIndex = 0;
            // 
            // tablepanel
            // 
            this.tablepanel.ColumnCount = 2;
            this.tablepanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablepanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablepanel.Controls.Add(this.bforward, 1, 2);
            this.tablepanel.Controls.Add(this.panelclient, 0, 1);
            this.tablepanel.Controls.Add(this.comboselection, 0, 0);
            this.tablepanel.Controls.Add(this.bback, 0, 2);
            this.tablepanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablepanel.Location = new System.Drawing.Point(0, 0);
            this.tablepanel.Name = "tablepanel";
            this.tablepanel.RowCount = 3;
            this.tablepanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tablepanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablepanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tablepanel.Size = new System.Drawing.Size(246, 297);
            this.tablepanel.TabIndex = 0;
            // 
            // bforward
            // 
            this.bforward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.bforward.Location = new System.Drawing.Point(126, 269);
            this.bforward.Name = "bforward";
            this.bforward.Size = new System.Drawing.Size(117, 25);
            this.bforward.TabIndex = 2;
            this.bforward.Text = "Front";
            this.bforward.UseVisualStyleBackColor = true;
            this.bforward.Click += new System.EventHandler(this.bforward_Click);
            // 
            // bback
            // 
            this.bback.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.bback.Location = new System.Drawing.Point(3, 269);
            this.bback.Name = "bback";
            this.bback.Size = new System.Drawing.Size(117, 25);
            this.bback.TabIndex = 1;
            this.bback.Text = "Back";
            this.bback.UseVisualStyleBackColor = true;
            this.bback.Click += new System.EventHandler(this.bback_Click);
            // 
            // PropertyFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tablepanel);
            this.Name = "PropertyFrame";
            this.Size = new System.Drawing.Size(246, 297);
            this.tablepanel.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox comboselection;
      private System.Windows.Forms.Panel panelclient;
        private System.Windows.Forms.TableLayoutPanel tablepanel;
        private System.Windows.Forms.Button bforward;
        private System.Windows.Forms.Button bback;
    }
}
