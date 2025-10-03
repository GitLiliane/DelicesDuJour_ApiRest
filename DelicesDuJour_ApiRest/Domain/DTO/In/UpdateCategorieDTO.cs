using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    public class UpdateCategorieDTO
    {
        public string nom { get; set; }
    }

    public class UpdateCategorieDTOValidator : AbstractValidator<UpdateCategorieDTO>
    {
        public UpdateCategorieDTOValidator()
        {

            // Arrêter la validation dès qu'une règle échoue
            //RuleLevelCascadeMode = CascadeMode.Stop;
            //ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(e => e.nom).NotNull().NotEmpty().WithMessage("Le nom est obligatoire.");

        }
    }
}
