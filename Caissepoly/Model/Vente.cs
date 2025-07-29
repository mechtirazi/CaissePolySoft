using CaissePoly.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Vente
{
    [Key]
    public int IdV { get; set; }

    public int IdA { get; set; }
    [ForeignKey("IdA")]
    public virtual Article Article { get; set; }

    public int TicketIdT { get; set; }  // Correspond au nom colonne SQL
    [ForeignKey("TicketIdT")]
    public virtual Ticket Ticket { get; set; }

    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }

    public decimal Total => Quantite * PrixUnitaire;
}

