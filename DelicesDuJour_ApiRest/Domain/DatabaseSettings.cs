using FluentValidation;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using System;
using System.Linq;

namespace DelicesDuJour_ApiRest.Domain
{
    /// <summary>
    /// Énumère les différents types de fournisseurs de base de données supportés.
    /// </summary>
    public enum DatabaseProviderName
    {
        /// <summary>
        /// Fournisseur MariaDB.
        /// </summary>
        MariaDB,
        /// <summary>
        /// Fournisseur MySQL.
        /// </summary>
        MySQL,
        /// <summary>
        /// Fournisseur Microsoft SQL Server.
        /// </summary>
        SQLServer,
        /// <summary>
        /// Fournisseur PostgreSQL.
        /// </summary>
        PostgreSQL,
        /// <summary>
        /// Fournisseur Oracle Database.
        /// </summary>
        Oracle
    }

    /// <summary>
    /// Représente la configuration nécessaire pour se connecter à une base de données.
    /// </summary>
    public class DatabaseSettings : IDatabaseSettings
    {
        /// <summary>
        /// Chaîne de connexion à la base de données.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Type du fournisseur de base de données utilisé.
        /// </summary>
        public DatabaseProviderName? DatabaseProviderName { get; set; }
    }

    /// <summary>
    /// Définit les règles de validation pour la classe <see cref="DatabaseSettings"/>.
    /// </summary>
    public class DatabaseSettingsValidator : AbstractValidator<DatabaseSettings>
    {
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="DatabaseSettingsValidator"/>.
        /// Configure les règles de validation pour la chaîne de connexion et le fournisseur de base de données.
        /// </summary>
        public DatabaseSettingsValidator()
        {
            /// <summary>
            /// Message d’erreur si la chaîne de connexion est invalide.
            /// </summary>
            var connectionMessage = "La chaîne de connexion est invalide.";

            /// <summary>
            /// Message d’erreur si le fournisseur de base de données est invalide.
            /// </summary>
            var providerMessage = $"Le type de base de données est invalide. Valeurs possibles : {string.Join(", ", Enum.GetNames(typeof(DatabaseProviderName)))}";

            /// <summary>
            /// Règle : la chaîne de connexion ne doit pas être nulle ni vide.
            /// </summary>
            RuleFor(x => x.ConnectionString)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(connectionMessage)
                .NotEmpty().WithMessage(connectionMessage);

            /// <summary>
            /// Règle : le fournisseur de base de données doit être défini et appartenir à l’énumération.
            /// </summary>
            RuleFor(x => x.DatabaseProviderName)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(providerMessage)
                .IsInEnum().WithMessage(providerMessage);
        }
    }
}
