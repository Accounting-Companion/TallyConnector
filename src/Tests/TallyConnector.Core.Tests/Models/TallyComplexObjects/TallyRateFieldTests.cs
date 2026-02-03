using NUnit.Framework;
using TallyConnector.Core.Models.TallyComplexObjects;

namespace TallyConnector.Core.Tests.Models.TallyComplexObjects;

[TestFixture]
public class TallyRateFieldTests
{
    [Test]
    public void ToString_SimpleRate_ReturnsFormattedString()
    {
        var field = new TallyRateField(10.5m, "Nos");
        Assert.That(field.ToString(), Is.EqualTo("10.5/Nos"));
    }

    [Test]
    public void ToString_ForexRate_ReturnsFormattedString()
    {
        var field = new TallyRateField(750m, "Nos", 10m);
        // Code: $"{ForexRate} = {Rate}/{Unit}"
        Assert.That(field.ToString(), Is.EqualTo("10 = 750/Nos"));
    }

    [Test]
    public void ToString_WithAmountContext_ReturnsDetailedString()
    {
        var rateField = new TallyRateField(750m, "Nos", 10m);
        var amountField = new TallyAmountField
        {
            ForexCurrency = "$",
            Currency = "INR",
            RateOfExchange = 75m
        };

        // Code: $"{amount.ForexCurrency}{ForexRate} = {amount.Currency} {Rate}/{Unit}"
        // Expected: "$10 = INR 750/Nos"
        Assert.That(rateField.ToString(amountField), Is.EqualTo("$10 = INR 750/Nos"));
    }
    
    [Test]
    public void ToString_WithAmountContext_NoForex_ReturnsSimpleString()
    {
        var rateField = new TallyRateField(10.5m, "Nos");
        var amountField = new TallyAmountField { Amount = 100 }; // No Forex info

        Assert.That(rateField.ToString(amountField), Is.EqualTo("10.5/Nos"));
    }
}
