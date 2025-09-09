using static NUnit.Framework.Legacy.StringAssert;
namespace TallyConnector.Core.Tests.Requests;
public class FilterBuilderTests
{

    [Test]
    public void TestFieldFilters()
    {
        AreEqualIgnoringCase("$MasterId = 50", FilterBuilder.WithMasterId(c => c.IsEqualTo(50)));
        AreEqualIgnoringCase("$MasterId < 50", FilterBuilder.WithMasterId(c => c.IsLessThan(50)));
        AreEqualIgnoringCase("$MasterId <= 50", FilterBuilder.WithMasterId(c => c.IsLessThanOrEqualTo(50)));

        AreEqualIgnoringCase("$MasterId > 50", FilterBuilder.WithMasterId(c => c.IsGreaterThan(50)));
        AreEqualIgnoringCase("$MasterId >= 50", FilterBuilder.WithMasterId(c => c.IsGreaterThanOrEqualTo(50)));


        AreEqualIgnoringCase("$MASTERID = 50 AND $ALTERID > 20", FilterBuilder.WithMasterId(c => c.IsEqualTo(50))
            .And(FilterBuilder.WithAlterId(c => c.IsGreaterThan(20))));

        AreEqualIgnoringCase("$MASTERID = 50 OR $ALTERID > 20", FilterBuilder.WithMasterId(c => c.IsEqualTo(50))
            .Or(FilterBuilder.WithAlterId(c => c.IsGreaterThan(20))));

    }
}