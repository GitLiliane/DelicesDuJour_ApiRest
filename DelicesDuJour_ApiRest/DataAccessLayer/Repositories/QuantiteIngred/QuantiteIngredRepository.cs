using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngred
{
    public class QuantiteIngredRepository : IQuantiteIngredRepository
    {
        const string RECETTE_TABLE = "recettes";
        const string INGREDIENT_TABLE = "ingredients";
        const string RECETTE_INGREDIENT_TABLE = "ingredients_recettes";

        readonly IDBSession _dbSession;

        public QuantiteIngredRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        #region Relation Recette Ingredient   

        public async Task<IEnumerable<QuantiteIngredients>> GetAllAsync()
        {
            string query = $"SELECT * FROM {RECETTE_INGREDIENT_TABLE}";
            return await _dbSession.Connection.QueryAsync<QuantiteIngredients>(query, transaction: _dbSession.Transaction);
        }

        public async Task<QuantiteIngredients> GetAsync((int, int) key)
        {        
            string query = $"SELECT * FROM {RECETTE_INGREDIENT_TABLE} WHERE id_ingredient = @id_ingredient AND id_recette = @id_recette";
            var reponse = await _dbSession.Connection.QuerySingleOrDefaultAsync<QuantiteIngredients>(query, new { id_ingredient = key.Item1, id_recette= key.Item2 }, transaction: _dbSession.Transaction);
            return reponse;
        }

        public async Task<QuantiteIngredients> CreateAsync(QuantiteIngredients CreateRelationRI)
        { 
            string query = $"INSERT INTO {RECETTE_INGREDIENT_TABLE}(id_ingredient, id_recette, quantite) VALUES(@idIngredient, @idRecette, @quantite)";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idIngredient = CreateRelationRI.id_ingredient, idRecette = CreateRelationRI.id_recette, quantite = CreateRelationRI.quantite }, transaction: _dbSession.Transaction);
            return numLine == 0 ? null : CreateRelationRI;
        }

        public async Task<QuantiteIngredients> ModifyAsync(QuantiteIngredients updateQuantiteIngredient)
        {           
            string query = $"UPDATE {RECETTE_INGREDIENT_TABLE} SET quantite = @quantite WHERE id_ingredient = @id_ingredient AND id_recette = @id_recette";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, updateQuantiteIngredient, transaction: _dbSession.Transaction);
            return numLine == 0 ? null : updateQuantiteIngredient;
        }

        public async Task<bool> DeleteAsync((int, int) key)
        {
            string query = $"DELETE FROM {RECETTE_INGREDIENT_TABLE} WHERE id_ingredient = @id_ingredient AND id_recette = @id_recette";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { id_ingredient = key.Item1, id_recette = key.Item2 }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        public async Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient)
        {
            string query = $"SELECT r.* FROM {RECETTE_TABLE} r JOIN {RECETTE_INGREDIENT_TABLE} ri ON r.id = ri.id_recette WHERE ri.id_ingredient = @idIngredient";
            return await _dbSession.Connection.QueryAsync<Recette>(query, new { idIngredient }, transaction: _dbSession.Transaction);
        }

        public async Task<IEnumerable<QuantiteIngredients>> GetIngredientsByIdRecetteAsync(int idRecette)
        {
            string query = $"SELECT * FROM {INGREDIENT_TABLE} i JOIN {RECETTE_INGREDIENT_TABLE} ri ON i.id = ri.id_ingredient WHERE ri.id_recette = @idRecette";
            return await _dbSession.Connection.QueryAsync<QuantiteIngredients>(query, new { idRecette }, transaction: _dbSession.Transaction);
        }

        public async Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette)
        {
            string query = $"DELETE FROM {RECETTE_INGREDIENT_TABLE} WHERE id_recette = @idRecette";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idRecette }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        public async Task<bool> DeleteIngredientRelationsRecetteAsync(int idIngredient)
        {
            string query = $"DELETE FROM {RECETTE_INGREDIENT_TABLE} WHERE id_ingredient = @idIngredient";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idIngredient }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }
        #endregion Fin relation Recette Ingredient
    }
}
