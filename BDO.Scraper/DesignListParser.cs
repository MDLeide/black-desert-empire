using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace BDO.WebScraper
{
    class DesignListParser
    {
        public List<DesignParse> Parse(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var table = doc.DocumentNode.SelectSingleNode(@"//table[@id='RecipesTable']/tbody");
            return table.SelectNodes(@"tr").Select(ParseRow).ToList();
        }
        

        DesignParse ParseRow(HtmlNode node)
        {
            var idString = node.SelectSingleNode(@"td[@class=' dt-id']").InnerText;
            var name = node.SelectSingleNode(@"td[@class='dt-title sorting_1']/a/b").InnerText;
            var id = int.Parse(idString);
            return new DesignParse() {Id = id, DesignName = name};
        }
    }
}