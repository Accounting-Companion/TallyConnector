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
        Assert.AreEqual(json,
            "{\"Amount\":0,\"ForexAmount\":5000,\"RateOfExchange\":20," +
            "\"Currency\":\"$\",\"IsDebit\":false}");
    }

    [Test]
    public void TestSerializeSimpleAmount()
    {
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new TallyAmountJsonConverter(true));
        TallyAmount tallyAmount = new() { Amount = 5000 };
        var json = JsonSerializer.Serialize(tallyAmount, jsonSerializerOptions);
        Assert.AreEqual(json, "5000");
    }
    [Test]
    public void TestSerializeSimpleDebitAmount()
    {
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new TallyAmountJsonConverter(true));
        TallyAmount tallyAmount = new() { Amount = -5000 };
        var json = JsonSerializer.Serialize(tallyAmount, jsonSerializerOptions);
        Assert.AreEqual(json, "-5000");
    }
    [Test]
    public void TestSerializeSimpleAmountWithoutallowingSimple()
    {
        TallyAmount tallyAmount = new() { Amount = 5000 };
        var json = JsonSerializer.Serialize(tallyAmount, jsonSerializerOptions);
        Assert.AreEqual(json, "{\"Amount\":5000,\"ForexAmount\":null,\"RateOfExchange\":null,\"Currency\":null,\"IsDebit\":false}");
    }
    [Test]
    public void TestSerializeSimpleDebitAmountWithoutallowingSimple()
    {
        TallyAmount tallyAmount = new(-5000);
        var json = JsonSerializer.Serialize(tallyAmount, jsonSerializerOptions);
        Assert.AreEqual(json, "{\"Amount\":5000,\"ForexAmount\":null,\"RateOfExchange\":null,\"Currency\":null,\"IsDebit\":true}");
    }
    [Test]
    public void TestSimpleAmount()
    {
        string json = "-5000";
        var tallyAmount = JsonSerializer.Deserialize<TallyAmount>(json, jsonSerializerOptions);
        Assert.AreEqual(tallyAmount.Amount, 5000);
        Assert.AreEqual(tallyAmount.IsDebit, true);
    }

    [Test]
    public void ParseCompleAmount()
    {
        string inputJson = "{\"Amount\":0,\"ForexAmount\":5000," +
            "\"RateOfExchange\":20," +
            "\"Currency\":\"$\",\"IsDebit\":false}";

        var amount = JsonSerializer.Deserialize<TallyAmount>(inputJson, jsonSerializerOptions);
        Assert.AreEqual(amount.Amount, 0);
        Assert.AreEqual(amount.RateOfExchange, 20);
        Assert.AreEqual(amount.ForexAmount, 5000);
        Assert.AreEqual(amount.Currency, "$");
    }

    [Test]
    public void ParseCompleAmountvariant2()
    {
        string inputJson = "{\"Amount\":0," +
            "\"RateOfExchange\":20," +
            "\"Currency\":\"$\",\"IsDebit\":false}";

        var amount = JsonSerializer.Deserialize<TallyAmount>(inputJson, jsonSerializerOptions);
        Assert.AreEqual(amount.Amount, 0);
        Assert.AreEqual(amount.RateOfExchange, 20);
        Assert.AreEqual(amount.Currency, "$");

    }


}



