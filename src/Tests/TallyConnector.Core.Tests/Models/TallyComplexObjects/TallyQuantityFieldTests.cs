using NUnit.Framework;
using System.Globalization;
using TallyConnector.Core.Models.TallyComplexObjects;

namespace TallyConnector.Core.Tests.Models.TallyComplexObjects;

[TestFixture]
public class TallyQuantityFieldTests
{
    [TestCase("300", "Box of 10 Nos", "30 Box 0 Nos")]
    [TestCase("2545", "Box of 100 strips of 10 tablets", "2 Box 54 strips 5 tablets")]
    [TestCase("10000", "Box of 100 strips of 10 tablets", "10 Box 0 strips 0 tablets")]
    [TestCase("37", "Box of 12 Pcs", "3 Box 1 Pcs")]
    [TestCase("50", "Nos", "50 Nos")]
    [TestCase("100", "", "100")]
    [TestCase("0", "Box of 10 Nos", "0 Box 0 Nos")]

    [TestCase("221", "Carton of 10 Box of 20 Pcs", "1 Carton 1 Box 1 Pcs")]
    [TestCase("21", "Carton of 10 Box of 20 Pcs", "0 Carton 1 Box 1 Pcs")]
    [TestCase("-305", "Box of 10 Nos", "-30 Box 5 Nos")]
    [TestCase("100", "Box OF 10 Nos", "10 Box 0 Nos")]
    [TestCase("50", "InvalidUnit", "50 InvalidUnit")]
    [TestCase("50", "Box 10 Nos", "50 Box 10 Nos")]

    [TestCase("5", "Box of 10 Nos", "0 Box 5 Nos")]
    [TestCase("200", "Carton of 10 Box of 20 Pcs", "1 Carton 0 Box 0 Pcs")]
    [TestCase("220", "Carton of 10 Box of 20 Pcs", "1 Carton 1 Box 0 Pcs")]
    [TestCase("205", "Carton of 10 Box of 20 Pcs", "1 Carton 0 Box 5 Pcs")]
    [TestCase("10.5", "Box of 10 Nos", "1 Box 0.5 Nos")]
    [TestCase("1450", "Crate of 10 Carton of 10 Box of 10 Nos", "1 Crate 4 Carton 5 Box 0 Nos")]
    [TestCase("144", "Gross of 12 Dozen of 12 Nos", "1 Gross 0 Dozen 0 Nos")]
    [TestCase("13", "Dozen of 12 Nos", "1 Dozen 1 Nos")]
    [TestCase("1500", "Ton of 1000 Kgs", "1 Ton 500 Kgs")]
    [TestCase("1000500", "Ton of 1000 Kg of 1000 g", "1 Ton 0 Kg 500 g")]

    [TestCase("-305.5", "Box of 10 Nos", "-30 Box 5.5 Nos")]
    [TestCase("-0.5", "Box of 10 Nos", "-0 Box 0.5 Nos")]
    [TestCase("-1000", "Ton of 1000 Kgs", "-1 Ton 0 Kgs")]
    [TestCase("-1450", "Crate of 10 Carton of 10 Box of 10 Nos", "-1 Crate 4 Carton 5 Box 0 Nos")]
    [TestCase("0.99", "Box of 10 Nos", "0 Box 0.99 Nos")]
    [TestCase("-0.99", "Box of 10 Nos", "-0 Box 0.99 Nos")]
    public void ToString_ReturnsFormattedString(string quantityStr, string unit, string expected)
    {
        // Arrange
        decimal quantity = decimal.Parse(quantityStr, CultureInfo.InvariantCulture);
        var field = new TallyQuantityField(quantity, unit);

        // Act
        var result = field.ToString();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToString_WithZeroQuantity_ReturnsCorrectUnit()
    {
        // Special check for zero behavior
        var field = new TallyQuantityField(0, "Box of 10 Nos");
        var result = field.ToString();
        Assert.That(result, Is.EqualTo("0 Box 0 Nos"));
    }
    
    [Test]
    public void ToString_WithComplexDecimal_ReturnsFormatted()
    {
        var field = new TallyQuantityField(300.5m, "Box of 10 Nos");
        var result = field.ToString();
        Assert.That(result, Is.EqualTo("30 Box 0.5 Nos"));
    }

}
