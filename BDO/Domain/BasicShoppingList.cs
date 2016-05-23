using System.Collections.Generic;
using BDO.Domain.Interfaces;

namespace BDO.Domain
{
    public class BasicShoppingList : BdoDomainObject, IItemList, INamedEntity, IValidatesSave
    {
        public virtual string Name { get; set; }
        public virtual IList<Item> Items { get; set; } = new List<Item>();

        IEnumerable<Item> IItemList.Items => Items;

        public virtual ValidationResult ValidateSave()
        {
            return new ValidationResult() {IsValid = true};
        }
    }
}