using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Ingredients
{
    public interface IIngredientRepository : IGenericReadRepository<int, Ingredient>, IGenericWriteRepository<int, Ingredient>
    {
        Task<IEnumerable<Ingredient>> GetIngredientsByIdRecetteAsync(int idRecette);
        // Ajouter ici des méthodes spécifiques au repository Book si nécessaire
    }
}
