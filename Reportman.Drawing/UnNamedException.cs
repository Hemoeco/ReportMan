using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Drawing
{
    /// <summary>
    /// Provide utitilies for handling streams, write strings, read/write
    /// datatypes etc.
    /// </summary>
    /// <summary>
    /// Common exception, providing a message for information
    /// </summary>
    public class UnNamedException : System.Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UnNamedException(string amessage)
            : base(amessage)
        {
        }
    }

}
