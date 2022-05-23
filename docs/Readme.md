# Tally Connector [![NuGet Version](http://img.shields.io/nuget/v/TallyConnector.svg?style=flat)](https://www.nuget.org/packages/TallyConnector/)

You can use **[Tally Connector](https://github.com/saivineeth100/TallyConnector/)** to Integrate your desktop/Mobile Applications with Tally.

## Supported Tally Versions

1. Tally Prime
2. Tally ERP9

## Supported Environments

1. .Net Core 6.0 | .Net Core 5.0
2. .Net Framework 4.8
3. Visual Basic

This Library is complete abstraction for Tally XML API, Using this library
you don't need to known or understand tally XML to interact with Tally.
___

## Getting Started

```shell
Install-Package TallyConnector
```

Intiate Tally in your Project

### For C#

```C#
Using TallyConnector //Importing TallyConnector Library

//public Tally Tally = new Tally("http://localhost",9000); --You can Specify url and port on which tally is running
public Tally Ctally = new Tally(); //If Nothing is specified default url is localhost running on port 9000

//We can also Setup default Configuration using Setup method - Once setup you no need to explicitly send these through each methods
Ctally.Setup(URL,Port,CompName,fromDate,toDate); //URL and port are mandatory Fields 

//Check() Returns true if tally is running
public bool status = await Ctally.Check(); // To check Whether Tally is running on Given url and port. 

//GetCompaniesList() Returns List of companies opened in Tally
List<Company> CompaniesList = await  Ctally.GetCompaniesList();

//FetchAllTallyData() will get all tally masters like Groups/Ledgers ...etc., in Tally.Masters
await Ctally.FetchAllTallyData("ABC Company");
//Ex:
//masterType can be Ledgers,Groups ... or any masters from Tally
string masterType = "Ledgers"
List<BasicTallyObject> LedgerMasters = Ctally.Masters.Find(master => master.MasterType == masterType).Masters;

//To Get Full Object from Tally use Specific methods like GetGroup, GetLedger, GetCostCategory,GetCostCenter ..etc.,
//For Ex. For getting Group by name
Group TGrp = await Ctally.GetGroup<Group>("TestGroup");

//To Create/Alter/Delete/Cancel Group,Ledger,Voucher from Tally use Specific methods like PostGroup, PostLedger, PostCostCategory,PostCostCenter ..etc.,
//For Ex. To create group
PResult result = await Ctally.PostGroup(new Group()
            {
                Name = "TestGroup",
                Parent = "Sundry Debtors",
            });
//For Ex. To Alter group we need to Set Group.Action to Delete and use the same method
PResult result = await Ctally.PostGroup(new Group()
            {
                OldName = "TestGroup",
                Name = "TestGroup_Edited",
                Parent = "Sundry Debtors",
                Action = Action.Alter,
                AddLAllocType = AdAllocType.NotApplicable,
            });
//For Ex. To Delete group we need to Set Group.Action to Delete and use the same method
PResult result = await Ctally.PostGroup(new Group()
            {
                OldName = "TestGroup",
                Action = Action.Delete,
            }); 
```

___

## Other Useful Resources Related to Tally Integration

Xmls used under the hood are listed here - [PostMan Collection](https://documenter.getpostman.com/view/13855108/TzeRpAMt)

StackOverflow threads answered by me  - [StackOverflow Answers](https://stackoverflow.com/search?tab=newest&q=user%3a13605609%20%5btally%5d)

TDLExpert.com threads answered by me - [TDLexpert.com](http://tdlexpert.com/index.php?members/sai-vineeth.12412/)
