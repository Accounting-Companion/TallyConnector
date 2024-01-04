using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using TallyConnector.Core.Models;

namespace IntegrationTests;

[TestClass]
public class BasicIntegrationTests
{
    [TestMethod]
    public void BasicFieldsTest()
    {
        var fields = Group.GetTDLFields();
        Assert.IsNotNull(fields);
        Assert.AreEqual(1, fields.Length);
    }
    [TestMethod]
    public void BasicEnvTest()
    {
        TallyConnector.Core.Models.RequestEnvelope requestEnvelope = Group.GetRequestEnevelope();
        //requestEnvelope.Body.Desc.TDL.TDLMessage.Report = [new()];
        //requestEnvelope.Body.Desc.TDL.TDLMessage.Field = [.. Group.GetTDLFields()];
        Assert.IsNotNull(requestEnvelope);
        string v = requestEnvelope.GetXML();
        Assert.AreEqual(TallyConnector.Core.Models.HType.Data, requestEnvelope.Header!.Type);
    } 
    [TestMethod]
    public void ComplexModelTest()
    {
        TallyConnector.Core.Models.RequestEnvelope requestEnvelope = Voucher.GetRequestEnevelope();
        var c = Voucher.ReportName;
        Assert.IsNotNull(requestEnvelope);
        string v = requestEnvelope.GetXML();
        Assert.AreEqual(TallyConnector.Core.Models.HType.Data, requestEnvelope.Header!.Type);
    }
}

public partial class Group : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }


}

public partial class Voucher : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = "VOUCHERNUMBER")]
    public string? VoucherNumber { get; set; }

    public List<LedgerEntry> LedgerEntries { get; set; }


}
public partial class LedgerEntry
{
    [XmlElement(ElementName = "LEDGERNAME")]
    public string? LedgerName { get; set; }
}