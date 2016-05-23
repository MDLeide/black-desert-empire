using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BDO.Domain;
using BDO.Utl;
using BDO.WPF.V.Win;
using BDO.WPF.VM.Finders;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM
{
    class MergeViewModel : ViewModelBase
    {

        Item _selectedSource;

        public Item SelectedSource
        {
            get { return _selectedSource; }
            set
            {
                if (Equals(value, _selectedSource)) return;
                _selectedSource = value;
                OnPropertyChanged(nameof(SelectedSource));
            }
        }

        Item _selectedTarget;

        public Item SelectedTarget
        {
            get { return _selectedTarget; }
            set
            {
                if (Equals(value, _selectedTarget)) return;
                _selectedTarget = value;
                OnPropertyChanged(nameof(SelectedTarget));
            }
        }

        ItemMergeResults _results;

        public ItemMergeResults Results
        {
            get { return _results; }
            set
            {
                if (Equals(value, _results)) return;
                _results = value;
                OnPropertyChanged(nameof(Results));
            }
        }

        #region SelectSource

        RelayCommand _selectSource;

        public RelayCommand SelectSource
        {
            get { return _selectSource ?? (_selectSource = new RelayCommand(o => OnSelectSource(), o => CanSelectSource())); }
        }

        void OnSelectSource()
        {
            SelectedSource = GetItemHelper.GetItem();
        }

        bool CanSelectSource()
        {
            return true;
        }

        #endregion SelectSource

        #region SelectTarget

        RelayCommand _selectTarget;

        public RelayCommand SelectTarget
        {
            get { return _selectTarget ?? (_selectTarget = new RelayCommand(o => OnSelectTarget(), o => CanSelectTarget())); }
        }

        void OnSelectTarget()
        {
            SelectedTarget = GetItemHelper.GetItem();
        }

        bool CanSelectTarget()
        {
            return true;
        }

        #endregion SelectTarget

        #region Merge

        RelayCommand _merge;

        public RelayCommand Merge
        {
            get { return _merge ?? (_merge = new RelayCommand(o => OnMerge(), o => CanMerge())); }
        }

        void OnMerge()
        {
            if (
                !string.Equals(SelectedSource.Name.Trim(), SelectedTarget.Name.Trim(),
                    StringComparison.InvariantCultureIgnoreCase))
            {
                if (MessageBox.Show("The item names do not appear to match. Continue?", "Item Warning",
                    MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }

            var merge = new ItemMerger(
                DomainObjectRepositories.ItemRepository,
                DomainObjectRepositories.MarketObservationRepository,
                DomainObjectRepositories.RecipeRepository,
                DomainObjectRepositories.BasicShoppingListRepository,
                DomainObjectRepositories.ProcessingObservationRepository);

            Results = merge.MergeItems(SelectedSource, SelectedTarget);

            SelectedSource = null;
            SelectedTarget = null;
        }

        bool CanMerge()
        {
            return SelectedSource != null && SelectedTarget != null;
        }

        #endregion Merge
    }
}
