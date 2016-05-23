using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDO.Analysis;
using BDO.Domain;
using BDO.Import.FileParser;
using BDO.Persistence.Repo;

namespace BDO.Import
{
    public static class Program
    {
        static void Main(string[] args)
        {
            //Analyze();
            //return;

            //using (var itemRepo = new ItemRepository())
            //{
            //    using (var recipeRepo = new RecipeRepository(itemRepo))
            //    {
            //        var importer = new ImportUtil();
            //        importer.DoImport(itemRepo, recipeRepo);
            //    }
            //}

            var importDirectory = new DirectoryInfo(@"C:\Users\Michael\Dropbox\BDO\imports");

            foreach (var f in importDirectory.EnumerateFiles())
            {
                ImportItems(f.FullName);
            }

            //ImportItems(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "general items import.csv"));
            //ImportMarket();
            //ImportRecipes(
            //    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ruby recipe header.csv"),
            //    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ruby recipe materials.csv"));
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        static void Analyze()
        {
            using (var itemRepo = new ItemRepository())
            {
                var items = itemRepo.Get();

                var analyzer = new ProfitabilityAnalyzer();
                var results = analyzer.Analyze(items);

                for (int i = 0; i < 10; i++)
                {
                    WriteMetaDeta(results.Entries[i].ProfitData);
                }
            }

            Console.ReadKey();
        }

        static void WriteMetaDeta(ItemMetaData data)
        {
            Console.WriteLine($"{data.Item.Name}: {data.MarketPrice} - {data.CraftCost} ({data.CraftYield}) = {data.Profit} ({data.BestMethod})");
        }

        public static void ImportItems(string path)
        {
            string items;
            using (var sr = new StreamReader(path))
                items = sr.ReadToEnd();
            
            using (var repo = new ItemRepository())
            {
                var import = new ImportItems(repo);
                import.Import(ItemParser.Parse(new ItemParserSettings(), items));
            }
        }
        
        public static void ImportMarket()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "market import.csv");
            string obs;
            using (var sr = new StreamReader(path))
                obs = sr.ReadToEnd();

            using (var repo = new ItemRepository())
            {
                var parser = new MarketObservationParser(new MarketObservationParserSettings(), repo, obs);
                using (var mRepo = new MarketObservationRepository())
                {
                    mRepo.Begin();
                    foreach (var o in parser.Observations)
                        mRepo.Save(o);
                    mRepo.End();
                }
            }
        }

        public static void ImportRecipes(string headerPath, string materialPath)
        {
            string header;
            using (var sr = new StreamReader(headerPath))
                header = sr.ReadToEnd();

            
            string materials;
            using (var sr = new StreamReader(materialPath))
                materials = sr.ReadToEnd();


            using (var itemrepo = new ItemRepository())
            {
                var parser = new RecipeParser(itemrepo, new RecipeParserSettings(), header, string.Empty, materials);

                using (var reciperepo = new RecipeRepository())
                {
                    reciperepo.Begin();
                    foreach (var recipe in parser.Recipes)
                    {
                        var existing = reciperepo.GetByPrimaryResult(recipe.Result);
                        bool skip = false;
                        foreach (var e in existing)
                        {
                            var match = true;
                            foreach (var kvp in recipe.Materials)
                                if (!e.Materials.ContainsKey(kvp.Key) || e.Materials[kvp.Key] != kvp.Value)
                                    match = false;
                            if (match)
                                skip = true;
                        }

                        if (skip)
                        {
                            Console.WriteLine($"Skipping duplicate recipe for {recipe.Result.Name}");
                            continue;
                        }

                        reciperepo.Save(recipe);
                    }
                    reciperepo.End();
                }
            }
        }
    }

}
