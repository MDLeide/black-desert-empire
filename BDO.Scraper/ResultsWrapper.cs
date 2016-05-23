using System.Collections.Generic;
using BDO.Domain;

namespace BDO.Scraper
{
    class ResultsWrapper
    {
        public Recipe Recipe { get; set; }
        public RecipeParse Parse { get; set; }
        public bool Duplicate { get; set; }
        public bool ResultMissing { get; set; }
        public bool TypeParseError { get; set; }
        public List<string> ItemsMissing { get; set; } = new List<string>();
    }
}