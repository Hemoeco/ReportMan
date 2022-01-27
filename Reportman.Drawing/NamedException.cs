using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Drawing
{
    /// <summary>
    /// Common exception, providing a name as additional exception information
    /// </summary>
    public class NamedException : System.Exception
    {
        private string FName;
        /// <summary>
        /// Provide aditional information, usually the name of the component throwing the exception
        /// </summary>
        public string Name
        {
            get { return FName; }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public NamedException(string amessage, string name)
            : base(amessage)
        {
            FName = name;
        }
    }

}
