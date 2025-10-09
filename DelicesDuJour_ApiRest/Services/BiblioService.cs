using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;
using DelicesDuJour_ApiRest.DataAccessLayer.Unit_of_Work;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using MySqlX.XDevAPI.Common;
using System.Diagnostics.Eventing.Reader;
using static Mysqlx.Expect.Open.Types.Condition.Types;

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
            var recette = await _UoW.Recettes.GetAsync(id);

            var listEtapes = await _UoW.Etapes.GetEtapesByIdRecetteAsync(id);

            var listQuantiteIngredients = await _UoW.QuantiteIngred.GetIngredientsByIdRecetteAsync(id);

            var listIngredients = await _UoW.Ingredients.GetIngredientsByIdRecetteAsync(id);

            Recette recetteCompleteDTO = new()
            {
                Id = recette.Id,
                nom = recette.nom,
                temps_preparation = recette.temps_preparation,
                temps_cuisson = recette.temps_cuisson,
                difficulte = recette.difficulte,
                ingredients = listIngredients.ToList(),
                etapes = listEtapes.ToList()
            };
            return recetteCompleteDTO;
        }

        public async Task<Recette> AddRecetteAsync(Recette newRecette)
        {
            _UoW.BeginTransaction();

            // Gestion Ingredients
            var ingredientsExistants = (await _UoW.Ingredients.GetAllAsync()).ToList();
            List<Ingredient> newIngredients = new();

            foreach (Ingredient i in newRecette.ingredients)
            {
                Ingredient? newIngredient = null;

                // On cherche un ingrédient avec le même nom
                foreach (Ingredient ingredient in ingredientsExistants)
                {
                    if (!string.IsNullOrEmpty(i.nom) && ingredient.nom == i.nom)
                    {
                        // trouvé → on le réutilise
                        newIngredient = ingredient;
                        newIngredient.quantite = i.quantite;
                        break; // on s'arrête ici
                    }
                }

                // Si on n’a rien trouvé, on le crée
                if (newIngredient == null)
                {
                    newIngredient = await _UoW.Ingredients.CreateAsync(new Ingredient
                    {
                        nom = i.nom
                    });
                    newIngredient.quantite = i.quantite;

                    // l'ajouter à la liste des existants
                    // pour éviter de le recréer à la prochaine boucle
                    ingredientsExistants.Add(newIngredient);
                }

                newIngredients.Add(newIngredient);
            }

            newRecette.ingredients = newIngredients;




            var CreatedRecette = await _UoW.Recettes.CreateAsync(newRecette);

            List<QuantiteIngredients> listQuantiteIngredients = new();

            foreach (Ingredient ingredient in newRecette.ingredients)
            {
                QuantiteIngredients quantiteIngredients = new()
                {
                    id_ingredient = ingredient.id,
                    id_recette = CreatedRecette.Id,
                    quantite = ingredient.quantite
                };

                listQuantiteIngredients.Add(quantiteIngredients);
            }

            // création des liens entre la recette et chaque ingedients (la nouvelle méthode du repo des recettes)
            if (CreatedRecette is not null)
            {
                foreach (QuantiteIngredients quantiteIngredients in listQuantiteIngredients)
                {
                    await _UoW.QuantiteIngred.CreateAsync(quantiteIngredients);
                }
            }

            // Création des étapes de la recette
            List<Etape> createdEtapes = new();

            foreach (Etape etape in newRecette.etapes)
            {
                etape.id_recette = CreatedRecette.Id;
                var createdEtape = await _UoW.Etapes.CreateAsync(etape);
                createdEtapes.Add(createdEtape);
            }

            // Création des relations recette/catégories

            List<Categorie> categoriesCreatedRecette = new();

            foreach (Categorie categorie in newRecette.categories)
            {
                var relationRecetteCategorie = await _UoW.Recettes.AddRecetteCategorieRelationshipAsync(categorie.id, newRecette.Id);
                if (relationRecetteCategorie == true)
                {
                    categoriesCreatedRecette = newRecette.categories;
                }

            }

            Recette recetteComplete = new()
            {
                Id = CreatedRecette.Id,
                nom = CreatedRecette.nom,
                temps_preparation = CreatedRecette.temps_preparation,
                temps_cuisson = CreatedRecette.temps_cuisson,
                difficulte = CreatedRecette.difficulte,
                etapes = createdEtapes,
                ingredients = newIngredients,
                categories = categoriesCreatedRecette
            };

            if (recetteComplete is not null)
                _UoW.Commit();

            return recetteComplete;
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

        public async Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int id)
        {
            return await _UoW.Etapes.GetEtapesByIdRecetteAsync(id);
        }

        public async Task<Etape> AddEtapeAsync(Etape newEtape)
        {
            return await _UoW.Etapes.CreateAsync(newEtape);
        }

        public async Task<Etape> ModifyEtapeAsync(Etape updateEtape)
        {
            return await _UoW.Etapes.ModifyAsync(updateEtape);
        }

        public async Task<bool> DeleteEtapeAsync(int id)
        {
            return await _UoW.Etapes.DeleteAsync(id);
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

        #region Gestion des relations entre Recettes et Ingredients

        public async Task<IEnumerable<QuantiteIngredients>> GetQuantiteIngredientsAsync()
        {
            return await _UoW.QuantiteIngred.GetAllAsync();
        }

        public async Task<QuantiteIngredients> GetQuantiteIngredientsByIdAsync((int, int) key)
        {
            return await _UoW.QuantiteIngred.GetAsync(key);
        }
        public async Task<QuantiteIngredients> AddRecetteIngredientRelationshipAsync(QuantiteIngredients CreateRelationRI)
        {
            return await _UoW.QuantiteIngred.CreateAsync(CreateRelationRI);
        }

        public async Task<QuantiteIngredients> updateRecetteIngredientRelationshipAsync(QuantiteIngredients updateRelationRI)
        {
            return await _UoW.QuantiteIngred.ModifyAsync(updateRelationRI);
        }

        public async Task<bool> RemoveRecetteIngredientRelationshipAsync((int, int) key)
        {
            return await _UoW.QuantiteIngred.DeleteAsync(key);
        }

        public async Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient)
        {
            return await _UoW.QuantiteIngred.GetRecettesByIdIngredientAsync(idIngredient);
        }

        public async Task<IEnumerable<QuantiteIngredients>> GetIngredientsByIdRecetteAsync(int idRecette)
        {
            return await _UoW.QuantiteIngred.GetIngredientsByIdRecetteAsync(idRecette);
        }

        public async Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette)
        {
            return await _UoW.QuantiteIngred.DeleteRecetteRelationsIngredientAsync(idRecette);
        }

        public async Task<bool> DeleteIngredientRelationsAsync(int idIngredient)
        {
            return await _UoW.QuantiteIngred.DeleteIngredientRelationsRecetteAsync(idIngredient);
        }

        #endregion Fin Gestion des relations entre Recettes et Ingredients

    }
}
