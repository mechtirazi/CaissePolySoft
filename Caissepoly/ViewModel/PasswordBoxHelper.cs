using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace CaissePoly.ViewModel
{


        public static class PasswordBoxHelper
        {
            public static readonly DependencyProperty BoundPassword =
                DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
                    new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

            public static string GetBoundPassword(DependencyObject obj) =>
                (string)obj.GetValue(BoundPassword);

            public static void SetBoundPassword(DependencyObject obj, string value) =>
                obj.SetValue(BoundPassword, value);

            private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                if (d is PasswordBox passwordBox)
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                    if (!(bool)passwordBox.GetValue(IsUpdatingProperty))
                    {
                        passwordBox.Password = e.NewValue?.ToString() ?? "";
                    }
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
            }

            private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
            {
                if (sender is PasswordBox passwordBox)
                {
                    SetIsUpdating(passwordBox, true);
                    SetBoundPassword(passwordBox, passwordBox.Password);
                    SetIsUpdating(passwordBox, false);
                }
            }

            private static readonly DependencyProperty IsUpdatingProperty =
                DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBoxHelper));
            private static bool GetIsUpdating(DependencyObject obj) => (bool)obj.GetValue(IsUpdatingProperty);
            private static void SetIsUpdating(DependencyObject obj, bool value) => obj.SetValue(IsUpdatingProperty, value);
        }
    

}
