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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Reportman.Drawing;
using Reportman.Drawing.Forms;
using Reportman.Reporting;

namespace Reportman.Reporting.Forms
{
	/// <summary>
	/// Page setup form, page configuration and general options for the report
	/// </summary>
	public class PageSetup : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel pbottom;
		private System.Windows.Forms.Button bcancel;
		private System.Windows.Forms.Button bok;
		private System.Windows.Forms.TabControl PControl;
		private System.Windows.Forms.TabPage tabpagesetup;
		private System.Windows.Forms.TabPage tabprintsetup;
        private System.Windows.Forms.TabPage taboptions;
        private GroupBox RPageSize;
        private RadioButton rpagecustom;
        private RadioButton rpagedefine;
        private RadioButton rpagedefault;
        private GroupBox GUserDefined;
        private Label LForceFormName;
        private Label LHeight;
        private Label LWidth;
        private GroupBox GPageSize;
        private ComboBox ComboPageSize;
        private Label LMetrics2;
        private Label LMetrics1;
        private TextBox EForceFormName;
        private NumericUpDown EPageHeight;
        private NumericUpDown EPageWidth;
        private GroupBox RPageOrientation;
        private RadioButton rorientationdefine;
        private RadioButton rorientationdefault;
        private GroupBox RPageMargins;
        private Label LMetrics4;
        private NumericUpDown MRight;
        private Label LMRight;
        private Label LMetrics3;
        private NumericUpDown MLeft;
        private Label LMLeft;
        private GroupBox RPageOrientationDefine;
        private RadioButton rlandscape;
        private RadioButton rportrait;
        private Label LMetrics5;
        private NumericUpDown MBottom;
        private Label LMBottom;
        private Label LMetrics6;
        private NumericUpDown MTop;
        private Label LMTop;
        private NumericUpDown ECopies;
        private Label LCopies;
        private CheckBox CheckDefaultCopies;
        private CheckBox CheckCollateCopies;
        private Label LBackColor;
        private Panel PanelColor;
        private ComboBox ComboStyle;
        private ComboBox ComboPreview;
        private Label LPreview;
        private ComboBox ComboLanguage;
        private Label LRLang;
        private Button BConfigurePrinters;
        private ComboBox ComboSelPrinter;
        private Label LSelectPrinter;
        private ComboBox ComboPaperSource;
        private Label LPaperSource;
        private NumericUpDown EPaperSource;
        private CheckBox CheckPreviewAbout;
        private CheckBox CheckMargins;
        private CheckBox CheckPrintOnlyIfData;
        private CheckBox CheckTwoPass;
        private NumericUpDown ELinesPerInch;
        private Label LMLinesPerInch;
        private ComboBox ComboDuplex;
        private Label LDuplex;
        private CheckBox CheckDrawerAfter;
        private CheckBox CheckDrawerBefore;
        private ComboBox ComboFormat;
        private Label LPreferedFormat;
        private ComboBox ComboPrinterFonts;
        private Label LPrinterFonts;
        private ColorDialog cdialog;
        private decimal oldcustompageheight, oldcustompagewidth;
        private decimal oldmleft, oldmright, oldmtop, oldmbottom;
		/// <summary>
		/// Required variable by the designer
		/// </summary>
		private System.ComponentModel.Container components = null;
        /// <summary>
        /// Constructor
        /// </summary>
		public PageSetup()
		{
			//
			// Necessary for Windows forms designer
			//
			InitializeComponent();

			bok.Text = Translator.TranslateStr(93);
			bcancel.Text = Translator.TranslateStr(94);
			RPageSize.Text = Translator.TranslateStr(97);
			Text = Translator.TranslateStr(110);
			tabpagesetup.Text = Translator.TranslateStr(857);
			tabprintsetup.Text = Translator.TranslateStr(858);
			taboptions.Text= Translator.TranslateStr(974);
			rpagecustom.Text = Translator.TranslateStr(732);
			rpagedefine.Text = Translator.TranslateStr(96);
			rpagedefault.Text = Translator.TranslateStr(95);
            LWidth.Text = Translator.TranslateStr(554);
            LHeight.Text = Translator.TranslateStr(555);
            LForceFormName.Text = Translator.TranslateStr(1319);
            GPageSize.Text = Translator.TranslateStr(104);
            GUserDefined.Text = Translator.TranslateStr(733);
            LCopies.Text = Translator.TranslateStr(108);
            LMLinesPerInch.Text = Translator.TranslateStr(1377);
            CheckCollateCopies.Text = Translator.TranslateStr(109);
            CheckDefaultCopies.Text = Translator.TranslateStr(1432);
            LBackColor.Text = Translator.TranslateStr(116);
            LMLeft.Text = Translator.TranslateStr(100);
            LMRight.Text = Translator.TranslateStr(101);
            LMBottom.Text = Translator.TranslateStr(103);
            LMTop.Text = Translator.TranslateStr(102);
            RPageMargins.Text = Translator.TranslateStr(99);
            LRLang.Text = Translator.TranslateStr(112);
            LPrinterFonts.Text = Translator.TranslateStr(113);
            ComboPrinterFonts.Items.Clear();
            ComboPrinterFonts.Items.Add(Translator.TranslateStr(95));
            ComboPrinterFonts.Items.Add(Translator.TranslateStr(114));
            ComboPrinterFonts.Items.Add(Translator.TranslateStr(115));
            ComboPrinterFonts.Items.Add(Translator.TranslateStr(1433));
            CheckPreviewAbout.Text = Translator.TranslateStr(1163);
            CheckMargins.Text = Translator.TranslateStr(1264);
            CheckPrintOnlyIfData.Text = Translator.TranslateStr(800);
            CheckTwoPass.Text = Translator.TranslateStr(111);
            RPageOrientationDefine.Text = Translator.TranslateStr(105);
            RPageOrientation.Text = Translator.TranslateStr(98);
            rorientationdefault.Text = Translator.TranslateStr(95);
            rorientationdefine.Text = Translator.TranslateStr(96);
            rportrait.Text = Translator.TranslateStr(106);
            rlandscape.Text = Translator.TranslateStr(107);
            LSelectPrinter.Text = Translator.TranslateStr(741);
            LPreview.Text = Translator.TranslateStr(840);
            ComboPreview.Items.Clear();
            ComboPreview.Items.Add(Translator.TranslateStr(841));
            ComboPreview.Items.Add(Translator.TranslateStr(842));
            ComboStyle.Items.Clear();
            ComboStyle.Items.Add(Translator.TranslateStr(843));
            ComboStyle.Items.Add(Translator.TranslateStr(844));
            ComboStyle.Items.Add(Translator.TranslateStr(845));
            LDuplex.Text = Translator.TranslateStr(1300);
            ComboDuplex.Items.Clear();
            ComboDuplex.Items.Add(Translator.TranslateStr(95));
            ComboDuplex.Items.Add(Translator.TranslateStr(1301));
            ComboDuplex.Items.Add(Translator.TranslateStr(1303));
            ComboDuplex.Items.Add(Translator.TranslateStr(1302));
            ComboPaperSource.Items.Clear();
            ComboPaperSource.Items.Add(Translator.TranslateStr(95));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1287));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1288));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1289));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1290));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1291));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1292));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1293));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1294));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1295));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1296));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1297));
            ComboPaperSource.Items.Add("---");
            ComboPaperSource.Items.Add("---");
            ComboPaperSource.Items.Add(Translator.TranslateStr(1298));
            ComboPaperSource.Items.Add(Translator.TranslateStr(1299));
            LPaperSource.Text = Translator.TranslateStr(1286);

            CheckDrawerBefore.Text = Translator.TranslateStr(1052);
            CheckDrawerAfter.Text = Translator.TranslateStr(1053);
            // Page sizes
            ComboPageSize.Items.Clear();
            int i;
            for (i = 0; i <= PrintOut.PageSizeArray.GetUpperBound(0); i++)
            {
                string papername = PrintOut.PageSizeName(i) + " (" +
                    (Twips.TwipsToCms(PrintOut.PageSizeArray[i, 0] * Twips.TWIPS_PER_INCH / 1000)).ToString("#,0.000") + " x " +
                    (Twips.TwipsToCms(PrintOut.PageSizeArray[i, 1] * Twips.TWIPS_PER_INCH / 1000)).ToString("#,0.000") + ") "+
                     Twips.TranslateUnit(Twips.DefaultUnit()).ToString();
                ComboPageSize.Items.Add(papername);
            }
            Translator.GetLanguageDescriptions(ComboLanguage.Items);

            LMetrics1.Text = Twips.DefaultUnitString();
            LMetrics2.Text = LMetrics1.Text;
            LMetrics3.Text = LMetrics1.Text;
            LMetrics4.Text = LMetrics1.Text;
            LMetrics5.Text = LMetrics1.Text;
            LMetrics6.Text = LMetrics1.Text;
            LPreferedFormat.Text = Translator.TranslateStr(970);

            ComboSelPrinter.Items.Clear();
            ComboSelPrinter.Items.Add(Translator.TranslateStr(467));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(468));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(469));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(470));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(471));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(472));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(473));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(474));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(475));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(476));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(477));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(478));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(479));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(480));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(481));
            ComboSelPrinter.Items.Add(Translator.TranslateStr(482));
            ComboFormat.Items.Clear();
            ComboFormat.Items.Add(Translator.TranslateStr(971));
            ComboFormat.Items.Add(Translator.TranslateStr(973));
            ComboFormat.Items.Add(Translator.TranslateStr(972));
            ComboFormat.Items.Add("XML");
            ComboFormat.Items.Add(Translator.TranslateStr(1350));
        }

		/// <summary>
		/// Code cleanup
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

		#region Windows forms designed generated code
		/// <summary>
		/// Necessary for windows forms designer
		/// </summary>
		private void InitializeComponent()
		{
            this.pbottom = new System.Windows.Forms.Panel();
            this.bcancel = new System.Windows.Forms.Button();
            this.bok = new System.Windows.Forms.Button();
            this.PControl = new System.Windows.Forms.TabControl();
            this.tabpagesetup = new System.Windows.Forms.TabPage();
            this.PanelColor = new System.Windows.Forms.Panel();
            this.LBackColor = new System.Windows.Forms.Label();
            this.CheckCollateCopies = new System.Windows.Forms.CheckBox();
            this.CheckDefaultCopies = new System.Windows.Forms.CheckBox();
            this.ECopies = new System.Windows.Forms.NumericUpDown();
            this.LCopies = new System.Windows.Forms.Label();
            this.RPageMargins = new System.Windows.Forms.GroupBox();
            this.LMetrics5 = new System.Windows.Forms.Label();
            this.MBottom = new System.Windows.Forms.NumericUpDown();
            this.LMBottom = new System.Windows.Forms.Label();
            this.LMetrics6 = new System.Windows.Forms.Label();
            this.MTop = new System.Windows.Forms.NumericUpDown();
            this.LMTop = new System.Windows.Forms.Label();
            this.LMetrics4 = new System.Windows.Forms.Label();
            this.MRight = new System.Windows.Forms.NumericUpDown();
            this.LMRight = new System.Windows.Forms.Label();
            this.LMetrics3 = new System.Windows.Forms.Label();
            this.MLeft = new System.Windows.Forms.NumericUpDown();
            this.LMLeft = new System.Windows.Forms.Label();
            this.RPageOrientationDefine = new System.Windows.Forms.GroupBox();
            this.rlandscape = new System.Windows.Forms.RadioButton();
            this.rportrait = new System.Windows.Forms.RadioButton();
            this.RPageOrientation = new System.Windows.Forms.GroupBox();
            this.rorientationdefine = new System.Windows.Forms.RadioButton();
            this.rorientationdefault = new System.Windows.Forms.RadioButton();
            this.GPageSize = new System.Windows.Forms.GroupBox();
            this.ComboPageSize = new System.Windows.Forms.ComboBox();
            this.RPageSize = new System.Windows.Forms.GroupBox();
            this.rpagecustom = new System.Windows.Forms.RadioButton();
            this.rpagedefine = new System.Windows.Forms.RadioButton();
            this.rpagedefault = new System.Windows.Forms.RadioButton();
            this.GUserDefined = new System.Windows.Forms.GroupBox();
            this.LMetrics2 = new System.Windows.Forms.Label();
            this.LMetrics1 = new System.Windows.Forms.Label();
            this.EForceFormName = new System.Windows.Forms.TextBox();
            this.EPageHeight = new System.Windows.Forms.NumericUpDown();
            this.EPageWidth = new System.Windows.Forms.NumericUpDown();
            this.LForceFormName = new System.Windows.Forms.Label();
            this.LHeight = new System.Windows.Forms.Label();
            this.LWidth = new System.Windows.Forms.Label();
            this.tabprintsetup = new System.Windows.Forms.TabPage();
            this.ComboPrinterFonts = new System.Windows.Forms.ComboBox();
            this.LPrinterFonts = new System.Windows.Forms.Label();
            this.CheckDrawerAfter = new System.Windows.Forms.CheckBox();
            this.CheckDrawerBefore = new System.Windows.Forms.CheckBox();
            this.CheckPreviewAbout = new System.Windows.Forms.CheckBox();
            this.CheckMargins = new System.Windows.Forms.CheckBox();
            this.CheckPrintOnlyIfData = new System.Windows.Forms.CheckBox();
            this.CheckTwoPass = new System.Windows.Forms.CheckBox();
            this.ELinesPerInch = new System.Windows.Forms.NumericUpDown();
            this.LMLinesPerInch = new System.Windows.Forms.Label();
            this.ComboDuplex = new System.Windows.Forms.ComboBox();
            this.LDuplex = new System.Windows.Forms.Label();
            this.EPaperSource = new System.Windows.Forms.NumericUpDown();
            this.ComboPaperSource = new System.Windows.Forms.ComboBox();
            this.LPaperSource = new System.Windows.Forms.Label();
            this.BConfigurePrinters = new System.Windows.Forms.Button();
            this.ComboSelPrinter = new System.Windows.Forms.ComboBox();
            this.LSelectPrinter = new System.Windows.Forms.Label();
            this.ComboStyle = new System.Windows.Forms.ComboBox();
            this.ComboPreview = new System.Windows.Forms.ComboBox();
            this.LPreview = new System.Windows.Forms.Label();
            this.ComboLanguage = new System.Windows.Forms.ComboBox();
            this.LRLang = new System.Windows.Forms.Label();
            this.taboptions = new System.Windows.Forms.TabPage();
            this.ComboFormat = new System.Windows.Forms.ComboBox();
            this.LPreferedFormat = new System.Windows.Forms.Label();
            this.cdialog = new System.Windows.Forms.ColorDialog();
            this.pbottom.SuspendLayout();
            this.PControl.SuspendLayout();
            this.tabpagesetup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ECopies)).BeginInit();
            this.RPageMargins.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MLeft)).BeginInit();
            this.RPageOrientationDefine.SuspendLayout();
            this.RPageOrientation.SuspendLayout();
            this.GPageSize.SuspendLayout();
            this.RPageSize.SuspendLayout();
            this.GUserDefined.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EPageHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EPageWidth)).BeginInit();
            this.tabprintsetup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELinesPerInch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EPaperSource)).BeginInit();
            this.taboptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbottom
            // 
            this.pbottom.Controls.Add(this.bcancel);
            this.pbottom.Controls.Add(this.bok);
            this.pbottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pbottom.Location = new System.Drawing.Point(0, 424);
            this.pbottom.Name = "pbottom";
            this.pbottom.Size = new System.Drawing.Size(619, 48);
            this.pbottom.TabIndex = 1;
            // 
            // bcancel
            // 
            this.bcancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bcancel.Location = new System.Drawing.Point(221, 7);
            this.bcancel.Name = "bcancel";
            this.bcancel.Size = new System.Drawing.Size(115, 33);
            this.bcancel.TabIndex = 1;
            this.bcancel.Text = "Cancel";
            // 
            // bok
            // 
            this.bok.Location = new System.Drawing.Point(8, 8);
            this.bok.Name = "bok";
            this.bok.Size = new System.Drawing.Size(112, 32);
            this.bok.TabIndex = 0;
            this.bok.Text = "OK";
            this.bok.Click += new System.EventHandler(this.bok_Click);
            // 
            // PControl
            // 
            this.PControl.Controls.Add(this.tabpagesetup);
            this.PControl.Controls.Add(this.tabprintsetup);
            this.PControl.Controls.Add(this.taboptions);
            this.PControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PControl.Location = new System.Drawing.Point(0, 0);
            this.PControl.Name = "PControl";
            this.PControl.SelectedIndex = 0;
            this.PControl.Size = new System.Drawing.Size(619, 424);
            this.PControl.TabIndex = 2;
            // 
            // tabpagesetup
            // 
            this.tabpagesetup.Controls.Add(this.PanelColor);
            this.tabpagesetup.Controls.Add(this.LBackColor);
            this.tabpagesetup.Controls.Add(this.CheckCollateCopies);
            this.tabpagesetup.Controls.Add(this.CheckDefaultCopies);
            this.tabpagesetup.Controls.Add(this.ECopies);
            this.tabpagesetup.Controls.Add(this.LCopies);
            this.tabpagesetup.Controls.Add(this.RPageMargins);
            this.tabpagesetup.Controls.Add(this.RPageOrientation);
            this.tabpagesetup.Controls.Add(this.RPageSize);
            this.tabpagesetup.Controls.Add(this.RPageOrientationDefine);
            this.tabpagesetup.Controls.Add(this.GPageSize);
            this.tabpagesetup.Controls.Add(this.GUserDefined);
            this.tabpagesetup.Location = new System.Drawing.Point(4, 22);
            this.tabpagesetup.Name = "tabpagesetup";
            this.tabpagesetup.Size = new System.Drawing.Size(611, 398);
            this.tabpagesetup.TabIndex = 0;
            this.tabpagesetup.Text = "Page setup";
            // 
            // PanelColor
            // 
            this.PanelColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelColor.Location = new System.Drawing.Point(129, 327);
            this.PanelColor.Name = "PanelColor";
            this.PanelColor.Size = new System.Drawing.Size(57, 20);
            this.PanelColor.TabIndex = 14;
            this.PanelColor.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelColor_Paint);
            this.PanelColor.Click += new System.EventHandler(this.PanelColor_Click);
            // 
            // LBackColor
            // 
            this.LBackColor.AutoSize = true;
            this.LBackColor.Location = new System.Drawing.Point(17, 330);
            this.LBackColor.Name = "LBackColor";
            this.LBackColor.Size = new System.Drawing.Size(91, 13);
            this.LBackColor.TabIndex = 13;
            this.LBackColor.Text = "Background color";
            // 
            // CheckCollateCopies
            // 
            this.CheckCollateCopies.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckCollateCopies.Location = new System.Drawing.Point(220, 299);
            this.CheckCollateCopies.Name = "CheckCollateCopies";
            this.CheckCollateCopies.Size = new System.Drawing.Size(383, 19);
            this.CheckCollateCopies.TabIndex = 8;
            this.CheckCollateCopies.Text = "Collate copies";
            // 
            // CheckDefaultCopies
            // 
            this.CheckDefaultCopies.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckDefaultCopies.Location = new System.Drawing.Point(220, 328);
            this.CheckDefaultCopies.Name = "CheckDefaultCopies";
            this.CheckDefaultCopies.Size = new System.Drawing.Size(383, 19);
            this.CheckDefaultCopies.TabIndex = 9;
            this.CheckDefaultCopies.Text = "Default printer copies";
            this.CheckDefaultCopies.CheckedChanged += new System.EventHandler(this.CheckDefaultCopies_CheckedChanged);
            // 
            // ECopies
            // 
            this.ECopies.Location = new System.Drawing.Point(129, 298);
            this.ECopies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ECopies.Name = "ECopies";
            this.ECopies.Size = new System.Drawing.Size(57, 20);
            this.ECopies.TabIndex = 7;
            this.ECopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // LCopies
            // 
            this.LCopies.AutoSize = true;
            this.LCopies.Location = new System.Drawing.Point(17, 303);
            this.LCopies.Name = "LCopies";
            this.LCopies.Size = new System.Drawing.Size(39, 13);
            this.LCopies.TabIndex = 12;
            this.LCopies.Text = "Copies";
            // 
            // RPageMargins
            // 
            this.RPageMargins.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RPageMargins.Controls.Add(this.LMetrics5);
            this.RPageMargins.Controls.Add(this.MBottom);
            this.RPageMargins.Controls.Add(this.LMBottom);
            this.RPageMargins.Controls.Add(this.LMetrics6);
            this.RPageMargins.Controls.Add(this.MTop);
            this.RPageMargins.Controls.Add(this.LMTop);
            this.RPageMargins.Controls.Add(this.LMetrics4);
            this.RPageMargins.Controls.Add(this.MRight);
            this.RPageMargins.Controls.Add(this.LMRight);
            this.RPageMargins.Controls.Add(this.LMetrics3);
            this.RPageMargins.Controls.Add(this.MLeft);
            this.RPageMargins.Controls.Add(this.LMLeft);
            this.RPageMargins.Location = new System.Drawing.Point(4, 208);
            this.RPageMargins.Name = "RPageMargins";
            this.RPageMargins.Size = new System.Drawing.Size(599, 77);
            this.RPageMargins.TabIndex = 6;
            this.RPageMargins.TabStop = false;
            this.RPageMargins.Text = "Page margins";
            // 
            // LMetrics5
            // 
            this.LMetrics5.AutoSize = true;
            this.LMetrics5.Location = new System.Drawing.Point(487, 47);
            this.LMetrics5.Name = "LMetrics5";
            this.LMetrics5.Size = new System.Drawing.Size(27, 13);
            this.LMetrics5.TabIndex = 19;
            this.LMetrics5.Text = "inch";
            // 
            // MBottom
            // 
            this.MBottom.DecimalPlaces = 3;
            this.MBottom.Location = new System.Drawing.Point(399, 45);
            this.MBottom.Name = "MBottom";
            this.MBottom.Size = new System.Drawing.Size(82, 20);
            this.MBottom.TabIndex = 18;
            // 
            // LMBottom
            // 
            this.LMBottom.AutoSize = true;
            this.LMBottom.Location = new System.Drawing.Point(262, 47);
            this.LMBottom.Name = "LMBottom";
            this.LMBottom.Size = new System.Drawing.Size(40, 13);
            this.LMBottom.TabIndex = 17;
            this.LMBottom.Text = "Bottom";
            // 
            // LMetrics6
            // 
            this.LMetrics6.AutoSize = true;
            this.LMetrics6.Location = new System.Drawing.Point(213, 47);
            this.LMetrics6.Name = "LMetrics6";
            this.LMetrics6.Size = new System.Drawing.Size(27, 13);
            this.LMetrics6.TabIndex = 16;
            this.LMetrics6.Text = "inch";
            // 
            // MTop
            // 
            this.MTop.DecimalPlaces = 3;
            this.MTop.Location = new System.Drawing.Point(125, 45);
            this.MTop.Name = "MTop";
            this.MTop.Size = new System.Drawing.Size(82, 20);
            this.MTop.TabIndex = 15;
            // 
            // LMTop
            // 
            this.LMTop.AutoSize = true;
            this.LMTop.Location = new System.Drawing.Point(13, 47);
            this.LMTop.Name = "LMTop";
            this.LMTop.Size = new System.Drawing.Size(26, 13);
            this.LMTop.TabIndex = 14;
            this.LMTop.Text = "Top";
            // 
            // LMetrics4
            // 
            this.LMetrics4.AutoSize = true;
            this.LMetrics4.Location = new System.Drawing.Point(487, 21);
            this.LMetrics4.Name = "LMetrics4";
            this.LMetrics4.Size = new System.Drawing.Size(27, 13);
            this.LMetrics4.TabIndex = 13;
            this.LMetrics4.Text = "inch";
            // 
            // MRight
            // 
            this.MRight.DecimalPlaces = 3;
            this.MRight.Location = new System.Drawing.Point(399, 19);
            this.MRight.Name = "MRight";
            this.MRight.Size = new System.Drawing.Size(82, 20);
            this.MRight.TabIndex = 12;
            // 
            // LMRight
            // 
            this.LMRight.AutoSize = true;
            this.LMRight.Location = new System.Drawing.Point(262, 21);
            this.LMRight.Name = "LMRight";
            this.LMRight.Size = new System.Drawing.Size(32, 13);
            this.LMRight.TabIndex = 11;
            this.LMRight.Text = "Right";
            // 
            // LMetrics3
            // 
            this.LMetrics3.AutoSize = true;
            this.LMetrics3.Location = new System.Drawing.Point(213, 21);
            this.LMetrics3.Name = "LMetrics3";
            this.LMetrics3.Size = new System.Drawing.Size(27, 13);
            this.LMetrics3.TabIndex = 10;
            this.LMetrics3.Text = "inch";
            // 
            // MLeft
            // 
            this.MLeft.DecimalPlaces = 3;
            this.MLeft.Location = new System.Drawing.Point(125, 19);
            this.MLeft.Name = "MLeft";
            this.MLeft.Size = new System.Drawing.Size(82, 20);
            this.MLeft.TabIndex = 9;
            // 
            // LMLeft
            // 
            this.LMLeft.AutoSize = true;
            this.LMLeft.Location = new System.Drawing.Point(13, 21);
            this.LMLeft.Name = "LMLeft";
            this.LMLeft.Size = new System.Drawing.Size(25, 13);
            this.LMLeft.TabIndex = 8;
            this.LMLeft.Text = "Left";
            // 
            // RPageOrientationDefine
            // 
            this.RPageOrientationDefine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RPageOrientationDefine.Controls.Add(this.rportrait);
            this.RPageOrientationDefine.Controls.Add(this.rlandscape);
            this.RPageOrientationDefine.Location = new System.Drawing.Point(258, 121);
            this.RPageOrientationDefine.Name = "RPageOrientationDefine";
            this.RPageOrientationDefine.Size = new System.Drawing.Size(345, 81);
            this.RPageOrientationDefine.TabIndex = 5;
            this.RPageOrientationDefine.TabStop = false;
            this.RPageOrientationDefine.Text = "Orientation definition";
            // 
            // rlandscape
            // 
            this.rlandscape.Location = new System.Drawing.Point(16, 46);
            this.rlandscape.Name = "rlandscape";
            this.rlandscape.Size = new System.Drawing.Size(208, 24);
            this.rlandscape.TabIndex = 1;
            this.rlandscape.Text = "Landscape";
            // 
            // rportrait
            // 
            this.rportrait.Location = new System.Drawing.Point(16, 16);
            this.rportrait.Name = "rportrait";
            this.rportrait.Size = new System.Drawing.Size(208, 24);
            this.rportrait.TabIndex = 1;
            this.rportrait.Text = "Portrait";
            // 
            // RPageOrientation
            // 
            this.RPageOrientation.Controls.Add(this.rorientationdefine);
            this.RPageOrientation.Controls.Add(this.rorientationdefault);
            this.RPageOrientation.Location = new System.Drawing.Point(3, 121);
            this.RPageOrientation.Name = "RPageOrientation";
            this.RPageOrientation.Size = new System.Drawing.Size(240, 81);
            this.RPageOrientation.TabIndex = 4;
            this.RPageOrientation.TabStop = false;
            this.RPageOrientation.Text = "Page orientation";
            // 
            // rorientationdefine
            // 
            this.rorientationdefine.Location = new System.Drawing.Point(16, 46);
            this.rorientationdefine.Name = "rorientationdefine";
            this.rorientationdefine.Size = new System.Drawing.Size(208, 24);
            this.rorientationdefine.TabIndex = 1;
            this.rorientationdefine.Text = "Define";
            this.rorientationdefine.CheckedChanged += new System.EventHandler(this.rorientationdefault_CheckedChanged);
            // 
            // rorientationdefault
            // 
            this.rorientationdefault.Location = new System.Drawing.Point(16, 16);
            this.rorientationdefault.Name = "rorientationdefault";
            this.rorientationdefault.Size = new System.Drawing.Size(208, 24);
            this.rorientationdefault.TabIndex = 1;
            this.rorientationdefault.Text = "Default";
            this.rorientationdefault.CheckedChanged += new System.EventHandler(this.rorientationdefault_CheckedChanged);
            // 
            // GPageSize
            // 
            this.GPageSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.GPageSize.Controls.Add(this.ComboPageSize);
            this.GPageSize.Location = new System.Drawing.Point(258, 3);
            this.GPageSize.Name = "GPageSize";
            this.GPageSize.Size = new System.Drawing.Size(345, 54);
            this.GPageSize.TabIndex = 2;
            this.GPageSize.TabStop = false;
            this.GPageSize.Text = "Custom size";
            // 
            // ComboPageSize
            // 
            this.ComboPageSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ComboPageSize.Location = new System.Drawing.Point(6, 23);
            this.ComboPageSize.Name = "ComboPageSize";
            this.ComboPageSize.Size = new System.Drawing.Size(322, 21);
            this.ComboPageSize.TabIndex = 0;
            // 
            // RPageSize
            // 
            this.RPageSize.Controls.Add(this.rpagecustom);
            this.RPageSize.Controls.Add(this.rpagedefine);
            this.RPageSize.Controls.Add(this.rpagedefault);
            this.RPageSize.Location = new System.Drawing.Point(3, 3);
            this.RPageSize.Name = "RPageSize";
            this.RPageSize.Size = new System.Drawing.Size(240, 112);
            this.RPageSize.TabIndex = 1;
            this.RPageSize.TabStop = false;
            this.RPageSize.Text = "Page size";
            // 
            // rpagecustom
            // 
            this.rpagecustom.Location = new System.Drawing.Point(16, 80);
            this.rpagecustom.Name = "rpagecustom";
            this.rpagecustom.Size = new System.Drawing.Size(208, 24);
            this.rpagecustom.TabIndex = 2;
            this.rpagecustom.Text = "Customized";
            this.rpagecustom.Click += new System.EventHandler(this.rpagedefault_Click);
            // 
            // rpagedefine
            // 
            this.rpagedefine.Location = new System.Drawing.Point(16, 46);
            this.rpagedefine.Name = "rpagedefine";
            this.rpagedefine.Size = new System.Drawing.Size(208, 24);
            this.rpagedefine.TabIndex = 1;
            this.rpagedefine.Text = "Define";
            this.rpagedefine.Click += new System.EventHandler(this.rpagedefault_Click);
            // 
            // rpagedefault
            // 
            this.rpagedefault.Location = new System.Drawing.Point(16, 16);
            this.rpagedefault.Name = "rpagedefault";
            this.rpagedefault.Size = new System.Drawing.Size(208, 24);
            this.rpagedefault.TabIndex = 1;
            this.rpagedefault.Text = "Default";
            this.rpagedefault.Click += new System.EventHandler(this.rpagedefault_Click);
            // 
            // GUserDefined
            // 
            this.GUserDefined.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.GUserDefined.Controls.Add(this.LMetrics2);
            this.GUserDefined.Controls.Add(this.LMetrics1);
            this.GUserDefined.Controls.Add(this.EForceFormName);
            this.GUserDefined.Controls.Add(this.EPageHeight);
            this.GUserDefined.Controls.Add(this.EPageWidth);
            this.GUserDefined.Controls.Add(this.LForceFormName);
            this.GUserDefined.Controls.Add(this.LHeight);
            this.GUserDefined.Controls.Add(this.LWidth);
            this.GUserDefined.Location = new System.Drawing.Point(258, 3);
            this.GUserDefined.Name = "GUserDefined";
            this.GUserDefined.Size = new System.Drawing.Size(345, 112);
            this.GUserDefined.TabIndex = 3;
            this.GUserDefined.TabStop = false;
            this.GUserDefined.Text = "Custom page size";
            // 
            // LMetrics2
            // 
            this.LMetrics2.AutoSize = true;
            this.LMetrics2.Location = new System.Drawing.Point(263, 52);
            this.LMetrics2.Name = "LMetrics2";
            this.LMetrics2.Size = new System.Drawing.Size(27, 13);
            this.LMetrics2.TabIndex = 7;
            this.LMetrics2.Text = "inch";
            // 
            // LMetrics1
            // 
            this.LMetrics1.AutoSize = true;
            this.LMetrics1.Location = new System.Drawing.Point(263, 17);
            this.LMetrics1.Name = "LMetrics1";
            this.LMetrics1.Size = new System.Drawing.Size(27, 13);
            this.LMetrics1.TabIndex = 6;
            this.LMetrics1.Text = "inch";
            // 
            // EForceFormName
            // 
            this.EForceFormName.Location = new System.Drawing.Point(145, 79);
            this.EForceFormName.Name = "EForceFormName";
            this.EForceFormName.Size = new System.Drawing.Size(112, 20);
            this.EForceFormName.TabIndex = 5;
            // 
            // EPageHeight
            // 
            this.EPageHeight.DecimalPlaces = 3;
            this.EPageHeight.Location = new System.Drawing.Point(145, 50);
            this.EPageHeight.Name = "EPageHeight";
            this.EPageHeight.Size = new System.Drawing.Size(112, 20);
            this.EPageHeight.TabIndex = 4;
            this.EPageHeight.Maximum = 9999999;
            // 
            // EPageWidth
            // 
            this.EPageWidth.DecimalPlaces = 3;
            this.EPageWidth.Location = new System.Drawing.Point(145, 15);
            this.EPageWidth.Name = "EPageWidth";
            this.EPageWidth.Size = new System.Drawing.Size(112, 20);
            this.EPageWidth.TabIndex = 3;
            this.EPageWidth.Maximum = 9999999;
            // 
            // LForceFormName
            // 
            this.LForceFormName.AutoSize = true;
            this.LForceFormName.Location = new System.Drawing.Point(8, 86);
            this.LForceFormName.Name = "LForceFormName";
            this.LForceFormName.Size = new System.Drawing.Size(59, 13);
            this.LForceFormName.TabIndex = 2;
            this.LForceFormName.Text = "Form name";
            // 
            // LHeight
            // 
            this.LHeight.AutoSize = true;
            this.LHeight.Location = new System.Drawing.Point(8, 52);
            this.LHeight.Name = "LHeight";
            this.LHeight.Size = new System.Drawing.Size(38, 13);
            this.LHeight.TabIndex = 1;
            this.LHeight.Text = "Height";
            // 
            // LWidth
            // 
            this.LWidth.AutoSize = true;
            this.LWidth.Location = new System.Drawing.Point(8, 22);
            this.LWidth.Name = "LWidth";
            this.LWidth.Size = new System.Drawing.Size(35, 13);
            this.LWidth.TabIndex = 0;
            this.LWidth.Text = "Width";
            // 
            // tabprintsetup
            // 
            this.tabprintsetup.Controls.Add(this.ComboPrinterFonts);
            this.tabprintsetup.Controls.Add(this.LPrinterFonts);
            this.tabprintsetup.Controls.Add(this.CheckDrawerAfter);
            this.tabprintsetup.Controls.Add(this.CheckDrawerBefore);
            this.tabprintsetup.Controls.Add(this.CheckPreviewAbout);
            this.tabprintsetup.Controls.Add(this.CheckMargins);
            this.tabprintsetup.Controls.Add(this.CheckPrintOnlyIfData);
            this.tabprintsetup.Controls.Add(this.CheckTwoPass);
            this.tabprintsetup.Controls.Add(this.ELinesPerInch);
            this.tabprintsetup.Controls.Add(this.LMLinesPerInch);
            this.tabprintsetup.Controls.Add(this.ComboDuplex);
            this.tabprintsetup.Controls.Add(this.LDuplex);
            this.tabprintsetup.Controls.Add(this.EPaperSource);
            this.tabprintsetup.Controls.Add(this.ComboPaperSource);
            this.tabprintsetup.Controls.Add(this.LPaperSource);
            this.tabprintsetup.Controls.Add(this.BConfigurePrinters);
            this.tabprintsetup.Controls.Add(this.ComboSelPrinter);
            this.tabprintsetup.Controls.Add(this.LSelectPrinter);
            this.tabprintsetup.Controls.Add(this.ComboStyle);
            this.tabprintsetup.Controls.Add(this.ComboPreview);
            this.tabprintsetup.Controls.Add(this.LPreview);
            this.tabprintsetup.Controls.Add(this.ComboLanguage);
            this.tabprintsetup.Controls.Add(this.LRLang);
            this.tabprintsetup.Location = new System.Drawing.Point(4, 22);
            this.tabprintsetup.Name = "tabprintsetup";
            this.tabprintsetup.Size = new System.Drawing.Size(611, 398);
            this.tabprintsetup.TabIndex = 1;
            this.tabprintsetup.Text = "Print setup";
            // 
            // ComboPrinterFonts
            // 
            this.ComboPrinterFonts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboPrinterFonts.Location = new System.Drawing.Point(241, 138);
            this.ComboPrinterFonts.Name = "ComboPrinterFonts";
            this.ComboPrinterFonts.Size = new System.Drawing.Size(271, 21);
            this.ComboPrinterFonts.TabIndex = 13;
            // 
            // LPrinterFonts
            // 
            this.LPrinterFonts.AutoSize = true;
            this.LPrinterFonts.Location = new System.Drawing.Point(8, 141);
            this.LPrinterFonts.Name = "LPrinterFonts";
            this.LPrinterFonts.Size = new System.Drawing.Size(63, 13);
            this.LPrinterFonts.TabIndex = 21;
            this.LPrinterFonts.Text = "Printer fonts";
            // 
            // CheckDrawerAfter
            // 
            this.CheckDrawerAfter.Location = new System.Drawing.Point(11, 331);
            this.CheckDrawerAfter.Name = "CheckDrawerAfter";
            this.CheckDrawerAfter.Size = new System.Drawing.Size(224, 26);
            this.CheckDrawerAfter.TabIndex = 20;
            this.CheckDrawerAfter.Text = "Open drawer after print";
            // 
            // CheckDrawerBefore
            // 
            this.CheckDrawerBefore.Location = new System.Drawing.Point(11, 306);
            this.CheckDrawerBefore.Name = "CheckDrawerBefore";
            this.CheckDrawerBefore.Size = new System.Drawing.Size(224, 25);
            this.CheckDrawerBefore.TabIndex = 19;
            this.CheckDrawerBefore.Text = "Open drawer before print";
            // 
            // CheckPreviewAbout
            // 
            this.CheckPreviewAbout.Location = new System.Drawing.Point(11, 275);
            this.CheckPreviewAbout.Name = "CheckPreviewAbout";
            this.CheckPreviewAbout.Size = new System.Drawing.Size(224, 31);
            this.CheckPreviewAbout.TabIndex = 18;
            this.CheckPreviewAbout.Text = "About box in preview";
            // 
            // CheckMargins
            // 
            this.CheckMargins.Location = new System.Drawing.Point(11, 247);
            this.CheckMargins.Name = "CheckMargins";
            this.CheckMargins.Size = new System.Drawing.Size(223, 28);
            this.CheckMargins.TabIndex = 17;
            this.CheckMargins.Text = "Printable margins in preview";
            // 
            // CheckPrintOnlyIfData
            // 
            this.CheckPrintOnlyIfData.Location = new System.Drawing.Point(11, 220);
            this.CheckPrintOnlyIfData.Name = "CheckPrintOnlyIfData";
            this.CheckPrintOnlyIfData.Size = new System.Drawing.Size(224, 27);
            this.CheckPrintOnlyIfData.TabIndex = 16;
            this.CheckPrintOnlyIfData.Text = "Print only if data available";
            // 
            // CheckTwoPass
            // 
            this.CheckTwoPass.Location = new System.Drawing.Point(11, 193);
            this.CheckTwoPass.Name = "CheckTwoPass";
            this.CheckTwoPass.Size = new System.Drawing.Size(223, 27);
            this.CheckTwoPass.TabIndex = 15;
            this.CheckTwoPass.Text = "Two pass report";
            // 
            // ELinesPerInch
            // 
            this.ELinesPerInch.Location = new System.Drawing.Point(241, 165);
            this.ELinesPerInch.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ELinesPerInch.Name = "ELinesPerInch";
            this.ELinesPerInch.Size = new System.Drawing.Size(58, 20);
            this.ELinesPerInch.TabIndex = 14;
            this.ELinesPerInch.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // LMLinesPerInch
            // 
            this.LMLinesPerInch.AutoSize = true;
            this.LMLinesPerInch.Location = new System.Drawing.Point(8, 172);
            this.LMLinesPerInch.Name = "LMLinesPerInch";
            this.LMLinesPerInch.Size = new System.Drawing.Size(73, 13);
            this.LMLinesPerInch.TabIndex = 13;
            this.LMLinesPerInch.Text = "Lines per inch";
            // 
            // ComboDuplex
            // 
            this.ComboDuplex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboDuplex.Location = new System.Drawing.Point(241, 113);
            this.ComboDuplex.Name = "ComboDuplex";
            this.ComboDuplex.Size = new System.Drawing.Size(271, 21);
            this.ComboDuplex.TabIndex = 12;
            // 
            // LDuplex
            // 
            this.LDuplex.AutoSize = true;
            this.LDuplex.Location = new System.Drawing.Point(8, 116);
            this.LDuplex.Name = "LDuplex";
            this.LDuplex.Size = new System.Drawing.Size(74, 13);
            this.LDuplex.TabIndex = 11;
            this.LDuplex.Text = "Duplex Option";
            // 
            // EPaperSource
            // 
            this.EPaperSource.Location = new System.Drawing.Point(241, 88);
            this.EPaperSource.Name = "EPaperSource";
            this.EPaperSource.Size = new System.Drawing.Size(57, 20);
            this.EPaperSource.TabIndex = 9;
            this.EPaperSource.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.EPaperSource.ValueChanged += new System.EventHandler(this.EPaperSource_ValueChanged);
            // 
            // ComboPaperSource
            // 
            this.ComboPaperSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboPaperSource.Location = new System.Drawing.Point(304, 88);
            this.ComboPaperSource.Name = "ComboPaperSource";
            this.ComboPaperSource.Size = new System.Drawing.Size(208, 21);
            this.ComboPaperSource.TabIndex = 10;
            this.ComboPaperSource.SelectedIndexChanged += new System.EventHandler(this.ComboPaperSource_SelectedIndexChanged);
            // 
            // LPaperSource
            // 
            this.LPaperSource.AutoSize = true;
            this.LPaperSource.Location = new System.Drawing.Point(8, 91);
            this.LPaperSource.Name = "LPaperSource";
            this.LPaperSource.Size = new System.Drawing.Size(102, 13);
            this.LPaperSource.TabIndex = 8;
            this.LPaperSource.Text = "Select paper source";
            // 
            // BConfigurePrinters
            // 
            this.BConfigurePrinters.Location = new System.Drawing.Point(518, 61);
            this.BConfigurePrinters.Name = "BConfigurePrinters";
            this.BConfigurePrinters.Size = new System.Drawing.Size(41, 23);
            this.BConfigurePrinters.TabIndex = 7;
            this.BConfigurePrinters.Text = "...";
            this.BConfigurePrinters.Click += new System.EventHandler(this.BConfigurePrinters_Click);
            // 
            // ComboSelPrinter
            // 
            this.ComboSelPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboSelPrinter.Location = new System.Drawing.Point(241, 61);
            this.ComboSelPrinter.Name = "ComboSelPrinter";
            this.ComboSelPrinter.Size = new System.Drawing.Size(271, 21);
            this.ComboSelPrinter.TabIndex = 6;
            // 
            // LSelectPrinter
            // 
            this.LSelectPrinter.AutoSize = true;
            this.LSelectPrinter.Location = new System.Drawing.Point(8, 64);
            this.LSelectPrinter.Name = "LSelectPrinter";
            this.LSelectPrinter.Size = new System.Drawing.Size(69, 13);
            this.LSelectPrinter.TabIndex = 5;
            this.LSelectPrinter.Text = "Select printer";
            // 
            // ComboStyle
            // 
            this.ComboStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboStyle.Location = new System.Drawing.Point(377, 35);
            this.ComboStyle.Name = "ComboStyle";
            this.ComboStyle.Size = new System.Drawing.Size(135, 21);
            this.ComboStyle.TabIndex = 4;
            // 
            // ComboPreview
            // 
            this.ComboPreview.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboPreview.Location = new System.Drawing.Point(241, 35);
            this.ComboPreview.Name = "ComboPreview";
            this.ComboPreview.Size = new System.Drawing.Size(135, 21);
            this.ComboPreview.TabIndex = 3;
            // 
            // LPreview
            // 
            this.LPreview.AutoSize = true;
            this.LPreview.Location = new System.Drawing.Point(8, 38);
            this.LPreview.Name = "LPreview";
            this.LPreview.Size = new System.Drawing.Size(133, 13);
            this.LPreview.TabIndex = 2;
            this.LPreview.Text = "Preview window and scale";
            // 
            // ComboLanguage
            // 
            this.ComboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboLanguage.Location = new System.Drawing.Point(241, 9);
            this.ComboLanguage.Name = "ComboLanguage";
            this.ComboLanguage.Size = new System.Drawing.Size(271, 21);
            this.ComboLanguage.TabIndex = 1;
            // 
            // LRLang
            // 
            this.LRLang.AutoSize = true;
            this.LRLang.Location = new System.Drawing.Point(8, 12);
            this.LRLang.Name = "LRLang";
            this.LRLang.Size = new System.Drawing.Size(86, 13);
            this.LRLang.TabIndex = 0;
            this.LRLang.Text = "Report language";
            // 
            // taboptions
            // 
            this.taboptions.Controls.Add(this.ComboFormat);
            this.taboptions.Controls.Add(this.LPreferedFormat);
            this.taboptions.Location = new System.Drawing.Point(4, 22);
            this.taboptions.Name = "taboptions";
            this.taboptions.Size = new System.Drawing.Size(611, 398);
            this.taboptions.TabIndex = 2;
            this.taboptions.Text = "Options";
            // 
            // ComboFormat
            // 
            this.ComboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboFormat.Location = new System.Drawing.Point(241, 8);
            this.ComboFormat.Name = "ComboFormat";
            this.ComboFormat.Size = new System.Drawing.Size(271, 21);
            this.ComboFormat.TabIndex = 3;
            // 
            // LPreferedFormat
            // 
            this.LPreferedFormat.AutoSize = true;
            this.LPreferedFormat.Location = new System.Drawing.Point(8, 11);
            this.LPreferedFormat.Name = "LPreferedFormat";
            this.LPreferedFormat.Size = new System.Drawing.Size(105, 13);
            this.LPreferedFormat.TabIndex = 2;
            this.LPreferedFormat.Text = "Prefered save format";
            // 
            // PageSetup
            // 
            this.AcceptButton = this.bok;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(619, 472);
            this.Controls.Add(this.PControl);
            this.Controls.Add(this.pbottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PageSetup";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.pbottom.ResumeLayout(false);
            this.PControl.ResumeLayout(false);
            this.tabpagesetup.ResumeLayout(false);
            this.tabpagesetup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ECopies)).EndInit();
            this.RPageMargins.ResumeLayout(false);
            this.RPageMargins.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MLeft)).EndInit();
            this.RPageOrientationDefine.ResumeLayout(false);
            this.RPageOrientation.ResumeLayout(false);
            this.GPageSize.ResumeLayout(false);
            this.RPageSize.ResumeLayout(false);
            this.GUserDefined.ResumeLayout(false);
            this.GUserDefined.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EPageHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EPageWidth)).EndInit();
            this.tabprintsetup.ResumeLayout(false);
            this.tabprintsetup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ELinesPerInch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EPaperSource)).EndInit();
            this.taboptions.ResumeLayout(false);
            this.taboptions.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
		private bool dook;
		private Report FReport;
		/// <summary>
		/// Show page setup dialog for the report
		/// </summary>
		/// <param name="rp">Report to modify</param>
		/// <param name="showadvanced">Set this parameter to true to show advanced tab, usually only useful 
		/// while designing the report</param>
		/// <returns>Returns true if the user modified the report</returns>
		public static bool ShowPageSetup(Report rp,bool showadvanced,IWin32Window parent = null)
		{
			using (PageSetup dia=new PageSetup())
			{
				dia.SetReport(rp);
				dia.ShowDialog(parent);
				return (dia.dook);
			}
		}
		private void bok_Click(object sender, System.EventArgs e)
		{
            short linch=(short)Math.Round(ELinesPerInch.Value*100);
            if (linch<100)
                linch=100;
            if (linch>3000)
                linch=3000;
            Report rp = FReport;
            rp.LinesPerInch = linch;

            int acopies;
            if (CheckDefaultCopies.Checked)
                acopies=0;
            else
                acopies=(int)ECopies.Value;
            if (acopies<0)
                acopies=1;
            rp.Copies=acopies;
            rp.CollateCopies = CheckCollateCopies.Checked;
            rp.TwoPass = CheckTwoPass.Checked;
            rp.PreviewAbout = CheckPreviewAbout.Checked;
            rp.PrintOnlyIfDataAvailable = CheckPrintOnlyIfData.Checked;
            rp.ActionBefore = CheckDrawerBefore.Checked;
            rp.ActionAfter = CheckDrawerAfter.Checked;
            rp.PageSize = PageSizeType.Default;
            if (rpagedefine.Checked)
                rp.PageSize = PageSizeType.Custom;
            else
                if (rpagecustom.Checked)
                    rp.PageSize = PageSizeType.User;
            rp.PageSizeIndex = ComboPageSize.SelectedIndex;
            if (rp.PageSizeIndex<0)
                rp.PageSizeIndex=0;
            rp.PageWidth=PrintOut.PageSizeArray[rp.PageSizeIndex, 0] * Twips.TWIPS_PER_INCH / 1000;
            rp.PageHeight = PrintOut.PageSizeArray[rp.PageSizeIndex, 0] * Twips.TWIPS_PER_INCH / 1000;
            if (EPageWidth.Value != oldcustompagewidth)
                rp.CustomPageWidth = Twips.TwipsFromUnits(EPageWidth.Value);
            if (EPageHeight.Value != oldcustompageheight)
                rp.CustomPageHeight = Twips.TwipsFromUnits(EPageHeight.Value);
            if (MLeft.Value!=oldmleft)
                rp.LeftMargin= Twips.TwipsFromUnits(MLeft.Value);
            if (MTop.Value != oldmtop)
                rp.TopMargin = Twips.TwipsFromUnits(MTop.Value);
            if (MRight.Value != oldmright)
                rp.RightMargin = Twips.TwipsFromUnits(MRight.Value);
            if (MBottom.Value != oldmbottom)
                rp.BottomMargin = Twips.TwipsFromUnits(MBottom.Value);

            rp.PageOrientation = OrientationType.Default;
            if (rorientationdefine.Checked)
            {
                if (rportrait.Checked)
                    rp.PageOrientation = OrientationType.Portrait;
                else
                    rp.PageOrientation = OrientationType.Landscape;
            }
            rp.PrinterSelect = (PrinterSelectType)ComboSelPrinter.SelectedIndex;
            rp.PageBackColor = GraphicUtils.IntegerFromColor(PanelColor.BackColor);
            rp.Language = ComboLanguage.SelectedIndex - 1;
            rp.PrinterFonts = (PrinterFontsType)ComboPrinterFonts.SelectedIndex;
            rp.PreviewMargins = CheckMargins.Checked;
            rp.PreviewWindow = (PreviewWindowStyleType)ComboPreview.SelectedIndex;
            rp.AutoScale = (AutoScaleType)ComboStyle.SelectedIndex;
            rp.StreamFormat = (StreamFormatType)ComboFormat.SelectedIndex;
            rp.PaperSource = (int)EPaperSource.Value;
            rp.Duplex = ComboDuplex.SelectedIndex;
            rp.ForcePaperName = EForceFormName.Text;

			dook=true;
            Close();
		}
		private void SetReport (Report rp)
		{
			FReport=rp;
            ELinesPerInch.Value = rp.LinesPerInch / 100;
            if (rp.Copies == 0)
            {
                CheckDefaultCopies.Checked = true;
                ECopies.Value = 1;
                CheckDefaultCopies_CheckedChanged(this, new EventArgs());
            }
            else
                ECopies.Value = rp.Copies;
			switch (FReport.PageSize)
			{
				case PageSizeType.Custom:
                    this.rpagedefine.Checked = true;
					break;
				case PageSizeType.Default:
					this.rpagedefault.Checked=true;
					break;
				case PageSizeType.User:
                    this.rpagecustom.Checked = true;
                    break;
			}
            rpagedefault_Click(this, new EventArgs());
            EPageWidth.Value = Twips.UnitsFromTwips(FReport.CustomPageWidth);
            EPageHeight.Value = Twips.UnitsFromTwips(FReport.CustomPageHeight);
            EForceFormName.Text = FReport.ForcePaperName;
            CheckCollateCopies.Checked = rp.CollateCopies;
            CheckTwoPass.Checked = rp.TwoPass;
            CheckPreviewAbout.Checked = rp.PreviewAbout;
            CheckMargins.Checked = rp.PreviewMargins;
            CheckPrintOnlyIfData.Checked = rp.PrintOnlyIfDataAvailable;
            MLeft.Value = Twips.UnitsFromTwips(rp.LeftMargin);
            MTop.Value = Twips.UnitsFromTwips(rp.TopMargin);
            MRight.Value = Twips.UnitsFromTwips(rp.RightMargin);
            MBottom.Value = Twips.UnitsFromTwips(rp.BottomMargin);
            oldcustompageheight = EPageHeight.Value;
            oldcustompagewidth = EPageWidth.Value;
            oldmbottom = MBottom.Value;
            oldmright = MRight.Value;
            oldmtop = MTop.Value;
            oldmleft = MLeft.Value;
            ComboPageSize.SelectedIndex = rp.PageSizeIndex;
            if (rp.PageOrientation > OrientationType.Default)
            {
                rorientationdefine.Checked=true;
                if (rp.PageOrientation == OrientationType.Landscape)
                    rlandscape.Checked = true;
                else
                    rportrait.Checked = true;
            }
            else
            {
                rorientationdefault.Checked=true;
            }
            rorientationdefault_CheckedChanged(this, new EventArgs());
            ComboSelPrinter.SelectedIndex = (int)rp.PrinterSelect;
            PanelColor.BackColor = GraphicUtils.ColorFromInteger(rp.PageBackColor);
            ComboPrinterFonts.SelectedIndex = (int)rp.PrinterFonts;
            ComboLanguage.SelectedIndex = 0;
            if ((rp.Language + 1) < ComboLanguage.Items.Count)
                ComboLanguage.SelectedIndex = rp.Language + 1;
            ComboPreview.SelectedIndex=(int)rp.PreviewWindow;
            ComboStyle.SelectedIndex = (int)rp.AutoScale;
            ComboFormat.SelectedIndex = (int)rp.StreamFormat;
            EPaperSource.Value = rp.PaperSource;
            ComboDuplex.SelectedIndex = rp.Duplex;
        }

        private void rpagedefault_Click(object sender, EventArgs e)
        {
             GPageSize.Visible=rpagedefine.Checked;
             GUserDefined.Visible=rpagecustom.Checked;
        }

        private void CheckDefaultCopies_CheckedChanged(object sender, EventArgs e)
        {
            ECopies.Enabled = !CheckDefaultCopies.Checked;
        }

        private void rorientationdefault_CheckedChanged(object sender, EventArgs e)
        {
            RPageOrientationDefine.Visible = rorientationdefault.Checked;
        }

        private void EPaperSource_ValueChanged(object sender, EventArgs e)
        {
            ComboPaperSource.SelectedIndex = -1;
            if (EPaperSource.Value < ComboPaperSource.Items.Count)
                ComboPaperSource.SelectedIndex = System.Convert.ToInt32(EPaperSource.Value);
        }

        private void ComboPaperSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboPaperSource.SelectedIndex < 0)
                return;
            if (ComboPaperSource.SelectedIndex != EPaperSource.Value)
                EPaperSource.Value = ComboPaperSource.SelectedIndex;
        }

        private void PanelColor_Paint(object sender, PaintEventArgs e)
        {
        }

        private void PanelColor_Click(object sender, EventArgs e)
        {
            cdialog.Color = PanelColor.BackColor;
            if (cdialog.ShowDialog() == DialogResult.OK)
            {
                PanelColor.BackColor = cdialog.Color;
            }
        }

        private void BConfigurePrinters_Click(object sender, EventArgs e)
        {
            // Configure printers
            PrintersConfiguration.ShowPrintersConfiguration(this);
        }
	}
}
