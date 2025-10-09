using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.Domain.BO
{
    public class Etape
    {
        
        public int id_recette { get; set; }
        public int numero { get; set; }
        public string titre { get; set; }
        public string texte { get; set; }
        
    }
}
