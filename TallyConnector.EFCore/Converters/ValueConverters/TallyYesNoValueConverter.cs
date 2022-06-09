using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TallyConnector.Core.Converters.XMLConverterHelpers;

namespace TallyConnector.EFCore.Converters.ValueConverters;
public class TallyYesNoValueConverter : ValueConverter<TallyYesNo, bool?>
{
    public TallyYesNoValueConverter()
       : base(
           v => (bool?)v,
           v => v)
    {
    }
}
