using System.Windows;
using System.Windows.Input;

namespace BDO.WPF.V.Win
{
    /// <summary>
    /// Interaction logic for NewProcessingObservationWindow.xaml
    /// </summary>
    public partial class NewProcessingObservationWindow : Window
    {
        public NewProcessingObservationWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Topmost = true;
        }


        void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            //DragMove();
        }
    }
}
