using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Reporting
{
    /// <summary>
    /// Identifier with the functionality of a variable
    /// </summary>
	public class IdenVariable : EvalIdentifier
    {
        /// <summary>
        /// Constructor for the variable
        /// </summary>
        /// <param name="eval"></param>
		public IdenVariable(Evaluator eval)
            : base(eval)
        {
            FValue = new Variant();
        }
        /// <summary>
        /// Internal value
        /// </summary>
		protected Variant FValue;
        /// <summary>
        /// The value is assigned to the internal value
        /// </summary>
        /// <param name="avalue"></param>
		protected override void SetValue(Variant avalue)
        {
            FValue = avalue;
        }
        /// <summary>
        /// Gets the internal value
        /// </summary>
        /// <returns></returns>
		protected override Variant GetValue()
        {
            return FValue;
        }

    }

}
