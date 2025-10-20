namespace DelicesDuJour_ApiRest.Domain.BO
{
    /// <summary>
    /// Représente un ingrédient avec son nom et sa quantité.
    /// </summary>
    public class Ingredient
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
        /// Quantité de l'ingrédient (ex : "200g", "2 c. à soupe").
        /// </summary>
        public string quantite { get; set; }
    }
}
