using System.Collections.Generic;
using BDO.Domain;
using BDO.WPF.V.Win;
using BDO.WPF.VM.Base;
using BDO.WPF.VM.Domain;

namespace BDO.WPF.VM.Collections
{
    class RecipeCollectionViewModel : DomainObjectCollectionViewModel<RecipeViewModel, Recipe>
    {
        public RecipeCollectionViewModel(IList<Recipe> recipes)
            : base(
                recipes,
                DomainObjectRepositories.RecipeRepository,
                recipe => new RecipeViewModel(
                    recipe),
                () => new NewRecipeWindow())
        {
        }

        public RecipeCollectionViewModel(IList<Recipe> recipes, Item fromItem)
            : base(
                recipes, 
                DomainObjectRepositories.RecipeRepository,
                recipe =>
                {
                    recipe.Result = fromItem;
                    return new RecipeViewModel(
                        recipe);
                },
                () => new NewRecipeWindow())
        {
        }


    }
}