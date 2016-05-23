using System;
using System.Linq;
using BDO.Domain;

namespace BDO.Analysis
{
    public class ItemMetaData
    {
        public ItemMetaData(Item item)
            : this(item, null)
        {
        }

        public ItemMetaData(Item item, Character character)
        {
            Item = item;
            Character = character;
            Recalculate();
        }

        public event EventHandler Refreshed;


        public Item Item { get; }
        public Character Character { get; }

        public int UnitCost { get; set; }
        public UnitPriceMethod BestMethod { get; set; }
        public int MarketPrice { get; set; }
        public int NetRevenue { get; set; }
        public int Profit { get; set; }
        public int CraftCost { get; set; }
        public double CraftYield { get; set; }

        /// <summary>
        /// The difference between market price and the craft cost.
        /// </summary>
        public int MarketCraftDifference { get; set; }

        public void Recalculate()
        {
            UnitPriceMethod method;
            double craftYield = 0;
            if (Character != null)
            {
                UnitCost = ItemCostCalculator.GetUnitPrice(Item, Character, out method);
                CraftCost = Item.MadeFrom.Any() ? CraftCostCalculator.CostToCraft(Item, Character, out craftYield) : 0;
            }
            else
            {
                UnitCost = ItemCostCalculator.GetUnitPrice(Item, out method);
                CraftCost = Item.MadeFrom.Any() ? CraftCostCalculator.CostToCraft(Item, out craftYield) : 0;
            }

            CraftYield = craftYield;
            BestMethod = method;
            MarketPrice = MarketPriceCalculator.GetMarketPrice(Item);
            NetRevenue = MarketPriceCalculator.GetNetSaleRevenue(Item);
            Profit = NetRevenue - UnitCost;
            MarketCraftDifference = MarketPrice - CraftCost;

            Refreshed?.Invoke(this, new EventArgs());
        }
    }
}