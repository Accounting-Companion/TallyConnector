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
                    if (propertyName == nameof(TallyRate.RatePerUnit))
                    {
                        RatePerUnit = reader.GetDecimal();
                        continue;
                    }
                    if (propertyName == nameof(TallyRate.Unit))
                    {
                        Unit = reader.GetString() ?? string.Empty;
                        continue;
                    }
                    if (propertyName == nameof(TallyRate.ForexAmount))
                    {
                        ForexAmount = reader.GetDecimal();
                        continue;
                    }
                    if (propertyName == nameof(TallyRate.RateOfExchange))
                    {
                        RateOfExchange = reader.GetDecimal();
                        continue;
                    }
                    if (propertyName == nameof(TallyRate.ForeignCurrency))
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
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(value.RatePerUnit), value.RatePerUnit);
            writer.WriteString(nameof(value.Unit), value.Unit);
            writer.WriteNumber(nameof(value.ForexAmount), value.ForexAmount ?? 0);
            writer.WriteNumber(nameof(value.RateOfExchange), value.RateOfExchange ?? 0);
            writer.WriteString(nameof(value.ForeignCurrency), value.ForeignCurrency);
            writer.WriteEndObject();
        }
    }
}
