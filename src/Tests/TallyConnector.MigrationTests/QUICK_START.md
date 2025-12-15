# Migration Tests - Quick Start

## Summary

Created **TallyConnector.MigrationTests** project to compare XML serialization between:
- **Current**: `TallyXml.GetXML()` using `XmlSerializer`  
- **New**: XmlSourceGenerator's generated `WriteToXml()` method

## Project Location

```
\\saivineeth\d\SourceCode\AccountingCompanion\TallyConnector_New\src\Tests\TallyConnector.MigrationTests\
```

## What to Do Next

### 1. Build the Project

The project has been added to `TallyConnector.sln`. You can build two ways:

**Option A - Visual Studio:**
1. Reload the solution (if already open)
2. Build the `TallyConnector.MigrationTests` project

**Option B - Command Line:**
```bash
cd \\saivineeth\d\SourceCode\AccountingCompanion\TallyConnector_New
dotnet build src\Tests\TallyConnector.MigrationTests
```

### 2. Run the Tests

```bash
cd \\saivineeth\d\SourceCode\AccountingCompanion\TallyConnector_New\src\Tests\TallyConnector.MigrationTests
dotnet test
```

Or use Visual Studio's Test Explorer.

## What the Tests Do

Each test:
1. Creates a test model with sample data
2. Serializes using `GetXML()` (current XmlSerializer)
3. Serializes using `WriteToXml()` (XmlSourceGenerator)
4. Compares the XML outputs semantically
5. Asserts they're equivalent

## Test Coverage (22 tests)

- **BasicTypesSerializationTests** (5 tests) - string, int, decimal, bool, DateTime, nullables
- **AttributeMappingTests** (3 tests) - XML attributes vs elements
- **CollectionSerializationTests** (5 tests) - Lists, arrays, nested objects
- **RealWorldModelTests** (5 tests) - Company, Voucher with LedgerEntries

## Important Notes

⚠️ **Dual Attributes**: Test models use BOTH attribute sets:
- `System.Xml.Serialization` attributes (for XmlSerializer)
- `SourceGeneratorUtils` attributes (for XmlSourceGenerator)

This allows the same model to work with both approaches.

## Troubleshooting

If the build  fails, check:
1. XmlSourceGenerator.Abstractions reference path is correct
2. XmlSourceGenerator analyzer is properly referenced
3. All dependencies are restored (`dotnet restore`)

## For Questions

See `README.md` in the project folder for more details.
