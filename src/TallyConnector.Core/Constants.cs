using TallyConnector.Core.Models;

namespace TallyConnector.Core;
public static class Constants
{
    // Source - "DEFTDL:src\..\..\tdl.core\src\..\..\tdl.template\src\formula\formula.tdl"
    // Search with Const Name 
    
    public const string Prefix = "TC_";
    public const string MaxNumberLength = "31";
    public const string MaxAmountLength = "26";
    public const int IMaxAmountLength = 26;
    public const string MaxQtyLength = "15";
    public const string MaxSymbolLength = "15";
    public const string MaxRateLength = "9";
    public const string MaxUnitLength = "3";
    public const string MaxNarrLength = "4000";
    public const string MaxNameLength = "100";
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


    public const string GetBooleanFromLogicFieldFunctionName = "TC_GetBooleanFromLogicField";
    public const string TransformDateFunctionName = "TC_TransformDateToXSD";

    public static class Periodicty
    {
        public const string Month = "Month";
        public const string Day = "Day";
        public const string Week = "Week";
        public const string Fortnight = "Fortnight";
        public const string ThreeMonth = "3 Month";
        public const string SixMonth = "6 Month";
        public const string Year = "Year";
    }

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
                public static Filter AttndVoucherFilter = new("AttndViewType", $"$PERSISTEDVIEW = $$SysName:{ViewType.AccountingVoucherView} AND $$IsAttendance:$VOUCHERTYPENAME");
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

        public static List<string> All =
        [
            PaymentVoucherType, ReceiptNoteVoucherType, ContraVoucherType, JournalVoucherType,
            ReversingJournalVoucherType, SalesVoucherType, PurchaseVoucherType, SalesOrderVoucherType,
            PurchaseOrderVoucherType, DebitNoteVoucherType, CreditNoteVoucherType,MemoVoucherType,
            JobWorkOutVoucherType,JobWorkInVoucherType,MaterialOutVoucherType,MaterialInVoucherType,
            DeliveryNoteVoucherType, ReceiptNoteVoucherType,StockJournalVoucherType,PhysicalStockVoucherType,
            RejectionsInVoucherType,RejectionsOutVoucherType,PayrollVoucherType,AttendanceVoucherType
        ];
    }

    public static List<string> DefaultFetchList = ["MasterId", "*", "CanDelete"];

    public static List<string> VoucherViews =
    [
        Voucher.ViewType.AccountingVoucherView, Voucher.ViewType.InvoiceVoucherView,
        Voucher.ViewType.MfgJournalVoucherView, Voucher.ViewType.InventoryVoucherView,
        Voucher.ViewType.PayrollVoucherView
    ];

    public static class Groups
    {
        public const string Stock = "GroupStock";
        public const string CurrentLiab = "GroupCurrentLiab";
        public const string CurrentAssets = "GroupCurrentAssets";
        public const string FixedAssets = "GroupFixedAssets";
        public const string Capital = "GroupCapital";
        public const string Branches = "GroupBranches";
        public const string DirectExpenses = "GroupDirectExpenses";
        public const string DirectIncomes = "GroupDirectIncomes";
        public const string IndirectExpenses = "GroupIndirectExpenses";
        public const string IndirectIncomes = "GroupIndirectIncomes";
        public const string Investments = "GroupInvestments";
        public const string LoansLiab = "GroupLoansLiab";
        public const string MiscExp = "GroupMiscExp";
        public const string Purchase = "GroupPurchase";
        public const string Sales = "GroupSales";
        public const string Suspense = "GroupSuspense";
        public const string Duties = "GroupDuties";
        public const string SundryDebtors = "GroupSundryDebtors";
        public const string SundryCreditors = "GroupSundryCreditors";
        public const string Bank = "GroupBank";
        public const string BankOD = "GroupBankOD";
        public const string Cash = "GroupCash";
        public const string Deposits = "GroupDeposits";
        public const string Provisions = "GroupProvisions";
        public const string Reserves = "GroupReserves";
        public const string SecuredLoans = "GroupSecuredLoans";
        public const string UnsecuredLoans = "GroupUnsecuredLoans";
        public const string Advances = "GroupAdvances";

    }
}
