using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.IO;

using System.Windows.Media.Imaging;

namespace NewShopShoeApp.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return LoadImage("Resources/picture.png");
            string path = value.ToString();
            if (File.Exists(path))
                return LoadImage(path);
            return LoadImage("Resources/picture.png");

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public BitmapImage  LoadImage(string path)
        {
            var bitm = new BitmapImage();
            bitm.BeginInit();
            bitm.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bitm.CacheOption = BitmapCacheOption.OnLoad;
            bitm.EndInit();
            return bitm;
        }
    }
}
