namespace TallyConnector.Models;

public enum MasterLookupField
{
    MasterId = 1,
    AlterId = 2,
    Name = 3,
    GUID = 4,
}

public enum Action
{
    [XmlEnum(Name = "Create")]
    Create = 0,
    [XmlEnum(Name = "Alter")]
    Alter = 1,
    [XmlEnum(Name = "Delete")]
    Delete = 2,
    [XmlEnum(Name = "Cancel")]
    Cancel = 3,
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
    [XmlEnum(Name = " Not Applicable")]
    NotApplicable = 0,
    [XmlEnum(Name = "Appropriate by Qty")]
    AppropriateByQty = 1,
    [XmlEnum(Name = "Appropriate by Value")]
    AppropriateByValue = 2,
}

public enum TaxType
{
    [XmlEnum(Name = "Others")]
    Others = 0,
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
}

public enum GSTTaxType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Central Tax")]
    CentralTax = 1,
    [XmlEnum(Name = "Cess")]
    Cess = 2,
    [XmlEnum(Name = "Integrated Tax")]
    IntegratedTax = 3,
    [XmlEnum(Name = "UT Tax")]
    UTTax = 4,
    [XmlEnum(Name = "State Tax")]
    StateTax = 5,
}
public enum ExciseNatureOfPurchase
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = " Any")]
    Any = 1,
    [XmlEnum(Name = "Importer")]
    Importer = 2,
}
public enum GSTRegistrationType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = " Unknown")]
    Unknown = 0,
    [XmlEnum(Name = "Composition")]
    Composition = 1,
    [XmlEnum(Name = "Consumer")]
    Consumer = 2,
    [XmlEnum(Name = "Regular")]
    Regular = 3,
    [XmlEnum(Name = "Unregistered")]
    Unregistered = 4,

}

public enum GSTPartyType
{

    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = " Not Applicable")]
    NotApplicable = 0,
    [XmlEnum(Name = "Deemed Export")]
    DeemedExport = 1,
    [XmlEnum(Name = "Embassy/UN Body")]
    Embassy_UN_Body = 2,
    [XmlEnum(Name = "SEZ")]
    SEZ = 3,
    [XmlEnum(Name = "No")]
    No = 4,
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

public enum InterestBalancetype
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
    [XmlEnum(Name = " Not Applicable")]
    NotApplicable = 0,
    [XmlEnum(Name = "Downward Rounding")]
    Dowward = 1,
    [XmlEnum(Name = "Normal Rounding")]
    Normal = 2,
    [XmlEnum(Name = "Upward Rounding")]
    Upward = 3,
}

