using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.Domain.DTO.Out
{
    public class EtapeDTO
    {
        public int id_recette { get; set; }
        public int numero { get; set; }
        public string titre { get; set; }
        public string texte { get; set; }
    }
}
