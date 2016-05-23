using BDO.Domain.Observation;

namespace BDO.Persistence.Mappings
{
    public abstract class RecipeEnergyObservationMap<T> : RecipeObservationMap<T>
        where T : RecipeEnergyObservation
    {
        protected RecipeEnergyObservationMap()
        {
            Map(p => p.StartingEnergy);
            Map(p => p.EndingEnergy);
            Map(p => p.EnergyRecoveryAmount);
            Map(p => p.EnergyRecoveryIntervalInSeconds);
            Map(p => p.TotalTimeInSeconds);
        }
             
    }
}