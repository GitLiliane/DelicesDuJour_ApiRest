using DelicesDuJour_ApiRest.Controllers;
using DelicesDuJour_ApiRest.DataAccessLayer;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace DelicesDuJour_ApiRest
{
    /// <summary>
    /// Point d'entrée principal de l'application Web API.
    /// Configure les services, la validation, l'authentification, la documentation Swagger et le pipeline HTTP.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Méthode principale de démarrage de l'application.
        /// </summary>
        /// <param name="args">Arguments passés à l'application lors du démarrage.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Enregistre tous les validateurs FluentValidation présents dans l’assembly courant.
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // Charge et valide la configuration DatabaseSettings depuis appsettings.json.
            if (TryBuildSettings<IDatabaseSettings, DatabaseSettings, DatabaseSettingsValidator>(builder, "DatabaseSettings", out DatabaseSettings dbSettings))
                builder.Services.AddSingleton<IDatabaseSettings>(dbSettings);
            else
                return; // Si la configuration est invalide, l'application s'arrête.

            // Charge et valide la configuration JwtSettings depuis appsettings.json.
            if (TryBuildSettings<IJwtSettings, JwtSettings, JwtSettingsValidator>(builder, "JwtSettings", out JwtSettings jwtSettings))
                builder.Services.AddSingleton<IJwtSettings>(jwtSettings);
            else
                return;

            // Injection de la couche d’accès aux données (DAL)
            builder.Services.AddDal(dbSettings);

            // Injection de la couche métier (BLL)
            builder.Services.AddBll();

            // Configuration des contrôleurs avec une politique d’autorisation globale :
            // toutes les actions nécessitent un utilisateur authentifié par défaut.
            // Les endpoints publics doivent explicitement utiliser [AllowAnonymous].
            builder.Services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            // Configuration du système d’authentification JWT.
            builder.Services.AddAuthentication(options =>
            {
                // Schéma par défaut utilisé pour l’authentification et les erreurs 401.
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               // Définit les règles de validation des tokens JWT.
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true, // Vérifie que le token provient du bon émetteur.
                   ValidateAudience = true, // Vérifie que le token est destiné à la bonne audience.
                   ValidateLifetime = true, // Vérifie la date d’expiration du token.
                   ValidateIssuerSigningKey = true, // Vérifie la signature du token.

                   ValidIssuer = jwtSettings.Issuer,
                   ValidAudience = jwtSettings.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),

                   // Permet d’utiliser [Authorize(Roles = "admin")] avec le claim "role".
                   RoleClaimType = ClaimTypes.Role
               };
           });

            // Configuration de Swagger pour générer la documentation interactive de l’API.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // Définition de la documentation de base.
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Délices du jour", Version = "v1" });

                // Inclusion des commentaires XML des contrôleurs et modèles, si disponibles.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath, true);

                // Ajoute la définition du schéma de sécurité JWT dans Swagger.
                options.AddSecurityDefinition("jwt", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Saisir le token JWT généré après connexion.\n\nExemple : Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                });

                // Applique la sécurité JWT globalement dans l’interface Swagger.
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "jwt"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // Middleware global pour intercepter et gérer les exceptions de l’API.
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // En mode développement, active Swagger pour tester et documenter l’API.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Ajout de la sécurité au pipeline HTTP.
            app.UseAuthentication(); // Vérifie la validité du token JWT.
            app.UseAuthorization();  // Vérifie les rôles et permissions.

            // Active l’accès aux fichiers statiques (par ex. : photos des recettes).
            app.UseStaticFiles();

            // Mappe les routes des contrôleurs.
            app.MapControllers();

            // Lance le serveur et l’application.
            app.Run();
        }

        /// <summary>
        /// Tente de construire et de valider une configuration à partir d’une section appsettings.json.
        /// </summary>
        /// <typeparam name="TService">Type d’interface de service.</typeparam>
        /// <typeparam name="TImplementation">Type concret de la configuration.</typeparam>
        /// <typeparam name="TValidator">Type du validateur FluentValidation à utiliser.</typeparam>
        /// <param name="builder">Instance du WebApplicationBuilder.</param>
        /// <param name="sectionName">Nom de la section de configuration à charger.</param>
        /// <param name="settings">Instance de configuration validée en sortie.</param>
        /// <returns>True si la configuration est valide, sinon false.</returns>
        public static bool TryBuildSettings<TService, TImplementation, TValidator>(WebApplicationBuilder builder, string sectionName, out TImplementation settings)
            where TService : class
            where TImplementation : class, TService, new()
            where TValidator : AbstractValidator<TImplementation>, new()
        {
            // Lecture et liaison des paramètres depuis le fichier appsettings.json.
            settings = new TImplementation();
            builder.Configuration.GetSection(sectionName).Bind(settings);

            // Création d’un logger temporaire pour afficher les messages de démarrage.
            using var loggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Information);
            });
            var logger = loggerFactory.CreateLogger("Startup");

            // Validation des paramètres via FluentValidation.
            var validator = new TValidator();
            var result = validator.Validate(settings);

            if (result.IsValid)
            {
                logger.LogInformation("Configuration '{SectionName}' chargée et validée avec succès.", sectionName);
            }
            else
            {
                logger.LogError("Configuration invalide dans la section '{SectionName}'.", sectionName);
                foreach (var error in result.Errors)
                {
                    logger.LogError(" - {Property}: {ErrorMessage}", error.PropertyName, error.ErrorMessage);
                }
                settings = null;
            }

            return result.IsValid;
        }
    }
}
