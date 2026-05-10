using NewShopShoeApp.Database;
using NewShopShoeApp.Statics;
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

namespace NewShopShoeApp
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private ShopShoeDbEntities _db = new ShopShoeDbEntities();
        private List<Product> _products = new List<Product>();
        public ProductWindow(User user = null)
        {
            InitializeComponent();
            if (user != null )
                FIOTextBlock.Text = user.FullName;
            else
                FIOTextBlock.Text = "Гость";
            LoadProducts();
        }
        public void LoadProducts()
        {
            _products = _db.Product.ToList();
            ProductList.ItemsSource = _products;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentSession.CurrentUser = null;
            new MainWindow().Show();
            Close();
        }
    }
}
