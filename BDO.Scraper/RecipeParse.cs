using System.Collections.Generic;

namespace BDO.WebScraper
{
    class RecipeParse
    {
        public string ItemName { get; set; }
        public string RecipeType { get; set; }
        public Dictionary<string, int> Materials { get; set; } 
        public string Id { get; set; }
    }
}