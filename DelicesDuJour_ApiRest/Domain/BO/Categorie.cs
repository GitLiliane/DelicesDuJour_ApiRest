namespace DelicesDuJour_ApiRest.Domain.BO
{
    /// <summary>
    /// Représente une catégorie pouvant être associée à une recette.
    /// </summary>
    public class Categorie
    {
        /// <summary>
        /// Identifiant unique de la catégorie.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Nom de la catégorie.
        /// </summary>
        public string nom { get; set; }
    }
}
