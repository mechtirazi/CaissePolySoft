using CaissePoly.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CaissePoly.admin
{
    /// <summary>
    /// Logique d'interaction pour LoginAdmin.xaml
    /// </summary>
    public partial class LoginAdmin : Window
    {
        public bool IsAuthenticated { get; private set; } = false;
       

        private readonly CDBContext _context;
        public Utilisateur UtilisateurConnecte { get; private set; }
        public LoginAdmin()
        {
            InitializeComponent();
            _context = new CDBContext(); // Assure-toi que le contexte est bien configuré
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            var utilisateur = _context.Utilisateur
                .FirstOrDefault(u => u.nomU == username && u.MP == password);

            if (utilisateur != null)
            {
                IsAuthenticated = true;
                UtilisateurConnecte = utilisateur;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                IsAuthenticated = false;
            }
        }
    }
}
