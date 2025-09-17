using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.Domain.BO
{
    public class Etape
    {
        
        public TupleClass<int, int>  Key { get; set; }
        public string titre { get; set; }
        public string texte { get; set; }
        
    }
}
