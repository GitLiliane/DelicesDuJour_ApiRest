using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Ingredients;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngred;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.DataAccessLayer.Session.MariaDB;
using DelicesDuJour_ApiRest.DataAccessLayer.Session.MySQL;
using DelicesDuJour_ApiRest.DataAccessLayer.Session.PostGres;
using DelicesDuJour_ApiRest.DataAccessLayer.Unit_of_Work;
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

            services.AddScoped<IUoW, UoW>();
            services.AddTransient<IRecetteRepository, RecetteRepository>();
            services.AddTransient<IEtapeRepository, EtapeRepository>();
            services.AddTransient<ICategorieRepository, CategorieRepository>();
            services.AddTransient<IIngredientRepository, IngredientRepository>();
            services.AddTransient<IQuantiteIngredRepository, QuantiteIngredRepository>();



        }
    }
}
