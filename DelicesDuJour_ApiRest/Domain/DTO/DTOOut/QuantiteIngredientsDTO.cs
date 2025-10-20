namespace DelicesDuJour_ApiRest.Domain.DTO.DTOOut
{
    /// <summary>
    /// Data Transfer Object (DTO) représentant la quantité d'un ingrédient pour une recette.
    /// </summary>
    public class QuantiteIngredientsDTO
    {
        /// <summary>
        /// Identifiant de l'ingrédient.
        /// </summary>
        public int id_ingredient { get; set; }

        /// <summary>
        /// Identifiant de la recette.
        /// </summary>
        public int id_recette { get; set; }

        /// <summary>
        /// Nom de l'ingrédient.
        /// </summary>
        public string nom { get; set; }

        /// <summary>
        /// Quantité de l'ingrédient pour cette recette (ex : "200g", "2 c. à soupe").
        /// </summary>
        public string quantite { get; set; }
    }
}
