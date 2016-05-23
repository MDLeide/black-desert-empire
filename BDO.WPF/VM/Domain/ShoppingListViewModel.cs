using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDO.Domain;
using BDO.Persistence.Repo;
using BDO.WPF.VM.Base;
using BDO.WPF.VM.Finders;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM.Domain
{
    class ShoppingListViewModel : DomainObjectViewModel<BasicShoppingList>
    {
        ObservableViewModelCollection<Item, ItemViewModel> _listITems;
        ItemViewModel _selectedItem;


        public ShoppingListViewModel(BasicShoppingList shoppingList)
            :base(shoppingList, DomainObjectRepositories.BasicShoppingListRepository)
        {
            ListItems = new ObservableViewModelCollection<Item, ItemViewModel>(
                shoppingList.Items,
                shoppingList.Items.Select(
                    p =>
                        new ItemViewModel(p)));

            AllItems = new ItemFinderViewModel();
        }

        ItemFinderViewModel _allItems;

        public ItemFinderViewModel AllItems
        {
            get { return _allItems; }
            set
            {
                if (Equals(value, _allItems)) return;
                _allItems = value;
                OnPropertyChanged(nameof(AllItems));
            }
        }

        public string Name
        {
            get { return DomainObject.Name; }
            set
            {
                DomainObject.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public ObservableViewModelCollection<Item, ItemViewModel> ListItems
        {
            get { return _listITems; }
            set
            {
                if (Equals(value, _listITems)) return;
                _listITems = value;
                OnPropertyChanged(nameof(ListItems));
            }
        }

        public ItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (Equals(value, _selectedItem)) return;
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        #region AddItems

        RelayCommand _addItems;

        public RelayCommand AddItems
        {
            get { return _addItems ?? (_addItems = new RelayCommand(o => OnAddItems(o), p => CanAddItems(p))); }
        }

        void OnAddItems(object o)
        {
            var list = o as IEnumerable<object>;
            if (list == null)
                return;

            foreach (var i in list.Cast<ItemViewModel>())
            {
                if (ListItems.All(p => p.DomainObject.Id != i.DomainObject.Id))
                    ListItems.Add(i);
            }
        }

        bool CanAddItems(object o)
        {
            var list = o as IEnumerable<object>;
            if (list == null)
                return false;
            foreach (var l in list)
            {
                var i = l as ItemViewModel;
                if (i == null)
                    return false;
                return true;
            }
            return false;
        }

        #endregion

        #region RemoveItems

        RelayCommand _removeItems;

        public RelayCommand RemoveItems
        {
            get { return _removeItems ?? (_removeItems = new RelayCommand(o => OnRemoveItems(o), p => CanRemoveItems(p))); }
        }

        void OnRemoveItems(object o)
        {
            var items = o as ObservableCollection<ItemViewModel>;
            if (items == null) return;
        }

        bool CanRemoveItems(object o)
        {
            var items = o as ObservableCollection<ItemViewModel>;
            return items != null && items.Any();
        }

        #endregion
    }
}
