using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Reportman.Drawing;

namespace Reportman.Drawing.Forms
{
	/// <summary>
	/// Descripción breve de MessageBoxError.
	/// </summary>
	public class MessageBoxError : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel paneltop;
		private System.Windows.Forms.Panel panelbottom;
		private System.Windows.Forms.Button bok;
        private System.Windows.Forms.Button bcancel;
		private System.Windows.Forms.Button bdetail;
		private System.Windows.Forms.TextBox textdetalle;
        private TextBox lerror;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MessageBoxError()
		{
			//
			// Necesario para admitir el Diseñador de Windows Forms
			//
			InitializeComponent();

			//
			// TODO: agregar código de constructor después de llamar a InitializeComponent
			//
			Text=Translator.TranslateStr(12);
			bok.Text=Translator.TranslateStr(93);
			bcancel.Text=Translator.TranslateStr(94);
			bdetail.Text=Translator.TranslateStr(129);
		}

		/// <summary>
		/// Limpiar los recursos que se estén utilizando.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
        public static DialogResult ShowThreadExceptionDialog(Form ParentForm,string Caption, string message, Exception e,bool ShowInTaskBar)
        {
            DialogResult aresult;
            using (MessageBoxError dia = new MessageBoxError())
            {
                if (Caption.Length > 0)
                    dia.Text = Caption;
                dia.textdetalle.Text = e.StackTrace;
                dia.ShowInTaskbar = ShowInTaskBar;
                dia.lerror.Text = e.Message+(char)13+(char)10+message;
                if (ParentForm == null)
                    aresult = dia.ShowDialog();
                else
                    aresult = dia.ShowDialog(ParentForm);
                if (aresult == DialogResult.Cancel)
                    aresult = DialogResult.OK;
            }
            return aresult;

        }
        public static DialogResult ShowThreadExceptionDialog(Form MainForm,string Caption, Exception e)
		{
            return ShowThreadExceptionDialog(MainForm,Caption, "",e,false);
		}


		#region Código generado por el Diseñador de Windows Forms
		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            this.paneltop = new System.Windows.Forms.Panel();
            this.lerror = new System.Windows.Forms.TextBox();
            this.textdetalle = new System.Windows.Forms.TextBox();
            this.bdetail = new System.Windows.Forms.Button();
            this.panelbottom = new System.Windows.Forms.Panel();
            this.bcancel = new System.Windows.Forms.Button();
            this.bok = new System.Windows.Forms.Button();
            this.paneltop.SuspendLayout();
            this.panelbottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // paneltop
            // 
            this.paneltop.Controls.Add(this.lerror);
            this.paneltop.Controls.Add(this.textdetalle);
            this.paneltop.Controls.Add(this.bdetail);
            this.paneltop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paneltop.Location = new System.Drawing.Point(0, 0);
            this.paneltop.Name = "paneltop";
            this.paneltop.Size = new System.Drawing.Size(615, 387);
            this.paneltop.TabIndex = 2;
            // 
            // lerror
            // 
            this.lerror.Location = new System.Drawing.Point(8, 12);
            this.lerror.Multiline = true;
            this.lerror.Name = "lerror";
            this.lerror.ReadOnly = true;
            this.lerror.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.lerror.Size = new System.Drawing.Size(591, 113);
            this.lerror.TabIndex = 5;
            // 
            // textdetalle
            // 
            this.textdetalle.Location = new System.Drawing.Point(8, 169);
            this.textdetalle.Multiline = true;
            this.textdetalle.Name = "textdetalle";
            this.textdetalle.ReadOnly = true;
            this.textdetalle.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textdetalle.Size = new System.Drawing.Size(591, 150);
            this.textdetalle.TabIndex = 4;
            this.textdetalle.Visible = false;
            // 
            // bdetail
            // 
            this.bdetail.Location = new System.Drawing.Point(480, 131);
            this.bdetail.Name = "bdetail";
            this.bdetail.Size = new System.Drawing.Size(128, 32);
            this.bdetail.TabIndex = 3;
            this.bdetail.Text = "Detalle";
            this.bdetail.Click += new System.EventHandler(this.bdetail_Click);
            // 
            // panelbottom
            // 
            this.panelbottom.Controls.Add(this.bcancel);
            this.panelbottom.Controls.Add(this.bok);
            this.panelbottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelbottom.Location = new System.Drawing.Point(0, 339);
            this.panelbottom.Name = "panelbottom";
            this.panelbottom.Size = new System.Drawing.Size(615, 48);
            this.panelbottom.TabIndex = 1;
            // 
            // bcancel
            // 
            this.bcancel.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.bcancel.Location = new System.Drawing.Point(224, 8);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(128, 32);
            this.bcancel.TabIndex = 1;
            this.bcancel.Text = "Cancel";
            this.bcancel.Visible = false;
            // 
            // bok
            // 
            this.bok.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.bok.Location = new System.Drawing.Point(8, 8);
            this.bok.Name = "bok";
            this.bok.Size = new System.Drawing.Size(128, 32);
            this.bok.TabIndex = 0;
            this.bok.Text = "Ignore";
            // 
            // MessageBoxError
            // 
            this.AcceptButton = this.bok;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.bcancel;
            this.ClientSize = new System.Drawing.Size(615, 387);
            this.Controls.Add(this.panelbottom);
            this.Controls.Add(this.paneltop);
            this.Name = "MessageBoxError";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MessageBoxError";
            this.paneltop.ResumeLayout(false);
            this.paneltop.PerformLayout();
            this.panelbottom.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void bdetail_Click(object sender, System.EventArgs e)
		{
			bdetail.Visible=false;
			textdetalle.Visible=true;
			Height = Height+textdetalle.Height+30;
		}
	}
}
