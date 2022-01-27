using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace Reportman.Drawing.Forms
{
    public static class WinFormsGraphics
    {
        static object flag = 2;
        private static int intdpi;
        public static int ScreenDPI()
        {
            Monitor.Enter(flag);
            try
            {
                //#if REPMAN_COMPACT
                //		 	intdpi = PrintOutNet.DEFAULT_RESOLUTION;
                //		  	intdpiy=PrintOutNet.DEFAULT_RESOLUTION;
                //#else
                if (intdpi == 0)
                {
                    using (Control ncontrol = new Control())
                    {
                        using (Graphics gr = ncontrol.CreateGraphics())
                        {
                            intdpi = System.Convert.ToInt32(gr.DpiX);
                        }
                    }
                }
                //#endif
                return intdpi;

            }
            finally
            {
                Monitor.Exit(flag);
            }
        }
        static float fDPIScale = 0f;
        public static float DPIScale
        {
            get
            {
                if (fDPIScale == 0)
                {
                    float ndpi = ScreenDPI();
                    fDPIScale = ndpi / 96.0f;
                }
                return fDPIScale;
            }
        }
        public static bool IsWindowsFormsDPIAware()
        {
            bool dpiAware = false;
            object sec = System.Configuration.ConfigurationManager.GetSection("System.Windows.Forms.ApplicationConfigurationSection");
            if (sec is System.Collections.Specialized.NameValueCollection)
                foreach (string key in ((System.Collections.Specialized.NameValueCollection)sec).AllKeys)
                {
                    if (key == "DpiAwareness")
                        dpiAware = true;
                }
            return dpiAware;
        }

    }


}
