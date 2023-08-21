namespace TallyConnector.Core.Models;

public enum MasterLookupField
{
    MasterId = 1,
    AlterId = 2,
    Name = 3,
    GUID = 4,
}

public enum Action
{
    [XmlEnum(Name = "")]
    None,
    [XmlEnum(Name = "Create")]
    Create,
    [XmlEnum(Name = "Alter")]
    Alter,
    [XmlEnum(Name = "Delete")]
    Delete,
    [XmlEnum(Name = "Cancel")]
    Cancel,
}

/// <summary>
/// Yes or No field in Tally
/// </summary>
public enum YesNo
{

    [XmlEnum(Name = "")]
    None,
    [XmlEnum(Name = "Yes")]
    Yes = 1,
    [XmlEnum(Name = "No")]
    No = 2,
}

/// <summary>
/// Method to allocate When used in purchase invoice
/// </summary>
public enum AdAllocType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Not Applicable")]
    NotApplicable = 1,
    [XmlEnum(Name = "Appropriate by Qty")]
    AppropriateByQty = 2,
    [XmlEnum(Name = "Appropriate by Value")]
    AppropriateByValue = 3,
}

public enum TaxType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "GST")]
    GST = 1,
    [XmlEnum(Name = "TCS")]
    TCS = 2,
    [XmlEnum(Name = "TDS")]
    TDS = 3,
    [XmlEnum(Name = "Excise")]
    Excise = 4,
    [XmlEnum(Name = "FBT")]
    FBT = 5,
    [XmlEnum(Name = "Service Tax")]
    ServiceTax = 6,
    [XmlEnum(Name = "VAT")]
    VAT = 7,
    [XmlEnum(Name = "Default")]
    Default = 8,
    [XmlEnum(Name = "CST")]
    CST = 9,
    [XmlEnum(Name = "CENVAT")]
    CENVAT = 10,
    [XmlEnum(Name = "Krishi Kalyan Cess")]
    KrishiKalyanCess = 11,
    [XmlEnum(Name = "Swachh Bharat Cess")]
    SwachhBharatCess = 12,

    [XmlEnum(Name = "Others")]
    Others = 13,
}

public enum GSTTaxType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Central Tax")]
    CentralTax = 1,
    [XmlEnum(Name = "CGST")]
    CGST = 1,
    [XmlEnum(Name = "Cess")]
    Cess = 2,
    [XmlEnum(Name = "Integrated Tax")]
    IntegratedTax = 3,
    [XmlEnum(Name = "IGST")]
    IGST = 3,
    [XmlEnum(Name = "UT Tax")]
    UTTax = 4,
    [XmlEnum(Name = "State Tax")]
    StateTax = 5,
    [XmlEnum(Name = "SGST/UTGST")]
    SGST = 5,
}
public enum ExciseNatureOfPurchase
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Any")]
    Any = 1,
    [XmlEnum(Name = "Importer")]
    Importer = 2,
}
public enum GSTRegistrationType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Unknown")]
    Unknown = 1,
    [XmlEnum(Name = "Composition")]
    Composition = 2,
    [XmlEnum(Name = "Consumer")]
    Consumer = 3,
    [XmlEnum(Name = "Regular")]
    Regular = 4,
    [XmlEnum(Name = "Unregistered")]
    Unregistered = 5,

}

public enum GSTPartyType
{

    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Not Applicable")]
    NotApplicable = 1,
    [XmlEnum(Name = "Deemed Export")]
    DeemedExport = 2,
    [XmlEnum(Name = "Embassy/UN Body")]
    Embassy_UN_Body = 3,
    [XmlEnum(Name = "SEZ")]
    SEZ = 4,
    [XmlEnum(Name = "No")]
    No = 5,
}

public enum InterestStyle
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "30-Day Month")]
    Month_30Day = 1,
    [XmlEnum(Name = "365-Day Year")]
    Year_365Day = 2,
    [XmlEnum(Name = "Calendar Month")]
    CalendarMonth = 3,
    [XmlEnum(Name = "Calendar Year")]
    CalendarYear = 4,

}

public enum InterestBalanceType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "All Balances")]
    AllBalances = 1,
    [XmlEnum(Name = "Credit Balances Only")]
    OnCreditBalances = 2,
    [XmlEnum(Name = "Debit Balances Only")]
    OnDebitBalances = 3,
}

public enum InterestAppliedOn
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Always")]
    Always = 1,
    [XmlEnum(Name = "Past Due Date")]
    PastDueDate = 2,

}
public enum InterestFromType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "From Appl Date")]
    DateOfApplicability = 1,
    [XmlEnum(Name = "Date specified during entry")]
    DateSpecifiedDuringEntry = 2,
    [XmlEnum(Name = "Past Due Date")]
    DueDateOfInvoice = 3,
    [XmlEnum(Name = "Date specified during entry")]
    EffectiveDateOfTransaction = 4,
}

public enum RoundType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Not Applicable")]
    NotApplicable = 1,
    [XmlEnum(Name = "Downward Rounding")]
    Dowward = 2,
    [XmlEnum(Name = "Normal Rounding")]
    Normal = 3,
    [XmlEnum(Name = "Upward Rounding")]
    Upward = 4,
}

public enum PeriodicityType
{
    [XmlEnum(Name = Constants.Periodicty.Month)]
    Month = 0,
    [XmlEnum(Name = Constants.Periodicty.Day)]
    Day = 1,
    [XmlEnum(Name = Constants.Periodicty.Week)]
    Week = 2,
    [XmlEnum(Name = Constants.Periodicty.Fortnight)]
    Fortnight = 3,
    [XmlEnum(Name = Constants.Periodicty.ThreeMonth)]
    ThreeMonth = 4,
    [XmlEnum(Name = Constants.Periodicty.SixMonth)]
    SixMonth = 5,
    [XmlEnum(Name = Constants.Periodicty.Year)]
    Year = 6,
}
