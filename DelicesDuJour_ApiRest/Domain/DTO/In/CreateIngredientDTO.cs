using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    /// <summary>
    /// Data Transfer Object (DTO) utilisé pour créer un nouvel ingrédient.
    /// </summary>
    public class CreateIngredientDTO
    {
        /// <summary>
        /// Nom de l'ingrédient.
        /// </summary>
        public string nom { get; set; }

        /// <summary>
        /// Quantité de l'ingrédient (ex : "200g", "2 c. à soupe").
        /// </summary>
        public string quantite { get; set; }
    }

    /// <summary>
    /// Définit les règles de validation pour la classe <see cref="CreateIngredientDTO"/>.
    /// </summary>
    public class CreateIngredientDTOValidator : AbstractValidator<CreateIngredientDTO>
    {
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="CreateIngredientDTOValidator"/> et configure les règles de validation.
        /// </summary>
        public CreateIngredientDTOValidator()
        {
            // Validation du nom de l'ingrédient : obligatoire
            RuleFor(e => e.nom)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le nom est obligatoire.");

            // Validation de la quantité de l'ingrédient : obligatoire
            RuleFor(e => e.quantite)
                .NotNull()
                .NotEmpty()
                .WithMessage("La quantité d'ingrédient est obligatoire.");

            // Les paramètres de cascade de validation sont commentés mais peuvent être activés si nécessaire
            // RuleLevelCascadeMode = CascadeMode.Stop;
            // ClassLevelCascadeMode = CascadeMode.Stop;
        }
    }
}
