using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using BDO.MarketScraper.Img;

namespace BDO.MarketScraper
{
    public class WorkUnit
    {
        Action<WorkUnit> _startAction;
        bool _started;
        
        internal WorkUnit(Action<WorkUnit> startAction)
        {
            _startAction = startAction;
            Items = new ReadOnlyCollection<ItemAnalysis>(InternalItems);
            InvalidItems = new ReadOnlyCollection<ItemAnalysis>(InternalInvalidItems);
            ValidItems = new ReadOnlyCollection<ItemAnalysis>(InternalValidItems);
            NotFoundInDatabase = new ReadOnlyCollection<ItemAnalysis>(InternalNotFoundInDatabase);
            SavedToDatabase = new ReadOnlyCollection<ItemAnalysis>(InternalSavedToDatabase);
        }

        public event EventHandler DecompositionComplete;
        public event EventHandler ImageSavingComplete;
        public event EventHandler OcrComplete;
        public event EventHandler DatabaseOperationsComplete;
        public event EventHandler Complete;

        internal Bitmap Bitmap { get; set; }
        internal List<ItemAnalysis> InternalItems { get; set; } = new List<ItemAnalysis>();
        internal List<ItemAnalysis> InternalValidItems { get; set; } = new List<ItemAnalysis>();
        internal List<ItemAnalysis> InternalInvalidItems { get; set; } = new List<ItemAnalysis>();
        internal List<ItemAnalysis> InternalNotFoundInDatabase { get; set; } = new List<ItemAnalysis>();
        internal List<ItemAnalysis> InternalSavedToDatabase { get; set; } = new List<ItemAnalysis>();

        public string ID { get; internal set; }
        public int IntId { get; internal set; }
        public MarketScreen MarketScreen { get; internal set; }
        public ReadOnlyCollection<ItemAnalysis> Items { get; } 
        public ReadOnlyCollection<ItemAnalysis> ValidItems { get; } 
        public ReadOnlyCollection<ItemAnalysis> InvalidItems { get; } 
        public ReadOnlyCollection<ItemAnalysis> NotFoundInDatabase { get; } 
        public ReadOnlyCollection<ItemAnalysis> SavedToDatabase { get; } 

        internal void InvokeDecompComplete()
        {
            DecompositionComplete?.Invoke(this, new EventArgs());
        }

        internal void InvokeImageSaveComplete()
        {
            ImageSavingComplete?.Invoke(this, new EventArgs());
        }

        internal void InvokeOcrComplete()
        {
            OcrComplete?.Invoke(this, new EventArgs());
        }

        internal void InvokeDatabaseOpsComplete()
        {
            DatabaseOperationsComplete?.Invoke(this, new EventArgs());
        }

        internal void InvokeComplete()
        {
            Complete?.Invoke(this, new EventArgs());
            Bitmap.Dispose();
        }

        public void Start()
        {
            if (_started)
                throw new InvalidOperationException();
            _startAction.Invoke(this);
        }
    }
}