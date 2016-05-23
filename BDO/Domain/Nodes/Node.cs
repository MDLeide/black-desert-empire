using System.Collections.Generic;
using NTC.NHIB.DomainModel;

namespace BDO.Domain.Nodes
{
    public class Node : BdoDomainObject
    {
        public virtual string Name { get; set; }
        public virtual IList<Node> AssociatedNodes { get; set; } = new List<Node>();
        public virtual IList<Node> ConnectedNodes { get; set; } = new List<Node>();
    }
}