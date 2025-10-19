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
    /// <summary>
    /// Contrôleur gérant les relations entre les recettes et leurs ingrédients,
    /// ainsi que les quantités associées.
    /// </summary>
    [Route("api/[controller]")] // Route de base : api/QuantiteIngredients
    [ApiController] // Indique que ce contrôleur répond à des requêtes d’API
    public class QuantiteIngredientsController : ControllerBase
    {
        /// <summary>
        /// Service métier pour la gestion des quantités ingrédient-recette.
        /// </summary>
        private readonly IBiblioService _biblioService;

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur <see cref="QuantiteIngredientsController"/>.
        /// </summary>
        /// <param name="biblioService">Service métier injecté pour la gestion des quantités ingrédient-recette.</param>
        public QuantiteIngredientsController(IBiblioService biblioService)
        {
            // Injection du service métier
            _biblioService = biblioService;
        }

        /// <summary>
        /// Récupère toutes les relations ingrédient-recette avec leurs quantités.
        /// </summary>
        /// <returns>Une liste d’objets <see cref="QuantiteIngredientsDTO"/>.</returns>
        [Authorize(Roles = "Administrateur, Utilisateur")] // Accessible aux deux rôles
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuantiteIngredients()
        {
            // Récupération de toutes les relations ingrédient-recette
            var quantites = await _biblioService.GetQuantiteIngredientsAsync();

            // Conversion en DTO pour la réponse API
            IEnumerable<QuantiteIngredientsDTO> response = quantites.Select(q => new QuantiteIngredientsDTO()
            {
                id_ingredient = q.id_ingredient,
                id_recette = q.id_recette,
                quantite = q.quantite
            });

            return Ok(response);
        }

        /// <summary>
        /// Récupère une relation ingrédient-recette par identifiants combinés.
        /// </summary>
        /// <param name="id_ingredient">Identifiant de l’ingrédient.</param>
        /// <param name="id_recette">Identifiant de la recette.</param>
        /// <returns>La relation correspondante au format DTO ou 404 si non trouvée.</returns>
        [Authorize(Roles = "Administrateur, Utilisateur")]
        [HttpGet(nameof(GetQuantiteById) + "/{id_ingredient}/{id_recette}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQuantiteById([FromRoute] int id_ingredient, [FromRoute] int id_recette)
        {
            // Création de la clé composite
            (int, int) key = (id_ingredient, id_recette);

            // Récupération de la relation correspondante
            var quantite = await _biblioService.GetQuantiteIngredientsByIdAsync(key);

            if (quantite is null)
                return NotFound();

            // Conversion de l’objet métier en DTO
            QuantiteIngredientsDTO response = new()
            {
                id_ingredient = quantite.id_ingredient,
                id_recette = quantite.id_recette,
                quantite = quantite.quantite
            };

            return Ok(response);
        }

        /// <summary>
        /// Crée une nouvelle relation ingrédient-recette avec une quantité associée.
        /// </summary>
        /// <param name="idIngredient">Identifiant de l’ingrédient.</param>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <param name="validator">Validateur du modèle <see cref="CreateQuantiteIngredientsDTO"/>.</param>
        /// <param name="createQuantiteIngredientsDTO">Données de la relation à créer.</param>
        /// <returns>La relation créée au format DTO ou 400 si invalide.</returns>
        [Authorize(Roles = "Administrateur")]
        [HttpPost(nameof(AddRecetteIngredientRelationship) + "/{idIngredient}/{idRecette}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddRecetteIngredientRelationship([FromRoute] int idIngredient, [FromRoute] int idRecette, IValidator<CreateQuantiteIngredientsDTO> validator, [FromBody] CreateQuantiteIngredientsDTO createQuantiteIngredientsDTO)
        {
            // Validation du modèle d’entrée
            validator.ValidateAndThrow(createQuantiteIngredientsDTO);

            // Création de l’objet métier
            QuantiteIngredients newQuantiteIngredients = new()
            {
                id_ingredient = createQuantiteIngredientsDTO.id_ingredient,
                id_recette = createQuantiteIngredientsDTO.id_recette,
                quantite = createQuantiteIngredientsDTO.quantite
            };

            // Appel au service métier
            var request = await _biblioService.AddRecetteIngredientRelationshipAsync(newQuantiteIngredients);

            if (request is null)
                return BadRequest("Invalid quantité ingrédients data.");

            // Conversion de la réponse en DTO
            QuantiteIngredientsDTO response = new()
            {
                id_ingredient = request.id_ingredient,
                id_recette = request.id_recette,
                quantite = request.quantite
            };

            return Ok(response);
        }

        /// <summary>
        /// Met à jour la quantité associée à une relation ingrédient-recette.
        /// </summary>
        /// <param name="validator">Validateur du modèle <see cref="UpdateQuantiteIngredientsDTO"/>.</param>
        /// <param name="idIngredient">Identifiant de l’ingrédient concerné.</param>
        /// <param name="idRecette">Identifiant de la recette concernée.</param>
        /// <param name="request">Données de mise à jour.</param>
        /// <returns>La relation mise à jour au format DTO ou 400 si invalide.</returns>
        [Authorize(Roles = "Administrateur")]
        [HttpPut(nameof(UpdateRecetteIngredientRelationship) + "/{idIngredient}/{idRecette}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRecetteIngredientRelationship(IValidator<UpdateQuantiteIngredientsDTO> validator, [FromRoute] int idIngredient, [FromRoute] int idRecette, [FromBody] UpdateQuantiteIngredientsDTO request)
        {
            // Validation du modèle
            validator.ValidateAndThrow(request);

            // Création de l’objet métier à mettre à jour
            QuantiteIngredients quantiteIngredients = new()
            {
                id_ingredient = request.id_ingredient,
                id_recette = request.id_recette,
                quantite = request.quantite
            };

            // Appel du service de mise à jour
            var modifiedQuantite = await _biblioService.updateRecetteIngredientRelationshipAsync(quantiteIngredients);

            if (modifiedQuantite is null)
                return BadRequest("Invalid quantité ingrédients.");

            // Conversion en DTO pour la réponse
            QuantiteIngredientsDTO response = new()
            {
                id_ingredient = modifiedQuantite.id_ingredient,
                id_recette = modifiedQuantite.id_recette,
                quantite = modifiedQuantite.quantite
            };

            return Ok(response);
        }

        /// <summary>
        /// Supprime une relation spécifique ingrédient-recette.
        /// </summary>
        /// <param name="idIngredient">Identifiant de l’ingrédient.</param>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>Code 204 si supprimé, ou 404 si non trouvé.</returns>
        [Authorize(Roles = "Administrateur")]
        [HttpDelete(nameof(RemoveRecetteIngredientRelationship) + "/{idIngredient}/{idRecette}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRecetteIngredientRelationship([FromRoute] int idIngredient, [FromRoute] int idRecette)
        {
            // Clé composite pour identifier la relation
            (int, int) key = (idIngredient, idRecette);

            // Appel au service pour suppression
            var success = await _biblioService.RemoveRecetteIngredientRelationshipAsync(key);
            return success ? NoContent() : NotFound();
        }

        /// <summary>
        /// Récupère toutes les recettes associées à un ingrédient.
        /// </summary>
        /// <param name="idIngredient">Identifiant de l’ingrédient.</param>
        /// <returns>Une liste de recettes associées.</returns>
        [Authorize(Roles = "Administrateur, Utilisateur")]
        [HttpGet(nameof(GetRecettesByIdIngredient) + "/{idIngredient}")]
        public async Task<IActionResult> GetRecettesByIdIngredient([FromRoute] int idIngredient)
        {
            // Appel du service pour récupérer les recettes
            var response = await _biblioService.GetRecettesByIdIngredientAsync(idIngredient);
            return Ok(response);
        }

        /// <summary>
        /// Récupère tous les ingrédients associés à une recette.
        /// </summary>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>Une liste d’ingrédients associés.</returns>
        [Authorize(Roles = "Administrateur, Utilisateur")]
        [HttpGet(nameof(GetIngredientsByIdRecette) + "/{idRecette}")]
        public async Task<IActionResult> GetIngredientsByIdRecette([FromRoute] int idRecette)
        {
            // Appel du service pour récupérer les ingrédients
            var response = await _biblioService.GetIngredientsByIdRecetteAsync(idRecette);
            return Ok(response);
        }

        /// <summary>
        /// Supprime toutes les relations ingrédient-recette pour une recette donnée.
        /// </summary>
        /// <param name="idRecette">Identifiant de la recette.</param>
        /// <returns>Code 204 si succès, sinon 404.</returns>
        [Authorize(Roles = "Administrateur")]
        [HttpDelete(nameof(DeleteRecetteRelationsIngredient) + "/{idRecette}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRecetteRelationsIngredient([FromRoute] int idRecette)
        {
            var success = await _biblioService.DeleteRecetteRelationsIngredientAsync(idRecette);
            return success ? NoContent() : NotFound();
        }

        /// <summary>
        /// Supprime toutes les relations ingrédient-recette pour un ingrédient donné.
        /// </summary>
        /// <param name="idIngredient">Identifiant de l’ingrédient.</param>
        /// <returns>Code 204 si succès, sinon 404.</returns>
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
