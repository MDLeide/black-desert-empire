using BDO.Domain;

namespace BDO.Persistence.Mappings
{
    public class BasicShoppingListMap : DomainMap<BasicShoppingList>
    {
        public BasicShoppingListMap()
        {
            Map(p => p.Name);
            //HasMany(p => p.Items);
            HasManyToMany(p => p.Items);
        }
    }
}