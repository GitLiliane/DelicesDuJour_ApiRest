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
            string query = $"SELECT * FROM {RECETTE_TABLE}";
            return await _dbSession.Connection.QueryAsync<Recette>(query, transaction: _dbSession.Transaction);
        }

        public async Task<Recette> GetAsync(int id)
        {
            string query = $"SELECT * FROM {RECETTE_TABLE} WHERE id = @id";
            return await _dbSession.Connection.QuerySingleOrDefaultAsync<Recette>(query, new { id }, transaction: _dbSession.Transaction);

            //string query = $"SELECT r.*, ir.*, i.*, e.id_recette as recetteId, e.numero, e.titre, e.texte FROM {RECETTE_TABLE} r JOIN ingredients_recettes ir ON r.id = ir.id_recette JOIN ingredients i ON ir.id_ingredient = i.id JOIN etapes e ON e.id_recette = r.id WHERE r.id = @identifiant";
            //var recettes = await _dbSession.Connection.QueryAsync<Recette, Ingredient, Etape, Recette>(query, (recette, ingredient, etape) =>
            //{
            //    recette.ingredients.Add(ingredient);
            //    recette.etapes.Add(etape);
            //    return recette;
            //}
            //, new { identifiant = id },
            //splitOn: "id_ingredient, recetteId", transaction: _dbSession.Transaction);

            //Recette recette = recettes.GroupBy(r => r.Id).Select(g =>
            //{
            //    Recette groupedRecette = g.First();
            //    groupedRecette.ingredients = g.Select(r => r.ingredients.Single()).ToList();
            //    groupedRecette.etapes = g.SelectMany(r => r.etapes).Distinct().ToList();
            //    return groupedRecette;
            //}).First();

            //recette.ingredients = recette.ingredients.GroupBy(i => i.id).Select(g => g.First()).ToList();
            //recette.etapes = recette.etapes.GroupBy(i =>
            //{
            //    var id_recette = i.Key.t;
            //    return id_recette;
            //}).Select(g => g.First()).ToList();

            //return recette;
        }

        //    public async Task<Recette> GetAsync(int id)
        //    {
        //        string query = $@"
        //    SELECT 
        //        r.*
        //        i.*
        //        ir.*
        //        e.id_recette AS IdRecette, e.numero, e.titre, e.texte
        //    FROM {RECETTE_TABLE} r
        //    LEFT JOIN ingredients_recettes ir ON r.id = ir.id_recette
        //    LEFT JOIN ingredients i ON ir.id_ingredient = i.id
        //    LEFT JOIN etapes e ON e.id_recette = r.id
        //    WHERE r.id = @identifiant;
        //";

        //        var recetteDict = new Dictionary<int, Recette>();

        //        var data = await _dbSession.Connection.QueryAsync<Recette, Ingredient, Etape, Recette>(
        //            query,
        //            (recette, ingredient, etape) =>
        //            {
        //                // Regrouper les lignes pour éviter la création multiple de la même recette
        //                if (!recetteDict.TryGetValue(recette.Id, out var currentRecette))
        //                {
        //                    currentRecette = recette;
        //                    currentRecette.ingredients = new List<Ingredient>();
        //                    currentRecette.etapes = new List<Etape>();
        //                    recetteDict.Add(currentRecette.Id, currentRecette);
        //                }

        //                // Ajouter les ingrédients sans doublons
        //                if (ingredient != null && !currentRecette.ingredients.Any(i => i.id == ingredient.id))
        //                    currentRecette.ingredients.Add(ingredient);

        //                // Ajouter les étapes sans doublons
        //                if (etape != null && !currentRecette.etapes.Any(e => e.Key == etape.Key))
        //                    currentRecette.etapes.Add(etape);

        //                return currentRecette;
        //            },
        //            new { identifiant = id },
        //            splitOn: "IdIngredient,IdRecette",
        //            transaction: _dbSession.Transaction
        //        );

        //        return recetteDict.Values.FirstOrDefault();
        //    }


        public async Task<Recette> CreateAsync(Recette recette)
        {
            string query = string.Empty;

            if (_dbSession.DatabaseProviderName == DatabaseProviderName.MariaDB || _dbSession.DatabaseProviderName == DatabaseProviderName.MySQL)
                query = $"INSERT INTO {RECETTE_TABLE} (nom, temps_preparation, temps_cuisson, difficulte) VALUES (@nom, @temps_preparation, @temps_cuisson, @difficulte); Select LAST_INSERT_ID()";

            else if (_dbSession.DatabaseProviderName == DatabaseProviderName.PostgreSQL)
                query = $"INSERT INTO {RECETTE_TABLE} (nom, temps_preparation, temps_cuisson, difficulte) VALUES (@nom, @temps_preparation, @temps_cuisson, @difficulte) RETURNING id";

            int lastId = await _dbSession.Connection.ExecuteScalarAsync<int>(query, recette, transaction: _dbSession.Transaction);
            recette.Id = lastId;

            return recette;
        }

        public async Task<Recette> ModifyAsync(Recette recette)
        {
            string query = $"UPDATE {RECETTE_TABLE} SET nom = @nom, temps_preparation = @temps_preparation, temps_cuisson = @temps_cuisson, difficulte = @difficulte WHERE id = @Id";

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
            string query = $"SELECT r.* FROM {RECETTE_TABLE} r JOIN {RECETTE_CATEGORIE_TABLE} rc ON r.id = rc.id_recette WHERE rc.id_categorie = @idCategorie";
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
