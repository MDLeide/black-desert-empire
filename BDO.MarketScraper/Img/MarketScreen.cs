using System.Collections.Generic;

namespace BDO.MarketScraper.Img
{
    public class MarketScreen
    {
        public ImageRegion FullMarketScreen { get; set; }
        public ImageRegion MarketCategories { get; set; }
        public ImageRegion ItemListingRegion { get; set; }
        public List<ItemListing> ItemListings { get; } = new List<ItemListing>();
    }
}