using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Reporting
{
    public class IdenVariableParam : IdenVariable
    {
        Param FParam;
        /// <summary>
        /// Constructor for the variable parameter
        /// </summary>
        /// <param name="eval"></param>
        public IdenVariableParam(Evaluator eval, Param nparam)
            : base(eval)
        {
            FParam = nparam;
        }
        /// <summary>
        /// The value is assigned to the internal value and to the parameter
        /// </summary>
        /// <param name="avalue"></param>
        protected override void SetValue(Variant avalue)
        {
            FParam.LastValue = avalue;
        }
        /// <summary>
        /// Gets the internal value
        /// </summary>
        /// <returns></returns>
        protected override Variant GetValue()
        {
            if (FParam.ParamType == ParamType.Multiple)
                return FParam.GetMultiValue();
            else
                return FParam.LastValue;
        }

    }


}
