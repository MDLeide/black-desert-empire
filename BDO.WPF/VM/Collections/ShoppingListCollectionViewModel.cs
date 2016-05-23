using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDO.Domain;
using BDO.WPF.VM.Base;
using BDO.WPF.VM.Domain;

namespace BDO.WPF.VM.Collections
{
    class ShoppingListCollectionViewModel : DomainObjectFilterableCollectionViewModel<ShoppingListViewModel, BasicShoppingList>
    {
        public ShoppingListCollectionViewModel(IList<BasicShoppingList> shoppingLists)
            : base(
                shoppingLists,
                DomainObjectRepositories.BasicShoppingListRepository,
                list => new ShoppingListViewModel(list),
                () => { throw new InvalidOperationException(); })
        {
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(SelectedObject))
                if (SelectedObject != null)
                    ActiveList =
                        new ItemCollectionViewModel(SelectedObject.ListItems.Select(p => p.DomainObject).ToList());
        }

        ItemCollectionViewModel _activeList;

        public ItemCollectionViewModel ActiveList
        {
            get { return _activeList; }
            set
            {
                if (Equals(value, _activeList)) return;
                _activeList = value;
                OnPropertyChanged(nameof(ActiveList));
            }
        }
    }
}
