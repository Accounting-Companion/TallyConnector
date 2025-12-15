using TallyConnector.MigrationTests.Infrastructure;
using TallyConnector.MigrationTests.Models;

namespace TallyConnector.MigrationTests.Tests;

/// <summary>
/// Tests for basic type serialization comparison between XmlSerializer and XmlSourceGenerator
/// </summary>
[TestFixture]
public class BasicTypesSerializationTests
{
    [Test]
    public void SimpleObject_SerializesIdentically()
    {
        // Arrange
        var testObj = new SimpleTestObject
        {
            Name = "Test Object",
            Id = 42,
            Amount = 1234.56m,
            IsActive = true,
            CreatedOn = new DateTime(2024, 1, 15, 10, 30, 0),
            OptionalValue = 100
        };

        // Act - Get XML using current approach (TallyXml.GetXML)
        string currentXml = testObj.GetXML(indent: true);

        // Act - Get XML using XmlSourceGenerator approach
        var generatedXml = testObj.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }

    [Test]
    public void SimpleObject_WithNullOptionalValue_SerializesIdentically()
    {
        // Arrange
        var testObj = new SimpleTestObject
        {
            Name = "Test Object",
            Id = 42,
            Amount = 1234.56m,
            IsActive = false,
            CreatedOn = new DateTime(2024, 1, 15),
            OptionalValue = null
        };

        // Act
        string currentXml = testObj.GetXML(indent: true);
        var generatedXml = testObj.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }

    [Test]
    public void SimpleObject_WithNullString_SerializesIdentically()
    {
        // Arrange
        var testObj = new SimpleTestObject
        {
            Name = null,
            Id = 1,
            Amount = 0m,
            IsActive = false,
            CreatedOn = DateTime.Now,
            OptionalValue = null
        };

        // Act
        string currentXml = testObj.GetXML(indent: true);
        var generatedXml = testObj.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }

    [Test]
    public void DecimalValues_SerializeIdentically()
    {
        // Arrange - Test various decimal values
        var testValues = new[] { 0m, 1.5m, 1234.56m, -100.25m, 0.001m };

        foreach (var value in testValues)
        {
            var testObj = new SimpleTestObject
            {
                Name = "Decimal Test",
                Id = 1,
                Amount = value,
                IsActive = true,
                CreatedOn = DateTime.Now
            };

            // Act
            string currentXml = testObj.GetXML(indent: true);
            var generatedXml = testObj.WriteToXml();
            string sourceGeneratorXml = generatedXml.ToString();

            // Assert
            bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
            Assert.That(areEqual, Is.True, $"XML outputs differ for value {value}:\n{difference}");
        }
    }

    [Test]
    public void BooleanValues_SerializeIdentically()
    {
        // Test both true and false
        foreach (var boolValue in new[] { true, false })
        {
            var testObj = new SimpleTestObject
            {
                Name = "Bool Test",
                Id = 1,
                Amount = 0m,
                IsActive = boolValue,
                CreatedOn = DateTime.Now
            };

            // Act
            string currentXml = testObj.GetXML(indent: true);
            var generatedXml = testObj.WriteToXml();
            string sourceGeneratorXml = generatedXml.ToString();

            // Assert
            bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
            Assert.That(areEqual, Is.True, $"XML outputs differ for boolean value {boolValue}:\n{difference}");
        }
    }
}
