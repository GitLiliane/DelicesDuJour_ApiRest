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
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur <see cref="AuthenticationController"/>.
        /// </summary>
        /// <param name="jwtTokenService">Service pour la génération de jetons JWT.</param>
        public AuthenticationController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Authentifie un utilisateur et retourne un jeton JWT si les informations sont valides.
        /// </summary>
        /// <param name="validator">Validateur pour le modèle <see cref="LoginDTO"/>.</param>
        /// <param name="request">Données de connexion de l'utilisateur.</param>
        /// <returns>Un jeton JWT si l'authentification réussit, sinon un code 401.</returns>
        [HttpPost("Login")]
        public IActionResult Login(IValidator<LoginDTO> validator, [FromBody] LoginDTO request)
        {
            validator.ValidateAndThrow(request);

            // Exemples à modifier avec une vraie validation 
            if (request.Username == "admin" && request.Password == "admin")
            {
                var token = _jwtTokenService.GenerateToken(request.Username, "Administrateur", "Utilisateur");
                return Ok(new JwtDTO { Token = token });
            }
            else if (request.Username == "user" && request.Password == "user")
            {
                var token = _jwtTokenService.GenerateToken(request.Username, "Utilisateur");
                return Ok(new JwtDTO { Token = token });
            }

            throw new UnauthorizedAccessException("Nom d'utilisateur ou mot de passe incorrect.");
        }
    }
}
