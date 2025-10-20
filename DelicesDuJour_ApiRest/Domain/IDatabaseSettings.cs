namespace DelicesDuJour_ApiRest.Domain
{
    /// <summary>
    /// Interface représentant la configuration nécessaire pour se connecter à une base de données.
    /// </summary>
    public interface IDatabaseSettings
    {
        /// <summary>
        /// Chaîne de connexion à la base de données.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Type du fournisseur de base de données utilisé.
        /// </summary>
        DatabaseProviderName? DatabaseProviderName { get; set; }
    }
}
