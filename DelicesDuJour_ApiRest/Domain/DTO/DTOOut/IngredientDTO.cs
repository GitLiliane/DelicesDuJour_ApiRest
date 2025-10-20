namespace DelicesDuJour_ApiRest.Domain.DTO.DTOOut
{
    /// <summary>
    /// Data Transfer Object (DTO) représentant un ingrédient d'une recette.
    /// </summary>
    public class IngredientDTO
    {
        /// <summary>
        /// Identifiant unique de l'ingrédient.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Nom de l'ingrédient.
        /// </summary>
        public string nom { get; set; }

        /// <summary>
        /// Quantité de l'ingrédient pour la recette (ex : "200g", "2 c. à soupe").
        /// </summary>
        public string quantite { get; set; }
    }
}
