using BDO.Domain;

namespace BDO.Persistence.Mappings
{
    public class RecipeMap : DomainMap<Recipe>
    {
        public RecipeMap()
        {
            References(p => p.Result);

            HasMany(p => p.Materials)
                .AsEntityMap("Item_id")
                .Element("integer_col", part => part.Type<int>());

            HasMany(p => p.SecondaryResults);

            Map(p => p.SubType);
            Map(p => p.Type);
            Map(p => p.ExpectedYield);

            HasMany(p => p.ProcessingObservations)
                .Inverse();
            HasMany(p => p.CraftObservations)
                .Inverse();
        } 
    }
}