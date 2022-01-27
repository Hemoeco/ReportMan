using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reportman.Drawing.Forms
{
    public partial class InputBox : Form
    {
        private bool dook;
        private bool isdate;
        public InputBox()
        {
            InitializeComponent();
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            bok.Text = Translator.TranslateStr(93);
            bcancel.Text = Translator.TranslateStr(271);
        }
        public static string Execute(string caption, string prompt, string defaultvalue)
        {
            string aresult = defaultvalue;
            using (InputBox dia = new InputBox())
            {
                dia.Text = caption;
                dia.ltext.Text = prompt;
                dia.EditText.Text = defaultvalue;
                dia.ShowDialog();
                if (dia.dook)
                    aresult = dia.EditText.Text;
            }
            return aresult;
        }
        public static bool Execute(string caption, string prompt, ref DateTime value,string dateformat)
        {
            bool aresult = false;
            using (InputBox dia = new InputBox())
            {
                dia.Text = caption;
                dia.ltext.Text = prompt;
                dia.datepicker.Value = value;
                if (dateformat.Length > 0)
                    dia.datepicker.CustomFormat = dateformat;
                dia.isdate = true;
                dia.ShowDialog();
                if (dia.dook)
                {
                    value = dia.datepicker.Value;
                    aresult = true;
                }
            }
            return aresult;
        }
        public static bool Execute(string caption, string prompt, ref DateTime value)
        {
            return Execute(caption, prompt, ref value, "");
        }

        private void bok_Click(object sender, EventArgs e)
        {
            dook = true;
            Close();
        }

        private void bcancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void InputBox_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void InputBox_Shown(object sender, EventArgs e)
        {
            if (isdate)
            {
                datepicker.Visible = true;
                EditText.Visible = false;
                maintable.Controls.Remove(EditText);
                maintable.Controls.Add(datepicker);
                datepicker.Focus();
            }
            else
                EditText.Focus();
            this.ClientSize = new Size(this.Width, maintable.Height);
        }
    }
}