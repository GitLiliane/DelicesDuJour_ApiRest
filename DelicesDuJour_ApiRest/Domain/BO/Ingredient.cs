namespace DelicesDuJour_ApiRest.Domain.BO
{
    /// <summary>
    /// Représente un ingrédient avec son nom et sa quantité.
    /// </summary>
    public class Ingredient
    {

        public int id { get; set; }

        public string nom { get; set; }

        public string quantite { get; set; }
    }
}
