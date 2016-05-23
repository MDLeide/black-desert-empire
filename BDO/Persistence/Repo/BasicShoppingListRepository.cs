using System;
using System.Collections.Generic;
using BDO.Domain;
using NHibernate;
using NHibernate.Transform;

namespace BDO.Persistence.Repo
{
    public class BasicShoppingListRepository : Repository<BasicShoppingList>
    {
        public BasicShoppingListRepository()
        {
        }

        public BasicShoppingListRepository(ISession session)
            : base(session)
        {
        }

        public IEnumerable<BasicShoppingList> GetByName(string name)
        {
            return
                ExecuteQuery(
                    () =>
                        Session.QueryOver<BasicShoppingList>()
                            .Where(p => string.Equals(p.Name, name, StringComparison.InvariantCultureIgnoreCase)));
        }

        public IEnumerable<BasicShoppingList> GetByItem(Item item)
        {
            Item i = null;
            return
                ExecuteQuery(
                    () =>
                        Session.QueryOver<BasicShoppingList>()
                            .JoinAlias(l => l.Items, () => i)
                            .Where(() => item == i)
                            .TransformUsing(Transformers.DistinctRootEntity));

        } 
    }
}