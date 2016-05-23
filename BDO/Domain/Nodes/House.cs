using NTC.NHIB.DomainModel;

namespace BDO.Domain.Nodes
{
    public class House : BdoDomainObject
    {
        public PopulationCenter Location { get; set; }
        /// <summary>
        /// Indicates that this house has been purchased.
        /// </summary>
        public virtual bool Active { get; set; }
    }
}