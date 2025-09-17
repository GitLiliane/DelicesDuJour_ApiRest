namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories
{
    public interface IGenericReadRepository<TKey, TEntity>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetAsync(TKey key);
    }
}
