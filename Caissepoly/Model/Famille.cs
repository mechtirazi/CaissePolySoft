using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaissePoly.Model
{
    public class Famille
    {
        [Key]
        public int idF { get; set; }
        [Required(ErrorMessage = "Le nom de la famille est obligatoire")]
        public string nomF { get; set; }
        public virtual ICollection<Article> Article { get; set; } = new List<Article>();
    }
}
