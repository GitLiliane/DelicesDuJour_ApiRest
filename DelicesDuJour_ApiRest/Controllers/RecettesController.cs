using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;


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
                photo = r.photo
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

            RecetteDTO recetteDTO = new()
            {
                Id = recette.Id,
                nom = recette.nom,
                temps_preparation = recette.temps_preparation,
                temps_cuisson = recette.temps_cuisson,
                difficulte = recette.difficulte,
                //etapes = recette.etapes,
                //ingredients = recette.ingredients,
                //avis = recette.avis,
                //categories = recette.categories,
                photo = recette.photo
            };

            return Ok(recetteDTO);
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> CreateRecette(IValidator<CreateRecetteDTO> validator, [FromBody] CreateRecetteDTO request)
        {
            validator.ValidateAndThrow(request);

            Recette recette = new()
            {
                nom = request.nom,
                temps_preparation = request.temps_preparation,
                temps_cuisson = request.temps_cuisson,
                difficulte = request.difficulte,
                //etapes = request.etapes,
                //ingredients = request.ingredients,                
                //categories = request.categories,
                photo = request.photo
            };

            var newRecette = await _biblioservice.AddRecetteAsync(recette);

            if (recette == null)
                return BadRequest("Invalid Reciep data.");

            RecetteDTO newRecetteDTO = new()
            {
                nom = newRecette.nom,
                temps_preparation = newRecette.temps_preparation,
                temps_cuisson = newRecette.temps_cuisson,
                difficulte = newRecette.difficulte,
                //etapes = newRecette.etapes,
                //ingredients = newRecette.ingredients,
                //categories = newRecette.categories,
                photo = newRecette.photo
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
                photo = updateRecette.photo
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
