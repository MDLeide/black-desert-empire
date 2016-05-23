using System.Collections.Generic;
using BDO.Domain.Enum;
using NTC.NHIB.DomainModel;

namespace BDO.Domain.Nodes
{
    public class Workshop : BdoDomainObject
    {
        public House House { get; set; }
        public List<WorkshopLevel> Levels { get; set; } = new List<WorkshopLevel>();
        public WorkshopType Type { get; set; }
        /// <summary>
        /// Indicates that this workshop has been purchased.
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Indiciates the highest level purchased.
        /// </summary>
        public int ActiveLevel { get; set; }
    }
}