using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;
using Microsoft.AspNetCore.Http;
using Npgsql;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Utilisateurs
{
    public class UtilisateurRepository : IUtilisateurRepository
    {
        
        private const string UTILISATEUR_TABLE = "utilisateurs";
        private const string ROLE_TABLE = "roles";

        // Session de base de données partagée pour gérer les connexions et transactions
        private readonly IDBSession _dbSession;

        /// <summary>
        /// Constructeur du repository des recettes.
        /// </summary>
        /// <param name="dBSession">Session de base de données injectée par dépendance.</param>
        public UtilisateurRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }


        /// <summary>
        /// Récupère un utilisateur et son rôle à partir de son identifiant (login).
        /// </summary>
        /// <param name="username">Identifiant de l'utilisateur</param>
        /// <returns>Un objet Utilisateur ou null si non trouvé</returns>
        public async Task<Utilisateur?> GetByUsernameAsync(string username)
        {
            string query = $@"
                SELECT 
                    u.id, 
                    u.identifiant, 
                    u.pass_word, 
                    u.role_id,
                    r.id, 
                    r.nom 
                FROM {UTILISATEUR_TABLE} u
                LEFT JOIN {ROLE_TABLE} r ON u.role_id = r.id
                WHERE u.identifiant = @Username
                LIMIT 1;";

            // Utilisation de Dapper pour mapper Utilisateur + Role
            var result = await _dbSession.Connection.QueryAsync<Utilisateur, Role, Utilisateur>(
                query,
                (u, role) =>
                {
                    u.role = role;
                    return u;
                },
                new { Username = username },
                transaction: _dbSession.Transaction,
                splitOn: "id"
            );

            // Retourne un seul utilisateur ou null
            return result.SingleOrDefault();
        }
    }
}
