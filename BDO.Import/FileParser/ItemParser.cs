using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BDO.Domain;
using BDO.Domain.Enum;

namespace BDO.Import.FileParser
{
    public class ParsedItem
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public MarketCategory MarketCategory { get; set; }
        public bool VendorSells { get; set; }
        public int VendorCost { get; set; }

        public bool MarketCategoryParseError { get; set; }
    }

    public static class ItemParser
    {
        public static IEnumerable<ParsedItem> Parse(ItemParserSettings settings, string content)
        {
            var lines = Regex.Split(content, Environment.NewLine).Where(p => !string.IsNullOrEmpty(p));

            var items = new List<ParsedItem>();

            foreach (var l in lines.Skip(settings.HasHeader ? 1 : 0))
            {
                var fields = l.Split(',');
                var item = new ParsedItem();
                item.Name = fields[settings.NamePosition];
                item.Category = fields[settings.CategoryPosition];
                MarketCategory cat = MarketCategory.General;
                if (!MarketCategory.TryParse(fields[settings.MarketCategoryPosition], out cat))
                    item.MarketCategoryParseError = true;

                item.MarketCategory = cat;

                item.VendorSells = bool.Parse(fields[settings.VendorSellsPosition]);

                if (fields.Length > settings.VendorCostPosition)
                    item.VendorCost = int.Parse(fields[settings.VendorCostPosition]);
                else
                    item.VendorCost = 0;

                items.Add(item);
            }

            return items;
        }
    }
}