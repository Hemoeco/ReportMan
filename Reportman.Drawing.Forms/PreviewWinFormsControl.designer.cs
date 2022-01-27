namespace Reportman.Drawing.Forms
{
    partial class PreviewWinFormsControl
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
          DoDispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewWinFormsControl));
            this.maincontainer = new System.Windows.Forms.ToolStripContainer();
            this.mainstatus = new System.Windows.Forms.StatusStrip();
            this.StatusPage = new System.Windows.Forms.ToolStripStatusLabel();
            this.BarStatusEdit = new System.Windows.Forms.ToolStripStatusLabel();
            this.PParent = new System.Windows.Forms.Panel();
            this.maintoolstrip = new Reportman.Drawing.Forms.ToolStripAdvanced();
            this.bsfirst = new System.Windows.Forms.ToolStripButton();
            this.bsprior = new System.Windows.Forms.ToolStripButton();
            this.EPage = new System.Windows.Forms.ToolStripTextBox();
            this.bsnext = new System.Windows.Forms.ToolStripButton();
            this.bslast = new System.Windows.Forms.ToolStripButton();
            this.bdivsearch = new System.Windows.Forms.ToolStripSeparator();
            this.textsearch = new System.Windows.Forms.ToolStripTextBox();
            this.bssearch = new System.Windows.Forms.ToolStripButton();
            this.BDivPageSetup = new System.Windows.Forms.ToolStripSeparator();
            this.BPageSetup = new System.Windows.Forms.ToolStripButton();
            this.BParameters = new System.Windows.Forms.ToolStripButton();
            this.BDivParams = new System.Windows.Forms.ToolStripSeparator();
            this.BPrint = new System.Windows.Forms.ToolStripButton();
            this.BSave = new System.Windows.Forms.ToolStripButton();
            this.BMail = new System.Windows.Forms.ToolStripButton();
            this.bdivscale = new System.Windows.Forms.ToolStripSeparator();
            this.BScaleWide = new System.Windows.Forms.ToolStripButton();
            this.BScaleFull = new System.Windows.Forms.ToolStripButton();
            this.BScaleEntire = new System.Windows.Forms.ToolStripSplitButton();
            this.MScale1 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale2 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale3 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale4 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale5 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale6 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale8 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale9 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale12 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale14 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale15 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale16 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale18 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale32 = new System.Windows.Forms.ToolStripMenuItem();
            this.MScale64 = new System.Windows.Forms.ToolStripMenuItem();
            this.MTopDown = new System.Windows.Forms.ToolStripMenuItem();
            this.BDivZoom = new System.Windows.Forms.ToolStripSeparator();
            this.BZoomMinus = new System.Windows.Forms.ToolStripButton();
            this.BZoomPlus = new System.Windows.Forms.ToolStripButton();
            this.bdivexit = new System.Windows.Forms.ToolStripSeparator();
            this.BExit = new System.Windows.Forms.ToolStripButton();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.maincontainer.BottomToolStripPanel.SuspendLayout();
            this.maincontainer.ContentPanel.SuspendLayout();
            this.maincontainer.TopToolStripPanel.SuspendLayout();
            this.maincontainer.SuspendLayout();
            this.mainstatus.SuspendLayout();
            this.maintoolstrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // maincontainer
            // 
            // 
            // maincontainer.BottomToolStripPanel
            // 
            this.maincontainer.BottomToolStripPanel.Controls.Add(this.mainstatus);
            // 
            // maincontainer.ContentPanel
            // 
            this.maincontainer.ContentPanel.Controls.Add(this.PParent);
            this.maincontainer.ContentPanel.Size = new System.Drawing.Size(632, 395);
            this.maincontainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maincontainer.Location = new System.Drawing.Point(0, 0);
            this.maincontainer.Name = "maincontainer";
            this.maincontainer.Size = new System.Drawing.Size(632, 443);
            this.maincontainer.TabIndex = 0;
            // 
            // maincontainer.TopToolStripPanel
            // 
            this.maincontainer.TopToolStripPanel.Controls.Add(this.maintoolstrip);
            // 
            // mainstatus
            // 
            this.mainstatus.Dock = System.Windows.Forms.DockStyle.None;
            this.mainstatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusPage,
            this.BarStatusEdit});
            this.mainstatus.Location = new System.Drawing.Point(0, 0);
            this.mainstatus.Name = "mainstatus";
            this.mainstatus.Size = new System.Drawing.Size(632, 22);
            this.mainstatus.TabIndex = 0;
            // 
            // StatusPage
            // 
            this.StatusPage.Name = "StatusPage";
            this.StatusPage.Size = new System.Drawing.Size(31, 17);
            this.StatusPage.Text = "        ";
            // 
            // BarStatusEdit
            // 
            this.BarStatusEdit.Name = "BarStatusEdit";
            this.BarStatusEdit.Size = new System.Drawing.Size(16, 17);
            this.BarStatusEdit.Text = "   ";
            // 
            // PParent
            // 
            this.PParent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PParent.Location = new System.Drawing.Point(0, 0);
            this.PParent.Name = "PParent";
            this.PParent.Size = new System.Drawing.Size(632, 395);
            this.PParent.TabIndex = 5;
            this.PParent.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // maintoolstrip
            // 
            this.maintoolstrip.Dock = System.Windows.Forms.DockStyle.None;
            this.maintoolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.maintoolstrip.ImageScalingSize = new System.Drawing.Size(19, 19);
            this.maintoolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bsfirst,
            this.bsprior,
            this.EPage,
            this.bsnext,
            this.bslast,
            this.bdivsearch,
            this.textsearch,
            this.bssearch,
            this.BDivPageSetup,
            this.BPageSetup,
            this.BParameters,
            this.BDivParams,
            this.BPrint,
            this.BSave,
            this.BMail,
            this.bdivscale,
            this.BScaleWide,
            this.BScaleFull,
            this.BScaleEntire,
            this.BDivZoom,
            this.BZoomMinus,
            this.BZoomPlus,
            this.bdivexit,
            this.BExit});
            this.maintoolstrip.Location = new System.Drawing.Point(3, 0);
            this.maintoolstrip.Name = "maintoolstrip";
            this.maintoolstrip.Size = new System.Drawing.Size(604, 26);
            this.maintoolstrip.TabIndex = 0;
            this.maintoolstrip.TabStop = true;
            // 
            // bsfirst
            // 
            this.bsfirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bsfirst.Image = ((System.Drawing.Image)(resources.GetObject("bsfirst.Image")));
            this.bsfirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bsfirst.Name = "bsfirst";
            this.bsfirst.Size = new System.Drawing.Size(23, 23);
            this.bsfirst.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // bsprior
            // 
            this.bsprior.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bsprior.Image = ((System.Drawing.Image)(resources.GetObject("bsprior.Image")));
            this.bsprior.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bsprior.Name = "bsprior";
            this.bsprior.Size = new System.Drawing.Size(23, 23);
            this.bsprior.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // EPage
            // 
            this.EPage.Name = "EPage";
            this.EPage.Size = new System.Drawing.Size(50, 26);
            this.EPage.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.EPage.Leave += new System.EventHandler(this.EPage_Leave);
            this.EPage.Validating += new System.ComponentModel.CancelEventHandler(this.EPage_Validating);
            this.EPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EPage_KeyPress);
            // 
            // bsnext
            // 
            this.bsnext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bsnext.Image = ((System.Drawing.Image)(resources.GetObject("bsnext.Image")));
            this.bsnext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bsnext.Name = "bsnext";
            this.bsnext.Size = new System.Drawing.Size(23, 23);
            this.bsnext.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // bslast
            // 
            this.bslast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bslast.Image = ((System.Drawing.Image)(resources.GetObject("bslast.Image")));
            this.bslast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bslast.Name = "bslast";
            this.bslast.Size = new System.Drawing.Size(23, 23);
            this.bslast.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // bdivsearch
            // 
            this.bdivsearch.Name = "bdivsearch";
            this.bdivsearch.Size = new System.Drawing.Size(6, 26);
            // 
            // textsearch
            // 
            this.textsearch.Name = "textsearch";
            this.textsearch.Size = new System.Drawing.Size(100, 26);
            this.textsearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textsearch_KeyPress);
            this.textsearch.TextChanged += new System.EventHandler(this.textsearch_TextChanged);
            // 
            // bssearch
            // 
            this.bssearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bssearch.Image = ((System.Drawing.Image)(resources.GetObject("bssearch.Image")));
            this.bssearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bssearch.Name = "bssearch";
            this.bssearch.Size = new System.Drawing.Size(23, 23);
            this.bssearch.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // BDivPageSetup
            // 
            this.BDivPageSetup.Name = "BDivPageSetup";
            this.BDivPageSetup.Size = new System.Drawing.Size(6, 26);
            // 
            // BPageSetup
            // 
            this.BPageSetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BPageSetup.Image = ((System.Drawing.Image)(resources.GetObject("BPageSetup.Image")));
            this.BPageSetup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BPageSetup.Name = "BPageSetup";
            this.BPageSetup.Size = new System.Drawing.Size(23, 23);
            this.BPageSetup.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // BParameters
            // 
            this.BParameters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BParameters.Image = ((System.Drawing.Image)(resources.GetObject("BParameters.Image")));
            this.BParameters.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BParameters.Name = "BParameters";
            this.BParameters.Size = new System.Drawing.Size(23, 23);
            this.BParameters.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // BDivParams
            // 
            this.BDivParams.Name = "BDivParams";
            this.BDivParams.Size = new System.Drawing.Size(6, 26);
            // 
            // BPrint
            // 
            this.BPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BPrint.Image = ((System.Drawing.Image)(resources.GetObject("BPrint.Image")));
            this.BPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BPrint.Name = "BPrint";
            this.BPrint.Size = new System.Drawing.Size(23, 23);
            this.BPrint.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // BSave
            // 
            this.BSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BSave.Image = ((System.Drawing.Image)(resources.GetObject("BSave.Image")));
            this.BSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BSave.Name = "BSave";
            this.BSave.Size = new System.Drawing.Size(23, 23);
            this.BSave.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // BMail
            // 
            this.BMail.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BMail.Image = ((System.Drawing.Image)(resources.GetObject("BMail.Image")));
            this.BMail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BMail.Name = "BMail";
            this.BMail.Size = new System.Drawing.Size(23, 23);
            this.BMail.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // bdivscale
            // 
            this.bdivscale.Name = "bdivscale";
            this.bdivscale.Size = new System.Drawing.Size(6, 26);
            // 
            // BScaleWide
            // 
            this.BScaleWide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BScaleWide.Image = ((System.Drawing.Image)(resources.GetObject("BScaleWide.Image")));
            this.BScaleWide.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BScaleWide.Name = "BScaleWide";
            this.BScaleWide.Size = new System.Drawing.Size(23, 23);
            this.BScaleWide.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // BScaleFull
            // 
            this.BScaleFull.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BScaleFull.Image = ((System.Drawing.Image)(resources.GetObject("BScaleFull.Image")));
            this.BScaleFull.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BScaleFull.Name = "BScaleFull";
            this.BScaleFull.Size = new System.Drawing.Size(23, 23);
            this.BScaleFull.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // BScaleEntire
            // 
            this.BScaleEntire.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BScaleEntire.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MScale1,
            this.MScale2,
            this.MScale3,
            this.MScale4,
            this.MScale5,
            this.MScale6,
            this.MScale8,
            this.MScale9,
            this.MScale12,
            this.MScale14,
            this.MScale15,
            this.MScale16,
            this.MScale18,
            this.MScale32,
            this.MScale64,
            this.MTopDown});
            this.BScaleEntire.Image = ((System.Drawing.Image)(resources.GetObject("BScaleEntire.Image")));
            this.BScaleEntire.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BScaleEntire.Name = "BScaleEntire";
            this.BScaleEntire.Size = new System.Drawing.Size(35, 23);
            this.BScaleEntire.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // MScale1
            // 
            this.MScale1.Checked = true;
            this.MScale1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MScale1.Name = "MScale1";
            this.MScale1.Size = new System.Drawing.Size(87, 22);
            this.MScale1.Text = "1";
            this.MScale1.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale2
            // 
            this.MScale2.Name = "MScale2";
            this.MScale2.Size = new System.Drawing.Size(87, 22);
            this.MScale2.Text = "2";
            this.MScale2.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale3
            // 
            this.MScale3.Name = "MScale3";
            this.MScale3.Size = new System.Drawing.Size(87, 22);
            this.MScale3.Text = "3";
            this.MScale3.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale4
            // 
            this.MScale4.Name = "MScale4";
            this.MScale4.Size = new System.Drawing.Size(87, 22);
            this.MScale4.Text = "4";
            this.MScale4.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale5
            // 
            this.MScale5.Name = "MScale5";
            this.MScale5.Size = new System.Drawing.Size(87, 22);
            this.MScale5.Text = "5";
            this.MScale5.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale6
            // 
            this.MScale6.Name = "MScale6";
            this.MScale6.Size = new System.Drawing.Size(87, 22);
            this.MScale6.Text = "6";
            this.MScale6.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale8
            // 
            this.MScale8.Name = "MScale8";
            this.MScale8.Size = new System.Drawing.Size(87, 22);
            this.MScale8.Text = "8";
            this.MScale8.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale9
            // 
            this.MScale9.Name = "MScale9";
            this.MScale9.Size = new System.Drawing.Size(87, 22);
            this.MScale9.Text = "9";
            this.MScale9.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale12
            // 
            this.MScale12.Name = "MScale12";
            this.MScale12.Size = new System.Drawing.Size(87, 22);
            this.MScale12.Text = "12";
            this.MScale12.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale14
            // 
            this.MScale14.Name = "MScale14";
            this.MScale14.Size = new System.Drawing.Size(87, 22);
            this.MScale14.Text = "14";
            this.MScale14.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale15
            // 
            this.MScale15.Name = "MScale15";
            this.MScale15.Size = new System.Drawing.Size(87, 22);
            this.MScale15.Text = "15";
            this.MScale15.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale16
            // 
            this.MScale16.Name = "MScale16";
            this.MScale16.Size = new System.Drawing.Size(87, 22);
            this.MScale16.Text = "16";
            this.MScale16.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale18
            // 
            this.MScale18.Name = "MScale18";
            this.MScale18.Size = new System.Drawing.Size(87, 22);
            this.MScale18.Text = "18";
            this.MScale18.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale32
            // 
            this.MScale32.Name = "MScale32";
            this.MScale32.Size = new System.Drawing.Size(87, 22);
            this.MScale32.Text = "32";
            this.MScale32.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MScale64
            // 
            this.MScale64.Name = "MScale64";
            this.MScale64.Size = new System.Drawing.Size(87, 22);
            this.MScale64.Text = "64";
            this.MScale64.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // MTopDown
            // 
            this.MTopDown.Name = "MTopDown";
            this.MTopDown.Size = new System.Drawing.Size(87, 22);
            this.MTopDown.Text = "->";
            this.MTopDown.Click += new System.EventHandler(this.MScale1_Click);
            // 
            // BDivZoom
            // 
            this.BDivZoom.Name = "BDivZoom";
            this.BDivZoom.Size = new System.Drawing.Size(6, 26);
            // 
            // BZoomMinus
            // 
            this.BZoomMinus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BZoomMinus.Image = ((System.Drawing.Image)(resources.GetObject("BZoomMinus.Image")));
            this.BZoomMinus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BZoomMinus.Name = "BZoomMinus";
            this.BZoomMinus.Size = new System.Drawing.Size(23, 23);
            this.BZoomMinus.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // BZoomPlus
            // 
            this.BZoomPlus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BZoomPlus.Image = ((System.Drawing.Image)(resources.GetObject("BZoomPlus.Image")));
            this.BZoomPlus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BZoomPlus.Name = "BZoomPlus";
            this.BZoomPlus.Size = new System.Drawing.Size(23, 23);
            this.BZoomPlus.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // bdivexit
            // 
            this.bdivexit.Name = "bdivexit";
            this.bdivexit.Size = new System.Drawing.Size(6, 26);
            // 
            // BExit
            // 
            this.BExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BExit.Image = ((System.Drawing.Image)(resources.GetObject("BExit.Image")));
            this.BExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BExit.Name = "BExit";
            this.BExit.Size = new System.Drawing.Size(23, 23);
            this.BExit.Text = "BExit";
            this.BExit.Click += new System.EventHandler(this.bsfirst_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "PDF File|*.pdf|PDF File (Uncompressed)|*.pdf";
            this.saveFileDialog1.FilterIndex = 2;
            this.saveFileDialog1.SupportMultiDottedExtensions = true;
            // 
            // PreviewWinFormsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.maincontainer);
            this.Name = "PreviewWinFormsControl";
            this.Size = new System.Drawing.Size(632, 443);
            this.Load += new System.EventHandler(this.PreviewWinForms2_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ExecuteKeyDown);
            this.maincontainer.BottomToolStripPanel.ResumeLayout(false);
            this.maincontainer.BottomToolStripPanel.PerformLayout();
            this.maincontainer.ContentPanel.ResumeLayout(false);
            this.maincontainer.TopToolStripPanel.ResumeLayout(false);
            this.maincontainer.TopToolStripPanel.PerformLayout();
            this.maincontainer.ResumeLayout(false);
            this.maincontainer.PerformLayout();
            this.mainstatus.ResumeLayout(false);
            this.mainstatus.PerformLayout();
            this.maintoolstrip.ResumeLayout(false);
            this.maintoolstrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer maincontainer;
        private System.Windows.Forms.StatusStrip mainstatus;
        private Reportman.Drawing.Forms.ToolStripAdvanced maintoolstrip;
        private System.Windows.Forms.ToolStripTextBox EPage;
        private System.Windows.Forms.ToolStripSeparator bdivsearch;
        private System.Windows.Forms.ToolStripTextBox textsearch;
        private System.Windows.Forms.ToolStripSeparator BDivPageSetup;
        private System.Windows.Forms.ToolStripSeparator BDivParams;
        private System.Windows.Forms.ToolStripSeparator bdivscale;
        private System.Windows.Forms.ToolStripSeparator BDivZoom;
        private System.Windows.Forms.ToolStripSeparator bdivexit;
        private System.Windows.Forms.ToolStripStatusLabel StatusPage;
        private System.Windows.Forms.ToolStripStatusLabel BarStatusEdit;
        private System.Windows.Forms.Panel PParent;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem MScale1;
        private System.Windows.Forms.ToolStripMenuItem MScale2;
        private System.Windows.Forms.ToolStripMenuItem MScale3;
        private System.Windows.Forms.ToolStripMenuItem MScale4;
        private System.Windows.Forms.ToolStripMenuItem MScale5;
        private System.Windows.Forms.ToolStripMenuItem MScale6;
        private System.Windows.Forms.ToolStripMenuItem MScale8;
        private System.Windows.Forms.ToolStripMenuItem MScale9;
        private System.Windows.Forms.ToolStripMenuItem MScale12;
        private System.Windows.Forms.ToolStripMenuItem MScale14;
        private System.Windows.Forms.ToolStripMenuItem MScale15;
        private System.Windows.Forms.ToolStripMenuItem MScale16;
        private System.Windows.Forms.ToolStripMenuItem MScale18;
        private System.Windows.Forms.ToolStripMenuItem MScale32;
        private System.Windows.Forms.ToolStripMenuItem MScale64;
        private System.Windows.Forms.ToolStripMenuItem MTopDown;
        public System.Windows.Forms.ToolStripButton bsfirst;
        public System.Windows.Forms.ToolStripButton bsprior;
        public System.Windows.Forms.ToolStripButton bsnext;
        public System.Windows.Forms.ToolStripButton bslast;
        public System.Windows.Forms.ToolStripButton bssearch;
        public System.Windows.Forms.ToolStripButton BPageSetup;
        public System.Windows.Forms.ToolStripButton BParameters;
        public System.Windows.Forms.ToolStripButton BPrint;
        public System.Windows.Forms.ToolStripButton BSave;
        public System.Windows.Forms.ToolStripButton BMail;
        public System.Windows.Forms.ToolStripButton BScaleWide;
        public System.Windows.Forms.ToolStripButton BScaleFull;
        public System.Windows.Forms.ToolStripButton BZoomMinus;
        public System.Windows.Forms.ToolStripButton BZoomPlus;
        public System.Windows.Forms.ToolStripButton BExit;
        public System.Windows.Forms.ToolStripSplitButton BScaleEntire;
    }
}