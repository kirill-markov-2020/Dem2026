using NewShopShoeApp.Database;
using NewShopShoeApp.Helpers;
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
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        private ShopShoeDbEntities _db = new ShopShoeDbEntities();
        private List<Order> _orders = new List<Order>();
        private User _currentUser = CurrentSession.CurrentUser;
        public OrderWindow()
        {
            InitializeComponent();
            FIOTextBlock.Text = _currentUser.FullName;
            LoadOrders();
        }
        public void LoadOrders()
        {
            
            _db = new ShopShoeDbEntities();
            _orders = _db.Order.ToList();
            OrderList.ItemsSource = _orders;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new ProductWindow(_currentUser).Show();
            Close();
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditWindowOpen())
                return;
            var editWindow = new OrderEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                LoadOrders();
                MessageHelper.ShowInformation("Список заказов обновлён");
            }
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessHelper.IsAdmin)
            {
                MessageHelper.ShowError("Доступ запрещен! Только администратор может редактировать заказы.");
                return;
            }
            var menuItem = sender as MenuItem;
            var order = menuItem?.Tag as Order;
            if (order == null)
                return;
            var selectedOrder = OrderList.SelectedItem as Order;
            if (selectedOrder == null)
                return;
            if (IsEditWindowOpen())
                return;
            var editWindow = new OrderEditWindow(order,_currentUser);
            if (editWindow.ShowDialog() == true)
            {
                LoadOrders();
                MessageHelper.ShowInformation("Список заказов обновлён");
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessHelper.IsAdmin)
            {
                MessageHelper.ShowError("Доступ запрещен! Только администратор может удалять заказы.");
                return;
            }
            var menuItem = sender as MenuItem;
            var order = menuItem?.Tag as Order;
            if (order == null) return;
            var result = MessageBox.Show("Вы уверены, что хотите удалить заказ?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Order.Remove(order);
                    _db.SaveChanges();
                    LoadOrders();
                    MessageHelper.ShowInformation("Заказ удален");
                }
                catch (Exception ex)
                {
                    MessageHelper.ShowError($"Ошибка при удалении заказа: {ex.Message}");
                }
            }
        }

        private void OrderList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!AccessHelper.IsAdmin)
            {
                MessageHelper.ShowError("Доступ запрещен! Только администратор может редактировать заказы.");
                return;
            }
            var selectedOrder = OrderList.SelectedItem as Order;
            if (selectedOrder == null)
                return;
            if (IsEditWindowOpen())
                return;
            var editWindow = new OrderEditWindow(selectedOrder, _currentUser);
            if (editWindow.ShowDialog() == true)
            {
                LoadOrders();
                MessageHelper.ShowInformation("Список заказов обновлён");
            }
        }
        private bool IsEditWindowOpen()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is OrderEditWindow)
                {
                    MessageHelper.ShowWarning("Окно редактирования уже открыто");
                    window.Activate();
                    return true;
                }

            }
            return false;
        }
    }
}
