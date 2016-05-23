using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BDO.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ShadowText.xaml
    /// </summary>
    public partial class ShadowText : UserControl
    {
        public ShadowText()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof (string), typeof (ShadowText), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ShadowDepthProperty = DependencyProperty.Register(
            "ShadowDepth", typeof (double), typeof (ShadowText), new PropertyMetadata(1d));

        public double ShadowDepth
        {
            get { return (double) GetValue(ShadowDepthProperty); }
            set { SetValue(ShadowDepthProperty, value); }
        }
    }
}
