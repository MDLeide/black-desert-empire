using System.Collections.Generic;
using BDO.Domain;
using BDO.Domain.Observation;
using NHibernate;

namespace BDO.Persistence.Repo
{
    public abstract class RecipeObservationRepository<T> : Repository<T>
        where T : RecipeObservation
    {
        protected RecipeObservationRepository()
        {
        }

        protected RecipeObservationRepository(ISession session)
            : base(session)
        {
        }

        public IEnumerable<T> GetByResultItem(Item item)
        {
            return ExecuteQuery(() =>
                Session.QueryOver<T>()
                    .Where(p => p.Recipe.Result == item));
        }

        public IEnumerable<T> GetByRecipe(Recipe recipe)
        {
            return ExecuteQuery(() =>
                Session.QueryOver<T>()
                    .Where(p => p.Recipe == recipe));
        }
    }
}