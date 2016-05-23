using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BDO.Domain;
using BDO.Domain.Enum;
using BDO.Persistence.Repo;
using BDO.WPF.V.Win;
using BDO.WPF.VM.Base;
using BDO.WPF.VM.Domain;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM.Finders
{
    class ItemFinderViewModel : DomainObjectFilterableCollectionViewModel<ItemViewModel, Item>
    {
        string _categoryFilter;
        bool _filterByMarketCategory;
        MarketCategory _marketCategoryFilter;
        ObservableCollection<MarketCategory> _marketCategories;

        public ItemFinderViewModel(IEnumerable<Item> items)
            : base(
                items.ToList(),
                DomainObjectRepositories.ItemRepository,
                (i) => new ItemViewModel(i),
                () => new NewItemWindow())
        {
            MarketCategories = new ObservableCollection<MarketCategory>();
            foreach (var n in Enum.GetValues(typeof (MarketCategory)))
                MarketCategories.Add((MarketCategory) n);
        }

        public ItemFinderViewModel()
            : this(CollectionHelper.AllItems)
        {
        }


        public event EventHandler<EventArgs> ItemSelected;
        public event EventHandler<EventArgs> Canceled;


        public bool FilterByMarketCategory
        {
            get { return _filterByMarketCategory; }
            set
            {
                if (Equals(value, _filterByMarketCategory)) return;
                _filterByMarketCategory = value;
                OnPropertyChanged(nameof(FilterByMarketCategory));
                DoFilter();
            }
        }

        public ObservableCollection<MarketCategory> MarketCategories
        {
            get { return _marketCategories; }
            set
            {
                if (Equals(value, _marketCategories)) return;
                _marketCategories = value;
                OnPropertyChanged(nameof(MarketCategories));
            }
        }

        public string CategoryFilter
        {
            get { return _categoryFilter; }
            set
            {
                if (Equals(value, _categoryFilter)) return;
                _categoryFilter = value;
                OnPropertyChanged(nameof(CategoryFilter));
                DoFilter();
            }
        }
        
        public MarketCategory MarketCategoryFilter
        {
            get { return _marketCategoryFilter; }
            set
            {
                if (Equals(value, _marketCategoryFilter)) return;
                _marketCategoryFilter = value;
                OnPropertyChanged(nameof(MarketCategoryFilter));
                DoFilter();
            }
        }
        

        protected override void DoFilter()
        {
            base.DoFilter();

            if (!string.IsNullOrEmpty(CategoryFilter) || FilterByMarketCategory)
                FilteredCollection = new ObservableCollection<DomainObjectViewModel<Item>>(
                    FilteredCollection.Where(p =>
                        (string.IsNullOrEmpty(CategoryFilter) ||
                         (!string.IsNullOrEmpty(p.DomainObject.Category) &&
                          p.DomainObject.Category.ToLower().Contains(CategoryFilter.ToLower()))) &&
                        (!FilterByMarketCategory || p.DomainObject.MarketCategory == MarketCategoryFilter))
                        .OrderBy(p => p.DomainObject.Name));

        }

        public void SuggestRefresh()
        {
            DoFilter();
        }
        
        #region Select

        RelayCommand _select;

        public RelayCommand Select
        {
            get { return _select ?? (_select = new RelayCommand(o => OnSelect(), o => CanSelect())); }
        }

        void OnSelect()
        {
            ItemSelected?.Invoke(this, new EventArgs());
        }

        bool CanSelect()
        {
            return SelectedObject != null;
        }

        #endregion Select

        #region Cancel

        RelayCommand _cancel;

        public RelayCommand Cancel
        {
            get { return _cancel ?? (_cancel = new RelayCommand(o => OnCancel(), o => CanCancel())); }
        }

        void OnCancel()
        {
            Canceled?.Invoke(this, new EventArgs());
        }

        bool CanCancel()
        {
            return true;
        }

        #endregion Cancel
    }
}