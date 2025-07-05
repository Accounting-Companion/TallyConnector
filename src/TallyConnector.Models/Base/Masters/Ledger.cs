using TallyConnector.Models.Common;

namespace TallyConnector.Models.Base.Masters;

[TDLCollection(Type = "Ledger")]
[XmlRoot("LEDGER")]
[XmlType(AnonymousType = true)]
public partial class Ledger : BaseAliasedMasterObject
{
    public Ledger()
    {

    }

    /// <summary>
    /// Name of parent group
    /// </summary>
    [XmlElement(ElementName = "PARENT")]
    public string? Group { get; set; }

    [XmlElement(ElementName = "OPENINGBALANCE")]
    [TDLCollection(ExplodeCondition = "$$ISEMPTY:$OPENINGBALANCE")]
    public TallyAmountField? OpeningBalance { get; set; }

    [XmlElement(ElementName = "CURRENCY")]
    public string? Currency { get; set; }

    [XmlElement(ElementName = "TAXTYPE")]
    public TaxType? TaxType { get; set; }

    [XmlElement(ElementName = "GSTTYPE")]
    public GSTTaxType? GSTTaxType { get; set; }

    [XmlElement(ElementName = "RATEOFTAXCALCULATION")]
    public float? RateofTax { get; set; }

    [XmlElement(ElementName = "APPROPRIATEFOR")]
    public AdAllocType? AppropriateFor { get; set; }

    [XmlElement(ElementName = "ISBILLWISEON")]
    public bool? IsBillWise { get; set; }

    [XmlElement(ElementName = "ISCOSTCENTRESON")]
    public bool? IsCostCentresOn { get; set; }

    [XmlElement(ElementName = "ISINTERESTON")]
    public bool? IsInterestOn { get; set; }

    [XmlElement(ElementName = "ISCREDITDAYSCHKON")]
    public bool? IsCreditCheck { get; set; }

    [XmlElement(ElementName = "CREDITLIMIT")]
    public string? CreditLimit { get; set; }

    [XmlElement(ElementName = "EMAIL")]
    public string? EMail { get; set; }

    [XmlElement(ElementName = "EMAILCC")]
    public string? EMailCC { get; set; }

    [XmlElement(ElementName = "WEBSITE")]
    public string? Website { get; set; }

    [XmlElement(ElementName = "INCOMETAXNUMBER")]
    public string? PANNumber { get; set; }

    [XmlElement(ElementName = "GSTTYPEOFSUPPLY")]
    public GSTTypeOfSupply? GSTTypeOfSupply { get; set; }

    [XmlElement(ElementName = "CONTACTDETAILS.LIST")]
    [TDLCollection(CollectionName = "CONTACTDETAILS", ExplodeCondition = "$$NUMITEMS:CONTACTDETAILS>0")]
    public List<ContactDetail>? ContactDetails { get; set; }

    [XmlElement(ElementName = "LEDMULTIADDRESSLIST.LIST")]
    [TDLCollection(CollectionName = "LEDMULTIADDRESSLIST", ExplodeCondition = "$$NUMITEMS:LEDMULTIADDRESSLIST>0")]
    public List<MultiAddress>? Addresses { get; set; }


    [XmlElement(ElementName = "LEDMAILINGDETAILS.LIST")]
    [TDLCollection(CollectionName = "LEDMAILINGDETAILS", ExplodeCondition = "$$NUMITEMS:LEDMAILINGDETAILS>0")]
    public List<MailingDetail>? MailingDetails { get; set; }

    [XmlElement(ElementName = "LEDGSTREGDETAILS.LIST")]
    [TDLCollection(CollectionName = "LEDGSTREGDETAILS", ExplodeCondition = "$$NUMITEMS:LEDGSTREGDETAILS>0")]
    public List<LedgerGSTRegistrationDetail>? GSTRegistrationDetails { get; set; }

    [XmlElement(ElementName = "GSTDETAILS.LIST")]
    [TDLCollection(CollectionName = "GSTDETAILS", ExplodeCondition = "$$NUMITEMS:GSTDETAILS>0")]
    public List<GSTDetail>? GSTDetails { get; set; }

    [XmlElement(ElementName = "HSNDETAILS.LIST")]
    [TDLCollection(CollectionName = "HSNDETAILS", ExplodeCondition = "$$NUMITEMS:HSNDETAILS>0")]
    public List<HSNDetail>? HSNDetails { get; set; }


    [XmlElement(ElementName = "UPDATEDDATETIME")]
    [IgnoreForCreateDTO]
    public DateTime? UpdatedAt { get; set; }
    public override string ToString()
    {
        return $"Ledger - {base.ToString()}";
    }
}

public  enum GSTTypeOfSupply
{ 
    [EnumXMLChoice(Choice ="Goods")]
    Goods,
    [EnumXMLChoice(Choice ="Services")]
    Services,
    [EnumXMLChoice(Choice ="Capital Goods")]
    CapitalGoods
}

public partial class MailingDetail


{
    [XmlArray(ElementName = "ADDRESS.LIST")]
    [XmlArrayItem(ElementName = "ADDRESS")]
    [TDLCollection(CollectionName = "ADDRESS")]
    public List<string> AdressLines { get; set; }

    [XmlElement("APPLICABLEFROM")]
    public DateTime ApplicableFrom { get; set; }

    [XmlElement("MAILINGNAME")]
    public string MailingName { get; set; }

    [XmlElement("COUNTRY")]
    public string? Country { get; set; }

    [XmlElement("STATE")]
    public string? State { get; set; }

    [XmlElement("PINCODE")]
    public string? PINCode { get; set; }
}

public partial class ContactDetail
{
    [XmlElement("COUNTRYISDCODE")]
    public string CountryISOCode { get; set; }

    [XmlElement("ISDEFAULTWHATSAPPNUM")]
    public bool IsDefaultWhatsAppNumber { get; set; }

    [XmlElement("NAME")]
    public string Name { get; set; }

    [XmlElement("PHONENUMBER")]
    public string PhoneNumber { get; set; }
}