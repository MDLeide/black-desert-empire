using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using BDO.Analysis;
using BDO.Domain;
using BDO.Domain.Enum;
using BDO.Persistence.Repo;
using BDO.WPF.VM.Base;
using BDO.WPF.VM.Collections;
using NTC.NHIB.DomainModel;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM.Domain
{
    class ItemViewModel : DomainObjectViewModel<Item>
    {
        bool _needsRecalc;

        public ObservableCollection<MarketCategory> MarketCategories { get; set; }

        RecipeCollectionViewModel _madeFrom;
        RecipeCollectionViewModel _usedIn;
        MarketObservationCollectionViewModel _marketObservationCollection;
        MarketObservationViewModel _mostRecentMarketObservation;

        RecipeViewModel _primarySourceRecipe;
        ItemMetaData _metaData;

        int _delta;
        int _unitCost;
        int _profit;
        int _craftCost;
        int _marketPrice;
        int _netRevenue;
        double _craftUnits;
        UnitPriceMethod _costingMethod;

        bool _transient;

        public ItemViewModel(Item item)
            : base(item, DomainObjectRepositories.ItemRepository)
        {
            _transient = true;
            _metaData = ItemMetaDataProvider.GetMetaData(item);
            //_metaData.Refreshed += (sender, args) => UpdateMetaDataProperties();

            item.ObjectSaved += (sender, args) =>
            {
                UpdateMetaDataProperties();
            };

            UpdateMetaDataProperties();
            
            MadeFrom = new RecipeCollectionViewModel(item.MadeFrom, item);
            foreach (var i in MadeFrom.Collection)
            {
                i.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(UnitCost))
                    {
                        _metaData.Recalculate();
                        UpdateMetaDataProperties();
                    }
                };
            }
            
            UsedIn = new RecipeCollectionViewModel(item.UsedIn);

            var primaryRecipe = item.MadeFrom.FirstOrDefault();
            if (primaryRecipe != null)
                PrimarySourceRecipe = new RecipeViewModel(primaryRecipe);


            MarketObservationCollection = new MarketObservationCollectionViewModel(item.AllMarketData, item);
            MarketObservationCollection.Collection.CollectionChanged += MarketObservationCollection_CollectionChanged;

            if (item.CurrentMarketData != null)
                MostRecentMarketObservation = new MarketObservationViewModel(item.CurrentMarketData);
            else
                MessageLog.GetLog().LogMessage($"Couldn't find any market data for {item.Name}");
            
            MarketCategories = CollectionHelper.MarketCategories;
            _transient = item.Id == Guid.Empty;
        }


        void MarketObservationCollection_CollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
                if (DomainObject.CurrentMarketData != null)
                {
                    MostRecentMarketObservation = new MarketObservationViewModel(DomainObject.CurrentMarketData);
                    //this doesn't actually save the object, despite the incredibly straightforward name of the method
                    DomainObject.Save();
                }
            //Recalculate();
            //RefreshHelper.NotifyChanged(DomainObject);
        }

        public RecipeViewModel PrimarySourceRecipe
        {
            get { return _primarySourceRecipe; }
            set
            {
                if (Equals(value, _primarySourceRecipe)) return;
                _primarySourceRecipe = value;
                OnPropertyChanged(nameof(PrimarySourceRecipe));
            }
        }
        
        public string Name
        {
            get { return DomainObject.Name; }
            set
            {
                if (Equals(value, DomainObject.Name)) return;
                DomainObject.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Category
        {
            get { return DomainObject.Category; }
            set
            {
                if (Equals(value, DomainObject.Category)) return;
                DomainObject.Category = value;
                OnPropertyChanged(nameof(Category));
            }
        }

        public MarketCategory MarketCategory
        {
            get { return DomainObject.MarketCategory; }
            set
            {
                if (Equals(value, DomainObject.MarketCategory)) return;
                DomainObject.MarketCategory = value;
                OnPropertyChanged(nameof(MarketCategory));
            }
        }

        public int VendorCost
        {
            get { return DomainObject.VendorCost; }
            set
            {
                if (Equals(value, DomainObject.VendorCost)) return;
                DomainObject.VendorCost = value;
                OnPropertyChanged(nameof(VendorCost));
                //Recalculate();
                //RefreshHelper.NotifyChanged(DomainObject);
            }
        }

        public bool VendorSells
        {
            get { return DomainObject.VendorSells; }
            set
            {
                if (Equals(value, DomainObject.VendorSells)) return;
                DomainObject.VendorSells = value;
                OnPropertyChanged(nameof(VendorSells));
                //Recalculate();
                //RefreshHelper.NotifyChanged(DomainObject);
            }
        }

        public int SpecialPricing
        {
            get { return DomainObject.SpecialPricing; }
            set
            {
                if (Equals(value, DomainObject.SpecialPricing)) return;
                DomainObject.SpecialPricing = value;
                OnPropertyChanged(nameof(SpecialPricing));
                //Recalculate();
                //RefreshHelper.NotifyChanged(DomainObject);
            }
        }

        public bool HasSpecialPricing
        {
            get { return DomainObject.HasSpecialPricing; }
            set
            {
                if (Equals(value, DomainObject.HasSpecialPricing)) return;
                DomainObject.HasSpecialPricing = value;
                OnPropertyChanged(nameof(HasSpecialPricing));
                //Recalculate();
                //RefreshHelper.NotifyChanged(DomainObject);
            }
        }

        public bool Craft
        {
            get
            {
                return DomainObject.Craft;
            }
            set
            {
                if (Equals(value, DomainObject.Craft)) return;
                DomainObject.Craft = value;
                OnPropertyChanged(nameof(Craft));
                //Recalculate();
                //RefreshHelper.NotifyChanged(DomainObject);
            }
        }

        public UnitPriceMethod CostingMethod
        {
            get { return _costingMethod; }
            set
            {
                if (Equals(value, _costingMethod)) return;
                _costingMethod = value;
                OnPropertyChanged(nameof(CostingMethod));
            }
        }

        public int Delta
        {
            get
            {
                CheckMetaState();
                return _delta;
            }
            set
            {
                if (Equals(value, _delta)) return;
                _delta = value;
                OnPropertyChanged(nameof(Delta));
            }
        }
        
        public int UnitCost
        {
            get
            {
                CheckMetaState();
                return _unitCost;
            }
            set
            {
                if (Equals(value, _unitCost)) return;
                _unitCost = value;
                OnPropertyChanged(nameof(UnitCost));
            }
        }
        
        public int Profit
        {
            get
            {
                CheckMetaState();
                return _profit;
            }
            set
            {
                if (Equals(value, _profit)) return;
                _profit = value;
                OnPropertyChanged(nameof(Profit));
            }
        }
        
        public int CraftCost
        {
            get
            {
                CheckMetaState();
                return _craftCost;
            }
            set
            {
                if (Equals(value, _craftCost)) return;
                _craftCost = value;
                OnPropertyChanged(nameof(CraftCost));
            }
        }
        
        public int MarketPrice
        {
            get
            {
                CheckMetaState();
                return _marketPrice;
            }
            set
            {
                if (Equals(value, _marketPrice)) return;
                _marketPrice = value;
                OnPropertyChanged(nameof(MarketPrice));
            }
        }
        
        public double CraftUnits
        {
            get
            {
                CheckMetaState();
                return _craftUnits;
            }
            set
            {
                if (Equals(value, _craftUnits)) return;
                _craftUnits = value;
                OnPropertyChanged(nameof(CraftUnits));
            }
        }

        public int NetRevenue
        {
            get
            {
                CheckMetaState();
                return _netRevenue;
            }
            set
            {
                if (Equals(value, _netRevenue)) return;
                _netRevenue = value;
                OnPropertyChanged(nameof(NetRevenue));
            }
        }
        
        public MarketObservationViewModel MostRecentMarketObservation
        {
            get { return _mostRecentMarketObservation; }
            set
            {
                if (Equals(value, _mostRecentMarketObservation)) return;
                _mostRecentMarketObservation = value;
                OnPropertyChanged(nameof(MostRecentMarketObservation));
            }
        }

        public MarketObservationCollectionViewModel MarketObservationCollection
        {
            get { return _marketObservationCollection; }
            set
            {
                if (Equals(value, _marketObservationCollection)) return;
                _marketObservationCollection = value;
                OnPropertyChanged(nameof(MarketObservationCollection));
            }
        }
        
        public RecipeCollectionViewModel MadeFrom
        {
            get { return _madeFrom; }
            set
            {
                if (Equals(value, _madeFrom)) return;
                _madeFrom = value;
                OnPropertyChanged(nameof(MadeFrom));
            }
        }

        public RecipeCollectionViewModel UsedIn
        {
            get { return _usedIn; }
            set
            {
                if (Equals(value, _usedIn)) return;
                _usedIn = value;
                OnPropertyChanged(nameof(UsedIn));
            }
        }
        
        public void RefreshMarket()
        {
            _transient = true;
            MessageLog.GetLog().LogMessage($"Refreshing market data for {DomainObject.Name}");
            MarketObservationCollection.Collection.CollectionChanged -= MarketObservationCollection_CollectionChanged;
            MarketObservationCollection = new MarketObservationCollectionViewModel(DomainObject.AllMarketData, DomainObject);
            MarketObservationCollection.Collection.CollectionChanged += MarketObservationCollection_CollectionChanged;

            if (DomainObject.CurrentMarketData != null)
                MostRecentMarketObservation = new MarketObservationViewModel(DomainObject.CurrentMarketData);
            else
                MessageLog.GetLog().LogMessage($"Couldn't find any market data for {DomainObject.Name}");
            UpdateMetaDataProperties();
            _transient = false;
        }

        public void Refresh(DomainObject obj)
        {
            _needsRecalc = true;

            //todo: causes double updates
            OnPropertyChanged(nameof(Delta));
            OnPropertyChanged(nameof(UnitCost));
            OnPropertyChanged(nameof(Profit));
            OnPropertyChanged(nameof(CraftCost));
            OnPropertyChanged(nameof(MarketPrice));
            OnPropertyChanged(nameof(CostingMethod));
            OnPropertyChanged(nameof(NetRevenue));

            if (obj == DomainObject)
                return;

            //RefreshHelper.NotifyChanged(DomainObject);
        }

        void CheckMetaState()
        {
            //if (_needsRecalc)
            //{
            //    _needsRecalc = false;
            //    Recalculate();
            //}
        }
        
        void UpdateMetaDataProperties()
        {
            CraftUnits = _metaData.CraftYield;
            Delta = _metaData.MarketCraftDifference;
            UnitCost = _metaData.UnitCost;
            Profit = _metaData.Profit;
            CraftCost = _metaData.CraftCost;
            MarketPrice = _metaData.MarketPrice;
            CostingMethod = _metaData.BestMethod;
            NetRevenue = _metaData.NetRevenue;
        }

        //void Recalculate()
        //{
        //    _metaData.Recalculate();
        //    UpdateMetaDataProperties();
        //}

        #region AddProcessingObservation

        RelayCommand _addProcessingObservation;

        public RelayCommand AddProcessingObservation
        {
            get { return _addProcessingObservation ?? (_addProcessingObservation = new RelayCommand(o => OnAddProcessingObservation(), o => CanAddProcessingObservation())); }
        }

        void OnAddProcessingObservation()
        {
            if (PrimarySourceRecipe.MakeObservation.CanExecute(this))
                PrimarySourceRecipe.MakeObservation.Execute(this);
        }

        bool CanAddProcessingObservation()
        {
            return PrimarySourceRecipe != null;
        }

        #endregion AddProcessingObservation

        #region AddMarketObservation

        public RelayCommand AddMarketObservation
        {
            get { return MarketObservationCollection.Add; }
        }

        #endregion AddMarketObservation

        protected override void OnSave()
        {
            base.OnSave();
            _transient = false;
        }

        readonly string[] _ignore = 
        {
            nameof(Delta),
            nameof(UnitCost),
            nameof(Profit),
            nameof(CraftCost),
            nameof(MarketPrice),
            nameof(CostingMethod),
            nameof(NetRevenue),
            nameof(CraftUnits)
        };

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (_ignore.Contains(propertyName))
                return;

            if (!_transient && StaticSettings.AutoSave && CanSave())
                Save.Execute(this);
        }
    }
}
