using System.Globalization;

namespace Hearts.Converters;

public class AlternatingRowColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int roundNumber)
        {
            return roundNumber % 2 == 0 ? Color.FromArgb("#F5F5F5") : Colors.White;
        }
        return Colors.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
