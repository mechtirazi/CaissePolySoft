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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaissePoly.admin
{
    /// <summary>
    /// Logique d'interaction pour MenuPrincipale.xaml
    /// </summary>
    public partial class MenuPrincipale : UserControl
    {
        public MenuPrincipale()
        {
            InitializeComponent();
        }
        private void CaisseMainButton_Loaded(object sender, RoutedEventArgs e)
        {
            // Démarrer l'animation de pulsation pour la caisse principale
            if (sender is Button btn)
            {
                // Définir un DropShadowEffect si le bouton n'en a pas
                if (btn.Effect == null)
                {
                    btn.Effect = new DropShadowEffect
                    {
                        Color = Colors.Black,
                        BlurRadius = 15,
                        ShadowDepth = 4,
                        Opacity = 0.3
                    };
                }

                // Récupérer le storyboard et l’appliquer au bouton
                var pulseAnimation = (Storyboard)FindResource("PulseAnimation");

                // 🔥 Important : définir la cible de l’animation
                Storyboard.SetTarget(pulseAnimation, btn);
                pulseAnimation.Begin();
            }
        }

        // Gestionnaires d'événements pour les boutons
        private void OnArticlesClick(object sender, RoutedEventArgs e)
        {

            var inventaireWindow = new FicheArticle(); // ← tu dois avoir une classe InventaireWindow;
            inventaireWindow.Show();

            // Optionnel : Fermer la fenêtre actuelle si c'est une Window (non un UserControl)
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void OnBonEntreeClick(object sender, RoutedEventArgs e)
        {
            var inventaireWindow = new BonSortieWindow(); // ← tu dois avoir une classe InventaireWindow;
            inventaireWindow.Show();

            // Optionnel : Fermer la fenêtre actuelle si c'est une Window (non un UserControl)
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void OnInventaireClick(object sender, RoutedEventArgs e)
        {
            var inventaireWindow = new inventaire.inventaire(); // ← tu dois avoir une classe InventaireWindow;
            inventaireWindow.Show();

            // Optionnel : Fermer la fenêtre actuelle si c'est une Window (non un UserControl)
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void OnInformationClick(object sender, RoutedEventArgs e)
        {
            var inventaireWindow = new information(); // ← tu dois avoir une classe InventaireWindow;
            inventaireWindow.Show();

            // Optionnel : Fermer la fenêtre actuelle si c'est une Window (non un UserControl)
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void OnFermerCaisseClick(object sender, RoutedEventArgs e)
        {

            var result = MessageBox.Show("Voulez-vous vraiment fermer la caisse ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }

        }

        private void OnFacturationClick(object sender, RoutedEventArgs e)
        {
            var inventaireWindow = new AjouterFamille(); // ← tu dois avoir une classe InventaireWindow;
            inventaireWindow.Show();

            // Optionnel : Fermer la fenêtre actuelle si c'est une Window (non un UserControl)
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void OnUtilisateursClick(object sender, RoutedEventArgs e)
        {
            var inventaireWindow = new FicheUtilisateur(); // ← tu dois avoir une classe InventaireWindow
            inventaireWindow.Show();

            // Optionnel : Fermer la fenêtre actuelle si c'est une Window (non un UserControl)
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void OnContactClick(object sender, RoutedEventArgs e)
        {
            var inventaireWindow = new Contact(); // ← tu dois avoir une classe InventaireWindow;
            inventaireWindow.Show();

            // Optionnel : Fermer la fenêtre actuelle si c'est une Window (non un UserControl)
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void OnParametreClick(object sender, RoutedEventArgs e)
        { 
        }

        private void OnCaisseMainClick(object sender, RoutedEventArgs e)
        {
            var existingWindow = Application.Current.Windows
                               .OfType<MainWindow>()
                               .FirstOrDefault();

            if (existingWindow != null)
            {
                // Si trouvée, on la met au premier plan
                if (existingWindow.WindowState == WindowState.Minimized)
                    existingWindow.WindowState = WindowState.Normal;
                existingWindow.Activate();
            }
            else
            {
                // Sinon, on crée et on affiche une nouvelle fenêtre
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Window.GetWindow(this)?.Close();

            }


        }
        private void FermerCaisse()
        {
            // Logique pour fermer la caisse
            MessageBox.Show("Caisse fermée", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

            // Vous pouvez ajouter ici :
            // - Sauvegarde des données
            // - Fermeture des connexions
            // - Nettoyage des ressources
            // - Fermeture de l'application
        }
    }
}
