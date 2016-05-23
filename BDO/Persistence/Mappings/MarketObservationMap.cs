using BDO.Domain.Observation;

namespace BDO.Persistence.Mappings
{
    public class MarketObservationMap : DomainMap<MarketObservation>
    {
        public MarketObservationMap()
        {
            References(p => p.Item);
            Map(p => p.EntryTime);

            Map(p => p.Price);
            Map(p => p.LastSalePrice);

            Map(p => p.High);
            Map(p => p.Low);

            Map(p => p.TotalTrades);
            Map(p => p.UnitsOnMarket);

            Map(p => p.MinPrice);
            Map(p => p.MaxPrice);
        }
    }
}