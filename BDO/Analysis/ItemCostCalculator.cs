using System.Collections.Generic;
using System.Linq;
using BDO.Domain;

namespace BDO.Analysis
{
    /// <summary>
    /// Gets the unit cost of an item based on current configuration settings.
    /// </summary>
    public static class ItemCostCalculator
    {
        /// <summary>
        /// Gets or sets a value instructing the <see cref="ItemCostCalculator"/> to override the <see cref="Item.Craft"/> 
        /// setting, meaning even if Craft is set to true, all methods will be considered. Defaults to <c>false</c>.
        /// </summary>
        public static bool OverideDoCraftSetting { get; set; } = false;
        
        /// <summary>
        /// Gets or sets a value instructing the <see cref="ItemCostCalculator"/> to override the <see cref="Item.HasSpecialPricing"/> 
        /// setting if there is a cheaper acquisition method available. Defaults to <c>false</c>.
        /// </summary>
        public static bool OverideSpecialPricing { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating that crafting methods should not be considered in the cost evaluation. Defaults
        /// to <c>false</c>.
        /// </summary>
        public static bool DoNotCraft { get; set; } = false;


        /// <summary>
        /// Gets the lowest price possible for an item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int GetUnitPrice(Item item)
        {
            UnitPriceMethod method;
            return GetUnitPrice(item, out method);
        }

        public static int GetUnitPrice(Item item, Character character)
        {
            UnitPriceMethod method;
            return GetUnitPrice(item, character, out method);
        }
        
        public static int GetUnitPrice(Item item, out UnitPriceMethod methodUsed)
        {
            int marketPrice = MarketPriceCalculator.GetMarketPrice(item);
            int craftCost = item.MadeFrom.Any() ? CraftCostCalculator.CostToCraft(item) : -1;
            var price = DetermineUnitPrice(item, craftCost, marketPrice, out methodUsed);
            return price;
        }

        public static int GetUnitPrice(Item item, Character character, out UnitPriceMethod methodUsed)
        {
            int marketPrice = MarketPriceCalculator.GetMarketPrice(item);
            int craftCost = item.MadeFrom.Any() ? CraftCostCalculator.CostToCraft(item, character) : -1;
            var price = DetermineUnitPrice(item, craftCost, marketPrice, out methodUsed);
            return price;
        }

        static int DetermineUnitPrice(Item item, int craftCost, int marketPrice, out UnitPriceMethod methodUsed)
        {
            if (item.HasSpecialPricing && !OverideSpecialPricing)
            {
                methodUsed = UnitPriceMethod.SpecialPricing;
                return item.SpecialPricing;
            }

            if (item.Craft && !OverideDoCraftSetting)
            {
                methodUsed = UnitPriceMethod.Craft;
                return craftCost;
            }

            Dictionary<UnitPriceMethod, int> values = new Dictionary<UnitPriceMethod, int>();
            values.Add(UnitPriceMethod.Market, marketPrice);
            if (craftCost > 0 && !item.Craft && OverideDoCraftSetting)
                values.Add(UnitPriceMethod.Craft, craftCost);
            if (item.HasSpecialPricing)
                values.Add(UnitPriceMethod.SpecialPricing, item.SpecialPricing);
            if (item.VendorSells)
                values.Add(UnitPriceMethod.Vendor, item.VendorCost);

            var min = values.Min(p => p.Value);
            var kvp = values.FirstOrDefault(p => p.Value == min);
            methodUsed = kvp.Key;
            return kvp.Value;
        }
    }
}