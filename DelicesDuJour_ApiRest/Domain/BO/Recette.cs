using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DelicesDuJour_ApiRest.Domain.BO
{
    /// <summary>
    /// Représente une recette avec ses propriétés, étapes, ingrédients et catégories.
    /// </summary>
    public class Recette
    {     
        public int Id { get; set; }

        public string nom { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan temps_preparation { get; set; }
     
        [DataType(DataType.Time)]
        public TimeSpan temps_cuisson { get; set; }
     
        [Range(1, 3)]
        public int difficulte { get; set; }
       
        public List<Etape>? etapes { get; set; } = new List<Etape>();
       
        public List<Ingredient>? ingredients { get; set; } = new List<Ingredient>();

        public List<Categorie> categories { get; set; } = new List<Categorie>();
        
        public string? photo { get; set; }

    }
}
