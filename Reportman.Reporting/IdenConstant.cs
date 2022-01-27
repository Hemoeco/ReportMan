using Reportman.Drawing;

namespace Reportman.Reporting
{
    /// <summary>
    /// Class avaible to define constants in expression evaluator
    /// </summary>
	public class IdenConstant : EvalIdentifier
    {
        protected Variant FValue;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eval"></param>
		public IdenConstant(Evaluator eval)
            : base(eval)
        {
            FValue = new Variant();
        }
        /// <summary>
        /// Returns the value of the constant
        /// </summary>
        /// <returns></returns>
		protected override Variant GetValue()
        {
            return FValue;
        }
        /// <summary>
        /// Constants can not be assigned so a exception is lauched
        /// </summary>
        /// <param name="avalue"></param>
		protected override void SetValue(Variant avalue)
        {
            throw new UnNamedException(Translator.TranslateStr(365));
        }
    }
}
