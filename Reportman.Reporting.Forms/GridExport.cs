using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Reportman.Reporting.Forms
{
    public class GridExport
    {
        private static void SetAlignment(Reportman.Reporting.PrintItemText label,System.Windows.Forms.DataGridViewCellStyle cellstyle)
        {
            if (cellstyle != null)
            {
                switch (cellstyle.Alignment)
                {
                    case System.Windows.Forms.DataGridViewContentAlignment.MiddleRight:
                        label.Alignment = Reportman.Drawing.TextAlignType.Right;
                        label.VAlignment = Reportman.Drawing.TextAlignVerticalType.Center;
                        break;
                    case System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter:
                        label.Alignment = Reportman.Drawing.TextAlignType.Center;
                        label.VAlignment = Reportman.Drawing.TextAlignVerticalType.Center;
                        break;
                    case System.Windows.Forms.DataGridViewContentAlignment.TopCenter:
                        label.Alignment = Reportman.Drawing.TextAlignType.Center;
                        label.VAlignment = Reportman.Drawing.TextAlignVerticalType.Top;
                        break;
                    case System.Windows.Forms.DataGridViewContentAlignment.BottomCenter:
                        label.Alignment = Reportman.Drawing.TextAlignType.Center;
                        label.VAlignment = Reportman.Drawing.TextAlignVerticalType.Bottom;
                        break;
                    case System.Windows.Forms.DataGridViewContentAlignment.BottomRight:
                        label.Alignment = Reportman.Drawing.TextAlignType.Right;
                        label.VAlignment = Reportman.Drawing.TextAlignVerticalType.Bottom;
                        break;
                    case System.Windows.Forms.DataGridViewContentAlignment.TopRight:
                        label.Alignment = Reportman.Drawing.TextAlignType.Right;
                        label.VAlignment = Reportman.Drawing.TextAlignVerticalType.Top;
                        break;
                    case System.Windows.Forms.DataGridViewContentAlignment.BottomLeft:
                        label.Alignment = Reportman.Drawing.TextAlignType.Left;
                        label.VAlignment = Reportman.Drawing.TextAlignVerticalType.Bottom;
                        break;
                    case System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft:
                        label.Alignment = Reportman.Drawing.TextAlignType.Left;
                        label.VAlignment = Reportman.Drawing.TextAlignVerticalType.Center;
                        break;
                }
            }
        }
        public static void Export(System.Windows.Forms.DataGridView grid,string filename)
        {
            Reportman.Reporting.Report nreport = new Reportman.Reporting.Report();
            nreport.CreateNew();
            Reportman.Reporting.SubReport subrep = nreport.SubReports[0];
            Reportman.Reporting.Section detail = subrep.Sections[0];
            Reportman.Reporting.Section header = subrep.AddGroup("HEADER");
            header.Height = 780;
            detail.Height = 250;
            
            using (DataTable tlines = new DataTable())
            {
                Reportman.Reporting.DatabaseInfo dbinfo = new Reportman.Reporting.DatabaseInfo(nreport);
                dbinfo.Driver = Reportman.Reporting.DriverType.Mybase;
                dbinfo.Alias = "DB";
                nreport.DatabaseInfo.Add(dbinfo);

                Reportman.Reporting.DataInfo dinfo = new Reportman.Reporting.DataInfo(nreport);
                nreport.DataInfo.Add(dinfo);
                dinfo.DatabaseAlias = "DB";
                dinfo.Alias = "P1";
                dinfo.DataViewOverride = new DataView(tlines, "", "", DataViewRowState.CurrentRows);

                subrep.Alias = "P1";

                int xpos = 0;
                foreach (System.Windows.Forms.DataGridViewColumn gcol in grid.Columns)
                {
                    if (gcol.Visible)
                    {
                        tlines.Columns.Add(gcol.Index.ToString(), System.Type.GetType("System.String"));
                        string headerTitle = gcol.HeaderText;
                        Reportman.Reporting.LabelItem label = new Reportman.Reporting.LabelItem(nreport);
                        label.Height = 750;
                        label.Width = gcol.Width * 1440 / 96;
                        label.PosX = xpos;
                        label.PosY = 0;
                        label.Text = headerTitle;
                        label.WordWrap = true;
                        label.WordBreak = true;
                        SetAlignment(label, gcol.DefaultCellStyle);
                        header.Components.Add(label);

                        Reportman.Reporting.ExpressionItem expre = new Reportman.Reporting.ExpressionItem(nreport);
                        expre.Height = 250;
                        expre.Width = gcol.Width * 1440 / 96;
                        expre.PosX = xpos;
                        expre.PosY = 0;
                        expre.Expression = "P1."+gcol.Index.ToString();
                        SetAlignment(expre, gcol.DefaultCellStyle);
                        detail.Components.Add(expre);

                        xpos = xpos + label.Width+10;
                    }
                }
                foreach (System.Windows.Forms.DataGridViewRow vrow in grid.Rows)
                {
                    DataRow newRow = tlines.NewRow();
                    foreach (System.Windows.Forms.DataGridViewColumn gcol in grid.Columns)
                    {
                        if (gcol.Visible)
                        {
                            newRow[gcol.Index.ToString()] = vrow.Cells[gcol.Index].FormattedValue;
                        }
                    }
                    tlines.Rows.Add(newRow);
                }
                nreport.SaveToFile(System.IO.Path.ChangeExtension(filename, ".rep"));
                Reportman.Drawing.PrintOutPDF pdfdriver = new Reportman.Drawing.PrintOutPDF();
                pdfdriver.FileName = filename;
                pdfdriver.Print(nreport.MetaFile);
            }
        }
        
    }
}
