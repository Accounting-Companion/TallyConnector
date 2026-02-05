# Tally Connector  [![NuGet Version](https://img.shields.io/nuget/v/TallyConnector.svg?style=flat)](https://www.nuget.org/packages/TallyConnector/)

![builtwithlove](https://forthebadge.com/images/badges/built-with-love.svg)
![opensource](https://forthebadge.com/images/badges/open-source.svg)
![c#](https://forthebadge.com/images/badges/made-with-c-sharp.svg)


**TallyConnector** is a C# library that serves as a bridge to the Tally XML API.

With **TallyConnector**, you can say goodbye to the complexities of creating and parsing XMLs. Instead, you interact directly with C# objects, making your coding experience smoother and more efficient.

Experience the power of abstraction and let **TallyConnector** handle the heavy lifting of XML manipulation for you!


You can use **[Tally Connector](https://github.com/saivineeth100/TallyConnector/)** to Integrate your desktop/Mobile Applications with Tally.

## Supported Tally Versions
1. Tally Prime 7
2. Tally Prime 6
3. Tally Prime 5
4. Tally Prime 4
___
## Getting Started
1. [Getting Started with C#](https://github.com/Accounting-Companion/TallyConnector_Samples/blob/master/1.GettingStrated.ipynb)
2. [Getting Started with Python](https://github.com/saivineeth100/Python_Tally/blob/main/Tally.ipynb)
3. [Documentation](https://docs.accountingcompanion.com/tally-connector)

### Example Usage
```csharp
using TallyConnector.Services;
using TallyConnector.Models.TallyPrime.V6;
using TallyConnector.Core.Models.Request;

// Instantiate the service
TallyPrimeService primeService = new();

// Setup connection (default is localhost:9000)
primeService.Setup("http://localhost", 9000);

// Fetch Ledgers
var ledgers = await primeService.GetLedgersAsync();
```
___

## Supported Environments

1. .Net Core 8.0 | .Net Core 9.0 | .Net Core 10.0
3. Visual Basic (No Source Generator Support)

___

## 🚀 Release Notes

### [v3.0.0](v3_CHANGELOG.md)
- **Auto TDL Report Generation**: Automatic creation of TDL reports using source generators.
- **Improved Performance**: Fetch only required fields from Tally for faster operations.


___

## 📁 What's included?

| Name| Framework |
| --- | --- |
| **TallyConnector.Abstractions**  <br/>Contains Abstractions and Interfaces | ![NET8](https://img.shields.io/badge/.NET-8.0-green)  ![NET9](https://img.shields.io/badge/.NET-9.0-orange) ![NET10](https://img.shields.io/badge/.NET-10.0-blue) |
| **TallyConnector.Core**  <br/>Core Library that used in TallyConnector.<br/> Contains Attributes, Constants | ![NET8](https://img.shields.io/badge/.NET-8.0-green)  ![NET9](https://img.shields.io/badge/.NET-9.0-orange) ![NET10](https://img.shields.io/badge/.NET-10.0-blue) |
| **TallyConnector.Models**  <br/>Contains Models that are used in TallyConnector | ![NET8](https://img.shields.io/badge/.NET-8.0-green)  ![NET9](https://img.shields.io/badge/.NET-9.0-orange) ![NET10](https://img.shields.io/badge/.NET-10.0-blue) |
| **TallyConnector.TDLReportSourceGenerator**  <br/>Contains source generators and analyzers <br/> | ![NET8](https://img.shields.io/badge/.NET-8.0-green)  ![NET9](https://img.shields.io/badge/.NET-9.0-orange) ![NET10](https://img.shields.io/badge/.NET-10.0-blue)|
| **TallyConnector**  <br/>Main library to interact with Tally Erp9 / Tally prime using C# objects  | ![NET8](https://img.shields.io/badge/.NET-8.0-green)  ![NET9](https://img.shields.io/badge/.NET-9.0-orange) ![NET10](https://img.shields.io/badge/.NET-10.0-blue)|
___

## Other Useful Resources Related to Tally Integration

Xmls used under the hood are listed here - [PostMan Collection](https://documenter.getpostman.com/view/13855108/TzeRpAMt)

StackOverflow threads answered by me  - [StackOverflow Answers](https://stackoverflow.com/search?tab=newest&q=user%3a13605609%20%5btally%5d)

TDLExpert.com threads answered by me - [TDLexpert.com](http://tdlexpert.com/index.php?members/sai-vineeth.12412/)
