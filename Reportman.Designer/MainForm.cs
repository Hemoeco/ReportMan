using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Reportman.Designer
{
    public partial class MainForm : Form
    {
        public FrameMainDesigner maindesigner;
        public MainForm()
        {
            InitializeComponent();

            maindesigner = new FrameMainDesigner();
            maindesigner.Dock = DockStyle.Fill;
            this.Controls.Add(maindesigner);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Browse the command line
            string[] args = System.Environment.GetCommandLineArgs();
            if (args.Length>1)
                maindesigner.OpenFile(args[1]);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (maindesigner != null)
            {
                e.Cancel = !maindesigner.CheckSave();
            }
        }
    }
}