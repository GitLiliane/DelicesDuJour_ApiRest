using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Utilisateurs;
using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUtilisateurRepository _userRepo;

        public AuthService(IUtilisateurRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<Utilisateur?> AuthenticateAsync(string usernameOrEmail, string password)
        {
            // 1) Récupère l'utilisateur (et le rôle) depuis la BDD
            var utilisateur = await _userRepo.GetByUsernameAsync(usernameOrEmail);

            if (utilisateur is null)
                return null;

            // 2) Vérifie le mot de passe (bcrypt)
            // Utiliser la même librairie que pour le hash lors de l'inscription
            var hashed = utilisateur.pass_word ?? string.Empty;

            // Si la base contient des null ou un autre schéma -> retourner null
            bool valid = false;
            try
            {
                valid = BCrypt.Net.BCrypt.Verify(password, hashed);
            }
            catch
            {
                // en cas d'erreur de format du hash on considère invalide
                valid = false;
            }

            if (!valid)
                return null;

            // 3) Authentification OK : retourne l'utilisateur (avec role déjà renseigné)
            return utilisateur;
        }
    }
}
