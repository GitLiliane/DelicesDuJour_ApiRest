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
        /// <summary>
        /// Identifiant unique de la recette.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom de la recette.
        /// </summary>
        public string nom { get; set; }

        /// <summary>
        /// Temps de préparation de la recette.
        /// </summary>
        [DataType(DataType.Time)]
        public TimeSpan temps_preparation { get; set; }

        /// <summary>
        /// Temps de cuisson de la recette.
        /// </summary>
        [DataType(DataType.Time)]
        public TimeSpan temps_cuisson { get; set; }

        /// <summary>
        /// Niveau de difficulté de la recette (1 à 3).
        /// </summary>
        [Range(1, 3)]
        public int difficulte { get; set; }

        /// <summary>
        /// Liste des étapes de la recette.
        /// </summary>
        public List<Etape>? etapes { get; set; } = new List<Etape>();

        /// <summary>
        /// Liste des ingrédients de la recette.
        /// </summary>
        public List<Ingredient>? ingredients { get; set; } = new List<Ingredient>();

        /// <summary>
        /// Liste des catégories associées à la recette.
        /// </summary>
        public List<Categorie> categories { get; set; } = new List<Categorie>();

        /// <summary>
        /// URL ou chemin de la photo de la recette.
        /// </summary>
        public string? photo { get; set; }

        //// <summary>
        //// Fichier de la photo de la recette (non utilisé actuellement).
        //// </summary>
        //// public IFormFile photoFile { get; set; }
    }
}
