using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using BDO.MarketScraper;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM
{
    class MarketScraperViewModel : ViewModelBase
    {
        Engine _engine;

        bool _saveInProgress;

        public bool SaveInProgress
        {
            get { return _saveInProgress; }
            set
            {
                if (Equals(value, _saveInProgress)) return;
                _saveInProgress = value;
                OnPropertyChanged(nameof(SaveInProgress));
            }
        }

        bool _pendingSave;

        public bool PendingSave
        {
            get { return _pendingSave; }
            set
            {
                if (Equals(value, _pendingSave)) return;
                _pendingSave = value;
                OnPropertyChanged(nameof(PendingSave));
            }
        }

        bool _scrapeActive;
        
        public bool ScrapeActive
        {
            get { return _scrapeActive; }
            set
            {
                if (Equals(value, _scrapeActive)) return;
                _scrapeActive = value;
                OnPropertyChanged(nameof(ScrapeActive));
            }
        }

        bool _stoppingEngine;

        public bool StoppingEngine
        {
            get { return _stoppingEngine; }
            set
            {
                if (Equals(value, _stoppingEngine)) return;
                _stoppingEngine = value;
                OnPropertyChanged(nameof(StoppingEngine));
            }
        }

        ObservableCollection<ItemAnalysisViewModel> _itemAnalyses;

        public ObservableCollection<ItemAnalysisViewModel> ItemAnalyses
        {
            get { return _itemAnalyses; }
            set
            {
                if (Equals(value, _itemAnalyses)) return;
                _itemAnalyses = value;
                OnPropertyChanged(nameof(ItemAnalyses));
            }
        }

        ObservableCollection<ItemAnalysisViewModel> _invalidItems;

        public ObservableCollection<ItemAnalysisViewModel> InvalidItems
        {
            get { return _invalidItems; }
            set
            {
                if (Equals(value, _invalidItems)) return;
                _invalidItems = value;
                OnPropertyChanged(nameof(InvalidItems));
            }
        }

        #region StartScraping

        RelayCommand _startScraping;

        public RelayCommand StartScraping
        {
            get { return _startScraping ?? (_startScraping = new RelayCommand(o => OnStartScraping(), o => CanStartScraping())); }
        }

        void OnStartScraping()
        {
            ScrapeActive = true;
            _engine = BuildEngine();
            _engine.StartAutoSnapping(MarketScraperSettings.DelayBetweenScreenshots);
        }

        bool CanStartScraping()
        {
            return !ScrapeActive && !PendingSave && !SaveInProgress;
        }

        #endregion StartScraping
        
        #region StopScraping

        RelayCommand _stopScraping;

        public RelayCommand StopScraping
        {
            get { return _stopScraping ?? (_stopScraping = new RelayCommand(o => OnStopScraping(), o => CanStopScraping())); }
        }

        void OnStopScraping()
        {
            PendingSave = true;

            var context = TaskScheduler.FromCurrentSynchronizationContext();

            var t = new Thread(() =>
            {
                var workers = _engine.StopAutoSnapping().ToArray();
                var validTasks = new List<Task<ItemAnalysisViewModel>>();
                var invalidTasks = new List<Task<ItemAnalysisViewModel>>();
                

                foreach (var valid in workers.SelectMany(p => p.ValidItems))
                {
                    var vTask = Task<ItemAnalysisViewModel>.Factory.StartNew(() =>
                    {
                        var v = valid;
                        var vm = new ItemAnalysisViewModel(v);
                        return vm;
                    }, CancellationToken.None, TaskCreationOptions.None, context);
                    validTasks.Add(vTask);
                }

                foreach (var invalid in workers.SelectMany(p => p.InvalidItems))
                {
                    var vTask = Task<ItemAnalysisViewModel>.Factory.StartNew(() =>
                    {
                        var v = invalid;
                        var vm = new ItemAnalysisViewModel(v);
                        return vm;
                    }, CancellationToken.None, TaskCreationOptions.None, context);
                    invalidTasks.Add(vTask);
                }

                Task.WaitAll(validTasks.ToArray());
                Task.WaitAll(invalidTasks.ToArray());
                
                var validModels = validTasks.Select(p => p.Result);
                var invalidModels = invalidTasks.Select(p => p.Result);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ItemAnalyses =
                        new ObservableCollection<ItemAnalysisViewModel>(validModels);

                    InvalidItems =
                        new ObservableCollection<ItemAnalysisViewModel>(invalidModels);

                    ScrapeActive = false;
                });
            });

            t.Start();
        }

        bool CanStopScraping()
        {
            return ScrapeActive && !PendingSave;
        }

        #endregion StopScraping
        
        #region SaveToDatabase
        
        RelayCommand _saveToDatabase;

        public RelayCommand SaveToDatabase
        {
            get { return _saveToDatabase ?? (_saveToDatabase = new RelayCommand(o => OnSaveToDatabase(), o => CanSaveToDatabase())); }
        }

        void OnSaveToDatabase()
        {
            var t = new Thread(() =>
            {
                foreach (var i in ItemAnalyses.Where(p => p.Keep))
                {
                    try
                    {
                        _engine.SaveToDatabase(i.Analysis);
                    }
                    catch (Exception e)
                    {
                        MessageLog.GetLog().LogMessage($"Failed to save {i.Analysis.ItemName} auto market observation.");
                    }
                    i.Analysis.Icon.Dispose();
                }

                foreach (var i in ItemAnalyses.Where(p => !p.Keep))
                    i.Analysis.Icon.Dispose();

                foreach (var i in InvalidItems)
                    i.Analysis.Icon.Dispose();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var n in ItemAnalyses.Where(p => p.Keep).Select(p => p.Analysis.ItemName))
                        CollectionHelper.RefreshItemMarketData(n);

                    ItemAnalyses.Clear();
                    InvalidItems.Clear();
                    SaveInProgress = false;
                });
            });
            PendingSave = false;
            SaveInProgress = true;
            t.Start();
        }

        bool CanSaveToDatabase()
        {
            return PendingSave && !SaveInProgress && !ScrapeActive;
        }

        #endregion SaveToDatabase
        
        #region KeepAll

        RelayCommand _keepAll;

        public RelayCommand KeepAll
        {
            get { return _keepAll ?? (_keepAll = new RelayCommand(o => OnKeepAll(), o => CanKeepAll())); }
        }

        void OnKeepAll()
        {
            foreach (var i in ItemAnalyses)
                i.Keep = true;
        }

        bool CanKeepAll()
        {
            return PendingSave && !SaveInProgress & !ScrapeActive;
        }

        #endregion KeepAll
        
        Engine BuildEngine()
        {
            var engine = new Engine(MarketScraperSettings.TessData, DomainObjectRepositories.ItemRepository,
                DomainObjectRepositories.MarketObservationRepository);
            engine = new Engine(MarketScraperSettings.TessData, DomainObjectRepositories.ItemRepository,
                DomainObjectRepositories.MarketObservationRepository);

            engine.AutoAddNewItemsToDatabase = MarketScraperSettings.AutoAddNewItems;
            engine.SaveObservationsToDatabase = false;
            engine.KeepIconBitmapWithAnalysis = true;

            engine.SaveDecomposedImages = MarketScraperSettings.SaveDecomposedImages;
            engine.DecomposedImageDirectory = MarketScraperSettings.DecomposedImageDirectory;

            engine.SaveScreenshots = MarketScraperSettings.SaveScreenShots;
            engine.ScreenshotDirectory = MarketScraperSettings.ScreenShotDirectory;

            engine.BeepOnAutosnap = MarketScraperSettings.BeepOnAutoSnap;

            engine.Is64Bit = MarketScraperSettings.Is64Bit;

            engine.LogObservationsToFile = MarketScraperSettings.LogToFile;
            engine.ObservationLogFile = MarketScraperSettings.LogFile;


            engine.ObservationSaveThreshold = MarketScraperSettings.ObservationSaveThreshold;

            engine.MinimumConfidence = MarketScraperSettings.MinimumConfidence;

            engine.RequireHighest = MarketScraperSettings.RequireHighest;
            engine.RequirestLowest = MarketScraperSettings.RequirestLowest;

            engine.RequireLastSalesPrice = MarketScraperSettings.RequireLastSalesPrice;
            engine.RequireMarketPrice = MarketScraperSettings.RequireMarketPrice;

            engine.RequireTotalTrades = MarketScraperSettings.RequireTotalTrades;
            engine.RequireCurrentListings = MarketScraperSettings.RequireCurrentListings;

            engine.UIScale = MarketScraperSettings.UiScale;
            return engine;
        }
    }
}
