using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using static Mysqlx.Expect.Open.Types.Condition.Types;


namespace DelicesDuJour_ApiRest.Controllers
{
    [Authorize(Roles = "Administrateur, Utilisateur")]
    [Route("api/[controller]")]
    [ApiController]
    public class EtapesController : ControllerBase
    {
        private readonly IBiblioService _biblioservice;

        public EtapesController(IBiblioService biblioService)
        {
            _biblioservice = biblioService;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEtapes()
        {
            var etapes = await _biblioservice.GetAllEtapesAsync();

            IEnumerable<EtapeDTO> etapeDTOs = etapes.Select(e => new EtapeDTO()
            {
                id_recette = e.id_recette,
                numero = e.numero,
                titre = e.titre,
                texte = e.texte
            });

            return Ok(etapeDTOs);
        }

        [HttpGet("{id_recette}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEtapeByIdRecette([FromRoute] int id_recette)
        {
            var etapes = await _biblioservice.GetEtapesByIdRecetteAsync(id_recette);

            if (etapes is null)
                return NotFound();

            IEnumerable<EtapeDTO> etapeDTOs = etapes.Select(e => new EtapeDTO()
            {
                id_recette = e.id_recette,
                numero = e.numero,
                titre = e.titre,
                texte = e.texte
            });

            return Ok(etapeDTOs);
        }


        [HttpPost("{id_recette}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> CreateEtape(IValidator<CreateEtapeDTO> validator, [FromBody] CreateEtapeDTO request,
          [FromRoute(Name ="id_recette")] int id_recette)
        {
            validator.ValidateAndThrow(request);

            Etape etape = new()
            {
                id_recette = id_recette,
                numero = request.numero,
                titre = request.titre,
                texte = request.texte
            };

            var newEtape = await _biblioservice.AddEtapeAsync(etape);

            if (etape == null)
                return BadRequest("Invalid Etape data.");

            EtapeDTO newEtapeDTO = new()
            {
                id_recette = id_recette, 
                numero = request.numero,
                titre = request.titre,
                texte = request.texte
            };

            return CreatedAtAction(nameof(GetEtapeByIdRecette), new { id_recette = id_recette }, newEtapeDTO);
        }

        [HttpPut("{id_recette}/{numero}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEtape(IValidator<UpdateEtapeDTO> validator, [FromRoute] int id_recette, [FromRoute] int numero, [FromBody] UpdateEtapeDTO updateEtapeDTO)
        {
            validator.ValidateAndThrow(updateEtapeDTO);

            Etape updateE = new()
            {
                id_recette = id_recette,
                numero = numero,
                titre = updateEtapeDTO.titre,
                texte = updateEtapeDTO.texte
            };

            var updateEtape = await _biblioservice.ModifyEtapeAsync(updateE);

            if (updateEtape is null)
                return BadRequest("Invalid etape.");

            EtapeDTO etapeDTO = new()
            {
                id_recette = updateEtape.id_recette,
                numero = updateEtape.numero,
                titre = updateEtape.titre,
                texte = updateEtape.texte
            };

            return Ok(etapeDTO);
        }

        [HttpDelete("{id_recette}/{numero}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteEtape([FromRoute] int id_recette, [FromRoute] int numero)
        {
            (int, int) key = new();
            key = (id_recette, numero);
            var sucess = await _biblioservice.DeleteEtapeAsync(key);

            return sucess ? NoContent() : NotFound();
        }
    }
}
