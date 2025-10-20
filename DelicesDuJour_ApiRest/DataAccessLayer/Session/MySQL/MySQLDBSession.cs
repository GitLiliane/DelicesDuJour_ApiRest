using DelicesDuJour_ApiRest.Domain;
using MySql.Data.MySqlClient;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Session.MySQL
{
    /// <summary>
    /// Classe représentant une session de base de données spécifique à MySQL.
    /// Hérite de <see cref="BaseSession"/> et configure la connexion à la base MySQL.
    /// </summary>
    public class MySQLDBSession : BaseSession
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="MySQLDBSession"/>.
        /// </summary>
        /// <param name="settings">
        /// Paramètres de configuration de la base de données, incluant la chaîne de connexion
        /// et le nom du fournisseur de base de données.
        /// </param>
        public MySQLDBSession(IDatabaseSettings settings)
        {
            // Initialise la connexion MySQL à l’aide de la chaîne de connexion configurée.
            Connection = new MySqlConnection(settings.ConnectionString);

            // Définit le nom du fournisseur (ici "MySQL") à partir des paramètres.
            DatabaseProviderName = settings.DatabaseProviderName;
        }
    }
}
