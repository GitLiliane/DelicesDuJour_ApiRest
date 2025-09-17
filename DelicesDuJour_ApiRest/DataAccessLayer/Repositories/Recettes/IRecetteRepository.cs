using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes
{
    public interface IRecetteRepository : IGenericReadRepository<int, Recette>, IGenericWriteRepository<int, Recette>
    {
        // Ajouter ici des méthodes spécifiques au repository Book si nécessaire

    }
}
