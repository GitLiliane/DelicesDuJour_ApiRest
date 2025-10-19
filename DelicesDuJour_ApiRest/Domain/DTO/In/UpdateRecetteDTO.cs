using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    public class UpdateRecetteDTO
    {
        public int Id { get; set; }
        public string nom { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan temps_preparation { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan temps_cuisson { get; set; }

        [Range(1, 3)]
        public int difficulte { get; set; }
        public List<UpdateEtapeDTO>? etapes { get; set; } = new List<UpdateEtapeDTO>();
        public List<IngredientDTO>? ingredients { get; set; } = new List<IngredientDTO>();
        public List<CategorieDTO> categories { get; set; } = new List<CategorieDTO>();
        public string? photo { get; set; }
        //public IFormFile photoFile { get; set; }

    }

    public class UpdateRecetteDTOValidator : AbstractValidator<UpdateRecetteDTO>
    {
        public UpdateRecetteDTOValidator()
        {

            // Arrêter la validation dès qu'une règle échoue
            //RuleLevelCascadeMode = CascadeMode.Stop;
            //ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(r => r.nom).NotNull().NotEmpty().WithMessage("Le nom est obligatoire.");
            RuleFor(r => r.difficulte).NotNull().NotEmpty().WithMessage("Le niveau de difficulté est obligatoire.");
            //RuleFor(r => r.etapes).NotNull().NotEmpty().WithMessage("Les étapes sont obligatoires.");
            //RuleFor(r => r.ingredients).NotNull().NotEmpty().WithMessage("Les ingrédients sont obligatoires.");
            //RuleFor(r => r.categories).NotNull().NotEmpty().WithMessage("Le choix de catégorie(s) est obligatoire.");
            //RuleFor(r => r.photo).NotNull().NotEmpty().WithMessage("Une image est requise.");
        }
    }
}
