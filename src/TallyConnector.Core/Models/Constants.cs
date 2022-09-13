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


    public static class Voucher
    {
        public static class Category
        {
            public const string EInvoiceDetails = "E-InvoiceDetails";
        }
        public static class InvoiceViewFetchList
        {
            public const string LedgerId = "LEDGERENTRIES.LEDGERID,ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.LEDGERID";
            public const string LedgerTaxType = "LEDGERENTRIES.LEDGERTAXTYPE,ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.LEDGERTAXTYPE";
            public const string VCHLedgerType = "LEDGERENTRIES.VCHLEDGERTYPE,ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.VCHLEDGERTYPE";
            public const string StockItemId = "ALLINVENTORYENTRIES.STOCKITEMID";
            public const string GodownId = "ALLINVENTORYENTRIES.BATCHALLOCATIONS.GODOWNID," +
                                           "ALLINVENTORYENTRIES.BATCHALLOCATIONS.DESTINATIONGODOWNID";

            public const string CostCategoryId = "LEDGERENTRIES.CATEGORYALLOCATIONS.COSTCATEGORYID," +
                "ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.CATEGORYALLOCATIONS.COSTCATEGORYID";
            public const string CostCenterId = "LEDGERENTRIES.CATEGORYALLOCATIONS.COSTCENTREALLOCATIONS.COSTCENTREID," +
                "ALLINVENTORYENTRIES.ACCOUNTINGALLOCATIONS.CATEGORYALLOCATIONS.COSTCENTREALLOCATIONS.COSTCENTREID";

            public static List<string> All = new(DefaultFetchList)
            {
                LedgerId, LedgerTaxType, VCHLedgerType,
                StockItemId, GodownId,CostCategoryId,CostCenterId
            };
        }
        public static class AccountingViewFetchList
        {
            public const string LedgerId = "LEDGERENTRIES.LEDGERID";
            public const string LedgerTaxType = "LEDGERENTRIES.LEDGERTAXTYPE";
            public const string VCHLedgerType = "LEDGERENTRIES.VCHLEDGERTYPE";
            public const string StockItemId = "LEDGERENTRIES.INVENTORYALLOCATIONS.STOCKITEMID";
            public const string GodownId = "LEDGERENTRIES.INVENTORYALLOCATIONS.BATCHALLOCATIONS.GODOWNID," +
                                           "LEDGERENTRIES.INVENTORYALLOCATIONS.BATCHALLOCATIONS.DESTINATIONGODOWNID";

            public const string CostCategoryId = "LEDGERENTRIES.CATEGORYALLOCATIONS.COSTCATEGORYID," +
                "LEDGERENTRIES.INVENTORYALLOCATIONS.CATEGORYALLOCATIONS.COSTCATEGORYID";
            public const string CostCenterId = "LEDGERENTRIES.CATEGORYALLOCATIONS.COSTCENTREALLOCATIONS.COSTCENTREID," +
                "LEDGERENTRIES.INVENTORYALLOCATIONS.CATEGORYALLOCATIONS.COSTCENTREALLOCATIONS.COSTCENTREID";

            public static List<string> All = new(DefaultFetchList)
            {
                LedgerId, LedgerTaxType, VCHLedgerType,
                StockItemId, GodownId ,CostCategoryId,CostCenterId
            };
        }

    }

    public static List<string> DefaultFetchList = new() { "MasterId", "*" };

    public static List<string> VoucherViews = new() { "AcctgVchView", "InvVchView", "MulConsVchView", "ConsVchView" };
}
