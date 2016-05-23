using System;
using System.Collections.Generic;
using System.Linq;
using BDO.Analysis;
using BDO.Domain;

namespace BDO.WPF
{
    static class ItemMetaDataProvider
    {
        //todo: debug posible repeat updates

        static Dictionary<Item, ItemMetaData> _itemMetaData = new Dictionary<Item, ItemMetaData>();

        public static ItemMetaData GetMetaData(Item item)
        {
            ItemMetaData meta;
            
            if (!_itemMetaData.TryGetValue(item, out meta))
            {
                item.ObjectSaved += ItemOnObjectSaved;
                meta = new ItemMetaData(item, StaticSettings.Character);
                _itemMetaData.Add(item, meta);
            }

            return meta;
        }

        public static void PromptRecalc(Item item)
        {
            ItemMetaData meta;

            if (!_itemMetaData.TryGetValue(item, out meta))
                return;

            meta.Recalculate();

            PromptRecalc(DomainObjectRepositories.RecipeRepository.GetByComponent(item).Select(p => p.Result));
        }

        public static void PromptRecalc(IEnumerable<Item> items)
        {
            //var usedIn = new List<Item>();
            foreach (var i in items)
            {
                ItemMetaData data;
                if (!_itemMetaData.TryGetValue(i, out data)) 
                    continue;
                data.Recalculate();
                
                //foreach (var r in DomainObjectRepositories.RecipeRepository.GetByComponent(i).Select(p => p.Result))
                //    if (!usedIn.Contains(r))
                //        usedIn.Add(r);
            }

            //if (usedIn.Any())
            //    PromptRecalc(usedIn);
        }

        static void ItemOnObjectSaved(object sender, EventArgs eventArgs)
        {
            var item = sender as Item;
            if (item == null)
                return;
            PromptRecalc(item);
            //_itemMetaData[item].Recalculate();
        }
    }
}