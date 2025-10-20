namespace DelicesDuJour_ApiRest.Domain
{
    /// <summary>
    /// Interface représentant les paramètres JWT utilisés pour la génération et la validation des tokens.
    /// </summary>
    public interface IJwtSettings
    {
        /// <summary>
        /// Clé secrète utilisée pour signer le JWT.
        /// </summary>
        string Secret { get; }

        /// <summary>
        /// Émetteur du JWT (Issuer).
        /// </summary>
        string Issuer { get; }

        /// <summary>
        /// Destinataire du JWT (Audience).
        /// </summary>
        string Audience { get; }

        /// <summary>
        /// Durée de validité du JWT en minutes.
        /// </summary>
        int ExpirationMinutes { get; }
    }
}
