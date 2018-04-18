using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BDO.Domain;
using BDO.Domain.Enum;
using BDO.Persistence.Repo;
using BDO.Utl;

namespace BDO.WebScraper
{
    static class RecipePreimport
    {
        static List<string> _materialExclusions = new List<string>()
        {
            "plywood",
            "ingot",
            "hide",
            "fur",
            "plume",
            "feather"
        };

        static Dictionary<string, string> _itemReplacements = new Dictionary<string, string>()
        {
            ["Blood 5"] = "Bat Blood",
            ["Blood 4"] = "Bear Blood",
            ["Blood 3"] = "Weasel Blood",
            ["Blood 2"] = "Deer Blood",
            ["Blood 1"] = "Wolf Blood"
        };


        /// <summary>
        /// Excludes things like ingots, plywood, etc. Implemented to prevent problems with 'PrimaryRecipe' that is used
        /// for market calculations, etc. That should be refected and exposed so the user can choose how they produce the item.
        /// </summary>
        public static bool ExcludeMaterialDesigns { get; set; } = true;

        public static bool ExcludeBlackStones { get; set; } = true;

        public static bool ExcludeMissingItems { get; set; } = true;

        public static bool ExcludeDuplicates { get; set; } = true;

        public static bool ExcludeTypeParseErrors { get; set; } = true;
        

        public static void SaveParseResults(IEnumerable<RecipeParse> parses, string headerPath, string detailPath)
        {
            parses = Filter(parses);

            var header = new StringBuilder();
            var detail = new StringBuilder();

            using (var iRepo = new ItemRepository())
            using (var rRepo = new RecipeRepository(iRepo))
            {
                var wrappers = parses.Select(p => BuildWrapper(p, iRepo, rRepo));

                header.AppendLine("Recipe Id,Result Name,Result Item Not Found,Duplicate Recipe,Type Parse Error");
                detail.AppendLine("Recipe Id,Result Name,Material Name,Material Quantity,Material Item Not Found");

                foreach (var w in wrappers)
                {
                    header.AppendLine($"{w.Parse.Id},{w.Parse.ItemName},{w.ResultMissing},{w.Duplicate},{w.TypeParseError}");
                    foreach (var mat in w.Parse.Materials)
                        detail.AppendLine($"{w.Parse.Id},{w.Parse.ItemName},{mat.Key},{mat.Value},{w.ItemsMissing.Contains(mat.Key)}");
                }
            }

            using (var sw = new StreamWriter(headerPath))
                sw.Write(header.ToString());
            using (var sw = new StreamWriter(detailPath))
                sw.Write(detail.ToString());
        }

        public static int FilterAndSaveToDatabase(IEnumerable<RecipeParse> parses)
        {
            using (var itemRepo = new ItemRepository())
            using (var recipeRepo = new RecipeRepository(itemRepo))
            {
                var wrappers = Filter(Filter(parses).Select(p => BuildWrapper(p, itemRepo, recipeRepo)));
                foreach (var w in wrappers.Where(p => p.Recipe != null))
                    recipeRepo.Save(w.Recipe);
                return wrappers.Count(p => p.Recipe != null);
            }
            
        }

        static IEnumerable<RecipeParse> Filter(IEnumerable<RecipeParse> parses)
        {
            if (ExcludeMaterialDesigns)
                parses = parses.Where(p => !_materialExclusions.Any(z => p.ItemName.ToLower().Contains(z.ToLower())));

            if (ExcludeBlackStones)
                parses = parses.Where(p => !p.ItemName.ToLower().Contains("black stone"));
            
            return parses;
        }

        static IEnumerable<ResultsWrapper> Filter(IEnumerable<ResultsWrapper> wrappers)
        {
            Console.WriteLine("Starting Wrappers: " + wrappers.Count());

            if (ExcludeMissingItems)
                wrappers = wrappers.Where(p => !p.ResultMissing && !p.ItemsMissing.Any());
            Console.WriteLine("Missing Items Filtered: " + wrappers.Count());

            if (ExcludeDuplicates)
                wrappers = wrappers.Where(p => !p.Duplicate);
            Console.WriteLine("Duplicates Filtered: " + wrappers.Count());

            if (ExcludeTypeParseErrors)
                wrappers = wrappers.Where(p => !p.TypeParseError);
            Console.WriteLine("Type Parses Filtered: " + wrappers.Count());

            return wrappers;
        }

        static ResultsWrapper BuildWrapper(RecipeParse parse, ItemRepository itemRepo, RecipeRepository recipeRepo)
        {
            var wrapper = new ResultsWrapper();
            wrapper.Parse = parse;
            var resultItem = itemRepo.GetByName(parse.ItemName).FirstOrDefault();
            if (resultItem == null)
            {
                wrapper.ResultMissing = true;
                return wrapper;
            }

            var recipe = new Recipe();
            wrapper.Recipe = recipe;
            recipe.Result = resultItem;

            var type = parse.RecipeType.Trim().ToLower();

            if (type == "alchemy")
            {
                recipe.Type = RecipeType.Craft;
                recipe.SubType = "Alchemy";
            }
            else if (type == "design")
            {
                recipe.Type = RecipeType.Workshop;
            }
            else if (type == "cook")
            {
                recipe.Type = RecipeType.Craft;
                recipe.SubType = "Cooking";
            }
            else
            {
                wrapper.TypeParseError = true;
            }
            
            foreach (var mat in parse.Materials)
            {
                var matName = mat.Key;
                if (_itemReplacements.ContainsKey(matName))
                    matName = _itemReplacements[matName];

                var mItem = itemRepo.GetByName(matName).FirstOrDefault();
                if (mItem == null)
                {
                    wrapper.ItemsMissing.Add(matName);
                    continue;
                }

                recipe.Materials.Add(mItem, mat.Value);
            }

            wrapper.Duplicate = DuplicateChecker.RecipeIsDuplicate(recipe, recipeRepo);

            return wrapper;
        }
        
    }
}