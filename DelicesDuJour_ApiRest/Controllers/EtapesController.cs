using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using static Mysqlx.Expect.Open.Types.Condition.Types;


namespace DelicesDuJour_ApiRest.Controllers
{
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
                // Assigner la propriété Key en créant une nouvelle instance de TupleDTO
                // 'e.Key.t' représente la première partie de votre clé (par ex. id_recette)
                // 'e.Key.v' représente la deuxième partie (par ex. numero)
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
                // Assigner la propriété Key en créant une nouvelle instance de TupleDTO
                // 'e.Key.t' représente la première partie de votre clé (par ex. id_recette)
                // 'e.Key.v' représente la deuxième partie (par ex. numero)
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

        public async Task<IActionResult> CreateEtape(IValidator<CreateEtapeDTO> validator, [FromBody] CreateEtapeDTO request)
        {
            validator.ValidateAndThrow(request);

            Etape etape = new()
            {
                //Key = new TupleClass<int, int>
                //{
                //    t = request.Key.t,
                //    v = request.Key.v
                //},
                titre = request.titre,
                texte = request.texte
            };

            var newEtape = await _biblioservice.AddEtapeAsync(etape);

            if (etape == null)
                return BadRequest("Invalid Etape data.");

            EtapeDTO newEtapeDTO = new()
            {
                //Key = new TupleDTO<int, int>
                //{
                //    t = etape.Key.t,
                //    v = etape.Key.v
                //},
                titre = etape.titre,
                texte = etape.texte
            };

            return null;  /*CreatedAtAction(nameof(GetEtapeById), new { id_recette = newEtapeDTO.Key.t, numero = newEtapeDTO.Key.v }, newEtapeDTO);*/

        }

        [HttpPut("{id_recette}/{numero}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEtape(IValidator<UpdateEtapeDTO> validator, [FromRoute] int id_recette, [FromRoute] int numero, [FromBody] UpdateEtapeDTO updateEtapeDTO)
        {
            validator.ValidateAndThrow(updateEtapeDTO);

            Etape updateE = new()
            {
                //Key = new TupleClass<int, int>
                //{
                //    t = id_recette,
                //    v = numero
                //},
                titre = updateEtapeDTO.titre,
                texte = updateEtapeDTO.texte
            };

            var updateEtape = await _biblioservice.ModifyEtapeAsync(updateE);

            if (updateEtape is null)
                return BadRequest("Invalid etape.");

            EtapeDTO etapeDTO = new()
            {
                //Key = new TupleDTO<int, int>
                //{
                //    t = updateEtape.Key.t,
                //    v = updateEtape.Key.v
                //},
                titre = updateEtapeDTO.titre,
                texte = updateEtapeDTO.texte
            };

            return Ok(etapeDTO);     

        }

        [HttpDelete("{id_recette}/{numero}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteEtape(int id_recette, int numero)
        {
            //var key = new TupleClass<int, int>((id_recette, numero));
            var sucess = await _biblioservice.DeleteEtapeAsync(id_recette);

            return sucess ? NoContent() : NotFound();
        }
    }
}
