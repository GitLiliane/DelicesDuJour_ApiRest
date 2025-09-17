using DelicesDuJour_ApiRest.Controllers;
using DelicesDuJour_ApiRest.DataAccessLayer;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;

namespace DelicesDuJour_ApiRest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add FluentValidation ti the container.
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // Build and validate settings, then add to the container.
            if (TryBuildSettings<IDatabaseSettings, DatabaseSettings, DatabaseSettingsValidator>(builder, "DatabaseSettings", out DatabaseSettings settings))
                builder.Services.AddSingleton<IDatabaseSettings>(settings);
            else
                return;

            // Add DAL to the container.
            builder.Services.AddDal(settings);

            // Add services to the container.
            builder.Services.AddBll();

            // Add controllers to the container.
            builder.Services.AddControllers();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Middleware globa d'exception
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

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


