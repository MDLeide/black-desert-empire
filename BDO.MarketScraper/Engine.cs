using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BDO.Domain;
using BDO.Domain.Observation;
using BDO.MarketScraper.Img;
using BDO.Persistence.Repo;
using NTC.UTL;
using NTC.UTL.ScreenShotDemo;

namespace BDO.MarketScraper
{
    //todo: get item icons

    public class Engine
    {
        int _counter = 0;

        const string ProcessName64 = "BlackDesert64";
        const string ProcessName32 = "BlackDesert32";

        IntPtr _windowHandle = IntPtr.Zero;

        bool _autoSnapRunning;
        int _autoSnapDelay = 0;
        Thread _autoSnapThread;
        int _autoSnapWorkerCount = 0;
        List<WorkUnit> _autoWorkUnits = new List<WorkUnit>();

        ImageAnalyzer _imageAnalyzer;
        ItemRepository _itemRepository;
        MarketObservationRepository _marketObservationRepository;
        ObservableCollection<string> _operations = new ObservableCollection<string>();
        int _idCounter = 1;


        object _autoSnapLock = new object();
        object _counterLock = new object();
        object _obsLock = new object();


        public Engine(string tessDataPath, ItemRepository itemRepository, MarketObservationRepository marketObservationRepository)
        {
            _imageAnalyzer = new ImageAnalyzer(tessDataPath);
            _itemRepository = itemRepository;
            _marketObservationRepository = marketObservationRepository;
            Operations = new ReadOnlyObservableCollection<string>(_operations);
            ScreenCapture.Use4kHack = false;
        }

        public bool Is64Bit { get; set; } = true;

        public double UIScale { get; set; }

        /// <summary>
        /// Keeps a bitmap object of the item's icon. Uses a lot of memory.
        /// </summary>
        public bool KeepIconBitmapWithAnalysis { get; set; }

        /// <summary>
        /// Saves a bitmap of the item's icon to disk and keeps a reference to the file.
        /// </summary>
        public bool SaveIconsToDisk { get; set; }

        public bool SaveScreenshots { get; set; }
        public string ScreenshotDirectory { get; set; }

        public bool SaveDecomposedImages { get; set; }
        public string DecomposedImageDirectory { get; set; }

        public bool LogObservationsToFile { get; set; }
        public string ObservationLogFile { get; set; }

        public bool AutoAddNewItemsToDatabase { get; set; }
        public bool SaveObservationsToDatabase { get; set; }

        public bool RequireHighest { get; set; }
        public bool RequirestLowest { get; set; }
        public bool RequireMarketPrice { get; set; }
        public bool RequireLastSalesPrice { get; set; }
        public bool RequireTotalTrades { get; set; }
        public bool RequireCurrentListings { get; set; }

        /// <summary>
        /// The span of time between now and the most recent market observation required to 
        /// save a new observation to the database.
        /// </summary>
        public TimeSpan ObservationSaveThreshold { get; set; } = new TimeSpan(0, 30, 0);

        public ReadOnlyObservableCollection<string> Operations { get; }

        public bool BeepOnAutosnap { get; set; } = true;

        public double MinimumConfidence
        {
            get { return _imageAnalyzer.MinimumConfidence; }
            set { _imageAnalyzer.MinimumConfidence = value; }
        }



        public void StartAutoSnapping(int msBetweenScreenshots)
        {
            if (_autoSnapRunning)
                throw new InvalidOperationException();

            _autoWorkUnits.Clear();
            _autoSnapDelay = msBetweenScreenshots;
            _autoSnapRunning = true;
            _autoSnapThread = new Thread(AutoSnap);
            _autoSnapThread.Start();
        }

        public IEnumerable<WorkUnit> StopAutoSnapping()
        {
            if (!_autoSnapRunning)
                throw new InvalidOperationException();
            _autoSnapRunning = false;
            _autoSnapThread.Join();
            while (_autoSnapWorkerCount > 0)
                Thread.Sleep(10);
            return _autoWorkUnits;
        }

        public WorkUnit ScreenshotAndProcess(bool startWork)
        {
            return ScreenshotAndProcess(false, startWork);
        }

        public WorkUnit ProcessImage(Bitmap screenShot, bool startWork = false)
        {
            return ProcessImage(screenShot, $"BMP-{_idCounter}", _idCounter++, startWork);
        }

        public void SaveToDatabase(ItemAnalysis itemAnalysis)
        {
            var item = _itemRepository.GetByName(itemAnalysis.ItemName).FirstOrDefault();
            if (item == null)
            {
                if (AutoAddNewItemsToDatabase)
                {
                    item = new Item();
                    item.Name = itemAnalysis.ItemName;
                    _itemRepository.Save(item);
                }
            }

            if (item != null)
                SaveObservation(itemAnalysis, item);
        }


        void AutoSnap()
        {
            while (_autoSnapRunning)
            {
                var worker = ScreenshotAndProcess(true, false);
                worker.Complete += (s, e) =>
                {
                    lock (_autoSnapLock)
                    {
                        _autoSnapWorkerCount--;
                    }
                };

                lock (_autoSnapLock)
                {
                    _autoSnapWorkerCount++;
                }

                _autoWorkUnits.Add(worker);
                worker.Start();

                if (BeepOnAutosnap)
                    Console.Beep();

                Thread.Sleep(_autoSnapDelay);
            }
        }

        WorkUnit ScreenshotAndProcess(bool autoSnap, bool startWork)
        {
            Bitmap image;
            if (_windowHandle == IntPtr.Zero)
            {
                var pName = Is64Bit ? ProcessName64 : ProcessName32;
                image = (Bitmap) ScreenCapture.CaptureWindowByProcess(pName, out _windowHandle);
            }
            else
            {
                image = (Bitmap) ScreenCapture.CaptureWindow(_windowHandle);
            }

            return ProcessImage(image, $"{(autoSnap ? "AUTO" : "SS")}-{_idCounter}", _idCounter++, startWork);
        }
        
        WorkUnit ProcessImage(Bitmap screenShot, string id, int intId, bool startWork)
        {
            var workUnit = new WorkUnit(unit =>
            {
                var t = new Thread(o => Process(o as WorkUnit));
                t.Start(unit);
            });

            workUnit.ID = id;
            workUnit.IntId = intId;
            workUnit.Bitmap = screenShot;

            if (startWork)
                workUnit.Start();
            return workUnit;
        }
        
        void Process(WorkUnit workUnit)
        {
            var ssName = GetScreenshotName();
            if (SaveScreenshots)
            {
                workUnit.Bitmap.Save(Path.Combine(ScreenshotDirectory, ssName + ".png"));
            }

            var marketScreen = ImageDecomposer.GetMarketScreen(workUnit.Bitmap, UIScale, workUnit.Bitmap.Width,
                workUnit.Bitmap.Height);
            workUnit.MarketScreen = marketScreen;
            workUnit.InvokeDecompComplete();

            if (SaveDecomposedImages)
                SaveImages(ssName, marketScreen);
            
            if (SaveScreenshots || SaveDecomposedImages)
                workUnit.InvokeImageSaveComplete();

            List<ItemAnalysis> analysis = new List<ItemAnalysis>();
            
            foreach (var l in marketScreen.ItemListings)
            {
                var a = _imageAnalyzer.Analyze(l);
                if (!KeepIconBitmapWithAnalysis)
                    a.Icon.Dispose();
                analysis.Add(a);
            }

            workUnit.InternalItems.AddRange(analysis);
            workUnit.InvokeOcrComplete();

            foreach (var a in analysis)
                if (ItemPasses(a))
                    workUnit.InternalValidItems.Add(a);
                else
                    workUnit.InternalInvalidItems.Add(a);

            if (LogObservationsToFile)
            {
                LogObservation(workUnit.ValidItems, true);
                LogObservation(workUnit.InvalidItems, false);
            }

            if (AutoAddNewItemsToDatabase || SaveObservationsToDatabase)
            {
                ProcessAnalysis(workUnit.ValidItems, workUnit);
                workUnit.InvokeDatabaseOpsComplete();
            }

            workUnit.Bitmap.Dispose();
            workUnit.InvokeComplete();
        }

        void LogObservation(IEnumerable<ItemAnalysis> analysis, bool valid)
        {
            lock (_obsLock)
            {
                var sb = new StringBuilder();

                foreach (var a in analysis)
                {
                    sb.AppendLine(
                        $"{a.ItemName}\t{(valid ? "Valid" : "Invalid")}\t{a.Highest}\t{a.Lowest}\t{a.MarketPrice}\t{a.LastPrice}\t{a.TotalTrades}\t{a.CurrentListings}");
                }

                using (var sw = new StreamWriter(ObservationLogFile, true))
                    sw.Write(sb.ToString());
            }
        }

        void ProcessAnalysis(IEnumerable<ItemAnalysis> analysis, WorkUnit workUnit)
        {
            foreach (var a in analysis)
            {
                var item = _itemRepository.GetByName(a.ItemName).FirstOrDefault();
                if (item == null)
                {
                    workUnit.InternalNotFoundInDatabase.Add(a);
                    if (AutoAddNewItemsToDatabase)
                    {
                        item = new Item();
                        item.Name = a.ItemName;
                        _itemRepository.Save(item);
                    }
                }

                if (SaveObservationsToDatabase && item != null)
                {
                    if (SaveObservation(a, item ))
                        workUnit.InternalSavedToDatabase.Add(a);
                }
            }
        }

        bool SaveObservation(ItemAnalysis analysis, Item item)
        {
            var recentObs = _marketObservationRepository.GetByItemAndDate(item, DateTime.Now - ObservationSaveThreshold,
                        DateTime.Today.AddDays(1));

            if (recentObs.Any())
                return false;

            var obs = new MarketObservation();
            obs.Item = item;
            obs.EntryTime = DateTime.Now;
            obs.High = analysis.Highest;
            obs.Low = analysis.Lowest;

            obs.LastSalePrice = analysis.LastPrice;
            obs.Price = analysis.MarketPrice;

            obs.TotalTrades = analysis.TotalTrades;
            obs.UnitsOnMarket = analysis.CurrentListings;

            _marketObservationRepository.Save(obs);
            return true;
        }

        void SaveImages(string ssName, MarketScreen marketScreen)
        {
            var dir = new DirectoryInfo(Path.Combine(DecomposedImageDirectory, ssName));
            if (!dir.Exists)
                dir.Create();
            var queue = new Queue<ImageRegion>();
            queue.Enqueue(marketScreen.FullMarketScreen);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var c in current.ChildRegions)
                    queue.Enqueue(c);

                current.GetColorData().Save(Path.Combine(dir.FullName, current.RegionName + ".png"));
            }
        }

        bool ItemPasses(ItemAnalysis itemAnalysis)
        {
            if (string.IsNullOrEmpty(itemAnalysis.ItemName))
                return false;

            if (itemAnalysis.ItemNameConfidence < MinimumConfidence)
                return false;

            if (itemAnalysis.HighestConfidence < MinimumConfidence && RequireHighest)
                return false;

            if (itemAnalysis.LowestConfidence < MinimumConfidence && RequirestLowest)
                return false;

            if (itemAnalysis.MarketPriceConfidence < MinimumConfidence && RequireMarketPrice)
                return false;

            if (itemAnalysis.LastPriceConfidence < MinimumConfidence && RequireLastSalesPrice)
                return false;

            if (itemAnalysis.TotalTradesConfidence < MinimumConfidence && RequireTotalTrades)
                return false;

            if (itemAnalysis.CurrentListingsConfidence < MinimumConfidence && RequireCurrentListings)
                return false;

            return true;
        }

        string GetScreenshotName()
        {
            return DateTime.Now.ToFileSafe();
        }
    }
}
