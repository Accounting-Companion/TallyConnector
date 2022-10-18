namespace TallyConnector.Core.Models;
public static class Constants
{
    // Source - "DEFTDL:src\..\..\tdl.core\src\..\..\tdl.template\src\formula\formula.tdl"
    // Search with Const Name 

    public const string MaxNumberLength = "31";
    public const string MaxAmountLength = "26";
    public const int IMaxAmountLength = 26;
    public const string MaxQtyLength = "15";
    public const string MaxSymbolLength = "15";
    public const string MaxRateLength = "9";
    public const string MaxUnitLength = "3";
    public const string MaxNarrLength = "250";
    public const string MaxNameLength = "51";
    public const string MaxDateLength = "25";
    public const string MaxParticularsLength = "30";
    public const string GUIDLength = "100";

    public const string TallyPrimeLicense = "$$SPrintf:@@CapProductDetails:@@VersionReleaseString:@@VersionBuildString:" +
                                            "@@ProductBitnessStr:($$String:@@MajorReleaseeFormula):($$String:@@MinorReleaseFormula)" +
                                            ":\"0\":@@CapBuildNumberFormula";
    public const string TallyERP9License = "$$SPrintf:@@CapProductDetails:@@VersionGetProductSeries:@@VersionReleaseString" +
                                            ":@@VersionBuildString:@@ProductBitnessStr:($$String:@@MajorReleaseeFormula)" +
                                            ":($$String:@@MinorReleaseFormula):\"0\":@@CapBuildNumberFormula";

    public const string License = "if @@CapProductDetails contains \"Tally.ERP 9\" then " + TallyERP9License + " else " + TallyPrimeLicense;

    public static class Voucher
    {
        public static class Filters
        {
            public static class ViewTypeFilters
            {
                public static Filter AccountingVoucherFilter = new("ViewType", $"$PERSISTEDVIEW = $$SysName:{ViewType.AccountingVoucherView}");
                public static Filter InvoiceVoucherFilter = new("ViewType", $"$PERSISTEDVIEW = $$SysName:{ViewType.InvoiceVoucherView}");
                public static Filter InventoryVoucherFilter = new("ViewType", $"$PERSISTEDVIEW = $$SysName:{ViewType.InventoryVoucherView}");
                public static Filter MfgJournalVoucherFilter = new("ViewType", $"$PERSISTEDVIEW = $$SysName:{ViewType.MfgJournalVoucherView}");
                public static Filter PayslipVoucherFilter = new("ViewType", $"$PERSISTEDVIEW = $$SysName:{ViewType.PayrollVoucherView}");
            }
        }
        public static class Category
        {
            public const string EInvoiceDetails = "E-InvoiceDetails";
        }

        public static class ViewType
        {
            public const string AccountingVoucherView = "AcctgVchView";
            public const string InvoiceVoucherView = "InvVchView";
            public const string InventoryVoucherView = "ConsVchView";
            public const string MfgJournalVoucherView = "MulConsVchView";
            public const string PayrollVoucherView = "PaySlipVchView";
        }
        public static class InvoiceViewFetchList
        {
            public const string LedgerId = "LEDGERENTRIES.LEDGERID,ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.LEDGERID";
            public const string LedgerTaxType = "LEDGERENTRIES.LEDGERTAXTYPE,ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.LEDGERTAXTYPE";
            public const string VCHLedgerType = "LEDGERENTRIES.VCHLEDGERTYPE,ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.VCHLEDGERTYPE";
            public const string VCHLedgerIndex = "LEDGERENTRIES.INDEXNUMBER,ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.INDEXNUMBER";
            public const string StockItemId = "ALLINVENTORYENTRIES.STOCKITEMID";
            public const string GodownId = "ALLINVENTORYENTRIES.BATCHALLOCATIONS.GODOWNID," +
                                           "ALLINVENTORYENTRIES.BATCHALLOCATIONS.DESTINATIONGODOWNID";

            public const string CostCategoryId = "LEDGERENTRIES.CATEGORYALLOCATIONS.COSTCATEGORYID," +
                "ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.CATEGORYALLOCATIONS.COSTCATEGORYID";
            public const string CostCenterId = "LEDGERENTRIES.CATEGORYALLOCATIONS.COSTCENTREALLOCATIONS.COSTCENTREID," +
                "ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.CATEGORYALLOCATIONS.COSTCENTREALLOCATIONS.COSTCENTREID";

            public static List<string> All = new(DefaultFetchList)
            {
                LedgerId, LedgerTaxType, VCHLedgerType,VCHLedgerIndex,
                StockItemId, GodownId,CostCategoryId,CostCenterId
            };
        }
        public static class AccountingViewFetchList
        {
            public const string LedgerId = "LEDGERENTRIES.LEDGERID";
            public const string LedgerTaxType = "LEDGERENTRIES.LEDGERTAXTYPE";
            public const string VCHLedgerType = "LEDGERENTRIES.VCHLEDGERTYPE";
            public const string VCHLedgerIndex = "LEDGERENTRIES.INDEXNUMBER";
            public const string StockItemId = "LEDGERENTRIES.INVENTORYALLOCATIONS.STOCKITEMID";
            public const string GodownId = "LEDGERENTRIES.INVENTORYALLOCATIONS.BATCHALLOCATIONS.GODOWNID," +
                                           "LEDGERENTRIES.INVENTORYALLOCATIONS.BATCHALLOCATIONS.DESTINATIONGODOWNID";

            public const string CostCategoryId = "LEDGERENTRIES.CATEGORYALLOCATIONS.COSTCATEGORYID," +
                "LEDGERENTRIES.INVENTORYALLOCATIONS.CATEGORYALLOCATIONS.COSTCATEGORYID";
            public const string CostCenterId = "LEDGERENTRIES.CATEGORYALLOCATIONS.COSTCENTREALLOCATIONS.COSTCENTREID," +
                "LEDGERENTRIES.INVENTORYALLOCATIONS.CATEGORYALLOCATIONS.COSTCENTREALLOCATIONS.COSTCENTREID";

            public static List<string> All = new(DefaultFetchList)
            {
                LedgerId, LedgerTaxType, VCHLedgerType,VCHLedgerIndex,
                StockItemId, GodownId ,CostCategoryId,CostCenterId
            };
        }
        public static class InventoryViewFetchList
        {
            public const string LedgerId = "LEDGERENTRIES.LEDGERID";
            public const string LedgerTaxType = "LEDGERENTRIES.LEDGERTAXTYPE";
            public const string VCHLedgerType = "LEDGERENTRIES.VCHLEDGERTYPE";
            public const string VCHLedgerIndex = "LEDGERENTRIES.INDEXNUMBER";
            public const string StockItemId = "INVENTORYENTRIESIN.STOCKITEMID,INVENTORYENTRIESOUT.STOCKITEMID";
            public const string GodownId = "INVENTORYENTRIESIN.BATCHALLOCATIONS.GODOWNID,INVENTORYENTRIESOUT.BATCHALLOCATIONS.GODOWNID" +
                                           "INVENTORYENTRIESIN.BATCHALLOCATIONS.DESTINATIONGODOWNID,INVENTORYENTRIESOUT.BATCHALLOCATIONS.DESTINATIONGODOWNID";

            public static List<string> All = new(DefaultFetchList)
            {
                LedgerId, LedgerTaxType, VCHLedgerType,VCHLedgerIndex,
                StockItemId, GodownId
            };
        }

    }

    public static List<string> DefaultFetchList = new() { "MasterId", "*", "CanDelete" };

    public static List<string> VoucherViews = new()
    {
        Voucher.ViewType.AccountingVoucherView, Voucher.ViewType.InvoiceVoucherView,
        Voucher.ViewType.MfgJournalVoucherView, Voucher.ViewType.InventoryVoucherView,
        Voucher.ViewType.PayrollVoucherView
    };
}
