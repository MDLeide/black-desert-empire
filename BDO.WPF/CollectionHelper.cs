using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BDO.Domain;
using BDO.Domain.Enum;

namespace BDO.WPF
{
    static class CollectionHelper
    {
        static ObservableCollection<BasicShoppingList> _allShoppingLists;
        static ObservableCollection<MarketCategory> _marketCategories;
        static ObservableCollection<RecipeType> _recipeTypes; 
        static ObservableCollection<Item> _allItems;   

        static CollectionHelper()
        {
        }


        public static ObservableCollection<MarketCategory> MarketCategories
            => _marketCategories ?? (_marketCategories = GetMarketCategories());

        public static ObservableCollection<Item> AllItems
            => _allItems ?? (_allItems = new ObservableCollection<Item>(DomainObjectRepositories.ItemRepository.Get()));

        public static ObservableCollection<RecipeType> RecipeTypes
            => _recipeTypes ?? (_recipeTypes = GetRecipeTypes());

        public static ObservableCollection<BasicShoppingList> AllShoppingLists
            =>
                _allShoppingLists ??
                (_allShoppingLists =
                    new ObservableCollection<BasicShoppingList>(
                        DomainObjectRepositories.BasicShoppingListRepository.Get()));

        public static void RegisterNewItem(Item item)
        {
            AllItems.Add(item);
        }

        public static void RegisterNewShoppingList(BasicShoppingList list)
        {
            AllShoppingLists.Add(list);
        }

        public static void RefreshItemMarketData(string itemName)
        {
            foreach (
                var item in
                    AllItems.Where(p => string.Equals(p.Name, itemName, StringComparison.InvariantCultureIgnoreCase)).Distinct())
            {
                DomainObjectRepositories.ItemRepository.Session.Refresh(item);
                ItemMetaDataProvider.PromptRecalc(item);
            }
        }

        public static void RefreshItemMarketData(IEnumerable<string> itemNames)
        {
            var items =
                AllItems.Where(
                    p => itemNames.Any(z => string.Equals(p.Name, z, StringComparison.InvariantCultureIgnoreCase))).ToArray();
            foreach (var i in items)
                DomainObjectRepositories.ItemRepository.Session.Refresh(i);
            ItemMetaDataProvider.PromptRecalc(items);
        }

        static ObservableCollection<RecipeType> GetRecipeTypes()
        {
            var types = new ObservableCollection<RecipeType>();
            foreach (var n in Enum.GetValues(typeof(RecipeType)))
                types.Add((RecipeType)n);
            return types;
        } 

        static ObservableCollection<MarketCategory> GetMarketCategories()
        {
            var marketCategories = new ObservableCollection<MarketCategory>();
            foreach (var n in Enum.GetValues(typeof(MarketCategory)))
                marketCategories.Add((MarketCategory)n);
            return marketCategories;
        }

        
    }
}