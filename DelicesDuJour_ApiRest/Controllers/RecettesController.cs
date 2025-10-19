using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.Text.Json;

namespace DelicesDuJour_ApiRest.Controllers
{
    /// <summary>
    /// Contrôleur API gérant les opérations CRUD sur les recettes.
    /// </summary>
    [Route("api/[controller]")] // Route de base : api/Recettes
    [ApiController] // Indique qu'il s'agit d'un contrôleur API
    public class RecettesController : ControllerBase
    {
        /// <summary>
        /// Service métier pour gérer les recettes.
        /// </summary>
        private readonly IBiblioService _biblioservice;

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur <see cref="RecettesController"/>.
        /// </summary>
        /// <param name="biblioService">Service pour gérer les recettes.</param>
        public RecettesController(IBiblioService biblioService)
        {
            _biblioservice = biblioService; // Injection du service métier
        }

        /// <summary>
        /// Récupère toutes les recettes.
        /// </summary>
        /// <returns>Une liste de <see cref="RecetteDTO"/>.</returns>
        [Authorize(Roles = "Administrateur, Utilisateur")]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecettes()
        {
            // Récupération de toutes les recettes via le service métier
            var recettes = await _biblioservice.GetAllRecettesAsync();

            // Transformation des recettes en DTO pour la réponse
            IEnumerable<RecetteDTO> response = recettes.Select(r => new RecetteDTO()
            {
                Id = r.Id,
                nom = r.nom,
                temps_preparation = r.temps_preparation,
                temps_cuisson = r.temps_cuisson,
                difficulte = r.difficulte,
            });

            // Retourne la liste de recettes avec un code HTTP 200
            return Ok(response);
        }

        /// <summary>
        /// Récupère une recette par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de la recette.</param>
        /// <returns>La recette correspondante ou un code 404 si non trouvée.</returns>
        [Authorize(Roles = "Administrateur, Utilisateur")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRecetteById([FromRoute] int id)
        {
            // Récupération de la recette complète
            var recette = await _biblioservice.GetRecetteByIdAsync(id);

            // Si la recette n'existe pas, retourne 404
            if (recette is null)
                return NotFound();

            // Conversion des ingrédients en DTO
            var ingredientDTOs = recette.ingredients.Select(ingredient => new IngredientDTO
            {
                id = ingredient.id,
                nom = ingredient.nom,
                quantite = ingredient.quantite
            }).ToList();

            // Conversion des étapes en DTO
            var etapeDTOs = recette.etapes.Select(etape => new EtapeDTO
            {
                numero = etape.numero,
                titre = etape.titre,
                texte = etape.texte
            }).ToList();

            // Conversion des catégories en DTO
            var categorieDTOs = recette.categories.Select(categorie => new CategorieDTO
            {
                id = categorie.id,
                nom = categorie.nom
            }).ToList();

            // Construction d'une URL absolue pour la photo
            string? fullPhotoUrl = null;
            if (!string.IsNullOrEmpty(recette.photo))
            {
                var request = HttpContext.Request;
                fullPhotoUrl = $"{request.Scheme}://{request.Host}/{recette.photo.TrimStart('/')}";
            }

            // Création du DTO final pour la réponse
            var recetteDTO = new RecetteDTO
            {
                Id = recette.Id,
                nom = recette.nom,
                temps_preparation = recette.temps_preparation,
                temps_cuisson = recette.temps_cuisson,
                difficulte = recette.difficulte,
                etapes = etapeDTOs,
                ingredients = ingredientDTOs,
                categories = categorieDTOs,
                photo = fullPhotoUrl
            };

            // Retourne la recette avec un code HTTP 200
            return Ok(recetteDTO);
        }

        /// <summary>
        /// Crée une nouvelle recette.
        /// </summary>
        /// <param name="validator">Validateur pour <see cref="CreateRecetteDTO"/>.</param>
        /// <param name="request">Données JSON de la recette.</param>
        /// <param name="photoFile">Fichier image de la recette.</param>
        /// <returns>La recette créée avec code HTTP 201.</returns>
        [Authorize(Roles = "Administrateur")]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRecette(IValidator<CreateRecetteDTO> validator, [FromForm] string? request, IFormFile? photoFile)
        {
            CreateRecetteDTO dto;

            // Lecture du corps de la requête si le paramètre "request" est vide
            if (string.IsNullOrEmpty(request))
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                // Désérialisation JSON vers l'objet CreateRecetteDTO
                dto = JsonSerializer.Deserialize<CreateRecetteDTO>(
                    body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                )!;
            }
            else
            {
                // Désérialisation JSON depuis le paramètre "request"
                dto = JsonSerializer.Deserialize<CreateRecetteDTO>(
                    request,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                )!;
            }

            // Validation du DTO
            await validator.ValidateAndThrowAsync(dto);

            // Gestion de l'image si fournie
            if (photoFile != null && photoFile.Length > 0)
            {
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "recettes");

                // Création du dossier si inexistant
                if (!Directory.Exists(imagesPath))
                    Directory.CreateDirectory(imagesPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photoFile.FileName)}";
                var filePath = Path.Combine(imagesPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photoFile.CopyToAsync(stream); // Copie du fichier sur le serveur
                }

                // Mise à jour du chemin de la photo dans le DTO
                dto.photo = $"/images/recettes/{fileName}";
            }

            // Transformation des DTO en objets métier
            List<Ingredient> ingredients = new();
            foreach (IngredientDTO ingredientDTO in dto.ingredients)
            {
                Ingredient ingredient = new()
                {
                    id = ingredientDTO.id,
                    nom = ingredientDTO.nom,
                    quantite = ingredientDTO.quantite
                };
                ingredients.Add(ingredient);
            }

            List<Etape> etapes = new();
            foreach (CreateEtapeDTO createEtapeDTO in dto.etapes)
            {
                Etape etape = new()
                {
                    numero = createEtapeDTO.numero,
                    titre = createEtapeDTO.titre,
                    texte = createEtapeDTO.texte
                };
                etapes.Add(etape);
            }

            List<Categorie> categories = new();
            foreach (CategorieDTO categorieDTO in dto.categories)
            {
                Categorie categorie = new()
                {
                    id = categorieDTO.id,
                    nom = categorieDTO.nom
                };
                categories.Add(categorie);
            }

            // Création de l'objet Recette complet
            Recette recette = new()
            {
                nom = dto.nom,
                temps_preparation = dto.temps_preparation,
                temps_cuisson = dto.temps_cuisson,
                difficulte = dto.difficulte,
                etapes = etapes,
                ingredients = ingredients,
                categories = categories,
                photo = dto.photo
            };

            // Ajout de la recette via le service métier
            var newRecette = await _biblioservice.AddRecetteAsync(recette, photoFile);

            if (newRecette == null)
                return BadRequest("Invalid Reciep data.");

            // Conversion de la recette créée en DTO pour la réponse
            List<IngredientDTO> ingredientDtos = new();
            foreach (Ingredient ingredient in newRecette.ingredients)
            {
                ingredientDtos.Add(new IngredientDTO
                {
                    id = ingredient.id,
                    nom = ingredient.nom,
                    quantite = ingredient.quantite
                });
            }

            List<EtapeDTO> etapeDtos = new();
            foreach (Etape etape in newRecette.etapes)
            {
                etapeDtos.Add(new EtapeDTO
                {
                    numero = etape.numero,
                    titre = etape.titre,
                    texte = etape.texte
                });
            }

            List<CategorieDTO> categorieDtos = new();
            foreach (Categorie categorie in newRecette.categories)
            {
                categorieDtos.Add(new CategorieDTO
                {
                    id = categorie.id,
                    nom = categorie.nom
                });
            }

            // Construction du DTO final de la recette créée
            RecetteDTO newRecetteDTO = new()
            {
                Id = newRecette.Id,
                nom = newRecette.nom,
                temps_preparation = newRecette.temps_preparation,
                temps_cuisson = newRecette.temps_cuisson,
                difficulte = newRecette.difficulte,
                etapes = etapeDtos,
                ingredients = ingredientDtos,
                categories = categorieDtos,
                photo = newRecette.photo
            };

            // Retourne la recette créée avec code HTTP 201
            return CreatedAtAction(nameof(GetRecetteById), new { id = newRecetteDTO.Id }, newRecetteDTO);
        }

        /// <summary>
        /// Modifie une recette existante.
        /// </summary>
        /// <param name="validator">Validateur pour <see cref="UpdateRecetteDTO"/>.</param>
        /// <param name="id">Identifiant de la recette à modifier.</param>
        /// <param name="request">Données JSON de la recette.</param>
        /// <param name="photoFile">Fichier image de la recette.</param>
        /// <returns>La recette mise à jour ou un code 400 si la modification échoue.</returns>
        [Authorize(Roles = "Administrateur")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRecette(IValidator<UpdateRecetteDTO> validator, [FromRoute] int id, [FromForm] string? request, IFormFile? photoFile)
        {
            UpdateRecetteDTO updateRecetteDTO;

            // Lecture du corps de la requête ou du paramètre "request"
            if (string.IsNullOrEmpty(request))
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                updateRecetteDTO = JsonSerializer.Deserialize<UpdateRecetteDTO>(
                    body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                )!;
            }
            else
            {
                updateRecetteDTO = JsonSerializer.Deserialize<UpdateRecetteDTO>(
                    request,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                )!;
            }

            // Validation du DTO
            await validator.ValidateAndThrowAsync(updateRecetteDTO);

            // Gestion de l'image si fournie
            if (photoFile != null && photoFile.Length > 0)
            {
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "recettes");
                if (!Directory.Exists(imagesPath))
                    Directory.CreateDirectory(imagesPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photoFile.FileName)}";
                var filePath = Path.Combine(imagesPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photoFile.CopyToAsync(stream);
                }

                updateRecetteDTO.photo = $"/images/recettes/{fileName}";
            }

            // Transformation des DTO en objets métier
            List<Ingredient> ingredients = new();
            foreach (IngredientDTO ingredientDTO in updateRecetteDTO.ingredients)
            {
                Ingredient ingredient = new()
                {
                    id = ingredientDTO.id,
                    nom = ingredientDTO.nom,
                    quantite = ingredientDTO.quantite
                };
                ingredients.Add(ingredient);
            }

            List<Etape> etapes = new();
            foreach (UpdateEtapeDTO updateEtapeDTO in updateRecetteDTO.etapes)
            {
                Etape etape = new()
                {
                    numero = updateEtapeDTO.numero,
                    titre = updateEtapeDTO.titre,
                    texte = updateEtapeDTO.texte
                };
                etapes.Add(etape);
            }

            List<Categorie> categories = new();
            foreach (CategorieDTO categorieDTO in updateRecetteDTO.categories)
            {
                Categorie categorie = new()
                {
                    id = categorieDTO.id,
                    nom = categorieDTO.nom
                };
                categories.Add(categorie);
            }

            // Création de l'objet Recette complet pour mise à jour
            Recette updateRecette = new()
            {
                Id = id,
                nom = updateRecetteDTO.nom,
                temps_preparation = updateRecetteDTO.temps_preparation,
                temps_cuisson = updateRecetteDTO.temps_cuisson,
                difficulte = updateRecetteDTO.difficulte,
                etapes = etapes,
                ingredients = ingredients,
                categories = categories,
                photo = updateRecetteDTO.photo
            };

            // Mise à jour de la recette via le service métier
            var recetteUpdated = await _biblioservice.ModifyRecetteAsync(updateRecette);

            if (recetteUpdated is null)
                return BadRequest("Invalid reciep.");

            // Transformation en DTO pour la réponse
            List<IngredientDTO> ingredientsDTO = new();
            foreach (Ingredient ingredient in recetteUpdated.ingredients)
            {
                IngredientDTO ingredientDTO = new()
                {
                    id = ingredient.id,
                    nom = ingredient.nom,
                    quantite = ingredient.quantite
                };
                ingredientsDTO.Add(ingredientDTO);
            }

            List<EtapeDTO> etapesDTO = new();
            foreach (Etape etape in recetteUpdated.etapes)
            {
                EtapeDTO etapeDTO = new()
                {
                    id_recette = etape.id_recette,
                    numero = etape.numero,
                    titre = etape.titre,
                    texte = etape.texte
                };
                etapesDTO.Add(etapeDTO);
            }

            List<CategorieDTO> categoriesDTO = new();
            foreach (Categorie categorie in recetteUpdated.categories)
            {
                CategorieDTO categorieDTO = new()
                {
                    id = categorie.id,
                    nom = categorie.nom
                };
                categoriesDTO.Add(categorieDTO);
            }

            // Construction du DTO final pour la réponse
            RecetteDTO recetteDTO = new()
            {
                Id = recetteUpdated.Id,
                nom = recetteUpdated.nom,
                temps_preparation = recetteUpdated.temps_preparation,
                temps_cuisson = recetteUpdated.temps_cuisson,
                difficulte = recetteUpdated.difficulte,
                etapes = etapesDTO,
                ingredients = ingredientsDTO,
                categories = categoriesDTO,
                photo = recetteUpdated.photo
            };

            // Retourne la recette mise à jour avec code HTTP 200
            return Ok(recetteDTO);
        }

        /// <summary>
        /// Supprime une recette par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de la recette à supprimer.</param>
        /// <returns>Code 204 si supprimée, 404 si non trouvée.</returns>
        [Authorize(Roles = "Administrateur")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRecette(int id)
        {
            // Suppression de la recette via le service métier
            var success = await _biblioservice.DeleteRecetteAsync(id);

            // Retourne 204 si succès, 404 sinon
            return success ? NoContent() : NotFound();
        }
    }
}
