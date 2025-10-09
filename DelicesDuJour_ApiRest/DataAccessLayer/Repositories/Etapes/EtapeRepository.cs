using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
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
            // Étape 1 : Récupérer les données brutes de la base de données dans un tuple
            string query = $"SELECT * FROM {ETAPE_TABLE} ORDER BY id_recette ASC;";

            var etapesData = await _dbSession.Connection.QueryAsync<(int id_recette, int numero, string titre, string texte)>(query, transaction: _dbSession.Transaction);

            // Étape 2 : Mapper manuellement les données du tuple vers votre classe Etape
            var etapes = etapesData.Select(e => new Etape
            {
                // La propriété 'Key' est une instance de TupleClass
                // On la crée en passant le tuple récupéré de la base de données
                //Key = new TupleClass<int, int>((e.id_recette, e.numero)),
                titre = e.titre,
                texte = e.texte
            });

            return etapes;
        }


        public async Task<Etape> GetAsync(int id)
        {
            string query = $"SELECT * FROM {ETAPE_TABLE} WHERE id_recette = @id_recette";

            var result = await _dbSession.Connection.QueryAsync<Etape>(query, transaction: _dbSession.Transaction);
            return null;
        }
        public async Task<Etape> CreateAsync(Etape etape)
        {
            string query = $"INSERT INTO {ETAPE_TABLE} (id_recette, numero, titre, texte) VALUES (@id_recette, @numero, @titre, @texte) RETURNING id_recette";          

            int id_recette = await _dbSession.Connection.ExecuteScalarAsync<int>(query, etape, transaction: _dbSession.Transaction);
            etape.id_recette = id_recette;
            // Retournez simplement l'entité car sa clé est déjà renseignée
            return etape;
        }

        public async Task<Etape> ModifyAsync(Etape etape)
        {
            string query = $"UPDATE {ETAPE_TABLE} SET titre = @titre, texte = @texte WHERE id_recette = @id_recette AND numero = @numero";

            var parameters = new
            {
                //id_recette = etape.Key.t,
                //numero = etape.Key.v,
                etape.titre,
                etape.texte
            };

            int rowsAffected = await _dbSession.Connection.ExecuteAsync(query, parameters, transaction: _dbSession.Transaction);

            if (rowsAffected > 0)
            {
                return etape; // Retourne l'objet Etape mis à jour
            }
            else
            {
                return null; // Retourne null si aucune ligne n'a été mise à jour
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            string query = $"DELETE FROM {ETAPE_TABLE} WHERE id_recette = @id_recette AND numero = @numero";

            var parameters = new
            {
                //id_recette = key.t,
                //numero = key.v                
            };
            int numLine = await _dbSession.Connection.ExecuteAsync(query, parameters, transaction: _dbSession.Transaction);
            return numLine != 0;            
        }

        public async Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int id)
        {
            string query = $"SELECT * FROM {ETAPE_TABLE} WHERE id_recette = @id_recette";

            var result = await _dbSession.Connection.QueryAsync<Etape>(query, new { id_recette = id }, transaction: _dbSession.Transaction);
            return result;
        }

    }
}
