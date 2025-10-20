using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.DTOIn;
using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngred
{
    /// <summary>
    /// Repository pour la gestion des relations entre les recettes et leurs ingrédients,
    /// incluant la quantité de chaque ingrédient par recette.
    /// </summary>
    public class QuantiteIngredRepository : IQuantiteIngredRepository
    {
        // Noms des tables utilisées dans les requêtes SQL
        const string RECETTE_TABLE = "recettes";
        const string INGREDIENT_TABLE = "ingredients";
        const string RECETTE_INGREDIENT_TABLE = "ingredients_recettes";

        // Session de base de données active (connexion + transaction éventuelle)
        readonly IDBSession _dbSession;

        /// <summary>
        /// Initialise une nouvelle instance du repository de gestion des relations recette-ingrédient.
        /// </summary>
        /// <param name="dBSession">Session de base de données injectée pour exécuter les requêtes.</param>
        public QuantiteIngredRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        #region Relation Recette Ingredient   

        /// <summary>
        /// Récupère toutes les relations entre recettes et ingrédients existantes dans la base.
        /// </summary>
        /// <returns>Une liste d’objets <see cref="QuantiteIngredients"/>.</returns>
        public async Task<IEnumerable<QuantiteIngredients>> GetAllAsync()
        {
            string query = $"SELECT * FROM {RECETTE_INGREDIENT_TABLE} ORDER BY id_recette ASC";
            return await _dbSession.Connection.QueryAsync<QuantiteIngredients>(query, transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Récupère une relation spécifique entre une recette et un ingrédient, identifiée par une clé composite.
        /// </summary>
        /// <param name="key">Tuple (id_ingredient, id_recette) identifiant la relation.</param>
        /// <returns>L’objet <see cref="QuantiteIngredients"/> correspondant, ou <c>null</c> s’il n’existe pas.</returns>
        public async Task<QuantiteIngredients> GetAsync((int, int) key)
        {
            string query = $"SELECT * FROM {RECETTE_INGREDIENT_TABLE} WHERE id_ingredient = @id_ingredient AND id_recette = @id_recette";
            var reponse = await _dbSession.Connection.QuerySingleOrDefaultAsync<QuantiteIngredients>(
                query,
                new { id_ingredient = key.Item1, id_recette = key.Item2 },
                transaction: _dbSession.Transaction);
            return reponse;
        }

        /// <summary>
        /// Crée une nouvelle relation entre une recette et un ingrédient avec une quantité donnée.
        /// </summary>
        /// <param name="CreateRelationRI">Objet <see cref="QuantiteIngredients"/> à insérer.</param>
        /// <returns>La relation créée si l’insertion a réussi, sinon <c>null</c>.</returns>
        public async Task<QuantiteIngredients> CreateAsync(QuantiteIngredients CreateRelationRI)
        {
            string query = $"INSERT INTO {RECETTE_INGREDIENT_TABLE}(id_ingredient, id_recette, quantite) VALUES(@idIngredient, @idRecette, @quantite)";
            int numLine = await _dbSession.Connection.ExecuteAsync(
                query,
                new
                {
                    idIngredient = CreateRelationRI.id_ingredient,
                    idRecette = CreateRelationRI.id_recette,
                    quantite = CreateRelationRI.quantite
                },
                transaction: _dbSession.Transaction);

            return numLine == 0 ? null : CreateRelationRI;
        }

        /// <summary>
        /// Met à jour la quantité d’un ingrédient pour une recette donnée.
        /// </summary>
        /// <param name="updateQuantiteIngredient">Objet contenant la relation mise à jour.</param>
        /// <returns>L’objet mis à jour, ou <c>null</c> si aucune modification n’a été effectuée.</returns>
        public async Task<QuantiteIngredients> ModifyAsync(QuantiteIngredients updateQuantiteIngredient)
        {
            string query = $"UPDATE {RECETTE_INGREDIENT_TABLE} SET quantite = @quantite WHERE id_ingredient = @id_ingredient AND id_recette = @id_recette";
            int numLine = await _dbSession.Connection.ExecuteAsync(
                query,
                updateQuantiteIngredient,
                transaction: _dbSession.Transaction);

            return numLine == 0 ? null : updateQuantiteIngredient;
        }

        /// <summary>
        /// Supprime une relation spécifique entre une recette et un ingrédient.
        /// </summary>
        /// <param name="key">Tuple (id_ingredient, id_recette) identifiant la relation à supprimer.</param>
        /// <returns><c>true</c> si la suppression a réussi, sinon <c>false</c>.</returns>
        public async Task<bool> DeleteAsync((int, int) key)
        {
            string query = $"DELETE FROM {RECETTE_INGREDIENT_TABLE} WHERE id_ingredient = @id_ingredient AND id_recette = @id_recette";
            int numLine = await _dbSession.Connection.ExecuteAsync(
                query,
                new { id_ingredient = key.Item1, id_recette = key.Item2 },
                transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        /// <summary>
        /// Récupère toutes les recettes associées à un ingrédient donné.
        /// </summary>
        /// <param name="idIngredient">Identifiant unique de l’ingrédient.</param>
        /// <returns>Une collection d’objets <see cref="Recette"/> associée à cet ingrédient.</returns>
        public async Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient)
        {
            string query = $"SELECT r.* FROM {RECETTE_TABLE} r JOIN {RECETTE_INGREDIENT_TABLE} ri ON r.id = ri.id_recette WHERE ri.id_ingredient = @idIngredient ORDER BY ri.id_recette, ri.id_ingredient";
            return await _dbSession.Connection.QueryAsync<Recette>(
                query,
                new { idIngredient },
                transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Récupère tous les ingrédients (avec quantités) liés à une recette donnée.
        /// </summary>
        /// <param name="idRecette">Identifiant unique de la recette.</param>
        /// <returns>Une collection d’objets <see cref="QuantiteIngredients"/> pour cette recette.</returns>
        public async Task<IEnumerable<QuantiteIngredients>> GetIngredientsByIdRecetteAsync(int idRecette)
        {
            string query = $"SELECT * FROM {INGREDIENT_TABLE} i JOIN {RECETTE_INGREDIENT_TABLE} ri ON i.id = ri.id_ingredient WHERE ri.id_recette = @idRecette ORDER BY ri.id_ingredient";
            return await _dbSession.Connection.QueryAsync<QuantiteIngredients>(
                query,
                new { idRecette },
                transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Supprime toutes les relations ingrédient-recette associées à une recette spécifique.
        /// </summary>
        /// <param name="idRecette">Identifiant de la recette dont les relations doivent être supprimées.</param>
        /// <returns><c>true</c> si au moins une relation a été supprimée, sinon <c>false</c>.</returns>
        public async Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette)
        {
            string query = $"DELETE FROM {RECETTE_INGREDIENT_TABLE} WHERE id_recette = @idRecette";
            int numLine = await _dbSession.Connection.ExecuteAsync(
                query,
                new { idRecette },
                transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        /// <summary>
        /// Supprime toutes les relations ingrédient-recette associées à un ingrédient spécifique.
        /// </summary>
        /// <param name="idIngredient">Identifiant de l’ingrédient dont les relations doivent être supprimées.</param>
        /// <returns><c>true</c> si au moins une relation a été supprimée, sinon <c>false</c>.</returns>
        public async Task<bool> DeleteIngredientRelationsRecetteAsync(int idIngredient)
        {
            string query = $"DELETE FROM {RECETTE_INGREDIENT_TABLE} WHERE id_ingredient = @idIngredient";
            int numLine = await _dbSession.Connection.ExecuteAsync(
                query,
                new { idIngredient },
                transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        #endregion Fin relation Recette Ingredient
    }
}
