using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using BDO.MarketScraper.Img;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM
{
    public class ItemAnalysisViewModel : ViewModelBase
    {
        public ItemAnalysisViewModel(ItemAnalysis analysis)
        {
            Analysis = analysis;
            IconSource = LoadBitmap(analysis.Icon);
        }

        BitmapSource _iconSource;

        public BitmapSource IconSource
        {
            get { return _iconSource; }
            set
            {
                if (Equals(value, _iconSource)) return;
                _iconSource = value;
                OnPropertyChanged(nameof(IconSource));
            }
        }

        ItemAnalysis _analysis;

        public ItemAnalysis Analysis
        {
            get { return _analysis; }
            set
            {
                if (Equals(value, _analysis)) return;
                _analysis = value;
                OnPropertyChanged(nameof(Analysis));
            }
        }

        bool _keep;

        public bool Keep
        {
            get { return _keep; }
            set
            {
                if (Equals(value, _keep)) return;
                _keep = value;
                OnPropertyChanged(nameof(Keep));
            }
        }

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        static BitmapSource LoadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                    IntPtr.Zero, Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }
            
            return bs;
        }
    }
}