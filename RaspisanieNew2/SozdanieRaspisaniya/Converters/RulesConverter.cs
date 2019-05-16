using SozdanieRaspisaniya.ViewModel.Rules;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SozdanieRaspisaniya.Converters
{
    class RulesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NoOverlay)
            {
                return "Накладки";
            }
            if (value is CountPair)
            {
                return "Превышение количества пар";
            }
            if (value is PlanCompleted)
            {
                return "Выполнение учебного плана";
            }
            if (value is Windows)
            {
                return "Окна";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
