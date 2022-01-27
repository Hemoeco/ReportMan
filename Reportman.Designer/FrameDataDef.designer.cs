namespace Reportman.Designer
{
    partial class FrameDataDef
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameDataDef));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.RView = new System.Windows.Forms.TreeView();
            this.TopBar = new System.Windows.Forms.ToolStrip();
            this.badd = new System.Windows.Forms.ToolStripSplitButton();
            this.mdataaddconnection = new System.Windows.Forms.ToolStripMenuItem();
            this.madddataset = new System.Windows.Forms.ToolStripMenuItem();
            this.maddparam = new System.Windows.Forms.ToolStripMenuItem();
            this.bconnect = new System.Windows.Forms.ToolStripButton();
            this.bdelete = new System.Windows.Forms.ToolStripButton();
            this.bup = new System.Windows.Forms.ToolStripButton();
            this.bdown = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.TopBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.RView);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(248, 244);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(248, 270);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.TopBar);
            // 
            // RView
            // 
            this.RView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RView.HideSelection = false;
            this.RView.Location = new System.Drawing.Point(0, 0);
            this.RView.Name = "RView";
            this.RView.Size = new System.Drawing.Size(248, 244);
            this.RView.TabIndex = 1;
            this.RView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.RView_AfterSelect);
            // 
            // TopBar
            // 
            this.TopBar.Dock = System.Windows.Forms.DockStyle.None;
            this.TopBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.TopBar.ImageScalingSize = new System.Drawing.Size(19, 19);
            this.TopBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.badd,
            this.bconnect,
            this.bdelete,
            this.bup,
            this.bdown});
            this.TopBar.Location = new System.Drawing.Point(3, 0);
            this.TopBar.Name = "TopBar";
            this.TopBar.Size = new System.Drawing.Size(161, 26);
            this.TopBar.TabIndex = 0;
            // 
            // badd
            // 
            this.badd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.badd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mdataaddconnection,
            this.madddataset,
            this.maddparam});
            this.badd.Image = ((System.Drawing.Image)(resources.GetObject("badd.Image")));
            this.badd.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.badd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.badd.Name = "badd";
            this.badd.Size = new System.Drawing.Size(35, 23);
            this.badd.Text = "Add";
            // 
            // mdataaddconnection
            // 
            this.mdataaddconnection.Name = "mdataaddconnection";
            this.mdataaddconnection.Size = new System.Drawing.Size(136, 22);
            this.mdataaddconnection.Text = "Connection";
            this.mdataaddconnection.Click += new System.EventHandler(this.mdataaddconnection_Click);
            // 
            // madddataset
            // 
            this.madddataset.Name = "madddataset";
            this.madddataset.Size = new System.Drawing.Size(136, 22);
            this.madddataset.Text = "Dataset";
            this.madddataset.Click += new System.EventHandler(this.madddataset_Click);
            // 
            // maddparam
            // 
            this.maddparam.Name = "maddparam";
            this.maddparam.Size = new System.Drawing.Size(136, 22);
            this.maddparam.Text = "Parameter";
            this.maddparam.Click += new System.EventHandler(this.maddparam_Click);
            // 
            // bconnect
            // 
            this.bconnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bconnect.Image = ((System.Drawing.Image)(resources.GetObject("bconnect.Image")));
            this.bconnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bconnect.Name = "bconnect";
            this.bconnect.Size = new System.Drawing.Size(23, 23);
            this.bconnect.Text = "Connect";
            this.bconnect.Click += new System.EventHandler(this.bconnect_Click);
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
            // FrameDataDef
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "FrameDataDef";
            this.Size = new System.Drawing.Size(248, 270);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.TopBar.ResumeLayout(false);
            this.TopBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip TopBar;
        private System.Windows.Forms.ToolStripSplitButton badd;
        private System.Windows.Forms.ToolStripMenuItem mdataaddconnection;
        private System.Windows.Forms.ToolStripMenuItem madddataset;
        private System.Windows.Forms.ToolStripMenuItem maddparam;
        private System.Windows.Forms.ToolStripButton bdelete;
        private System.Windows.Forms.TreeView RView;
        private System.Windows.Forms.ToolStripButton bconnect;
        private System.Windows.Forms.ToolStripButton bup;
        private System.Windows.Forms.ToolStripButton bdown;
    }
}
