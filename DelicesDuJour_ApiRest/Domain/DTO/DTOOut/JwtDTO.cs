namespace DelicesDuJour_ApiRest.Domain.DTO.DTOOut
{
    /// <summary>
    /// Data Transfer Object (DTO) représentant un token JWT.
    /// </summary>
    public class JwtDTO
    {
        /// <summary>
        /// Jeton JWT généré pour l'authentification.
        /// </summary>
        public string Token { get; set; }
    }
}

