using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace CaissePoly.ViewModel.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as ListViewItem;
            if (item == null) return "";

            var listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            if (listView == null) return "";

            int index = listView.ItemContainerGenerator.IndexFromContainer(item);
            return (index + 1).ToString(); // Start at 1
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
