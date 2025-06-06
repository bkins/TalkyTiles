﻿using System.Globalization;

namespace TalkyTiles.MobileApp.Utilities.Converters;

public class InverseBoolConverter : IValueConverter
{
    public object Convert (object      value
                         , Type        targetType
                         , object      parameter
                         , CultureInfo culture) =>
            value is bool b
                    ? ! b
                    : value;

    public object ConvertBack (object      value
                             , Type        targetType
                             , object      parameter
                             , CultureInfo culture) =>
            value is bool b
                    ? ! b
                    : value;
}
