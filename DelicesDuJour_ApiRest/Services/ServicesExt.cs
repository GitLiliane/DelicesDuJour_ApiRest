using System.Runtime.CompilerServices;

namespace DelicesDuJour_ApiRest.Services
{
    public static class ServicesExt
    {
        public static void AddBll(this IServiceCollection services)
        {
            services.AddTransient<IBiblioService, BiblioService>();
            services.AddTransient<IJwtTokenService, JwtTokenService>();
        }
    }
}
