namespace Tests.Converters.JsonConverters;
internal class TallyDateJsonConverterTests
{
    JsonSerializerOptions jsonSerializerOptions;
    public TallyDateJsonConverterTests()
    {
        jsonSerializerOptions = new();
        jsonSerializerOptions.Converters.Add(new TallyDateJsonConverter());
    }
    [Test]
    public void TestSerializeTallyDate()
    {
        TallyDate tallyDate = DateTime.Now;
        string json = JsonSerializer.Serialize(tallyDate, jsonSerializerOptions);
        Assert.That(json, Is.EqualTo($"\"{((DateTime)tallyDate).ToString("dd-MM-yyyy")}\""));
    }
    [Test]
    public void TestSerializeTallyDatewhenNull()
    {
        TallyDate tallyDate = null;
        string json = JsonSerializer.Serialize(tallyDate, jsonSerializerOptions);
        Assert.That(json, Is.EqualTo("null"));
    }
    [Test]
    public void TestDeSerializeTallyDate()
    {
        var Date = DateTime.Now;

        string dateJson = $"\"{Date:dd-MM-yyyy}\"";
        TallyDate date = JsonSerializer.Deserialize<TallyDate>(dateJson, jsonSerializerOptions);
        Assert.That(date.ToString("dd-MM-yyyy"), Is.EqualTo(Date.ToString("dd-MM-yyyy")));
    }
    [Test]
    public void TestDeSerializeTallyDateWhenNull()
    {
        var Date = DateTime.Now;

        string dateJson = $"\"\"";
        TallyDate date = JsonSerializer.Deserialize<TallyDate>(dateJson, jsonSerializerOptions);
        Assert.That(date, Is.EqualTo(null));
    }

}
