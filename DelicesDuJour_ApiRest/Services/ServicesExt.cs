using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Utilisateurs;
using System.Runtime.CompilerServices;

namespace DelicesDuJour_ApiRest.Services
{
    /// <summary>
    /// Classe statique contenant les méthodes d’extension pour enregistrer les services de la couche "Business Logic" (BLL) 
    /// dans le conteneur d’injection de dépendances.
    /// </summary>
    public static class ServicesExt
    {
        /// <summary>
        /// Méthode d’extension permettant d’ajouter les services métiers à la collection de services de l’application.
        /// Elle configure notamment l’injection de dépendances pour les services principaux de la logique métier.
        /// </summary>
        /// <param name="services">Collection des services utilisée pour l’injection de dépendances.</param>
        public static void AddBll(this IServiceCollection services)
        {
            // Enregistre le service de gestion des recettes, ingrédients, étapes et catégories.
            // "AddTransient" crée une nouvelle instance du service à chaque fois qu’il est demandé.
            services.AddTransient<IBiblioService, BiblioService>();

            // Enregistre le service de génération et de gestion des tokens JWT pour l’authentification.
            services.AddTransient<IJwtTokenService, JwtTokenService>();

            services.AddTransient<IUtilisateurRepository, UtilisateurRepository>();
            services.AddTransient<IAuthService, AuthService>();

        }
    }
}
