using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Reportman.Drawing;

namespace Reportman.Drawing.Forms
{
  public partial class UpdateForm : Form
  {
    private Updater update;
    private bool working;
    private DataTable tfiles;
    public UpdateForm()
    {
      InitializeComponent();
      larchivo.Text = "";
      lkbytes.Text = "";
    }
    public static void DoUpdate(string FilePath,DataTable files,bool DoBackup)
    {
      using (UpdateForm ndia = new UpdateForm())
      {
        ndia.tfiles = files;
        ndia.update = new Updater(FilePath);
        ndia.update.PerformBackup = DoBackup;
        ndia.ShowDialog();
      }
    }
    public void PerformUpdate()
    {
      CopyProgress nevent =new CopyProgress(CopyProgress); 
      update.OnProgress += nevent;
      working = true;
      try
      {
        update.Update(tfiles);
      }
      finally
      {
        working = false;
        update.OnProgress -= nevent;
      }
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      timer1.Enabled = false;
      try
      {
        PerformUpdate();
      }
      finally
      {
        Close();
      }
    }
    public void CopyProgress(string filename, int file,
     int filecount, int position, int size, ref bool docancel)
    {
      progarchivo.Maximum = filecount;
      larchivo.Text = filename;
      progarchivo.Value = file;
      if (progkbytes.Value > size)
      {
        progkbytes.Value = 0;
      }
      progkbytes.Maximum = size;
      progkbytes.Value = position;
      lkbytes.Text = StringUtil.GetSizeAsString((int)position) + " de " + StringUtil.GetSizeAsString((int)size);
      Application.DoEvents();
    }
    private void UpdateForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (working)
        e.Cancel = true;
    }
  }
}