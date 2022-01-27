namespace Reportman.Designer
{
  partial class EditSubReport
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
      InterrnalDispose();
      base.Dispose(disposing);
    }

    #region Código generado por el Diseñador de componentes

    /// <summary> 
    /// Método necesario para admitir el Diseñador. No se puede modificar
    /// el contenido del método con el editor de código.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.paneltop = new System.Windows.Forms.Panel();
        this.paneltopclient = new System.Windows.Forms.Panel();
        this.rulertop = new Reportman.Drawing.Forms.Ruler();
        this.panellefttop = new System.Windows.Forms.Panel();
        this.boptions = new System.Windows.Forms.Button();
        this.panelclient = new System.Windows.Forms.Panel();
        this.parentcontextmenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        this.msendtoback = new System.Windows.Forms.ToolStripMenuItem();
        this.mbringtofront = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
        this.mcurrentfont = new System.Windows.Forms.ToolStripMenuItem();
        this.mchangedefaultfontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
        this.mhide = new System.Windows.Forms.ToolStripMenuItem();
        this.mshowall = new System.Windows.Forms.ToolStripMenuItem();
        this.nfontdialog = new System.Windows.Forms.FontDialog();
        this.paneltop.SuspendLayout();
        this.paneltopclient.SuspendLayout();
        this.panellefttop.SuspendLayout();
        this.parentcontextmenu.SuspendLayout();
        this.SuspendLayout();
        // 
        // paneltop
        // 
        this.paneltop.Controls.Add(this.paneltopclient);
        this.paneltop.Controls.Add(this.panellefttop);
        this.paneltop.Dock = System.Windows.Forms.DockStyle.Top;
        this.paneltop.Location = new System.Drawing.Point(0, 0);
        this.paneltop.Name = "paneltop";
        this.paneltop.Size = new System.Drawing.Size(290, 24);
        this.paneltop.TabIndex = 1;
        // 
        // paneltopclient
        // 
        this.paneltopclient.Controls.Add(this.rulertop);
        this.paneltopclient.Dock = System.Windows.Forms.DockStyle.Fill;
        this.paneltopclient.Location = new System.Drawing.Point(25, 0);
        this.paneltopclient.Name = "paneltopclient";
        this.paneltopclient.Size = new System.Drawing.Size(265, 24);
        this.paneltopclient.TabIndex = 2;
        // 
        // rulertop
        // 
        this.rulertop.Location = new System.Drawing.Point(0, 0);
        this.rulertop.Name = "rulertop";
        this.rulertop.Size = new System.Drawing.Size(146, 24);
        this.rulertop.TabIndex = 0;
        // 
        // panellefttop
        // 
        this.panellefttop.Controls.Add(this.boptions);
        this.panellefttop.Dock = System.Windows.Forms.DockStyle.Left;
        this.panellefttop.Location = new System.Drawing.Point(0, 0);
        this.panellefttop.Name = "panellefttop";
        this.panellefttop.Size = new System.Drawing.Size(25, 24);
        this.panellefttop.TabIndex = 1;
        // 
        // boptions
        // 
        this.boptions.Dock = System.Windows.Forms.DockStyle.Fill;
        this.boptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.boptions.Location = new System.Drawing.Point(0, 0);
        this.boptions.Name = "boptions";
        this.boptions.Size = new System.Drawing.Size(25, 24);
        this.boptions.TabIndex = 0;
        this.boptions.UseVisualStyleBackColor = true;
        this.boptions.Click += new System.EventHandler(this.boptions_Click);
        // 
        // panelclient
        // 
        this.panelclient.AutoScroll = true;
        this.panelclient.BackColor = System.Drawing.SystemColors.AppWorkspace;
        this.panelclient.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panelclient.Location = new System.Drawing.Point(0, 24);
        this.panelclient.Name = "panelclient";
        this.panelclient.Size = new System.Drawing.Size(290, 294);
        this.panelclient.TabIndex = 2;
        // 
        // parentcontextmenu
        // 
        this.parentcontextmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.msendtoback,
            this.mbringtofront,
            this.toolStripMenuItem2,
            this.mcurrentfont,
            this.mchangedefaultfontToolStripMenuItem,
            this.toolStripMenuItem1,
            this.mhide,
            this.mshowall});
        this.parentcontextmenu.Name = "parentcontextmenu";
        this.parentcontextmenu.Size = new System.Drawing.Size(181, 170);
        this.parentcontextmenu.Opening += new System.ComponentModel.CancelEventHandler(this.parentcontextmenu_Opening);
        // 
        // msendtoback
        // 
        this.msendtoback.Name = "msendtoback";
        this.msendtoback.Size = new System.Drawing.Size(180, 22);
        this.msendtoback.Text = "Send to back";
        this.msendtoback.Click += new System.EventHandler(this.msendtoback_Click);
        // 
        // mbringtofront
        // 
        this.mbringtofront.Name = "mbringtofront";
        this.mbringtofront.Size = new System.Drawing.Size(180, 22);
        this.mbringtofront.Text = "Bring to front";
        this.mbringtofront.Click += new System.EventHandler(this.mbringtofront_Click);
        // 
        // toolStripMenuItem2
        // 
        this.toolStripMenuItem2.Name = "toolStripMenuItem2";
        this.toolStripMenuItem2.Size = new System.Drawing.Size(177, 6);
        // 
        // mcurrentfont
        // 
        this.mcurrentfont.Name = "mcurrentfont";
        this.mcurrentfont.Size = new System.Drawing.Size(180, 22);
        this.mcurrentfont.Text = "Current font";
        // 
        // mchangedefaultfontToolStripMenuItem
        // 
        this.mchangedefaultfontToolStripMenuItem.Name = "mchangedefaultfontToolStripMenuItem";
        this.mchangedefaultfontToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
        this.mchangedefaultfontToolStripMenuItem.Text = "Change default font";
        this.mchangedefaultfontToolStripMenuItem.Click += new System.EventHandler(this.mchangedefaultfontToolStripMenuItem_Click);
        // 
        // toolStripMenuItem1
        // 
        this.toolStripMenuItem1.Name = "toolStripMenuItem1";
        this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
        // 
        // mhide
        // 
        this.mhide.Name = "mhide";
        this.mhide.Size = new System.Drawing.Size(180, 22);
        this.mhide.Text = "Hide";
        this.mhide.Click += new System.EventHandler(this.mhide_Click);
        // 
        // mshowall
        // 
        this.mshowall.Name = "mshowall";
        this.mshowall.Size = new System.Drawing.Size(180, 22);
        this.mshowall.Text = "Show All";
        this.mshowall.Click += new System.EventHandler(this.mshowall_Click);
        // 
        // EditSubReport
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.panelclient);
        this.Controls.Add(this.paneltop);
        this.Name = "EditSubReport";
        this.Size = new System.Drawing.Size(290, 318);
        this.paneltop.ResumeLayout(false);
        this.paneltopclient.ResumeLayout(false);
        this.panellefttop.ResumeLayout(false);
        this.parentcontextmenu.ResumeLayout(false);
        this.ResumeLayout(false);

    }

    #endregion

    private Reportman.Drawing.Forms.Ruler rulertop;
    private System.Windows.Forms.Panel paneltop;
    private System.Windows.Forms.Panel panelclient;
    private System.Windows.Forms.Panel paneltopclient;
    private System.Windows.Forms.Panel panellefttop;
    private System.Windows.Forms.Button boptions;
    private System.Windows.Forms.ContextMenuStrip parentcontextmenu;
    private System.Windows.Forms.ToolStripMenuItem msendtoback;
    private System.Windows.Forms.ToolStripMenuItem mbringtofront;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem mhide;
    private System.Windows.Forms.ToolStripMenuItem mshowall;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    private System.Windows.Forms.ToolStripMenuItem mcurrentfont;
    private System.Windows.Forms.ToolStripMenuItem mchangedefaultfontToolStripMenuItem;
    private System.Windows.Forms.FontDialog nfontdialog;
  }
}
