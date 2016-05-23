using System.Collections.Generic;
using NTC.NHIB.DomainModel;

namespace BDO.Domain.Observation
{
    public abstract class RecipeObservation : BdoDomainObject
    {
        public virtual Recipe Recipe { get; set; }

        public virtual int SkillLevel { get; set; } = -1;
        public virtual int Iterations { get; set; } = -1;
        public virtual int Yield { get; set; } = -1;

        public virtual IDictionary<Item, int> AdditionalYield { get; set; } = new Dictionary<Item, int>();
    }
}