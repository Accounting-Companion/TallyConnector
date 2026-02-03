using NUnit.Framework;
using TallyConnector.Core.Models.TallyComplexObjects;

namespace TallyConnector.Core.Tests.Models.TallyComplexObjects;

[TestFixture]
public class TallyAmountFieldTests
{
    [Test]
    public void ToString_SimpleAmount_ReturnsFormattedString()
    {
        var field = new TallyAmountField(100.50m, false);
        Assert.That(field.ToString(), Is.EqualTo("100.50")); // Uses CultureInfo.InvariantCulture
    }

    [Test]
    public void ToString_DebitAmount_ReturnsNegativeString()
    {
        var field = new TallyAmountField(100.50m, true);
        Assert.That(field.ToString(), Is.EqualTo("-100.50"));
    }

    [Test]
    public void ToString_ForexAmount_ReturnsCompositeString()
    {
        var field = new TallyAmountField
        {
            Amount = 1500m,
            Currency = "INR",
            ForexAmount = 20m,
            ForexCurrency = "$",
            RateOfExchange = 75m
        };
        // Expected: "$20 @ 75/$ = INR 1500"
        Assert.That(field.ToString(), Is.EqualTo("$20 @ 75/$ = INR 1500"));
    }

    [Test]
    public void ToString_WithFormat_ReturnsFormattedString()
    {
        var field = new TallyAmountField(1000m, false);
        Assert.That(field.ToString("N2"), Is.EqualTo("1,000.00"));
    }

    [Test]
    public void ToString_WithFormat_Debit_ReturnsFormattedNegativeString()
    {
        var field = new TallyAmountField(1000m, true);
        Assert.That(field.ToString("N2"), Is.EqualTo("-1,000.00"));
    }
}
