using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;
using DelicesDuJour_ApiRest.Domain.BO;
using System.Diagnostics.Eventing.Reader;

namespace DelicesDuJour_ApiRest.Services
{
    public class BiblioService : IBiblioService
    {        

        private readonly IRecetteRepository _recettes;
        private readonly IEtapeRepository _etapes;


        public BiblioService(IRecetteRepository recettes, IEtapeRepository etapes)
        {
            _recettes = recettes;
            _etapes = etapes;
        }

        #region Gestion des recettes
        public async Task<IEnumerable<Recette>> GetAllRecettesAsync()
        {
            return await _recettes.GetAllAsync();
        }

        public async Task<Recette> GetRecetteByIdAsync(int id)
        {
            return await _recettes.GetAsync(id);
        }

        public async Task<Recette> AddRecetteAsync(Recette newRecette)
        {
            return await _recettes.CreateAsync(newRecette);
        }

        public async Task<Recette> ModifyRecetteAsync(Recette updateRecette)
        {
            return await _recettes.ModifyAsync(updateRecette);
        }

        public async Task<bool> DeleteRecetteAsync(int id)
        {
            return await _recettes.DeleteAsync(id);
        }

        #endregion Fin Gestion des recettes

        #region Gestion des Etapes
       
        public async Task<IEnumerable<Etape>> GetAllEtapesAsync()
        {
            return await _etapes.GetAllAsync();
        }

        public async Task<Etape> GetEtapeByIdAsync(TupleClass<int, int> key)
        {
            return await _etapes.GetAsync(key);
        }

        public async Task<Etape> AddEtapeAsync(Etape newEtape)
        {
            return await _etapes.CreateAsync(newEtape);
        }

        public async Task<Etape> ModifyEtapeAsync(Etape updateEtape)
        {
            return await _etapes.ModifyAsync(updateEtape);
        }

        public async Task<bool> DeleteEtapeAsync(TupleClass<int, int> key)
        {
            return await _etapes.DeleteAsync(key);
        }


        #endregion Fin Etapes

    }
}
