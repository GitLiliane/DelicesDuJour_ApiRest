using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes
{
    /// <summary>
    /// Interface définissant le contrat pour le repository gérant les entités <see cref="Etape"/>.
    /// Elle hérite des interfaces génériques de lecture et d’écriture pour les opérations CRUD de base.
    /// </summary>
    public interface IEtapeRepository : IGenericReadRepository<(int, int), Etape>, IGenericWriteRepository<(int, int), Etape>
    {
        /// <summary>
        /// Récupère toutes les étapes associées à une recette spécifique.
        /// </summary>
        /// <param name="id">Identifiant unique de la recette.</param>
        /// <returns>Une collection d’objets <see cref="Etape"/> appartenant à la recette spécifiée.</returns>
        Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int id);

        /// <summary>
        /// Supprime toutes les étapes associées à une recette donnée.
        /// </summary>
        /// <param name="id_recette">Identifiant unique de la recette dont les étapes doivent être supprimées.</param>
        /// <returns>
        /// True si au moins une étape a été supprimée,
        /// sinon False si aucune correspondance n’a été trouvée.
        /// </returns>
        Task<bool> DeleteEtapesRelationByIdRecetteAsync(int id_recette);
    }
}

