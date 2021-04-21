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
        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "PARENT")]
        public string Parent { get; set; }

        [XmlElement(ElementName = "ADDRESS.LIST")]
        public HAddress FAddress { get; set; }

        [JsonIgnore]
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
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class GdwnMessage
    {
        [XmlElement(ElementName = "GODOWN")]
        public Godown Godown { get; set; }
    }
}
