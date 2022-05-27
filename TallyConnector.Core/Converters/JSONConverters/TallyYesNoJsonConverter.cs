using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Converters.XMLConverterHelpers;

namespace TallyConnector.Core.Converters.JSONConverters;
public class TallyYesNoValueConverter : JsonConverter<TallyYesNo>
{
    public override TallyYesNo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetBoolean();
    }

    public override void Write(Utf8JsonWriter writer, TallyYesNo value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteBooleanValue(value);
        }
    }
}

