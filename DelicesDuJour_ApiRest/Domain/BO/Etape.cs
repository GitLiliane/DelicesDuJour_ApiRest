using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DelicesDuJour_ApiRest.Domain.BO
{
    /// <summary>
    /// Représente une étape d'une recette avec son numéro, son titre et son texte descriptif.
    /// </summary>
    public class Etape
    {     
        public int id_recette { get; set; }

        public int numero { get; set; }

        public string titre { get; set; }
      
        public string texte { get; set; }
    }
}
