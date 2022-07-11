using System.Text.Json.Serialization;

namespace Tests.Converters.JsonConverters;
internal class CommonTests
{
    JsonSerializerOptions jsonSerializerOptions;
    public CommonTests()
    {
        jsonSerializerOptions = new();
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    [Test]
    public void TestEnumSerialization()
    {
        TCM.VoucherViewType voucherViewType = TCM.VoucherViewType.AccountingVoucherView;
        var json = JsonSerializer.Serialize(voucherViewType, jsonSerializerOptions);
        Assert.That(json, Is.EqualTo("\"accountingVoucherView\""));
    }
    [Test]
    public void TestEnumDeSerialization()
    {
        string json = "\"AccountingVoucherView\"";
        var res = JsonSerializer.Deserialize<TCM.VoucherViewType>(json, jsonSerializerOptions);
        Assert.That(res, Is.EqualTo(TCM.VoucherViewType.AccountingVoucherView));
    }
}
