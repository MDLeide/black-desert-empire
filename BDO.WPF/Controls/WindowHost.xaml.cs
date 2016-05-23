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
    /// Interaction logic for WindowHost.xaml
    /// </summary>
    public partial class WindowHost : UserControl
    {
        public WindowHost()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }
        
        void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var window = Window.GetWindow(this);
            if (window == null)
                return;

            window.Deactivated += (s, e) =>
            {
                //window.Topmost = false;
                //if (KeepOnTop && StaticSettings.AllowKeepOnTop)
                window.Topmost = true;
            };
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof (string), typeof (WindowHost), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty KeepOnTopProperty = DependencyProperty.Register(
            "KeepOnTop", typeof (bool), typeof (WindowHost), new PropertyMetadata(StaticSettings.KeepOnTopDefault));

        public bool KeepOnTop
        {
            get { return (bool) GetValue(KeepOnTopProperty); }
            set { SetValue(KeepOnTopProperty, value); }
        }


        void Close_OnClick(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            parent?.Close();
        }

        void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var parent = Window.GetWindow(this);
            parent?.DragMove();
        }

        void Minimize_OnClick(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            if (parent != null)
                parent.WindowState = WindowState.Minimized;
        }

        double _lastHeight = 17;

        void Hide_OnClick(object sender, RoutedEventArgs e)
        {
            if (Height == 17)
                Height = _lastHeight;
            else
            {
                _lastHeight = Height;
                Height = 17;
            }
        }

        void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            KeepOnTop = true;
        }

        void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            KeepOnTop = false;
        }
    }
}
