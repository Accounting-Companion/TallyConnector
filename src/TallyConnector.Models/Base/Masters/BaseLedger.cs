using TallyConnector.Core.Models.TallyComplexObjects;
using TallyConnector.Models.Common;

namespace TallyConnector.Models.Base.Masters;

[TDLCollection(Type = "Ledger")]
[XmlRoot("LEDGER")]
[XmlType(AnonymousType = true)]
public class BaseLedger : BaseAliasedMasterObject
{
    public BaseLedger()
    {

    }

    /// <summary>
    /// Name of parent group
    /// </summary>
    [XmlElement(ElementName = "PARENT")]
    public string? Group { get; set; }

    [XmlElement(ElementName = "OPENINGBALANCE")]
    public TallyAmountField? OpeningBalance { get; set; }

    [XmlElement(ElementName = "CURRENCY")]
    public string? Currency { get; set; }

    [XmlElement(ElementName = "TAXTYPE")]
    public TaxType TaxType { get; set; }

    [XmlElement(ElementName = "GSTTYPE")]
    public GSTTaxType GSTTaxType { get; set; }

    [XmlElement(ElementName = "RATEOFTAXCALCULATION")]
    public float? RateofTax { get; set; }

    [XmlElement(ElementName = "APPROPRIATEFOR")]
    public AdAllocType AppropriateFor { get; set; }

    [XmlElement(ElementName = "ISBILLWISEON")]
    public bool IsBillWise { get; set; }

    [XmlElement(ElementName = "LEDMULTIADDRESSLIST.LIST")]
    [TDLCollection(CollectionName = "LEDMULTIADDRESSLIST", ExplodeCondition = "$$NUMITEMS:LEDMULTIADDRESSLIST>0")]
    public List<MultiAddress> Addresses { get; set; }


    [XmlElement(ElementName = "LEDMAILINGDETAILS.LIST")]
    [TDLCollection(CollectionName = "LEDMAILINGDETAILS", ExplodeCondition = "$$NUMITEMS:LEDMAILINGDETAILS>0")]
    public List<MailingDetail> MailingDetails { get; set; }

    [XmlElement(ElementName = "LEDGSTREGDETAILS.LIST")]
    [TDLCollection(CollectionName = "LEDGSTREGDETAILS", ExplodeCondition = "$$NUMITEMS:LEDGSTREGDETAILS>0")]
    public List<LedgerGSTRegistrationDetail> GSTRegistrationDetails { get; set; }

    [XmlElement(ElementName = "GSTDETAILS.LIST")]
    [TDLCollection(CollectionName = "GSTDETAILS", ExplodeCondition = "$$NUMITEMS:GSTDETAILS>0")]
    public List<GSTDetail> GSTDetail { get; set; }

    [XmlElement(ElementName = "HSNDETAILS.LIST")]
    [TDLCollection(CollectionName = "HSNDETAILS", ExplodeCondition = "$$NUMITEMS:HSNDETAILS>0")]
    public List<HSNDetail> HSNDetails { get; set; }


    public override string ToString()
    {
        return $"Ledger - {base.ToString()}";
    }
}

public class MailingDetail


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