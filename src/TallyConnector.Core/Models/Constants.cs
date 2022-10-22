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
                public static Filter AccountingVoucherFilter = new("ViewType", $"$PERSISTEDVIEW = $$SysName:{ViewType.AccountingVoucherView} AND NOT $$IsAttendance:$VOUCHERTYPENAME");
                public static Filter InvoiceVoucherFilter = new("ViewType", $"$PERSISTEDVIEW = $$SysName:{ViewType.InvoiceVoucherView}");
                public static Filter InventoryVoucherFilter = new("ViewType", $"$PERSISTEDVIEW = $$SysName:{ViewType.InventoryVoucherView}");
                public static Filter MfgJournalVoucherFilter = new("ViewType", $"$PERSISTEDVIEW = $$SysName:{ViewType.MfgJournalVoucherView}");
                public static Filter AttndVoucherFilter = new("ViewType", $"$PERSISTEDVIEW = $$SysName:{ViewType.AccountingVoucherView} AND $$IsAttendance:$VOUCHERTYPENAME");
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
            public const string LedgerId = "LEDGERENTRIES.LEDGERID,ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.LEDGERID,LEDGERENTRIES.BILLALLOCATIONS.LEDGERID";
            public const string LedgerTaxType = "LEDGERENTRIES.LEDGERTAXTYPE,ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.LEDGERTAXTYPE";
            public const string VCHLedgerType = "LEDGERENTRIES.VCHLEDGERTYPE,ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.VCHLEDGERTYPE";
            public const string VCHLedgerIndex = "LEDGERENTRIES.INDEXNUMBER,ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.INDEXNUMBER";
            public const string InvAllocIndex = "ALLINVENTORYENTRIES.INDEXNUMBER";

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
                StockItemId, GodownId,CostCategoryId,CostCenterId,InvAllocIndex
            };
        }
        public static class AccountingViewFetchList
        {
            public const string LedgerId = "LEDGERENTRIES.LEDGERID,LEDGERENTRIES.BILLALLOCATIONS.LEDGERID";
            public const string LedgerTaxType = "LEDGERENTRIES.LEDGERTAXTYPE";
            public const string VCHLedgerType = "LEDGERENTRIES.VCHLEDGERTYPE";
            public const string VCHLedgerIndex = "LEDGERENTRIES.INDEXNUMBER";
            public const string StockItemId = "LEDGERENTRIES.INVENTORYALLOCATIONS.STOCKITEMID";
            public const string GodownId = "LEDGERENTRIES.INVENTORYALLOCATIONS.BATCHALLOCATIONS.GODOWNID," +
                                           "LEDGERENTRIES.INVENTORYALLOCATIONS.BATCHALLOCATIONS.DESTINATIONGODOWNID";

            public const string InvAllocIndex = "LEDGERENTRIES.INVENTORYALLOCATIONS.INDEXNUMBER";
            //public const string EmployeeId = "ATTENDANCEENTRY.EMPLOYEEID";

            public const string CostCategoryId = "LEDGERENTRIES.CATEGORYALLOCATIONS.COSTCATEGORYID," +
                "LEDGERENTRIES.INVENTORYALLOCATIONS.CATEGORYALLOCATIONS.COSTCATEGORYID";
            public const string CostCenterId = "LEDGERENTRIES.CATEGORYALLOCATIONS.COSTCENTREALLOCATIONS.COSTCENTREID," +
                "LEDGERENTRIES.INVENTORYALLOCATIONS.CATEGORYALLOCATIONS.COSTCENTREALLOCATIONS.COSTCENTREID";

            public static List<string> All = new(DefaultFetchList)
            {
                LedgerId, LedgerTaxType, VCHLedgerType,VCHLedgerIndex,
                StockItemId, GodownId ,CostCategoryId,CostCenterId,InvAllocIndex
            };
        }
        public static class AttendanceFetchList
        {
            public const string EmployeeId = "ATTENDANCEENTRIES.EMPLOYEEID";
            public const string AttendanceTypeId = "ATTENDANCEENTRIES.ATTENDANCETYPEID";


            public static List<string> All = new(DefaultFetchList)
            {
                EmployeeId,AttendanceTypeId
            };
        }
        public static class PaySlipViewFetchList
        {
            public const string LedgerId = "LEDGERENTRIES.LEDGERID";
            public const string VCHLedgerIndex = "LEDGERENTRIES.INDEXNUMBER";
            public const string CategoryId = "CATEGORYENTRY.CATEGORYID";
            public const string EmployeeId = "CATEGORYENTRY.EMPLOYEEENTRIES.EMPLOYEEID";
            public const string PayHeadLedgerId = "CATEGORYENTRY.EMPLOYEEENTRIES.PAYHEADALLOCATIONS.LEDGERID";


            public static List<string> All = new(DefaultFetchList)
            {
               LedgerId,VCHLedgerIndex, CategoryId,EmployeeId,PayHeadLedgerId
            };
        }
        public static class InventoryViewFetchList
        {
            public const string LedgerId = "LEDGERENTRIES.LEDGERID,LEDGERENTRIES.BILLALLOCATIONS.LEDGERID";
            public const string LedgerTaxType = "LEDGERENTRIES.LEDGERTAXTYPE";
            public const string VCHLedgerType = "LEDGERENTRIES.VCHLEDGERTYPE";
            public const string VCHLedgerIndex = "LEDGERENTRIES.INDEXNUMBER";
            public const string InvAllocIndex = "INVENTORYENTRIESIN.INDEXNUMBER,INVENTORYENTRIESOUT.INDEXNUMBER";

            public const string StockItemId = "INVENTORYENTRIESIN.STOCKITEMID,INVENTORYENTRIESOUT.STOCKITEMID";
            public const string GodownId = "INVENTORYENTRIESIN.BATCHALLOCATIONS.GODOWNID,INVENTORYENTRIESOUT.BATCHALLOCATIONS.GODOWNID," +
                                           "INVENTORYENTRIESIN.BATCHALLOCATIONS.DESTINATIONGODOWNID,INVENTORYENTRIESOUT.BATCHALLOCATIONS.DESTINATIONGODOWNID";

            public static List<string> All = new(DefaultFetchList)
            {
                LedgerId, LedgerTaxType,InvAllocIndex, VCHLedgerType,VCHLedgerIndex,
                StockItemId, GodownId
            };
        }

    }


    public static class VoucherType
    {
        public const string PaymentVoucherType = "$$VchTypePayment";
        public const string ReceiptVoucherType = "$$VchTypeReceipt";
        public const string ContraVoucherType = "$$VchTypeContra";

        public const string JournalVoucherType = "$$VchTypeJournal";
        public const string ReversingJournalVoucherType = "$$VchTypeRevJrnl";

        public const string SalesVoucherType = "$$VchTypeSales";
        public const string PurchaseVoucherType = "$$VchTypePurchase";
        public const string SalesOrderVoucherType = "$$VchTypeSalesOrder";
        public const string PurchaseOrderVoucherType = "$$VchTypePurcOrder";
        public const string DebitNoteVoucherType = "$$VchTypeDebitNote";
        public const string CreditNoteVoucherType = "$$VchTypeCreditNote";

        public const string JobWorkOutVoucherType = "$$VchTypeJobOrderIn";
        public const string JobWorkInVoucherType = "$$VchTypeJobOrderOut";

        public const string MaterialOutVoucherType = "$$VchTypeJobMaterialIssue";
        public const string MaterialInVoucherType = "$$VchTypeJobMaterialReceive";

        public const string DeliveryNoteVoucherType = "$$VchTypeDelNote";

        public const string ReceiptNoteVoucherType = "$$VchTypeRcptNote";

        public const string StockJournalVoucherType = "$$VchTypeStockJrnl";

        public const string PhysicalStockVoucherType = "$$VchTypePhysStock";

        public const string RejectionsInVoucherType = "$$VchTypeRejIn";
        public const string RejectionsOutVoucherType = "$$VchTypeRejOut";

        public const string PayrollVoucherType = "$$VchTypePayroll";
        public const string AttendanceVoucherType = "$$VchTypeAttendance";


        public const string MemoVoucherType = "$$VchTypeMemo";

        public static List<string> All = new()
        {
            PaymentVoucherType, ReceiptNoteVoucherType, ContraVoucherType, JournalVoucherType,
            ReversingJournalVoucherType, SalesVoucherType, PurchaseVoucherType, SalesOrderVoucherType,
            PurchaseOrderVoucherType, DebitNoteVoucherType, CreditNoteVoucherType,MemoVoucherType,
            JobWorkOutVoucherType,JobWorkInVoucherType,MaterialOutVoucherType,MaterialInVoucherType,
            DeliveryNoteVoucherType, ReceiptNoteVoucherType,StockJournalVoucherType,PhysicalStockVoucherType,
            RejectionsInVoucherType,RejectionsOutVoucherType,PayrollVoucherType,AttendanceVoucherType
        };
    }

    public static List<string> DefaultFetchList = new() { "MasterId", "*", "CanDelete" };

    public static List<string> VoucherViews = new()
    {
        Voucher.ViewType.AccountingVoucherView, Voucher.ViewType.InvoiceVoucherView,
        Voucher.ViewType.MfgJournalVoucherView, Voucher.ViewType.InventoryVoucherView,
        Voucher.ViewType.PayrollVoucherView
    };
}
