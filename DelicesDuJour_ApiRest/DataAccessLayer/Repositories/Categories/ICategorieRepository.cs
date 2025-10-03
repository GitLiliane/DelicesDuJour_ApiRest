using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories
{
    public interface ICategorieRepository : IGenericReadRepository<int, Categorie>, IGenericWriteRepository<int, Categorie>
    {
        // Ajouter ici des méthodes spécifiques au repository Book si nécessaire

        Task<IEnumerable<Categorie>> GetCategoriesByIdRecetteAsync(int idRecette);
        Task<bool> DeleteCategorieRelationsAsync(int idCategorie);

        Task<bool> HasRecetteRelationsAsync(int categorieId);
       

    }
}
