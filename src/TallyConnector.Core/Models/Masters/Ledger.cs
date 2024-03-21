using TallyConnector.Core.Models.Interfaces.Masters;
using TallyConnector.Core.Models.TallyComplexObjects;

namespace TallyConnector.Core.Models.Masters;

[XmlRoot("LEDGER")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.Ledgers)]
public class BaseLedger : BaseMasterObject, IBaseLedger
{


    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [Required]
    public string Group { get; set; }
}


[XmlRoot("LEDGER")]
[XmlType(AnonymousType = true)]
public partial class Ledger : BaseLedger
{

    public Ledger()
    {
        //LanguageNameList = [];
        Addresses = [];
        OldName = Group = string.Empty;
    }

    /// <summary>
    /// Create Ledger under Specified Group
    /// </summary>
    /// <param name="name"></param>
    /// <param name="group"></param>
    public Ledger(string name, string group)
    {
        //LanguageNameList = [];
        Addresses = [];
        Group = group;
    }

    [XmlElement(ElementName = "OLDNAME")]
    [TDLField(Set = "$Name")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string OldName { get; set; }


    [XmlElement(ElementName = "PARENTID")]
    [TDLField(Set = "$guid:group:$parent")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? GroupId { get; set; }


    [XmlIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [TDLField(Set = "$_FirstAlias")]
    public string? Alias { get; set; }

    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool IsDeemedPositive { get; set; }

    [XmlElement(ElementName = "OPENINGBALANCE")]
    public TallyAmountField? OpeningBal { get; set; }

    [XmlElement(ElementName = "CURRENCYNAME")]
    [Column(TypeName = "nvarchar(5)")]
    public string? Currency { get; set; }

    [XmlElement(ElementName = "CURRENCYID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    [TDLField(Set = "$GUID:Currency:$CURRENCYNAME")]
    public string? CurrencyId { get; set; }

    [XmlElement(ElementName = "TAXTYPE")]
    [Column(TypeName = "nvarchar(20)")]
    public TaxType TaxType { get; set; }

    [XmlElement(ElementName = "GSTDUTYHEAD")]
    [Column(TypeName = "nvarchar(15)")]
    public GSTTaxType? GSTTaxType { get; set; }

    [XmlElement(ElementName = "RATEOFTAXCALCULATION")]
    public double? RateofTax { get; set; }

    [XmlElement(ElementName = "ISBILLWISEON")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsBillwise { get; set; }

    [XmlElement(ElementName = "BILLCREDITPERIOD")]
    [Column(TypeName = $"nvarchar({Constants.MaxDateLength})")]
    public string? CreditPeriod { get; set; }

    [XmlElement(ElementName = "ISCREDITDAYSCHKON")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsCreditCheck { get; set; }


    [XmlElement(ElementName = "CREDITLIMIT")]
    [MaxLength(20)]
    public string? CreditLimit { get; set; }


    [XmlElement(ElementName = "MAILINGNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? MailingName { get; set; }


    [XmlElement(ElementName = "COUNTRYNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Country { get; set; }

    [XmlElement(ElementName = "LEDSTATENAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxAmountLength})")]
    public string? State { get; set; }

    [XmlElement(ElementName = "PINCODE")]
    [Column(TypeName = $"nvarchar({Constants.MaxAmountLength})")]
    public string? PinCode { get; set; }

    [XmlElement(ElementName = "LEDGERCONTACT")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? ContactPerson { get; set; }

    [XmlElement(ElementName = "LEDGERPHONE")]
    [Column(TypeName = "nvarchar(20)")]
    public string? LandlineNo { get; set; }

    [XmlElement(ElementName = "LEDGERMOBILE")]
    [Column(TypeName = "nvarchar(20)")]
    public string? MobileNo { get; set; }

    [XmlElement(ElementName = "LEDGERFAX")]
    [Column(TypeName = "nvarchar(20)")]
    public string? FaxNo { get; set; }

    [XmlElement(ElementName = "EMAIL")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Email { get; set; }

    [XmlElement(ElementName = "EMAILCC")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Emailcc { get; set; }

    [XmlElement(ElementName = "WEBSITE")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Website { get; set; }

    [XmlElement(ElementName = "INCOMETAXNUMBER")]
    [Column(TypeName = "nvarchar(12)")]
    public string? PANNumber { get; set; }

    [XmlElement(ElementName = "GSTREGISTRATIONTYPE")]
    [Column(TypeName = "nvarchar(15)")]
    public GSTRegistrationType GSTRegistrationType { get; set; }

    [XmlElement(ElementName = "ISOTHTERRITORYASSESSEE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsOtherTerritoryAssessee { get; set; }

    [XmlElement(ElementName = "PARTYGSTIN")]
    [Column(TypeName = "nvarchar(17)")]
    public string? GSTIN { get; set; }

    [XmlElement(ElementName = "ISECOMMOPERATOR")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsECommerceOperator { get; set; }

    [XmlElement(ElementName = "CONSIDERPURCHASEFOREXPORT")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? DeemedExport { get; set; }

    [XmlElement(ElementName = "GSTNATUREOFSUPPLY")]
    [Column(TypeName = "nvarchar(20)")]
    public GSTPartyType? GSTPartyType { get; set; }

    [XmlElement(ElementName = "ISTRANSPORTER")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsTransporter { get; set; }

    [XmlElement(ElementName = "TRANSPORTERID")]
    [Column(TypeName = "nvarchar(20)")]
    public string? TransporterID { get; set; }


    [XmlElement(ElementName = "AFFECTSSTOCK")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? AffectStock { get; set; }

    [XmlElement(ElementName = "ISCOSTCENTRESON")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsCostCenter { get; set; }

    [XmlElement(ElementName = "ISREVENUE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsRevenue { get; set; }

    [XmlElement(ElementName = "ISINTERESTON")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsInterestOn { get; set; }

    [XmlElement(ElementName = "INTERESTONBILLWISE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsInterestOnBillWise { get; set; }

    [XmlElement(ElementName = "OVERRIDEINTEREST")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? OverrideInterest { get; set; }

    [XmlElement(ElementName = "OVERRIDEADVINTEREST")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? OverrideAdvanceInterest { get; set; }

    [XmlElement(ElementName = "INTERESTINCLDAYOFADDITION")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? InterestIncludeForAmountsAdded { get; set; }

    [XmlElement(ElementName = "INTERESTINCLDAYOFDEDUCTION")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? InterestIncludeForAmountsDeducted { get; set; }


    //[XmlElement(ElementName = "INTERESTCOLLECTION.LIST", IsNullable = true)]
    //[TDLCollection(CollectionName = "Interest Collection")]
    //public List<InterestList>? InterestList { get; set; }


    [XmlElement(ElementName = "FORPAYROLL")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? ForPayroll { get; set; }


    [XmlElement(ElementName = "DESCRIPTION")]
    [Column(TypeName = $"nvarchar({Constants.MaxNarrLength})")]
    public string? Description { get; set; }

    [XmlElement(ElementName = "NARRATION")]
    [Column(TypeName = $"nvarchar({Constants.MaxNarrLength})")]
    public string? Notes { get; set; }

    [JsonIgnore]

    [XmlArray(ElementName = "ADDRESS.LIST")]
    [XmlArrayItem(ElementName = "ADDRESS")]
    [TDLCollection(CollectionName = "Address", ExplodeCondition = "$$NumItems:ADDRESS<1")]
    public List<string> Addresses { get; set; }


    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    ////[XmlElement(ElementName = "LEDMULTIADDRESSLIST.LIST")]
    ////public List<MultiAddress>? MultipleAddresses { get; set; }


    [XmlElement(ElementName = "LEDGERCLOSINGVALUES.LIST")]
    [TDLCollection(CollectionName = "LEDGERCLOSINGVALUES", ExplodeCondition = "$$NUMITEMS:LEDGERCLOSINGVALUES > 0")]
    public List<ClosingBalances>? ClosingBalances { get; set; }

    //[XmlElement(ElementName = "GSTDETAILS.LIST")]
    //public List<GSTDetail>? GSTDetails { get; set; }


    //[XmlElement(ElementName = "LEDGSTREGDETAILS.LIST")]
    //public List<LedgerGSTRegistrationDetails>? LedgerGSTRegistrationDetails { get; set; }

    [XmlElement(ElementName = "CANDELETE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? CanDelete { get; set; }






    public override string ToString()
    {
        return $"Ledger - {Name}";
    }

}

[XmlRoot(ElementName = "INTERESTCOLLECTION.LIST", IsNullable = true)]
public class InterestList : ICheckNull
{
    public InterestList()
    {
    }

    [XmlElement(ElementName = "INTERESTFROMDATE")]
    public TallyDate? FromDate { get; set; }

    [XmlElement(ElementName = "INTERESTTODATE")]
    public TallyDate? ToDate { get; set; }

    [XmlElement(ElementName = "INTERESTSTYLE")]
    [Column(TypeName = "nvarchar(20)")]
    public InterestStyle? InterestStyle { get; set; }

    [XmlElement(ElementName = "INTERESTBALANCETYPE")]
    [Column(TypeName = "nvarchar(25)")]
    public InterestBalanceType? InterestBalanceType { get; set; }

    [XmlElement(ElementName = "INTERESTAPPLON")]
    [Column(TypeName = "nvarchar(20)")]
    public InterestAppliedOn? InterestAppliedOn { get; set; }

    [XmlElement(ElementName = "INTERESTFROMTYPE")]
    [Column(TypeName = "nvarchar(30)")]
    public InterestFromType? InterestFromType { get; set; }

    [XmlElement(ElementName = "ROUNDTYPE")]
    [Column(TypeName = "nvarchar(25)")]
    public RoundType? RoundType { get; set; }

    [XmlElement(ElementName = "INTERESTRATE")]
    public double? InterestRate { get; set; }

    [XmlElement(ElementName = "INTERESTAPPLFROM")]
    public double? InterestAppliedFrom { get; set; }

    [XmlElement(ElementName = "ROUNDLIMIT")]
    public double? RoundLimit { get; set; }

    public bool IsNull()
    {
        if (FromDate is null
            && ToDate is null
            && (InterestStyle is null || InterestStyle is Models.InterestStyle.None)
            && (InterestBalanceType is null || InterestBalanceType is Models.InterestBalanceType.None)
            && (InterestAppliedOn is null || InterestAppliedOn is Models.InterestAppliedOn.None)
            && (InterestFromType is null || InterestFromType is Models.InterestFromType.None)
            && (RoundType is null || RoundType is Models.RoundType.None)
            && (InterestRate is null || InterestRate is 0)
            && (InterestAppliedFrom is null || InterestAppliedFrom is 0)
            && (RoundLimit is null || RoundLimit is 0))
        {
            return true;
        }
        return false;
    }
}



[XmlRoot(ElementName = "LEDGERCLOSINGVALUES.LIST")]
public class ClosingBalances
{

    [XmlElement(ElementName = "DATE")]
    public DateTime? Date { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmountField? Amount { get; set; }

}
