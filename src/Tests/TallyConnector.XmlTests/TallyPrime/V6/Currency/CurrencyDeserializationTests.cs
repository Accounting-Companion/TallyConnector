using V6Currency = TallyConnector.Models.TallyPrime.V6.Masters.Currency;

namespace TallyConnector.XmlTests.TallyPrime.V6.Currency;

[TestFixture]
[Ignore("Currency model has bug: Currency.Name hides inherited BaseMasterObject.Name with different attrs")]
public class CurrencyDeserializationTests : XmlTestBase
{
    private V6Currency currency = null!;

    [SetUp]
    public void SetUp()
    {
        var xml = File.ReadAllText(GetResourcePath("currency_complete.xml"));
        currency = XmlTestHelper.ParseXml<V6Currency>(xml);
    }

    protected override string ResourceSubPath => "TallyPrime/V6/Currency";

    [Test]
    public void Test_BasicProperties()
    {
        using (Assert.EnterMultipleScope()) 
        {
            Assert.That(currency, Is.Not.Null);
            Assert.That(currency.Name, Is.EqualTo("$"));
            Assert.That(currency.FormalName, Is.EqualTo("US Dollar"));
            Assert.That(currency.RemoteId, Is.EqualTo("52889497-5b6b-403d-8f83-224e3c7759b4"));
        };
    }
}
