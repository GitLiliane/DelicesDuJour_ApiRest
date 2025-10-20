using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngred
{
    /// <summary>
    /// Interface du repository pour la gestion des relations entre les recettes et leurs ingrédients,
    /// incluant la quantité associée à chaque ingrédient.
    /// </summary>
    /// <remarks>
    /// Cette interface hérite de <see cref="IGenericReadRepository{TKey, TEntity}"/> et 
    /// <see cref="IGenericWriteRepository{TKey, TEntity}"/>, en utilisant une clé composite <c>(int, int)</c> 
    /// représentant respectivement <c>id_ingredient</c> et <c>id_recette</c>.
    /// </remarks>
    public interface IQuantiteIngredRepository : IGenericReadRepository<(int, int), QuantiteIngredients>, IGenericWriteRepository<(int, int), QuantiteIngredients>
    {
        #region Relation Recette Ingrédient

        /// <summary>
        /// Récupère l'ensemble des relations recette-ingrédient enregistrées dans la base de données.
        /// </summary>
        /// <returns>Une collection d'objets <see cref="QuantiteIngredients"/>.</returns>
        Task<IEnumerable<QuantiteIngredients>> GetAllAsync();

        /// <summary>
        /// Récupère une relation recette-ingrédient spécifique à partir de sa clé composite.
        /// </summary>
        /// <param name="key">Tuple contenant l’identifiant de l’ingrédient et celui de la recette.</param>
        /// <returns>L’objet <see cref="QuantiteIngredients"/> correspondant, ou <c>null</c> s’il n’existe pas.</returns>
        Task<QuantiteIngredients> GetAsync((int, int) key);

        /// <summary>
        /// Crée une nouvelle relation entre une recette et un ingrédient avec une quantité donnée.
        /// </summary>
        /// <param name="CreateRelationRI">Objet contenant les informations de la relation à créer.</param>
        /// <returns>La relation créée sous forme d’objet <see cref="QuantiteIngredients"/>.</returns>
        Task<QuantiteIngredients> CreateAsync(QuantiteIngredients CreateRelationRI);

        /// <summary>
        /// Met à jour une relation recette-ingrédient existante.
        /// </summary>
        /// <param name="updateBook">Objet contenant les données mises à jour de la relation.</param>
        /// <returns>La relation mise à jour, ou <c>null</c> si aucune ligne n’a été modifiée.</returns>
        Task<QuantiteIngredients> ModifyAsync(QuantiteIngredients updateBook);

        /// <summary>
        /// Supprime une relation recette-ingrédient spécifique.
        /// </summary>
        /// <param name="key">Tuple contenant les identifiants de l’ingrédient et de la recette.</param>
        /// <returns><c>true</c> si la suppression a réussi, sinon <c>false</c>.</returns>
        Task<bool> DeleteAsync((int, int) key);

        /// <summary>
        /// Récupère toutes les recettes associées à un ingrédient donné.
        /// </summary>
        /// <param name="idIngredient">Identifiant unique de l’ingrédient.</param>
        /// <returns>Une collection d’objets <see cref="Recette"/>.</returns>
        Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient);

        /// <summary>
        /// Récupère tous les ingrédients (avec leurs quantités) associés à une recette donnée.
        /// </summary>
        /// <param name="idRecette">Identifiant unique de la recette.</param>
        /// <returns>Une collection d’objets <see cref="QuantiteIngredients"/>.</returns>
        Task<IEnumerable<QuantiteIngredients>> GetIngredientsByIdRecetteAsync(int idRecette);

        /// <summary>
        /// Supprime toutes les relations ingrédient-recette associées à une recette donnée.
        /// </summary>
        /// <param name="idRecette">Identifiant unique de la recette.</param>
        /// <returns><c>true</c> si au moins une relation a été supprimée, sinon <c>false</c>.</returns>
        Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette);

        /// <summary>
        /// Supprime toutes les relations ingrédient-recette associées à un ingrédient donné.
        /// </summary>
        /// <param name="idIngredient">Identifiant unique de l’ingrédient.</param>
        /// <returns><c>true</c> si au moins une relation a été supprimée, sinon <c>false</c>.</returns>
        Task<bool> DeleteIngredientRelationsRecetteAsync(int idIngredient);

        #endregion Fin Relation Recette Ingrédient
    }
}
