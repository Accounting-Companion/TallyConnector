namespace TallyConnector.Core.Models;

[Serializable]
[XmlRoot(ElementName = "ADDRESS.LIST")]
public class HAddress
{
    private List<string> _Address = new();


    [XmlElement(ElementName = "ADDRESS")]
    public List<string> Address
    {
        get { return _Address; }
        set { _Address = value; }
    }
    [JsonIgnore]
    [XmlIgnore]
    public string? FullAddress
    {
        get { return _Address.Count > 0 ? string.Join(" ..\n", _Address) : null; }
        set
        {
            string[] stringSeparators = new string[] { " ..\n" };
            _Address = value != null ? value.Split(stringSeparators, StringSplitOptions.None).ToList() : new();
        }
    }

}
[XmlRoot(ElementName = "LEDMULTIADDRESSLIST.LIST")]
public class MultiAddress : ICheckNull
{

    public MultiAddress()
    {
        FAddress = new();
        ExciseJurisdictions = new();
    }

    [JsonIgnore]
    [XmlElement(ElementName = "ADDRESS.LIST")]
    public HAddress FAddress;

    [XmlIgnore]
    public string? Address
    {
        get
        {
            return FAddress?.FullAddress;
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
    [Column(TypeName = "nvarchar(15)")]
    public GSTRegistrationType GSTDealerType { get; set; }

    [XmlElement(ElementName = "ISOTHTERRITORYASSESSEE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsOtherTerritoryAssessee { get; set; }

    [XmlElement(ElementName = "PARTYGSTIN")]
    [Column(TypeName = "nvarchar(17)")]
    public string? GSTIN { get; set; }

    [XmlElement(ElementName = "EXCISEJURISDICTIONDETAILS.LIST")]
    public List<ExciseJurisdiction>? ExciseJurisdictions { get; set; }


    public bool IsNull()
    {
        ExciseJurisdictions = ExciseJurisdictions?.Where(ExcJur => !ExcJur.IsNull()).ToList();
        if (ExciseJurisdictions?.Count == 0)
        {
            ExciseJurisdictions = null;
        }
        if (true)
        {

        }
        return false;
    }

}
[XmlRoot(ElementName = "EXCISEJURISDICTIONDETAILS.LIST")]
public class ExciseJurisdiction : ICheckNull
{
    [XmlElement(ElementName = "APPLICABLEFROM")]
    public TallyDate? ApplicableFrom { get; set; }

    [XmlElement(ElementName = "RANGE")]
    [Column(TypeName = "nvarchar(20)")]
    public string? Range { get; set; }

    [XmlElement(ElementName = "DIVISION")]
    [Column(TypeName = "nvarchar(20)")]
    public string? Division { get; set; }

    [XmlElement(ElementName = "COMMISSIONERATE")]
    [Column(TypeName = "nvarchar(20)")]
    public string? Commissionerate { get; set; }

    public bool IsNull()
    {
        return false;
    }
}

