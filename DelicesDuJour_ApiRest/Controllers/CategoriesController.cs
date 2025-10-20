using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.DTOIn;
using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DelicesDuJour_ApiRest.Controllers
{
    /// <summary>
    /// Contrôleur pour gérer les catégories dans l'application.
    /// Permet les opérations CRUD sur les catégories.
    /// </summary>
    [Route("api/[controller]")] // Définit la route de base pour ce contrôleur : api/Categories
    [ApiController] // Indique qu'il s'agit d'un contrôleur API
    public class CategoriesController : ControllerBase
    {
        /// <summary>
        /// Service métier pour gérer les catégories.
        /// </summary>
        private readonly IBiblioService _biblioService;

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur <see cref="CategoriesController"/>.
        /// </summary>
        /// <param name="biblioService">Service métier injecté pour gérer les catégories.</param>
        public CategoriesController(IBiblioService biblioService)
        {
            // Injection du service métier via le constructeur
            _biblioService = biblioService;
        }

        /// <summary>
        /// Récupère toutes les catégories.
        /// Accessible uniquement par les administrateurs.
        /// </summary>
        /// <returns>Liste de catégories au format DTO.</returns>
        [Authorize(Roles = "Administrateur")] // Seul l'administrateur peut accéder à cette action
        [HttpGet()] // Route GET : api/Categories
        [ProducesResponseType(StatusCodes.Status200OK)] // Indique que l'action retourne un code 200 en cas de succès
        public async Task<IActionResult> GetCategories()
        {
            // Récupération de toutes les catégories via le service métier
            var categories = await _biblioService.GetAllCategoriesAsync();

            // Conversion des objets métiers en DTO
            IEnumerable<CategorieDTO> response = categories.Select(c => new CategorieDTO()
            {
                id = c.id, // ID de la catégorie
                nom = c.nom // Nom de la catégorie
            });

            // Retour de la liste des catégories avec code HTTP 200
            return Ok(response);
        }

        /// <summary>
        /// Récupère une catégorie par son identifiant.
        /// Accessible par les administrateurs et utilisateurs.
        /// </summary>
        /// <param name="id">Identifiant de la catégorie à récupérer.</param>
        /// <returns>La catégorie correspondante au format DTO ou 404 si non trouvée.</returns>
        [Authorize(Roles = "Administrateur, Utilisateur")] // Rôle Administrateur ou Utilisateur
        [HttpGet("{id}")] // Route GET : api/Categories/{id}
        [ProducesResponseType(StatusCodes.Status200OK)] // Code 200 si trouvé
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Code 404 si non trouvé
        public async Task<IActionResult> GetCategorieById([FromRoute] int id)
        {
            // Récupération de la catégorie par ID
            var categorie = await _biblioService.GetCategorieByIdAsync(id);

            // Si la catégorie n'existe pas, retourne 404
            if (categorie is null)
                return NotFound();

            // Conversion de l'objet métier en DTO
            CategorieDTO response = new()
            {
                id = categorie.id,
                nom = categorie.nom
            };

            // Retour de la catégorie avec code HTTP 200
            return Ok(response);
        }

        /// <summary>
        /// Crée une nouvelle catégorie.
        /// Accessible uniquement par les administrateurs.
        /// </summary>
        /// <param name="validator">Validateur pour le modèle <see cref="CreateCategorieDTO"/>.</param>
        /// <param name="request">Objet contenant les informations de la catégorie à créer.</param>
        /// <returns>La catégorie créée au format DTO avec code 201.</returns>
        [Authorize(Roles = "Administrateur")]
        [HttpPost()] // Route POST : api/Categories
        [ProducesResponseType(StatusCodes.Status201Created)] // Code 201 si créé
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Code 400 si invalide
        public async Task<IActionResult> CreateCategorie(IValidator<CreateCategorieDTO> validator, [FromBody] CreateCategorieDTO request)
        {
            // Validation du DTO avec FluentValidation
            validator.ValidateAndThrow(request);

            // Création de l'objet métier à partir du DTO
            Categorie categorie = new()
            {
                nom = request.nom
            };

            // Ajout de la catégorie via le service métier
            var newCategorie = await _biblioService.AddCategorieAsync(categorie);

            // Si l'ajout échoue, retourne 400
            if (newCategorie is null)
                return BadRequest("Invalid categorie data.");

            // Conversion de l'objet métier en DTO pour la réponse
            CategorieDTO response = new()
            {
                id = newCategorie.id,
                nom = newCategorie.nom
            };

            // Retourne la catégorie créée avec code HTTP 201 et location de la ressource
            return CreatedAtAction(nameof(GetCategorieById), new { id = response.id }, response);
        }

        /// <summary>
        /// Met à jour une catégorie existante.
        /// Accessible uniquement par les administrateurs.
        /// </summary>
        /// <param name="validator">Validateur pour le modèle <see cref="UpdateCategorieDTO"/>.</param>
        /// <param name="id">Identifiant de la catégorie à mettre à jour.</param>
        /// <param name="request">DTO contenant les nouvelles informations de la catégorie.</param>
        /// <returns>La catégorie mise à jour au format DTO ou 400 si échec.</returns>
        [Authorize(Roles = "Administrateur")]
        [HttpPut("{id}")] // Route PUT : api/Categories/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCategorie(IValidator<UpdateCategorieDTO> validator, [FromRoute] int id, [FromBody] UpdateCategorieDTO request)
        {
            // Validation du DTO
            validator.ValidateAndThrow(request);

            // Création de l'objet métier avec l'ID fourni
            Categorie categorie = new()
            {
                id = id,
                nom = request.nom
            };

            // Mise à jour de la catégorie via le service métier
            var modifiedCategorie = await _biblioService.ModifyCategorieAsync(categorie);

            // Si l'opération échoue, retourne 400
            if (modifiedCategorie is null)
                return BadRequest("Invalid categorie.");

            // Conversion de l'objet métier en DTO pour la réponse
            CategorieDTO response = new()
            {
                id = modifiedCategorie.id,
                nom = modifiedCategorie.nom
            };

            // Retourne la catégorie mise à jour avec code 200
            return Ok(response);
        }

        /// <summary>
        /// Supprime une catégorie existante.
        /// Accessible uniquement par les administrateurs.
        /// </summary>
        /// <param name="id">Identifiant de la catégorie à supprimer.</param>
        /// <returns>Code 204 si supprimée ou 404 si non trouvée.</returns>
        [Authorize(Roles = "Administrateur")]
        [HttpDelete("{id}")] // Route DELETE : api/Categories/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategorie([FromRoute] int id)
        {
            // Tentative de suppression de la catégorie
            var success = await _biblioService.DeleteCategorieAsync(id);

            // Retourne 204 si succès, sinon 404
            return success ? NoContent() : NotFound();
        }
    }
}
