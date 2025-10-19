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
    [Route("api/[controller]")]
    [ApiController]
    public class RecettesController : ControllerBase
    {
        private readonly IBiblioService _biblioservice;

        public RecettesController(IBiblioService biblioService)
        {
            _biblioservice = biblioService;
        }

        [Authorize(Roles = "Administrateur, Utilisateur")]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecettes()
        {
            var recettes = await _biblioservice.GetAllRecettesAsync();

            IEnumerable<RecetteDTO> response = recettes.Select(r => new RecetteDTO()
            {
                Id = r.Id,
                nom = r.nom,
                temps_preparation = r.temps_preparation,
                temps_cuisson = r.temps_cuisson,
                difficulte = r.difficulte,
                //etapes = r.etapes,
                //ingredients = r.ingredients,
                //avis = r.avis,
                //categories =r.categories,
                //photo = r.photo
            });

            return Ok(response);
        }

        [Authorize(Roles = "Administrateur, Utilisateur")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRecetteById([FromRoute] int id)
        {
            // 🔍 Récupération de la recette complète
            var recette = await _biblioservice.GetRecetteByIdAsync(id);

            if (recette is null)
                return NotFound();

            // 🧂 Conversion des ingrédients en DTO
            var ingredientDTOs = recette.ingredients.Select(ingredient => new IngredientDTO
            {
                id = ingredient.id,
                nom = ingredient.nom,
                quantite = ingredient.quantite
            }).ToList();

            // 📝 Conversion des étapes en DTO
            var etapeDTOs = recette.etapes.Select(etape => new EtapeDTO
            {
                numero = etape.numero,
                titre = etape.titre,
                texte = etape.texte
            }).ToList();

            // 🏷️ Conversion des catégories en DTO
            var categorieDTOs = recette.categories.Select(categorie => new CategorieDTO
            {
                id = categorie.id,
                nom = categorie.nom
            }).ToList();

            // 🖼️ Construction d’une URL absolue correcte pour la photo
            string? fullPhotoUrl = null;
            if (!string.IsNullOrEmpty(recette.photo))
            {
                var request = HttpContext.Request;
                // Ajoute le "/" manquant entre le host et le chemin relatif
                fullPhotoUrl = $"{request.Scheme}://{request.Host}/{recette.photo.TrimStart('/')}";
            }

            // 🍽️ Construction du DTO final
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
                photo = fullPhotoUrl // URL absolue propre
            };

            return Ok(recetteDTO);
        }


        [Authorize(Roles = "Administrateur")]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRecette(IValidator<CreateRecetteDTO> validator,[FromForm] string? request,IFormFile? photoFile)
        {
            CreateRecetteDTO dto;

            if (string.IsNullOrEmpty(request))
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                dto = JsonSerializer.Deserialize<CreateRecetteDTO>(
                    body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                )!;
            }
            else
            {
                dto = JsonSerializer.Deserialize<CreateRecetteDTO>(
                    request,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                )!;
            }

            await validator.ValidateAndThrowAsync(dto);

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

                dto.photo = $"/images/recettes/{fileName}";
            }

            // 🔽 À partir d’ici : ton code d’origine (inchangé)
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

            Recette recette = new()
            {
                nom = dto.nom,
                temps_preparation = dto.temps_preparation,
                temps_cuisson = dto.temps_cuisson,
                difficulte = dto.difficulte,
                etapes = etapes,
                ingredients = ingredients,
                categories = categories,
                photo = dto.photo // 🖼️ Ajouté pour enregistrer le lien image
            };

            var newRecette = await _biblioservice.AddRecetteAsync(recette, photoFile);

            if (newRecette == null)
                return BadRequest("Invalid Reciep data.");

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
                photo = newRecette.photo // 🖼️ renvoie aussi le lien
            };

            return CreatedAtAction(nameof(GetRecetteById), new { id = newRecetteDTO.Id }, newRecetteDTO);
        }


        [Authorize(Roles = "Administrateur")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRecette(IValidator<UpdateRecetteDTO> validator, [FromRoute] int id, [FromForm] string? request, IFormFile? photoFile)
        {
            UpdateRecetteDTO updateRecetteDTO;

            // ✅ Lecture des données selon le type de contenu
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

            // ✅ Validation
            await validator.ValidateAndThrowAsync(updateRecetteDTO);

            // ✅ Gestion de la photo si un fichier a été envoyé
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

            // --- Ton code initial (inchangé) ---
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

            Recette updateRecette = new()
            {
                Id = updateRecetteDTO.Id,
                nom = updateRecetteDTO.nom,
                temps_preparation = updateRecetteDTO.temps_preparation,
                temps_cuisson = updateRecetteDTO.temps_cuisson,
                difficulte = updateRecetteDTO.difficulte,
                etapes = etapes,
                ingredients = ingredients,
                categories = categories,
                photo = updateRecetteDTO.photo // 🖼️ Peut être null si aucune image modifiée
            };

            var recetteUpdated = await _biblioservice.ModifyRecetteAsync(updateRecette);

            if (recetteUpdated is null)
                return BadRequest("Invalid reciep.");

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

            return Ok(recetteDTO);
        }

        [Authorize(Roles = "Administrateur")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteRecette(int id)
        {
            var sucess = await _biblioservice.DeleteRecetteAsync(id);

            return sucess ? NoContent() : NotFound();
        }
    }
}
