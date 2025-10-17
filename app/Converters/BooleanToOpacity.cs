using System.Globalization;
using System.Windows.Data;

namespace LMStreamer.Converters;

internal class BooleanToOpacity : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isTrue = (bool)value;
        var opacity = isTrue ? 1.0 : 0.5;
        if (!isTrue && parameter is double opacityIfFalse)
            opacity = opacityIfFalse;
        return opacity;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (double)value == 1.0;
    }
}
