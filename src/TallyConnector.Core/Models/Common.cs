namespace TallyConnector.Core.Models;
public enum GenerationMode
{
    Get,
    GetMultiple,
    Post,
}
public enum MasterLookupField
{
    MasterId = 1,
    AlterId = 2,
    Name = 3,
    GUID = 4,
}

public enum Action
{
    [EnumXMLChoice(Choice = "")]
    None,

    Create,

    Alter,

    Delete,

    Cancel,
}

/// <summary>
/// Yes or No field in Tally
/// </summary>
public enum YesNo
{

    [EnumXMLChoice(Choice = "")]
    None,
    [EnumXMLChoice(Choice = "Yes")]
    Yes = 1,
    [EnumXMLChoice(Choice = "No")]
    No = 2,


}

/// <summary>
/// Method to allocate When used in purchase invoice
/// </summary>
public enum AdAllocType
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "Not Applicable")]
    NotApplicable = 1,
    [EnumXMLChoice(Choice = "Appropriate by Qty")]
    AppropriateByQty = 2,
    [EnumXMLChoice(Choice = "Appropriate by Value")]
    AppropriateByValue = 3,
    [EnumXMLChoice(Choice = "Appropriate by condition")]
    AppropriateByCondition = 4,
}

public enum TaxType
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "GST")]
    GST = 1,
    [EnumXMLChoice(Choice = "TCS")]
    TCS = 2,
    [EnumXMLChoice(Choice = "TDS")]
    TDS = 3,
    [EnumXMLChoice(Choice = "Excise")]
    Excise = 4,
    [EnumXMLChoice(Choice = "FBT")]
    FBT = 5,
    [EnumXMLChoice(Choice = "Service Tax")]
    ServiceTax = 6,
    [EnumXMLChoice(Choice = "VAT")]
    VAT = 7,
    [EnumXMLChoice(Choice = "Default")]
    Default = 8,
    [EnumXMLChoice(Choice = "CST")]
    CST = 9,
    [EnumXMLChoice(Choice = "CENVAT")]
    CENVAT = 10,
    [EnumXMLChoice(Choice = "Krishi Kalyan Cess")]
    KrishiKalyanCess = 11,
    [EnumXMLChoice(Choice = "Swachh Bharat Cess")]
    SwachhBharatCess = 12,

    [EnumXMLChoice(Choice = "Others")]
    Others = 13,
}


public enum GSTTaxType
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "Central Tax",Versions = ["6.6.3", "1.1.1", "1.1.2", "1.1.3", "1.1.4", "2.0", "2.0.1", "2.1"])]
    [EnumXMLChoice(Choice = "CGST")]
    CGST = 1,
    [EnumXMLChoice(Choice = "Cess")]
    Cess = 2,
    [EnumXMLChoice(Choice = "Integrated Tax", Versions = ["6.6.3", "1.1.1", "1.1.2", "1.1.3", "1.1.4", "2.0", "2.0.1", "2.1"])]
    [EnumXMLChoice(Choice = "IGST")]
    IGST = 3,
    [EnumXMLChoice(Choice = "UT Tax")]
    UTTax = 4,
    [EnumXMLChoice(Choice = "State Tax", Versions = ["6.6.3", "1.1.1", "1.1.2", "1.1.3", "1.1.4", "2.0", "2.0.1", "2.1"])]
    [EnumXMLChoice(Choice = "SGST/UTGST")]
    SGST = 5,
}
public enum ExciseNatureOfPurchase
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "Any")]
    Any = 1,
    [EnumXMLChoice(Choice = "Importer")]
    Importer = 2,
}
public enum GSTRegistrationType
{
    
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "Unknown")]
    Unknown = 1,
    [EnumXMLChoice(Choice = "Composition")]
    Composition = 2,

    [EnumXMLChoice(Choice = "Consumer", Versions = ["6.6.3", "1.1.1", "1.1.2", "1.1.3", "1.1.4", "2.0", "2.0.1", "2.1"])]
    [EnumXMLChoice(Choice = "Unregistered/Consumer")]
    Consumer = 3,

    [EnumXMLChoice(Choice = "Regular")]
    Regular = 4,
    [EnumXMLChoice(Choice = "Unregistered")]
    Unregistered = 5,

    // Below are added in Prime 3.0

    [EnumXMLChoice(Choice = "Government entity / TDS")]
    GovtOrTDS = 6,
    [EnumXMLChoice(Choice = "Regular - SEZ")]
    RegularSEZ = 7,
    [EnumXMLChoice(Choice = "Regular-Deemed Exporter")]
    RegularDeemedExporter = 8,
    [EnumXMLChoice(Choice = "Regular-Exports (EOU)")]
    RegularEXports_EOU = 9,
    [EnumXMLChoice(Choice = "e-Commerce Operator")]
    ECommerceOperator = 10,
    [EnumXMLChoice(Choice = "Input Service Distributor")]
    InputServiceDistributor = 78,
    [EnumXMLChoice(Choice = "Embassy/UN Body")]
    Embassy_UNBody = 89,
    [EnumXMLChoice(Choice = "Non-Resident Taxpayer")]
    NonResidentTaxpayer = 100,

    [EnumXMLChoice(Choice = "OIDAR")]
    OIDAR = 101,

}

public enum GSTPartyType
{

    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "Not Applicable")]
    NotApplicable = 1,
    [EnumXMLChoice(Choice = "Deemed Export")]
    DeemedExport = 2,
    [EnumXMLChoice(Choice = "Embassy/UN Body")]
    Embassy_UN_Body = 3,
    [EnumXMLChoice(Choice = "SEZ")]
    SEZ = 4,
    [EnumXMLChoice(Choice = "No")]
    No = 5,
}

public enum InterestStyle
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "30-Day Month")]
    Month_30Day = 1,
    [EnumXMLChoice(Choice = "365-Day Year")]
    Year_365Day = 2,
    [EnumXMLChoice(Choice = "Calendar Month")]
    CalendarMonth = 3,
    [EnumXMLChoice(Choice = "Calendar Year")]
    CalendarYear = 4,

}

public enum InterestBalanceType
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "All Balances")]
    AllBalances = 1,
    [EnumXMLChoice(Choice = "Credit Balances Only")]
    OnCreditBalances = 2,
    [EnumXMLChoice(Choice = "Debit Balances Only")]
    OnDebitBalances = 3,
}

public enum InterestAppliedOn
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "Always")]
    Always = 1,
    [EnumXMLChoice(Choice = "Past Due Date")]
    PastDueDate = 2,

}
public enum InterestFromType
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "From Appl Date")]
    DateOfApplicability = 1,
    [EnumXMLChoice(Choice = "Date specified during entry")]
    DateSpecifiedDuringEntry = 2,
    [EnumXMLChoice(Choice = "Past Due Date")]
    DueDateOfInvoice = 3,
    [EnumXMLChoice(Choice = "Date specified during entry")]
    EffectiveDateOfTransaction = 4,
}

public enum RoundType
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "Not Applicable")]
    NotApplicable = 1,
    [EnumXMLChoice(Choice = "Downward Rounding")]
    Dowward = 2,
    [EnumXMLChoice(Choice = "Normal Rounding")]
    Normal = 3,
    [EnumXMLChoice(Choice = "Upward Rounding")]
    Upward = 4,
}

public enum PeriodicityType
{
    [EnumXMLChoice(Choice = Constants.Periodicty.Month)]
    Month = 0,
    [EnumXMLChoice(Choice = Constants.Periodicty.Day)]
    Day = 1,
    [EnumXMLChoice(Choice = Constants.Periodicty.Week)]
    Week = 2,
    [EnumXMLChoice(Choice = Constants.Periodicty.Fortnight)]
    Fortnight = 3,
    [EnumXMLChoice(Choice = Constants.Periodicty.ThreeMonth)]
    ThreeMonth = 4,
    [EnumXMLChoice(Choice = Constants.Periodicty.SixMonth)]
    SixMonth = 5,
    [EnumXMLChoice(Choice = Constants.Periodicty.Year)]
    Year = 6,
}
