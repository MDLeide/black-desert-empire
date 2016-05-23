using BDO.Domain.Observation;
using NHibernate;

namespace BDO.Persistence.Repo
{
    public class ProcessingObservationRepository : RecipeObservationRepository<ProcessingObservation>
    {
        public ProcessingObservationRepository()
        {
        }

        public ProcessingObservationRepository(ISession session)
            : base(session)
        {
        }
    }
}