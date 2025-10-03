using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DelicesDuJour_ApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IBiblioService _biblioService;

        public IngredientsController(IBiblioService biblioService)
        {
            _biblioService = biblioService;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIngredients()
        {
            var ingredients = await _biblioService.GetAllIngredientsAsync();

            IEnumerable<IngredientDTO> response = ingredients.Select(i => new IngredientDTO()
            {
                id = i.id,
                nom = i.nom              
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetIngredientById([FromRoute] int id)
        {
            var ingredient = await _biblioService.GetIngredientByIdAsync(id);

            if (ingredient is null)
                return NotFound();

            IngredientDTO response = new()
            {
                id = ingredient.id,
                nom = ingredient.nom
            };

            return Ok(response);
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateIngredient(IValidator<CreateIngredientDTO> validator, [FromBody] CreateIngredientDTO request)
        {
            validator.ValidateAndThrow(request);

            Ingredient ingredient = new()
            {
                nom = request.nom
            };

            var newIngredient = await _biblioService.AddIngredientAsync(ingredient);

            if (newIngredient is null)
                return BadRequest("Invalid ingredient data.");

            IngredientDTO response = new()
            {
                id = newIngredient.id,
                nom = newIngredient.nom
            };

            return CreatedAtAction(nameof(GetIngredientById), new { id = response.id }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateIngredient(IValidator<UpdateIngredientDTO> validator, [FromRoute] int id, [FromBody] UpdateIngredientDTO request)
        {
            validator.ValidateAndThrow(request);

            Ingredient ingredient = new()
            {
                id = id,
                nom = request.nom                
            };

            var modifiedIngredient = await _biblioService.ModifyIngredientAsync(ingredient);

            if (modifiedIngredient is null)
                return BadRequest("Invalid ingredient.");

            IngredientDTO response = new()
            {
                id = modifiedIngredient.id,
                nom = modifiedIngredient.nom
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteIngredient([FromRoute] int id)
        {
            var success = await _biblioService.DeleteIngredientAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
