using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.DTOIn
{
    /// <summary>
    /// Data Transfer Object (DTO) utilisé pour la connexion d'un utilisateur.
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// Nom d'utilisateur de l'utilisateur.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Mot de passe de l'utilisateur.
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Validateur FluentValidation pour <see cref="LoginDTO"/>.
    /// </summary>
    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        /// <summary>
        /// Initialise les règles de validation pour la connexion d'un utilisateur.
        /// </summary>
        public LoginDTOValidator()
        {
            // Validation du nom d'utilisateur : obligatoire et longueur minimale de 3 caractères
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Le nom d'utilisateur est requis.")
                .MinimumLength(3).WithMessage("Le nom d'utilisateur doit contenir au moins 3 caractères.");

            // Validation du mot de passe : obligatoire et longueur minimale de 4 caractères
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est requis.")
                .MinimumLength(4).WithMessage("Le mot de passe doit contenir au moins 4 caractères.");
        }
    }
}
