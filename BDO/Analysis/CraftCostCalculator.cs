using System;
using System.Diagnostics;
using System.Linq;
using BDO.Domain;
using BDO.Persistence.Repo;

namespace BDO.Analysis
{
    //todo: factor beer costs!

    public static class CraftCostCalculator
    {
        public static int CostToCraft(Item item)
        {
            if (!item.MadeFrom.Any())
                throw new Exception("No recipes found for this item.");
            return item.MadeFrom.Select(p => CalcCost(p)).Min();
        }
        
        public static int CostToCraft(Item item, out double yield)
        {
            if (!item.MadeFrom.Any())
                throw new Exception("No recipes found for this item.");

            yield = 0;
            var min = int.MaxValue;
            foreach (var r in item.MadeFrom)
            {
                double y;
                var cost = CalcCost(r, out y);
                if (cost < min)
                {
                    yield = y;
                    min = cost;
                }
            }

            return min;
            //return item.MadeFrom.Select(CalcCost).Min();
        }

        public static int CostToCraft(Item item, Character character)
        {
            if (!item.MadeFrom.Any())
                throw new Exception("No recipes found for this item.");
            return item.MadeFrom.Select(p => CalcCost(p, character)).Min();
        }

        public static int CostToCraft(Item item, Character character, out double yield)
        {
            if (!item.MadeFrom.Any())
                throw new Exception("No recipes found for this item.");

            yield = 0;
            var min = int.MaxValue;

            foreach (var r in item.MadeFrom)
            {
                double y;
                var cost = CalcCost(r, character, out y);
                if (cost < min)
                {
                    yield = y;
                    min = cost;
                }
            }
            return min;
        }

        static int CalcCost(Recipe recipe, Character character)
        {
            return
                (int)
                    (recipe.Materials.Sum(p => ItemCostCalculator.GetUnitPrice(p.Key)*p.Value)/
                     RecipeYieldCalculator.GetYield(recipe, character));
        }

        static int CalcCost(Recipe recipe)
        {
            return
                (int)
                    (recipe.Materials.Sum(p => ItemCostCalculator.GetUnitPrice(p.Key)*p.Value)/
                     RecipeYieldCalculator.GetYield(recipe));
        }

        static int CalcCost(Recipe recipe, Character character, out double yield)
        {
            yield = RecipeYieldCalculator.GetYield(recipe, character);
            return
                (int)
                    (recipe.Materials.Sum(p => ItemCostCalculator.GetUnitPrice(p.Key)*p.Value)/yield);
        }

        static int CalcCost(Recipe recipe, out double yield)
        {
            yield = RecipeYieldCalculator.GetYield(recipe);
            return
                (int)
                    (recipe.Materials.Sum(p => ItemCostCalculator.GetUnitPrice(p.Key)*p.Value)/yield);
        }
    }
}
