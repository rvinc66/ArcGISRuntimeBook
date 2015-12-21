using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chapter4a.UserControls
{
    /// <summary>
    /// Interaction logic for CameraUserControl.xaml
    /// </summary>
    public partial class CameraUserControl : UserControl
    {
        public CameraUserControl()
        {
            InitializeComponent();
        }

        private void Slider_Heading_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
