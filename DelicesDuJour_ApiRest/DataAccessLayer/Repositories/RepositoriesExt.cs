using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories
{
    public static class RepositoriesExt
    {
        public static void AddDal(this IServiceCollection services)
        {
            services.AddTransient<IRecetteRepository, RecetteRepository>();
            services.AddTransient<IEtapeRepository, EtapeRepository>();

        }
    }
}
