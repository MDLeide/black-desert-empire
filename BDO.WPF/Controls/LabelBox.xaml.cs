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

namespace BDO.WPF.Controls
{
    /// <summary>
    /// Interaction logic for LabelBox.xaml
    /// </summary>
    public partial class LabelBox : UserControl
    {
        public LabelBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof (string), typeof (LabelBox), new PropertyMetadata(default(string)));

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }



        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof (object), typeof (LabelBox), new PropertyMetadata(default(object)));

        public object Text
        {
            get { return (object) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }



        public static readonly DependencyProperty TextBoxWidthProperty = DependencyProperty.Register(
            "TextBoxWidth", typeof (int), typeof (LabelBox), new FrameworkPropertyMetadata(150, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public int TextBoxWidth
        {
            get { return (int) GetValue(TextBoxWidthProperty); }
            set { SetValue(TextBoxWidthProperty, value); }
        }
    }
}
