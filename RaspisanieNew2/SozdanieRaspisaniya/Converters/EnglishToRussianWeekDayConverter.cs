using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SozdanieRaspisaniya.Converters
{
    class EnglishToRussianWeekDayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<DayOfWeek, string> dct = new Dictionary<DayOfWeek, string>();
            string[] strDayWeek = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
            for (int i = 0; i < strDayWeek.Length; i++)
                dct.Add((DayOfWeek)i + 1, strDayWeek[i]);
            return dct[((DayOfWeek)value)];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

}
