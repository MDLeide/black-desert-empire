using System;
using System.Windows;
using System.Windows.Input;
using BDO.WPF.VM.Finders;

namespace BDO.WPF.V.Win
{
    /// <summary>
    /// Interaction logic for ItemFinderWindow.xaml
    /// </summary>
    public partial class ItemFinderWindow : Window
    {
        public ItemFinderWindow()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                var vm = DataContext as ItemFinderViewModel;
                if (vm != null)
                    vm.ItemSelected += (s, e) => Close();
            };
        }

        void ItemFinderWindow_OnClosed(object sender, EventArgs e)
        {
            (DataContext as ItemFinderViewModel)?.Cancel.Execute(this);
        }

        void Finder_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var vm = DataContext as ItemFinderViewModel;
                if (vm == null)
                    return;

                if (vm.Select.CanExecute(this))
                    vm.Select.Execute(this);
            }
        }
    }
}
