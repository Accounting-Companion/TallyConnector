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
        //xmlSerializer = new(typeof(RequestEnvelope));
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task TestAsync()
    {
        RequestEnvelope value = new();
        value.Header = new() { Id = "TC_GROUP", Type = "DATA",TallyRequest="Export" ,Version="1"};
      

        value.Body = new();
        value.Body.Description = new();
        value.Body.Description.TDL.TDLMessage = Group.GetTDLMessaget();
        string v = await tc.GetRequestEnvelopeXMLAsync(value);


    }
    [Test]
    public void Test()
    {
        RequestEnvelope value = new();
        value.Header = new() { Id = "dzfd", Type = "Collection" };
        //value.Body = new() { Description = new() { TDL = new() { Reports = new() { new("cvgbh"), new("asdfr") }, Fields = new() { new("sdfg","") } } } };
        //string v = tc.GetRequestEnvelopeXML(value);

    }
    [Test]
    public async Task Test3()
    {
        RequestEnvelope value = new();
        value.Header = new() { Id = "dzfd", Type = "Collection" };
        //value.Body = new() { Description = new() { TDL = new() { Reports = new() { new("cvgbh"), new("asdfr") }, Fields = new() { new("sdfg", "") } } } };
        using StringWriter stringWriter = new();
        xmlSerializer.Serialize(stringWriter, value);
        var k = stringWriter.ToString();
    }
    [Test]
    public async Task Test4()
    {
      //  tc.GetGroupTDLReportFields();
    }
}