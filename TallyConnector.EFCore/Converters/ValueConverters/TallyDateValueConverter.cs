
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TallyConnector.Core.Converters.XMLConverterHelpers;

namespace TallyConnector.EFCore.Converters.ValueConverters;
public class TallyDateValueConverter : ValueConverter<TallyDate, DateTime?>
{
    public TallyDateValueConverter()
        : base(
            v => (DateTime?)v,
            v => v)
    {
    }

    //public TallyDateValueConverter(Expression<Func<TallyDate, DateTime?>> convertToProviderExpression, Expression<Func<DateTime?, TallyDate>> convertFromProviderExpression, ConverterMappingHints? mappingHints = null) : base(convertToProviderExpression, convertFromProviderExpression, mappingHints)
    //{
    //}

    //public TallyDateValueConverter(Expression<Func<TallyDate, DateTime?>> convertToProviderExpression, Expression<Func<DateTime?, TallyDate>> convertFromProviderExpression, bool convertsNulls, ConverterMappingHints? mappingHints = null) : base(convertToProviderExpression, convertFromProviderExpression, convertsNulls, mappingHints)
    //{
    //}
}
