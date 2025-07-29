using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CaissePoly.ViewModel
{
    public class SearchBoxViewModel : INotifyPropertyChanged
    {
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

        public event Action<string> OnSearchRequested; // Si tu veux réagir dans un parent

        public SearchBoxViewModel()
        {
            SearchCommand = new RelayCommand(() =>
            {
                Console.WriteLine($"🔍 Recherche lancée : {SearchText}");
                OnSearchRequested?.Invoke(SearchText);
                // tu peux ici appeler une méthode pour filtrer les articles, par exemple
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
