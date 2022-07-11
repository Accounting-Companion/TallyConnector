namespace Tests.Converters.JsonConverters;
internal class TallyRateJsonConverterTests
{

    [Test]
    public void TestSerializeTallyRatewhenNull()
    {
        TallyRate tallyRate = null;
        string json = JsonSerializer.Serialize(tallyRate);
        Assert.That(json, Is.EqualTo("null"));
    }
    [Test]
    public void TestSerializeTallyRatewhenEmpty()
    {
        TallyRate tallyRate = new();
        string json = JsonSerializer.Serialize(tallyRate);
        Assert.That(json,
            Is.EqualTo("{\"RatePerUnit\":0,\"Unit\":\"\",\"ForexAmount\":0,\"RateOfExchange\":0,\"ForeignCurrency\":null}"));
    }
    [Test]
    public void TestSerializeTallyRate()
    {
        TallyRate tallyRate = new(10, "Nos");
        string json = JsonSerializer.Serialize(tallyRate);
        Assert.That(json,
            Is.EqualTo("{\"RatePerUnit\":10,\"Unit\":\"Nos\",\"ForexAmount\":0,\"RateOfExchange\":0,\"ForeignCurrency\":null}"));
    }
    [Test]
    public void TestSerializeTallyRatewithForex()
    {
        TallyRate tallyRate = new("Nos", 10, 10, "$");
        string json = JsonSerializer.Serialize(tallyRate);
        Assert.That(json,
            Is.EqualTo("{\"RatePerUnit\":100,\"Unit\":\"Nos\",\"ForexAmount\":10,\"RateOfExchange\":10,\"ForeignCurrency\":\"$\"}"));
    }

    [Test]
    public void TestDeSerializeTallyRatewhenNull()
    {
        string json = "null";
        var tallyRate = JsonSerializer.Deserialize<TallyRate>(json);
        Assert.That(tallyRate, Is.EqualTo(null));
    }

    [Test]
    public void TestDeSerializeTallyRatewhenEmpty()
    {
        string json = "{\"RatePerUnit\":0,\"Unit\":\"\",\"ForexAmount\":0,\"RateOfExchange\":0,\"ForeignCurrency\":null}";
        var tallyRate = JsonSerializer.Deserialize<TallyRate>(json);
        Assert.Multiple(() =>
        {
            Assert.That(tallyRate.Unit, Is.EqualTo(string.Empty));
            Assert.That(tallyRate.RatePerUnit, Is.EqualTo(0));
        });
    }

    [Test]
    public void TestDeSerializeTallyRate()
    {
        string json = "{\"RatePerUnit\":10,\"Unit\":\"Nos\"}";
        var tallyRate = JsonSerializer.Deserialize<TallyRate>(json);
        Assert.Multiple(() =>
        {
            Assert.That(tallyRate.Unit, Is.EqualTo("Nos"));
            Assert.That(tallyRate.RatePerUnit, Is.EqualTo(10));
        });
    }

    [Test]
    public void TestDeSerializeTallyRatewithForex()
    {
        string json = "{\"RatePerUnit\":100,\"Unit\":\"Nos\",\"ForexAmount\":10,\"RateOfExchange\":10,\"ForeignCurrency\":\"$\"}";
        var tallyRate = JsonSerializer.Deserialize<TallyRate>(json);

        Assert.Multiple(() =>
        {
            Assert.That(tallyRate.Unit, Is.EqualTo("Nos"));
            Assert.That(tallyRate.RatePerUnit, Is.EqualTo(100));
            Assert.That(tallyRate.RateOfExchange, Is.EqualTo(10));
            Assert.That(tallyRate.ForeignCurrency, Is.EqualTo("$"));
            Assert.That(tallyRate.ForexAmount, Is.EqualTo(10));
        });
    }
}
