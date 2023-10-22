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
}

public class ViewTypeMapping
{
    public ViewTypeMapping()
    {
    }

    public ViewTypeMapping(VoucherViewType viewType,
                           List<string> fetchList,
                           Filter filter)
    {
        ViewType = viewType;
        FetchList = fetchList;
        Filter = filter;
    }

    public VoucherViewType ViewType { get; }

    public List<string> FetchList { get; }
    public Filter Filter { get; }
}
public class Mappings
{
    private const string CoreVchtypeFormulae1 = "CoreVoucherType:(if $$IsSales:$NAME then \"Sales\" else if $$IsPurchase:$NAME then \"Purchase\" else if $$IsDebitNote:$Name then \"DebitNote\" else if $$IsCreditNote:$Name then \"CreditNote\" else if $$IsPayment:$Name then \"Payment\" else if $$IsReceipt:$Name then \"Receipt\" else if  $$IsContra:$Name then \"Contra\" else if  $$IsJournal:$Name then \"Journal\" else if  $$IsSalesOrder:$Name then \"SalesOrder\" else if  $$IsPurcOrder:$Name then \"PurchaseOrder\" else if  $$IsMemo:$Name then \"Memo\" else \"\") +  ";
    private const string CoreVchtypeFormulae2 = "(if  $$IsRevJrnl:$Name then \"Reversing Journal\"  else if $$IsJobMaterialReceive:$NAME then \"MaterialIn\" else if  $$IsJobMaterialIssue:$Name then \"MaterialOut\" else if  $$IsJobOrderIn:$Name then \"JobWork In Order\" else if  $$IsJobOrderOut:$Name then \"JobWork Out Order\" else if  $$IsRcptNote:$Name then \"ReceiptNote\" else \"\") + ";
    private const string CoreVchtypeFormulae3 = "(if  $$IsDelNote:$Name then \"DeliveryNote\" else if  $$IsPhysStock:$Name then \"PhysicalStock\" else if  $$IsPayroll:$Name then \"Payroll\" else if  $$IsAttendance:$Name then \"Attendance\" else if  $$IsRejIn:$Name then \"RejectionsIn\"  else if  $$IsRejOut:$Name then \"RejectionsOut\" else if  $$IsStockJrnl:$Name then \"StockJournal\" else \"\")";


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
                               computeFields: new(){ "NAME:$NAME","PARENTID:$GUID:Group:$Parent","PrimaryGroup:$_PrimaryGroup" }),

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
                                   "NAME:$NAME",
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
                                   "PARENTID:$GUID:VoucherType:$Parent",

                                   "DefaultVoucherCategory:if $$IsAccountingVch:$NAME then \"AccountingVch\" else " +
                                   "if $$IsInventoryVch:$NAME then \"InventoryVch\" " +
                                   "else if $$IsOrderVch:$Name then \"OrderVch\" else " +
                                   "if $$IsPayrollVch:$Name then \"PayrollVch\" else " +
                                   "if $$IsAttendance:$Name then \"PayrollAttndVch\" else \"\"",

                                   CoreVchtypeFormulae1 + CoreVchtypeFormulae2 + CoreVchtypeFormulae3

                               }),
    };

    public static readonly List<TallyObjectMapping> TallyObjectMappings = new(MastersMappings)
    {
        new TallyObjectMapping(masterType : TallyObjectType.Vouchers,
                               tallyMasterType : "Voucher",
                               defaultPaginateCount : 1000,
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
                                       "LedgerId: $GUID:Ledger:$LedgerName"
                                   }){IsModify=YesNo.Yes},
                                   new("BillAllocations", new()
                                   {
                                       "LedgerID: $.LedgerId"
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
                                   }){IsModify=YesNo.Yes},
                                   new("AttendanceEntry", new()
                                   {
                                       "EmployeeId: $GUID:COSTCENTER:$NAME",
                                       "AttendanceTypeId: $GUID:ATTENDANCETYPE:$ATTENDANCETYPE",
                                   }){IsModify=YesNo.Yes},
                                   new("CategoryEntry", new()
                                   {
                                       "CategoryId: $GUID:CostCategory:$Category",
                                   }){IsModify=YesNo.Yes},
                                   new("EmployeeEntry", new()
                                   {
                                       "EmployeeId: $GUID:CostCenter:$EmployeeName",
                                   }){IsModify=YesNo.Yes},
                                   new("PayheadAllocations", new()
                                   {
                                       "LedgerId: $GUID:Ledger:$PayHeadName",
                                   }){IsModify=YesNo.Yes}
                               }),
    };

    public static readonly List<ViewTypeMapping> VoucherViewTypeMappings = new()
    {
        new(VoucherViewType.AccountingVoucherView,
            Constants.Voucher.AccountingViewFetchList.All,
            Constants.Voucher.Filters.ViewTypeFilters.AccountingVoucherFilter),

        new(VoucherViewType.InvoiceVoucherView,
            Constants.Voucher.InvoiceViewFetchList.All,
            Constants.Voucher.Filters.ViewTypeFilters.InvoiceVoucherFilter),

        new(VoucherViewType.ConsumptionVoucherView,
            Constants.Voucher.InventoryViewFetchList.All,
            Constants.Voucher.Filters.ViewTypeFilters.InventoryVoucherFilter),

        new(VoucherViewType.MultiConsumptionVoucherView,
            Constants.Voucher.InventoryViewFetchList.All,
            Constants.Voucher.Filters.ViewTypeFilters.MfgJournalVoucherFilter),

        new(VoucherViewType.AccountingVoucherView,
            Constants.Voucher.AttendanceFetchList.All,
            Constants.Voucher.Filters.ViewTypeFilters.AttndVoucherFilter),
        new(VoucherViewType.PaySlipVoucherView,
            Constants.Voucher.PaySlipViewFetchList.All,
            Constants.Voucher.Filters.ViewTypeFilters.PayslipVoucherFilter),
    };


}

