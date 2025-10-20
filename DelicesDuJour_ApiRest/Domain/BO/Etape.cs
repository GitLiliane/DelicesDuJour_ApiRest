using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DelicesDuJour_ApiRest.Domain.BO
{
    /// <summary>
    /// Représente une étape d'une recette avec son numéro, son titre et son texte descriptif.
    /// </summary>
    public class Etape
    {
        /// <summary>
        /// Identifiant de la recette à laquelle cette étape appartient.
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
