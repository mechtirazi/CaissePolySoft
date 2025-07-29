using CaissePoly.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CaissePoly.ViewModel
{
   public class familleViewModel: INotifyPropertyChanged
    {
        private readonly CDBContext _context;

        public ObservableCollection<Famille> Familles { get; set; }
        private Famille _selectedFamille;

        public Famille SelectedFamille
        {
            get => _selectedFamille;
            set
            {
                _selectedFamille = value;
                OnPropertyChanged(nameof(SelectedFamille)); // Ensure this raises PropertyChanged
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

                // Try updating SelectedArticle quantity live
                if (SelectedArticle != null && int.TryParse(_valeurSaisie, out int quantite))
                {
                    SelectedArticle.quantiteVente = quantite;

                    // Optionally update DB immediately, or defer for Enter
                    _context.Article.Update(SelectedArticle);
                    _context.SaveChanges();

                    // If you want, raise property changed for SelectedArticle so UI refreshes
                    OnPropertyChanged(nameof(SelectedArticle));
                }
            }
        }

        private decimal _totalTicket;
        public decimal TotalTicket
        {
            get => _totalTicket;
            set
            {
                _totalTicket = value;
                OnPropertyChanged();
            }
        }
        public Action? OnArticlesUpdated;

        public ICommand ValiderCommandeCommand { get; }
        public ICommand EnterCommand { get; }
        public ICommand DeleteCharCommand { get; }
        public ICommand DeleteArticleCommand { get; }

        public ICommand NumberCommand { get; }

        public ICommand SelectFamilleCommand { get; }
        public ICommand SelectArticleCommand { get; }
        public ICommand AfficherFamillesCommand { get; }

        private bool _afficherFamilles;

        public bool AfficherFamilles
        {
            get => _afficherFamilles;
            set
            {
                _afficherFamilles = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TestVisibility)); // ➕ très important
                ChargerArticlesParFamille(); // 🔁 mise à jour automatique des articles
            }
        }
        private Article _selectedArticle;
        public Article SelectedArticle
        {
            get => _selectedArticle;
            set
            {
                _selectedArticle = value;
                Console.WriteLine($"SelectedArticle changé : {(value != null ? value.designation : "null")}");
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Article> _articlesParFamille = new();
        public ObservableCollection<Article> ArticlesParFamille
        {
            get => _articlesParFamille;
            set { _articlesParFamille = value; OnPropertyChanged(nameof(ArticlesParFamille)); }
        }
        private ObservableCollection<Article>  _filteredArticles = new ObservableCollection<Article>();
        

        public ObservableCollection<Article> FilteredArticles
        {
            get => _filteredArticles;
            set
            {
                _filteredArticles = value;
                OnPropertyChanged(nameof(FilteredArticles)); // This should notify the UI
            }
        }




        public Visibility TestVisibility => AfficherFamilles ? Visibility.Visible : Visibility.Collapsed;

        public familleViewModel()
        {
            _context = new CDBContext();
            _context.Database.EnsureCreated(); // crée la base si elle n’existe pas
            FilteredArticles = new ObservableCollection<Article>(); // Initialize if needed
            Familles = new ObservableCollection<Famille>(_context.Famille.ToList());

            // ✅ Initialize FilteredArticles early


            AfficherFamillesCommand = new RelayCommand(() =>
            {
                AfficherFamilles = !AfficherFamilles;
            });

            AfficherFamilles = true;

            SelectFamilleCommand = new RelayCommand<Famille>(fam =>
            {
                if (fam != null)
                {
                    SelectedFamille = fam;
                    ChargerArticlesParFamille();
                    Console.WriteLine($"Famille sélectionnée : {fam.nomF}");
                }
            });
            SelectArticleCommand = new RelayCommand<Article>(article =>
            {
                if (article != null)
                {
                    if (!FilteredArticles.Contains(article))
                    {
                        FilteredArticles.Add(article);
                        article.PropertyChanged += Article_PropertyChanged; // 🔴 Ajout ici
                        OnArticlesUpdated?.Invoke();

                    }

                    else
                    {
                        // Optionnel: Gérer le cas où l'article est déjà présent
                        Console.WriteLine($"L'article {article.designation} est déjà dans la liste filtrée.");
                    }
                }


            });
            NumberCommand = new RelayCommand<string>(chiffre =>
            {
                ValeurSaisie += chiffre;
                
            });
            EnterCommand = new RelayCommand(() =>
            {
                Console.WriteLine("Entrée cliquée");
                Console.WriteLine($"ValeurSaisie='{ValeurSaisie}'");
                Console.WriteLine($"SelectedArticle = {(SelectedArticle != null ? SelectedArticle.designation : "null")}");

                if (int.TryParse(ValeurSaisie, out int quantite) && SelectedArticle != null)
                {
                    SelectedArticle.quantiteVente = quantite;
                    Console.WriteLine("******************************* " + quantite);

                    _context.Article.Update(SelectedArticle);
                    _context.SaveChanges();

                    ValeurSaisie = ""; // reset après mise à jour
                }
                else
                {
                    Console.WriteLine("Condition if failed: TryParse or SelectedArticle is null");
                }
            });
            DeleteCharCommand = new RelayCommand(() =>
            {
                if (!string.IsNullOrEmpty(ValeurSaisie))
                {
                    ValeurSaisie = ValeurSaisie.Substring(0, ValeurSaisie.Length - 1);
                }
            });

            // Commande pour supprimer l'article sélectionné
            DeleteArticleCommand = new RelayCommand(() =>
            {
                if (SelectedArticle != null)
                {
                    try
                    {
                        FilteredArticles.Remove(SelectedArticle);

                        SelectedArticle = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression : {ex.Message}");
                    }
                }
            });
            ValiderCommandeCommand = new RelayCommand(() =>
            {
                RecalculerTotalTicket();

                var nouveauTicket = new Ticket
                {
                    DateTicket = DateTime.Now,
                    Total = TotalTicket,
                    ModePaiement = "Espèce" // ou autre, tu peux le rendre dynamique
                };

                foreach (var article in FilteredArticles)
                {
                    if (article.quantiteVente > 0)
                    {
                        var vente = new Vente
                        {
                            IdA = article.idA,
                            Quantite = article.quantiteVente,
                            PrixUnitaire = article.prixunitaire ?? 0,
                             // Liaison directe au ticket
                             Article = article,
                        };

                        nouveauTicket.Ventes.Add(vente);
                    }
                }

                _context.Tickets.Add(nouveauTicket);
                _context.SaveChanges();

                MessageBox.Show($"✅ Commande validée. Total : {TotalTicket} Dinars");

                FilteredArticles.Clear();
            });










        }
        private void RecalculerTotalTicket()
        {
            TotalTicket = FilteredArticles.Sum(a => a.Total);
        }

        private void Article_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Article.quantiteVente))
            {
                var article = sender as Article;
                if (article != null)
                {
                    try
                    {
                        _context.Article.Update(article);
                        _context.SaveChanges();
                        Console.WriteLine($"✅ Quantité mise à jour pour {article.designation} : {article.quantiteVente}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ Erreur lors de la mise à jour : {ex.Message}");
                    }
                }
            }
        }



        private void ChargerArticlesParFamille()
        {
            
            if (SelectedFamille != null)
            {

                var articles = _context.Article
                    .Where(a => a.idF == SelectedFamille.idF)
                    .ToList();

                ArticlesParFamille = new ObservableCollection<Article>(articles);
            }
            else
            {
                ArticlesParFamille.Clear();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
