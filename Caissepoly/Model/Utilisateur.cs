using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaissePoly.Model
{
    public class Utilisateur
    {
        [Key]
        public int idU { get; set; }
        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
        public string nomU { get; set; }
        public string Role { get; set; }
        public string MP { get; set; }
        public virtual ICollection<Vente> Vente { get; set; } = new List<Vente>();
    }
}
