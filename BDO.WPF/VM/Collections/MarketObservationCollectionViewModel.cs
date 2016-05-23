using System.Collections.Generic;
using BDO.Domain;
using BDO.Domain.Observation;
using BDO.Persistence.Repo;
using BDO.WPF.V.Win;
using BDO.WPF.VM.Base;
using BDO.WPF.VM.Domain;

namespace BDO.WPF.VM.Collections
{
    class MarketObservationCollectionViewModel : DomainObjectCollectionViewModel<MarketObservationViewModel, MarketObservation>
    {
        public MarketObservationCollectionViewModel(
            IList<MarketObservation> observations)
            : base(
                observations,
                DomainObjectRepositories.MarketObservationRepository, 
                observation => new MarketObservationViewModel(observation),
                () => new NewMarketObservationWindow())
        {
        }

        public MarketObservationCollectionViewModel(
            IList<MarketObservation> observations,
            Item forItem)
            : base(
                observations,
                DomainObjectRepositories.MarketObservationRepository,
                (observation) =>
                {
                    observation.Item = forItem;
                    return new MarketObservationViewModel(observation);
                }, 
                () => new NewMarketObservationWindow())
        {
        }
    }
}