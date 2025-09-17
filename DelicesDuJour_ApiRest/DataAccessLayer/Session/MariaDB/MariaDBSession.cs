using DelicesDuJour_ApiRest.Domain;
using MySql.Data.MySqlClient;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Session.MariaDB
{
    public class MariaDBSession : BaseSession
    {
        public MariaDBSession(IDatabaseSettings settings)
        {
            Connection = new MySqlConnection(settings.ConnectionString);
            DatabaseProviderName = settings.DatabaseProviderName;
        }
    }
}
