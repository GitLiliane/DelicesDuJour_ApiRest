using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.DTOIn;
using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace DelicesDuJour_ApiRest.Services
{
    public interface IBiblioService
    {
        #region Recettes

        Task<IEnumerable<Recette>> GetAllRecettesAsync();
        Task<Recette> GetRecetteByIdAsync(int id);
        Task<Recette> AddRecetteAsync(Recette newRecette, IFormFile? photoFile);
        Task<Recette> ModifyRecetteAsync(Recette updateRecette);
        Task<bool> DeleteRecetteAsync(int id);

        #endregion Fin Recettes

        #region Etapes

        Task<IEnumerable<Etape>> GetAllEtapesAsync();
        Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int id);
        Task<Etape> AddEtapeAsync(Etape newEtape);
        Task<Etape> ModifyEtapeAsync(Etape updateEtape);
        Task<bool> DeleteEtapeAsync((int, int) key);

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

        #region Gestion des relations entre Recettes et Ingredients

        Task<IEnumerable<QuantiteIngredients>> GetQuantiteIngredientsAsync();        

        Task<QuantiteIngredients> GetQuantiteIngredientsByIdAsync((int, int) key);
       
        Task<QuantiteIngredients> AddRecetteIngredientRelationshipAsync(QuantiteIngredients CreateRelationRI);        

        Task<QuantiteIngredients> updateRecetteIngredientRelationshipAsync(QuantiteIngredients updateRelationRI);        

        Task<bool> RemoveRecetteIngredientRelationshipAsync((int, int) key);        

        Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient);       

        Task<IEnumerable<QuantiteIngredients>> GetIngredientsByIdRecetteAsync(int idRecette);      

        Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette);        

        Task<bool> DeleteIngredientRelationsAsync(int idIngredient);       

        #endregion Fin Gestion des relations entre Recettes et Ingredients

    }
}
