using DelicesDuJour_ApiRest.Domain;
using Npgsql;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Session.PostGres
{
    public class PostGresDBSession : BaseSession
    {
        public PostGresDBSession(IDatabaseSettings settings)
        {
            Connection = new NpgsqlConnection(settings.ConnectionString);
            DatabaseProviderName = settings.DatabaseProviderName;
        }
    }
}

