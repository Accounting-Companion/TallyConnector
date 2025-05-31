﻿using TallyConnector.Core;

namespace TallyConnector.Models.Base;
/// <summary>
/// Base Model for Company
/// </summary>
[XmlRoot(ElementName = "COMPANY")]
[TDLFunctionsMethodName(nameof(TC_BaseCompanyFunctions))]
[TDLCollection(Type = "Ledger")]
public class BaseCompany : IBaseCompany
{
    private const string CleanCompanyNumberFunctionName = "TC_GetCleanedCompanyNumber";

    /// <summary>
    /// Name of Company
    /// </summary>
    [XmlElement(ElementName = "NAME")]
    [Column(TypeName = $"nvarchar({MaxNameLength})")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "TC_STARTINGFROM")]
    [TDLField(Set = "$STARTINGFROM")]
    public DateTime? StartingFrom { get; set; }

    [XmlElement(ElementName = "TC_ENDINGAT")]
    [TDLField(Set = "$ENDINGAT")]
    public DateTime? EndDate { get; set; }

    [XmlElement(ElementName = "CLEANEDCOMPANYNUMBER")]
    [TDLField(Set = $"$${CleanCompanyNumberFunctionName}")]
    public string CompNum { get; set; } = null!;

    [XmlElement(ElementName = "ISAGGREGATE")]
    public bool IsGroupCompany { get; set; }
    public static TDLFunction[] TC_BaseCompanyFunctions()
    {
        return [new TDLFunction(CleanCompanyNumberFunctionName) {
            Returns="String",
            Variables=["TC_CompNum:String:@@SetCmpNumStr"],
            Actions=[
                "01:if:##TC_CompNum Contains \"(\"",
                "02: SET :TC_CompNum: $$StringFindandReplace:##TC_CompNum:\"(\":\"\"",
                "03: if:##TC_CompNum Contains \")\"",
                "04: SET :TC_CompNum:$$StringFindandReplace:##TC_CompNum:\")\":\"\"",
                "05: ENDIF",
                "06: ENDIF",
                "07: Return:##TC_CompNum"
                ]
        }];
    }

    public override string ToString()
    {
        return $"Company - {Name}";
    }
}


[XmlRoot(ElementName = "COMPANY")]
[XmlType(AnonymousType = true)]
public partial class Company : BaseCompany, ICompany
{

    [XmlElement(ElementName = "BOOKSFROM")]
    public DateTime BooksFrom { get; set; }

    [XmlElement(ElementName = "GUID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string GUID { get; set; }

    [XmlElement(ElementName = "BASICCOMPANYFORMALNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? FormalName { get; set; }

    [XmlElement(ElementName = "STATENAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? State { get; set; }

    [XmlElement(ElementName = "COUNTRYNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
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


    //Settings

    [XmlElement(ElementName = "ISINVENTORYON")]
    public bool IsInventoryOn { get; set; }

    [XmlElement(ElementName = "ISINTEGRATED")]
    public bool IntegrateAccountswithInventory { get; set; }

    [XmlElement(ElementName = "ISBILLWISEON")]
    public bool IsBillWiseOn { get; set; }

    [XmlElement(ElementName = "ISCOSTCENTRESON")]
    public bool IsCostCentersOn { get; set; }

    [XmlElement(ElementName = "ISTDSON")]
    public bool IsTDSOn { get; set; }


    [XmlElement(ElementName = "ISTCSON")]
    public bool IsTCSOn { get; set; }

    [XmlElement(ElementName = "ISGSTON")]
    public bool IsGSTOn { get; set; }


    [XmlElement(ElementName = "ISPAYROLLON")]
    public bool IsPayrollOn { get; set; }

    [XmlElement(ElementName = "ISINTERESTON")]
    public bool IsInterestOn { get; set; }

}


[XmlRoot(ElementName = "COMPANYONDISK")]
public class CompanyOnDisk : BaseCompany, IBaseObject
{


    public override string ToString()
    {
        return $"Company - {Name}";
    }
}

