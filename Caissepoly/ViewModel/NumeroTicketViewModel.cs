using CaissePoly.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace CaissePoly.ViewModel
{
    public class NumeroTicketViewModel : INotifyPropertyChanged
    {
        private readonly CDBContext _context;
        public Action? OnArticlesUpdated;

        public ObservableCollection<int> ListeTickets { get; set; } = new();

        private int _selectedTicket;
        public int SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                if (_selectedTicket != value)
                {
                    _selectedTicket = value;
                    Console.WriteLine($"SelectedTicket changed to: {_selectedTicket}");
                    UpdateTicketInfo();
                    OnPropertyChanged(nameof(SelectedTicket));
                    
                }
            }
        }

        private decimal _totalTicket;
        public decimal TotalTicket
        {
            get => _totalTicket;
            set
            {
                if (_totalTicket != value)
                {
                    _totalTicket = value;
                    OnPropertyChanged(nameof(TotalTicket));
                }
            }
        }

        private string _paiement;
        public string Paiement
        {
            get => _paiement;
            set
            {
                if (_paiement != value)
                {
                    _paiement = value;
                    OnPropertyChanged(nameof(Paiement));
                }
            }
        }

        // Liste des articles filtrés pour le ticket sélectionné
        public ObservableCollection<Article> FilteredArticles { get; set; } = new();

        public NumeroTicketViewModel()
        {
            _context = new CDBContext();
            _context.Database.EnsureCreated();

            var tickets = _context.Tickets.ToList();

            if (tickets.Any())
            {
                foreach (var t in tickets)
                    ListeTickets.Add(t.IdT);
                // or some valid ticket ID

              
            }
            else
            {
                SelectedTicket = 1;
            }
            SelectedTicket = ListeTickets.FirstOrDefault();
            Console.WriteLine("********************");
            UpdateTicketInfo();


           
        }

        private void UpdateTicketInfo()
        {
            Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            var ticket = _context.Tickets
                .Where(t => t.IdT == SelectedTicket)
                .Select(t => new
                {
                    t.ModePaiement,
                    t.Total,
                    Ventes = t.Ventes.Select(v => new
                    {
                        v.Quantite,
                        Article = v.Article
                    }).ToList()
                })
                .FirstOrDefault();

            if (ticket != null)
            {
                Paiement = ticket.ModePaiement ?? "----";
                TotalTicket = ticket.Total ?? 0;
                Console.WriteLine(TotalTicket);
                FilteredArticles.Clear();

                foreach (var vente in ticket.Ventes)
                {
                    if (vente.Article != null)
                    {
                        var article = vente.Article;
                        article.quantiteVente = vente.Quantite;
                        FilteredArticles.Add(article);
                    }
                }
                foreach (var article in FilteredArticles)
                {
                    Console.WriteLine($"Article : {article.designation}, Prix : {article.designation}");
                }


                // MessageBox uniquement si tu en as vraiment besoin
                //MessageBox.Show("Ticket chargé : " + SelectedTicket);

                OnArticlesUpdated?.Invoke();
            }
            else
            {
                Paiement = "----";
                TotalTicket = 0;
                FilteredArticles.Clear();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string nom) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nom));
    }
}
