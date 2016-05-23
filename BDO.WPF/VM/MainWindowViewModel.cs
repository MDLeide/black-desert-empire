using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BDO.Analysis;
using BDO.Domain;
using BDO.Domain.Enum;
using BDO.Persistence.Config;
using BDO.Persistence.Repo;
using BDO.WPF.V;
using BDO.WPF.V.Win;
using BDO.WPF.VM.Collections;
using BDO.WPF.VM.Domain;
using BDO.WPF.VM.Finders;
using BDO.WPF.VM.Screens;
using NTC.WPF.MVVM;
using EnergyTimer = BDO.WPF.Controls.EnergyTimer;

namespace BDO.WPF.VM
{
    class MainWindowViewModel : ViewModelBase
    {
        string _systemMessage;
        ViewModelBase _activeViewModel;
        ItemScreenViewModel _itemScreen;
        ShoppingListScreenViewModel _shoppingListScreen;

        bool _characterIsActive;
        CharacterWindow _characterWindow;

        bool _marketScraperIsActive;
        ScraperWindow _marketScraperWindow;

        bool _energyTimerIsActive;
        EnergyTimer _energyTimer;

        bool _hotbarIsActive;
        HotBarWindow _hotbar;

        bool _actionListIsActive;
        AllItemsWindow _actionWindow;

        public MainWindowViewModel()
        {
            _energyTimer = new EnergyTimer();
            
            MessageLog.GetLog().MessageAdded += (s, e) => SystemMessage = MessageLog.GetLog().MostRecentMessage;
            
            _itemScreen = new ItemScreenViewModel();
            _shoppingListScreen = new ShoppingListScreenViewModel();

            ActiveViewModel = _itemScreen;

            _hotbar = new HotBarWindow();
            _hotbar.DataContext = this;
            _hotbar.Show();
            _hotbarIsActive = true;
        }
        
        public string SystemMessage
        {
            get { return _systemMessage; }
            set
            {
                if (Equals(value, _systemMessage)) return;
                _systemMessage = value;
                OnPropertyChanged(nameof(SystemMessage));
            }
        }
        
        public ViewModelBase ActiveViewModel
        {
            get { return _activeViewModel; }
            set
            {
                if (Equals(value, _activeViewModel)) return;
                _activeViewModel = value;
                OnPropertyChanged(nameof(ActiveViewModel));
            }
        }
        
        public bool EnergyTimerIsActive
        {
            get { return _energyTimerIsActive; }
            set
            {
                if (Equals(value, _energyTimerIsActive)) return;
                _energyTimerIsActive = value;
                OnPropertyChanged(nameof(EnergyTimerIsActive));
            }
        }
        
        public bool HotbarIsActive
        {
            get { return _hotbarIsActive; }
            set
            {
                if (Equals(value, _hotbarIsActive)) return;
                _hotbarIsActive = value;
                OnPropertyChanged(nameof(HotbarIsActive));
            }
        }
        
        public bool CharacterIsActive
        {
            get { return _characterIsActive; }
            set
            {
                if (Equals(value, _characterIsActive)) return;
                _characterIsActive = value;
                OnPropertyChanged(nameof(CharacterIsActive));
            }
        }
        
        public bool MarketScraperIsActive
        {
            get { return _marketScraperIsActive; }
            set
            {
                if (Equals(value, _marketScraperIsActive)) return;
                _marketScraperIsActive = value;
                OnPropertyChanged(nameof(MarketScraperIsActive));
            }
        }
        
        public bool ActionListIsActive
        {
            get { return _actionListIsActive; }
            set
            {
                if (Equals(value, _actionListIsActive)) return;
                _actionListIsActive = value;
                OnPropertyChanged(nameof(ActionListIsActive));
            }
        }

        #region ManageItems

        RelayCommand _manageItems;

        public RelayCommand ManageItems
        {
            get { return _manageItems ?? (_manageItems = new RelayCommand(o => OnManageItems(), o => CanManageItems())); }
        }

        void OnManageItems()
        {
            ActiveViewModel = _itemScreen;
        }

        bool CanManageItems()
        {
            return _activeViewModel != _itemScreen;
        }

        #endregion ManageItems

        #region ManageLists

        RelayCommand _manageLists;

        public RelayCommand ManageLists
        {
            get { return _manageLists ?? (_manageLists = new RelayCommand(o => OnManageLists(), o => CanManageLists())); }
        }

        void OnManageLists()
        {
            ActiveViewModel = _shoppingListScreen;
        }

        bool CanManageLists()
        {
            return _activeViewModel != _shoppingListScreen;
        }

        #endregion ManageLists


        #region ShowEnergyTimer

        RelayCommand _showEnergyTimer;

        public RelayCommand ShowEnergyTimer
        {
            get { return _showEnergyTimer ?? (_showEnergyTimer = new RelayCommand(o => OnShowEnergyTimer(), p => CanShowEnergyTimer())); }
        }

        void OnShowEnergyTimer()
        {
            if (EnergyTimerIsActive)
            {
                _energyTimer.Hide();
                EnergyTimerIsActive = false;
            }
            else
            {
                _energyTimer.Show();
                EnergyTimerIsActive = true;
            }
        }

        bool CanShowEnergyTimer()
        {
            return true;
        }

        #endregion

        #region ShowHotbar

        RelayCommand _showHotbar;

        public RelayCommand ShowHotbar
        {
            get { return _showHotbar ?? (_showHotbar = new RelayCommand(o => OnShowHotbar(), o => CanShowHotbar())); }
        }

        void OnShowHotbar()
        {
            if (HotbarIsActive)
            {
                _hotbar.Hide();
                HotbarIsActive = false;
            }
            else
            {
                _hotbar.Show();
                HotbarIsActive = true;
            }
        }

        bool CanShowHotbar()
        {
            return true;
        }

        #endregion ShowHotbar

        #region ShowMasterActionList

        RelayCommand _showMasterActionList;

        public RelayCommand ShowMasterActionList
        {
            get { return _showMasterActionList ?? (_showMasterActionList = new RelayCommand(o => OnShowMasterActionList(), p => CanShowMasterActionList())); }
        }

        void OnShowMasterActionList()
        {
            if (_actionWindow == null)
            {
                var finder = new ItemFinderViewModel();
                _actionWindow = new AllItemsWindow();
                _actionWindow.DataContext = finder;
                _actionWindow.Closing += (s, e) =>
                {
                    e.Cancel = true;
                    _actionWindow.Hide();
                    ActionListIsActive = false;
                };
            }

            if (ActionListIsActive)
                _actionWindow.Hide();
            else
                _actionWindow.Show();

            ActionListIsActive = !ActionListIsActive;
        }

        bool CanShowMasterActionList()
        {
            return true;
        }

        #endregion

        #region ManageCharacter

        RelayCommand _manageCharacter;

        public RelayCommand ManageCharacter
        {
            get { return _manageCharacter ?? (_manageCharacter = new RelayCommand(o => OnManageCharacter(), o => CanManageCharacter())); }
        }

        void OnManageCharacter()
        {
            if (_characterWindow == null)
            {
                var vm = new CharacterViewModel(StaticSettings.Character, DomainObjectRepositories.CharacterRepository);
                _characterWindow = new CharacterWindow();
                _characterWindow.DataContext = vm;
                _characterWindow.Closing += (s, e) =>
                {
                    e.Cancel = true;
                    _characterWindow.Hide();
                    CharacterIsActive = false;
                };
            }

            if (CharacterIsActive)
                _characterWindow.Hide();
            else
                _characterWindow.Show();

            CharacterIsActive = !CharacterIsActive;
        }

        bool CanManageCharacter()
        {
            return true;
        }

        #endregion ManageCharacter

        #region ShowProcessingList

        RelayCommand _showProcessingList;

        public RelayCommand ShowProcessingList
        {
            get { return _showProcessingList ?? (_showProcessingList = new RelayCommand(o => OnShowProcessingList(), o => CanShowProcessingList())); }
        }

        void OnShowProcessingList()
        {
            var analyzer = new ProfitabilityAnalyzer();
            var recipes = DomainObjectRepositories.RecipeRepository.Get().Where(p => p.Type == RecipeType.Processing);
            var results = analyzer.AnalyzeProcessing(recipes);
            var finder = new ItemFinderViewModel(new Item[] {});
            finder.OrderByName = false;
            foreach (var i in results.Entries.Select(p => p.Item))
                finder.Collection.Add(new ItemViewModel(i));
            var win = new AllItemsWindow();
            win.DataContext = finder;
            win.Show();
        }

        bool CanShowProcessingList()
        {
            return true;
        }

        #endregion ShowProcessingList


        #region ShowTopTwenty

        RelayCommand _showTopTwenty;

        public RelayCommand ShowTopTwenty
        {
            get { return _showTopTwenty ?? (_showTopTwenty = new RelayCommand(o => OnShowTopTwenty(), o => CanShowTopTwenty())); }
        }

        void OnShowTopTwenty()
        {
            var allItems = CollectionHelper.AllItems;
            var analyzer = new ProfitabilityAnalyzer();
            var results = analyzer.Analyze(allItems);
            var items = results.Entries.Take(20).Select(p => p.Item);
            var finder = new ItemFinderViewModel(new Item[] {});
            finder.OrderByName = false;
            foreach (var i in items)
                finder.Collection.Add(new ItemViewModel(i));
            var win = new AllItemsWindow();
            win.DataContext = finder;
            win.Show();
        }

        bool CanShowTopTwenty()
        {
            return true;
        }

        #endregion ShowTopTwenty

        #region ShowItemList

        RelayCommand _showItemList;

        public RelayCommand ShowItemList
        {
            get { return _showItemList ?? (_showItemList = new RelayCommand(o => OnShowItemList(), o => CanShowItemList())); }
        }

        void OnShowItemList()
        {
            var vm = new ShoppingListFinderViewModel(CollectionHelper.AllShoppingLists);
            var v = new ShoppingListWindow();
            v.DataContext = vm;
            v.Show();
        }

        bool CanShowItemList()
        {
            return true;
        }

        #endregion ShowItemList

        #region MarketScraper

        RelayCommand _marketScraper;

        public RelayCommand MarketScraper
        {
            get { return _marketScraper ?? (_marketScraper = new RelayCommand(o => OnMarketScraper(), o => CanMarketScraper())); }
        }

        void OnMarketScraper()
        {
            if (_marketScraperWindow == null)
            {
                var vm = new MarketScraperViewModel();
                _marketScraperWindow = new ScraperWindow();
                _marketScraperWindow.DataContext = vm;
                _marketScraperWindow.Closing += (sender, args) =>
                {
                    args.Cancel = true;
                    _marketScraperWindow.Hide();
                    MarketScraperIsActive = false;
                };
            }

            if (MarketScraperIsActive)
                _marketScraperWindow.Hide();
            else
                _marketScraperWindow.Show();

            MarketScraperIsActive = !MarketScraperIsActive;
        }

        bool CanMarketScraper()
        {
            return true;
        }

        #endregion MarketScraper


        #region MergeItems

        RelayCommand _mergeItems;

        public RelayCommand MergeItems
        {
            get { return _mergeItems ?? (_mergeItems = new RelayCommand(o => OnMergeItems(), o => CanMergeItems())); }
        }

        void OnMergeItems()
        {
            var vm = new MergeViewModel();
            var v = new MergeWindow();
            v.DataContext = vm;
            v.Show();
        }

        bool CanMergeItems()
        {
            return true;
        }

        #endregion MergeItems

        public void WindowClosing(object sender, EventArgs args)
        {
            _energyTimer.Close();
            DomainObjectRepositories.Dispose();
            Application.Current.Shutdown();
        }
    }
}
