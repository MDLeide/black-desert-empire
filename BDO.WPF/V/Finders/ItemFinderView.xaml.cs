using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BDO.WPF.V.Finders
{
    /// <summary>
    /// Interaction logic for ItemFinderView.xaml
    /// </summary>
    public partial class ItemFinderView : UserControl
    {
        public ItemFinderView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            NameInput.Focus();
        }

        void NameInput_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (FilteredItems.Items.Count > FilteredItems.SelectedIndex + 1)
                    FilteredItems.SelectedIndex++;
            }
            else if (e.Key == Key.Up)
            {
                if (FilteredItems.Items.Count > 1)
                    if (FilteredItems.SelectedIndex > 0)
                        FilteredItems.SelectedIndex--;
            }
        }
    }
}
