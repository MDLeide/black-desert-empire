using BDO.Domain.Observation;
using NHibernate;

namespace BDO.Persistence.Repo
{
    public class CraftObservationRepository : RecipeObservationRepository<CraftObservation>
    {
        public CraftObservationRepository()
        {
        }

        public CraftObservationRepository(ISession session)
            : base(session)
        {
        }
    }
}