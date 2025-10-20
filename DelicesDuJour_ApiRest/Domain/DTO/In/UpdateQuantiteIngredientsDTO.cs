using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    /// <summary>
    /// Data Transfer Object (DTO) utilisé pour mettre à jour la quantité d'un ingrédient pour une recette.
    /// </summary>
    public class UpdateQuantiteIngredientsDTO
    {
        /// <summary>
        /// Identifiant de l'ingrédient.
        /// </summary>
        public int id_ingredient { get; set; }

        /// <summary>
        /// Identifiant de la recette.
        /// </summary>
        public int id_recette { get; set; }

        /// <summary>
        /// Nom de l'ingrédient.
        /// </summary>
        public string nom { get; set; }

        /// <summary>
        /// Quantité de l'ingrédient pour cette recette (ex : "200g", "2 c. à soupe").
        /// </summary>
        public string quantite { get; set; }
    }

    /// <summary>
    /// Définit les règles de validation pour la classe <see cref="UpdateQuantiteIngredientsDTO"/>.
    /// </summary>
    public class UpdateRecetteIngredientRelationshipDTOValidator : AbstractValidator<UpdateQuantiteIngredientsDTO>
    {
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UpdateRecetteIngredientRelationshipDTOValidator"/> et configure les règles de validation.
        /// </summary>
        public UpdateRecetteIngredientRelationshipDTOValidator()
        {
            // Validation du nom de l'ingrédient : obligatoire
            RuleFor(e => e.nom)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le nom de l'ingrédient est obligatoire.");

            // Validation de la quantité : obligatoire
            RuleFor(e => e.quantite)
                .NotNull()
                .NotEmpty()
                .WithMessage("La quantité est obligatoire.");

            // Les paramètres de cascade de validation sont commentés mais peuvent être activés si nécessaire
            // RuleLevelCascadeMode = CascadeMode.Stop;
            // ClassLevelCascadeMode = CascadeMode.Stop;
        }
    }
}
