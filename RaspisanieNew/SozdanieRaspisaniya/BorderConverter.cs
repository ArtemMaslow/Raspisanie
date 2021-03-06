﻿using SozdanieRaspisaniya.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace SozdanieRaspisaniya
{
    class BorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PairInfo)
            {
                var info = value as PairInfo;
                if (info.Day != DayOfWeek.Saturday && info.Pair == SheduleSettings.WeekDayMaxCount)
                    return 0;
            }
            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
