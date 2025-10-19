using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    public class UpdateQuantiteIngredientsDTO
    {
        public int id_ingredient { get; set; }
        public int id_recette { get; set; }
        public string nom { get; set; }
        public string quantite { get; set; }
    }

    public class UpdateRecetteIngredientRelationshipDTOValidator : AbstractValidator<UpdateQuantiteIngredientsDTO>
    {
        public UpdateRecetteIngredientRelationshipDTOValidator()
        {
            // Arrêter la validation dès qu'une règle échoue
            //RuleLevelCascadeMode = CascadeMode.Stop;
            //ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(e => e.nom).NotNull().NotEmpty().WithMessage("Le nom de l'ingrédient est obligatoire.");
            RuleFor(e => e.quantite).NotNull().NotEmpty().WithMessage("La quantité est obligatoire.");

        }
    }
}
