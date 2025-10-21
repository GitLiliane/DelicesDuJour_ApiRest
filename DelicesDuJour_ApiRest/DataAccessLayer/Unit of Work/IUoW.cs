using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Ingredients;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngred;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Utilisateurs;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Unit_of_Work
{
    /// <summary>
    /// Interface représentant une unité de travail (Unit of Work) pour gérer
    /// les transactions et l'accès aux différents repositories.
    /// </summary>
    public interface IUoW : IDisposable
    {
        #region Repositories

        /// <summary>
        /// Accès au repository des recettes.
        /// </summary>
        IRecetteRepository Recettes { get; }

        /// <summary>
        /// Accès au repository des étapes.
        /// </summary>
        IEtapeRepository Etapes { get; }

        /// <summary>
        /// Accès au repository des catégories.
        /// </summary>
        ICategorieRepository Categories { get; }

        /// <summary>
        /// Accès au repository des ingrédients.
        /// </summary>
        IIngredientRepository Ingredients { get; }

        /// <summary>
        /// Accès au repository des relations quantité-ingrédient.
        /// </summary>
        IQuantiteIngredRepository QuantiteIngred { get; }

        IUtilisateurRepository Utilisateurs { get; }

        #endregion Fin Repositories

        #region Transactions

        /// <summary>
        /// Indique si une transaction est actuellement active.
        /// </summary>
        bool HasActiveTransaction { get; }

        /// <summary>
        /// Démarre une transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Valide (commit) la transaction en cours.
        /// </summary>
        void Commit();

        /// <summary>
        /// Annule (rollback) la transaction en cours.
        /// </summary>
        void Rollback();

        #endregion Fin Transactions
    }
}
