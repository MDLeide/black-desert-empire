using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BDO.Domain;
using BDO.Domain.Enum;
using BDO.Domain.Observation;
using BDO.Persistence.Repo;
using BDO.WPF.V.Win;
using BDO.WPF.VM.Base;
using BDO.WPF.VM.Collections;
using BDO.WPF.VM.Finders;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM.Domain
{
    class RecipeViewModel : DomainObjectViewModel<Recipe>
    {
        bool _canChooseItem;
        ItemQuantityPair _selectedMaterial;
        ObservableCollection<ItemQuantityPair> _materials;


        public RecipeViewModel(Recipe recipe)
            : base(recipe, DomainObjectRepositories.RecipeRepository)
        {
            _canChooseItem = recipe.Result == null;
            
            SecondaryResults = new ItemCollectionViewModel(recipe.SecondaryResults);

            SecondaryResults.CanAddNew = false;

            Materials =
                new ObservableCollection<ItemQuantityPair>(
                    recipe.Materials.Select(
                        p =>
                            new ItemQuantityPair()
                            {
                                Item = new ItemViewModel(p.Key),
                                Quantity = p.Value
                            }));

            ObjectSaved += (s, e) => _canChooseItem = false;
            AllRecipeTypes = CollectionHelper.RecipeTypes;
        }

        ObservableCollection<RecipeType> _allRecipeTypes;

        public ObservableCollection<RecipeType> AllRecipeTypes
        {
            get { return _allRecipeTypes; }
            set
            {
                if (Equals(value, _allRecipeTypes)) return;
                _allRecipeTypes = value;
                OnPropertyChanged(nameof(AllRecipeTypes));
            }
        }

        public RecipeType Type
        {
            get { return DomainObject.Type; }
            set
            {
                if (Equals(value, DomainObject.Type)) return;
                DomainObject.Type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public ItemCollectionViewModel SecondaryResults { get; set; }
        
        public ItemQuantityPair SelectedMaterial
        {
            get { return _selectedMaterial; }
            set
            {
                if (Equals(value, _selectedMaterial)) return;
                _selectedMaterial = value;
                OnPropertyChanged(nameof(SelectedMaterial));
            }
        }

        public ObservableCollection<ItemQuantityPair> Materials
        {
            get { return _materials; }
            set
            {
                if (Equals(value, _materials)) return;
                _materials = value;
                OnPropertyChanged(nameof(Materials));
            }
        }

        public double ExpectedYield
        {
            get { return DomainObject.ExpectedYield; }
            set
            {
                if (Equals(value, DomainObject.ExpectedYield)) return;
                DomainObject.ExpectedYield = value;
                OnPropertyChanged(nameof(ExpectedYield));
            }
        }


        #region ChooseItem

        RelayCommand _chooseItem;

        public RelayCommand ChooseItem
        {
            get { return _chooseItem ?? (_chooseItem = new RelayCommand(o => OnChooseItem(), p => CanChooseItem())); }
        }

        void OnChooseItem()
        {
            var item = GetItemHelper.GetItem();
            if (item != null)
                DomainObject.Result = item;
        }

        bool CanChooseItem()
        {
            return _canChooseItem;
        }

        #endregion

        #region AddSecondaryResult

        RelayCommand _addSecondaryResult;

        public RelayCommand AddSecondaryResult
        {
            get { return _addSecondaryResult ?? (_addSecondaryResult = new RelayCommand(o => OnAddSecondaryResult(), o => CanAddSecondaryResult())); }
        }

        void OnAddSecondaryResult()
        {
            var vm = new ItemFinderViewModel();
            vm.ItemSelected += (s, e) => SecondaryResults.Collection.Add(vm.SelectedObject);
            var w = new ItemFinderWindow();
            w.DataContext = vm;
            vm.Canceled += (s, e) => w.Close();
            w.ShowDialog();
        }

        bool CanAddSecondaryResult()
        {
            return true;
        }

        #endregion AddSecondaryResult

        #region RemoveSecondaryResult

        RelayCommand _removeSecondaryResult;

        public RelayCommand RemoveSecondaryResult
        {
            get { return _removeSecondaryResult ?? (_removeSecondaryResult = new RelayCommand(o => OnRemoveSecondaryResult(), o => CanRemoveSecondaryResult())); }
        }

        void OnRemoveSecondaryResult()
        {
            SecondaryResults.Collection.Remove(SecondaryResults.SelectedObject);
        }

        bool CanRemoveSecondaryResult()
        {
            return SecondaryResults.SelectedObject != null;
        }

        #endregion RemoveSecondaryResult

        #region AddMaterial

        RelayCommand _addMaterial;

        public RelayCommand AddMaterial
        {
            get { return _addMaterial ?? (_addMaterial = new RelayCommand(o => OnAddMaterial(), o => CanAddMaterial())); }
        }

        void OnAddMaterial()
        {
            var item = GetItemHelper.GetItem();
            if (item == null)
                return;
            if (Materials.Any(p => p.Item.DomainObject == item))
                return;
            Materials.Add(
                new ItemQuantityPair()
                {
                    Item = new ItemViewModel(item), Quantity = 1
                });
        }

        bool CanAddMaterial()
        {
            return true;
        }

        #endregion AddMaterial

        #region RemoveMaterial

        RelayCommand _removeMaterial;

        public RelayCommand RemoveMaterial
        {
            get { return _removeMaterial ?? (_removeMaterial = new RelayCommand(o => OnRemoveMaterial(), o => CanRemoveMaterial())); }
        }

        void OnRemoveMaterial()
        {
            Materials.Remove(SelectedMaterial);
            SelectedMaterial = null;
        }

        bool CanRemoveMaterial()
        {
            return SelectedMaterial != null;
        }

        #endregion RemoveMaterial

        #region MakeObservation

        RelayCommand _makeObservation;

        public RelayCommand MakeObservation
        {
            get { return _makeObservation ?? (_makeObservation = new RelayCommand(o => OnMakeObservation(), o => CanMakeObservation())); }
        }

        void OnMakeObservation()
        {
            //todo: update for other crafting types
            //if (DomainObject.Type == RecipeType.Processing)
            //{
                var obs = new ProcessingObservation();
                obs.Recipe = DomainObject;
                var vm = new ProcessingObservationViewModel(obs);
                var v = new NewProcessingObservationWindow();
                v.DataContext = vm;
                vm.ObjectSaved += (s, e) => v.Close();
                v.Show();
            //}
            //else
            //{
            //    MessageBox.Show("Only processing observations are currently supported.", "Not Yet Implemented.");
            //}
        }

        bool CanMakeObservation()
        {
            return true;
        }

        #endregion MakeObservation


        protected override void OnSave()
        {
            DomainObject.Materials.Clear();
            foreach (var m in Materials)
                DomainObject.Materials.Add(m.Item.DomainObject, m.Quantity);

            base.OnSave();
        }
    }
}