using NUnit.Framework;
using V6CostCentre = TallyConnector.Models.TallyPrime.V6.Masters.CostCentre;

namespace TallyConnector.XmlTests.TallyPrime.V6.CostCentre;

[TestFixture]
public class CostCentreDeserializationTests : XmlTestBase
{
    private V6CostCentre costCentre = null!;

    [SetUp]
    public void SetUp()
    {
        var xml = File.ReadAllText(GetResourcePath("costcentre_complete.xml"));
        costCentre = XmlTestHelper.ParseXml<V6CostCentre>(xml);
    }

    protected override string ResourceSubPath => "TallyPrime/V6/CostCentre";

    [Test]
    public void Test_BasicProperties()
    {
        using (Assert.EnterMultipleScope()) 
        {
            Assert.That(costCentre, Is.Not.Null);
            Assert.That(costCentre.Name, Is.EqualTo("Test Cost Centre"));
            Assert.That(costCentre.Alias, Is.EqualTo("TCC"));
            Assert.That(costCentre.RemoteId, Is.EqualTo("52889497-5b6b-403d-8f83-224e3c7759b4"));
            Assert.That(costCentre.Category, Is.EqualTo("Primary Cost Category"));
            Assert.That(costCentre.Parent, Is.EqualTo("Primary"));
            Assert.That(costCentre.ShowOpeningBal, Is.False);
        };
    }
}
