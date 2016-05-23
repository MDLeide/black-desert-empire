using System.Windows;

namespace BDO.WPF.V.Win
{
    /// <summary>
    /// Interaction logic for NewObservationWindow.xaml
    /// </summary>
    public partial class NewMarketObservationWindow : Window
    {
        public NewMarketObservationWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Topmost = true;
        }
    }
}
