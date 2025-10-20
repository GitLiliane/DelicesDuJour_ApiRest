namespace DelicesDuJour_ApiRest.Domain.DTO.DTOOut
{
    /// <summary>
    /// Data Transfer Object (DTO) représentant la relation entre une recette et une catégorie.
    /// </summary>
    public class RecetteCategorieRelationshipDTO
    {
        /// <summary>
        /// Identifiant de la catégorie.
        /// </summary>
        public int idCategorie { get; set; }

        /// <summary>
        /// Identifiant de la recette.
        /// </summary>
        public int idRecette { get; set; }
    }
}
