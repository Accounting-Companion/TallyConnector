namespace TallyConnector.Core.Models;

public class TallyObjectMapping
{
    public TallyObjectMapping(TallyObjectType masterType,
                              string tallyMasterType,
                              int? defaultPaginateCount,
                              List<Filter>? filters,
                              List<string>? computeFields = null,
                              List<TallyCustomObject>? objects = null)
    {
        MasterType = masterType;
        Filters = filters;
        ComputeFields = computeFields;
        TallyMasterType = tallyMasterType;
        DefaultPaginateCount = defaultPaginateCount;
        Objects = objects;
    }

    public TallyObjectType MasterType { get; }
    public string TallyMasterType { get; }

    public List<Filter>? Filters { get; }
    public List<string>? ComputeFields { get; }
    public int? DefaultPaginateCount { get; }

    public List<TallyCustomObject>? Objects { get; }

    public static readonly List<TallyObjectMapping> MastersMappings = new()
    {
        new TallyObjectMapping(masterType: TallyObjectType.Currencies,
                               tallyMasterType: "Currency",
                               defaultPaginateCount: 100,
                               filters: null),

        new TallyObjectMapping(masterType: TallyObjectType.Groups,
                               tallyMasterType: "Group",
                               defaultPaginateCount: 1000,
                               filters: null,
                               computeFields: new(){ "NAME:$NAME","PARENTID:$GUID:Group:$Parent" }),

        new TallyObjectMapping(TallyObjectType.Ledgers,
                               tallyMasterType: "Ledger",
                               defaultPaginateCount : 500,
                               filters: null,
                               computeFields: new()
                               {
                                   "NAME:$NAME",
                                   "PARENTID:$GUID:Group:$Parent",
                                   "CURRENCYID:$GUID:Currency:$CURRENCYNAME"
                               }),

        new TallyObjectMapping(masterType: TallyObjectType.CostCategories,
                               tallyMasterType: "CostCategory",
                               defaultPaginateCount : 1000,
                               filters: null,
                               computeFields:new(){"NAME:$NAME"}),
        new TallyObjectMapping(masterType: TallyObjectType.CostCentres,
                               tallyMasterType: "CostCentre",
                               defaultPaginateCount : 1000,
                               filters: new List<Filter>()
                               {
                                   new Filter("IsEmployeeGroup", "Not $ISEMPLOYEEGROUP"),
                                   new Filter("IsPayroll", "Not $FORPAYROLL")
                               },
                               computeFields: new()
                               {
                                   "NAME:$NAME",
                                   "CATEGORYID:$GUID:COSTCATEGORY:$CATEGORY",
                                   "PARENTID:$GUID:COSTCENTER:$Parent"
                               }),

        new TallyObjectMapping(masterType: TallyObjectType.Units,
                               tallyMasterType: "Unit",
                               defaultPaginateCount : 1000,
                               filters: null,
                               computeFields: new()
                               {
                                   "NAME:$NAME",
                                   "BASEUNITID:$GUID:Unit:$BaseUnits",
                                   "ADDITIONALUNITID:$GUID:Unit:$AdditionalUnits"
                               }),
        new TallyObjectMapping(masterType: TallyObjectType.Godowns,
                               tallyMasterType: "Godown",
                               defaultPaginateCount : 1000,
                               filters: null,
                               computeFields: new()
                               {
                                   "NAME:$NAME",
                                   "PARENTID:$GUID:Godown:$Parent"
                               }),
        new TallyObjectMapping(masterType: TallyObjectType.StockCategories,
                               tallyMasterType: "StockCategory",
                               defaultPaginateCount : 1000,
                               filters: null,
                               computeFields: new()
                               {
                                   "NAME:$NAME",
                                   "PARENTID:$GUID:StockCategory:$Parent"
                               }),
        new TallyObjectMapping(masterType : TallyObjectType.StockGroups,
                               tallyMasterType : "StockGroup",
                               defaultPaginateCount : 1000,
                               filters : null,
                               computeFields : new()
                               {
                                   "NAME:$NAME",
                                   "PARENTID:$GUID:StockGroup:$Parent"
                               }),
        new TallyObjectMapping(masterType : TallyObjectType.StockItems,
                               tallyMasterType : "StockItem",
                               defaultPaginateCount : 500,
                               filters : null,
                               computeFields : new()
                               {
                                   "NAME:$NAME",
                                   "PARENTID:$GUID:StockGroup:$Parent",
                                   "CATEGORYID:$GUID:StockCategory:$Category",
                                   "BASEUNITID:$GUID:Unit:$BaseUnits",
                                   "ADDITIONALUNITID:$GUID:Unit:$AdditionalUnits",
                               }),

        new TallyObjectMapping(masterType: TallyObjectType.AttendanceTypes,
                               tallyMasterType: "AttendanceType",
                               defaultPaginateCount : 1000,
                               filters: null,
                               computeFields: new()
                               {
                                   "NAME:$NAME",
                                   "PARENTID:$GUID:AttendanceType:$Parent",
                               }),

        new TallyObjectMapping(masterType: TallyObjectType.EmployeeGroups,
                               tallyMasterType: "CostCentre",
                               defaultPaginateCount : 1000,
                               filters: new List<Filter>()
                               {
                                   new Filter("IsEmployeeGroup", "$ISEMPLOYEEGROUP"),
                               },
                               computeFields: new()
                               {
                                   "CATEGORYID:$GUID:COSTCATEGORY:$CATEGORY",
                                   "PARENTID:$GUID:COSTCENTER:$Parent"
                               }),
        new TallyObjectMapping(masterType : TallyObjectType.Employees,
                               tallyMasterType : "CostCentre",
                               defaultPaginateCount : 500,
                               filters : new List < Filter >()
                               {
                                   new Filter("IsEmployeeGroup", "Not $ISEMPLOYEEGROUP"),
                                   new Filter("IsPayroll", "$FORPAYROLL")
                               },
                               computeFields : new()
                               {
                                   "NAME:$NAME",
                                   "CATEGORYID:$GUID:COSTCATEGORY:$CATEGORY",
                                   "PARENTID:$GUID:COSTCENTER:$Parent"
                               }),

        new TallyObjectMapping(masterType : TallyObjectType.VoucherTypes,
                               tallyMasterType : "VoucherType",
                               defaultPaginateCount : 1000,
                               filters : null,
                               computeFields : new()
                               {
                                   "NAME:$NAME",
                                   "PARENTID:$GUID:VoucherType:$Parent"
                               }),
    };

    public static readonly List<TallyObjectMapping> TallyObjectMappings = new(MastersMappings)
    {
        new TallyObjectMapping(masterType : TallyObjectType.Vouchers,
                               tallyMasterType : "Voucher",
                               defaultPaginateCount : 500,
                               filters : null,
                               computeFields : new()
                               {
                                   "VOUCHERTYPEID:$GUID:VoucherType:$VOUCHERTYPENAME",
                                   "PARTYLEDGERID:$GUID:Ledger:$PARTYLEDGERNAME"
                               },
                               objects: new()
                               {
                                   new("LedgerEntry", new()
                                   {
                                       "LedgerID: $GUID:Ledger:$LedgerName"
                                   }){IsModify=YesNo.Yes},
                                   new("InventoryEntry", new()
                                   {
                                       "StockItemID: $GUID:StockItem:$StockItemName"
                                   }){IsModify=YesNo.Yes},
                                   new("BatchAllocations", new()
                                   {
                                       "GodownID: $GUID:Godown:$GodownName",
                                       "DestinationGodownID: $GUID:Godown:$DestinationGodownName"
                                   }){IsModify=YesNo.Yes},
                                   new("CategoryAllocations", new()
                                   {
                                       "CostCategoryID: $GUID:CostCategory:$Category",
                                   }){IsModify=YesNo.Yes},
                                   new("CostCenterAllocations", new()
                                   {
                                       "CostCentreID: $GUID:COSTCENTER:$NAME",
                                   }){IsModify=YesNo.Yes}
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
