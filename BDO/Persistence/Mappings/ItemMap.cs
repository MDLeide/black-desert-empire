using BDO.Domain;

namespace BDO.Persistence.Mappings
{
    public class ItemMap : DomainMap<Item>
    {
        public ItemMap()
        {
            Map(p => p.Name);
            Map(p => p.Category);
            Map(p => p.MarketCategory);

            Map(p => p.IntId);

            Map(p => p.VendorCost);
            Map(p => p.VendorSells);

            Map(p => p.SpecialPricing);
            Map(p => p.HasSpecialPricing);

            Map(p => p.Craft);

            HasMany(p => p.AllMarketData)
                .Inverse();

            HasMany(p => p.UsedIn)
                .Inverse();

            HasMany(p => p.MadeFrom)
                .Inverse();
        }
    }
}