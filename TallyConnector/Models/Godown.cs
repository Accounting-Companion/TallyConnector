using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "GODOWN")]
    public class Godown:TallyXmlJson
    {

        public Godown()
        {
            FAddress = new();
        }

        [XmlElement(ElementName = "MASTERID")]
        public int? TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
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
        [JsonIgnore]
        [XmlElement(ElementName = "ADDRESS.LIST")]
        public HAddress FAddress { get; set; }

        
        [XmlIgnore]
        public string Address
        {
            get
            {
                return FAddress.FullAddress;
            }

            set
            {
                this.FAddress = new();
                this.FAddress.FullAddress = value;

            }

        }

        [XmlElement(ElementName = "PINCODE")]
        public string PinCode { get; set; }

        [XmlElement(ElementName = "PHONENUMBER")]
        public string PhoneNumber { get; set; }

        [XmlElement(ElementName = "ISEXTERNAL")]
        public string IsExternal { get; set; } // ThirdParty Stock with Us

        [XmlElement(ElementName = "ISINTERNAL")]
        public string IsInternal { get; set; } // Our Stock With Third Party

        [XmlElement(ElementName = "CANDELETE")]
        public string CanDelete { get; set; }


        [XmlIgnore]
        [Column(TypeName = "nvarchar(60)")]
        public string Alias { get; set; }


        [JsonIgnore]
        [XmlElement(ElementName = "LANGUAGENAME.LIST")]
        public List<LanguageNameList> LanguageNameList { get; set; }
        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public YesNo Action { get; set; }

        [XmlElement(ElementName = "GUID")]
        [Column(TypeName = "nvarchar(100)")]
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
    public class GodownEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public GdwnBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class GdwnBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public GdwnData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class GdwnData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public GdwnMessage Message { get; set; } = new();

        [XmlElement(ElementName = "COLLECTION")]
        public GodownColl Collection { get; set; } = new GodownColl();


    }

    [XmlRoot(ElementName = "COLLECTION")]
    public class GodownColl
    {
        [XmlElement(ElementName = "GODOWN")]
        public List<Godown> Godowns { get; set; }
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class GdwnMessage
    {
        [XmlElement(ElementName = "GODOWN")]
        public Godown Godown { get; set; }
    }
}
