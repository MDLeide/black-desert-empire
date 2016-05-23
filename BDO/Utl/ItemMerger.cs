using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDO.Domain;
using BDO.Domain.Observation;
using BDO.Persistence.Repo;

namespace BDO.Utl
{
    /// <summary>
    /// Provides methods for merging two <see cref="Item"/>'s data into a single item. Useful in situations 
    /// where two <see cref="Item"/>s were added that refer to the same Black Desert item, but were entered 
    /// with different names.
    /// </summary>
    public class ItemMerger
    { // check for resulting duplicates and merge those

        RecipeMerger _recipeMerger;

        ItemRepository _itemRepository;
        MarketObservationRepository _marketObservationRepository;
        RecipeRepository _recipeRepository;
        BasicShoppingListRepository _basicShoppingListRepository;

        public ItemMerger(ItemRepository itemRepo, MarketObservationRepository marketObservationRepo,
            RecipeRepository recipeRepo,
            BasicShoppingListRepository shoppingListRepo,
            ProcessingObservationRepository processingObservationRepo)
        {
            _itemRepository = itemRepo;
            _marketObservationRepository = marketObservationRepo;
            _recipeRepository = recipeRepo;
            _basicShoppingListRepository = shoppingListRepo;
            _recipeMerger = new RecipeMerger(recipeRepo, processingObservationRepo);
        }

        /// <summary>
        /// Merges the <see cref="source"/> and <see cref="target"/> items into a single item, using
        /// the <see cref="target"/>'s existing item data and ID. Coalesces recipes, observations and 
        /// shopping lists.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public ItemMergeResults MergeItems(Item source, Item target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (source.Id == Guid.Empty)
                throw new InvalidOperationException("Source is transient.");

            if (target.Id == Guid.Empty)
                throw new InvalidOperationException("Target is transient.");

            var results = new ItemMergeResults();

            results.MarketObservationsRedirected = MergeMarketObservations(source, target);
            results.RecipeMaterialsRedirected = MergeMaterialRecipes(source, target);
            results.RecipePrimaryResultsRedirected = MergePrimaryResultRecipes(source, target);
            results.RecipeSecondaryResultsRedirected = MergeSecondaryResultRecipes(source, target);
            results.ShoppingListItemsRedirected = MergeShoppingLists(source, target);
            results.RecipesMerged = MergeDuplicateRecipes(target);

            _itemRepository.Delete(source);

            return results;
        }
        
        int MergeMarketObservations(Item source, Item target)
        {
            var toChange = _marketObservationRepository.GetByItem(source).ToArray();
            foreach (var obs in toChange)
            {
                obs.Item = target;
                _marketObservationRepository.Save(obs);
            }
            return toChange.Length;
        }
        
        int MergePrimaryResultRecipes(Item source, Item target)
        {
            var asResult = _recipeRepository.GetByPrimaryResult(source).ToArray();
            foreach (var r in asResult)
            {
                r.Result = target;
                _recipeRepository.Save(r);
            }
            return asResult.Length;
        }

        int MergeSecondaryResultRecipes(Item source, Item target)
        {
            var asSecondaryResult = _recipeRepository.GetBySecondaryResult(source).ToArray();
            foreach (var r in asSecondaryResult)
            {
                r.SecondaryResults.Remove(source);
                r.SecondaryResults.Add(target);
                _recipeRepository.Save(r);
            }
            return asSecondaryResult.Length;
        }

        int MergeMaterialRecipes(Item source, Item target)
        {
            var asMaterial = _recipeRepository.GetByComponent(source).ToArray();
            foreach (var r in asMaterial)
            {
                var qty = r.Materials[source];
                r.Materials.Remove(source);
                if (!r.Materials.ContainsKey(target))
                    r.Materials.Add(target, qty);
                _recipeRepository.Save(r);
            }
            return asMaterial.Length;
        }
        
        int MergeShoppingLists(Item source, Item target)
        {
            var toMerge = _basicShoppingListRepository.GetByItem(source).ToArray();
            foreach (var l in toMerge)
            {
                l.Items.Remove(source);
                l.Items.Add(target);
                _basicShoppingListRepository.Save(l);
            }
            return toMerge.Length;
        }

        int MergeDuplicateRecipes(Item target)
        {
            var count = MergeDuplicateRecipes(_recipeRepository.GetByPrimaryResult(target).ToArray());
            count += MergeDuplicateRecipes(_recipeRepository.GetBySecondaryResult(target).ToArray());
            count += MergeDuplicateRecipes((_recipeRepository.GetByComponent(target)).ToArray());
            return count;
        }

        int MergeDuplicateRecipes(Recipe[] recipes)
        {
            var count = 0;
            for (int i = 0; i < recipes.Length; i++)
            {
                for (int j = i + 1; j < recipes.Length; j++)
                {
                    var source = recipes[j];
                    var target = recipes[i];
                    if (source == null || target == null)
                        continue;
                    if (!DuplicateChecker.RecipesAreEqual(source, target))
                        continue;

                    _recipeMerger.MergeRecipes(source, target);
                    count++;
                    recipes[j] = null;
                }
            }
            return count;
        }
    }

    public class ItemMergeResults
    {
        public int MarketObservationsRedirected { get; internal set; }
        public int ShoppingListItemsRedirected { get; internal set; }
        
        public int RecipePrimaryResultsRedirected { get; internal set; }
        public int RecipeSecondaryResultsRedirected { get; internal set; }
        public int RecipeMaterialsRedirected { get; internal set; }

        public int RecipesMerged { get; internal set; }
    }

    public class RecipeMerger
    {
        RecipeRepository _recipeRepository;
        ProcessingObservationRepository _processingObservationRepository;

        public RecipeMerger(RecipeRepository recipeRepository,
            ProcessingObservationRepository processingObservationRepository)
        {
            _recipeRepository = recipeRepository;
            _processingObservationRepository = processingObservationRepository;
        }

        /// <summary>
        /// Merges the associated processing, craft and design observations for two recipes. Deletes source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public int MergeRecipes(Recipe source, Recipe target)
        {
            //todo: update this when craft and design observations are implemented

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));
            
            if (source.Id == Guid.Empty)
                throw new InvalidOperationException("Source is transient.");

            if (target.Id == Guid.Empty)
                throw new InvalidOperationException("Target is transient.");

            if (!DuplicateChecker.RecipesAreEqual(source, target))
                throw new InvalidOperationException("Recipes are not duplicates. Cannot merge.");

            var obs = _processingObservationRepository.GetByRecipe(source).ToArray();
            foreach (var o in obs)
            {
                o.Recipe = target;
                _processingObservationRepository.Save(o);
            }

            _recipeRepository.Delete(source);

            return obs.Length;
        }
    }

    public static class DuplicateChecker
    {
        public static bool RecipeIsDuplicate(Recipe recipe, RecipeRepository repo)
        {
            var existing = repo.GetByPrimaryResult(recipe.Result).ToArray();
            if (!existing.Any())
                return false;

            return existing.Any(p => RecipesAreEqual(recipe, p));
        }

        public static bool RecipesAreEqual(Recipe a, Recipe b)
        {
            if (a == b)
                return true;

            if (a.Result != b.Result)
                return false;

            if (a.Materials.Count != b.Materials.Count)
                return false;

            foreach (var mat in a.Materials)
            {
                if (!b.Materials.ContainsKey(mat.Key))
                    return false;

                if (b.Materials[mat.Key] != mat.Value)
                    return false;
            }

            return true;
        }
    }
}
