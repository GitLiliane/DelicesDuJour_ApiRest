using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using DelicesDuJour_ApiRest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DelicesDuJour_ApiRest.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class RecettesCategoriesRelationsController : ControllerBase
    {
        private readonly IBiblioService _biblioService;

        public RecettesCategoriesRelationsController(IBiblioService biblioService)
        {
            _biblioService = biblioService;
        }

        [Authorize(Roles = "Administrateur")]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRecettesCategories()
        {
            var relations = await _biblioService.GetAllRecettesCategoriesAsync();

            IEnumerable<RecetteCategorieRelationshipDTO> response = relations.Select(r => new RecetteCategorieRelationshipDTO()
            {
                idRecette = r.id_recette,
                idCategorie = r.id_categorie
            });

            return Ok(response);
        }

        [Authorize(Roles = "Administrateur")]
        [HttpPost(nameof(AddRecetteCategorieRelationship) + "/{idCategorie}/{idRecette}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddRecetteCategorieRelationship([FromRoute] int idCategorie, [FromRoute] int idRecette)
        {
            var success = await _biblioService.AddRecetteCategorieRelationshipAsync(idCategorie, idRecette);
            return success ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Administrateur")]
        [HttpDelete(nameof(RemoveRecetteCategorieRelationship) + "/{idCategorie}/{idRecette}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRecetteCategorieRelationship([FromRoute] int idCategorie, [FromRoute] int idRecette)
        {
            var success = await _biblioService.RemoveRecetteCategorieRelationshipAsync(idCategorie, idRecette);
            return success ? NoContent() : NotFound();
        }

        [AllowAnonymous]
        [HttpGet(nameof(GetRecettesByIdCategorie) + "/{idCategorie}")]
        public async Task<IActionResult> GetRecettesByIdCategorie([FromRoute] int idCategorie)
        {
            var response = await _biblioService.GetRecettesByIdCategorieAsync(idCategorie);
            return Ok(response);
        }

        [Authorize(Roles = "Administrateur")]
        [HttpGet(nameof(GetCategoriesByIdRecette) + "/{idRecette}")]
        public async Task<IActionResult> GetCategoriesByIdRecette([FromRoute] int idRecette)
        {
            var response = await _biblioService.GetCategoriesByIdRecetteAsync(idRecette);
            return Ok(response);
        }

        [Authorize(Roles = "Administrateur")]
        [HttpDelete(nameof(DeleteRecetteRelations) + "/{idRecette}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRecetteRelations([FromRoute] int idRecette)
        {
            var success = await _biblioService.DeleteRecetteRelationsAsync(idRecette);
            return success ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Administrateur")]
        [HttpDelete(nameof(DeleteCategorieRelations) + "/{idCategorie}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategorieRelations([FromRoute] int idCategorie)
        {
            var success = await _biblioService.DeleteCategorieRelationsAsync(idCategorie);
            return success ? NoContent() : NotFound();
        }
    }


}
