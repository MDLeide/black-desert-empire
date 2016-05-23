using System.Collections.Generic;
using System.Linq;
using BDO.Domain;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace BDO.Persistence.Repo
{
    public class RecipeRepository : Repository<Recipe>
    {
        public RecipeRepository()
        {
        }

        public RecipeRepository(ISession session)
            : base(session)
        {
        }

        public RecipeRepository(ISessionHolder repo)
            : base(repo)
        {
        }

        public IEnumerable<Recipe> GetByPrimaryResult(Item result)
        {
            return ExecuteQuery(() =>
                Session.QueryOver<Recipe>()
                    .Where(p => p.Result == result));
        }

        public IEnumerable<Recipe> GetBySecondaryResult(Item result)
        {
            //todo: fix this

            return new Recipe[] {};
            Recipe rec = null;
            Item sec = null;
            Start();
            var r =
                Session.QueryOver<Recipe>(() => rec)
                    .JoinAlias(() => rec.SecondaryResults, () => sec)
                    .Where(() => sec != null && rec != null && sec == result).List<Recipe>();
            Finish();
            return r;
        }

        public IEnumerable<Recipe> GetByComponent(Item component)
        {
            //todo: figure this out
            return Get().Where(p => p.Materials.ContainsKey(component));
        } 
    }
}