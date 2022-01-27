namespace Reportman.Drawing.Forms
{
  partial class UpdateForm
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
      this.components = new System.ComponentModel.Container();
      this.progarchivo = new System.Windows.Forms.ProgressBar();
      this.larchivo = new System.Windows.Forms.Label();
      this.lkbytes = new System.Windows.Forms.Label();
      this.progkbytes = new System.Windows.Forms.ProgressBar();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.SuspendLayout();
      // 
      // progarchivo
      // 
      this.progarchivo.Location = new System.Drawing.Point(12, 23);
      this.progarchivo.Name = "progarchivo";
      this.progarchivo.Size = new System.Drawing.Size(417, 37);
      this.progarchivo.TabIndex = 0;
      // 
      // larchivo
      // 
      this.larchivo.AutoSize = true;
      this.larchivo.Location = new System.Drawing.Point(12, 7);
      this.larchivo.Name = "larchivo";
      this.larchivo.Size = new System.Drawing.Size(43, 13);
      this.larchivo.TabIndex = 1;
      this.larchivo.Text = "Archivo";
      // 
      // lkbytes
      // 
      this.lkbytes.AutoSize = true;
      this.lkbytes.Location = new System.Drawing.Point(12, 76);
      this.lkbytes.Name = "lkbytes";
      this.lkbytes.Size = new System.Drawing.Size(40, 13);
      this.lkbytes.TabIndex = 3;
      this.lkbytes.Text = "KBytes";
      // 
      // progkbytes
      // 
      this.progkbytes.Location = new System.Drawing.Point(12, 92);
      this.progkbytes.Name = "progkbytes";
      this.progkbytes.Size = new System.Drawing.Size(417, 17);
      this.progkbytes.TabIndex = 2;
      // 
      // timer1
      // 
      this.timer1.Enabled = true;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // UpdateForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(440, 126);
      this.Controls.Add(this.lkbytes);
      this.Controls.Add(this.progkbytes);
      this.Controls.Add(this.larchivo);
      this.Controls.Add(this.progarchivo);
      this.Name = "UpdateForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdateForm_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ProgressBar progarchivo;
    private System.Windows.Forms.Label larchivo;
    private System.Windows.Forms.Label lkbytes;
    private System.Windows.Forms.ProgressBar progkbytes;
    private System.Windows.Forms.Timer timer1;
  }
}