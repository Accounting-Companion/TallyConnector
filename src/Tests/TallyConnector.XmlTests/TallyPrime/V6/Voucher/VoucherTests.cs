using TallyConnector.Core.Models;
using TallyConnector.Core.Models.TallyComplexObjects;
using TallyConnector.Models.Base;
using TallyConnector.Models.TallyPrime.V6;
using V6Voucher = TallyConnector.Models.TallyPrime.V6.Voucher;

namespace TallyConnector.XmlTests.TallyPrime.V6.Voucher;

[TestFixture]
public class VoucherTests : XmlTestBase
{
    protected override string ResourceSubPath => "TallyPrime/V6/Voucher";

    [Test]
    public void Test_Voucher_Deserialization()
    {
        // Arrange
        var xml = File.ReadAllText(GetResourcePath("voucher_sample.xml"));

        // Act
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(voucher, Is.Not.Null);
            Assert.That(voucher.Date, Is.EqualTo(new DateTime(2023, 4, 1)));
            Assert.That(voucher.VoucherType, Is.EqualTo("Sales"));
            Assert.That(voucher.VoucherNumber, Is.EqualTo("001"));
            Assert.That(voucher.PartyName, Is.EqualTo("Test Party"));
            Assert.That(voucher.Narration, Is.EqualTo("Sample Voucher for Testing"));
            Assert.That(voucher.LedgerEntries, Has.Count.EqualTo(2));
            Assert.That(voucher.LedgerEntries[0].LedgerName, Is.EqualTo("Test Party"));
            Assert.That(voucher.LedgerEntries[0].Amount.Amount, Is.EqualTo(-1000.00m));
        });
    }

    [Test]
    public void Test_Voucher_Serialization()
    {
        // Arrange
        var voucher = new V6Voucher
        {
            Date = new DateTime(2023, 5, 1),
            VoucherType = "Receipt",
            VoucherNumber = "R-001",
            PartyName = "Cash",
            Narration = "Test Serialization",
            LedgerEntries = new List<AllLedgerEntry>
            {
                new AllLedgerEntry { LedgerName = "Cash", Amount = new TallyAmountField(500, true), IsDeemedPositive = true },
                new AllLedgerEntry { LedgerName = "Party", Amount = new TallyAmountField(500, false), IsDeemedPositive = false }
            }
        };

        // Act
        var xml = XmlTestHelper.GenerateXml(voucher.ToDTO());
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert - Use XDocument to verify specific elements
        Assert.Multiple(() =>
        {
            Assert.That(doc.Root?.Name.LocalName, Is.EqualTo("VOUCHER"));
            Assert.That(doc.Root?.Element("VOUCHERTYPENAME")?.Value, Is.EqualTo("Receipt"));
            Assert.That(doc.Root?.Element("VOUCHERNUMBER")?.Value, Is.EqualTo("R-001"));
            Assert.That(doc.Root?.Element("PARTYNAME")?.Value, Is.EqualTo("Cash"));
            Assert.That(doc.Root?.Element("NARRATION")?.Value, Is.EqualTo("Test Serialization"));
        });
    }
}
