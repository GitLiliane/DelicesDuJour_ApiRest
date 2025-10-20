using Dapper;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.DTOIn;
using Mysqlx.Crud;
using MySqlX.XDevAPI.Common;
using System.Diagnostics.Eventing.Reader;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes
{
    /// <summary>
    /// Repository pour la gestion des entités <see cref="Etape"/> dans la base de données.
    /// Utilise Dapper pour effectuer les opérations SQL et une session de base de données injectée via <see cref="IDBSession"/>.
    /// </summary>
    public class EtapeRepository : IEtapeRepository
    {
        /// <summary>
        /// Nom de la table correspondante dans la base de données.
        /// </summary>
        private const string ETAPE_TABLE = "etapes";

        /// <summary>
        /// Session de base de données (connexion et transaction en cours).
        /// </summary>
        private readonly IDBSession _dbSession;

        /// <summary>
        /// Initialise une nouvelle instance du repository <see cref="EtapeRepository"/>.
        /// </summary>
        /// <param name="dBSession">Session de base de données à utiliser.</param>
        public EtapeRepository(IDBSession dBSession)
        {
            _dbSession = dBSession;
        }

        /// <summary>
        /// Récupère toutes les étapes de toutes les recettes.
        /// </summary>
        /// <returns>Une collection de toutes les étapes triées par recette et numéro.</returns>
        public async Task<IEnumerable<Etape>> GetAllAsync()
        {
            // Requête SQL pour récupérer toutes les étapes, triées par recette et numéro d'étape.
            string query = $"SELECT * FROM {ETAPE_TABLE} ORDER BY id_recette ASC, numero ASC;";

            // Exécution de la requête
            var etapes = await _dbSession.Connection.QueryAsync<Etape>(query, transaction: _dbSession.Transaction);

            return etapes;
        }

        /// <summary>
        /// Récupère une étape spécifique selon la clé composite (id_recette, numero).
        /// </summary>
        /// <param name="key">Tuple contenant id_recette et numero.</param>
        /// <returns>L’étape correspondante, ou null si non trouvée.</returns>
        public async Task<Etape> GetAsync((int, int) key)
        {
            // Requête SQL pour récupérer une étape précise.
            string query = $"SELECT * FROM {ETAPE_TABLE} WHERE id_recette = @id_recette AND numero = @numero";

            var result = await _dbSession.Connection.QuerySingleOrDefaultAsync<Etape>(
                query,
                new { id_recette = key.Item1, numero = key.Item2 },
                transaction: _dbSession.Transaction
            );

            return result;
        }

        /// <summary>
        /// Crée une nouvelle étape dans la base de données.
        /// </summary>
        /// <param name="etape">Objet <see cref="Etape"/> à insérer.</param>
        /// <returns>L’objet <see cref="Etape"/> créé avec l’ID mis à jour.</returns>
        public async Task<Etape> CreateAsync(Etape etape)
        {
            // Requête SQL d’insertion avec retour de l’ID recette associé.
            string query = $"INSERT INTO {ETAPE_TABLE} (id_recette, numero, titre, texte) VALUES (@id_recette, @numero, @titre, @texte) RETURNING id_recette";

            // Exécution et récupération de l’ID généré.
            int id_recette = await _dbSession.Connection.ExecuteScalarAsync<int>(query, etape, transaction: _dbSession.Transaction);
            etape.id_recette = id_recette;

            return etape;
        }

        /// <summary>
        /// Met à jour une étape existante selon son id_recette et son numéro.
        /// </summary>
        /// <param name="etape">L’étape modifiée à enregistrer.</param>
        /// <returns>L’objet <see cref="Etape"/> modifié, ou null si aucune ligne n’a été mise à jour.</returns>
        public async Task<Etape> ModifyAsync(Etape etape)
        {
            // Requête SQL de mise à jour
            string query = $"UPDATE {ETAPE_TABLE} SET titre = @titre, texte = @texte WHERE id_recette = @id_recette AND numero = @numero";

            int numLine = await _dbSession.Connection.ExecuteAsync(query, etape, transaction: _dbSession.Transaction);

            return numLine == 0 ? null : etape;
        }

        /// <summary>
        /// Supprime une étape spécifique selon sa clé composite.
        /// </summary>
        /// <param name="key">Tuple contenant id_recette et numero.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        public async Task<bool> DeleteAsync((int, int) key)
        {
            // Requête SQL de suppression
            string query = $"DELETE FROM {ETAPE_TABLE} WHERE id_recette = @id_recette AND numero = @numero";

            int numLine = await _dbSession.Connection.ExecuteAsync(
                query,
                new { id_recette = key.Item1, numero = key.Item2 },
                transaction: _dbSession.Transaction
            );

            return numLine != 0;
        }

        #region Methods specific Recette Etape

        /// <summary>
        /// Récupère toutes les étapes d’une recette spécifique.
        /// </summary>
        /// <param name="id">Identifiant unique de la recette.</param>
        /// <returns>Liste des étapes associées à la recette.</returns>
        public async Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int id)
        {
            // Requête SQL triant les étapes par numéro d’ordre
            string query = $"SELECT * FROM {ETAPE_TABLE} WHERE id_recette = @id_recette ORDER BY numero ASC";

            var result = await _dbSession.Connection.QueryAsync<Etape>(
                query,
                new { id_recette = id },
                transaction: _dbSession.Transaction
            );

            return result;
        }

        /// <summary>
        /// Supprime toutes les étapes associées à une recette donnée.
        /// </summary>
        /// <param name="id_recette">Identifiant unique de la recette.</param>
        /// <returns>True si au moins une étape a été supprimée, sinon false.</returns>
        public async Task<bool> DeleteEtapesRelationByIdRecetteAsync(int id_recette)
        {
            // Requête SQL pour supprimer toutes les étapes liées à une recette
            string query = $"DELETE FROM {ETAPE_TABLE} WHERE id_recette = @id_recette";

            int numLine = await _dbSession.Connection.ExecuteAsync(
                query,
                new { id_recette },
                transaction: _dbSession.Transaction
            );

            return numLine != 0;
        }

        #endregion Methods specific Recette Etape
    }
}
