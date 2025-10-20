using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.DTOIn
{
    /// <summary>
    /// Data Transfer Object (DTO) utilisé pour mettre à jour un ingrédient.
    /// </summary>
    public class UpdateIngredientDTO
    {
        /// <summary>
        /// Nom de l'ingrédient à mettre à jour.
        /// </summary>
        public string nom { get; set; }
    }

    /// <summary>
    /// Définit les règles de validation pour la classe <see cref="UpdateIngredientDTO"/>.
    /// </summary>
    public class UpdateIngredientDTOValidator : AbstractValidator<UpdateIngredientDTO>
    {
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UpdateIngredientDTOValidator"/> et configure les règles de validation.
        /// </summary>
        public UpdateIngredientDTOValidator()
        {
            // Validation du nom : obligatoire
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
