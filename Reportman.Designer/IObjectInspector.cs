using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;

namespace Reportman.Designer
{
    internal interface IObjectInspector
    {
        ComboBox ComboSelection { get; set; }
        DockStyle Dock { get; set; }
        Control GetControl();
        void Initialize(FrameMainDesigner nFrameMain);
        void SetObject(DesignerInterface nobj);
        void SetObjectFromCombo();
        EditSubReport SubReportEdit { get; set; }
        FrameStructure Structure { get; set; }
        PropertyChanged OnPropertyChange { get; set; }
        void FinishEdit();
    }
    public delegate void PropertyChanged(string propertyName, object value);

}
