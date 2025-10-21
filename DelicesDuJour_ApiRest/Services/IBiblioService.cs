using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.DTOIn;
using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace DelicesDuJour_ApiRest.Services
{
    /// <summary>
    /// Interface définissant les opérations principales du service de gestion de la bibliothèque culinaire.
    /// </summary>
    public interface IBiblioService
    {
        #region Recettes

        /// <summary>
        /// Récupère toutes les recettes disponibles.
        /// </summary>
        /// <returns>Une liste de <see cref="Recette"/>.</returns>
        Task<IEnumerable<Recette>> GetAllRecettesAsync();

        /// <summary>
        /// Récupère une recette spécifique à partir de son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de la recette.</param>
        /// <returns>Une instance de <see cref="Recette"/> correspondante.</returns>
        Task<Recette> GetRecetteByIdAsync(int id);

        /// <summary>
        /// Ajoute une nouvelle recette avec la possibilité d'associer une image.
        /// </summary>
        /// <param name="newRecette">La recette à créer.</param>
        /// <param name="photoFile">Fichier image optionnel associé à la recette.</param>
        /// <returns>La recette créée, incluant ses données complètes.</returns>
        Task<Recette> AddRecetteAsync(Recette newRecette, IFormFile? photoFile);

        /// <summary>
        /// Modifie une recette existante.
        /// </summary>
        /// <param name="updateRecette">La recette à mettre à jour.</param>
        /// <returns>La recette mise à jour.</returns>
        Task<Recette> ModifyRecetteAsync(Recette updateRecette);

        /// <summary>
        /// Supprime une recette et toutes ses relations associées.
        /// </summary>
        /// <param name="id">Identifiant de la recette à supprimer.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        Task<bool> DeleteRecetteAsync(int id);

        #endregion Fin Recettes

        #region Etapes

        /// <summary>
        /// Récupère toutes les étapes existantes.
        /// </summary>
        /// <returns>Une liste de <see cref="Etape"/>.</returns>
        Task<IEnumerable<Etape>> GetAllEtapesAsync();

        /// <summary>
        /// Récupère toutes les étapes associées à une recette donnée.
        /// </summary>
        /// <param name="id">Identifiant de la recette.</param>
        /// <returns>Liste des étapes associées.</returns>
        Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int id);

        /// <summary>
        /// Ajoute une nouvelle étape à une recette.
        /// </summary>
        /// <param name="newEtape">L’étape à ajouter.</param>
        /// <returns>L’étape créée.</returns>
        Task<Etape> AddEtapeAsync(Etape newEtape);

        /// <summary>
        /// Modifie une étape existante.
        /// </summary>
        /// <param name="updateEtape">L’étape à mettre à jour.</param>
        /// <returns>L’étape mise à jour.</returns>
        Task<Etape> ModifyEtapeAsync(Etape updateEtape);

        /// <summary>
        /// Supprime une étape d’une recette.
        /// </summary>
        /// <param name="key">Tuple représentant la clé (id_recette, numero_etape).</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        Task<bool> DeleteEtapeAsync((int, int) key);

        #endregion Fin Etapes

        #region Catégories

        /// <summary>
        /// Récupère toutes les catégories existantes.
        /// </summary>
        /// <returns>Une liste de <see cref="Categorie"/>.</returns>
        Task<IEnumerable<Categorie>> GetAllCategoriesAsync();

        /// <summary>
        /// Récupère une catégorie par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de la catégorie.</param>
        /// <returns>La catégorie correspondante.</returns>
        Task<Categorie> GetCategorieByIdAsync(int id);

        /// <summary>
        /// Ajoute une nouvelle catégorie.
        /// </summary>
        /// <param name="newCategorie">La catégorie à créer.</param>
        /// <returns>La catégorie créée.</returns>
        Task<Categorie> AddCategorieAsync(Categorie newCategorie);

        /// <summary>
        /// Modifie une catégorie existante.
        /// </summary>
        /// <param name="updateCategorie">La catégorie à mettre à jour.</param>
        /// <returns>La catégorie mise à jour.</returns>
        Task<Categorie> ModifyCategorieAsync(Categorie updateCategorie);

        /// <summary>
        /// Supprime une catégorie si elle n’est plus utilisée.
        /// </summary>
        /// <param name="id">Identifiant de la catégorie à supprimer.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        Task<bool> DeleteCategorieAsync(int id);

        #endregion Fin Catégories

        #region Relations Recettes - Categories

        /// <summary>
        /// Récupère toutes les relations entre les recettes et les catégories.
        /// </summary>
        /// <returns>Une liste de <see cref="RecetteCategorieRelationship"/>.</returns>
        Task<IEnumerable<RecetteCategorieRelationship>> GetAllRecettesCategoriesAsync();

        /// <summary>
        /// Crée une relation entre une recette et une catégorie.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la catégorie.</param>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>True si la relation a été créée avec succès.</returns>
        Task<bool> AddRecetteCategorieRelationshipAsync(int idCategorie, int idRecette);

        /// <summary>
        /// Supprime une relation entre une recette et une catégorie.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la catégorie.</param>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        Task<bool> RemoveRecetteCategorieRelationshipAsync(int idCategorie, int idRecette);

        /// <summary>
        /// Récupère toutes les recettes associées à une catégorie donnée.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la catégorie.</param>
        /// <returns>Liste des recettes correspondantes.</returns>
        Task<IEnumerable<Recette>> GetRecettesByIdCategorieAsync(int idCategorie);

        /// <summary>
        /// Récupère toutes les catégories associées à une recette donnée.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la recette.</param>
        /// <returns>Liste des catégories correspondantes.</returns>
        Task<IEnumerable<Categorie>> GetCategoriesByIdRecetteAsync(int idCategorie);

        /// <summary>
        /// Supprime toutes les relations entre une recette et ses catégories.
        /// </summary>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        Task<bool> DeleteRecetteRelationsAsync(int idRecette);

        /// <summary>
        /// Supprime toutes les relations entre une catégorie et ses recettes.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la catégorie.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        Task<bool> DeleteCategorieRelationsAsync(int idCategorie);

        #endregion Fin Relations Recettes - Categories

        #region Gestion Ingredients

        /// <summary>
        /// Récupère tous les ingrédients existants.
        /// </summary>
        /// <returns>Liste de <see cref="Ingredient"/>.</returns>
        Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();

        /// <summary>
        /// Récupère un ingrédient par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de l’ingrédient.</param>
        /// <returns>L’ingrédient correspondant.</returns>
        Task<Ingredient> GetIngredientByIdAsync(int id);

        /// <summary>
        /// Ajoute un nouvel ingrédient.
        /// </summary>
        /// <param name="newIngredient">L’ingrédient à créer.</param>
        /// <returns>L’ingrédient créé.</returns>
        Task<Ingredient> AddIngredientAsync(Ingredient newIngredient);

        /// <summary>
        /// Modifie un ingrédient existant.
        /// </summary>
        /// <param name="updateIngredient">L’ingrédient à mettre à jour.</param>
        /// <returns>L’ingrédient mis à jour.</returns>
        Task<Ingredient> ModifyIngredientAsync(Ingredient updateIngredient);

        /// <summary>
        /// Supprime un ingrédient.
        /// </summary>
        /// <param name="id">Identifiant de l’ingrédient.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        Task<bool> DeleteIngredientAsync(int id);

        #endregion Fin Gestion Ingredients

        #region Gestion des relations entre Recettes et Ingredients

        /// <summary>
        /// Récupère toutes les relations entre recettes et ingrédients.
        /// </summary>
        /// <returns>Liste des relations <see cref="QuantiteIngredients"/>.</returns>
        Task<IEnumerable<QuantiteIngredients>> GetQuantiteIngredientsAsync();

        /// <summary>
        /// Récupère une relation recette/ingrédient spécifique.
        /// </summary>
        /// <param name="key">Tuple représentant la clé (id_ingredient, id_recette).</param>
        /// <returns>La relation correspondante.</returns>
        Task<QuantiteIngredients> GetQuantiteIngredientsByIdAsync((int, int) key);

        /// <summary>
        /// Crée une relation entre une recette et un ingrédient avec une quantité donnée.
        /// </summary>
        /// <param name="CreateRelationRI">Objet contenant les informations de la relation.</param>
        /// <returns>La relation créée.</returns>
        Task<QuantiteIngredients> AddRecetteIngredientRelationshipAsync(QuantiteIngredients CreateRelationRI);

        /// <summary>
        /// Met à jour une relation recette/ingrédient existante.
        /// </summary>
        /// <param name="updateRelationRI">Relation à mettre à jour.</param>
        /// <returns>La relation mise à jour.</returns>
        Task<QuantiteIngredients> updateRecetteIngredientRelationshipAsync(QuantiteIngredients updateRelationRI);

        /// <summary>
        /// Supprime une relation recette/ingrédient.
        /// </summary>
        /// <param name="key">Tuple représentant la clé (id_ingredient, id_recette).</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        Task<bool> RemoveRecetteIngredientRelationshipAsync((int, int) key);

        /// <summary>
        /// Récupère toutes les recettes associées à un ingrédient spécifique.
        /// </summary>
        /// <param name="idIngredient">Identifiant de l’ingrédient.</param>
        /// <returns>Liste des recettes correspondantes.</returns>
        Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient);

        /// <summary>
        /// Récupère toutes les quantités d’ingrédients d’une recette donnée.
        /// </summary>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>Liste des ingrédients avec leur quantité.</returns>
        Task<IEnumerable<QuantiteIngredients>> GetIngredientsByIdRecetteAsync(int idRecette);

        /// <summary>
        /// Supprime toutes les relations entre une recette et ses ingrédients.
        /// </summary>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette);

        /// <summary>
        /// Supprime toutes les relations entre un ingrédient et les recettes associées.
        /// </summary>
        /// <param name="idIngredient">Identifiant de l’ingrédient.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        Task<bool> DeleteIngredientRelationsAsync(int idIngredient);

        #endregion Fin Gestion des relations entre Recettes et Ingredients
    }
}
