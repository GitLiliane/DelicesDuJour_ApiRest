using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace DelicesDuJour_ApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuantiteIngredientsController : ControllerBase
    {
        private readonly IBiblioService _biblioService;

        public QuantiteIngredientsController(IBiblioService biblioService)
        {
            _biblioService = biblioService;
        }

        [Authorize(Roles = "Administrateur, Utilisateur")]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuantiteIngredients()
        {
            var quantites = await _biblioService.GetQuantiteIngredientsAsync();

            IEnumerable<QuantiteIngredientsDTO> response = quantites.Select(q => new QuantiteIngredientsDTO()
            {
                id_ingredient = q.id_ingredient,
                id_recette = q.id_recette,
                quantite = q.quantite             
            });

            return Ok(response);
        }

        [Authorize(Roles = "Administrateur, Utilisateur")]
        [HttpGet(nameof(GetQuantiteById) + "/{id_ingredient}/{id_recette}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQuantiteById([FromRoute] int id_ingredient, [FromRoute] int id_recette)
        {            
            (int, int) key = new();
            key = (id_ingredient, id_recette);

            var quantite = await _biblioService.GetQuantiteIngredientsByIdAsync(key);

            if (quantite is null)
                return NotFound();

            QuantiteIngredientsDTO response = new()
            {
                id_ingredient = quantite.id_ingredient,
                id_recette = quantite.id_recette,
                quantite = quantite.quantite
            };

            return Ok(response);
        }

        [Authorize(Roles = "Administrateur")]
        [HttpPost(nameof(AddRecetteIngredientRelationship) + "/{idIngredient}/{idRecette}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddRecetteIngredientRelationship([FromRoute] int idIngredient, [FromRoute] int idRecette, IValidator<CreateQuantiteIngredientsDTO> validator, [FromBody] CreateQuantiteIngredientsDTO createQuantiteIngredientsDTO)
        {
            validator.ValidateAndThrow(createQuantiteIngredientsDTO);

            QuantiteIngredients newQuantiteIngredients = new()
            {
                id_ingredient = createQuantiteIngredientsDTO.id_ingredient,
                id_recette = createQuantiteIngredientsDTO.id_recette,
                quantite = createQuantiteIngredientsDTO.quantite
            };
            
            var request = await _biblioService.AddRecetteIngredientRelationshipAsync(newQuantiteIngredients);

            if (request is null)
                return BadRequest("Invalid quantité ingrédients data.");

            QuantiteIngredientsDTO response = new()
            {
                id_ingredient = request.id_ingredient,
                id_recette = request.id_recette,
                quantite = request.quantite
            };

            return Ok(response);
        }

        [Authorize(Roles = "Administrateur")]
        [HttpPut(nameof(UpdateRecetteIngredientRelationship) + "/{idIngredient}/{idRecette}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRecetteIngredientRelationship(IValidator<UpdateQuantiteIngredientsDTO> validator, [FromRoute] int idIngredient, [FromRoute] int idRecette, [FromBody] UpdateQuantiteIngredientsDTO request)
        {
            validator.ValidateAndThrow(request);

            QuantiteIngredients quantiteIngredients = new()
            {
                id_ingredient = request.id_ingredient,
                id_recette = request.id_recette,
                quantite = request.quantite
            };

            var modifiedQuantite = await _biblioService.updateRecetteIngredientRelationshipAsync(quantiteIngredients);

            if (modifiedQuantite is null)
                return BadRequest("Invalid quantité ingrédients.");

            QuantiteIngredientsDTO response = new()
            {
                id_ingredient = modifiedQuantite.id_ingredient,
                id_recette = modifiedQuantite.id_recette,
                quantite = modifiedQuantite.quantite
            };

            return Ok(response);
        }

        [Authorize(Roles = "Administrateur")]
        [HttpDelete(nameof(RemoveRecetteIngredientRelationship) + "/{idIngredient}/{idRecette}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRecetteIngredientRelationship([FromRoute] int idIngredient, [FromRoute] int idRecette)
        {
            (int, int) key = new();
            key = (idIngredient, idRecette);

            var success = await _biblioService.RemoveRecetteIngredientRelationshipAsync(key);
            return success ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Administrateur, Utilisateur")]
        [HttpGet(nameof(GetRecettesByIdIngredient) + "/{idIngredient}")]
        public async Task<IActionResult> GetRecettesByIdIngredient([FromRoute] int idIngredient)
        {
            var response = await _biblioService.GetRecettesByIdIngredientAsync(idIngredient);
            return Ok(response);
        }

        [Authorize(Roles = "Administrateur, Utilisateur")]
        [HttpGet(nameof(GetIngredientsByIdRecette) + "/{idRecette}")]
        public async Task<IActionResult> GetIngredientsByIdRecette([FromRoute] int idRecette)
        {
            var response = await _biblioService.GetIngredientsByIdRecetteAsync(idRecette);

            return Ok(response);
        }

        [Authorize(Roles = "Administrateur")]
        [HttpDelete(nameof(DeleteRecetteRelationsIngredient) + "/{idRecette}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRecetteRelationsIngredient([FromRoute] int idRecette)
        {
            var success = await _biblioService.DeleteRecetteRelationsIngredientAsync(idRecette);
            return success ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Administrateur")]
        [HttpDelete(nameof(DeleteIngredientRelations) + "/{idIngredient}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteIngredientRelations([FromRoute] int idIngredient)
        {
            var success = await _biblioService.DeleteIngredientRelationsAsync(idIngredient);
            return success ? NoContent() : NotFound();
        }
    }
}
