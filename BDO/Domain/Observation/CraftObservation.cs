namespace BDO.Domain.Observation
{
    public class CraftObservation : RecipeEnergyObservation
    {
        public virtual double CraftTimeInSeconds { get; set; }

        public override double TotalTimeInSeconds
        {
            get { return CraftTimeInSeconds*Iterations; }
            set { }
        }
    }
}