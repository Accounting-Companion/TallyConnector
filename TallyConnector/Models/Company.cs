using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    
    [XmlRoot(ElementName = "COMPANY")]
    public class Company:TallyXmlJson
    {

        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "BASICCOMPANYFORMALNAME")]
        public string MailingName { get; set; }

        [XmlElement(ElementName = "STATENAME")]
        public string State { get; set; }

        [XmlElement(ElementName = "COUNTRYNAME")]
        public string Country { get; set; }

        [XmlElement(ElementName = "PINCODE")]
        public string PinCode { get; set; }

        [XmlElement(ElementName = "PHONENUMBER")]
        public string PhoneNumber { get; set; }

        [XmlElement(ElementName = "FAXNUMBER")]
        public string FaxNumber { get; set; }

        [XmlElement(ElementName = "EMAIL")]
        public string Email { get; set; }


        [XmlElement(ElementName = "WEBSITE")]
        public string Website { get; set; }

        [XmlElement(ElementName = "TANUMBER")]
        public string TANNumber { get; set; }

        [XmlElement(ElementName = "TANREGNO")]
        public string TANRegNumber { get; set; }

        [XmlElement(ElementName = "TDSDEDUCTORTYPE")]
        public string TDSDeductorType { get; set; }

        [XmlElement(ElementName = "DEDUCTORBRANCH")]
        public string TDSDeductorBranch { get; set; }


        [XmlElement(ElementName = "BOOKSFROM")]
        public string BooksFrom { get; set; }

        [XmlElement(ElementName = "STARTINGFROM")]
        public string StartingFrom { get; set; }

        [XmlElement(ElementName = "ENDINGAT")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "COMPANYNUMBER")]
        public string CompNum { get; set; }


        [XmlElement(ElementName = "GUID")]
        public string GUID { get; set; }

        //Settings
        
        [XmlElement(ElementName = "ISINVENTORYON")]
        public string IsInventoryOn { get; set; }

        [XmlElement(ElementName = "ISINTEGRATED")]
        public string IntegrateAccountswithInventory { get; set; }

        [XmlElement(ElementName = "ISBILLWISEON")]
        public string IsBillWiseOn { get; set; }

        [XmlElement(ElementName = "ISCOSTCENTRESON")]
        public string IsCostCentersOn { get; set; }

        [XmlElement(ElementName = "ISTDSON")]
        public string IsTDSon { get; set; }


        [XmlElement(ElementName = "ISTCSON")]
        public string IsTCSon { get; set; }

        [XmlElement(ElementName = "ISGSTON")]
        public string IsGSTon { get; set; }


        [XmlElement(ElementName = "ISPAYROLLON")]
        public string IsPayrollOn { get; set; }

        [XmlElement(ElementName = "ISINTERESTON")]
        public string Isintereston { get; set; }


    }

    [XmlRoot(ElementName = "COMPANYONDISK")]
    public class CompanyOnDisk 
    {
        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "STARTINGFROM")]
        public string StartDate { get; set; }

        [XmlElement(ElementName = "ENDINGAT")]
        public string EndDate { get; set; }

        [XmlElement(ElementName = "COMPANYNUMBER")]
        public string CompNum { get; set; }
    }
}
