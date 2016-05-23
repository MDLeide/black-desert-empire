using System;
using BDO.Domain.Enum;
using NTC.UTL;

namespace BDO.MarketScraper.Img
{
    static class RegionDefinitions
    {
        public static ImageRegion CreateMarketScreenRegion()
        {
            var marketScreen = new ImageRegion("Market", 0, 0, 890, 690);
            var marketCategories = new ImageRegion("Market Categories", 30, 59, 787, 39);
            marketScreen.AddChild(marketCategories);
            var itemListings = new ImageRegion("Item Listings", 165, 180, 690, 432);
            marketScreen.AddChild(itemListings);

            var marketCats = Enum.GetValues(typeof (MarketCategory));

            for (int i = 0; i < marketCats.Length; i++)
            {
                var catSide = 39;
                var spacer = 5;
                var region = new ImageRegion($"{((MarketCategory) i).GetDescription()} Market Category", catSide*i + spacer*i, 0, catSide, catSide);
                marketCategories.AddChild(region);
            }

            var itemsPerScreen = 7;
            for (int i = 0; i < itemsPerScreen; i++)
            {
                var itemHeight = 60;
                var itemWidth = 690;
                var spacer = 2;
                var region = new ImageRegion($"Item {i}", 0, itemHeight * i + spacer * i, itemWidth, itemHeight );

                var icon = new ImageRegion($"Item {i} Icon", 12, 9, 40, 40);

                var name = new ImageRegion($"Item {i} Name", 58, 0, 166, 60);
                var lowest = new ImageRegion($"Item {i} Lowest", 224, 30, 96, 30);
                var highest = new ImageRegion($"Item {i} Highest", 320, 30, 126, 30);

                var price = new ImageRegion($"Item {i} Market Price", 489, 0, 75, 30);
                var last = new ImageRegion($"Item {i} Last Sale Price", 489, 30, 75, 30);

                var total = new ImageRegion($"Item {i} Total Sales", 605, 0, 65, 30);
                var current = new ImageRegion($"Item {i} Current Listings", 605, 30, 65, 30);

                region.AddChild(icon);
                region.AddChild(name);
                region.AddChild(lowest);
                region.AddChild(highest);
                region.AddChild(price);
                region.AddChild(last);
                region.AddChild(total);
                region.AddChild(current);

                itemListings.AddChild(region);
            }

            return marketScreen;
        }
    }
}