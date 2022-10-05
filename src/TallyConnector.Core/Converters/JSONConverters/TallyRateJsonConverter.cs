namespace TallyConnector.Core.Converters.JSONConverters;
public class TallyRateJsonConverter : JsonConverter<TallyRate>
{
    public override TallyRate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        decimal RatePerUnit = 0;
        string Unit = string.Empty;

        decimal ForexAmount = 0;

        decimal RateOfExchange = 0;

        string ForeignCurrency = string.Empty;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (ForexAmount == 0 && RateOfExchange == 0 && ForeignCurrency == string.Empty)
                {
                    return new TallyRate(RatePerUnit, Unit);
                }
                return new TallyRate(Unit, ForexAmount, RateOfExchange, ForeignCurrency);
            }
            else
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    if (propertyName?.Equals(nameof(TallyRate.RatePerUnit), StringComparison.InvariantCultureIgnoreCase) ?? false)
                    {
                        RatePerUnit = reader.GetDecimal();
                        continue;
                    }
                    if (propertyName?.Equals(nameof(TallyRate.Unit), StringComparison.InvariantCultureIgnoreCase) ?? false)
                    {
                        Unit = reader.GetString() ?? string.Empty;
                        continue;
                    }
                    if (propertyName?.Equals(nameof(TallyRate.ForexAmount), StringComparison.InvariantCultureIgnoreCase) ?? false)
                    {
                        ForexAmount = reader.GetDecimal();
                        continue;
                    }
                    if (propertyName?.Equals(nameof(TallyRate.RateOfExchange), StringComparison.InvariantCultureIgnoreCase) ?? false)
                    {
                        RateOfExchange = reader.GetDecimal();
                        continue;
                    }
                    if (propertyName?.Equals(nameof(TallyRate.ForeignCurrency), StringComparison.InvariantCultureIgnoreCase) ?? false)
                    {
                        ForeignCurrency = reader.GetString() ?? string.Empty;
                        continue;
                    }
                }
            }
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, TallyRate value, JsonSerializerOptions options)
    {
        JsonNamingPolicy? propertyNamingPolicy = options.PropertyNamingPolicy;
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStartObject();
            writer.WriteNumber(propertyNamingPolicy?.ConvertName(nameof(value.RatePerUnit)) ?? nameof(value.RatePerUnit), value.RatePerUnit);
            writer.WriteString(propertyNamingPolicy?.ConvertName(nameof(value.Unit)) ?? nameof(value.Unit), value.Unit);
            writer.WriteNumber(propertyNamingPolicy?.ConvertName(nameof(value.ForexAmount)) ?? nameof(value.ForexAmount), value.ForexAmount ?? 0);
            writer.WriteNumber(propertyNamingPolicy?.ConvertName(nameof(value.RateOfExchange)) ?? nameof(value.RateOfExchange), value.RateOfExchange ?? 0);
            writer.WriteString(propertyNamingPolicy?.ConvertName(nameof(value.ForeignCurrency)) ?? nameof(value.ForeignCurrency), value.ForeignCurrency);
            writer.WriteEndObject();
        }
    }
}
