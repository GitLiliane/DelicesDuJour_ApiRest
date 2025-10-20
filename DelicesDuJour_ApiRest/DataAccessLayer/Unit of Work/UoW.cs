using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Ingredients;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngred;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;
using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using Microsoft.Extensions.DependencyInjection;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Unit_of_Work
{
    /// <summary>
    /// Implémentation de l'unité de travail (Unit of Work) pour gérer
    /// les transactions et l'accès aux repositories.
    /// </summary>
    public class UoW : IUoW
    {
        private readonly IDBSession _dbSession;
        private readonly Lazy<IRecetteRepository> _recettes;
        private readonly Lazy<IEtapeRepository> _etapes;
        private readonly Lazy<ICategorieRepository> _categories;
        private readonly Lazy<IIngredientRepository> _ingredients;
        private readonly Lazy<IQuantiteIngredRepository> _quantiteIngred;

        public UoW(IDBSession dbSession, IServiceProvider serviceProvider)
        {
            _dbSession = dbSession;
            _recettes = new Lazy<IRecetteRepository>(() => serviceProvider.GetRequiredService<IRecetteRepository>());
            _etapes = new Lazy<IEtapeRepository>(() => serviceProvider.GetRequiredService<IEtapeRepository>());
            _categories = new Lazy<ICategorieRepository>(() => serviceProvider.GetRequiredService<ICategorieRepository>());
            _ingredients = new Lazy<IIngredientRepository>(() => serviceProvider.GetRequiredService<IIngredientRepository>());
            _quantiteIngred = new Lazy<IQuantiteIngredRepository>(() => serviceProvider.GetRequiredService<IQuantiteIngredRepository>());
        }

        #region Repositories

        /// <summary>
        /// Repository des recettes.
        /// </summary>
        public IRecetteRepository Recettes => _recettes.Value;

        /// <summary>
        /// Repository des étapes.
        /// </summary>
        public IEtapeRepository Etapes => _etapes.Value;

        /// <summary>
        /// Repository des catégories.
        /// </summary>
        public ICategorieRepository Categories => _categories.Value;

        /// <summary>
        /// Repository des ingrédients.
        /// </summary>
        public IIngredientRepository Ingredients => _ingredients.Value;

        /// <summary>
        /// Repository des relations quantité-ingrédient.
        /// </summary>
        public IQuantiteIngredRepository QuantiteIngred => _quantiteIngred.Value;

        #endregion Fin Repositories

        #region Transactions

        /// <summary>
        /// Indique si une transaction est actuellement active.
        /// </summary>
        public bool HasActiveTransaction => _dbSession.HasActiveTransaction;

        /// <summary>
        /// Démarre une transaction.
        /// </summary>
        public void BeginTransaction() => _dbSession.BeginTransaction();

        /// <summary>
        /// Valide (commit) la transaction en cours.
        /// </summary>
        public void Commit() => _dbSession.Commit();

        /// <summary>
        /// Annule (rollback) la transaction en cours.
        /// </summary>
        public void Rollback() => _dbSession.Rollback();

        /// <summary>
        /// Libère les ressources et effectue un rollback si nécessaire.
        /// </summary>
        public void Dispose() => Rollback();

        #endregion Fin Transactions
    }
}
