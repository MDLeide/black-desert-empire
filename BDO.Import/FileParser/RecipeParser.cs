using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BDO.Domain;
using BDO.Domain.Enum;
using BDO.Persistence.Repo;

namespace BDO.Import.FileParser
{
    public class RecipeParser
    {
        public RecipeParser(ItemRepository itemRepository, RecipeParserSettings settings, string header, string secondaryResults, string materials)
        {
            var recipes = new Dictionary<int, Recipe>();

            var lines = Regex.Split(header, Environment.NewLine).Where(p => !string.IsNullOrEmpty(p));
            foreach (var l in lines.Skip(settings.HasHeader ? 1 : 0))
            {
                var fields = l.Split(',');
                var id = int.Parse(fields[settings.IdPosition]);
                var recipe = new Recipe();
                var itemName = fields[settings.ResultItemNamePosition];
                var item = itemRepository.GetByName(itemName).FirstOrDefault();
                if (item == null)
                    throw new Exception($"Could not find result item {itemName}.");
                recipe.Result = item;
                RecipeType rType;
                if (!RecipeType.TryParse(fields[settings.TypePosition], out rType))
                    throw new Exception("Could not parse recipe type.");
                recipe.Type = rType;
                recipe.SubType = fields[settings.SubTypePosition];
                recipes.Add(id, recipe);
            }

            var secondary = new Dictionary<int, List<Item>>();

            if (!string.IsNullOrEmpty(secondaryResults))
            {
                lines = Regex.Split(secondaryResults, Environment.NewLine).Where(p => !string.IsNullOrEmpty(p));
                foreach (var l in lines)
                {
                    var fields = l.Split(',');
                    var id = int.Parse(fields[settings.SecondaryResultsParentIdPosition]);
                    var itemName = fields[settings.SecondaryResultsItemNamePosition];
                    var item = itemRepository.GetByName(itemName).FirstOrDefault();
                    if (item == null)
                        throw new Exception($"Could not find secondary result item {itemName}.");
                    if (!secondary.ContainsKey(id))
                        secondary.Add(id, new List<Item>());
                    secondary[id].Add(item);
                }
            }

            var mats = new Dictionary<int, Dictionary<Item, int>>();

            lines = Regex.Split(materials, Environment.NewLine).Where(p => !string.IsNullOrEmpty(p));
            foreach (var l in lines)
            {
                var fields = l.Split(',');
                var id = int.Parse(fields[settings.MaterialsParentIdPosition]);
                var itemName = fields[settings.MaterialsItemNamePosition];
                var item = itemRepository.GetByName(itemName).FirstOrDefault();
                if (item == null)
                    throw new Exception($"Could not find material item {itemName}.");
                var qty = int.Parse(fields[settings.MaterialsQuantityPostiion]);

                if (!mats.ContainsKey(id))
                    mats.Add(id, new Dictionary<Item, int>());
                mats[id].Add(item, qty);
            }

            foreach (var recipe in recipes)
            {
                if (secondary.ContainsKey(recipe.Key))
                    recipe.Value.SecondaryResults = secondary[recipe.Key];
                recipe.Value.Materials = mats[recipe.Key];
            }

            Recipes = recipes.Values.ToList();
        }

        public List<Recipe> Recipes { get; private set; } 
    }
}