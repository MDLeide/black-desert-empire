using BDO.Domain.Observation;

namespace BDO.Persistence.Mappings
{
    public class ProcessingObservationMap : RecipeEnergyObservationMap<ProcessingObservation>
    {
        public ProcessingObservationMap()
        {
            Map(p => p.StartTime);
            Map(p => p.EndTime);
        }
    }
}