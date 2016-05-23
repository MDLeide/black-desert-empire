using System;
using BDO.Domain;
using BDO.Domain.Interfaces;
using BDO.Persistence.Repo;
using NTC.NHIB.DomainModel;
using NTC.WPF.MVVM;

namespace BDO.WPF.VM.Base
{
    abstract class DomainObjectViewModel<TModel> : ViewModelBase
        where TModel : DomainObject, IValidatesSave
    {
        protected DomainObjectViewModel(TModel obj, Repository<TModel> repository)
        {
            Repository = repository;
            DomainObject = obj;
        }

        public event EventHandler<EventArgs> ObjectDeleted;
        public event EventHandler<EventArgs> ObjectSaved; 

        protected Repository<TModel> Repository { get; }
        
        public TModel DomainObject { get; protected set; }

        #region Save

        RelayCommand _save;

        public RelayCommand Save
        {
            get { return _save ?? (_save = new RelayCommand(o => OnSave(), p => CanSave())); }
        }

        protected virtual void OnSave()
        {
            Repository.Save(DomainObject);
            MessageLog.GetLog().LogMessage($"{DomainObject.GetType().Name} : {DomainObject} saved.");
            ObjectSaved?.Invoke(this, new EventArgs());
        }

        protected virtual bool CanSave()
        {
            return DomainObject.ValidateSave().IsValid;
        }

        #endregion
        
        #region Delete

        RelayCommand _delete;

        public RelayCommand Delete
        {
            get { return _delete ?? (_delete = new RelayCommand(o => OnDelete(), p => CanDelete())); }
        }

        void OnDelete()
        {
            Repository.Delete(DomainObject);
            ObjectDeleted?.Invoke(this, new EventArgs());
        }

        bool CanDelete()
        {
            return true;
        }

        #endregion
    }
}