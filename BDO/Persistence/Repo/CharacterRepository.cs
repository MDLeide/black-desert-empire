using System.Collections.Generic;
using BDO.Domain;
using NHibernate;

namespace BDO.Persistence.Repo
{
    public class CharacterRepository : Repository<Character>
    {
        public CharacterRepository()
        {
        }

        public CharacterRepository(ISession session)
            : base(session)
        {
            
        }

        public IEnumerable<Character> GetByName(string characterName)
        {
            return ExecuteQuery(() =>
                Session.QueryOver<Character>()
                    .Where(p => p.Name == characterName));
        } 
    }
}