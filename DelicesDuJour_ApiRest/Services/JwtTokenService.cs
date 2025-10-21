using DelicesDuJour_ApiRest.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DelicesDuJour_ApiRest.Services
{
    /// <summary>
    /// Service responsable de la génération des tokens JWT pour l'authentification et l'autorisation des utilisateurs.
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IJwtSettings _jwtSettings;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="JwtTokenService"/>.
        /// </summary>
        /// <param name="jwtSettings">Paramètres de configuration JWT injectés (clé secrète, émetteur, audience, durée de vie).</param>
        public JwtTokenService(IJwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        /// <summary>
        /// Génère un token JWT signé pour un utilisateur donné et les rôles qui lui sont attribués.
        /// </summary>
        /// <param name="username">Nom d'utilisateur ou identifiant de connexion.</param>
        /// <param name="roles">Liste des rôles associés à cet utilisateur (ex : Admin, Client, etc.).</param>
        /// <returns>Le token JWT signé sous forme de chaîne de caractères.</returns>
        public string GenerateToken(string username, params string[] roles)
        {
            // Création des "claims" : informations d'identité et métadonnées incluses dans le token
            var claims = new List<Claim>
            {
                // 'sub' représente le sujet du token (ici le nom d'utilisateur)
                new(JwtRegisteredClaimNames.Sub, username),

                // 'jti' est un identifiant unique du token (permet d'éviter les réutilisations ou collisions)
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                // Claim spécifique à .NET pour identifier l'utilisateur dans le système
                new(ClaimTypes.NameIdentifier, username)
            };

            // Ajout des rôles dans les claims pour la gestion des autorisations dans l’application
            foreach (var role in roles)
            {
                // Chaque rôle est ajouté comme un claim distinct
                claims.Add(new(ClaimTypes.Role, role.Trim()));
            }

            // Création de la clé de sécurité à partir de la clé secrète configurée dans les paramètres JWT
            // Cette clé est utilisée pour signer le token et garantir son intégrité
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            // Définition des informations de signature (algorithme utilisé pour la signature du token)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Construction du token JWT avec tous les paramètres nécessaires
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,               // Émetteur du token (souvent le nom du serveur ou de l'application)
                audience: _jwtSettings.Audience,           // Audience cible (clients autorisés à consommer le token)
                claims: claims,                            // Claims contenant les infos utilisateur et rôles
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes), // Date d’expiration du token
                signingCredentials: creds                  // Signature cryptographique
            );

            // Génération finale du token sous forme de chaîne encodée (JWT compact)
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
