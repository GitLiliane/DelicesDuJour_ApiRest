using DelicesDuJour_ApiRest.Domain.BO;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace DelicesDuJour_ApiRest.Services
{
    public interface IBiblioService
    {
        #region Recettes

        Task<IEnumerable<Recette>> GetAllRecettesAsync();
        Task<Recette> GetRecetteByIdAsync(int id);
        Task<Recette> AddRecetteAsync(Recette newRecette);
        Task<Recette> ModifyRecetteAsync(Recette updateRecette);
        Task<bool> DeleteRecetteAsync(int id);

        #endregion Fin Recettes

        #region Etapes

        Task<IEnumerable<Etape>> GetAllEtapesAsync();
        Task<Etape> GetEtapeByIdAsync(TupleClass<int, int> key);
        Task<Etape> AddEtapeAsync(Etape newEtape);
        Task<Etape> ModifyEtapeAsync(Etape updateEtape);
        Task<bool> DeleteEtapeAsync(TupleClass<int, int> key);

        #endregion Fin Etapes

        #region Catégories

        Task<IEnumerable<Categorie>> GetAllCategoriesAsync();
        Task<Categorie> GetCategorieByIdAsync(int id);
        Task<Categorie> AddCategorieAsync(Categorie newCategorie);
        Task<Categorie> ModifyCategorieAsync(Categorie updateCategorie);
        Task<bool> DeleteCategorieAsync(int id);

        #endregion Fin Catégories

        #region Relations Recettes - Categories

        Task<IEnumerable<RecetteCategorieRelationship>> GetAllRecettesCategoriesAsync();
        Task<bool> AddRecetteCategorieRelationshipAsync(int idCategorie, int idRecette);
        Task<bool> RemoveRecetteCategorieRelationshipAsync(int idCategorie, int idRecette);
        Task<IEnumerable<Recette>> GetRecettesByIdCategorieAsync(int idCategorie);
        Task<IEnumerable<Categorie>> GetCategoriesByIdRecetteAsync(int idCategorie);
        Task<bool> DeleteRecetteRelationsAsync(int idRecette);
        Task<bool> DeleteCategorieRelationsAsync(int idCategorie);

        #endregion Fin Relations Recettes - Categories

        #region Gestion Ingredients

        Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();

        Task<Ingredient> GetIngredientByIdAsync(int id);

        Task<Ingredient> AddIngredientAsync(Ingredient newIngredient);

        Task<Ingredient> ModifyIngredientAsync(Ingredient updateIngredient);

        Task<bool> DeleteIngredientAsync(int id);
        

        #endregion Fin Gestion Ingredients


    }
}
