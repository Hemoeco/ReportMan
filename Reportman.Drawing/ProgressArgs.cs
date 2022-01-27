using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Drawing
{
    public class ProgressArgs
    {
        private long fcount;
        private long ftotal;
        public bool Cancel;
        public long Count
        {
            get
            {
                return fcount;
            }
        }
        public long Total
        {
            get
            {
                return ftotal;
            }
        }
        public ProgressArgs(long ncount, long ntotal)
        {
            fcount = ncount;
            ftotal = ntotal;
            Cancel = true;
        }
    }
    public delegate void ProgressEvent(object sender, ProgressArgs args);

}
