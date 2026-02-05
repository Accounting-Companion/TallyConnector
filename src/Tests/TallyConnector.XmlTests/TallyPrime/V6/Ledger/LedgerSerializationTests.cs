using NUnit.Framework;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.TallyComplexObjects;
using TallyConnector.Models.Base.Masters;
using TallyConnector.Models.Common;
using TallyConnector.Services.TallyPrime.V6;
using v6 = TallyConnector.Models.TallyPrime.V6;

namespace TallyConnector.XmlTests.TallyPrime.V6.Ledger;

[TestFixture]
public class LedgerSerializationTests : XmlTestBase
{
    private XMLOverrideswithTracking overides;

    public LedgerSerializationTests()
    {
       overides = new TallyPrimeService().GetPostXMLOverrides() ?? new();
    }

    protected override string ResourceSubPath => "TallyPrime/V6/Ledger";

    [Test]
    public void Test_BasicProperties()
    {
        // Arrange
        var ledger = new v6.Masters.Ledger
        {
            Name = "New Ledger",
            Group = "Indirect Expenses",
            OpeningBalance = new TallyAmountField(0, false),
            RemoteId = "190dc6e4-9f01-4250-9ce3-e075a5512de4-AC_TC"
        };

        // Act
        TallyObjectDTO obj = ledger.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(doc.Root?.Name.LocalName, Is.EqualTo("LEDGER"));
            Assert.That(doc.Root?.Attribute("NAME")?.Value, Is.EqualTo("New Ledger"));
            Assert.That(doc.Root?.Element("PARENT")?.Value, Is.EqualTo("Indirect Expenses"));
            Assert.That(doc.Root?.Element("OPENINGBALANCE")?.Value, Is.EqualTo("0"));
        });
        Assert.That(xml, Is.EqualTo($"<LEDGER ACTION=\"Create\" NAME=\"New Ledger\">\r\n  <REMOTEALTGUID>{obj.RemoteId}</REMOTEALTGUID>\r\n  <LANGUAGENAME.LIST>\r\n    <NAME.LIST>\r\n      <NAME>New Ledger</NAME>\r\n    </NAME.LIST>\r\n  </LANGUAGENAME.LIST>\r\n  <PARENT>Indirect Expenses</PARENT>\r\n  <OPENINGBALANCE>0</OPENINGBALANCE>\r\n  <RATEOFTAXCALCULATION />\r\n</LEDGER>"));
    }

    [Test]
    public void Test_MailingDetails()
    {
        // Arrange
        var ledger = new v6.Masters.Ledger
        {
            Name = "Party Ledger",
            Group = "Sundry Debtors",
            RemoteId = "test-mailing-001",
            MailingDetails =
            [
                new MailingDetail
                {
                    MailingName = "Test Company Ltd",
                    Country = "India",
                    State = "Karnataka",
                    PINCode = "560001",
                    AdressLines = ["123 Main Street", "Bangalore"]
                }
            ]
        };

        // Act
        TallyObjectDTO obj = ledger.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        var mailingDetails = doc.Root?.Element("LEDMAILINGDETAILS.LIST");
        Assert.Multiple(() =>
        {
            Assert.That(mailingDetails, Is.Not.Null);
            Assert.That(mailingDetails?.Element("MAILINGNAME")?.Value, Is.EqualTo("Test Company Ltd"));
            Assert.That(mailingDetails?.Element("COUNTRY")?.Value, Is.EqualTo("India"));
            Assert.That(mailingDetails?.Element("STATE")?.Value, Is.EqualTo("Karnataka"));
        });
    }

    [Test]
    public void Test_GSTRegistrationDetails()
    {
        // Arrange
        var ledger = new v6.Masters.Ledger
        {
            Name = "GST Party",
            Group = "Sundry Debtors",
            RemoteId = "test-gst-001",
            GSTRegistrationDetails =
            [
                new LedgerGSTRegistrationDetail
                {
                    GSTRegistrationType = GSTRegistrationType.Regular,
                    State = "Karnataka",
                    PlaceOfSupply = "Karnataka",
                    GSTIN = "29ABCDE1234F1Z5"
                }
            ]
        };

        // Act
        TallyObjectDTO obj = ledger.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        var gstDetails = doc.Root?.Element("LEDGSTREGDETAILS.LIST");
        Assert.Multiple(() =>
        {
            Assert.That(gstDetails, Is.Not.Null);
            Assert.That(gstDetails?.Element("GSTREGISTRATIONTYPE")?.Value, Is.EqualTo("Regular"));
            Assert.That(gstDetails?.Element("STATE")?.Value, Is.EqualTo("Karnataka"));
            Assert.That(gstDetails?.Element("GSTIN")?.Value, Is.EqualTo("29ABCDE1234F1Z5"));
        });
    }

    [Test]
    public void Test_Addresses()
    {
        // Arrange
        var ledger = new v6.Masters.Ledger
        {
            Name = "Multi Address Party",
            Group = "Sundry Debtors",
            RemoteId = "test-address-001",
            Addresses =
            [
                new MultiAddress
                {
                    AddressName = "Head Office",
                    Country = "India",
                    State = "Karnataka",
                    PinCode = "560001",
                    AddressLines = ["456 Corporate Tower", "MG Road"]
                }
            ]
        };

        // Act
        TallyObjectDTO obj = ledger.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        var address = doc.Root?.Element("LEDMULTIADDRESSLIST.LIST");
        Assert.Multiple(() =>
        {
            Assert.That(address, Is.Not.Null);
            Assert.That(address?.Element("MAILINGNAME")?.Value, Is.EqualTo("Head Office"));
            Assert.That(address?.Element("COUNTRYNAME")?.Value, Is.EqualTo("India"));
        });
    }

    [Test]
    public void Test_HSNDetails()
    {
        // Arrange
        var ledger = new v6.Masters.Ledger
        {
            Name = "HSN Ledger",
            Group = "Sales Accounts",
            RemoteId = "test-hsn-001",
            HSNDetails =
            [
                new HSNDetail
                {
                    HSNCode = "9971",
                    HSNDescription = "Financial Services",
                    HSNClassificationName = "GST"
                }
            ]
        };

        // Act
        TallyObjectDTO obj = ledger.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        var hsnDetails = doc.Root?.Element("HSNDETAILS.LIST");
        Assert.Multiple(() =>
        {
            Assert.That(hsnDetails, Is.Not.Null);
            Assert.That(hsnDetails?.Element("HSNCODE")?.Value, Is.EqualTo("9971"));
            Assert.That(hsnDetails?.Element("HSN")?.Value, Is.EqualTo("Financial Services"));
        });
    }

}
