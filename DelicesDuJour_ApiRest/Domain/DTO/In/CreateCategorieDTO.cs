using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    /// <summary>
    /// Data Transfer Object (DTO) utilisé pour créer une nouvelle catégorie.
    /// </summary>
    public class CreateCategorieDTO
    {
        /// <summary>
        /// Nom de la catégorie.
        /// </summary>
        public string nom { get; set; }
    }

    /// <summary>
    /// Définit les règles de validation pour la classe <see cref="CreateCategorieDTO"/>.
    /// </summary>
    public class CreateCategorieDTOValidator : AbstractValidator<CreateCategorieDTO>
    {
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="CreateCategorieDTOValidator"/> et configure les règles de validation.
        /// </summary>
        public CreateCategorieDTOValidator()
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
