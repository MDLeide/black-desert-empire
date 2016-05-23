using System.Linq;
using BDO.Domain.Enum;
using BDO.Persistence.Config;
using NHibernate;
using NTC.NHIB.DomainModel;

namespace BDO.Persistence.Repo
{
    public class Repository<T> : DomainObjectRepository<T>, ISessionHolder
        where T : DomainObject
    {
        public Repository() 
            : base(Configuration.GetSession())
        {
        }

        public Repository(ISession session)
            : base(session)
        {
        }
        
        public Repository(ISessionHolder repo )
            :base(repo.Session)
        {
        }
    }

    public interface ISessionHolder
    {
        ISession Session { get; }
    }
}
