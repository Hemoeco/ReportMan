using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Reportman.Reporting;
using Reportman.Drawing;

namespace Reportman.Designer
{
    public partial class ExpressionDlg : UserControl
    {
        Report Report;
        Evaluator Evaluator;

        public ExpressionDlg()
        {
            InitializeComponent();


            BOK.Text = Translator.TranslateStr(93);
            BCancel.Text = Translator.TranslateStr(94);
            // LExpression.Caption:=TranslateStr(239,LExpression.Caption);
            Text = Translator.TranslateStr(240);
            //LabelCategory.Text = Translator.TranslateStr(241);
            //LOperation.Text = Translator.TranslateStr(242);
            BAdd.Text = Translator.TranslateStr(243);
            BCheckSyn.Text = Translator.TranslateStr(244);
            BShowResult.Text = Translator.TranslateStr(246);
            LCategory.Items.Clear();
            LCategory.Items.Add(Translator.TranslateStr(247));
            LCategory.Items.Add(Translator.TranslateStr(248));
            LCategory.Items.Add(Translator.TranslateStr(249));
            LCategory.Items.Add(Translator.TranslateStr(250));
            LCategory.Items.Add(Translator.TranslateStr(251));
            Dock = DockStyle.Fill;
            LModel.Text = "";
            LHelp.Text = "";
            LParams.Text = "";

            LItems.Items.Clear();
        }
        private void Label1_Click(object sender, EventArgs e)
        {

        }
        public static bool ShowDialog(ref string expression,FrameMainDesigner framemain)
        {
            using (Form newform = new Form())
            {
                newform.ShowIcon = false;
                newform.ShowInTaskbar = false;
                newform.StartPosition = FormStartPosition.CenterScreen;
                newform.Width = Convert.ToInt32(800 * Reportman.Drawing.GraphicUtils.DPIScale);
                newform.Height = Convert.ToInt32(600 * Reportman.Drawing.GraphicUtils.DPIScale);
                ExpressionDlg dia = new ExpressionDlg();
                dia.Report = framemain.Report;
                dia.MemoExpre.Text = expression;
                dia.Init();

                newform.Controls.Add(dia);
                if (newform.ShowDialog(framemain.FindForm()) == DialogResult.OK)
                {
                    expression = dia.MemoExpre.Text;
                    return true;
                }
                else
                    return false;
            }
        }

        private void BOK_Click(object sender, EventArgs e)
        {
            FindForm().DialogResult = DialogResult.OK;
        }

        private void BCancel_Click(object sender, EventArgs e)
        {
            FindForm().DialogResult = DialogResult.Cancel;
        }
        class HelpInformation
        {
            public HelpInformation(string nFunction,string nHelp,string nModel,string nParameters)
            {
                Function = nFunction;
                Help = nHelp;
                Model = nModel;
                Parameters = nParameters;
            }
            public string Help;
            public string Function;
            public string Model;
            public string Parameters;
            public override string ToString()
            {
                return Function;
            }
        }
        SortedList<int, List<HelpInformation>> HelpList = new SortedList<int, List<HelpInformation>>();
        private void FillConnectedDataSets()
        {
            List<HelpInformation> list1 = HelpList[0];
            list1.Clear();
            foreach (DatabaseInfo dbinfo in Report.DatabaseInfo)
            {
                dbinfo.Connect();
            }
            foreach (DataInfo datainfo in Report.DataInfo)
            {
                if (datainfo.Data != null)
                {
                    foreach (DataColumn ndata in datainfo.Data.Columns)
                    {
                        HelpInformation newhelp = new HelpInformation(datainfo.Alias + "." + ndata.ColumnName, "", "","");
                        list1.Add(newhelp);
                    }
                }
            }
        }
        private void Init()
        {
            Evaluator = new Evaluator();
            Evaluator.Language = Report.Language;
            Report.AddReportItemsToEvaluator(Evaluator);


            HelpList.Add(0, new List<HelpInformation>());
            HelpList.Add(1, new List<HelpInformation>());
            HelpList.Add(2, new List<HelpInformation>());
            HelpList.Add(3, new List<HelpInformation>());
            HelpList.Add(4, new List<HelpInformation>());
            FillConnectedDataSets();
            foreach (EvalIdentifier iden in Evaluator.Identifiers)
            {
                List<HelpInformation> list1 = null;
                if (iden is EvalIdenExpression)
                    list1 = HelpList[2];
                else
                    if (iden is IdenFunction)
                    list1 = HelpList[1];
                else
                        if (iden is IdenVariable)
                    list1 = HelpList[2];
                else
                    if (iden is IdenConstant)
                    list1 = HelpList[3];
                if (list1 != null)
                {
                    HelpInformation newhelp = new HelpInformation(iden.Name,iden.Help,iden.Model, "");
                    list1.Add(newhelp);
                }
            }
            List<HelpInformation> oplist = HelpList[4];
            // +
            oplist.Add(new HelpInformation("+", Translator.TranslateStr(453), "", ""));
            // -
            oplist.Add(new HelpInformation("-", Translator.TranslateStr(454), "", ""));
            // *
            oplist.Add(new HelpInformation("*", Translator.TranslateStr(455), "", ""));
            // /
            oplist.Add(new HelpInformation("/", Translator.TranslateStr(456), "", ""));
            // =
            oplist.Add(new HelpInformation("=", Translator.TranslateStr(457), "", ""));
            // ==
            oplist.Add(new HelpInformation("==", Translator.TranslateStr(457), "", ""));
            // >=
            oplist.Add(new HelpInformation(">=", Translator.TranslateStr(457), "", ""));
            // <=
            oplist.Add(new HelpInformation("<=", Translator.TranslateStr(457), "", ""));
            // >
            oplist.Add(new HelpInformation(">", Translator.TranslateStr(457), "", ""));
            // <
            oplist.Add(new HelpInformation("<", Translator.TranslateStr(457), "", ""));
            // <>
            oplist.Add(new HelpInformation("<>", Translator.TranslateStr(457), "", ""));
            // AND
            oplist.Add(new HelpInformation("AND", Translator.TranslateStr(458), "", ""));
            // OR
            oplist.Add(new HelpInformation("OR", Translator.TranslateStr(458), "", ""));
            // NOT
            oplist.Add(new HelpInformation("NOT", Translator.TranslateStr(458), "", ""));
            // ;
            oplist.Add(new HelpInformation(";", Translator.TranslateStr(462), Translator.TranslateStr(463), ""));
            // IIF
            oplist.Add(new HelpInformation("IIF", Translator.TranslateStr(462), Translator.TranslateStr(463), ""));

        }

        DatasetAlias FDataAlias = new DatasetAlias();
        private void BConectar_Click(object sender, EventArgs e)
        {
            List<HelpInformation> list1 = HelpList[0];
            foreach (DatabaseInfo dbinfo in Report.DatabaseInfo)
            {
                dbinfo.Connect();
            }
            FDataAlias.List.Clear();
            foreach (DataInfo datainfo in Report.DataInfo)
            {
                    datainfo.Connect();
                AliasCollectionItem aitem = new AliasCollectionItem();
                aitem.Alias = datainfo.Alias;
                aitem.Data = datainfo.Data;
                FDataAlias.List.Add(aitem);
            }
            FillConnectedDataSets();

            Evaluator.AliasList = FDataAlias;


            LCategory_SelectedIndexChanged(this, new EventArgs());
        }

        private void LCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LItems.Items.Clear();
            if (LCategory.SelectedIndex < 0)
                return;
            List<HelpInformation> list1 = HelpList[LCategory.SelectedIndex];
            foreach (HelpInformation ninfo in list1)
            {
                LItems.Items.Add(ninfo);
            }
            LModel.Text = "";
            LHelp.Text = "";
            LParams.Text = "";
        }

        private void LItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LItems.SelectedIndex < 0)
            {
                LModel.Text = "";
                LHelp.Text = "";
                LParams.Text = "";
            }
            HelpInformation ninfo = (HelpInformation)LItems.Items[LItems.SelectedIndex];
            LModel.Text = ninfo.Model;
            LHelp.Text = ninfo.Help;
            LParams.Text = ninfo.Parameters;
        }

        private void BAdd_Click(object sender, EventArgs e)
        {
            if (LItems.SelectedIndex >= 0)
                MemoExpre.Text = MemoExpre.Text + LItems.Items[LItems.SelectedIndex].ToString();
        }

        private void BCheckSyn_Click(object sender, EventArgs e)
        {
             Evaluator.Expression = MemoExpre.Text;
            try
            {
                Evaluator.CheckSyntax(MemoExpre.Text);
            }
            catch (Exception ex)
            {
                if (ex is EvalException)
                {
                    MemoExpre.SelectionStart = ((EvalException)ex).SourcePos;
                    MemoExpre.SelectionLength = 0;
                }
                MemoExpre.Focus();
                throw;
            }
        }

        private void BShowResult_Click(object sender, EventArgs e)
        {
            Evaluator.Expression = MemoExpre.Text;
            try
            {
                Evaluator.Evaluate();
            }
            catch (Exception ex)
            {
                if (ex is EvalException)
                {
                    MemoExpre.SelectionStart = ((EvalException)ex).SourcePos;
                    MemoExpre.SelectionLength = 0;
                }
                MemoExpre.Focus();
                throw;
            }
            MessageBox.Show(Evaluator.Result.ToString());
        }
    }
}
