namespace DelicesDuJour_ApiRest.Services
{
    /// <summary>
    /// Définit le contrat du service responsable de la génération de jetons JWT.
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Génère un jeton JWT pour un utilisateur donné, incluant ses rôles.
        /// </summary>
        /// <param name="username">Nom d'utilisateur pour lequel le jeton est généré.</param>
        /// <param name="roles">Liste des rôles associés à l'utilisateur.</param>
        /// <returns>Un jeton JWT signé sous forme de chaîne de caractères.</returns>
        string GenerateToken(string username, params string[] roles);
    }
}
