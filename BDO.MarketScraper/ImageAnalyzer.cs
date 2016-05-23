using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BDO.MarketScraper.Img;
using Tesseract;

namespace BDO.MarketScraper
{
    public class ImageAnalyzer
    {
        string _dataPath;

        static List<Tuple<Color, Tuple<int, int>>> _enhancedItem;
        static List<Tuple<Color, Tuple<int, int>>> _unavailableEnhancedItem;
        static List<Tuple<Color, Tuple<int, int>>> _enhancedItemPlusTen;
        static List<Tuple<Color, Tuple<int, int>>> _unavailableEnhancedItemPlusTen;
        
        Color _availEnhanceColor = Color.FromArgb(231, 212, 213);
        Color _availBlurryEnhanceColor = Color.FromArgb(215, 208, 208);
        Color _unavailEnhanceColor = Color.FromArgb(213, 213, 213);
        int _threshold = 30;

        public ImageAnalyzer(string tessDataPath)
        {
            ConfigureEnhancedData();
            _dataPath = tessDataPath;
        }

        public double MinimumConfidence { get; set; } = .75;

        public int EnhancedColorThreshold { get; set; } = 250;
        
        public ItemAnalysis Analyze(ItemListing itemListing)
        {
            using (var engine = new TesseractEngine(_dataPath, "eng", EngineMode.Default))
                return Analyze(itemListing, engine);
        }

        public ItemAnalysis Analyze(ItemListing itemListing, TesseractEngine engine)
        {
            var analysis = new ItemAnalysis();

            double conf;
            
            analysis.ItemName = GetText(itemListing.Name, engine, out conf);
            analysis.ItemNameConfidence = conf;

            analysis.Highest = TryParse(GetText(itemListing.Highest, engine, out conf));
            analysis.HighestConfidence = conf;
            if (analysis.Highest == -1)
                analysis.HighestConfidence = 0;

            analysis.Lowest = TryParse(GetText(itemListing.Lowest, engine, out conf));
            analysis.LowestConfidence = conf;
            if (analysis.Lowest == -1)
                analysis.LowestConfidence = 0;

            analysis.MarketPrice = TryParse(GetText(itemListing.MarketPrice, engine, out conf));
            analysis.MarketPriceConfidence = conf;
            if (analysis.MarketPrice == -1)
                analysis.MarketPriceConfidence = 0;

            analysis.LastPrice = TryParse(GetText(itemListing.LastSalePrice, engine, out conf));
            analysis.LastPriceConfidence = conf;
            if (analysis.LastPrice == -1)
                analysis.LastPriceConfidence = 0;

            analysis.TotalTrades = TryParse(GetText(itemListing.TotalSales, engine, out conf));
            analysis.TotalTradesConfidence = conf;
            if (analysis.TotalTrades == -1)
                analysis.TotalTradesConfidence = 0;

            analysis.CurrentListings = TryParse(GetText(itemListing.CurrentListings, engine, out conf));
            analysis.CurrentListingsConfidence = conf;
            if (analysis.CurrentListings == -1)
                analysis.CurrentListingsConfidence = 0;

            analysis.Icon = itemListing.Icon.GetColorData();
            //analysis.IsEnhanced = IsEnhanced(analysis.Icon, analysis.ItemName);

            return analysis;
        }

        public List<ItemAnalysis> Analyze(IEnumerable<ItemListing> itemListings)
        {
            var analysis = new List<ItemAnalysis>();
            using (var engine = new TesseractEngine(_dataPath, "eng", EngineMode.Default))
            {
                foreach (var item in itemListings)
                {
                    analysis.Add(Analyze(item, engine));
                }
            }
            return analysis;
        }

        static void ConfigureEnhancedData()
        {
            if (_enhancedItem != null)
                return;

            _enhancedItem = new List<Tuple<Color, Tuple<int, int>>>();
            _enhancedItemPlusTen = new List<Tuple<Color, Tuple<int, int>>>();
            _unavailableEnhancedItem = new List<Tuple<Color, Tuple<int, int>>>();
            _unavailableEnhancedItemPlusTen = new List<Tuple<Color, Tuple<int, int>>>();

            var color = ColorTranslator.FromHtml("#FFE1E1E1");
            var darkColor = ColorTranslator.FromHtml("#FFAEAEAE");
            var darkColorTwo = ColorTranslator.FromHtml("#FF9F9F9F");
            
            int x = 8;
            int y = 16;
            for (int i = 0; i < 10; i++)
                _unavailableEnhancedItem.Add(new Tuple<Color, Tuple<int, int>>(color, new Tuple<int, int>(x + i, y)));

            //for (int i = 13; i < 18; i++)
            //    _unavailableEnhancedItem.Add(new Tuple<Color, Tuple<int, int>>(color, new Tuple<int, int>(x + i, y)));

            _unavailableEnhancedItem.Add(new Tuple<Color, Tuple<int, int>>(darkColor, new Tuple<int, int>(8, 15)));
            _unavailableEnhancedItem.Add(new Tuple<Color, Tuple<int, int>>(darkColor, new Tuple<int, int>(9, 15)));
            //_unavailableEnhancedItem.Add(new Tuple<Color, Tuple<int, int>>(darkColorTwo, new Tuple<int, int>(8, 17)));
            //_unavailableEnhancedItem.Add(new Tuple<Color, Tuple<int, int>>(darkColorTwo, new Tuple<int, int>(9, 17)));

            for (int i = 0; i < 10; i++)
                _unavailableEnhancedItemPlusTen.Add(new Tuple<Color, Tuple<int, int>>(color, new Tuple<int, int>(i, y)));

            _unavailableEnhancedItemPlusTen.Add(new Tuple<Color, Tuple<int, int>>(darkColor, new Tuple<int, int>(0, 15)));
            _unavailableEnhancedItemPlusTen.Add(new Tuple<Color, Tuple<int, int>>(darkColor, new Tuple<int, int>(1, 15)));
            //_unavailableEnhancedItemPlusTen.Add(new Tuple<Color, Tuple<int, int>>(darkColorTwo, new Tuple<int, int>(0, 17)));
            //_unavailableEnhancedItemPlusTen.Add(new Tuple<Color, Tuple<int, int>>(darkColorTwo, new Tuple<int, int>(1, 17)));

            color = ColorTranslator.FromHtml("#FFE7D8D9");
            darkColor = ColorTranslator.FromHtml("#FFE54244");
            darkColorTwo = ColorTranslator.FromHtml("#FFDA7A81");

            for (int i = 0; i < 10; i++)
                _enhancedItem.Add(new Tuple<Color, Tuple<int, int>>(color, new Tuple<int, int>(x + i, y)));

            _enhancedItem.Add(new Tuple<Color, Tuple<int, int>>(darkColor, new Tuple<int, int>(11, 11)));
            _enhancedItem.Add(new Tuple<Color, Tuple<int, int>>(darkColor, new Tuple<int, int>(11, 12)));
            //_enhancedItem.Add(new Tuple<Color, Tuple<int, int>>(darkColorTwo, new Tuple<int, int>(8, 17)));
            //_enhancedItem.Add(new Tuple<Color, Tuple<int, int>>(darkColorTwo, new Tuple<int, int>(9, 17)));

            for (int i = 0; i < 10; i++)
                _enhancedItemPlusTen.Add(new Tuple<Color, Tuple<int, int>>(color, new Tuple<int, int>(i, y)));


            _enhancedItemPlusTen.Add(new Tuple<Color, Tuple<int, int>>(darkColor, new Tuple<int, int>(4, 11)));
            _enhancedItemPlusTen.Add(new Tuple<Color, Tuple<int, int>>(darkColor, new Tuple<int, int>(4, 12)));
            //_enhancedItemPlusTen.Add(new Tuple<Color, Tuple<int, int>>(darkColorTwo, new Tuple<int, int>(0, 17)));
            //_enhancedItemPlusTen.Add(new Tuple<Color, Tuple<int, int>>(darkColorTwo, new Tuple<int, int>(1, 17)));

        }

        bool IsEnhanced(Bitmap bitmap, string name)
        {
            Console.WriteLine();
            Console.WriteLine("Checking Item " + name);
            var area = GetPlusArea(bitmap);
            var avg = GetColorAverage(area);

            if (WithinThreshold(avg, _availEnhanceColor, _threshold))
                return true;

            if (WithinThreshold(avg, _availBlurryEnhanceColor, _threshold))
                return true;

            if (WithinThreshold(avg, _unavailEnhanceColor, _threshold))
                return true;

            area = GetPlusTenArea(bitmap);
            avg = GetColorAverage(area);

            if (WithinThreshold(avg, _availEnhanceColor, _threshold))
                return true;

            if (WithinThreshold(avg, _unavailEnhanceColor, _threshold))
                return true;

            return false;
        }

        bool WithinThreshold(Color a, Color b, int threshold)
        {
            var rDelta = Math.Abs(a.R - b.R);
            var gDelta = Math.Abs(a.G - b.G);
            var bDelta = Math.Abs(a.B - b.B);

            Console.WriteLine($"{rDelta} {gDelta} {bDelta}");

            return (rDelta < threshold && gDelta < threshold && bDelta < threshold);
        }

        bool IsEnhanced(Bitmap bitmap)
        {
            Console.WriteLine("Enhanced");
            if (IsEnhanced(bitmap, _enhancedItem))
            {
                Console.WriteLine();
                Console.WriteLine();
                return true;
            }
            Console.WriteLine();
            Console.WriteLine("Enhanced Plus Ten");
            if (IsEnhanced(bitmap, _enhancedItemPlusTen))
            {
                Console.WriteLine();
                Console.WriteLine();
                return true;
            }
            Console.WriteLine();
            Console.WriteLine("Unavailable Enhanced");
            if (IsEnhanced(bitmap, _unavailableEnhancedItem))
            {
                Console.WriteLine();
                Console.WriteLine();
                return true;
            }
            Console.WriteLine();
            Console.WriteLine("Unavailable Enhanced Plus Ten");
            if (IsEnhanced(bitmap, _unavailableEnhancedItemPlusTen))
            {
                Console.WriteLine();
                Console.WriteLine();
                return true;
            }

            Console.WriteLine();
            Console.WriteLine();
            return false;
        }

        bool IsEnhanced(Bitmap bitmap, List<Tuple<Color, Tuple<int, int>>> colorMap)
        {
            foreach (var t in colorMap)
            {
                var targetColor = t.Item1;
                var pixelColor = bitmap.GetPixel(t.Item2.Item1, t.Item2.Item2);
                var delta = Math.Abs(targetColor.R - pixelColor.R) + Math.Abs(targetColor.G - pixelColor.G) +
                            Math.Abs(targetColor.B - pixelColor.B);

                Console.WriteLine(
                    $"{t.Item2.Item1},{t.Item2.Item2} : {delta} : {targetColor.R} {targetColor.G} {targetColor.B} - {pixelColor.R} {pixelColor.G} {pixelColor.B} ");

                if (delta > EnhancedColorThreshold)
                {
                    Console.WriteLine("Failed");
                    return false;
                }
            }
            Console.WriteLine("Passed");
            return true;
        }

        Color[] GetPlusArea(Bitmap bitmap)
        {
            // 6, 16
            // 19, 17

            // 12, 9
            // 13, 24
            var colors = new List<Color>();
            for (int x = 6; x < 19; x++)
            {
                colors.Add(bitmap.GetPixel(x, 16));
                //colors.Add(bitmap.GetPixel(x, 17));
            }

            for (int y = 9; y < 25; y++)
            {
                if (y == 16 || y == 17)
                    continue;

                colors.Add(bitmap.GetPixel(12, y));
                //colors.Add(bitmap.GetPixel(13, y));
            }
            return colors.ToArray();
        }

        Color[] GetPlusTenArea(Bitmap bitmap)
        {
            // 0, 16
            // 12, 16

            // 6, 9
            // 6, 24
            var colors = new List<Color>();
            for (int x = 0; x < 13; x++)
            {
                colors.Add(bitmap.GetPixel(x, 16));
                //colors.Add(bitmap.GetPixel(x, 17));
            }

            for (int y = 9; y < 24; y++)
            {
                if (y == 16 || y == 17)
                    continue;

                colors.Add(bitmap.GetPixel(6, y));
                //colors.Add(bitmap.GetPixel(13, y));
            }
            return colors.ToArray();
        }

        Color GetColorAverage(Color[] colors)
        {
            var count = colors.Length;
            var r = colors.Sum(p => p.R);
            var g = colors.Sum(p => p.G);
            var b = colors.Sum(p => p.B);

            return Color.FromArgb(r/count, g/count, b/count);
        }

        string GetText(ImageRegion region, TesseractEngine engine, out double confidence)
        {
            using (var page = engine.Process(region.GetColorData()))
            {
                confidence = page.GetMeanConfidence();
                if (confidence < MinimumConfidence)
                    return "-" + (int)(confidence*100);
                var text = page.GetText();
                text = text.Replace(Environment.NewLine, " ");
                text = text.Replace("\n", " ");
                return text.Trim();
            }
        }

        int TryParse(string str)
        {
            if (string.IsNullOrEmpty(str))
                return -1;
            str = str.Replace(",", "").Replace(" ", "").Trim();
            int p;
            if (!int.TryParse(str, out p))
                return -1;
            return p;
        }
    }
}