using TallyConnector.MigrationTests.Infrastructure;
using TallyConnector.MigrationTests.Models;

namespace TallyConnector.MigrationTests.Tests;

/// <summary>
/// Tests for real-world model serialization based on TallyConnector.Models
/// </summary>
[TestFixture]
public class RealWorldModelTests
{
    [Test]
    public void Company_SerializesIdentically()
    {
        // Arrange
        var company = new TestCompany
        {
            Name = "Test Company Pvt Ltd",
            GUID = "12345678-1234-1234-1234-123456789012",
            BooksFrom = new DateTime(2024, 4, 1),
            State = "Karnataka",
            PinCode = "560001",
            Email = "test@company.com",
            IsInventoryOn = true,
            IsGSTOn = true
        };

        // Act
        string currentXml = company.GetXML(indent: true);
        var generatedXml = company.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }

    [Test]
    public void Voucher_WithLedgerEntries_SerializesIdentically()
    {
        // Arrange
        var voucher = new TestVoucher
        {
            Date = new DateTime(2024, 11, 30),
            VoucherType = "Sales",
            VoucherNumber = "INV-001",
            PartyName = "ABC Customer",
            Narration = "Test sale transaction",
            LedgerEntries = new List<TestLedgerEntry>
            {
                new TestLedgerEntry
                {
                    LedgerName = "Sales Account",
                    IsDeemedPositive = false,
                    Amount = 10000m
                },
                new TestLedgerEntry
                {
                    LedgerName = "Party Account",
                    IsDeemedPositive = true,
                    Amount = 10000m
                }
            }
        };

        // Act
        string currentXml = voucher.GetXML(indent: true);
        var generatedXml = voucher.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }

    [Test]
    public void Voucher_WithoutLedgerEntries_SerializesIdentically()
    {
        // Arrange
        var voucher = new TestVoucher
        {
            Date = new DateTime(2024, 11, 30),
            VoucherType = "Receipt",
            VoucherNumber = "RCP-001",
            PartyName = "XYZ Customer",
            Narration = null,
            LedgerEntries = null
        };

        // Act
        string currentXml = voucher.GetXML(indent: true);
        var generatedXml = voucher.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }

    [Test]
    public void ComplexVoucher_WithMultipleLedgerEntries_SerializesIdentically()
    {
        // Arrange - Complex voucher with multiple ledger entries
        var voucher = new TestVoucher
        {
            Date = DateTime.Today,
            VoucherType = "Journal",
            VoucherNumber = "JV-100",
            PartyName = "Multiple Entries Test",
            Narration = "Complex journal entry with multiple ledgers",
            LedgerEntries = new List<TestLedgerEntry>
            {
                new TestLedgerEntry { LedgerName = "Cash", IsDeemedPositive = true, Amount = 5000m },
                new TestLedgerEntry { LedgerName = "Bank", IsDeemedPositive = true, Amount = 3000m },
                new TestLedgerEntry { LedgerName = "Sales", IsDeemedPositive = false, Amount = 8000m }
            }
        };

        // Act
        string currentXml = voucher.GetXML(indent: true);
        var generatedXml = voucher.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }

    [Test]
    public void Company_WithMinimalData_SerializesIdentically()
    {
        // Arrange
        var company = new TestCompany
        {
            Name = "Minimal Company",
            GUID = "00000000-0000-0000-0000-000000000000",
            BooksFrom = DateTime.Now,
            IsInventoryOn = false,
            IsGSTOn = false
        };

        // Act
        string currentXml = company.GetXML(indent: true);
        var generatedXml = company.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }
}
