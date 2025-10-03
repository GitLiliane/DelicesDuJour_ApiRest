using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes
{
    public interface IEtapeRepository : IGenericReadRepository<TupleClass<int, int>, Etape>, IGenericWriteRepository<TupleClass<int, int>, Etape>
    {
        // Ajouter ici des méthodes spécifiques au repository Book si nécessaire

    }

}
