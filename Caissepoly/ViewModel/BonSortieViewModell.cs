using CaissePoly.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace CaissePoly.ViewModel
{
    public class BonSortieViewModell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly CDBContext _context = new CDBContext();
        private int _selectedTicket;
        public int SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                _selectedTicket = value;
                OnPropertyChanged(nameof(SelectedTicket));
            }
        }
        public ICommand PrintCommand { get; set; }

        // Champs privés pour StartDate et EndDate
        private DateTime? _startDate;
        private DateTime? _endDate;

        // Propriétés publiques pour binding
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }
        private VenteViewModel _selectedVente;
        public VenteViewModel SelectedVente
        {
            get => _selectedVente;
            set
            {
                _selectedVente = value;
                SelectedTicket = value?.TicketId ?? 0; // met à jour SelectedTicket
                OnPropertyChanged(nameof(SelectedVente));
            }
        }



        private void ImprimerTicket()

        {
            Console.WriteLine(SelectedTicket);
            var ticketCourant = _context.Tickets
                .Include(t => t.Ventes)
                .ThenInclude(v => v.Article)
                .FirstOrDefault(t => t.IdT == SelectedTicket);

            if (ticketCourant == null || ticketCourant.Ventes.Count == 0)
            {
                MessageBox.Show("Aucun ticket à imprimer.");
                return;
            }

            FlowDocument doc = new FlowDocument
            {
                FontFamily = new FontFamily("Consolas"),
                FontSize = 14,
                PageWidth = 300
            };

            Paragraph header = new Paragraph(new Run("🧾 Ticket de caisse PolySoft"))
            {
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.Bold
            };
            doc.Blocks.Add(header);

            doc.Blocks.Add(new Paragraph(new Run($"🕒 {ticketCourant.DateTicket:dd/MM/yyyy HH:mm}")));
            doc.Blocks.Add(new Paragraph(new Run($"💳 Paiement : {ticketCourant.ModePaiement}")));
            doc.Blocks.Add(new Paragraph(new Run("----------------------------------")));

            foreach (var vente in ticketCourant.Ventes)
            {
                string designation = vente.Article?.designation ?? "Article inconnu";
                decimal totalLigne = vente.Quantite * vente.PrixUnitaire;
                string ligne = $"{vente.Quantite} x {designation} : {vente.PrixUnitaire:F2} DT = {totalLigne:F2} DT";
                doc.Blocks.Add(new Paragraph(new Run(ligne)));
            }

            doc.Blocks.Add(new Paragraph(new Run("----------------------------------")));
            doc.Blocks.Add(new Paragraph(new Run($"🧮 Total : {ticketCourant.Total:F2} DT")));
            doc.Blocks.Add(new Paragraph(new Run("✅ Merci pour votre achat")));

            // 🖨️ Sauvegarde en XPS temporaire
            string xpsPath = Path.Combine(Path.GetTempPath(), "ticket_temp.xps");
            using (var xpsDoc = new XpsDocument(xpsPath, FileAccess.Write))
            {
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                writer.Write(((IDocumentPaginatorSource)doc).DocumentPaginator);
            }

            // 📄 Convertir XPS en PDF via Microsoft Print to PDF
            string pdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"Ticket_{ticketCourant.IdT}.pdf");

            PrintDialog printDialog = new PrintDialog();

            // Sélectionner "Microsoft Print to PDF" comme imprimante
            PrintQueue printQueue = null;
            var server = new PrintServer();
            foreach (var pq in server.GetPrintQueues())
            {
                if (pq.Name.Equals("Microsoft Print to PDF", StringComparison.OrdinalIgnoreCase))
                {
                    printQueue = pq;
                    break;
                }
            }

            if (printQueue == null)
            {
                MessageBox.Show("Imprimante 'Microsoft Print to PDF' introuvable.");
                return;
            }

            printDialog.PrintQueue = printQueue;
            printDialog.PrintTicket = new System.Printing.PrintTicket { PageOrientation = PageOrientation.Portrait };

            using (var xpsDoc = new XpsDocument(xpsPath, FileAccess.Read))
            {
                var paginator = xpsDoc.GetFixedDocumentSequence().DocumentPaginator;

                var writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);

                // ATTENTION : Il n’y a pas d’API officielle simple pour définir le nom du fichier PDF de sortie.
                // Ce que tu peux faire : afficher la boîte de dialogue et demander à l’utilisateur de choisir le chemin.

                if (printDialog.ShowDialog() == true)
                {
                    writer.Write(paginator);
                }
                else
                {
                    return; // L’utilisateur a annulé l’impression
                }
            }

            // 🗃️ Attendre un peu puis ouvrir le PDF (si tu connais le chemin exact)


         
        }
        // Liste observable pour la DataGrid
        private ObservableCollection<VenteViewModel> _sortieArticles;
        public ObservableCollection<VenteViewModel> SortieArticles
        {
            get => _sortieArticles;
            set
            {
                _sortieArticles = value;
                OnPropertyChanged(nameof(SortieArticles));
            }
        }

        // Liste complète non observable
        private List<VenteViewModel> AllArticles;

        // Commandes pour les boutons
        public ICommand FilterCommand { get; set; }
        public ICommand ResetFilterCommand { get; set; }

        // Constructeur
        public BonSortieViewModell()
        {
            LoadVentes();

            FilterCommand = new RelayCommand(FilterVentes);
            ResetFilterCommand = new RelayCommand(ResetVentes);
            PrintCommand = new RelayCommand(ImprimerTicket);


        }

        // Charger toutes les ventes depuis la base
        private void LoadVentes()
        {
            using var context = new CDBContext();

            var ventesFromDb = context.Vente
                .Include(v => v.Article)
                .Include(v => v.Ticket)
                .ToList();

            AllArticles = ventesFromDb.Select(v => new VenteViewModel
            {
                Article = v.Article,
                Quantite = v.Quantite,
                PrixUnitaire = v.PrixUnitaire,
                TicketDate = v.Ticket?.DateTicket,
                TicketId = v.Ticket?.IdT ?? 0
            }).ToList();

            SortieArticles = new ObservableCollection<VenteViewModel>(AllArticles);
        }

        // Appliquer le filtre sur les dates
        private void FilterVentes()
        {
            var filtered = AllArticles.AsEnumerable();

            if (StartDate.HasValue)
                filtered = filtered.Where(v => v.TicketDate >= StartDate.Value);

            if (EndDate.HasValue)
                filtered = filtered.Where(v => v.TicketDate <= EndDate.Value);

            SortieArticles = new ObservableCollection<VenteViewModel>(filtered);
        }

        // Réinitialiser le filtre
        private void ResetVentes()
        {
            StartDate = null;
            EndDate = null;
            SortieArticles = new ObservableCollection<VenteViewModel>(AllArticles);

            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }

        // Notification
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}