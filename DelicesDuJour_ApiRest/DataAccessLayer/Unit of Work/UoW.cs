using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Ingredients;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngred;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Unit_of_Work
{
    public class UoW : IUoW
    {
        private readonly IDBSession _dbSession;
        private readonly Lazy<IRecetteRepository> _recettes;
        private readonly Lazy<IEtapeRepository> _etapes;
        private readonly Lazy<ICategorieRepository> _categories;
        private readonly Lazy<IIngredientRepository> _ingredients;
        private readonly Lazy<IQuantiteIngredRepository> _quantiteIngred;


        public UoW(IDBSession dBSession, IServiceProvider serviceProvider)
        {
            _dbSession = dBSession;
            _recettes = new Lazy<IRecetteRepository>(() => serviceProvider.GetRequiredService<IRecetteRepository>());
            _etapes = new Lazy<IEtapeRepository>(() => serviceProvider.GetRequiredService<IEtapeRepository>());
            _categories = new Lazy<ICategorieRepository>(() => serviceProvider.GetRequiredService<ICategorieRepository>());
            _ingredients = new Lazy<IIngredientRepository>(() => serviceProvider.GetRequiredService<IIngredientRepository>());
            _quantiteIngred = new Lazy<IQuantiteIngredRepository>(() => serviceProvider.GetRequiredService<IQuantiteIngredRepository>());
        }

        #region Repositories

        // ATTENTION : Les repositories doivent utiliser la transaction en cours dans les requêtes Dapper

        public IRecetteRepository Recettes => _recettes.Value;
        public IEtapeRepository Etapes => _etapes.Value;
        public ICategorieRepository Categories => _categories.Value;
        public IIngredientRepository Ingredients => _ingredients.Value;
        public IQuantiteIngredRepository QuantiteIngred => _quantiteIngred.Value;

        #endregion Fin Repositories

        #region Transactions

        public bool HasActiveTransaction => _dbSession.HasActiveTransaction;

        public void BeginTransaction() 
            => _dbSession.Commit();

        public void Commit() 
            => _dbSession.Commit();
        public void Rollback() 
            => _dbSession.Rollback();

        #endregion Fin Transactions


    }
}
