namespace TallyConnector.Core.Models;
[GenerateMeta]
public enum TallyObjectType
{
    [EnumXMLChoice("Currencies")]
    Currencies = 1,
    [EnumXMLChoice("Groups")]
    Groups = 2,
    [EnumXMLChoice("Ledgers")]
    Ledgers = 3,
    [EnumXMLChoice("Cost Categories")]
    CostCategories = 4,
    [EnumXMLChoice("Cost Centres")]
    CostCentres = 5,
    [EnumXMLChoice("Godowns")]
    Godowns = 6,
    [EnumXMLChoice("Stock Categories")]
    StockCategories = 7,
    [EnumXMLChoice("Stock Groups")]
    StockGroups = 8,
    [EnumXMLChoice("Stock Items")]
    StockItems = 9,
    [EnumXMLChoice("Units")]
    Units = 10,
    [EnumXMLChoice("Attendance/Production Types")]
    AttendanceTypes = 11,
    [EnumXMLChoice("Employee Groups")]
    EmployeeGroups = 12,
    [EnumXMLChoice("Employees")]
    Employees = 13,
    [EnumXMLChoice("Voucher Types")]
    VoucherTypes = 14,
    [EnumXMLChoice("TaxUnits")]
    TaxUnits = 15,
    [EnumXMLChoice("GSTRegistrations")]
    GSTRegistrations = 16,
    [EnumXMLChoice("Budgets")]
    Budgets = 17,
    [EnumXMLChoice("Budgets & Scenarios", Versions = ["6.6.3"])]
    [EnumXMLChoice("Scenarios")]
    BudgetsScenarios = 18,
    [EnumXMLChoice("Vouchers")]
    Vouchers = 100,
}
/// <summary>
/// Yes or No field in Tally
/// </summary>
public enum YesNo
{
    [EnumXMLChoice(Choice = "")]
    None,
    [EnumXMLChoice(Choice = "Yes")]
    Yes = 1,
    [EnumXMLChoice(Choice = "No")]
    No = 2,
}
public enum Action
{

    Create,

    Alter,

    Delete,

    Cancel,
}

public enum MasterLookupField
{
    MasterId = 1,
    AlterId = 2,
    Name = 3,
    GUID = 4,
}
public enum PeriodicityType
{
    [EnumXMLChoice(Choice = Constants.Periodicty.Month)]
    Month = 0,
    [EnumXMLChoice(Choice = Constants.Periodicty.Day)]
    Day = 1,
    [EnumXMLChoice(Choice = Constants.Periodicty.Week)]
    Week = 2,
    [EnumXMLChoice(Choice = Constants.Periodicty.Fortnight)]
    Fortnight = 3,
    [EnumXMLChoice(Choice = Constants.Periodicty.ThreeMonth)]
    ThreeMonth = 4,
    [EnumXMLChoice(Choice = Constants.Periodicty.SixMonth)]
    SixMonth = 5,
    [EnumXMLChoice(Choice = Constants.Periodicty.Year)]
    Year = 6,
}