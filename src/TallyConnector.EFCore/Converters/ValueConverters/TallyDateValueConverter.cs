
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TallyConnector.Core.Converters.XMLConverterHelpers;

namespace TallyConnector.EFCore.Converters.ValueConverters;
public class TallyDateValueConverter : ValueConverter<TallyDate, DateTime?>
{
    public TallyDateValueConverter()
        : base(
            v => (DateTime?)v,
            v => v!)
    {
    }


}
