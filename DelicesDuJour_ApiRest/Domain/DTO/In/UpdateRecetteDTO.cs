using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    /// <summary>
    /// Data Transfer Object (DTO) utilisé pour mettre à jour une recette.
    /// </summary>
    public class UpdateRecetteDTO
    {
        /// <summary>
        /// Identifiant unique de la recette à mettre à jour.
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
        /// Liste des étapes mises à jour de la recette.
        /// </summary>
        public List<UpdateEtapeDTO>? etapes { get; set; } = new List<UpdateEtapeDTO>();

        /// <summary>
        /// Liste des ingrédients mis à jour de la recette.
        /// </summary>
        public List<IngredientDTO>? ingredients { get; set; } = new List<IngredientDTO>();

        /// <summary>
        /// Liste des catégories associées à la recette.
        /// </summary>
        public List<CategorieDTO> categories { get; set; } = new List<CategorieDTO>();

        /// <summary>
        /// URL ou chemin de la photo de la recette.
        /// </summary>
        public string? photo { get; set; }

        //// <summary>
        //// Fichier de la photo de la recette (non utilisé actuellement).
        //// </summary>
        //// public IFormFile photoFile { get; set; }
    }

    /// <summary>
    /// Définit les règles de validation pour la classe <see cref="UpdateRecetteDTO"/>.
    /// </summary>
    public class UpdateRecetteDTOValidator : AbstractValidator<UpdateRecetteDTO>
    {
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UpdateRecetteDTOValidator"/> et configure les règles de validation.
        /// </summary>
        public UpdateRecetteDTOValidator()
        {
            // Validation du nom de la recette : obligatoire
            RuleFor(r => r.nom)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le nom est obligatoire.");

            // Validation du niveau de difficulté : obligatoire
            RuleFor(r => r.difficulte)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le niveau de difficulté est obligatoire.");

            // Les validations suivantes sont commentées, mais peuvent être activées si nécessaire
            RuleFor(r => r.etapes).NotNull().NotEmpty().WithMessage("Les étapes sont obligatoires.");
            RuleFor(r => r.ingredients).NotNull().NotEmpty().WithMessage("Les ingrédients sont obligatoires.");
            RuleFor(r => r.categories).NotNull().NotEmpty().WithMessage("Le choix de catégorie(s) est obligatoire.");
            // RuleFor(r => r.photo).NotNull().NotEmpty().WithMessage("Une image est requise.");
        }
    }
}
