using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace DemoDesktopApp.Converters;

public class ObjectToCollectionConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // If the value is null, return null.
        if (value == null)
        {
            return value;
        }

        // If the value is already a collection, just return it.
        if (value is IEnumerable)
        {
            return value;
        }

        return new List<object> { value };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
