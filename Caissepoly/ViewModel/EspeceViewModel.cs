using CaissePoly.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CaissePoly.ViewModel
{
    public class EspeceViewModel : ViewModelBase
    {
        private readonly CDBContext _context = new CDBContext();

        public Ticket TicketActuel { get; set; }
        private decimal montantRecu;
        public decimal MontantRecu
        {
            get => montantRecu;
            set
            {
                if (montantRecu != value)
                {
                    montantRecu = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(MontantRendu));
                }
            }
        }

        public decimal TotalTicket => TicketActuel?.Total ?? 0;
        public decimal MontantRendu => MontantRecu - TotalTicket;

        public ICommand ValidatePaymentCommand { get; }
        public ICommand CancelCommand { get; }

        public Action<bool?> CloseWindowAction { get; set; }

        public EspeceViewModel(Ticket ticket)
        {
            TicketActuel = ticket;

            ValidatePaymentCommand = new RelayCommand(() =>
            {
                if (MontantRecu >= TotalTicket)
                {
                    TicketActuel.ModePaiement = "Espèce";

                    var t = _context.Tickets.FirstOrDefault(x => x.IdT == TicketActuel.IdT);
                    if (t != null)
                    {
                        t.ModePaiement = "Espèce";
                        _context.SaveChanges();
                    }

                    CloseWindowAction?.Invoke(true); // Payment OK → close with true
                }
                else
                {
                    MessageBox.Show($"Montant insuffisant. Montant à payer : {TotalTicket:F3} DT");
                    CloseWindowAction?.Invoke(false); // Payment failed → close with false
                }
            });

            CancelCommand = new RelayCommand(() =>
            {
                CloseWindowAction?.Invoke(null); // Cancel → close with null
            });
        }
    }

}
