using System;
using System.Collections.Generic;
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
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlIgnore]
        public string VName { get; set; }

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
        public string Alias
        {
            get
            {
                if (this.LanguageNameList.NameList.NAMES.Count > 0)
                {
                    if (VName == null)
                    {
                        VName = this.LanguageNameList.NameList.NAMES[0];
                    }
                    if (Name == VName)
                    {
                        this.LanguageNameList.NameList.NAMES[0] = this.Name;
                        return string.Join("..\n", this.LanguageNameList.NameList.NAMES.GetRange(1, this.LanguageNameList.NameList.NAMES.Count - 1));

                    }
                    else
                    {
                        //Name = this.LanguageNameList.NameList.NAMES[0];
                        return string.Join("..\n", this.LanguageNameList.NameList.NAMES);

                    }
                }
                else
                {
                    this.LanguageNameList.NameList.NAMES.Add(this.Name);
                    return null;
                }


            }
            set
            {
                this.LanguageNameList = new();
               
                if (value != null)
                {
                    List<string> lis = value.Split('\n').ToList();

                    LanguageNameList.NameList.NAMES.Add(Name);
                    if (value != "")
                    {
                        LanguageNameList.NameList.NAMES.AddRange(lis);
                    }

                }
                else
                {
                    LanguageNameList.NameList.NAMES.Add(Name);
                }


            }
        }

        [JsonIgnore]
        [XmlElement(ElementName = "LANGUAGENAME.LIST")]
        public LanguageNameList LanguageNameList { get; set; }
        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }

        [XmlElement(ElementName = "GUID")]
        public string GUID { get; set; }
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
