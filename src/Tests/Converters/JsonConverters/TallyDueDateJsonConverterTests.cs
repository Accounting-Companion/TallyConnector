namespace Tests.Converters.JsonConverters;
public class TallyDueDateJsonConverterTests
{
    JsonSerializerOptions jsonSerializerOptions;
    public TallyDueDateJsonConverterTests()
    {
        jsonSerializerOptions = new();
        //jsonSerializerOptions.PropertyNamingPolicy =  JsonNamingPolicy.CamelCase;
    }

    [Test]
    [TestCase(DueDateFormat.Day, 5)]
    [TestCase(DueDateFormat.Month, 10)]
    [TestCase(DueDateFormat.Year, 1)]
    [TestCase(DueDateFormat.Week, 2)]
    public void TestDeSerializeTallyDueDate(DueDateFormat dueDateFormat, int value)
    {
        string json = $"{{\"Suffix\":\"{dueDateFormat}\",\"Value\":{value}}}";
        TallyDueDate tallyDueDate = JsonSerializer.Deserialize<TallyDueDate>(json, jsonSerializerOptions);
        Assert.Multiple(() =>
        {
            Assert.That(tallyDueDate.Value, Is.EqualTo(value));
            Assert.That(tallyDueDate.Suffix, Is.EqualTo(dueDateFormat));
        });
    }

    [Test]
    public void TestSerializeTallyDueDate()
    {
        TallyDueDate tallyDate = DateTime.Now;
        string json = JsonSerializer.Serialize(tallyDate, jsonSerializerOptions);
        Assert.That(json, Is.EqualTo($"{{\"BillDate\":\"{DateTime.Now:dd-MM-yyyy}\",\"DueDate\":\"30-10-2023\",\"Suffix\":\"\",\"Value\":0}}"));
    }
    //[Test]
    //[TestCase(5, DueDateFormat.Day, 5)]
    //[TestCase(7, DueDateFormat.Week, 1)]
    //[TestCase(30, DueDateFormat.Month, 1)]
    //public void TestSerializeTallyDateVariant(int days, DueDateFormat dueDateFormat, int Val)
    //{
    //    DateTime dateTime = DateTime.Now;
    //    TallyDueDate tallyDate = new(dateTime, dateTime.AddDays(days), dueDateFormat);
    //    string json = JsonSerializer.Serialize(tallyDate, jsonSerializerOptions);
    //    Assert.That(json, Is.EqualTo($"{{\"DueDate\":\"{dateTime.AddDays(days):dd-MM-yyyy}\",\"Suffix\":\"{dueDateFormat}\",\"Value\":{Val}}}"));
    //}
}
