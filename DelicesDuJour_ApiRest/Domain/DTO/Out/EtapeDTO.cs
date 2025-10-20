using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.Domain.DTO.Out
{
    /// <summary>
    /// Data Transfer Object (DTO) représentant une étape d'une recette.
    /// </summary>
    public class EtapeDTO
    {
        /// <summary>
        /// Identifiant de la recette à laquelle l'étape appartient.
        /// </summary>
        public int id_recette { get; set; }

        /// <summary>
        /// Numéro de l'étape dans la séquence de la recette.
        /// </summary>
        public int numero { get; set; }

        /// <summary>
        /// Titre ou résumé de l'étape.
        /// </summary>
        public string titre { get; set; }

        /// <summary>
        /// Description détaillée de l'étape.
        /// </summary>
        public string texte { get; set; }
    }
}
