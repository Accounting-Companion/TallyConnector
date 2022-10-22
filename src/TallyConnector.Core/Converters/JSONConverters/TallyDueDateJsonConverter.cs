using System.Globalization;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Masters;

namespace TallyConnector.Core.Converters.JSONConverters;
public class TallyDueDateJsonConverter : JsonConverter<TallyDueDate>
{
    public override TallyDueDate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        DateTime? dueDate = null;
        DueDateFormat? suffix = DueDateFormat.Day;
        int? value = 0;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {

                if (value != 0 && suffix != null)
                {
                    return new TallyDueDate((int)value, (DueDateFormat)suffix);
                }
                else if (dueDate != null)
                {
                    return new TallyDueDate((DateTime)dueDate);
                }
                return null;
            }
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName?.Equals(nameof(TallyDueDate.DueDate), StringComparison.InvariantCultureIgnoreCase) ?? false)
                {
                    if (reader.TokenType != JsonTokenType.Null)
                    {
                        bool IsSucess = DateTime.TryParseExact(reader.GetString(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
                        if (IsSucess)
                        {
                            dueDate = date;
                        }

                    }
                    continue;
                }
                if (propertyName?.Equals(nameof(TallyDueDate.Suffix), StringComparison.InvariantCultureIgnoreCase) ?? false)
                {
                    if (reader.TokenType != JsonTokenType.Null)
                    {
                        var data = reader.GetString();
                        suffix = (DueDateFormat?)Enum.Parse(typeof(DueDateFormat), data, true);
                    }
                    continue;
                }
                if (propertyName?.Equals(nameof(TallyDueDate.Value), StringComparison.InvariantCultureIgnoreCase) ?? false)
                {
                    if (reader.TokenType != JsonTokenType.Null)
                    {
                        value = reader.GetInt32();
                    }
                    continue;
                }

            }
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, TallyDueDate value, JsonSerializerOptions options)
    {
        JsonNamingPolicy? propertyNamingPolicy = options.PropertyNamingPolicy;
        if (value is null)
        {
            writer.WriteNullValue();

        }
        else
        {
            writer.WriteStartObject();
            // writer.WriteString(propertyNamingPolicy?.ConvertName(nameof(value.BillDate)) ?? nameof(value.BillDate), value.BillDate.ToString("dd-MM-yyyy"));
            writer.WriteString(propertyNamingPolicy?.ConvertName(nameof(value.DueDate)) ?? nameof(value.DueDate), value.DueDate?.ToString("dd-MM-yyyy"));
            writer.WriteString(propertyNamingPolicy?.ConvertName(nameof(value.Suffix)) ?? nameof(value.Suffix), value.Suffix.ToString());
            writer.WriteNumber(propertyNamingPolicy?.ConvertName(nameof(value.Value)) ?? nameof(value.Value), value.Value);
            writer.WriteEndObject();
        }
    }
}
