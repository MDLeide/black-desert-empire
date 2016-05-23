using System.Collections.Generic;
using System.Linq;

namespace BDO.Domain
{
    public class RecipeList : BdoDomainObject
    {
        public virtual string Name { get; set; }
        public virtual IList<Recipe> Recipes { get; set; } = new List<Recipe>();
        public virtual IList<Item> Items => Recipes.SelectMany(p => p.Materials.Keys).Distinct().ToList();

        public virtual IDictionary<Item, int> ItemQuantities 
            =>
                Recipes.SelectMany(p => p.Materials)
                    .GroupBy(p => p.Key)
                    .ToDictionary(p => p.Key, p => p.Sum(v => v.Value));

        //public virtual IDictionary<Item, int> ItemQuantities => Recipes.SelectMany(p => p.Materials.Keys).Distinct().ToDictionary(p => p, Recipes.)
    }
}