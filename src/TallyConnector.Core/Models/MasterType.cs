namespace TallyConnector.Core.Models;

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
    [EnumXMLChoice("Locations")]
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
    [EnumXMLChoice("Budgets & Scenarios")]
    [EnumXMLChoice("Scenarios")]
    BudgetsScenarios = 18,
    [EnumXMLChoice("Vouchers")]
    Vouchers = 100,
}

