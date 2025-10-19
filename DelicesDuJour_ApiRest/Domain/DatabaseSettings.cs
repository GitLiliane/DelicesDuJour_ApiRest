using FluentValidation;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using System;
using System.Linq;

namespace DelicesDuJour_ApiRest.Domain
{
    public enum DatabaseProviderName
    {
        MariaDB,
        MySQL,
        SQLServer,
        PostgreSQL,
        Oracle
    }

    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public DatabaseProviderName? DatabaseProviderName { get; set; }

    }

    public class DatabaseSettingsValidator : AbstractValidator<DatabaseSettings>
    {
        public DatabaseSettingsValidator()
        {
            var connectionMessage = "La chaîne de connexion est invalide.";
            var providerMessage = $"Le type de base de données est invalide. Valeurs possibles : {string.Join(", ", Enum.GetNames(typeof(DatabaseProviderName)))}";

            RuleFor(x => x.ConnectionString)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(connectionMessage)
                .NotEmpty().WithMessage(connectionMessage);

            RuleFor(x => x.DatabaseProviderName)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(providerMessage)
                .IsInEnum().WithMessage(providerMessage);
        }
    }
}
