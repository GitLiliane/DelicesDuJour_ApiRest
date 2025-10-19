using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using Mysqlx.Crud;
using MySqlX.XDevAPI.Common;
using System.Diagnostics.Eventing.Reader;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes
{
    public class EtapeRepository : IEtapeRepository
    {
        const string ETAPE_TABLE = "etapes";
        readonly IDBSession _dbSession;

        public EtapeRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        public async Task<IEnumerable<Etape>> GetAllAsync()
        {           
            string query = $"SELECT * FROM {ETAPE_TABLE} ORDER BY id_recette ASC, numero ASC;";

            var etapes = await _dbSession.Connection.QueryAsync<Etape>(query, transaction: _dbSession.Transaction);         

            return etapes;
        }

        public async Task<Etape> GetAsync((int, int) key)
        {
            string query = $"SELECT * FROM {ETAPE_TABLE} WHERE id_recette = @id_recette AND numero = numero";

            var result = await _dbSession.Connection.QuerySingleOrDefaultAsync<Etape>(query, new { id_recette = key.Item1, numero = key.Item2 }, transaction: _dbSession.Transaction);
            return null;
        }
        public async Task<Etape> CreateAsync(Etape etape)
        {
            string query = $"INSERT INTO {ETAPE_TABLE} (id_recette, numero, titre, texte) VALUES (@id_recette, @numero, @titre, @texte) RETURNING id_recette";

            int id_recette = await _dbSession.Connection.ExecuteScalarAsync<int>(query, etape, transaction: _dbSession.Transaction);
            etape.id_recette = id_recette;            
            return etape;
        }

        public async Task<Etape> ModifyAsync(Etape etape)
        {
            string query = $"UPDATE {ETAPE_TABLE} SET titre = @titre, texte = @texte WHERE id_recette = @id_recette AND numero = @numero";

            int numLine = await _dbSession.Connection.ExecuteAsync(query, etape, transaction: _dbSession.Transaction);

            return numLine == 0 ? null : etape;
        }

        public async Task<bool> DeleteAsync((int, int) key)
        {
            string query = $"DELETE FROM {ETAPE_TABLE} WHERE id_recette = @id_recette AND numero = @numero";

            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { id_recette = key.Item1, numero = key.Item2 }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        #region Methods specific Recette Etape

        public async Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int id)
        {
            string query = $"SELECT * FROM {ETAPE_TABLE} WHERE id_recette = @id_recette ORDER BY numero ASC";

            var result = await _dbSession.Connection.QueryAsync<Etape>(query, new { id_recette = id }, transaction: _dbSession.Transaction);
            return result;
        }

        public async Task<bool> DeleteEtapesRelationByIdRecetteAsync(int id_recette)
        {
            string query = $"DELETE FROM {ETAPE_TABLE} WHERE id_recette = @id_recette";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { id_recette }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        #endregion Methods specific Recette Etape
    }
}
