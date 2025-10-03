using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DelicesDuJour_ApiRest.Controllers
{
    [Authorize(Roles = "Administrateur,Utilisateur")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IBiblioService _biblioService;

        public CategoriesController(IBiblioService biblioService)
        {
            _biblioService = biblioService;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _biblioService.GetAllCategoriesAsync();

            IEnumerable<CategorieDTO> response = categories.Select(c => new CategorieDTO()
            {
                id = c.id,
                nom = c.nom             
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategorieById([FromRoute] int id)
        {
            var categorie = await _biblioService.GetCategorieByIdAsync(id);

            if (categorie is null)
                return NotFound();

            CategorieDTO response = new()
            {
                id = categorie.id,
                nom = categorie.nom
            };

            return Ok(response);
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategorie(IValidator<CreateCategorieDTO> validator, [FromBody] CreateCategorieDTO request)
        {
            validator.ValidateAndThrow(request);

            Categorie categorie = new()
            {
                nom = request.nom
            };

            var newCategorie = await _biblioService.AddCategorieAsync(categorie);

            if (newCategorie is null)
                return BadRequest("Invalid categorie data.");

            CategorieDTO response = new()
            {
                id = newCategorie.id,
                nom = newCategorie.nom
            };

            return CreatedAtAction(nameof(GetCategorieById), new { id = response.id }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCategorie(IValidator<UpdateCategorieDTO> validator, [FromRoute] int id, [FromBody] UpdateCategorieDTO request)
        {
            validator.ValidateAndThrow(request);

            Categorie categorie = new()
            {
                id = id,
                nom = request.nom
            };

            var modifiedCategorie = await _biblioService.ModifyCategorieAsync(categorie);

            if (modifiedCategorie is null)
                return BadRequest("Invalid categorie.");

            CategorieDTO response = new()
            {
                id = modifiedCategorie.id,
                nom = modifiedCategorie.nom
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategorie([FromRoute] int id)
        {
            var success = await _biblioService.DeleteCategorieAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
