namespace Reportman.Designer
{
    partial class FrameStructure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameStructure));
            this.maincontainer = new System.Windows.Forms.ToolStripContainer();
            this.RView = new System.Windows.Forms.TreeView();
            this.topbar = new System.Windows.Forms.ToolStrip();
            this.bexpand = new System.Windows.Forms.ToolStripButton();
            this.bcontract = new System.Windows.Forms.ToolStripButton();
            this.baddstruc = new System.Windows.Forms.ToolStripDropDownButton();
            this.mstrucaddpheader = new System.Windows.Forms.ToolStripMenuItem();
            this.mstrucaddpfooter = new System.Windows.Forms.ToolStripMenuItem();
            this.mstrucaddgroup = new System.Windows.Forms.ToolStripMenuItem();
            this.mstrucaddsubreport = new System.Windows.Forms.ToolStripMenuItem();
            this.mstrucadddetail = new System.Windows.Forms.ToolStripMenuItem();
            this.bdelete = new System.Windows.Forms.ToolStripButton();
            this.bup = new System.Windows.Forms.ToolStripButton();
            this.bdown = new System.Windows.Forms.ToolStripButton();
            this.bexport = new System.Windows.Forms.ToolStripButton();
            this.bimport = new System.Windows.Forms.ToolStripButton();
            this.maincontainer.ContentPanel.SuspendLayout();
            this.maincontainer.TopToolStripPanel.SuspendLayout();
            this.maincontainer.SuspendLayout();
            this.topbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // maincontainer
            // 
            // 
            // maincontainer.ContentPanel
            // 
            this.maincontainer.ContentPanel.Controls.Add(this.RView);
            this.maincontainer.ContentPanel.Size = new System.Drawing.Size(336, 276);
            this.maincontainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maincontainer.Location = new System.Drawing.Point(0, 0);
            this.maincontainer.Name = "maincontainer";
            this.maincontainer.Size = new System.Drawing.Size(336, 302);
            this.maincontainer.TabIndex = 0;
            this.maincontainer.Text = "toolStripContainer1";
            // 
            // maincontainer.TopToolStripPanel
            // 
            this.maincontainer.TopToolStripPanel.Controls.Add(this.topbar);
            // 
            // RView
            // 
            this.RView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RView.HideSelection = false;
            this.RView.Location = new System.Drawing.Point(0, 0);
            this.RView.Name = "RView";
            this.RView.Size = new System.Drawing.Size(336, 276);
            this.RView.TabIndex = 0;
            this.RView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.RView_AfterSelect);
            this.RView.DragEnter += new System.Windows.Forms.DragEventHandler(this.RView_DragEnter);
            // 
            // topbar
            // 
            this.topbar.Dock = System.Windows.Forms.DockStyle.None;
            this.topbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.topbar.ImageScalingSize = new System.Drawing.Size(19, 19);
            this.topbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bexpand,
            this.bcontract,
            this.baddstruc,
            this.bdelete,
            this.bup,
            this.bdown,
            this.bexport,
            this.bimport});
            this.topbar.Location = new System.Drawing.Point(3, 0);
            this.topbar.Name = "topbar";
            this.topbar.Size = new System.Drawing.Size(227, 26);
            this.topbar.TabIndex = 0;
            // 
            // bexpand
            // 
            this.bexpand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bexpand.Image = global::Reportman.Designer.Properties.Resources.expand;
            this.bexpand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bexpand.Name = "bexpand";
            this.bexpand.Size = new System.Drawing.Size(23, 23);
            this.bexpand.Click += new System.EventHandler(this.bexpand_Click);
            // 
            // bcontract
            // 
            this.bcontract.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bcontract.Image = global::Reportman.Designer.Properties.Resources.contract;
            this.bcontract.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bcontract.Name = "bcontract";
            this.bcontract.Size = new System.Drawing.Size(23, 23);
            this.bcontract.Click += new System.EventHandler(this.bcontract_Click);
            // 
            // baddstruc
            // 
            this.baddstruc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.baddstruc.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mstrucaddpheader,
            this.mstrucaddpfooter,
            this.mstrucaddgroup,
            this.mstrucaddsubreport,
            this.mstrucadddetail});
            this.baddstruc.Image = global::Reportman.Designer.Properties.Resources.newdocument;
            this.baddstruc.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.baddstruc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.baddstruc.Name = "baddstruc";
            this.baddstruc.Size = new System.Drawing.Size(32, 23);
            this.baddstruc.Text = "Add";
            // 
            // mstrucaddpheader
            // 
            this.mstrucaddpheader.Name = "mstrucaddpheader";
            this.mstrucaddpheader.Size = new System.Drawing.Size(204, 22);
            this.mstrucaddpheader.Text = "Page header";
            this.mstrucaddpheader.Click += new System.EventHandler(this.mstrucaddpheader_Click);
            // 
            // mstrucaddpfooter
            // 
            this.mstrucaddpfooter.Name = "mstrucaddpfooter";
            this.mstrucaddpfooter.Size = new System.Drawing.Size(204, 22);
            this.mstrucaddpfooter.Text = "Page footer";
            this.mstrucaddpfooter.Click += new System.EventHandler(this.mstrucaddpfooter_Click);
            // 
            // mstrucaddgroup
            // 
            this.mstrucaddgroup.Name = "mstrucaddgroup";
            this.mstrucaddgroup.Size = new System.Drawing.Size(204, 22);
            this.mstrucaddgroup.Text = "Group header and footer";
            this.mstrucaddgroup.Click += new System.EventHandler(this.mstrucaddgroup_Click);
            // 
            // mstrucaddsubreport
            // 
            this.mstrucaddsubreport.Name = "mstrucaddsubreport";
            this.mstrucaddsubreport.Size = new System.Drawing.Size(204, 22);
            this.mstrucaddsubreport.Text = "Subreport";
            this.mstrucaddsubreport.Click += new System.EventHandler(this.mstrucaddsubreport_Click);
            // 
            // mstrucadddetail
            // 
            this.mstrucadddetail.Name = "mstrucadddetail";
            this.mstrucadddetail.Size = new System.Drawing.Size(204, 22);
            this.mstrucadddetail.Text = "Detail";
            this.mstrucadddetail.Click += new System.EventHandler(this.mstrucadddetail_Click);
            // 
            // bdelete
            // 
            this.bdelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bdelete.Image = ((System.Drawing.Image)(resources.GetObject("bdelete.Image")));
            this.bdelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bdelete.Name = "bdelete";
            this.bdelete.Size = new System.Drawing.Size(23, 23);
            this.bdelete.Text = "Delete";
            this.bdelete.Click += new System.EventHandler(this.bdelete_Click);
            // 
            // bup
            // 
            this.bup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bup.Image = ((System.Drawing.Image)(resources.GetObject("bup.Image")));
            this.bup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bup.Name = "bup";
            this.bup.Size = new System.Drawing.Size(23, 23);
            this.bup.Text = "Up";
            this.bup.Click += new System.EventHandler(this.bup_Click);
            // 
            // bdown
            // 
            this.bdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bdown.Image = ((System.Drawing.Image)(resources.GetObject("bdown.Image")));
            this.bdown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bdown.Name = "bdown";
            this.bdown.Size = new System.Drawing.Size(23, 23);
            this.bdown.Text = "Down";
            this.bdown.Click += new System.EventHandler(this.bdown_Click);
            // 
            // bexport
            // 
            this.bexport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bexport.Image = global::Reportman.Designer.Properties.Resources.export_subreport;
            this.bexport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bexport.Name = "bexport";
            this.bexport.Size = new System.Drawing.Size(23, 23);
            this.bexport.Text = "Exportar";
            this.bexport.Click += new System.EventHandler(this.bexport_Click);
            // 
            // bimport
            // 
            this.bimport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bimport.Image = global::Reportman.Designer.Properties.Resources.import_subreport;
            this.bimport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bimport.Name = "bimport";
            this.bimport.Size = new System.Drawing.Size(23, 23);
            this.bimport.Text = "Importar";
            this.bimport.Click += new System.EventHandler(this.bimport_Click);
            // 
            // FrameStructure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.maincontainer);
            this.Name = "FrameStructure";
            this.Size = new System.Drawing.Size(336, 302);
            this.maincontainer.ContentPanel.ResumeLayout(false);
            this.maincontainer.TopToolStripPanel.ResumeLayout(false);
            this.maincontainer.TopToolStripPanel.PerformLayout();
            this.maincontainer.ResumeLayout(false);
            this.maincontainer.PerformLayout();
            this.topbar.ResumeLayout(false);
            this.topbar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer maincontainer;
        private System.Windows.Forms.ToolStrip topbar;
        private System.Windows.Forms.ToolStripDropDownButton baddstruc;
        private System.Windows.Forms.ToolStripMenuItem mstrucaddpheader;
        private System.Windows.Forms.ToolStripMenuItem mstrucaddpfooter;
        private System.Windows.Forms.ToolStripMenuItem mstrucaddgroup;
        private System.Windows.Forms.ToolStripMenuItem mstrucaddsubreport;
        private System.Windows.Forms.ToolStripMenuItem mstrucadddetail;
        private System.Windows.Forms.TreeView RView;
        private System.Windows.Forms.ToolStripButton bdelete;
        private System.Windows.Forms.ToolStripButton bup;
        private System.Windows.Forms.ToolStripButton bdown;
        private System.Windows.Forms.ToolStripButton bexpand;
        private System.Windows.Forms.ToolStripButton bcontract;
        private System.Windows.Forms.ToolStripButton bexport;
        private System.Windows.Forms.ToolStripButton bimport;
    }
}
