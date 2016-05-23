using System.Windows;
using System.Windows.Controls;
using BDO.WPF.VM.Domain;

namespace BDO.WPF.V.Domain
{
    /// <summary>
    /// Interaction logic for NewMarketObservationView.xaml
    /// </summary>
    public partial class NewMarketObservationView : UserControl
    {
        public NewMarketObservationView()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                LowText.Focus();
                LowText.SelectAll();
                var vm = DataContext as MarketObservationViewModel;
                if (vm != null)
                    vm.ObjectSaved += (s, a) =>
                    {
                        Window.GetWindow(this)?.Close();
                    };
            };
        }

        void UIElement_OnGotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }
    }
}
