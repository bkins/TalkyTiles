using System.Globalization;

namespace TalkyTiles.MobileApp.Utilities.Converters;

public class BoolToEditModeLabelConverter : IValueConverter
{
    public object Convert (object      value
                         , Type        targetType
                         , object      parameter
                         , CultureInfo culture)
    {
        return value is bool and true
                ? "✅"
                : "✏️";
    }

    public object ConvertBack (object      value
                             , Type        targetType
                             , object      parameter
                             , CultureInfo culture)
        => throw new NotImplementedException();
}
