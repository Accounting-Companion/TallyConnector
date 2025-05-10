namespace TallyConnector.Models.Common;


[XmlRoot(ElementName = "LEDMULTIADDRESSLIST")]
public class MultiAddress
{

    public MultiAddress()
    {
        ExciseJurisdictions = [];
        AddressName = string.Empty;
    }

    [XmlElement(ElementName = "MAILINGNAME")]
    public string AddressName { get; set; }

    [XmlArray(ElementName = "ADDRESS.LIST")]
    [XmlArrayItem(ElementName = "ADDRESS")]
    [TDLCollection(CollectionName = "ADDRESS")]
    public List<string> AddressLines {  get; set; }

    [XmlElement(ElementName = "COUNTRYNAME")]
    [Column(TypeName = "nvarchar(60)")]
    public string? Country { get; set; }

    [XmlElement(ElementName = "LEDSTATENAME")]
    [Column(TypeName = "nvarchar(100)")]
    public string? State { get; set; }

    [XmlElement(ElementName = "PINCODE")]
    [Column(TypeName = "nvarchar(15)")]
    public string? PinCode { get; set; }

    [XmlElement(ElementName = "CONTACTPERSON")]
    [Column(TypeName = "nvarchar(20)")]
    public string? ContactPerson { get; set; }

    [XmlElement(ElementName = "MOBILENUMBER")]
    [Column(TypeName = "nvarchar(20)")]
    public string? MobileNo { get; set; }

    [XmlElement(ElementName = "PHONENUMBER")]
    [Column(TypeName = "nvarchar(20)")]
    public string? PhoneNumber { get; set; }

    [XmlElement(ElementName = "FAXNUMBER")]
    [Column(TypeName = "nvarchar(20)")]
    public string? FaxNumber { get; set; }

    [XmlElement(ElementName = "EMAIL")]
    [Column(TypeName = "nvarchar(60)")]
    public string? Email { get; set; }



    [XmlElement(ElementName = "INCOMETAXNUMBER")]
    [Column(TypeName = "nvarchar(12)")]
    public string? PANNumber { get; set; }

    [XmlElement(ElementName = "VATTINNUMBER")]
    [Column(TypeName = "nvarchar(20)")]
    public string? VATNumber { get; set; }

    [XmlElement(ElementName = "INTERSTATESTNUMBER")]
    [Column(TypeName = "nvarchar(20)")]
    public string? CSTNumber { get; set; }

    [XmlElement(ElementName = "EXCISENATUREOFPURCHASE")]
    [Column(TypeName = "nvarchar(10)")]
    public ExciseNatureOfPurchase ExciseNatureOfPurchase { get; set; }

    [XmlElement(ElementName = "EXCISEREGNO")]
    [Column(TypeName = "nvarchar(20)")]
    public string? ExciseRegistrationNo { get; set; }

    [XmlElement(ElementName = "EXCISEIMPORTSREGISTARTIONNO")]
    [Column(TypeName = "nvarchar(20)")]
    public string? ExciseImportRegistrationNo { get; set; }

    [XmlElement(ElementName = "IMPORTEREXPORTERCODE")]
    [Column(TypeName = "nvarchar(20)")]
    public string? ImportExportCode { get; set; }

    [XmlElement(ElementName = "GSTREGISTRATIONTYPE")]
    public GSTRegistrationType GSTDealerType { get; set; }

    [XmlElement(ElementName = "ISOTHTERRITORYASSESSEE")]
    public bool? IsOtherTerritoryAssessee { get; set; }

    [XmlElement(ElementName = "PARTYGSTIN")]
    [Column(TypeName = "nvarchar(17)")]
    public string? GSTIN { get; set; }

    [XmlElement(ElementName = "EXCISEJURISDICTIONDETAILS.LIST")]
    [TDLCollection(CollectionName = "EXCISEJURISDICTIONDETAILS", ExplodeCondition = "$$NUMITEMS:EXCISEJURISDICTIONDETAILS>0")]
    public List<ExciseJurisdiction>? ExciseJurisdictions { get; set; }
    

}
[XmlRoot(ElementName = "EXCISEJURISDICTIONDETAILS.LIST")]
public class ExciseJurisdiction
{
    [XmlElement(ElementName = "APPLICABLEFROM")]
    public DateTime? ApplicableFrom { get; set; }

    [XmlElement(ElementName = "RANGE")]
    [Column(TypeName = "nvarchar(20)")]
    public string? Range { get; set; }

    [XmlElement(ElementName = "DIVISION")]
    [Column(TypeName = "nvarchar(20)")]
    public string? Division { get; set; }

    [XmlElement(ElementName = "COMMISSIONERATE")]
    [Column(TypeName = "nvarchar(20)")]
    public string? Commissionerate { get; set; }

  
}

