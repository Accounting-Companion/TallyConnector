﻿using TallyConnector.Abstractions.Models;
using TallyConnector.Models.Base.Meta;
using TallyConnector.Models.TallyPrime.V6;
using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Services.TallyPrime.V6;

namespace TallyConnector.Tests.Services.TallyPrime.V6;
public class TallyPrimeServiceV6Tests
{
    private readonly TallyPrimeService primeService;
    public TallyPrimeServiceV6Tests()
    {
        primeService = new TallyPrimeService();
        //primeService.SetupTallyService("http://localhost", 9001);
    }
    [Test]
    public async Task TestGetLedgerAsync()
    {
        //var ledgers = await primeService.GetObjectsAsync<Ledger>();
        var c = new Vucheta();
        var scsa = c.Ledgers.Amount;
        var scds = c.Ledgers.As<AllLedgerEntryMeta>().Amount;
    }
}
public class Vucheta
{
    public CustomMeta Ledgers => new("LedgerEntries_fe", pathPrefix: "6476");
}
public class CustomMeta : AllLedgerEntryMeta, IMultiXMLMeta
{
    Dictionary<string, AllLedgerEntryMeta> _metas => new()
        {
            { "name", this},
        };
    public CustomMeta(string name, string? xmlTag = null, string pathPrefix = "") : base(name, xmlTag, pathPrefix)
    {

    }
    public new List<string> FetchText => [..base.FetchText,];
    public T As<T>() where T : MetaObject
    {
        var name = typeof(T).FullName;
        _metas.TryGetValue(name!, out var value);
        if (value is T castedMeta)
        {
            return castedMeta;
        }
        throw new Exception($"{name} is not valid type");
    }
}
public interface IMultiXMLMeta
{
    public T As<T>() where T : MetaObject;
}