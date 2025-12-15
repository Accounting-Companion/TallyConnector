using TallyConnector.MigrationTests.Models;

namespace TallyConnector.MigrationTests.Tests;

[TestFixture]
public class PolymorphicXmlElementTests
{
    [Test]
    public void PolymorphicXmlElement_SerializesCorrectly()
    {
        var voucher = new PolymorphicVoucher
        {
            LedgerEntries = new List<BaseLedgerEntry>
            {
                new AllLedgerEntry { Amount = 100, ExtraInfo = "Extra", Id = 999 },
                new LedgerEntry { Amount = 200, Priority = 1 }
            }
        };

        // Act - Write using XmlSourceGenerator
        var element = ((XmlSourceGenerator.Abstractions.IXmlStreamable)voucher).WriteToXml();
        
        // Assert
        var elements = element.Elements().ToList();
        Assert.That(elements.Count, Is.EqualTo(2));
        
        Assert.That(elements[0].Name.LocalName, Is.EqualTo("ALLLEDGERENTRIES.LIST"));
        Assert.That(elements[0].Element("Amount")?.Value, Is.EqualTo("100"));
        Assert.That(elements[0].Element("ExtraInfo")?.Value, Is.EqualTo("Extra"));
        Assert.That(elements[0].Element("Id"), Is.Null); // Verify ignored
        
        Assert.That(elements[1].Name.LocalName, Is.EqualTo("LEDGERENTRIES.LIST"));
        Assert.That(elements[1].Element("Amount")?.Value, Is.EqualTo("200"));
        Assert.That(elements[1].Element("Priority")?.Value, Is.EqualTo("1"));
    }

    [Test]
    public void PolymorphicXmlElement_DeserializesCorrectly()
    {
        string xml = @"<Voucher>
  <ALLLEDGERENTRIES.LIST>
    <Amount>100</Amount>
    <ExtraInfo>Extra</ExtraInfo>
    <Id>123</Id>
  </ALLLEDGERENTRIES.LIST>
  <LEDGERENTRIES.LIST>
    <Amount>200</Amount>
    <Priority>1</Priority>
  </LEDGERENTRIES.LIST>
</Voucher>";

        var element = System.Xml.Linq.XElement.Parse(xml);
        var voucher = new PolymorphicVoucher();
        ((XmlSourceGenerator.Abstractions.IXmlStreamable)voucher).ReadFromXml(element);

        Assert.That(voucher.LedgerEntries, Is.Not.Null);
        Assert.That(voucher.LedgerEntries.Count, Is.EqualTo(2));
        
        Assert.That(voucher.LedgerEntries[0], Is.InstanceOf<AllLedgerEntry>());
        Assert.That(voucher.LedgerEntries[0].Amount, Is.EqualTo(100));
        Assert.That(((AllLedgerEntry)voucher.LedgerEntries[0]).ExtraInfo, Is.EqualTo("Extra"));
        Assert.That(((AllLedgerEntry)voucher.LedgerEntries[0]).Id, Is.EqualTo(0)); // Verify ignored (not read)
        
        Assert.That(voucher.LedgerEntries[1], Is.InstanceOf<LedgerEntry>());
        Assert.That(voucher.LedgerEntries[1].Amount, Is.EqualTo(200));
        Assert.That(((LedgerEntry)voucher.LedgerEntries[1]).Priority, Is.EqualTo(1));
    }
}
