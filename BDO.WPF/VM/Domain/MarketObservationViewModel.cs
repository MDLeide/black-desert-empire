using System;
using System.Windows;
using BDO.Domain.Observation;
using BDO.Persistence.Repo;
using BDO.WPF.VM.Base;
using NHibernate;

namespace BDO.WPF.VM.Domain
{
    class MarketObservationViewModel : DomainObjectViewModel<MarketObservation>
    {
        public MarketObservationViewModel(
            MarketObservation observation)
            : base(observation, DomainObjectRepositories.MarketObservationRepository)
        {
        }

        protected override void OnSave()
        {
            try
            {
                if (DomainObject.EntryTime == DateTime.MinValue)
                    DomainObject.EntryTime = DateTime.Now;

                base.OnSave();
            }
            catch (TransientObjectException)
            {
                MessageBox.Show(
                    "The item this observation is for is not saved. Please save the item first, then try again.",
                    "Unsaved Item");
            }
            
        }
    }
}