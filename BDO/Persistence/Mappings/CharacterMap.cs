using BDO.Domain;

namespace BDO.Persistence.Mappings
{
    public class CharacterMap : DomainMap<Character>
    {
        public CharacterMap()
        {
            Map(p => p.Name);
            Map(p => p.Level);

            Map(p => p.AlchemyLevel);
            Map(p => p.CookingLevel);
            Map(p => p.ProcessingLevel);
            Map(p => p.GatheringLevel);
            Map(p => p.FishingLevel);

            Map(p => p.AlchemyProgress);
            Map(p => p.CookingProgress);
            Map(p => p.ProcessingProgress);
            Map(p => p.GatheringProgress);
            Map(p => p.FishingProgress);
        }
    }
}