using System;
using NTC.NHIB.DomainModel;

namespace BDO.Domain
{
    public abstract class BdoDomainObject : DomainObject
    {
        public virtual event EventHandler ObjectSaved;
         
        public override void Save()
        {
            base.Save();
            ObjectSaved?.Invoke(this, new EventArgs());
        }
    }
}