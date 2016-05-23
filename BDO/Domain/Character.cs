using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDO.Domain.Interfaces;
using NTC.NHIB.DomainModel;

namespace BDO.Domain
{
    public class Character : BdoDomainObject, IValidatesSave
    {
        public virtual string Name { get; set; }
        public virtual int Level { get; set; }
        
        public virtual int AlchemyLevel { get; set; }
        public virtual int CookingLevel { get; set; }
        public virtual int ProcessingLevel { get; set; }
        public virtual int GatheringLevel { get; set; }
        public virtual int FishingLevel { get; set; }

        public virtual int AlchemyProgress { get; set; }
        public virtual int CookingProgress { get; set; }
        public virtual int ProcessingProgress { get; set; }
        public virtual int GatheringProgress { get; set; }
        public virtual int FishingProgress { get; set; }


        public virtual ValidationResult ValidateSave()
        {
            //todo: update save validation on character class
            return new ValidationResult() {IsValid = true};
        }
    }
}
