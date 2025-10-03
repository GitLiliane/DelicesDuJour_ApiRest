using DelicesDuJour_ApiRest.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DelicesDuJour_ApiRest.Services
{
    /// <summary>
    /// Service responsable de la génération des tokens JWT pour l'authentification et l'autorisation.
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IJwtSettings _jwtSettings;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="JwtTokenService"/>.
        /// </summary>
        /// <param name="jwtSettings">Paramètres de configuration JWT.</param>
        public JwtTokenService(IJwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }
        /// <summary>
        /// Génère un token JWT pour un utilisateur donné et ses rôles.
        /// </summary>
        /// <param name="username">Nom d'utilisateur pour lequel générer le token.</param>
        /// <param name="roles">Liste des rôles associés à l'utilisateur.</param>
        /// <returns>Le token JWT généré sous forme de chaîne.</returns>
        /// 
        public string GenerateToken(string username, params string[] roles)
        {
            // Ajoute les informations de l'utilisateur dans les claims
            var claims = new List<Claim>
            {
               new(JwtRegisteredClaimNames.Sub, username), // ID utilisateur (Subject)
               new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID du token
               new(ClaimTypes.NameIdentifier, username) // ID utilisateur (NameIdentifier)
            };

            // Ajoute les rôles de l'utilisateur dans les claims
            foreach (var role in roles)
            {
                claims.Add(new(ClaimTypes.Role, role.Trim()));
            }

            // Crée la clé de sécurité symétrique
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crée le token JWT
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: creds
            );

            // Retourne le token sous forme de chaîne
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
