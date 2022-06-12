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
                else
                {
                    return null;
                }
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                while (propertyName == Comparestring && reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        if (Comparestring != nameof(TallyQuantity.SecondaryUnits))
                        {
                            Comparestring = nameof(TallyQuantity.SecondaryUnits);
                            continue;
                        }
                        else
                        {
                            if (secondaryQuantity != null && secondaryUnit != null)
                            {
                                break;
                            }
                            else
                            {
                                return new TallyQuantity((decimal)quantity, unit);
                            }

                        }
                    }

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        var NpropertyName = reader.GetString();
                        reader.Read();
                        if (NpropertyName == nameof(TallyQuantity.SecondaryUnits.Number))
                        {
                            if (propertyName == nameof(TallyQuantity.PrimaryUnits))
                            {
                                quantity = reader.GetDecimal();
                            }
                            else
                            {
                                secondaryQuantity = reader.GetDecimal();
                            }
                        }
                        if (NpropertyName == nameof(TallyQuantity.PrimaryUnits.Unit))
                        {
                            if (propertyName == nameof(TallyQuantity.PrimaryUnits))
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
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {

            writer.WriteStartObject();
            writer.WriteNumber(nameof(value.Number), value.Number);
            writer.WritePropertyName(nameof(value.PrimaryUnits));
            if (value.PrimaryUnits is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteNumber(nameof(value.PrimaryUnits.Number), value.PrimaryUnits.Number);
                writer.WriteString(nameof(value.PrimaryUnits.Unit), value.PrimaryUnits.Unit);
                writer.WriteEndObject();
            }
            writer.WritePropertyName(nameof(value.SecondaryUnits));
            if (value.SecondaryUnits is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteNumber(nameof(value.SecondaryUnits.Number), value.SecondaryUnits.Number);
                writer.WriteString(nameof(value.SecondaryUnits.Unit), value.SecondaryUnits.Unit);
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }
    }
}
