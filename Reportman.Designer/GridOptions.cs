using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Reportman.Drawing;
using Reportman.Reporting;
using System.Text;
using System.Windows.Forms;

namespace Reportman.Designer
{
    public partial class GridOptions : Form
    {
        public GridOptions()
        {
            InitializeComponent();

             bok.Text = Translator.TranslateStr(93);
             bcancel.Text = Translator.TranslateStr(94);
             Text = Translator.TranslateStr(94);

             lhorzontal.Text = Translator.TranslateStr(180);
             lvertical.Text = Translator.TranslateStr(181);
             lcolor.Text = Translator.TranslateStr(185);
             lenabled.Text = Translator.TranslateStr(182);              
             lvisible.Text = Translator.TranslateStr(183);
             lcombogridstyle.Text = Translator.TranslateStr(646);

            combostyle.Items[0] = Translator.TranslateStr(896);
            combostyle.Items[1] = Translator.TranslateStr(184);
        }
        public static bool AlterGridOptions(Report nreport)
        {
            bool nresult = false;

            using (GridOptions ndia = new GridOptions())
            {
                ndia.bcolor.BackColor = GraphicUtils.ColorFromInteger(nreport.GridColor);
                ndia.checkenabled.Checked = nreport.GridEnabled;
                ndia.textwidth.Text = Twips.TextFromTwips(nreport.GridWidth);
                ndia.textheight.Text = Twips.TextFromTwips(nreport.GridHeight);
                if (nreport.GridLines)
                    ndia.combostyle.SelectedIndex = 1;
                else
                    ndia.combostyle.SelectedIndex = 0;
                ndia.checkvisible.Checked = nreport.GridVisible;
                if (ndia.ShowDialog() == DialogResult.OK)
                {
                    nreport.GridLines = (ndia.combostyle.SelectedIndex == 1);
                    nreport.GridEnabled = ndia.checkenabled.Checked;
                    nreport.GridWidth = Twips.TwipsFromText(ndia.textwidth.Text);
                    nreport.GridHeight = Twips.TwipsFromText(ndia.textheight.Text);
                    nreport.GridColor = GraphicUtils.IntegerFromColor(ndia.bcolor.BackColor);
                    nreport.GridVisible = ndia.checkvisible.Checked;
                    nresult = true;
                }
            }
            return nresult;
        }

        private void bcolor_Click(object sender, EventArgs e)
        {
            colordialog1.Color = bcolor.BackColor;
            if (colordialog1.ShowDialog()==DialogResult.OK)
                bcolor.BackColor = colordialog1.Color;
        }
    }
}
