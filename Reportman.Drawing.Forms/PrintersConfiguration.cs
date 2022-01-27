using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Reportman.Drawing;
using Reportman.Drawing.Forms;

namespace Reportman.Drawing.Forms
{
    public delegate void EventPrinterNamesHandler (object sender,EventPrinterNamesArgs args);
    public partial class PrintersConfiguration : Form
    {
        EventPrinterNamesHandler OnGetNames;
        bool userconfig;
        string systemconfigfilename;
        string userconfigfilename;
        string configfilename;
        IniFile configfile;
        public Strings printernames;
        public MemoryStream StreamResult;
        public Stream configstream;
        private void ReadPrintersConfig()
        {
            if (configstream != null)
            {
                configstream.Seek(0, SeekOrigin.Begin);
                configfile = new IniFile(configstream);
                gconfigfile.Visible = false;
            }
            else
            {
                userconfig = true;
                systemconfigfilename = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                systemconfigfilename = Path.Combine(systemconfigfilename, "reportman.ini");
                userconfigfilename = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                userconfigfilename = Path.Combine(userconfigfilename, "reportman.ini");
                configfilename = userconfigfilename;
                if (File.Exists(systemconfigfilename))
                {
                    if (!File.Exists(userconfigfilename))
                    {
                        configfilename = systemconfigfilename;
                        userconfig = false;
                    }
                }
                configfile = new IniFile(configfilename);
            }
        }
        public void Initialize()
        {
            ReadPrintersConfig();
            if (printernames == null)
            {
                printernames = new Strings();
                foreach (string pname in PrinterSettings.InstalledPrinters)
                {
                    comboprinters.Items.Add(pname);
                }
            }
            foreach (string pname in printernames)
            {
                comboprinters.Items.Add(pname);
            }
            comboprinters.Items.Insert(0, Translator.TranslateStr(467));
            foreach (string textdriver in PrinterConfig.GetTextOnlyPrintDrivers())
                combotextdriver.Items.Add(textdriver);
            bok.Text = Translator.TranslateStr(93);
            bcancel.Text = Translator.TranslateStr(94);
            lprinterdevice.Text = Translator.TranslateStr(741);
            gconfigfile.Text = Translator.TranslateStr(743);
            radiouserconfig.Text = Translator.TranslateStr(744);
            radiosystemconfig.Text = Translator.TranslateStr(745);
            this.Text = Translator.TranslateStr(742);
            goperations.Text = Translator.TranslateStr(763);
            lsample1.Text = Translator.TranslateStr(764);
            lsample2.Text = Translator.TranslateStr(765);
            checkcutpaper.Text = Translator.TranslateStr(766);
            checkopendrawer.Text = Translator.TranslateStr(767);
            ltextdriver.Text = Translator.TranslateStr(1058);


            SortedList<int, string> altnames = new SortedList<int, string>();

            if (OnGetNames != null)
            {
                EventPrinterNamesArgs nargs = new EventPrinterNamesArgs(new SortedList<int, string>());
                OnGetNames(this, nargs);
                altnames = nargs.PrinterNames;
            }

            ldefinedprinters.Items.Clear();

            Strings nstrings = PrinterConfig.GetConfigurablePrinters();
            for (int i = 0; i < nstrings.Count; i++)
            {
                string printname = nstrings[i];
                string[] titles = new string[3];
                titles[0] = i.ToString();
                titles[1] = printname;
                titles[2] = "";
                if (altnames.IndexOfKey(i)>=0)
                    titles[2] = altnames[i];
                ldefinedprinters.Items.Add(new ListViewItem(titles));
            }

            ldefinedprinters.Items[0].Selected = true;
            ldefinedprinters.Items[0].Focused = true;

            radiosystemconfig.Checked = !userconfig;
            radiouserconfig.Checked = userconfig;
        }
        public PrintersConfiguration()
        {
            InitializeComponent();
            if (GraphicUtils.DPIScale != 1.0)
            {
                foreach (ColumnHeader column in ldefinedprinters.Columns)
                {
                    column.Width = Convert.ToInt32(column.Width * GraphicUtils.DPIScale);
                }
            }

        }
        public static MemoryStream ShowPrintersConfiguration(Form ParentForm,Strings printernames,Stream configstream,
                    EventPrinterNamesHandler OnGetNames)
        {
            MemoryStream nresult = null;
            using (PrintersConfiguration dia = new PrintersConfiguration())
            {
                dia.printernames = printernames;
                dia.configstream = configstream;
                dia.OnGetNames = OnGetNames;
                dia.Initialize();
                if (dia.ShowDialog(ParentForm) == DialogResult.OK)
                    nresult = dia.StreamResult;
            }
            return nresult;
        }
        public static MemoryStream ShowPrintersConfiguration(Form ParentForm)
        {
            return ShowPrintersConfiguration(ParentForm, null, null,null);
        }

        private void checktextdriver_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void PrintersConfiguration_Load(object sender, EventArgs e)
        {

        }

        private void radiouserconfig_CheckedChanged(object sender, EventArgs e)
        {
            if (radiouserconfig.Checked)
                textconfigfile.Text = userconfigfilename;
            else
                textconfigfile.Text = systemconfigfilename;
        }

        private void ldefinedprinters_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void bok_Click(object sender, EventArgs e)
        {
            if (configstream==null)
                configfile.SaveToFile(configfilename);
            StreamResult = new MemoryStream();
            configfile.SaveToStream(StreamResult);
            StreamResult.Seek(0, SeekOrigin.Begin);
            PrinterConfig.ReloadParameters();
            DialogResult = DialogResult.OK;
        }

        private void bcancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void comboprinters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ldefinedprinters.SelectedIndices.Count != 1)
                return;
            int index = ldefinedprinters.SelectedIndices[0];
            string printername;
            if (index == 0)
                printername = "";
            else
                printername = comboprinters.Text;
            configfile.WriteString("PrinterNames",
                "Printer"+index.ToString(),printername);
        }

        private void checkoem_CheckedChanged(object sender, EventArgs e)
        {
            if (ldefinedprinters.SelectedIndices.Count != 1)
                return;
            int index = ldefinedprinters.SelectedIndices[0];
            configfile.WriteBool("PrinterEscapeOem",
                "Printer"+index.ToString(),checkoem.Checked);
        }

        private void combotextdriver_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ldefinedprinters.SelectedIndices.Count != 1)
                return;
            int index = ldefinedprinters.SelectedIndices[0];
            string drivername = combotextdriver.Text.Trim().ToUpper();
            if (drivername.Length != 0)
            {
                //configfile.WriteInteger("PrinterEscapeStyle",
                //    "Printer"+ldefinedprinters.SelectedIndex.ToString(),(int).DefaultPrinter);
                configfile.WriteString("PrinterDriver",
                    "Printer"+index.ToString(),drivername);

            }
            else
            {
                //configfile.WriteInteger("PrinterEscapeStyle","Printer"+IntToStr(LSelPrinter.ItemIndex),Integer(rpPrinterDefault));
                configfile.WriteString("PrinterDriver", 
                    "Printer" + index.ToString(), " ");
            }
        }

        private void checkcutpaper_CheckedChanged(object sender, EventArgs e)
        {
            if (ldefinedprinters.SelectedIndices.Count != 1)
                return;
            int index = ldefinedprinters.SelectedIndices[0];
            configfile.WriteBool("CutPaperOn",
                "Printer"+index.ToString(),checkcutpaper.Checked);
        }

        private void checkopendrawer_CheckedChanged(object sender, EventArgs e)
        {
            if (ldefinedprinters.SelectedIndices.Count != 1)
                return;
            int index = ldefinedprinters.SelectedIndices[0];
            configfile.WriteBool("OpenDrawerOn",
                "Printer" + index.ToString(), checkopendrawer.Checked);
        }

        private void textescapecutpaper_TextChanged(object sender, EventArgs e)
        {
            if (ldefinedprinters.SelectedIndices.Count != 1)
                return;
            int index = ldefinedprinters.SelectedIndices[0];
            configfile.WriteString("CutPaper",
                 "Printer"+index.ToString(),textescapecutpaper.Text);
        }

        private void textescapeopendrawer_TextChanged(object sender, EventArgs e)
        {
            if (ldefinedprinters.SelectedIndices.Count != 1)
                return;
            int index = ldefinedprinters.SelectedIndices[0];
            configfile.WriteString("OpenDrawer",
                "Printer" + index.ToString(), textescapecutpaper.Text);
        }

        private void ldefinedprinters_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string defdriver;
            if (ldefinedprinters.SelectedIndices.Count != 1)
                return;
            int index = ldefinedprinters.SelectedIndices[0];
            if (index == 0)
            {

                lprinterdevice.Visible = false;
                comboprinters.SelectedIndex = 0;
                comboprinters.Visible = false;
            }
            else
            {
                lprinterdevice.Visible = true;
                comboprinters.Visible = true;
                string printername = configfile.ReadString("PrinterNames",
                    "Printer" + index.ToString(), "");
                comboprinters.SelectedIndex = comboprinters.Items.IndexOf(printername);
                if (comboprinters.SelectedIndex == -1)
                    comboprinters.SelectedIndex = 0;
            }
            checkcutpaper.Checked = configfile.ReadBool("CutPaperOn",
                "Printer" + index.ToString(), false);
            textescapecutpaper.Text = configfile.ReadString("CutPaper",
                "Printer" + index.ToString(), "");
            checkopendrawer.Checked = configfile.ReadBool("OpenDrawerOn",
                "Printer" + index.ToString(), false);
            textescapeopendrawer.Text = configfile.ReadString("OpenDrawer",
                "Printer" + index.ToString(), "#27#112#0#100#100");
            /*defdriver = " ";
            if (index == (int)PrinterSelectType.Characterprinter)
                defdriver = "EPSON";
            if (index == (int)PrinterSelectType.TicketPrinter)
                defdriver = "EPSONTMU210";
            defdriver = configfile.ReadString("PrinterDriver", "Printer" + index.ToString(), defdriver);
            if (defdriver.Length < 1)
                defdriver = " ";*/
            PrinterSelectType pselect = (PrinterSelectType)index;
            defdriver = Reportman.Drawing.PrinterConfig.GetDriverName(pselect);
            if (defdriver.Length == 0)
                defdriver = " ";
            int index2 = combotextdriver.Items.IndexOf(defdriver);
            if (index2 >= 0)
                combotextdriver.SelectedIndex = index2;
            checkoem.Checked = configfile.ReadBool("PrinterEscapeOem",
                "Printer" + index.ToString(), true);
        }

    }
    public class  EventPrinterNamesArgs
    {
        public SortedList<int,string> PrinterNames;
        public EventPrinterNamesArgs(SortedList<int, string> init_names)
        {
            PrinterNames = init_names;
        }
    }

}
