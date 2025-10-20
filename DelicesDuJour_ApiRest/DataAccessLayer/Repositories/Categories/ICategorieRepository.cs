using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories
{
    /// <summary>
    /// Interface définissant les opérations spécifiques au repository des <see cref="Categorie"/>.
    /// Hérite des interfaces génériques pour la lecture et l’écriture des entités.
    /// </summary>
    public interface ICategorieRepository : IGenericReadRepository<int, Categorie>, IGenericWriteRepository<int, Categorie>
    {
        // Cette interface regroupe les opérations de base (CRUD) issues des interfaces génériques
        // et ajoute des méthodes propres à la gestion des catégories.

        /// <summary>
        /// Récupère toutes les catégories associées à une recette donnée.
        /// </summary>
        /// <param name="idRecette">Identifiant unique de la recette.</param>
        /// <returns>Une collection d’objets <see cref="Categorie"/> liés à la recette.</returns>
        Task<IEnumerable<Categorie>> GetCategoriesByIdRecetteAsync(int idRecette);

        /// <summary>
        /// Supprime toutes les relations entre une catégorie et les recettes associées.
        /// </summary>
        /// <param name="idCategorie">Identifiant unique de la catégorie.</param>
        /// <returns>
        /// True si au moins une relation a été supprimée, sinon false.
        /// </returns>
        Task<bool> DeleteCategorieRelationsAsync(int idCategorie);

        /// <summary>
        /// Vérifie si une catégorie est associée à une ou plusieurs recettes.
        /// </summary>
        /// <param name="categorieId">Identifiant unique de la catégorie.</param>
        /// <returns>
        /// True si la catégorie est liée à au moins une recette, sinon false.
        /// </returns>
        Task<bool> HasRecetteRelationsAsync(int categorieId);
    }
}
