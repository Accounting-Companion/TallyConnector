using System;
using System.Collections.Generic;
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
        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "PARENT")]
        public string Parent { get; set; }

        [XmlElement(ElementName = "CATEGORY")]
        public string Category { get; set; }

        [XmlElement(ElementName = "BASEUNITS")]
        public string BaseUnit { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }
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
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class SIMessage
    {
        [XmlElement(ElementName = "STOCKITEM")]
        public StockItem StockItem { get; set; }
    }
}
