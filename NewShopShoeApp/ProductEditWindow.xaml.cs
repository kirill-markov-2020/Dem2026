using Microsoft.Win32;
using NewShopShoeApp.Database;
using NewShopShoeApp.Helpers;
using System;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;



namespace NewShopShoeApp
{
    /// <summary>
    /// Логика взаимодействия для ProductEditWindow.xaml
    /// </summary>
    public partial class ProductEditWindow : Window
    {
        private ShopShoeDbEntities _db = new ShopShoeDbEntities();
        private Product _editingProduct;
        private string _selectedPhotoPath;
        private string _originalPhotoPath;
        public ProductEditWindow(Product product = null)
        {
            InitializeComponent();
            _editingProduct = product;
            LoadComboboxes();
            if (product != null)
            {
                Title = "Редактирование товара";
                LoadProductData();
            }
            else
            {
                Title = "Добавление товара";
                IdTextBox.Visibility = Visibility.Collapsed;
                IdTextBlock.Visibility = Visibility.Collapsed;
            }
        }
        public void LoadProductData()
        {
            IdTextBox.Text = _editingProduct.Id.ToString();
            ArticleTextBox.Text = _editingProduct.Article;
            NameTextBox.Text = _editingProduct.Name;
            DescriptionTextBox.Text = _editingProduct.Description;
            PriceTextBox.Text = _editingProduct.Price.ToString();
            DiscountTextBox.Text = _editingProduct.Discount.ToString();
            AmountStockTextBox.Text = _editingProduct.AmountStock.ToString();
            _selectedPhotoPath = _editingProduct.Photo;
            _originalPhotoPath = _editingProduct.Photo;
            var image = LoadImageFromPath(_selectedPhotoPath);
            PhotoPreview.Source = image != null ? image : GetDefaultImage();
            CategoryComboBox.SelectedValue = _editingProduct.CategoryId;
            UnitComboBox.SelectedValue = _editingProduct.UnitId;
            SupplierComboBox.SelectedValue = _editingProduct.SupplierId;
            ProducerComboBox.SelectedValue = _editingProduct.ProducerId;
        }
        public void LoadComboboxes()
        {
            CategoryComboBox.ItemsSource = _db.Category.ToList();
            UnitComboBox.ItemsSource = _db.Unit.ToList();
            ProducerComboBox.ItemsSource = _db.Producer.ToList();
            SupplierComboBox.ItemsSource = _db.Supplier.ToList();

        }

        public BitmapImage LoadImageFromPath(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return null;
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return null;
            }

        }
        public BitmapImage GetDefaultImage()
        {
            try
            {

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("Resources/picture.png", UriKind.Relative);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedPhotoPath = openFileDialog.FileName;
                var image = LoadImageFromPath(_selectedPhotoPath);
                PhotoPreview.Source = image != null ? image : GetDefaultImage();
            }
        }
        private bool ValidateFields()
        {
            var errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                errors.AppendLine("Введите наименование товара!");
            if (string.IsNullOrWhiteSpace(ArticleTextBox.Text))
                errors.AppendLine("Введите артикул товара!");
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
                errors.AppendLine("Цена должна быть положительным числом");
            if (!int.TryParse(AmountStockTextBox.Text, out int amount) || amount < 0)
                errors.AppendLine("Количество должно быть положительным числом или равно 0");
            if (!decimal.TryParse(DiscountTextBox.Text, out decimal discount) || discount < 0 || discount > 100)
                errors.AppendLine("Скидка должна быть числом от 1 до 100");
            if (CategoryComboBox.SelectedValue == null)
                errors.AppendLine("Категория не выбрана");
            if (UnitComboBox.SelectedValue == null)
                errors.AppendLine("Единица измерения не выбрана");
            if (ProducerComboBox.SelectedValue == null)
                errors.AppendLine("Производитель не выбран");
            if (SupplierComboBox.SelectedValue == null)
                errors.AppendLine("Поставщик не выбран");
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

                Product product = _editingProduct ?? new Product();

                product.Name = NameTextBox.Text;
                product.Article = ArticleTextBox.Text;
                product.Description = DescriptionTextBox.Text;
                product.Price = decimal.Parse(PriceTextBox.Text);
                product.AmountStock = int.Parse(AmountStockTextBox.Text);
                product.Discount = decimal.Parse(DiscountTextBox.Text);
                product.CategoryId = (int)CategoryComboBox.SelectedValue;
                product.ProducerId = (int)ProducerComboBox.SelectedValue;
                product.SupplierId = (int)SupplierComboBox.SelectedValue;
                product.UnitId = (int)UnitComboBox.SelectedValue;

                string savedPhotoPath = _selectedPhotoPath;

                if (!string.IsNullOrEmpty(_selectedPhotoPath) && _selectedPhotoPath != _editingProduct?.Photo)
                {
                    savedPhotoPath = CopyPhotoToProject(_selectedPhotoPath);
                    if (_editingProduct != null && !string.IsNullOrEmpty(_originalPhotoPath) && File.Exists(_originalPhotoPath))
                    {
                        File.Delete(_originalPhotoPath);
                    }
                }
                product.Photo = savedPhotoPath;

                if (_editingProduct == null)
                {
                    _db.Product.Add(product);
                }
                else
                {
                    _db.Product.AddOrUpdate(product);

                }
                _db.SaveChanges();
                string message = _editingProduct == null ? "Товар успешно добавлен!" : "Товар успешно обновлен!";
                MessageHelper.ShowInformation(message);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }
        public string CopyPhotoToProject(string sourcePath)
        {
            string targetDir = AppDomain.CurrentDomain.BaseDirectory + "Resources/";
            if(!Directory.Exists(targetDir)) 
                Directory.CreateDirectory(targetDir);
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(sourcePath);
            string targetPath = Path.Combine(targetDir, fileName);
            File.Copy(sourcePath, targetPath, true);
            ResizeImage(sourcePath, targetPath, 300, 200);
            return targetPath;
        }
        /*public void ResizeImage(string sourcePath, string targetPath, int maxWidth, int maxHeight)
        {
            using (var srcImage = Image.FromFile(sourcePath))
            {
                int newWidth, newHeight;
                if(srcImage.Width > srcImage.Height)
                {
                    newWidth = maxWidth;
                    newHeight = (int)((double)srcImage.Height / srcImage.Width * maxWidth);
                }
                else
                {
                    newHeight = maxHeight;
                    newWidth = (int)((double)srcImage.Width / srcImage.Height * maxHeight);
                }
                using (var destImage = new Bitmap(newWidth, newHeight))
                {
                    using (var graphics = Graphics.FromImage(destImage))
                    {
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(srcImage, 0, 0, newWidth, newHeight);
                    }
                    destImage.Save(targetPath, ImageFormat.Png);
                }
            }
        }*/
        public void ResizeImage(string sourcePath, string targetPath, int maxWidth, int maxHeight)
        {
            using (var srcImage = Image.FromFile(sourcePath))
            {
                var destImage = new Bitmap(srcImage, new System.Drawing.Size(maxWidth, maxHeight));
                destImage.Save(targetPath, ImageFormat.Png);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
