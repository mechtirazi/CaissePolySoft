using CaissePoly.Model;
using CaissePoly.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.EntityFrameworkCore;

namespace CaissePoly.admin
{
    /// <summary>
    /// Logique d'interaction pour BonSortieWindow.xaml
    /// </summary>
    public partial class BonSortieWindow : Window
    {
        public ObservableCollection<VenteViewModel> SortieArticles { get; set; }

        public BonSortieWindow()
        {
            InitializeComponent();

            DataContext = new BonSortieViewModell();
        }


        private void RetourAccueilButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Content is CaissePoly.admin.MenuPrincipale)
                {
                    window.Activate();             // Affiche cette fenêtre existante
                    this.Close();                  // Ferme la fenêtre actuelle (par exemple Inventaire)
                    return;
                }
            }

            // Si elle n’est pas déjà ouverte, tu peux en créer une
            var newMenu = new Window
            {
                Title = "Menu Principale",
                Content = new CaissePoly.admin.MenuPrincipale(),
                Height = 700,
        Width = 1302
            };

            newMenu.Show();
            this.Close();
        }

    }
    
}