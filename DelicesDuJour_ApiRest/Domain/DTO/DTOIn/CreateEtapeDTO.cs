using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.DTOIn
{
    /// <summary>
    /// Data Transfer Object (DTO) utilisé pour créer une nouvelle étape d'une recette.
    /// </summary>
    public class CreateEtapeDTO
    {
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
    /// Définit les règles de validation pour la classe <see cref="CreateEtapeDTO"/>.
    /// </summary>
    public class CreateEtapeDTOValidator : AbstractValidator<CreateEtapeDTO>
    {
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="CreateEtapeDTOValidator"/> et configure les règles de validation.
        /// </summary>
        public CreateEtapeDTOValidator()
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
