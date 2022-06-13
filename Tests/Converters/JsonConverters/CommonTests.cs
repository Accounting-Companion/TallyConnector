using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        Assert.AreEqual(json, "\"accountingVoucherView\"");
    }
    [Test]
    public void TestEnumDeSerialization()
    {
        string json = "\"AccountingVoucherView\"";
        var res = JsonSerializer.Deserialize<TCM.VoucherViewType>(json, jsonSerializerOptions);
        Assert.AreEqual(res, TCM.VoucherViewType.AccountingVoucherView);
    }
}
