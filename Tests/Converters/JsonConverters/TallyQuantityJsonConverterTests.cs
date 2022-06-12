namespace Tests.Converters.JsonConverters;
public class TallyQuantityJsonConverterTests
{

    [Test]
    public void TestSerializeTallyQantitywhenNull()
    {
        TallyQuantity tallyQuantity = null;
        string json = JsonSerializer.Serialize(tallyQuantity);
        Assert.AreEqual(json, "null");
    }
    [Test]
    public void TestSerializeTallyQantitywhenEmpty()
    {
        TallyQuantity tallyQuantity = new();
        string json = JsonSerializer.Serialize(tallyQuantity);
        Assert.AreEqual(json, "{\"Number\":0,\"PrimaryUnits\":null,\"SecondaryUnits\":null}");
    }
    [Test]
    public void TestSerializeTallyQantity()
    {
        TallyQuantity tallyQuantity = new(50, "Nos");
        string json = JsonSerializer.Serialize(tallyQuantity);
        Assert.AreEqual(json, "{\"Number\":50,\"PrimaryUnits\":{\"Number\":50,\"Unit\":\"Nos\"},\"SecondaryUnits\":null}");
    }
    [Test]
    public void TestSerializeTallyQantitywithSecondaryUnit()
    {
        TallyQuantity tallyQuantity = new(50, "Nos", 10, "Box");
        string json = JsonSerializer.Serialize(tallyQuantity);
        Assert.AreEqual(json, "{\"Number\":50,\"PrimaryUnits\":{\"Number\":50,\"Unit\":\"Nos\"},\"SecondaryUnits\":{\"Number\":10,\"Unit\":\"Box\"}}");
    }
    [Test]
    public void TestDeSerializeTallyQantitywhenNull()
    {
        string json = "null";
        var tallyQuantity = JsonSerializer.Deserialize<TallyQuantity>(json);
        Assert.AreEqual(tallyQuantity, null);
    }
    [Test]
    public void TestDeSerializeTallyQantitywhenEmpty()
    {
        string json = "{\"Number\":0,\"PrimaryUnits\":null,\"SecondaryUnits\":null}";
        var tallyQuantity = JsonSerializer.Deserialize<TallyQuantity>(json);
        Assert.AreEqual(tallyQuantity, null);
    }[Test]
    public void TestDeSerializeTallyQantitywhenEmptyVariant2()
    {
        string json = "{\"PrimaryUnits\":null,\"SecondaryUnits\":null}";
        var tallyQuantity = JsonSerializer.Deserialize<TallyQuantity>(json);
        Assert.AreEqual(tallyQuantity, null);
    }
    [Test]
    public void TestDeSerializeTallyQantity()
    {
        string inJson = "{\"Number\":50,\"PrimaryUnits\":{\"Number\":50,\"Unit\":\"Nos\"},\"SecondaryUnits\":null}";

        TallyQuantity tallyAmount = JsonSerializer.Deserialize<TallyQuantity>(inJson);

        Assert.AreEqual(tallyAmount.Number, 50);
        Assert.AreEqual(tallyAmount.PrimaryUnits.Number, 50);
        Assert.AreEqual(tallyAmount.PrimaryUnits.Unit, "Nos");
        Assert.AreEqual(tallyAmount.SecondaryUnits, null);

    }

    [Test]
    public void TestDeSerializeTallyQantitywithSecondaryUnit()
    {
        string inJson = "{\"Number\":50,\"PrimaryUnits\":{\"Number\":50,\"Unit\":\"Nos\"},\"SecondaryUnits\":{\"Number\":10,\"Unit\":\"Box\"}}";

        TallyQuantity tallyAmount = JsonSerializer.Deserialize<TallyQuantity>(inJson);

        Assert.AreEqual(tallyAmount.Number, 50);
        Assert.AreEqual(tallyAmount.PrimaryUnits.Number, 50);
        Assert.AreEqual(tallyAmount.PrimaryUnits.Unit, "Nos");
        Assert.AreEqual(tallyAmount.SecondaryUnits.Number, 10);
        Assert.AreEqual(tallyAmount.SecondaryUnits.Unit, "Box");

    }
}
