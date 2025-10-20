namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories
{
    /// <summary>
    /// Interface générique définissant les opérations de lecture (Read)
    /// communes à tous les repositories.
    /// </summary>
    /// <typeparam name="TKey">Type de la clé primaire de l’entité.</typeparam>
    /// <typeparam name="TEntity">Type de l’entité manipulée par le repository.</typeparam>
    public interface IGenericReadRepository<TKey, TEntity>
    {
        /// <summary>
        /// Récupère l’ensemble des entités du type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <returns>Une collection d’objets <typeparamref name="TEntity"/>.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>
        /// Récupère une entité spécifique à partir de sa clé primaire.
        /// </summary>
        /// <param name="key">Identifiant unique de l’entité.</param>
        /// <returns>L’entité correspondante ou <c>null</c> si elle n’existe pas.</returns>
        Task<TEntity> GetAsync(TKey key);
    }
}

