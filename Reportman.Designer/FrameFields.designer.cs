namespace Reportman.Designer
{
    partial class FrameFields
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
            this.RView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // RView
            // 
            this.RView.AllowDrop = true;
            this.RView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RView.HideSelection = false;
            this.RView.Location = new System.Drawing.Point(0, 0);
            this.RView.Name = "RView";
            this.RView.Size = new System.Drawing.Size(319, 301);
            this.RView.TabIndex = 0;
            this.RView.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.RView_GiveFeedback);
            this.RView.DragLeave += new System.EventHandler(this.RView_DragLeave);
            this.RView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.RView_BeforeExpand);
            this.RView.DragEnter += new System.Windows.Forms.DragEventHandler(this.RView_DragEnter);
            this.RView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.RView_ItemDrag);
            // 
            // FrameFields
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RView);
            this.Name = "FrameFields";
            this.Size = new System.Drawing.Size(319, 301);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView RView;
    }
}
