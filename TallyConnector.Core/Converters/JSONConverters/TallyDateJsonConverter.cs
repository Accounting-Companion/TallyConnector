namespace TallyConnector.Core.Converters.JSONConverters;
public class TallyDateJsonConverter : JsonConverter<TallyDate>
{
    public override TallyDate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, TallyDate value, JsonSerializerOptions options)
    {
        if (value == null || ((DateTime?)value) == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(((DateTime)value)!.ToString("dd-MM-yyyy"));
        }
    }
}
