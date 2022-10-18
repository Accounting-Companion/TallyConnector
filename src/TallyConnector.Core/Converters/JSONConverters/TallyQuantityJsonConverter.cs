namespace TallyConnector.Core.Converters.JSONConverters;
public class TallyQuantityJsonConverter : JsonConverter<TallyQuantity>
{
    public override TallyQuantity? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        decimal? quantity = null;
        string? unit = null;
        decimal? secondaryQuantity = null;
        string? secondaryUnit = null;

        string Comparestring = nameof(TallyQuantity.PrimaryUnits);
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (quantity != null && unit != null && secondaryQuantity != null && secondaryUnit != null)
                {
                    return new TallyQuantity((decimal)quantity, unit, (decimal)secondaryQuantity, secondaryUnit);

                }
                else if (quantity != null && unit != null)
                {
                    return new TallyQuantity((decimal)quantity, unit);
                }
                else
                {
                    return null;
                }
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                if (reader.TokenType == JsonTokenType.Null)
                {
                    continue;
                }
                while ((propertyName?.Equals(Comparestring, StringComparison.InvariantCultureIgnoreCase) ?? false) && reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        if (!Comparestring.Equals(nameof(TallyQuantity.SecondaryUnits), StringComparison.InvariantCultureIgnoreCase))
                        {
                            Comparestring = nameof(TallyQuantity.SecondaryUnits);
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        var NpropertyName = reader.GetString();
                        reader.Read();

                        if (NpropertyName?.Equals(nameof(TallyQuantity.SecondaryUnits.Number), StringComparison.InvariantCultureIgnoreCase) ?? false)
                        {

                            if (propertyName.Equals(nameof(TallyQuantity.PrimaryUnits), StringComparison.InvariantCultureIgnoreCase))
                            {
                                quantity = reader.GetDecimal();
                            }
                            else
                            {
                                if (reader.TokenType != JsonTokenType.Null)
                                {
                                    secondaryQuantity = reader.GetDecimal();
                                }
                            }
                        }
                        if (NpropertyName?.Equals(nameof(TallyQuantity.PrimaryUnits.Unit), StringComparison.InvariantCultureIgnoreCase) ?? false)
                        {
                            if (propertyName.Equals(nameof(TallyQuantity.PrimaryUnits), StringComparison.InvariantCultureIgnoreCase))
                            {
                                unit = reader.GetString();
                            }
                            else
                            {
                                secondaryUnit = reader.GetString();
                            }
                        }

                    }


                }

            }
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, TallyQuantity value, JsonSerializerOptions options)
    {
        JsonNamingPolicy? propertyNamingPolicy = options.PropertyNamingPolicy;
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {

            writer.WriteStartObject();
            //writer.WriteNumber(propertyNamingPolicy?.ConvertName(nameof(value.Number)) ?? nameof(value.Number), value.Number);
            writer.WritePropertyName(propertyNamingPolicy?.ConvertName(nameof(value.PrimaryUnits)) ?? nameof(value.PrimaryUnits));
            if (value.PrimaryUnits is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteNumber(propertyNamingPolicy?.ConvertName(nameof(value.PrimaryUnits.Number)) ?? nameof(value.PrimaryUnits.Number), value.PrimaryUnits.Number);
                writer.WriteString(propertyNamingPolicy?.ConvertName(nameof(value.PrimaryUnits.Unit)) ?? nameof(value.PrimaryUnits.Unit), value.PrimaryUnits.Unit);
                writer.WriteEndObject();
            }
            writer.WritePropertyName(propertyNamingPolicy?.ConvertName(nameof(value.SecondaryUnits)) ?? nameof(value.SecondaryUnits));
            if (value.SecondaryUnits is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteNumber(propertyNamingPolicy?.ConvertName(nameof(value.SecondaryUnits.Number)) ?? nameof(value.SecondaryUnits.Number), value.SecondaryUnits.Number);
                writer.WriteString(propertyNamingPolicy?.ConvertName(nameof(value.SecondaryUnits.Unit)) ?? nameof(value.SecondaryUnits.Unit), value.SecondaryUnits.Unit);
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }
    }
}
