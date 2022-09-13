namespace Tests.Converters.JsonConverters;
public class TallyAmountJsonConverterTests
{
    JsonSerializerOptions jsonSerializerOptions;
    public TallyAmountJsonConverterTests()
    {
        jsonSerializerOptions = new();
        jsonSerializerOptions.Converters.Add(new TallyAmountJsonConverter());
    }


    [Test]
    public void TestSerializeComplexObject()
    {
        TallyAmount tallyAmount = new(5000, 20, "$");
        var json = JsonSerializer.Serialize(tallyAmount, jsonSerializerOptions);
        Assert.That(json,
            Is.EqualTo("{\"Amount\":0,\"ForexAmount\":5000,\"RateOfExchange\":20," +
            "\"Currency\":\"$\",\"IsDebit\":false}"));
    }

    [Test]
    public void TestSerializeSimpleAmount()
    {
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new TallyAmountJsonConverter(true));
        TallyAmount tallyAmount = new(5000);
        var json = JsonSerializer.Serialize(tallyAmount, jsonSerializerOptions);
        Assert.That(json, Is.EqualTo("5000"));
    }
    [Test]
    public void TestSerializeSimpleDebitAmount()
    {
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new TallyAmountJsonConverter(true));
        TallyAmount tallyAmount = new(-5000);
        var json = JsonSerializer.Serialize(tallyAmount, jsonSerializerOptions);
        Assert.That(json, Is.EqualTo("-5000"));
    }
    [Test]
    public void TestSerializeSimpleAmountWithoutallowingSimple()
    {
        TallyAmount tallyAmount = new(5000);
        var json = JsonSerializer.Serialize(tallyAmount, jsonSerializerOptions);
        Assert.That(json,
            Is.EqualTo("{\"Amount\":5000,\"ForexAmount\":null,\"RateOfExchange\":null,\"Currency\":null,\"IsDebit\":false}"));
    }
    [Test]
    public void TestSerializeSimpleDebitAmountWithoutallowingSimple()
    {
        TallyAmount tallyAmount = new(-5000);
        var json = JsonSerializer.Serialize(tallyAmount, jsonSerializerOptions);
        Assert.That(json,
            Is.EqualTo("{\"Amount\":5000,\"ForexAmount\":null,\"RateOfExchange\":null,\"Currency\":null,\"IsDebit\":true}"));
    }
    [Test]
    public void TestSimpleAmount()
    {
        string json = "-5000";
        var tallyAmount = JsonSerializer.Deserialize<TallyAmount>(json, jsonSerializerOptions);
        Assert.Multiple(() =>
        {
            Assert.That(tallyAmount.Amount, Is.EqualTo(5000));
            Assert.That(tallyAmount.IsDebit, Is.EqualTo(true));
        });
    }

    [Test]
    public void ParseCompleAmount()
    {
        string inputJson = "{\"Amount\":0,\"ForexAmount\":5000," +
            "\"RateOfExchange\":20," +
            "\"Currency\":\"$\",\"IsDebit\":false}";

        var amount = JsonSerializer.Deserialize<TallyAmount>(inputJson, jsonSerializerOptions);
        Assert.Multiple(() =>
        {
            Assert.That(amount.Amount, Is.EqualTo(0));
            Assert.That(amount.RateOfExchange, Is.EqualTo(20));
            Assert.That(amount.ForexAmount, Is.EqualTo(5000));
            Assert.That(amount.Currency, Is.EqualTo("$"));
        });
    }

    [Test]
    public void ParseCompleAmountvariant2()
    {
        string inputJson = "{\"Amount\":0," +
            "\"RateOfExchange\":20," +
            "\"Currency\":\"$\",\"IsDebit\":false}";

        var amount = JsonSerializer.Deserialize<TallyAmount>(inputJson, jsonSerializerOptions);
        Assert.Multiple(() =>
        {
            Assert.That(amount.Amount, Is.EqualTo(0));
            Assert.That(amount.RateOfExchange, Is.EqualTo(20));
            Assert.That(amount.Currency, Is.EqualTo("$"));
        });

    }

    [Test]
    public void ParseCompleAmountvariant3()
    {
        string inputJson = "{\"Amount\": 0,\"IsDebit\": false,\"Currency\": null," +
            "\"ForexAmount\": null,\"RateOfExchange\": null}";

        var amount = JsonSerializer.Deserialize<TallyAmount>(inputJson, jsonSerializerOptions);
        Assert.Multiple(() =>
        {
            Assert.That(amount.Amount, Is.EqualTo(0));
            Assert.That(amount.RateOfExchange, Is.EqualTo(20));
            Assert.That(amount.Currency, Is.EqualTo("$"));
        });

    }


}



