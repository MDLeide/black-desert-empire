using System.Collections.Generic;

namespace BDO.Domain.Interfaces
{
    public interface IShoppingList
    {
        IDictionary<Item, int> ItemQuantities { get; } 
    }
}