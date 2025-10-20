namespace DelicesDuJour_ApiRest.Domain.BO
{
    /// <summary>
    /// Représente la quantité d'un ingrédient pour une recette donnée.
    /// </summary>
    public class QuantiteIngredients
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
        /// Quantité de l'ingrédient pour cette recette (ex : "200g", "2 c. à soupe").
        /// </summary>
        public string quantite { get; set; }
    }
}
