namespace TallyConnector.Models;

[XmlRoot(ElementName = "COSTCENTRE")]
public class Employee : CostCenter
{
    public Employee()
    {
        FAddress = new HAddress();
    }

    [XmlElement(ElementName = "FORPAYROLL")]
    public string ForPayroll { get; set; }

    [XmlElement(ElementName = "ISEMPLOYEEGROUP")]
    public string IsEmployeeGroup { get; set; }

    [XmlElement(ElementName = "DATEOFJOIN")]
    public string JoiningDate { get; set; }

    [XmlElement(ElementName = "DEACTIVATIONDATE")]
    public string RelievingDate { get; set; }

    [XmlElement(ElementName = "REASONSFORLEAVING")]
    public string RelievingReason { get; set; }



    [XmlElement(ElementName = "DESIGNATION")]
    public string Designation { get; set; }

    [XmlElement(ElementName = "FUNCTION")]
    public string Function { get; set; }

    [XmlElement(ElementName = "LOCATION")]
    public string Location { get; set; }

    [XmlElement(ElementName = "GENDER")]
    public string Gender { get; set; }

    [XmlElement(ElementName = "DATEOFBIRTH")]
    public string DateOfBirth { get; set; }

    [XmlElement(ElementName = "BLOODGROUP")]
    public string BloodGroup { get; set; }

    [XmlElement(ElementName = "FATHERNAME")]
    public string FatherName { get; set; }

    [XmlElement(ElementName = "SPOUSENAME")]
    public string SpouseName { get; set; }

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

    [XmlElement(ElementName = "CONTACTNUMBERS")]
    public string PhoneNumber { get; set; }


    [XmlElement(ElementName = "PANNUMBER")]
    public string PANNumber { get; set; }

    [XmlElement(ElementName = "AADHARNUMBER")]
    public string AadhaarNumber { get; set; }

    [XmlElement(ElementName = "UANNUMBER")]
    public string UAN { get; set; }

    [XmlElement(ElementName = "PFACCOUNTNUMBER")]
    public string PFAccountNumber { get; set; }

    [XmlElement(ElementName = "FPFACCOUNTNUMBER")]
    public string EPSAccountNumber { get; set; }

    [XmlElement(ElementName = "PFJOININGDATE")]
    public string JoiningDate_PF { get; set; }

    [XmlElement(ElementName = "PFRELIEVINGDATE")]
    public string RelievingDate_PF { get; set; }

    [XmlElement(ElementName = "PRACCOUNTNUMBER")]
    public string PRAccountNumber { get; set; }

    [XmlElement(ElementName = "ESINUMBER")]
    public string ESINumber { get; set; }

    [XmlElement(ElementName = "ESIDISPENSARYNAME")]
    public string ESIDispensaryName { get; set; }

    [XmlElement(ElementName = "PASSPORTDETAILS")]
    public string PassportNumber { get; set; }

    [XmlElement(ElementName = "COUNTRYOFISSUE")]
    public string CountryofIssue { get; set; }

    [XmlElement(ElementName = "PASSPORTEXPIRYDATE")]
    public string PassportExpiryDate { get; set; }

    [XmlElement(ElementName = "VISANUMBER")]
    public string VisaNumber { get; set; }

    [XmlElement(ElementName = "VISAEXPIRYDATE")]
    public string VisaExpiryDate { get; set; }

    [XmlElement(ElementName = "WORKPERMITNUMBER")]
    public string WorkPermitNumber { get; set; }

    [XmlElement(ElementName = "CONTRACTSTARTDATE")]
    public string Contract_StartDate { get; set; }

    [XmlElement(ElementName = "CONTRACTEXPIRYDATE")]
    public string Contract_ExpiryDate { get; set; }

    [XmlElement(ElementName = "NARRATION")]
    public string Narration { get; set; }

    [XmlElement(ElementName = "BANKDETAILS")]
    public string BankName { get; set; }

    [XmlElement(ElementName = "BANKACCOUNTNUMBER")]
    public string BankAccountNo { get; set; }

    [XmlElement(ElementName = "BANKBRANCH")]
    public string BankBranch { get; set; }

    [XmlElement(ElementName = "IFSCODE")]
    public string IFSC { get; set; }



    [JsonIgnore]
    [XmlElement(ElementName = "ADDRESS.LIST")]
    public HAddress FAddress { get; set; }

    [XmlElement(ElementName = "TAXREGIMEDETAILS.LIST")]
    public List<TaxRegimeDetails> TaxRegimeDetails { get; set; }

    [XmlElement(ElementName = "PAYMENTDETAILS.LIST")]
    public PaymentDetails PaymentDetails { get; set; }


}
[XmlRoot(ElementName = "TAXREGIMEDETAILS.LIST")]
public class TaxRegimeDetails
{
    [XmlElement(ElementName = "APPLICABLEFROM")]
    public string ApplicableFrom { get; set; }

    [XmlElement(ElementName = "TAXREGIME")]
    public string TaxRegime { get; set; }

}


[XmlRoot(ElementName = "ENVELOPE")]
public class EmployeeEnvelope : TallyXmlJson
{

    [XmlElement(ElementName = "HEADER")]
    public Header Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public EmployeeBody Body { get; set; } = new();
}

[XmlRoot(ElementName = "BODY")]
public class EmployeeBody
{
    [XmlElement(ElementName = "DESC")]
    public Description Desc { get; set; } = new();

    [XmlElement(ElementName = "DATA")]
    public EmployeeData Data { get; set; } = new();
}

[XmlRoot(ElementName = "DATA")]
public class EmployeeData
{
    [XmlElement(ElementName = "TALLYMESSAGE")]
    public EmployeeMessage Message { get; set; } = new();


    [XmlElement(ElementName = "COLLECTION")]
    public EmployeeColl Collection { get; set; } = new EmployeeColl();


}

[XmlRoot(ElementName = "COLLECTION")]
public class EmployeeColl
{
    [XmlElement(ElementName = "COSTCENTRE")]
    public List<Employee> Employees { get; set; }
}

[XmlRoot(ElementName = "TALLYMESSAGE")]
public class EmployeeMessage
{
    [XmlElement(ElementName = "COSTCENTRE")]
    public Employee Employee { get; set; }
}

