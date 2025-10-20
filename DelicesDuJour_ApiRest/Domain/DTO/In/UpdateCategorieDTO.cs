using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    /// <summary>
    /// Data Transfer Object (DTO) utilisé pour mettre à jour une catégorie.
    /// </summary>
    public class UpdateCategorieDTO
    {
        /// <summary>
        /// Nom de la catégorie à mettre à jour.
        /// </summary>
        public string nom { get; set; }
    }

    /// <summary>
    /// Définit les règles de validation pour la classe <see cref="UpdateCategorieDTO"/>.
    /// </summary>
    public class UpdateCategorieDTOValidator : AbstractValidator<UpdateCategorieDTO>
    {
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UpdateCategorieDTOValidator"/> et configure les règles de validation.
        /// </summary>
        public UpdateCategorieDTOValidator()
        {
            // Validation du nom de la catégorie : obligatoire
            RuleFor(e => e.nom)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le nom est obligatoire.");

            // Les paramètres de cascade de validation sont commentés mais peuvent être activés si nécessaire
            // RuleLevelCascadeMode = CascadeMode.Stop;
            // ClassLevelCascadeMode = CascadeMode.Stop;
        }
    }
}
