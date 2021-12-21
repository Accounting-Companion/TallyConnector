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
    [XmlRoot(ElementName = "STOCKGROUP")]
    public class StockGroup:TallyXmlJson
    {
        public StockGroup()
        {
            BaseUnit = "";
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
        public string Parent { get; set; }


        [XmlElement(ElementName = "ISADDABLE")]
        public string IsAddable { get; set; }  //Should Quantities of Items be Added

        [XmlElement(ElementName = "GSTAPPLICABLE")]
        public string GSTApplicability { get; set; }
        
        [XmlElement(ElementName = "BASEUNITS")]
        public string BaseUnit { get; set; }


        [XmlIgnore]
        public string Alias { get; set; }

        [JsonIgnore]
        [XmlElement(ElementName = "LANGUAGENAME.LIST")]
        public List<LanguageNameList> LanguageNameList { get; set; }
        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
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
    public class StockGrpEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public SGBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class SGBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public SGData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class SGData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public SGMessage Message { get; set; } = new();

        [XmlElement(ElementName = "COLLECTION")]
        public StockGrpColl Collection { get; set; } = new StockGrpColl();


    }

    [XmlRoot(ElementName = "COLLECTION")]
    public class StockGrpColl
    {
        [XmlElement(ElementName = "STOCKGROUP")]
        public List<StockGroup> StockGroups { get; set; }
    }
    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class SGMessage
    {
        [XmlElement(ElementName = "STOCKGROUP")]
        public StockGroup StockGroup { get; set; }
    }
}
