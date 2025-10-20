using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Ingredients
{
    /// <summary>
    /// Interface définissant le contrat du repository pour la gestion des entités <see cref="Ingredient"/>.
    /// Hérite des interfaces génériques de lecture et d’écriture pour les opérations CRUD standard.
    /// </summary>
    public interface IIngredientRepository : IGenericReadRepository<int, Ingredient>, IGenericWriteRepository<int, Ingredient>
    {
        /// <summary>
        /// Récupère la liste des ingrédients associés à une recette donnée.
        /// </summary>
        /// <param name="idRecette">Identifiant unique de la recette.</param>
        /// <returns>
        /// Une collection d’objets <see cref="Ingredient"/> correspondant aux ingrédients liés à la recette spécifiée.
        /// </returns>
        Task<IEnumerable<Ingredient>> GetIngredientsByIdRecetteAsync(int idRecette);

        // Ajouter ici des méthodes spécifiques au repository Ingredient si nécessaire
    }
}
