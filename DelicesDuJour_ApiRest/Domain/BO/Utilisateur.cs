using System.ComponentModel.DataAnnotations;
using System.Data;

namespace DelicesDuJour_ApiRest.Domain.BO
{
    public class Utilisateur
    {
        public int id { get; set; }
        public string? identifiant { get; set; }
        
        [DataType(DataType.Password)] // Mot de passe, marqué comme tel pour que le champ HTML généré soit de type password
        public string? pass_word { get; set; }

        public Role? role { get; set; }

        [Required(ErrorMessage = "Le rôle est requis.")]
        public int role_id { get; set; } // Clé étrangère associée au rôle. Elle est requise, ce qui signifie qu’un utilisateur doit obligatoirement avoir un rôle défini

    }
}
