using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.DTOIn;
using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace DelicesDuJour_ApiRest.Controllers
{
    /// <summary>
    /// Contrôleur pour gérer les étapes des recettes.
    /// Permet les opérations CRUD sur les étapes.
    /// </summary>
    [Authorize(Roles = "Administrateur, Utilisateur")] // Accessible uniquement aux utilisateurs et administrateurs
    [Route("api/[controller]")] // Route de base : api/Etapes
    [ApiController] // Indique qu'il s'agit d'un contrôleur API
    public class EtapesController : ControllerBase
    {
        /// <summary>
        /// Service métier pour gérer les étapes.
        /// </summary>
        private readonly IBiblioService _biblioservice;

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur <see cref="EtapesController"/>.
        /// </summary>
        /// <param name="biblioService">Service métier injecté pour gérer les étapes.</param>
        public EtapesController(IBiblioService biblioService)
        {
            _biblioservice = biblioService; // Injection du service métier
        }

        /// <summary>
        /// Récupère toutes les étapes.
        /// </summary>
        /// <returns>Liste de toutes les étapes au format DTO.</returns>
        [HttpGet()] // Route GET : api/Etapes
        [ProducesResponseType(StatusCodes.Status200OK)] // Code 200 si succès
        public async Task<IActionResult> GetEtapes()
        {
            // Récupération de toutes les étapes via le service métier
            var etapes = await _biblioservice.GetAllEtapesAsync();

            // Conversion en DTO pour la réponse
            IEnumerable<EtapeDTO> etapeDTOs = etapes.Select(e => new EtapeDTO()
            {
                id_recette = e.id_recette,
                numero = e.numero,
                titre = e.titre,
                texte = e.texte
            });

            return Ok(etapeDTOs); // Retourne la liste avec code HTTP 200
        }

        /// <summary>
        /// Récupère toutes les étapes d'une recette spécifique.
        /// </summary>
        /// <param name="id_recette">ID de la recette dont on souhaite les étapes.</param>
        /// <returns>Liste des étapes au format DTO ou 404 si aucune trouvée.</returns>
        [HttpGet("{id_recette}")] // Route GET : api/Etapes/{id_recette}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEtapeByIdRecette([FromRoute] int id_recette)
        {
            // Récupération des étapes d'une recette via le service métier
            var etapes = await _biblioservice.GetEtapesByIdRecetteAsync(id_recette);

            if (etapes is null)
                return NotFound(); // Retourne 404 si aucune étape trouvée

            // Conversion en DTO pour la réponse
            IEnumerable<EtapeDTO> etapeDTOs = etapes.Select(e => new EtapeDTO()
            {
                id_recette = e.id_recette,
                numero = e.numero,
                titre = e.titre,
                texte = e.texte
            });

            return Ok(etapeDTOs); // Retourne la liste des étapes avec code 200
        }

        /// <summary>
        /// Crée une nouvelle étape pour une recette.
        /// </summary>
        /// <param name="validator">Validateur pour le modèle <see cref="CreateEtapeDTO"/>.</param>
        /// <param name="request">DTO contenant les informations de la nouvelle étape.</param>
        /// <param name="id_recette">ID de la recette à laquelle l'étape appartient.</param>
        /// <returns>La nouvelle étape au format DTO avec code 201 ou 400 si invalide.</returns>
        [HttpPost("{id_recette}")] // Route POST : api/Etapes/{id_recette}
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEtape(IValidator<CreateEtapeDTO> validator, [FromBody] CreateEtapeDTO request,
            [FromRoute(Name = "id_recette")] int id_recette)
        {
            validator.ValidateAndThrow(request); // Validation du DTO

            // Création de l'objet métier étape
            Etape etape = new()
            {
                id_recette = id_recette,
                numero = request.numero,
                titre = request.titre,
                texte = request.texte
            };

            // Ajout via le service métier
            var newEtape = await _biblioservice.AddEtapeAsync(etape);

            // Vérification de l'ajout
            if (etape == null)
                return BadRequest("Invalid Etape data."); // Retour 400 si échec

            // Conversion en DTO pour la réponse
            EtapeDTO newEtapeDTO = new()
            {
                id_recette = id_recette,
                numero = request.numero,
                titre = request.titre,
                texte = request.texte
            };

            // Retourne la nouvelle étape avec code 201
            return CreatedAtAction(nameof(GetEtapeByIdRecette), new { id_recette = id_recette }, newEtapeDTO);
        }

        /// <summary>
        /// Met à jour une étape existante.
        /// </summary>
        /// <param name="validator">Validateur pour le modèle <see cref="UpdateEtapeDTO"/>.</param>
        /// <param name="id_recette">ID de la recette.</param>
        /// <param name="numero">Numéro de l'étape à mettre à jour.</param>
        /// <param name="updateEtapeDTO">DTO contenant les nouvelles informations.</param>
        /// <returns>L'étape mise à jour au format DTO ou 400 si invalide.</returns>
        [HttpPut("{id_recette}/{numero}")] // Route PUT : api/Etapes/{id_recette}/{numero}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEtape(IValidator<UpdateEtapeDTO> validator, [FromRoute] int id_recette, [FromRoute] int numero, [FromBody] UpdateEtapeDTO updateEtapeDTO)
        {
            validator.ValidateAndThrow(updateEtapeDTO); // Validation du DTO

            // Création de l'objet métier étape avec les nouvelles valeurs
            Etape updateE = new()
            {
                id_recette = id_recette,
                numero = numero,
                titre = updateEtapeDTO.titre,
                texte = updateEtapeDTO.texte
            };

            // Mise à jour via le service métier
            var updateEtape = await _biblioservice.ModifyEtapeAsync(updateE);

            if (updateEtape is null)
                return BadRequest("Invalid etape."); // Retour 400 si échec

            // Conversion en DTO pour la réponse
            EtapeDTO etapeDTO = new()
            {
                id_recette = updateEtape.id_recette,
                numero = updateEtape.numero,
                titre = updateEtape.titre,
                texte = updateEtape.texte
            };

            return Ok(etapeDTO); // Retourne l'étape mise à jour avec code 200
        }

        /// <summary>
        /// Supprime une étape existante.
        /// </summary>
        /// <param name="id_recette">ID de la recette.</param>
        /// <param name="numero">Numéro de l'étape à supprimer.</param>
        /// <returns>Code 204 si supprimée ou 404 si non trouvée.</returns>
        [HttpDelete("{id_recette}/{numero}")] // Route DELETE : api/Etapes/{id_recette}/{numero}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEtape([FromRoute] int id_recette, [FromRoute] int numero)
        {
            // Création de la clé composite (id_recette, numero)
            (int, int) key = new();
            key = (id_recette, numero);

            // Tentative de suppression via le service métier
            var sucess = await _biblioservice.DeleteEtapeAsync(key);

            // Retourne 204 si succès, 404 si non trouvé
            return sucess ? NoContent() : NotFound();
        }
    }
}
