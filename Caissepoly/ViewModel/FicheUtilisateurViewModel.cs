using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::CaissePoly.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
namespace CaissePoly.ViewModel
{
    
        public class FicheUtilisateurViewModel : INotifyPropertyChanged
        {
            private readonly CDBContext _context = new CDBContext();

            public ObservableCollection<Utilisateur> Users { get; set; } = new ObservableCollection<Utilisateur>();
            public ObservableCollection<string> Roles { get; set; } = new ObservableCollection<string> { "Admin", "Caissier", "Invité" };

            private Utilisateur _selectedUser;
            public Utilisateur SelectedUser
            {
                get => _selectedUser;
                set
                {
                    _selectedUser = value;
                    OnPropertyChanged();
                }
            }

            private string _searchText;
            public string SearchText
            {
                get => _searchText;
                set
                {
                    _searchText = value;
                    OnPropertyChanged();
                }
            }

            // Commands
            public ICommand NewUserCommand { get; }
            public ICommand SaveUserCommand { get; }
            public ICommand DeleteUserCommand { get; }
            public ICommand CloseCommand { get; }
            public ICommand SearchCommand { get; }

            public FicheUtilisateurViewModel()
            {
                LoadUsers();

                SelectedUser = new Utilisateur();

                NewUserCommand = new RelayCommand(NewUser);
                SaveUserCommand = new RelayCommand(SaveUser, () => SelectedUser != null);
                DeleteUserCommand = new RelayCommand(DeleteUser, () => SelectedUser != null);
                CloseCommand = new RelayCommand(FermerFiche);
                SearchCommand = new RelayCommand(SearchUser);
            }

            private void LoadUsers()
            {
                Users.Clear();
                var usersFromDb = _context.Utilisateur.ToList();

                foreach (var user in usersFromDb)
                {
                    Users.Add(user);
                }
            }

            private void NewUser()
            {
                SelectedUser = new Utilisateur();
            }

            private void SaveUser()
            {
                if (SelectedUser == null) return;

                if (SelectedUser.idU == 0)
                {
                    _context.Utilisateur.Add(SelectedUser);
                    _context.SaveChanges();
                    Users.Add(SelectedUser);
                }
                else
                {
                    _context.Utilisateur.Update(SelectedUser);
                    _context.SaveChanges();
                    LoadUsers();
                }
            }

            private void DeleteUser()
            {
                if (SelectedUser == null) return;

                _context.Utilisateur.Remove(SelectedUser);
                _context.SaveChanges();
                Users.Remove(SelectedUser);
                SelectedUser = new Utilisateur();
            }

            private void FermerFiche()
            {
                var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                currentWindow?.Close();
            }

            private void SearchUser()
            {
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var filteredUsers = _context.Utilisateur
                        .Where(u => u.nomU.ToLower().Contains(SearchText.ToLower()))
                        .ToList();

                    Users.Clear();
                    foreach (var user in filteredUsers)
                    {
                        Users.Add(user);
                    }
                }
                else
                {
                    LoadUsers();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    
}
