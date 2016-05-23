using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using BDO.Domain;
using BDO.Domain.Interfaces;
using BDO.Persistence.Repo;
using NTC.NHIB.DomainModel;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM.Base
{
    abstract class DomainObjectCollectionViewModel<TViewModel, TModel> : ViewModelBase
        where TViewModel : DomainObjectViewModel<TModel>
        where TModel : DomainObject, IValidatesSave, new()
    {
        readonly IList<TModel> _objects;
        readonly Func<TModel, TViewModel> _createViewModel;
        readonly Func<Window> _createWindow;
        
        ObservableCollection<TViewModel> _collection;
        TViewModel _selectedObject;
        
        protected DomainObjectCollectionViewModel(IList<TModel> objects, Repository<TModel> repository, Func<TModel, TViewModel> createViewModel, Func<Window> createAddNewWindow )
        {
            _createViewModel = createViewModel;
            _createWindow = createAddNewWindow;
            _objects = objects;

            Repository = repository;
            Collection = new ObservableCollection<TViewModel>();

            foreach (var o in objects)
                Collection.Add(RegisterObject(o));

            Collection.CollectionChanged += CollectionOnCollectionChanged;

            SelectedObject = Collection.FirstOrDefault();
        }

        void CollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (UpdateSourceList)
                switch (notifyCollectionChangedEventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var o in notifyCollectionChangedEventArgs.NewItems)
                            _objects.Add((o as TViewModel)?.DomainObject);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var o in notifyCollectionChangedEventArgs.OldItems)
                            _objects.Remove((o as TViewModel)?.DomainObject);
                        break;
                }
        }

        protected Repository<TModel> Repository { get; }

        public bool ShowNewWindowAsDialog { get; set; }
        public bool UpdateSourceList { get; set; } = true;
        public bool CanAddNew { get; set; } = true;

        public TViewModel SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (Equals(value, _selectedObject)) return;
                _selectedObject = value;
                OnPropertyChanged(nameof(SelectedObject));
            }
        }

        public ObservableCollection<TViewModel> Collection
        {
            get { return _collection; }
            set
            {
                if (Equals(value, _collection)) return;
                _collection = value;
                OnPropertyChanged(nameof(Collection));
            }
        }

        #region Add

        RelayCommand _add;

        public RelayCommand Add
        {
            get { return _add ?? (_add = new RelayCommand(o => OnAdd(), o => CanAdd())); }
        }

        void OnAdd()
        {
            var obj = new TModel();
            var vm = RegisterObject(obj);

            vm.ObjectSaved += DomainObjectSaved;

            var window = _createWindow();
            window.DataContext = vm;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (ShowNewWindowAsDialog)
                window.ShowDialog();
            else
                window.Show();
        }

        bool CanAdd()
        {
            return CanAddNew;
        }

        #endregion Add

        void DomainObjectSaved(object sender, EventArgs args)
        {
            Collection.Add(sender as TViewModel);
            (sender as TViewModel).ObjectSaved -= DomainObjectSaved;
        }

        TViewModel RegisterObject(TModel obj)
        {
            var vm = _createViewModel(obj);
            vm.ObjectDeleted += (s, e) =>
            {
                Collection.Remove(vm);
            };
            return vm;
        }
    }
}