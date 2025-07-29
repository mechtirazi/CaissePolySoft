using CaissePoly.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CaissePoly.ViewModel
{
    public class FicheArticleViewModel : INotifyPropertyChanged
    {
        private readonly CDBContext _context = new CDBContext();
        public ICommand RemoveSelectedArticleCommand { get; }
        public ICommand UpdateSelectedArticleCommand { get; }
        public ICommand NewArticleCommand { get; }
        public ICommand SaveArticleCommand { get; }

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

        public ICommand SearchCommand { get; }

        public event Action<string> OnSearchRequested;

        private bool _isCodeEnabled;
        public bool IsCodeEnabled
        {
            get => _isCodeEnabled;
            set
            {
                if (_isCodeEnabled != value)
                {
                    _isCodeEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        // Liste filtrée des articles depuis la base
        public ObservableCollection<Article> Articles { get; set; } = new ObservableCollection<Article>();

        // Article sélectionné dans la vue
        private Article _selectedArticle;
        public Article SelectedArticle
        {
            get => _selectedArticle;
            set
            {
                if (_selectedArticle != value)
                {
                    _selectedArticle = value;
                    OnPropertyChanged();
                    IsCodeEnabled = _selectedArticle != null && _selectedArticle.idA != 0;
                }
            }
        }
        private Article _currentArticle;

        public Article CurrentArticle
        {
            get => _currentArticle;
            set
            {
                if (_currentArticle != value)
                {
                    _currentArticle = value;
                    OnPropertyChanged();
                }
            }
        }
        public void NewArticle()
        {
            IsCodeEnabled = false;
             // Initialiser un nouvel article au démarrage
            // Obtenir la fenêtre active (FicheArticleWindow par exemple)
            var currentWindow = System.Windows.Application.Current.Windows
                                      .OfType<Window>()
                                      .FirstOrDefault(w => w.IsActive);

            if (currentWindow != null)
            {
                // Créer une nouvelle instance de la même fenêtre
                var newWindow = new FicheArticle(); // Remplace par le nom réel de ta fenêtre

                newWindow.Show();   // Ouvre la nouvelle fenêtre
                currentWindow.Close(); // Ferme l'ancienne
            }
        }



        public void RemoveSelectedArticle()

        {
            Console.WriteLine(SelectedArticle);

            if (SelectedArticle != null)
            {
                _context.Article.Remove(SelectedArticle);
                _context.SaveChanges();

                Articles.Remove(SelectedArticle);
                SelectedArticle = null;
            }
        }
        public void UpdateSelectedArticle()
        {
            if (SelectedArticle != null)
            {
                _context.Article.Update(SelectedArticle);
                _context.SaveChanges();

                // Optional: reload to ensure UI has updated values
                LoadArticles();
            }
        }





        private void LoadArticles()
        {
            Articles.Clear();
            var articlesFromDb = _context.Article
                                         .Include(a => a.famille) // Inclure la famille si besoin
                                         .ToList();

            foreach (var article in articlesFromDb)
            {
                Articles.Add(article);
            }
        }

        public void SaveArticle()
        {
            if (SelectedArticle == null) return;

            if (SelectedArticle.idA == 0)
            {
                // Nouvel article
                _context.Article.Add(SelectedArticle);
                _context.SaveChanges();
                Articles.Add(SelectedArticle); // Mettre à jour la liste
            }
            else
            {
                // Modification
                _context.Article.Update(SelectedArticle);
                _context.SaveChanges();
                LoadArticles(); // Rafraîchir toute la liste pour refléter les changements
            }
        }
        public FicheArticleViewModel()
        {
            IsCodeEnabled = false;
            LoadArticles();
            RemoveSelectedArticleCommand = new RelayCommand(RemoveSelectedArticle, () => SelectedArticle != null);
            UpdateSelectedArticleCommand = new RelayCommand(UpdateSelectedArticle, () => SelectedArticle != null);
            NewArticleCommand = new RelayCommand(NewArticle);
           
            SaveArticleCommand = new RelayCommand(SaveArticle, () => SelectedArticle != null);
            SelectedArticle = new Article();

            SearchCommand = new RelayCommand(() =>
            {
                Console.WriteLine($"🔍 Recherche lancée : {SearchText}");
                OnSearchRequested?.Invoke(SearchText);

                if (SearchText != null)
                {
                    var filteredArticles = _context.Article
                        .Where(a => a.designation.ToLower().Contains(SearchText.ToLower()))
                        .ToList();
                    Articles.Clear();

                    foreach (var article in filteredArticles)
                    {
                        Articles.Add(article);
                    }
                }
                else
                {
                    LoadArticles();
                }
            });

        }



    
      

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
