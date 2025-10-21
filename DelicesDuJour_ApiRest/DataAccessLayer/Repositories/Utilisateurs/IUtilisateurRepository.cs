using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Utilisateurs
{
    public interface IUtilisateurRepository
    {
        Task<Utilisateur?> GetByUsernameAsync(string username);
    }
}
