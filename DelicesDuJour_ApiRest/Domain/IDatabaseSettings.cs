namespace DelicesDuJour_ApiRest.Domain
{
    public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }
        DatabaseProviderName? DatabaseProviderName { get; set; }
    }
}
