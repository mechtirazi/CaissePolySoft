using CaissePoly.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
    /// Logique d'interaction pour espece.xaml
    /// </summary>
    public partial class espece : Window
    {
        // Propriété pour stocker le résultat du paiement (null = annulation, true = validé, false = rejeté)
        public bool? PaymentResult { get; private set; } = null;
        public espece(EspeceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Cette action est appelée par le ViewModel pour fermer la fenêtre et transmettre le résultat
            viewModel.CloseWindowAction = result =>
            {
                PaymentResult = result; // Stocker le résultat ici
                this.Close();           // Fermer la fenêtre
            };
        }


        private void Espece_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModelBase vm)
            {
                vm.CloseWindowAction = this.Close;
            }
        }

        private void Espece_Loaded_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
