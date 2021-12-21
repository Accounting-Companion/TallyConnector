using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "STOCKITEM")]
    public class StockItem:TallyXmlJson
    {
        public StockItem()
        {
            LanguageNameList = new();
        }

        [XmlElement(ElementName = "MASTERID")]
        public int? TallyId { get; set; }


        [XmlAttribute(AttributeName = "NAME")]
        [JsonIgnore]
        public string OldName { get; set; }

        private string name;

        [XmlElement(ElementName = "NAME")]
        [Required]
        public string Name
        {
            get { return (name == null || name == string.Empty) ? OldName : name; }
            set => name = value;
        }

        [XmlElement(ElementName = "PARENT")]
        public string StockGroup { get; set; }

        [XmlElement(ElementName = "CATEGORY")]
        public string Category { get; set; }

        [XmlElement(ElementName = "GSTAPPLICABLE")]
        public string GSTApplicable { get; set; }

        [XmlElement(ElementName = "GSTTYPEOFSUPPLY")]
        public string GSTTypeOfSupply { get; set; }

        [XmlElement(ElementName = "TCSAPPLICABLE")]
        public string TCSApplicable { get; set; }

        [XmlElement(ElementName = "DESCRIPTION")]
        public string Description { get; set; }

        [XmlElement(ElementName = "NARRATION")]
        public string Narration { get; set; }

        [XmlElement(ElementName = "COSTINGMETHOD")]
        public string CostingMethod { get; set; }

        [XmlElement(ElementName = "ISCOSTCENTRESON")]
        public string IsCostTracking { get; set; }

        [XmlElement(ElementName = "ISBATCHWISEON")]
        public string MaintainInBranches { get; set; }

        [XmlElement(ElementName = "ISPERISHABLEON")]
        public string UseExpiryDates { get; set; }

        [XmlElement(ElementName = "HASMFGDATE")]
        public string TrackDateOfManufacturing { get; set; }

        [XmlElement(ElementName = "BASEUNITS")]
        public string BaseUnit { get; set; }

        [XmlElement(ElementName = "ADDITIONALUNITS")]
        public string AdditionalUnits { get; set; }

        [XmlElement(ElementName = "INCLUSIVETAX")]
        public string InclusiveOfTax { get; set; }

        
        [XmlElement(ElementName = "CONVERSION")]
        public string Conversion { get; set; }
        
        [XmlElement(ElementName = "BASICRATEOFEXCISE")]
        public string RateOfDuty { get; set; }

        [XmlElement(ElementName = "OPENINGBALANCE")]
        public string OpeningBal { get; set; }

        [XmlElement(ElementName = "OPENINGVALUE")]
        public string OpeningValue { get; set; }

        [XmlElement(ElementName = "OPENINGRATE")]
        public string OpeningRate { get; set; }



        [XmlIgnore]
        public string Alias { get; set; }

        [JsonIgnore]
        [XmlElement(ElementName = "LANGUAGENAME.LIST")]
        public List<LanguageNameList> LanguageNameList { get; set; }
        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }

        [XmlElement(ElementName = "GUID")]
        public string GUID { get; set; }

        public void CreateNamesList()
        {
            if (this.LanguageNameList.Count == 0)
            {
                this.LanguageNameList.Add(new LanguageNameList());
                this.LanguageNameList[0].NameList.NAMES.Add(this.Name);

            }
            if (this.Alias != null && this.Alias != string.Empty)
            {
                this.LanguageNameList[0].LanguageAlias = this.Alias;
            }
        }
    }
    [XmlRoot(ElementName = "ENVELOPE")]
    public class StockItemEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public SIBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class SIBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public SIData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class SIData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public SIMessage Message { get; set; } = new();

        [XmlElement(ElementName = "COLLECTION")]
        public StockItemColl Collection { get; set; } = new StockItemColl();


    }

    [XmlRoot(ElementName = "COLLECTION")]
    public class StockItemColl
    {
        [XmlElement(ElementName = "STOCKITEM")]
        public List<StockItem> StockItems { get; set; }
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class SIMessage
    {
        [XmlElement(ElementName = "STOCKITEM")]
        public StockItem StockItem { get; set; }
    }
}
