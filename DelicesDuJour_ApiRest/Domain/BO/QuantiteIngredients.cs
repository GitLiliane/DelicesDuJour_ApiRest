namespace DelicesDuJour_ApiRest.Domain.BO
{
    public class QuantiteIngredients
    {
        public int id_ingredient { get; set; }
        public int id_recette { get; set; }
        public string quantite { get; set; }
    }
}
