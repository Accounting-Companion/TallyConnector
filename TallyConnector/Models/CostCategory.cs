using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "COSTCATEGORY")]
    public class CostCategory:TallyXmlJson
    {
        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "REQNAME")]
        public string VName { get; set; }

        [XmlElement(ElementName = "ALLOCATEREVENUE")]
        public string AllocateRevenue { get; set; }

        [XmlElement(ElementName = "ALLOCATENONREVENUE")]
        public string AllocateNonRevenue { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }
    }
    [XmlRoot(ElementName = "ENVELOPE")]
    public class CostCatEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public CCBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class CCBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public CCData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class CCData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public CCMessage Message { get; set; } = new();
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class CCMessage
    {
        [XmlElement(ElementName = "COSTCATEGORY")]
        public CostCategory CostCategory { get; set; }
    }

}
