using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes
{
    /// <summary>
    /// Implémentation du repository pour la gestion des recettes.
    /// Fournit les opérations CRUD de base ainsi que les fonctionnalités
    /// spécifiques liées aux relations entre les recettes et les catégories.
    /// </summary>
    public class RecetteRepository : IRecetteRepository
    {
        // Constantes représentant les noms des tables de la base de données
        private const string RECETTE_TABLE = "recettes";
        private const string RECETTE_CATEGORIE_TABLE = "categories_recettes";

        // Session de base de données partagée pour gérer les connexions et transactions
        private readonly IDBSession _dbSession;

        /// <summary>
        /// Constructeur du repository des recettes.
        /// </summary>
        /// <param name="dBSession">Session de base de données injectée par dépendance.</param>
        public RecetteRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        /// <summary>
        /// Récupère toutes les recettes, triées par nom.
        /// </summary>
        public async Task<IEnumerable<Recette>> GetAllAsync()
        {
            string query = $"SELECT * FROM {RECETTE_TABLE} ORDER BY nom ASC";
            return await _dbSession.Connection.QueryAsync<Recette>(query, transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Récupère une recette par son identifiant unique.
        /// </summary>
        public async Task<Recette> GetAsync(int id)
        {
            string query = $"SELECT * FROM {RECETTE_TABLE} WHERE id = @id";
            return await _dbSession.Connection.QuerySingleOrDefaultAsync<Recette>(query, new { id }, transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Crée une nouvelle recette dans la base de données.
        /// </summary>
        /// <param name="recette">Objet recette à insérer.</param>
        /// <returns>La recette insérée avec son identifiant généré.</returns>
        public async Task<Recette> CreateAsync(Recette recette)
        {
            string query = string.Empty;

            // Gestion de la syntaxe d'insertion selon le SGBD utilisé
            if (_dbSession.DatabaseProviderName == DatabaseProviderName.MariaDB ||
                _dbSession.DatabaseProviderName == DatabaseProviderName.MySQL)
            {
                query = $"INSERT INTO {RECETTE_TABLE} (nom, temps_preparation, temps_cuisson, difficulte, photo) " +
                        $"VALUES (@nom, @temps_preparation, @temps_cuisson, @difficulte, @photo); SELECT LAST_INSERT_ID();";
            }
            else if (_dbSession.DatabaseProviderName == DatabaseProviderName.PostgreSQL)
            {
                query = $"INSERT INTO {RECETTE_TABLE} (nom, temps_preparation, temps_cuisson, difficulte, photo) " +
                        $"VALUES (@nom, @temps_preparation, @temps_cuisson, @difficulte, @photo) RETURNING id;";
            }

            int lastId = await _dbSession.Connection.ExecuteScalarAsync<int>(query, recette, transaction: _dbSession.Transaction);
            recette.Id = lastId;
            return recette;
        }

        /// <summary>
        /// Met à jour une recette existante.
        /// </summary>
        public async Task<Recette> ModifyAsync(Recette recette)
        {
            string query = $"UPDATE {RECETTE_TABLE} " +
                           $"SET nom = @nom, temps_preparation = @temps_preparation, temps_cuisson = @temps_cuisson, difficulte = @difficulte, photo = @photo " +
                           $"WHERE id = @Id";

            int numLine = await _dbSession.Connection.ExecuteAsync(query, recette, transaction: _dbSession.Transaction);
            return numLine == 0 ? null : recette;
        }

        /// <summary>
        /// Supprime une recette de la base de données.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            string query = $"DELETE FROM {RECETTE_TABLE} WHERE id = @id";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { id }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        #region Méthodes spécifiques au repository Recette

        #region Relation Recette - Catégorie

        /// <summary>
        /// Récupère toutes les relations entre les recettes et les catégories.
        /// </summary>
        public async Task<IEnumerable<RecetteCategorieRelationship>> GetAllRecetteCategorieRelationshipAsync()
        {
            string query = $"SELECT * FROM {RECETTE_CATEGORIE_TABLE}";
            return await _dbSession.Connection.QueryAsync<RecetteCategorieRelationship>(query, transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Ajoute une nouvelle relation entre une recette et une catégorie.
        /// </summary>
        public async Task<bool> AddRecetteCategorieRelationshipAsync(int idCategorie, int idRecette)
        {
            string query = $"INSERT INTO {RECETTE_CATEGORIE_TABLE}(id_categorie, id_recette) VALUES(@idCategorie, @idRecette)";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idCategorie, idRecette }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        /// <summary>
        /// Supprime une relation spécifique entre une recette et une catégorie.
        /// </summary>
        public async Task<bool> RemoveRecetteCategorieRelationshipAsync(int idCategorie, int idRecette)
        {
            string query = $"DELETE FROM {RECETTE_CATEGORIE_TABLE} WHERE id_categorie = @idCategorie AND id_recette = @idRecette";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idCategorie, idRecette }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        /// <summary>
        /// Récupère toutes les recettes associées à une catégorie donnée.
        /// </summary>
        public async Task<IEnumerable<Recette>> GetRecettesByIdCategorieAsync(int idCategorie)
        {
            string query = $"SELECT r.* FROM {RECETTE_TABLE} r " +
                           $"JOIN {RECETTE_CATEGORIE_TABLE} rc ON r.id = rc.id_recette " +
                           $"WHERE rc.id_categorie = @idCategorie ORDER BY r.nom ASC, rc.id_categorie ASC";

            return await _dbSession.Connection.QueryAsync<Recette>(query, new { idCategorie }, transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Supprime toutes les relations entre une recette et ses catégories.
        /// </summary>
        public async Task<bool> DeleteRecetteRelationsAsync(int idRecette)
        {
            string query = $"DELETE FROM {RECETTE_CATEGORIE_TABLE} WHERE id_recette = @idRecette";
            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idRecette }, transaction: _dbSession.Transaction);
            return numLine != 0;
        }

        #endregion Fin Relation Recette - Catégorie

        #endregion Fin méthodes spécifiques
    }
}
