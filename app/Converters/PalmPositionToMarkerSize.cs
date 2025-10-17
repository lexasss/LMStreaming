using System.Globalization;
using System.Windows.Data;

namespace LMStreamer.Converters;

internal class PalmPositionToMarkerSize : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var n = (double)value;
        return n == 0 ? 2 : Math.Max(2, 80 * Math.Exp(-0.1 * n)); // exponential decay based on distance
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
