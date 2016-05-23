using System.Drawing;
using System.Linq;

namespace BDO.MarketScraper.Img
{
    public static class ImageDecomposer
    {
        /// <summary>
        /// Gets the market image region of a screen shot.
        /// </summary>
        /// <param name="fullScreen"></param>
        /// <param name="uiScale"></param>
        /// <param name="resX"></param>
        /// <param name="resY"></param>
        /// <returns></returns>
        public static ImageRegion Decompose(Bitmap fullScreen, double uiScale, int resX, int resY)
        {
            //todo: probably infer x and y resolution from fullscreen bitmap dimensions

            var fullImage = new ImageRegion("Full Screen", 0, 0, resX, resY);
            fullImage.FullImage = fullScreen;

            var market = RegionDefinitions.CreateMarketScreenRegion();
            market.Scale(uiScale, true);

            var xOffset = (resX - market.RegionWidth) / 2;
            var yOffset = (resY - market.RegionHeight) / 2;

            market.RelativeOffsetX = xOffset;
            market.RelativeOffsetY = yOffset;
            fullImage.AddChild(market);
            //fullImage.SetColorData(fullScreen, true);

            return market;
        }

        public static MarketScreen GetMarketScreen(Bitmap fullScreen, double uiScale, int resX, int resY)
        {
            var region = Decompose(fullScreen, uiScale, resX, resY);
            var market = new MarketScreen();
            market.FullMarketScreen = region;
            market.MarketCategories = region.ChildRegions.FirstOrDefault(p => p.RegionName == "Market Categories");
            market.ItemListingRegion = region.ChildRegions.FirstOrDefault(p => p.RegionName == "Item Listings");

            foreach (var item in market.ItemListingRegion.ChildRegions)
            {
                var i = new ItemListing();
                
                i.FullItem = item;

                i.Icon = item.ChildRegions.FirstOrDefault(p => p.RegionName.ToLower().Contains("icon"));
                i.Name = item.ChildRegions.FirstOrDefault(p => p.RegionName.ToLower().Contains("name"));

                i.Lowest = item.ChildRegions.FirstOrDefault(p => p.RegionName.ToLower().Contains("lowest"));
                i.Highest = item.ChildRegions.FirstOrDefault(p => p.RegionName.ToLower().Contains("highest"));

                i.MarketPrice = item.ChildRegions.FirstOrDefault(p => p.RegionName.ToLower().Contains("market price"));
                i.LastSalePrice = item.ChildRegions.FirstOrDefault(p => p.RegionName.ToLower().Contains("sale price"));

                i.TotalSales = item.ChildRegions.FirstOrDefault(p => p.RegionName.ToLower().Contains("total sales"));
                i.CurrentListings = item.ChildRegions.FirstOrDefault(p => p.RegionName.ToLower().Contains("current listings"));
                market.ItemListings.Add(i);
            }

            return market;
        }
    }
}