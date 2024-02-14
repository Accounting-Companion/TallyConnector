using System.Globalization;
using System.Windows.Data;

namespace DemoDesktopApp.Converters;
public class ViewTypeBoolConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {

            bool ismatch = (ViewType)values[0] == (ViewType)values[1];
            return ismatch;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
