using NewShopShoeApp.Database;
using NewShopShoeApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
    /// Логика взаимодействия для OrderEditWindow.xaml
    /// </summary>
    public partial class OrderEditWindow : Window
    {
        private Order _editingOrder;
        private ShopShoeDbEntities _db = new ShopShoeDbEntities();
        public OrderEditWindow(Order order = null, User currentUser = null)
        {
            InitializeComponent();
            _editingOrder = order;
            LoadComboBoxes();
            if(order != null )
            {
                Title = "Редактирование заказа";
                LoadOrderData();
            }
            else
            {
                Title = "Создание нового заказа";
                IdTextBlock.Visibility = Visibility.Collapsed;
                IdTextBox.Visibility = Visibility.Collapsed;

            }
        }
        /*Загрузка данных в ListBox*/        
        public void LoadOrderData()
        {
            IdTextBox.Text = _editingOrder.Id.ToString();
            ArticleTextBox.Text = _editingOrder.Article;
            StatusComboBox.SelectedValue = _editingOrder.StatusId;
            UserComboBox.SelectedValue = _editingOrder.UserId;
            PickUpPointComboBox.SelectedValue = _editingOrder.PickUpPointId;
            OrderDatePicker.SelectedDate = _editingOrder.OrderDate;
            DeliveryDatePicker.SelectedDate = _editingOrder.DeliveryDate;
        }
        public void LoadComboBoxes()
        {
            StatusComboBox.ItemsSource = _db.Status.ToList();
            UserComboBox.ItemsSource = _db.User.ToList();
            PickUpPointComboBox.ItemsSource = _db.PickUpPoint.ToList();
            
        }
        /*Валидация полей*/
        private bool ValidateFields()
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(ArticleTextBox.Text))
                errors.AppendLine("Введите артикул заказа!");

            if (StatusComboBox.SelectedValue == null)
                errors.AppendLine("Выберите статус заказа!");

            if (PickUpPointComboBox.SelectedValue == null)
                errors.AppendLine("Выберите пункт выдачи!");

            if (OrderDatePicker.SelectedDate == null)
                errors.AppendLine("Выберите дату заказа!");
            else
            {
                DateTime orderDate = OrderDatePicker.SelectedDate.Value;
                if (DeliveryDatePicker.SelectedDate != null && DeliveryDatePicker.SelectedDate.Value < orderDate)
                    errors.AppendLine("Дата доставки не может быть раньше даты заказа!");
            }

            if (errors.Length > 0)
            {
                MessageHelper.ShowError(errors.ToString());
                return false;
            }
            return true;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateFields())
                    return;
                Order order = _editingOrder ?? new Order();
                
                order.Article = ArticleTextBox.Text;
                order.StatusId = (int)StatusComboBox.SelectedValue;
                order.PickUpPointId = (int)PickUpPointComboBox.SelectedValue;
                order.OrderDate = OrderDatePicker.SelectedDate.Value;
                order.DeliveryDate = DeliveryDatePicker.SelectedDate ?? DateTime.Now;
                order.UserId = (int)UserComboBox.SelectedValue;
                if (_editingOrder == null)
                {
                    int maxGetCode = _db.Order.Any() ? _db.Order.Max(o => o.GetCode) : 0;
                    order.GetCode = maxGetCode + 1;
                    _db.Order.Add(order);

                }
                else
                {
                    order.GetCode = _editingOrder.GetCode;
                    _db.Order.AddOrUpdate(order);
                }
                    _db.SaveChanges();

                string message = _editingOrder == null ? "Заказ успешно добавлен!" : "Заказ успешно обновлен!";
                MessageHelper.ShowInformation(message);
                DialogResult = true;
                Close();
            }
            catch(Exception ex) 
            {
                MessageHelper.ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
