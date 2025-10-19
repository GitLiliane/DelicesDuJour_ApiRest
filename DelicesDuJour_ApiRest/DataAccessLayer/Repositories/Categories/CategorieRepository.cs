using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories
{
    /// <summary>
    /// Repository responsable de la gestion des entités <see cref="Categorie"/> en base de données.
    /// </summary>
    public class CategorieRepository : ICategorieRepository
    {
        /// <summary>
        /// Nom de la table principale des catégories.
        /// </summary>
        private const string CATEGORIE_TABLE = "categories";

        /// <summary>
        /// Nom de la table de liaison entre recettes et catégories.
        /// </summary>
        private const string RECETTE_CATEGORIE_TABLE = "categories_recettes";

        /// <summary>
        /// Session de base de données (connexion + transaction en cours).
        /// </summary>
        private readonly IDBSession _dbSession;

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="CategorieRepository"/>.
        /// </summary>
        /// <param name="dBSession">Session de base de données injectée.</param>
        public CategorieRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        /// <summary>
        /// Récupère toutes les catégories de la base de données.
        /// </summary>
        /// <returns>Une collection d’objets <see cref="Categorie"/>.</returns>
        public async Task<IEnumerable<Categorie>> GetAllAsync()
        {
            // Construction de la requête SQL
            string query = $"SELECT * FROM {CATEGORIE_TABLE}";

            // Exécution de la requête avec Dapper
            return await _dbSession.Connection.QueryAsync<Categorie>(query, transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Récupère une catégorie spécifique à partir de son identifiant.
        /// </summary>
        /// <param name="id">Identifiant unique de la catégorie.</param>
        /// <returns>L’objet <see cref="Categorie"/> correspondant, ou null s’il n’existe pas.</returns>
        public async Task<Categorie> GetAsync(int id)
        {
            string query = $"SELECT * FROM {CATEGORIE_TABLE} WHERE id = @id";

            // QuerySingleOrDefaultAsync : renvoie un seul enregistrement ou null
            return await _dbSession.Connection.QuerySingleOrDefaultAsync<Categorie>(query, new { id }, transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Crée une nouvelle catégorie dans la base de données.
        /// </summary>
        /// <param name="categorie">Catégorie à insérer.</param>
        /// <returns>La catégorie créée avec son identifiant généré.</returns>
        public async Task<Categorie> CreateAsync(Categorie categorie)
        {
            string query = string.Empty;

            // Choix de la requête selon le moteur de base de données
            if (_dbSession.DatabaseProviderName == DatabaseProviderName.MariaDB || _dbSession.DatabaseProviderName == DatabaseProviderName.MySQL)
                query = $"INSERT INTO {CATEGORIE_TABLE}(nom) VALUES (@nom); SELECT LAST_INSERT_ID()";
            else if (_dbSession.DatabaseProviderName == DatabaseProviderName.PostgreSQL)
                query = $"INSERT INTO {CATEGORIE_TABLE}(nom) VALUES (@nom) RETURNING id";

            // Exécution et récupération du dernier ID inséré
            int lastId = await _dbSession.Connection.ExecuteScalarAsync<int>(query, categorie, transaction: _dbSession.Transaction);

            // Affectation de l’ID généré à l’objet
            categorie.id = lastId;

            return categorie;
        }

        /// <summary>
        /// Met à jour une catégorie existante.
        /// </summary>
        /// <param name="categorie">Catégorie avec les nouvelles données.</param>
        /// <returns>La catégorie mise à jour, ou null si aucune ligne n’a été affectée.</returns>
        public async Task<Categorie> ModifyAsync(Categorie categorie)
        {
            string query = $"UPDATE {CATEGORIE_TABLE} SET nom = @nom WHERE id = @Id";

            // ExecuteAsync renvoie le nombre de lignes affectées
            int numLine = await _dbSession.Connection.ExecuteAsync(query, categorie, transaction: _dbSession.Transaction);

            // Si aucune ligne n’a été mise à jour, on renvoie null
            return numLine == 0 ? null : categorie;
        }

        /// <summary>
        /// Supprime une catégorie à partir de son identifiant.
        /// </summary>
        /// <param name="id">Identifiant unique de la catégorie.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            string query = $"DELETE FROM {CATEGORIE_TABLE} WHERE id = @id";

            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { id }, transaction: _dbSession.Transaction);

            return numLine != 0;
        }

        #region Méthodes spécifiques à CategorieRepository

        /// <summary>
        /// Récupère toutes les catégories associées à une recette donnée.
        /// </summary>
        /// <param name="idRecette">Identifiant unique de la recette.</param>
        /// <returns>Une liste de catégories associées à la recette.</returns>
        public async Task<IEnumerable<Categorie>> GetCategoriesByIdRecetteAsync(int idRecette)
        {
            string query = $@"
                SELECT c.* 
                FROM {CATEGORIE_TABLE} c
                JOIN {RECETTE_CATEGORIE_TABLE} rc ON c.id = rc.id_categorie
                WHERE rc.id_recette = @idRecette";

            return await _dbSession.Connection.QueryAsync<Categorie>(query, new { idRecette }, transaction: _dbSession.Transaction);
        }

        /// <summary>
        /// Supprime toutes les relations entre une catégorie et ses recettes.
        /// </summary>
        /// <param name="idCategorie">Identifiant unique de la catégorie.</param>
        /// <returns>True si au moins une relation a été supprimée, sinon false.</returns>
        public async Task<bool> DeleteCategorieRelationsAsync(int idCategorie)
        {
            string query = $"DELETE FROM {RECETTE_CATEGORIE_TABLE} WHERE id_categorie = @idCategorie";

            int numLine = await _dbSession.Connection.ExecuteAsync(query, new { idCategorie }, transaction: _dbSession.Transaction);

            return numLine != 0;
        }

        /// <summary>
        /// Vérifie si une catégorie est liée à au moins une recette.
        /// </summary>
        /// <param name="idCategorie">Identifiant unique de la catégorie.</param>
        /// <returns>True si la catégorie a des relations, sinon false.</returns>
        public async Task<bool> HasRecetteRelationsAsync(int idCategorie)
        {
            string query = $"SELECT COUNT(*) FROM {RECETTE_CATEGORIE_TABLE} WHERE id_categorie = @idCategorie";

            int count = await _dbSession.Connection.ExecuteScalarAsync<int>(query, new { idCategorie }, transaction: _dbSession.Transaction);

            return count > 0;
        }

        #endregion
    }
}
