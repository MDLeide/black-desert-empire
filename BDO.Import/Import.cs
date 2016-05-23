using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDO.Domain;
using BDO.Import.FileParser;
using BDO.Persistence.Repo;
using NHibernate.SqlCommand;

namespace BDO.Import
{
    public class ImportItems
    {
        ItemRepository _itemRepository;

        public ImportItems(ItemRepository itemRepository)
        {
            if (itemRepository == null)
                throw new ArgumentNullException(nameof(itemRepository));
            _itemRepository = itemRepository;
        }

        public bool ExcludeItemsWithDuplicateName { get; set; } = true;
        public bool ExcludeMarketParseErrors { get; set; } = true;
        public bool ConsiderCaseWhenComparingNames { get; set; } = false;


        public List<ImportResult<Item>> Import(IEnumerable<ParsedItem> itemsToImport)
        {
            return ImportFromParse(itemsToImport, true);
        }

        public List<ImportResult<Item>> PreviewImport(IEnumerable<ParsedItem> itemsToImport)
        {
            return ImportFromParse(itemsToImport, false);
        }

        public List<ImportResult<Item>> Import(IEnumerable<Item> itemsToImport)
        {
            return ImportFromItem(itemsToImport, true);
        }

        public List<ImportResult<Item>> PreviewImport(IEnumerable<Item> itemsToImport)
        {
            return ImportFromItem(itemsToImport, false);
        }


        List<ImportResult<Item>> ImportFromParse(IEnumerable<ParsedItem> itemsToImport, bool executeSave)
        {
            if (ExcludeMarketParseErrors)
                return ImportFromItem(itemsToImport.Where(p => !p.MarketCategoryParseError).Select(ItemFromParse), executeSave);
            return ImportFromItem(itemsToImport.Select(ItemFromParse), executeSave);
        }

        List<ImportResult<Item>> ImportFromItem(IEnumerable<Item> itemsToImport, bool executeSave)
        {
            var results = new List<ImportResult<Item>>();

            if (executeSave)
                _itemRepository.Begin();
            foreach (var item in itemsToImport)
            {
                var r = new ImportResult<Item>(item);
                results.Add(r);

                if (item.Id != Guid.Empty)
                {
                    r.InternalExcludedReasons.Add("Item already present in database.");
                    continue;
                }

                if (ExcludeItemsWithDuplicateName)
                {
                    var existing = _itemRepository.GetByName(item.Name, ConsiderCaseWhenComparingNames);
                    if (existing.Any())
                    {
                        r.InternalExcludedReasons.Add("Item with same name exists.");
                        continue;
                    }
                }

                if (executeSave)
                {
                    try
                    {
                        _itemRepository.Save(item);
                    }
                    catch (Exception e)
                    {
                        r.InternalExcludedReasons.Add("Exception on save: " + e.Message);
                    }
                }

                r.Imported = true;
            }

            if (executeSave)
                _itemRepository.End();

            return results;
        }

        public static Item ItemFromParse(ParsedItem parse)
        {
            var item = new Item();
            item.Name = parse.Name;
            item.Category = parse.Category;
            item.MarketCategory = parse.MarketCategory;
            item.VendorCost = parse.VendorCost;
            item.VendorSells = parse.VendorSells;
            return item;
        }
    }

    public class ImportRecipes
    {
        
    }


    public class ImportResult<T>
        where T : BdoDomainObject
    {
        internal ImportResult(T obj)
        {
            Object = obj;
            InternalExcludedReasons = new List<string>();
            ExcludedReasons = new ReadOnlyCollection<string>(InternalExcludedReasons);
        } 

        public T Object { get; internal set; }
        public bool Imported { get; internal set; }
        internal List<string> InternalExcludedReasons { get; }
        public ReadOnlyCollection<string> ExcludedReasons { get; }
    }
}
