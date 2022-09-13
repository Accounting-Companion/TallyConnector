namespace TallyConnector.Core.Models.Masters;

[Serializable]
[XmlRoot("LEDGER")]
[XmlType(AnonymousType = true)]
public class Ledger : BasicTallyObject, ITallyObject
{
    private string? name;
    public Ledger()
    {
        LanguageNameList = new();
        FAddress = new HAddress();
        Group = string.Empty;
    }

    /// <summary>
    /// Create Ledger under Specified Group
    /// </summary>
    /// <param name="name"></param>
    /// <param name="group"></param>
    public Ledger(string name, string group)
    {
        LanguageNameList = new();
        FAddress = new HAddress();
        Group = group;
        this.name = name;
    }

    [XmlAttribute(AttributeName = "NAME")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? OldName { get; set; }

    [XmlElement(ElementName = "NAME")]
    [Required]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string Name
    {
        get
        {
            name = name == null || name == string.Empty ? OldName : name;
            return name!;
        }
        set => name = value;
    }

    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [Required]
    public string Group { get; set; }

    [XmlElement(ElementName = "PARENTID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? GroupId { get; set; }


    [XmlIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Alias { get; set; }

    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsDeemedPositive
    {
        get
        {
            if (OpeningBal != null)
            {
                return OpeningBal.IsDebit;
            }
            return null;

        }
        set { }
    }

    [XmlElement(ElementName = "OPENINGBALANCE")]
    public TallyAmount? OpeningBal { get; set; }

    //[XmlIgnore]
    //[Column(TypeName = $"nvarchar({Constants.MaxAmountLength})")]
    //public string? ClosingForexAmount { get; set; }
    //[XmlIgnore]
    //[Column(TypeName = $"nvarchar({Constants.MaxRateLength})")]
    //public string? ClosingRateofExchange { get; set; }
    //[XmlIgnore]
    //public double? CleanedClosingBal { get; set; }

    //private string? _ClosingBal;

    //[XmlElement(ElementName = "CLOSINGBALANCE")]
    //[Column(TypeName = $"nvarchar({Constants.MaxAmountLength})")]
    //public string? ClosingBal
    //{
    //    get
    //    {
    //        if (ClosingForexAmount != null && ClosingRateofExchange != null)
    //        {
    //            _OpeningBal = $"{ClosingForexAmount} @ {ClosingRateofExchange}";
    //        }
    //        else if (ClosingForexAmount != null)
    //        {
    //            _ClosingBal = ClosingForexAmount;
    //        }
    //        return _ClosingBal;
    //    }
    //    set
    //    {
    //        if (value != null)
    //        {
    //            double t_opbal;
    //            if (value.ToString().Contains('='))
    //            {
    //                List<string> SplittedValues = value.ToString().Split('=').ToList();
    //                var CleanedAmount = Regex.Match(SplittedValues[1], @"[0-9.]+");
    //                bool Isnegative = SplittedValues[1].Contains('-');
    //                bool sucess = Isnegative ? double.TryParse('-' + CleanedAmount.Value, out t_opbal) : double.TryParse(CleanedAmount.ToString(), out t_opbal);
    //                CleanedClosingBal = t_opbal;
    //                var ForexInfo = SplittedValues[0].Split('@');
    //                ClosingForexAmount = ForexInfo[0].Trim();
    //                ClosingRateofExchange = Regex.Match(ForexInfo[1], @"[0-9.]+").Value;
    //            }
    //            else
    //            {
    //                CleanedClosingBal = double.TryParse(value, out t_opbal) ? t_opbal : 0;
    //                _ClosingBal = value;
    //            }
    //        }
    //        else
    //        {
    //            _ClosingBal = value;
    //        }
    //    }

    //}


    private string? _Currency;
    [XmlElement(ElementName = "CURRENCYNAME")]
    [Column(TypeName = "nvarchar(5)")]
    public string? Currency
    {
        get { return _Currency; }
        set
        {
            if (value == "?")
            {
                _Currency = null;
            }
            else
            {
                _Currency = value;
            }
        }
    }

    [XmlElement(ElementName = "CURRENCYID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? CurrencyId { get; set; }

    [XmlElement(ElementName = "TAXTYPE")]
    [Column(TypeName = "nvarchar(20)")]
    public TaxType TaxType { get; set; }

    [XmlElement(ElementName = "GSTDUTYHEAD")]
    [Column(TypeName = "nvarchar(15)")]
    public GSTTaxType GSTTaxType { get; set; }

    [XmlElement(ElementName = "RATEOFTAXCALCULATION")]
    public double? RateofTax { get; set; }

    [XmlElement(ElementName = "ISBILLWISEON")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsBillwise { get; set; }

    [XmlElement(ElementName = "BILLCREDITPERIOD")]
    [Column(TypeName = $"nvarchar({Constants.MaxDateLength})")]
    public string? CreditPeriod { get; set; }

    [XmlElement(ElementName = "ISCREDITDAYSCHKON")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsCreditCheck { get; set; }


    [XmlElement(ElementName = "CREDITLIMIT")]
    [MaxLength(20)]
    public string? CreditLimit { get; set; }


    [XmlElement(ElementName = "MAILINGNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? MailingName { get; set; }


    [XmlIgnore]
    public string? Address
    {
        get
        {
            return FAddress.FullAddress;
        }

        set
        {
            if (value != "")
            {

                FAddress.FullAddress = value;
            }


        }

    }

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
    [Column(TypeName = $"nvarchar({Constants.MaxAmountLength})")]
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
    public TallyYesNo? IsOtherTerritoryAssessee { get; set; }

    [XmlElement(ElementName = "PARTYGSTIN")]
    [Column(TypeName = "nvarchar(17)")]
    public string? GSTIN { get; set; }

    [XmlElement(ElementName = "ISECOMMOPERATOR")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsECommerceOperator { get; set; }

    [XmlElement(ElementName = "CONSIDERPURCHASEFOREXPORT")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? DeemedExport { get; set; }

    [XmlElement(ElementName = "GSTNATUREOFSUPPLY")]
    [Column(TypeName = "nvarchar(20)")]
    public GSTPartyType GSTPartyType { get; set; }

    [XmlElement(ElementName = "ISTRANSPORTER")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsTransporter { get; set; }

    [XmlElement(ElementName = "TRANSPORTERID")]
    [Column(TypeName = "nvarchar(20)")]
    public string? TransporterID { get; set; }


    [XmlElement(ElementName = "AFFECTSSTOCK")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? AffectStock { get; set; }

    [XmlElement(ElementName = "ISCOSTCENTRESON")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsCostCenter { get; set; }

    [XmlElement(ElementName = "ISREVENUE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsRevenue { get; set; }

    [XmlElement(ElementName = "ISINTERESTON")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsInterestOn { get; set; }

    [XmlElement(ElementName = "INTERESTONBILLWISE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsInterestOnBillWise { get; set; }

    [XmlElement(ElementName = "OVERRIDEINTEREST")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? OverrideInterest { get; set; }

    [XmlElement(ElementName = "OVERRIDEADVINTEREST")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? OverrideAdvanceInterest { get; set; }

    [XmlElement(ElementName = "INTERESTINCLDAYOFADDITION")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? InterestIncludeForAmountsAdded { get; set; }

    [XmlElement(ElementName = "INTERESTINCLDAYOFDEDUCTION")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? InterestIncludeForAmountsDeducted { get; set; }


    [XmlElement(ElementName = "INTERESTCOLLECTION.LIST", IsNullable = true)]
    [TDLCollection(CollectionName = "Interest Collection")]
    public List<InterestList>? InterestList { get; set; }


    [XmlElement(ElementName = "FORPAYROLL")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? ForPayroll { get; set; }


    [XmlElement(ElementName = "DESCRIPTION")]
    [Column(TypeName = $"nvarchar({Constants.MaxNarrLength})")]
    public string? Description { get; set; }

    [XmlElement(ElementName = "NARRATION")]
    [Column(TypeName = $"nvarchar({Constants.MaxNarrLength})")]
    public string? Notes { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "ADDRESS.LIST")]
    public HAddress FAddress { get; set; }


    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    [XmlElement(ElementName = "LEDMULTIADDRESSLIST.LIST")]
    public List<MultiAddress>? MultipleAddresses { get; set; }


    [XmlElement(ElementName = "LEDGERCLOSINGVALUES.LIST")]
    public List<ClosingBalances>? ClosingBalances { get; set; }



    [XmlElement(ElementName = "CANDELETE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? CanDelete { get; set; }


    public void CreateNamesList()
    {
        if (LanguageNameList.Count == 0)
        {
            LanguageNameList.Add(new LanguageNameList());
            LanguageNameList?[0]?.NameList?.NAMES?.Add(Name);

        }
        if (Alias != null && Alias != string.Empty)
        {
            LanguageNameList![0].LanguageAlias = Alias;
        }
    }

    public new string GetXML(XmlAttributeOverrides? attrOverrides = null)
    {
        CreateNamesList();
        return base.GetXML(attrOverrides);
    }

    public new void PrepareForExport()
    {
        if (Group != null && Group.Contains("Primary"))
        {
            Group = string.Empty;
        }
        CreateNamesList();
    }

    public override string ToString()
    {
        return $"Ledger - {Name}";
    }

    /// <summary>
    /// Removes Null Childs that are created during xml deserilisation
    /// </summary>
    public override void RemoveNullChilds()
    {
        InterestList = InterestList?.Where(IntList => !IntList.IsNull())?.ToList();
        if (InterestList?.Count == 0)
        {
            InterestList = null;
        }
        ClosingBalances = ClosingBalances?.Where(ClsBal => !ClsBal.IsNull())?.ToList();
        if (ClosingBalances?.Count == 0)
        {
            ClosingBalances = null;
        }

        MultipleAddresses = MultipleAddresses?.Where(MulAdress => !MulAdress.IsNull())?.ToList();
        if (MultipleAddresses?.Count == 0)
        {
            MultipleAddresses = null;
        }
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
public class ClosingBalances : ICheckNull
{

    [XmlElement(ElementName = "DATE")]
    public TallyDate? Date { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmount? Amount { get; set; }

    public bool IsNull()
    {
        if (Date is null
            && Amount is null || Amount?.Amount == 0)
        {
            return true;
        }
        return false;
    }
}
