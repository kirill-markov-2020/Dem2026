using NewShopShoeApp.Database;
using NewShopShoeApp.Helpers;
using NewShopShoeApp.Statics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            LoadUI();
        }
        public void LoadProducts()
        {
            _db = new ShopShoeDbEntities();
            _products = _db.Product.ToList();
            ProductList.ItemsSource = _products;

            /* Заполнение комбобокса фильтрации поставщиками*/
            var filters = new List<string>();
            filters.Add("Все поставщики");
            filters.AddRange(_db.Supplier.Select(s => s.Name).ToList());
            FilteringCombobox.ItemsSource = filters;
        }
        /*Разграничение прав доступа по ролям*/
        public void LoadUI()
        {
            AddProductButton.Visibility = Visibility.Collapsed;
            FilterPanel.Visibility = Visibility.Collapsed;
            OrderButton.Visibility = Visibility.Collapsed;
            if (AccessHelper.IsAdmin)
            {
                AddProductButton.Visibility = Visibility.Visible;
                FilterPanel.Visibility = Visibility.Visible;
                OrderButton.Visibility = Visibility.Visible;
            }
            if (AccessHelper.IsManager)
            {
                FilterPanel.Visibility = Visibility.Visible;
                OrderButton.Visibility = Visibility.Visible;

            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentSession.CurrentUser = null;
            new MainWindow().Show();
            Close();
        }

        private void SortingCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void FilteringCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }
        /*Логика фильтрации, сортировки  и поиска */
        public void ApplyFilter()
        {
            if (SearchTextBox == null || FilteringCombobox == null || SortingCombobox == null)
                return;
            var query = _products.AsEnumerable();
            string search = SearchTextBox.Text ?? "";
            string filter = FilteringCombobox.SelectedItem as string ?? "Все поставщики";
            int sort = SortingCombobox.SelectedIndex;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => (s.Name != null && s.Name.ToLower().Contains(search.ToLower())) ||
                (s.Description != null && s.Description.ToLower().Contains(search.ToLower())) ||
                (s.Category != null && s.Category.Name.ToLower().Contains(search.ToLower())) ||
                (s.Unit != null && s.Unit.Name.ToLower().Contains(search.ToLower())) ||
                (s.Supplier != null && s.Supplier.Name.ToLower().Contains(search.ToLower())) ||
                (s.Producer != null && s.Producer.Name.ToLower().Contains(search.ToLower())));
            }

            if(filter != "Все поставщики")
                query = query.Where(s => s.Supplier != null && s.Supplier.Name == filter);

            if (sort == 1)
                query = query.OrderByDescending(s => s.AmountStock);
            else if(sort == 2)
                query = query.OrderBy(s => s.AmountStock);
            ProductList.ItemsSource = query.ToList();
            
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditWindowOpen())
                return;
            var editWindow = new ProductEditWindow();
            if(editWindow.ShowDialog() == true)
            {
                LoadProducts();
                MessageHelper.ShowInformation("Список товаров обновлён");
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessHelper.IsAdmin)
            {
                MessageHelper.ShowError("Доступ запрещен! Только администратор может редактировать товары.");
                return;
            }
            var menuItem = sender as MenuItem;
            var product = menuItem?.Tag as Product;
            if (product == null)
                return;
            
            
            var selectedProduct = ProductList.SelectedItem as Product;
            if (selectedProduct == null) 
                return;
            if (IsEditWindowOpen())
                return;
            var editWindow = new ProductEditWindow(product);
            if(editWindow.ShowDialog() == true)
            {
                LoadProducts();
                MessageHelper.ShowInformation("Список товаров обновлён");
            }
            
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessHelper.IsAdmin)
            {
                MessageHelper.ShowError("Доступ запрещен! Только администратор может удалять товары.");
                return;
            }
            var menuItem = sender as MenuItem;
            var product = menuItem?.Tag as Product;
            if(product == null)
                return;
            
            bool isInOrder = _db.OrderItem.Any(oi => oi.ProductId == product.Id);
            if(isInOrder)
            {
                MessageHelper.ShowError("Товар присутствует в заказах");
                return;
            }
            var result = MessageBox.Show("Вы действительно хотите удалить данный товар?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (!string.IsNullOrEmpty(product.Photo) && File.Exists(product.Photo))
                        File.Delete(product.Photo);
                    _db.Product.Remove(product);
                    _db.SaveChanges();
                    LoadProducts();
                    MessageHelper.ShowInformation("Удалено!");
                }
                catch (Exception ex)
                {
                    MessageHelper.ShowError($"{ex.Message}");
                }
            }

            
        }
        /*Редактирование товара при двойном клике по нему*/
        private void ProductList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!AccessHelper.IsAdmin)
            {
                MessageHelper.ShowError("Доступ запрещен! Только администратор может редактировать товары.");
                return;
            }
            
            var selectedProduct = ProductList.SelectedItem as Product;
            if (selectedProduct == null)
                return;
            if (IsEditWindowOpen())
                return;
            var editWindow = new ProductEditWindow(selectedProduct);
            if (editWindow.ShowDialog() == true)
            {
                LoadProducts();
                MessageHelper.ShowInformation("Список товаров обновлён");
            }
            
        }
        /*Невозможность открытия более одного окна редактирования*/
        private bool IsEditWindowOpen()
        {
            foreach(Window window in Application.Current.Windows)
            {
                if(window is ProductEditWindow)
                {
                    MessageHelper.ShowWarning("Окно редактирования уже открыто");
                    window.Activate();
                    return true;
                }
                
            }
            return false;
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow().Show();
            Close();
        }
    }
}
