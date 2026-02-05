using V6Group = TallyConnector.Models.TallyPrime.V6.Masters.Group;

namespace TallyConnector.XmlTests.TallyPrime.V6.Group;

[TestFixture]
public class GroupDeserializationTests : XmlTestBase
{
    private V6Group group = null!;

    [SetUp]
    public void SetUp()
    {
        var xml = File.ReadAllText(GetResourcePath("group_complete.xml"));
        group = XmlTestHelper.ParseXml<V6Group>(xml);
    }

    protected override string ResourceSubPath => "TallyPrime/V6/Group";

    [Test]
    public void Test_BasicProperties()
    {
        using (Assert.EnterMultipleScope()) 
        {
            Assert.That(group, Is.Not.Null);
            Assert.That(group.Name, Is.EqualTo("Bank Accounts"));
            Assert.That(group.Alias, Is.EqualTo("BA"));
            Assert.That(group.RemoteId, Is.EqualTo("52889497-5b6b-403d-8f83-224e3c7759b4"));
            Assert.That(group.Parent, Is.EqualTo("Current Assets"));
            Assert.That(group.ReservedName, Is.EqualTo("Bank Accounts"));
        };
    }

    [Test]
    public void Test_BooleanFlags()
    {
        using (Assert.EnterMultipleScope()) 
        {
            Assert.That(group.IsRevenue, Is.False);
            Assert.That(group.IsDeemedPositive, Is.True);
            Assert.That(group.AffectGrossProfit, Is.False);
            Assert.That(group.IsSubledger, Is.False);
            Assert.That(group.IsCalculable, Is.False);
            Assert.That(group.IsAddable, Is.False);
        };
    }

    [Test]
    public void Test_NumericProperties()
    {
        Assert.That(group.SortPosition, Is.EqualTo(220));
    }
}
