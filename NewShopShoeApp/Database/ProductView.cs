using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NewShopShoeApp.Database
{
    public partial class Product
    {
        public decimal NewPrice => Price * (1 - (Discount / 100));
        public Brush BackgroundColor => GetBack();
        public Visibility PriceVisibility => GetVisibility();

        private Brush GetBack()
        {
            if (Discount > 15)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E8B57"));
            }
            if(AmountStock <= 0)
            {
                return Brushes.LightBlue;
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7FFF00"));
        }
        public Visibility GetVisibility()
        {
            if (Discount != 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
    }
}
