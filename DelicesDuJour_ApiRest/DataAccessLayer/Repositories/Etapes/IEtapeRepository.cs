using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes
{
    public interface IEtapeRepository : IGenericReadRepository<(int, int), Etape>, IGenericWriteRepository<(int, int), Etape>
    {
        
        // Ajouter ici des méthodes spécifiques au repository Book si nécessaire

        Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int id);

        Task<bool> DeleteEtapesRelationByIdRecetteAsync(int id_recette);
    }

}
