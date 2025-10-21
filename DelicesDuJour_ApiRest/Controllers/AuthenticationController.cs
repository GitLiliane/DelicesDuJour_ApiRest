using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.DTOIn;
using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace DelicesDuJour_ApiRest.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAuthService _authService;

        public AuthenticationController(IJwtTokenService jwtTokenService, IAuthService AuthService)
        {
            _jwtTokenService = jwtTokenService;
            _authService = AuthService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(IValidator<LoginDTO> validator, [FromBody] LoginDTO request)
        {
            // Validation du modèle
            validator.ValidateAndThrow(request);

            // On demande au service de vérifier l'utilisateur dans la base
            Utilisateur utilisateur = await _authService.AuthenticateAsync(request.Username, request.Password);

            // Si aucun utilisateur n’est trouvé
            if (utilisateur == null)
            {
                // Retourne une réponse HTTP 401 (Unauthorized)
                return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect." });
            }

            // Vérifie le mot de passe avec BCrypt
            bool motDePasseValide = BCrypt.Net.BCrypt.Verify(request.Password, utilisateur.pass_word);

            if (!motDePasseValide)
            {
                // Mot de passe incorrect → réponse HTTP 401
                return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect." });
            }

            // Si tout est bon → on génère le token
            string nomUtilisateur;

            if (utilisateur.identifiant != null)
            {
                nomUtilisateur = utilisateur.identifiant;
            }
            else
            {
                nomUtilisateur = request.Username;
            }

            string[] roles = new string[] { utilisateur.role.nom };

            string token = _jwtTokenService.GenerateToken(nomUtilisateur, roles);

            // Retourne le token
            return Ok(new JwtDTO { Token = token });
        }
    }
}
