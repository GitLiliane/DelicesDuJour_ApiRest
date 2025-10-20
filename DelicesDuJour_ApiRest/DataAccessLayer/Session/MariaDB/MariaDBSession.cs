using DelicesDuJour_ApiRest.Domain;
using MySql.Data.MySqlClient;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Session.MariaDB
{
    /// <summary>
    /// Classe représentant une session de base de données spécifique à MariaDB.
    /// Hérite de <see cref="BaseSession"/> et initialise la connexion MySQL.
    /// </summary>
    public class MariaDBSession : BaseSession
    {
        /// <summary>
        /// Constructeur de la session MariaDB.
        /// Initialise la connexion à la base de données en utilisant les paramètres fournis.
        /// </summary>
        /// <param name="settings">
        /// Objet contenant les paramètres de configuration de la base de données,
        /// tels que la chaîne de connexion et le nom du fournisseur.
        /// </param>
        public MariaDBSession(IDatabaseSettings settings)
        {
            // Crée une nouvelle connexion MySQL à partir de la chaîne de connexion fournie.
            Connection = new MySqlConnection(settings.ConnectionString);

            // Stocke le nom du fournisseur de base de données (ici "MariaDB").
            DatabaseProviderName = settings.DatabaseProviderName;
        }
    }
}

