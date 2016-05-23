using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDO.Domain.Interfaces;
using FluentNHibernate.Conventions.Helpers.Builders;
using NTC.NHIB.DomainModel;

namespace BDO.Domain
{
    public class ShoppingList : BdoDomainObject, IItemList, IShoppingList, INamedEntity, IValidatesSave
    {
        public virtual string Name { get; set; }
        public virtual IDictionary<Item, int> ItemQuantities { get; set; }
            = new Dictionary<Item, int>();

        IEnumerable<Item> IItemList.Items => ItemQuantities.Keys;


        public ValidationResult ValidateSave()
        {
            var valid = new ValidationResult();
            if (string.IsNullOrEmpty(Name))
                valid.Violations.Add(new FieldViolation(nameof(Name), Name, "Must have a name."));
            valid.IsValid = !valid.Violations.Any();
            return valid;
        }
    }
}
