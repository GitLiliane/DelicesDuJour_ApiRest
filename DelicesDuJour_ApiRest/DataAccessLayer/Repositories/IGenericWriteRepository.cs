namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories
{
    /// <summary>
    /// Interface générique définissant les opérations d'écriture (CRUD) de base
    /// pour un repository manipulant des entités identifiées par une clé spécifique.
    /// </summary>
    /// <typeparam name="TKey">Type de la clé primaire (ex. int, Guid, tuple...)</typeparam>
    /// <typeparam name="TEntity">Type de l'entité manipulée (ex. Recette, Categorie...)</typeparam>
    public interface IGenericWriteRepository<TKey, TEntity>
    {
        /// <summary>
        /// Crée une nouvelle entité dans la source de données.
        /// </summary>
        /// <param name="entity">Objet à insérer.</param>
        /// <returns>L'entité créée, incluant les informations mises à jour (ex. ID généré).</returns>
        Task<TEntity> CreateAsync(TEntity entity);

        /// <summary>
        /// Met à jour une entité existante dans la source de données.
        /// </summary>
        /// <param name="entity">Objet à modifier.</param>
        /// <returns>L'entité modifiée, ou null si aucune mise à jour n'a été effectuée.</returns>
        Task<TEntity> ModifyAsync(TEntity entity);

        /// <summary>
        /// Supprime une entité de la source de données en utilisant sa clé unique.
        /// </summary>
        /// <param name="key">Clé de l'entité à supprimer.</param>
        /// <returns>True si la suppression a réussi, False sinon.</returns>
        Task<bool> DeleteAsync(TKey key);
    }
}
