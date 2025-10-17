using System.Globalization;
using System.Windows.Data;

namespace LMStreamer.Converters;

internal class PalmPositionToCanvas : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            var n = (double)values[0] * 2.5;
            if (parameter is bool inverse && inverse)
            {
                n = -n;
            }

            var scale = (double)values[1];
            var size = (double)values[2];
            return n + scale / 2 - size / 2; // offset to center on canvas
        }
        catch
        {
            return 0.0;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
