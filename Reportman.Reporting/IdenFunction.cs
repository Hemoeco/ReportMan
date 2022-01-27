using Reportman.Drawing;

namespace Reportman.Reporting
{
    /// <summary>
    /// Base class to implement evaluator functions
    /// </summary>
	public abstract class IdenFunction : EvalIdentifier
    {
        /// <summary>
        /// Function constructor
        /// </summary>
        /// <param name="eval"></param>
		protected IdenFunction(Evaluator eval)
            : base(eval)
        {

        }
        private int FParamCount;
        /// <summary>
        /// Function parameters
        /// </summary>
		public Variant[] Params;
        /// <summary>
        /// Sets the param count
        /// </summary>
        /// <param name="newparamcount"></param>
		protected void SetParamCount(int newparamcount)
        {
            if (newparamcount <= 0)
                Params = null;
            else
                Params = new Variant[newparamcount];
            FParamCount = newparamcount;
        }
        /// <summary>
        /// Gets the param count for the function, read only
        /// </summary>
		public int ParamCount
        {
            get
            {
                return FParamCount;
            }
        }
        /// <summary>
        /// Setting a value to a function is not possible, a exception is thrown
        /// </summary>
        /// <param name="avalue"></param>
		protected override void SetValue(Variant avalue)
        {
            throw new UnNamedException(Translator.TranslateStr(364));
        }
    }
}
