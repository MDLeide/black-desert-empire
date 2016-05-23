using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BDO.Domain.Observation;
using BDO.Persistence.Config;
using BDO.Persistence.Repo;
using BDO.WPF.VM.Base;
using BDO.WPF.VM.Domain;
using NHibernate;
using NTC.NHIB.DomainModel;

namespace BDO.WPF
{
    static class DomainObjectRepositories
    {
        static RecipeRepository _recipeRepository;
        static ItemRepository _itemRepository;
        static MarketObservationRepository _marketObservationRepository;
        static ProcessingObservationRepository _processingObservationRepository;
        static CharacterRepository _characterRepository;
        static BasicShoppingListRepository _basicShoppingListRepository;
        static ISession _session;
        
        static ISession Session => _session ?? (_session = Configuration.GetSession());

        public static RecipeRepository RecipeRepository 
            => _recipeRepository ?? (_recipeRepository = new RecipeRepository(Session));

        public static ItemRepository ItemRepository
            => _itemRepository ?? (_itemRepository = new ItemRepository(Session));

        public static MarketObservationRepository MarketObservationRepository
            => _marketObservationRepository ?? (_marketObservationRepository = new MarketObservationRepository(Session));

        public static ProcessingObservationRepository ProcessingObservationRepository
            => _processingObservationRepository ?? (_processingObservationRepository = new ProcessingObservationRepository(Session));

        public static CharacterRepository CharacterRepository
            => _characterRepository ?? (_characterRepository = new CharacterRepository(Session));

        public static BasicShoppingListRepository BasicShoppingListRepository
            => _basicShoppingListRepository ?? (_basicShoppingListRepository = new BasicShoppingListRepository(Session));

        public static void Dispose()
        {
            RecipeRepository?.Dispose();
            ItemRepository?.Dispose();
            MarketObservationRepository?.Dispose();
            ProcessingObservationRepository?.Dispose();
            CharacterRepository?.Dispose();
            BasicShoppingListRepository?.Dispose();
        }

    }
}
