using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Reportman.Drawing;

namespace Reportman.WPF
{
    /// <summary>
    /// Lógica de interacción para PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : UserControl
    {
        public PreviewWindow()
        {
            InitializeComponent();
        }
        public static void PreviewMetaFile(MetaFile meta)
        {
            Window dia = new Window();
            PreviewWindow preview = new PreviewWindow();
            dia.Content = preview;
            preview.previewcontrol.MetaFile = meta;
            dia.ShowDialog();
        }
        public static void PreviewDocument(FixedDocument document)
        {
            Window dia = new Window();
            Grid ngrid = new Grid();
            dia.Content = ngrid;
            DocumentViewer viewer = new DocumentViewer();
            viewer.Document = document;
            ngrid.Children.Add(viewer);
            dia.ShowDialog();
        }
    }
}
