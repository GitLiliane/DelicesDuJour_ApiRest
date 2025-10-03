using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories
{
    public class CategorieRepository : ICategorieRepository
    {
        const string CATEGORIE_TABLE = "categories";
        const string RECETTE_CATEGORIE_TABLE = "categories_recettes";
        readonly IDBSession _dbSession;

        public CategorieRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        public async Task<IEnumerable<Categorie>> GetAllAsync()
        {
            string query = $"SELECT * FROM {CATEGORIE_TABLE}";
            return await _dbSession.Connection.QueryAsync<Categorie>(query, transaction: _dbSession.Transaction);
        }

        public async Task<Categorie> GetAsync(int id)
        {
            string query = $"SELECT * FROM {CATEGORIE_TABLE} WHERE id = @id";
            return await _dbSession.Connection.QuerySingleOrDefaultAsync<Categorie>(query, new { id }, transaction: _dbSession.Transaction);
        }

        public async Task<Categorie> CreateAsync(Categorie categorie)
        {
            string query = string.Empty;

            if (_dbSession.DatabaseProviderName == DatabaseProviderName.MariaDB || _dbSession.DatabaseProviderName == DatabaseProviderName.MySQL)
                query = $"INSERT INTO {CATEGORIE_TABLE}(nom) VALUES (@nom); Select LAST_INSERT_ID()";
            else if (_dbSession.DatabaseProviderName == DatabaseProviderName.PostgreSQL)
                query = $"INSERT INTO {CATEGORIE_TABLE}(nom) VALUES (@nom) RETURNING id";

            int lastId = await _dbSession.Connection.ExecuteScalarAsync<int>(query, categorie, transaction: _dbSession.Transaction);
            categorie.id = lastId;
            return categorie;
        }

        public async Task<Categorie> ModifyAsync(Categorie categorie)
        {
            string query = $"UPDATE {CATEGORIE_TABLE} SET nom = @nom WHERE id = @Id";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, categorie, transaction: _dbSession.Transaction);
            return numLine == 0 ? null : categorie;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            string query = $"DELETE FROM {CATEGORIE_TABLE} WHERE id = @id";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { id }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        #region Methods specific to CategorieRepository

        public async Task<IEnumerable<Categorie>> GetCategoriesByIdRecetteAsync(int idRecette)
        {
            string query = $"SELECT c.* FROM {CATEGORIE_TABLE} c JOIN {RECETTE_CATEGORIE_TABLE} rc ON c.id = rc.id_categorie WHERE rc.id_recette = @idRecette";
            return await _dbSession.Connection.QueryAsync<Categorie>(query, new { idRecette }, transaction: _dbSession.Transaction);
        }
        public async Task<bool> DeleteCategorieRelationsAsync(int idCategorie)
        {
            string query = $"DELETE FROM {RECETTE_CATEGORIE_TABLE} WHERE id_categorie = @idCategorie";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idCategorie }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        public async Task<bool> HasRecetteRelationsAsync(int idCategorie)
        {
            string query = $"SELECT COUNT(*) FROM {RECETTE_CATEGORIE_TABLE} WHERE id_categorie = @idCategorie";
            int count = await _dbSession.Connection.ExecuteScalarAsync<int>(query, new { idCategorie }, transaction: _dbSession.Transaction);
            return count > 0;
        }


        #endregion Fin Methods specific to CategorieRepository
    }
}
