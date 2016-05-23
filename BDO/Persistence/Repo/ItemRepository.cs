using System.Collections.Generic;
using BDO.Domain;
using NHibernate;

namespace BDO.Persistence.Repo
{
    public class ItemRepository : Repository<Item>
    {
        public ItemRepository()
        {
        }
        
        public ItemRepository(ISession session)
            : base(session)
        {
        }

        public IEnumerable<Item> GetByName(string itemName)
        {
            return ExecuteQuery(() =>
                Session.QueryOver<Item>()
                    .Where(p => p.Name == itemName));
        }

        public IEnumerable<Item> GetByName(string itemName, bool caseSensitive)
        {
            if (caseSensitive)
                return GetByName(itemName);

            return ExecuteQuery(() =>
                Session.QueryOver<Item>()
                    .Where(p => p.Name.ToUpper() == itemName.ToUpper()));
        }
    }
}