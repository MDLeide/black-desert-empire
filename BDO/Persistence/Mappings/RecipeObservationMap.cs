using BDO.Domain.Observation;

namespace BDO.Persistence.Mappings
{
    public abstract class RecipeObservationMap<T> : DomainMap<T>
        where T : RecipeObservation
    {
        protected RecipeObservationMap()
        {
            References(p => p.Recipe);

            Map(p => p.Iterations);
            Map(p => p.Yield);
            Map(p => p.SkillLevel);

            //HasMany(p => p.AdditionalYield)
            //    .AsEntityMap();
        } 
    }
}