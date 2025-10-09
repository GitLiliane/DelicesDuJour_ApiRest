using FluentValidation;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    public class UpdateEtapeDTO
    {
        public int id_recette { get; set; }
        public int numero { get; set; }
        public string titre { get; set; }
        public string texte { get; set; }
    }

    public class UpdateEtapeDTOValidator : AbstractValidator<UpdateEtapeDTO>
    {
        public UpdateEtapeDTOValidator()
        {

            // Arrêter la validation dès qu'une règle échoue
            //RuleLevelCascadeMode = CascadeMode.Stop;
            //ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(e => e.titre).NotNull().NotEmpty().WithMessage("Le titre est obligatoire.");
    
        }
    }
}
