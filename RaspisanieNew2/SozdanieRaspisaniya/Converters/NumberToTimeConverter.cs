using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SozdanieRaspisaniya.Converters
{
    public class NumberToTimeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<int, string> dct = new Dictionary<int, string>();
            string[] strPairTime = { "I 8:30-10:05", "II 10:20-11:55", "III 12:10-13:45", "IV 14:15-15:50", "V 16:05-17:40", "VI 17:50-19:25" };
            for (int i = 0; i < strPairTime.Length; i++)
                dct.Add(i+1, strPairTime[i]);
            return dct[((int)value)];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
