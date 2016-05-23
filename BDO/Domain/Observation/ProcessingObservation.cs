using System;
using BDO.Domain.Interfaces;

namespace BDO.Domain.Observation
{
    public class ProcessingObservation : RecipeEnergyObservation, IValidatesSave
    {
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime EndTime { get; set; }

        public override double TotalTimeInSeconds
        {
            get { return (EndTime - StartTime).TotalSeconds; }
            set { }
        }

        public virtual ValidationResult ValidateSave()
        {
            return new ValidationResult() {IsValid = true};
        }
    }
}