using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes
{
    /// <summary>
    /// Interface du repository des recettes.  
    /// Fournit les opérations génériques de lecture/écriture 
    /// ainsi que des méthodes spécifiques liées aux relations entre recettes et catégories.
    /// </summary>
    public interface IRecetteRepository : IGenericReadRepository<int, Recette>, IGenericWriteRepository<int, Recette>
    {
        #region Relation Recette - Catégorie

        /// <summary>
        /// Récupère toutes les relations entre les recettes et les catégories.
        /// </summary>
        /// <returns>Une collection de relations <see cref="RecetteCategorieRelationship"/>.</returns>
        Task<IEnumerable<RecetteCategorieRelationship>> GetAllRecetteCategorieRelationshipAsync();

        /// <summary>
        /// Crée une nouvelle relation entre une recette et une catégorie.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la catégorie.</param>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns><c>true</c> si l’ajout a réussi, sinon <c>false</c>.</returns>
        Task<bool> AddRecetteCategorieRelationshipAsync(int idCategorie, int idRecette);

        /// <summary>
        /// Supprime une relation entre une recette et une catégorie.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la catégorie.</param>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns><c>true</c> si la suppression a réussi, sinon <c>false</c>.</returns>
        Task<bool> RemoveRecetteCategorieRelationshipAsync(int idCategorie, int idRecette);

        /// <summary>
        /// Récupère la liste des recettes associées à une catégorie donnée.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la catégorie.</param>
        /// <returns>Une collection de <see cref="Recette"/> correspondant à la catégorie.</returns>
        Task<IEnumerable<Recette>> GetRecettesByIdCategorieAsync(int idCategorie);

        /// <summary>
        /// Supprime toutes les relations entre une recette et les catégories auxquelles elle est liée.
        /// </summary>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns><c>true</c> si la suppression a réussi, sinon <c>false</c>.</returns>
        Task<bool> DeleteRecetteRelationsAsync(int idRecette);

        #endregion Fin relation Recette - Catégorie
    }
}
