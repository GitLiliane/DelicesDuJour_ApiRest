using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Ingredients;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngred;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Utilisateurs;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.DataAccessLayer.Session.MariaDB;
using DelicesDuJour_ApiRest.DataAccessLayer.Session.MySQL;
using DelicesDuJour_ApiRest.DataAccessLayer.Session.PostGres;
using DelicesDuJour_ApiRest.DataAccessLayer.Unit_of_Work;
using DelicesDuJour_ApiRest.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DelicesDuJour_ApiRest.DataAccessLayer
{
    /// <summary>
    /// Extension pour l'injection des dépendances DAL (Data Access Layer).
    /// </summary>
    public static class DalExt
    {
        /// <summary>
        /// Ajoute les services nécessaires pour la DAL et configure le provider de base de données.
        /// </summary>
        /// <param name="services">Collection de services pour l'injection de dépendances.</param>
        /// <param name="settings">Paramètres de configuration de la base de données.</param>
        public static void AddDal(this IServiceCollection services, IDatabaseSettings settings)
        {
            // Configuration du provider de base de données
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

            // Unit of Work
            services.AddScoped<IUoW, UoW>();

            // Repositories
            services.AddTransient<IRecetteRepository, RecetteRepository>();
            services.AddTransient<IEtapeRepository, EtapeRepository>();
            services.AddTransient<ICategorieRepository, CategorieRepository>();
            services.AddTransient<IIngredientRepository, IngredientRepository>();
            services.AddTransient<IQuantiteIngredRepository, QuantiteIngredRepository>();
            services.AddTransient<IUtilisateurRepository, UtilisateurRepository>();
        }
    }
}
