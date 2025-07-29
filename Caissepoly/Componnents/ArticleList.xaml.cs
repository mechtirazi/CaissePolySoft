using CaissePoly.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaissePoly.Componnents
{
    /// <summary>
    /// Logique d'interaction pour ArticleList.xaml
    /// </summary>
    public partial class ArticleList : UserControl
    {
        public ArticleList()
        {
            InitializeComponent();

        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                if (DataContext is MainViewModel vm && vm.EnterCommand.CanExecute(null))
                {
                    vm.EnterCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
