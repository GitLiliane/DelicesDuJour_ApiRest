using Microsoft.AspNetCore.Mvc;

namespace DelicesDuJour_ApiRest.Domain
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _Logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _Logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "\r\nException interceptée globalement. \r\n");
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
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                ErrorResponse response = new()
                {
                    Error = "Une erreur interne est survenue.",
                    Details = _env.IsDevelopment() ? $"{exception.GetType().Name} : {exception.Message}" : "Veuillez vous adresser à l'adminnistrateur du système."
                };
                return context.Response.WriteAsJsonAsync(response);
            }
        }

        class ErrorResponse
        {
            public string Error { get; set; }
            public string Details { get; set; }
        }
    }
}
