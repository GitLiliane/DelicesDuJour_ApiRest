using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;
using MySqlX.XDevAPI.Common;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes
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

            var etapesData = await _dbSession.Connection.QueryAsync<(int id_recette, int numero, string titre, string texte)>(query);

            // Étape 2 : Mapper manuellement les données du tuple vers votre classe Etape
            var etapes = etapesData.Select(e => new Etape
            {
                // La propriété 'Key' est une instance de TupleClass
                // On la crée en passant le tuple récupéré de la base de données
                Key = new TupleClass<int, int>((e.id_recette, e.numero)),
                titre = e.titre,
                texte = e.texte
            });

            return etapes;
        }


        public async Task<Etape> GetAsync(TupleClass<int, int> key)
        {
            string query = "SELECT * FROM etapes WHERE id_recette = @id_recette AND numero = @numero";

            // Utilisez QueryFirstOrDefaultAsync pour éviter l'exception
            var result = await _dbSession.Connection.QueryFirstOrDefaultAsync<(int id_recette, int numero, string titre, string texte)>(
                query,
                new { id_recette = key.t, numero = key.v }
            );

            // Vérifiez si un résultat a été trouvé
            if (result.id_recette == 0 && result.numero == 0)
            {
                return null; // Retourne null si aucun résultat n'a été trouvé
            }

            // Mappez les données du tuple vers votre objet Etape
            var etape = new Etape
            {
                Key = new TupleClass<int, int>((result.id_recette, result.numero)),
                titre = result.titre,
                texte = result.texte
            };

            return etape;
        }
        public async Task<Etape> CreateAsync(Etape etape)
        {
            string query = $"INSERT INTO {ETAPE_TABLE} (id_recette, numero, titre, texte) VALUES (@id_recette, @numero, @titre, @texte)";

            // Créez un objet de paramètres pour Dapper
            var parameters = new
            {
                id_recette = etape.Key.t,
                numero = etape.Key.v,
                titre = etape.titre,
                texte = etape.texte
            };

            await _dbSession.Connection.ExecuteAsync(query, parameters);

            // Retournez simplement l'entité car sa clé est déjà renseignée
            return etape;
        }

        public async Task<Etape> ModifyAsync(Etape etape)
        {
            string query = $"UPDATE {ETAPE_TABLE} SET titre = @titre, texte = @texte WHERE id_recette = @id_recette AND numero = @numero";

            var parameters = new
            {
                id_recette = etape.Key.t,
                numero = etape.Key.v,
                titre = etape.titre,
                texte = etape.texte
            };

            int rowsAffected = await _dbSession.Connection.ExecuteAsync(query, parameters);

            if (rowsAffected > 0)
            {
                return etape; // Retourne l'objet Etape mis à jour
            }
            else
            {
                return null; // Retourne null si aucune ligne n'a été mise à jour
            }
        }

        public async Task<bool> DeleteAsync(TupleClass<int, int> key)
        {
            string query = $"DELETE FROM {ETAPE_TABLE} WHERE id_recette = @id_recette AND numero = @numero";

            var parameters = new
            {
                id_recette = key.t,
                numero = key.v                
            };
            int numLine = await _dbSession.Connection.ExecuteAsync(query, parameters);
            return numLine != 0;            
        }

    }
}
