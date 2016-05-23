using System;
using System.Linq;
using BDO.Domain.Interfaces;
using NTC.NHIB.DomainModel;

namespace BDO.Domain.Observation
{
    public class MarketObservation : BdoDomainObject, IValidatesSave
    {
        public virtual Item Item { get; set; }
        public virtual DateTime EntryTime { get; set; }

        public virtual int Price { get; set; } = -1;
        public virtual int LastSalePrice { get; set; } = -1;

        public virtual int High { get; set; } = -1;
        public virtual int Low { get; set; } = -1;

        public virtual int TotalTrades { get; set; } = -1;
        public virtual int UnitsOnMarket { get; set; } = -1;

        public virtual int MinPrice { get; set; } = -1;
        public virtual int MaxPrice { get; set; } = -1;


        public virtual ValidationResult ValidateSave()
        {
            var results = new ValidationResult();

            if (Item == null)
                results.Violations.Add(new FieldViolation(nameof(Item), Item, "Item must have a value."));

            results.IsValid = !results.Violations.Any();
            return results;
        }

        public override string ToString()
        {
            return "Market Observation: " + Item.Name;
        }
    }
}