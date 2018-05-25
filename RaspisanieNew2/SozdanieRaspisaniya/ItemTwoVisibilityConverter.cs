using SozdanieRaspisaniya.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SozdanieRaspisaniya
{
    class ItemTwoVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DropItem)
            {
                var di = value as DropItem;
                if (di.ItemTwo == null)
                {
                    if (di.State == 0)
                        return Visibility.Collapsed;
                    else
                        return Visibility.Hidden;
                }
                return Visibility.Visible;

            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }
}
