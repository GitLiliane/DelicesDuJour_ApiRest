using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes
{
    public class RecetteRepository : IRecetteRepository
    {
        const string RECETTE_TABLE = "recettes";
        readonly IDBSession _dbSession;

        public RecetteRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        public async Task<IEnumerable<Recette>> GetAllAsync()
        {
            string query = $"SELECT * FROM {RECETTE_TABLE}";
            return await _dbSession.Connection.QueryAsync<Recette>(query);
        }

        public async Task<Recette> GetAsync(int id)
        {

            string query = $"SELECT * FROM {RECETTE_TABLE} WHERE id = @id";
            return await _dbSession.Connection.QuerySingleOrDefaultAsync<Recette>(query, new { id });
        }

        public async Task<Recette> CreateAsync(Recette recette)
        {
            string query = string.Empty;

            if (_dbSession.DatabaseProviderName == DatabaseProviderName.MariaDB || _dbSession.DatabaseProviderName == DatabaseProviderName.MySQL)
                query = $"INSERT INTO {RECETTE_TABLE} (nom, temps_preparation, temps_cuisson, difficulte, photo) VALUES (@nom, @temps_preparation, @temps_cuisson, @difficulte, @photo); Select LAST_INSERT_ID()";

            else if (_dbSession.DatabaseProviderName == DatabaseProviderName.PostgreSQL)
                query = $"INSERT INTO {RECETTE_TABLE} (nom, temps_preparation, temps_cuisson, difficulte, photo) VALUES (@nom, @temps_preparation, @temps_cuisson, @difficulte, @photo) RETURNING id";

            int lastId = await _dbSession.Connection.ExecuteScalarAsync<int>(query, recette);
            recette.Id = lastId;

            return recette;
        }

        public async Task<Recette> ModifyAsync(Recette recette)
        {
            string query = $"UPDATE {RECETTE_TABLE} SET nom = @nom, temps_preparation = @temps_preparation, temps_cuisson = @temps_cuisson, difficulte = @difficulte WHERE id = @Id";

            int numLine = await _dbSession.Connection.ExecuteAsync(query, recette);
            return numLine == 0 ? null : recette;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            string query = $"DELETE FROM {RECETTE_TABLE} WHERE id = @id";

            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { id });
            return numLine != 0;
        }


    }
}
