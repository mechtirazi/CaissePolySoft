using CaissePoly.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static CaissePoly.espece;
using System.Diagnostics;
using System.IO;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Markup;
using System.Windows.Controls;



namespace CaissePoly.ViewModel
{
    public class MainViewModel :INotifyPropertyChanged

    {
        private readonly CDBContext _context = new CDBContext();

        public ObservableCollection<Famille> Familles { get; set; }
        public ObservableCollection<int> ListeTickets { get; set; } = new();

        private Famille _selectedFamille;
        public Famille SelectedFamille
        {
            get => _selectedFamille;
            set
            {
                _selectedFamille = value;
                OnPropertyChanged();
                ChargerArticlesParFamille();
            }
        }

        private string _searchText = "";

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _index;
        public int Index
        {
            get => _index;
            set { _index = value; OnPropertyChanged(); }
        }
        public int TotalQuantite => FilteredArticles?.Sum(a => a.quantiteVente) ?? 0;

        public decimal TotalPrix => FilteredArticles?.Sum(a => a.Total) ?? 0;

        public ICommand SearchCommand { get; }

        public event Action<string> OnSearchRequested; // Si tu veux réagir dans un parent


        private List<Article> TousLesArticles = new(); // ✅ Liste complète à filtrer


        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                OnPropertyChanged();
            }
        }

        private int _selectedTicket;
        public int SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                _selectedTicket = value;
                if (SelectedTicket == ListeTickets.Last()+1 && estonloading)
                {
                    IsReadOnly = true;
                    estonloading = !estonloading;

                }
                else if (SelectedTicket == ListeTickets.Last() && !estonloading)
                {
                    IsReadOnly = true;
                }
                else
                {
                    IsReadOnly = false;
                }
                FilteredArticles.Clear();

                OnPropertyChanged();
                UpdateTicketInfo();
            }
        }

        private ObservableCollection<Article> _filteredArticles = new();
        public ObservableCollection<Article> FilteredArticles
        {
            get => _filteredArticles;
            set
            {
                _filteredArticles = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Article> _articlesParFamille = new();
        public ObservableCollection<Article> ArticlesParFamille
        {
            get => _articlesParFamille;
            set { _articlesParFamille = value; OnPropertyChanged(); }
        }

        private Article _selectedArticle;
        public Article SelectedArticle
        {
            get => _selectedArticle;
            set
            {
                _selectedArticle = value;
                RecalculerTotalTicket();
                OnPropertyChanged();

            }
        }

        private string _valeurSaisie = "";
        public string ValeurSaisie
        {
            get => _valeurSaisie;
            set
            {
                _valeurSaisie = value;
                OnPropertyChanged();

                if (SelectedArticle != null && int.TryParse(_valeurSaisie, out int quantite))
                {
                    if (!IsReadOnly) return;

                    if (quantite > SelectedArticle.quantiteStock)
                    {
                        MessageBox.Show($"Quantité saisie ({quantite}) dépasse le stock disponible ({SelectedArticle.quantiteStock}).", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    SelectedArticle.quantiteVente = quantite;
                    _context.Article.Update(SelectedArticle);
                    _context.SaveChanges();
                    RecalculerTotalTicket();
                    OnPropertyChanged(nameof(SelectedArticle));
                }
            }
        }


        private decimal _totalTicket;
        public decimal TotalTicket
        {
            get => _totalTicket;
            set { _totalTicket = value; OnPropertyChanged(); }
        }
        private string _paiement;
        public string Paiement
        {
            get => _paiement;
            set { _paiement = value; OnPropertyChanged(); }
        }
    


        public Action CloseWindowAction { get; set; }


        public ICommand PrintTicketCommand { get; }
        public ICommand SelectFamilleCommand { get; }
        public ICommand SelectArticleCommand { get; }
        public ICommand ValiderCommandeCommand { get; }
        public ICommand NumberCommand { get; }
        public ICommand EnterCommand { get; }
        public ICommand DeleteCharCommand { get; }
        public ICommand DeleteArticleCommand { get; }
        public ICommand ValiderPaiementCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand AfficherFamillesCommand { get; }

        private bool _afficherFamilles;

        public bool AfficherFamilles
        {
            get => _afficherFamilles;
            set
            {
                _afficherFamilles = value;
                OnPropertyChanged();
                ChargerArticlesParFamille();
                FilteredArticles.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (Article article in e.NewItems)
                        {
                            article.PropertyChanged += Article_PropertyChanged;
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (Article article in e.OldItems)
                        {
                            article.PropertyChanged -= Article_PropertyChanged;
                        }
                    }

                    OnPropertyChanged(nameof(TotalPrix));
                    OnPropertyChanged(nameof(TotalQuantite));
                };

            }
        }
        public bool estonloading;
        public MainViewModel()
        {
            estonloading = true ; 
            IsReadOnly = true;
            _context.Database.EnsureCreated();
            Familles = new ObservableCollection<Famille>(_context.Famille.ToList());
            ListeTickets = new ObservableCollection<int>(_context.Tickets.Select(t => t.IdT));
            ArticlesParFamille = new ObservableCollection<Article>(_context.Article.ToList());

            SelectFamilleCommand = new RelayCommand<Famille>(fam =>
            {
                if (fam != null)
                {
                    SelectedFamille = fam;
                }
            });

            SelectArticleCommand = new RelayCommand<Article>(article =>
            {
                if (!IsReadOnly) return;
                if (article != null && !FilteredArticles.Contains(article) )
                {
                    FilteredArticles.Add(article);
                    article.PropertyChanged += Article_PropertyChanged;
                    MettreAJourIndices(); 
                    RecalculerTotalTicket();
                }
            });

            NumberCommand = new RelayCommand<string>(chiffre => { ValeurSaisie += chiffre; });

            EnterCommand = new RelayCommand(() =>
            {
                
                if (!IsReadOnly) return;
     
                if (SelectedArticle.quantiteVente > SelectedArticle.quantiteStock)

                {
                    
                    Console.WriteLine(SelectedArticle.quantiteVente);
                    MessageBox.Show($"Quantité saisie ({SelectedArticle.quantiteVente}) dépasse le stock disponible ({SelectedArticle.quantiteStock}).", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (int.TryParse(ValeurSaisie, out int quantite) && SelectedArticle != null)
                {
                    SelectedArticle.quantiteVente = quantite;
                    _context.Article.Update(SelectedArticle);
                    _context.SaveChanges();
                    ValeurSaisie = "";
                }
            });
            SearchCommand = new RelayCommand(() =>
            {
                Console.WriteLine($"🔍 Recherche lancée : {SearchText}");
                OnSearchRequested?.Invoke(SearchText);

                if (SearchText != null)
                {
                    var filteredArticles = _context.Article
                        .Where(a => a.designation.ToLower().Contains(SearchText.ToLower()))
                        .ToList();

                    ArticlesParFamille = new ObservableCollection<Article>(filteredArticles);
                }
                else
                {
                    ChargerArticlesParFamille();
                }
            });



            AfficherFamillesCommand = new RelayCommand(() =>
            {
                AfficherFamilles = !AfficherFamilles;
                ArticlesParFamille.Clear();
                ArticlesParFamille = new ObservableCollection<Article>(_context.Article.ToList());
            });

            DeleteCharCommand = new RelayCommand(() =>
            {
                if (!string.IsNullOrEmpty(ValeurSaisie))
                    ValeurSaisie = ValeurSaisie[..^1];
            });

            DeleteArticleCommand = new RelayCommand(() =>
            {
                if (!IsReadOnly) return;
                if (SelectedArticle != null)
                {
                    FilteredArticles.Remove(SelectedArticle);
                    SelectedArticle = null;
                    MettreAJourIndices(); 
                }
            });

            ValiderCommandeCommand = new RelayCommand(() =>
            {
                if (!IsReadOnly) return;
                Console.WriteLine("***********");
                if (SelectedArticle.quantiteVente > SelectedArticle.quantiteStock)

                {

                    Console.WriteLine(SelectedArticle.quantiteVente);
                    MessageBox.Show($"Quantité saisie ({SelectedArticle.quantiteVente}) dépasse le stock disponible ({SelectedArticle.quantiteStock}).", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                RecalculerTotalTicket();

                var ticketTemporaire = new Ticket
                {
                    DateTicket = DateTime.Now,
                    Total = TotalTicket,
                    Ventes = new List<Vente>()
                };

                foreach (var article in FilteredArticles)
                {
                    if (article.quantiteVente > 0)
                    {
                        ticketTemporaire.Ventes.Add(new Vente
                        {
                            IdA = article.idA,
                            Quantite = article.quantiteVente,
                            PrixUnitaire = article.prixunitaire ?? 0,
                            Article = article
                        });
                    }
                }

                var viewModel = new EspeceViewModel(ticketTemporaire);
                var fenetre = new espece(viewModel);

                fenetre.ShowDialog();                      // <-- juste appeler ShowDialog()
                bool? result = fenetre.PaymentResult;      // <-- récupérer résultat ici

                if (result == true)
                {
                    foreach (var article in FilteredArticles)
                    {
                        if (article.quantiteVente > 0)
                        {
                            // Ajout de la vente au ticket temporaire
                            ticketTemporaire.Ventes.Add(new Vente
                            {
                                IdA = article.idA,
                                Quantite = article.quantiteVente,
                                PrixUnitaire = article.prixunitaire ?? 0,
                                Article = article
                            });

                            // 🔽 Mise à jour du stock dans la base de données
                            var articleInDb = _context.Article.FirstOrDefault(a => a.idA == article.idA);
                            if (articleInDb != null)
                            {
                                // Vérifie qu'on ne dépasse pas le stock
                                if (articleInDb.quantiteStock >= article.quantiteVente)
                                {
                                    articleInDb.quantiteStock -= article.quantiteVente;
                                }
                                else
                                {
                                    MessageBox.Show($"❌ Stock insuffisant pour l'article {article.designation}. Stock disponible : {articleInDb.quantiteStock}");
                                    return; // Annuler la commande
                                }
                            }
                        }
                    }

                    _context.Tickets.Add(ticketTemporaire);
                    _context.SaveChanges();

                    MessageBox.Show($"✅ Commande validée. Total : {TotalTicket} Dinars");

                    FilteredArticles.Clear();

                    var currentWindow = Application.Current.Windows
                                          .OfType<Window>()
                                          .FirstOrDefault(w => w.IsActive);

                    if (currentWindow != null)
                    {
                        var newWindow = new MainWindow();
                        newWindow.Show();
                        currentWindow.Close();
                    }
                }
                else
                {
                    MessageBox.Show("❌ Paiement annulé. La commande n'a pas été enregistrée.");
                }
            });



            if (ListeTickets.Any())
            {
                IsReadOnly = true;

                SelectedTicket = ListeTickets.Last() + 1;
                ListeTickets.Add(SelectedTicket);



            }
            
            CancelCommand = new RelayCommand(() =>
            {
                CloseWindowAction?.Invoke();
            });

            ValiderPaiementCommand = new RelayCommand(() =>
            {
                var ticket = _context.Tickets.FirstOrDefault(t => t.IdT == SelectedTicket);
                if (ticket != null)
                {
                    ticket.ModePaiement = Paiement; // Paiement est la propriété qui sera "Espèce" ou autre
                    _context.SaveChanges();

                    MessageBox.Show($"Le mode de paiement du ticket {SelectedTicket} est mis à jour à {Paiement}");
                }
                else
                {
                    MessageBox.Show("Aucun ticket sélectionné");
                }

                CloseWindowAction?.Invoke();
            });
            PrintTicketCommand = new RelayCommand(ImprimerTicket);
            


        }

        private void ImprimerTicket()
        {
            var ticketCourant = _context.Tickets
                .Include(t => t.Ventes)
                .ThenInclude(v => v.Article)
                .FirstOrDefault(t => t.IdT == SelectedTicket);

            if (ticketCourant == null || ticketCourant.Ventes.Count == 0)
            {
                MessageBox.Show("Aucun ticket à imprimer.");
                return;
            }

            FlowDocument doc = new FlowDocument();
            doc.FontFamily = new FontFamily("Consolas");
            doc.FontSize = 14;
            doc.PageWidth = 300;

            Paragraph header = new Paragraph(new Run("🧾 Ticket de caisse PolySoft"));
            header.TextAlignment = TextAlignment.Center;
            header.FontWeight = FontWeights.Bold;
            doc.Blocks.Add(header);

            doc.Blocks.Add(new Paragraph(new Run($"🕒 {ticketCourant.DateTicket}")));
            doc.Blocks.Add(new Paragraph(new Run($"💳 Paiement : {ticketCourant.ModePaiement}")));
            doc.Blocks.Add(new Paragraph(new Run("----------------------------------")));

            foreach (var vente in ticketCourant.Ventes)
            {
                string designation = vente.Article?.designation ?? "Article inconnu";
                decimal totalLigne = vente.Quantite * vente.PrixUnitaire;
                string ligne = $"{vente.Quantite} x {designation} : {vente.Quantite} x {vente.PrixUnitaire} = {totalLigne:F2} DT"; 
                doc.Blocks.Add(new Paragraph(new Run(ligne)));
            }

            doc.Blocks.Add(new Paragraph(new Run("----------------------------------")));
            doc.Blocks.Add(new Paragraph(new Run($"🧮 Total : {ticketCourant.Total:F2} DT")));
            doc.Blocks.Add(new Paragraph(new Run("✅ Merci pour votre achat")));

            // 🖨️ Sauvegarde en XPS (temporaire)
            string xpsPath = Path.Combine(Path.GetTempPath(), "ticket_temp.xps");
            using (var xpsDoc = new XpsDocument(xpsPath, FileAccess.Write))
            {
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                writer.Write(((IDocumentPaginatorSource)doc).DocumentPaginator);
            }

            // 📄 Convertir XPS -> PDF avec Microsoft Print to PDF
            string pdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Ticket_{ticketCourant.IdT}.pdf");

            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintQueue = new PrintQueue(new PrintServer(), "Microsoft Print to PDF");
            printDialog.PrintTicket = new System.Printing.PrintTicket
            {
                PageOrientation = PageOrientation.Portrait
            };

            using (var xpsDoc = new XpsDocument(xpsPath, FileAccess.Read))
            {
                var paginator = xpsDoc.GetFixedDocumentSequence().DocumentPaginator;

                // Rediriger vers PDF avec le nom du fichier
                var writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
                printDialog.PrintQueue.CurrentJobSettings.Description = pdfPath;
                writer.Write(paginator);
            }

            // 🗃️ Attendre un peu puis ouvrir le fichier
            Task.Delay(1500).ContinueWith(_ =>
            {
                if (File.Exists(pdfPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = pdfPath,
                        UseShellExecute = true
                    });
                }
            });
        }





        private void ChargerArticlesParFamille()
        {
            if (SelectedFamille != null)
            {
                TousLesArticles = _context.Article
                    .Where(a => a.idF == SelectedFamille.idF)
                    .ToList();

                ArticlesParFamille = new ObservableCollection<Article>(TousLesArticles);
                
            }
            else
            {
                TousLesArticles = new List<Article>();
                ArticlesParFamille.Clear();
                FilteredArticles.Clear();
            }
        }

        private void UpdateTicketInfo()
        {
            if (IsReadOnly) return;
            var ticket = _context.Tickets
                .Where(t => t.IdT == SelectedTicket)
                .Select(t => new
                {
                    t.ModePaiement,
                    t.Total,
                    Ventes = t.Ventes.Select(v => new { v.Quantite, Article = v.Article }).ToList()
                })
                .FirstOrDefault();

            if (ticket != null)
            {
                Paiement = ticket.ModePaiement ?? "----";
                TotalTicket = ticket.Total ?? 0;
                FilteredArticles.Clear();

                foreach (var vente in ticket.Ventes)
                {
                    if (vente.Article != null)
                    {
                        vente.Article.quantiteVente = vente.Quantite;
                        FilteredArticles.Add(vente.Article);
                    }
                }
                MettreAJourIndices(); 
                IsReadOnly = false; 
            }
            else
            {
                Paiement = "----";
                TotalTicket = 0;
                FilteredArticles.Clear();
                
            }
        }

        private void RecalculerTotalTicket()
        {

            TotalTicket = FilteredArticles.Sum(a => a.Total);
            OnPropertyChanged(nameof(TotalPrix));
            OnPropertyChanged(nameof(TotalQuantite));
        }
        public void ValiderPaiementEnEspece()
        {
            Paiement = "Espèce";

            var ticket = _context.Tickets.FirstOrDefault(t => t.IdT == SelectedTicket);
            if (ticket != null)
            {
                ticket.ModePaiement = Paiement;
                _context.SaveChanges();
            }

            CloseWindowAction?.Invoke();
        }


        private void Article_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Article.quantiteVente))
            {
                RecalculerTotalTicket();
                OnPropertyChanged(nameof(TotalPrix));
                OnPropertyChanged(nameof(TotalQuantite));
            }
        }
        private void MettreAJourIndices()
        {
            for (int i = 0; i < FilteredArticles.Count; i++)
            {
                Index = i;// Commence à 1
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
