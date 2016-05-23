using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BDO.Domain;
using BDO.Domain.Interfaces;
using BDO.Persistence.Repo;
using NTC.NHIB.DomainModel;

namespace BDO.WPF.VM.Base
{
    abstract class DomainObjectFilterableCollectionViewModel<TViewModel, TModel>
        : DomainObjectCollectionViewModel<TViewModel, TModel>
        where TViewModel : DomainObjectViewModel<TModel>
        where TModel : DomainObject, IValidatesSave, INamedEntity, new()
    {
        string _filter;
        bool _useAdvancedFilter = true;
        ObservableCollection<DomainObjectViewModel<TModel>> _filteredCollection;

        static class AdvancedFilter
        {
            public static AdvancedFilterResult[] Parse(string input)
            {
                var results = new List<AdvancedFilterResult>();
                if (string.IsNullOrEmpty(input))
                    return new AdvancedFilterResult[] {};

                var currentString = string.Empty;
                var currentType = AdvancedFilterType.Normal;
                
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == '-')
                    {
                        if (!string.IsNullOrEmpty(currentString))
                        {
                            if (currentType == AdvancedFilterType.Normal)
                                results.Add(new AdvancedFilterResult()
                                {
                                    Type = currentType,
                                    Value = currentString
                                });
                            else if (!string.IsNullOrEmpty(currentString.Trim()))
                                results.Add(new AdvancedFilterResult()
                                {
                                    Type = currentType,
                                    Value = currentString.Trim()
                                });

                        }
                        currentString = string.Empty;
                        currentType = AdvancedFilterType.Negate;
                    }
                    else
                    {
                        currentString += input[i];
                    }
                }

                if (currentType == AdvancedFilterType.Normal)
                    results.Add(new AdvancedFilterResult()
                    {
                        Type = currentType,
                        Value = currentString
                    });
                else if (!string.IsNullOrEmpty(currentString.Trim()))
                    results.Add(new AdvancedFilterResult()
                    {
                        Type = currentType,
                        Value = currentString.Trim()
                    });

                return results.ToArray();
            }
        }

        class AdvancedFilterResult
        {
            public AdvancedFilterType Type { get; set; }
            public string Value { get; set; }
        }

        enum AdvancedFilterType
        {
            Normal,
            Negate
        }

        protected DomainObjectFilterableCollectionViewModel(IList<TModel> objects, Repository<TModel> repository,
            Func<TModel, TViewModel> createViewModel,
            Func<Window> newObjectWindowProvider)
            : base(objects, repository, createViewModel, newObjectWindowProvider)
        {
            FilteredCollection = new ObservableCollection<DomainObjectViewModel<TModel>>(
                Collection.OrderBy(p => p.DomainObject.Name));
            Collection.CollectionChanged += (sender, args) => DoFilter();
        }

        public bool OrderByName { get; set; } = true;

        public string Filter
        {
            get { return _filter; }
            set
            {
                if (Equals(value, _filter)) return;
                _filter = value;
                OnPropertyChanged(nameof(Filter));
                DoFilter();
            }
        }
        
        //todo: implement wildcards
        /// <summary>
        /// Gets or sets a value indicating advanced filtering methods should be used. You can use
        /// '*' as a multi char wildcard '?' as a single char wildcard and '-' to negate. [Wildcards not yet implemented].
        /// </summary>
        public bool UseAdvancedFilter
        {
            get { return _useAdvancedFilter; }
            set
            {
                if (Equals(value, _useAdvancedFilter)) return;
                _useAdvancedFilter = value;
                OnPropertyChanged(nameof(UseAdvancedFilter));
            }
        }

        public ObservableCollection<DomainObjectViewModel<TModel>> FilteredCollection
        {
            get { return _filteredCollection; }
            set
            {
                if (Equals(value, _filteredCollection)) return;
                _filteredCollection = value;
                OnPropertyChanged(nameof(FilteredCollection));
            }
        }

        protected virtual void DoFilter()
        {
            FilteredCollection =
                new ObservableCollection<DomainObjectViewModel<TModel>>(DoFilter(Filter, OrderByName, UseAdvancedFilter));

            //if (OrderByName)
            //{
            //    if (string.IsNullOrEmpty(Filter))
            //    {
            //        FilteredCollection = new ObservableCollection<DomainObjectViewModel<TModel>>(
            //            Collection.OrderBy(p => p.DomainObject.Name));
            //    }
            //    else
            //    {
            //        if (UseAdvancedFilter)
            //        {
            //            var terms = AdvancedFilter.Parse(Filter);
            //            var normal = terms.FirstOrDefault(p => p.Type == AdvancedFilterType.Normal);
            //            var negate = terms.Where(p => p.Type == AdvancedFilterType.Negate);
            //        }
            //        else
            //        {
            //            FilteredCollection =
            //                new ObservableCollection<DomainObjectViewModel<TModel>>(
            //                    Collection.Where(p => p.DomainObject.Name.ToLower().Contains(Filter.ToLower()))
            //                        .OrderBy(p => p.DomainObject.Name));
            //        }
            //    }
            //}
            //else
            //{
            //    if (string.IsNullOrEmpty(Filter))
            //    {
            //        FilteredCollection = new ObservableCollection<DomainObjectViewModel<TModel>>(
            //            Collection);
            //    }
            //    else
            //    {
            //        FilteredCollection =
            //            new ObservableCollection<DomainObjectViewModel<TModel>>(
            //                Collection.Where(p => p.DomainObject.Name.ToLower().Contains(Filter.ToLower())));
            //    }
            //}
        }

        IEnumerable<TViewModel> DoFilter(string input, bool order, bool advanced)
        {
            IEnumerable<TViewModel> results;
            if (string.IsNullOrEmpty(input))
                results = Collection;
            else
            {
                if (advanced)
                {
                    var terms = AdvancedFilter.Parse(input);
                    if (!terms.Any())
                        results = Collection;
                    else
                    {
                        var normal = terms.FirstOrDefault(p => p.Type == AdvancedFilterType.Normal);
                        var negate = terms.Where(p => p.Type == AdvancedFilterType.Negate);
                        results = Collection.Where(p =>
                            (normal == null || p.DomainObject.Name.ToLower().Contains(normal.Value.ToLower())) &&
                            negate.All(z => !p.DomainObject.Name.ToLower().Contains(z.Value.ToLower())));
                    }
                }
                else
                {
                    results = Collection.Where(p => p.DomainObject.Name.ToLower().Contains(input.ToLower()));
                }
            }

            if (order)
                results = results.OrderBy(p => p.DomainObject.Name);

            return results;
        }
    }
}