using FluentNHibernate.Mapping;
using NTC.NHIB.DomainModel;

namespace BDO.Persistence.Mappings
{
    public abstract class DomainMap<T> : ClassMap<T>
        where T : DomainObject
    {
        protected DomainMap()
        {
            Id(p => p.Id);
            Map(p => p.Created);
            Map(p => p.Modified);
        }  
    }
}
