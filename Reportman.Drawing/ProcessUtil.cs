using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Drawing
{
    /// <summary>
    /// Process utilities
    /// </summary>
    public class ProcessUtil
    {
        /// <summary>
        /// Returns true if the process is running in 32bits mode
        /// </summary>
        public static bool Is64BitProcess
        {
            get { return IntPtr.Size == 8; }
        }

    }
}
