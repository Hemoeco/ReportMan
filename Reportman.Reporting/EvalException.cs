using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Reporting
{
    /// <summary>
    /// Definition of a specialiced exception used by expression evaluator
    /// </summary>
    public class EvalException : System.Exception
    {
        private int FSourceLine, FSourcePos;
        /// <summary>
        /// The element name that caused the exception
        /// </summary>
		public string ElementName;
        /// <summary>
        /// Source line number in the original expression (full expression)
        /// </summary>
		public int SourceLine
        {
            get
            {
                return FSourceLine;
            }
        }
        /// <summary>
        /// Position where the exception have been thrown, in the original expression
        /// </summary>
		public int SourcePos
        {
            get
            {
                return FSourcePos;
            }
        }
        /// <summary>
        /// Constructor, this will provide additional debug information for the designer
        /// to find the source of the problem inside a expression
        /// </summary>
        /// <param name="amessage"></param>
        /// <param name="asourceline"></param>
        /// <param name="asourcepos"></param>
        /// <param name="aelementname"></param>
		public EvalException(string amessage, int asourceline, int asourcepos, string aelementname)
            : base(amessage)
        {
            FSourceLine = asourceline;
            ElementName = aelementname;
            FSourcePos = asourcepos;
        }
    }
}
