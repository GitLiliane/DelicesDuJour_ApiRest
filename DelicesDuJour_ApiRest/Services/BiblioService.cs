using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;
using DelicesDuJour_ApiRest.DataAccessLayer.Unit_of_Work;
using DelicesDuJour_ApiRest.Domain.BO;
using System.Diagnostics.Eventing.Reader;

namespace DelicesDuJour_ApiRest.Services
{
    /// <summary>
    /// Service de gestion de la bibliothèque.
    /// Fournit des méthodes pour gérer les livres, les auteurs et leurs relations.
    /// </summary>
    public class BiblioService : IBiblioService
    {

        private readonly IUoW _UoW;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="BiblioService"/>.
        /// </summary>
        /// <param name="UoW">Unité de travail pour accéder aux repositories.</param>

        public BiblioService(IUoW UoW)
        {
            _UoW = UoW;
        }

        #region Gestion des recettes
        public async Task<IEnumerable<Recette>> GetAllRecettesAsync()
        {
            return await _UoW.Recettes.GetAllAsync();
        }

        public async Task<Recette> GetRecetteByIdAsync(int id)
        {
            return await _UoW.Recettes.GetAsync(id);
        }

        public async Task<Recette> AddRecetteAsync(Recette newRecette)
        {
            // parcourir tous les ingredients de la recettes
            {
                // si l'ingredient n'existe pas => on le crée (avec le repo des ingredients)
            }
            // await _UoW.Recettes.CreateAsync(newRecette)

            // création des liens entre la recette et chaque ingedients (la nouvelle méthode du repo des recettes)
            return await _UoW.Recettes.CreateAsync(newRecette);
        }

        public async Task<Recette> ModifyRecetteAsync(Recette updateRecette)
        {
            return await _UoW.Recettes.ModifyAsync(updateRecette);
        }

        public async Task<bool> DeleteRecetteAsync(int id)

        {
            _UoW.BeginTransaction();

            // Supprime toutes les relations de la recette avce les catégories
            await _UoW.Recettes.DeleteRecetteRelationsAsync(id);

            // Supprime la recette elle-même
            var result = await _UoW.Recettes.DeleteAsync(id);

            // Valide la transaction si toutes les opérations ont réussi
            if (result)
                _UoW.Commit();

            return result;
        }

        #endregion Fin Gestion des recettes

        #region Gestion des Etapes

        public async Task<IEnumerable<Etape>> GetAllEtapesAsync()
        {
            return await _UoW.Etapes.GetAllAsync();
        }

        public async Task<Etape> GetEtapeByIdAsync(TupleClass<int, int> key)
        {
            return await _UoW.Etapes.GetAsync(key);
        }

        public async Task<Etape> AddEtapeAsync(Etape newEtape)
        {
            return await _UoW.Etapes.CreateAsync(newEtape);
        }

        public async Task<Etape> ModifyEtapeAsync(Etape updateEtape)
        {
            return await _UoW.Etapes.ModifyAsync(updateEtape);
        }

        public async Task<bool> DeleteEtapeAsync(TupleClass<int, int> key)
        {
            return await _UoW.Etapes.DeleteAsync(key);
        }


        #endregion Fin Etapes

        #region Categories

        public async Task<IEnumerable<Categorie>> GetAllCategoriesAsync()
        {
            return await _UoW.Categories.GetAllAsync();
        }

        public async Task<Categorie> GetCategorieByIdAsync(int id)
        {
            return await _UoW.Categories.GetAsync(id);
        }

        public async Task<Categorie> AddCategorieAsync(Categorie newCategorie)
        {
            return await _UoW.Categories.CreateAsync(newCategorie);
        }

        public async Task<Categorie> ModifyCategorieAsync(Categorie updateCategorie)
        {
            return await _UoW.Categories.ModifyAsync(updateCategorie);
        }

        public async Task<bool> DeleteCategorieAsync(int id)
        {
            _UoW.BeginTransaction();

            // Vérifie si la catégorie a encore des relations
            bool hasRelations = await _UoW.Categories.HasRecetteRelationsAsync(id);

            if (hasRelations)
            {
                _UoW.Rollback();
                throw new InvalidOperationException("Impossible de supprimer la catégorie car elle est encore associée à des recettes.");
            }

            // Supprime la catégorie
            var result = await _UoW.Categories.DeleteAsync(id);

            if (result)
                _UoW.Commit();

            return result;
        }


        #endregion Fin Gestion Categories

        #region Gestion des relations entre Recettes et Catégories

        public async Task<IEnumerable<RecetteCategorieRelationship>> GetAllRecettesCategoriesAsync()
        {
            return await _UoW.Recettes.GetAllRecetteCategorieRelationshipAsync();

        }

        public async Task<bool> AddRecetteCategorieRelationshipAsync(int idCategorie, int idRecette)
        {
            return await _UoW.Recettes.AddRecetteCategorieRelationshipAsync(idCategorie, idRecette);
        }

        public async Task<bool> RemoveRecetteCategorieRelationshipAsync(int idCategorie, int idRecette)
        {
            return await _UoW.Recettes.RemoveRecetteCategorieRelationshipAsync(idCategorie, idRecette);
        }

        public async Task<IEnumerable<Recette>> GetRecettesByIdCategorieAsync(int idCategorie)
        {
            return await _UoW.Recettes.GetRecettesByIdCategorieAsync(idCategorie);
        }

        public async Task<IEnumerable<Categorie>> GetCategoriesByIdRecetteAsync(int idRecette)
        {
            return await _UoW.Categories.GetCategoriesByIdRecetteAsync(idRecette);
        }

        public async Task<bool> DeleteRecetteRelationsAsync(int idRecette)
        {
            return await _UoW.Recettes.DeleteRecetteRelationsAsync(idRecette);
        }

        public async Task<bool> DeleteCategorieRelationsAsync(int idCategorie)
        {
            return await _UoW.Categories.DeleteCategorieRelationsAsync(idCategorie);
        }


        #endregion Fin Gestion des relations entre Recettes et Catégories

        #region Gestion Ingredients

        public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
        {
            return await _UoW.Ingredients.GetAllAsync();
        }

        public async Task<Ingredient> GetIngredientByIdAsync(int id)
        {
            return await _UoW.Ingredients.GetAsync(id);
        }

        public async Task<Ingredient> AddIngredientAsync(Ingredient newIngredient)
        {
            return await _UoW.Ingredients.CreateAsync(newIngredient);
        }

        public async Task<Ingredient> ModifyIngredientAsync(Ingredient updateIngredient)
        {
            return await _UoW.Ingredients.ModifyAsync(updateIngredient);
        }

        public async Task<bool> DeleteIngredientAsync(int id)
        {
            return await _UoW.Ingredients.DeleteAsync(id);
        }

        #endregion Fin Gestion Ingredients

        #region Fin Gestion des relations entre Recettes et Ingredients

        public async Task<bool> AddRecetteIngredientRelationshipAsync(int idIngredient, int idRecette, string quantite)
        {
            return await _UoW.Recettes.AddRecetteIngredientRelationshipAsync(idIngredient, idRecette, quantite);
        }

        public async Task<bool> RemoveRecetteIngredientRelationshipAsync(int idIngredient, int idRecette)
        {
            return await _UoW.Recettes.RemoveRecetteIngredientRelationshipAsync(idIngredient, idRecette);
        }

        public async Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient)
        {
            return await _UoW.Recettes.GetRecettesByIdIngredientAsync(idIngredient);
        }

        //public async Task<IEnumerable<Ingredient>> GetIngredientsByIdRecetteAsync(int idRecette)
        //{
        //    return await _UoW.Ingredients.GetIngredientsByIdRecetteAsync(idRecette);
        //}

        public async Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette)
        {
            return await _UoW.Recettes.DeleteRecetteRelationsAsync(idRecette);
        }

        //public async Task<bool> DeleteIngredientRelationsAsync(int idIngredient)
        //{
        //    return await _UoW.Ingredients.DeleteIngredientRelationsAsync(idIngredient);
        //}

        #endregion Fin Gestion des relations entre Recettes et Ingredients

    }
}
