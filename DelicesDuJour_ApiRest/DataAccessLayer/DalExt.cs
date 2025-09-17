using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.DataAccessLayer.Session.MariaDB;
using DelicesDuJour_ApiRest.DataAccessLayer.Session.MySQL;
using DelicesDuJour_ApiRest.DataAccessLayer.Session.PostGres;
using DelicesDuJour_ApiRest.Domain;


namespace DelicesDuJour_ApiRest.DataAccessLayer
{
    public static class DalExt
    {
        public static void AddDal(this IServiceCollection services, IDatabaseSettings settings)
        {
            switch (settings.DatabaseProviderName)
            {
                case DatabaseProviderName.MariaDB:
                    services.AddScoped<IDBSession, MariaDBSession>();
                    break;
                case DatabaseProviderName.MySQL:
                    services.AddScoped<IDBSession, MySQLDBSession>();
                    break;
                case DatabaseProviderName.PostgreSQL:
                    services.AddScoped<IDBSession, PostGresDBSession>();
                    break;
            }


            services.AddTransient<IRecetteRepository, RecetteRepository>();
            services.AddTransient<IEtapeRepository, EtapeRepository>();

        }
    }
}
