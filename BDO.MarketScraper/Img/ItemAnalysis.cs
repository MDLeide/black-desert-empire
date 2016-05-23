using System.Drawing;

namespace BDO.MarketScraper.Img
{
    public class ItemAnalysis
    {
        public Bitmap Icon { get; set; }

        public string ItemName { get; set; }
        public double ItemNameConfidence { get; set; }

        public bool IsEnhanced { get; set; }

        public int Highest { get; set; }
        public int Lowest { get; set; }
        public int MarketPrice { get; set; }
        public int LastPrice { get; set; }
        public int TotalTrades { get; set; }
        public int CurrentListings { get; set; }

        public double HighestConfidence { get; set; }
        public double LowestConfidence { get; set; }
        public double MarketPriceConfidence { get; set; }
        public double LastPriceConfidence { get; set; }
        public double TotalTradesConfidence { get; set; }
        public double CurrentListingsConfidence { get; set; }
    }
}