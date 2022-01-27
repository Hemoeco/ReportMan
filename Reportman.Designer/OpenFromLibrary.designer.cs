namespace Reportman.Designer
{
    partial class OpenFromLibrary
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
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
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenFromLibrary));
            this.ReportTree = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.panelbottom = new System.Windows.Forms.Panel();
            this.bcancel = new System.Windows.Forms.Button();
            this.baceptar = new System.Windows.Forms.Button();
            this.toolbar = new Reportman.Drawing.Forms.ToolStripAdvanced();
            this.bexpand = new System.Windows.Forms.ToolStripButton();
            this.bcontract = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.bnuevo = new System.Windows.Forms.ToolStripSplitButton();
            this.mnewfolder = new System.Windows.Forms.ToolStripMenuItem();
            this.mnewreport = new System.Windows.Forms.ToolStripMenuItem();
            this.brename = new System.Windows.Forms.ToolStripButton();
            this.bup = new System.Windows.Forms.ToolStripButton();
            this.bdown = new System.Windows.Forms.ToolStripButton();
            this.bleft = new System.Windows.Forms.ToolStripButton();
            this.bdelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.textfilter = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bsearch = new System.Windows.Forms.ToolStripButton();
            this.textboxsearch = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bexport = new System.Windows.Forms.ToolStripButton();
            this.bimport = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.panelbottom.SuspendLayout();
            this.toolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // ReportTree
            // 
            this.ReportTree.AllowDrop = true;
            this.ReportTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReportTree.HideSelection = false;
            this.ReportTree.Location = new System.Drawing.Point(0, 0);
            this.ReportTree.Name = "ReportTree";
            this.ReportTree.Size = new System.Drawing.Size(774, 442);
            this.ReportTree.TabIndex = 0;
            this.ReportTree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.ReportTree_BeforeCollapse);
            this.ReportTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.ReportTree_BeforeExpand);
            this.ReportTree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.ReportTree_BeforeSelect);
            this.ReportTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ReportTree_AfterSelect);
            this.ReportTree.DoubleClick += new System.EventHandler(this.ReportTree_DoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(32, 32);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.ReportTree);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panelbottom);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(774, 483);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(774, 511);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolbar);
            // 
            // panelbottom
            // 
            this.panelbottom.Controls.Add(this.bcancel);
            this.panelbottom.Controls.Add(this.baceptar);
            this.panelbottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelbottom.Location = new System.Drawing.Point(0, 442);
            this.panelbottom.Name = "panelbottom";
            this.panelbottom.Size = new System.Drawing.Size(774, 41);
            this.panelbottom.TabIndex = 1;
            // 
            // bcancel
            // 
            this.bcancel.Location = new System.Drawing.Point(152, 5);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(98, 33);
            this.bcancel.TabIndex = 1;
            this.bcancel.Text = "Cancelar";
            this.bcancel.UseVisualStyleBackColor = true;
            this.bcancel.Click += new System.EventHandler(this.Bcancel_Click);
            // 
            // baceptar
            // 
            this.baceptar.Location = new System.Drawing.Point(4, 5);
            this.baceptar.Name = "baceptar";
            this.baceptar.Size = new System.Drawing.Size(110, 33);
            this.baceptar.TabIndex = 0;
            this.baceptar.Text = "Aceptar";
            this.baceptar.UseVisualStyleBackColor = true;
            this.baceptar.Click += new System.EventHandler(this.Baceptar_Click);
            // 
            // toolbar
            // 
            this.toolbar.Dock = System.Windows.Forms.DockStyle.None;
            this.toolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolbar.ImageScalingSize = new System.Drawing.Size(21, 21);
            this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bexpand,
            this.bcontract,
            this.toolStripSeparator3,
            this.bnuevo,
            this.brename,
            this.bup,
            this.bdown,
            this.bleft,
            this.bdelete,
            this.toolStripSeparator4,
            this.toolStripButton1,
            this.textfilter,
            this.toolStripSeparator1,
            this.bsearch,
            this.textboxsearch,
            this.toolStripSeparator2,
            this.bexport,
            this.bimport});
            this.toolbar.Location = new System.Drawing.Point(3, 0);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(547, 28);
            this.toolbar.TabIndex = 0;
            // 
            // bexpand
            // 
            this.bexpand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bexpand.Image = global::Reportman.Designer.Properties.Resources.expand;
            this.bexpand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bexpand.Name = "bexpand";
            this.bexpand.Size = new System.Drawing.Size(25, 25);
            this.bexpand.Text = "Expandir";
            this.bexpand.Click += new System.EventHandler(this.Bexpand_Click);
            // 
            // bcontract
            // 
            this.bcontract.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bcontract.Image = global::Reportman.Designer.Properties.Resources.contract;
            this.bcontract.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bcontract.Name = "bcontract";
            this.bcontract.Size = new System.Drawing.Size(25, 25);
            this.bcontract.Text = "Contraer";
            this.bcontract.Click += new System.EventHandler(this.Bcontract_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // bnuevo
            // 
            this.bnuevo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bnuevo.DropDownButtonWidth = 15;
            this.bnuevo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnewfolder,
            this.mnewreport});
            this.bnuevo.Image = ((System.Drawing.Image)(resources.GetObject("bnuevo.Image")));
            this.bnuevo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bnuevo.Name = "bnuevo";
            this.bnuevo.Size = new System.Drawing.Size(41, 25);
            this.bnuevo.Text = "Nuevo";
            // 
            // mnewfolder
            // 
            this.mnewfolder.Image = global::Reportman.Designer.Properties.Resources.closed_folder;
            this.mnewfolder.Name = "mnewfolder";
            this.mnewfolder.Size = new System.Drawing.Size(116, 22);
            this.mnewfolder.Text = "Carpeta";
            this.mnewfolder.Click += new System.EventHandler(this.Bnewfolder_Click);
            // 
            // mnewreport
            // 
            this.mnewreport.Image = global::Reportman.Designer.Properties.Resources.document32;
            this.mnewreport.Name = "mnewreport";
            this.mnewreport.Size = new System.Drawing.Size(116, 22);
            this.mnewreport.Text = "Informe";
            this.mnewreport.Click += new System.EventHandler(this.Bnewreport_Click);
            // 
            // brename
            // 
            this.brename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.brename.Image = global::Reportman.Designer.Properties.Resources.label;
            this.brename.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.brename.Name = "brename";
            this.brename.Size = new System.Drawing.Size(25, 25);
            this.brename.Text = "Rename";
            this.brename.Click += new System.EventHandler(this.Brename_Click);
            // 
            // bup
            // 
            this.bup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bup.Image = global::Reportman.Designer.Properties.Resources.arrowup;
            this.bup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bup.Name = "bup";
            this.bup.Size = new System.Drawing.Size(25, 25);
            this.bup.Text = "Arriba";
            this.bup.Click += new System.EventHandler(this.Bup_Click);
            // 
            // bdown
            // 
            this.bdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bdown.Image = global::Reportman.Designer.Properties.Resources.arrowdown;
            this.bdown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bdown.Name = "bdown";
            this.bdown.Size = new System.Drawing.Size(25, 25);
            this.bdown.Text = "Arriba";
            this.bdown.Click += new System.EventHandler(this.Bdown_Click);
            // 
            // bleft
            // 
            this.bleft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bleft.Image = global::Reportman.Designer.Properties.Resources.arrowleft;
            this.bleft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bleft.Name = "bleft";
            this.bleft.Size = new System.Drawing.Size(25, 25);
            this.bleft.Text = "Izquierda";
            this.bleft.Click += new System.EventHandler(this.Bleft_Click);
            // 
            // bdelete
            // 
            this.bdelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bdelete.Image = global::Reportman.Designer.Properties.Resources.delete;
            this.bdelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bdelete.Name = "bdelete";
            this.bdelete.Size = new System.Drawing.Size(25, 25);
            this.bdelete.Text = "Delete Item";
            this.bdelete.Click += new System.EventHandler(this.Bdelete_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Reportman.Designer.Properties.Resources.filter;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(25, 25);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // textfilter
            // 
            this.textfilter.Name = "textfilter";
            this.textfilter.Size = new System.Drawing.Size(100, 28);
            this.textfilter.TextChanged += new System.EventHandler(this.Textfilter_TextChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // bsearch
            // 
            this.bsearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bsearch.Image = global::Reportman.Designer.Properties.Resources.find;
            this.bsearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bsearch.Name = "bsearch";
            this.bsearch.Size = new System.Drawing.Size(25, 25);
            this.bsearch.Text = "Search";
            this.bsearch.Click += new System.EventHandler(this.Bsearch_Click);
            // 
            // textboxsearch
            // 
            this.textboxsearch.Name = "textboxsearch";
            this.textboxsearch.Size = new System.Drawing.Size(100, 28);
            this.textboxsearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Textboxsearch_KeyDown);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // bexport
            // 
            this.bexport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bexport.Image = global::Reportman.Designer.Properties.Resources.export_subreport;
            this.bexport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bexport.Name = "bexport";
            this.bexport.Size = new System.Drawing.Size(25, 25);
            this.bexport.Text = "Exportar";
            this.bexport.Click += new System.EventHandler(this.Bexport_Click);
            // 
            // bimport
            // 
            this.bimport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bimport.Image = global::Reportman.Designer.Properties.Resources.import_subreport;
            this.bimport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bimport.Name = "bimport";
            this.bimport.Size = new System.Drawing.Size(25, 25);
            this.bimport.Text = "Importar";
            this.bimport.Click += new System.EventHandler(this.Bimport_Click);
            // 
            // OpenFromLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "OpenFromLibrary";
            this.Size = new System.Drawing.Size(774, 511);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.panelbottom.ResumeLayout(false);
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView ReportTree;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripButton bdelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox textboxsearch;
        private System.Windows.Forms.ToolStripButton bsearch;
        private System.Windows.Forms.Panel panelbottom;
        private System.Windows.Forms.Button bcancel;
        private System.Windows.Forms.Button baceptar;
        private System.Windows.Forms.ToolStripButton brename;
        public Drawing.Forms.ToolStripAdvanced toolbar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripTextBox textfilter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSplitButton bnuevo;
        private System.Windows.Forms.ToolStripMenuItem mnewfolder;
        private System.Windows.Forms.ToolStripMenuItem mnewreport;
        private System.Windows.Forms.ToolStripButton bexpand;
        private System.Windows.Forms.ToolStripButton bcontract;
        private System.Windows.Forms.ToolStripButton bup;
        private System.Windows.Forms.ToolStripButton bdown;
        private System.Windows.Forms.ToolStripButton bleft;
        private System.Windows.Forms.ToolStripButton bexport;
        private System.Windows.Forms.ToolStripButton bimport;
    }
}
