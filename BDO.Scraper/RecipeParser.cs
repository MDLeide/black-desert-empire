using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace BDO.WebScraper
{
    class RecipeParser
    {
        public RecipeParse Parse(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var result = new RecipeParse();
            result.ItemName = GetItemName(doc);
            result.RecipeType = GetRecipeType(doc);
            result.Materials = GetMaterials(doc);
            return result;
        }

        public bool WriteToConsole { get; set; } = false;
        public bool StripUltimate { get; set; } = true;
        public bool StripLucky { get; set; } = true;

        string GetItemName(HtmlDocument doc)
        {
            if (WriteToConsole)
                Console.WriteLine("Item Name:");
            var nodes = doc.DocumentNode.SelectNodes("//span[@id='item_name']/b");
            if (WriteToConsole)
                foreach (var n in nodes)
                    Console.WriteLine(n.InnerText);
            if (WriteToConsole)
                Console.WriteLine();

            var text = nodes.FirstOrDefault()?.InnerText;
            if (text == null)
                return string.Empty;
            else
            {
                var decoded = WebUtility.HtmlDecode(text);
                if (StripUltimate)
                    decoded = decoded.Replace("Ultimate", "").Replace("  ", " ").Trim();
                if (StripLucky)
                    decoded = decoded.Replace("Lucky", "").Replace("  ", " ").Trim();
                return decoded;
            }
        }

        string GetRecipeType(HtmlDocument doc)
        {
            if (WriteToConsole)
                Console.WriteLine("Recipe Type:");

            var nodes = doc.DocumentNode.SelectNodes("//span[@class='category_text']");
            var text = string.Empty;
            
            foreach (var n in nodes)
                foreach (var innerNode in n.ParentNode.SelectNodes("span[@class='yellow_text']"))
                {
                    text = WebUtility.HtmlDecode(innerNode.InnerText);
                    if (WriteToConsole)
                        Console.WriteLine(WebUtility.HtmlDecode(innerNode.InnerText));
                }

            if (WriteToConsole)
                Console.WriteLine();

            if (text == null)
                return string.Empty;
            else
                return text;
        }

        Dictionary<string, int> GetMaterials(HtmlDocument doc)
        {
            if (WriteToConsole)
                Console.WriteLine("Materials:");
            var td =
                doc.DocumentNode.SelectSingleNode("//span[@class='yellow_text' and text() = '- Crafting Materials']").ParentNode;

            int lastQuantity = 1;
            
            var materials = new Dictionary<string, int>();

            foreach (var node in td.ChildNodes)
            {
                if (node.Name == "div")
                {
                    var qty = node.SelectSingleNode("a/div[@class='quantity_small nowrap']");
                    if (qty == null)
                        lastQuantity = 1;
                    else
                        lastQuantity = int.Parse(qty.InnerText);
                    continue;
                }
                if (node.Name == "a")
                {
                    var item = WebUtility.HtmlDecode(node.InnerText);
                    if (materials.ContainsKey(item))
                        materials[item] += lastQuantity;
                    else
                        materials.Add(item, lastQuantity);
                }
            }

            if (WriteToConsole)
                foreach (var m in materials)
                    Console.WriteLine($"{m.Key}:{m.Value}");
            
            return materials;
        } 

    }
}