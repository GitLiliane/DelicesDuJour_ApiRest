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
    /// Contrôleur pour l'authentification des utilisateurs et la génération de jetons JWT.
    /// </summary>
    [AllowAnonymous] // Permet l'accès sans authentification préalable
    [Route("api/[controller]")] // Route de base : api/Authentication
    [ApiController] // Indique qu'il s'agit d'un contrôleur API
    public class AuthenticationController : ControllerBase
    {
        /// <summary>
        /// Service pour générer des jetons JWT.
        /// </summary>
        private readonly IJwtTokenService _jwtTokenService;

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur <see cref="AuthenticationController"/>.
        /// </summary>
        /// <param name="jwtTokenService">Service pour la génération de jetons JWT.</param>
        public AuthenticationController(IJwtTokenService jwtTokenService)
        {
            // Injection du service JWT via le constructeur
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Authentifie un utilisateur et retourne un jeton JWT si les informations sont valides.
        /// </summary>
        /// <param name="validator">Validateur pour le modèle <see cref="LoginDTO"/>.</param>
        /// <param name="request">Données de connexion de l'utilisateur.</param>
        /// <returns>Un jeton JWT si l'authentification réussit, sinon un code 401.</returns>
        [HttpPost("Login")] // Route : api/Authentication/Login
        public IActionResult Login(IValidator<LoginDTO> validator, [FromBody] LoginDTO request)
        {
            // Validation du DTO LoginDTO selon les règles définies
            validator.ValidateAndThrow(request);

            // Vérification des identifiants : exemple simplifié à remplacer par une vraie logique
            if (request.Username == "admin" && request.Password == "admin")
            {
                // Génération d'un jeton JWT pour un administrateur
                var token = _jwtTokenService.GenerateToken(request.Username, "Administrateur", "Utilisateur");
                // Retourne le jeton avec un code HTTP 200
                return Ok(new JwtDTO { Token = token });
            }
            else if (request.Username == "user" && request.Password == "user")
            {
                // Génération d'un jeton JWT pour un utilisateur standard
                var token = _jwtTokenService.GenerateToken(request.Username, "Utilisateur");
                return Ok(new JwtDTO { Token = token });
            }

            // Si les identifiants sont incorrects, lève une exception 401
            throw new UnauthorizedAccessException("Nom d'utilisateur ou mot de passe incorrect.");
        }
    }
}
