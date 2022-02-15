namespace TallyConnector.Models;

public class MastersMapping
{
    public MastersMapping(TallyObjectType masterType, string tallyMasterType, List<Filter> filters)
    {
        MasterType = masterType;
        Filters = filters;
        TallyMasterType = tallyMasterType;
    }

    public TallyObjectType MasterType { get; set; }
    public string TallyMasterType { get; set; }

    public List<Filter> Filters { get; set; }

    public static readonly List<MastersMapping> MastersMappings = new()
    {
        new MastersMapping(TallyObjectType.Currencies, "Currency", null),

        new MastersMapping(TallyObjectType.Groups, "Group", null),
        new MastersMapping(TallyObjectType.Ledgers, "Ledger", null),

        new MastersMapping(TallyObjectType.CostCategories, "CostCategory", null),
        new MastersMapping(TallyObjectType.CostCenters, "CostCentre",
            new List<Filter>()
            {
                new Filter("IsEmployeeGroup", "Not $ISEMPLOYEEGROUP"),
                new Filter("IsPayroll", "Not $FORPAYROLL")
            }),

        new MastersMapping(TallyObjectType.Units, "Unit", null),
        new MastersMapping(TallyObjectType.Godowns, "Godown", null),
        new MastersMapping(TallyObjectType.StockCategories, "StockCategory", null),
        new MastersMapping(TallyObjectType.StockGroups, "StockGroup", null),
        new MastersMapping(TallyObjectType.StockItems, "StockItem", null),

        new MastersMapping(TallyObjectType.AttendanceTypes, "AttendanceType", null),
        new MastersMapping(TallyObjectType.EmployeeGroups, "CostCentre", new List<Filter>()
            {
                new Filter("IsEmployeeGroup", "$ISEMPLOYEEGROUP"),
            }),
        new MastersMapping(TallyObjectType.Employees, "CostCentre", new List<Filter>()
            {
                new Filter("IsEmployeeGroup", "Not $ISEMPLOYEEGROUP"),
                new Filter("IsPayroll", "$FORPAYROLL")
            }),

        new MastersMapping(TallyObjectType.VoucherTypes, "VoucherType", null),
    };
}

public class MastersBasicInfo<T>
{
    public MastersBasicInfo(TallyObjectType masterType, List<T> masters)
    {
        MasterType = masterType;
        Masters = masters;
    }

    public TallyObjectType MasterType { get; set; }

    public List<T> Masters { get; set; } = new();

    public int Count => Masters.Count;
}
