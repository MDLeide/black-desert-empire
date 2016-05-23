namespace BDO.Domain.Nodes
{
    public class PopulationCenter : Node
    {
        /// <summary>
        /// The parent city or village that this node's lodging contributes to.
        /// </summary>
        public virtual PopulationCenter Parent { get; set; }
    }
}