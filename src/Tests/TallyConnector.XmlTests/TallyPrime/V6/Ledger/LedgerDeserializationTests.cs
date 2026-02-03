using V6Ledger = TallyConnector.Models.TallyPrime.V6.Masters.Ledger;

namespace TallyConnector.XmlTests.TallyPrime.V6.Ledger;

[TestFixture]
public class LedgerDeserializationTests : XmlTestBase
{
    private V6Ledger ledger = null!;

    [SetUp]
    public void SetUp()
    {
        var xml = File.ReadAllText(GetResourcePath("ledger_complete.xml"));
        ledger = XmlTestHelper.ParseXml<V6Ledger>(xml);
    }

    protected override string ResourceSubPath => "TallyPrime/V6/Ledger";

    [Test]
    public void Test_BasicProperties()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ledger, Is.Not.Null);
            Assert.That(ledger.Name, Is.EqualTo("Test Party Ledger"));
            Assert.That(ledger.Alias, Is.EqualTo("TPL"));
            Assert.That(ledger.Group, Is.EqualTo("Sundry Debtors"));
            Assert.That(ledger.RemoteId, Is.EqualTo("52889497-5b6b-403d-8f83-224e3c7759b4"));
            Assert.That(ledger.OpeningBalance?.Amount, Is.EqualTo(50000m));
            Assert.That(ledger.OpeningBalance?.IsDebit, Is.True);
            Assert.That(ledger.IsBillWise, Is.True);
            Assert.That(ledger.IsCostCentresOn, Is.True);
            Assert.That(ledger.IsInterestOn, Is.True);
            Assert.That(ledger.EMail, Is.EqualTo("test@example.com"));
            Assert.That(ledger.PANNumber, Is.EqualTo("ABCDE1234F"));
        }
        ;
    }

    [Test]
    public void Test_MailingDetails()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ledger.MailingDetails, Has.Count.GreaterThan(0));
            Assert.That(ledger.MailingDetails?[0].MailingName, Is.EqualTo("Test Party Ltd"));
            Assert.That(ledger.MailingDetails?[0].Country, Is.EqualTo("India"));
            Assert.That(ledger.MailingDetails?[0].State, Is.EqualTo("Karnataka"));
            Assert.That(ledger.MailingDetails?[0].PINCode, Is.EqualTo("560001"));
            Assert.That(ledger.MailingDetails?[0].AdressLines, Has.Count.EqualTo(2));
            Assert.That(ledger.MailingDetails?[0].AdressLines?[0], Is.EqualTo("123 Main Street"));
        }
        ;
    }

    [Test]
    public void Test_GSTRegistrationDetails()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ledger.GSTRegistrationDetails, Has.Count.GreaterThan(0));
            Assert.That(ledger.GSTRegistrationDetails?[0].GSTRegistrationType, Is.EqualTo(TallyConnector.Models.Common.GSTRegistrationType.Regular));
            Assert.That(ledger.GSTRegistrationDetails?[0].State, Is.EqualTo("Karnataka"));
            Assert.That(ledger.GSTRegistrationDetails?[0].PlaceOfSupply, Is.EqualTo("Karnataka"));
        }
        ;
    }

    [Test]
    public void Test_ContactDetails()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ledger.ContactDetails, Has.Count.GreaterThan(0));
            Assert.That(ledger.ContactDetails?[0].Name, Is.EqualTo("Primary Contact"));
            Assert.That(ledger.ContactDetails?[0].PhoneNumber, Is.EqualTo("9876543210"));
            Assert.That(ledger.ContactDetails?[0].CountryISOCode, Is.EqualTo("+91"));
        }
        ;
    }

    [Test]
    public void Test_GSTDetails()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ledger.GSTDetails, Has.Count.GreaterThan(0));
            Assert.That(ledger.GSTDetails?[0].Taxability, Is.EqualTo(TallyConnector.Models.Common.GSTTaxabilityType.Taxable));
            Assert.That(ledger.GSTDetails?[0].SourceOfGSTDetails, Is.EqualTo("As per Company/Group"));

            // Nested StateWiseDetails
            Assert.That(ledger.GSTDetails?[0].StateWiseDetails, Has.Count.GreaterThan(0));
            Assert.That(ledger.GSTDetails?[0].StateWiseDetails?[0].StateName, Is.EqualTo("Karnataka"));

            // Nested RateDetails
            Assert.That(ledger.GSTDetails?[0].StateWiseDetails?[0].GSTRateDetails, Has.Count.EqualTo(2));
            Assert.That(ledger.GSTDetails?[0].StateWiseDetails?[0].GSTRateDetails?[0].DutyHead, Is.EqualTo("CGST"));
            Assert.That(ledger.GSTDetails?[0].StateWiseDetails?[0].GSTRateDetails?[0].GSTRate, Is.EqualTo(9f));
            Assert.That(ledger.GSTDetails?[0].StateWiseDetails?[0].GSTRateDetails?[1].DutyHead, Is.EqualTo("SGST/UTGST"));
        }
        ;
    }

    [Test]
    public void Test_HSNDetails()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ledger.HSNDetails, Has.Count.GreaterThan(0));
            Assert.That(ledger.HSNDetails?[0].HSNCode, Is.EqualTo("9971"));
            Assert.That(ledger.HSNDetails?[0].HSNDescription, Is.EqualTo("Financial Services"));
            Assert.That(ledger.HSNDetails?[0].HSNClassificationName, Is.EqualTo("GST"));
        }
        ;
    }

    [Test]
    public void Test_Addresses()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ledger.Addresses, Has.Count.GreaterThan(0));
            Assert.That(ledger.Addresses?[0].AddressName, Is.EqualTo("Head Office"));
            Assert.That(ledger.Addresses?[0].State, Is.EqualTo("Karnataka"));
            Assert.That(ledger.Addresses?[0].Country, Is.EqualTo("India"));
            Assert.That(ledger.Addresses?[0].PinCode, Is.EqualTo("560001"));
            Assert.That(ledger.Addresses?[0].GSTDealerType, Is.EqualTo(TallyConnector.Models.Common.GSTRegistrationType.Regular));
            Assert.That(ledger.Addresses?[0].AddressLines, Has.Count.EqualTo(2));
            Assert.That(ledger.Addresses?[0].AddressLines?[0], Is.EqualTo("456 Corporate Tower"));
        }
        ;
    }
}
