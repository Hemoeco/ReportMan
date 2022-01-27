using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Reporting
{
    /// <summary>
    /// EvalIdentifier is the base class for all objects capable of integration
    /// with Evaluator,this includes variables, functions, constants...
    /// </summary>
    public abstract class EvalIdentifier
    {
        /// <summary>
        /// Main constructor for identifiers
        /// </summary>
        /// <param name="eval"></param>
		protected EvalIdentifier(Evaluator eval)
        {
            EvalObject = eval;
        }
        /// <summary>
        /// Override the function to allow the returning of values by the identifier (functions)
        /// </summary>
        /// <returns></returns>
		protected abstract Variant GetValue();
        /// <summary>
        /// Override the function to allow the assignment of values to the identifier (variables)
        /// </summary>
        /// <param name="avalue"></param>
		protected abstract void SetValue(Variant avalue);
        /// <summary>
        /// Name of the identifier
        /// </summary>
		public string Name;
        /// <summary>
        /// Model, used usually for functions, string representing function model
        /// </summary>
		protected string FModel;
        /// <summary>
        /// Help string used by the expression builder wizard
        /// </summary>
		protected string FHelp;
        /// <summary>
        /// Evaluator that owns the ident
        /// </summary>
		public Evaluator EvalObject;
        /// <summary>
        /// Gets the model of the identifier, usually used in functions to define
        /// the name, input parameters and output
        /// </summary>
		public string Model
        {
            get
            {
                return FModel;
            }
        }
        /// <summary>
        /// Defines the purpose of the identifier, usually used by functions and constants to
        /// explain how to use the function
        /// </summary>
		public string Help
        {
            get
            {
                return FHelp;
            }
        }
        /// <summary>
        /// Gets or sets the value of the identifier, internally calls the protected methods, 
        /// GetValue and SetValue
        /// </summary>
		public Variant Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
    }

}
