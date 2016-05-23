using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDO.Domain;
using BDO.WPF.VM.Domain;
using BDO.WPF.VM.Finders;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM.Screens
{
    class ShoppingListScreenViewModel : ViewModelBase
    {
        ItemFinderViewModel _allItems;
        ShoppingListViewModel _selectedShoppingList;
        ShoppingListFinderViewModel _shoppingListFinderViewModel;


        public ShoppingListScreenViewModel()
        {
            ShoppingListFinderViewModel = new ShoppingListFinderViewModel(CollectionHelper.AllShoppingLists);

            ShoppingListFinderViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ShoppingListFinderViewModel.SelectedObject))
                    if (ShoppingListFinderViewModel.SelectedObject != null)
                        SelectedShoppingList = ShoppingListFinderViewModel.SelectedObject;
            };

            //AllItems = new ItemFinderViewModel();
        }
       
        public ShoppingListFinderViewModel ShoppingListFinderViewModel
        {
            get { return _shoppingListFinderViewModel; }
            set
            {
                if (Equals(value, _shoppingListFinderViewModel)) return;
                _shoppingListFinderViewModel = value;
                OnPropertyChanged(nameof(ShoppingListFinderViewModel));
            }
        }

        //public ItemFinderViewModel AllItems
        //{
        //    get { return _allItems; }
        //    set
        //    {
        //        if (Equals(value, _allItems)) return;
        //        _allItems = value;
        //        OnPropertyChanged(nameof(AllItems));
        //    }
        //}

        public ShoppingListViewModel SelectedShoppingList
        {
            get { return _selectedShoppingList; }
            set
            {
                if (Equals(value, _selectedShoppingList)) return;
                _selectedShoppingList = value;
                OnPropertyChanged(nameof(SelectedShoppingList));
            }
        }

        #region NewShoppingList

        RelayCommand _newShoppingList;

        public RelayCommand NewShoppingList
        {
            get { return _newShoppingList ?? (_newShoppingList = new RelayCommand(o => OnNewShoppingList(), p => CanNewShoppingList())); }
        }

        void OnNewShoppingList()
        {
            SelectedShoppingList = new ShoppingListViewModel(new BasicShoppingList());
            SelectedShoppingList.ObjectSaved += SelectedShoppingListOnObjectSaved;
        }

        bool CanNewShoppingList()
        {
            return true;
        }

        void SelectedShoppingListOnObjectSaved(object sender, EventArgs eventArgs)
        {
            var vm = sender as ShoppingListViewModel;
            if (vm == null)
                return;
            ShoppingListFinderViewModel.Collection.Add(vm);
            vm.ObjectSaved -= SelectedShoppingListOnObjectSaved;
        }

        #endregion
    }
}
