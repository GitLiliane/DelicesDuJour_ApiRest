using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace DelicesDuJour_ApiRest.Domain
{
    /// <summary>
    /// Middleware global pour intercepter toutes les exceptions et gérer les erreurs HTTP.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="GlobalExceptionMiddleware"/>.
        /// </summary>
        /// <param name="next">Delegate représentant le middleware suivant dans le pipeline.</param>
        /// <param name="logger">Logger pour consigner les exceptions.</param>
        /// <param name="env">Environnement de l'application (Développement, Production...).</param>
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Méthode appelée par le pipeline pour traiter la requête HTTP.
        /// Intercepte les exceptions et gère certains codes HTTP après exécution.
        /// </summary>
        /// <param name="context">Contexte HTTP de la requête.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Exécute le middleware suivant dans le pipeline
                await _next(context);

                // Vérifie les codes HTTP 401 et 403 même si aucune exception n'a été levée
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    await HandleStatusCodeAsync(context, 401);
                }
                else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    await HandleStatusCodeAsync(context, 403);
                }
            }
            catch (Exception ex)
            {
                // Log de l'exception interceptée
                _logger.LogError(ex, "\r\nException interceptée globalement.\r\n");

                // Gestion centralisée des exceptions
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Gère une exception et renvoie une réponse HTTP JSON appropriée selon le type d'exception.
        /// </summary>
        /// <param name="context">Contexte HTTP de la requête.</param>
        /// <param name="exception">Exception interceptée.</param>
        /// <returns>Une tâche représentant l'écriture asynchrone de la réponse HTTP.</returns>
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Définit le type de contenu de la réponse
            context.Response.ContentType = "application/json";

            // Gestion des erreurs de validation FluentValidation
            if (exception is FluentValidation.ValidationException fvex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                // Création de la réponse JSON contenant les erreurs de validation
                ErrorResponse response = new()
                {
                    Error = "Des erreurs de validation sont survenues.",
                    Details = string.Join("\r", fvex.Errors.Select(e => e.ErrorMessage))
                };

                // Retourne la réponse JSON
                return context.Response.WriteAsJsonAsync(response);
            }
            // Gestion des exceptions d'accès non autorisé
            else if (exception is UnauthorizedAccessException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                // Création de la réponse JSON
                ErrorResponse response = new()
                {
                    Error = "Accès non autorisé.",
                    Details = exception.Message
                };

                return context.Response.WriteAsJsonAsync(response);
            }
            // Gestion des autres exceptions (erreurs internes)
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // Création de la réponse JSON avec détails selon l'environnement
                ErrorResponse response = new()
                {
                    Error = "Une erreur interne est survenue.",
                    Details = _env.IsDevelopment() ? $"{exception.GetType().Name} : {exception.Message}" : "Veuillez vous adresser à l'administrateur du système."
                };

                return context.Response.WriteAsJsonAsync(response);
            }
        }

        /// <summary>
        /// Gère certains codes HTTP spécifiques (401, 403) et renvoie une réponse JSON.
        /// </summary>
        /// <param name="context">Contexte HTTP de la requête.</param>
        /// <param name="statusCode">Code HTTP à traiter.</param>
        /// <returns>Une tâche représentant l'écriture asynchrone de la réponse HTTP.</returns>
        private Task HandleStatusCodeAsync(HttpContext context, int statusCode)
        {
            // Définit le type de contenu de la réponse
            context.Response.ContentType = "application/json";

            // Création de la réponse JSON selon le code
            ErrorResponse response = statusCode switch
            {
                401 => new ErrorResponse
                {
                    Error = "Accès non autorisé.",
                    Details = "Vous devez être authentifié pour accéder à cette ressource."
                },
                403 => new ErrorResponse
                {
                    Error = "Accès interdit.",
                    Details = "Vous n'avez pas les droits nécessaires pour accéder à cette ressource."
                },
                _ => null
            };

            // Si réponse définie, écrit la réponse JSON, sinon tâche terminée
            return response != null ? context.Response.WriteAsJsonAsync(response) : Task.CompletedTask;
        }
    }

    /// <summary>
    /// Modèle représentant la réponse JSON pour les erreurs.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Message d'erreur principal.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Détails supplémentaires sur l'erreur.
        /// </summary>
        public string Details { get; set; }
    }
}
