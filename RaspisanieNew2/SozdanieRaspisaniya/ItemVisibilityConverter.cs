using SozdanieRaspisaniya.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SozdanieRaspisaniya
{
    class ItemVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DropItem)
            {
                var di = value as DropItem;
                if (di.Item != null)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
