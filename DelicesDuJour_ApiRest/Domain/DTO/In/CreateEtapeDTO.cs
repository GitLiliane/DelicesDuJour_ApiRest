using DelicesDuJour_ApiRest.Domain.DTO.Out;
using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    public class CreateEtapeDTO
    {        
        public int numero { get; set; }
        public string titre { get; set; }
        public string texte { get; set; }
    }

    public class CreateEtapeDTOValidator : AbstractValidator<CreateEtapeDTO>
    {
        public CreateEtapeDTOValidator()
        {
            // Arrêter la validation dès qu'une règle échoue
            //RuleLevelCascadeMode = CascadeMode.Stop;
            //ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(e => e.numero).NotNull().NotEmpty().WithMessage("Le numéro de l'étape est obligatoire.");
            RuleFor(e => e.titre).NotNull().NotEmpty().WithMessage("Le titre est obligatoire.");
            RuleFor(e => e.texte).NotNull().NotEmpty().WithMessage("Le texte est obligatoire.");

        }
    }
}
