using TallyConnector.MigrationTests.Infrastructure;
using TallyConnector.MigrationTests.Models;

namespace TallyConnector.MigrationTests.Tests;

/// <summary>
/// Tests for collection serialization comparison
/// </summary>
[TestFixture]
public class CollectionSerializationTests
{
    [Test]
    public void StringCollection_SerializesIdentically()
    {
        // Arrange
        var testObj = new CollectionTestObject
        {
            Name = "Collection Test",
            Items = new List<string> { "Item1", "Item2", "Item3" },
            Numbers = new List<int> { 1, 2, 3, 4, 5 }
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
    public void EmptyCollection_SerializesIdentically()
    {
        // Arrange
        var testObj = new CollectionTestObject
        {
            Name = "Empty Collection Test",
            Items = new List<string>(),
            Numbers = new List<int>()
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
    public void NullCollection_SerializesIdentically()
    {
        // Arrange
        var testObj = new CollectionTestObject
        {
            Name = "Null Collection Test",
            Items = null,
            Numbers = null
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
    public void NestedObjectCollection_SerializesIdentically()
    {
        // Arrange
        var testObj = new NestedTestObject
        {
            Name = "Parent Object",
            NestedItems = new List<NestedItem>
            {
                new NestedItem { ItemName = "Item 1", Quantity = 10.5m },
                new NestedItem { ItemName = "Item 2", Quantity = 20.75m },
                new NestedItem { ItemName = "Item 3", Quantity = 5.0m }
            }
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
    public void LargeCollection_SerializesIdentically()
    {
        // Arrange - Test with a larger collection
        var items = Enumerable.Range(1, 50).Select(i => $"Item_{i}").ToList();
        var numbers = Enumerable.Range(1, 50).ToList();

        var testObj = new CollectionTestObject
        {
            Name = "Large Collection Test",
            Items = items,
            Numbers = numbers
        };

        // Act
        string currentXml = testObj.GetXML(indent: true);
        var generatedXml = testObj.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }
}
