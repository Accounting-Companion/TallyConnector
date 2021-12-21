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
    [XmlRoot(ElementName = "STOCKCATEGORY")]
    public class StockCategory : TallyXmlJson
    {
        public StockCategory()
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
        public string Parent { get; set; }

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
    public class StockCatEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public SCBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class SCBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public SCData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class SCData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public SCMessage Message { get; set; } = new();

        [XmlElement(ElementName = "COLLECTION")]
        public CostCatColl Collection { get; set; } = new CostCatColl();


    }

    [XmlRoot(ElementName = "COLLECTION")]
    public class CostCatColl
    {
        [XmlElement(ElementName = "STOCKCATEGORY")]
        public List<StockCategory> StockCategories { get; set; }
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class SCMessage
    {
        [XmlElement(ElementName = "STOCKCATEGORY")]
        public StockCategory StockCategory { get; set; }
    }
}
