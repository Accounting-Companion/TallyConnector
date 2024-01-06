using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using TallyConnector.Core.Attributes;
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

    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameLis>? LanguageNameList { get; set; }
}
[XmlRoot(ElementName = "LANGUAGENAME.LIST")]
[TDLCollection(CollectionName = "LanguageName")]
public partial class LanguageNameLis
{
    public LanguageNameLis()
    {
        NameList = new();

    }
    [XmlIgnore]
    public string? LanguageAlias
    {
        get { return NameList.NAMES?.Count > 1 ? string.Join("..\n", NameList.NAMES.GetRange(1, NameList.NAMES.Count - 1)) : null; }
        set
        {
            if (NameList.NAMES?.Count > 1)
            {
                NameList.NAMES.RemoveRange(1, NameList.NAMES.Count - 1);
                NameList.NAMES.InsertRange(1, value?.Split("..\n".ToCharArray()).ToList()!);
            }
            else if (NameList.NAMES?.Count == 1)
            {
                NameList.NAMES.InsertRange(1, value?.Split("..\n".ToCharArray()).ToList()!);
            }
        }
    }

    [XmlElement(ElementName = "NAME.LIST")]
    public NamesL NameList { get; set; }

    //[XmlElement(ElementName = "LANGUAGEID")]
    //public LANGUAGEID LANGUAGEID { get; set; }
}
[XmlRoot(ElementName = "NAME.LIST")]

public partial class NamesL
{
    public NamesL()
    {
        NAMES = new();
    }

    [XmlElement(ElementName = "NAME")]
    [TDLCollection(CollectionName = "Name")]
    public List<string>? NAMES { get; set; }
    //public List<int>? Tests { get; set; }

    //[XmlAttribute(AttributeName = "TYPE")]
    //public string TYPE { get; set; }

    //[XmlText]
    //public string Text { get; set; }
}
public partial class Voucher : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = "VOUCHERNUMBER")]
    public string? VoucherNumber { get; set; }
    [XmlElement(ElementName = "PERSISTEDVIEW")]
    public string? PersistedView { get; set; }

    [TDLCollection(CollectionName = "LedgerEntries")]
    public List<LedgerEntry> LedgerEntries { get; set; }

    [TDLCollection(CollectionName = "ALLINVENTORYENTRIES")]
    public List<InventoryEntry> InventoryEntries { get; set; }


}
public partial class LedgerEntry
{
    [XmlElement(ElementName = "LEDGERNAME")]
    public string? LedgerName { get; set; }
}
public partial class InventoryEntry
{
    [XmlElement(ElementName = "STOCKITEMNAME")]
    public string? StockItemName { get; set; }

    [TDLCollection(CollectionName = "ACCOUNTINGALLOCATIONS")]
    public List<LedgerEntry> LedgerEntries { get; set; }
}