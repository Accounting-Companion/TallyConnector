using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    
    [XmlRoot(ElementName = "COMPANY")]
    public class Company
    {

        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "STARTINGFROM")]
        public string StartDate { get; set; }

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
