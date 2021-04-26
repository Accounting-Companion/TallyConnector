using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    public class TallyList
    {

    }

   
    [XmlRoot(ElementName = "LISTOFGROUPS")]
    public class GroupsList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> GroupNames { get; set; } = new();

    }
    [XmlRoot(ElementName = "LISTOFLEDGERS")]
    public class LedgersList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> LedgerNames { get; set; } = new();

    }

    [XmlRoot(ElementName = "LISTOFCOSTCATEGORIES")]
    public class CostCategoriesList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> CostCategories { get; set; } = new();

    }

    
    [XmlRoot(ElementName = "LISTOFCOSTCENTERS")]
    public class CostCentersList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> CostCenters { get; set; } = new();

    }

    [XmlRoot(ElementName = "LISTOFSTOCKGROUPS")]
    public class StockGroupsList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> StockGroups { get; set; } = new();

    }

    [XmlRoot(ElementName = "LISTOFSTOCKCATEGORIES")]
    public class StockCategoriesList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> StockCategories { get; set; } = new();

    }
    [XmlRoot(ElementName = "LISTOFSTOCKITEMS")]
    public class StockItemsList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> StockItems { get; set; } = new();

    }

    [XmlRoot(ElementName = "LISTOFGODOWNS")]
    public class GodownsList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> Godowns { get; set; } = new();

    }

    [XmlRoot(ElementName = "LISTOFVOUCHERTYPES")]
    public class VoucherTypesList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> VoucherTypes { get; set; } = new();

    }

    [XmlRoot(ElementName = "LISTOFUNITS")]
    public class UnitsList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> Units { get; set; } = new();

    }

    [XmlRoot(ElementName = "LISTOFCURRENCIES")]
    public class CurrenciesList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> Currencies { get; set; } = new();

    }

    [XmlRoot(ElementName = "LISTOFATTENDANCETYPES")]
    public class AttendanceTypesList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> AttendanceTypes { get; set; } = new();

    }

    [XmlRoot(ElementName = "LISTOFEMPLOYEEGROUPS")]
    public class EmployeeGroupList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> EmployeeGroups { get; set; } = new();

    }

    [XmlRoot(ElementName = "LISTOFEMPLOYEES")]
    public class EmployeesList
    {
        [XmlElement(ElementName = "NAME")]
        public List<string> Employees { get; set; } = new();

    }

    [XmlRoot(ElementName = "LISTOFVOUCHERS")]
    public class VouchersList
    {

        [XmlElement(ElementName = "VOUCHERNUMBER")]
        public List<string> VoucherIds { get; set; }

        [XmlElement(ElementName = "MASTERID")]
        public List<int> MasterIDs { get; set; }
        
        [XmlElement(ElementName = "DATE")]
        public List<string> VoucherDates { get; set; }

    }

    [XmlRoot(ElementName = "ENVELOPE")]
    public class ComListEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public ComListBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class ComListBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public ComListData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class ComListData
    {
        [XmlElement(ElementName = "COLLECTION")]
        public ComListColl Collection { get; set; } = new();
    }

    [XmlRoot(ElementName = "COLLECTION")]
    public class ComListColl
    {
        [XmlElement(ElementName = "COMPANY")]
        public List<Company> CompaniesList { get; set; }
    }




}
