using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BDO.WPF.Controls
{
    /// <summary>
    /// Interaction logic for EnergyTimer.xaml
    /// </summary>
    public partial class EnergyTimer : Window
    {
        Stopwatch _stopwatch;
        System.Threading.Timer _timer;
        int _interval = 50;
        int _elapsed = 0;
        int _total = 180*1000;

        double _progress = 0;

        public EnergyTimer()
        {
            InitializeComponent();
            _timer = new System.Threading.Timer(Callback, null, 0, _interval);
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            
            Deactivated += (sender, args) => Topmost = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Dispose();
            base.OnClosed(e);
        }

        void Callback(object obj)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var elapsed = _stopwatch.ElapsedMilliseconds;
                if (elapsed >= _total)
                    elapsed = elapsed - _total;

                var progress = (double) elapsed/_total;
                var rotation = 360*progress;

                HandPath.RenderTransform = new RotateTransform(rotation);
            });
        }

        void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            _stopwatch.Restart();
        }

        void EnergyTimer_OnMouseLeave(object sender, MouseEventArgs e)
        {
            //ResetButton.Opacity = 0;
        }

        void EnergyTimer_OnMouseEnter(object sender, MouseEventArgs e)
        {
            //ResetButton.Opacity = .75;
        }

        void EnergyTimer_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
