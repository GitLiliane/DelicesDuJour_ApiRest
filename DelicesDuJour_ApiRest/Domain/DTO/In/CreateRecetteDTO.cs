using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    public class CreateRecetteDTO
    {
        public string nom { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan temps_preparation { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan temps_cuisson { get; set; }
               
        public int difficulte { get; set; }
        public List<CreateEtapeDTO>? etapes { get; set; } = new List<CreateEtapeDTO>();
        public List<IngredientDTO>? ingredients { get; set; } = new List<IngredientDTO>();
        public List<CategorieDTO> categories { get; set; } = new List<CategorieDTO>();
        public string? photo { get; set; }
        ////public IFormFile photoFile { get; set; }

    }

    public class CreateRecetteDTOValidator : AbstractValidator<CreateRecetteDTO>
    {
        public CreateRecetteDTOValidator()
        {
            // Arrêter la validation dès qu'une règle échoue
            //RuleLevelCascadeMode = CascadeMode.Stop;
            //ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(r => r.nom).NotNull().NotEmpty().WithMessage("Le nom est obligatoire.");
            RuleFor(r => r.temps_preparation).NotNull().NotEmpty().WithMessage("Le temps de préparation est obligatoire.");
            RuleFor(r => r.temps_cuisson).NotNull().NotEmpty().WithMessage("Le temps de cuisson est obligatoire.");
            RuleFor(r => r.difficulte).NotNull().NotEmpty().WithMessage("Le niveau de difficulté est obligatoire.");
        }
    }



}
