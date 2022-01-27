#region Copyright
/*
 *  Report Manager:  Database Reporting tool for .Net and Mono
 *
 *     The contents of this file are subject to the MPL License
 *     with optional use of GPL or LGPL licenses.
 *     You may not use this file except in compliance with the
 *     Licenses. You may obtain copies of the Licenses at:
 *     http://reportman.sourceforge.net/license
 *
 *     Software is distributed on an "AS IS" basis,
 *     WITHOUT WARRANTY OF ANY KIND, either
 *     express or implied.  See the License for the specific
 *     language governing rights and limitations.
 *
 *  Copyright (c) 1994 - 2008 Toni Martir (toni@reportman.es)
 *  All Rights Reserved.
*/
#endregion

using System;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing;

namespace Reportman.Drawing.Forms
{
	/// <summary>
	/// Nullable DateTime picker
	/// </summary>	
	public class DateTimePickerNullable : System.Windows.Forms.DateTimePicker   
	{
		public static readonly System.DateTime MaxDateValue = new System.DateTime(9997,12,31);
		public static readonly System.DateTime MinDateValue = new System.DateTime(1900,12,31);
		private DateTimePickerFormat oldFormat = DateTimePickerFormat.Long;
		private string oldCustomFormat = null;
		private bool bIsNull = false;

		public DateTimePickerNullable() : base()
		{
            MaxDate = MaxDateValue;
            MinDate = System.DateTime.MinValue;
		}
        public bool EnterAsTab { get; set; }

        public BeforeEnterTabEvent BeforeEnterTab;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (EnterAsTab)
            {
                if (keyData == (Keys.Enter))
                {
                    bool cancelled = false;
                    BeforeEnterTab(ref cancelled);
                    if (!cancelled)
                        SendKeys.Send("{TAB}");
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public new DateTime Value 
		{
			get 
			{
				if (bIsNull)
					return DateTime.MinValue;
				else
					return base.Value;
			}
			set 
			{
				if (value == DateTime.MinValue)
				{
					if (bIsNull == false) 
					{
						oldFormat = this.Format;
						oldCustomFormat = this.CustomFormat;
						bIsNull = true;
					}

					this.Format = DateTimePickerFormat.Custom;
					this.CustomFormat = " ";
                }
				else 
				{
					if (bIsNull) 
					{
						this.Format = oldFormat;
						this.CustomFormat = oldCustomFormat;
						bIsNull = false;
					}
					base.Value = value;
				}
			}
		}
        protected override void OnValueChanged(EventArgs eventargs)
        {
            bIsNull = (Value == DateTime.MinValue);
            base.OnValueChanged(eventargs);
        }
        protected override void OnValidated(EventArgs e)
        {
	        bIsNull = (Value == DateTime.MinValue);
            base.OnValidated(e);
        }
		protected override void OnCloseUp(EventArgs eventargs)
		{
			if (Control.MouseButtons == MouseButtons.None) 
			{
				if (bIsNull) 
				{
					this.Format = oldFormat;
					this.CustomFormat = oldCustomFormat;
					bIsNull = false;
				}
			}
			base.OnCloseUp (eventargs);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
            if (EnterAsTab)
            {
                if (e.KeyCode == Keys.Return)
                    e.SuppressKeyPress = true;
            }
            base.OnKeyDown (e);
        if (this.Value == DateTime.MinValue)
        {

            char keychar = '0';

            if ((e.KeyValue >= (int)Keys.NumPad0) && (e.KeyValue <= (int)Keys.NumPad9))
            {
                keychar = (char)(((byte)keychar) + (e.KeyValue - Keys.NumPad0));
                this.Value = DateTime.Today;
            }
            else if (e.KeyValue >= ((int)Keys.D0) && e.KeyValue <= ((int)Keys.D9))
            {
                keychar = (char)(((byte)keychar) + (e.KeyValue - Keys.D0));
                this.Value = DateTime.Today;
            }
            OnValueChanged(new EventArgs());

            SendKeys.Send("{RIGHT 1}");
            SendKeys.Send(keychar.ToString());
            e.Handled = true;
        }
            if (e.KeyCode == Keys.Delete)
            {
                this.Value = DateTime.MinValue;
                OnValueChanged(new EventArgs());
            }
        
		}
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
#if MONO
			base.OnPaint(e);
            Graphics g = this.CreateGraphics();
            Rectangle dropDownRectangle =
               new Rectangle(ClientRectangle.Width - 17, 0, 17, 16);
            Brush bkgBrush;
            ComboBoxState visualState;

            //When the control is enabled the brush is set to Backcolor, 
            //otherwise to color stored in _backDisabledColor
            if (this.Enabled)
            {
                bkgBrush = new SolidBrush(this.BackColor);
                visualState = ComboBoxState.Normal;
            }
            else
            {
                bkgBrush = new SolidBrush(SystemColors.ButtonFace);
                visualState = ComboBoxState.Disabled;
            }
            // Painting...in action

            //Filling the background
            g.FillRectangle(bkgBrush, 0, 0, ClientRectangle.Width, ClientRectangle.Height);

            //Drawing the datetime text
            g.DrawString(this.Text, this.Font, Brushes.Black, 0, 2);

            //Drawing the dropdownbutton using ComboBoxRenderer
			if (ComboBoxRenderer.IsSupported)
            	ComboBoxRenderer.DrawDropDownButton(g, dropDownRectangle, visualState);
			{
				using (SolidBrush grebrush = new SolidBrush(SystemColors.ButtonFace))
				{
					g.FillRectangle(grebrush,dropDownRectangle);
					using (Pen bpen = new Pen(SystemColors.WindowText))
					{
						g.DrawRectangle(bpen,dropDownRectangle);
						StringFormat nformat = new StringFormat();
						nformat.Alignment = StringAlignment.Center;
						nformat.LineAlignment = StringAlignment.Center;
						g.DrawString("-",this.Font,Brushes.Black,dropDownRectangle,nformat);
					}
				}
			}

            g.Dispose();
            bkgBrush.Dispose();
			return;
#else
            Graphics g = this.CreateGraphics();

            //The dropDownRectangle defines position and size of dropdownbutton block, 
            //the width is fixed to 17 and height to 16. 
            //The dropdownbutton is aligned to right
            Rectangle dropDownRectangle =
               new Rectangle(ClientRectangle.Width - 17, 0, 17, 16);
            Brush bkgBrush;
            ComboBoxState visualState;

            //When the control is enabled the brush is set to Backcolor, 
            //otherwise to color stored in _backDisabledColor
            if (this.Enabled)
            {
                bkgBrush = new SolidBrush(this.BackColor);
                visualState = ComboBoxState.Normal;
            }
            else
            {
                bkgBrush = new SolidBrush(SystemColors.ButtonFace);
                visualState = ComboBoxState.Disabled;
            }

            // Painting...in action

            //Filling the background
            g.FillRectangle(bkgBrush, 0, 0, ClientRectangle.Width, ClientRectangle.Height);

            //Drawing the datetime text
            g.DrawString(this.Text, this.Font, Brushes.Black, 0, 2);

            //Drawing the dropdownbutton using ComboBoxRenderer
            ComboBoxRenderer.DrawDropDownButton(g, dropDownRectangle, visualState);

            g.Dispose();
            bkgBrush.Dispose();
#endif
        }
	}
}
