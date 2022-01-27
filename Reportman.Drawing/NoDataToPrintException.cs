
namespace Reportman.Drawing
{
    /// <summary>
    /// Custom exception to be controlled,when the report is empty
    /// </summary>
    public class NoDataToPrintException : System.Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NoDataToPrintException(string amessage)
            : base(amessage)
        {
        }
    }

}
