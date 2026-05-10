using NewShopShoeApp.Database;
using NewShopShoeApp.Helpers;
using NewShopShoeApp.Statics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NewShopShoeApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ShopShoeDbEntities _db = new ShopShoeDbEntities();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            var user = _db.User.Where(u => u.Login == LoginTextBox.Text && u.Password == PasswordBox.Password).FirstOrDefault();
            if (user == null)
            {
                MessageHelper.ShowError("Неверный логин или пароль");
                return;
            }
            CurrentSession.CurrentUser = user;
            new ProductWindow(user).Show();
            Close();
        }

        private void GuestAuthorizationTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new ProductWindow().Show();
            Close();
        }
    }
}
