using DelicesDuJour_ApiRest.Domain;
using System.Data;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Session
{
    public interface IDBSession : IDisposable
    {
        DatabaseProviderName? DatabaseProviderName { get; }
        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; }
        bool HasActiveTransaction { get; }


        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
