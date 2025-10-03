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
    /// Configure les services, l'injection de dépendances, l'authentification, la documentation Swagger et le pipeline HTTP.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Méthode principale de démarrage de l'application.
        /// </summary>
        /// <param name="args">Arguments de la ligne de commande.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Ajout de FluentValidation au conteneur de services.
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // Construction et validation de DatabaseSettings, puis ajout au conteneur.
            if (TryBuildSettings<IDatabaseSettings, DatabaseSettings, DatabaseSettingsValidator>(builder, "DatabaseSettings", out DatabaseSettings dbSettings))
                builder.Services.AddSingleton<IDatabaseSettings>(dbSettings);
            else
                return;

            // Construction et validation de JwtSettings, puis ajout au conteneur.
            if (TryBuildSettings<IJwtSettings, JwtSettings, JwtSettingsValidator>(builder, "JwtSettings", out JwtSettings jwtSettings))
                builder.Services.AddSingleton<IJwtSettings>(jwtSettings);
            else
                return;

            // Ajout de la couche d'accès aux données (DAL) au conteneur.
            builder.Services.AddDal(dbSettings);

            // Ajout des services métier (BLL) au conteneur.
            builder.Services.AddBll();

            // Ajout des contrôleurs au conteneur de services avec une politique d'autorisation globale.
            // Cette configuration applique automatiquement l'attribut [Authorize] à tous les contrôleurs et actions,
            // ce qui signifie que toutes les requêtes nécessitent un utilisateur authentifié par défaut.
            // Les actions ou contrôleurs qui doivent rester accessibles sans authentification
            // peuvent être annotés avec [AllowAnonymous].

            builder.Services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            // Configuration de l'authentification JWT.
            // Cette section configure le middleware d'authentification pour utiliser le schéma "Bearer" avec JWT.
            // Elle définit également les paramètres de validation du token, tels que l'émetteur, l'audience, la durée de vie
            // et la clé de signature. Ces paramètres sont essentiels pour garantir que seuls les tokens valides et émis
            // par une source de confiance sont acceptés.
            //
            // Le champ RoleClaimType est défini pour permettre l'utilisation de l'attribut [Authorize(Roles = "...")].
            builder.Services.AddAuthentication(options =>
            {
                // Définit le schéma par défaut pour l'authentification et les défis (401 Unauthorized).
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               // Paramètres de validation du token JWT.
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true, // Vérifie que le token provient de l'émetteur attendu.
                   ValidateAudience = true, // Vérifie que le token est destiné à l'audience prévue.
                   ValidateLifetime = true, // Vérifie que le token n'est pas expiré.
                   ValidateIssuerSigningKey = true, // Vérifie que la signature du token est valide.

                   ValidIssuer = jwtSettings.Issuer, // Émetteur attendu.
                   ValidAudience = jwtSettings.Audience, // Audience attendue.
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)), // Clé secrète utilisée pour signer le token.

                   RoleClaimType = ClaimTypes.Role // Permet d'utiliser les rôles dans les contrôleurs via [Authorize(Roles = "...")].
               };
           });
            // Configuration de Swagger/OpenAPI pour la documentation de l'API.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // Affichage du titre et de la version dans Swagger UI
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Délices du jour", Version = "v1" });

                // Inclure les commentaires XML (Le fichier XML doit être généré dans les propriétés du projet)
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath, true);

                // Définition de la sécurité JWT dans Swagger
                options.AddSecurityDefinition("jwt", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Entrez le token JWT.\n\nExemple : eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                });

                // Application de la sécurité globalement (toutes les opérations dans swagger)
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "jwt"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // Middleware global de gestion des exceptions
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // Configuration du pipeline HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Ajout de la sécurité
            app.UseAuthentication(); // Toujours avant UseAuthorization
            app.UseAuthorization();

            // Mappage des contrôleurs
            app.MapControllers();

            // Démarrage de l'application
            app.Run();
        }

        /// <summary>
        /// Tente de construire et de valider une section de configuration, puis retourne l'instance validée.
        /// </summary>
        /// <typeparam name="TService">Type de service à enregistrer.</typeparam>
        /// <typeparam name="TImplementation">Type d'implémentation de la configuration.</typeparam>
        /// <typeparam name="TValidator">Type du validateur FluentValidation.</typeparam>
        /// <param name="builder">Le builder de l'application.</param>
        /// <param name="sectionName">Nom de la section de configuration.</param>
        /// <param name="settings">Instance validée de la configuration.</param>
        /// <returns>True si la configuration est valide, sinon false.</returns>
        public static bool TryBuildSettings<TService, TImplementation, TValidator>(WebApplicationBuilder builder, string sectionName, out TImplementation settings)
            where TService : class
            where TImplementation : class, TService, new()
            where TValidator : AbstractValidator<TImplementation>, new()
        {
            // Liaison de la configuration
            settings = new TImplementation();
            builder.Configuration.GetSection(sectionName).Bind(settings);

            // Création du logger via LoggerFactory
            using var loggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Information);
            });
            var logger = loggerFactory.CreateLogger("Startup");

            // Validation avec FluentValidation
            var validator = new TValidator();
            var result = validator.Validate(settings);

            if (result.IsValid)
                logger.LogInformation("Configuration '{SectionName}' chargée et validée.", sectionName);

            else
            {
                logger.LogError("Configuration invalide dans la section '{SectionName}'", sectionName);
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


