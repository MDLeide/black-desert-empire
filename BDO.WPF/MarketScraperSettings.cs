using System;

namespace BDO.WPF
{
    static class MarketScraperSettings
    {
        public static int DelayBetweenScreenshots { get; set; } = 1000;

        public static double UiScale { get; set; } = 1.7;
        public static bool Is64Bit { get; set; } = true;

        public static bool AutoAddNewItems { get; set; } = false;
        public static bool BeepOnAutoSnap { get; set; } = true;
        public static string TessData { get; set; } = @"./tessdata";

        public static bool SaveScreenShots { get; set; } = false;
        public static string ScreenShotDirectory { get; set; } = @"C:\Users\Michael\Documents\BDO\Scraper\Screenshots\";

        public static bool SaveDecomposedImages { get; set; } = false;
        public static string DecomposedImageDirectory { get; set; } =
            @"C:\Users\Michael\Documents\BDO\Scraper\Decomposed\";

        public static bool LogToFile { get; set; } = false;
        public static string LogFile { get; set; }

        public static double MinimumConfidence { get; set; } = .7;

        public static TimeSpan ObservationSaveThreshold { get; set; } = new TimeSpan(0, 30, 0);

        public static bool RequireHighest { get; set; }
        public static bool RequirestLowest { get; set; }
        public static bool RequireMarketPrice { get; set; } = true;
        public static bool RequireLastSalesPrice { get; set; }
        public static bool RequireTotalTrades { get; set; }
        public static bool RequireCurrentListings { get; set; }
    }
}