using BDO.Domain.Observation;

namespace BDO.Persistence.Mappings
{
    public class CraftObservationMap : RecipeEnergyObservationMap<CraftObservation>
    {
        public CraftObservationMap()
        {
            Map(p => p.CraftTimeInSeconds);
        }
    }
}