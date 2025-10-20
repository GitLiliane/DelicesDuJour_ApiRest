using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    /// <summary>
    /// Data Transfer Object (DTO) utilisé pour mettre à jour une étape d'une recette.
    /// </summary>
    public class UpdateEtapeDTO
    {
        /// <summary>
        /// Identifiant de la recette à laquelle l'étape appartient.
        /// </summary>
        public int id_recette { get; set; }

        /// <summary>
        /// Numéro de l'étape dans la séquence de la recette.
        /// </summary>
        public int numero { get; set; }

        /// <summary>
        /// Titre ou résumé de l'étape.
        /// </summary>
        public string titre { get; set; }

        /// <summary>
        /// Description détaillée de l'étape.
        /// </summary>
        public string texte { get; set; }
    }

    /// <summary>
    /// Définit les règles de validation pour la classe <see cref="UpdateEtapeDTO"/>.
    /// </summary>
    public class UpdateEtapeDTOValidator : AbstractValidator<UpdateEtapeDTO>
    {
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UpdateEtapeDTOValidator"/> et configure les règles de validation.
        /// </summary>
        public UpdateEtapeDTOValidator()
        {
            // Validation du numéro de l'étape : obligatoire
            RuleFor(e => e.numero)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le numéro de l'étape est obligatoire.");

            // Validation du titre de l'étape : obligatoire
            RuleFor(e => e.titre)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le titre est obligatoire.");

            // Validation du texte de l'étape : obligatoire
            RuleFor(e => e.texte)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le texte est obligatoire.");

            // Les paramètres de cascade de validation sont commentés mais peuvent être activés si nécessaire
            // RuleLevelCascadeMode = CascadeMode.Stop;
            // ClassLevelCascadeMode = CascadeMode.Stop;
        }
    }
}
