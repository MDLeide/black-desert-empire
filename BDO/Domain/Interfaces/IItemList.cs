using System.Collections.Generic;

namespace BDO.Domain.Interfaces
{
    public interface IItemList
    {
        IEnumerable<Item> Items { get; } 
    }
}