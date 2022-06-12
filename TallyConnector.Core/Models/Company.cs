namespace TallyConnector.Core.Models;


[XmlRoot(ElementName = "COMPANY")]
public class Company : TallyXmlJson
{

    [XmlElement(ElementName = "NAME")]

    public string? Name { get; set; }

    [XmlElement(ElementName = "GUID")]
    [Column(TypeName = "nvarchar(100)")]
    public string? GUID { get; set; }

    [XmlElement(ElementName = "BASICCOMPANYFORMALNAME")]
    public string? FormalName { get; set; }

    [XmlElement(ElementName = "STATENAME")]
    public string? State { get; set; }

    [XmlElement(ElementName = "COUNTRYNAME")]
    public string? Country { get; set; }

    [XmlElement(ElementName = "PINCODE")]
    public string? PinCode { get; set; }

    [XmlElement(ElementName = "PHONENUMBER")]
    public string? PhoneNumber { get; set; }

    [XmlElement(ElementName = "MOBILENO")]
    public string? MobileNumber { get; set; }

    [XmlElement(ElementName = "REMOTEFULLLISTNAME")]
    public string? Address { get; set; }

    [XmlElement(ElementName = "FAXNUMBER")]
    public string? FaxNumber { get; set; }

    [XmlElement(ElementName = "EMAIL")]
    public string? Email { get; set; }


    [XmlElement(ElementName = "WEBSITE")]
    public string? Website { get; set; }

    [XmlElement(ElementName = "TANUMBER")]
    public string? TANNumber { get; set; }

    [XmlElement(ElementName = "TANREGNO")]
    public string? TANRegNumber { get; set; }

    [XmlElement(ElementName = "TDSDEDUCTORTYPE")]
    public string? TDSDeductorType { get; set; }

    [XmlElement(ElementName = "DEDUCTORBRANCH")]
    public string? TDSDeductorBranch { get; set; }


    [XmlElement(ElementName = "BOOKSFROM")]
    public TallyDate? BooksFrom { get; set; }

    [XmlElement(ElementName = "STARTINGFROM")]
    public TallyDate? StartingFrom { get; set; }

    [XmlElement(ElementName = "ENDINGAT")]
    public TallyDate? EndDate { get; set; }

    [XmlElement(ElementName = "COMPANYNUMBER")]
    public string? CompNum { get; set; }


    //Settings

    [XmlElement(ElementName = "ISINVENTORYON")]
    public TallyYesNo? IsInventoryOn { get; set; }

    [XmlElement(ElementName = "ISINTEGRATED")]
    public TallyYesNo? IntegrateAccountswithInventory { get; set; }

    [XmlElement(ElementName = "ISBILLWISEON")]
    public TallyYesNo? IsBillWiseOn { get; set; }

    [XmlElement(ElementName = "ISCOSTCENTRESON")]
    public TallyYesNo? IsCostCentersOn { get; set; }

    [XmlElement(ElementName = "ISTDSON")]
    public TallyYesNo? IsTDSOn { get; set; }


    [XmlElement(ElementName = "ISTCSON")]
    public TallyYesNo? IsTCSOn { get; set; }

    [XmlElement(ElementName = "ISGSTON")]
    public TallyYesNo? IsGSTOn { get; set; }


    [XmlElement(ElementName = "ISPAYROLLON")]
    public TallyYesNo? IsPayrollOn { get; set; }

    [XmlElement(ElementName = "ISINTERESTON")]
    public TallyYesNo? IsInterestOn { get; set; }

    public override string ToString()
    {
        return $"Company - {Name}";
    }


}

[XmlRoot(ElementName = "COMPANYONDISK")]
public class CompanyOnDisk : TallyXmlJson
{
    [XmlElement(ElementName = "NAME")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "STARTINGFROM")]
    public TallyDate? StartDate { get; set; }

    [XmlElement(ElementName = "ENDINGAT")]
    public TallyDate? EndDate { get; set; }

    [XmlElement(ElementName = "COMPANYNUMBER")]
    public string? CompNum { get; set; }

    public override string ToString()
    {
        return $"Company - {Name}";
    }
}

