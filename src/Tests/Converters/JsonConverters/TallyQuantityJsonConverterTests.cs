namespace Tests.Converters.JsonConverters;
public class TallyQuantityJsonConverterTests
{

    [Test]
    public void TestSerializeTallyQantitywhenNull()
    {
        TallyQuantity tallyQuantity = null;
        string json = JsonSerializer.Serialize(tallyQuantity);
        Assert.That(json, Is.EqualTo("null"));
    }
    [Test]
    public void TestSerializeTallyQantitywhenEmpty()
    {
        TallyQuantity tallyQuantity = new();
        string json = JsonSerializer.Serialize(tallyQuantity);
        Assert.That(json,
            Is.EqualTo("{\"PrimaryUnits\":null,\"SecondaryUnits\":null}"));
    }
    [Test]
    public void TestSerializeTallyQantity()
    {
        TallyQuantity tallyQuantity = new(50, "Nos");
        string json = JsonSerializer.Serialize(tallyQuantity);
        Assert.That(json,
            Is.EqualTo("{\"PrimaryUnits\":{\"Number\":50,\"Unit\":\"Nos\"},\"SecondaryUnits\":null}"));
    }
    [Test]
    public void TestSerializeTallyQantitywithSecondaryUnit()
    {
        TallyQuantity tallyQuantity = new(50, "Nos", 10, "Box");
        string json = JsonSerializer.Serialize(tallyQuantity);
        Assert.That(json,
            Is.EqualTo("{\"PrimaryUnits\":{\"Number\":50,\"Unit\":\"Nos\"},\"SecondaryUnits\":{\"Number\":10,\"Unit\":\"Box\"}}"));
    }
    [Test]
    public void TestDeSerializeTallyQantitywhenNull()
    {
        string json = "null";
        var tallyQuantity = JsonSerializer.Deserialize<TallyQuantity>(json);
        Assert.That(tallyQuantity, Is.EqualTo(null));
    }
    [Test]
    public void TestDeSerializeTallyQantitywhenEmpty()
    {
        string json = "{\"Number\":0,\"PrimaryUnits\":null,\"SecondaryUnits\":null}";
        var tallyQuantity = JsonSerializer.Deserialize<TallyQuantity>(json);
        Assert.That(tallyQuantity, Is.EqualTo(null));
    }
    [Test]
    public void TestDeSerializeTallyQantitywhenEmptyVariant2()
    {
        string json = "{\"PrimaryUnits\":null,\"SecondaryUnits\":null}";
        var tallyQuantity = JsonSerializer.Deserialize<TallyQuantity>(json);
        Assert.That(tallyQuantity, Is.EqualTo(null));
    }
    [Test]
    public void TestDeSerializeTallyQantity()
    {
        string inJson = "{\"Number\":50,\"PrimaryUnits\":{\"Number\":50,\"Unit\":\"Nos\"},\"SecondaryUnits\":null}";

        TallyQuantity tallyAmount = JsonSerializer.Deserialize<TallyQuantity>(inJson);
        
        Assert.Multiple(() =>
        {
            Assert.That(tallyAmount.Number, Is.EqualTo(50));
            Assert.That(tallyAmount.PrimaryUnits.Number, Is.EqualTo(50));
            Assert.That(tallyAmount.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(tallyAmount.SecondaryUnits, Is.EqualTo(null));
        });
    }

    [Test]
    public void TestDeSerializeTallyQantitywithSecondaryUnit()
    {
        string inJson = "{\"Number\":50,\"PrimaryUnits\":{\"Number\":50,\"Unit\":\"Nos\"},\"SecondaryUnits\":{\"Number\":10,\"Unit\":\"Box\"}}";

        TallyQuantity tallyAmount = JsonSerializer.Deserialize<TallyQuantity>(inJson);

        Assert.Multiple(() =>
        {
            Assert.That(tallyAmount.Number, Is.EqualTo(50));
            Assert.That(tallyAmount.PrimaryUnits.Number, Is.EqualTo(50));
            Assert.That(tallyAmount.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(tallyAmount.SecondaryUnits.Number, Is.EqualTo(10));
            Assert.That(tallyAmount.SecondaryUnits.Unit, Is.EqualTo("Box"));
        });
    }
}
