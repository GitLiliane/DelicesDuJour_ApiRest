using DelicesDuJour_ApiRest.Domain;
using Npgsql;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Session.PostGres
{
    /// <summary>
    /// Classe représentant une session de base de données PostgreSQL.
    /// Elle gère la connexion et identifie le fournisseur de base de données utilisé.
    /// </summary>
    public class PostGresDBSession : BaseSession
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PostGresDBSession"/>.
        /// </summary>
        /// <param name="settings">
        /// Paramètres de configuration de la base de données (chaîne de connexion et nom du fournisseur).
        /// </param>
        public PostGresDBSession(IDatabaseSettings settings)
        {
            // Initialise une connexion PostgreSQL à l’aide de la chaîne de connexion configurée.
            Connection = new NpgsqlConnection(settings.ConnectionString);

            // Définit le fournisseur de base de données (ici PostgreSQL).
            DatabaseProviderName = settings.DatabaseProviderName;
        }
    }
}
