using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Ingredients
{
    /// <summary>
    /// Repository permettant la gestion des entités <see cref="Ingredient"/> dans la base de données.
    /// Utilise Dapper pour exécuter les requêtes SQL et interagit via <see cref="IDBSession"/> pour la gestion des connexions et transactions.
    /// </summary>
    public class IngredientRepository : IIngredientRepository
    {
        // Nom de la table principale des ingrédients
        private const string INGREDIENT_TABLE = "ingredients";

        // Table de liaison entre les recettes et les ingrédients (relation N-N)
        private const string RECETTE_INGREDIENT_TABLE = "ingredients_recettes";

        // Session de base de données (permet la gestion des transactions et de la connexion)
        private readonly IDBSession _dbSession;

        /// <summary>
        /// Initialise une nouvelle instance du repository <see cref="IngredientRepository"/>.
        /// </summary>
        /// <param name="dBSession">Session de base de données injectée via dépendance.</param>
        public IngredientRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        /// <summary>
        /// Récupère l’ensemble des ingrédients de la base de données.
        /// </summary>
        /// <returns>Une collection d’objets <see cref="Ingredient"/> triés par nom.</returns>
        public async Task<IEnumerable<Ingredient>> GetAllAsync()
        {
            string query = $"SELECT * FROM {INGREDIENT_TABLE} ORDER BY nom ASC";
            // Exécution de la requête SQL et retour des résultats
            return await _dbSession.Connection.QueryAsync<Ingredient>(query, transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Récupère un ingrédient à partir de son identifiant unique.
        /// </summary>
        /// <param name="id">Identifiant de l’ingrédient à récupérer.</param>
        /// <returns>
        /// L’objet <see cref="Ingredient"/> correspondant ou <c>null</c> s’il n’existe pas.
        /// </returns>
        public async Task<Ingredient> GetAsync(int id)
        {
            string query = $"SELECT * FROM {INGREDIENT_TABLE} WHERE id = @id";
            return await _dbSession.Connection.QuerySingleOrDefaultAsync<Ingredient>(query, new { id }, transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Crée un nouvel ingrédient dans la base de données.
        /// </summary>
        /// <param name="ingredient">Objet <see cref="Ingredient"/> à insérer.</param>
        /// <returns>L’objet <see cref="Ingredient"/> créé, avec son identifiant mis à jour.</returns>
        public async Task<Ingredient> CreateAsync(Ingredient ingredient)
        {
            string query = string.Empty;

            // Choix de la requête en fonction du fournisseur de base de données
            if (_dbSession.DatabaseProviderName == DatabaseProviderName.MariaDB || _dbSession.DatabaseProviderName == DatabaseProviderName.MySQL)
                query = $"INSERT INTO {INGREDIENT_TABLE}(nom) VALUES(@nom); SELECT LAST_INSERT_ID()";
            else if (_dbSession.DatabaseProviderName == DatabaseProviderName.PostgreSQL)
                query = $"INSERT INTO {INGREDIENT_TABLE}(nom) VALUES(@nom) RETURNING id";

            // Exécution et récupération de l'identifiant du nouvel ingrédient
            int lastId = await _dbSession.Connection.ExecuteScalarAsync<int>(query, ingredient, transaction: _dbSession.Transaction);
            ingredient.id = lastId;

            return ingredient;
        }

        /// <summary>
        /// Met à jour un ingrédient existant.
        /// </summary>
        /// <param name="ingredient">Objet <see cref="Ingredient"/> contenant les nouvelles données.</param>
        /// <returns>
        /// L’objet <see cref="Ingredient"/> modifié ou <c>null</c> si aucune ligne n’a été affectée.
        /// </returns>
        public async Task<Ingredient> ModifyAsync(Ingredient ingredient)
        {
            string query = $"UPDATE {INGREDIENT_TABLE} SET nom = @nom WHERE id = @Id";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, ingredient, transaction: _dbSession.Transaction);

            // Si aucune ligne n’est modifiée, l’ingrédient n’existait probablement pas
            return numLine == 0 ? null : ingredient;
        }

        /// <summary>
        /// Supprime un ingrédient de la base de données.
        /// </summary>
        /// <param name="id">Identifiant de l’ingrédient à supprimer.</param>
        /// <returns><c>true</c> si la suppression a réussi, sinon <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            string query = $"DELETE FROM {INGREDIENT_TABLE} WHERE id = @id";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { id }, transaction: _dbSession.Transaction);

            return numLine != 0;
        }

        /// <summary>
        /// Récupère tous les ingrédients associés à une recette donnée.
        /// </summary>
        /// <param name="idRecette">Identifiant unique de la recette.</param>
        /// <returns>
        /// Une collection d’objets <see cref="Ingredient"/> liés à la recette spécifiée.
        /// </returns>
        public async Task<IEnumerable<Ingredient>> GetIngredientsByIdRecetteAsync(int idRecette)
        {
            string query = $@"
                SELECT i.* 
                FROM {INGREDIENT_TABLE} i
                JOIN {RECETTE_INGREDIENT_TABLE} ri ON i.id = ri.id_ingredient
                WHERE ri.id_recette = @idRecette
                ORDER BY i.nom ASC";

            return await _dbSession.Connection.QueryAsync<Ingredient>(query, new { idRecette }, transaction: _dbSession.Transaction);
        }
    }
}
