using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;

namespace Reportman.Drawing.Forms
{

        [ProvideProperty("CueBannerText", typeof(TextBox))]
        [ProvideProperty("CueBannerText", typeof(TextBox))]
    public class CueHelper : Component, IExtenderProvider
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            private static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);
            private const int ECM_FIRST = 0x1500;
            private const int EM_SETCUEBANNER = ECM_FIRST + 1;
            private readonly IntPtr TRUE = (IntPtr)1;

            private Hashtable cueBannerTable = new Hashtable();
            public bool CanExtend(object extendee)
            {
                if (extendee is Control)
                    return true;
                return false;
            }

            [DefaultValue("")]
            [DisplayName("CueBannerText")]
            public string GetCueBannerText(TextBox control)
            {
                if (control is TextBox)
                {
                    string cueText = (string)this.cueBannerTable[control];
                    return cueText == null ? string.Empty : cueText;
                }
                return string.Empty;
            }
            public void SetCueBannerText(TextBox control, string cueText)
            {
                if (control is TextBox)
                {
                    this.cueBannerTable[control] = cueText;
                    SendMessage(control.Handle, EM_SETCUEBANNER, TRUE, Marshal.StringToBSTR(cueText));
                }
            }
        }
}
