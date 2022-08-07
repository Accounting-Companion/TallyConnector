using System.Globalization;

namespace TallyConnector.Core.Converters.JSONConverters;
public class TallyDateJsonConverter : JsonConverter<TallyDate>
{
    public override TallyDate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var Date = reader.GetString();

        if (Date != null && Date != string.Empty)
        {
            bool IsSucess = DateTime.TryParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
            return date;
        }
        return null;
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
