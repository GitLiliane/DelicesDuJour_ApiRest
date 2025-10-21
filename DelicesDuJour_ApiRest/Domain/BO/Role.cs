namespace DelicesDuJour_ApiRest.Domain.BO
{
    public class Role
    {
        public int id { get; set; }

        // Dans base de données, le nom = 'Administrateur' ou 'Utilisateur'
        public string? nom { get; set; }
    }
}
