using Reportman.Drawing;
using System.Data;

namespace Reportman.Reporting
{
    /// <summary>
    /// This identifier have a datarow column linked so, it will return the
    /// value of a field
    /// </summary>
	class IdenField : EvalIdentifier
    {
        /// <summary>
        /// Constructor
        /// </summary>
		public IdenField() : base(null) { }
        private string FField;
        private DataTable FData;
        /// <summary>
        /// Link to a row index in the datatable
        /// </summary>
		public int CurrentRow;
        /// <summary>
        /// Column name in the data row
        /// </summary>
		public string Field
        {
            get
            {
                return (FField);
            }
            set
            {
                FField = value;
            }
        }
        /// <summary>
        /// Link to the DataTable
        /// </summary>
		public DataTable Data
        {
            get
            {
                return (FData);
            }
            set
            {
                FData = value;
            }
        }
        /// <summary>
        /// Setting a value to a field is not possible, a exception is thrown
        /// </summary>
        /// <param name="avalue"></param>
		protected override void SetValue(Variant avalue)
        {
            throw new UnNamedException(Translator.TranslateStr(365));
        }
        /// <summary>
        /// Gets the value of the field
        /// </summary>
        /// <returns></returns>
		protected override Variant GetValue()
        {
            try
            {
                Variant FValue = new Variant();
                if (FData != null)
                    if (FField != null)
                    {
                        if (FData is ReportDataset)
                        {
                            if (((ReportDataset)FData).CurrentRowCount > 0)
                            {
                                FValue.AssignFromObject(((ReportDataset)FData).CurrentRow[FField]);
                            }
                        }
                        else
                        {
                            FValue.AssignFromObject(FData.Rows[CurrentRow][FField]);
                        }
                    }
                return (FValue);
            }
            catch
            {
                throw;
            }
        }

    }
}
