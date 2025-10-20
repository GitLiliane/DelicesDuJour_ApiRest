using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain
{
    /// <summary>
    /// Classe représentant les paramètres JWT utilisés pour la génération et la validation des tokens.
    /// Implémente <see cref="IJwtSettings"/>.
    /// </summary>
    public class JwtSettings : IJwtSettings
    {
        /// <summary>
        /// Clé secrète utilisée pour signer le JWT.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Émetteur du JWT (Issuer).
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Destinataire du JWT (Audience).
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Durée de validité du JWT en minutes.
        /// </summary>
        public int ExpirationMinutes { get; set; }
    }

    /// <summary>
    /// Définit les règles de validation pour la classe <see cref="JwtSettings"/>.
    /// </summary>
    public class JwtSettingsValidator : AbstractValidator<JwtSettings>
    {
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="JwtSettingsValidator"/> et configure les règles de validation.
        /// </summary>
        public JwtSettingsValidator()
        {
            // Longueur minimale recommandée pour le secret JWT
            const int MinSecretLength = 32;

            // Validation du secret JWT : requis et longueur minimale
            RuleFor(x => x.Secret)
                .NotNull().NotEmpty().WithMessage("Le secret JWT est requis.")
                .MinimumLength(MinSecretLength)
                .WithMessage($"Le secret JWT doit contenir au moins {MinSecretLength} caractères.");

            // Validation de l'émetteur (Issuer) : requis
            RuleFor(x => x.Issuer)
                .NotNull().NotEmpty().WithMessage("L'émetteur (Issuer) est requis.");

            // Validation de l'audience (Audience) : requis
            RuleFor(x => x.Audience)
                .NotNull().NotEmpty().WithMessage("L'audience (Audience) est requise.");

            // Validation de la durée d'expiration : doit être supérieure à 0
            RuleFor(x => x.ExpirationMinutes)
                .GreaterThan(0).WithMessage("La durée d'expiration doit être supérieure à 0.");
        }
    }
}
