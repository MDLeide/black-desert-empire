using System.Collections.Generic;
using BDO.Domain;
using BDO.Persistence.Repo;
using BDO.WPF.V.Win;
using BDO.WPF.VM.Base;
using BDO.WPF.VM.Domain;

namespace BDO.WPF.VM.Collections
{
    class ItemCollectionViewModel : DomainObjectCollectionViewModel<ItemViewModel, Item>
    {
        public ItemCollectionViewModel(IList<Item> items)
            : base(
                items, 
                DomainObjectRepositories.ItemRepository,
                item => new ItemViewModel(item),
                () => new NewItemWindow())
        {
        }
    }
}
