using Microsoft.AspNetCore.Mvc;

namespace DelicesDuJour_ApiRest.Domain
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Interception des erreurs 401 et 403 après exécution (pas d'exception levée)
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
                _logger.LogError(ex, "\r\nException interceptée globalement.\r\n");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Suivant le type d'exception, on peut retourner des codes HTTP et des réponses différentes.

            context.Response.ContentType = "application/json";

            if (exception is FluentValidation.ValidationException fvex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                ErrorResponse response = new()
                {
                    Error = "Des erreurs de validation sont survenues.",
                    Details = string.Join("\r", fvex.Errors.Select(e => e.ErrorMessage))
                };
                return context.Response.WriteAsJsonAsync(response);
            }
            else if (exception is UnauthorizedAccessException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                ErrorResponse response = new()
                {
                    Error = "Accès non autorisé.",
                    Details = exception.Message
                };
                return context.Response.WriteAsJsonAsync(response);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                ErrorResponse response = new()
                {
                    Error = "Une erreur interne est survenue.",
                    Details = _env.IsDevelopment() ? $"{exception.GetType().Name} : {exception.Message}" : "Veuillez vous adresser à l'administrateur du système."
                };
                return context.Response.WriteAsJsonAsync(response);
            }
        }

        private Task HandleStatusCodeAsync(HttpContext context, int statusCode)
        {
            context.Response.ContentType = "application/json";

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

            return response != null ? context.Response.WriteAsJsonAsync(response) : Task.CompletedTask;
        }        
    }
    class ErrorResponse
    {
        public string Error { get; set; }
        public string Details { get; set; }
    }
}
