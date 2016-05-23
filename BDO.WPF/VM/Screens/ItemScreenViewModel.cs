using System;
using System.Linq;
using BDO.Domain;
using BDO.Persistence.Repo;
using BDO.WPF.VM.Domain;
using BDO.WPF.VM.Finders;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM.Screens
{
    class ItemScreenViewModel : ViewModelBase
    {
        ItemFinderViewModel _itemFinderViewModel;
        ItemViewModel _itemViewModel;

        //todo: refactor this class 
        // probably remove it and use item collection

        public ItemScreenViewModel()
        {
            var items = CollectionHelper.AllItems;
            
            var item = items.First();
            ItemFinderViewModel = new ItemFinderViewModel();
            ItemViewModel = new ItemViewModel(item);

            ItemFinderViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ItemFinderViewModel.SelectedObject))
                    if (ItemFinderViewModel.SelectedObject != null)
                        ItemViewModel = ItemFinderViewModel.SelectedObject;
            };
        }
        
        public ItemFinderViewModel ItemFinderViewModel
        {
            get { return _itemFinderViewModel; }
            set
            {
                if (Equals(value, _itemFinderViewModel)) return;
                _itemFinderViewModel = value;
                OnPropertyChanged(nameof(ItemFinderViewModel));
            }
        }

        public ItemViewModel ItemViewModel
        {
            get { return _itemViewModel; }
            set
            {
                if (Equals(value, _itemViewModel)) return;
                _itemViewModel = value;
                OnPropertyChanged(nameof(ItemViewModel));
            }
        }

        #region NewItem

        RelayCommand _newItem;

        public RelayCommand NewItem
        {
            get { return _newItem ?? (_newItem = new RelayCommand(o => OnNewItem(), o => CanNewItem())); }
        }

        void OnNewItem()
        {
            ItemViewModel = new ItemViewModel(new Item());
            ItemViewModel.ObjectSaved += NewItemSaved;
        }
        
        bool CanNewItem()
        {
            return true;
        }

        #endregion NewItem

        void NewItemSaved(object sender, EventArgs eventArgs)
        {
            var vm = sender as ItemViewModel;
            if (vm == null)
                return;

            ItemFinderViewModel.Collection.Add(vm);
            ItemFinderViewModel.SuggestRefresh();
            vm.ObjectSaved -= NewItemSaved;
            CollectionHelper.RegisterNewItem(vm.DomainObject);
        }

    }
}
