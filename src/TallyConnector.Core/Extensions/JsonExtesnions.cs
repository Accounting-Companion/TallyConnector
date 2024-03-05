using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Masters;
using TallyConnector.Core.Models.Masters.CostCenter;
using TallyConnector.Core.Models.Masters.Inventory;
using TallyConnector.Core.Models.Masters.Payroll;

namespace TallyConnector.Core.Extensions;
public static class JsonExtesnions
{
    /// <summary>
    /// Supports conversion of objects of type T to json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <returns></returns>
    public static string ToJson<T>(this IEnumerable<T> list, JsonSerializerOptions? jsonSerializerOptions = null) where T : TallyXmlJson
    {
        jsonSerializerOptions ??= new() { Converters = { new JsonStringEnumConverter() } };
        JsonContext jsonContext = jsonSerializerOptions ==null ? JsonContext.Default : new(jsonSerializerOptions);
        string result = JsonSerializer.Serialize(list, typeof(IEnumerable<T>), jsonContext);
        return result;
    }
    /// <inheritdoc cref="ToJson{T}(IEnumerable{T}, JsonSerializerOptions?)"/>
    public static string ToJson<T>(this List<T> list, JsonSerializerOptions? jsonSerializerOptions = null) where T : TallyXmlJson
    {
        jsonSerializerOptions ??= new() { Converters = { new JsonStringEnumConverter() } };
        JsonContext jsonContext = jsonSerializerOptions == null ? JsonContext.Default : new(jsonSerializerOptions);
        string result = JsonSerializer.Serialize(list, typeof(List<T>), jsonContext);
        return result;
    }

    public static IEnumerable<T>? FromJson<T>(this string json, JsonSerializerOptions? jsonSerializerOptions = null) where T : TallyXmlJson
    {
        jsonSerializerOptions ??= new() { Converters = { new JsonStringEnumConverter() } };
        JsonContext jsonContext = jsonSerializerOptions == null ? JsonContext.Default : new(jsonSerializerOptions);
        IEnumerable<T>? result = (IEnumerable<T>?)JsonSerializer.Deserialize(json, typeof(IEnumerable<T>), jsonContext);
        return result;
    }
}

[JsonSerializable(typeof(BaseCompany))]
[JsonSerializable(typeof(BaseCompany[]))]
[JsonSerializable(typeof(List<BaseCompany>))]
[JsonSerializable(typeof(IEnumerable<BaseCompany>))]

[JsonSerializable(typeof(Company))]
[JsonSerializable(typeof(Company[]))]
[JsonSerializable(typeof(List<Company>))]
[JsonSerializable(typeof(IEnumerable<Company>))]


[JsonSerializable(typeof(Currency))]
[JsonSerializable(typeof(Currency[]))]
[JsonSerializable(typeof(List<Currency>))]
[JsonSerializable(typeof(IEnumerable<Currency>))]

[JsonSerializable(typeof(Group))]
[JsonSerializable(typeof(Group[]))]
[JsonSerializable(typeof(List<Group>))]
[JsonSerializable(typeof(IEnumerable<Group>))]

[JsonSerializable(typeof(Ledger))]
[JsonSerializable(typeof(Ledger[]))]
[JsonSerializable(typeof(List<Ledger>))]
[JsonSerializable(typeof(IEnumerable<Ledger>))]


[JsonSerializable(typeof(CostCategory))]
[JsonSerializable(typeof(CostCategory[]))]
[JsonSerializable(typeof(List<CostCategory>))]
[JsonSerializable(typeof(IEnumerable<CostCategory>))]

[JsonSerializable(typeof(CostCentre))]
[JsonSerializable(typeof(CostCentre[]))]
[JsonSerializable(typeof(List<CostCentre>))]
[JsonSerializable(typeof(IEnumerable<CostCentre>))]


[JsonSerializable(typeof(VoucherType))]
[JsonSerializable(typeof(VoucherType[]))]
[JsonSerializable(typeof(List<VoucherType>))]
[JsonSerializable(typeof(IEnumerable<VoucherType>))]

//[JsonSerializable(typeof(Voucher))]
//[JsonSerializable(typeof(Voucher[]))]
//[JsonSerializable(typeof(List<Voucher>))]
//[JsonSerializable(typeof(IEnumerable<Voucher>))]


[JsonSerializable(typeof(Unit))]
[JsonSerializable(typeof(Unit[]))]
[JsonSerializable(typeof(List<Unit>))]
[JsonSerializable(typeof(IEnumerable<Unit>))]

[JsonSerializable(typeof(Godown))]
[JsonSerializable(typeof(Godown[]))]
[JsonSerializable(typeof(List<Godown>))]
[JsonSerializable(typeof(IEnumerable<Godown>))]

[JsonSerializable(typeof(StockCategory))]
[JsonSerializable(typeof(StockCategory[]))]
[JsonSerializable(typeof(List<StockCategory>))]
[JsonSerializable(typeof(IEnumerable<StockCategory>))]

[JsonSerializable(typeof(StockGroup))]
[JsonSerializable(typeof(StockGroup[]))]
[JsonSerializable(typeof(List<StockGroup>))]
[JsonSerializable(typeof(IEnumerable<StockGroup>))]

[JsonSerializable(typeof(StockItem))]
[JsonSerializable(typeof(StockItem[]))]
[JsonSerializable(typeof(List<StockItem>))]
[JsonSerializable(typeof(IEnumerable<StockItem>))]

[JsonSerializable(typeof(AttendanceType))]
[JsonSerializable(typeof(AttendanceType[]))]
[JsonSerializable(typeof(List<AttendanceType>))]
[JsonSerializable(typeof(IEnumerable<AttendanceType>))]

[JsonSerializable(typeof(EmployeeGroup[]))]
[JsonSerializable(typeof(EmployeeGroup[]))]
[JsonSerializable(typeof(List<EmployeeGroup>))]
[JsonSerializable(typeof(IEnumerable<EmployeeGroup>))]

[JsonSerializable(typeof(Employee))]
[JsonSerializable(typeof(Employee[]))]
[JsonSerializable(typeof(List<Employee>))]
[JsonSerializable(typeof(IEnumerable<Employee>))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]

internal partial class JsonContext : JsonSerializerContext
{

}
