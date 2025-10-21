using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Vérifie les identifiants et retourne l'utilisateur si OK (avec son rôle).
        /// </summary>
        /// <returns>Utilisateur ou null si échec.</returns>
        Task<Utilisateur?> AuthenticateAsync(string username, string password);
    }
}
