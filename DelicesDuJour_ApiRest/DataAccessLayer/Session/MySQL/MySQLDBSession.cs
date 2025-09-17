using DelicesDuJour_ApiRest.Domain;
using MySql.Data.MySqlClient;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Session.MySQL
{
    public class MySQLDBSession : BaseSession
    {
        public MySQLDBSession(IDatabaseSettings settings)
        {
            Connection = new MySqlConnection(settings.ConnectionString);
            DatabaseProviderName = settings.DatabaseProviderName;
        }
    }
}
