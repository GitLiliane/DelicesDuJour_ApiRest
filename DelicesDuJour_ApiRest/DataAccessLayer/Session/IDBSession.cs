using DelicesDuJour_ApiRest.Domain;
using System.Data;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Session
{
    /// <summary>
    /// Interface représentant une session de base de données.
    /// Fournit la gestion de la connexion, de la transaction et du cycle de vie.
    /// </summary>
    public interface IDBSession : IDisposable
    {
        /// <summary>
        /// Nom du fournisseur de base de données (MariaDB, MySQL, PostgreSQL, etc.).
        /// </summary>
        DatabaseProviderName? DatabaseProviderName { get; }

        /// <summary>
        /// Connexion à la base de données active.
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Transaction actuellement en cours (null si aucune).
        /// </summary>
        IDbTransaction Transaction { get; }

        /// <summary>
        /// Indique si une transaction est actuellement active.
        /// </summary>
        bool HasActiveTransaction { get; }

        /// <summary>
        /// Démarre une nouvelle transaction si aucune n’est déjà active.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Valide la transaction en cours.
        /// </summary>
        void Commit();

        /// <summary>
        /// Annule la transaction en cours.
        /// </summary>
        void Rollback();
    }
}
