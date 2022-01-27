using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Reportman.Designer
{
    public delegate bool EllipsisClick(EllipsisEditingControl sender,ref string text);
	public partial class EllipsisEditingControl : UserControl, IDataGridViewEditingControl
	{
		public DataGridView m_dataGridView = null;
		int m_rowIndex = 0;
		bool m_valueChanged = false;
		string m_prevText = null;
        public object Data;
        public event EllipsisClick ButtonClick;
        public bool AssignedEvent = false;
        public override string Text
        {
            get
            {
                return textcontrol.Text;
            }
            set
            {
                textcontrol.Text = value;
            }
        }
		public EllipsisEditingControl()
		{
			InitializeComponent();
			this.textcontrol.LostFocus += new EventHandler(filePathTextBox_LostFocus);
            this.textcontrol.TextChanged += new EventHandler(nvaluechange);
        }
        private void nvaluechange(object sender, EventArgs ev)
        {
            NotifyChange();
        }
		void filePathTextBox_LostFocus(object sender, EventArgs e)
		{
			NotifyChange();
		}

		private void browseButton_Click(object sender, EventArgs e)
		{
            bool aresult = false;
            string ntext = this.textcontrol.Text;
            if (ButtonClick != null)
            {
                aresult = ButtonClick(this,ref ntext);
            }
            if (aresult)
            {
                this.textcontrol.Text = ntext;
                NotifyChange();
            }
        }

		private void NotifyChange()
		{
			if (this.textcontrol.Text != m_prevText)
			{
				m_valueChanged = true;
				m_dataGridView.NotifyCurrentCellDirty(true);
			}
		}

		#region IDataGridViewEditingControl Members

		public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
		{
			// Do nothing
		}

		public Cursor EditingControlCursor
		{
			get 
			{
				return Cursors.IBeam;
			}
		}


		public Cursor EditingPanelCursor
		{
			get
			{
				return Cursors.IBeam;
			}
		}

		public DataGridView EditingControlDataGridView
		{
			get
			{
				return m_dataGridView;
			}
			set
			{
				m_dataGridView = value;
			}
		}

		public object EditingControlFormattedValue
		{
			get
			{
				return this.textcontrol.Text;
			}
			set 
			{
				this.textcontrol.Text = value.ToString();
			}
		}

		public int EditingControlRowIndex
		{
			get
			{
				return m_rowIndex;
			}
			set
			{
				m_rowIndex = value;
			}
		}

		public bool EditingControlValueChanged
		{
			get
			{
				return m_valueChanged;
			}
			set
			{
				m_valueChanged = value;
			}
		}

		public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
		{
			switch (keyData)
			{
				case Keys.Tab:
					return true;
				case Keys.Home:
				case Keys.End:
				case Keys.Left:
					if (this.textcontrol.SelectionLength == this.textcontrol.Text.Length)
						return false;
					else
						return true;
				case Keys.Right:
					return true;
				case Keys.Delete:
					this.textcontrol.Text = "";
					return true;
				case Keys.Enter:
					NotifyChange();
					return false;
				default:
					return false;
			}
		}

		public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
		{
			return this.textcontrol.Text;
		}

		public void PrepareEditingControlForEdit(bool selectAll)
		{
			if (this.m_dataGridView.CurrentCell.Value == null)
				this.textcontrol.Text = "";
			else
				this.textcontrol.Text = this.m_dataGridView.CurrentCell.Value.ToString();
			if (selectAll)
				this.textcontrol.SelectAll();
			m_prevText = this.textcontrol.Text;
		}

		public bool RepositionEditingControlOnValueChange
		{
			get 
			{
				return false;
			}
		}

		#endregion

	}
}
