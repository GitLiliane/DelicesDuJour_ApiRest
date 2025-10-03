using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngredients
{
    public class QuantiteIngredientsRepository : IQuantiteIngredientsRepository
    {
        const string RECETTE_TABLE = "recettes";        
        const string RECETTE_INGREDIENT_TABLE = "ingredients_recettes";

        readonly IDBSession _dbSession;

        public QuantiteIngredientsRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        #region Relation Recette Ingredient

        public async Task<bool> AddRecetteIngredientRelationshipAsync(int idIngredient, int idRecette, CreateQuantiteIngredientsDTO CreateRelationRI)
        {
            string query = $"INSERT INTO {RECETTE_INGREDIENT_TABLE}(id_ingredient, id_recette, quantite) VALUES(@idIngredient, @idRecette, @quantite)";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { CreateRelationRI.id_ingredient, CreateRelationRI.id_recette, CreateRelationRI.quantite }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        public async Task<bool> RemoveRecetteIngredientRelationshipAsync(int idIngredient, int idRecette)
        {
            string query = $"DELETE FROM {RECETTE_INGREDIENT_TABLE} WHERE id_ingredient = @idIngredient AND id_recette = @idRecette";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idIngredient, idRecette }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        public async Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient)
        {
            string query = $"SELECT r.* FROM {RECETTE_TABLE} r JOIN {RECETTE_INGREDIENT_TABLE} ri ON r.id = ri.id_recette WHERE ri.id_ingredient = @idIngredient";
            return await _dbSession.Connection.QueryAsync<Recette>(query, new { idIngredient }, transaction: _dbSession.Transaction);
        }

        public async Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette)
        {
            string query = $"DELETE FROM {RECETTE_INGREDIENT_TABLE} WHERE id_recette = @idRecette";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idRecette }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        #endregion Fin relation Recette Ingredient
    }
}
