using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDO.Domain;
using BDO.Domain.Enum;
using BDO.Persistence.Repo;

namespace BDO.Import
{
    /// <summary>
    /// Throwaway class.
    /// </summary>
    class ImportUtil
    {
        public void DoImport(ItemRepository itemRepo, RecipeRepository recipeRepo)
        {
            Cleanup(itemRepo, recipeRepo);
            //return;

            var woodTiers = GetWoodTiers();
            foreach (var t in GetWoodTypes())
                CreateMaterials(itemRepo, recipeRepo, t, woodTiers);
            var oreTiers = GetMetalTiers();
            foreach (var t in GetOreTypes())
                CreateMaterials(itemRepo, recipeRepo, t, oreTiers);
            var hideTiers = GetHideTiers();
            foreach (var t in GetHideTypes())
                CreateMaterials(itemRepo, recipeRepo, t, hideTiers);
            var gemTiers = GetGemTiers();
            foreach (var t in GetGemTypes())
                CreateMaterials(itemRepo, recipeRepo, t, gemTiers);

            CreateNecklaces("Sapphire", "Gold", itemRepo, recipeRepo);
            CreateNecklaces("Diamond", "Gold", itemRepo, recipeRepo);
            CreateNecklaces("Emerald", "Silver", itemRepo, recipeRepo);
        }

        void Cleanup(ItemRepository itemRepository, RecipeRepository recipeRepository)
        {
            foreach (var metal in GetOreTypes())
            {
                var crystal = $"Crystal {metal} Pure";
                var i = itemRepository.GetByName(crystal).FirstOrDefault();
                if (i == null)
                    continue;

                foreach (var r in i.MadeFrom)
                    recipeRepository.Delete(r);

                itemRepository.Delete(i);
            }

            return;

            var item = itemRepository.GetByName("Sapphire").FirstOrDefault();
            var recipes = recipeRepository.GetByPrimaryResult(item);
            foreach (var r in recipes)
                recipeRepository.Delete(r);

            item = itemRepository.GetByName("Diamond").FirstOrDefault();
            recipes = recipeRepository.GetByPrimaryResult(item);
            foreach (var r in recipes)
                recipeRepository.Delete(r);
        }

        void CreateMaterials(ItemRepository itemRepo, RecipeRepository recipeRepo, string type, List<ItemTier> tiers)
        {
            foreach (var t in tiers)
            {
                var itemName = $"{t.Prefix} {type} {t.Suffix}".Trim();
                if (t.PrefixSuffixDependent)
                {
                    itemName = $"{GetGemTypeToPrefix()[type]} {type}";
                }

                var item = itemRepo.GetByName(itemName).FirstOrDefault();
                if (item == null)
                {
                    item = new Item();
                    item.Name = itemName;
                    itemRepo.Save(item);
                }
            }

            var tierCount = tiers.Max(p => p.Level);

            for (int i = 1; i <= tierCount; i++)
            {
                var tier = tiers.FirstOrDefault(p => p.Level == i);
                var itemName = $"{tier.Prefix} {type} {tier.Suffix}".Trim();
                if (tier.PrefixSuffixDependent)
                {
                    itemName = $"{GetGemTypeToPrefix()[type]} {type}";
                }

                var item = itemRepo.GetByName(itemName).FirstOrDefault();
                if (item.MadeFrom.Any())
                    continue;

                var recipe = new Recipe();
                recipe.Result = item;
                recipe.ExpectedYield = 2;
                var pTier = tiers.FirstOrDefault(p => p.Level == i - 1);
                var pItem = itemRepo.GetByName($"{pTier.Prefix} {type} {pTier.Suffix}".Trim()).FirstOrDefault();
                recipe.Materials.Add(pItem, tier.PreviousTierQuantityRequired);
                foreach (var additional in tier.AdditionalRequired)
                {
                    var aItem = itemRepo.GetByName(additional.Key).FirstOrDefault();
                    recipe.Materials.Add(aItem, additional.Value);
                }

                recipeRepo.Save(recipe);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gemName"></param>
        /// <param name="oreName"></param>
        /// <returns></returns>
        void CreateNecklaces(string gemName, string oreName, ItemRepository itemRepo, RecipeRepository recipeRepo)
        {
            var perfect = GetGemTypeToPrefix()[gemName];

            var neckName = $"{gemName} Necklace";
            var rNeckName = $"Resplendent {gemName} Necklace";
            var pNeckName = $"{perfect} {gemName} Necklace";

            string fNeckName = string.Empty;
            if (gemName.ToLower() == "diamond")
                fNeckName = "Starry Night Diamond Necklace";
            else
                fNeckName = $"{GetGemTypeToNecklacePrefix()[gemName]} {perfect} {gemName} Necklace";
            var uNeckName = GetGemTypeToUltimateNecklace()[gemName];

            var names = new string[]
            {
                neckName,
                rNeckName,
                pNeckName,
                fNeckName,
                uNeckName
            };

            foreach (var n in names)
            {
                var item = itemRepo.GetByName(n).FirstOrDefault();
                if (item == null)
                {
                    item = new Item();
                    item.Name = n;
                    item.MarketCategory = MarketCategory.Accessory;
                    itemRepo.Save(item);
                }
            }

            var rough = itemRepo.GetByName($"Rough {gemName}").FirstOrDefault();
            var gem = itemRepo.GetByName(gemName).FirstOrDefault();
            var rGem = itemRepo.GetByName($"Resplendent {gemName}").FirstOrDefault();
            var pGem = itemRepo.GetByName($"{GetGemTypeToPrefix()[gemName]} {gemName}").FirstOrDefault();

            var ore = itemRepo.GetByName($"{oreName} Ore").FirstOrDefault();
            var melted = itemRepo.GetByName($"Melted {oreName} Shard").FirstOrDefault();
            var ingot = itemRepo.GetByName($"{oreName} Ingot").FirstOrDefault();
            var crystal = itemRepo.GetByName($"Pure {oreName} Crystal").FirstOrDefault();

            for (int i = 0; i < names.Length; i++)
            {
                var item = itemRepo.GetByName(names[i]).FirstOrDefault();
                if (item == null)
                    throw new InvalidOperationException();

                if (item.MadeFrom.Any())
                    continue;

                Recipe recipe = new Recipe();
                recipe.Result = item;
                recipe.ExpectedYield = 1;

                if (i == 0)
                {
                    recipe.Materials.Add(rough, 5);
                    recipe.Materials.Add(ore, 5);
                }
                if (i == 1)
                {
                    var neck = itemRepo.GetByName(neckName).FirstOrDefault();
                    recipe.Materials.Add(neck, 1);
                    recipe.Materials.Add(melted, 2);
                    recipe.Materials.Add(gem, 1);
                }
                if (i == 2)
                {
                    var neck = itemRepo.GetByName(rNeckName).FirstOrDefault();
                    recipe.Materials.Add(neck, 1);
                    recipe.Materials.Add(ingot, 1);
                    recipe.Materials.Add(gem, 1);
                }
                if (i == 3)
                {
                    var neck = itemRepo.GetByName(pNeckName).FirstOrDefault();
                    recipe.Materials.Add(neck, 1);
                    recipe.Materials.Add(ingot, 1);
                    recipe.Materials.Add(rGem, 1);
                }
                if (i == 4)
                {
                    var neck = itemRepo.GetByName(fNeckName).FirstOrDefault();
                    recipe.Materials.Add(neck, 1);
                    recipe.Materials.Add(crystal, 2);
                    recipe.Materials.Add(pGem, 1);
                }

                recipeRepo.Save(recipe);
            }
        }

        // gem necklace
        // rough gem 5
        // ore 5

        // resplendent gem necklace
        // gem necklace
        // ore shard 2
        // gem 1

        // 'perfect' gem necklace
        // respledent gem necklace
        // ore ingot 1
        // gem 2

        // 'prefix' 'perfect' gem necklace
        // 'perfect' gem necklace
        // ore ingot 1
        // resplendent gem 1

        // gem necklace of 'suffix'
        // 'prefix' 'perfect' gem necklace
        // pure crystal 2
        // 'perfect' gem 1    

        List<string> GetWoodTypes()
        {
            return new List<string>()
            {
                "Birch",
                "Ash",
                "Fir",
                "Cedar",
                "Maple",
                "Pine",
                "Acacia",
                "White Cedar",
            };
        }

        List<string> GetOreTypes()
        {
            return new List<string>()
            {
                "Iron",
                "Copper",
                "Tin",
                "Lead",
                "Zinc",
                "Platinum",
                "Silver",
                "Gold",

            };
        }

        List<string> GetHideTypes()
        {
            return new List<string>()
            {
                "Thin",
                "Tough",
                "Soft",
                "Hard",
            };
        }

        List<string> GetGemTypes()
        {
            return new List<string>()
            {
                "Ruby",
                "Emerald",
                "Sapphire",
                "Diamond",
                "Topaz",
            };
        }

        Dictionary<string, string> GetGemTypeToPrefix()
        {
            return new Dictionary<string, string>()
            {
                ["Ruby"] = "Blood",
                ["Emerald"] = "Forest",
                ["Sapphire"] = "Ocean",
                ["Diamond"] = "Star",
                ["Topaz"] = "Gold"
            };
        }

        Dictionary<string, string> GetGemTypeToNecklacePrefix()
        {
            return new Dictionary<string, string>()
            {
                ["Ruby"] = "Dark",
                ["Sapphire"] = "Translucent",
                ["Topaz"] = "Shining",
                ["Emerald"] = "Placid"
            };
        }

        Dictionary<string, string> GetGemTypeToUltimateNecklace()
        {
            return new Dictionary<string, string>()
            {
                ["Ruby"] = "Corrupt Ruby Necklace",
                ["Sapphire"] = "Sapphire Necklace of Storms",
                ["Topaz"] = "Topaz Necklace of Regeneration",
                ["Emerald"] = "Emerald Necklace of Tranquility",
                ["Diamond"] = "Diamond Necklace of Fortitude    "
            };
        }

        List<ItemTier> GetWoodTiers()
        {
            return new List<ItemTier>
            {
                new ItemTier()
                {
                    Level = 0,
                    Suffix = "Timber"
                },
                new ItemTier()
                {
                    Level = 1,
                    Suffix = "Plank",
                    PreviousTierQuantityRequired = 5
                },
                new ItemTier()
                {
                    Level = 2,
                    Suffix = "Plywood",
                    PreviousTierQuantityRequired = 10
                },
                new ItemTier()
                {
                    Level = 3,
                    Prefix = "Sturdy",
                    Suffix = "Plywood",
                    PreviousTierQuantityRequired = 10,
                    AdditionalRequired = new Dictionary<string, int>()
                    {
                        ["Plywood Hardener"] = 3
                    }
                }
            };
        }

        List<ItemTier> GetMetalTiers()
        {
            return new List<ItemTier>
            {
                new ItemTier()
                {
                    Level = 0,
                    Suffix = "Ore"
                },
                new ItemTier()
                {
                    Level = 1,
                    Prefix = "Melted",
                    Suffix = "Shard",
                    PreviousTierQuantityRequired = 5
                },
                new ItemTier()
                {
                    Level = 2,
                    Suffix = "Ingot",
                    PreviousTierQuantityRequired = 10
                },
                new ItemTier()
                {
                    Level = 3,
                    Prefix = "Pure",
                    Suffix = "Crystal",
                    PreviousTierQuantityRequired = 3,
                    AdditionalRequired = new Dictionary<string, int>()
                    {
                        ["Metal Solvent"] = 2
                    }
                }
            };
        }

        List<ItemTier> GetHideTiers()
        {
            return new List<ItemTier>()
            {
                new ItemTier()
                {
                    Level = 0,
                    Suffix = "Hide"
                },
                new ItemTier()
                {
                    Level = 1,
                    Prefix = "Fine",
                    Suffix = "Hide",
                    PreviousTierQuantityRequired = 10
                },
                new ItemTier()
                {
                    Level = 2,
                    Prefix = "Supreme",
                    Suffix = "Hide",
                    PreviousTierQuantityRequired = 3,
                    AdditionalRequired = new Dictionary<string, int>()
                    {
                        ["Leather Glaze"] = 5
                    }
                }
            };
        }

        List<ItemTier> GetGemTiers()
        {
            return new List<ItemTier>()
            {
                new ItemTier()
                {
                    Level = 0,
                    Prefix = "Rough"
                },
                new ItemTier()
                {
                    Level = 1,
                    PreviousTierQuantityRequired = 5
                },
                new ItemTier()
                {
                    Level = 2,
                    Prefix = "Resplendent",
                    PreviousTierQuantityRequired = 5
                },
                new ItemTier()
                {
                    Level = 3,
                    PrefixSuffixDependent = true,
                    PreviousTierQuantityRequired = 7,
                    AdditionalRequired = new Dictionary<string, int>()
                    {
                        ["Gem Polisher"] = 3
                    }
                }
            };
        }


    }

    

    class ItemTier
    {
        public int Level { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public int PreviousTierQuantityRequired { get; set; } = 0;
        public Dictionary<string, int> AdditionalRequired { get; set; } = new Dictionary<string, int>();
        public int ExpectedYield { get; set; } = 2;
        public bool PrefixSuffixDependent { get; set; } = false;
    }
}
