namespace TallyConnector.Core.Converters.JSONConverters;
public class TallyYesNoJsonConverter : JsonConverter<TallyYesNo>
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
            bool boolvalue = value;
            writer.WriteBooleanValue(boolvalue);
        }
    }
}

