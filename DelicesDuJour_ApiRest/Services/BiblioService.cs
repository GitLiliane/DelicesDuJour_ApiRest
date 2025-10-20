using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;
using DelicesDuJour_ApiRest.DataAccessLayer.Unit_of_Work;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.DTOIn;
using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Tls;
using System.Diagnostics.Eventing.Reader;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace DelicesDuJour_ApiRest.Services
{
    /// <summary>
    /// Service pour la gestion des recettes et des entités associées.
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

        /// <summary>
        /// Récupère toutes les recettes.
        /// </summary>
        /// <returns>Une collection de toutes les recettes.</returns>
        public async Task<IEnumerable<Recette>> GetAllRecettesAsync()
        {
            return await _UoW.Recettes.GetAllAsync();
        }

        /// <summary>
        /// Récupère une recette complète par son identifiant, incluant étapes, ingrédients et catégories.
        /// </summary>
        /// <param name="id">Identifiant de la recette.</param>
        /// <returns>Une instance complète de <see cref="Recette"/> ou null si non trouvée.</returns>
        public async Task<Recette> GetRecetteByIdAsync(int id)
        {
            // Début de la transaction
            _UoW.BeginTransaction();

            // Récupération de la recette
            var recette = await _UoW.Recettes.GetAsync(id);

            // Récupération des étapes associées
            var listEtapes = await _UoW.Etapes.GetEtapesByIdRecetteAsync(id);

            // Récupération des quantités d'ingrédients
            var listQuantiteIngredients = await _UoW.QuantiteIngred.GetIngredientsByIdRecetteAsync(id);

            // Récupération des ingrédients détaillés
            var listIngredients = await _UoW.Ingredients.GetIngredientsByIdRecetteAsync(id);

            // Récupération des catégories associées
            var listCategories = await _UoW.Categories.GetCategoriesByIdRecetteAsync(id);

            // Construction de la recette complète
            Recette recetteCompleteDTO = new()
            {
                Id = recette.Id,
                nom = recette.nom,
                temps_preparation = recette.temps_preparation,
                temps_cuisson = recette.temps_cuisson,
                difficulte = recette.difficulte,
                ingredients = listIngredients.ToList(),
                etapes = listEtapes.ToList(),
                categories = listCategories.ToList(),
                photo = recette.photo
            };

            // Validation et commit de la transaction
            if (recetteCompleteDTO is not null)
                _UoW.Commit();

            return recetteCompleteDTO;
        }

        /// <summary>
        /// Ajoute une nouvelle recette avec ses ingrédients, étapes, catégories et photo éventuelle.
        /// </summary>
        /// <param name="newRecette">Objet <see cref="Recette"/> à créer.</param>
        /// <param name="photoFile">Fichier photo optionnel de la recette.</param>
        /// <returns>La recette complète créée avec tous ses liens et sa photo.</returns>
        public async Task<Recette> AddRecetteAsync(Recette newRecette, IFormFile? photoFile)
        {
            // Début de la transaction
            _UoW.BeginTransaction();

            // Traitement de la photo si présente
            if (photoFile != null && photoFile.Length > 0)
            {
                // Définition du dossier de destination pour les images
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "recettes");

                // Création du dossier si celui-ci n'existe pas
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Génération d'un nom de fichier unique pour éviter les collisions
                string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(photoFile.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Copie du fichier téléchargé vers le dossier cible
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photoFile.CopyToAsync(fileStream);
                }

                // Mise à jour du chemin relatif de la photo dans l'objet recette
                newRecette.photo = $"images/recettes/{uniqueFileName}";
            }

            // Création de la recette dans la base
            Recette CreatedRecette = await _UoW.Recettes.CreateAsync(newRecette);

            // Gestion des ingrédients
            var ingredientsExistants = (await _UoW.Ingredients.GetAllAsync()).ToList();
            List<Ingredient> newIngredients = new();

            foreach (Ingredient i in newRecette.ingredients)
            {
                Ingredient? newIngredient = null;

                // Vérifie si l'ingrédient existe déjà dans la base
                foreach (Ingredient ingredient in ingredientsExistants)
                {
                    if (!string.IsNullOrEmpty(i.nom) && ingredient.nom == i.nom)
                    {
                        newIngredient = ingredient;
                        newIngredient.quantite = i.quantite;
                        break;
                    }
                }

                // Création de l'ingrédient s'il n'existe pas encore
                if (newIngredient == null)
                {
                    newIngredient = await _UoW.Ingredients.CreateAsync(new Ingredient { nom = i.nom });
                    newIngredient.quantite = i.quantite;
                    ingredientsExistants.Add(newIngredient);
                }

                newIngredients.Add(newIngredient);
            }

            newRecette.ingredients = newIngredients;

            // Création des liens recette / quantités ingrédients
            if (CreatedRecette != null)
            {
                foreach (var ingredient in newIngredients)
                {
                    await _UoW.QuantiteIngred.CreateAsync(new QuantiteIngredients
                    {
                        id_recette = CreatedRecette.Id,
                        id_ingredient = ingredient.id,
                        quantite = ingredient.quantite
                    });
                }
            }

            // Création des étapes
            List<Etape> createdEtapes = new();
            foreach (Etape etape in newRecette.etapes)
            {
                etape.id_recette = CreatedRecette.Id;
                var createdEtape = await _UoW.Etapes.CreateAsync(etape);
                createdEtapes.Add(createdEtape);
            }

            // Création des relations recette / catégories
            List<Categorie> categoriesCreatedRecette = new();
            foreach (Categorie categorie in newRecette.categories)
            {
                bool relation = await _UoW.Recettes.AddRecetteCategorieRelationshipAsync(categorie.id, CreatedRecette.Id);
                if (relation) categoriesCreatedRecette = newRecette.categories;
            }

            // Préparer l'objet complet à retourner
            Recette recetteComplete = new()
            {
                Id = CreatedRecette.Id,
                nom = CreatedRecette.nom,
                temps_preparation = CreatedRecette.temps_preparation,
                temps_cuisson = CreatedRecette.temps_cuisson,
                difficulte = CreatedRecette.difficulte,
                etapes = createdEtapes,
                ingredients = newIngredients,
                categories = categoriesCreatedRecette,
                photo = newRecette.photo
            };

            // Commit de la transaction si l'objet complet est valide
            if (recetteComplete is not null)
                _UoW.Commit();

            return recetteComplete;
        }


        /// <summary>
        /// Modifie une recette existante avec ses ingrédients, étapes, catégories et éventuellement sa photo.
        /// </summary>
        /// <param name="updateRecette">Recette mise à jour.</param>
        /// <returns>Recette complète après modification.</returns>
        public async Task<Recette> ModifyRecetteAsync(Recette updateRecette)
        {
            // Début de la transaction
            _UoW.BeginTransaction();

            // Gestion de la photo existante
            if (!string.IsNullOrEmpty(updateRecette.photo))
            {
                // On récupère la recette existante pour supprimer l’ancienne photo si nécessaire
                var ancienneRecette = await _UoW.Recettes.GetAsync(updateRecette.Id);
                if (ancienneRecette != null && !string.IsNullOrEmpty(ancienneRecette.photo))
                {
                    var ancienChemin = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        ancienneRecette.photo.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    if (File.Exists(ancienChemin))
                    {
                        try
                        {
                            File.Delete(ancienChemin);
                        }
                        catch
                        {
                            // On ignore les erreurs lors de la suppression de l'ancien fichier
                        }
                    }
                }
            }

            // Gestion des ingrédients
            var ingredientsExistants = (await _UoW.Ingredients.GetAllAsync()).ToList();

            // Suppression des anciennes relations recette/ingrédients
            var relationDelete = await _UoW.QuantiteIngred.DeleteRecetteRelationsIngredientAsync(updateRecette.Id);

            List<Ingredient> newIngredients = new();

            foreach (Ingredient i in updateRecette.ingredients)
            {
                Ingredient? newIngredient = null;

                // Recherche d'un ingrédient existant avec le même nom
                foreach (Ingredient ingredient in ingredientsExistants)
                {
                    if (!string.IsNullOrEmpty(i.nom) && ingredient.nom == i.nom)
                    {
                        newIngredient = ingredient;
                        newIngredient.quantite = i.quantite;
                        break; // Arrêt de la recherche
                    }
                }

                // Si l'ingrédient n'existe pas, création
                if (newIngredient == null)
                {
                    newIngredient = await _UoW.Ingredients.CreateAsync(new Ingredient
                    {
                        nom = i.nom
                    });
                    newIngredient.quantite = i.quantite;

                    // Ajout à la liste des existants pour éviter duplication
                    ingredientsExistants.Add(newIngredient);
                }

                newIngredients.Add(newIngredient);
            }

            updateRecette.ingredients = newIngredients;

            // Mise à jour de la recette
            var recetteUpdated = await _UoW.Recettes.ModifyAsync(updateRecette);

            // Création des liens entre la recette et les quantités d'ingrédients
            List<QuantiteIngredients> listQuantiteIngredients = new();
            foreach (Ingredient ingredient in updateRecette.ingredients)
            {
                QuantiteIngredients quantiteIngredients = new()
                {
                    id_ingredient = ingredient.id,
                    id_recette = recetteUpdated.Id,
                    quantite = ingredient.quantite
                };
                listQuantiteIngredients.Add(quantiteIngredients);
            }

            if (recetteUpdated is not null)
            {
                foreach (QuantiteIngredients quantiteIngredients in listQuantiteIngredients)
                {
                    await _UoW.QuantiteIngred.CreateAsync(quantiteIngredients);
                }
            }

            // Gestion des étapes de la recette
            await _UoW.Etapes.DeleteEtapesRelationByIdRecetteAsync(updateRecette.Id);
            List<Etape> createdEtapes = new();
            foreach (Etape etape in updateRecette.etapes)
            {
                etape.id_recette = recetteUpdated.Id;
                var createdEtape = await _UoW.Etapes.CreateAsync(etape);
                createdEtapes.Add(createdEtape);
            }

            // Gestion des relations recette/catégories
            List<Categorie> categoriesUpdatedRecette = new();
            await _UoW.Recettes.DeleteRecetteRelationsAsync(updateRecette.Id);

            foreach (Categorie categorie in updateRecette.categories)
            {
                var relationRecetteCategorie = await _UoW.Recettes.AddRecetteCategorieRelationshipAsync(categorie.id, recetteUpdated.Id);
                if (relationRecetteCategorie)
                {
                    categoriesUpdatedRecette.Add(categorie);
                }
            }

            // Construction de l'objet complet de retour
            Recette recetteComplete = new()
            {
                Id = recetteUpdated.Id,
                nom = recetteUpdated.nom,
                temps_preparation = recetteUpdated.temps_preparation,
                temps_cuisson = recetteUpdated.temps_cuisson,
                difficulte = recetteUpdated.difficulte,
                etapes = createdEtapes,
                ingredients = newIngredients,
                categories = categoriesUpdatedRecette,
                photo = recetteUpdated.photo
            };

            // Commit de la transaction
            if (recetteComplete is not null)
                _UoW.Commit();

            return recetteComplete;
        }

        /// <summary>
        /// Supprime une recette ainsi que toutes ses relations (ingrédients, quantités, catégories et étapes).
        /// </summary>
        /// <param name="id">Identifiant de la recette à supprimer.</param>
        /// <returns>Retourne true si la suppression a réussi, false sinon.</returns>
        public async Task<bool> DeleteRecetteAsync(int id)
        {
            // Début de la transaction
            _UoW.BeginTransaction();

            // Suppression des relations recette/ingrédients (quantités associées)
            var relationDelete = await _UoW.QuantiteIngred.DeleteRecetteRelationsIngredientAsync(id);

            // Suppression des relations recette/catégories
            var res = await _UoW.Recettes.DeleteRecetteRelationsAsync(id);

            // Suppression des étapes associées à la recette
            var numLine = await _UoW.Etapes.DeleteEtapesRelationByIdRecetteAsync(id);

            // Suppression de la recette elle-même
            var result = await _UoW.Recettes.DeleteAsync(id);

            // Vérification que toutes les opérations ont réussi
            bool succes = result && numLine && res && relationDelete;

            // Validation de la transaction uniquement si toutes les suppressions ont réussi
            if (succes)
                _UoW.Commit();

            // Retourne le statut global de la suppression
            return succes;
        }


        #endregion Fin Gestion des recettes

        #region Gestion des Etapes

        /// <summary>
        /// Récupère toutes les étapes existantes.
        /// </summary>
        /// <returns>Une liste de <see cref="Etape"/>.</returns>
        public async Task<IEnumerable<Etape>> GetAllEtapesAsync()
        {
            return await _UoW.Etapes.GetAllAsync();
        }

        /// <summary>
        /// Récupère toutes les étapes associées à une recette spécifique.
        /// </summary>
        /// <param name="id">Identifiant de la recette.</param>
        /// <returns>Une liste d'étapes de la recette.</returns>
        public async Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int id)
        {
            return await _UoW.Etapes.GetEtapesByIdRecetteAsync(id);
        }

        /// <summary>
        /// Ajoute une nouvelle étape.
        /// </summary>
        /// <param name="newEtape">L'étape à créer.</param>
        /// <returns>L'étape créée.</returns>
        public async Task<Etape> AddEtapeAsync(Etape newEtape)
        {
            return await _UoW.Etapes.CreateAsync(newEtape);
        }

        /// <summary>
        /// Modifie une étape existante.
        /// </summary>
        /// <param name="updateEtape">L'étape avec les modifications.</param>
        /// <returns>L'étape mise à jour.</returns>
        public async Task<Etape> ModifyEtapeAsync(Etape updateEtape)
        {
            return await _UoW.Etapes.ModifyAsync(updateEtape);
        }

        /// <summary>
        /// Supprime une étape spécifique.
        /// </summary>
        /// <param name="key">Clé tuple (id_recette, numero étape) de l'étape.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        public async Task<bool> DeleteEtapeAsync((int, int) key)
        {
            return await _UoW.Etapes.DeleteAsync(key);
        }

        #endregion Fin Etapes

        #region Categories

        /// <summary>
        /// Récupère toutes les catégories.
        /// </summary>
        /// <returns>Une liste de <see cref="Categorie"/>.</returns>
        public async Task<IEnumerable<Categorie>> GetAllCategoriesAsync()
        {
            return await _UoW.Categories.GetAllAsync();
        }

        /// <summary>
        /// Récupère une catégorie par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de la catégorie.</param>
        /// <returns>La catégorie correspondante.</returns>
        public async Task<Categorie> GetCategorieByIdAsync(int id)
        {
            return await _UoW.Categories.GetAsync(id);
        }

        /// <summary>
        /// Ajoute une nouvelle catégorie.
        /// </summary>
        /// <param name="newCategorie">La catégorie à créer.</param>
        /// <returns>La catégorie créée.</returns>
        public async Task<Categorie> AddCategorieAsync(Categorie newCategorie)
        {
            return await _UoW.Categories.CreateAsync(newCategorie);
        }

        /// <summary>
        /// Modifie une catégorie existante.
        /// </summary>
        /// <param name="updateCategorie">La catégorie avec les modifications.</param>
        /// <returns>La catégorie mise à jour.</returns>
        public async Task<Categorie> ModifyCategorieAsync(Categorie updateCategorie)
        {
            return await _UoW.Categories.ModifyAsync(updateCategorie);
        }

        /// <summary>
        /// Supprime une catégorie.
        /// </summary>
        /// <param name="id">Identifiant de la catégorie à supprimer.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        public async Task<bool> DeleteCategorieAsync(int id)
        {
            _UoW.BeginTransaction();

            // Vérifie si la catégorie est encore utilisée par des recettes
            bool hasRelations = await _UoW.Categories.HasRecetteRelationsAsync(id);

            if (hasRelations)
            {
                _UoW.Rollback();
                throw new InvalidOperationException("Impossible de supprimer la catégorie car elle est encore associée à des recettes.");
            }

            // Supprime la catégorie
            var result = await _UoW.Categories.DeleteAsync(id);

            // Valide la transaction si la suppression a réussi
            if (result)
                _UoW.Commit();

            return result;
        }

        #endregion Fin Gestion Categories

        #region Gestion des relations entre Recettes et Catégories

        /// <summary>
        /// Récupère toutes les relations entre recettes et catégories.
        /// </summary>
        /// <returns>Une liste de <see cref="RecetteCategorieRelationship"/>.</returns>
        public async Task<IEnumerable<RecetteCategorieRelationship>> GetAllRecettesCategoriesAsync()
        {
            return await _UoW.Recettes.GetAllRecetteCategorieRelationshipAsync();
        }

        /// <summary>
        /// Ajoute une relation entre une recette et une catégorie.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la catégorie.</param>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>True si la relation a été ajoutée, sinon false.</returns>
        public async Task<bool> AddRecetteCategorieRelationshipAsync(int idCategorie, int idRecette)
        {
            return await _UoW.Recettes.AddRecetteCategorieRelationshipAsync(idCategorie, idRecette);
        }

        /// <summary>
        /// Supprime une relation entre une recette et une catégorie.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la catégorie.</param>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        public async Task<bool> RemoveRecetteCategorieRelationshipAsync(int idCategorie, int idRecette)
        {
            return await _UoW.Recettes.RemoveRecetteCategorieRelationshipAsync(idCategorie, idRecette);
        }

        /// <summary>
        /// Récupère toutes les recettes associées à une catégorie spécifique.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la catégorie.</param>
        /// <returns>Une liste de recettes associées.</returns>
        public async Task<IEnumerable<Recette>> GetRecettesByIdCategorieAsync(int idCategorie)
        {
            return await _UoW.Recettes.GetRecettesByIdCategorieAsync(idCategorie);
        }

        /// <summary>
        /// Récupère toutes les catégories associées à une recette spécifique.
        /// </summary>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>Une liste de catégories associées.</returns>
        public async Task<IEnumerable<Categorie>> GetCategoriesByIdRecetteAsync(int idRecette)
        {
            return await _UoW.Categories.GetCategoriesByIdRecetteAsync(idRecette);
        }

        /// <summary>
        /// Supprime toutes les relations entre une recette et ses catégories.
        /// </summary>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        public async Task<bool> DeleteRecetteRelationsAsync(int idRecette)
        {
            return await _UoW.Recettes.DeleteRecetteRelationsAsync(idRecette);
        }

        /// <summary>
        /// Supprime toutes les relations entre une catégorie et ses recettes.
        /// </summary>
        /// <param name="idCategorie">Identifiant de la catégorie.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        public async Task<bool> DeleteCategorieRelationsAsync(int idCategorie)
        {
            return await _UoW.Categories.DeleteCategorieRelationsAsync(idCategorie);
        }

        #endregion Fin Gestion des relations entre Recettes et Catégories

        #region Gestion Ingredients

        /// <summary>
        /// Récupère tous les ingrédients.
        /// </summary>
        /// <returns>Une liste de <see cref="Ingredient"/>.</returns>
        public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
        {
            return await _UoW.Ingredients.GetAllAsync();
        }

        /// <summary>
        /// Récupère un ingrédient par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de l'ingrédient.</param>
        /// <returns>L'ingrédient correspondant.</returns>
        public async Task<Ingredient> GetIngredientByIdAsync(int id)
        {
            return await _UoW.Ingredients.GetAsync(id);
        }

        /// <summary>
        /// Ajoute un nouvel ingrédient.
        /// </summary>
        /// <param name="newIngredient">L'ingrédient à créer.</param>
        /// <returns>L'ingrédient créé.</returns>
        public async Task<Ingredient> AddIngredientAsync(Ingredient newIngredient)
        {
            return await _UoW.Ingredients.CreateAsync(newIngredient);
        }

        /// <summary>
        /// Modifie un ingrédient existant.
        /// </summary>
        /// <param name="updateIngredient">L'ingrédient avec les modifications.</param>
        /// <returns>L'ingrédient mis à jour.</returns>
        public async Task<Ingredient> ModifyIngredientAsync(Ingredient updateIngredient)
        {
            return await _UoW.Ingredients.ModifyAsync(updateIngredient);
        }

        /// <summary>
        /// Supprime un ingrédient.
        /// </summary>
        /// <param name="id">Identifiant de l'ingrédient à supprimer.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        public async Task<bool> DeleteIngredientAsync(int id)
        {
            return await _UoW.Ingredients.DeleteAsync(id);
        }

        #endregion Fin Gestion Ingredients

        #region Gestion des relations entre Recettes et Ingredients

        /// <summary>
        /// Récupère toutes les relations recette / ingrédients.
        /// </summary>
        /// <returns>Une liste de <see cref="QuantiteIngredients"/>.</returns>
        public async Task<IEnumerable<QuantiteIngredients>> GetQuantiteIngredientsAsync()
        {
            return await _UoW.QuantiteIngred.GetAllAsync();
        }

        /// <summary>
        /// Récupère une relation recette / ingrédient par sa clé.
        /// </summary>
        /// <param name="key">Clé tuple (id_ingredient, id_recette).</param>
        /// <returns>La relation correspondante.</returns>
        public async Task<QuantiteIngredients> GetQuantiteIngredientsByIdAsync((int, int) key)
        {
            return await _UoW.QuantiteIngred.GetAsync(key);
        }

        /// <summary>
        /// Ajoute une relation entre une recette et un ingrédient.
        /// </summary>
        /// <param name="CreateRelationRI">Relation à créer.</param>
        /// <returns>La relation créée.</returns>
        public async Task<QuantiteIngredients> AddRecetteIngredientRelationshipAsync(QuantiteIngredients CreateRelationRI)
        {
            return await _UoW.QuantiteIngred.CreateAsync(CreateRelationRI);
        }

        /// <summary>
        /// Modifie une relation existante entre une recette et un ingrédient.
        /// </summary>
        /// <param name="updateRelationRI">Relation avec les modifications.</param>
        /// <returns>La relation mise à jour.</returns>
        public async Task<QuantiteIngredients> updateRecetteIngredientRelationshipAsync(QuantiteIngredients updateRelationRI)
        {
            return await _UoW.QuantiteIngred.ModifyAsync(updateRelationRI);
        }

        /// <summary>
        /// Supprime une relation spécifique entre une recette et un ingrédient.
        /// </summary>
        /// <param name="key">Clé tuple (id_ingredient, id_recette).</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        public async Task<bool> RemoveRecetteIngredientRelationshipAsync((int, int) key)
        {
            return await _UoW.QuantiteIngred.DeleteAsync(key);
        }

        #region Méthodes spécifiques

        /// <summary>
        /// Récupère toutes les recettes utilisant un ingrédient spécifique.
        /// </summary>
        /// <param name="idIngredient">Identifiant de l'ingrédient.</param>
        /// <returns>Liste des recettes correspondantes.</returns>
        public async Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient)
        {
            return await _UoW.QuantiteIngred.GetRecettesByIdIngredientAsync(idIngredient);
        }

        /// <summary>
        /// Récupère toutes les relations ingrédients pour une recette donnée.
        /// </summary>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>Liste des quantités d'ingrédients associées à la recette.</returns>
        public async Task<IEnumerable<QuantiteIngredients>> GetIngredientsByIdRecetteAsync(int idRecette)
        {
            return await _UoW.QuantiteIngred.GetIngredientsByIdRecetteAsync(idRecette);
        }

        /// <summary>
        /// Supprime toutes les relations entre une recette et ses ingrédients.
        /// </summary>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        public async Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette)
        {
            return await _UoW.QuantiteIngred.DeleteRecetteRelationsIngredientAsync(idRecette);
        }

        /// <summary>
        /// Supprime toutes les relations entre un ingrédient et les recettes qui l'utilisent.
        /// </summary>
        /// <param name="idIngredient">Identifiant de l'ingrédient.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        public async Task<bool> DeleteIngredientRelationsAsync(int idIngredient)
        {
            return await _UoW.QuantiteIngred.DeleteIngredientRelationsRecetteAsync(idIngredient);
        }

        #endregion Méthodes spécifiques

        #endregion Fin Gestion des relations entre Recettes et Ingredients


    }
}
