using CaissePoly.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Ticket
{
    [Key]
    public int IdT { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime DateTicket { get; set; } = DateTime.Now;

    public string? ModePaiement { get; set; }

    public decimal? Total { get; set; }

    // Une commande contient plusieurs lignes de vente
    
    public virtual ICollection<Vente> Ventes { get; set; } = new List<Vente>();

}
