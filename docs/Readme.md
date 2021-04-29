# Welcome to Tally Connector

You can use **[Tally Connector](https://github.com/saivineeth100/TallyConnector/)** to Connect your desktop/Mobile Applications with Tally seamlessly.

## Supported Tally Versions

1. Tally Prime
2. Tally ERP9

This Library acts as complete abstraction for Tally XML API,
You don't need to known or understand tally XML to interact with Tally.

This Repository has seperate branches for .Net Core 5.0 and .NET Framework 4.7.2 main branch is for .Net Core

1. [Tally Connector for .NET Core](https://github.com/saivineeth100/TallyConnector/tree/master)
2. [Tally Connector for .Net Framework](https://github.com/saivineeth100/TallyConnector/tree/NET-Framework)

___

## Getting Started

Add reference to Tallyconnector.dll in Your Project  
Intiatate Tally in your Project

### For C#

```C#
Using TallyConnector //Importing TallyConnector Library

//public Tally = new Tally("http://localhost",9000); --You can Specify url and port on which tally is running
public Ctally = new Tally(); //If Nothing is specified default url is localhost running on port 9000

//We can also Setup default Configuration using Setup method - Once setup you no need to explicitly send these through each methods
Ctally.Setup(URL,Port,CompName,fromDate,toDate); //URL and port are mandatory Fields 

//Check() Returns true if tally is running
public bool status = await Ctally.Check(); // To check Whether Tally is running on Given url and port. 

//GetCompaniesList() Returns List of companies opened in Tally
List<Company> CompaniesList = await  Ctally.GetCompaniesList();

//FetchAllTallyData() will get all tally masters like Groups/Ledgers ...etc., in Tally.Groups,Tally.Ledgers lists
await Ctally.FetchAllTallyData(CompName);


//To Get Full Object from Tally use Specific methods like GetGroup, GetLedger, GetCostCategory,GetCostCenter ..etc.,
//For Ex. For getting Group by name
Group TGrp = await GetGroup(GroupName);

//To Create/Alter/Delete/Cancel Group,Ledger,Voucher from Tally use Specific methods like PostGroup, PostLedger, PostCostCategory,PostCostCenter ..etc.,
//For Ex. To create group
await PostGroup(Group);
//For Ex. To Delete group we need to Set Group.Action to Delete and use the same method
await PostGroup(Group); 
```
