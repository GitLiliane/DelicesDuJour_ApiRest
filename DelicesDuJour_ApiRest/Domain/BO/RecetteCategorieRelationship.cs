namespace DelicesDuJour_ApiRest.Domain.BO
{
    /// <summary>
    /// Représente la relation entre une recette et une catégorie.
    /// </summary>
    public class RecetteCategorieRelationship
    {
        /// <summary>
        /// Identifiant de la catégorie associée.
        /// </summary>
        public int id_categorie { get; set; }

        /// <summary>
        /// Identifiant de la recette associée.
        /// </summary>
        public int id_recette { get; set; }
    }
}
