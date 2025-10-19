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
    /// <summary>
    /// Contrôleur gérant les opérations CRUD sur les ingrédients.
    /// </summary>
    [Route("api/[controller]")] // Route de base : api/Ingredients
    [ApiController] // Indique qu'il s'agit d'un contrôleur API
    public class IngredientsController : ControllerBase
    {
        /// <summary>
        /// Service métier pour la gestion des ingrédients.
        /// </summary>
        private readonly IBiblioService _biblioService;

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur <see cref="IngredientsController"/>.
        /// </summary>
        /// <param name="biblioService">Service métier injecté pour la gestion des ingrédients.</param>
        public IngredientsController(IBiblioService biblioService)
        {
            // Injection du service métier via le constructeur
            _biblioService = biblioService;
        }

        /// <summary>
        /// Récupère la liste complète des ingrédients.
        /// </summary>
        /// <returns>Une liste d’ingrédients au format DTO.</returns>
        [Authorize(Roles = "Administrateur, Utilisateur")] // Accessible aux administrateurs et utilisateurs
        [HttpGet()] // Route GET : api/Ingredients
        [ProducesResponseType(StatusCodes.Status200OK)] // Retourne 200 si succès
        public async Task<IActionResult> GetIngredients()
        {
            // Récupération de tous les ingrédients via le service
            var ingredients = await _biblioService.GetAllIngredientsAsync();

            // Transformation des objets métiers en DTO pour la réponse
            IEnumerable<IngredientDTO> response = ingredients.Select(i => new IngredientDTO()
            {
                id = i.id,
                nom = i.nom
            });

            // Retourne la liste des ingrédients avec code 200
            return Ok(response);
        }

        /// <summary>
        /// Récupère un ingrédient par son identifiant unique.
        /// </summary>
        /// <param name="id">Identifiant de l’ingrédient recherché.</param>
        /// <returns>L’ingrédient correspondant au format DTO ou 404 s’il n’existe pas.</returns>
        [Authorize(Roles = "Administrateur, Utilisateur")] // Accessible aux utilisateurs et administrateurs
        [HttpGet("{id}")] // Route GET : api/Ingredients/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetIngredientById([FromRoute] int id)
        {
            // Recherche de l’ingrédient par son ID
            var ingredient = await _biblioService.GetIngredientByIdAsync(id);

            // Vérifie si l’ingrédient existe
            if (ingredient is null)
                return NotFound(); // Retour 404 si non trouvé

            // Conversion de l’objet métier en DTO
            IngredientDTO response = new()
            {
                id = ingredient.id,
                nom = ingredient.nom
            };

            // Retourne le DTO avec un code 200
            return Ok(response);
        }

        /// <summary>
        /// Crée un nouvel ingrédient.
        /// </summary>
        /// <param name="validator">Validateur du modèle <see cref="CreateIngredientDTO"/>.</param>
        /// <param name="request">Données du nouvel ingrédient à créer.</param>
        /// <returns>L’ingrédient créé au format DTO avec code 201 ou une erreur 400 si invalide.</returns>
        [Authorize(Roles = "Administrateur")] // Seul un administrateur peut créer
        [HttpPost()] // Route POST : api/Ingredients
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateIngredient(IValidator<CreateIngredientDTO> validator, [FromBody] CreateIngredientDTO request)
        {
            // Validation du DTO selon les règles définies
            validator.ValidateAndThrow(request);

            // Création d’un objet métier à partir du DTO
            Ingredient ingredient = new()
            {
                nom = request.nom
            };

            // Appel au service pour ajouter l’ingrédient
            var newIngredient = await _biblioService.AddIngredientAsync(ingredient);

            // Vérifie si l’ajout a échoué
            if (newIngredient is null)
                return BadRequest("Invalid ingredient data.");

            // Conversion de l’objet métier créé en DTO pour la réponse
            IngredientDTO response = new()
            {
                id = newIngredient.id,
                nom = newIngredient.nom
            };

            // Retourne l’objet créé avec code HTTP 201 (Created)
            return CreatedAtAction(nameof(GetIngredientById), new { id = response.id }, response);
        }

        /// <summary>
        /// Met à jour un ingrédient existant.
        /// </summary>
        /// <param name="validator">Validateur du modèle <see cref="UpdateIngredientDTO"/>.</param>
        /// <param name="id">Identifiant de l’ingrédient à modifier.</param>
        /// <param name="request">Nouvelles données de l’ingrédient.</param>
        /// <returns>L’ingrédient modifié au format DTO ou 400 si invalide.</returns>
        [Authorize(Roles = "Administrateur")] // Seul un administrateur peut modifier
        [HttpPut("{id}")] // Route PUT : api/Ingredients/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateIngredient(IValidator<UpdateIngredientDTO> validator, [FromRoute] int id, [FromBody] UpdateIngredientDTO request)
        {
            // Validation du DTO
            validator.ValidateAndThrow(request);

            // Création d’un objet métier à partir des nouvelles données
            Ingredient ingredient = new()
            {
                id = id,
                nom = request.nom
            };

            // Appel au service pour mettre à jour l’ingrédient
            var modifiedIngredient = await _biblioService.ModifyIngredientAsync(ingredient);

            // Vérifie si la mise à jour a échoué
            if (modifiedIngredient is null)
                return BadRequest("Invalid ingredient.");

            // Conversion en DTO pour la réponse
            IngredientDTO response = new()
            {
                id = modifiedIngredient.id,
                nom = modifiedIngredient.nom
            };

            // Retourne le DTO mis à jour avec code HTTP 200
            return Ok(response);
        }

        /// <summary>
        /// Supprime un ingrédient existant.
        /// </summary>
        /// <param name="id">Identifiant de l’ingrédient à supprimer.</param>
        /// <returns>Code 204 si supprimé, ou 404 si non trouvé.</returns>
        [Authorize(Roles = "Administrateur")] // Seul un administrateur peut supprimer
        [HttpDelete("{id}")] // Route DELETE : api/Ingredients/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteIngredient([FromRoute] int id)
        {
            // Appel du service pour supprimer l’ingrédient
            var success = await _biblioService.DeleteIngredientAsync(id);

            // Retourne 204 si succès, sinon 404
            return success ? NoContent() : NotFound();
        }
    }
}
