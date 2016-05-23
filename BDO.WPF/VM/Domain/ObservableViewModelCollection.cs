using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using BDO.Domain.Interfaces;
using BDO.WPF.VM.Base;
using NTC.NHIB.DomainModel;

namespace BDO.WPF.VM.Domain
{
    /// <summary>
    /// Keeps an ObservableCollection in synch with its underlying collection.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    class ObservableViewModelCollection<TModel, TViewModel> : ObservableCollection<TViewModel>
        where TModel : DomainObject, IValidatesSave
        where TViewModel : DomainObjectViewModel<TModel>
        
    {
        readonly IList<TModel> _baseList;

        public ObservableViewModelCollection(IList<TModel> baseList, IEnumerable<TViewModel> viewModelList)
            : base(viewModelList)
        {
            if (baseList == null)
                throw new ArgumentNullException(nameof(baseList));

            _baseList = baseList;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var o in e.NewItems)
                        _baseList.Add((o as TViewModel).DomainObject);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var o in e.OldItems)
                        _baseList.Remove((o as TViewModel).DomainObject);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var o in e.NewItems)
                        _baseList.Add((o as TViewModel).DomainObject);
                    foreach (var o in e.OldItems)
                        _baseList.Remove((o as TViewModel).DomainObject);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.OnCollectionChanged(e);
        }
    }
}