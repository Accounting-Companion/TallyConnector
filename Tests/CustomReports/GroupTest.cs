using NUnit.Framework;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TallyConnector.Core.Converters.JSONConverters;
using TallyConnector.Core.Converters.XMLConverterHelpers;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Masters;

namespace Tests.CustomReports;
internal class GroupTest
{
    TallyConnector.Tally Tally = new();
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void CheckGetReportforGroup()
    {
        //ReportField reportField = Tally.CrateTDLReport(typeof(Group));

        //RequestEnvelope requestEnvelope = new(reportField);
        //var xml = requestEnvelope.GetXML();
        Group group = new("Trst", "Parent");
        string xml = group.GetXML();
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            Converters =
            {
                new TallyYesNoValueConverter(),
                new TallyDateJsonConverter(),
            }
        };
        string json = JsonSerializer.Serialize(group, jsonSerializerOptions);
        Assert.IsNotNull(xml);
    }
}

public class TGroup : Group, IXmlSerializable
{
    public TGroup()
    {
    }

    public TGroup(string name, string parent) : base(name, parent)
    {
    }

    [XmlElement(ElementName = "CALCULABLE")]
    public TallyYesNo Calculable { get; set; }

    public XmlSchema GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        while (reader.Read())
        {

        }
    }

    public void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }
}
