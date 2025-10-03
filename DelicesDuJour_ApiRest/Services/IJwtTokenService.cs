namespace DelicesDuJour_ApiRest.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username, params string[] roles);
    }
}
