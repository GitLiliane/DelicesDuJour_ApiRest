using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes
{
    public interface IEtapeRepository : IGenericReadRepository<int, Etape>, IGenericWriteRepository<int, Etape>
    {
        // Ajouter ici des méthodes spécifiques au repository Book si nécessaire

        Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int id);
    }

}
