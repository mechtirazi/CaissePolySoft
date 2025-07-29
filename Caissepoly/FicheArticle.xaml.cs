using CaissePoly.admin;
using CaissePoly.ViewModel;
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

namespace CaissePoly
{
    /// <summary>
    /// Logique d'interaction pour FicheArticle.xaml
    /// </summary>
    public partial class FicheArticle : Window
    {
        public FicheArticle()
        {
            InitializeComponent();
            this.DataContext = new FicheArticleViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var existingWindow = Application.Current.Windows
                    .OfType<MenuPrincipal >()
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
                MenuPrincipal mainWindow = new MenuPrincipal();
                this.Close();
                mainWindow.Show();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
