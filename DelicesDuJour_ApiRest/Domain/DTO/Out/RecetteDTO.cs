using DelicesDuJour_ApiRest.Domain.BO;
using System;
using System.ComponentModel.DataAnnotations;

namespace DelicesDuJour_ApiRest.Domain.DTO.Out
{
    public class RecetteDTO
    {
        public int Id { get; set; }
        public string nom { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan temps_preparation { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan temps_cuisson { get; set; }

        [Range(1, 3)]
        public int difficulte { get; set; }
        public List<EtapeDTO>? etapes { get; set; } = new List<EtapeDTO>();
        public List<IngredientDTO>? ingredients { get; set; } = new List<IngredientDTO>();

        public List<CategorieDTO> categories { get; set; } = new List<CategorieDTO>();

        public string? photo { get; set; }
        ////public IFormFile? photoFile { get; set; }

    }
}
