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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaissePoly.Componnents.boutton
{
    /// <summary>
    /// Logique d'interaction pour Espece.xaml
    /// </summary>
    public partial class Espece : UserControl
    {
        public Espece()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            // Récupère la fenêtre parente
            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = false;
            Window.GetWindow(this).Close();
        }



    }
}
