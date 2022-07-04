namespace TallyConnector.Core.Converters.JSONConverters;
public class TallyAmountJsonConverter : JsonConverter<TallyAmount>
{
    private readonly bool _alllowSimple;

    public TallyAmountJsonConverter()
    {
    }

    public TallyAmountJsonConverter(bool alllowSimple = false)
    {
        _alllowSimple = alllowSimple;
    }

    public override TallyAmount? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject && reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal();
        }
        decimal? Amount = 0;
        decimal? ForexAmount = 0;
        decimal? RateOfExchange = 0;
        string Currency = string.Empty;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new TallyAmount(ForexAmount, RateOfExchange, Currency, amount: (decimal)Amount);
            }
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                if (propertyName == "Amount")
                {
                    Amount = reader.GetDecimal();
                    continue;
                }
                if (propertyName == "ForexAmount")
                {
                    ForexAmount = reader.GetDecimal();
                    continue;
                }
                if (propertyName == "RateOfExchange")
                {
                    RateOfExchange = reader.GetDecimal();
                    continue;
                }
                if (propertyName == "Currency")
                {
                    Currency = reader.GetString();
                    continue;
                }
            }
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, TallyAmount value, JsonSerializerOptions options)
    {
        if (!_alllowSimple || value.ForexAmount != null
            && value.ForexAmount != 0
            && value.RateOfExchange != null
            && value.RateOfExchange != 0
            && value.Currency != null
            && value.Currency != string.Empty)
        {
            writer.WriteStartObject();
            writer.WriteNumber("Amount", value.Amount);
            if (value.ForexAmount != null)
            {
                writer.WriteNumber("ForexAmount", (decimal)value.ForexAmount);
            }
            else
            {
                writer.WriteNull("ForexAmount");
            }
            if (value.RateOfExchange != null)
            {
                writer.WriteNumber("RateOfExchange", (decimal)value.RateOfExchange);
            }
            else
            {
                writer.WriteNull("RateOfExchange");
            }
            writer.WriteString("Currency", value.Currency);
            writer.WriteBoolean("IsDebit", value.IsDebit);
            writer.WriteEndObject();

        }
        else
        {
            if (value.IsDebit)
            {
                writer.WriteNumberValue(value.Amount * -1);
            }
            else
            {
                writer.WriteNumberValue(value.Amount);
            }
        }
    }
}
