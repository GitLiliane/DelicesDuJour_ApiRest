using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.Domain.DTO.Out
{
    public class EtapeDTO
    {
        public TupleDTO<int, int> Key { get; set;}
        public string titre { get; set; }
        public string texte { get; set; }
    }
}
