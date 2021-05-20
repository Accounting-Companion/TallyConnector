using System;
using System.Collections.Generic;
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
        
        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "CATEGORY")]
        public string Category { get; set; }

        [XmlElement(ElementName = "PARENT")]
        public string Parent { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }
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
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class CCentMessage
    {
        [XmlElement(ElementName = "COSTCENTRE")]
        public CostCenter CostCenter { get; set; }
    }
}
