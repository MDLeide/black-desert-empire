namespace BDO.Domain.Observation
{
    public abstract class RecipeEnergyObservation : RecipeObservation
    {
        public virtual int StartingEnergy { get; set; }
        public virtual int EndingEnergy { get; set; }

        public virtual int EnergyRecoveryAmount { get; set; }
        public virtual double EnergyRecoveryIntervalInSeconds { get; set; }

        public virtual double TotalTimeInSeconds { get; set; }
    }
}