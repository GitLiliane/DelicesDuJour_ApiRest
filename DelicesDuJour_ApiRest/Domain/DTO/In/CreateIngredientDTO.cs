using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    public class CreateIngredientDTO
    {
        public string nom { get; set; }
        public string quantite { get; set; }
    }

    public class CreateIngredientDTOValidator : AbstractValidator<CreateIngredientDTO>
    {
        public CreateIngredientDTOValidator()
        {
            // Arrêter la validation dès qu'une règle échoue
            //RuleLevelCascadeMode = CascadeMode.Stop;
            //ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(e => e.nom).NotNull().NotEmpty().WithMessage("Le nom est obligatoire.");

        }
    }
}
