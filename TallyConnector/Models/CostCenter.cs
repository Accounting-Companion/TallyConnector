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
    [XmlRoot(ElementName = "COSTCENTRE")]
    public class CostCenter:TallyXmlJson
    {
        public CostCenter()
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

        [XmlElement(ElementName = "CATEGORY")]
        public string Category { get; set; }

        [XmlElement(ElementName = "PARENT")]
        public string Parent { get; set; }

        [XmlElement(ElementName = "EMAILID")]
        public string Emailid { get; set; }

        [XmlElement(ElementName = "REVENUELEDFOROPBAL")]
        public string ShowOpeningBal { get; set; }


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
    public class CostCentEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public CCentBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class CCentBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public CCentData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class CCentData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public CCentMessage Message { get; set; } = new();

        [XmlElement(ElementName = "COLLECTION")]
        public CostCentColl Collection { get; set; } = new CostCentColl();


    }

    [XmlRoot(ElementName = "COLLECTION")]
    public class CostCentColl
    {
        [XmlElement(ElementName = "COSTCENTRE")]
        public List<CostCenter> CostCenters { get; set; }
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class CCentMessage
    {
        [XmlElement(ElementName = "COSTCENTRE")]
        public CostCenter CostCenter { get; set; }
    }
}
