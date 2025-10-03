using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes
{
    public interface IRecetteRepository : IGenericReadRepository<int, Recette>, IGenericWriteRepository<int, Recette>
    {
        // Ajouter ici des méthodes spécifiques au repository Book si nécessaire

        #region Relation Recette catégorie
        Task<IEnumerable<RecetteCategorieRelationship>> GetAllRecetteCategorieRelationshipAsync();
        Task<bool> AddRecetteCategorieRelationshipAsync(int idCategorie, int idRecette);
        Task<bool> RemoveRecetteCategorieRelationshipAsync(int idCategorie, int idRecette);
        Task<IEnumerable<Recette>> GetRecettesByIdCategorieAsync(int idCategorie);
        Task<bool> DeleteRecetteRelationsAsync(int idRecette);       

        #endregion Fin relation Recette Catégorie
        
    }
}
