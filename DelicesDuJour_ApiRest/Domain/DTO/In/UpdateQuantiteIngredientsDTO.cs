using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    public class UpdateQuantiteIngredientsDTO
    {
        public int id_ingredient { get; set; }
        public int id_recette { get; set; }
        public string quantite { get; set; }
    }

    public class UpdateRecetteIngredientRelationshipDTOValidator : AbstractValidator<UpdateQuantiteIngredientsDTO>
    {
        public UpdateRecetteIngredientRelationshipDTOValidator()
        {
            // Arrêter la validation dès qu'une règle échoue
            //RuleLevelCascadeMode = CascadeMode.Stop;
            //ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(e => e.quantite).NotNull().NotEmpty().WithMessage("Le quantité est obligatoire.");

        }
    }
}
