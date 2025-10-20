using DelicesDuJour_ApiRest.Domain;
using System.Data;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Session
{
    /// <summary>
    /// Classe de base représentant une session de base de données.
    /// Elle gère la connexion, la transaction et le cycle de vie des ressources.
    /// </summary>
    public class BaseSession : IDBSession
    {
        /// <summary>
        /// Nom du fournisseur de base de données (MariaDB, MySQL, PostgreSQL, etc.).
        /// </summary>
        public DatabaseProviderName? DatabaseProviderName { get; protected set; }

        /// <summary>
        /// Connexion à la base de données active.
        /// </summary>
        public IDbConnection Connection { get; set; }

        /// <summary>
        /// Transaction actuellement en cours (null si aucune).
        /// </summary>
        public IDbTransaction Transaction { get; private set; }

        /// <summary>
        /// Indique si une transaction est actuellement active.
        /// </summary>
        public bool HasActiveTransaction => Transaction != null;

        /// <summary>
        /// Démarre une nouvelle transaction si aucune n’est déjà active.
        /// Ouvre la connexion si nécessaire.
        /// </summary>
        public void BeginTransaction()
        {
            if (Transaction == null)
            {
                if (Connection?.State != ConnectionState.Open)
                    Connection.Open();

                Transaction = Connection?.BeginTransaction();
            }
        }

        /// <summary>
        /// Valide la transaction en cours et ferme la connexion.
        /// </summary>
        public void Commit()
        {
            Transaction?.Commit();
            Transaction = null;
            Connection?.Close();
        }

        /// <summary>
        /// Annule la transaction en cours et ferme la connexion.
        /// </summary>
        public void Rollback()
        {
            Transaction?.Rollback();
            Transaction = null;
            Connection?.Close();
        }

        #region IDisposable Support

        private bool _disposed = false;

        /// <summary>
        /// Libère les ressources utilisées par la session.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libère les ressources managées et non managées.
        /// Peut être surchargée par les classes dérivées si elles possèdent des ressources supplémentaires à libérer.
        /// </summary>
        /// <param name="disposing">Indique si les ressources managées doivent être libérées.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Libère les ressources managées
                    Transaction?.Dispose();
                    Connection?.Dispose();
                }

                // Libère ici les ressources non managées si nécessaire

                _disposed = true;
            }
        }

        #endregion IDisposable Support
    }
}
