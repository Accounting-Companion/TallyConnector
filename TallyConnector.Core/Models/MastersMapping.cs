namespace TallyConnector.Core.Models;

public class TallyObjectMapping
{
    public TallyObjectMapping(TallyObjectType masterType,
                              string tallyMasterType,
                              List<Filter>? filters,
                              List<string>? computeFields = null)
    {
        MasterType = masterType;
        Filters = filters;
        ComputeFields = computeFields;
        TallyMasterType = tallyMasterType;
    }

    public TallyObjectType MasterType { get; set; }
    public string TallyMasterType { get; set; }

    public List<Filter>? Filters { get; set; }
    public List<string>? ComputeFields { get; set; }

    public static readonly List<TallyObjectMapping> MastersMappings = new()
    {
        new TallyObjectMapping(
            masterType: TallyObjectType.Currencies, tallyMasterType: "Currency", filters: null),

        new TallyObjectMapping(
            masterType: TallyObjectType.Groups,
            tallyMasterType: "Group", filters: null,
            computeFields: new(){"PARENTID:$GUID:Group:$Parent"}),

        new TallyObjectMapping(
            TallyObjectType.Ledgers, "Ledger", null,
            new()
            {
                "PARENTID:$GUID:Group:$Parent",
                "CURRENCYID:$GUID:Currency:$CURRENCYNAME"
            }),

        new TallyObjectMapping(TallyObjectType.CostCategories, "CostCategory", null),
        new TallyObjectMapping(TallyObjectType.CostCenters, "CostCentre",
            new List<Filter>()
            {
                new Filter("IsEmployeeGroup", "Not $ISEMPLOYEEGROUP"),
                new Filter("IsPayroll", "Not $FORPAYROLL")
            },
            new()
            {
                "CATEGORYID:$GUID:COSTCATEGORY:$CATEGORY",
                "PARENTID:$GUID:COSTCENTER:$Parent"
            }),

        new TallyObjectMapping(
            TallyObjectType.Units, "Unit", null,
            new()
            {
                "BASEUNITID:$GUID:Unit:$BaseUnits",
                "ADDITIONALUNITID:$GUID:Unit:$AdditionalUnits"
            }),
        new TallyObjectMapping(
            TallyObjectType.Godowns, "Godown", null,
            new()
            {
                "PARENTID:$GUID:Godown:$Parent"
            }),
        new TallyObjectMapping(
            TallyObjectType.StockCategories, "StockCategory", null,
            new()
            {
                "PARENTID:$GUID:StockCategory:$Parent"
            }),
        new TallyObjectMapping(
            TallyObjectType.StockGroups, "StockGroup", null,new()
            {
                "PARENTID:$GUID:StockGroup:$Parent"
            }),
        new TallyObjectMapping(
            TallyObjectType.StockItems, "StockItem", null,new()
            {
                "PARENTID:$GUID:StockGroup:$Parent",
                "CATEGORYID:$GUID:StockCategory:$Category",
                "BASEUNITID:$GUID:Unit:$BaseUnits",
                "ADDITIONALUNITID:$GUID:Unit:$AdditionalUnits",
            }),

        new TallyObjectMapping(
            TallyObjectType.AttendanceTypes, "AttendanceType", null,
            new()
            {
                "PARENTID:$GUID:AttendanceType:$Parent",
            }),
        new TallyObjectMapping(TallyObjectType.EmployeeGroups, "CostCentre", new List<Filter>()
            {
                new Filter("IsEmployeeGroup", "$ISEMPLOYEEGROUP"),
            },
            new()
            {
                "CATEGORYID:$GUID:COSTCATEGORY:$CATEGORY",
                "PARENTID:$GUID:COSTCENTER:$Parent"
            }),
        new TallyObjectMapping(TallyObjectType.Employees, "CostCentre", new List<Filter>()
            {
                new Filter("IsEmployeeGroup", "Not $ISEMPLOYEEGROUP"),
                new Filter("IsPayroll", "$FORPAYROLL")
            },
            new()
            {
                "CATEGORYID:$GUID:COSTCATEGORY:$CATEGORY",
                "PARENTID:$GUID:COSTCENTER:$Parent"
            }),

        new TallyObjectMapping(
            TallyObjectType.VoucherTypes, "VoucherType", null,
            new()
            {
                 "PARENTID:$GUID:VoucherType:$Parent"
            }),
    };

    public static readonly List<TallyObjectMapping> TallyObjectMappings = new(MastersMappings)
    {
        new TallyObjectMapping(TallyObjectType.Vouchers, "Voucher", null,
            new()
            {
                 "VOUCHERTYPEID:$GUID:VoucherType:$VOUCHERTYPENAME",
                 "PARTYLEDGERID:$GUID:Ledger:$PARTYLEDGERNAMEs"
            }),
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
