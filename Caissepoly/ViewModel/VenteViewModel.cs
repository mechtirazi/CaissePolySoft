using CaissePoly.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaissePoly.ViewModel
{
    public class VenteViewModel
    {
        public Article Article { get; set; }
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public DateTime? TicketDate { get; set; }
        public int TicketId { get; set; }  // ← ajout du numéro de ticket
    }

}
