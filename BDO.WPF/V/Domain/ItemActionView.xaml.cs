using System.Windows.Controls;
using System.Windows.Input;
using BDO.WPF.VM.Domain;

namespace BDO.WPF.V.Domain
{
    /// <summary>
    /// Interaction logic for ItemActionView.xaml
    /// </summary>
    public partial class ItemActionView : UserControl
    {
        public ItemActionView()
        {
            InitializeComponent();
        }

        void InnerGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = DataContext as ItemViewModel;
            if (item?.PrimarySourceRecipe == null)
                return;
            Popup.IsOpen = !Popup.IsOpen;
        }
    }
}
