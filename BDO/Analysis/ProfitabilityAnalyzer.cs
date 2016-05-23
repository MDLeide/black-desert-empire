using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BDO.Domain;
using BDO.Domain.Enum;
using BDO.Persistence.Repo;

namespace BDO.Analysis
{
    public class ProfitabilityAnalyzer
    {
        public ProfitabilityAnalysis Analyze(IEnumerable<Item> items)
        {
            var analysis = new ProfitabilityAnalysis();

            foreach (var i in items)
            {
                if (!i.MadeFrom.Any())
                    continue;

                var initial = i.Craft;
                i.Craft = true;
                var meta = new ItemMetaData(i);
                analysis.Add(new ProfitabilityAnlysisEntry() {Item = i, ProfitData = meta});
                i.Craft = initial;
            }

            return analysis;
        }

        public ProfitabilityAnalysis AnalyzeProcessing(IEnumerable<Recipe> processingRecipes )
        {
            var analysis = new ProfitabilityAnalysis();

            foreach (var recipe in processingRecipes)
            {
                var initial = recipe.Result.Craft;
                recipe.Result.Craft = true;
                var meta = new ItemMetaData(recipe.Result);
                analysis.Add(new ProfitabilityAnlysisEntry() {Item = recipe.Result, ProfitData = meta});
                recipe.Result.Craft = initial;
            }

            return analysis;
        }
    }

    public class ProfitabilityAnalysis
    {
        internal ProfitabilityAnalysis()
        {
        }

        public void Add(ProfitabilityAnlysisEntry entry)
        {
            Entries.Add(entry);

            Entries.Sort((p1, p2) => { return p2.ProfitData.Profit - p1.ProfitData.Profit; });

            for (int i = 0; i < Entries.Count; i++)
                Entries[i].Rank = i + 1;
        }

        public List<ProfitabilityAnlysisEntry> Entries { get; set; } = new List<ProfitabilityAnlysisEntry>();
    }

    public class ProfitabilityAnlysisEntry
    {
        public Item Item { get; set; }
        public int Rank { get; set; }
        public ItemMetaData ProfitData { get; set; }
    }
}
