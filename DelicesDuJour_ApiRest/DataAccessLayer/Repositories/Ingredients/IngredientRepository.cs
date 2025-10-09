using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Ingredients
{
    public class IngredientRepository : IIngredientRepository
    {
        const string INGREDIENT_TABLE = "ingredients";
        const string RECETTE_INGREDIENT_TABLE = "ingredients_recettes";
        readonly IDBSession _dbSession;

        public IngredientRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        public async Task<IEnumerable<Ingredient>> GetAllAsync()
        {
            string query = $"SELECT * FROM {INGREDIENT_TABLE}";
            return await _dbSession.Connection.QueryAsync<Ingredient>(query, transaction: _dbSession.Transaction);
        }

        public async Task<Ingredient> GetAsync(int id)
        {
            string query = $"SELECT * FROM {INGREDIENT_TABLE} WHERE id = @id";
            return await _dbSession.Connection.QuerySingleOrDefaultAsync<Ingredient>(query, new { id }, transaction: _dbSession.Transaction);
        }

        public async Task<Ingredient> CreateAsync(Ingredient ingredient)
        {
            string query = string.Empty;

            if (_dbSession.DatabaseProviderName == DatabaseProviderName.MariaDB || _dbSession.DatabaseProviderName == DatabaseProviderName.MySQL)
                query = $"INSERT INTO {INGREDIENT_TABLE}(nom) VALUES(@nom); Select LAST_INSERT_ID()";
            else if (_dbSession.DatabaseProviderName == DatabaseProviderName.PostgreSQL)
                query = $"INSERT INTO {INGREDIENT_TABLE}(nom) VALUES(@nom) RETURNING id";

            int lastId = await _dbSession.Connection.ExecuteScalarAsync<int>(query, ingredient, transaction: _dbSession.Transaction);
            ingredient.id = lastId;
            return ingredient;
        }

        public async Task<Ingredient> ModifyAsync(Ingredient ingredient)
        {
            string query = $"UPDATE {INGREDIENT_TABLE} SET nom = @nom WHERE id = @Id";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, ingredient, transaction: _dbSession.Transaction);
            return numLine == 0 ? null : ingredient;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            string query = $"DELETE FROM {INGREDIENT_TABLE} WHERE id = @id";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { id }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        public async Task<IEnumerable<Ingredient>> GetIngredientsByIdRecetteAsync(int idRecette)
        {
            string query = $"SELECT * FROM {INGREDIENT_TABLE} i JOIN {RECETTE_INGREDIENT_TABLE} ri ON i.id = ri.id_ingredient WHERE ri.id_recette = @idRecette";
            return await _dbSession.Connection.QueryAsync<Ingredient>(query, new { idRecette }, transaction: _dbSession.Transaction);
        }

    }
}
