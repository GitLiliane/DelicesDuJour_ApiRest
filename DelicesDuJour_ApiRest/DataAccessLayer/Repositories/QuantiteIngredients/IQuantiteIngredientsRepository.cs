using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngredients
{
    public interface IQuantiteIngredientsRepository : IGenericReadRepository<int, Recette>, IGenericWriteRepository<int, Recette>
    {
        #region Relation Recette Ingrédient

        Task<bool> AddRecetteIngredientRelationshipAsync(int idIngredient, int idRecette, CreateQuantiteIngredientsDTO CreateRelationRI);
        Task<bool> RemoveRecetteIngredientRelationshipAsync(int idIngredient, int idRecette);
        Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient);
        Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette);

        #endregion Fin Relation Recette Ingrédient
    }
}
