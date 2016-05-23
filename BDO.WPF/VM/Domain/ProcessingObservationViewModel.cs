using System;
using System.Linq;
using System.Windows;
using BDO.Domain;
using BDO.Domain.Observation;
using BDO.Persistence.Repo;
using BDO.WPF.VM.Base;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM.Domain
{
    class ProcessingObservationViewModel : DomainObjectViewModel<ProcessingObservation>
    {
        int _iterations;
        double _average;

        string _itemNameOne;
        string _itemNameTwo;

        int _quantityOne;
        int _quantityTwo;

        int _endingQuantityOne;
        int _endingQuantityTwo;

        bool _started;
        bool _ended;
        
        bool _twoItems;

        Recipe _recipe;

        int _recipeQuantityOne;
        int _recipeQuantityTwo;

        Item _itemOne;
        Item _itemTwo;
        

        public ProcessingObservationViewModel(ProcessingObservation observation)
            : base(observation, DomainObjectRepositories.ProcessingObservationRepository)
        {
            observation.EnergyRecoveryAmount = 1;
            observation.EnergyRecoveryIntervalInSeconds = 180;

            _recipe = observation.Recipe;
            _itemOne = _recipe.Materials.FirstOrDefault().Key;

            RecipeQuantityOne = _recipe.Materials[_itemOne];
            ItemNameOne = _itemOne.Name;
            SecondItemVisibility = Visibility.Hidden;
            
            if (_recipe.Materials.Count == 2)
            {
                _itemTwo = _recipe.Materials.Skip(1).FirstOrDefault().Key;
                RecipeQuantityTwo = _recipe.Materials[_itemTwo];
                ItemNameTwo = _itemTwo.Name;
                TwoItems = true;
                SecondItemVisibility = Visibility.Visible;
            }
        }

        Visibility _secondItemVisibility;

        public Visibility SecondItemVisibility
        {
            get { return _secondItemVisibility; }
            set
            {
                if (Equals(value, _secondItemVisibility)) return;
                _secondItemVisibility = value;
                OnPropertyChanged(nameof(SecondItemVisibility));
            }
        }
        
        public bool TwoItems
        {
            get { return _twoItems; }
            set
            {
                if (Equals(value, _twoItems)) return;
                _twoItems = value;
                OnPropertyChanged(nameof(TwoItems));
            }
        }
        
        public string ItemNameOne
        {
            get { return _itemNameOne; }
            set
            {
                if (Equals(value, _itemNameOne)) return;
                _itemNameOne = value;
                OnPropertyChanged(nameof(ItemNameOne));
            }
        }
        
        public string ItemNameTwo
        {
            get { return _itemNameTwo; }
            set
            {
                if (Equals(value, _itemNameTwo)) return;
                _itemNameTwo = value;
                OnPropertyChanged(nameof(ItemNameTwo));
            }
        }
        
        public int QuantityOne
        {
            get { return _quantityOne; }
            set
            {
                if (Equals(value, _quantityOne)) return;
                _quantityOne = value;
                OnPropertyChanged(nameof(QuantityOne));
                UpdateIterations();
            }
        }   
        
        public int QuantityTwo
        {
            get { return _quantityTwo; }
            set
            {
                if (Equals(value, _quantityTwo)) return;
                _quantityTwo = value;
                OnPropertyChanged(nameof(QuantityTwo));
                UpdateIterations();
            }
        }
        
        public int EndingQuantityOne
        {
            get { return _endingQuantityOne; }
            set
            {
                if (Equals(value, _endingQuantityOne)) return;
                _endingQuantityOne = value;
                OnPropertyChanged(nameof(EndingQuantityOne));
                UpdateIterations();
            }
        }
        
        public int EndingQuantityTwo
        {
            get { return _endingQuantityTwo; }
            set
            {
                if (Equals(value, _endingQuantityTwo)) return;
                _endingQuantityTwo = value;
                OnPropertyChanged(nameof(EndingQuantityTwo));
                UpdateIterations();
            }
        }
        
        public int RecipeQuantityTwo
        {
            get { return _recipeQuantityTwo; }
            private set
            {
                if (Equals(value, _recipeQuantityTwo)) return;
                _recipeQuantityTwo = value;
                OnPropertyChanged(nameof(RecipeQuantityTwo));
            }
        }
        
        public int RecipeQuantityOne
        {
            get { return _recipeQuantityOne; }
            private set
            {
                if (Equals(value, _recipeQuantityOne)) return;
                _recipeQuantityOne = value;
                OnPropertyChanged(nameof(RecipeQuantityOne));
            }
        }

        public int Iterations
        {
            get { return DomainObject.Iterations; }
            set
            {
                if (Equals(value, DomainObject.Iterations)) return;
                DomainObject.Iterations = value;
                OnPropertyChanged(nameof(Iterations));
                UpdateAverage();
            }
        }

        public bool Started
        {
            get { return _started; }
            set
            {
                if (Equals(value, _started)) return;
                _started = value;
                OnPropertyChanged(nameof(Started));
            }
        }

        public bool Ended
        {
            get { return _ended; }
            set
            {
                if (Equals(value, _ended)) return;
                _ended = value;
                OnPropertyChanged(nameof(Ended));
            }
        }
        
        public int Yield
        {
            get { return DomainObject.Yield; }
            set
            {
                if (Equals(value, DomainObject.Yield)) return;
                DomainObject.Yield = value;
                OnPropertyChanged(nameof(Yield));
                UpdateAverage();
            }
        }

        public double Average
        {
            get { return _average; }
            set
            {
                if (Equals(value, _average)) return;
                _average = value;
                OnPropertyChanged(nameof(Average));
            }
        }
        
        public DateTime StartTime
        {
            get { return DomainObject.StartTime; }
            set
            {
                if (Equals(value, DomainObject.StartTime)) return;
                DomainObject.StartTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }
        
        public DateTime EndTime
        {
            get { return DomainObject.EndTime; }
            set
            {
                if (Equals(value, DomainObject.EndTime)) return;
                DomainObject.EndTime = value;
                OnPropertyChanged(nameof(EndTime));
            }
        }

        #region Start

        RelayCommand _start;

        public RelayCommand Start
        {
            get { return _start ?? (_start = new RelayCommand(o => OnStart(), o => CanStart())); }
        }

        void OnStart()
        {
            StartTime = DateTime.Now;
            Started = true;
        }

        bool CanStart()
        {
            return !Started;
        }

        #endregion Start

        #region End

        RelayCommand _end;

        public RelayCommand End
        {
            get { return _end ?? (_end = new RelayCommand(o => OnEnd(), o => CanEnd())); }
        }

        void OnEnd()
        {
            EndTime = DateTime.Now;
            Ended = true;
        }

        bool CanEnd()
        {
            return Started && !Ended;
        }

        #endregion End

        protected override bool CanSave()
        {
            return base.CanSave() && Ended && Iterations > 0 && Yield > 0;
        }

        void UpdateAverage()
        {
            if (Iterations == 0 || Yield == 0)
            {
                Average = 0;
                return;
            }

            Average = (double)Yield/Iterations;
        }

        void UpdateIterations()
        {
            if (QuantityOne == 0)
            {
                Iterations = 0;
                return;
            }

            int iterations = (QuantityOne-EndingQuantityOne)/_recipeQuantityOne;

            if (TwoItems)
            {
                if (QuantityTwo == 0)
                {
                    Iterations = 0;
                    return;
                }

                var second = (QuantityTwo-EndingQuantityTwo)/_recipeQuantityTwo;
                iterations = Math.Min(iterations, second);
            }

            Iterations = iterations;
        }
    }
}
