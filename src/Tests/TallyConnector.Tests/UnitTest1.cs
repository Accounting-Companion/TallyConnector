using System.Xml.Serialization;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Common.Request;
using TallyConnector.Services;

namespace TallyConnector.Tests;

public class Tests
{
    TallyService tc;
    System.Xml.Serialization.XmlSerializer xmlSerializer;
    public Tests()
    {
        tc = new();
        xmlSerializer = new(typeof(RequestEnvelope));
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test1()
    {
        RequestEnvelope value = new();
        value.Header = new() { Id = "dzfd", Type = "Collection" };
        value.Body = new() { Description = new() { TDL = new() { Reports = new() { new() { Name = "cvgbh" }, new() { Name = "asdfr" } } ,Fields=new() { new() { Name="sdfg"} } } } };
        string v = await tc.GetRequestEnvelopeXMLAsync(value);

    }
    [Test]
    public async Task Test2()
    {
        RequestEnvelope value = new();
        value.Header = new() { Id = "dzfd", Type = "Collection" };
        using StringWriter stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, value);
        var k = stringWriter.ToString();
    }
}