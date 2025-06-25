using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace DemoDesktopApp.Converters;
public class IEnumerableToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not IEnumerable collection)
        {
            return string.Empty;
        }

        // Use string.Join to create a comma-separated list.
        // This works for collections of strings, ints, etc.
        return string.Join(", ", collection.Cast<object>());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
