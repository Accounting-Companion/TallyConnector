using TallyConnector.Models.Base.Masters;
using TallyConnector.Models.Common;

namespace TallyConnector.Models.Base;
[XmlRoot(ElementName = "GSTREGISTRATION")]
[XmlType(AnonymousType = true)]
[TDLCollection(Type = "TAXUNIT")]
[TDLDefaultFiltersMethodName(nameof(GetDefaultFilters))]

public partial class GSTRegistration : BaseAliasedMasterObject
{

    [XmlElement(ElementName = "STATENAME")]
    public string StateName { get; set; }

    [XmlElement(ElementName = "PRIORSTATENAME")]
    public string? PriorStateName { get; set; }

    [XmlElement(ElementName = "GSTREGNUMBER")]
    public string? GSTIN { get; set; }

    [XmlElement(ElementName = "EWAYBILLAPPLICABLETYPE")]
    public string? EWayApplicableType { get; set; }

    [XmlElement(ElementName = "GSTNUSERNAME")]
    public string? GSTUserName { get; set; }

    [XmlElement(ElementName = "ESIGNMETHOD")]
    public string? ESignMethod { get; set; }

    [XmlElement(ElementName = "ISOTHTERRITORYASSESSEE")]
    public bool? IsOtherTerritoryAssessee { get; set; }

    [XmlElement(ElementName = "ISEWAYBILLPRINTAPPLICABLE")]
    public bool? IsEwayBillApplicable { get; set; }

    [XmlElement(ElementName = "ISEWAYBILLAPPLICABLEFORINTRA")]
    public bool? IsEwayBillApplicableForIntra { get; set; }


    [XmlElement(ElementName = "GSTREGISTRATIONDETAILS.LIST")]
    [TDLCollection(CollectionName = "GSTREGISTRATIONDETAILS", ExplodeCondition = "$$NUMITEMS:GSTREGISTRATIONDETAILS>0")]
    public List<GSTRegistrationDetail> RegistrationDetails { get; set; } = [];


    public static IEnumerable<string> GetDefaultFilters() => ["TaxUnitForGST"];
}

public partial class GSTRegistrationDetail
{
    [XmlElement("FROMDATE")]
    public DateTime ApplicableFrom { get; set; }

    [XmlElement("REGISTRATIONTYPE")]
    public GSTRegistrationType GSTRegistrationType { get; set; }

    [XmlElement("STATE")]
    public string? State { get; set; }

    [XmlElement("PLACEOFSUPPLY")]
    public string? PlaceOfSupply { get; set; }

    [XmlElement("ISOTHTERRITORYASSESSEE")]
    public bool? IsOtherTerritoryAssesse { get; set; }

    [XmlElement("ISSTATECESSON")]
    public bool? IsStateCessOn { get; set; }
}