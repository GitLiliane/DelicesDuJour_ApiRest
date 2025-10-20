using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.Out;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DelicesDuJour_ApiRest.Domain.DTO.In
{
    /// <summary>
    /// Data Transfer Object (DTO) utilisé pour créer une nouvelle recette.
    /// </summary>
    public class CreateRecetteDTO
    {
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
        public int difficulte { get; set; }

        /// <summary>
        /// Liste des étapes de la recette à créer.
        /// </summary>
        public List<CreateEtapeDTO>? etapes { get; set; } = new List<CreateEtapeDTO>();

        /// <summary>
        /// Liste des ingrédients de la recette à créer.
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
    /// Définit les règles de validation pour la classe <see cref="CreateRecetteDTO"/>.
    /// </summary>
    public class CreateRecetteDTOValidator : AbstractValidator<CreateRecetteDTO>
    {
        /// <summary>
        /// Initialise une nouvelle instance de <see cref="CreateRecetteDTOValidator"/> et configure les règles de validation.
        /// </summary>
        public CreateRecetteDTOValidator()
        {
            // Validation du nom de la recette : obligatoire
            RuleFor(r => r.nom)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le nom est obligatoire.");

            // Validation du temps de préparation : obligatoire
            RuleFor(r => r.temps_preparation)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le temps de préparation est obligatoire.");

            // Validation du temps de cuisson : obligatoire
            RuleFor(r => r.temps_cuisson)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le temps de cuisson est obligatoire.");

            // Validation du niveau de difficulté : obligatoire
            RuleFor(r => r.difficulte)
                .NotNull()
                .NotEmpty()
                .WithMessage("Le niveau de difficulté est obligatoire.");

            // Les paramètres de cascade de validation sont commentés mais peuvent être activés si nécessaire
            // RuleLevelCascadeMode = CascadeMode.Stop;
            // ClassLevelCascadeMode = CascadeMode.Stop;
        }
    }
}
