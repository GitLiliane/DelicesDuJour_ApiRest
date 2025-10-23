namespace DelicesDuJour_ApiRest.Domain.BO
{
    /// <summary>
    /// Représente la quantité d'un ingrédient pour une recette donnée.
    /// </summary>
    public class QuantiteIngredients
    {
  
        public int id_ingredient { get; set; }

        public int id_recette { get; set; }

        public string quantite { get; set; }
    }
}
