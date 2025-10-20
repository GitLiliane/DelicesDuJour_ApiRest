namespace DelicesDuJour_ApiRest.Domain.DTO.DTOOut
{
    /// <summary>
    /// Data Transfer Object (DTO) représentant une catégorie de recette.
    /// </summary>
    public class CategorieDTO
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

