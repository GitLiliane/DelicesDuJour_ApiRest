using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain
{
    public class JwtSettings : IJwtSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationMinutes { get; set; }
    }

    public class JwtSettingsValidator : AbstractValidator<JwtSettings>
    {
        public JwtSettingsValidator()
        {
            const int MinSecretLength = 32;
            RuleFor(x => x.Secret)
                .NotNull().NotEmpty().WithMessage("Le secret JWT est requis.")
                .MinimumLength(MinSecretLength)
                .WithMessage($"Le secret JWT doit contenir au moins {MinSecretLength} caractères.");

            RuleFor(x => x.Issuer)
                .NotNull().NotEmpty().WithMessage("L'émetteur (Issuer) est requis.");

            RuleFor(x => x.Audience)
                .NotNull().NotEmpty().WithMessage("L'audience (Audience) est requise.");

            RuleFor(x => x.ExpirationMinutes)
                .GreaterThan(0).WithMessage("La durée d'expiration doit être supérieure à 0.");
        }
    }
}
