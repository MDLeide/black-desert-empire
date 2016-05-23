using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BDO.Domain.Observation;
using BDO.Persistence.Repo;

namespace BDO.Import.FileParser
{
    public class MarketObservationParser
    {
        public MarketObservationParser(MarketObservationParserSettings settings, ItemRepository itemRepository,
            string contents)
        {
            var lines = Regex.Split(contents, Environment.NewLine).Where(p => !string.IsNullOrEmpty(p));
            var observations = new List<MarketObservation>();
            foreach (var l in lines.Skip(settings.HasHeader ? 1 : 0))
            {
                var fields = l.Split(',');
                var obs = new MarketObservation();
                var itemName = fields[settings.ItemNamePosition];
                var item = itemRepository.GetByName(itemName).FirstOrDefault();
                if (item == null)
                    throw new Exception($"Could not find item {itemName}");

                obs.Item = item;
                obs.EntryTime = DateTime.Parse(fields[settings.EntryTimePosition]);
                obs.Price = int.Parse(fields[settings.PricePosition]);
                obs.LastSalePrice = int.Parse(fields[settings.LastSalePricePosition]);
                obs.High = int.Parse(fields[settings.HighPosition]);
                obs.Low = int.Parse(fields[settings.LowPosition]);
                obs.TotalTrades = int.Parse(fields[settings.TotalTradesPosition]);
                obs.UnitsOnMarket = int.Parse(fields[settings.UnitsOnMarketPosition]);
                obs.MaxPrice = int.Parse(fields[settings.MaxPricePosition]);
                obs.MinPrice = int.Parse(fields[settings.MinPricePosition]);
                observations.Add(obs);
            }

            Observations = observations;
        }

        public List<MarketObservation> Observations { get; private set; }
    }
}