using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewShopShoeApp.Helpers
{
    public class MessageHelper
    {
        public static void ShowError(string message) => MessageBox.Show(message, "Ошибка", MessageBoxButton.OK , MessageBoxImage.Error);
        public static void ShowInformation(string message) => MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        public static void ShowWarning(string message) => MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
