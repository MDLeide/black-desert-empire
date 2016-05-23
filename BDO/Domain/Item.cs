using System.Collections.Generic;
using System.Linq;
using BDO.Domain.Enum;
using BDO.Domain.Interfaces;
using BDO.Domain.Observation;

namespace BDO.Domain
{
    /*
    The kind of try-hard i am, the worst kind: the kind with spreadsheets.

    Well... here you go.
    */

    public class Item : BdoDomainObject, IValidatesSave, INamedEntity
    {
        public virtual string Name { get; set; }
        public virtual string Category { get; set; }
        public virtual MarketCategory MarketCategory { get; set; }

        public virtual int IntId { get; set; }
        
        public virtual int VendorCost { get; set; }
        public virtual bool VendorSells { get; set; }

        public virtual int SpecialPricing { get; set; }
        public virtual bool HasSpecialPricing { get; set; }

        public virtual bool Craft { get; set; }

        public virtual MarketObservation CurrentMarketData
        {
            get { return AllMarketData.FirstOrDefault(p => p.EntryTime == AllMarketData.Max(z => z.EntryTime)); }
        }

        //todo: this doesn't really belong here...
        public virtual Recipe PrimaryRecipe { get; set; }

        public virtual IList<MarketObservation> AllMarketData { get; set; } = new List<MarketObservation>();
        public virtual IList<Recipe> UsedIn { get; set; } = new List<Recipe>();
        public virtual IList<Recipe> MadeFrom { get; set; } = new List<Recipe>();


        public virtual ValidationResult ValidateSave()
        {
            var result = new ValidationResult();
            if (string.IsNullOrEmpty(Name))
                result.Violations.Add(new FieldViolation(nameof(Name), Name, "Name cannot be empty."));
            result.IsValid = !result.Violations.Any();
            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}