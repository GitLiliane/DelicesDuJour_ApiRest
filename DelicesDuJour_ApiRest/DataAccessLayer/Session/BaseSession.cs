using DelicesDuJour_ApiRest.Domain;
using System.Data;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Session
{
    public class BaseSession : IDBSession
    {
        public DatabaseProviderName? DatabaseProviderName { get; protected set; }
        public IDbConnection Connection { get; protected set; }

        #region IDisposable Support

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Cette méthode doit être surchargée (override) dans les classes dérivées
        // si elles contiennent des ressources managées. Pour cela elle est virtuelle (virtual).

        protected virtual void Dispose(bool disposing)
        {
            if (! _disposed)
            {
                if (disposing)
                {
                    // Libérer les ressources managées
                    Connection?.Dispose();
                }

                // Libérer les ressources non managées ici si nécessaire

                _disposed = true;
            }
        }

        #endregion IDisposable Support
    }
}
