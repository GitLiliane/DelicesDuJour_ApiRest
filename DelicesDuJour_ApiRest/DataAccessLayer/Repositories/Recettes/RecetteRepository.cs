using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes
{
    public class RecetteRepository : IRecetteRepository
    {
        const string RECETTE_TABLE = "recettes";
        const string RECETTE_CATEGORIE_TABLE = "categories_recettes";

        readonly IDBSession _dbSession;

        public RecetteRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        public async Task<IEnumerable<Recette>> GetAllAsync()
        {
            string query = $"SELECT * FROM {RECETTE_TABLE} ORDER BY nom ASC";
            return await _dbSession.Connection.QueryAsync<Recette>(query, transaction: _dbSession.Transaction);
        }

        public async Task<Recette> GetAsync(int id)
        {
            string query = $"SELECT * FROM {RECETTE_TABLE} WHERE id = @id";
            return await _dbSession.Connection.QuerySingleOrDefaultAsync<Recette>(query, new { id }, transaction: _dbSession.Transaction);
        }
             
        public async Task<Recette> CreateAsync(Recette recette)
        {
            string query = string.Empty;

            if (_dbSession.DatabaseProviderName == DatabaseProviderName.MariaDB ||
                _dbSession.DatabaseProviderName == DatabaseProviderName.MySQL)
                query = $"INSERT INTO {RECETTE_TABLE} (nom, temps_preparation, temps_cuisson, difficulte, photo) " +
                        $"VALUES (@nom, @temps_preparation, @temps_cuisson, @difficulte, @photo); SELECT LAST_INSERT_ID();";

            else if (_dbSession.DatabaseProviderName == DatabaseProviderName.PostgreSQL)
                query = $"INSERT INTO {RECETTE_TABLE} (nom, temps_preparation, temps_cuisson, difficulte, photo) " +
                        $"VALUES (@nom, @temps_preparation, @temps_cuisson, @difficulte, @photo) RETURNING id;";

            int lastId = await _dbSession.Connection.ExecuteScalarAsync<int>(
                query, recette, transaction: _dbSession.Transaction);

            recette.Id = lastId;
            return recette;
        }


        public async Task<Recette> ModifyAsync(Recette recette)
        {
            string query = $"UPDATE {RECETTE_TABLE} SET nom = @nom, temps_preparation = @temps_preparation, temps_cuisson = @temps_cuisson, difficulte = @difficulte, photo = @photo WHERE id = @Id";

            int numLine = await _dbSession.Connection.ExecuteAsync(query, recette, transaction: _dbSession.Transaction);
            return numLine == 0 ? null : recette;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            string query = $"DELETE FROM {RECETTE_TABLE} WHERE id = @id";

            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { id }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        #region Methods specific to RecetteRepository

        #region Relation Recette - Catégorie

        public async Task<IEnumerable<RecetteCategorieRelationship>> GetAllRecetteCategorieRelationshipAsync()
        {
            string query = $"SELECT * FROM {RECETTE_CATEGORIE_TABLE}";
            return await _dbSession.Connection.QueryAsync<RecetteCategorieRelationship>(query, transaction: _dbSession.Transaction);
        }

        public async Task<bool> AddRecetteCategorieRelationshipAsync(int idCategorie, int idRecette)
        {
            string query = $"INSERT INTO {RECETTE_CATEGORIE_TABLE}(id_categorie, id_recette) VALUES(@idCategorie, @idRecette)";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idCategorie, idRecette }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        public async Task<bool> RemoveRecetteCategorieRelationshipAsync(int idCategorie, int idRecette)
        {
            string query = $"DELETE FROM {RECETTE_CATEGORIE_TABLE} WHERE id_categorie = @idCategorie AND id_recette = @idRecette";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idCategorie, idRecette }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        public async Task<IEnumerable<Recette>> GetRecettesByIdCategorieAsync(int idCategorie)
        {
            string query = $"SELECT r.* FROM {RECETTE_TABLE} r JOIN {RECETTE_CATEGORIE_TABLE} rc ON r.id = rc.id_recette WHERE rc.id_categorie = @idCategorie ORDER BY r.nom ASC, rc.id_categorie ASC";
            return await _dbSession.Connection.QueryAsync<Recette>(query, new { idCategorie }, transaction: _dbSession.Transaction);
        }

        public async Task<bool> DeleteRecetteRelationsAsync(int idRecette)
        {
            string query = $"DELETE FROM {RECETTE_CATEGORIE_TABLE} WHERE id_recette = @idRecette";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idRecette }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        #endregion Fin Relation Recette - Catégorie 

        #endregion Methods specific to RecettesRepository
    }
}
