using CaissePoly.admin;
using CaissePoly.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
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

namespace CaissePoly
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly CDBContext _context = new CDBContext();

        public Login()
        {
            InitializeComponent();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            using (var context = new CDBContext())
            {
                var user = context.Utilisateur
                    .FirstOrDefault(u => u.nomU == username && u.MP == password);

                if (user != null)
                {
                    SessionManager.UtilisateurConnecte = user;
                    MessageBox.Show($"Bienvenue {user.nomU}", "Connexion réussie", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (user.Role.ToLower() == "administrateur")
                    {
                        var main = new MenuPrincipal();
                        main.Show();
                        this.Close();
                    }
                    else
                    {
                        var main = new MainWindow();
                        main.Show();
                        this.Close();
                    }

                }
                

                
                else
                {
                    MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
    
}
