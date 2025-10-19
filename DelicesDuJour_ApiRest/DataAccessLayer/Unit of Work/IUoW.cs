using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Ingredients;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngred;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Unit_of_Work
{
    public interface IUoW : IDisposable
    {
        #region Repositories

        IRecetteRepository Recettes { get; }
        IEtapeRepository Etapes { get; }
        ICategorieRepository Categories { get; }
        IIngredientRepository Ingredients { get; }
        IQuantiteIngredRepository QuantiteIngred { get; }


        #endregion Fin Repositories

        #region Transactions

        bool HasActiveTransaction { get; }

        void BeginTransaction();
        void Commit();
        void Rollback();

        #endregion Fin Transactions


    }
}
