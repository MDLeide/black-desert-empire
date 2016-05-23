using System.Collections.Generic;
using NTC.NHIB.DomainModel;

namespace BDO.Domain.Nodes
{
    public class WorkshopLevel : BdoDomainObject
    {
        public List<Recipe> Designs { get; set; } = new List<Recipe>();
        public int Level { get; set; }
    }
}