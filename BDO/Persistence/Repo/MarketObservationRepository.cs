using System;
using System.Collections.Generic;
using BDO.Domain;
using BDO.Domain.Observation;
using NHibernate;

namespace BDO.Persistence.Repo
{
    public class MarketObservationRepository : Repository<MarketObservation>
    {
        public MarketObservationRepository()
        {
        }

        public MarketObservationRepository(ISession session)
            : base(session)
        {
        }

        public IEnumerable<MarketObservation> GetByItem(Item item)
        {
            return ExecuteQuery(() =>
                Session.QueryOver<MarketObservation>()
                    .Where(p => p.Item == item));
        }

        public IEnumerable<MarketObservation> GetByItemAndDate(Item item, DateTime startDate, DateTime endDate)
        {
            return ExecuteQuery(() =>
                Session.QueryOver<MarketObservation>()
                    .Where(p =>
                        p.Item == item &&
                        p.EntryTime <= endDate &&
                        p.EntryTime >= startDate));
        } 
    }
}