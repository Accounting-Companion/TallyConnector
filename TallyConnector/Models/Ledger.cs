using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [Serializable]
    [XmlRoot("LEDGER")]
    public class Ledger : TallyXmlJson
    {
        public Ledger()
        {
            FAddress = new HAddress();
            InterestList = new();
            LanguageNameList = new();
            ClosingBalances = new();
            MultipleAddresses = new();
        }

        [XmlElement(ElementName = "MASTERID")]
        [MaxLength(20)]
        public int? TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        [JsonIgnore]
        [Column(TypeName = "nvarchar(60)")]
        public string OldName { get; set; }

        private string name;
        [XmlElement(ElementName = "NAME")]
        [Required]
        [Column(TypeName = "nvarchar(60)")]
        public string Name
        {
            get { return (name == null || name == string.Empty) ? OldName : name; }
            set => name = value;
        }


        [XmlElement(ElementName = "PARENT")]
        [Required]
        [Column(TypeName = "nvarchar(60)")]
        public string Group { get; set; }


        [XmlIgnore]
        [Column(TypeName = "nvarchar(60)")]
        public string Alias { get; set; }

        [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo DeemedPositive { get; set; }
        [XmlIgnore]
        [Column(TypeName = "nvarchar(20)")]
        public string ForexAmount { get; set; }
        [XmlIgnore]
        [Column(TypeName = "nvarchar(20)")]
        public string RateofExchange { get; set; }


        private string _OpeningBal;

        [XmlElement(ElementName = "OPENINGBALANCE")]
        [Column(TypeName = "nvarchar(20)")]
        public string OpeningBal
        {
            get
            {
                if (ForexAmount != null && RateofExchange != null)
                {
                    _OpeningBal = $"{ForexAmount} @ {RateofExchange}";
                }
                else if (ForexAmount != null)
                {
                    _OpeningBal = ForexAmount;
                }
                return _OpeningBal;
            }
            set
            {
                if (value != null)
                {
                    double t_opbal;
                    Match CleanedAmount;
                    if (value.ToString().Contains('='))
                    {
                        List<string> SplittedValues = value.ToString().Split('=').ToList();
                        CleanedAmount = Regex.Match(SplittedValues[1], @"[0-9.]+");
                        bool Isnegative = SplittedValues[1].Contains('-');
                        bool sucess = Isnegative ? double.TryParse('-' + CleanedAmount.Value, out t_opbal) : double.TryParse(CleanedAmount.ToString(), out t_opbal);
                        CleanedOpeningBal = t_opbal;
                        var ForexInfo = SplittedValues[0].Split('@');
                        ForexAmount = ForexInfo[0].Trim();
                        RateofExchange = Regex.Match(ForexInfo[1], @"[0-9.]+").Value;
                    }
                    else
                    {
                        double.TryParse(value, out t_opbal);
                        CleanedOpeningBal = t_opbal;
                        _OpeningBal = value;
                    }
                }
                else
                {
                    _OpeningBal = value;
                }


            }
        }
        [XmlIgnore]
        [MaxLength(20)]
        public double? CleanedOpeningBal { get; set; }

        [XmlIgnore]
        [Column(TypeName = "nvarchar(60)")]
        public string ClosingForexAmount { get; set; }
        [XmlIgnore]
        [Column(TypeName = "nvarchar(60)")]
        public string ClosingRateofExchange { get; set; }
        [XmlIgnore]
        [MaxLength(20)]
        public double? CleanedClosingBal { get; set; }

        private string _ClosingBal;

        [XmlElement(ElementName = "CLOSINGBALANCE")]
        [Column(TypeName = "nvarchar(60)")]
        public string ClosingBal
        {
            get
            {
                if (ClosingForexAmount != null && ClosingRateofExchange != null)
                {
                    _OpeningBal = $"{ClosingForexAmount} @ {ClosingRateofExchange}";
                }
                else if (ClosingForexAmount != null)
                {
                    _ClosingBal = ClosingForexAmount;
                }
                return _ClosingBal;
            }
            set
            {
                if (value != null)
                {
                    double t_opbal;
                    if (value.ToString().Contains('='))
                    {
                        List<string> SplittedValues = value.ToString().Split('=').ToList();
                        var CleanedAmount = Regex.Match(SplittedValues[1], @"[0-9.]+");
                        bool Isnegative = SplittedValues[1].Contains('-');
                        bool sucess = Isnegative ? double.TryParse('-' + CleanedAmount.Value, out t_opbal) : double.TryParse(CleanedAmount.ToString(), out t_opbal);
                        CleanedClosingBal = t_opbal;
                        var ForexInfo = SplittedValues[0].Split('@');
                        ClosingForexAmount = ForexInfo[0].Trim();
                        ClosingRateofExchange = Regex.Match(ForexInfo[1], @"[0-9.]+").Value;
                    }
                    else
                    {
                        double.TryParse(value, out t_opbal);
                        CleanedClosingBal = t_opbal;
                        _ClosingBal = value;
                    }
                }
                else
                {
                    _ClosingBal = value;
                }
            }

        }


        private string _Currency;
        [XmlElement(ElementName = "CURRENCYNAME")]
        [Column(TypeName = "nvarchar(5)")]
        public string Currency
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

        [XmlElement(ElementName = "TAXTYPE")]
        [Column(TypeName = "nvarchar(20)")]
        public TaxType TaxType { get; set; }

        [XmlElement(ElementName = "GSTDUTYHEAD")]
        [Column(TypeName = "nvarchar(15)")]
        public GSTTaxType GSTTaxType { get; set; }

        [XmlElement(ElementName = "RATEOFTAXCALCULATION")]
        [MaxLength(3)]
        public double? RateofTax { get; set; }

        [XmlElement(ElementName = "ISBILLWISEON")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo IsBillwise { get; set; }

        [XmlElement(ElementName = "BILLCREDITPERIOD")]
        [Column(TypeName = "nvarchar(10)")]
        public string CreditPeriod { get; set; }

        [XmlElement(ElementName = "ISCREDITDAYSCHKON")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo IsCreditCheck { get; set; }


        [XmlElement(ElementName = "CREDITLIMIT", IsNullable = true)]
        [MaxLength(20)]
        public string CreditLimit { get; set; }


        [XmlElement(ElementName = "MAILINGNAME")]
        [Column(TypeName = "nvarchar(60)")]
        public string MailingName { get; set; }


        [XmlIgnore]
        public string Address
        {
            get
            {
                return FAddress.FullAddress;
            }

            set
            {
                if (value != "")
                {

                    this.FAddress.FullAddress = value;
                }


            }

        }

        [XmlElement(ElementName = "COUNTRYNAME")]
        [Column(TypeName = "nvarchar(60)")]
        public string Country { get; set; }

        [XmlElement(ElementName = "LEDSTATENAME")]
        [Column(TypeName = "nvarchar(100)")]
        public string State { get; set; }

        [XmlElement(ElementName = "PINCODE")]
        [Column(TypeName = "nvarchar(15)")]
        public string PinCode { get; set; }

        [XmlElement(ElementName = "LEDGERCONTACT")]
        [Column(TypeName = "nvarchar(20)")]
        public string ContactPerson { get; set; }

        [XmlElement(ElementName = "LEDGERPHONE")]
        [Column(TypeName = "nvarchar(20)")]
        public string LandlineNo { get; set; }

        [XmlElement(ElementName = "LEDGERMOBILE")]
        [Column(TypeName = "nvarchar(20)")]
        public string MobileNo { get; set; }

        [XmlElement(ElementName = "LEDGERFAX")]
        [Column(TypeName = "nvarchar(20)")]
        public string FaxNo { get; set; }

        [XmlElement(ElementName = "EMAIL")]
        [Column(TypeName = "nvarchar(60)")]
        public string Email { get; set; }

        [XmlElement(ElementName = "EMAILCC")]
        [Column(TypeName = "nvarchar(60)")]
        public string Emailcc { get; set; }

        [XmlElement(ElementName = "WEBSITE")]
        [Column(TypeName = "nvarchar(60)")]
        public string Website { get; set; }

        [XmlElement(ElementName = "INCOMETAXNUMBER")]
        [Column(TypeName = "nvarchar(12)")]
        public string PANNumber { get; set; }

        [XmlElement(ElementName = "GSTREGISTRATIONTYPE")]
        [Column(TypeName = "nvarchar(15)")]
        public GSTRegistrationType GSTRegistrationType { get; set; }

        [XmlElement(ElementName = "ISOTHTERRITORYASSESSEE")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo IsOtherTerritoryAssessee { get; set; }

        [XmlElement(ElementName = "PARTYGSTIN")]
        [Column(TypeName = "nvarchar(17)")]
        public string GSTIN { get; set; }

        [XmlElement(ElementName = "ISECOMMOPERATOR")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo IsECommerceOperator { get; set; }

        [XmlElement(ElementName = "CONSIDERPURCHASEFOREXPORT")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo DeemedExport { get; set; }

        [XmlElement(ElementName = "GSTNATUREOFSUPPLY")]
        [Column(TypeName = "nvarchar(20)")]
        public GSTPartyType GSTPartyType { get; set; }

        [XmlElement(ElementName = "ISTRANSPORTER")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo IsTransporter { get; set; }

        [XmlElement(ElementName = "TRANSPORTERID")]
        [Column(TypeName = "nvarchar(20)")]
        public string TransporterID { get; set; }


        [XmlElement(ElementName = "AFFECTSSTOCK")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo AffectStock { get; set; }

        [XmlElement(ElementName = "ISCOSTCENTRESON")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo IsCostcenter { get; set; }

        [XmlElement(ElementName = "ISREVENUE")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo Isrevenue { get; set; }

        [XmlElement(ElementName = "ISINTERESTON")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo Isintereston { get; set; }

        [XmlElement(ElementName = "INTERESTONBILLWISE")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo IsinterestonBillWise { get; set; }

        [XmlElement(ElementName = "OVERRIDEINTEREST")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo OverrideInterest { get; set; }

        [XmlElement(ElementName = "OVERRIDEADVINTEREST")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo OverrideAdvanceInterest { get; set; }

        [XmlElement(ElementName = "INTERESTINCLDAYOFADDITION")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo InterestIncludeForAmountsAdded { get; set; }

        [XmlElement(ElementName = "INTERESTINCLDAYOFDEDUCTION")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo InterestIncludeForAmountsDeducted { get; set; }


        [XmlElement(ElementName = "INTERESTCOLLECTION.LIST")]
        public List<InterestList> InterestList { get; set; }


        [XmlElement(ElementName = "FORPAYROLL")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo Forpayroll { get; set; }


        [XmlElement(ElementName = "DESCRIPTION")]
        public string Description { get; set; }

        [XmlElement(ElementName = "NARRATION")]
        public string Notes { get; set; }

        [JsonIgnore]
        [XmlElement(ElementName = "ADDRESS.LIST")]
        public HAddress FAddress { get; set; }


        [JsonIgnore]
        [XmlElement(ElementName = "LANGUAGENAME.LIST")]
        public List<LanguageNameList> LanguageNameList { get; set; }

        [XmlElement(ElementName = "LEDMULTIADDRESSLIST.LIST")]
        public List<MultiAddress> MultipleAddresses { get; set; }


        [XmlElement(ElementName = "LEDGERCLOSINGVALUES.LIST")]
        public List<ClosingBalances> ClosingBalances { get; set; }



        [XmlElement(ElementName = "CANDELETE")]
        [Column(TypeName = "nvarchar(3)")]
        public YesNo CanDelete { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public Action Action { get; set; }

        [XmlElement(ElementName = "GUID")]
        [Column(TypeName = "nvarchar(100)")]
        public string GUID { get; set; }

        public void CreateNamesList()
        {
            if (this.LanguageNameList.Count == 0)
            {
                this.LanguageNameList.Add(new LanguageNameList());
                this.LanguageNameList[0].NameList.NAMES.Add(this.Name);

            }
            if (this.Alias != null && this.Alias != string.Empty)
            {
                this.LanguageNameList[0].LanguageAlias = this.Alias;
            }
        }
    }

    [XmlRoot(ElementName = "INTERESTCOLLECTION.LIST")]
    public class InterestList
    {
        [XmlElement(ElementName = "INTERESTFROMDATE")]
        [Column(TypeName = "nvarchar(10)")]
        public string FromDate { get; set; }

        [XmlElement(ElementName = "INTERESTTODATE")]
        [Column(TypeName = "nvarchar(10)")]
        public string ToDate { get; set; }

        [XmlElement(ElementName = "INTERESTSTYLE")]
        [Column(TypeName = "nvarchar(20)")]
        public InterestStyle InterestStyle { get; set; }

        [XmlElement(ElementName = "INTERESTBALANCETYPE")]
        [Column(TypeName = "nvarchar(25)")]
        public InterestBalancetype InterestBalancetype { get; set; }

        [XmlElement(ElementName = "INTERESTAPPLON")]
        [Column(TypeName = "nvarchar(20)")]
        public InterestAppliedOn Interestappliedon { get; set; }

        [XmlElement(ElementName = "INTERESTFROMTYPE")]
        [Column(TypeName = "nvarchar(30)")]
        public InterestFromType Interestfromtype { get; set; }

        [XmlElement(ElementName = "ROUNDTYPE")]
        [Column(TypeName = "nvarchar(25)")]
        public RoundType RoundType { get; set; }

        [XmlElement(ElementName = "INTERESTRATE")]
        [MaxLength(3)]
        public double? InterestRate { get; set; }

        [XmlElement(ElementName = "INTERESTAPPLFROM")]
        [MaxLength(3)]
        public double? Interestappliedfrom { get; set; }

        [XmlElement(ElementName = "ROUNDLIMIT")]
        [MaxLength(3)]
        public double? RoundLimit { get; set; }

    }



    [XmlRoot(ElementName = "LEDGERCLOSINGVALUES.LIST")]
    public class ClosingBalances
    {

        [XmlElement(ElementName = "DATE")]
        [Column(TypeName = "nvarchar(10)")]
        public string Date { get; set; }

        [XmlElement(ElementName = "AMOUNT")]
        [Column(TypeName = "nvarchar(20)")]
        public string Amount { get; set; }

    }

    [XmlRoot(ElementName = "ENVELOPE")]
    public class LedgerEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public LBody Body { get; set; } = new LBody();
    }

    [XmlRoot(ElementName = "BODY")]
    public class LBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new Description();

        [XmlElement(ElementName = "DATA")]
        public LData Data { get; set; } = new LData();
    }

    [XmlRoot(ElementName = "DATA")]
    public class LData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public LedgerMessage Message { get; set; } = new LedgerMessage();

        [XmlElement(ElementName = "COLLECTION")]
        public LedgColl Collection { get; set; } = new LedgColl();


    }

    [XmlRoot(ElementName = "COLLECTION")]
    public class LedgColl
    {
        [XmlElement(ElementName = "LEDGER")]
        public List<Ledger> Ledgers { get; set; }
    }
    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class LedgerMessage
    {
        [XmlElement(ElementName = "LEDGER")]
        public Ledger Ledger { get; set; }
    }





}
