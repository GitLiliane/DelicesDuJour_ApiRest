using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;


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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetRecetteById([FromRoute] int id)
        {
            var recette = await _biblioservice.GetRecetteByIdAsync(id);

            if (recette is null)
                return NotFound();
           
            List<IngredientDTO> ingredientDTOs = new();

            foreach (Ingredient ingredient in recette.ingredients)
            {
                IngredientDTO ingredientDTO = new()
                {
                    id = ingredient.id,
                    nom = ingredient.nom,
                    quantite = ingredient.quantite
                };
                                
                ingredientDTOs.Add(ingredientDTO);
            }

            List<EtapeDTO> etapeDTOs = new();

            foreach (Etape etape in recette.etapes)
            {
                EtapeDTO etapeDTO = new()
                {
                    numero = etape.numero,
                    titre = etape.titre,
                    texte = etape.texte
                };

                etapeDTOs.Add(etapeDTO);
            }

            List<CategorieDTO> categorieDTOs = new();

            foreach (Categorie categorie in recette.categories)
            {
                CategorieDTO categorieDTO = new()
                {
                    id = categorie.id,
                    nom = categorie.nom
                };

                categorieDTOs.Add(categorieDTO);
            }

            RecetteDTO recetteDTO = new()
            {
                Id = recette.Id,
                nom = recette.nom,
                temps_preparation = recette.temps_preparation,
                temps_cuisson = recette.temps_cuisson,
                difficulte = recette.difficulte,
                etapes = etapeDTOs,
                ingredients = ingredientDTOs,               
                categories = categorieDTOs,
                //photo = recette.photo
            };

            return Ok(recetteDTO);
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> CreateRecette(IValidator<CreateRecetteDTO> validator, [FromBody] CreateRecetteDTO request)
        {
            validator.ValidateAndThrow(request);

            List<Ingredient> ingredients = new();

            foreach (IngredientDTO ingredientDTO in request.ingredients)
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

            foreach (CreateEtapeDTO createEtapeDTO in request.etapes)
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

            foreach (CategorieDTO categorieDTO in request.categories)
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
                nom = request.nom,
                temps_preparation = request.temps_preparation,
                temps_cuisson = request.temps_cuisson,
                difficulte = request.difficulte,
                etapes = etapes,
                ingredients = ingredients,
                categories = categories                
            };

            var newRecette = await _biblioservice.AddRecetteAsync(recette);

            if (newRecette == null)
                return BadRequest("Invalid Reciep data.");

            List<IngredientDTO> ingredientDtos = new();

            foreach (Ingredient ingredient in newRecette.ingredients)
            {
                IngredientDTO ingredientDTO = new()
                {
                    id = ingredient.id,
                    nom = ingredient.nom,
                    quantite = ingredient.quantite
                };

                ingredientDtos.Add(ingredientDTO);
            }

            List<EtapeDTO> etapeDtos = new();

            foreach (Etape etape in newRecette.etapes)
            {
                EtapeDTO etapeDTO = new()
                {
                    numero = etape.numero,
                    titre = etape.titre,
                    texte = etape.texte
                };

                etapeDtos.Add(etapeDTO);
            }

            List<CategorieDTO> categorieDtos = new();

            foreach (Categorie categorie in newRecette.categories)
            {
                CategorieDTO categorieDTO = new()
                {
                    id = categorie.id,
                    nom = categorie.nom
                };

                categorieDtos.Add(categorieDTO);
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
                categories = categorieDtos
            };

            return CreatedAtAction(nameof(GetRecetteById), new { id = newRecetteDTO.Id }, newRecetteDTO);

        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdateRecette(IValidator<UpdateRecetteDTO> validator, [FromRoute] int id, [FromBody] UpdateRecetteDTO updateRecetteDTO)
        {
            validator.ValidateAndThrow(updateRecetteDTO);

            Recette updateR = new()
            {
                Id = id,
                nom = updateRecetteDTO.nom,
                temps_preparation = updateRecetteDTO.temps_preparation,
                temps_cuisson = updateRecetteDTO.temps_cuisson,
                difficulte = updateRecetteDTO.difficulte,
                //etapes = updateRecetteDTO.etapes,
                //ingredients = updateRecetteDTO.ingredients,
                //categories = updateRecetteDTO.categories,
                //photo = updateRecetteDTO.photo
            };

            var updateRecette = await _biblioservice.ModifyRecetteAsync(updateR);

            if (updateRecette is null)
                return BadRequest("Invalid reciep.");

            RecetteDTO recetteDTO = new()
            {
                Id = updateRecette.Id,
                nom = updateRecette.nom,
                temps_preparation = updateRecette.temps_preparation,
                temps_cuisson = updateRecette.temps_cuisson,
                difficulte = updateRecette.difficulte,
                //etapes = updateRecette.etapes,
                //ingredients = updateRecette.ingredients,
                //categories = updateRecette.categories,
                //photo = updateRecette.photo
            };

            return Ok(recetteDTO);

        }

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
