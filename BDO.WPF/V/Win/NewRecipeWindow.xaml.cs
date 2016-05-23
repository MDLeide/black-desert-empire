using System.Windows;
using BDO.WPF.VM.Domain;

namespace BDO.WPF.V.Win
{
    /// <summary>
    /// Interaction logic for NewRecipeWindow.xaml
    /// </summary>
    public partial class NewRecipeWindow : Window
    {
        public NewRecipeWindow()
        {
            InitializeComponent();
            Loaded += (se, ev) =>
            {
                var vm = DataContext as RecipeViewModel;
                if (vm != null)
                    vm.ObjectSaved += (s, e) => Close();
            };
        }

        void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
