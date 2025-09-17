using DelicesDuJour_ApiRest.Domain.BO;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace DelicesDuJour_ApiRest.Services
{
    public interface IBiblioService
    {
        #region Recettes

        Task<IEnumerable<Recette>> GetAllRecettesAsync();
        Task<Recette> GetRecetteByIdAsync(int id);
        Task<Recette> AddRecetteAsync(Recette newRecette);
        Task<Recette> ModifyRecetteAsync(Recette updateRecette);
        Task<bool> DeleteRecetteAsync(int id);

        #endregion Fin Recettes

        #region Etapes

        Task<IEnumerable<Etape>> GetAllEtapesAsync();
        Task<Etape> GetEtapeByIdAsync(TupleClass<int, int> key);
        Task<Etape> AddEtapeAsync(Etape newEtape);
        Task<Etape> ModifyEtapeAsync(Etape updateEtape);
        Task<bool> DeleteEtapeAsync(TupleClass<int, int> key);

        #endregion Fin Etapes

    }
}
