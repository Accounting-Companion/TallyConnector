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
        string? Currency = string.Empty;
        bool Isdebit = false;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new TallyAmount(ForexAmount, RateOfExchange, Currency, amount: (decimal)Amount, Isdebit);
            }
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                if (propertyName?.Equals("Amount", StringComparison.InvariantCultureIgnoreCase) ?? false)
                {
                    Amount = reader.TokenType == JsonTokenType.Null ? null : reader.GetDecimal();
                    continue;
                }
                if (propertyName?.Equals("ForexAmount", StringComparison.InvariantCultureIgnoreCase) ?? false)
                {
                    ForexAmount = reader.TokenType == JsonTokenType.Null ? null : reader.GetDecimal();
                    continue;
                }
                if (propertyName?.Equals("RateOfExchange", StringComparison.InvariantCultureIgnoreCase) ?? false)
                {
                    RateOfExchange = reader.TokenType == JsonTokenType.Null ? null : reader.GetDecimal();
                    continue;
                }
                if (propertyName?.Equals("Currency", StringComparison.InvariantCultureIgnoreCase) ?? false)
                {
                    Currency = reader.TokenType == JsonTokenType.Null ? string.Empty : reader.GetString();
                    continue;
                }
                if (propertyName?.Equals("IsDebit", StringComparison.InvariantCultureIgnoreCase) ?? false)
                {
                    Isdebit = reader.TokenType != JsonTokenType.Null && reader.GetBoolean();
                    continue;
                }
            }
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, TallyAmount value, JsonSerializerOptions options)
    {
        JsonNamingPolicy? propertyNamingPolicy = options.PropertyNamingPolicy;

        if (!_alllowSimple || value.ForexAmount != null
            && value.ForexAmount != 0
            && value.RateOfExchange != null
            && value.RateOfExchange != 0
            && value.Currency != null
            && value.Currency != string.Empty)
        {
            writer.WriteStartObject();
            writer.WriteNumber(propertyNamingPolicy?.ConvertName(nameof(TallyAmount.Amount)) ?? nameof(TallyAmount.Amount), value.Amount);
            if (value.ForexAmount != null)
            {
                writer.WriteNumber(propertyNamingPolicy?.ConvertName(nameof(TallyAmount.ForexAmount)) ?? nameof(TallyAmount.ForexAmount), (decimal)value.ForexAmount);
            }
            else
            {
                writer.WriteNull(propertyNamingPolicy?.ConvertName(nameof(TallyAmount.ForexAmount)) ?? nameof(TallyAmount.ForexAmount));
            }
            if (value.RateOfExchange != null)
            {
                writer.WriteNumber(propertyNamingPolicy?.ConvertName(nameof(TallyAmount.RateOfExchange)) ?? nameof(TallyAmount.RateOfExchange), (decimal)value.RateOfExchange);
            }
            else
            {
                writer.WriteNull(propertyNamingPolicy?.ConvertName(nameof(TallyAmount.RateOfExchange)) ?? nameof(TallyAmount.RateOfExchange));
            }
            writer.WriteString(propertyNamingPolicy?.ConvertName(nameof(TallyAmount.Currency)) ?? nameof(TallyAmount.Currency), value.Currency);
            writer.WriteBoolean(propertyNamingPolicy?.ConvertName(nameof(TallyAmount.IsDebit)) ?? nameof(TallyAmount.IsDebit), value.IsDebit);
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
