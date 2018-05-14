﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Sklad2018.Model.Converter
{
    public class DecimalToPositiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal)
                return (0 <= (decimal)value);
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                    return 1;
                else
                    return -1;
            }

            return 0;
        }
    }
}