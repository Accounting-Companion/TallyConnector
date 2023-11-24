namespace TallyConnector.Core.Models;

public enum TallyObjectType
{
    [XmlEnum("Currencies")]
    Currencies = 1,
    [XmlEnum("Groups")]
    Groups = 2,
    [XmlEnum("Ledgers")]
    Ledgers = 3,
    [XmlEnum("Cost Categories")]
    CostCategories = 4,
    [XmlEnum("Cost Centres")]
    CostCentres = 5,
    [XmlEnum("Godowns")]
    Godowns = 6,
    [XmlEnum("Locations")]
    Locations = 6,
    [XmlEnum("Stock Categories")]
    StockCategories = 7,
    [XmlEnum("Stock Groups")]
    StockGroups = 8,
    [XmlEnum("Stock Items")]
    StockItems = 9,
    [XmlEnum("Units")]
    Units = 10,
    [XmlEnum("Attendance/Production Types")]
    AttendanceTypes = 11,
    [XmlEnum("Employee Groups")]
    EmployeeGroups = 12,
    [XmlEnum("Employees")]
    Employees = 13,
    [XmlEnum("Voucher Types")]
    VoucherTypes = 14,
    [XmlEnum("TaxUnits")]
    TaxUnits = 15,
    [XmlEnum("GSTRegistrations")]
    GSTRegistrations = 16,
    [XmlEnum("Budgets")]
    Budgets = 17,
    [XmlEnum("Budgets & Scenarios")]
    BudgetsScenarios = 18,
    [XmlEnum("Scenarios")]
    Scenarios = 18,
    [XmlEnum("Vouchers")]
    Vouchers = 100,
}

